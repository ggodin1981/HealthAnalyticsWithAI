using HealthAnalytics.Application.Ai.Models;

namespace HealthAnalytics.Application.Ai;

public interface IAnalyticsAssistantService
{
    Task<AnalyzeResponse> AnalyzeAsync(AnalyzeRequest request, CancellationToken cancellationToken = default);
    Task<ChatResponse> ChatAsync(ChatRequest request, CancellationToken cancellationToken = default);
}
