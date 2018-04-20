using System;
using System.Collections.Concurrent;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RpcClient.Logger;

namespace RpcClient2
{
    public class RpcClient : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly EventingBasicConsumer _consumer;
        private readonly BlockingCollection<string> _respQueue = new BlockingCollection<string>();
        private readonly IBasicProperties _selfReplyProps;
        private const string QueueName = "rpc_client_2_queue";
        private const string ServerQueueName = "rpc";
        private readonly string _correlationId = "client_2_correlation_id";
        private readonly ILogger _logger;

        public RpcClient(ILogger logger)
        {
            _logger = logger;
            var factory = new ConnectionFactory { HostName = "localhost" };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(
                queue: QueueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            _selfReplyProps = _channel.CreateBasicProperties();
            _selfReplyProps.CorrelationId = _correlationId;
            _selfReplyProps.ReplyTo = QueueName;

            _consumer = new EventingBasicConsumer(_channel);
            _consumer.Received += ConsumerReceived;
        }

        private void ConsumerReceived(object sender, BasicDeliverEventArgs e)
        {
            var body = e.Body;
            var response = Encoding.UTF8.GetString(body);
            if (e.BasicProperties.CorrelationId != _correlationId)
                return;

            _respQueue.Add(response);
            _logger.Log($" [.] Got {response}");
        }

        public string Call(string message)
        {
            _logger.Log($" [x] Requesting factorial({message})");

            var messageBytes = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(
                exchange: string.Empty,
                routingKey: ServerQueueName,
                basicProperties: _selfReplyProps,
                body: messageBytes);

            _channel.BasicConsume(
                consumer: _consumer,
                queue: QueueName,
                autoAck: true);

            return _respQueue.Take();
        }

        public void Dispose()
        {
            _consumer.Received -= ConsumerReceived;
            _channel.Close();
            _connection.Close();
        }
    }
}
