using System.Windows;

namespace Moshi.WPF;
public partial class App : Application
{
    protected override void OnExit(ExitEventArgs e)
    {
        if (MainWindow is MainWindow mainWindow)
        {
            mainWindow.Log("Application OnExit called");
            if (mainWindow is IDisposable disposable)
            {
                try
                {
                    disposable.Dispose();
                    mainWindow.Log("MainWindow disposed successfully");
                }
                catch (Exception ex)
                {
                    mainWindow.Log($"Error disposing MainWindow: {ex.Message}");
                }
            }
        }
        base.OnExit(e);
    }
}
