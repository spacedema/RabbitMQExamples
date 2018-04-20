using System;
using RpcClient.Logger;

namespace RpcClient2
{
    class Program
    {
        static void Main()
        {
            using (var rpcClient = new RpcClient(new Logger()))
            {
                var response = rpcClient.Call("3");
                DoSomethingWithResponse(response);
                Console.ReadLine();
            }
        }

        // ReSharper disable once UnusedParameter.Local
        private static void DoSomethingWithResponse(string response)
        {
            
        }
    }
}
