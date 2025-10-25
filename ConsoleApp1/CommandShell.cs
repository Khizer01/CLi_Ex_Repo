using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConsoleApp1
{
    public class CommandShell
    {
        private bool isRunning = true;
        private Dictionary<string, Action<string[]>> commands;
        private FileCommands fileCommands = new FileCommands();
        private DirectoryCommands directoryCommands = new DirectoryCommands();
        private SystemCommands systemCommands;
        private AICommands aiCommands;
        private TaskManagerCommands taskManagerCommands = new TaskManagerCommands(); // Add this field to your CommandShell class

        public CommandShell(string apiKey)
        {
            systemCommands = new SystemCommands(this);
            aiCommands = new AICommands(apiKey);  // Pass API key directly
            
            // Initialize command dictionary
            commands = new Dictionary<string, Action<string[]>>(StringComparer.OrdinalIgnoreCase)
            {
                // Directory commands
                { "cd", directoryCommands.ChangeDirectory },
                { "dir", directoryCommands.ListDirectory },
                { "mkdir", directoryCommands.CreateDirectory },
                
                // File commands
                { "create", fileCommands.CreateFile },
                { "edit", fileCommands.EditFile },
                { "cat", fileCommands.DisplayFileContents },
                { "delete", fileCommands.DeleteFile },
                
                // System commands
                { "help", systemCommands.ShowHelp },
                { "exit", systemCommands.Exit },
                
                // AI commands
                { "ask-ai", aiCommands.AskAI },
                { "tasks", taskManagerCommands.ShowTaskManager } // Add this to your commands dictionary in the CommandShell constructor    
                // Remove the set-api-key command as it's no longer needed
            };
        }

        public void Run()
        {
            Console.WriteLine("Simple Command Shell. Type 'help' for available commands.");

            while (isRunning)
            {
                // Display current directory in prompt
                Console.Write($"{Directory.GetCurrentDirectory()}> ");

                // Read command
                string input = Console.ReadLine();

                // Parse command and arguments
                if (string.IsNullOrWhiteSpace(input))
                    continue;

                string[] parts = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                string command = parts[0];
                string[] arguments = parts.Skip(1).ToArray();

                // Execute command if it exists
                if (commands.TryGetValue(command, out var action))
                {
                    try
                    {
                        action(arguments);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine($"Command not recognized: {command}");
                }
            }
        }

        public void StopShell()
        {
            isRunning = false;
        }
    }
}