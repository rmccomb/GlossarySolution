CREATE TABLE [dbo].[Definition] (
    [DefinitionId] INT           IDENTITY (1, 1) NOT NULL,
    [Term]         VARCHAR (80)  NOT NULL,
    [TermDefinition]   VARCHAR (MAX) NOT NULL,
    PRIMARY KEY CLUSTERED ([DefinitionId] ASC)
);

