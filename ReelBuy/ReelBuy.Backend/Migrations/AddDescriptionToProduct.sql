IF NOT EXISTS (
    SELECT * FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'[dbo].[Products]') 
    AND name = 'Description'
)
BEGIN
    ALTER TABLE [dbo].[Products]
    ADD [Description] nvarchar(max) NOT NULL DEFAULT ''
END 