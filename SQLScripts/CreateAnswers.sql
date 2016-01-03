IF EXISTS(select name from sysobjects where name = 'Answers') 
SELECT 'PRESENT'
ELSE
CREATE TABLE [dbo].[Answers] (
    [AnswerID]   INT            IDENTITY (1, 1) NOT NULL,
    [QuestionID] INT            NOT NULL,
    [AnswerText] NVARCHAR (200) NOT NULL,
    [Count]      INT            DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([AnswerID] ASC)
);

