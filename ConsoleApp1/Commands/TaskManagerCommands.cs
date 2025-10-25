using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Management;

namespace ConsoleApp1
{
    public class TaskManagerCommands
    {
        private bool _isRunning;
        private Timer _refreshTimer;
        private readonly object _lockObject = new object();

        public void ShowTaskManager(string[] args)
        {
            _isRunning = true;
            Console.Clear();
            Console.WriteLine("Simple Task Manager - Press ESC to exit, K to kill a process, D for details");
            Console.WriteLine("-----------------------------------------------------------------------");

            // Start refresh timer to update every 2 seconds
            _refreshTimer = new Timer(RefreshProcessList, null, 0, 2000);

            // Keep running until ESC is pressed
            while (_isRunning)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.Escape)
                    {
                        _isRunning = false;
                    }
                    else if (key.Key == ConsoleKey.K)
                    {
                        KillProcess();
                    }
                    else if (key.Key == ConsoleKey.D)
                    {
                        ShowProcessDetails();
                    }
                }

                Thread.Sleep(100);
            }

            // Clean up
            _refreshTimer.Dispose();
            Console.Clear();
            Console.WriteLine("Task Manager closed.");
        }

        private void RefreshProcessList(object state)
        {
            lock (_lockObject)
            {
                try
                {
                    // Save cursor position
                    int currentRow = Console.CursorTop;
                    int currentCol = Console.CursorLeft;

                    // Clear the area below the header
                    Console.SetCursorPosition(0, 2);
                    for (int i = 0; i < Console.WindowHeight - 5; i++)
                    {
                        Console.Write(new string(' ', Console.WindowWidth));
                        if (i < Console.WindowHeight - 6)
                            Console.WriteLine();
                    }

                    // Reset cursor to start of process list
                    Console.SetCursorPosition(0, 2);

                    // Get all running processes
                    var processes = Process.GetProcesses()
                        .OrderByDescending(p => p.WorkingSet64)
                        .Take(Console.WindowHeight - 7) // Limit to fit window
                        .ToList();

                    // Display column headers
                    Console.WriteLine("PID\tMemory (MB)\tCPU\tName");
                    Console.WriteLine("-----------------------------------------------------------------------");

                    // Display processes
                    foreach (var process in processes)
                    {
                        try
                        {
                            float memoryMB = process.WorkingSet64 / 1024f / 1024f;
                            string cpuUsage = GetCpuUsage(process);
                            Console.WriteLine($"{process.Id,-7}\t{memoryMB,10:F2}\t{cpuUsage,5}\t{process.ProcessName}");
                        }
                        catch
                        {
                            // Skip processes we can't access due to permissions
                        }
                    }

                    // Display system information
                    Console.WriteLine();
                    Console.WriteLine($"Total processes: {Process.GetProcesses().Length}");
                    
                    // Restore cursor position
                    Console.SetCursorPosition(currentCol, currentRow);
                }
                catch (Exception ex)
                {
                    // Handle exceptions during refresh
                    Console.SetCursorPosition(0, Console.WindowHeight - 3);
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }

        private string GetCpuUsage(Process process)
        {
            try
            {
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                    $"SELECT PercentProcessorTime FROM Win32_PerfFormattedData_PerfProc_Process WHERE IDProcess = {process.Id}"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        return obj["PercentProcessorTime"].ToString() + "%";
                    }
                }
            }
            catch
            {
                // Return N/A if CPU usage can't be determined
            }
            return "N/A";
        }

        private void KillProcess()
        {
            lock (_lockObject)
            {
                // Pause timer to avoid screen corruption
                _refreshTimer.Change(Timeout.Infinite, Timeout.Infinite);

                try
                {
                    // Get process ID from user
                    Console.SetCursorPosition(0, Console.WindowHeight - 3);
                    Console.Write(new string(' ', Console.WindowWidth));
                    Console.SetCursorPosition(0, Console.WindowHeight - 3);
                    Console.Write("Enter PID to kill: ");
                    string input = Console.ReadLine();
                    
                    if (int.TryParse(input, out int pid))
                    {
                        Process process = Process.GetProcessById(pid);
                        string processName = process.ProcessName;
                        process.Kill();
                        Console.SetCursorPosition(0, Console.WindowHeight - 2);
                        Console.Write(new string(' ', Console.WindowWidth));
                        Console.SetCursorPosition(0, Console.WindowHeight - 2);
                        Console.WriteLine($"Process {pid} ({processName}) terminated successfully.");
                    }
                    else
                    {
                        Console.SetCursorPosition(0, Console.WindowHeight - 2);
                        Console.Write(new string(' ', Console.WindowWidth));
                        Console.SetCursorPosition(0, Console.WindowHeight - 2);
                        Console.WriteLine("Invalid process ID.");
                    }
                }
                catch (Exception ex)
                {
                    Console.SetCursorPosition(0, Console.WindowHeight - 2);
                    Console.Write(new string(' ', Console.WindowWidth));
                    Console.SetCursorPosition(0, Console.WindowHeight - 2);
                    Console.WriteLine($"Error: {ex.Message}");
                }
                finally
                {
                    // Clear input line and resume timer
                    Console.SetCursorPosition(0, Console.WindowHeight - 3);
                    Console.Write(new string(' ', Console.WindowWidth));
                    _refreshTimer.Change(0, 2000);
                }
            }
        }

        private void ShowProcessDetails()
        {
            lock (_lockObject)
            {
                // Pause timer to avoid screen corruption
                _refreshTimer.Change(Timeout.Infinite, Timeout.Infinite);

                try
                {
                    // Get process ID from user
                    Console.SetCursorPosition(0, Console.WindowHeight - 3);
                    Console.Write(new string(' ', Console.WindowWidth));
                    Console.SetCursorPosition(0, Console.WindowHeight - 3);
                    Console.Write("Enter PID for details: ");
                    string input = Console.ReadLine();
                    
                    if (int.TryParse(input, out int pid))
                    {
                        // Clear screen for details
                        Console.Clear();
                        Process process = Process.GetProcessById(pid);
                        
                        // Display detailed info
                        Console.WriteLine($"Process Details - {process.ProcessName} (PID: {process.Id})");
                        Console.WriteLine("-----------------------------------------------------------------------");
                        Console.WriteLine($"Memory Usage: {process.WorkingSet64 / 1024 / 1024} MB");
                        Console.WriteLine($"Start Time: {(process.StartTime.Year > 1900 ? process.StartTime.ToString() : "N/A")}");
                        Console.WriteLine($"Threads: {process.Threads.Count}");
                        Console.WriteLine($"Priority: {process.BasePriority}");
                        Console.WriteLine($"Window Title: {process.MainWindowTitle}");
                        Console.WriteLine($"CPU Time: {process.TotalProcessorTime}");
                        
                        try
                        {
                            Console.WriteLine($"Path: {process.MainModule?.FileName}");
                        }
                        catch
                        {
                            Console.WriteLine("Path: Access denied");
                        }
                        
                        Console.WriteLine("\nModules:");
                        try
                        {
                            foreach (ProcessModule module in process.Modules)
                            {
                                try
                                {
                                    Console.WriteLine($"  - {module.ModuleName} ({module.FileVersionInfo.FileVersion})");
                                }
                                catch
                                {
                                    Console.WriteLine($"  - {module.ModuleName} (version unknown)");
                                }
                            }
                        }
                        catch
                        {
                            Console.WriteLine("  Unable to access modules (insufficient permissions)");
                        }
                        
                        Console.WriteLine("\nPress any key to return to Task Manager...");
                        Console.ReadKey(true);
                        
                        // Redraw task manager
                        Console.Clear();
                        Console.WriteLine("Simple Task Manager - Press ESC to exit, K to kill a process, D for details");
                        Console.WriteLine("-----------------------------------------------------------------------");
                    }
                    else
                    {
                        Console.SetCursorPosition(0, Console.WindowHeight - 2);
                        Console.Write(new string(' ', Console.WindowWidth));
                        Console.SetCursorPosition(0, Console.WindowHeight - 2);
                        Console.WriteLine("Invalid process ID.");
                    }
                }
                catch (Exception ex)
                {
                    Console.SetCursorPosition(0, Console.WindowHeight - 2);
                    Console.Write(new string(' ', Console.WindowWidth));
                    Console.SetCursorPosition(0, Console.WindowHeight - 2);
                    Console.WriteLine($"Error: {ex.Message}");
                }
                finally
                {
                    // Clear input line and resume timer
                    Console.SetCursorPosition(0, Console.WindowHeight - 3);
                    Console.Write(new string(' ', Console.WindowWidth));
                    _refreshTimer.Change(0, 2000);
                }
            }
        }
    }
}