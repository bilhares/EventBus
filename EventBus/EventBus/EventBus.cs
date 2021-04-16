using System;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EventBus
{
    public class EventBus : IEventBus
    {
        private readonly IConnection _connection;
        private IModel _channel;
        public IModel Channel
        {
            get
            {
                if (_channel == null)
                    _channel = _connection.CreateModel();

                return _channel;
            }
        }

        private readonly IServiceProvider _serviceProvider;

        public EventBus(IConfiguration config, IServiceProvider provider)
        {
            var connectionFactory = new ConnectionFactory
            {
                HostName = config["RabbitMq:HostName"],
                Port = int.Parse(config["RabbitMq:Port"]),
                UserName = config["RabbitMq:UserName"],
                Password = config["RabbitMq:Password"],
                DispatchConsumersAsync = true
            };

            _connection = connectionFactory.CreateConnection();

            _serviceProvider = provider;
        }

        public void Publish(IIntegrationEvent @event, string exchangeName)
        {
            CreateExchangeIfNotExists(exchangeName);

            string message = JsonConvert.SerializeObject(@event);
            var body = Encoding.UTF8.GetBytes(message);

            Channel.BasicPublish(exchangeName, string.Empty, body: body);
        }

        public void Subscribe<TH, TE>(string exchangeName, string subscriberName)
            where TH : IIntegrationEventHandler<TE>
            where TE : IIntegrationEvent
        {
            BindQueue(exchangeName, subscriberName);

            var consumer = new AsyncEventingBasicConsumer(Channel);

            consumer.Received += async (obj, args) =>
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var handler = scope.ServiceProvider.GetRequiredService<IIntegrationEventHandler<TE>>();

                    var jsonMessage = Encoding.UTF8.GetString(args.Body.ToArray());
                    var message = JsonConvert.DeserializeObject<TE>(jsonMessage);

                    await handler.HandleAsync(message);

                    Channel.BasicAck(args.DeliveryTag, false);
                }
            };

            Channel.BasicConsume(subscriberName, false, consumer);
        }

        private void CreateExchangeIfNotExists(string exchangeName)
        {
            Channel.ExchangeDeclare(exchangeName, ExchangeType.Fanout, true);
        }

        private void BindQueue(string exchangeName, string subscriberName)
        {
            CreateExchangeIfNotExists(exchangeName);

            Channel.QueueDeclare(subscriberName, true, false, false);
            Channel.QueueBind(subscriberName, exchangeName, string.Empty);
        }
    }
}
