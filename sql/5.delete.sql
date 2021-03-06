--param
declare @id int
set @id = '$(Param1)'
--sql
BEGIN TRY
    BEGIN TRANSACTION
        DELETE FROM [Test].[dbo].[user]
                WHERE id = @id
    COMMIT TRANSACTION
END TRY

BEGIN CATCH
    ROLLBACK TRANSACTION
    PRINT ERROR_MESSAGE()
    PRINT 'ROLLBACK TRANSACTION'
END CATCH
