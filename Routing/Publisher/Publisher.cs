using System;
using System.Text;
using System.Threading;
using RabbitMQ.Client;

namespace Publisher
{
    class Publisher
    {
        private const string ExchangeName = "direct";
        private const string ExchangeType = "direct";

        private enum PriorityEnum
        {
            Low,
            Normal,
            High
        }

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
                            Thread.Sleep(1000);
                            var priority = GetPriority(i);
                            SendMEssage($"Message #{i} with [{priority}] level", priority, channel);
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


        private static string GetPriority(int i)
        {
            var priority = $"{PriorityEnum.Low}";

            if (i%3 == 0)
                priority = $"{PriorityEnum.Normal}";

            if (i%5 == 0)
                priority = $"{PriorityEnum.High}";

            return priority;
        }

        private static void SendMEssage(string message, string priority, IModel channel)
        {
            var body = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(exchange: ExchangeName,
                            routingKey: priority,
                            basicProperties: null,
                            body: body);

            Console.WriteLine(" [x] Sent {0}", message);
        }
    }
}
