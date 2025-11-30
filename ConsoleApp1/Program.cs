using System;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string apiKey = "AIzaSyAeulJakYAFxeA2PGRg9a5RH2KA03C95ak";
            CommandShell shell = new CommandShell(apiKey);
            shell.Run();
        }
    }
}