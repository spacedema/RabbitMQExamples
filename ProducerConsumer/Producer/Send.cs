using System;
using System.Text;
using RabbitMQ.Client;

namespace Producer
{
    public class Send
    {
        const string QueueName = "task_queue3";

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
                        // Guaranteed delivery: durable: true
                        channel.QueueDeclare(queue: QueueName,
                            durable: true,
                            exclusive: false,
                            autoDelete: false,
                            arguments: null);

                        // Guaranteed delivery
                        // Marking messages as persistent doesn't fully guarantee that a message won't be lost. 
                        // Although it tells RabbitMQ to save the message to disk, there is still a short time window when RabbitMQ 
                        // has accepted a message and hasn't saved it yet. 
                        // Also, RabbitMQ doesn't do fsync(2) for every message -- it may be just saved to cache and not really written to the disk. 
                        // The persistence guarantees aren't strong, but it's more than enough for our simple task queue. 
                        // If you need a stronger guarantee then you can use publisher confirms.
                        // https://www.rabbitmq.com/confirms.html
                        var properties = channel.CreateBasicProperties();
                        properties.Persistent = true;

                        for (var i = 1; i <= 10; i++)
                        {
                            SendWork($"Message #{i} " + new string('.', i % 2 == 0 ? 3 : 1), channel, properties);
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

        private static void SendWork(string work, IModel channel, IBasicProperties properties)
        {
            var body = Encoding.UTF8.GetBytes(work);
            channel.BasicPublish(exchange: "",
                            routingKey: QueueName,
                            basicProperties: properties,
                            body: body);

            Console.WriteLine(" [x] Sent {0}", work);
        }
    }
}
