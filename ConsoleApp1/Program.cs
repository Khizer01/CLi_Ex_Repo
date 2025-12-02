using System;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string apiKey = "AIzaSyDNX1my7VNyONDLV9TS3vN6pGIXaQFsfhA";
            CommandShell shell = new CommandShell(apiKey);
            shell.Run();
        }
    }
}