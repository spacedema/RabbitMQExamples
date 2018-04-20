using System;

namespace RpcClient
{
    class Program
    {
        static void Main()
        {
            using (var rpcClient = new RpcClient())
            {
                Console.WriteLine(" [x] Requesting factorial(2)");
                var response = rpcClient.Call("2");
                Console.WriteLine(" [.] Got '{0}'", response);

                Console.WriteLine(" [x] Requesting factorial(5) with response to client 2");
                rpcClient.CallWithReplyToCLient2("5");

                Console.ReadLine();
            }
        }
    }
}
