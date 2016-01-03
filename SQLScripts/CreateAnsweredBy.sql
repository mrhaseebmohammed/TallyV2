IF EXISTS(select name from sysobjects where name = 'AnsweredBy') 
SELECT 'PRESENT'
ELSE
CREATE TABLE [dbo].[AnsweredBy] (
    [AnsweredByID] INT            IDENTITY (1, 1) NOT NULL,
    [BrowserKey]   NVARCHAR (MAX) NOT NULL,
    [AnswerID]     INT            NOT NULL,
	[QuestionID]     INT            NOT NULL,
    PRIMARY KEY CLUSTERED ([AnsweredByID] ASC)
);


