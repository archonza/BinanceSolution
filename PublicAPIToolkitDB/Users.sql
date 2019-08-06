CREATE TABLE [dbo].[Users]
(
	[Id] INT IDENTITY(0,1) PRIMARY KEY, 
    [FirstName] NCHAR(32) NULL, 
    [LastName] NCHAR(32) NULL, 
    [Country] NCHAR(32) NULL, 
    [ContactNumber] NCHAR(32) NULL, 
    [NID] NCHAR(64) NULL, 
    [UserName] NCHAR(64) NULL, 
    [Email] NCHAR(256) NULL, 
    [Password] NCHAR(128) NULL
)
