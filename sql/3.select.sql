--param
declare @id int
set @id = '$(Param1)'
--sql
select *
from [Test].[dbo].[user]
where id = @id
