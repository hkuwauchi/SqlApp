namespace SqlWebApi.Controllers.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SqlWebApi.Controllers;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [TestClass()]
    public class QueryControllerTests
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
            var query = new QueryController();
            query.Get(1, new List<string[]> { new[] { create } });

        }

        [TestCleanup]
        public void TestCleanup()
        {
            Trace.WriteLine("TestCleanup");
            var query = new QueryController();
            query.Get(1, new List<string[]> { new[] { drop } });

        }

        [TestMethod()]
        public void GetTest()
        {
            var query = new QueryController();
            var res = query.Get(1, new List<string[]> { new[] { create } });
            res.IsNotNull();
            res.Count().Is(0);


            res = query.Get(6, new List<string[]> { new[] { "1", "Name1" } });
            res.IsNotNull();
            res.Count().Is(0);

            res = query.Get(5, new List<string[]> { null });

            res.IsNotNull();
            res.Count().Is(1);
            var row = res.First();
            row["Id"].Is(1);
            row["Name"].Is("Name1");

        }
    }
}