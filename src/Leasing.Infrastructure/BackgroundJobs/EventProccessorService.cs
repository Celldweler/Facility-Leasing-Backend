using Leasing.Domain.Models.Events;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;

namespace Leasing.Infrastructure.BackgroundJobs;

public class EventProccessorService : BackgroundService
{
    private ILogger<EventProccessorService> _logger;
    private Channel<ContractCreatedEvent> _channel;

    public EventProccessorService(
        Channel<ContractCreatedEvent> channel,
        ILogger<EventProccessorService> logger)
    {
        _logger = logger;
        _channel = channel;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await _channel.Reader.WaitToReadAsync(stoppingToken))
        {
            var message = await _channel.Reader.ReadAsync(stoppingToken);

            _logger.LogInformation(
                "New Contract placed at {0} with equipment: {1} and facility {2}", message.CreatedAt, message.EquipmentCode, message.FacilityCode);
        }
    }
}
