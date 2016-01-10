using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Collections;

namespace R54IN0.Test
{
    [Ignore]
    [TestClass]
    public class TimeMeasuration
    {
        [Ignore]
        [TestMethod]
        public void DbSaveLoadDuration()
        {
            using (var db = LexDb.GetDbInstance())
            {
                db.Purge();
            }

            Stopwatch sw = new Stopwatch();
            Console.WriteLine("데이터베이스 저장 테스트 시작 : ");
            sw.Start();
            new DYDummyDbData().Create(300, 301);
            sw.Stop();
            Console.WriteLine("데이터베이스 저장 소요 시간: " + (new TimeSpan(sw.ElapsedTicks)).ToString());

            sw.Reset();
            sw.Start();
            Console.WriteLine("데이터베이스 불러오기 테스트 시작: ");
            InOutStock[] invens = null;
            using (var db = LexDb.GetDbInstance())
            {
                invens = db.LoadAll<InOutStock>();
            }
            sw.Stop();
            Console.WriteLine("데이터베이스 불러오기 소요 시간 : " + (new TimeSpan(sw.ElapsedTicks)).ToString());
            Console.WriteLine("불러온 데이터의 개수 : " + invens.Length);
        }

        void Destroy()
        {
            FieldWrapperDirector.Distroy();
            InventoryWrapperDirector.Distory();
            CollectionViewModelObserverSubject.Distory();
            FinderDirector.Distroy();
            StockWrapperDirector.Distory();
            LexDb.Distroy();
        }

        [TestMethod]
        public void CallStockWrapperDirector()
        {
            Destroy();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            StockWrapperDirector.GetInstance();
            sw.Stop();
            Console.WriteLine("StockWrapperDirector 생성 시간 : " + (new TimeSpan(sw.ElapsedTicks)).ToString());
        }

        [TestMethod]
        public void CallStockWrapperEditorViewModel()
        {
            Destroy();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            CollectionViewModelObserverSubject sub = CollectionViewModelObserverSubject.GetInstance();
            StockWrapperViewModel svm = new StockWrapperViewModel(IOStockType.ALL, sub);
            StockWrapperEditorViewModel vm = new StockWrapperEditorViewModel(svm);
            sw.Stop();
            Console.WriteLine("StockWrapperEditorViewModel 생성 시간 : " + (new TimeSpan(sw.ElapsedTicks)).ToString());
        }

        [TestMethod]
        public void CallInvenWrapperEditorViewModel()
        {
            Destroy();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            CollectionViewModelObserverSubject sub = CollectionViewModelObserverSubject.GetInstance();
            InventoryWrapperViewModel svm = new InventoryWrapperViewModel(sub);
            InventoryWrapperEditorViewModel vm = new InventoryWrapperEditorViewModel(svm);
            sw.Stop();
            Console.WriteLine("InventoryWrapperEditorViewModel 생성 시간 : " + (new TimeSpan(sw.ElapsedTicks)).ToString());
        }

        [TestMethod]
        public void CallInventoryWrapperDirector()
        {
            Destroy();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            InventoryWrapperDirector.GetInstance();
            sw.Stop();
            Console.WriteLine("InventoryWrapperDirector 생성 시간 : " + (new TimeSpan(sw.ElapsedTicks)).ToString());
        }

        //[Ignore]
        [TestMethod]
        public void CreateStockWrapperViewModel()
        {
            Destroy();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            StockWrapperViewModel vm = new StockWrapperViewModel(IOStockType.ALL, CollectionViewModelObserverSubject.GetInstance());
            sw.Stop();
            Console.WriteLine("StockWrapperViewModel 생성 시간 : " + (new TimeSpan(sw.ElapsedTicks)).ToString());
        }
    }
}
