using Microsoft.Extensions.Options;
using Operation.Application.Contracts.Services;
using Operation.Infrastructure.Configuration;
using RabbitMQ.Client;

namespace Operation.Infrastructure.Services;

public class RabbitMqService : IRabbitMqService
{
    private readonly RabbitMqConfiguration _configuration;
    public RabbitMqService(IOptions<RabbitMqConfiguration> options)
    {
        _configuration = options.Value;
    }
    public async Task<IChannel> CreateChannelAsync(CancellationToken cancellationToken)
    {
        ConnectionFactory connectionFactory = new()
        {
            UserName = _configuration.Username,
            Password = _configuration.Password,
            HostName = _configuration.HostName,
        };
        var connection = await connectionFactory.CreateConnectionAsync(cancellationToken);
        var chanel = await connection.CreateChannelAsync(null, cancellationToken);
        return chanel;
    }
}
