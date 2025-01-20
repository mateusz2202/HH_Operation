using Operation.Application.Contracts.Services;
using Operation.Shared.Constans;
using RabbitMQ.Client;
using System.Text;

namespace Operation.Infrastructure.Services;

public class OperationService : IOperationService
{
    private readonly IRabbitMqService _rabbitMqService;

    public OperationService(IRabbitMqService rabbitMqService)
    {
        _rabbitMqService = rabbitMqService;
    }

    public async Task SendInfoAddedOperationAsync(CancellationToken cancellationToken)
    {
        using var channel = await _rabbitMqService.CreateChannelAsync(cancellationToken);


        await channel.ExchangeDeclareAsync(
            exchange: ApplicationConstants.RabbitMq.EXCHANGE_OPERATION,
            type: ExchangeType.Fanout,
            cancellationToken: cancellationToken);

        var body = Encoding.UTF8.GetBytes("refresh_operation");

        var props = new BasicProperties();

        await channel.BasicPublishAsync(exchange: ApplicationConstants.RabbitMq.EXCHANGE_OPERATION,
                        routingKey: string.Empty,
                        mandatory: false,
                        basicProperties: props,
                        body: body,
                        cancellationToken);

        await Task.CompletedTask;
    }

}
