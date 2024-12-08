using Azure.Messaging.ServiceBus;
using Leasing.Services.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using static System.Threading.CancellationTokenSource;

namespace Leasing.Infrastructure.BackgroundJobs;

internal class ServiceBusProccessorJob<T> : BackgroundService, IAsyncDisposable
{
    private readonly IMessageHandler<T> _handler;
    private readonly ServiceBusProcessor _processor;
    private ILogger<ServiceBusProccessorJob<T>> _logger;

    private CancellationTokenSource? stoppingCts;

    public ServiceBusProccessorJob(
        IMessageHandler<T> handler,
        ServiceBusProcessor processor,
        ILogger<ServiceBusProccessorJob<T>> logger)
    {
        _logger = logger;
        _handler = handler;
        _processor = processor;

        _processor.ProcessMessageAsync += ProcessMessageAsync;
        _processor.ProcessErrorAsync += ProcessErrorAsync; 
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingCts = CreateLinkedTokenSource(stoppingToken);
        await _processor.StartProcessingAsync(CancellationToken.None);

        await CompleteOnCancelAsync(stoppingToken);

        stoppingCts.Cancel();
        await _processor.StopProcessingAsync(CancellationToken.None);
    }    

    private Task ProcessMessageAsync(ProcessMessageEventArgs args)
    {
        var body = args.Message.Body;
        var message = JsonSerializer.Deserialize<T>(body);
        var cts = CreateLinkedTokenSource(stoppingCts!.Token, args.CancellationToken);

        return _handler.HandleAsync(message, cts.Token);
    }

    private Task ProcessErrorAsync(ProcessErrorEventArgs args)
    {
        _logger.LogWarning("Error Processing {@Error}",
            new
            {
                args.Identifier,
                ErrorSource = $"{args.ErrorSource}",
                Exception = $"{args.Exception}"
            });

        return Task.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        await _processor.DisposeAsync();
        stoppingCts?.Dispose();
        base.Dispose();
    }

    private Task CompleteOnCancelAsync(CancellationToken token)
    {
        var tcs = new TaskCompletionSource();
        token.Register(t =>
        {
            if (t is TaskCompletionSource tcs)
                tcs.TrySetResult();
        }, tcs);

        return tcs.Task;
    }
}
