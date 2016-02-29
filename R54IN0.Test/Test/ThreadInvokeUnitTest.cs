using System;
using NUnit.Framework;
using System.Windows.Threading;
using System.Threading;
using System.Threading.Tasks;

namespace R54IN0.WPF.Test
{
    [TestFixture]
    public class ThreadInvokeUnitTest
    {
        public void Output()
        {
            Console.WriteLine("hello");
        }

        /// <summary>
        /// 동기수행
        /// </summary>
        [Test]
        public void TestInvoke()
        {
            Dispatcher.CurrentDispatcher.Invoke(Output);
            Console.WriteLine("world");

        }

        /// <summary>
        /// 비동기수행
        /// </summary>
        [Test]
        public void TestBeginInvoke()
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(Output), null);
            Console.WriteLine("world");
        }

        [Ignore]
        [Test]
        public async Task TestInvokeAsync()
        {
            await Dispatcher.CurrentDispatcher.InvokeAsync(Output);
            Console.WriteLine("world");
        }

        //쓰레드가 멈춤
        [Test]
        [Ignore]
        public async Task TestBeginInvokeAsync()
        {
            await Dispatcher.CurrentDispatcher.BeginInvoke(new Action(Output), null);
            Console.WriteLine("world");
        }
    }
}