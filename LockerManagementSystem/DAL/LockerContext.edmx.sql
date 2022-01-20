
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 01/20/2022 19:05:57
-- Generated from EDMX file: D:\Web Project\ASP\LockerManagement\LockerManagementSystem\DAL\LockerContext.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [LockerManagement];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[LockerManagementSystem].[FK_EmployeeLocker_Locker]', 'F') IS NOT NULL
    ALTER TABLE [LockerManagementSystem].[EmployeeLocker] DROP CONSTRAINT [FK_EmployeeLocker_Locker];
GO
IF OBJECT_ID(N'[LockerManagementSystem].[FK_EmployeeLocker_Transaction]', 'F') IS NOT NULL
    ALTER TABLE [LockerManagementSystem].[EmployeeLocker] DROP CONSTRAINT [FK_EmployeeLocker_Transaction];
GO
IF OBJECT_ID(N'[LockerManagementSystem].[FK_Transaction_Locker]', 'F') IS NOT NULL
    ALTER TABLE [LockerManagementSystem].[Transaction] DROP CONSTRAINT [FK_Transaction_Locker];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[LockerManagementSystem].[Admin]', 'U') IS NOT NULL
    DROP TABLE [LockerManagementSystem].[Admin];
GO
IF OBJECT_ID(N'[LockerManagementSystem].[EmployeeLocker]', 'U') IS NOT NULL
    DROP TABLE [LockerManagementSystem].[EmployeeLocker];
GO
IF OBJECT_ID(N'[LockerManagementSystem].[Feedback]', 'U') IS NOT NULL
    DROP TABLE [LockerManagementSystem].[Feedback];
GO
IF OBJECT_ID(N'[LockerManagementSystem].[Locker]', 'U') IS NOT NULL
    DROP TABLE [LockerManagementSystem].[Locker];
GO
IF OBJECT_ID(N'[LockerManagementSystem].[SpecialEmployee]', 'U') IS NOT NULL
    DROP TABLE [LockerManagementSystem].[SpecialEmployee];
GO
IF OBJECT_ID(N'[LockerManagementSystem].[Transaction]', 'U') IS NOT NULL
    DROP TABLE [LockerManagementSystem].[Transaction];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Admin'
CREATE TABLE [dbo].[Admin] (
    [Username] varchar(50)  NOT NULL,
    [Name] varchar(max)  NULL,
    [Access] int  NULL
);
GO

-- Creating table 'EmployeeLocker'
CREATE TABLE [dbo].[EmployeeLocker] (
    [BadgeId] varchar(max)  NOT NULL,
    [LockerNumber] varchar(10)  NOT NULL,
    [TransactionId] int  NOT NULL,
    [Id] int IDENTITY(1,1) NOT NULL,
    [LockerID] int  NOT NULL
);
GO

-- Creating table 'Feedback'
CREATE TABLE [dbo].[Feedback] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [BadgeId] nvarchar(50)  NOT NULL,
    [Name] varchar(max)  NOT NULL,
    [Locker] varchar(10)  NOT NULL,
    [Remark] varchar(max)  NULL,
    [Superior] varchar(max)  NOT NULL,
    [SuperiorEmail] varchar(max)  NOT NULL,
    [Status] varchar(50)  NULL,
    [DateSubmit] datetime  NOT NULL,
    [Category] varchar(50)  NOT NULL,
    [Department] varchar(max)  NOT NULL
);
GO

-- Creating table 'Locker'
CREATE TABLE [dbo].[Locker] (
    [LockerNumber] varchar(10)  NOT NULL,
    [LockerKeyNumber] varchar(50)  NULL,
    [LockerType] int  NOT NULL,
    [Area] int  NOT NULL,
    [Site] varchar(50)  NOT NULL,
    [Stock] int  NULL,
    [IsActive] bit  NOT NULL,
    [Id] int IDENTITY(1,1) NOT NULL
);
GO

-- Creating table 'SpecialEmployee'
CREATE TABLE [dbo].[SpecialEmployee] (
    [BadgeId] varchar(100)  NOT NULL,
    [Name] varchar(100)  NULL
);
GO

-- Creating table 'Transaction'
CREATE TABLE [dbo].[Transaction] (
    [TransactionId] int IDENTITY(1,1) NOT NULL,
    [BadgeId] varchar(max)  NOT NULL,
    [Name] varchar(max)  NOT NULL,
    [Department] varchar(max)  NOT NULL,
    [Area] varchar(max)  NOT NULL,
    [Site] varchar(10)  NOT NULL,
    [TransactionType] int  NOT NULL,
    [FriendBadgeNumber] varchar(10)  NULL,
    [LockerId] int  NULL,
    [LockerNumber] varchar(10)  NULL,
    [DateRequest] datetime  NOT NULL,
    [DateRelease] datetime  NULL,
    [Releaseby] varchar(max)  NULL,
    [DateReturn] datetime  NULL,
    [ReceivedBy] varchar(max)  NULL,
    [Status] int  NULL,
    [DateTemporaryReturn] datetime  NULL,
    [Remark] varchar(max)  NULL,
    [EmployeeGender] varchar(10)  NULL,
    [Designation] varchar(max)  NULL,
    [PIC] varchar(max)  NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Username] in table 'Admin'
ALTER TABLE [dbo].[Admin]
ADD CONSTRAINT [PK_Admin]
    PRIMARY KEY CLUSTERED ([Username] ASC);
GO

-- Creating primary key on [Id] in table 'EmployeeLocker'
ALTER TABLE [dbo].[EmployeeLocker]
ADD CONSTRAINT [PK_EmployeeLocker]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Feedback'
ALTER TABLE [dbo].[Feedback]
ADD CONSTRAINT [PK_Feedback]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Locker'
ALTER TABLE [dbo].[Locker]
ADD CONSTRAINT [PK_Locker]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [BadgeId] in table 'SpecialEmployee'
ALTER TABLE [dbo].[SpecialEmployee]
ADD CONSTRAINT [PK_SpecialEmployee]
    PRIMARY KEY CLUSTERED ([BadgeId] ASC);
GO

-- Creating primary key on [TransactionId] in table 'Transaction'
ALTER TABLE [dbo].[Transaction]
ADD CONSTRAINT [PK_Transaction]
    PRIMARY KEY CLUSTERED ([TransactionId] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [TransactionId] in table 'EmployeeLocker'
ALTER TABLE [dbo].[EmployeeLocker]
ADD CONSTRAINT [FK_EmployeeLocker_Transaction]
    FOREIGN KEY ([TransactionId])
    REFERENCES [dbo].[Transaction]
        ([TransactionId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_EmployeeLocker_Transaction'
CREATE INDEX [IX_FK_EmployeeLocker_Transaction]
ON [dbo].[EmployeeLocker]
    ([TransactionId]);
GO

-- Creating foreign key on [LockerId] in table 'Transaction'
ALTER TABLE [dbo].[Transaction]
ADD CONSTRAINT [FK_Transaction_Locker]
    FOREIGN KEY ([LockerId])
    REFERENCES [dbo].[Locker]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Transaction_Locker'
CREATE INDEX [IX_FK_Transaction_Locker]
ON [dbo].[Transaction]
    ([LockerId]);
GO

-- Creating foreign key on [LockerID] in table 'EmployeeLocker'
ALTER TABLE [dbo].[EmployeeLocker]
ADD CONSTRAINT [FK_EmployeeLocker_Locker]
    FOREIGN KEY ([LockerID])
    REFERENCES [dbo].[Locker]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_EmployeeLocker_Locker'
CREATE INDEX [IX_FK_EmployeeLocker_Locker]
ON [dbo].[EmployeeLocker]
    ([LockerID]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------