using GreenSpec.Application.DTOs;
using GreenSpec.Application.Interfaces;
using GreenSpec.Domain.Interfaces;
using GreenSpec.Domain.Services;
using GreenSpec.Domain.ValueObjects;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GreenSpec.Infrastructure.BackgroundServices;

public class SensorSimulationService(
    IServiceProvider serviceProvider,
    ILogger<SensorSimulationService> logger) : BackgroundService
{
    private readonly Random _random = new();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Sensor Simulation Service started");

        // Wait a bit for the application to fully start
        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await SimulateSensorReading(stoppingToken);

                // Wait 3-5 seconds before next reading
                var delay = _random.Next(3000, 5000);
                await Task.Delay(delay, stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "Error in sensor simulation");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }

        logger.LogInformation("Sensor Simulation Service stopped");
    }

    private async Task SimulateSensorReading(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();

        var configRepository = scope.ServiceProvider.GetRequiredService<IConfigRepository>();
        var alertRepository = scope.ServiceProvider.GetRequiredService<IAlertRepository>();
        var alertNotifier = scope.ServiceProvider.GetRequiredService<IAlertNotifier>();
        var alertGenerationService = new AlertGenerationService();

        // Get current config
        var config = await configRepository.GetCurrentConfigAsync(cancellationToken);
        if (config == null)
        {
            logger.LogWarning("No configuration found, skipping sensor reading");
            return;
        }

        // Generate random sensor reading
        var temperature = GenerateRandomDecimal(15m, 40m);
        var humidity = GenerateRandomDecimal(40m, 95m);
        var reading = new SensorReading(temperature, humidity);

        logger.LogInformation(
            "Sensor Reading - Temperature: {Temperature}°C, Humidity: {Humidity}%",
            temperature, humidity);

        // Evaluate against thresholds
        var alerts = alertGenerationService.EvaluateReading(reading, config);

        // Save and notify about any alerts
        foreach (var alert in alerts)
        {
            var savedAlert = await alertRepository.AddAsync(alert, cancellationToken);

            var alertDto = new AlertDto(
                savedAlert.Id,
                savedAlert.Type.ToString(),
                savedAlert.Value,
                savedAlert.Threshold,
                savedAlert.CreatedAt,
                savedAlert.Status.ToString()
            );

            await alertNotifier.NotifyAlertAsync(alertDto, cancellationToken);

            logger.LogWarning(
                "Alert Generated - Type: {Type}, Value: {Value}, Threshold: {Threshold}",
                alertDto.Type, alertDto.Value, alertDto.Threshold);
        }
    }

    private decimal GenerateRandomDecimal(decimal min, decimal max)
    {
        var range = max - min;
        var randomValue = (decimal)_random.NextDouble() * range;
        return Math.Round(min + randomValue, 2);
    }
}
