CREATE TABLE [dbo].[Login]
(
	[LoginId] INT IDENTITY(0,1) PRIMARY KEY, 
    [UserId] INT, 
    [UserName] VARCHAR(64), 
    [Password] VARCHAR(128), 
    [LoggedIn] BIT NULL DEFAULT 0,
    FOREIGN KEY(UserId, UserName, Password) REFERENCES Users(UserId, UserName, Password)
)
