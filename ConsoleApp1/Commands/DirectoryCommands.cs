using System;
using System.IO;

namespace ConsoleApp1
{
    public class DirectoryCommands
    {
        public void ChangeDirectory(string[] args)
        {
            if (args.Length == 0)
            {
                // If no arguments, show current directory
                Console.WriteLine(Directory.GetCurrentDirectory());
                return;
            }

            string path = args[0];
            if (Directory.Exists(path))
            {
                Directory.SetCurrentDirectory(path);
            }
            else
            {
                Console.WriteLine($"Directory not found: {path}");
            }
        }

        public void ListDirectory(string[] args)
        {
            string path = args.Length > 0 ? args[0] : Directory.GetCurrentDirectory();

            if (!Directory.Exists(path))
            {
                Console.WriteLine($"Directory not found: {path}");
                return;
            }

            // List directories
            Console.WriteLine("Directories:");
            foreach (string dir in Directory.GetDirectories(path))
            {
                Console.WriteLine($"[DIR] {Path.GetFileName(dir)}");
            }

            // List files
            Console.WriteLine("\nFiles:");
            foreach (string file in Directory.GetFiles(path))
            {
                var fileInfo = new FileInfo(file);
                Console.WriteLine($"[FILE] {Path.GetFileName(file)} - {fileInfo.Length} bytes");
            }
        }

        public void CreateDirectory(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: mkdir <directory_name>");
                return;
            }

            string dirPath = args[0];
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
                Console.WriteLine($"Directory created: {dirPath}");
            }
            else
            {
                Console.WriteLine($"Directory already exists: {dirPath}");
            }
        }
    }
}