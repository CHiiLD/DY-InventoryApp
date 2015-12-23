using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using MahApps.Metro;

namespace R54IN0.WPF
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            FinderDirector.Distroy();
            DatabaseDirector.Distroy();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            //Accent accent = ThemeManager.GetAccent("Lime");
            //Tuple<AppTheme, Accent> appStyle = ThemeManager.DetectAppStyle(Application.Current);
            //ThemeManager.ChangeAppStyle(Application.Current, accent, appStyle.Item1);
            //ThemeManager.ChangeAppTheme(Application.Current, "BaseDark");
#if DEBUG
            new R54IN0.Test.DummyDbData().Create();
#endif
        }
    }
}
