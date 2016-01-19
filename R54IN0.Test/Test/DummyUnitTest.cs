using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace R54IN0.Test.New
{
    [TestClass]
    public class DummyTest
    {
        [TestMethod]
        public void CanCreate()
        {
            new Dummy().Create();
        }
    }
}