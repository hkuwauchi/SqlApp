namespace SqlApi.Tests
{
    using Codeplex.Data;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SqlApi;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [TestClass()]
    public class ApiTests
    {
        [TestMethod()]
        public void SqlFilesTest()
        {
            var api = new Api();
            api.SqlFiles.Count.Is(5);
            api.SqlFiles.ForEach(c => Console.WriteLine($"{c.Id},{c.FileName},{string.Join(",", c.ParamList.Select(p => p.Name))}{Environment.NewLine}{c.SqlText}"));

            api.Execute(1);

//            api.Execute(4, new { p1 = 3, p2 = "test3" });

            api.Execute(4, new[] { "3", "test4" });

            Console.WriteLine();

            api.Execute(1);
        }
}
}