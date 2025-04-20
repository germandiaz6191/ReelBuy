IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = '20250406012352_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES ('20250406012352_InitialCreate', '8.0.10')
END

-- Agregar la columna Description si no existe
IF NOT EXISTS (
    SELECT * FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'[dbo].[Products]') 
    AND name = 'Description'
)
BEGIN
    ALTER TABLE [dbo].[Products]
    ADD [Description] nvarchar(max) NOT NULL DEFAULT ''
END 