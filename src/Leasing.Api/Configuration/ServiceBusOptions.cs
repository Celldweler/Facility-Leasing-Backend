namespace Leasing.Api.Configuration;

public class ServiceBusOptions
{
    public const string Section = $"Azure:{nameof(ServiceBusOptions)}";

    public required string QueueName { get; init; }
}
