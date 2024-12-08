using Leasing.Domain.Models.Events;
using Leasing.Services.Abstractions;
using Microsoft.Extensions.Logging;

namespace Leasing.Infrastructure;

public class ContractCreatedEventHandler : IMessageHandler<ContractCreatedEvent>
{
    private readonly ILogger<ContractCreatedEventHandler> _logger;

    public ContractCreatedEventHandler(ILogger<ContractCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(ContractCreatedEvent message, CancellationToken cancelToken)
    {
        _logger.LogInformation("New Contract placed at {0} with equipment: {1} and facility {2}", message.CreatedAt, message.EquipmentCode, message.FacilityCode);

        return Task.CompletedTask;
    }
}
