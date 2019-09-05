CREATE TABLE [dbo].[Users]
(
	 [UserId] INT IDENTITY(0,1), 
    [FirstName] VARCHAR(32) NOT NULL, 
    [LastName] VARCHAR(32) NOT NULL, 
    [Country] VARCHAR(32) NOT NULL, 
    [ContactNumber] VARCHAR(32) NOT NULL, 
    [NID] VARCHAR(64) NOT NULL, 
    [UserName] VARCHAR(64) NOT NULL, 
    [Email] VARCHAR(256) NOT NULL, 
    [Password] VARCHAR(128) NOT NULL,
    PRIMARY KEY(UserId, UserName, Password)
)
