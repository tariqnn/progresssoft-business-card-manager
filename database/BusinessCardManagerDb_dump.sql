IF DB_ID(N'BusinessCardManagerDb') IS NULL
BEGIN
    CREATE DATABASE [BusinessCardManagerDb];
END;
GO

USE [BusinessCardManagerDb];
GO

IF OBJECT_ID(N'[dbo].[__EFMigrationsHistory]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

IF OBJECT_ID(N'[dbo].[BusinessCards]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[BusinessCards] (
        [Id] int NOT NULL IDENTITY(1,1),
        [Name] nvarchar(150) NOT NULL,
        [Gender] nvarchar(20) NOT NULL,
        [DateOfBirth] date NOT NULL,
        [Email] nvarchar(254) NOT NULL,
        [Phone] nvarchar(30) NOT NULL,
        [PhotoBase64] nvarchar(max) NULL,
        [Address] nvarchar(500) NOT NULL,
        [CreatedAtUtc] datetime2 NOT NULL CONSTRAINT [DF_BusinessCards_CreatedAtUtc] DEFAULT (GETUTCDATE()),
        CONSTRAINT [PK_BusinessCards] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT 1
    FROM [dbo].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260427233305_InitialCreate'
)
BEGIN
    INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260427233305_InitialCreate', N'8.0.22');
END;
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[BusinessCards] WHERE [Email] = N'ali.test@example.com')
BEGIN
    INSERT INTO [dbo].[BusinessCards] ([Name], [Gender], [DateOfBirth], [Email], [Phone], [PhotoBase64], [Address])
    VALUES (
        N'Ali Ahmad',
        N'Male',
        '1998-01-15',
        N'ali.test@example.com',
        N'+962790000001',
        NULL,
        N'Amman, Jordan'
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[BusinessCards] WHERE [Email] = N'sara.test@example.com')
BEGIN
    INSERT INTO [dbo].[BusinessCards] ([Name], [Gender], [DateOfBirth], [Email], [Phone], [PhotoBase64], [Address])
    VALUES (
        N'Sara Khaled',
        N'Female',
        '1997-02-20',
        N'sara.test@example.com',
        N'+962790000002',
        N'iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/x8AAwMCAO+/p9sAAAAASUVORK5CYII=',
        N'Irbid, Jordan'
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[BusinessCards] WHERE [Email] = N'omar.test@example.com')
BEGIN
    INSERT INTO [dbo].[BusinessCards] ([Name], [Gender], [DateOfBirth], [Email], [Phone], [PhotoBase64], [Address])
    VALUES (
        N'Omar Saleh',
        N'Male',
        '1996-03-25',
        N'omar.test@example.com',
        N'+962790000003',
        NULL,
        N'Zarqa, Jordan'
    );
END;
GO
