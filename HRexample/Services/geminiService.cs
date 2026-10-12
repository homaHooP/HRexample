using Google.GenAI;
using Google.GenAI.Types;
using HRexample.Models;
using Newtonsoft.Json;

public class GeminiService (Client client, ILogger<GeminiService> logger, List<string> models)
{
    public async Task<ModelResponse> AnalyzeResume(string vymogi, string resume)
    {
        Exception? lastError = null;
        foreach (var model in models)
        {
            try
            {
                var resp = await client.Models.GenerateContentAsync(
                model: model,
                contents: $"""
                Переглянь текст резюме та вимог до посади. Визнач, наскільки кандидат підходить на цю посаду.
                Відповідь має бути ЛИШЕ у валідному форматі JSON з такими полями: 
                "Guid": "" (встав рандомний Guid),
                "Name": "",
                "Surname": "",
                "skills": ["",""],
                "Education": "",
                "Experience": "",
                "PercentageFitForJob": 0 (int від 0 до 100) 
                Вимоги до посади: {vymogi}
                Резюме кандидата: {resume}
                """,
                config: new GenerateContentConfig { ResponseMimeType = "application/json" }
                );
                var result = JsonConvert.DeserializeObject<ModelResponse>(resp.Text ?? "");
                if (result != null)
                    return result;
            }
            catch (Exception ex)
            {
                // Модель недоступна або повернула некоректну відповідь — пробуємо наступну
                lastError = ex;
                logger.LogWarning(ex, "Model {Model} failed to analyze resume", model);
            }
        }
        logger.LogError(lastError, "Error occurred while analyzing resume");
        throw new InvalidOperationException("All Gemini models failed to analyze resume", lastError);
    }
}