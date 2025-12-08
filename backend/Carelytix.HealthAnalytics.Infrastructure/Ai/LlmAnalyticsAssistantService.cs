using System.Net.Http.Json;
using System.Text;
using System.Net.Http;
using System.Text.Json;
using HealthAnalytics.Application.Ai;
using HealthAnalytics.Application.Ai.Models;
using HealthAnalytics.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace HealthAnalytics.Infrastructure.Ai;

public class LlmAnalyticsAssistantService : IAnalyticsAssistantService
{
    private readonly HealthAnalyticsDbContext _db;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    public LlmAnalyticsAssistantService(
        HealthAnalyticsDbContext db,
        IHttpClientFactory httpClientFactory,
        IConfiguration config)
    {
        _db = db;
        _httpClient = httpClientFactory.CreateClient("ai");
        _config = config;
    }

    public async Task<AnalyzeResponse> AnalyzeAsync(AnalyzeRequest request, CancellationToken cancellationToken = default)
    {
        var sb = new StringBuilder();

        if (!string.IsNullOrWhiteSpace(request.PatientMrn))
        {
            var patient = await _db.Patients
                .AsNoTracking()
                .Include(p => p.Encounters)
                .FirstOrDefaultAsync(p => p.Mrn == request.PatientMrn, cancellationToken);

            if (patient is not null)
            {
                sb.AppendLine($"Patient MRN: {patient.Mrn}");
                sb.AppendLine($"Name: {patient.FirstName} {patient.LastName}");
                sb.AppendLine($"DOB: {patient.DateOfBirth:yyyy-MM-dd}, Gender: {patient.Gender}");
                sb.AppendLine($"IsActive: {patient.IsActive}");
                sb.AppendLine("Encounters:");
                foreach (var enc in patient.Encounters.OrderByDescending(e => e.EncounterDate).Take(10))
                {
                    sb.AppendLine(
                        $" - {enc.EncounterDate:yyyy-MM-dd}: {enc.EncounterType}, Dx={enc.DiagnosisCode}, RiskScore={enc.RiskScore}");
                }
            }
        }

        if (request.IncludePopulationStats)
        {
            var totalPatients = await _db.Patients.LongCountAsync(cancellationToken);
            var activePatients = await _db.Patients.LongCountAsync(p => p.IsActive, cancellationToken);
            var avgRiskScore = await _db.Encounters.AnyAsync(cancellationToken)
                ? await _db.Encounters.AverageAsync(e => e.RiskScore, cancellationToken)
                : 0m;

            sb.AppendLine();
            sb.AppendLine("Population Summary:");
            sb.AppendLine($"TotalPatients: {totalPatients}");
            sb.AppendLine($"ActivePatients: {activePatients}");
            sb.AppendLine($"AverageEncounterRiskScore: {avgRiskScore:F2}");
        }

        var systemPrompt = "You are a senior healthcare analytics assistant working for a value-based care organization. " +
                           "You receive EHR-style data and high-level metrics and must provide clear, concise, and safe insights. " +
                           "Do not provide medical advice or diagnosis. Focus on patterns, utilization, risk, and data quality. " +
                           "When unsure, state your assumptions.";

        var userPrompt = $"Question: {request.Question}\n\nData Context:\n{sb}";

        var model = _config["Ai:Model"] ?? "gpt-4.1-mini";

        var payload = new
        {
            model,
            messages = new[]
            {
            new { role = "system", content = systemPrompt },
            new { role = "user", content = userPrompt }
        }
        };

        // response is a plain string now
        var response = await PostToLlmAsync(payload, cancellationToken);

        // Assuming AnalyzeResponse(string answer, string model)
        return new AnalyzeResponse(response, model);
    }


    public async Task<ChatResponse> ChatAsync(ChatRequest request, CancellationToken cancellationToken = default)
    {
        var systemPrompt = "You are a helpful analytics co-pilot for a healthcare data platform. " +
                           "Answer as a senior C# / .NET / data engineering lead who understands healthcare, risk scoring, and SQL. " +
                           "Never expose secrets or credentials. Never claim to provide clinical diagnosis.";

        var model = _config["Ai:Model"] ?? "gpt-4.1-mini";

        var messages = new List<object>
    {
        new { role = "system", content = systemPrompt }
    };

        messages.AddRange(request.Messages.Select(m => new { role = m.Role.ToLowerInvariant(), content = m.Content }));

        var payload = new
        {
            model,
            messages = messages.ToArray()
        };

        var answer = await PostToLlmAsync(payload, cancellationToken);

        var mergedMessages = request.Messages.ToList();
        mergedMessages.Add(new ChatMessageDto("assistant", answer));

        return new ChatResponse(mergedMessages, model);
    }

    private async Task<string> PostToLlmAsync(object payload, CancellationToken cancellationToken)
    {
        var endpoint = _config["Ai:Endpoint"] ?? "https://YOUR-AI-ENDPOINT";
        var path = _config["Ai:Path"] ?? "/v1/chat/completions";
        var apiKey = _config["Ai:ApiKey"] ?? "SET_ME_IN_CONFIG";

        var url = endpoint.TrimEnd('/') + path;
        using var req = new HttpRequestMessage(HttpMethod.Post, url);
        req.Headers.Add("Authorization", $"Bearer {apiKey}");
        req.Content = JsonContent.Create(payload);

        using var resp = await _httpClient.SendAsync(req, cancellationToken);
        resp.EnsureSuccessStatusCode();

        using var stream = await resp.Content.ReadAsStreamAsync(cancellationToken);
        using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

        var root = doc.RootElement;

        // OpenAI format
        if (root.TryGetProperty("choices", out var choices) &&
            choices.ValueKind == JsonValueKind.Array &&
            choices.GetArrayLength() > 0)
        {
            var msg = choices[0].GetProperty("message");
            return msg.GetProperty("content").GetString() ?? string.Empty;
        }

        // Azure AI or fallback
        if (root.TryGetProperty("output_text", out var textElement))
        {
            return textElement.GetString() ?? string.Empty;
        }

        return "No content returned from AI provider.";
    }

}
