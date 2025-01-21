using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Threading;
using System.Threading.Tasks;
using ViewerData_WPF_APP.Interfaces;

namespace ViewerData_WPF_APP.Services;

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
