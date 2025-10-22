using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TrevelOperation
{
   internal static class ViteJob
    {
        private static Process? _viteProcess;

        public static void StartViteDevServer()
        {
            string projectDirectory = string.Empty;
            try
            {
                string executablePath = AppDomain.CurrentDomain.BaseDirectory;
                DirectoryInfo di = new DirectoryInfo(executablePath); 
                projectDirectory = Directory.GetParent(Path.GetFullPath(Path.Combine(di.FullName, "..", "..", ".."))).FullName;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error determining project directory: {ex.Message}", "Directory Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string? clientAppPath = Directory.EnumerateDirectories(projectDirectory, "*RazorLib", SearchOption.TopDirectoryOnly)
                                               .FirstOrDefault();
      
            if (!Directory.Exists(clientAppPath))
            {
                MessageBox.Show($"Vite project directory not found at: {clientAppPath}\nPlease check the path.", "Vite Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!File.Exists(Path.Combine(clientAppPath, "package.json")))
            {
                MessageBox.Show($"package.json not found in: {clientAppPath}\nCannot start Vite server.", "Vite Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "/c npm run dev",
                WorkingDirectory = clientAppPath,
                CreateNoWindow = false, 
                UseShellExecute = true, 
            };

            try
            {
                _viteProcess = Process.Start(processStartInfo);
                if (_viteProcess != null && !_viteProcess.HasExited)
                {
                    Console.WriteLine($"Vite (cmd.exe) process started with ID: {_viteProcess.Id}");

                    if (!JobManager.AssignProcess(_viteProcess))
                    {
                        Console.WriteLine($"Warning: Failed to assign Vite (cmd.exe pid: {_viteProcess.Id}) process to Job Object. It might not auto-terminate if the app is killed.");
                    }
                }
                else
                {
                    MessageBox.Show("Failed to start Vite process or it exited immediately.", "Vite Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    _viteProcess = null; // Ensure it's null if failed
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting Vite dev server: {ex.Message}\n\nWorking Directory: {clientAppPath}\nCommand: npm run dev", "Vite Startup Error", MessageBoxButton.OK, MessageBoxImage.Error);
                _viteProcess = null;
            }
        }

        public static void AttemptGracefulViteShutdown()
        {
            if (_viteProcess != null && !_viteProcess.HasExited)
            {
                Console.WriteLine($"Attempting graceful shutdown of Vite (cmd.exe pid: {_viteProcess.Id}) process.");
                try
                {
                    // Use taskkill to terminate the entire process tree started by cmd.exe
                    ProcessStartInfo killInfo = new ProcessStartInfo("taskkill", $"/PID {_viteProcess.Id} /F /T")
                    {
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    };
                    using (Process killer = Process.Start(killInfo))
                    {
                        if (killer == null)
                        {
                            Console.WriteLine("Failed to start taskkill process.");
                            return;
                        }
                        string output = killer.StandardOutput.ReadToEnd();
                        string error = killer.StandardError.ReadToEnd();
                        killer.WaitForExit(5000); // Wait up to 5 seconds

                        if (!string.IsNullOrWhiteSpace(output)) Console.WriteLine($"Taskkill output: {output.Trim()}");
                        if (!string.IsNullOrWhiteSpace(error)) Console.WriteLine($"Taskkill error: {error.Trim()}");

                        if (killer.ExitCode != 0 && !_viteProcess.HasExited) // If taskkill reported an error and process still there
                        {
                            Console.WriteLine($"Taskkill failed or Vite process (cmd.exe pid: {_viteProcess.Id}) still running. Trying direct kill.");
                            // _viteProcess.Kill(true); // For .NET 5+ to kill entire tree if taskkill fails
                            // For older .NET, or if above is not available/effective for the cmd tree:
                            if (!_viteProcess.HasExited) _viteProcess.Kill();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception during Vite process taskkill: {ex.Message}");
                    // Final attempt if process still seems to exist
                    try
                    {
                        if (_viteProcess != null && !_viteProcess.HasExited)
                        {
                            _viteProcess.Kill();
                        }
                    }
                    catch (Exception killEx)
                    {
                        Console.WriteLine($"Exception during final Vite process kill: {killEx.Message}");
                    }
                }
                finally
                {
                    _viteProcess.Dispose();
                    _viteProcess = null;
                }
            }
        }
    }
}
