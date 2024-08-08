using Microsoft.Web.WebView2.Core;
using System.IO;
using System.Windows;

namespace Moshi.FrontendShell;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        InitializeAsync();
    }

    async void InitializeAsync()
    {
        await webView.EnsureCoreWebView2Async(null);
        SetupWebView();
    }

    void SetupWebView()
    {
        string buildDir = "C:/2024/h/Moshi/Frontends/moshi.blog.react/dist/";

        if (File.Exists(Path.Combine(buildDir, "index.html")))
        {
            webView.CoreWebView2.SetVirtualHostNameToFolderMapping("vite.react.app", buildDir, CoreWebView2HostResourceAccessKind.Allow);

            // Basic security settings
            webView.CoreWebView2.Settings.IsScriptEnabled = true;
            webView.CoreWebView2.Settings.AreDefaultScriptDialogsEnabled = true;
            webView.CoreWebView2.Settings.IsWebMessageEnabled = true;

            webView.CoreWebView2.Navigate("http://vite.react.app/index.html");
        }
        else
        {
            MessageBox.Show("Vite-React app not found. Ensure the build files are in the correct location.");
        }
    }
}