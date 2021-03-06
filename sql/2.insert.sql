--param
declare @id int
set @id = '$(Param1)'
declare @name nchar(10)
set @name = '$(Param2)'
--sql
BEGIN TRY
    BEGIN TRANSACTION
        INSERT INTO [Test].[dbo].[user]
                    ([id],[name])
                VALUES
                    (@id,@name)
    COMMIT TRANSACTION
END TRY

BEGIN CATCH
    ROLLBACK TRANSACTION
    PRINT ERROR_MESSAGE()
    PRINT 'ROLLBACK TRANSACTION'
END CATCH
