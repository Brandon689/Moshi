using System.Net.Http;
using System.Windows;

namespace Moshi.Explorer;

public partial class MainWindow : Window
{
    HttpClient client = new();
    public MainWindow()
    {
        InitializeComponent();
    }

    private async void Button_Click(object sender, RoutedEventArgs e)
    {
        var a = await client.GetStringAsync("https://localhost:7052/api/users");
        shelied.Text = a;
    }
}