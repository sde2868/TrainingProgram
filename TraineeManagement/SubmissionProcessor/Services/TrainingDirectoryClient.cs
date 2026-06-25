using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using SubmissionProcessor.Models;
using SubmissionProcessor.Services;

namespace SubmissionProcessor.Services;
public class TrainingDirectoryClient : ITrainingDirectoryClient
{
    private readonly HttpClient _httpClient;

    public TrainingDirectoryClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<TraineeProcessingProfileResponse?>
        GetProcessingProfileAsync(
            int traineeId,
            string correlationId,
            CancellationToken cancellationToken)
    {  
        Console.WriteLine($"Calling profile endpoint for trainee {traineeId}");

        _httpClient.DefaultRequestHeaders.Remove(
            "X-Correlation-Id");

        _httpClient.DefaultRequestHeaders.Add(
            "X-Correlation-Id",
            correlationId);

        var profile = await _httpClient.GetFromJsonAsync<TraineeProcessingProfileResponse>(
                $"api/trainees/{traineeId}/processing-profile",
                cancellationToken);

        Console.WriteLine("Response received");

        return profile;
    }
}
