using RabbitMQ.Client;
using System.Threading;
using System.Threading.Tasks;

namespace ViewerData_WPF_APP.Interfaces;

public interface IRabbitMqService
{
    Task<IChannel> CreateChannelAsync(CancellationToken cancellationToken);
}
