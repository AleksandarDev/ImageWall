
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 12/08/2012 17:26:10
-- Generated from EDMX file: C:\Users\Aleksandar\Dropbox\Documents\JustBuild Development\Projects - Software\Project - ImageWall\www\ImageWallService\Data\ImageWallServiceDataModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [team95_ImageWall];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_UserImageTag]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ImageTagSet] DROP CONSTRAINT [FK_UserImageTag];
GO
IF OBJECT_ID(N'[dbo].[FK_UserImageDetails]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ImageDetailsSet] DROP CONSTRAINT [FK_UserImageDetails];
GO
IF OBJECT_ID(N'[dbo].[FK_ImageTagImageDetails_ImageTag]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ImageTagImageDetails] DROP CONSTRAINT [FK_ImageTagImageDetails_ImageTag];
GO
IF OBJECT_ID(N'[dbo].[FK_ImageTagImageDetails_ImageDetails]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ImageTagImageDetails] DROP CONSTRAINT [FK_ImageTagImageDetails_ImageDetails];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[ImageDetailsSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ImageDetailsSet];
GO
IF OBJECT_ID(N'[dbo].[ImageTagSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ImageTagSet];
GO
IF OBJECT_ID(N'[dbo].[UserSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserSet];
GO
IF OBJECT_ID(N'[dbo].[ImageTagImageDetails]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ImageTagImageDetails];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'ImageDetailsSet'
CREATE TABLE [dbo].[ImageDetailsSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Hash] nvarchar(max)  NOT NULL,
    [Date] datetime  NOT NULL,
    [Latitude] float  NOT NULL,
    [Longitude] float  NOT NULL,
    [URL] nvarchar(max)  NOT NULL,
    [URLThumb] nvarchar(max)  NOT NULL,
    [User_Id] int  NOT NULL
);
GO

-- Creating table 'ImageTagSet'
CREATE TABLE [dbo].[ImageTagSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Alias] nvarchar(max)  NOT NULL,
    [Size] int  NOT NULL,
    [IsPopular] bit  NOT NULL,
    [Date] datetime  NOT NULL,
    [User_Id] int  NOT NULL
);
GO

-- Creating table 'UserSet'
CREATE TABLE [dbo].[UserSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [IsRegistered] bit  NOT NULL,
    [UserId] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'ImageTagImageDetails'
CREATE TABLE [dbo].[ImageTagImageDetails] (
    [ImageTag_Id] int  NOT NULL,
    [ImageDetails_Id] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'ImageDetailsSet'
ALTER TABLE [dbo].[ImageDetailsSet]
ADD CONSTRAINT [PK_ImageDetailsSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ImageTagSet'
ALTER TABLE [dbo].[ImageTagSet]
ADD CONSTRAINT [PK_ImageTagSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'UserSet'
ALTER TABLE [dbo].[UserSet]
ADD CONSTRAINT [PK_UserSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [ImageTag_Id], [ImageDetails_Id] in table 'ImageTagImageDetails'
ALTER TABLE [dbo].[ImageTagImageDetails]
ADD CONSTRAINT [PK_ImageTagImageDetails]
    PRIMARY KEY NONCLUSTERED ([ImageTag_Id], [ImageDetails_Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [User_Id] in table 'ImageTagSet'
ALTER TABLE [dbo].[ImageTagSet]
ADD CONSTRAINT [FK_UserImageTag]
    FOREIGN KEY ([User_Id])
    REFERENCES [dbo].[UserSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserImageTag'
CREATE INDEX [IX_FK_UserImageTag]
ON [dbo].[ImageTagSet]
    ([User_Id]);
GO

-- Creating foreign key on [User_Id] in table 'ImageDetailsSet'
ALTER TABLE [dbo].[ImageDetailsSet]
ADD CONSTRAINT [FK_UserImageDetails]
    FOREIGN KEY ([User_Id])
    REFERENCES [dbo].[UserSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserImageDetails'
CREATE INDEX [IX_FK_UserImageDetails]
ON [dbo].[ImageDetailsSet]
    ([User_Id]);
GO

-- Creating foreign key on [ImageTag_Id] in table 'ImageTagImageDetails'
ALTER TABLE [dbo].[ImageTagImageDetails]
ADD CONSTRAINT [FK_ImageTagImageDetails_ImageTag]
    FOREIGN KEY ([ImageTag_Id])
    REFERENCES [dbo].[ImageTagSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [ImageDetails_Id] in table 'ImageTagImageDetails'
ALTER TABLE [dbo].[ImageTagImageDetails]
ADD CONSTRAINT [FK_ImageTagImageDetails_ImageDetails]
    FOREIGN KEY ([ImageDetails_Id])
    REFERENCES [dbo].[ImageDetailsSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ImageTagImageDetails_ImageDetails'
CREATE INDEX [IX_FK_ImageTagImageDetails_ImageDetails]
ON [dbo].[ImageTagImageDetails]
    ([ImageDetails_Id]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------