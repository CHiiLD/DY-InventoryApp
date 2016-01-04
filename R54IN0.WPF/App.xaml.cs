using System.Windows;

namespace R54IN0.WPF
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            TreeViewNodeDirector.Distroy();
            LexDb.Distroy();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            //Accent accent = ThemeManager.GetAccent("Pink");
            //Tuple<AppTheme, Accent> appStyle = ThemeManager.DetectAppStyle(Application.Current);
            //ThemeManager.ChangeAppStyle(Application.Current, accent, appStyle.Item1);
            //ThemeManager.ChangeAppTheme(Application.Current, "BaseDark");

            //SwatchesProvider swatchesProvider = new SwatchesProvider();
            //Swatch swatchPrimary = swatchesProvider.Swatches.Where(x => x.Name == "lightblue").SingleOrDefault();
            //Swatch swatchAccent = swatchesProvider.Swatches.Where(x => x.Name == "lightblue").SingleOrDefault();
            //PaletteHelper paletteHelper = new PaletteHelper();
            //paletteHelper.ReplacePrimaryColor(swatchPrimary);
            //paletteHelper.ReplaceAccentColor(swatchAccent);
#if DEBUG
            //new R54IN0.Test.DummyDbData().Create();
#endif
        }
    }
}