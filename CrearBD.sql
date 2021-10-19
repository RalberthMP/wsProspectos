USE [master]
GO

/****** Object:  Database [DBProspectos]    Script Date: 19/10/2021 12:31:57 p. m. ******/
CREATE DATABASE [DBProspectos]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'DBProspectos', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA\DBProspectos.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'DBProspectos_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA\DBProspectos_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO

IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
--EXEC [DBProspectos].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO

ALTER DATABASE [DBProspectos] SET ANSI_NULL_DEFAULT OFF 
GO

ALTER DATABASE [DBProspectos] SET ANSI_NULLS OFF 
GO

ALTER DATABASE [DBProspectos] SET ANSI_PADDING OFF 
GO

ALTER DATABASE [DBProspectos] SET ANSI_WARNINGS OFF 
GO

ALTER DATABASE [DBProspectos] SET ARITHABORT OFF 
GO

ALTER DATABASE [DBProspectos] SET AUTO_CLOSE OFF 
GO

ALTER DATABASE [DBProspectos] SET AUTO_SHRINK OFF 
GO

ALTER DATABASE [DBProspectos] SET AUTO_UPDATE_STATISTICS ON 
GO

ALTER DATABASE [DBProspectos] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO

ALTER DATABASE [DBProspectos] SET CURSOR_DEFAULT  GLOBAL 
GO

ALTER DATABASE [DBProspectos] SET CONCAT_NULL_YIELDS_NULL OFF 
GO

ALTER DATABASE [DBProspectos] SET NUMERIC_ROUNDABORT OFF 
GO

ALTER DATABASE [DBProspectos] SET QUOTED_IDENTIFIER OFF 
GO

ALTER DATABASE [DBProspectos] SET RECURSIVE_TRIGGERS OFF 
GO

ALTER DATABASE [DBProspectos] SET  DISABLE_BROKER 
GO

ALTER DATABASE [DBProspectos] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO

ALTER DATABASE [DBProspectos] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO

ALTER DATABASE [DBProspectos] SET TRUSTWORTHY OFF 
GO

ALTER DATABASE [DBProspectos] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO

ALTER DATABASE [DBProspectos] SET PARAMETERIZATION SIMPLE 
GO

ALTER DATABASE [DBProspectos] SET READ_COMMITTED_SNAPSHOT OFF 
GO

ALTER DATABASE [DBProspectos] SET HONOR_BROKER_PRIORITY OFF 
GO

ALTER DATABASE [DBProspectos] SET RECOVERY SIMPLE 
GO

ALTER DATABASE [DBProspectos] SET  MULTI_USER 
GO

ALTER DATABASE [DBProspectos] SET PAGE_VERIFY CHECKSUM  
GO

ALTER DATABASE [DBProspectos] SET DB_CHAINING OFF 
GO

ALTER DATABASE [DBProspectos] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO

ALTER DATABASE [DBProspectos] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO

ALTER DATABASE [DBProspectos] SET DELAYED_DURABILITY = DISABLED 
GO

ALTER DATABASE [DBProspectos] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO

ALTER DATABASE [DBProspectos] SET QUERY_STORE = OFF
GO

ALTER DATABASE [DBProspectos] SET  READ_WRITE 
GO

