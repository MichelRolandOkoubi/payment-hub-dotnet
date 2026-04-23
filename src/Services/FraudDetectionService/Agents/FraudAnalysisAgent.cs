using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Text.Json;

namespace FraudDetectionService.Agents;

public record FraudScore(float Score, string RiskLevel, string[] Reasons, string Recommendation);

public class FraudAnalysisAgent
{
    private readonly Kernel _kernel;
    private readonly ILogger<FraudAnalysisAgent> _logger;

    public FraudAnalysisAgent(Kernel kernel, ILogger<FraudAnalysisAgent> logger)
    {
        _kernel = kernel;
        _logger = logger;
    }

    public async Task<FraudScore> AnalyzeAsync(object transaction)
    {
        var chat = _kernel.GetRequiredService<IChatCompletionService>();
        
        var history = new ChatHistory();
        history.AddSystemMessage("""
            You are a senior fraud analyst. Analyze the transaction and return ONLY a JSON:
            {
              "score": 0.0-1.0,
              "riskLevel": "Low|Medium|High|Critical",
              "reasons": ["string"],
              "recommendation": "Approve|Review|Decline"
            }
            Rules:
            - Amount > 10 000 EUR → increase score
            - New merchant + high amount → critical
            - Velocity > 5 tx/min → high risk
            """);

        history.AddUserMessage($"Transaction: {JsonSerializer.Serialize(transaction)}");

        var response = await chat.GetChatMessageContentAsync(history);
        
        var result = JsonSerializer.Deserialize<FraudScore>(
            response.Content!, 
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        _logger.LogInformation("Analysis result: {Recommendation} (Score: {Score})", result!.Recommendation, result.Score);

        return result;
    }
}
