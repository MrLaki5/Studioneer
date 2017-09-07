
CREATE TYPE [IDENTITY]
	FROM INTEGER NOT NULL
go

CREATE TABLE [Answer]
( 
	[IdA]                [IDENTITY]  IDENTITY ( 1,1 ) ,
	[IsCorrect]          integer  NULL ,
	[IdP]                [IDENTITY] ,
	[Tag]                char(1)  NULL ,
	[Text]               varchar(100)  NULL ,
	[Number]             integer  NULL 
)
go

ALTER TABLE [Answer]
	ADD CONSTRAINT [XPKAnswer] PRIMARY KEY  CLUSTERED ([IdA] ASC)
go

CREATE TABLE [Channel]
( 
	[IdC]                [IDENTITY]  IDENTITY ( 1,1 ) ,
	[Name]               varchar(20)  NULL ,
	[OpenTime]           datetime  NULL ,
	[IdU]                [IDENTITY] ,
	[Password]           varchar(100)  NULL ,
	[CloseTime]          datetime  NULL ,
	[StudentNumber]      integer  NULL 
)
go

ALTER TABLE [Channel]
	ADD CONSTRAINT [XPKChannel] PRIMARY KEY  CLUSTERED ([IdC] ASC)
go

CREATE TABLE [Order]
( 
	[IdO]                [IDENTITY]  IDENTITY ( 1,1 ) ,
	[IdU]                [IDENTITY] ,
	[Number]             integer  NULL ,
	[Price]              integer  NULL ,
	[State]              varchar(20)  NULL ,
	[Tag]                varchar(100)  NULL 
)
go

ALTER TABLE [Order]
	ADD CONSTRAINT [XPKOrder] PRIMARY KEY  CLUSTERED ([IdO] ASC)
go

CREATE TABLE [Parameters]
( 
	[IdP]                [IDENTITY]  IDENTITY ( 1,1 ) ,
	[AnswerNumber]       integer  NOT NULL ,
	[SilverNumber]       integer  NOT NULL ,
	[GoldNumber]         integer  NOT NULL ,
	[PlatinumNumber]     integer  NOT NULL ,
	[UnlockNumber]       integer  NOT NULL ,
	[PremiumNumber]      integer  NOT NULL 
)
go

ALTER TABLE [Parameters]
	ADD CONSTRAINT [XPKParameters] PRIMARY KEY  CLUSTERED ([IdP] ASC)
go

CREATE TABLE [Published]
( 
	[IdP]                [IDENTITY] ,
	[IdC]                [IDENTITY] ,
	[IdPub]              [IDENTITY]  IDENTITY ( 1,1 ) ,
	[PubTime]            datetime  NULL 
)
go

ALTER TABLE [Published]
	ADD CONSTRAINT [XPKPublished] PRIMARY KEY  CLUSTERED ([IdPub] ASC)
go

CREATE TABLE [Question]
( 
	[IdP]                [IDENTITY]  IDENTITY ( 1,1 ) ,
	[Title]              varchar(20)  NULL ,
	[Text]               varchar(200)  NULL ,
	[Image]              varbinary(max)  NULL ,
	[CreationTime]       datetime  NULL ,
	[LastLock]           datetime  NULL ,
	[IdU]                [IDENTITY] ,
	[IsLocked]           integer  NULL ,
	[IsClone]            integer  NULL 
)
go

ALTER TABLE [Question]
	ADD CONSTRAINT [XPKQuestion] PRIMARY KEY  CLUSTERED ([IdP] ASC)
go

CREATE TABLE [Response]
( 
	[IdU]                [IDENTITY] ,
	[IdP]                [IDENTITY] ,
	[IdC]                [IDENTITY] ,
	[IdA]                [IDENTITY] ,
	[SendTime]           datetime  NULL 
)
go

ALTER TABLE [Response]
	ADD CONSTRAINT [XPKResponse] PRIMARY KEY  CLUSTERED ([IdU] ASC,[IdP] ASC,[IdC] ASC)
go

CREATE TABLE [Subscription]
( 
	[IdC]                [IDENTITY] ,
	[IdU]                [IDENTITY] ,
	[IsPremium]          integer  NULL 
)
go

ALTER TABLE [Subscription]
	ADD CONSTRAINT [XPKSubscription] PRIMARY KEY  CLUSTERED ([IdC] ASC,[IdU] ASC)
go

CREATE TABLE [User]
( 
	[IdU]                [IDENTITY]  IDENTITY ( 1,1 ) ,
	[Name]               varchar(100)  NULL ,
	[Mail]               varchar(100)  NULL ,
	[Password]           varchar(100)  NULL ,
	[Balans]             integer  NULL 
	CONSTRAINT [geZero_1621994408]
		CHECK  ( Balans >= 0 ),
	[Type]               varchar(20)  NULL ,
	[Lastname]           varchar(100)  NULL ,
	[Status]             integer  NULL 
)
go

ALTER TABLE [User]
	ADD CONSTRAINT [XPKUser] PRIMARY KEY  CLUSTERED ([IdU] ASC)
go


ALTER TABLE [Answer]
	ADD CONSTRAINT [R_36] FOREIGN KEY ([IdP]) REFERENCES [Question]([IdP])
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go


ALTER TABLE [Channel]
	ADD CONSTRAINT [R_42] FOREIGN KEY ([IdU]) REFERENCES [User]([IdU])
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go


ALTER TABLE [Order]
	ADD CONSTRAINT [R_27] FOREIGN KEY ([IdU]) REFERENCES [User]([IdU])
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go


ALTER TABLE [Published]
	ADD CONSTRAINT [R_43] FOREIGN KEY ([IdP]) REFERENCES [Question]([IdP])
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go

ALTER TABLE [Published]
	ADD CONSTRAINT [R_44] FOREIGN KEY ([IdC]) REFERENCES [Channel]([IdC])
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go


ALTER TABLE [Question]
	ADD CONSTRAINT [R_35] FOREIGN KEY ([IdU]) REFERENCES [User]([IdU])
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go


ALTER TABLE [Response]
	ADD CONSTRAINT [R_37] FOREIGN KEY ([IdU]) REFERENCES [User]([IdU])
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go

ALTER TABLE [Response]
	ADD CONSTRAINT [R_38] FOREIGN KEY ([IdP]) REFERENCES [Question]([IdP])
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go

ALTER TABLE [Response]
	ADD CONSTRAINT [R_39] FOREIGN KEY ([IdC]) REFERENCES [Channel]([IdC])
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go

ALTER TABLE [Response]
	ADD CONSTRAINT [R_40] FOREIGN KEY ([IdA]) REFERENCES [Answer]([IdA])
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go


ALTER TABLE [Subscription]
	ADD CONSTRAINT [R_45] FOREIGN KEY ([IdC]) REFERENCES [Channel]([IdC])
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go

ALTER TABLE [Subscription]
	ADD CONSTRAINT [R_46] FOREIGN KEY ([IdU]) REFERENCES [User]([IdU])
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go


INSERT INTO [dbo].[User]
           ([Name]
           ,[Mail]
           ,[Password]
           ,[Balans]
           ,[Type]
           ,[Lastname]
           ,[Status])
     VALUES
           ('Admin'
           ,'admin@mail.com'
           ,'7C4A8D09CA3762AF61E59520943DC26494F8941B'
           ,0
           ,'Administrator'
           ,'Admin'
           ,1)
GO


INSERT INTO [dbo].[Parameters]
           ([AnswerNumber]
           ,[SilverNumber]
           ,[GoldNumber]
           ,[PlatinumNumber]
           ,[UnlockNumber]
           ,[PremiumNumber])
     VALUES
           (0,0,0,0,0,0)
GO