using System;
using System.Text;
using System.Threading;
using RabbitMQ.Client;

namespace Publisher
{
    class Publisher
    {
        private const string ExchangeName = "fanout";
        private const string ExchangeType = "fanout";

        static void Main()
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost"
            };

            try
            {
                using (var connection = factory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        channel.ExchangeDeclare(exchange: ExchangeName, type: ExchangeType);

                        for (var i = 1; i <= 100; i++)
                        {
                            Thread.Sleep(2 * 1000);
                            SendMEssage($"Message #{i}", channel);
                        }
                    }
                }

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.ReadKey();
            }
        }

        private static void SendMEssage(string message, IModel channel)
        {
            var body = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(exchange: ExchangeName,
                            routingKey: "",
                            basicProperties: null,
                            body: body);

            Console.WriteLine(" [x] Sent {0}", message);
        }
    }
}
