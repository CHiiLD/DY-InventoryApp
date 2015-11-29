using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace R54IN0.Test
{
    [TestClass]
    public class InOutStockEditorViewModelTest
    {
        [TestMethod]
        public void CanCreateInOutStockEditorViewModel()
        {
            new InOutStockEditorViewModel();
        }

        [TestMethod]
        public void CanCopyInOutStockEditorViewModel()
        {
            using (var db = DatabaseDirector.GetDbInstance())
            {
                var io = db.LoadAll<InOutStock>().FirstOrDefault();
                new InOutStockEditorViewModel(io);
            }
        }
    }
}
