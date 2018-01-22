create table [dbo].[Attachments](
	[Id] uniqueidentifier default newsequentialid() primary key not null,
	[MessageId] nvarchar(50) not null,
	[Name] nvarchar(50) not null,
	[Expiry] datetime not null,
	[Data] varbinary(max) not null,
)