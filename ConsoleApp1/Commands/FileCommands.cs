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

        // New: copy command - supports files and directories
        public void Copy(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: copy <source> <destination>");
                return;
            }

            string source = args[0];
            string destination = args[1];

            try
            {
                if (File.Exists(source))
                {
                    File.Copy(source, destination, overwrite: false);
                    Console.WriteLine($"File copied from {source} to {destination}");
                }
                else if (Directory.Exists(source))
                {
                    DirectoryCopy(source, destination);
                    Console.WriteLine($"Directory copied from {source} to {destination}");
                }
                else
                {
                    Console.WriteLine($"Source not found: {source}");
                }
            }
            catch (IOException ioEx)
            {
                Console.WriteLine("IO Error: " + ioEx.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        // New: info command - display file or directory information
        public void Info(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: info <path>");
                return;
            }

            string path = args[0];

            if (File.Exists(path))
            {
                var fi = new FileInfo(path);
                Console.WriteLine($"File: {fi.FullName}");
                Console.WriteLine($"Size: {fi.Length} bytes");
                Console.WriteLine($"Created: {fi.CreationTime}");
                Console.WriteLine($"Modified: {fi.LastWriteTime}");
            }
            else if (Directory.Exists(path))
            {
                var di = new DirectoryInfo(path);
                Console.WriteLine($"Directory: {di.FullName}");
                Console.WriteLine($"Created: {di.CreationTime}");
                Console.WriteLine($"Modified: {di.LastWriteTime}");
                Console.WriteLine($"Contains: {di.GetFiles().Length} files, {di.GetDirectories().Length} directories");
            }
            else
            {
                Console.WriteLine($"Path not found: {path}");
            }
        }

        // Helper to copy directories recursively
        private void DirectoryCopy(string sourceDirName, string destDirName)
        {
            // Create destination directory if it doesn't exist
            var dir = new DirectoryInfo(sourceDirName);
            if (!dir.Exists)
                throw new DirectoryNotFoundException("Source directory does not exist: " + sourceDirName);

            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Copy files
            foreach (FileInfo file in dir.GetFiles())
            {
                string tempPath = Path.Combine(destDirName, file.Name);
                file.CopyTo(tempPath, false);
            }

            // Copy subdirectories
            foreach (DirectoryInfo subdir in dir.GetDirectories())
            {
                string tempPath = Path.Combine(destDirName, subdir.Name);
                DirectoryCopy(subdir.FullName, tempPath);
            }
        }
    }
}