namespace SqlApi
{
    using System.Collections.Generic;

    public class SqlFactory
    {
        public static IEnumerable<Sql> CreateDefaultSqlSet()
        {
            return new List<Sql>()
            {
                new Sql()
                {
                    SqlFile = new SqlFile()
                    {
                        FileName = "1.drop",
                        RawText = @"
--sql
IF OBJECT_ID('[Test].[dbo].[User]') IS NULL
    BEGIN
        GOTO Skip
    END
ELSE

BEGIN TRY
    BEGIN TRANSACTION
        DROP TABLE [Test].[dbo].[User]
    COMMIT TRANSACTION
END TRY

BEGIN CATCH
    ROLLBACK TRANSACTION
    PRINT ERROR_MESSAGE()
    PRINT 'ROLLBACK TRANSACTION'
END CATCH

Skip:
"
                    }
                },
                new Sql()
                {
                    SqlFile = new SqlFile()
                    {
                        FileName = "2.create",
                        RawText = @"
--sql
IF OBJECT_ID('[Test].[dbo].[User]') IS NOT NULL
    BEGIN
        GOTO Skip
    END
ELSE

CREATE TABLE [Test].[dbo].[User](
    [Id] [int] NOT NULL,
    [Name] [varchar](10) NULL,
    [Note] [text] NULL,
    CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
    [Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

Skip:
"
                    }
                },
                new Sql()
                {
                    SqlFile = new SqlFile()
                    {
                        FileName = "3.drop_param",
                        RawText = @"
--param
declare @table varchar(10)
set @table = 'User'
--sql
exec(
  'IF OBJECT_ID(''[Test].[dbo].[' + @table + ']'') IS NULL '
+ '    BEGIN '
+ '        GOTO Skip '
+ '    END '
+ 'ELSE '
+ 'BEGIN TRY '
+ '    BEGIN TRANSACTION '
+ '        DROP TABLE [Test].[dbo].[User] '
+ '    COMMIT TRANSACTION '
+ 'END TRY '
+ 'BEGIN CATCH '
+ '    ROLLBACK TRANSACTION '
+ '    PRINT ERROR_MESSAGE() '
+ '    PRINT ''ROLLBACK TRANSACTION'' '
+ 'END CATCH '
+ 'Skip: '
)
"
                    }
                },
                new Sql()
                {
                    SqlFile = new SqlFile()
                    {
                        FileName = "4.create_param",
                        RawText = @"
--param
declare @table varchar(10)
set @table = 'User'
--sql
exec(
  'IF OBJECT_ID(''[Test].[dbo].[' + @table + ']'') IS NOT NULL '
+ '    BEGIN '
+ '        GOTO Skip '
+ '    END '
+ 'ELSE '
+ 'CREATE TABLE [Test].[dbo].[' + @table + ']( '
+ '    [Id] [int] NOT NULL, '
+ '    [Name] [varchar](10) NULL, '
+ '    [Note] [text] NULL, '
+ '    CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED  '
+ '( '
+ '    [Id] ASC '
+ ')WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY] '
+ ') ON [PRIMARY] TEXTIMAGE_ON [PRIMARY] '
+ 'Skip: '
)
"
                    }
                }
            };
        }
    }
}
