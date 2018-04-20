using System;

namespace RpcClient.Logger
{
    public class Logger : ILogger
    {
        public void Log(string value)
        {
            Console.WriteLine(value);
        }
    }
}
