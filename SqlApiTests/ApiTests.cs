namespace SqlApi.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SqlApi;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    [TestClass()]
    public class ApiTests
    {
        string create = @"
IF OBJECT_ID('[Test].[dbo].[User]') IS NOT NULL
    BEGIN
        GOTO Skip
    END
ELSE
CREATE TABLE[Test].[dbo].[User]
        (

[Id][int] NOT NULL,

[Name] [varchar] (10) NULL, 
    [Note] [text] NULL, 
    CONSTRAINT[PK_User] PRIMARY KEY CLUSTERED
(
   [Id] ASC
)WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY] 
) ON[PRIMARY] TEXTIMAGE_ON[PRIMARY]
Skip: 
";
        string drop = @"
IF OBJECT_ID('[Test].[dbo].[User]') IS NULL
    BEGIN
        GOTO Skip
    END
ELSE
BEGIN TRY
    BEGIN TRANSACTION
        DROP TABLE  [Test].[dbo].[User]
    COMMIT TRANSACTION
END TRY

BEGIN CATCH
    ROLLBACK TRANSACTION
    PRINT ERROR_MESSAGE()
    PRINT 'ROLLBACK TRANSACTION'
END CATCH
Skip: 
";

        [TestInitialize]
        public void TestInitialize()
        {
            Trace.WriteLine("TestInitialize");

            var api = new Api();

            var pin = new[] { drop };
            api.Query(1, pin);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            Trace.WriteLine("TestCleanup");

            var api = new Api();

            var pin = new[] { drop };
            api.Query(1, pin);
        }

        [TestMethod()]
        public void SqlFilesTest()
        {
            var api = new Api();
            api.SqlDic.Count.Is(8);
            foreach (var c in api.SqlDic.OrderBy(c => c.Key))
            {
                Console.WriteLine(
                string.Join(Environment.NewLine,
                $"-----------------------",
                $"Id:{c.Value.Id}",
                $"FileName:{c.Value.SqlFile.FileName}",
                $"ParamNames:{(c.Value.ParamNames != null ? string.Join(",", c.Value.ParamNames) : "null")}",
                "QueryText:",
                c.Value.QueryText));
            }
        }


        [TestMethod()]
        public void GetHeaderTest()
        {
            var api = new Api();
            var header = api.GetHeader(1);

            header.IsNotNull();
            header["description"].Is("SQLを実行する");
            header["author"].Is("unknown");
            header["client"].Is("unknown");

            Console.WriteLine($"description:");
            Console.WriteLine($"{header["description"]}");
            Console.WriteLine($"author:");
            Console.WriteLine($"{header["author"]}");
            Console.WriteLine($"client:");
            Console.WriteLine($"{header["client"]}");
        }

        [TestMethod()]
        public void GetParamNamesTest()
        {
            var api = new Api();
            var pl = api.GetParamNames(0);
            pl.IsNull();

            pl = api.GetParamNames(1);
            pl.Count.Is(1);
            pl[0].Is("statements");

            pl = api.GetParamNames(2);
            pl.Count.Is(1);
            pl[0].Is("id");

            pl = api.GetParamNames(6);
            pl.Count.Is(2);
            pl[0].Is("id");
            pl[1].Is("name");
        }

        [TestMethod()]
        public void QueryTest()
        {
            var api = new Api();

            var pin = new[] { create };
            var res = api.Query(1, pin);
            res.IsNotNull();
            res.Count().Is(0);

            pin = new[] { "1", "Name1" };
            res = api.Query(6, pin);
            res.IsNotNull();
            res.Count().Is(0);

            res = api.Query(5);
            res.IsNotNull();
            res.Count().Is(1);
            var row = res.First();
            row["Id"].Is(1);
            row["Name"].Is("Name1");

            pin = new[] { "2", "Name2" };
            res = api.Query(6, pin);
            res.IsNotNull();
            res.Count().Is(0);

            res = api.Query(5);
            res.IsNotNull();
            res.Count().Is(2);
            row = res.Skip(1).First();
            row["Id"].Is(2);
            row["Name"].Is("Name2");

            pin = new string[] { "1", "2" };
            res = api.Query(7, pin);
            res.IsNotNull();
            res.Count().Is(2);
            row = res.First();
            row["i"].Is(1);

            res = api.Query(8);
            res.IsNotNull();
            res.Count().Is(2);
            row = res.First();
            row["i"].Is(1);

            row = res.Skip(1).First();
            row["i"].Is(2);

            pin = new[] { drop };
            res = api.Query(1, pin);
            res.IsNotNull();
            res.Count().Is(0);
        }

        [TestMethod()]
        public void ExecuteTest()
        {
            var api = new Api();

            var pin = new[] { create };
            var res = api.Query(1, pin);
            res.IsNotNull();
            res.Count().Is(0);

            var pins = new List<string[]>()
            {
                new[] { "1", "Name1" },
                new[] { "2", "Name2" },
            };

            var cnt = api.Execute(6, pins);
            cnt.IsNotNull();
            cnt.Is(2);

            res = api.Query(5);
            res.IsNotNull();
            res.Count().Is(2);
            var row = res.First();
            row["Id"].Is(1);
            row["Name"].Is("Name1");

            row = res.Skip(1).First();
            row["Id"].Is(2);
            row["Name"].Is("Name2");

            pin = new string[] { "1" };
            res = api.Query(4, pin);
            res.IsNotNull();
            res.Count().Is(1);
            row = res.First();
            row["Id"].Is(1);
            row["Name"].Is("Name1");
        }
    }
}