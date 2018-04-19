using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Subscriber
{
    class Subscriber
    {
        private const string ExchangeName = "fanout";
        private const string ExchangeType = "fanout";

        static void Main()
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
            };

            try
            {
                var connection = factory.CreateConnection();
                using (connection)
                {
                    using (var channel = connection.CreateModel())
                    {

                        channel.ExchangeDeclare(exchange: ExchangeName, type: ExchangeType);

                        var queueName = channel.QueueDeclare().QueueName;
                        channel.QueueBind(queue: queueName,
                                          exchange: ExchangeName,
                                          routingKey: "");

                        Console.WriteLine(" [*] Waiting for messages.");

                        var consumer = new EventingBasicConsumer(channel);
                        consumer.Received += (model, ea) =>
                        {
                            var body = ea.Body;
                            var message = Encoding.UTF8.GetString(body);
                            Console.WriteLine(" [x] {0}", message);
                        };
                        channel.BasicConsume(queue: queueName,
                                             autoAck: true,
                                             consumer: consumer);


                        Console.WriteLine(" Press [enter] to exit.");
                        Console.ReadLine();
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.ReadKey();
            }
        }
    }
}
