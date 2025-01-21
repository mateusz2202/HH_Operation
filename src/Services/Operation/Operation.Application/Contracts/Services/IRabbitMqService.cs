using RabbitMQ.Client;

namespace Operation.Application.Contracts.Services;

public interface IRabbitMqService
{
    Task<IChannel> CreateChannelAsync(CancellationToken cancellationToken);
}
