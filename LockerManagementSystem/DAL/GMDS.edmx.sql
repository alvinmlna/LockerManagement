
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 01/20/2022 19:05:01
-- Generated from EDMX file: D:\Web Project\ASP\LockerManagement\LockerManagementSystem\DAL\GMDS.edmx
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


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[tbl_GMDS]', 'U') IS NOT NULL
    DROP TABLE [dbo].[tbl_GMDS];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'tbl_GMDS'
CREATE TABLE [dbo].[tbl_GMDS] (
    [No] int IDENTITY(1,1) NOT NULL,
    [Badge_ID] varchar(20)  NOT NULL,
    [Name] varchar(50)  NULL,
    [Group] varchar(10)  NULL,
    [Department] varchar(50)  NULL,
    [Join_Date] datetime  NULL,
    [Supervisor] varchar(50)  NULL,
    [Area] varchar(40)  NULL,
    [Password] varchar(10)  NULL,
    [Superior_Email] varchar(150)  NULL,
    [Email] varchar(150)  NULL,
    [Gender] varchar(50)  NULL,
    [Designation] varchar(150)  NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Badge_ID] in table 'tbl_GMDS'
ALTER TABLE [dbo].[tbl_GMDS]
ADD CONSTRAINT [PK_tbl_GMDS]
    PRIMARY KEY CLUSTERED ([Badge_ID] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------