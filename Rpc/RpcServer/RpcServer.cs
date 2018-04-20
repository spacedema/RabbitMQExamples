using System;
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RpcServer.Logger;

namespace RpcServer
{
    public class RpcServer : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly EventingBasicConsumer _consumer;
        private const string QueueName = "rpc";
        private readonly ILogger _logger;

        public RpcServer(ILogger logger)
        {
            _logger = logger;
            var factory = new ConnectionFactory {HostName = "localhost"};

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _consumer = new EventingBasicConsumer(_channel);
            _consumer.Received += ConsumerReceived;
        }

        public void Start()
        {
            _channel.QueueDeclare(queue: QueueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            _channel.BasicQos(
                prefetchSize: 0,
                prefetchCount: 1,
                global: false);

            _channel.BasicConsume(queue: QueueName,
                autoAck: false,
                consumer: _consumer);
        }

        private void ConsumerReceived(object sender, BasicDeliverEventArgs e)
        {
            var response = "";

            var body = e.Body;
            var props = e.BasicProperties;
            var replyProps = _channel.CreateBasicProperties();
            replyProps.CorrelationId = props.CorrelationId;

            try
            {
                var message = Encoding.UTF8.GetString(body);
                var n = int.Parse(message);
                _logger.Log($" [.] factorial({message})");
                response = Factorial(n).ToString();
            }
            catch (Exception ex)
            {
                _logger.Log(" [.] " + ex.Message);
                response = "";
            }
            finally
            {
                var responseBytes = Encoding.UTF8.GetBytes(response);
                _channel.BasicPublish(exchange: "",
                    routingKey: props.ReplyTo,
                    basicProperties: replyProps,
                    body: responseBytes);

                _channel.BasicAck(deliveryTag: e.DeliveryTag,
                    multiple: false);
            }
        }

        public void Dispose()
        {
            _consumer.Received += ConsumerReceived;
            _channel.Dispose();
            _connection.Dispose();
        }

        private static int Factorial(int i)
        {
            Thread.Sleep(1000);

            if (i <= 1)
                return 1;

            return i * Factorial(i - 1);
        }
    }
}
