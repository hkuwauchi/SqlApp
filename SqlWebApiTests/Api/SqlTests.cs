namespace SqlWebApi.Api.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SqlWebApi.Api;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    [TestClass()]
    public class SqlTests
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
            var sql = new Sql();
            sql.Query(1, new List<string[]> { new[] { create } });
        }

        [TestCleanup]
        public void TestCleanup()
        {
            Trace.WriteLine("TestCleanup");
            var sql = new Sql();
            sql.Query(1, new List<string[]> { new[] { drop } });
        }

        [TestMethod()]
        public void GetTest()
        {
            var sql = new Sql();
            var res = sql.Query(1, new List<string[]> { new[] { create } });
            res.IsNotNull();
            res.Count().Is(0);


            res = sql.Query(6, new List<string[]> { new[] { "1", "Name1" } });
            res.IsNotNull();
            res.Count().Is(0);

            res = sql.Query(5, new List<string[]> { null });

            res.IsNotNull();
            res.Count().Is(1);
            var row = res.First();
            row["Id"].Is(1);
            row["Name"].Is("Name1");
        }

        [TestMethod()]
        public void QueryTest()
        {
            var sql = new Sql();
        }

        [TestMethod()]
        public void ListTest()
        {
            var sql = new Sql();
            sql.List().Count().Is(8);
        }

        [TestMethod()]
        public void GetParamNamesTest()
        {
            var sql = new Sql();
            sql.GetParamNames(1).Count().Is(1);
        }
    }
}