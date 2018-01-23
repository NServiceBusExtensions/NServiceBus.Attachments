declare @fullName nvarchar(max) = '[' + @schema + '].[' + @tableName + ']';
if not exists (
    select * from sys.objects
    where
        object_id = object_id(@fullName)
        and type in ('U')
)
begin


declare @createTable nvarchar(max);
set @createTable = '
create table ' + @fullName + '(
	[Id] uniqueidentifier default newsequentialid() primary key not null,
	[MessageId] nvarchar(50) not null,
	[Name] nvarchar(50) not null,
	[Expiry] datetime not null,
	[Data] varbinary(max) not null,
)
create unique index Index_MessageIdName
  on ' + @fullName + '(MessageId, Name);
';
exec(@createTable);


end