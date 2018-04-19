using System;
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Consumer
{
    internal class Receive
    {
        const string Queue = "task_queue3";

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
                        // Guaranteed delivery: durable: true
                        channel.QueueDeclare(queue: Queue,
                            durable: true,
                            exclusive: false,
                            autoDelete: false,
                            arguments: null);

                        // Fair dispatch: prefetchCount: 1
                        channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                        var channel1 = channel;
                        var consumer = new EventingBasicConsumer(channel);
                        consumer.Received += (model, ea) =>
                        {
                            var body = ea.Body;
                            var message = Encoding.UTF8.GetString(body);
                            Console.WriteLine(" [x] Received {0}", message);

                            var dots = message.Split('.').Length - 1;
                            Thread.Sleep(dots * 1000);

                            Console.WriteLine(" [x] Done");

                            // Message acknowledgment
                            channel1.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                        };

                        // Message acknowledgment: autoAck: false
                        channel.BasicConsume(queue: Queue,
                            autoAck: false,
                            consumer: consumer);

                        Console.WriteLine(" Press [enter] to exit.");
                        Console.ReadLine();
                    }
                }

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.ReadKey();
            }
        }
    }
}
