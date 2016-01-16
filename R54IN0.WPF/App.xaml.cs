using System.Windows;

namespace R54IN0.WPF
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        private async void Application_Exit(object sender, ExitEventArgs e)
        {
            await DbAdapter.GetInstance().DisconnectAsync();
            TreeViewNodeDirector.Destroy();
        }

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            await DbAdapter.GetInstance().ConnectAsync();
        }
    }
}