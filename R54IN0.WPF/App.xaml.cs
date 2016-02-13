using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;

namespace R54IN0.WPF
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        protected override void OnExit(ExitEventArgs e)
        {
            TreeViewNodeDirector.Destroy();
            InventoryDataCommander.Destroy();
            base.OnExit(e);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            Process thisProc = Process.GetCurrentProcess();
            if (Process.GetProcessesByName(thisProc.ProcessName).Length > 1)
            {
                MessageBox.Show("이미 실행중입니다");
                Application.Current.Shutdown();
                return;
            }
            AttachConsole(-1);
            base.OnStartup(e);
        }

        [DllImport("Kernel32.dll")]
        public static extern bool AttachConsole(int processId);
    }
}