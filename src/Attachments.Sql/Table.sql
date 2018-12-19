declare @fullName nvarchar(max) = @schema + '.' + @table;
if exists (
    select * from sys.objects
    where
        object_id = object_id(@fullName)
        and type in ('U')
)
	begin

		declare @alterTable nvarchar(max);
		set @alterTable = '
		drop index ' + @fullName + '.Index_MessageIdName
		alter table ' + @fullName + ' drop column NameLower
		alter table ' + @fullName + ' alter column Name nvarchar(255) not null
		alter table ' + @fullName + ' add NameLower as lower(Name)
		create unique index Index_MessageIdName
		  on ' + @fullName + '(MessageIdLower, NameLower);
		';
		exec(@alterTable);
	end
else
	begin

		declare @createTable nvarchar(max);
		set @createTable = '
		create table ' + @fullName + '(
			Id uniqueidentifier default newsequentialid() primary key not null,
			MessageId nvarchar(50) not null,
			MessageIdLower as lower(MessageId),
			Name nvarchar(50) not null,
			NameLower as lower(Name),
			Created datetime2(0) not null default sysutcdatetime(),
			Expiry datetime2(0) not null,
			Metadata nvarchar(max),
			Data varbinary(max) not null
		)
		create unique index Index_MessageIdName
		  on ' + @fullName + '(MessageIdLower, NameLower);
		';
		exec(@createTable);

	end