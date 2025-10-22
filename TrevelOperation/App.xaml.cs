using System.Diagnostics;
using System.IO;
using System.Windows;
using static TrevelOperation.Startup;

namespace TrevelOperation;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public App()
    {
        Init();
        InitializeComponent();
    }
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
#if DEBUG
        if (JobManager.CreateJob())
        {
            ViteJob.StartViteDevServer(); 
        }
        else
        {
            MessageBox.Show("Failed to create Job Object. Vite process might not be auto-terminated if the app is forcefully stopped.",
                            "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            ViteJob.StartViteDevServer();
        }
#endif
    }

    protected override void OnExit(ExitEventArgs e)
    {
#if DEBUG
        ViteJob.AttemptGracefulViteShutdown();
        JobManager.CloseJob();
#endif
        base.OnExit(e);
    }

 
}