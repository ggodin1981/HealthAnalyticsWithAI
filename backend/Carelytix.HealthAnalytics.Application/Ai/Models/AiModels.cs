namespace HealthAnalytics.Application.Ai.Models;

public record AnalyzeRequest(
    string Question,
    string? PatientMrn,
    bool IncludePopulationStats = true
);

public record AnalyzeResponse(
    string Answer,
    string Model
);

public record ChatMessageDto(
    string Role,
    string Content
);

public record ChatRequest(
    IList<ChatMessageDto> Messages
);

public record ChatResponse(
    IList<ChatMessageDto> Messages,
    string Model
);
