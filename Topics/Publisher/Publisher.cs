using System;
using System.Text;
using System.Threading;
using RabbitMQ.Client;

namespace Publisher
{
    class Publisher
    {
        private const string ExchangeName = "topic";
        private const string ExchangeType = "topic";

        public enum PriorityEnum
        {
            Low,
            Normal,
            High
        }

        public enum UrgencyEnum
        {
            Urgent,
            NonUrgent
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
                            var priorityWithUrgency = GetPriority(i);
                            SendMEssage($"Message #{i} with [{priorityWithUrgency}] level", priorityWithUrgency, channel);
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
            var urgency = $"{UrgencyEnum.NonUrgent}";

            if (i%2 == 0)
                urgency = $"{UrgencyEnum.Urgent}";

            if (i % 3 == 0)
                priority = $"{PriorityEnum.Normal}";

            if (i % 5 == 0)
                priority = $"{PriorityEnum.High}";

            return $"{urgency}.{priority}";
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
