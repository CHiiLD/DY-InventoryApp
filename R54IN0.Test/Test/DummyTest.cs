using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace R54IN0.Test.New
{
    [TestClass]
    public class DummyTest
    {
        [TestMethod]
        public async Task CanCreate()
        {
            await new Dummy().Create();
        }
    }
}