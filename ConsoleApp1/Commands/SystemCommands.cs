using System;

namespace ConsoleApp1
{
    public class SystemCommands
    {
        private readonly CommandShell _shell;

        public SystemCommands(CommandShell shell)
        {
            _shell = shell;
        }

        public void ShowHelp(string[] args)
        {
            Console.WriteLine("Available commands:");
            Console.WriteLine("  cd <directory>     - Change current directory");
            Console.WriteLine("  dir                - List files and directories in current directory");
            Console.WriteLine("  mkdir <directory>  - Create a new directory");
            Console.WriteLine("  create <file>      - Create a new empty file");
            Console.WriteLine("  edit <file>        - Edit file content");
            Console.WriteLine("  cat <file>         - Display file content");
            Console.WriteLine("  delete <path>      - Delete a file or empty directory");
            Console.WriteLine("  help               - Display this help information");
            Console.WriteLine("  exit               - Exit the shell");
            Console.WriteLine("  tasks              - Open the interactive task manager");
        }

        public void Exit(string[] args)
        {
            Console.WriteLine("Exiting shell...");
            _shell.StopShell();
        }
    }
}