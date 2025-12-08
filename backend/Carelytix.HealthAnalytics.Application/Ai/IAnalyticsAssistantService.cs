using Carelytix.HealthAnalytics.Application.Ai.Models;

namespace Carelytix.HealthAnalytics.Application.Ai;

public interface IAnalyticsAssistantService
{
    Task<AnalyzeResponse> AnalyzeAsync(AnalyzeRequest request, CancellationToken cancellationToken = default);
    Task<ChatResponse> ChatAsync(ChatRequest request, CancellationToken cancellationToken = default);
}
