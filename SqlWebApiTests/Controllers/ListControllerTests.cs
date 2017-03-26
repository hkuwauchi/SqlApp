using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlWebApi.Controllers;

namespace SqlWebApi.Controllers.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SqlWebApi.Controllers;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [TestClass()]
    public class ListControllerTests
    {
        [TestMethod()]
        public void GetTest()
        {
            var list = new ListController();
            list.Get().Count().Is(8);
        }

        [TestMethod()]
        public void GetIdTest()
        {
            var list = new ListController();
            list.Get(1).Count().Is(1);
        }
    }
}