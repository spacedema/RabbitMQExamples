using System;

namespace RpcServer
{
    public class Program
    {
        static void Main()
        {
            var logger = new Logger.Logger();
            logger.Log(" [x] Awaiting RPC requests");
            using (var server = new RpcServer(logger))
            {
                server.Start();
                logger.Log(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}
