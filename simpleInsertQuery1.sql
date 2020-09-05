select top 10 
'insert into corporateknowhowalert ([UniqueId], [ItemId], [Title], [Content], [Description], [Data], [AuthorEmail], [ImageFileName], [TagValue], [CreatedOn], [ModifiedOn]) VALUES (' +
'''' + convert(varchar(max), cka.uniqueid) + '''' + ',' +
convert(varchar(10), cka.ItemId) + ',' +
'''' + replace(convert(varchar(max), cka.title), '''', '''''') + '''' + ',' +
'''' + replace(convert(varchar(max), ISNULL(cka.content, '')), '''', '''''') + '''' + ',' +
'''' + replace(convert(varchar(max), ISNULL(cka.[description], '')), '''', '''''') + '''' + ',' +
'''' + replace(convert(varchar(max), ISNULL(cka.[data], '')), '''', '''''') + '''' + ',' +
'''' + convert(varchar(max), cka.AuthorEmail) + '''' + ',' +
'''' + convert(varchar(max), ISNULL(cka.ImageFileName, '')) + '''' + ',' +
'''' + replace(convert(varchar(max), cka.TagValue), '''', '''''') + '''' + ',' +
'''' + convert(varchar(max), cka.CreatedOn) + '''' + ',' +
'''' + convert(varchar(max), cka.ModifiedOn) + '''' + ')'
from corporateknowhowalert cka
