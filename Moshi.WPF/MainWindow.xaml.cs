﻿//using Moshi.Shared;
//using System;
//using System.Collections.ObjectModel;
//using System.ComponentModel;
//using System.Diagnostics;
//using System.IO;
//using System.Linq;
//using System.Threading.Tasks;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Navigation;

//namespace Moshi.WPF
//{
//    public partial class MainWindow : Window, IDisposable
//    {
//        public ObservableCollection<Project> ApiProjects { get; set; }
//        public ObservableCollection<Project> FrontendProjects { get; set; }
//        public ObservableCollection<Project> DemoProjects { get; set; }
//        private bool _isClosing = false;

//        public MainWindow()
//        {
//            InitializeComponent();
//            ApiProjects = new ObservableCollection<Project>();
//            FrontendProjects = new ObservableCollection<Project>();
//            DemoProjects = new ObservableCollection<Project>();

//            ApiListView.ItemsSource = ApiProjects;
//            FrontendListView.ItemsSource = FrontendProjects;
//            DemoListView.ItemsSource = DemoProjects;

//            LoadProjects(SolutionPathFinder.GetSolutionDirectoryPath());
//        }

//        private void LoadProjects(string solutionPath)
//        {
//            // Load API projects
//            ApiProjects.Add(new Project { Name = "Moshi.Blog", Path = Path.Combine(solutionPath, "Moshi.Blog"), IsApi = true, HasSwagger = true });
//            ApiProjects.Add(new Project { Name = "ProductCatalog", Path = Path.Combine(solutionPath, "ProductCatalog"), IsApi = true, HasSwagger = true });
//            // Add more API projects as needed

//            // Load Frontend projects
//            string frontendsPath = Path.Combine(solutionPath, "Frontends");
//            foreach (var directory in Directory.GetDirectories(frontendsPath))
//            {
//                FrontendProjects.Add(new Project { Name = new DirectoryInfo(directory).Name, Path = directory, IsFrontend = true });
//            }

//            // Load Demo projects
//            foreach (var directory in Directory.GetDirectories(solutionPath))
//            {
//                if (directory.Contains("Demo"))
//                {
//                    DemoProjects.Add(new Project { Name = new DirectoryInfo(directory).Name, Path = directory, IsDemo = true });
//                }
//            }
//        }

//        private async void StartProject_Click(object sender, RoutedEventArgs e)
//        {
//            await StartProject((sender as Button)?.DataContext as Project);
//        }

//        private async void StopProject_Click(object sender, RoutedEventArgs e)
//        {
//            await StopProject((sender as Button)?.DataContext as Project);
//        }

//        private async void StartFrontend_Click(object sender, RoutedEventArgs e)
//        {
//            await StartFrontend((sender as Button)?.DataContext as Project);
//        }

//        private async void StopFrontend_Click(object sender, RoutedEventArgs e)
//        {
//            await StopProject((sender as Button)?.DataContext as Project);
//        }

//        private async void StartDemo_Click(object sender, RoutedEventArgs e)
//        {
//            await StartProject((sender as Button)?.DataContext as Project);
//        }

//        private async void StopDemo_Click(object sender, RoutedEventArgs e)
//        {
//            await StopProject((sender as Button)?.DataContext as Project);
//        }

//        private async Task StartProject(Project project)
//        {
//            if (project != null && !project.IsRunning)
//            {
//                try
//                {
//                    Log($"Starting project: {project.Name}");
//                    var startInfo = new ProcessStartInfo
//                    {
//                        FileName = "dotnet",
//                        Arguments = $"run --project \"{project.Path}\"",
//                        UseShellExecute = false,
//                        RedirectStandardOutput = true,
//                        RedirectStandardError = true,
//                        CreateNoWindow = true
//                    };

//                    project.Process = new Process { StartInfo = startInfo };
//                    project.Process.Start();
//                    project.IsRunning = true;
//                    project.CancellationTokenSource = new CancellationTokenSource();

//                    // Start reading logs
//                    await Task.WhenAll(
//                        ReadProcessOutputAsync(project, project.Process.StandardOutput),
//                        ReadProcessOutputAsync(project, project.Process.StandardError, isError: true)
//                    );

//                    // Process has exited or been cancelled
//                    Log($"Project {project.Name} has stopped or been cancelled");
//                    project.IsRunning = false;
//                    project.SwaggerUrl = null;
//                    project.CancellationTokenSource = null;
//                }
//                catch (Exception ex)
//                {
//                    Log($"Error starting project {project.Name}: {ex.Message}");
//                    MessageBox.Show($"Error starting project: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
//                }
//                finally
//                {
//                    // Update UI
//                    await Dispatcher.InvokeAsync(() =>
//                    {
//                        ApiListView.Items.Refresh();
//                        FrontendListView.Items.Refresh();
//                        DemoListView.Items.Refresh();
//                    });
//                }
//            }
//        }

//        private async Task StartFrontend(Project project)
//        {

//            project = new Project();
//            project.Path = "C:\\2024\\h\\Moshi\\Frontends\\moshi.blog.react\\";
//            var startInfo = new ProcessStartInfo
//            {
//                FileName = "cmd.exe",
//                Arguments = "/c npm run dev",
//                WorkingDirectory = project.Path,
//                UseShellExecute = false,
//                RedirectStandardOutput = true,
//                RedirectStandardError = true,
//                CreateNoWindow = false
//            };


//            // Create and assign the Process object
//            project.Process = new Process { StartInfo = startInfo };

//            // Set up output and error handling
//            project.Process.OutputDataReceived += (sender, e) => {
//                if (e.Data != null)
//                    Console.WriteLine(e.Data);
//            };
//            project.Process.ErrorDataReceived += (sender, e) => {
//                if (e.Data != null)
//                    Console.WriteLine(e.Data);
//            };

//            // Start the process
//            project.Process.Start();
//            project.Process.BeginOutputReadLine();
//            project.Process.BeginErrorReadLine();

//            project.IsRunning = true;
//            project.CancellationTokenSource = new CancellationTokenSource();

//            // Wait for the process to exit
//            project.Process.WaitForExit();

//            //if (project != null && !project.IsRunning)
//            //{
//            //    try
//            //    {
//            //        Log($"Starting frontend: {project.Name}");
//            //        var startInfo = new ProcessStartInfo
//            //        {
//            //            FileName = "cmd.exe",
//            //            Arguments = "/c npm run dev",
//            //            WorkingDirectory = project.Path,
//            //            UseShellExecute = false,
//            //            RedirectStandardOutput = true,
//            //            RedirectStandardError = true,
//            //            CreateNoWindow = false
//            //        };

//            //        project.Process = new Process { StartInfo = startInfo };
//            //        project.Process.Start();
//            //        project.IsRunning = true;
//            //        project.CancellationTokenSource = new CancellationTokenSource();

//            //        // Start reading logs
//            //        await Task.WhenAll(
//            //            ReadProcessOutputAsync(project, project.Process.StandardOutput),
//            //            ReadProcessOutputAsync(project, project.Process.StandardError, isError: true)
//            //        );

//            //        // Process has exited or been cancelled
//            //        Log($"Frontend {project.Name} has stopped or been cancelled");
//            //        project.IsRunning = false;
//            //        project.CancellationTokenSource = null;
//            //    }
//            //    catch (Exception ex)
//            //    {
//            //        Log($"Error starting frontend {project.Name}: {ex.Message}");
//            //        MessageBox.Show($"Error starting frontend: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
//            //    }
//            //    finally
//            //    {
//            //        // Update UI
//            //        await Dispatcher.InvokeAsync(() => FrontendListView.Items.Refresh());
//            //    }
//            //}
//        }

//        private async Task StopProject(Project project)
//        {
//            if (project != null && project.IsRunning)
//            {
//                try
//                {
//                    // Cancel the ongoing tasks
//                    project.CancellationTokenSource?.Cancel();

//                    await Task.Run(() =>
//                    {
//                        // Try to stop the process gracefully
//                        project.Process.CloseMainWindow();

//                        // Wait for the process to exit, but with a timeout
//                        if (!project.Process.WaitForExit(5000))  // 5 second timeout
//                        {
//                            // If the process hasn't exited after 5 seconds, force kill it
//                            project.Process.Kill(entireProcessTree: true);
//                        }
//                    });

//                    project.IsRunning = false;
//                    project.SwaggerUrl = null;

//                    // Dispose of the Process object
//                    project.Process.Dispose();
//                    project.Process = null;

//                    // Dispose of the CancellationTokenSource
//                    project.CancellationTokenSource?.Dispose();
//                    project.CancellationTokenSource = null;

//                    // Clear the log for this project
//                    LogTextBox.Clear();
//                }
//                catch (Exception ex)
//                {
//                    Log($"Error stopping project: {ex.Message}");
//                    MessageBox.Show($"Error stopping project: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
//                }
//                finally
//                {
//                    // Update UI
//                    await Dispatcher.InvokeAsync(() =>
//                    {
//                        ApiListView.Items.Refresh();
//                        FrontendListView.Items.Refresh();
//                        DemoListView.Items.Refresh();
//                    });
//                }
//            }
//        }

//        private async Task ReadProcessOutputAsync(Project project, StreamReader reader, bool isError = false)
//        {
//            try
//            {
//                while (!reader.EndOfStream)
//                {
//                    if (project.CancellationTokenSource.Token.IsCancellationRequested)
//                    {
//                        Log($"Output reading cancelled for project: {project.Name}");
//                        break;
//                    }

//                    var line = await reader.ReadLineAsync();
//                    await Dispatcher.InvokeAsync(() =>
//                    {
//                        string logLine = isError ? $"ERROR: {line}" : line;
//                        LogTextBox.AppendText(logLine + "\n");

//                        // Check for the line that shows the application URL (for API projects)
//                        if (!isError && project.IsApi && line != null && line.Contains("Now listening on:"))
//                        {
//                            var url = line.Split(": ")[1].Trim();
//                            project.SwaggerUrl = project.HasSwagger ? $"{url}/swagger/index.html" : url;
//                            // Trigger UI update
//                            ApiListView.Items.Refresh();
//                        }
//                    });
//                }
//            }
//            catch (OperationCanceledException)
//            {
//                Log($"Output reading task cancelled for project: {project.Name}");
//            }
//            catch (Exception ex)
//            {
//                Log($"Error reading {(isError ? "error " : "")}output for project {project.Name}: {ex.Message}");
//            }
//        }

//        private async Task StopAllProjectsAsync()
//        {
//            Log("Starting to stop all projects");
//            var allProjects = ApiProjects.Concat(FrontendProjects).Concat(DemoProjects);
//            var tasks = allProjects.Where(p => p.IsRunning).Select(async project =>
//            {
//                try
//                {
//                    Log($"Attempting to stop project: {project.Name}");
//                    project.CancellationTokenSource?.Cancel();

//                    if (project.Process != null && !project.Process.HasExited)
//                    {
//                        Log($"Closing main window for project: {project.Name}");
//                        project.Process.CloseMainWindow();
//                        await Task.Run(() =>
//                        {
//                            Log($"Waiting for project to exit: {project.Name}");
//                            if (!project.Process.WaitForExit(5000))
//                            {
//                                Log($"Project did not exit gracefully, attempting to kill: {project.Name}");
//                                try
//                                {
//                                    project.Process.Kill(entireProcessTree: true);
//                                }
//                                catch (Exception ex)
//                                {
//                                    Log($"Error killing project {project.Name}: {ex.Message}");
//                                }
//                            }
//                        });
//                    }
//                    Log($"Successfully stopped project: {project.Name}");
//                }
//                catch (Exception ex)
//                {
//                    Log($"Error stopping project {project.Name}: {ex.Message}");
//                }
//                finally
//                {
//                    project.Process?.Dispose();
//                    project.CancellationTokenSource?.Dispose();
//                    project.IsRunning = false;
//                }
//            });

//            await Task.WhenAll(tasks);
//            Log("Finished stopping all projects");
//        }

//        protected override async void OnClosing(CancelEventArgs e)
//        {
//            Log("OnClosing method called");

//            if (!_isClosing)
//            {
//                _isClosing = true;
//                e.Cancel = true; // Temporarily cancel the closing

//                var result = MessageBox.Show("Are you sure you want to exit? All running projects will be stopped.", "Confirm Exit", MessageBoxButton.YesNo, MessageBoxImage.Question);

//                if (result == MessageBoxResult.Yes)
//                {
//                    Log("User confirmed exit");
//                    try
//                    {
//                        await StopAllProjectsAsync();
//                        Log("All projects stopped successfully");
//                    }
//                    catch (Exception ex)
//                    {
//                        Log($"Error during StopAllProjectsAsync: {ex.Message}");
//                    }
//                    finally
//                    {
//                        Application.Current.Shutdown();
//                    }
//                }
//                else
//                {
//                    Log("User cancelled exit");
//                    _isClosing = false;
//                }
//            }
//        }

//        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
//        {
//            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
//            e.Handled = true;
//        }

//        public void Dispose()
//        {
//            Log("Dispose method called");
//            try
//            {
//                StopAllProjectsAsync().GetAwaiter().GetResult();
//                Log("StopAllProjectsAsync completed in Dispose method");
//            }
//            catch (Exception ex)
//            {
//                Log($"Error in Dispose method: {ex.Message}");
//            }
//        }

//        public void Log(string message)
//        {
//            string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";
//            Console.WriteLine(logMessage);
//            Dispatcher.Invoke(() => LogTextBox.AppendText(logMessage + Environment.NewLine));
//        }
//    }

//    public class Project : IDisposable
//    {
//        public string Name { get; set; }
//        public string Path { get; set; }
//        public bool IsRunning { get; set; }
//        public Process Process { get; set; }
//        public string SwaggerUrl { get; set; }
//        public CancellationTokenSource CancellationTokenSource { get; set; }
//        public bool IsApi { get; set; }
//        public bool IsFrontend { get; set; }
//        public bool IsDemo { get; set; }
//        public bool HasSwagger { get; set; }

//        public void Dispose()
//        {
//            CancellationTokenSource?.Cancel();
//            CancellationTokenSource?.Dispose();
//            if (Process != null && !Process.HasExited)
//            {
//                try
//                {
//                    Process.CloseMainWindow();
//                    if (!Process.WaitForExit(5000))
//                    {
//                        Process.Kill(entireProcessTree: true);
//                    }
//                }
//                catch (InvalidOperationException)
//                {
//                    // Process has already exited
//                }
//                finally
//                {
//                    Process.Dispose();
//                }
//            }
//        }
//    }
//}



using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
namespace Moshi.WPF;

public class Project : IDisposable
{
    public string Name { get; set; }
    public string Path { get; set; }
    public bool IsRunning { get; set; }
    public Process Process { get; set; }
    public string SwaggerUrl { get; set; }
    public CancellationTokenSource CancellationTokenSource { get; set; }

    public void Dispose()
    {
        CancellationTokenSource?.Cancel();
        CancellationTokenSource?.Dispose();
        if (Process != null && !Process.HasExited)
        {
            try
            {
                Process.CloseMainWindow();
                if (!Process.WaitForExit(5000))
                {
                    Process.Kill(entireProcessTree: true);
                }
            }
            catch (InvalidOperationException)
            {
                // Process has already exited
            }
            finally
            {
                Process.Dispose();
            }
        }
    }
}




public partial class MainWindow : Window, IDisposable
{
    public ObservableCollection<Project> Projects { get; set; }
    private bool _isClosing = false;


    public MainWindow()
    {
        InitializeComponent();
        Projects = new ObservableCollection<Project>();
        ProjectListView.ItemsSource = Projects;
        LoadProjects(@"C:\2024\h\Moshi");
    }
    public void Dispose()
    {
        Log("Dispose method called");
        try
        {
            StopAllProjectsAsync().GetAwaiter().GetResult();
            Log("StopAllProjectsAsync completed in Dispose method");
        }
        catch (Exception ex)
        {
            Log($"Error in Dispose method: {ex.Message}");
        }
    }

    public void Log(string message)
    {
        string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";
        Console.WriteLine(logMessage);
        Dispatcher.Invoke(() => LogTextBox.AppendText(logMessage + Environment.NewLine));
    }

    private void LoadProjects(string directoryPath)
    {
        foreach (var directory in Directory.GetDirectories(directoryPath))
        {
            if (File.Exists(Path.Combine(directory, "Program.cs")))
            {
                Projects.Add(new Project
                {
                    Name = new DirectoryInfo(directory).Name,
                    Path = directory,
                    IsRunning = false
                });
            }
        }
    }

    private async void StartProject_Click(object sender, RoutedEventArgs e)
    {
        var button = sender as Button;
        if (button != null)
        {
            button.IsEnabled = false;  // Disable the button to prevent multiple clicks
        }

        var project = button?.DataContext as Project;
        if (project != null && !project.IsRunning)
        {
            try
            {
                Log($"Starting project: {project.Name}");
                var startInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = $"run --project \"{project.Path}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                project.Process = new Process { StartInfo = startInfo };
                project.Process.Start();
                project.IsRunning = true;
                project.CancellationTokenSource = new CancellationTokenSource();

                // Start reading logs
                var outputTask = Task.Run(async () =>
                {
                    try
                    {
                        using (var reader = project.Process.StandardOutput)
                        {
                            while (!reader.EndOfStream)
                            {
                                if (project.CancellationTokenSource.Token.IsCancellationRequested)
                                {
                                    Log($"Output reading cancelled for project: {project.Name}");
                                    break;
                                }

                                var line = await reader.ReadLineAsync();
                                await Dispatcher.InvokeAsync(() =>
                                {
                                    LogTextBox.AppendText(line + "\n");

                                    // Check for the line that shows the application URL
                                    if (line != null && line.Contains("Now listening on:"))
                                    {
                                        var url = line.Split(": ")[1].Trim();
                                        project.SwaggerUrl = $"{url}/swagger/index.html";
                                        // Trigger UI update
                                        ProjectListView.Items.Refresh();
                                    }
                                });
                            }
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        Log($"Output reading task cancelled for project: {project.Name}");
                    }
                    catch (Exception ex)
                    {
                        Log($"Error reading output for project {project.Name}: {ex.Message}");
                    }
                }, project.CancellationTokenSource.Token);

                // Also handle standard error
                var errorTask = Task.Run(async () =>
                {
                    try
                    {
                        using (var reader = project.Process.StandardError)
                        {
                            while (!reader.EndOfStream)
                            {
                                if (project.CancellationTokenSource.Token.IsCancellationRequested)
                                {
                                    Log($"Error reading cancelled for project: {project.Name}");
                                    break;
                                }

                                var line = await reader.ReadLineAsync();
                                await Dispatcher.InvokeAsync(() =>
                                {
                                    LogTextBox.AppendText("ERROR: " + line + "\n");
                                });
                            }
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        Log($"Error reading task cancelled for project: {project.Name}");
                    }
                    catch (Exception ex)
                    {
                        Log($"Error reading error output for project {project.Name}: {ex.Message}");
                    }
                }, project.CancellationTokenSource.Token);

                // Wait for the process to exit or cancellation
                await Task.Run(async () =>
                {
                    try
                    {
                        await Task.WhenAny(Task.Run(() => project.Process.WaitForExit()), Task.Delay(-1, project.CancellationTokenSource.Token));
                    }
                    catch (OperationCanceledException)
                    {
                        Log($"Process waiting task cancelled for project: {project.Name}");
                    }
                });

                // Process has exited or been cancelled
                Log($"Project {project.Name} has stopped or been cancelled");
                project.IsRunning = false;
                project.SwaggerUrl = null;
                project.CancellationTokenSource = null;
            }
            catch (Exception ex)
            {
                Log($"Error starting project {project.Name}: {ex.Message}");
                MessageBox.Show($"Error starting project: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                // Update UI
                await Dispatcher.InvokeAsync(() =>
                {
                    ProjectListView.Items.Refresh();
                    if (button != null)
                    {
                        button.IsEnabled = true;  // Re-enable the button
                    }
                });
            }
        }
    }



    private async void StopProject_Click(object sender, RoutedEventArgs e)
    {
        var button = sender as Button;
        if (button != null)
        {
            button.IsEnabled = false;  // Disable the button to prevent multiple clicks
        }

        var project = button?.DataContext as Project;
        if (project != null && project.IsRunning)
        {
            try
            {
                // Cancel the ongoing tasks
                project.CancellationTokenSource?.Cancel();

                await Task.Run(() =>
                {
                    // Try to stop the process gracefully
                    project.Process.CloseMainWindow();

                    // Wait for the process to exit, but with a timeout
                    if (!project.Process.WaitForExit(5000))  // 5 second timeout
                    {
                        // If the process hasn't exited after 5 seconds, force kill it
                        project.Process.Kill(entireProcessTree: true);
                    }
                });

                project.IsRunning = false;
                project.SwaggerUrl = null;

                // Dispose of the Process object
                project.Process.Dispose();
                project.Process = null;

                // Dispose of the CancellationTokenSource
                project.CancellationTokenSource?.Dispose();
                project.CancellationTokenSource = null;

                // Clear the log for this project
                LogTextBox.Clear();
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error stopping project: {ex.Message}");
                MessageBox.Show($"Error stopping project: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                // Update UI
                await Dispatcher.InvokeAsync(() =>
                {
                    ProjectListView.Items.Refresh();
                    if (button != null)
                    {
                        button.IsEnabled = true;  // Re-enable the button
                    }
                });
            }
        }
    }

    private async Task StopAllProjectsAsync()
    {
        Log("Starting to stop all projects");
        var tasks = Projects.Where(p => p.IsRunning).Select(async project =>
        {
            try
            {
                Log($"Attempting to stop project: {project.Name}");
                project.CancellationTokenSource?.Cancel();

                if (project.Process != null && !project.Process.HasExited)
                {
                    Log($"Closing main window for project: {project.Name}");
                    project.Process.CloseMainWindow();
                    await Task.Run(() =>
                    {
                        Log($"Waiting for project to exit: {project.Name}");
                        if (!project.Process.WaitForExit(5000))
                        {
                            Log($"Project did not exit gracefully, attempting to kill: {project.Name}");
                            try
                            {
                                project.Process.Kill(entireProcessTree: true);
                            }
                            catch (Exception ex)
                            {
                                Log($"Error killing project {project.Name}: {ex.Message}");
                            }
                        }
                    });
                }
                Log($"Successfully stopped project: {project.Name}");
            }
            catch (Exception ex)
            {
                Log($"Error stopping project {project.Name}: {ex.Message}");
            }
            finally
            {
                project.Process?.Dispose();
                project.CancellationTokenSource?.Dispose();
                project.IsRunning = false;
            }
        });

        await Task.WhenAll(tasks);
        Log("Finished stopping all projects");
    }


    protected override async void OnClosing(CancelEventArgs e)
    {
        Log("OnClosing method called");

        // If we're already in the process of closing, don't show the dialog again
        if (!_isClosing)
        {
            _isClosing = true;
            e.Cancel = true; // Temporarily cancel the closing

            var result = MessageBox.Show("Are you sure you want to exit? All running projects will be stopped.", "Confirm Exit", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Log("User confirmed exit");
                try
                {
                    await StopAllProjectsAsync();
                    Log("All projects stopped successfully");
                }
                catch (Exception ex)
                {
                    Log($"Error during StopAllProjectsAsync: {ex.Message}");
                }
                finally
                {
                    // Ensure the application closes
                    Application.Current.Shutdown();
                }
            }
            else
            {
                Log("User cancelled exit");
                _isClosing = false;
            }
        }
    }




    private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
        Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
        e.Handled = true;
    }
}
