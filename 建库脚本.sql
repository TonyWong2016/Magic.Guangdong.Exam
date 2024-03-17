USE [Magic.Guangdong.Exam]
GO
/****** Object:  Schema [cap]    Script Date: 2024/3/17 22:01:19 ******/
CREATE SCHEMA [cap]
GO
/****** Object:  Table [dbo].[UserAnswerRecord]    Script Date: 2024/3/17 22:01:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserAnswerRecord](
	[Id] [bigint] NOT NULL,
	[UserId] [bigint] NOT NULL,
	[UserName] [varchar](150) NULL,
	[PaperId] [uniqueidentifier] NOT NULL,
	[ExamId] [uniqueidentifier] NOT NULL,
	[SubmitAnswer] [varchar](max) NOT NULL,
	[Remark] [varchar](1000) NULL,
	[CreatedAt] [datetime] NOT NULL,
	[UpdatedAt] [datetime] NOT NULL,
	[CreatedBy] [varchar](50) NULL,
	[UpdatedBy] [varchar](50) NULL,
	[IsDeleted] [int] NOT NULL,
	[CheatCnt] [int] NOT NULL,
	[Complated] [int] NOT NULL,
	[ComplatedMode] [int] NOT NULL,
	[Score] [float] NOT NULL,
	[IdNumber] [varchar](50) NOT NULL,
	[ApplyId] [varchar](50) NOT NULL,
	[UsedTime] [float] NOT NULL,
	[LimitedTime] [datetime] NOT NULL,
	[Marked] [int] NOT NULL,
	[Stage] [int] NOT NULL,
 CONSTRAINT [PK_UserAnswerRecord] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Examination]    Script Date: 2024/3/17 22:01:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Examination](
	[Id] [uniqueidentifier] NOT NULL,
	[Title] [varchar](200) NOT NULL,
	[Description] [varchar](max) NULL,
	[AssociationId] [varchar](50) NOT NULL,
	[AssociationTitle] [varchar](100) NOT NULL,
	[ExtraInfo] [varchar](max) NULL,
	[Remark] [varchar](500) NULL,
	[StartTime] [datetime] NOT NULL,
	[EndTime] [datetime] NOT NULL,
	[BaseScore] [float] NOT NULL,
	[BaseDuration] [float] NOT NULL,
	[ExamType] [int] NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[UpdatedAt] [datetime] NOT NULL,
	[CreatedBy] [varchar](50) NULL,
	[UpdatedBy] [varchar](50) NULL,
	[IsDeleted] [int] NOT NULL,
	[Status] [int] NOT NULL,
	[OrderIndex] [int] NOT NULL,
	[GroupCode] [varchar](50) NULL,
	[IsStrict] [int] NOT NULL,
 CONSTRAINT [PK_Examination] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Paper]    Script Date: 2024/3/17 22:01:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Paper](
	[Id] [uniqueidentifier] NOT NULL,
	[Title] [varchar](150) NULL,
	[Remark] [varchar](500) NULL,
	[Status] [int] NOT NULL,
	[ExamId] [uniqueidentifier] NOT NULL,
	[Score] [float] NOT NULL,
	[Duration] [float] NULL,
	[CreatedAt] [datetime] NOT NULL,
	[UpdatedAt] [datetime] NOT NULL,
	[CreatedBy] [varchar](50) NULL,
	[UpdatedBy] [varchar](50) NULL,
	[IsDeleted] [int] NOT NULL,
	[QuestionDetailJson] [varchar](max) NULL,
	[OpenResult] [int] NOT NULL,
	[PaperType] [int] NULL,
	[PaperDegree] [varchar](100) NOT NULL,
 CONSTRAINT [PK_Paper] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  View [dbo].[UserAnswerRecordView]    Script Date: 2024/3/17 22:01:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[UserAnswerRecordView]
AS
SELECT   dbo.UserAnswerRecord.Id, dbo.UserAnswerRecord.UserId, dbo.UserAnswerRecord.UserName, 
                dbo.UserAnswerRecord.PaperId, dbo.UserAnswerRecord.ExamId, dbo.UserAnswerRecord.SubmitAnswer, 
                dbo.UserAnswerRecord.Remark, dbo.UserAnswerRecord.CreatedAt, dbo.UserAnswerRecord.UpdatedAt, 
                dbo.UserAnswerRecord.CreatedBy, dbo.UserAnswerRecord.UpdatedBy, dbo.UserAnswerRecord.IsDeleted, 
                dbo.UserAnswerRecord.CheatCnt, dbo.UserAnswerRecord.Complated, dbo.UserAnswerRecord.ComplatedMode, 
                dbo.UserAnswerRecord.Score, dbo.UserAnswerRecord.IdNumber, dbo.UserAnswerRecord.ApplyId, 
                dbo.UserAnswerRecord.UsedTime, dbo.UserAnswerRecord.LimitedTime, dbo.UserAnswerRecord.Marked, 
                dbo.Paper.Title AS PaperTitle, dbo.Paper.Duration AS PaperDuration, dbo.Paper.PaperDegree, dbo.Paper.PaperType, 
                dbo.Paper.OpenResult, dbo.Paper.Status AS PaperStatus, dbo.Paper.CreatedAt AS PaperCreatedAt, 
                dbo.Examination.Title AS ExamTitle, dbo.Examination.AssociationId, dbo.Examination.AssociationTitle, 
                dbo.Examination.StartTime, dbo.Examination.EndTime, dbo.Examination.ExamType, 
                dbo.Examination.Status AS ExamStatus, dbo.Examination.GroupCode, dbo.Examination.IsStrict, 
                dbo.UserAnswerRecord.Stage
FROM      dbo.Paper RIGHT OUTER JOIN
                dbo.UserAnswerRecord ON dbo.Paper.Id = dbo.UserAnswerRecord.PaperId LEFT OUTER JOIN
                dbo.Examination ON dbo.UserAnswerRecord.ExamId = dbo.Examination.Id
GO
/****** Object:  Table [dbo].[Question]    Script Date: 2024/3/17 22:01:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Question](
	[Id] [bigint] NOT NULL,
	[Title] [varchar](max) NULL,
	[TitleText] [varchar](500) NULL,
	[Analysis] [varchar](max) NULL,
	[TypeId] [uniqueidentifier] NOT NULL,
	[SubjectId] [uniqueidentifier] NOT NULL,
	[Remark] [varchar](150) NULL,
	[Author] [varchar](50) NULL,
	[CreatedAt] [datetime] NOT NULL,
	[UpdatedAt] [datetime] NOT NULL,
	[CreatedBy] [varchar](50) NULL,
	[UpdatedBy] [varchar](50) NULL,
	[IsDeleted] [int] NOT NULL,
	[Score] [float] NOT NULL,
	[Degree] [varchar](20) NOT NULL,
	[IsOpen] [int] NOT NULL,
	[ColumnId] [varchar](50) NULL,
 CONSTRAINT [PK_Question] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Subject]    Script Date: 2024/3/17 22:01:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Subject](
	[Id] [uniqueidentifier] NOT NULL,
	[Caption] [varchar](150) NULL,
	[Remark] [varchar](200) NULL,
	[CreatedAt] [datetime] NOT NULL,
	[UpdatedAt] [datetime] NOT NULL,
	[CreatedBy] [varchar](50) NULL,
	[UpdatedBy] [nchar](10) NULL,
	[IsDeleted] [int] NULL,
 CONSTRAINT [PK_Subject] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[QuestionType]    Script Date: 2024/3/17 22:01:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[QuestionType](
	[Id] [uniqueidentifier] NOT NULL,
	[Caption] [varchar](100) NULL,
	[Remark] [varchar](200) NULL,
	[CreatedAt] [datetime] NOT NULL,
	[Objective] [int] NOT NULL,
	[UpdatedAt] [datetime] NOT NULL,
	[CreatedBy] [varchar](50) NULL,
	[UpdatedBy] [varchar](50) NULL,
	[IsDeleted] [int] NULL,
	[SingleAnswer] [int] NULL,
 CONSTRAINT [PK_QuestionType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[QuestionView]    Script Date: 2024/3/17 22:01:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[QuestionView]
AS
SELECT   dbo.Question.Id, dbo.Question.Title, dbo.Question.TitleText, dbo.Question.Analysis, dbo.Question.TypeId, 
                dbo.Question.SubjectId, dbo.Question.Remark, dbo.Question.Author, dbo.Question.CreatedAt, dbo.Question.CreatedBy, 
                dbo.Question.UpdatedAt, dbo.Question.UpdatedBy, dbo.Question.Score, dbo.Question.Degree, dbo.Question.IsOpen, 
                dbo.Question.IsDeleted, dbo.Subject.Caption AS SubjectName, dbo.QuestionType.Caption AS TypeName, 
                dbo.QuestionType.Objective, dbo.QuestionType.SingleAnswer, dbo.Question.ColumnId
FROM      dbo.Question INNER JOIN
                dbo.Subject ON dbo.Question.SubjectId = dbo.Subject.Id LEFT OUTER JOIN
                dbo.QuestionType ON dbo.Question.TypeId = dbo.QuestionType.Id
GO
/****** Object:  Table [cap].[Published]    Script Date: 2024/3/17 22:01:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [cap].[Published](
	[Id] [bigint] NOT NULL,
	[Version] [nvarchar](20) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Content] [nvarchar](max) NULL,
	[Retries] [int] NOT NULL,
	[Added] [datetime2](7) NOT NULL,
	[ExpiresAt] [datetime2](7) NULL,
	[StatusName] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_cap.Published] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [cap].[Received]    Script Date: 2024/3/17 22:01:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [cap].[Received](
	[Id] [bigint] NOT NULL,
	[Version] [nvarchar](20) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Group] [nvarchar](200) NULL,
	[Content] [nvarchar](max) NULL,
	[Retries] [int] NOT NULL,
	[Added] [datetime2](7) NOT NULL,
	[ExpiresAt] [datetime2](7) NULL,
	[StatusName] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_cap.Received] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Admin]    Script Date: 2024/3/17 22:01:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Admin](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[Email] [varchar](150) NOT NULL,
	[Mobile] [varchar](50) NOT NULL,
	[Description] [varchar](150) NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[UpdatedAt] [datetime] NOT NULL,
	[IsDeleted] [int] NOT NULL,
 CONSTRAINT [PK_Admin] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AdminRole]    Script Date: 2024/3/17 22:01:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AdminRole](
	[Id] [bigint] NOT NULL,
	[AdminId] [uniqueidentifier] NOT NULL,
	[RoleId] [bigint] NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[UpdatedAt] [datetime] NOT NULL,
	[IsDeleted] [int] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Face]    Script Date: 2024/3/17 22:01:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Face](
	[Id] [bigint] NOT NULL,
	[faceToken] [varchar](50) NULL,
	[faceInfo] [varchar](500) NULL,
	[createdAt] [datetime] NOT NULL,
	[updatedAt] [datetime] NOT NULL,
	[groupId] [varchar](50) NULL,
	[role] [varchar](20) NULL,
	[userId] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Face] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Menu]    Script Date: 2024/3/17 22:01:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Menu](
	[Id] [bigint] NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[Router] [varchar](200) NOT NULL,
	[ParentId] [bigint] NOT NULL,
	[Depth] [int] NOT NULL,
	[IsLeef] [int] NOT NULL,
	[Description] [varchar](200) NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[UpdatedAt] [datetime] NOT NULL,
	[PermissionId] [bigint] NOT NULL,
	[IsDeleted] [int] NOT NULL,
 CONSTRAINT [PK_Menu] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Permission]    Script Date: 2024/3/17 22:01:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Permission](
	[Id] [bigint] NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[Description] [varchar](200) NULL,
	[CreatedAt] [datetime] NOT NULL,
	[UpdatedAt] [datetime] NOT NULL,
	[DataFilterJson] [varchar](500) NOT NULL,
	[IsDeleted] [int] NOT NULL,
 CONSTRAINT [PK_Permission] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[QuestionItem]    Script Date: 2024/3/17 22:01:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[QuestionItem](
	[Id] [bigint] NOT NULL,
	[Description] [varchar](1000) NULL,
	[Code] [varchar](50) NULL,
	[Remark] [varchar](150) NULL,
	[Analysis] [varchar](500) NULL,
	[IsOption] [int] NOT NULL,
	[IsAnswer] [int] NOT NULL,
	[QuestionId] [bigint] NOT NULL,
	[CreatdAt] [datetime] NOT NULL,
	[UpdatedAt] [datetime] NOT NULL,
	[CreatedBy] [varchar](50) NULL,
	[UpdatedBy] [varchar](50) NULL,
	[IsDeleted] [int] NOT NULL,
	[DescriptionText] [varchar](500) NULL,
	[OrderIndex] [int] NULL,
 CONSTRAINT [PK_QuestionItem] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[QuestionTag]    Script Date: 2024/3/17 22:01:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[QuestionTag](
	[Id] [bigint] NOT NULL,
	[Title] [varchar](50) NULL,
	[Remark] [varchar](150) NULL,
	[CreatedAt] [datetime] NOT NULL,
	[UpdatedAt] [datetime] NOT NULL,
	[CreatedBy] [varchar](50) NULL,
	[UpdatedBy] [varchar](50) NULL,
	[IsDeleted] [int] NOT NULL,
	[PaperId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_QuestionTag] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Relation]    Script Date: 2024/3/17 22:01:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Relation](
	[Id] [uniqueidentifier] NOT NULL,
	[ExamId] [uniqueidentifier] NOT NULL,
	[PaperId] [uniqueidentifier] NOT NULL,
	[QuestionId] [bigint] NOT NULL,
	[Remark] [varchar](100) NULL,
	[CreatedAt] [datetime] NOT NULL,
	[UpdatedAt] [datetime] NOT NULL,
	[CreatedBy] [varchar](50) NULL,
	[UpdatedBy] [varchar](50) NULL,
	[IsDeleted] [int] NOT NULL,
	[TagId] [bigint] NOT NULL,
	[ItemScore] [float] NOT NULL,
 CONSTRAINT [PK_Relation] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Relation_2023]    Script Date: 2024/3/17 22:01:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Relation_2023](
	[Id] [uniqueidentifier] NOT NULL,
	[ExamId] [uniqueidentifier] NOT NULL,
	[PaperId] [uniqueidentifier] NOT NULL,
	[QuestionId] [bigint] NOT NULL,
	[Remark] [varchar](100) NULL,
	[CreatedAt] [datetime] NOT NULL,
	[UpdatedAt] [datetime] NOT NULL,
	[CreatedBy] [varchar](50) NULL,
	[UpdatedBy] [varchar](50) NULL,
	[IsDeleted] [int] NOT NULL,
	[TagId] [bigint] NOT NULL,
	[ItemScore] [float] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Relation_Bak]    Script Date: 2024/3/17 22:01:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Relation_Bak](
	[Id] [uniqueidentifier] NOT NULL,
	[ExamId] [uniqueidentifier] NOT NULL,
	[PaperId] [uniqueidentifier] NOT NULL,
	[QuestionId] [bigint] NOT NULL,
	[Remark] [varchar](100) NULL,
	[CreatedAt] [datetime] NOT NULL,
	[UpdatedAt] [datetime] NOT NULL,
	[CreatedBy] [varchar](50) NULL,
	[UpdatedBy] [varchar](50) NULL,
	[IsDeleted] [int] NOT NULL,
	[TagId] [bigint] NOT NULL,
	[ItemScore] [float] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Role]    Script Date: 2024/3/17 22:01:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Role](
	[Id] [bigint] NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[Description] [varchar](50) NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[UpdatedAt] [datetime] NOT NULL,
	[Type] [int] NOT NULL,
	[IsDeleted] [int] NOT NULL,
 CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RolePermission]    Script Date: 2024/3/17 22:01:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RolePermission](
	[Id] [bigint] NOT NULL,
	[RoleId] [bigint] NOT NULL,
	[PremissionId] [bigint] NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[UpdatedAt] [datetime] NOT NULL,
	[IsDeleted] [int] NOT NULL,
 CONSTRAINT [PK_RolePermission] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SubmitLog]    Script Date: 2024/3/17 22:01:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SubmitLog](
	[Id] [bigint] NOT NULL,
	[Urid] [bigint] NOT NULL,
	[SubmitAnswer] [varchar](max) NULL,
	[CreatedAt] [datetime] NOT NULL,
	[ComplatedMode] [int] NOT NULL,
 CONSTRAINT [PK_SubmitLog] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Table_1]    Script Date: 2024/3/17 22:01:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Table_1](
	[Id] [bigint] NOT NULL,
	[AdminId] [uniqueidentifier] NOT NULL,
	[RoleId] [bigint] NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[UPdatedAt] [datetime] NOT NULL,
 CONSTRAINT [PK_Table_1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ValidateExpression]    Script Date: 2024/3/17 22:01:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ValidateExpression](
	[Id] [bigint] NOT NULL,
	[ExamId] [uniqueidentifier] NULL,
	[PaperId] [uniqueidentifier] NULL,
	[Expression] [varchar](max) NULL,
	[Status] [int] NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[UpdatedAt] [datetime] NOT NULL,
	[CreatedBy] [varchar](50) NULL,
	[UpdatedBy] [varchar](50) NULL,
	[Remark] [varchar](2000) NULL,
	[IsDeleted] [int] NOT NULL,
	[ColumnId] [varchar](50) NOT NULL,
 CONSTRAINT [PK_ValidateExpression] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[Admin] ADD  CONSTRAINT [DF_Admin_Id]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[Admin] ADD  CONSTRAINT [DF_Admin_CreatedAt]  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Admin] ADD  CONSTRAINT [DF_Admin_UpdatedAt]  DEFAULT (getdate()) FOR [UpdatedAt]
GO
ALTER TABLE [dbo].[Admin] ADD  CONSTRAINT [DF_Admin_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[AdminRole] ADD  CONSTRAINT [DF_AdminRole_CreatedAt]  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[AdminRole] ADD  CONSTRAINT [DF_AdminRole_UpdatedAt]  DEFAULT (getdate()) FOR [UpdatedAt]
GO
ALTER TABLE [dbo].[AdminRole] ADD  CONSTRAINT [DF_AdminRole_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[Examination] ADD  CONSTRAINT [DF_Examination_Id]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[Examination] ADD  CONSTRAINT [DF_Examination_AssociationId]  DEFAULT ((0)) FOR [AssociationId]
GO
ALTER TABLE [dbo].[Examination] ADD  CONSTRAINT [DF_Examination_AssociationTitle]  DEFAULT ('无') FOR [AssociationTitle]
GO
ALTER TABLE [dbo].[Examination] ADD  CONSTRAINT [DF_Examination_Score]  DEFAULT ((100)) FOR [BaseScore]
GO
ALTER TABLE [dbo].[Examination] ADD  CONSTRAINT [DF_Examination_Duration]  DEFAULT ((15)) FOR [BaseDuration]
GO
ALTER TABLE [dbo].[Examination] ADD  CONSTRAINT [DF_Examination_ExamType]  DEFAULT ((0)) FOR [ExamType]
GO
ALTER TABLE [dbo].[Examination] ADD  CONSTRAINT [DF_Table_1_CreatedAy]  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Examination] ADD  CONSTRAINT [DF_Examination_UpdatedAt]  DEFAULT (getdate()) FOR [UpdatedAt]
GO
ALTER TABLE [dbo].[Examination] ADD  CONSTRAINT [DF_Examination_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[Examination] ADD  CONSTRAINT [DF_Examination_Status]  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[Examination] ADD  CONSTRAINT [DEFAULT_Examination_OrderIndex]  DEFAULT ((1)) FOR [OrderIndex]
GO
ALTER TABLE [dbo].[Examination] ADD  CONSTRAINT [DEFAULT_Examination_GroupCode]  DEFAULT ('') FOR [GroupCode]
GO
ALTER TABLE [dbo].[Examination] ADD  CONSTRAINT [DEFAULT_Examination_IsStrict]  DEFAULT ((0)) FOR [IsStrict]
GO
ALTER TABLE [dbo].[Face] ADD  CONSTRAINT [DF_Face_createdAt]  DEFAULT (getdate()) FOR [createdAt]
GO
ALTER TABLE [dbo].[Face] ADD  CONSTRAINT [DF_Face_updatedAt]  DEFAULT (getdate()) FOR [updatedAt]
GO
ALTER TABLE [dbo].[Face] ADD  CONSTRAINT [DF_Face_role]  DEFAULT ('’user‘') FOR [role]
GO
ALTER TABLE [dbo].[Face] ADD  CONSTRAINT [DF_Face_userId]  DEFAULT ('0') FOR [userId]
GO
ALTER TABLE [dbo].[Menu] ADD  CONSTRAINT [DF_Menu_ParentId]  DEFAULT ((0)) FOR [ParentId]
GO
ALTER TABLE [dbo].[Menu] ADD  CONSTRAINT [DF_Menu_Depth]  DEFAULT ((0)) FOR [Depth]
GO
ALTER TABLE [dbo].[Menu] ADD  CONSTRAINT [DF_Menu_IsLeef]  DEFAULT ((0)) FOR [IsLeef]
GO
ALTER TABLE [dbo].[Menu] ADD  CONSTRAINT [DF_Menu_CreatedAt]  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Menu] ADD  CONSTRAINT [DF_Menu_UpdatedAt]  DEFAULT (getdate()) FOR [UpdatedAt]
GO
ALTER TABLE [dbo].[Menu] ADD  CONSTRAINT [DF_Menu_PermissionId]  DEFAULT ((0)) FOR [PermissionId]
GO
ALTER TABLE [dbo].[Menu] ADD  CONSTRAINT [DF_Menu_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[Paper] ADD  CONSTRAINT [DF_Paper_Id]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[Paper] ADD  CONSTRAINT [DF_Paper_Status]  DEFAULT ((0)) FOR [Status]
GO
ALTER TABLE [dbo].[Paper] ADD  CONSTRAINT [DF_Paper_CreatedAt]  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Paper] ADD  CONSTRAINT [DF_Paper_UpdatedAt]  DEFAULT (getdate()) FOR [UpdatedAt]
GO
ALTER TABLE [dbo].[Paper] ADD  CONSTRAINT [DF_Paper_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[Paper] ADD  CONSTRAINT [DF_Paper_Instant]  DEFAULT ((0)) FOR [OpenResult]
GO
ALTER TABLE [dbo].[Paper] ADD  CONSTRAINT [DF_Paper_PaperType]  DEFAULT ((0)) FOR [PaperType]
GO
ALTER TABLE [dbo].[Paper] ADD  CONSTRAINT [DEFAULT_Paper_PaperDegree]  DEFAULT ('all') FOR [PaperDegree]
GO
ALTER TABLE [dbo].[Permission] ADD  CONSTRAINT [DF_Permission_CreatedAt]  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Permission] ADD  CONSTRAINT [DF_Permission_UpdatedAt]  DEFAULT (getdate()) FOR [UpdatedAt]
GO
ALTER TABLE [dbo].[Permission] ADD  CONSTRAINT [DF_Permission_DataFilterJson]  DEFAULT ('') FOR [DataFilterJson]
GO
ALTER TABLE [dbo].[Permission] ADD  CONSTRAINT [DF_Permission_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[Question] ADD  CONSTRAINT [DF_Question_CreatedAt]  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Question] ADD  CONSTRAINT [DF_Question_UpdatedAt]  DEFAULT (getdate()) FOR [UpdatedAt]
GO
ALTER TABLE [dbo].[Question] ADD  CONSTRAINT [DF_Question_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[Question] ADD  CONSTRAINT [DF_Question_Score]  DEFAULT ((0)) FOR [Score]
GO
ALTER TABLE [dbo].[Question] ADD  CONSTRAINT [DF_Question_Degree]  DEFAULT ('normal') FOR [Degree]
GO
ALTER TABLE [dbo].[Question] ADD  CONSTRAINT [DF_Question_ForTest]  DEFAULT ((1)) FOR [IsOpen]
GO
ALTER TABLE [dbo].[Question] ADD  CONSTRAINT [DF_Question_ColumnId]  DEFAULT ('0') FOR [ColumnId]
GO
ALTER TABLE [dbo].[QuestionItem] ADD  CONSTRAINT [DF_QuestionItem_Answer]  DEFAULT ('') FOR [Analysis]
GO
ALTER TABLE [dbo].[QuestionItem] ADD  CONSTRAINT [DF_QuestionItem_IsOption]  DEFAULT ((1)) FOR [IsOption]
GO
ALTER TABLE [dbo].[QuestionItem] ADD  CONSTRAINT [DF_QuestionItem_IsAnswer]  DEFAULT ((0)) FOR [IsAnswer]
GO
ALTER TABLE [dbo].[QuestionItem] ADD  CONSTRAINT [DF_QuestionItem_QuestionId]  DEFAULT ((0)) FOR [QuestionId]
GO
ALTER TABLE [dbo].[QuestionItem] ADD  CONSTRAINT [DF_QuestionItem_CreatdAt]  DEFAULT (getdate()) FOR [CreatdAt]
GO
ALTER TABLE [dbo].[QuestionItem] ADD  CONSTRAINT [DF_QuestionItem_UpdatedAt]  DEFAULT (getdate()) FOR [UpdatedAt]
GO
ALTER TABLE [dbo].[QuestionItem] ADD  CONSTRAINT [DF_QuestionItem_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[QuestionItem] ADD  CONSTRAINT [DF_QuestionItem_OrderIndex]  DEFAULT ((0)) FOR [OrderIndex]
GO
ALTER TABLE [dbo].[QuestionTag] ADD  CONSTRAINT [DF_QuestionTag_CreatedAt]  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[QuestionTag] ADD  CONSTRAINT [DF_QuestionTag_UpdatedAt]  DEFAULT (getdate()) FOR [UpdatedAt]
GO
ALTER TABLE [dbo].[QuestionTag] ADD  CONSTRAINT [DF_QuestionTag_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[QuestionType] ADD  CONSTRAINT [DF_QuestionType_Id]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[QuestionType] ADD  CONSTRAINT [DF_QuestionType_CreatedAt]  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[QuestionType] ADD  CONSTRAINT [DF_QuestionType_Objective]  DEFAULT ((1)) FOR [Objective]
GO
ALTER TABLE [dbo].[QuestionType] ADD  CONSTRAINT [DF_QuestionType_UpdatedAt]  DEFAULT (getdate()) FOR [UpdatedAt]
GO
ALTER TABLE [dbo].[QuestionType] ADD  CONSTRAINT [DF_QuestionType_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[QuestionType] ADD  CONSTRAINT [DF_QuestionType_SingleAnswer]  DEFAULT ((1)) FOR [SingleAnswer]
GO
ALTER TABLE [dbo].[Relation] ADD  CONSTRAINT [DF_RelationExamTagQuestion_Id]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[Relation] ADD  CONSTRAINT [DF_RelationExamTagQuestion_CreatedAt]  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Relation] ADD  CONSTRAINT [DF_RelationExamTagQuestion_UpdatedAt]  DEFAULT (getdate()) FOR [UpdatedAt]
GO
ALTER TABLE [dbo].[Relation] ADD  CONSTRAINT [DF_RelationExamTagQuestion_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[Relation] ADD  CONSTRAINT [DF_Relation_TagId]  DEFAULT ((0)) FOR [TagId]
GO
ALTER TABLE [dbo].[Relation] ADD  CONSTRAINT [DF_Relation_Score]  DEFAULT ((0)) FOR [ItemScore]
GO
ALTER TABLE [dbo].[Role] ADD  CONSTRAINT [DF_Role_Description]  DEFAULT ('无') FOR [Description]
GO
ALTER TABLE [dbo].[Role] ADD  CONSTRAINT [DF_Role_CreatedAt]  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Role] ADD  CONSTRAINT [DF_Role_UpdatedAt]  DEFAULT (getdate()) FOR [UpdatedAt]
GO
ALTER TABLE [dbo].[Role] ADD  CONSTRAINT [DF_Role_Type]  DEFAULT ((0)) FOR [Type]
GO
ALTER TABLE [dbo].[Role] ADD  CONSTRAINT [DF_Role_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[RolePermission] ADD  CONSTRAINT [DF_RolePermission_CreatedAt]  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[RolePermission] ADD  CONSTRAINT [DF_RolePermission_UpdatedAt]  DEFAULT (getdate()) FOR [UpdatedAt]
GO
ALTER TABLE [dbo].[RolePermission] ADD  CONSTRAINT [DF_RolePermission_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[Subject] ADD  CONSTRAINT [DF_Subject_Id]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[Subject] ADD  CONSTRAINT [DF_Subject_CreatedAt]  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Subject] ADD  CONSTRAINT [DF_Subject_UpdatedAt]  DEFAULT (getdate()) FOR [UpdatedAt]
GO
ALTER TABLE [dbo].[Subject] ADD  CONSTRAINT [DF_Subject_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[SubmitLog] ADD  CONSTRAINT [DF_SubmitLog_SubmitAnswer]  DEFAULT ('') FOR [SubmitAnswer]
GO
ALTER TABLE [dbo].[SubmitLog] ADD  CONSTRAINT [DF_SubmitLog_CreatedAt]  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[SubmitLog] ADD  CONSTRAINT [DF_SubmitLog_Complated]  DEFAULT ((0)) FOR [ComplatedMode]
GO
ALTER TABLE [dbo].[Table_1] ADD  CONSTRAINT [DF_Table_1_CreatedAt]  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[UserAnswerRecord] ADD  CONSTRAINT [DF_UserAnswerRecord_CreatedAt]  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[UserAnswerRecord] ADD  CONSTRAINT [DF_UserAnswerRecord_UpdatedAt]  DEFAULT (getdate()) FOR [UpdatedAt]
GO
ALTER TABLE [dbo].[UserAnswerRecord] ADD  CONSTRAINT [DF_UserAnswerRecord_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[UserAnswerRecord] ADD  CONSTRAINT [DF_UserAnswerRecord_CheatCnt]  DEFAULT ((0)) FOR [CheatCnt]
GO
ALTER TABLE [dbo].[UserAnswerRecord] ADD  CONSTRAINT [DF_UserAnswerRecord_Complated]  DEFAULT ((0)) FOR [Complated]
GO
ALTER TABLE [dbo].[UserAnswerRecord] ADD  CONSTRAINT [DF_UserAnswerRecord_ComplatedMode]  DEFAULT ((0)) FOR [ComplatedMode]
GO
ALTER TABLE [dbo].[UserAnswerRecord] ADD  CONSTRAINT [DF_UserAnswerRecord_Score]  DEFAULT ((0)) FOR [Score]
GO
ALTER TABLE [dbo].[UserAnswerRecord] ADD  CONSTRAINT [DF_UserAnswerRecord_IdNumber]  DEFAULT ('') FOR [IdNumber]
GO
ALTER TABLE [dbo].[UserAnswerRecord] ADD  CONSTRAINT [DF_UserAnswerRecord_ApplyId]  DEFAULT ('') FOR [ApplyId]
GO
ALTER TABLE [dbo].[UserAnswerRecord] ADD  CONSTRAINT [DF_UserAnswerRecord_UsedTime]  DEFAULT ((0)) FOR [UsedTime]
GO
ALTER TABLE [dbo].[UserAnswerRecord] ADD  CONSTRAINT [DF__UserAnswe__Limit__00DF2177]  DEFAULT (dateadd(second,(-1),dateadd(day,(1),CONVERT([datetime],CONVERT([varchar](10),getdate(),(120)))))) FOR [LimitedTime]
GO
ALTER TABLE [dbo].[UserAnswerRecord] ADD  CONSTRAINT [DF__UserAnswe__Marke__01D345B0]  DEFAULT ((0)) FOR [Marked]
GO
ALTER TABLE [dbo].[UserAnswerRecord] ADD  CONSTRAINT [DF_UserAnswerRecord_Stage]  DEFAULT ((1)) FOR [Stage]
GO
ALTER TABLE [dbo].[ValidateExpression] ADD  CONSTRAINT [DF_ValidateExpression_Status]  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[ValidateExpression] ADD  CONSTRAINT [DF_ValidateExpression_CreatedAt]  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[ValidateExpression] ADD  CONSTRAINT [DF_ValidateExpression_UpdatedAt]  DEFAULT (getdate()) FOR [UpdatedAt]
GO
ALTER TABLE [dbo].[ValidateExpression] ADD  CONSTRAINT [DF_ValidateExpression_CreatedBy]  DEFAULT ('') FOR [CreatedBy]
GO
ALTER TABLE [dbo].[ValidateExpression] ADD  CONSTRAINT [DF_ValidateExpression_UpdatedBy]  DEFAULT ('') FOR [UpdatedBy]
GO
ALTER TABLE [dbo].[ValidateExpression] ADD  CONSTRAINT [DF_ValidateExpression_Remark]  DEFAULT ('无') FOR [Remark]
GO
ALTER TABLE [dbo].[ValidateExpression] ADD  CONSTRAINT [DF_ValidateExpression_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[ValidateExpression] ADD  CONSTRAINT [DF_ValidateExpression_ColumnId]  DEFAULT ((0)) FOR [ColumnId]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'标准总分数，若试卷未指定总分，则沿用该分数' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Examination', @level2type=N'COLUMN',@level2name=N'BaseScore'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'标准考试时长，单位：分钟，若生成得试卷没有指定时长，则沿用该时长' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Examination', @level2type=N'COLUMN',@level2name=N'BaseDuration'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'状态' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Examination', @level2type=N'COLUMN',@level2name=N'Status'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'聚合码，设置之后，可以生成聚合多个考试的二维码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Examination', @level2type=N'COLUMN',@level2name=N'GroupCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否严格交卷，是的话超时提交将不给分，否的话答考试结束后有多少分给多少分，1-是，0-否' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Examination', @level2type=N'COLUMN',@level2name=N'IsStrict'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'人脸标识' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Face', @level2type=N'COLUMN',@level2name=N'faceToken'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'人脸信息（json字符串）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Face', @level2type=N'COLUMN',@level2name=N'faceInfo'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'组别，{角色}_{活动名称}_{年份/年月}' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Face', @level2type=N'COLUMN',@level2name=N'groupId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'角色组，user-用户，admin-管理员' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Face', @level2type=N'COLUMN',@level2name=N'role'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'用户id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Face', @level2type=N'COLUMN',@level2name=N'userId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'试卷的卷面总分，默认继承examination的基础分数' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Paper', @level2type=N'COLUMN',@level2name=N'Score'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'试卷的考试时间，默认继承examination的时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Paper', @level2type=N'COLUMN',@level2name=N'Duration'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'抽题情况' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Paper', @level2type=N'COLUMN',@level2name=N'QuestionDetailJson'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否公开成绩查询，0-不公开，1-公开' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Paper', @level2type=N'COLUMN',@level2name=N'OpenResult'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'0-机器组卷（考试之前先随机生成多套试卷，考试时随机分发，推荐采用此方式，考试前生成多套试卷既满足随机性，也可以对不同的试卷提前校验，避免疏漏，默认），1-人工组卷（严谨的场合下适用，比如统一考试等），2-即时组卷（每个学生考试时随机生成，不推荐正式考试用这种方式，无法对试卷进行校验）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Paper', @level2type=N'COLUMN',@level2name=N'PaperType'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'title是通过富文本写入的，在列表页展示时，需要取消html字符，这里单独存一个500字符以内的纯文本标题用作列表展示' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Question', @level2type=N'COLUMN',@level2name=N'TitleText'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'题目解析' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Question', @level2type=N'COLUMN',@level2name=N'Analysis'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'出题人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Question', @level2type=N'COLUMN',@level2name=N'Author'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'题目难度，easy,normal,difficult三种，默认normal' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Question', @level2type=N'COLUMN',@level2name=N'Degree'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否是公开考题，设置否的话，生成练习题的时候会避开抽到该题，默认是' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Question', @level2type=N'COLUMN',@level2name=N'IsOpen'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'分析' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'QuestionItem', @level2type=N'COLUMN',@level2name=N'Analysis'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否是选项，如果是则生成试卷时渲染到页面，如过不是就不渲染，对应的description里就是答案了' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'QuestionItem', @level2type=N'COLUMN',@level2name=N'IsOption'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否是答案' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'QuestionItem', @level2type=N'COLUMN',@level2name=N'IsAnswer'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'选项内容（description）是通过富文本写入的，在列表页展示时，需要取消html字符，这里单独存一个500字符以内的纯文本标题用作列表展示' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'QuestionItem', @level2type=N'COLUMN',@level2name=N'DescriptionText'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'顺序' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'QuestionItem', @level2type=N'COLUMN',@level2name=N'OrderIndex'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否是客观题，即有标准答案' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'QuestionType', @level2type=N'COLUMN',@level2name=N'Objective'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'只有1个答案，1-是，其他-不是' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'QuestionType', @level2type=N'COLUMN',@level2name=N'SingleAnswer'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Relation', @level2type=N'COLUMN',@level2name=N'ItemScore'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'0-普通角色，1-系统级角色' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Role', @level2type=N'COLUMN',@level2name=N'Type'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'作弊次数' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserAnswerRecord', @level2type=N'COLUMN',@level2name=N'CheatCnt'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否完成考试' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserAnswerRecord', @level2type=N'COLUMN',@level2name=N'Complated'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'交卷方式，0-自主交卷，1-到时间自动交卷，2-作弊次数过多强制交卷' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserAnswerRecord', @level2type=N'COLUMN',@level2name=N'ComplatedMode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'答题已经消耗的时长' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserAnswerRecord', @level2type=N'COLUMN',@level2name=N'UsedTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'参与的第几阶段答题' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserAnswerRecord', @level2type=N'COLUMN',@level2name=N'Stage'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "Question"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 329
               Right = 192
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Subject"
            Begin Extent = 
               Top = 185
               Left = 319
               Bottom = 325
               Right = 473
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "QuestionType"
            Begin Extent = 
               Top = 6
               Left = 230
               Bottom = 146
               Right = 396
            End
            DisplayFlags = 280
            TopColumn = 6
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 3855
         Alias = 900
         Table = 1740
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'QuestionView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'QuestionView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[43] 4[18] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "Paper"
            Begin Extent = 
               Top = 198
               Left = 395
               Bottom = 338
               Right = 595
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Examination"
            Begin Extent = 
               Top = 36
               Left = 398
               Bottom = 176
               Right = 577
            End
            DisplayFlags = 280
            TopColumn = 14
         End
         Begin Table = "UserAnswerRecord"
            Begin Extent = 
               Top = 43
               Left = 30
               Bottom = 183
               Right = 218
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 4035
         Alias = 2400
         Table = 2070
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'UserAnswerRecordView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'UserAnswerRecordView'
GO
