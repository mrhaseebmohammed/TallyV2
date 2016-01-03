IF EXISTS(select name from sysobjects where name = 'Questions') 
SELECT 'PRESENT'
ELSE
CREATE TABLE [dbo].[Questions] (
    [QuestionID]      INT            IDENTITY (1, 1) NOT NULL,
    [ShortURL]        NVARCHAR (50)  NOT NULL,
    [QuestionText]    NVARCHAR (MAX) NOT NULL,
    [CreatedDateTime] SMALLDATETIME  DEFAULT (getdate()) NOT NULL,
    [Password]        NVARCHAR (50)  NOT NULL,
    [CreatedByUserID] INT            NULL,
	[Active] BIT NOT NULL DEFAULT 1, 
	[ModifiedDateTime] SMALLDATETIME NULL, 
    PRIMARY KEY CLUSTERED ([QuestionID] ASC)
);

