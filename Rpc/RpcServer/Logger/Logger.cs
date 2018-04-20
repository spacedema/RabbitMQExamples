using System;

namespace RpcServer.Logger
{
    public class Logger : ILogger
    {
        public void Log(string value)
        {
            Console.WriteLine(value);
        }
    }
}
