using Cryptocop.Software.API.Services.Interfaces;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System.Text;
using System;

namespace Cryptocop.Software.API.Services.Implementations
{
    public class QueueService : IQueueService, IDisposable
    {
        private readonly string _exchangeName;

        private readonly IConnection _connection;
        private readonly IModel _channel;

        public QueueService(IConfiguration configuration)
        {
            var configSection = configuration.GetSection("RabbitMQ");

            _exchangeName = configSection.GetValue<string>("Exchange");

            var inDocker = InDocker;
            var hostName = "";

            //This is done since the docker container cannot resolve the hostname "localhost"
            if (inDocker)
            {
                hostName = "rabbitmq";
            }
            else
            {
                hostName = configSection.GetValue<string>("Host");
            }

            IAsyncConnectionFactory connectionFactory = new ConnectionFactory
            {
                HostName = hostName
            };

            _connection = connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        private bool InDocker { get { return Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true"; } }

        public void PublishMessage(string routingKey, object body) => Publish(routingKey, body);


        private void Publish(string routingKey, object data)
        {
            //Serialize to json (camelcase) so it can be represented correctly bytewise on the channel
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            //convert to byte array
            var bytes = Encoding.UTF8.GetBytes(json);

            //publish to the channel
            _channel.BasicPublish(_exchangeName, routingKey, body: bytes);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);

            _channel.Dispose();
            _connection.Dispose();
        }
    }
}