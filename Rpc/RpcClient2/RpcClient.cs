using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RpcClient2
{
    public class RpcClient : IDisposable
    {
        public EventHandler<FactorialResultEventArgs> ResultReceivedEventHandler;

        public void OnResultReceivedEventHandler(string response)
        {
            ResultReceivedEventHandler?.Invoke(this, new FactorialResultEventArgs(response));
        }

        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly EventingBasicConsumer _consumer;
        private readonly IBasicProperties _selfReplyProps;
        private const string QueueName = "rpc_client_2_queue";
        private const string ServerQueueName = "rpc";
        private readonly string _correlationId = "client_2_correlation_id";
        
        public RpcClient()
        {
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

            OnResultReceivedEventHandler(response);
        }

        public void Call(string message, Action<string> doSmthing)
        {
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
        }

        public void Dispose()
        {
            _consumer.Received -= ConsumerReceived;
            _channel.Close();
            _connection.Close();
        }
    }

    public class FactorialResultEventArgs : EventArgs
    {
        public string Response { get; private set; }
        public FactorialResultEventArgs(string response)
        {
            Response = response;
        }
    }
}
