using GalaSoft.MvvmLight.Command;
using MahApps.Metro;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace R54IN0.WPF
{
    public class AccentColorMenuData
    {
        public AccentColorMenuData()
        {
            ChangeAccentCommand = new RelayCommand(ExecuteChangeTheme);
        }

        public string Name { get; set; }
        public Brush BorderColorBrush { get; set; }
        public Brush ColorBrush { get; set; }

        public ICommand ChangeAccentCommand
        {
            get; set;
        }

        protected virtual void ExecuteChangeTheme()
        {
            var theme = ThemeManager.DetectAppStyle(Application.Current);
            var accent = ThemeManager.GetAccent(Name);
            ThemeManager.ChangeAppStyle(Application.Current, accent, theme.Item1);
        }
    }

    public class AppThemeMenuData : AccentColorMenuData
    {
        protected override void ExecuteChangeTheme()
        {
            var theme = ThemeManager.DetectAppStyle(Application.Current);
            var appTheme = ThemeManager.GetAppTheme(Name);
            ThemeManager.ChangeAppStyle(Application.Current, theme.Item2, appTheme);
        }
    }
}