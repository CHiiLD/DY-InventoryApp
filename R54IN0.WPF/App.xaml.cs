﻿using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;

namespace R54IN0.WPF
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        protected async override void OnExit(ExitEventArgs e)
        {
            TreeViewNodeDirector.Destroy();
            await DbAdapter.GetInstance().DisconnectAsync();
            base.OnExit(e);
        }

        protected async override void OnStartup(StartupEventArgs e)
        {
            Process thisProc = Process.GetCurrentProcess();
            if (Process.GetProcessesByName(thisProc.ProcessName).Length > 1)
            {
                MessageBox.Show("이미 실행중입니다");
                Application.Current.Shutdown();
                return;
            }
            base.OnStartup(e);
            await DbAdapter.GetInstance().ConnectAsync();
        }
    }
}