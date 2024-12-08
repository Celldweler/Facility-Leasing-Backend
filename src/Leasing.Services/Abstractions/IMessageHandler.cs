using Leasing.Domain.Models.Events;
namespace Leasing.Services.Abstractions;

public interface IMessageHandler<T>
{
    public Task HandleAsync(T message, CancellationToken cancelToken);
}
