using System;
using System.Collections.Concurrent;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RpcClient
{
    public class RpcClient : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string _replyQueueName;
        private readonly EventingBasicConsumer _consumer;
        private readonly BlockingCollection<string> _respQueue = new BlockingCollection<string>();
        private readonly IBasicProperties _selfProps;
        private readonly IBasicProperties _client2Props;
        private readonly string _correlationId = "client_1_correlation_id";
        private readonly string _client2CorrelationId = "client_2_correlation_id";
        private const string ServerQueueName = "rpc";
        private const string Client2QueueName = "rpc_client_2_queue";

        public RpcClient()
        {
            var factory = new ConnectionFactory { HostName = "localhost" };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _replyQueueName = _channel.QueueDeclare().QueueName;
            _consumer = new EventingBasicConsumer(_channel);
            _consumer.Received += ConsumerReceived;

            _selfProps = _channel.CreateBasicProperties();
            _selfProps.CorrelationId = _correlationId;
            _selfProps.ReplyTo = _replyQueueName;

            _client2Props = _channel.CreateBasicProperties();
            _client2Props.CorrelationId = _client2CorrelationId;
            _client2Props.ReplyTo = Client2QueueName;
        }

        private void ConsumerReceived(object sender, BasicDeliverEventArgs e)
        {
            var body = e.Body;
            var response = Encoding.UTF8.GetString(body);
            if (e.BasicProperties.CorrelationId == _correlationId)
                _respQueue.Add(response);
        }

        public string Call(string message)
        {
            var messageBytes = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(
                exchange: "",
                routingKey: ServerQueueName,
                basicProperties: _selfProps,
                body: messageBytes);

            _channel.BasicConsume(
                consumer: _consumer,
                queue: _replyQueueName,
                autoAck: true);

            return _respQueue.Take();
        }

        public void CallWithReplyToCLient2(string message)
        {
            var messageBytes = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(
                exchange: "",
                routingKey: ServerQueueName,
                basicProperties: _client2Props,
                body: messageBytes);
        }

        public void Dispose()
        {
            _consumer.Received -= ConsumerReceived;
            _channel.Close();
            _connection.Close();
        }
    }
}
