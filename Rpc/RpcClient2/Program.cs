using System;

namespace RpcClient2
{
    class Program
    {
        static void Main()
        {
            using (var rpcClient = new RpcClient())
            {
                rpcClient.ResultReceivedEventHandler += (sender, e) =>
                {
                    Console.WriteLine($" [.] Got {e.Response}");
                };

                for (var i = 1; i < 5; i++)
                {
                    Console.WriteLine($" [x] Requesting factorial({i})");
                    rpcClient.Call(i.ToString(), Console.WriteLine);
                }

                Console.ReadLine();
            }
        }
    }
}
