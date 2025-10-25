using System;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Provide the required apiKey argument for CommandShell constructor
            string apiKey = "AIzaSyAeulJakYAFxeA2PGRg9a5RH2KA03C95ak"; // Replace with your actual API key
            CommandShell shell = new CommandShell(apiKey);
            shell.Run();
        }
    }
}