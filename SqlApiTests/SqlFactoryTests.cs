namespace SqlApi.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SqlApi;
    using SqlApiTests;
    using System.IO;
    using System.Linq;

    [TestClass()]
    public class SqlFactoryTests
    {
        [TestMethod()]
        public void CreateDefaultSqlSetTest()
        {
            var repoPath = "SqlFiles";
            if (Directory.Exists(repoPath))
            {
                DirectoryHelper.DeleteDirectory(repoPath);
            }
            SqlFactory.CreateDefaultSqlSet().Count().Is(9);
        }
    }
}