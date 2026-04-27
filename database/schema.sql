IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [BusinessCards] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(150) NOT NULL,
    [Gender] nvarchar(20) NOT NULL,
    [DateOfBirth] date NOT NULL,
    [Email] nvarchar(254) NOT NULL,
    [Phone] nvarchar(30) NOT NULL,
    [PhotoBase64] nvarchar(max) NULL,
    [Address] nvarchar(500) NOT NULL,
    [CreatedAtUtc] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
    CONSTRAINT [PK_BusinessCards] PRIMARY KEY ([Id])
);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260427233305_InitialCreate', N'8.0.22');
GO

COMMIT;
GO

