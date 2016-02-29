using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace R54IN0.WPF
{
    public static class DispatchService
    {
        public static void Invoke(Action action)
        {
            if (Application.Current != null)
            {
                Dispatcher dispatchObject = Application.Current.Dispatcher;
                if (dispatchObject == null || dispatchObject.CheckAccess())
                {
                    action();
                }
                else
                {
                    dispatchObject.Invoke(action);
                }
            }
            else
            {
                action();
            }
        }

        public static void Invoke<T>(Action<T> action, T arg)
        {
            if (Application.Current != null)
            {
                Dispatcher dispatchObject = Application.Current.Dispatcher;
                if (dispatchObject == null || dispatchObject.CheckAccess())
                {
                    action(arg);
                }
                else
                {
                    dispatchObject.Invoke(action, arg);
                }
            }
            else
            {
                action(arg);
            }
        }
    }
}