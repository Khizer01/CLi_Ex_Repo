using System;
using System.IO;
using System.Text;

namespace ConsoleApp1
{
    public class FileCommands
    {
        public void CreateFile(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: create <file_name>");
                return;
            }

            string filePath = args[0];
            if (!File.Exists(filePath))
            {
                File.Create(filePath).Close();
                Console.WriteLine($"File created: {filePath}");
            }
            else
            {
                Console.WriteLine($"File already exists: {filePath}");
            }
        }

        public void EditFile(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: edit <file_name>");
                return;
            }

            string filePath = args[0];

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File not found: {filePath}");
                Console.Write("Create new file? (y/n): ");
                if (Console.ReadLine().Trim().ToLower() != "y")
                    return;

                File.Create(filePath).Close();
            }

            Console.WriteLine($"Editing file: {filePath}");
            Console.WriteLine("Enter text content. Type ':wq' on a new line to save and exit.");

            var content = new StringBuilder();

            // If file exists, load its content
            if (File.Exists(filePath))
            {
                string existingContent = File.ReadAllText(filePath);
                content.Append(existingContent);
                Console.WriteLine("Current content:");
                Console.WriteLine(existingContent);
            }

            string line;
            while ((line = Console.ReadLine()) != ":wq")
            {
                content.AppendLine(line);
            }

            File.WriteAllText(filePath, content.ToString());
            Console.WriteLine($"File saved: {filePath}");
        }

        public void DisplayFileContents(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: cat <file_name>");
                return;
            }

            string filePath = args[0];
            if (File.Exists(filePath))
            {
                string content = File.ReadAllText(filePath);
                Console.WriteLine($"--- Content of {filePath} ---");
                Console.WriteLine(content);
                Console.WriteLine("--- End of file ---");
            }
            else
            {
                Console.WriteLine($"File not found: {filePath}");
            }
        }

        public void DeleteFile(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: delete <file_name>");
                return;
            }

            string path = args[0];
            if (File.Exists(path))
            {
                File.Delete(path);
                Console.WriteLine($"File deleted: {path}");
            }
            else if (Directory.Exists(path))
            {
                Directory.Delete(path, false);
                Console.WriteLine($"Directory deleted: {path}");
            }
            else
            {
                Console.WriteLine($"File or directory not found: {path}");
            }
        }
    }
}