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
            Console.WriteLine("  copy <src> <dst>   - Copy file or directory");
            Console.WriteLine("  info <path>        - Show info about file or directory");
            Console.WriteLine("  pwd                - Show current directory");
            Console.WriteLine("  date               - Show current date");
            Console.WriteLine("  time               - Show current time");
            Console.WriteLine("  echo <text>        - Print text to the console");
            Console.WriteLine("  help               - Display this help information");
            Console.WriteLine("  exit               - Exit the shell");
            Console.WriteLine("  tasks              - Open the interactive task manager");
        }

        public void Exit(string[] args)
        {
            Console.WriteLine("Exiting shell...");
            _shell.StopShell();
        }

        public void Date(string[] args)
        {
            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd"));
        }

        public void Time(string[] args)
        {
            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss"));
        }

        public void Pwd(string[] args)
        {
            Console.WriteLine(System.IO.Directory.GetCurrentDirectory());
        }

        public void Echo(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                Console.WriteLine();
                return;
            }

            Console.WriteLine(string.Join(" ", args));
        }
    }
}