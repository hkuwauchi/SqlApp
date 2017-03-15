namespace SqlApi.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SqlApi;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [TestClass()]
    public class ApiTests
    {
        [TestMethod()]
        public void SqlFilesTest()
        {
            var api = new Api();
            api.SqlFiles.Count.Is(5);
            api.SqlFiles.ForEach(c => Console.WriteLine(
                string.Join(Environment.NewLine,
                $"-----------------------",
                c.Id,
                c.FileName,
                string.Join(",", c.ParamList.Select(p => p)),
                c.SqlText)
                ));
        }

        [TestMethod()]
        public void GetParamListTest()
        {
            var api = new Api();
            var pl = api.GetParamList(1);
            pl.Count.Is(0);

            pl = api.GetParamList(2);
            pl.Count.Is(2);
            pl[0].Is("id");
            pl[1].Is("name");
        }

        [TestMethod()]
        public void ExecuteTest()
        {
            var api = new Api();

            var res = api.Execute(1);

            Console.WriteLine("-----------------------");
            foreach (var row in res)
            {
                Console.WriteLine(string.Join(",", row));
            }

            var pl = api.GetParamList(3);
            var pin = new[] { "3" };

            var paramList = pl.Select(c => c).Zip(pin, (p, v) => new { p, v }).ToDictionary(d => d.p, d => (object)d.v);

            res = api.Execute(3, paramList);


            Console.WriteLine("-----------------------");

            pl = api.GetParamList(4);
            var num = int.Parse(((IDictionary<string, object>)res.First())["name"].ToString().Replace("test", ""));

            pin = new[] { "3", $"test{(num == 3 ? 4 : 3)}" };

            paramList = pl.Select(c => c).Zip(pin, (p, v) => new { p, v }).ToDictionary(d => d.p, d => (object)d.v);

            res = api.Execute(4, paramList);

            res = api.Execute(1);

            Console.WriteLine("-----------------------");
            foreach (var row in res)
            {
                Console.WriteLine(string.Join(",", row));
            }

        }

    }
}