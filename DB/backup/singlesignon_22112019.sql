USE [singlesignon]
GO
/****** Object:  UserDefinedFunction [dbo].[CheckDuplicateUpdateUserInfo]    Script Date: 11/22/2019 10:12:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[CheckDuplicateUpdateUserInfo] (
	@UserId int,
	@Email nvarchar(256),
	@PhoneNumber nvarchar(50)
)
RETURNS int
AS BEGIN
	DECLARE @Result int;
	DECLARE @Checker int;

	SET @Checker = 0;
	/*
    EmailAlreadyUsed = 105;
    PhoneAlreadyUsed = 106;
	*/
	SET @Result = 0;

	IF(@Checker = 0)
	BEGIN
		--Checking with Email
		SET @Checker = (SELECT TOP 1 t.Id FROM tbl_users t WHERE 1=1 AND t.Email IS NOT NULL AND t.Email = @Email AND t.Id != @UserId);

		IF(@Checker > 0)
		BEGIN
			--Email already used: Code 105
			SET @Result = 105;
		END		
	END

	IF @Result = 0 AND LEN(ISNULL(@PhoneNumber, '')) > 0
	BEGIN 
		--Checking with PhoneNumber
		SET @Checker = (SELECT TOP 1 t.Id FROM tbl_users t WHERE 1=1 AND t.PhoneNumber = @PhoneNumber AND t.Id != @UserId);

		IF(@Checker> 0)
		BEGIN
			--PhoneNumber already used: Code 106
			SET @Result = 106;
		END	
	END

    RETURN @Result;
END
GO
/****** Object:  UserDefinedFunction [dbo].[CheckDuplicateUserInfo]    Script Date: 11/22/2019 10:12:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[CheckDuplicateUserInfo] (
	@UserName nvarchar (128),
	@Email nvarchar(256),
	@PhoneNumber nvarchar(50),
	@IDCard nvarchar(50)
)
RETURNS VARCHAR(250)
AS BEGIN
	DECLARE @Result int;
	DECLARE @Checker nvarchar(128);
	/*
	UserNameAlreadyUsed = 104;
    EmailAlreadyUsed = 105;
    PhoneAlreadyUsed = 106;
	IDCardAlreadyUsed = 107;
	*/
	SET @Result = 1;
    SET @Checker = (SELECT TOP 1 t.Id FROM tbl_users t WHERE 1=1 AND t.UserName = @UserName);
	
	IF(@Checker IS NOT NULL)
	BEGIN
		--Username already used: Code 104
		SET @Result = 104;

		RETURN @Result;
	END

	IF(@Checker IS NULL)
	BEGIN
		--Checking with Email
		SET @Checker = (SELECT TOP 1 t.Id FROM tbl_users t WHERE 1=1 AND t.Email = @Email);

		IF(@Checker IS NOT NULL)
		--Email already used: Code 105
		SET @Result = 105;

		RETURN @Result;
	END

	IF(@Checker IS NULL)
	BEGIN
		--Checking with PhoneNumber
		SET @Checker = (SELECT TOP 1 t.Id FROM tbl_users t WHERE 1=1 AND t.PhoneNumber = @PhoneNumber);

		IF(@Checker IS NOT NULL)
		--PhoneNumber already used: Code 106
		SET @Result = 106;

		RETURN @Result;
	END

	--IF(@Checker IS NULL)
	--BEGIN
	--	--Checking with Email
	--	SET @Checker = (SELECT TOP 1 t.Id FROM tbl_users t WHERE 1=1 AND t.IDCard = @IDCard);

	--	IF(@Checker IS NOT NULL)
	--	--IDCard already used: Code 106
	--	SET @Result = 107;
	--END

    RETURN @Result;
END
GO
/****** Object:  UserDefinedFunction [dbo].[CheckUserToken]    Script Date: 11/22/2019 10:12:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[CheckUserToken]
(
	@UserId int,
	@TokenKey nvarchar(128)
)
RETURNS int
AS
BEGIN
	DECLARE @Result int;
	SET @Result = 0;

	SET @Result = (SELECT TOP 1 COUNT(1) FROM tbl_users t 
		LEFT JOIN tbl_user_tokenkeys k on t.Id = k.UserId
		WHERE 1=1 
		AND t.Id = @UserId
		AND k.TokenKey = @TokenKey
		--AND ( GETDATE() BETWEEN k.CreatedDate AND k.ExpiredDate)
	)
	;

	RETURN @Result;

END
GO
/****** Object:  UserDefinedFunction [dbo].[HTMLBody]    Script Date: 11/22/2019 10:12:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*
*   SSMA informational messages:
*   M2SS0003: The following SQL clause was ignored during conversion:
*   DEFINER = `kplus`@`%`.
*/

/*
*   SSMA informational messages:
*   M2SS0003: The following SQL clause was ignored during conversion: DETERMINISTIC.
*/

CREATE FUNCTION [dbo].[HTMLBody] 
( 
   @Msg nvarchar(max)
)
/*
*   SSMA informational messages:
*   M2SS0055: Data type was converted to VARCHAR(MAX) according to character set mapping for latin1 character set
*/

RETURNS varchar(max)
AS 
   BEGIN

      DECLARE
         @tmpMsg nvarchar(max)

      /* 
      *   SSMA error messages:
      *   M2SS0201: MySQL standard function DATE_FORMAT is not supported in current SSMA version

      SET @tmpMsg = CAST(
         (N'Date: ')
          + 
         (DATE_FORMAT(getdate(), N'%e %b %Y %H:%i:%S -0600'))
          + 
         (NCHAR(13)+NCHAR(10))
          + 
         (N'MIME-Version: 1.0')
          + 
         (NCHAR(13)+NCHAR(10))
          + 
         (N'Content-Type: multipart/alternative;')
          + 
         (NCHAR(13)+NCHAR(10))
          + 
         (N' boundary="----=_NextPart_000_0000_01CA4B3F.8C263EE0"')
          + 
         (NCHAR(13)+NCHAR(10))
          + 
         (N'Content-Class: urn:content-classes:message')
          + 
         (NCHAR(13)+NCHAR(10))
          + 
         (N'Importance: normal')
          + 
         (NCHAR(13)+NCHAR(10))
          + 
         (N'Priority: normal')
          + 
         (NCHAR(13)+NCHAR(10))
          + 
         (N'')
          + 
         (NCHAR(13)+NCHAR(10))
          + 
         (N'')
          + 
         (NCHAR(13)+NCHAR(10))
          + 
         (N'This is a multi-part message in MIME format.')
          + 
         (NCHAR(13)+NCHAR(10))
          + 
         (N'')
          + 
         (NCHAR(13)+NCHAR(10))
          + 
         (N'------=_NextPart_000_0000_01CA4B3F.8C263EE0')
          + 
         (NCHAR(13)+NCHAR(10))
          + 
         (N'Content-Type: text/plain;')
          + 
         (NCHAR(13)+NCHAR(10))
          + 
         (N'  charset="iso-8859-1"')
          + 
         (NCHAR(13)+NCHAR(10))
          + 
         (N'Content-Transfer-Encoding: 7bit')
          + 
         (NCHAR(13)+NCHAR(10))
          + 
         (N'')
          + 
         (NCHAR(13)+NCHAR(10))
          + 
         (N'')
          + 
         (NCHAR(13)+NCHAR(10))
          + 
         (@Msg)
          + 
         (NCHAR(13)+NCHAR(10))
          + 
         (N'')
          + 
         (NCHAR(13)+NCHAR(10))
          + 
         (N'')
          + 
         (NCHAR(13)+NCHAR(10))
          + 
         (N'------=_NextPart_000_0000_01CA4B3F.8C263EE0')
          + 
         (NCHAR(13)+NCHAR(10))
          + 
         (N'Content-Type: text/html')
          + 
         (NCHAR(13)+NCHAR(10))
          + 
         (N'Content-Transfer-Encoding: 7bit')
          + 
         (NCHAR(13)+NCHAR(10))
          + 
         (N'')
          + 
         (NCHAR(13)+NCHAR(10))
          + 
         (@Msg)
          + 
         (NCHAR(13)+NCHAR(10))
          + 
         (N'------=_NextPart_000_0000_01CA4B3F.8C263EE0--') AS nchar(1))
      */



      RETURN @tmpMsg

   END
GO
/****** Object:  UserDefinedFunction [dbo].[LikedCounter]    Script Date: 11/22/2019 10:12:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
Create FUNCTION [dbo].[LikedCounter]
(
	@UserId int
)
RETURNS int
AS
BEGIN
	DECLARE @Total int;
	SET @Total = 0;

	SET @Total = (SELECT COUNT(1) FROM tbl_post_actions t 
		WHERE 1=1 
		AND t.UserId = @UserId
		AND t.ActionType = 'LIKE'
	);

	RETURN @Total;

END
GO
/****** Object:  UserDefinedFunction [dbo].[LocationCounter]    Script Date: 11/22/2019 10:12:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
create FUNCTION [dbo].[LocationCounter]
(
	@UserId int
)
RETURNS int
AS
BEGIN
	DECLARE @Total int;
	SET @Total = 0;

	SET @Total = (SELECT SUM(t.TotalLocations) FROM tbl_post_data t 
		WHERE 1=1 
		AND t.UserId = @UserId		
	);

	RETURN @Total;

END
GO
/****** Object:  UserDefinedFunction [dbo].[PhotoCounter]    Script Date: 11/22/2019 10:12:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[PhotoCounter]
(
	@UserId int
)
RETURNS int
AS
BEGIN
	DECLARE @Total int;
	SET @Total = 0;

	SET @Total = (SELECT SUM(t.TotalImages) FROM tbl_post_data t 
		WHERE 1=1 
		AND t.UserId = @UserId		
	);

	RETURN @Total;

END
GO
/****** Object:  UserDefinedFunction [dbo].[Post_CheckLike]    Script Date: 11/22/2019 10:12:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[Post_CheckLike]
(	
	@PostId int,
	@UserId int
)
RETURNS BIT
AS
BEGIN
DECLARE @Result bit

SET @Result=0;
IF(Exists(SELECT * FROM tbl_post_actions WHERE 1=1 AND PostId=@PostId AND UserId=@UserId)) SET @Result=1;

RETURN @Result;
END
GO
/****** Object:  UserDefinedFunction [dbo].[Post_GetRatingScore]    Script Date: 11/22/2019 10:12:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[Post_GetRatingScore]
(	
	@PostId int,
	@UserId int
)
RETURNS DECIMAL(18,2)
AS
BEGIN
DECLARE @Result DECIMAL(18,2)

SET @Result=0;

IF(Exists(SELECT * FROM tbl_post_actions WHERE 1=1 AND PostId=@PostId AND UserId=@UserId AND ActionType='RATINGSCORE')) 

SET @Result=(SELECT RatingScore FROM tbl_post_actions WHERE 1=1 AND PostId=@PostId AND UserId=@UserId AND ActionType='RATINGSCORE');

RETURN @Result;
END
GO
/****** Object:  Table [dbo].[tbl_access_roles]    Script Date: 11/22/2019 10:12:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_access_roles](
	[RoleId] [nvarchar](128) NOT NULL,
	[OperationId] [nvarchar](128) NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_accesses]    Script Date: 11/22/2019 10:12:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_accesses](
	[Id] [nvarchar](128) NOT NULL,
	[AccessName] [nvarchar](50) NOT NULL,
	[Active] [smallint] NULL,
	[Description] [nvarchar](255) NULL,
 CONSTRAINT [PK_aspnetaccess_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_activitylogs]    Script Date: 11/22/2019 10:12:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_activitylogs](
	[ActivityLogId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[ActivityText] [nvarchar](max) NULL,
	[TargetType] [nvarchar](50) NULL,
	[TargetId] [nvarchar](128) NULL,
	[IPAddress] [nvarchar](20) NULL,
	[ActivityDate] [datetime] NOT NULL,
	[ActivityType] [nvarchar](50) NULL,
 CONSTRAINT [PK_aspnetactivitylog_ActivityLogId] PRIMARY KEY CLUSTERED 
(
	[ActivityLogId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_categories]    Script Date: 11/22/2019 10:12:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_categories](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](128) NOT NULL,
	[Code] [nvarchar](20) NULL,
	[Icon] [nvarchar](20) NULL,
	[CreatedBy] [nvarchar](128) NULL,
	[CreatedDate] [datetime] NULL,
	[LastUpdated] [datetime] NULL,
	[LastUpdatedBy] [nvarchar](128) NULL,
	[Status] [smallint] NULL,
 CONSTRAINT [PK_tbl_category] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_cmn_settings]    Script Date: 11/22/2019 10:12:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_cmn_settings](
	[SettingName] [nvarchar](100) NOT NULL,
	[SettingType] [nvarchar](100) NOT NULL,
	[SettingValue] [nvarchar](2000) NULL,
 CONSTRAINT [PK_cmn_settings_SettingName] PRIMARY KEY CLUSTERED 
(
	[SettingName] ASC,
	[SettingType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_cmn_sql_errors]    Script Date: 11/22/2019 10:12:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_cmn_sql_errors](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ErrorMessage] [nvarchar](max) NULL,
	[ErrorServerity] [nvarchar](50) NULL,
	[ErrorState] [int] NULL,
	[ErrorLine] [int] NULL,
	[Actor] [nvarchar](128) NULL,
	[DateOfIssue] [datetime] NULL,
 CONSTRAINT [PK_cmn_sql_errors] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_document_api]    Script Date: 11/22/2019 10:12:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_document_api](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[LinkUrl] [nvarchar](500) NULL,
	[Data] [ntext] NULL,
 CONSTRAINT [PK_HistoryApi] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_domains]    Script Date: 11/22/2019 10:12:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_domains](
	[DomainKey] [nvarchar](128) NOT NULL,
	[DomainName] [nvarchar](128) NULL,
	[LoginDurations] [int] NOT NULL,
	[Status] [smallint] NOT NULL,
	[Des] [nvarchar](256) NULL,
 CONSTRAINT [PK_aspnetusergroups_1] PRIMARY KEY CLUSTERED 
(
	[DomainKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_log4netfiles]    Script Date: 11/22/2019 10:12:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_log4netfiles](
	[FileId] [nvarchar](128) NOT NULL,
	[FolderPath] [nvarchar](max) NULL,
	[FileName] [nvarchar](max) NULL,
	[FullPath] [nvarchar](max) NULL,
	[DateCreated] [datetime2](0) NULL,
	[CurrentItem] [int] NULL,
	[FileStatus] [int] NULL,
	[MachineName] [nvarchar](100) NULL,
	[MachineIP] [nvarchar](100) NULL,
	[LastUpdated] [datetime2](0) NULL,
	[Comments] [nvarchar](max) NULL,
	[AppName] [nvarchar](200) NULL,
 CONSTRAINT [PK_log4netfiles_FileId] PRIMARY KEY CLUSTERED 
(
	[FileId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_log4netrecords]    Script Date: 11/22/2019 10:12:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_log4netrecords](
	[EntryId] [nvarchar](128) NOT NULL,
	[Item] [int] NULL,
	[TimeStamp] [datetime2](0) NULL,
	[Level] [nvarchar](15) NULL,
	[Thread] [nvarchar](50) NULL,
	[Message] [nvarchar](max) NULL,
	[MachineName] [nvarchar](200) NULL,
	[UserName] [nvarchar](200) NULL,
	[HostName] [nvarchar](200) NULL,
	[App] [nvarchar](200) NULL,
	[Throwable] [nvarchar](max) NULL,
	[Class] [nvarchar](500) NULL,
	[Method] [nvarchar](100) NULL,
	[File] [nvarchar](max) NULL,
	[Line] [nvarchar](20) NULL,
	[DateCreated] [datetime2](0) NULL,
	[LogPath] [nvarchar](max) NULL,
	[FileId] [nvarchar](128) NULL,
 CONSTRAINT [PK_log4netrecords_EntryId] PRIMARY KEY CLUSTERED 
(
	[EntryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_log4netrecords_exceptions]    Script Date: 11/22/2019 10:12:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_log4netrecords_exceptions](
	[EntryId] [nvarchar](128) NOT NULL,
	[TimeStamp] [datetime2](0) NULL,
	[Message] [nvarchar](max) NULL,
	[MachineName] [nvarchar](200) NULL,
	[HostName] [nvarchar](200) NULL,
	[AppName] [nvarchar](200) NULL,
	[Throwable] [nvarchar](max) NULL,
 CONSTRAINT [PK_log4netrecords_exceptions_EntryId] PRIMARY KEY CLUSTERED 
(
	[EntryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_roles]    Script Date: 11/22/2019 10:12:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_roles](
	[Id] [nvarchar](128) NOT NULL,
	[Name] [nvarchar](256) NOT NULL,
 CONSTRAINT [PK_aspnetroles_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_social_provider]    Script Date: 11/22/2019 10:12:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_social_provider](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](50) NOT NULL,
	[Name] [nvarchar](100) NULL,
	[Status] [bit] NULL,
	[ClientId] [nvarchar](500) NULL,
	[ClientSecret] [nvarchar](500) NULL,
 CONSTRAINT [PK_tbl_social_provider_1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_system_emails]    Script Date: 11/22/2019 10:12:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_system_emails](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Subject] [nvarchar](500) NULL,
	[Body] [ntext] NULL,
	[Sender] [nvarchar](128) NULL,
	[Receiver] [nvarchar](128) NULL,
	[Action] [nvarchar](50) NULL,
	[ReceiverId] [int] NULL,
	[IsSent] [bit] NULL,
	[IsRead] [bit] NULL,
	[ReadDate] [datetime] NULL,
	[CreatedDate] [datetime] NULL,
 CONSTRAINT [PK_tbl_system_emails] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_system_emails_history]    Script Date: 11/22/2019 10:12:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_system_emails_history](
	[Id] [int] NOT NULL,
	[Subject] [nvarchar](500) NULL,
	[Body] [ntext] NULL,
	[Sender] [nvarchar](128) NULL,
	[Receiver] [nvarchar](128) NULL,
	[Action] [nvarchar](50) NULL,
	[ReceiverId] [int] NULL,
	[IsSent] [bit] NULL,
	[IsRead] [bit] NULL,
	[ReadDate] [datetime] NULL,
	[CreatedDate] [datetime] NULL,
 CONSTRAINT [PK_tbl_system_emails_history] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_traces]    Script Date: 11/22/2019 10:12:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_traces](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ActionType] [nvarchar](50) NULL,
	[UserIp] [nvarchar](20) NULL,
	[UserAgent] [nvarchar](2000) NULL,
	[Method] [nvarchar](50) NULL,
	[Domain] [nvarchar](128) NULL,
	[CreatedDate] [datetime] NULL,
	[ActionDesc] [nvarchar](max) NULL,
	[RawData] [nvarchar](max) NULL,
 CONSTRAINT [PK_aspnettrace] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_user_actions]    Script Date: 11/22/2019 10:12:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_user_actions](
	[UserId] [int] NULL,
	[UserActionId] [int] NULL,
	[ActionType] [nvarchar](50) NULL,
	[Status] [int] NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedDate] [datetime] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_user_claims]    Script Date: 11/22/2019 10:12:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_user_claims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_aspnetuserclaims_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_user_codes]    Script Date: 11/22/2019 10:12:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_user_codes](
	[Id] [nvarchar](128) NOT NULL,
	[UserId] [int] NOT NULL,
	[Code] [nvarchar](50) NULL,
	[CodeType] [nvarchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[ExpiredDate] [datetime] NULL,
	[IsUsed] [int] NULL,
	[UsedDate] [datetime] NULL,
	[Action] [nvarchar](50) NULL,
 CONSTRAINT [PK_aspnetusercodes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_user_codes_history]    Script Date: 11/22/2019 10:12:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_user_codes_history](
	[Id] [nvarchar](128) NOT NULL,
	[UserId] [int] NOT NULL,
	[Code] [nvarchar](50) NULL,
	[CodeType] [nvarchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[ExpiredDate] [datetime] NULL,
	[IsUsed] [int] NULL,
	[UsedDate] [datetime] NULL,
	[Action] [nvarchar](50) NULL,
 CONSTRAINT [PK_aspnetusercodes_history] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_user_data]    Script Date: 11/22/2019 10:12:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_user_data](
	[UserId] [int] NOT NULL,
	[FollowingCount] [int] NULL,
	[MessageCount] [int] NULL,
	[LikePostCount] [int] NULL,
	[PhotoCount] [int] NULL,
	[FollowerCount] [int] NULL,
	[PostCount] [int] NULL,
 CONSTRAINT [PK_tbl_user_data] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_user_devices]    Script Date: 11/22/2019 10:12:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_user_devices](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NULL,
	[DeviceName] [nvarchar](256) NULL,
	[DeviceID] [nvarchar](128) NULL,
	[RegistrationID] [nvarchar](500) NULL,
	[iosDevice] [bit] NULL,
	[CreatedDate] [datetime] NULL,
	[Status] [smallint] NULL,
	[LastConnected] [datetime] NOT NULL,
	[LangCode] [nvarchar](10) NULL,
 CONSTRAINT [PK_tbl_user_devices] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_user_otpactions]    Script Date: 11/22/2019 10:12:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_user_otpactions](
	[UserId] [int] NOT NULL,
	[CodeId] [nvarchar](128) NOT NULL,
	[TargetData] [nvarchar](500) NULL,
	[IsDone] [smallint] NOT NULL,
	[CreatedDate] [datetime] NULL,
	[Action] [nvarchar](50) NULL,
	[ImplementTime] [datetime] NULL,
 CONSTRAINT [PK_aspnetotpactions] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[CodeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_user_otpactions_history]    Script Date: 11/22/2019 10:12:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_user_otpactions_history](
	[UserId] [int] NOT NULL,
	[CodeId] [nvarchar](128) NOT NULL,
	[TargetData] [nvarchar](256) NULL,
	[IsDone] [smallint] NOT NULL,
	[CreatedDate] [datetime] NULL,
	[Action] [nvarchar](50) NULL,
	[ImplementTime] [datetime] NULL,
 CONSTRAINT [PK_aspnetotpactions_history] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[CodeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_user_roles]    Script Date: 11/22/2019 10:12:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_user_roles](
	[UserId] [int] NOT NULL,
	[RoleId] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_aspnetuserroles_UserId] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_user_tokenkeys]    Script Date: 11/22/2019 10:12:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_user_tokenkeys](
	[UserId] [int] NOT NULL,
	[TokenKey] [nvarchar](128) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ExpiredDate] [datetime] NOT NULL,
	[Method] [nvarchar](50) NULL,
	[Domain] [nvarchar](128) NULL,
 CONSTRAINT [PK_aspnetusertokenkeys] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[TokenKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_user_tokenkeys_history]    Script Date: 11/22/2019 10:12:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_user_tokenkeys_history](
	[UserId] [int] NOT NULL,
	[TokenKey] [nvarchar](128) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ExpiredDate] [datetime] NOT NULL,
	[Method] [nvarchar](50) NULL,
	[Domain] [nvarchar](128) NULL,
 CONSTRAINT [PK_aspnetusertokenkeys_history] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[TokenKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_users]    Script Date: 11/22/2019 10:12:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_users](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Email] [nvarchar](256) NULL,
	[EmailConfirmed] [tinyint] NOT NULL,
	[PasswordHash] [nvarchar](128) NULL,
	[SecurityStamp] [nvarchar](max) NULL,
	[PhoneNumber] [nvarchar](50) NULL,
	[PhoneNumberConfirmed] [tinyint] NOT NULL,
	[TwoFactorEnabled] [tinyint] NOT NULL,
	[LockoutEndDateUtc] [datetime] NULL,
	[LockoutEnabled] [tinyint] NOT NULL,
	[AccessFailedCount] [int] NOT NULL,
	[UserName] [nvarchar](128) NOT NULL,
	[CreatedDateUtc] [datetime] NULL,
	[PasswordHash2] [nvarchar](128) NULL,
	[FullName] [nvarchar](128) NULL,
	[DisplayName] [nvarchar](128) NULL,
	[Avatar] [nvarchar](1000) NULL,
	[OTPType] [nvarchar](20) NULL,
	[Birthday] [datetime] NULL,
	[Sex] [tinyint] NULL,
	[Address] [nvarchar](256) NULL,
	[IDCard] [nvarchar](50) NULL,
	[Note] [nvarchar](1000) NULL,
	[SocialProviderId] [int] NULL,
	[LastOnline] [datetime] NULL,
 CONSTRAINT [PK_aspnetusers_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
INSERT [dbo].[tbl_access_roles] ([RoleId], [OperationId]) VALUES (N'bdca3c47-2ea8-42dd-a3e4-18f3efa61388', N'd29d460c-ec18-11e5-ac44-5065f31c5bc6')
INSERT [dbo].[tbl_access_roles] ([RoleId], [OperationId]) VALUES (N'bdca3c47-2ea8-42dd-a3e4-18f3efa61388', N'6ede8286-f00e-11e5-bec1-5065f31c5bc6')
INSERT [dbo].[tbl_access_roles] ([RoleId], [OperationId]) VALUES (N'bdca3c47-2ea8-42dd-a3e4-18f3efa61388', N'2cb82b85-f00e-11e5-bec1-5065f31c5bc6')
INSERT [dbo].[tbl_access_roles] ([RoleId], [OperationId]) VALUES (N'07b53379-85a4-424f-9912-131ac4e7f702', N'562aeb72-f01e-11e5-bec1-5065f31c5bc6')
INSERT [dbo].[tbl_access_roles] ([RoleId], [OperationId]) VALUES (N'07b53379-85a4-424f-9912-131ac4e7f702', N'ec8f4e3d-f018-11e5-bec1-5065f31c5bc6')
INSERT [dbo].[tbl_access_roles] ([RoleId], [OperationId]) VALUES (N'07b53379-85a4-424f-9912-131ac4e7f702', N'd74c4244-efeb-11e5-bec1-5065f31c5bc6')
INSERT [dbo].[tbl_access_roles] ([RoleId], [OperationId]) VALUES (N'bdca3c47-2ea8-42dd-a3e4-18f3efa61388', N'ae067e13-f00a-11e5-bec1-5065f31c5bc6')
INSERT [dbo].[tbl_access_roles] ([RoleId], [OperationId]) VALUES (N'0cc6c588-c011-41fb-b2d1-4aeacced8701', N'71961266-f011-11e5-bec1-5065f31c5bc6')
INSERT [dbo].[tbl_access_roles] ([RoleId], [OperationId]) VALUES (N'0cc6c588-c011-41fb-b2d1-4aeacced8701', N'ed60c95b-f011-11e5-bec1-5065f31c5bc6')
INSERT [dbo].[tbl_access_roles] ([RoleId], [OperationId]) VALUES (N'0cc6c588-c011-41fb-b2d1-4aeacced8701', N'e9a2e8a2-215c-11e6-80db-5065f31c5bc6')
INSERT [dbo].[tbl_access_roles] ([RoleId], [OperationId]) VALUES (N'e6054dc6-c0d2-4fff-9dd4-cd95b6cabedf', N'575f6f05-3756-11e6-9aba-00505691ea97')
INSERT [dbo].[tbl_access_roles] ([RoleId], [OperationId]) VALUES (N'e6054dc6-c0d2-4fff-9dd4-cd95b6cabedf', N'56811434-44be-11e6-be49-5065f31c5bc6')
INSERT [dbo].[tbl_access_roles] ([RoleId], [OperationId]) VALUES (N'e6054dc6-c0d2-4fff-9dd4-cd95b6cabedf', N'ab5a9469-549f-11e6-b527-00505691ea97')
INSERT [dbo].[tbl_access_roles] ([RoleId], [OperationId]) VALUES (N'e6054dc6-c0d2-4fff-9dd4-cd95b6cabedf', N'2ea7685f-44bf-11e6-be49-5065f31c5bc6')
INSERT [dbo].[tbl_access_roles] ([RoleId], [OperationId]) VALUES (N'e6054dc6-c0d2-4fff-9dd4-cd95b6cabedf', N'c3a08db4-44bf-11e6-be49-5065f31c5bc6')
INSERT [dbo].[tbl_access_roles] ([RoleId], [OperationId]) VALUES (N'e6054dc6-c0d2-4fff-9dd4-cd95b6cabedf', N'684d64d8-44bf-11e6-be49-5065f31c5bc6')
INSERT [dbo].[tbl_access_roles] ([RoleId], [OperationId]) VALUES (N'c1612206-51d4-4b11-883c-a6f8e5e3e8aa', N'ae067e13-f00a-11e5-bec1-5065f31c5bc6')
INSERT [dbo].[tbl_access_roles] ([RoleId], [OperationId]) VALUES (N'c1612206-51d4-4b11-883c-a6f8e5e3e8aa', N'd29d460c-ec18-11e5-ac44-5065f31c5bc6')
INSERT [dbo].[tbl_access_roles] ([RoleId], [OperationId]) VALUES (N'c1612206-51d4-4b11-883c-a6f8e5e3e8aa', N'575f6f05-3756-11e6-9aba-00505691ea97')
INSERT [dbo].[tbl_access_roles] ([RoleId], [OperationId]) VALUES (N'c1612206-51d4-4b11-883c-a6f8e5e3e8aa', N'b08b3a7f-12a8-11e6-a77b-5065f31c5bc6')
INSERT [dbo].[tbl_access_roles] ([RoleId], [OperationId]) VALUES (N'c1612206-51d4-4b11-883c-a6f8e5e3e8aa', N'bb4a1976-12a9-11e6-a77b-5065f31c5bc6')
INSERT [dbo].[tbl_access_roles] ([RoleId], [OperationId]) VALUES (N'c1612206-51d4-4b11-883c-a6f8e5e3e8aa', N'9a7bf702-f00e-11e5-bec1-5065f31c5bc6')
INSERT [dbo].[tbl_access_roles] ([RoleId], [OperationId]) VALUES (N'c1612206-51d4-4b11-883c-a6f8e5e3e8aa', N'6ede8286-f00e-11e5-bec1-5065f31c5bc6')
INSERT [dbo].[tbl_access_roles] ([RoleId], [OperationId]) VALUES (N'c1612206-51d4-4b11-883c-a6f8e5e3e8aa', N'2cb82b85-f00e-11e5-bec1-5065f31c5bc6')
INSERT [dbo].[tbl_access_roles] ([RoleId], [OperationId]) VALUES (N'c1612206-51d4-4b11-883c-a6f8e5e3e8aa', N'63af1645-0123-11e6-9aba-00505691ea97')
INSERT [dbo].[tbl_access_roles] ([RoleId], [OperationId]) VALUES (N'a11d62f9-5fe1-40e6-8a2e-6a9c191ad9f5', N'575f6f05-3756-11e6-9aba-00505691ea97')
INSERT [dbo].[tbl_access_roles] ([RoleId], [OperationId]) VALUES (N'aa8504d4-f047-4478-8a5f-bff418404ce0', N'2d3004ff-b86e-11e6-9b3e-005056916e71')
INSERT [dbo].[tbl_access_roles] ([RoleId], [OperationId]) VALUES (N'aa8504d4-f047-4478-8a5f-bff418404ce0', N'65d286fc-b86f-11e6-9b3e-005056916e71')
INSERT [dbo].[tbl_access_roles] ([RoleId], [OperationId]) VALUES (N'aa8504d4-f047-4478-8a5f-bff418404ce0', N'548fb1cc-b86f-11e6-9b3e-005056916e71')
INSERT [dbo].[tbl_access_roles] ([RoleId], [OperationId]) VALUES (N'3ccce91d-eeb6-4246-b006-18a8beb0bde7', N'hh9d460c-ec18-11e5-dfvc-5065f3dsf123')
INSERT [dbo].[tbl_access_roles] ([RoleId], [OperationId]) VALUES (N'3ccce91d-eeb6-4246-b006-18a8beb0bde7', N'gg9d460c-ec18-11e5-dfvc-5065f3dsf123')
INSERT [dbo].[tbl_access_roles] ([RoleId], [OperationId]) VALUES (N'3ccce91d-eeb6-4246-b006-18a8beb0bde7', N'562aeb72-f01e-11e5-bec1-5065f31c5bc6')
INSERT [dbo].[tbl_access_roles] ([RoleId], [OperationId]) VALUES (N'3ccce91d-eeb6-4246-b006-18a8beb0bde7', N'ec8f4e3d-f018-11e5-bec1-5065f31c5bc6')
INSERT [dbo].[tbl_access_roles] ([RoleId], [OperationId]) VALUES (N'3ccce91d-eeb6-4246-b006-18a8beb0bde7', N'd74c4244-efeb-11e5-bec1-5065f31c5bc6')
INSERT [dbo].[tbl_access_roles] ([RoleId], [OperationId]) VALUES (N'3ccce91d-eeb6-4246-b006-18a8beb0bde7', N'9b032d92-f00b-11e5-bec1-5065f31c5bc6')
INSERT [dbo].[tbl_access_roles] ([RoleId], [OperationId]) VALUES (N'3ccce91d-eeb6-4246-b006-18a8beb0bde7', N'd4b27a34-f00b-11e5-bec1-5065f31c5bc6')
INSERT [dbo].[tbl_access_roles] ([RoleId], [OperationId]) VALUES (N'3ccce91d-eeb6-4246-b006-18a8beb0bde7', N'f39b53ce-effb-11e5-bec1-5065f31c5bc6')
INSERT [dbo].[tbl_access_roles] ([RoleId], [OperationId]) VALUES (N'3ccce91d-eeb6-4246-b006-18a8beb0bde7', N'f6004e29-1593-11e6-a03e-5065f31c5bc6')
INSERT [dbo].[tbl_access_roles] ([RoleId], [OperationId]) VALUES (N'3ccce91d-eeb6-4246-b006-18a8beb0bde7', N'a787233a-ec18-11e5-ac44-5065f31c5bc6')
INSERT [dbo].[tbl_access_roles] ([RoleId], [OperationId]) VALUES (N'3ccce91d-eeb6-4246-b006-18a8beb0bde7', N'ad021ff5-ec38-11e5-ac44-5065f31c5bc6')
INSERT [dbo].[tbl_access_roles] ([RoleId], [OperationId]) VALUES (N'3ccce91d-eeb6-4246-b006-18a8beb0bde7', N'a867233a-ec18-11e5-ac44-5065f31c5bc6')
INSERT [dbo].[tbl_access_roles] ([RoleId], [OperationId]) VALUES (N'3ccce91d-eeb6-4246-b006-18a8beb0bde7', N'b014f75e-f55f-11e5-98f1-5065f31c5bc6')
INSERT [dbo].[tbl_access_roles] ([RoleId], [OperationId]) VALUES (N'3ccce91d-eeb6-4246-b006-18a8beb0bde7', N'3c4b25ff-ef50-11e5-97da-5065f31c5bc6')
INSERT [dbo].[tbl_access_roles] ([RoleId], [OperationId]) VALUES (N'3ccce91d-eeb6-4246-b006-18a8beb0bde7', N'ad021ff0-ec18-11e5-ac44-5065f31c5bc6')
INSERT [dbo].[tbl_access_roles] ([RoleId], [OperationId]) VALUES (N'b9d17b10-adb6-4069-b10b-2007ecc49f1e', N'8f72a14b-c2b5-11e6-9b3e-005056916e71')
INSERT [dbo].[tbl_access_roles] ([RoleId], [OperationId]) VALUES (N'b9d17b10-adb6-4069-b10b-2007ecc49f1e', N'8f7d712f-c2b5-11e6-9b3e-005056916e71')
INSERT [dbo].[tbl_access_roles] ([RoleId], [OperationId]) VALUES (N'b9d17b10-adb6-4069-b10b-2007ecc49f1e', N'8f6ce48c-c2b5-11e6-9b3e-005056916e71')
INSERT [dbo].[tbl_access_roles] ([RoleId], [OperationId]) VALUES (N'b9d17b10-adb6-4069-b10b-2007ecc49f1e', N'8f77fdf0-c2b5-11e6-9b3e-005056916e71')
INSERT [dbo].[tbl_access_roles] ([RoleId], [OperationId]) VALUES (N'b9d17b10-adb6-4069-b10b-2007ecc49f1e', N'575f6f05-3756-11e6-9aba-00505691ea97')
INSERT [dbo].[tbl_access_roles] ([RoleId], [OperationId]) VALUES (N'b9d17b10-adb6-4069-b10b-2007ecc49f1e', N'56811434-44be-11e6-be49-5065f31c5bc6')
INSERT [dbo].[tbl_access_roles] ([RoleId], [OperationId]) VALUES (N'b9d17b10-adb6-4069-b10b-2007ecc49f1e', N'ab5a9469-549f-11e6-b527-00505691ea97')
INSERT [dbo].[tbl_access_roles] ([RoleId], [OperationId]) VALUES (N'b9d17b10-adb6-4069-b10b-2007ecc49f1e', N'2ea7685f-44bf-11e6-be49-5065f31c5bc6')
INSERT [dbo].[tbl_access_roles] ([RoleId], [OperationId]) VALUES (N'b9d17b10-adb6-4069-b10b-2007ecc49f1e', N'd60dbec0-44bf-11e6-be49-5065f31c5bc6')
INSERT [dbo].[tbl_access_roles] ([RoleId], [OperationId]) VALUES (N'b9d17b10-adb6-4069-b10b-2007ecc49f1e', N'c3a08db4-44bf-11e6-be49-5065f31c5bc6')
INSERT [dbo].[tbl_access_roles] ([RoleId], [OperationId]) VALUES (N'b9d17b10-adb6-4069-b10b-2007ecc49f1e', N'684d64d8-44bf-11e6-be49-5065f31c5bc6')
INSERT [dbo].[tbl_access_roles] ([RoleId], [OperationId]) VALUES (N'b9d17b10-adb6-4069-b10b-2007ecc49f1e', N'63af1645-0123-11e6-9aba-00505691ea97')
INSERT [dbo].[tbl_access_roles] ([RoleId], [OperationId]) VALUES (N'b9d17b10-adb6-4069-b10b-2007ecc49f1e', N'b38843b7-57eb-11e7-ab63-005056916e71')
INSERT [dbo].[tbl_access_roles] ([RoleId], [OperationId]) VALUES (N'b9d17b10-adb6-4069-b10b-2007ecc49f1e', N'b3c3ef7f-57eb-11e7-ab63-005056916e71')
INSERT [dbo].[tbl_accesses] ([Id], [AccessName], [Active], [Description]) VALUES (N'0116e9a3-1594-11e6-a03e-5065f31c5bc6', N'System', 1, N'System ')
INSERT [dbo].[tbl_accesses] ([Id], [AccessName], [Active], [Description]) VALUES (N'12e851ef-ec12-11e5-ac44-5065f31c5bc6', N'UsersAdmin', 1, N'Using to manage users')
INSERT [dbo].[tbl_accesses] ([Id], [AccessName], [Active], [Description]) VALUES (N'22a50c6b-ec12-11e5-ac44-5065f31c5bc6', N'AccessRoles', 1, N'Using to manage access roles')
INSERT [dbo].[tbl_accesses] ([Id], [AccessName], [Active], [Description]) VALUES (N'4c550c4f-0123-11e6-9aba-00505691ea97', N'Tools', 0, N'OTT Tools')
INSERT [dbo].[tbl_accesses] ([Id], [AccessName], [Active], [Description]) VALUES (N'77120BF4-C7D1-41C5-B871-567E37CD16FE', N'Demo', 1, N'fddddd')
INSERT [dbo].[tbl_accesses] ([Id], [AccessName], [Active], [Description]) VALUES (N'a3e2326f-f018-11e5-bec1-5065f31c5bc6', N'Account', 1, N'User can manage their account')
INSERT [dbo].[tbl_accesses] ([Id], [AccessName], [Active], [Description]) VALUES (N'ad48480d-9f20-11e6-9b3e-005056916e71', N'Monitoring', 0, N'Monitoring systems')
INSERT [dbo].[tbl_accesses] ([Id], [AccessName], [Active], [Description]) VALUES (N'bba66db3-effb-11e5-bec1-5065f31c5bc6', N'RolesAdmin', 1, N'Using to manage roles')
INSERT [dbo].[tbl_accesses] ([Id], [AccessName], [Active], [Description]) VALUES (N'c0af2286-efeb-11e5-bec1-5065f31c5bc6', N'Home', 1, N'User can view the backend dashboard')
INSERT [dbo].[tbl_accesses] ([Id], [AccessName], [Active], [Description]) VALUES (N'ea8d54c0-86e4-11e6-9d6d-00505691ea97', N'Statistics', 0, N'Kplus Statistics')
SET IDENTITY_INSERT [dbo].[tbl_categories] ON 

INSERT [dbo].[tbl_categories] ([Id], [Name], [Code], [Icon], [CreatedBy], [CreatedDate], [LastUpdated], [LastUpdatedBy], [Status]) VALUES (1, N'Vùng cao', NULL, NULL, NULL, NULL, NULL, NULL, 1)
INSERT [dbo].[tbl_categories] ([Id], [Name], [Code], [Icon], [CreatedBy], [CreatedDate], [LastUpdated], [LastUpdatedBy], [Status]) VALUES (2, N'Du lịch biển', NULL, NULL, NULL, NULL, NULL, NULL, 1)
SET IDENTITY_INSERT [dbo].[tbl_categories] OFF
INSERT [dbo].[tbl_cmn_settings] ([SettingName], [SettingType], [SettingValue]) VALUES (N'AdminEmail', N'GeneralSettings', N'bangkhmt3@gmail.com')
INSERT [dbo].[tbl_cmn_settings] ([SettingName], [SettingType], [SettingValue]) VALUES (N'CacheProvider', N'CacheSettings', N'WebCacheProvider')
INSERT [dbo].[tbl_cmn_settings] ([SettingName], [SettingType], [SettingValue]) VALUES (N'FtpPassword', N'FtpSettings', N'abcd1234')
INSERT [dbo].[tbl_cmn_settings] ([SettingName], [SettingType], [SettingValue]) VALUES (N'FtpPort', N'FtpSettings', N'21')
INSERT [dbo].[tbl_cmn_settings] ([SettingName], [SettingType], [SettingValue]) VALUES (N'FtpServer', N'FtpSettings', N'123.30.145.246')
INSERT [dbo].[tbl_cmn_settings] ([SettingName], [SettingType], [SettingValue]) VALUES (N'FtpUsername', N'FtpSettings', N'HDVN')
INSERT [dbo].[tbl_cmn_settings] ([SettingName], [SettingType], [SettingValue]) VALUES (N'HaNoiTimeZone', N'TimeZone', N'+07:00')
INSERT [dbo].[tbl_cmn_settings] ([SettingName], [SettingType], [SettingValue]) VALUES (N'LonDonTimeZone', N'TimeZone', N'+00:00')
INSERT [dbo].[tbl_cmn_settings] ([SettingName], [SettingType], [SettingValue]) VALUES (N'SiteName', N'GeneralSettings', N'Manager portal')
INSERT [dbo].[tbl_cmn_settings] ([SettingName], [SettingType], [SettingValue]) VALUES (N'SmtpPassword', N'MailSettings', N'Abcd1234')
INSERT [dbo].[tbl_cmn_settings] ([SettingName], [SettingType], [SettingValue]) VALUES (N'SmtpPort', N'MailSettings', N'25')
INSERT [dbo].[tbl_cmn_settings] ([SettingName], [SettingType], [SettingValue]) VALUES (N'SmtpServer', N'MailSettings', N'mail.smtp.vn')
INSERT [dbo].[tbl_cmn_settings] ([SettingName], [SettingType], [SettingValue]) VALUES (N'SmtpTimeout', N'MailSettings', N'30')
INSERT [dbo].[tbl_cmn_settings] ([SettingName], [SettingType], [SettingValue]) VALUES (N'SmtpUsername', N'MailSettings', N'user')
INSERT [dbo].[tbl_cmn_settings] ([SettingName], [SettingType], [SettingValue]) VALUES (N'SmtpUseSsl', N'MailSettings', N'False')
INSERT [dbo].[tbl_cmn_settings] ([SettingName], [SettingType], [SettingValue]) VALUES (N'SystemDefaultCacheDuration', N'CacheSettings', N'31')
INSERT [dbo].[tbl_cmn_settings] ([SettingName], [SettingType], [SettingValue]) VALUES (N'TimeZoneId', N'GeneralSettings', N'SE Asia Standard Time')
SET IDENTITY_INSERT [dbo].[tbl_cmn_sql_errors] ON 

INSERT [dbo].[tbl_cmn_sql_errors] ([Id], [ErrorMessage], [ErrorServerity], [ErrorState], [ErrorLine], [Actor], [DateOfIssue]) VALUES (49, N'Cannot insert the value NULL into column ''PostId'', table ''halo_social.dbo.tbl_post_comments''; column does not allow nulls. INSERT fails.', N'16', 2, 20, N'Post_AddComment', CAST(N'2018-05-02T11:10:11.540' AS DateTime))
INSERT [dbo].[tbl_cmn_sql_errors] ([Id], [ErrorMessage], [ErrorServerity], [ErrorState], [ErrorLine], [Actor], [DateOfIssue]) VALUES (50, N'Cannot insert the value NULL into column ''PostId'', table ''halo_social.dbo.tbl_post_comments''; column does not allow nulls. INSERT fails.', N'16', 2, 20, N'Post_AddComment', CAST(N'2018-05-02T11:10:17.057' AS DateTime))
INSERT [dbo].[tbl_cmn_sql_errors] ([Id], [ErrorMessage], [ErrorServerity], [ErrorState], [ErrorLine], [Actor], [DateOfIssue]) VALUES (51, N'Cannot insert the value NULL into column ''PostId'', table ''halo_social.dbo.tbl_post_comments''; column does not allow nulls. INSERT fails.', N'16', 2, 20, N'Post_AddComment', CAST(N'2018-05-02T11:10:28.517' AS DateTime))
INSERT [dbo].[tbl_cmn_sql_errors] ([Id], [ErrorMessage], [ErrorServerity], [ErrorState], [ErrorLine], [Actor], [DateOfIssue]) VALUES (52, N'Cannot insert the value NULL into column ''PostId'', table ''halo_social.dbo.tbl_post_comments''; column does not allow nulls. INSERT fails.', N'16', 2, 20, N'Post_AddComment', CAST(N'2018-05-02T11:11:26.923' AS DateTime))
INSERT [dbo].[tbl_cmn_sql_errors] ([Id], [ErrorMessage], [ErrorServerity], [ErrorState], [ErrorLine], [Actor], [DateOfIssue]) VALUES (53, N'Cannot insert the value NULL into column ''Title'', table ''halo_social.dbo.tbl_posts''; column does not allow nulls. INSERT fails.', N'16', 2, 31, N'Post_Insert', CAST(N'2018-05-02T17:51:07.847' AS DateTime))
INSERT [dbo].[tbl_cmn_sql_errors] ([Id], [ErrorMessage], [ErrorServerity], [ErrorState], [ErrorLine], [Actor], [DateOfIssue]) VALUES (54, N'Cannot insert the value NULL into column ''Title'', table ''halo_social.dbo.tbl_posts''; column does not allow nulls. INSERT fails.', N'16', 2, 31, N'Post_Insert', CAST(N'2018-05-02T17:51:52.420' AS DateTime))
INSERT [dbo].[tbl_cmn_sql_errors] ([Id], [ErrorMessage], [ErrorServerity], [ErrorState], [ErrorLine], [Actor], [DateOfIssue]) VALUES (55, N'Cannot insert the value NULL into column ''Title'', table ''halo_social.dbo.tbl_posts''; column does not allow nulls. INSERT fails.', N'16', 2, 31, N'Post_Insert', CAST(N'2018-05-02T17:52:11.390' AS DateTime))
INSERT [dbo].[tbl_cmn_sql_errors] ([Id], [ErrorMessage], [ErrorServerity], [ErrorState], [ErrorLine], [Actor], [DateOfIssue]) VALUES (56, N'Cannot insert the value NULL into column ''Title'', table ''halo_social.dbo.tbl_posts''; column does not allow nulls. INSERT fails.', N'16', 2, 31, N'Post_Insert', CAST(N'2018-05-02T17:53:30.223' AS DateTime))
INSERT [dbo].[tbl_cmn_sql_errors] ([Id], [ErrorMessage], [ErrorServerity], [ErrorState], [ErrorLine], [Actor], [DateOfIssue]) VALUES (57, N'Cannot insert the value NULL into column ''Title'', table ''halo_social.dbo.tbl_posts''; column does not allow nulls. INSERT fails.', N'16', 2, 31, N'Post_Insert', CAST(N'2018-05-02T17:53:50.107' AS DateTime))
INSERT [dbo].[tbl_cmn_sql_errors] ([Id], [ErrorMessage], [ErrorServerity], [ErrorState], [ErrorLine], [Actor], [DateOfIssue]) VALUES (58, N'Cannot insert the value NULL into column ''Title'', table ''halo_social.dbo.tbl_posts''; column does not allow nulls. INSERT fails.', N'16', 2, 31, N'Post_Insert', CAST(N'2018-05-02T17:54:36.167' AS DateTime))
INSERT [dbo].[tbl_cmn_sql_errors] ([Id], [ErrorMessage], [ErrorServerity], [ErrorState], [ErrorLine], [Actor], [DateOfIssue]) VALUES (59, N'Cannot insert the value NULL into column ''Description'', table ''halo.dbo.tbl_posts''; column does not allow nulls. INSERT fails.', N'16', 2, 28, N'Post_Insert', CAST(N'2018-05-10T09:53:59.590' AS DateTime))
INSERT [dbo].[tbl_cmn_sql_errors] ([Id], [ErrorMessage], [ErrorServerity], [ErrorState], [ErrorLine], [Actor], [DateOfIssue]) VALUES (60, N'Cannot insert the value NULL into column ''Description'', table ''halo.dbo.tbl_posts''; column does not allow nulls. INSERT fails.', N'16', 2, 28, N'Post_Insert', CAST(N'2018-05-10T09:54:00.293' AS DateTime))
INSERT [dbo].[tbl_cmn_sql_errors] ([Id], [ErrorMessage], [ErrorServerity], [ErrorState], [ErrorLine], [Actor], [DateOfIssue]) VALUES (61, N'Cannot insert the value NULL into column ''Description'', table ''halo.dbo.tbl_posts''; column does not allow nulls. INSERT fails.', N'16', 2, 28, N'Post_Insert', CAST(N'2018-05-10T09:54:00.793' AS DateTime))
INSERT [dbo].[tbl_cmn_sql_errors] ([Id], [ErrorMessage], [ErrorServerity], [ErrorState], [ErrorLine], [Actor], [DateOfIssue]) VALUES (62, N'Cannot insert the value NULL into column ''Description'', table ''halo.dbo.tbl_posts''; column does not allow nulls. INSERT fails.', N'16', 2, 28, N'Post_Insert', CAST(N'2018-05-10T09:54:01.070' AS DateTime))
INSERT [dbo].[tbl_cmn_sql_errors] ([Id], [ErrorMessage], [ErrorServerity], [ErrorState], [ErrorLine], [Actor], [DateOfIssue]) VALUES (63, N'Cannot insert the value NULL into column ''Description'', table ''halo.dbo.tbl_posts''; column does not allow nulls. INSERT fails.', N'16', 2, 28, N'Post_Insert', CAST(N'2018-05-10T09:55:20.070' AS DateTime))
INSERT [dbo].[tbl_cmn_sql_errors] ([Id], [ErrorMessage], [ErrorServerity], [ErrorState], [ErrorLine], [Actor], [DateOfIssue]) VALUES (64, N'Cannot insert the value NULL into column ''Description'', table ''halo.dbo.tbl_posts''; column does not allow nulls. INSERT fails.', N'16', 2, 28, N'Post_Insert', CAST(N'2018-05-10T09:55:32.190' AS DateTime))
INSERT [dbo].[tbl_cmn_sql_errors] ([Id], [ErrorMessage], [ErrorServerity], [ErrorState], [ErrorLine], [Actor], [DateOfIssue]) VALUES (65, N'Cannot insert the value NULL into column ''Description'', table ''halo.dbo.tbl_posts''; column does not allow nulls. INSERT fails.', N'16', 2, 28, N'Post_Insert', CAST(N'2018-05-10T09:55:33.163' AS DateTime))
INSERT [dbo].[tbl_cmn_sql_errors] ([Id], [ErrorMessage], [ErrorServerity], [ErrorState], [ErrorLine], [Actor], [DateOfIssue]) VALUES (66, N'Cannot insert the value NULL into column ''Description'', table ''halo.dbo.tbl_posts''; column does not allow nulls. INSERT fails.', N'16', 2, 28, N'Post_Insert', CAST(N'2018-05-10T09:56:44.990' AS DateTime))
INSERT [dbo].[tbl_cmn_sql_errors] ([Id], [ErrorMessage], [ErrorServerity], [ErrorState], [ErrorLine], [Actor], [DateOfIssue]) VALUES (67, N'Cannot insert the value NULL into column ''Description'', table ''halo.dbo.tbl_posts''; column does not allow nulls. INSERT fails.', N'16', 2, 28, N'Post_Insert', CAST(N'2018-05-10T09:56:46.337' AS DateTime))
INSERT [dbo].[tbl_cmn_sql_errors] ([Id], [ErrorMessage], [ErrorServerity], [ErrorState], [ErrorLine], [Actor], [DateOfIssue]) VALUES (68, N'Cannot insert the value NULL into column ''Description'', table ''halo.dbo.tbl_posts''; column does not allow nulls. INSERT fails.', N'16', 2, 28, N'Post_Insert', CAST(N'2018-05-10T09:56:46.767' AS DateTime))
INSERT [dbo].[tbl_cmn_sql_errors] ([Id], [ErrorMessage], [ErrorServerity], [ErrorState], [ErrorLine], [Actor], [DateOfIssue]) VALUES (69, N'Cannot insert the value NULL into column ''Description'', table ''halo.dbo.tbl_posts''; column does not allow nulls. INSERT fails.', N'16', 2, 28, N'Post_Insert', CAST(N'2018-05-10T09:56:47.043' AS DateTime))
INSERT [dbo].[tbl_cmn_sql_errors] ([Id], [ErrorMessage], [ErrorServerity], [ErrorState], [ErrorLine], [Actor], [DateOfIssue]) VALUES (70, N'Cannot insert the value NULL into column ''Description'', table ''halo.dbo.tbl_posts''; column does not allow nulls. INSERT fails.', N'16', 2, 28, N'Post_Insert', CAST(N'2018-05-10T09:56:47.217' AS DateTime))
INSERT [dbo].[tbl_cmn_sql_errors] ([Id], [ErrorMessage], [ErrorServerity], [ErrorState], [ErrorLine], [Actor], [DateOfIssue]) VALUES (1059, N'Cannot insert the value NULL into column ''Description'', table ''halo.dbo.tbl_posts''; column does not allow nulls. INSERT fails.', N'16', 2, 28, N'Post_Insert', CAST(N'2018-05-10T09:59:46.940' AS DateTime))
INSERT [dbo].[tbl_cmn_sql_errors] ([Id], [ErrorMessage], [ErrorServerity], [ErrorState], [ErrorLine], [Actor], [DateOfIssue]) VALUES (1060, N'Cannot insert the value NULL into column ''Description'', table ''halo.dbo.tbl_posts''; column does not allow nulls. INSERT fails.', N'16', 2, 28, N'Post_Insert', CAST(N'2018-05-10T09:59:48.180' AS DateTime))
INSERT [dbo].[tbl_cmn_sql_errors] ([Id], [ErrorMessage], [ErrorServerity], [ErrorState], [ErrorLine], [Actor], [DateOfIssue]) VALUES (1061, N'Cannot insert the value NULL into column ''Description'', table ''halo.dbo.tbl_posts''; column does not allow nulls. INSERT fails.', N'16', 2, 28, N'Post_Insert', CAST(N'2018-05-10T09:59:48.393' AS DateTime))
INSERT [dbo].[tbl_cmn_sql_errors] ([Id], [ErrorMessage], [ErrorServerity], [ErrorState], [ErrorLine], [Actor], [DateOfIssue]) VALUES (1062, N'Cannot insert the value NULL into column ''Description'', table ''halo.dbo.tbl_posts''; column does not allow nulls. INSERT fails.', N'16', 2, 28, N'Post_Insert', CAST(N'2018-05-10T10:20:53.060' AS DateTime))
INSERT [dbo].[tbl_cmn_sql_errors] ([Id], [ErrorMessage], [ErrorServerity], [ErrorState], [ErrorLine], [Actor], [DateOfIssue]) VALUES (1063, N'Cannot insert the value NULL into column ''Description'', table ''halo.dbo.tbl_posts''; column does not allow nulls. INSERT fails.', N'16', 2, 28, N'Post_Insert', CAST(N'2018-05-10T10:21:00.360' AS DateTime))
INSERT [dbo].[tbl_cmn_sql_errors] ([Id], [ErrorMessage], [ErrorServerity], [ErrorState], [ErrorLine], [Actor], [DateOfIssue]) VALUES (1064, N'Cannot insert the value NULL into column ''Description'', table ''halo.dbo.tbl_posts''; column does not allow nulls. INSERT fails.', N'16', 2, 28, N'Post_Insert', CAST(N'2018-05-10T10:21:02.313' AS DateTime))
INSERT [dbo].[tbl_cmn_sql_errors] ([Id], [ErrorMessage], [ErrorServerity], [ErrorState], [ErrorLine], [Actor], [DateOfIssue]) VALUES (1065, N'Cannot insert the value NULL into column ''UserId'', table ''halo.dbo.tbl_user_data''; column does not allow nulls. INSERT fails.', N'16', 2, 59, N'dbo.User_UpdateCounter', CAST(N'2018-05-11T10:05:51.693' AS DateTime))
INSERT [dbo].[tbl_cmn_sql_errors] ([Id], [ErrorMessage], [ErrorServerity], [ErrorState], [ErrorLine], [Actor], [DateOfIssue]) VALUES (1066, N'Transaction count after EXECUTE indicates a mismatching number of BEGIN and COMMIT statements. Previous count = 1, current count = 0.', N'16', 2, 0, N'dbo.User_UpdateCounter', CAST(N'2018-05-11T10:05:51.697' AS DateTime))
SET IDENTITY_INSERT [dbo].[tbl_cmn_sql_errors] OFF
SET IDENTITY_INSERT [dbo].[tbl_document_api] ON 

INSERT [dbo].[tbl_document_api] ([Id], [LinkUrl], [Data]) VALUES (1, N'/api/user/getlistprofile', N'{"ListUserId":[1]}')
INSERT [dbo].[tbl_document_api] ([Id], [LinkUrl], [Data]) VALUES (2, N'/api/user/login', N'{"UserName":"bangkhmt3@gmail.com","Password":"e10adc3949ba59abbe56e057f20f883e","Time":"20190922160700","Hash":"8d3bacbff1d163a3975f81666f06f9780","SocialProvider":null}')
INSERT [dbo].[tbl_document_api] ([Id], [LinkUrl], [Data]) VALUES (3, N'/api/user/register', N'{"UserName":"minhduc1","Password":"kakahaha@1","Email":"minhittduc@gmail.com","Birthday":null,"Sex":null,"Address":null,"Full_Name":null,"Display_Name":null,"CMTND":null,"Phone":null,"Note":null,"Time":"20190824140754","IsEmail":false,"IsPhoneNumber":false}')
SET IDENTITY_INSERT [dbo].[tbl_document_api] OFF
INSERT [dbo].[tbl_domains] ([DomainKey], [DomainName], [LoginDurations], [Status], [Des]) VALUES (N'123.30.145.186', N'Test server', 30, 1, N'Server test')
INSERT [dbo].[tbl_domains] ([DomainKey], [DomainName], [LoginDurations], [Status], [Des]) VALUES (N'192.168.2.25', N'Local server 8080', 1000, 1, N'Server local')
INSERT [dbo].[tbl_domains] ([DomainKey], [DomainName], [LoginDurations], [Status], [Des]) VALUES (N'AGENCY', N'agency.com.vn', 10, 1, N'Các đại lý ')
INSERT [dbo].[tbl_domains] ([DomainKey], [DomainName], [LoginDurations], [Status], [Des]) VALUES (N'localhost', N'Local 8080', 1000, 1, N'Server local')
INSERT [dbo].[tbl_roles] ([Id], [Name]) VALUES (N'07b53379-85a4-424f-9912-131ac4e7f702', N'Users')
INSERT [dbo].[tbl_roles] ([Id], [Name]) VALUES (N'3ccce91d-eeb6-4246-b006-18a8beb0bde7', N'Admin')
INSERT [dbo].[tbl_roles] ([Id], [Name]) VALUES (N'a11d62f9-5fe1-40e6-8a2e-6a9c191ad9f5', N'Anti Piracy')
INSERT [dbo].[tbl_roles] ([Id], [Name]) VALUES (N'aa8504d4-f047-4478-8a5f-bff418404ce0', N'OTT Bypass')
INSERT [dbo].[tbl_roles] ([Id], [Name]) VALUES (N'b760e1be-ad5d-41c7-97d3-4f9632708660', N'Members')
INSERT [dbo].[tbl_roles] ([Id], [Name]) VALUES (N'b9d17b10-adb6-4069-b10b-2007ecc49f1e', N'Tester')
INSERT [dbo].[tbl_roles] ([Id], [Name]) VALUES (N'bdca3c47-2ea8-42dd-a3e4-18f3efa61388', N'CCD Agent')
INSERT [dbo].[tbl_roles] ([Id], [Name]) VALUES (N'c1612206-51d4-4b11-883c-a6f8e5e3e8aa', N'CCD Super')
INSERT [dbo].[tbl_roles] ([Id], [Name]) VALUES (N'e6054dc6-c0d2-4fff-9dd4-cd95b6cabedf', N'Irdeto Tester')
SET IDENTITY_INSERT [dbo].[tbl_social_provider] ON 

INSERT [dbo].[tbl_social_provider] ([Id], [Code], [Name], [Status], [ClientId], [ClientSecret]) VALUES (1, N'Facebook', N'Nhà cung cấp Facebook', 1, N'1283899668348874', N'ba1f74ad1e82be4a6357d99cfeb08da0')
INSERT [dbo].[tbl_social_provider] ([Id], [Code], [Name], [Status], [ClientId], [ClientSecret]) VALUES (2, N'Google', N'Nhà cung cấp Google', 1, N'720014388885-mtsl6hs38puaqushvkd1u91vjd7p4l82.apps.googleusercontent.com', N'bnU4_WPy2ppDRCZjh-zb9_am')
SET IDENTITY_INSERT [dbo].[tbl_social_provider] OFF
SET IDENTITY_INSERT [dbo].[tbl_system_emails] ON 

INSERT [dbo].[tbl_system_emails] ([Id], [Subject], [Body], [Sender], [Receiver], [Action], [ReceiverId], [IsSent], [IsRead], [ReadDate], [CreatedDate]) VALUES (6, N'Registration verify', N'<html>
<body style="color:grey; font-size:15px;">
    <font face="Helvetica, Arial, sans-serif" />

    <!--<div style="position:absolute; height:100px; width:600px; background-color:#0d1d36; padding:30px;">
        <img src="logo" />
    </div>

    <br />
    <br />-->

    <div style="background-color: #ece8d4;
width:600px; height:200px; padding:30px; margin-top:30px;">

        <p>Dear bangvl@softcom.vn,<p>

        <p>Please click the link below to active your account.</p>
        <p>
            <a href="http://192.168.2.48:1991/WebAuth/ActiveAccount?token=1f6ffaa72285d66327ad00a6ba32b829.YmFuZ3ZsQHNvZnRjb20udm58YmFuZ3ZsQHNvZnRjb20udm4=.cbc2fa0c9e72e35ffe8fbf39a41573fe">CLICK</a>

            <br />
        <p>Thank you</p>
    </div>
</body>

</html>', N'', N'bangvl@softcom.vn', N'active_account', 1003080, 0, 0, NULL, CAST(N'2018-08-17T16:34:05.463' AS DateTime))
INSERT [dbo].[tbl_system_emails] ([Id], [Subject], [Body], [Sender], [Receiver], [Action], [ReceiverId], [IsSent], [IsRead], [ReadDate], [CreatedDate]) VALUES (7, N'Recover password', N'<html>
<body style="color:grey; font-size:15px;">
    <font face="Helvetica, Arial, sans-serif" />

    <!--<div style="position:absolute; height:100px; width:600px; background-color:#0d1d36; padding:30px;">
        <img src="logo" />
    </div>

    <br />
    <br />-->

    <div style="background-color: #ece8d4;
width:600px; height:200px; padding:30px; margin-top:30px;">

        <p>Dear bangvl@softcom.vn,<p>

        <p>Please click the link below to recover your password.</p>
        <p>
            <a href="http://192.168.2.48:1991/WebAuth/Password_Reset?token=b23f0bb1d398812306b791662898e114.MTAwMzA4MHxsZXZlbDE=.c7371379bfcab7d6080039a433af4f03">Begin recover</a>

            <br />
        <p>Thank you</p>
    </div>
</body>

</html>', N'', N'bangvl@softcom.vn', N'recover_password1', 1003080, 0, 0, NULL, CAST(N'2018-08-17T16:36:42.380' AS DateTime))
INSERT [dbo].[tbl_system_emails] ([Id], [Subject], [Body], [Sender], [Receiver], [Action], [ReceiverId], [IsSent], [IsRead], [ReadDate], [CreatedDate]) VALUES (8, N'Registration verify', N'<html>
<body style="color:grey; font-size:15px;">
    <font face="Helvetica, Arial, sans-serif" />

    <!--<div style="position:absolute; height:100px; width:600px; background-color:#0d1d36; padding:30px;">
        <img src="logo" />
    </div>

    <br />
    <br />-->

    <div style="background-color: #ece8d4;
width:600px; height:200px; padding:30px; margin-top:30px;">

        <p>Dear registeronly12891@gmail.com,<p>

        <p>Please click the link below to active your account.</p>
        <p>
            <a href="http://192.168.2.48:1991/WebAuth/ActiveAccount?token=991cfa381e5c42aa40bc02e30e944c3c.cmVnaXN0ZXJvbmx5MTI4OTFAZ21haWwuY29tfHJlZ2lzdGVyb25seTEyODkxQGdtYWlsLmNvbQ==.c0534157780d099f0340b3ab8e8a658f">CLICK</a>

            <br />
        <p>Thank you</p>
    </div>
</body>

</html>', N'', N'registeronly12891@gmail.com', N'active_account', 1003081, 0, 0, NULL, CAST(N'2018-08-17T16:38:55.843' AS DateTime))
INSERT [dbo].[tbl_system_emails] ([Id], [Subject], [Body], [Sender], [Receiver], [Action], [ReceiverId], [IsSent], [IsRead], [ReadDate], [CreatedDate]) VALUES (11, N'Registration verify', N'<html>
<body style="color:grey; font-size:15px;">
    <font face="Helvetica, Arial, sans-serif" />

    <!--<div style="position:absolute; height:100px; width:600px; background-color:#0d1d36; padding:30px;">
        <img src="logo" />
    </div>

    <br />
    <br />-->

    <div style="background-color: #ece8d4;
width:600px; height:200px; padding:30px; margin-top:30px;">

        <p>Dear vuducthuong1103@gmail.com,<p>

        <p>Please click the link below to active your account.</p>
        <p>
            <a href="http://192.168.2.48:1991/WebAuth/ActiveAccount?token=5932fd11663839691f0eecea167f4ada.dnVkdWN0aHVvbmcxMTAzQGdtYWlsLmNvbXx2dWR1Y3RodW9uZzExMDNAZ21haWwuY29t.37cb5101b7e01c66110dea37740643b5">CLICK</a>

            <br />
        <p>Thank you</p>
    </div>
</body>

</html>', N'', N'vuducthuong1103@gmail.com', N'active_account', 1003083, 0, 0, NULL, CAST(N'2018-08-21T16:13:04.400' AS DateTime))
INSERT [dbo].[tbl_system_emails] ([Id], [Subject], [Body], [Sender], [Receiver], [Action], [ReceiverId], [IsSent], [IsRead], [ReadDate], [CreatedDate]) VALUES (12, N'Khôi phục mật khẩu', N'<html>
<body style="color:grey; font-size:15px;">
    <font face="Helvetica, Arial, sans-serif" />

    <!--<div style="position:absolute; height:100px; width:600px; background-color:#0d1d36; padding:30px;">
        <img src="logo" />
    </div>

    <br />
    <br />-->

    <div style="background-color: #ece8d4;
width:600px; height:200px; padding:30px; margin-top:30px;">

        <p>Dear bangkhmt3@gmail.com,<p>

        <p>Please click the link below to recover your password.</p>
        <p>
            <a href="http://192.168.2.48:1991/WebAuth/Password_Reset?token=7b9ef03c96e93dbb028f2adbe2ce8e40.MXxzYW1wbGUgc3RyaW5nIDM=.c4ca4238a0b923820dcc509a6f75849b">Begin recover</a>

            <br />
        <p>Thank you</p>
    </div>
</body>

</html>', N'', N'bangkhmt3@gmail.com', N'recover_password2', 1, 0, 0, NULL, CAST(N'2019-09-07T10:21:13.613' AS DateTime))
INSERT [dbo].[tbl_system_emails] ([Id], [Subject], [Body], [Sender], [Receiver], [Action], [ReceiverId], [IsSent], [IsRead], [ReadDate], [CreatedDate]) VALUES (13, N'Khôi phục mật khẩu', N'<html>
<body style="color:grey; font-size:15px;">
    <font face="Helvetica, Arial, sans-serif" />

    <!--<div style="position:absolute; height:100px; width:600px; background-color:#0d1d36; padding:30px;">
        <img src="logo" />
    </div>

    <br />
    <br />-->

    <div style="background-color: #ece8d4;
width:600px; height:200px; padding:30px; margin-top:30px;">

        <p>Dear kakahaha6193@gmail.com,<p>

        <p>Please click the link below to recover your password.</p>
        <p>
            <a href="http://192.168.2.48:1991/WebAuth/Password_Reset?token=e10e58d9ad8633552062e29e75b170ae.MTJ8c2FtcGxlIHN0cmluZyAz.c20ad4d76fe97759aa27a0c99bff6710">Begin recover</a>

            <br />
        <p>Thank you</p>
    </div>
</body>

</html>', N'', N'kakahaha6193@gmail.com', N'recover_password2', 12, 0, 0, NULL, CAST(N'2019-09-07T10:21:33.067' AS DateTime))
INSERT [dbo].[tbl_system_emails] ([Id], [Subject], [Body], [Sender], [Receiver], [Action], [ReceiverId], [IsSent], [IsRead], [ReadDate], [CreatedDate]) VALUES (14, N'Khôi phục mật khẩu', N'<html>
<body style="color:grey; font-size:15px;">
    <font face="Helvetica, Arial, sans-serif" />

    <!--<div style="position:absolute; height:100px; width:600px; background-color:#0d1d36; padding:30px;">
        <img src="logo" />
    </div>

    <br />
    <br />-->

    <div style="background-color: #ece8d4;
width:600px; height:200px; padding:30px; margin-top:30px;">

        <p>Dear kakahaha6193@gmail.com,<p>

        <p>Please click the link below to recover your password.</p>
        <p>
            <a href="http://192.168.2.48:1991/WebAuth/Password_Reset?token=bc95fa6cf61fddbe9f138eae6e6667a9.MTJ8c2FtcGxlIHN0cmluZyAz.c20ad4d76fe97759aa27a0c99bff6710">Begin recover</a>

            <br />
        <p>Thank you</p>
    </div>
</body>

</html>', N'', N'kakahaha6193@gmail.com', N'recover_password2', 12, 0, 0, NULL, CAST(N'2019-09-07T10:22:07.893' AS DateTime))
SET IDENTITY_INSERT [dbo].[tbl_system_emails] OFF
INSERT [dbo].[tbl_system_emails_history] ([Id], [Subject], [Body], [Sender], [Receiver], [Action], [ReceiverId], [IsSent], [IsRead], [ReadDate], [CreatedDate]) VALUES (4, N'Xác nhận đăng ký tài khoản', N'<html>
<body style="color:grey; font-size:15px;">
    <font face="Helvetica, Arial, sans-serif" />

    <!--<div style="position:absolute; height:100px; width:600px; background-color:#0d1d36; padding:30px;">
        <img src="logo" />
    </div>

    <br />
    <br />-->

    <div style="background-color: #ece8d4;
width:600px; height:200px; padding:30px; margin-top:30px;">

        <p>Dear bangvl@softcom.vn,<p>

        <p>Please click the link below to active your account.</p>
        <p>
            <a href="http://192.168.2.48:1991/WebAuth/ActiveAccount?token=b15eca694aad6e4b234f6979d3281800.YmFuZ3ZsQHNvZnRjb20udm58YmFuZ3ZsQHNvZnRjb20udm4=.cbc2fa0c9e72e35ffe8fbf39a41573fe">CLICK</a>

            <br />
        <p>Thank you</p>
    </div>
</body>

</html>', N'', N'bangvl@softcom.vn', N'active_account', 1002061, 1, 1, CAST(N'2018-07-11T10:21:09.600' AS DateTime), CAST(N'2018-07-11T10:20:38.820' AS DateTime))
INSERT [dbo].[tbl_system_emails_history] ([Id], [Subject], [Body], [Sender], [Receiver], [Action], [ReceiverId], [IsSent], [IsRead], [ReadDate], [CreatedDate]) VALUES (5, N'Xác nhận đăng ký tài khoản', N'<html>
<body style="color:grey; font-size:15px;">
    <font face="Helvetica, Arial, sans-serif" />

    <!--<div style="position:absolute; height:100px; width:600px; background-color:#0d1d36; padding:30px;">
        <img src="logo" />
    </div>

    <br />
    <br />-->

    <div style="background-color: #ece8d4;
width:600px; height:200px; padding:30px; margin-top:30px;">

        <p>Dear bangvl@softcom.vn,<p>

        <p>Please click the link below to active your account.</p>
        <p>
            <a href="http://192.168.2.48:1991/WebAuth/ActiveAccount?token=a0a49127401dd5a4e2471f6365a3231a.YmFuZ3ZsQHNvZnRjb20udm58YmFuZ3ZsQHNvZnRjb20udm4=.cbc2fa0c9e72e35ffe8fbf39a41573fe">CLICK</a>

            <br />
        <p>Thank you</p>
    </div>
</body>

</html>', N'', N'bangvl@softcom.vn', N'active_account', 1002062, 1, 1, CAST(N'2018-07-11T10:58:02.020' AS DateTime), CAST(N'2018-07-11T10:51:06.920' AS DateTime))
INSERT [dbo].[tbl_system_emails_history] ([Id], [Subject], [Body], [Sender], [Receiver], [Action], [ReceiverId], [IsSent], [IsRead], [ReadDate], [CreatedDate]) VALUES (9, N'Registration verify', N'<html>
<body style="color:grey; font-size:15px;">
    <font face="Helvetica, Arial, sans-serif" />

    <!--<div style="position:absolute; height:100px; width:600px; background-color:#0d1d36; padding:30px;">
        <img src="logo" />
    </div>

    <br />
    <br />-->

    <div style="background-color: #ece8d4;
width:600px; height:200px; padding:30px; margin-top:30px;">

        <p>Dear registeronly12891@gmail.com,<p>

        <p>Please click the link below to active your account.</p>
        <p>
            <a href="http://192.168.2.48:1991/WebAuth/ActiveAccount?token=456258664a5024ec43328d87a49a71eb.cmVnaXN0ZXJvbmx5MTI4OTFAZ21haWwuY29tfHJlZ2lzdGVyb25seTEyODkxQGdtYWlsLmNvbQ==.c0534157780d099f0340b3ab8e8a658f">CLICK</a>

            <br />
        <p>Thank you</p>
    </div>
</body>

</html>', N'', N'registeronly12891@gmail.com', N'active_account', 1003082, 1, 1, CAST(N'2018-08-17T17:32:33.173' AS DateTime), CAST(N'2018-08-17T17:15:19.133' AS DateTime))
INSERT [dbo].[tbl_system_emails_history] ([Id], [Subject], [Body], [Sender], [Receiver], [Action], [ReceiverId], [IsSent], [IsRead], [ReadDate], [CreatedDate]) VALUES (10, N'Recover password', N'<html>
<body style="color:grey; font-size:15px;">
    <font face="Helvetica, Arial, sans-serif" />

    <!--<div style="position:absolute; height:100px; width:600px; background-color:#0d1d36; padding:30px;">
        <img src="logo" />
    </div>

    <br />
    <br />-->

    <div style="background-color: #ece8d4;
width:600px; height:200px; padding:30px; margin-top:30px;">

        <p>Dear vuducthuong1102@gmail.com,<p>

        <p>Please click the link below to recover your password.</p>
        <p>
            <a href="http://192.168.2.48:1991/WebAuth/Password_Reset?token=dd1b86fbd00a814d78b564bcc538df19.MTAwMzA3N3xsZXZlbDE=.9519615a90cfacbdd339676183c0b606">Begin recover</a>

            <br />
        <p>Thank you</p>
    </div>
</body>

</html>', N'', N'vuducthuong1102@gmail.com', N'recover_password1', 1003077, 1, 1, CAST(N'2018-08-21T15:14:04.987' AS DateTime), CAST(N'2018-08-21T15:13:18.997' AS DateTime))
INSERT [dbo].[tbl_system_emails_history] ([Id], [Subject], [Body], [Sender], [Receiver], [Action], [ReceiverId], [IsSent], [IsRead], [ReadDate], [CreatedDate]) VALUES (12, N'Khôi phục mật khẩu', N'<html>
<body style="color:grey; font-size:15px;">
    <font face="Helvetica, Arial, sans-serif" />

    <!--<div style="position:absolute; height:100px; width:600px; background-color:#0d1d36; padding:30px;">
        <img src="logo" />
    </div>

    <br />
    <br />-->

    <div style="background-color: #ece8d4;
width:600px; height:200px; padding:30px; margin-top:30px;">

        <p>Dear bangkhmt3@gmail.com,<p>

        <p>Please click the link below to recover your password.</p>
        <p>
            <a href="http://192.168.2.48:1991/WebAuth/Password_Reset?token=04a04c087a7c33c57e7df0f8984ae6c6.N3xsZXZlbDE=.8f14e45fceea167a5a36dedd4bea2543">Begin recover</a>

            <br />
        <p>Thank you</p>
    </div>
</body>

</html>', N'', N'bangkhmt3@gmail.com', N'recover_password1', 7, 1, 1, CAST(N'2018-08-23T15:34:51.767' AS DateTime), CAST(N'2018-08-23T15:34:34.503' AS DateTime))
SET IDENTITY_INSERT [dbo].[tbl_traces] ON 

INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (1, N'login', N'27.72.101.188', N'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/76.0.3809.100 Safari/537.36', N'WEB', N'account.job-market.jp', CAST(N'2019-08-16T14:49:52.467' AS DateTime), N'User [2] logged in successfully with token [dc78714df2a48c755a21e9e8bf349f7c]', N'{"userName":"tester@gmail.com","password":"e10adc3949ba59abbe56e057f20f883e","urlReturn":"/WebAccount/ViewProfile","numberOfFailedLogins":3}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (2, N'login', N'171.253.57.130', N'Mozilla/5.0 (Linux; Android 6.0.1; SM-N910U) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/72.0.3626.121 Mobile Safari/537.36', N'WEB', N'account.job-market.jp', CAST(N'2019-08-21T22:56:11.393' AS DateTime), N'User [2] logged in successfully with token [470f7947663409e34ec9048e646fa5d0]', N'{"userName":"tester@gmail.com","password":"e10adc3949ba59abbe56e057f20f883e","urlReturn":"/WebAccount/ViewProfile","numberOfFailedLogins":6}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (3, N'login', N'27.72.101.188', N'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/76.0.3809.100 Safari/537.36', N'WEB', N'account.job-market.jp', CAST(N'2019-08-22T08:35:02.860' AS DateTime), N'User [1] logged in successfully with token [8050ad151dd0d90b84a3adcc3d656c1d]', N'{"userName":"bangkhmt3@gmail.com","password":"e10adc3949ba59abbe56e057f20f883e","urlReturn":null,"numberOfFailedLogins":2}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (4, N'login', N'27.72.101.188', N'PostmanRuntime/7.15.2', N'API', N'account.job-market.jp', CAST(N'2019-08-22T16:10:15.307' AS DateTime), N'User [1] logged in successfully with token [5ec09526da33f33cce8d6a05b3ef5e0a]', N'{"userName":"bangkhmt3@gmail.com","password":"e10adc3949ba59abbe56e057f20f883e","time":"20190922160700","hash":"8d3bacbff1d163a3975f81666f06f780","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (5, N'register', N'27.72.101.188', N'PostmanRuntime/7.15.2', N'API', N'account.job-market.jp', CAST(N'2019-08-26T08:59:30.573' AS DateTime), N'User [10] was registered successfully. Need to be done by account activation', N'{"userName":"huynhtuanhuy1996@gmail.com","password":"e10adc3949ba59abbe56e057f20f883e","email":null,"birthday":null,"sex":null,"address":null,"full_Name":null,"display_Name":null,"cmtnd":null,"phone":null,"note":null,"time":null,"isEmail":false,"isPhoneNumber":false}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (6, N'login', N'45.118.145.151', NULL, N'API', N'account.job-market.jp', CAST(N'2019-08-29T16:55:50.900' AS DateTime), N'User [1] logged in successfully with token [d03a9409872b87cc0f20521f4540533c]', N'{"userName":"bangkhmt3@gmail.com","password":"e10adc3949ba59abbe56e057f20f883e","time":"20190829165550","hash":"6beec35b3473b548433873bd3b024197","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (7, N'login', N'45.118.145.151', NULL, N'API', N'account.job-market.jp', CAST(N'2019-08-30T20:15:30.773' AS DateTime), N'User [1] logged in successfully with token [53e2996454c9f7db14ab5590ed3d88ba]', N'{"userName":"bangkhmt3@gmail.com","password":"e10adc3949ba59abbe56e057f20f883e","time":"20190830201530","hash":"173f3f59eb7f0469096ee7fef95d6314","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (8, N'login', N'45.118.145.151', NULL, N'API', N'account.job-market.jp', CAST(N'2019-08-31T11:23:10.067' AS DateTime), N'User [1] logged in successfully with token [e394c2b9f0b8cc7574f6037dc3c18347]', N'{"userName":"bangkhmt3@gmail.com","password":"e10adc3949ba59abbe56e057f20f883e","time":"20190831112307","hash":"e782e005a300f0571bc47165051d1357","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (9, N'login', N'45.118.145.151', NULL, N'API', N'account.job-market.jp', CAST(N'2019-09-04T22:51:19.763' AS DateTime), N'User [1] logged in successfully with token [3a1c28bc237bbb564e8c5d64374925e0]', N'{"userName":"bangkhmt3@gmail.com","password":"e10adc3949ba59abbe56e057f20f883e","time":"20190904225119","hash":"b04fdc11525a0ea9f80b97ea38c70834","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (10, N'login', N'45.118.145.151', NULL, N'API', N'account.job-market.jp', CAST(N'2019-09-05T16:24:32.830' AS DateTime), N'User [1] logged in successfully with token [74b325332329f1feffdf28c754af1f36]', N'{"userName":"bangkhmt3@gmail.com","password":"e10adc3949ba59abbe56e057f20f883e","time":"20190905162431","hash":"3f7aa0ca280994b7183b54852e1ef5f8","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (11, N'register', N'14.177.223.120', N'Mozilla/5.0 (Macintosh; Intel Mac OS X 10_14_5) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/76.0.3809.132 Safari/537.36', N'API', N'account.job-market.jp', CAST(N'2019-09-07T07:18:09.617' AS DateTime), N'User [11] was registered successfully. Need to be done by account activation', N'{"userName":"minhduc6193@gmail.com","password":"kakahaha@1","email":null,"birthday":null,"sex":null,"address":null,"full_Name":null,"display_Name":null,"cmtnd":null,"phone":null,"note":null,"time":null,"isEmail":false,"isPhoneNumber":false}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (12, N'register', N'27.72.101.188', N'ViecLamTiengNhat/4 CFNetwork/976 Darwin/18.2.0', N'API', N'account.job-market.jp', CAST(N'2019-09-07T10:05:03.693' AS DateTime), N'User [12] was registered successfully. Need to be done by account activation', N'{"userName":"kakahaha6193@gmail.com","password":"fc7df0b596b34c9703c8dc050a9bf271","email":null,"birthday":null,"sex":null,"address":null,"full_Name":null,"display_Name":null,"cmtnd":null,"phone":null,"note":null,"time":null,"isEmail":false,"isPhoneNumber":false}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (13, N'login', N'27.72.101.188', N'ViecLamTiengNhat/4 CFNetwork/976 Darwin/18.2.0', N'API', N'account.job-market.jp', CAST(N'2019-09-07T10:05:24.427' AS DateTime), N'User [12] logged in successfully with token [5c109af7ae3cc16238f154885fee829c]', N'{"userName":"kakahaha6193@gmail.com","password":"fc7df0b596b34c9703c8dc050a9bf271","time":"20190907100524","hash":"83f2a0081fd8e5e579c4f23fa9437615","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (14, N'login', N'27.72.101.188', N'PostmanRuntime/7.16.3', N'API', N'account.job-market.jp', CAST(N'2019-09-12T08:05:04.180' AS DateTime), N'User [16] logged in successfully with token [1a2a41b369aba30adbe8d26df9bbcb0e]', N'{"userName":"124725983475","email":"xyz@gmail.com","socialProvider":"facebook","displayName":"Nguyen Van A"}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (15, N'login', N'14.162.38.190', N'ViecLamTiengNhat/1 CFNetwork/978.0.7 Darwin/18.7.0', N'API', N'account.job-market.jp', CAST(N'2019-09-13T06:23:12.900' AS DateTime), N'User [17] logged in successfully with token [374e98eecd44d91545a312b5a8503cc4]', N'{"userName":"1374291676057314","email":null,"socialProvider":"facebook","displayName":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (16, N'login', N'45.118.145.151', NULL, N'API', N'account.job-market.jp', CAST(N'2019-09-13T10:07:54.887' AS DateTime), N'User [1] logged in successfully with token [fe36a2dbf4971f113258a4f5e5e87a89]', N'{"userName":"bangkhmt3@gmail.com","password":"e10adc3949ba59abbe56e057f20f883e","time":"20190913100749","hash":"df57b16193b3bb7633d13f9deaa198a8","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (17, N'login', N'27.72.101.188', N'ViecLamTiengNhat/2 CFNetwork/893.14.2 Darwin/17.3.0', N'API', N'account.job-market.jp', CAST(N'2019-09-14T09:06:58.857' AS DateTime), N'User [18] logged in successfully with token [cd592503402d3bc5fc96ca72000b2ce4]', N'{"userName":"1406848862813015","email":null,"socialProvider":"facebook","displayName":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (18, N'login', N'45.118.145.151', NULL, N'API', N'account.job-market.jp', CAST(N'2019-09-14T15:07:12.380' AS DateTime), N'User [1] logged in successfully with token [cc9d3f0bc3d22c78ce8a6374fc711083]', N'{"userName":"bangkhmt3@gmail.com","password":"e10adc3949ba59abbe56e057f20f883e","time":"20190914150708","hash":"b5dae1400d9f29efbf5ee0abff7f6a0d","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (19, N'register', N'27.72.101.188', N'ViecLamTiengNhat/2 CFNetwork/893.14.2 Darwin/17.3.0', N'API', N'account.job-market.jp', CAST(N'2019-09-14T17:24:17.333' AS DateTime), N'User [19] was registered successfully. Need to be done by account activation', N'{"userName":"minhnv54@gmail.com","password":"08a4984d6cec066ab5707cdb42882f1c","email":null,"birthday":null,"sex":null,"address":null,"full_Name":null,"display_Name":null,"cmtnd":null,"phone":null,"note":null,"time":null,"isEmail":false,"isPhoneNumber":false}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (20, N'login', N'27.72.101.188', N'ViecLamTiengNhat/2 CFNetwork/893.14.2 Darwin/17.3.0', N'API', N'account.job-market.jp', CAST(N'2019-09-14T17:24:51.460' AS DateTime), N'User [19] logged in successfully with token [2caad16d82f1fdf33ea55748f4ec5ccf]', N'{"userName":"minhnv54@gmail.com","password":"08a4984d6cec066ab5707cdb42882f1c","time":"20190914172451","hash":"19f1f71256f4f395e418cb1c54f9b635","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (21, N'login', N'14.162.38.190', N'ViecLamTiengNhat/2 CFNetwork/978.0.7 Darwin/18.7.0', N'API', N'account.job-market.jp', CAST(N'2019-09-14T22:35:41.813' AS DateTime), N'User [20] logged in successfully with token [e96ffd53bdeb57bddc93849291e51dd7]', N'{"userName":"minhittduc@gmail.com","email":null,"socialProvider":"google","displayName":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (22, N'login', N'14.162.38.190', N'ViecLamTiengNhat/2 CFNetwork/978.0.7 Darwin/18.7.0', N'API', N'account.job-market.jp', CAST(N'2019-09-14T22:37:14.860' AS DateTime), N'User [17] logged in successfully with token [19a558ee6b55d0b5d9af2a2804d8d7a4]', N'{"userName":"1374291676057314","email":null,"socialProvider":"facebook","displayName":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (23, N'login', N'14.248.107.66', N'ViecLamTiengNhat/2 CFNetwork/893.14.2 Darwin/17.3.0', N'API', N'account.job-market.jp', CAST(N'2019-09-15T00:03:46.103' AS DateTime), N'User [19] logged in successfully with token [c9752544ec9abd7efe5fb040c87b352f]', N'{"userName":"minhnv54@gmail.com","password":"08a4984d6cec066ab5707cdb42882f1c","time":"20190915000346","hash":"13b7d12ecc4a4cdc6b5aa9359ea53beb","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (24, N'login', N'14.248.107.66', N'ViecLamTiengNhat/3 CFNetwork/893.14.2 Darwin/17.3.0', N'API', N'account.job-market.jp', CAST(N'2019-09-15T00:05:03.420' AS DateTime), N'User [21] logged in successfully with token [cdebb95a53e7f1bd5ccb02b04c9cca02]', N'{"userName":"minhnv54@gmail.com","email":null,"socialProvider":"google","displayName":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (25, N'login', N'45.118.145.151', NULL, N'API', N'account.job-market.jp', CAST(N'2019-09-15T14:43:36.850' AS DateTime), N'User [1] logged in successfully with token [7ac5fb4ac5815315be3e54b4b0bc42b2]', N'{"userName":"bangkhmt3@gmail.com","password":"e10adc3949ba59abbe56e057f20f883e","time":"20190915144336","hash":"cffc3c55ce9a563992083ba2ff5104cc","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (26, N'login', N'14.162.38.190', N'ViecLamTiengNhat/3 CFNetwork/978.0.7 Darwin/18.7.0', N'API', N'account.job-market.jp', CAST(N'2019-09-16T05:23:09.183' AS DateTime), N'User [20] logged in successfully with token [cd57dc604cbb8d8f3dafbbeb9de72715]', N'{"userName":"minhittduc@gmail.com","email":null,"socialProvider":"google","displayName":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (27, N'login', N'14.162.38.190', N'ViecLamTiengNhat/3 CFNetwork/978.0.7 Darwin/18.7.0', N'API', N'account.job-market.jp', CAST(N'2019-09-16T06:07:50.310' AS DateTime), N'User [17] logged in successfully with token [6c660bca308357e6881b87b34d6002d3]', N'{"userName":"1374291676057314","email":null,"socialProvider":"facebook","displayName":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (28, N'login', N'27.72.101.188', N'ViecLamTiengNhat/4 CFNetwork/893.14.2 Darwin/17.3.0', N'API', N'account.job-market.jp', CAST(N'2019-09-16T08:32:03.413' AS DateTime), N'User [19] logged in successfully with token [7e4259d5547ba53cc6a1608db55fda3f]', N'{"userName":"minhnv54@gmail.com","password":"08a4984d6cec066ab5707cdb42882f1c","time":"20190916083203","hash":"25ea32cd1794017acfbab2754c59aff1","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (29, N'login', N'45.118.145.151', NULL, N'API', N'account.job-market.jp', CAST(N'2019-09-16T11:18:37.410' AS DateTime), N'User [1] logged in successfully with token [2346a80a2d0d03f54b64c965770606d6]', N'{"userName":"bangkhmt3@gmail.com","password":"e10adc3949ba59abbe56e057f20f883e","time":"20190916111837","hash":"aaca7a153d81227a2ab81926caf70d59","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (30, N'login', N'45.118.145.151', NULL, N'API', N'account.job-market.jp', CAST(N'2019-09-16T17:43:59.577' AS DateTime), N'User [1] logged in successfully with token [22f6095e32a0b960a45ec139f47af27c]', N'{"userName":"bangkhmt3@gmail.com","password":"e10adc3949ba59abbe56e057f20f883e","time":"20190916174359","hash":"7d57b549104837c08417cc89f297289a","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (31, N'login', N'14.162.38.190', N'ViecLamTiengNhat/4 CFNetwork/978.0.7 Darwin/18.7.0', N'API', N'account.job-market.jp', CAST(N'2019-09-17T05:39:58.437' AS DateTime), N'User [20] logged in successfully with token [2c590b5124c8b2767732128f950bd47a]', N'{"userName":"minhittduc@gmail.com","email":null,"socialProvider":"google","displayName":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (32, N'register', N'27.72.101.188', N'ViecLamTiengNhat/4 CFNetwork/978.0.7 Darwin/18.7.0', N'API', N'account.job-market.jp', CAST(N'2019-09-18T09:19:03.723' AS DateTime), N'User [22] was registered successfully. Need to be done by account activation', N'{"userName":"huynhtuanhuy1996@gmail.com","password":"a10c109915e36770c291dcd3880f8517","email":null,"birthday":null,"sex":null,"address":null,"full_Name":null,"display_Name":null,"cmtnd":null,"phone":null,"note":null,"time":null,"isEmail":false,"isPhoneNumber":false}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (33, N'login', N'27.72.101.188', N'ViecLamTiengNhat/4 CFNetwork/978.0.7 Darwin/18.7.0', N'API', N'account.job-market.jp', CAST(N'2019-09-18T09:19:16.613' AS DateTime), N'User [22] logged in successfully with token [b36c6027ab7872d36c8d05036d8d2bd8]', N'{"userName":"huynhtuanhuy1996@gmail.com","password":"a10c109915e36770c291dcd3880f8517","time":"20190918092009","hash":"51135191dbff88c1164ed288fbdaff3d","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (34, N'register', N'27.72.101.188', N'PostmanRuntime/7.17.1', N'API', N'account.job-market.jp', CAST(N'2019-09-18T16:10:42.437' AS DateTime), N'User [24] was registered successfully. Need to be done by account activation', N'{"userName":"registeronly12892@gmail.com","password":"e10adc3949ba59abbe56e057f20f883e@1","email":"registeronly12891@gmail.com","birthday":null,"sex":null,"address":null,"full_Name":null,"display_Name":null,"cmtnd":null,"phone":null,"note":null,"time":"20190918160700","isEmail":false,"isPhoneNumber":false}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (35, N'register', N'27.72.101.188', N'PostmanRuntime/7.16.3', N'API', N'account.job-market.jp', CAST(N'2019-09-18T16:13:12.283' AS DateTime), N'User [25] was registered successfully. Need to be done by account activation', N'{"userName":"registeronly1289223@gmail.com","password":"e10adc3949ba59abbe56e057f20f883e@1","email":"registeronly1289123@gmail.com","birthday":null,"sex":null,"address":null,"full_Name":null,"display_Name":null,"cmtnd":null,"phone":null,"note":null,"time":"20190918160700","isEmail":false,"isPhoneNumber":false}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (36, N'register', N'27.72.101.188', N'PostmanRuntime/7.16.3', N'API', N'account.job-market.jp', CAST(N'2019-09-18T16:14:32.220' AS DateTime), N'User [27] was registered successfully. Need to be done by account activation', N'{"userName":"registeronly1289123asdasd@gmail.com","password":"e10adc3949ba59abbe56e057f20f883e@1","email":"registeronly1289123asasd@gmail.com","birthday":null,"sex":null,"address":null,"full_Name":null,"display_Name":null,"cmtnd":null,"phone":null,"note":null,"time":"20190918160700","isEmail":false,"isPhoneNumber":false}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (37, N'login', N'45.118.145.151', NULL, N'API', N'account.job-market.jp', CAST(N'2019-09-24T10:24:49.833' AS DateTime), N'User [1] logged in successfully with token [ba5e3a05ad2c8ea5fd4b01d0532f6bd4]', N'{"userName":"bangkhmt3@gmail.com","password":"e10adc3949ba59abbe56e057f20f883e","time":"20190924102446","hash":"45bec0f350f8439ada6b2761bc9ac82b","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (38, N'login', N'45.118.145.151', NULL, N'API', N'account.job-market.jp', CAST(N'2019-09-24T17:14:03.770' AS DateTime), N'User [1] logged in successfully with token [34fe88009906231777b123b9dc5ab0fc]', N'{"userName":"bangkhmt3@gmail.com","password":"e10adc3949ba59abbe56e057f20f883e","time":"20190924171403","hash":"08bcfae2e384244782c2a87218885d5d","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (39, N'login', N'42.112.216.184', N'ViecLamTiengNhat/5 CFNetwork/978.0.7 Darwin/18.7.0', N'API', N'account.job-market.jp', CAST(N'2019-09-25T11:03:07.407' AS DateTime), N'User [22] logged in successfully with token [e85221c94af048bafe3aa89f5fc72a34]', N'{"userName":"huynhtuanhuy1996@gmail.com","password":"a10c109915e36770c291dcd3880f8517","time":"20190925110307","hash":"969478a8f7391883d3b5a99519d2808b","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (40, N'register', N'45.118.145.151', NULL, N'API', N'account.job-market.jp', CAST(N'2019-09-27T10:52:50.820' AS DateTime), N'User [28] was registered successfully. Need to be done by account activation', N'{"userName":"vuducthuong1102@gmail.com","password":"e10adc3949ba59abbe56e057f20f883e","email":"vuducthuong1102@gmail.com","birthday":null,"sex":null,"address":null,"full_Name":null,"display_Name":"vuducthuong1102@gmail.com","cmtnd":null,"phone":null,"note":null,"time":"20190927105245","isEmail":true,"isPhoneNumber":false}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (41, N'login', N'45.118.145.151', NULL, N'API', N'account.job-market.jp', CAST(N'2019-09-27T10:53:34.973' AS DateTime), N'User [28] logged in successfully with token [9357413daef56227d2f74290cd2423df]', N'{"userName":"vuducthuong1102@gmail.com","password":"e10adc3949ba59abbe56e057f20f883e","time":"20190927105334","hash":"83fd395bc2e78ca2d16da44b660fe504","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (42, N'login', N'45.118.145.151', NULL, N'API', N'account.job-market.jp', CAST(N'2019-09-27T21:36:42.627' AS DateTime), N'User [1] logged in successfully with token [0292420656a536c81687f495db6ecbec]', N'{"userName":"bangkhmt3@gmail.com","password":"e10adc3949ba59abbe56e057f20f883e","time":"20190927213639","hash":"6a8040551e47a8df700b0409811a1b2f","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (43, N'login', N'113.190.167.57', N'ViecLamTiengNhat/5 CFNetwork/1098.7 Darwin/18.7.0', N'API', N'account.job-market.jp', CAST(N'2019-09-29T06:02:21.510' AS DateTime), N'User [22] logged in successfully with token [9bf432a51692d994ebfc3d762fda38a5]', N'{"userName":"huynhtuanhuy1996@gmail.com","password":"a10c109915e36770c291dcd3880f8517","time":"20190929060227","hash":"740166f3341c00fb86a959d419a154e6","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (44, N'register', N'14.181.187.180', N'PostmanRuntime/7.17.1', N'API', N'account.job-market.jp', CAST(N'2019-09-29T09:14:18.090' AS DateTime), N'User [29] was registered successfully. Need to be done by account activation', N'{"userName":"tranducminhhpvn99@gmail.com","password":"87ce06dec453f68c0266ee800ce76873","email":"tranducminhhpvn99@gmail.com","birthday":null,"sex":null,"address":null,"full_Name":null,"display_Name":null,"cmtnd":null,"phone":null,"note":null,"time":"20190929160700","isEmail":false,"isPhoneNumber":false}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (45, N'login', N'14.181.187.180', N'PostmanRuntime/7.17.1', N'API', N'account.job-market.jp', CAST(N'2019-09-29T09:21:16.270' AS DateTime), N'User [29] logged in successfully with token [7142ecec599c4be67f9dfe75ce9d18fd]', N'{"userName":"tranducminhhpvn99@gmail.com","password":"87ce06dec453f68c0266ee800ce76873","time":"20190929160900","hash":"0ed62f32f5a1597a26cf4b6f91891a5a","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (46, N'login', N'45.118.145.151', NULL, N'API', N'account.job-market.jp', CAST(N'2019-10-01T16:15:21.690' AS DateTime), N'User [1] logged in successfully with token [c5949025f9596af78af3f7055c42b99e]', N'{"userName":"bangkhmt3@gmail.com","password":"e10adc3949ba59abbe56e057f20f883e","time":"20191001161519","hash":"3e1dbc9c3fbe4eea2e1c396adfa41040","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (47, N'login', N'45.118.145.151', NULL, N'API', N'account.job-market.jp', CAST(N'2019-10-02T10:19:52.413' AS DateTime), N'User [1] logged in successfully with token [3832ca04659b7aa8f6cd5de337e60183]', N'{"userName":"bangkhmt3@gmail.com","password":"e10adc3949ba59abbe56e057f20f883e","time":"20191002101952","hash":"b3b4903f2ce6673376d0e70915af20a2","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (48, N'login', N'45.118.145.151', NULL, N'API', N'account.job-market.jp', CAST(N'2019-10-02T19:20:41.370' AS DateTime), N'User [29] logged in successfully with token [c19555619daaa630a77c4aa596be2b36]', N'{"userName":"tranducminhhpvn99@gmail.com","password":"87ce06dec453f68c0266ee800ce76873","time":"20191002192038","hash":"6f5a6237fa3858e588748c4c8b98e2f4","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (49, N'register', N'42.112.216.184', N'ViecLamTiengNhat/5 CFNetwork/1098.7 Darwin/18.7.0', N'API', N'account.job-market.jp', CAST(N'2019-10-03T09:13:52.510' AS DateTime), N'User [30] was registered successfully. Need to be done by account activation', N'{"userName":"huynhtuanhuy1997@gmail.com","password":"6f8f57715090da2632453988d9a1501b","email":null,"birthday":null,"sex":null,"address":null,"full_Name":null,"display_Name":null,"cmtnd":null,"phone":null,"note":null,"time":null,"isEmail":false,"isPhoneNumber":false}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (50, N'register', N'42.112.216.184', N'ViecLamTiengNhat/5 CFNetwork/1098.7 Darwin/18.7.0', N'API', N'account.job-market.jp', CAST(N'2019-10-03T09:29:11.910' AS DateTime), N'User [31] was registered successfully. Need to be done by account activation', N'{"userName":"huynhtuanhuy1238@gmail.com","password":"e10adc3949ba59abbe56e057f20f883e","email":null,"birthday":null,"sex":null,"address":null,"full_Name":null,"display_Name":null,"cmtnd":null,"phone":null,"note":null,"time":null,"isEmail":false,"isPhoneNumber":false}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (51, N'login', N'42.112.216.184', N'ViecLamTiengNhat/5 CFNetwork/1098.7 Darwin/18.7.0', N'API', N'account.job-market.jp', CAST(N'2019-10-03T09:29:28.753' AS DateTime), N'User [22] logged in successfully with token [de795f154219951936c8e765aea8fd4c]', N'{"userName":"huynhtuanhuy1996@gmail.com","password":"a10c109915e36770c291dcd3880f8517","time":"20191003092956","hash":"d10ec22f16385fb5a065203c5cdffa4e","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (52, N'register', N'42.112.216.184', N'ViecLamTiengNhat/5 CFNetwork/978.0.7 Darwin/18.7.0', N'API', N'account.job-market.jp', CAST(N'2019-10-03T14:39:02.963' AS DateTime), N'User [32] was registered successfully. Need to be done by account activation', N'{"userName":"huynhtuanhuy1097@gmail.com","password":"6db4ef0c498f805460d4db55d103c4de","email":null,"birthday":null,"sex":null,"address":null,"full_Name":null,"display_Name":null,"cmtnd":null,"phone":null,"note":null,"time":null,"isEmail":false,"isPhoneNumber":false}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (53, N'login', N'45.118.145.151', NULL, N'API', N'account.job-market.jp', CAST(N'2019-10-04T09:38:21.220' AS DateTime), N'User [19] logged in successfully with token [f3c4fd04536b209ae59762e398feb33b]', N'{"userName":"minhnv54@gmail.com","password":"08a4984d6cec066ab5707cdb42882f1c","time":"20191004093821","hash":"a45cfb894f971eb58766cdb166b17a30","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (54, N'login', N'14.162.163.190', N'ViecLamTiengNhat/1 CFNetwork/893.14.2 Darwin/17.3.0', N'API', N'account.job-market.jp', CAST(N'2019-10-04T19:42:02.863' AS DateTime), N'User [19] logged in successfully with token [210233461d1b68822bbc831a5ed33009]', N'{"userName":"minhnv54@gmail.com","password":"08a4984d6cec066ab5707cdb42882f1c","time":"20191004194202","hash":"cf1df0ec16163864917fb3f52605ff1b","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (55, N'login', N'45.118.145.151', NULL, N'API', N'account.job-market.jp', CAST(N'2019-10-05T08:30:43.890' AS DateTime), N'User [1] logged in successfully with token [b42a39069ff20a891507825b04d647d8]', N'{"userName":"bangkhmt3@gmail.com","password":"e10adc3949ba59abbe56e057f20f883e","time":"20191005083040","hash":"87ce5d1ffe3c603dd84b256280ae0706","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (56, N'login', N'45.118.145.151', NULL, N'API', N'account.job-market.jp', CAST(N'2019-10-05T10:58:01.467' AS DateTime), N'User [29] logged in successfully with token [e2ec39ec4326e7a9604d2e24f22a3616]', N'{"userName":"tranducminhhpvn99@gmail.com","password":"87ce06dec453f68c0266ee800ce76873","time":"20191005105801","hash":"f91146ecf39a75703111e5511de8f2af","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (57, N'login', N'118.71.71.114', N'ViecLamTiengNhat/2 CFNetwork/978.0.7 Darwin/18.7.0', N'API', N'account.job-market.jp', CAST(N'2019-10-05T13:22:43.553' AS DateTime), N'User [22] logged in successfully with token [0d823f46da0b7d7da69a6ed8c6c9ee12]', N'{"userName":"huynhtuanhuy1996@gmail.com","password":"a10c109915e36770c291dcd3880f8517","time":"20191005132243","hash":"7f2e38dbb7755ef843a29bee54c9bf22","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (58, N'login', N'118.71.71.114', N'PostmanRuntime/7.17.1', N'API', N'account.job-market.jp', CAST(N'2019-10-05T17:57:17.850' AS DateTime), N'User [16] logged in successfully with token [a78738c4b995b15150ca4cb4ea0217b5]', N'{"userName":"124725983475","email":"xyz@gmail.com","socialProvider":"facebook","displayName":"Nguyen Van A"}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (59, N'login', N'118.71.71.114', N'PostmanRuntime/7.17.1', N'API', N'account.job-market.jp', CAST(N'2019-10-05T17:58:21.617' AS DateTime), N'User [33] logged in successfully with token [60126150505004e2aac337b9f9e6f97b]', N'{"userName":"12321323213","email":"123123231@gmail.com","socialProvider":"facebook","displayName":"Nguyen Van A"}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (60, N'register', N'45.118.145.151', NULL, N'API', N'account.job-market.jp', CAST(N'2019-10-06T18:39:01.890' AS DateTime), N'User [34] was registered successfully. Need to be done by account activation', N'{"userName":"mr.tungvx@gmail.com","password":"bc25d3f216239ea36aa6275ee9752525","email":"mr.tungvx@gmail.com","birthday":null,"sex":null,"address":null,"full_Name":null,"display_Name":"mr.tungvx@gmail.com","cmtnd":null,"phone":null,"note":null,"time":"20191006183857","isEmail":true,"isPhoneNumber":false}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (61, N'login', N'45.118.145.151', NULL, N'API', N'account.job-market.jp', CAST(N'2019-10-06T18:39:32.970' AS DateTime), N'User [34] logged in successfully with token [fc264ba4ad817052f7ac0a786bdba182]', N'{"userName":"mr.tungvx@gmail.com","password":"bc25d3f216239ea36aa6275ee9752525","time":"20191006183932","hash":"73b70762abdc65a6909a1924f3c92caf","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (62, N'register', N'45.118.145.151', NULL, N'API', N'account.job-market.jp', CAST(N'2019-10-07T11:20:00.023' AS DateTime), N'User [35] was registered successfully. Need to be done by account activation', N'{"userName":"thuyngocjp92@gmail.com","password":"b44b0fc0c06383ba1ba7c6aeb926287c","email":"thuyngocjp92@gmail.com","birthday":null,"sex":null,"address":null,"full_Name":null,"display_Name":"thuyngocjp92@gmail.com","cmtnd":null,"phone":null,"note":null,"time":"20191007111955","isEmail":true,"isPhoneNumber":false}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (63, N'login', N'45.118.145.151', NULL, N'API', N'account.job-market.jp', CAST(N'2019-10-07T11:24:18.603' AS DateTime), N'User [35] logged in successfully with token [dabacd345b1771466afa39231f780564]', N'{"userName":"thuyngocjp92@gmail.com","password":"b44b0fc0c06383ba1ba7c6aeb926287c","time":"20191007112418","hash":"c8776049f923b951344bddf573ab56f2","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (1048, N'login', N'45.118.145.151', NULL, N'API', N'account.job-market.jp', CAST(N'2019-10-08T19:37:13.110' AS DateTime), N'User [2] logged in successfully with token [421051242d510747673430cfb95093b3]', N'{"userName":"tester@gmail.com","password":"e10adc3949ba59abbe56e057f20f883e","time":"20191008193713","hash":"9bc26cf95d2c7130db112e970c889849","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (1049, N'login', N'45.118.145.151', NULL, N'API', N'account.job-market.jp', CAST(N'2019-10-09T09:41:07.110' AS DateTime), N'User [1] logged in successfully with token [809810ff5293e2092d5aed1e8c0e470b]', N'{"userName":"bangkhmt3@gmail.com","password":"e10adc3949ba59abbe56e057f20f883e","time":"20191009094101","hash":"7d7467c309811a207b5a07e4045b90a9","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (1050, N'login', N'45.118.145.151', NULL, N'API', N'account.job-market.jp', CAST(N'2019-10-09T21:46:54.643' AS DateTime), N'User [29] logged in successfully with token [b7c4b0b695dc0c201c33205a30d99003]', N'{"userName":"tranducminhhpvn99@gmail.com","password":"87ce06dec453f68c0266ee800ce76873","time":"20191009214654","hash":"74aa8b4f44564ed98013b5c8018f1ced","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (1051, N'login', N'118.71.71.114', N'ViecLamTiengNhat/3 CFNetwork/1098.7 Darwin/19.0.0', N'API', N'account.job-market.jp', CAST(N'2019-10-10T17:30:50.630' AS DateTime), N'User [22] logged in successfully with token [873c1c9877de3843c603f496eaee81d8]', N'{"userName":"huynhtuanhuy1996@gmail.com","password":"a10c109915e36770c291dcd3880f8517","time":"20191010173112","hash":"56b1e07459d8f7351d0f413443a396d3","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (1052, N'login', N'118.71.71.114', N'ViecLamTiengNhat/3 CFNetwork/1098.7 Darwin/19.0.0', N'API', N'account.job-market.jp', CAST(N'2019-10-10T17:41:30.090' AS DateTime), N'User [1030] logged in successfully with token [46d256efcd9ab3396d275d099376dbe4]', N'{"userName":"huynhtuanhuy1996@gmail.com","email":null,"socialProvider":"google","displayName":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (1053, N'login', N'118.71.71.114', N'ViecLamTiengNhat/3 CFNetwork/1098.7 Darwin/19.0.0', N'API', N'account.job-market.jp', CAST(N'2019-10-10T17:57:11.903' AS DateTime), N'User [1031] logged in successfully with token [02252de5c0f7e190c6cda1922a22351e]', N'{"userName":"prince.of.sun.9x@gmail.com","email":"prince.of.sun.9x@gmail.com","socialProvider":"google","displayName":"anh nguyen"}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (1054, N'login', N'118.71.71.114', N'ViecLamTiengNhat/3 CFNetwork/1098.7 Darwin/19.0.0', N'API', N'account.job-market.jp', CAST(N'2019-10-10T18:28:35.077' AS DateTime), N'User [1032] logged in successfully with token [736fd660f22016df931c05cb0c1eec80]', N'{"userName":"1261994027340506","email":"huynhtuanhuy1996@gmail.com","socialProvider":"facebook","displayName":"Huỳnh Tuấn Huy"}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (1055, N'login', N'45.118.145.151', NULL, N'API', N'account.job-market.jp', CAST(N'2019-10-10T23:00:00.963' AS DateTime), N'User [19] logged in successfully with token [bd533db029767be0790c74de430f67d7]', N'{"userName":"minhnv54@gmail.com","password":"08a4984d6cec066ab5707cdb42882f1c","time":"20191010225957","hash":"a12e2487e4812849cbad2d7095accc71","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (1056, N'login', N'118.71.71.114', N'ViecLamTiengNhat/4 CFNetwork/893.14.2 Darwin/17.3.0', N'API', N'account.job-market.jp', CAST(N'2019-10-11T08:28:15.033' AS DateTime), N'User [18] logged in successfully with token [e6189f8dab8d6c419aab8f065d081bc8]', N'{"userName":"1406848862813015","email":"minhnv543@gmail.com","socialProvider":"facebook","displayName":"Nguyễn Văn Minh"}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (1057, N'login', N'118.71.71.114', N'ViecLamTiengNhat/4 CFNetwork/893.14.2 Darwin/17.3.0', N'API', N'account.job-market.jp', CAST(N'2019-10-11T08:28:54.017' AS DateTime), N'User [1033] logged in successfully with token [a43621e6f3da69c48e727294f70f7da2]', N'{"userName":"minhnv543@gmail.com","email":"minhnv543@gmail.com","socialProvider":"google","displayName":"Impossible Nothing''s"}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (2049, N'login', N'45.118.145.151', NULL, N'API', N'account.job-market.jp', CAST(N'2019-10-14T10:00:06.510' AS DateTime), N'User [1] logged in successfully with token [4c32cbedfbc15800e2198b31be9096a6]', N'{"userName":"bangkhmt3@gmail.com","password":"e10adc3949ba59abbe56e057f20f883e","time":"20191014100000","hash":"c530fc4f6993183f5516e0dd377ea477","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (2050, N'login', N'45.118.145.151', NULL, N'API', N'account.job-market.jp', CAST(N'2019-10-14T14:15:36.463' AS DateTime), N'User [1] logged in successfully with token [dd720f5ab8963282b77504c72222f5a7]', N'{"userName":"bangkhmt3@gmail.com","password":"e10adc3949ba59abbe56e057f20f883e","time":"20191014141536","hash":"ba7b213a6ccbc54a69e3a088430a9ac4","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (2051, N'login', N'45.118.145.151', NULL, N'API', N'account.job-market.jp', CAST(N'2019-10-14T14:39:04.963' AS DateTime), N'User [22] logged in successfully with token [468ae25e5a243e3f6b2b4f2e34d15f1b]', N'{"userName":"huynhtuanhuy1996@gmail.com","password":"a10c109915e36770c291dcd3880f8517","time":"20191014143904","hash":"19a08c2ead3c31c01fcfb4fd7dc45039","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (2052, N'login', N'45.118.145.151', NULL, N'API', N'account.job-market.jp', CAST(N'2019-10-14T15:45:09.117' AS DateTime), N'User [33] logged in successfully with token [fe5fb5ebec57eb98c82b65677e35841d]', N'{"userName":"12321323213","email":"123123231@gmail.com","socialProvider":"facebook","displayName":"Nguyen Van A"}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (2053, N'register', N'45.118.145.151', NULL, N'API', N'account.job-market.jp', CAST(N'2019-10-14T15:45:33.030' AS DateTime), N'User [2031] was registered successfully. Need to be done by account activation', N'{"userName":"registeronly12892@gmail.com","password":"e10adc3949ba59abbe56e057f20f883e@1","email":"registeronly12892@gmail.com","birthday":null,"sex":null,"address":null,"full_Name":null,"display_Name":"registeronly12892@gmail.com","cmtnd":null,"phone":null,"note":null,"time":"20191014154528","isEmail":true,"isPhoneNumber":false}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (2054, N'login', N'118.71.71.114', N'ViecLamTiengNhat/6 CFNetwork/1098.7 Darwin/19.0.0', N'API', N'account.job-market.jp', CAST(N'2019-10-14T16:20:30.073' AS DateTime), N'User [1030] logged in successfully with token [899859ffb3d6a415a8da30b30b29304e]', N'{"userName":"huynhtuanhuy1996@gmail.com","email":"huynhtuanhuy1996@gmail.com","socialProvider":"google","displayName":"Huy Huỳnh Tuấn"}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (2055, N'login', N'45.118.145.151', NULL, N'API', N'account.job-market.jp', CAST(N'2019-10-15T00:37:10.123' AS DateTime), N'User [1] logged in successfully with token [000585b817990ca1ec671405967ebd18]', N'{"userName":"bangkhmt3@gmail.com","password":"e10adc3949ba59abbe56e057f20f883e","time":"20191015003709","hash":"28bbd5d497fd6a26f8d39fa60b6ce98f","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (2056, N'login', N'45.118.145.151', NULL, N'API', N'account.job-market.jp', CAST(N'2019-10-16T17:50:23.680' AS DateTime), N'User [29] logged in successfully with token [8571e66661d674d589acc3ed2b78bae8]', N'{"userName":"tranducminhhpvn99@gmail.com","password":"87ce06dec453f68c0266ee800ce76873","time":"20191016175023","hash":"aad9a346ebfaec3eb333d431dd04894f","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (2057, N'login', N'45.118.145.151', NULL, N'API', N'account.job-market.jp', CAST(N'2019-10-18T09:49:54.780' AS DateTime), N'User [19] logged in successfully with token [43e3f76a97035514387a8ef55095bfa3]', N'{"userName":"minhnv54@gmail.com","password":"08a4984d6cec066ab5707cdb42882f1c","time":"20191018094954","hash":"60ebb2ac840073f16722f5f2db200a30","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (2058, N'login', N'113.190.161.158', N'ViecLamTiengNhat/7 CFNetwork/1098.7 Darwin/19.0.0', N'API', N'account.job-market.jp', CAST(N'2019-10-18T17:06:07.833' AS DateTime), N'User [22] logged in successfully with token [5583ade3ebde821449213cd4554bbd86]', N'{"userName":"huynhtuanhuy1996@gmail.com","password":"a10c109915e36770c291dcd3880f8517","time":"20191018170706","hash":"80196252d6eacbe7a8efd61fd50cae28","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (2059, N'login', N'45.118.145.151', NULL, N'API', N'account.job-market.jp', CAST(N'2019-10-20T07:28:02.233' AS DateTime), N'User [1] logged in successfully with token [c892d9815fd0424736af3fbf937f1d4b]', N'{"userName":"bangkhmt3@gmail.com","password":"e10adc3949ba59abbe56e057f20f883e","time":"20191020072801","hash":"6957b936b53feb6617c37b7e5ab9a85c","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (2060, N'login', N'45.118.145.151', NULL, N'API', N'account.job-market.jp', CAST(N'2019-10-20T20:39:14.630' AS DateTime), N'User [28] logged in successfully with token [3dbe15d66e96c961b92a8e2aeb308caf]', N'{"userName":"vuducthuong1102@gmail.com","password":"e10adc3949ba59abbe56e057f20f883e","time":"20191020203914","hash":"edc2cf4eecf335024ebb1383d4be0bc0","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (2061, N'login', N'45.118.145.151', NULL, N'API', N'account.job-market.jp', CAST(N'2019-10-22T17:12:14.943' AS DateTime), N'User [29] logged in successfully with token [6596dab41f9e22ad01eb3c57e54cdaa6]', N'{"userName":"tranducminhhpvn99@gmail.com","password":"87ce06dec453f68c0266ee800ce76873","time":"20191022171214","hash":"c7b311348437abeb9d56497a5be722ee","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (2062, N'login', N'42.117.104.237', N'ViecLamTiengNhat/8 CFNetwork/1098.7 Darwin/19.0.0', N'API', N'account.job-market.jp', CAST(N'2019-10-23T12:00:02.650' AS DateTime), N'User [22] logged in successfully with token [057ffa8bc9c9af47c14990da98832318]', N'{"userName":"huynhtuanhuy1996@gmail.com","password":"a10c109915e36770c291dcd3880f8517","time":"20191023120012","hash":"1b9cfb74392cf668f3acc11e8be0f3ce","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (2063, N'login', N'45.118.145.151', NULL, N'API', N'account.job-market.jp', CAST(N'2019-10-23T20:32:47.037' AS DateTime), N'User [1] logged in successfully with token [845b4c8b359314bfb0bd4b25f726e44e]', N'{"userName":"bangkhmt3@gmail.com","password":"e10adc3949ba59abbe56e057f20f883e","time":"20191023203246","hash":"b5375af9a5ccd17d3ca8d19582aaaa60","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (2064, N'login', N'113.190.161.158', N'ViecLamTiengNhat/8 CFNetwork/1098.7 Darwin/19.0.0', N'API', N'account.job-market.jp', CAST(N'2019-10-24T01:36:24.977' AS DateTime), N'User [1030] logged in successfully with token [fe77872c2576568346abdd43ee86f104]', N'{"userName":"huynhtuanhuy1996@gmail.com","email":"huynhtuanhuy1996@gmail.com","socialProvider":"google","displayName":"Huy Huỳnh Tuấn"}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (2065, N'register', N'45.118.145.151', NULL, N'API', N'account.job-market.jp', CAST(N'2019-10-24T01:38:29.200' AS DateTime), N'User [2032] was registered successfully. Need to be done by account activation', N'{"userName":"bpi84362@eanok.com","password":"a10c109915e36770c291dcd3880f8517","email":"bpi84362@eanok.com","birthday":null,"sex":null,"address":null,"full_Name":null,"display_Name":"bpi84362@eanok.com","cmtnd":null,"phone":null,"note":null,"time":"20191024013824","isEmail":true,"isPhoneNumber":false}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (2066, N'register', N'45.118.145.151', NULL, N'API', N'account.job-market.jp', CAST(N'2019-10-24T01:40:07.493' AS DateTime), N'User [2033] was registered successfully. Need to be done by account activation', N'{"userName":"agdahtahp@mailnesia.com","password":"e10adc3949ba59abbe56e057f20f883e","email":"agdahtahp@mailnesia.com","birthday":null,"sex":null,"address":null,"full_Name":null,"display_Name":"agdahtahp@mailnesia.com","cmtnd":null,"phone":null,"note":null,"time":"20191024014003","isEmail":true,"isPhoneNumber":false}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (2067, N'login', N'45.118.145.151', NULL, N'API', N'account.job-market.jp', CAST(N'2019-10-24T01:40:49.923' AS DateTime), N'User [2033] logged in successfully with token [d8d35bde86ceaded57b9ae5fd9668e05]', N'{"userName":"agdahtahp@mailnesia.com","password":"e10adc3949ba59abbe56e057f20f883e","time":"20191024014049","hash":"0913266ef0bd28606cbc0e7bdd7645c8","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (2068, N'login', N'113.190.161.158', N'ViecLamTiengNhat/8 CFNetwork/1098.7 Darwin/19.0.0', N'API', N'account.job-market.jp', CAST(N'2019-10-24T02:02:46.980' AS DateTime), N'User [22] logged in successfully with token [e2612b6956192ce11d1198776300418e]', N'{"userName":"huynhtuanhuy1996@gmail.com","password":"a10c109915e36770c291dcd3880f8517","time":"20191024020300","hash":"7f9bdaa7a43a5a59a1c494c23bf1152f","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (2069, N'login', N'45.118.145.151', NULL, N'API', N'account.job-market.jp', CAST(N'2019-10-24T12:33:50.183' AS DateTime), N'User [1] logged in successfully with token [8b8a496a376fe810e2b41c797fcd7705]', N'{"userName":"bangkhmt3@gmail.com","password":"e10adc3949ba59abbe56e057f20f883e","time":"20191024123349","hash":"512ab9637f75100c4ed226049ab6c579","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (2070, N'login', N'45.118.145.151', NULL, N'API', N'account.job-market.jp', CAST(N'2019-10-24T19:48:15.500' AS DateTime), N'User [1] logged in successfully with token [2935bd458a2864f7aeed6114efc7c114]', N'{"userName":"bangkhmt3@gmail.com","password":"e10adc3949ba59abbe56e057f20f883e","time":"20191024194815","hash":"a21524e2b951106f50277013fa766243","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (2071, N'login', N'45.118.145.151', NULL, N'API', N'account.job-market.jp', CAST(N'2019-10-26T12:03:32.430' AS DateTime), N'User [1] logged in successfully with token [fc57010814af1de02802448910c37097]', N'{"userName":"bangkhmt3@gmail.com","password":"e10adc3949ba59abbe56e057f20f883e","time":"20191026120332","hash":"ba2937a8b3abea56d7a3e3d8dea4bad3","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (2072, N'login', N'45.118.145.151', NULL, N'API', N'account.job-market.jp', CAST(N'2019-10-26T12:47:26.640' AS DateTime), N'User [19] logged in successfully with token [c0d02d9291e544721d5db6f6ad55b011]', N'{"userName":"minhnv54@gmail.com","password":"08a4984d6cec066ab5707cdb42882f1c","time":"20191026124724","hash":"f8335a6e4e550f5e38db110b1ca258cc","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (2073, N'login', N'113.190.161.158', N'ViecLamTiengNhat/9 CFNetwork/1107.1 Darwin/19.0.0', N'API', N'account.job-market.jp', CAST(N'2019-10-30T23:39:05.110' AS DateTime), N'User [22] logged in successfully with token [5c4bf70b2a33baab0daae645bea83d5c]', N'{"userName":"huynhtuanhuy1996@gmail.com","password":"a10c109915e36770c291dcd3880f8517","time":"20191030233952","hash":"083ef3d901bc361f4adbb0d82b7cd913","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (2074, N'login', N'45.118.145.151', NULL, N'API', N'account.job-market.jp', CAST(N'2019-11-01T12:02:08.120' AS DateTime), N'User [2] logged in successfully with token [aa652c6c28d02b5cb54994a418e39479]', N'{"userName":"tester@gmail.com","password":"e10adc3949ba59abbe56e057f20f883e","time":"20191101120208","hash":"109178249a4ebc4f474af15d3ce10ae1","socialProvider":null}')
GO
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (2075, N'login', N'45.118.145.151', NULL, N'API', N'account.job-market.jp', CAST(N'2019-11-02T20:43:05.113' AS DateTime), N'User [1] logged in successfully with token [a6a14cd763e13bd30bd7d2880ee7a60e]', N'{"userName":"bangkhmt3@gmail.com","password":"e10adc3949ba59abbe56e057f20f883e","time":"20191102204303","hash":"2fadb0fa29de3f3a57cdf2e1c337df67","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (2076, N'login', N'126.255.126.3', N'ViecLamTiengNhat/2 CFNetwork/893.14.2 Darwin/17.3.0', N'API', N'account.job-market.jp', CAST(N'2019-11-02T22:52:24.030' AS DateTime), N'User [19] logged in successfully with token [9e142e7515b11cd1793b26653c81afc3]', N'{"userName":"minhnv54@gmail.com","password":"08a4984d6cec066ab5707cdb42882f1c","time":"20191103005223","hash":"9b2938ce3128231cfb18b722ea752396","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (2077, N'login', N'113.190.161.158', N'ViecLamTiengNhat/2 CFNetwork/1107.1 Darwin/19.0.0', N'API', N'account.job-market.jp', CAST(N'2019-11-03T01:52:48.363' AS DateTime), N'User [22] logged in successfully with token [352d565f6e47a18db3698856635e3ba7]', N'{"userName":"huynhtuanhuy1996@gmail.com","password":"a10c109915e36770c291dcd3880f8517","time":"20191103015349","hash":"6cafbc1c23e75052e1937bbc808f236b","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (2078, N'login', N'45.118.145.151', NULL, N'API', N'account.job-market.jp', CAST(N'2019-11-08T10:01:37.277' AS DateTime), N'User [1] logged in successfully with token [a52f599a743066810d5ca01a4e73ae10]', N'{"userName":"bangkhmt3@gmail.com","password":"e10adc3949ba59abbe56e057f20f883e","time":"20191108100136","hash":"15f1c89474a28857be7986a50ebd584b","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (2079, N'login', N'45.118.145.151', NULL, N'API', N'account.job-market.jp', CAST(N'2019-11-10T11:47:38.057' AS DateTime), N'User [19] logged in successfully with token [c24679f945b23750562a106f118d54eb]', N'{"userName":"minhnv54@gmail.com","password":"08a4984d6cec066ab5707cdb42882f1c","time":"20191110114737","hash":"0a5b6a05cc064eea1134e69aa791ba7d","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (2080, N'login', N'113.190.252.26', N'ViecLamTiengNhat/2 CFNetwork/978.0.7 Darwin/18.7.0', N'API', N'account.job-market.jp', CAST(N'2019-11-11T13:50:49.313' AS DateTime), N'User [22] logged in successfully with token [dbaea25ebf103cc7f5d5f220b92375d8]', N'{"userName":"huynhtuanhuy1996@gmail.com","password":"a10c109915e36770c291dcd3880f8517","time":"20191111135049","hash":"b449a4e3bfffd13cf2835d8d654cdb32","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (2081, N'register', N'114.152.18.211', N'ViecLamTiengNhat/2 CFNetwork/1120 Darwin/19.0.0', N'API', N'account.job-market.jp', CAST(N'2019-11-12T13:06:56.710' AS DateTime), N'User [2034] was registered successfully. Need to be done by account activation', N'{"userName":"ynaka5678@gmail.com","password":"cba8891e98dbe6a4c11e00ca189548fb","email":null,"birthday":null,"sex":null,"address":null,"full_Name":null,"display_Name":null,"cmtnd":null,"phone":null,"note":null,"time":null,"isEmail":false,"isPhoneNumber":false}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (2082, N'login', N'114.152.18.211', N'ViecLamTiengNhat/2 CFNetwork/1120 Darwin/19.0.0', N'API', N'account.job-market.jp', CAST(N'2019-11-12T13:09:08.840' AS DateTime), N'User [2034] logged in successfully with token [4d72ce75e7586aa19630e97f46132819]', N'{"userName":"ynaka5678@gmail.com","password":"cba8891e98dbe6a4c11e00ca189548fb","time":"20191112150908","hash":"3a0d51a6aa37bd5e379c5a4b8f213c4f","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (2083, N'login', N'45.118.145.151', NULL, N'API', N'account.job-market.jp', CAST(N'2019-11-13T14:43:08.203' AS DateTime), N'User [1] logged in successfully with token [889d913e8952620ba3a950f0cddb0b35]', N'{"userName":"bangkhmt3@gmail.com","password":"e10adc3949ba59abbe56e057f20f883e","time":"20191113144307","hash":"182e59aa24b579ac6698d159a5a6df7b","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (2084, N'login', N'113.20.108.26', N'ViecLamTiengNhat/3 CFNetwork/1107.1 Darwin/19.0.0', N'API', N'account.job-market.jp', CAST(N'2019-11-13T15:11:40.500' AS DateTime), N'User [22] logged in successfully with token [3466d43d91df582461e914a058d75f49]', N'{"userName":"huynhtuanhuy1996@gmail.com","password":"a10c109915e36770c291dcd3880f8517","time":"20191113151332","hash":"a56d81888a0e892946b8b6f378f40611","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (2085, N'login', N'126.255.100.139', N'ViecLamTiengNhat/2 CFNetwork/893.14.2 Darwin/17.3.0', N'API', N'account.job-market.jp', CAST(N'2019-11-13T17:00:46.497' AS DateTime), N'User [19] logged in successfully with token [8aa312974c1494701e74c8832cbc70f5]', N'{"userName":"minhnv54@gmail.com","password":"08a4984d6cec066ab5707cdb42882f1c","time":"20191113190046","hash":"6c9f60bf36fa5097153ee63ba2948a1d","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (2086, N'login', N'45.118.145.151', NULL, N'API', N'account.job-market.jp', CAST(N'2019-11-13T21:08:43.320' AS DateTime), N'User [28] logged in successfully with token [787c4e4513c475c87f99cbe147db9339]', N'{"userName":"vuducthuong1102@gmail.com","password":"e10adc3949ba59abbe56e057f20f883e","time":"20191113210843","hash":"c2c5163d26b3bbb2c211fc8fe8552eff","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (2087, N'login', N'45.118.145.151', NULL, N'API', N'account.job-market.jp', CAST(N'2019-11-13T21:18:44.577' AS DateTime), N'User [2031] logged in successfully with token [4f34fdfae88b69b6e481356cc8435096]', N'{"userName":"registeronly12892@gmail.com","password":"e10adc3949ba59abbe56e057f20f883e","time":"20191113211844","hash":"926b3bf18855bba99d48026999ed1734","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (2088, N'login', N'45.118.145.151', NULL, N'API', N'account.job-market.jp', CAST(N'2019-11-15T19:38:55.910' AS DateTime), N'User [2] logged in successfully with token [dd10addbad5718718e5e983a7ea8421e]', N'{"userName":"tester@gmail.com","password":"e10adc3949ba59abbe56e057f20f883e","time":"20191115193855","hash":"08b34a838337221cf433e87898f7fb92","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (2089, N'register', N'14.162.235.188', N'Job%20Market/1 CFNetwork/1107.1 Darwin/19.0.0', N'API', N'account.job-market.jp', CAST(N'2019-11-16T04:31:20.930' AS DateTime), N'User [2035] was registered successfully. Need to be done by account activation', N'{"userName":"prince.of.sun.96@gmail.com","password":"a10c109915e36770c291dcd3880f8517","email":null,"birthday":null,"sex":null,"address":null,"full_Name":null,"display_Name":null,"cmtnd":null,"phone":null,"note":null,"time":null,"isEmail":false,"isPhoneNumber":false}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (2090, N'register', N'14.162.235.188', N'Job%20Market/1 CFNetwork/1107.1 Darwin/19.0.0', N'API', N'account.job-market.jp', CAST(N'2019-11-16T04:34:02.480' AS DateTime), N'User [2036] was registered successfully. Need to be done by account activation', N'{"userName":"huyht@mindx.edu.vn","password":"e10adc3949ba59abbe56e057f20f883e","email":null,"birthday":null,"sex":null,"address":null,"full_Name":null,"display_Name":null,"cmtnd":null,"phone":null,"note":null,"time":null,"isEmail":false,"isPhoneNumber":false}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (2091, N'login', N'14.162.235.188', N'Job%20Market/1 CFNetwork/1107.1 Darwin/19.0.0', N'API', N'account.job-market.jp', CAST(N'2019-11-16T04:38:54.900' AS DateTime), N'User [2036] logged in successfully with token [c3370ac1c7e2a353834f729af441fcb5]', N'{"userName":"huyht@mindx.edu.vn","password":"e10adc3949ba59abbe56e057f20f883e","time":"20191116044100","hash":"6d60b4b7e9c97d5a34a169388e8c9f63","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (2092, N'register', N'45.118.145.151', NULL, N'API', N'account.job-market.jp', CAST(N'2019-11-16T06:49:14.300' AS DateTime), N'User [2037] was registered successfully. Need to be done by account activation', N'{"userName":"vuluongbang_nd1991@yahoo.com","password":"e10adc3949ba59abbe56e057f20f883e","email":"vuluongbang_nd1991@yahoo.com","birthday":null,"sex":null,"address":null,"full_Name":null,"display_Name":"vuluongbang_nd1991@yahoo.com","cmtnd":null,"phone":null,"note":null,"time":"20191116064909","isEmail":true,"isPhoneNumber":false}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (2093, N'login', N'45.118.145.151', NULL, N'API', N'account.job-market.jp', CAST(N'2019-11-18T14:56:46.483' AS DateTime), N'User [7] logged in successfully with token [a4884a5e5bc2206e183ec3e501b19a08]', N'{"userName":"registeronly12891@gmail.com","password":"e10adc3949ba59abbe56e057f20f883e","time":"20191118145646","hash":"338ca5dc71d38f84bd0f4b7d59f79103","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (2094, N'login', N'45.118.145.151', NULL, N'API', N'account.job-market.jp', CAST(N'2019-11-20T17:41:08.403' AS DateTime), N'User [22] logged in successfully with token [b0c313590ed4f8dec0ead66ca97aaac9]', N'{"userName":"huynhtuanhuy1996@gmail.com","password":"a10c109915e36770c291dcd3880f8517","time":"20191120174108","hash":"e66641465b438068db0621ef77cd75de","socialProvider":null}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (2095, N'register', N'45.118.145.151', NULL, N'API', N'account.job-market.jp', CAST(N'2019-11-22T02:39:25.627' AS DateTime), N'User [2038] was registered successfully. Need to be done by account activation', N'{"userName":"11141768@st.neu.edu.vn","password":"a10c109915e36770c291dcd3880f8517","email":"11141768@st.neu.edu.vn","birthday":null,"sex":null,"address":null,"full_Name":null,"display_Name":"11141768@st.neu.edu.vn","cmtnd":null,"phone":null,"note":null,"time":"20191122023921","isEmail":true,"isPhoneNumber":false}')
INSERT [dbo].[tbl_traces] ([Id], [ActionType], [UserIp], [UserAgent], [Method], [Domain], [CreatedDate], [ActionDesc], [RawData]) VALUES (2096, N'login', N'45.118.145.151', NULL, N'API', N'account.job-market.jp', CAST(N'2019-11-22T02:48:37.367' AS DateTime), N'User [2038] logged in successfully with token [2391a1559beeca0419c7de1c0b811bc7]', N'{"userName":"11141768@st.neu.edu.vn","password":"a10c109915e36770c291dcd3880f8517","time":"20191122024837","hash":"2ae04bfa2fdb486b5485110a374d01cd","socialProvider":null}')
SET IDENTITY_INSERT [dbo].[tbl_traces] OFF
INSERT [dbo].[tbl_user_actions] ([UserId], [UserActionId], [ActionType], [Status], [CreatedDate], [ModifiedDate]) VALUES (2, 3, N'FOLLOW', 1, CAST(N'2018-05-10T16:45:35.100' AS DateTime), CAST(N'2018-05-10T16:45:35.100' AS DateTime))
INSERT [dbo].[tbl_user_actions] ([UserId], [UserActionId], [ActionType], [Status], [CreatedDate], [ModifiedDate]) VALUES (2, 6, N'FOLLOW', 1, CAST(N'2018-05-11T10:24:27.430' AS DateTime), CAST(N'2018-05-11T10:24:27.430' AS DateTime))
INSERT [dbo].[tbl_user_actions] ([UserId], [UserActionId], [ActionType], [Status], [CreatedDate], [ModifiedDate]) VALUES (5, 6, N'FOLLOW', 1, CAST(N'2018-05-11T10:24:28.230' AS DateTime), CAST(N'2018-05-11T10:24:28.230' AS DateTime))
INSERT [dbo].[tbl_user_actions] ([UserId], [UserActionId], [ActionType], [Status], [CreatedDate], [ModifiedDate]) VALUES (3, 6, N'FOLLOW', 1, CAST(N'2018-05-11T10:24:28.453' AS DateTime), CAST(N'2018-05-11T10:24:28.453' AS DateTime))
INSERT [dbo].[tbl_user_actions] ([UserId], [UserActionId], [ActionType], [Status], [CreatedDate], [ModifiedDate]) VALUES (1, 6, N'FOLLOW', 1, CAST(N'2018-05-11T10:24:28.620' AS DateTime), CAST(N'2018-05-11T10:24:28.620' AS DateTime))
INSERT [dbo].[tbl_user_actions] ([UserId], [UserActionId], [ActionType], [Status], [CreatedDate], [ModifiedDate]) VALUES (7, 6, N'FOLLOW', 1, CAST(N'2018-05-11T10:24:28.823' AS DateTime), CAST(N'2018-05-11T10:24:28.823' AS DateTime))
INSERT [dbo].[tbl_user_actions] ([UserId], [UserActionId], [ActionType], [Status], [CreatedDate], [ModifiedDate]) VALUES (2, 1, N'FOLLOW', 1, CAST(N'2018-05-04T11:35:59.390' AS DateTime), CAST(N'2018-05-04T11:35:59.390' AS DateTime))
INSERT [dbo].[tbl_user_actions] ([UserId], [UserActionId], [ActionType], [Status], [CreatedDate], [ModifiedDate]) VALUES (1, 2, N'FOLLOW', 1, CAST(N'2018-05-04T16:40:47.683' AS DateTime), CAST(N'2018-05-04T16:40:47.683' AS DateTime))
INSERT [dbo].[tbl_user_codes] ([Id], [UserId], [Code], [CodeType], [CreatedDate], [ExpiredDate], [IsUsed], [UsedDate], [Action]) VALUES (N'ID20180710161235_843d7', 1001012, N'67871', N'OTPSMS', CAST(N'2018-07-10T16:12:35.847' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[tbl_user_codes_history] ([Id], [UserId], [Code], [CodeType], [CreatedDate], [ExpiredDate], [IsUsed], [UsedDate], [Action]) VALUES (N'ID20180703095147_48cb9', 1001012, N'60383', N'OTPSMS', CAST(N'2018-07-03T09:51:47.690' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[tbl_user_codes_history] ([Id], [UserId], [Code], [CodeType], [CreatedDate], [ExpiredDate], [IsUsed], [UsedDate], [Action]) VALUES (N'ID20180704092420_896b6', 2, N'90888', N'OTPSMS', CAST(N'2018-07-04T09:24:20.390' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[tbl_user_codes_history] ([Id], [UserId], [Code], [CodeType], [CreatedDate], [ExpiredDate], [IsUsed], [UsedDate], [Action]) VALUES (N'ID20180704092859_dfd29', 2, N'97266', N'OTPSMS', CAST(N'2018-07-04T09:28:59.387' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[tbl_user_codes_history] ([Id], [UserId], [Code], [CodeType], [CreatedDate], [ExpiredDate], [IsUsed], [UsedDate], [Action]) VALUES (N'ID20180704093049_04c7e', 2, N'99379', N'OTPSMS', CAST(N'2018-07-04T09:30:49.313' AS DateTime), NULL, 1, CAST(N'2018-07-04T09:50:08.377' AS DateTime), N'recover_password1')
INSERT [dbo].[tbl_user_codes_history] ([Id], [UserId], [Code], [CodeType], [CreatedDate], [ExpiredDate], [IsUsed], [UsedDate], [Action]) VALUES (N'ID20180704095107_cfceb', 2, N'19607', N'OTPSMS', CAST(N'2018-07-04T09:51:07.443' AS DateTime), NULL, 1, CAST(N'2018-07-04T09:51:53.673' AS DateTime), N'recover_password1')
INSERT [dbo].[tbl_user_codes_history] ([Id], [UserId], [Code], [CodeType], [CreatedDate], [ExpiredDate], [IsUsed], [UsedDate], [Action]) VALUES (N'ID20180704095631_b209c', 2, N'60030', N'OTPSMS', CAST(N'2018-07-04T09:56:31.997' AS DateTime), NULL, 1, CAST(N'2018-07-04T09:56:54.957' AS DateTime), N'recover_password1')
INSERT [dbo].[tbl_user_codes_history] ([Id], [UserId], [Code], [CodeType], [CreatedDate], [ExpiredDate], [IsUsed], [UsedDate], [Action]) VALUES (N'ID20180704101835_a9e6b', 1002006, N'14487', N'OTPSMS', CAST(N'2018-07-04T10:18:35.813' AS DateTime), NULL, 1, CAST(N'2018-07-04T10:20:24.177' AS DateTime), N'active_account')
INSERT [dbo].[tbl_user_codes_history] ([Id], [UserId], [Code], [CodeType], [CreatedDate], [ExpiredDate], [IsUsed], [UsedDate], [Action]) VALUES (N'ID20180710160811_0182d', 1001012, N'60383', N'OTPSMS', CAST(N'2018-07-10T16:08:11.053' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[tbl_user_codes_history] ([Id], [UserId], [Code], [CodeType], [CreatedDate], [ExpiredDate], [IsUsed], [UsedDate], [Action]) VALUES (N'ID20180710160949_210e6', 1001012, N'95362', N'OTPSMS', CAST(N'2018-07-10T16:09:49.707' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[tbl_user_codes_history] ([Id], [UserId], [Code], [CodeType], [CreatedDate], [ExpiredDate], [IsUsed], [UsedDate], [Action]) VALUES (N'ID20180710161056_218f6', 1001012, N'44662', N'OTPSMS', CAST(N'2018-07-10T16:10:56.923' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[tbl_user_codes_history] ([Id], [UserId], [Code], [CodeType], [CreatedDate], [ExpiredDate], [IsUsed], [UsedDate], [Action]) VALUES (N'ID20180821154637_ad547', 2, N'15226', N'OTPSMS', CAST(N'2018-08-21T15:46:37.243' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[tbl_user_codes_history] ([Id], [UserId], [Code], [CodeType], [CreatedDate], [ExpiredDate], [IsUsed], [UsedDate], [Action]) VALUES (N'ID20180821154804_37cf0', 2, N'56321', N'OTPSMS', CAST(N'2018-08-21T15:48:04.260' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[tbl_user_codes_history] ([Id], [UserId], [Code], [CodeType], [CreatedDate], [ExpiredDate], [IsUsed], [UsedDate], [Action]) VALUES (N'ID20180821155048_61517', 2, N'44151', N'OTPSMS', CAST(N'2018-08-21T15:50:48.663' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[tbl_user_codes_history] ([Id], [UserId], [Code], [CodeType], [CreatedDate], [ExpiredDate], [IsUsed], [UsedDate], [Action]) VALUES (N'ID20180821155225_015d1', 2, N'79968', N'OTPSMS', CAST(N'2018-08-21T15:52:25.017' AS DateTime), NULL, 1, CAST(N'2018-08-21T15:52:42.190' AS DateTime), N'active_account')
INSERT [dbo].[tbl_user_codes_history] ([Id], [UserId], [Code], [CodeType], [CreatedDate], [ExpiredDate], [IsUsed], [UsedDate], [Action]) VALUES (N'ID20180821155547_0acb6', 2, N'71194', N'OTPSMS', CAST(N'2018-08-21T15:55:47.387' AS DateTime), NULL, 1, CAST(N'2018-08-21T15:55:58.510' AS DateTime), N'active_account')
INSERT [dbo].[tbl_user_codes_history] ([Id], [UserId], [Code], [CodeType], [CreatedDate], [ExpiredDate], [IsUsed], [UsedDate], [Action]) VALUES (N'ID20180821155835_cce82', 2, N'24140', N'OTPSMS', CAST(N'2018-08-21T15:58:35.383' AS DateTime), NULL, 1, CAST(N'2018-08-21T15:59:15.600' AS DateTime), N'recover_password1')
INSERT [dbo].[tbl_user_codes_history] ([Id], [UserId], [Code], [CodeType], [CreatedDate], [ExpiredDate], [IsUsed], [UsedDate], [Action]) VALUES (N'ID20180821160105_69eb5', 2, N'50432', N'OTPSMS', CAST(N'2018-08-21T16:01:05.277' AS DateTime), NULL, 1, CAST(N'2018-08-21T16:01:17.303' AS DateTime), N'recover_password1')
INSERT [dbo].[tbl_user_codes_history] ([Id], [UserId], [Code], [CodeType], [CreatedDate], [ExpiredDate], [IsUsed], [UsedDate], [Action]) VALUES (N'ID20180821160152_56b8d', 2, N'63506', N'OTPSMS', CAST(N'2018-08-21T16:01:52.630' AS DateTime), NULL, 1, CAST(N'2018-08-21T16:02:11.003' AS DateTime), N'recover_password1')
INSERT [dbo].[tbl_user_codes_history] ([Id], [UserId], [Code], [CodeType], [CreatedDate], [ExpiredDate], [IsUsed], [UsedDate], [Action]) VALUES (N'ID20180821160320_62a20', 2, N'74158', N'OTPSMS', CAST(N'2018-08-21T16:03:20.233' AS DateTime), NULL, 1, CAST(N'2018-08-21T16:03:35.960' AS DateTime), N'recover_password1')
INSERT [dbo].[tbl_user_codes_history] ([Id], [UserId], [Code], [CodeType], [CreatedDate], [ExpiredDate], [IsUsed], [UsedDate], [Action]) VALUES (N'ID20180823153706_a4d20', 1, N'66752', N'OTPSMS', CAST(N'2018-08-23T15:37:06.510' AS DateTime), NULL, 1, CAST(N'2018-08-23T15:37:20.837' AS DateTime), N'recover_password1')
INSERT [dbo].[tbl_user_data] ([UserId], [FollowingCount], [MessageCount], [LikePostCount], [PhotoCount], [FollowerCount], [PostCount]) VALUES (1, 0, 0, 0, 0, 1, 0)
INSERT [dbo].[tbl_user_data] ([UserId], [FollowingCount], [MessageCount], [LikePostCount], [PhotoCount], [FollowerCount], [PostCount]) VALUES (2, 0, 0, 0, 26, 2, 10)
INSERT [dbo].[tbl_user_data] ([UserId], [FollowingCount], [MessageCount], [LikePostCount], [PhotoCount], [FollowerCount], [PostCount]) VALUES (3, 1, 0, 0, 29, 1, -12)
INSERT [dbo].[tbl_user_data] ([UserId], [FollowingCount], [MessageCount], [LikePostCount], [PhotoCount], [FollowerCount], [PostCount]) VALUES (5, 0, 0, 2, 0, 1, 0)
INSERT [dbo].[tbl_user_data] ([UserId], [FollowingCount], [MessageCount], [LikePostCount], [PhotoCount], [FollowerCount], [PostCount]) VALUES (6, 5, 0, 1, 3, 0, 3)
INSERT [dbo].[tbl_user_data] ([UserId], [FollowingCount], [MessageCount], [LikePostCount], [PhotoCount], [FollowerCount], [PostCount]) VALUES (7, 0, 0, 0, 0, 1, 0)
SET IDENTITY_INSERT [dbo].[tbl_user_devices] ON 

INSERT [dbo].[tbl_user_devices] ([Id], [UserId], [DeviceName], [DeviceID], [RegistrationID], [iosDevice], [CreatedDate], [Status], [LastConnected], [LangCode]) VALUES (5, 1, N'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/69.0.3497.100 Safari/537.36', N'1_dvIdNew', N'1_xxxyyy', 1, CAST(N'2018-10-17T10:52:44.007' AS DateTime), 1, CAST(N'2018-10-17T10:52:44.007' AS DateTime), N'en-US')
INSERT [dbo].[tbl_user_devices] ([Id], [UserId], [DeviceName], [DeviceID], [RegistrationID], [iosDevice], [CreatedDate], [Status], [LastConnected], [LangCode]) VALUES (6, 1003084, N'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/69.0.3497.100 Safari/537.36', N'1003084_dvIdNew', N'1003084_xxxyyy', 1, CAST(N'2018-10-17T10:53:37.220' AS DateTime), 1, CAST(N'2018-10-17T10:53:37.220' AS DateTime), N'vi-VN')
SET IDENTITY_INSERT [dbo].[tbl_user_devices] OFF
INSERT [dbo].[tbl_user_otpactions] ([UserId], [CodeId], [TargetData], [IsDone], [CreatedDate], [Action], [ImplementTime]) VALUES (1001012, N'ID20180710161235_843d7', N'', 0, CAST(N'2018-07-10T16:12:35.857' AS DateTime), N'active_account', NULL)
INSERT [dbo].[tbl_user_otpactions_history] ([UserId], [CodeId], [TargetData], [IsDone], [CreatedDate], [Action], [ImplementTime]) VALUES (1, N'ID20180823153706_a4d20', N'e10adc3949ba59abbe56e057f20f883e', 1, CAST(N'2018-08-23T15:37:06.523' AS DateTime), N'recover_password1', CAST(N'2018-08-23T15:37:20.847' AS DateTime))
INSERT [dbo].[tbl_user_otpactions_history] ([UserId], [CodeId], [TargetData], [IsDone], [CreatedDate], [Action], [ImplementTime]) VALUES (2, N'ID20180704092420_896b6', N'123456', 0, CAST(N'2018-07-04T09:24:20.433' AS DateTime), N'recover_password1', NULL)
INSERT [dbo].[tbl_user_otpactions_history] ([UserId], [CodeId], [TargetData], [IsDone], [CreatedDate], [Action], [ImplementTime]) VALUES (2, N'ID20180704092859_dfd29', N'e10adc3949ba59abbe56e057f20f883e', 0, CAST(N'2018-07-04T09:28:59.400' AS DateTime), N'recover_password1', NULL)
INSERT [dbo].[tbl_user_otpactions_history] ([UserId], [CodeId], [TargetData], [IsDone], [CreatedDate], [Action], [ImplementTime]) VALUES (2, N'ID20180704093049_04c7e', N'e10adc3949ba59abbe56e057f20f883e', 1, CAST(N'2018-07-04T09:30:49.330' AS DateTime), N'recover_password1', CAST(N'2018-07-04T09:50:08.383' AS DateTime))
INSERT [dbo].[tbl_user_otpactions_history] ([UserId], [CodeId], [TargetData], [IsDone], [CreatedDate], [Action], [ImplementTime]) VALUES (2, N'ID20180704095107_cfceb', N'0b3bc9ce555f07d127c6da44337e364f', 1, CAST(N'2018-07-04T09:51:07.453' AS DateTime), N'recover_password1', CAST(N'2018-07-04T09:51:53.680' AS DateTime))
INSERT [dbo].[tbl_user_otpactions_history] ([UserId], [CodeId], [TargetData], [IsDone], [CreatedDate], [Action], [ImplementTime]) VALUES (2, N'ID20180704095631_b209c', N'e10adc3949ba59abbe56e057f20f883e', 1, CAST(N'2018-07-04T09:56:32.013' AS DateTime), N'recover_password1', CAST(N'2018-07-04T09:56:54.963' AS DateTime))
INSERT [dbo].[tbl_user_otpactions_history] ([UserId], [CodeId], [TargetData], [IsDone], [CreatedDate], [Action], [ImplementTime]) VALUES (2, N'ID20180821154637_ad547', N'123456', 0, CAST(N'2018-08-21T15:46:37.263' AS DateTime), N'recover_password1', NULL)
INSERT [dbo].[tbl_user_otpactions_history] ([UserId], [CodeId], [TargetData], [IsDone], [CreatedDate], [Action], [ImplementTime]) VALUES (2, N'ID20180821154804_37cf0', N'123456', 0, CAST(N'2018-08-21T15:48:04.273' AS DateTime), N'recover_password1', NULL)
INSERT [dbo].[tbl_user_otpactions_history] ([UserId], [CodeId], [TargetData], [IsDone], [CreatedDate], [Action], [ImplementTime]) VALUES (2, N'ID20180821155048_61517', N'123456', 0, CAST(N'2018-08-21T15:50:48.670' AS DateTime), N'recover_password1', NULL)
INSERT [dbo].[tbl_user_otpactions_history] ([UserId], [CodeId], [TargetData], [IsDone], [CreatedDate], [Action], [ImplementTime]) VALUES (2, N'ID20180821155225_015d1', N'123456', 1, CAST(N'2018-08-21T15:52:25.023' AS DateTime), N'recover_password1', CAST(N'2018-08-21T15:52:42.197' AS DateTime))
INSERT [dbo].[tbl_user_otpactions_history] ([UserId], [CodeId], [TargetData], [IsDone], [CreatedDate], [Action], [ImplementTime]) VALUES (2, N'ID20180821155547_0acb6', N'123456', 1, CAST(N'2018-08-21T15:55:47.397' AS DateTime), N'recover_password1', CAST(N'2018-08-21T15:55:58.517' AS DateTime))
INSERT [dbo].[tbl_user_otpactions_history] ([UserId], [CodeId], [TargetData], [IsDone], [CreatedDate], [Action], [ImplementTime]) VALUES (2, N'ID20180821155835_cce82', N'123456', 1, CAST(N'2018-08-21T15:58:35.393' AS DateTime), N'recover_password1', CAST(N'2018-08-21T15:59:15.610' AS DateTime))
INSERT [dbo].[tbl_user_otpactions_history] ([UserId], [CodeId], [TargetData], [IsDone], [CreatedDate], [Action], [ImplementTime]) VALUES (2, N'ID20180821160105_69eb5', N'123456', 1, CAST(N'2018-08-21T16:01:05.287' AS DateTime), N'recover_password1', CAST(N'2018-08-21T16:01:17.310' AS DateTime))
INSERT [dbo].[tbl_user_otpactions_history] ([UserId], [CodeId], [TargetData], [IsDone], [CreatedDate], [Action], [ImplementTime]) VALUES (2, N'ID20180821160152_56b8d', N'123456', 1, CAST(N'2018-08-21T16:01:52.640' AS DateTime), N'recover_password1', CAST(N'2018-08-21T16:02:11.003' AS DateTime))
INSERT [dbo].[tbl_user_otpactions_history] ([UserId], [CodeId], [TargetData], [IsDone], [CreatedDate], [Action], [ImplementTime]) VALUES (2, N'ID20180821160320_62a20', N'123456', 1, CAST(N'2018-08-21T16:03:20.240' AS DateTime), N'recover_password1', CAST(N'2018-08-21T16:03:35.967' AS DateTime))
INSERT [dbo].[tbl_user_otpactions_history] ([UserId], [CodeId], [TargetData], [IsDone], [CreatedDate], [Action], [ImplementTime]) VALUES (1001012, N'ID20180703095147_48cb9', N'', 0, CAST(N'2018-07-03T09:51:47.717' AS DateTime), N'active_account', NULL)
INSERT [dbo].[tbl_user_otpactions_history] ([UserId], [CodeId], [TargetData], [IsDone], [CreatedDate], [Action], [ImplementTime]) VALUES (1001012, N'ID20180710160811_0182d', N'', 0, CAST(N'2018-07-10T16:08:11.080' AS DateTime), N'active_account', NULL)
INSERT [dbo].[tbl_user_otpactions_history] ([UserId], [CodeId], [TargetData], [IsDone], [CreatedDate], [Action], [ImplementTime]) VALUES (1001012, N'ID20180710160949_210e6', N'', 0, CAST(N'2018-07-10T16:09:49.717' AS DateTime), N'active_account', NULL)
INSERT [dbo].[tbl_user_otpactions_history] ([UserId], [CodeId], [TargetData], [IsDone], [CreatedDate], [Action], [ImplementTime]) VALUES (1001012, N'ID20180710161056_218f6', N'', 0, CAST(N'2018-07-10T16:10:56.933' AS DateTime), N'active_account', NULL)
INSERT [dbo].[tbl_user_otpactions_history] ([UserId], [CodeId], [TargetData], [IsDone], [CreatedDate], [Action], [ImplementTime]) VALUES (1002006, N'ID20180704101835_a9e6b', N'', 1, CAST(N'2018-07-04T10:18:35.827' AS DateTime), N'active_account', CAST(N'2018-07-04T10:20:24.183' AS DateTime))
INSERT [dbo].[tbl_user_tokenkeys] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (1, N'01f2bedfd0ee009cdbf2c0b10c1281f8', CAST(N'2018-10-17T10:47:50.213' AS DateTime), CAST(N'2018-10-17T12:47:50.213' AS DateTime), N'API', N'192.168.2.48')
INSERT [dbo].[tbl_user_tokenkeys] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (1, N'889d913e8952620ba3a950f0cddb0b35', CAST(N'2019-11-13T14:43:08.187' AS DateTime), CAST(N'2019-11-13T16:43:08.187' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (1, N'c09a99b7b5ead5a2acd2f13b5323f353', CAST(N'2018-07-09T10:56:03.613' AS DateTime), CAST(N'2018-07-10T03:36:03.613' AS DateTime), N'API', N'localhost')
INSERT [dbo].[tbl_user_tokenkeys] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (2, N'2c7046abfba75088c7859d44506eb7fc', CAST(N'2018-08-03T11:06:41.797' AS DateTime), CAST(N'2018-08-03T13:06:41.797' AS DateTime), N'API', N'192.168.2.48')
INSERT [dbo].[tbl_user_tokenkeys] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (2, N'7ba98ef183378095442e5302f87bfe5a', CAST(N'2019-08-13T14:31:04.873' AS DateTime), CAST(N'2019-08-14T07:11:04.873' AS DateTime), N'WEB', N'localhost')
INSERT [dbo].[tbl_user_tokenkeys] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (2, N'dd10addbad5718718e5e983a7ea8421e', CAST(N'2019-11-15T19:38:55.880' AS DateTime), CAST(N'2019-11-15T21:38:55.880' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (7, N'a4884a5e5bc2206e183ec3e501b19a08', CAST(N'2019-11-18T14:56:46.453' AS DateTime), CAST(N'2019-11-18T16:56:46.453' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (12, N'5c109af7ae3cc16238f154885fee829c', CAST(N'2019-09-07T10:05:24.427' AS DateTime), CAST(N'2019-09-07T12:05:24.427' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (16, N'a78738c4b995b15150ca4cb4ea0217b5', CAST(N'2019-10-05T17:57:17.833' AS DateTime), CAST(N'2019-10-05T19:57:17.833' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (17, N'6c660bca308357e6881b87b34d6002d3', CAST(N'2019-09-16T06:07:50.297' AS DateTime), CAST(N'2019-09-16T08:07:50.297' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (18, N'e6189f8dab8d6c419aab8f065d081bc8', CAST(N'2019-10-11T08:28:15.017' AS DateTime), CAST(N'2019-10-11T10:28:15.017' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (19, N'8aa312974c1494701e74c8832cbc70f5', CAST(N'2019-11-13T17:00:46.497' AS DateTime), CAST(N'2019-11-13T19:00:46.497' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (20, N'2c590b5124c8b2767732128f950bd47a', CAST(N'2019-09-17T05:39:58.403' AS DateTime), CAST(N'2019-09-17T07:39:58.403' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (21, N'cdebb95a53e7f1bd5ccb02b04c9cca02', CAST(N'2019-09-15T00:05:03.420' AS DateTime), CAST(N'2019-09-15T02:05:03.420' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (22, N'b0c313590ed4f8dec0ead66ca97aaac9', CAST(N'2019-11-20T17:41:08.370' AS DateTime), CAST(N'2019-11-20T19:41:08.370' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (24, N'b804e7de7caa98b4b8453a200e68fb35', CAST(N'2018-07-09T10:55:11.533' AS DateTime), CAST(N'2018-07-10T03:35:11.533' AS DateTime), N'API', N'localhost')
INSERT [dbo].[tbl_user_tokenkeys] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (28, N'787c4e4513c475c87f99cbe147db9339', CAST(N'2019-11-13T21:08:43.307' AS DateTime), CAST(N'2019-11-13T23:08:43.307' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (29, N'6596dab41f9e22ad01eb3c57e54cdaa6', CAST(N'2019-10-22T17:12:14.923' AS DateTime), CAST(N'2019-10-22T19:12:14.923' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (33, N'fe5fb5ebec57eb98c82b65677e35841d', CAST(N'2019-10-14T15:45:09.107' AS DateTime), CAST(N'2019-10-14T17:45:09.107' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (34, N'fc264ba4ad817052f7ac0a786bdba182', CAST(N'2019-10-06T18:39:32.953' AS DateTime), CAST(N'2019-10-06T20:39:32.953' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (35, N'dabacd345b1771466afa39231f780564', CAST(N'2019-10-07T11:24:18.603' AS DateTime), CAST(N'2019-10-07T13:24:18.603' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (1030, N'fe77872c2576568346abdd43ee86f104', CAST(N'2019-10-24T01:36:24.967' AS DateTime), CAST(N'2019-10-24T03:36:24.967' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (1031, N'02252de5c0f7e190c6cda1922a22351e', CAST(N'2019-10-10T17:57:11.903' AS DateTime), CAST(N'2019-10-10T19:57:11.903' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (1032, N'736fd660f22016df931c05cb0c1eec80', CAST(N'2019-10-10T18:28:35.077' AS DateTime), CAST(N'2019-10-10T20:28:35.077' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (1033, N'a43621e6f3da69c48e727294f70f7da2', CAST(N'2019-10-11T08:28:54.017' AS DateTime), CAST(N'2019-10-11T10:28:54.017' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (2031, N'4f34fdfae88b69b6e481356cc8435096', CAST(N'2019-11-13T21:18:44.577' AS DateTime), CAST(N'2019-11-13T23:18:44.577' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (2033, N'd8d35bde86ceaded57b9ae5fd9668e05', CAST(N'2019-10-24T01:40:49.920' AS DateTime), CAST(N'2019-10-24T03:40:49.920' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (2034, N'4d72ce75e7586aa19630e97f46132819', CAST(N'2019-11-12T13:09:08.840' AS DateTime), CAST(N'2019-11-12T15:09:08.840' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (2036, N'c3370ac1c7e2a353834f729af441fcb5', CAST(N'2019-11-16T04:38:54.883' AS DateTime), CAST(N'2019-11-16T06:38:54.883' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (2038, N'2391a1559beeca0419c7de1c0b811bc7', CAST(N'2019-11-22T02:48:37.367' AS DateTime), CAST(N'2019-11-22T04:48:37.367' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (1001008, N'3d0273786a1da774d9a173b890681c5b', CAST(N'2018-09-05T16:20:59.147' AS DateTime), CAST(N'2018-09-05T18:20:59.147' AS DateTime), N'API', N'192.168.2.48')
INSERT [dbo].[tbl_user_tokenkeys] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (1001008, N'9f5e4708d7f2237853ffb6f93de037bb', CAST(N'2018-10-25T09:42:17.983' AS DateTime), CAST(N'2018-10-26T02:22:17.983' AS DateTime), N'API', N'localhost')
INSERT [dbo].[tbl_user_tokenkeys] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (1001009, N'bde013fb0c1a5dbc0da03da4fe34fff4', CAST(N'2018-07-20T16:03:32.977' AS DateTime), CAST(N'2018-07-21T08:43:32.977' AS DateTime), N'API', N'localhost')
INSERT [dbo].[tbl_user_tokenkeys] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (1001014, N'2f6977d1debc8ee976c7b5de51cc4e59', CAST(N'2018-07-20T14:31:55.417' AS DateTime), CAST(N'2018-07-21T07:11:55.417' AS DateTime), N'API', N'localhost')
INSERT [dbo].[tbl_user_tokenkeys] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (1002063, N'4badba585b71a2240dc1c3a4eb03b06d', CAST(N'2018-08-13T14:22:54.633' AS DateTime), CAST(N'2018-08-13T16:22:54.633' AS DateTime), N'API', N'192.168.2.48')
INSERT [dbo].[tbl_user_tokenkeys] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (1003063, N'f00d3f3e60e80ca12eac59b0322b824c', CAST(N'2018-07-26T16:10:09.603' AS DateTime), CAST(N'2018-07-26T18:10:09.603' AS DateTime), N'API', N'192.168.2.48')
INSERT [dbo].[tbl_user_tokenkeys] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (1003064, N'43e689c32a3ca9ba08255d3ba2462fc7', CAST(N'2018-07-20T15:31:45.743' AS DateTime), CAST(N'2018-07-21T08:11:45.743' AS DateTime), N'API', N'localhost')
INSERT [dbo].[tbl_user_tokenkeys] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (1003065, N'6e4c3db75b91e0a42aafa2fc362c6004', CAST(N'2018-07-20T15:33:52.487' AS DateTime), CAST(N'2018-07-21T08:13:52.487' AS DateTime), N'API', N'localhost')
INSERT [dbo].[tbl_user_tokenkeys] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (1003066, N'cdb64f9a0b3155dcd04f044b46a2cef8', CAST(N'2018-07-20T15:36:57.483' AS DateTime), CAST(N'2018-07-21T08:16:57.483' AS DateTime), N'API', N'localhost')
INSERT [dbo].[tbl_user_tokenkeys] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (1003067, N'd78cae7f4faf2eae6d7d3a3a7311d57f', CAST(N'2018-07-20T15:38:06.750' AS DateTime), CAST(N'2018-07-21T08:18:06.750' AS DateTime), N'API', N'localhost')
INSERT [dbo].[tbl_user_tokenkeys] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (1003068, N'f56856f2e75d21c018c1a45e8d8c0065', CAST(N'2018-07-20T15:39:00.960' AS DateTime), CAST(N'2018-07-21T08:19:00.960' AS DateTime), N'API', N'localhost')
INSERT [dbo].[tbl_user_tokenkeys] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (1003069, N'c06b448ab3c20a5f546c7fbf66304aec', CAST(N'2018-07-20T16:03:53.260' AS DateTime), CAST(N'2018-07-21T08:43:53.260' AS DateTime), N'API', N'localhost')
INSERT [dbo].[tbl_user_tokenkeys] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (1003070, N'5d2c6e897ba1ead3bb59338159fa417e', CAST(N'2018-07-20T16:15:23.873' AS DateTime), CAST(N'2018-07-21T08:55:23.873' AS DateTime), N'API', N'localhost')
INSERT [dbo].[tbl_user_tokenkeys] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (1003071, N'f7f8a0e5fabceed4d17602e28c5ecccd', CAST(N'2018-07-20T16:16:31.333' AS DateTime), CAST(N'2018-07-21T08:56:31.333' AS DateTime), N'API', N'localhost')
INSERT [dbo].[tbl_user_tokenkeys] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (1003072, N'fe64d8742e27642a9a2a6eee24ab8e6c', CAST(N'2018-07-20T16:18:50.290' AS DateTime), CAST(N'2018-07-21T08:58:50.290' AS DateTime), N'API', N'localhost')
INSERT [dbo].[tbl_user_tokenkeys] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (1003073, N'25a5148c491eead8e3a9d60bb328358b', CAST(N'2018-07-20T16:19:37.873' AS DateTime), CAST(N'2018-07-21T08:59:37.873' AS DateTime), N'API', N'localhost')
INSERT [dbo].[tbl_user_tokenkeys] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (1003074, N'ed7d410123e7c532f795794a981eb4c0', CAST(N'2018-08-28T09:55:43.117' AS DateTime), CAST(N'2018-08-29T02:35:43.117' AS DateTime), N'API', N'localhost')
INSERT [dbo].[tbl_user_tokenkeys] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (1003075, N'33f771dca61f74d79a4bcc027f25498a', CAST(N'2018-07-20T16:37:31.813' AS DateTime), CAST(N'2018-07-21T09:17:31.813' AS DateTime), N'API', N'localhost')
INSERT [dbo].[tbl_user_tokenkeys] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (1003076, N'649f2f9138009b6b27dea5aa95d3a722', CAST(N'2018-07-20T16:39:33.400' AS DateTime), CAST(N'2018-07-21T09:19:33.400' AS DateTime), N'API', N'localhost')
INSERT [dbo].[tbl_user_tokenkeys] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (1003077, N'3cbcc9298050b3a1a37534b1ec04df03', CAST(N'2018-09-05T16:21:25.063' AS DateTime), CAST(N'2018-09-05T18:21:25.063' AS DateTime), N'API', N'192.168.2.48')
INSERT [dbo].[tbl_user_tokenkeys] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (1003077, N'a7db3875f74e84ed36773475ce4e7573', CAST(N'2018-10-26T16:22:45.073' AS DateTime), CAST(N'2018-10-27T09:02:45.073' AS DateTime), N'API', N'localhost')
INSERT [dbo].[tbl_user_tokenkeys] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (1003078, N'561ceae9ae962addfa2afb258e84335d', CAST(N'2018-09-06T09:06:45.310' AS DateTime), CAST(N'2018-09-06T11:06:45.310' AS DateTime), N'API', N'192.168.2.48')
INSERT [dbo].[tbl_user_tokenkeys] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (1003079, N'ad58f43c3ed406ab7fcc25f9771ce4c9', CAST(N'2018-08-10T09:24:09.697' AS DateTime), CAST(N'2018-08-11T02:04:09.697' AS DateTime), N'WEB', N'localhost')
INSERT [dbo].[tbl_user_tokenkeys] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (1003084, N'fe930f92cc6dcce84fa4be041706dfa4', CAST(N'2018-10-17T10:48:53.090' AS DateTime), CAST(N'2018-10-17T12:48:53.090' AS DateTime), N'API', N'192.168.2.48')
INSERT [dbo].[tbl_user_tokenkeys] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (1003085, N'70f7024305cba506cc14387d1140ee30', CAST(N'2018-09-14T09:55:16.643' AS DateTime), CAST(N'2018-09-14T11:55:16.643' AS DateTime), N'API', N'192.168.2.48')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (1, N'000585b817990ca1ec671405967ebd18', CAST(N'2019-10-15T00:37:10.100' AS DateTime), CAST(N'2019-10-15T02:37:10.100' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (1, N'0292420656a536c81687f495db6ecbec', CAST(N'2019-09-27T21:36:42.563' AS DateTime), CAST(N'2019-09-27T23:36:42.563' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (1, N'22f6095e32a0b960a45ec139f47af27c', CAST(N'2019-09-16T17:43:59.547' AS DateTime), CAST(N'2019-09-16T19:43:59.547' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (1, N'2346a80a2d0d03f54b64c965770606d6', CAST(N'2019-09-16T11:18:37.410' AS DateTime), CAST(N'2019-09-16T13:18:37.410' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (1, N'2935bd458a2864f7aeed6114efc7c114', CAST(N'2019-10-24T19:48:15.493' AS DateTime), CAST(N'2019-10-24T21:48:15.493' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (1, N'34fe88009906231777b123b9dc5ab0fc', CAST(N'2019-09-24T17:14:03.753' AS DateTime), CAST(N'2019-09-24T19:14:03.753' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (1, N'3832ca04659b7aa8f6cd5de337e60183', CAST(N'2019-10-02T10:19:52.380' AS DateTime), CAST(N'2019-10-02T12:19:52.380' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (1, N'3a1c28bc237bbb564e8c5d64374925e0', CAST(N'2019-09-04T22:51:19.733' AS DateTime), CAST(N'2019-09-05T00:51:19.733' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (1, N'4c32cbedfbc15800e2198b31be9096a6', CAST(N'2019-10-14T10:00:06.427' AS DateTime), CAST(N'2019-10-14T12:00:06.427' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (1, N'53e2996454c9f7db14ab5590ed3d88ba', CAST(N'2019-08-30T20:15:30.727' AS DateTime), CAST(N'2019-08-30T22:15:30.727' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (1, N'5ec09526da33f33cce8d6a05b3ef5e0a', CAST(N'2019-08-22T16:10:15.273' AS DateTime), CAST(N'2019-08-22T18:10:15.273' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (1, N'74b325332329f1feffdf28c754af1f36', CAST(N'2019-09-05T16:24:32.797' AS DateTime), CAST(N'2019-09-05T18:24:32.797' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (1, N'7ac5fb4ac5815315be3e54b4b0bc42b2', CAST(N'2019-09-15T14:43:36.833' AS DateTime), CAST(N'2019-09-15T16:43:36.833' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (1, N'8050ad151dd0d90b84a3adcc3d656c1d', CAST(N'2019-08-22T08:35:02.860' AS DateTime), CAST(N'2019-08-22T10:35:02.860' AS DateTime), N'WEB', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (1, N'809810ff5293e2092d5aed1e8c0e470b', CAST(N'2019-10-09T09:41:07.050' AS DateTime), CAST(N'2019-10-09T11:41:07.050' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (1, N'845b4c8b359314bfb0bd4b25f726e44e', CAST(N'2019-10-23T20:32:47.030' AS DateTime), CAST(N'2019-10-23T22:32:47.030' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (1, N'8b8a496a376fe810e2b41c797fcd7705', CAST(N'2019-10-24T12:33:50.160' AS DateTime), CAST(N'2019-10-24T14:33:50.160' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (1, N'a52f599a743066810d5ca01a4e73ae10', CAST(N'2019-11-08T10:01:37.247' AS DateTime), CAST(N'2019-11-08T12:01:37.247' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (1, N'a6a14cd763e13bd30bd7d2880ee7a60e', CAST(N'2019-11-02T20:43:05.083' AS DateTime), CAST(N'2019-11-02T22:43:05.083' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (1, N'b42a39069ff20a891507825b04d647d8', CAST(N'2019-10-05T08:30:43.830' AS DateTime), CAST(N'2019-10-05T10:30:43.830' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (1, N'ba5e3a05ad2c8ea5fd4b01d0532f6bd4', CAST(N'2019-09-24T10:24:49.787' AS DateTime), CAST(N'2019-09-24T12:24:49.787' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (1, N'c5949025f9596af78af3f7055c42b99e', CAST(N'2019-10-01T16:15:21.640' AS DateTime), CAST(N'2019-10-01T18:15:21.640' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (1, N'c892d9815fd0424736af3fbf937f1d4b', CAST(N'2019-10-20T07:28:02.140' AS DateTime), CAST(N'2019-10-20T09:28:02.140' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (1, N'cc9d3f0bc3d22c78ce8a6374fc711083', CAST(N'2019-09-14T15:07:12.287' AS DateTime), CAST(N'2019-09-14T17:07:12.287' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (1, N'd03a9409872b87cc0f20521f4540533c', CAST(N'2019-08-29T16:55:50.853' AS DateTime), CAST(N'2019-08-29T18:55:50.853' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (1, N'dd720f5ab8963282b77504c72222f5a7', CAST(N'2019-10-14T14:15:36.430' AS DateTime), CAST(N'2019-10-14T16:15:36.430' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (1, N'e394c2b9f0b8cc7574f6037dc3c18347', CAST(N'2019-08-31T11:23:09.970' AS DateTime), CAST(N'2019-08-31T13:23:09.970' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (1, N'fc57010814af1de02802448910c37097', CAST(N'2019-10-26T12:03:32.403' AS DateTime), CAST(N'2019-10-26T14:03:32.403' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (1, N'fe36a2dbf4971f113258a4f5e5e87a89', CAST(N'2019-09-13T10:07:54.777' AS DateTime), CAST(N'2019-09-13T12:07:54.777' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (2, N'421051242d510747673430cfb95093b3', CAST(N'2019-10-08T19:37:13.063' AS DateTime), CAST(N'2019-10-08T21:37:13.063' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (2, N'470f7947663409e34ec9048e646fa5d0', CAST(N'2019-08-21T22:56:11.347' AS DateTime), CAST(N'2019-08-22T00:56:11.347' AS DateTime), N'WEB', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (2, N'aa652c6c28d02b5cb54994a418e39479', CAST(N'2019-11-01T12:02:08.090' AS DateTime), CAST(N'2019-11-01T14:02:08.090' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (2, N'dc78714df2a48c755a21e9e8bf349f7c', CAST(N'2019-08-16T14:49:52.420' AS DateTime), CAST(N'2019-08-16T16:49:52.420' AS DateTime), N'WEB', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (16, N'1a2a41b369aba30adbe8d26df9bbcb0e', CAST(N'2019-09-12T08:05:04.150' AS DateTime), CAST(N'2019-09-12T10:05:04.150' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (17, N'19a558ee6b55d0b5d9af2a2804d8d7a4', CAST(N'2019-09-14T22:37:14.847' AS DateTime), CAST(N'2019-09-15T00:37:14.847' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (17, N'374e98eecd44d91545a312b5a8503cc4', CAST(N'2019-09-13T06:23:12.870' AS DateTime), CAST(N'2019-09-13T08:23:12.870' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (18, N'cd592503402d3bc5fc96ca72000b2ce4', CAST(N'2019-09-14T09:06:58.840' AS DateTime), CAST(N'2019-09-14T11:06:58.840' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (19, N'210233461d1b68822bbc831a5ed33009', CAST(N'2019-10-04T19:42:02.847' AS DateTime), CAST(N'2019-10-04T21:42:02.847' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (19, N'2caad16d82f1fdf33ea55748f4ec5ccf', CAST(N'2019-09-14T17:24:51.460' AS DateTime), CAST(N'2019-09-14T19:24:51.460' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (19, N'43e3f76a97035514387a8ef55095bfa3', CAST(N'2019-10-18T09:49:54.753' AS DateTime), CAST(N'2019-10-18T11:49:54.753' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (19, N'7e4259d5547ba53cc6a1608db55fda3f', CAST(N'2019-09-16T08:32:03.383' AS DateTime), CAST(N'2019-09-16T10:32:03.383' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (19, N'9e142e7515b11cd1793b26653c81afc3', CAST(N'2019-11-02T22:52:24.023' AS DateTime), CAST(N'2019-11-03T00:52:24.023' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (19, N'bd533db029767be0790c74de430f67d7', CAST(N'2019-10-10T22:59:59.633' AS DateTime), CAST(N'2019-10-11T00:59:59.633' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (19, N'c0d02d9291e544721d5db6f6ad55b011', CAST(N'2019-10-26T12:47:26.587' AS DateTime), CAST(N'2019-10-26T14:47:26.587' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (19, N'c24679f945b23750562a106f118d54eb', CAST(N'2019-11-10T11:47:38.023' AS DateTime), CAST(N'2019-11-10T13:47:38.023' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (19, N'c9752544ec9abd7efe5fb040c87b352f', CAST(N'2019-09-15T00:03:46.103' AS DateTime), CAST(N'2019-09-15T02:03:46.103' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (19, N'f3c4fd04536b209ae59762e398feb33b', CAST(N'2019-10-04T09:38:21.187' AS DateTime), CAST(N'2019-10-04T11:38:21.187' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (20, N'cd57dc604cbb8d8f3dafbbeb9de72715', CAST(N'2019-09-16T05:23:09.153' AS DateTime), CAST(N'2019-09-16T07:23:09.153' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (20, N'e96ffd53bdeb57bddc93849291e51dd7', CAST(N'2019-09-14T22:35:41.813' AS DateTime), CAST(N'2019-09-15T00:35:41.813' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (22, N'057ffa8bc9c9af47c14990da98832318', CAST(N'2019-10-23T12:00:02.620' AS DateTime), CAST(N'2019-10-23T14:00:02.620' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (22, N'0d823f46da0b7d7da69a6ed8c6c9ee12', CAST(N'2019-10-05T13:22:43.540' AS DateTime), CAST(N'2019-10-05T15:22:43.540' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (22, N'3466d43d91df582461e914a058d75f49', CAST(N'2019-11-13T15:11:40.467' AS DateTime), CAST(N'2019-11-13T17:11:40.467' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (22, N'352d565f6e47a18db3698856635e3ba7', CAST(N'2019-11-03T01:52:48.353' AS DateTime), CAST(N'2019-11-03T03:52:48.353' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (22, N'468ae25e5a243e3f6b2b4f2e34d15f1b', CAST(N'2019-10-14T14:39:04.937' AS DateTime), CAST(N'2019-10-14T16:39:04.937' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (22, N'5583ade3ebde821449213cd4554bbd86', CAST(N'2019-10-18T17:06:07.827' AS DateTime), CAST(N'2019-10-18T19:06:07.827' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (22, N'5c4bf70b2a33baab0daae645bea83d5c', CAST(N'2019-10-30T23:39:05.077' AS DateTime), CAST(N'2019-10-31T01:39:05.077' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (22, N'873c1c9877de3843c603f496eaee81d8', CAST(N'2019-10-10T17:30:50.583' AS DateTime), CAST(N'2019-10-10T19:30:50.583' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (22, N'9bf432a51692d994ebfc3d762fda38a5', CAST(N'2019-09-29T06:02:21.493' AS DateTime), CAST(N'2019-09-29T08:02:21.493' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (22, N'b36c6027ab7872d36c8d05036d8d2bd8', CAST(N'2019-09-18T09:19:16.597' AS DateTime), CAST(N'2019-09-18T11:19:16.597' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (22, N'dbaea25ebf103cc7f5d5f220b92375d8', CAST(N'2019-11-11T13:50:49.297' AS DateTime), CAST(N'2019-11-11T15:50:49.297' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (22, N'de795f154219951936c8e765aea8fd4c', CAST(N'2019-10-03T09:29:28.753' AS DateTime), CAST(N'2019-10-03T11:29:28.753' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (22, N'e2612b6956192ce11d1198776300418e', CAST(N'2019-10-24T02:02:46.970' AS DateTime), CAST(N'2019-10-24T04:02:46.970' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (22, N'e85221c94af048bafe3aa89f5fc72a34', CAST(N'2019-09-25T11:03:07.390' AS DateTime), CAST(N'2019-09-25T13:03:07.390' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (28, N'3dbe15d66e96c961b92a8e2aeb308caf', CAST(N'2019-10-20T20:39:14.587' AS DateTime), CAST(N'2019-10-20T22:39:14.587' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (28, N'9357413daef56227d2f74290cd2423df', CAST(N'2019-09-27T10:53:34.957' AS DateTime), CAST(N'2019-09-27T12:53:34.957' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (29, N'7142ecec599c4be67f9dfe75ce9d18fd', CAST(N'2019-09-29T09:21:16.257' AS DateTime), CAST(N'2019-09-29T11:21:16.257' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (29, N'8571e66661d674d589acc3ed2b78bae8', CAST(N'2019-10-16T17:50:23.643' AS DateTime), CAST(N'2019-10-16T19:50:23.643' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (29, N'b7c4b0b695dc0c201c33205a30d99003', CAST(N'2019-10-09T21:46:54.627' AS DateTime), CAST(N'2019-10-09T23:46:54.627' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (29, N'c19555619daaa630a77c4aa596be2b36', CAST(N'2019-10-02T19:20:41.243' AS DateTime), CAST(N'2019-10-02T21:20:41.243' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (29, N'e2ec39ec4326e7a9604d2e24f22a3616', CAST(N'2019-10-05T10:58:01.450' AS DateTime), CAST(N'2019-10-05T12:58:01.450' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (33, N'60126150505004e2aac337b9f9e6f97b', CAST(N'2019-10-05T17:58:21.617' AS DateTime), CAST(N'2019-10-05T19:58:21.617' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (1030, N'46d256efcd9ab3396d275d099376dbe4', CAST(N'2019-10-10T17:41:30.073' AS DateTime), CAST(N'2019-10-10T19:41:30.073' AS DateTime), N'API', N'account.job-market.jp')
INSERT [dbo].[tbl_user_tokenkeys_history] ([UserId], [TokenKey], [CreatedDate], [ExpiredDate], [Method], [Domain]) VALUES (1030, N'899859ffb3d6a415a8da30b30b29304e', CAST(N'2019-10-14T16:20:30.063' AS DateTime), CAST(N'2019-10-14T18:20:30.063' AS DateTime), N'API', N'account.job-market.jp')
SET IDENTITY_INSERT [dbo].[tbl_users] ON 

INSERT [dbo].[tbl_users] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName], [CreatedDateUtc], [PasswordHash2], [FullName], [DisplayName], [Avatar], [OTPType], [Birthday], [Sex], [Address], [IDCard], [Note], [SocialProviderId], [LastOnline]) VALUES (1, N'bangkhmt3@gmail.com', 1, N'e10adc3949ba59abbe56e057f20f883e', N'371b9c84c640a9e121523156aeae4958', N'0966722466', 1, 0, NULL, 0, 0, N'bangkhmt3@gmail.com', NULL, NULL, N'Vũ Lương Bằng', N'Bằng', N'Avatars/1/1539328575.jpg', NULL, CAST(N'1991-08-12T00:00:00.000' AS DateTime), 1, N'', NULL, N'', 0, CAST(N'2018-10-12T15:42:07.863' AS DateTime))
INSERT [dbo].[tbl_users] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName], [CreatedDateUtc], [PasswordHash2], [FullName], [DisplayName], [Avatar], [OTPType], [Birthday], [Sex], [Address], [IDCard], [Note], [SocialProviderId], [LastOnline]) VALUES (2, N'tester@gmail.com', 1, N'e10adc3949ba59abbe56e057f20f883e', NULL, N'0123456789', 1, 0, NULL, 0, 0, N'tester@gmail.com', NULL, NULL, N'Nguyễn Văn A', N'Mr Nguyen', N'Avatars/2_1526366296.jpg', NULL, CAST(N'1991-08-12T00:00:00.000' AS DateTime), 1, N'', NULL, N'', 0, NULL)
INSERT [dbo].[tbl_users] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName], [CreatedDateUtc], [PasswordHash2], [FullName], [DisplayName], [Avatar], [OTPType], [Birthday], [Sex], [Address], [IDCard], [Note], [SocialProviderId], [LastOnline]) VALUES (7, N'registeronly12891@gmail.com', 1, N'e10adc3949ba59abbe56e057f20f883e', N'71d1d91bde21cb358c2c5895654685b1.N3xsZXZlbDE=.8f14e45fceea167a5a36dedd4bea2543', N'', 1, 0, NULL, 0, 0, N'registeronly12891@gmail.com', NULL, NULL, N'registeronly12891@gmail.com', N'Mr Bằng', N'', NULL, NULL, 1, N'', NULL, N'', 0, NULL)
INSERT [dbo].[tbl_users] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName], [CreatedDateUtc], [PasswordHash2], [FullName], [DisplayName], [Avatar], [OTPType], [Birthday], [Sex], [Address], [IDCard], [Note], [SocialProviderId], [LastOnline]) VALUES (8, N'khuatdinhtrong@gmail.com', 1, N'cc40bf3d9de0d62911c630c6dfdb7190', NULL, N'0338276363', 0, 0, NULL, 0, 0, N'trong.kd', CAST(N'2019-08-21T22:56:22.427' AS DateTime), NULL, NULL, NULL, NULL, N'OTPSMS', CAST(N'1994-10-09T00:00:00.000' AS DateTime), 1, NULL, NULL, NULL, 0, CAST(N'2019-08-21T22:56:22.427' AS DateTime))
INSERT [dbo].[tbl_users] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName], [CreatedDateUtc], [PasswordHash2], [FullName], [DisplayName], [Avatar], [OTPType], [Birthday], [Sex], [Address], [IDCard], [Note], [SocialProviderId], [LastOnline]) VALUES (12, N'kakahaha6193@gmail.com', 1, N'fc7df0b596b34c9703c8dc050a9bf271', N'371b9c84c640a9e121523156aeae4958', NULL, 0, 0, NULL, 0, 0, N'kakahaha6193@gmail.com', CAST(N'2019-09-07T10:05:00.270' AS DateTime), NULL, NULL, NULL, NULL, N'OTPSMS', CAST(N'2019-09-07T10:05:00.257' AS DateTime), 0, NULL, NULL, NULL, 0, CAST(N'2019-09-07T10:05:00.270' AS DateTime))
INSERT [dbo].[tbl_users] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName], [CreatedDateUtc], [PasswordHash2], [FullName], [DisplayName], [Avatar], [OTPType], [Birthday], [Sex], [Address], [IDCard], [Note], [SocialProviderId], [LastOnline]) VALUES (13, N'ducminh@gmail.com', 0, N'33453ff6679257c5299e94c8215b6be5', NULL, N'012346789', 0, 0, NULL, 0, 0, N'ducminh@gmail.com', CAST(N'2019-09-11T09:20:44.050' AS DateTime), NULL, NULL, NULL, NULL, N'OTPSMS', CAST(N'2019-09-11T09:20:44.020' AS DateTime), 1, NULL, NULL, NULL, 0, CAST(N'2019-09-11T09:20:44.050' AS DateTime))
INSERT [dbo].[tbl_users] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName], [CreatedDateUtc], [PasswordHash2], [FullName], [DisplayName], [Avatar], [OTPType], [Birthday], [Sex], [Address], [IDCard], [Note], [SocialProviderId], [LastOnline]) VALUES (14, N'minhscdhpvn@gmail.com', 0, N'883d963273467cc7a740d5428a275223', N'', NULL, 0, 0, NULL, 0, 0, N'ducminh', CAST(N'2019-09-11T09:29:11.133' AS DateTime), NULL, NULL, NULL, NULL, N'OTPSMS', CAST(N'2019-09-11T09:29:11.120' AS DateTime), 0, NULL, NULL, NULL, 0, CAST(N'2019-09-11T09:29:11.133' AS DateTime))
INSERT [dbo].[tbl_users] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName], [CreatedDateUtc], [PasswordHash2], [FullName], [DisplayName], [Avatar], [OTPType], [Birthday], [Sex], [Address], [IDCard], [Note], [SocialProviderId], [LastOnline]) VALUES (15, N'minhscd1hpvn@gmail.com', 0, N'883d963273467cc7a740d5428a275223', N'', NULL, 0, 0, NULL, 0, 0, N'ducminh1', CAST(N'2019-09-11T09:35:15.610' AS DateTime), NULL, NULL, NULL, NULL, N'OTPSMS', CAST(N'2019-09-11T09:35:15.610' AS DateTime), 0, NULL, NULL, NULL, 0, CAST(N'2019-09-11T09:35:15.610' AS DateTime))
INSERT [dbo].[tbl_users] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName], [CreatedDateUtc], [PasswordHash2], [FullName], [DisplayName], [Avatar], [OTPType], [Birthday], [Sex], [Address], [IDCard], [Note], [SocialProviderId], [LastOnline]) VALUES (16, N'xyz@gmail.com', 1, NULL, NULL, NULL, 1, 0, NULL, 0, 0, N'124725983475', NULL, NULL, N'Nguyen Van A', N'Nguyen Van A', N'https://graph.facebook.com/124725983475/picture?type=normal', NULL, NULL, 1, NULL, NULL, NULL, 1, CAST(N'2019-09-12T08:05:04.103' AS DateTime))
INSERT [dbo].[tbl_users] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName], [CreatedDateUtc], [PasswordHash2], [FullName], [DisplayName], [Avatar], [OTPType], [Birthday], [Sex], [Address], [IDCard], [Note], [SocialProviderId], [LastOnline]) VALUES (17, NULL, 1, NULL, NULL, NULL, 1, 0, NULL, 0, 0, N'1374291676057314', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, 1, CAST(N'2019-09-13T06:23:12.853' AS DateTime))
INSERT [dbo].[tbl_users] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName], [CreatedDateUtc], [PasswordHash2], [FullName], [DisplayName], [Avatar], [OTPType], [Birthday], [Sex], [Address], [IDCard], [Note], [SocialProviderId], [LastOnline]) VALUES (18, NULL, 1, NULL, NULL, NULL, 1, 0, NULL, 0, 0, N'1406848862813015', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, 1, CAST(N'2019-09-14T09:06:58.840' AS DateTime))
INSERT [dbo].[tbl_users] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName], [CreatedDateUtc], [PasswordHash2], [FullName], [DisplayName], [Avatar], [OTPType], [Birthday], [Sex], [Address], [IDCard], [Note], [SocialProviderId], [LastOnline]) VALUES (19, N'minhnv54@gmail.com', 1, N'08a4984d6cec066ab5707cdb42882f1c', NULL, NULL, 0, 0, NULL, 0, 0, N'minhnv54@gmail.com', CAST(N'2019-09-14T17:24:13.227' AS DateTime), NULL, NULL, NULL, NULL, N'OTPSMS', CAST(N'2019-09-14T17:24:13.180' AS DateTime), 0, NULL, NULL, NULL, 0, CAST(N'2019-09-14T17:24:13.227' AS DateTime))
INSERT [dbo].[tbl_users] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName], [CreatedDateUtc], [PasswordHash2], [FullName], [DisplayName], [Avatar], [OTPType], [Birthday], [Sex], [Address], [IDCard], [Note], [SocialProviderId], [LastOnline]) VALUES (20, NULL, 1, NULL, NULL, NULL, 1, 0, NULL, 0, 0, N'minhittduc@gmail.com', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, 2, CAST(N'2019-09-14T22:35:41.797' AS DateTime))
INSERT [dbo].[tbl_users] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName], [CreatedDateUtc], [PasswordHash2], [FullName], [DisplayName], [Avatar], [OTPType], [Birthday], [Sex], [Address], [IDCard], [Note], [SocialProviderId], [LastOnline]) VALUES (21, NULL, 1, NULL, NULL, NULL, 1, 0, NULL, 0, 0, N'minhnv54@gmail.com', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, 2, CAST(N'2019-09-15T00:05:03.420' AS DateTime))
INSERT [dbo].[tbl_users] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName], [CreatedDateUtc], [PasswordHash2], [FullName], [DisplayName], [Avatar], [OTPType], [Birthday], [Sex], [Address], [IDCard], [Note], [SocialProviderId], [LastOnline]) VALUES (22, N'huynhtuanhuy1996@gmail.com', 1, N'a10c109915e36770c291dcd3880f8517', NULL, NULL, 0, 0, NULL, 0, 0, N'huynhtuanhuy1996@gmail.com', CAST(N'2019-09-18T09:19:00.317' AS DateTime), NULL, NULL, NULL, NULL, N'OTPSMS', CAST(N'2019-09-18T09:19:00.303' AS DateTime), 0, NULL, NULL, NULL, 0, CAST(N'2019-09-18T09:19:00.317' AS DateTime))
INSERT [dbo].[tbl_users] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName], [CreatedDateUtc], [PasswordHash2], [FullName], [DisplayName], [Avatar], [OTPType], [Birthday], [Sex], [Address], [IDCard], [Note], [SocialProviderId], [LastOnline]) VALUES (23, N'minhscd2hpvn@gmail.com', 0, N'883d963273467cc7a740d5428a275223', N'', NULL, 0, 0, NULL, 0, 0, N'ducminh2', CAST(N'2019-09-18T15:58:20.730' AS DateTime), NULL, NULL, NULL, NULL, N'OTPSMS', CAST(N'2019-09-18T15:58:20.730' AS DateTime), 0, NULL, NULL, NULL, 0, CAST(N'2019-09-18T15:58:20.730' AS DateTime))
INSERT [dbo].[tbl_users] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName], [CreatedDateUtc], [PasswordHash2], [FullName], [DisplayName], [Avatar], [OTPType], [Birthday], [Sex], [Address], [IDCard], [Note], [SocialProviderId], [LastOnline]) VALUES (28, N'vuducthuong1102@gmail.com', 1, N'e10adc3949ba59abbe56e057f20f883e', NULL, NULL, 0, 0, NULL, 0, 0, N'vuducthuong1102@gmail.com', CAST(N'2019-09-27T10:52:46.233' AS DateTime), NULL, NULL, N'vuducthuong1102@gmail.com', NULL, N'OTPSMS', CAST(N'2019-09-27T10:52:46.193' AS DateTime), 0, NULL, NULL, NULL, 0, CAST(N'2019-09-27T10:52:46.233' AS DateTime))
INSERT [dbo].[tbl_users] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName], [CreatedDateUtc], [PasswordHash2], [FullName], [DisplayName], [Avatar], [OTPType], [Birthday], [Sex], [Address], [IDCard], [Note], [SocialProviderId], [LastOnline]) VALUES (29, N'tranducminhhpvn99@gmail.com', 1, N'87ce06dec453f68c0266ee800ce76873', NULL, N'', 0, 0, NULL, 0, 0, N'tranducminhhpvn99@gmail.com', CAST(N'2019-09-29T09:14:13.170' AS DateTime), NULL, N'', N'ducminh', N'', N'OTPSMS', CAST(N'2019-09-29T09:14:13.170' AS DateTime), 0, N'', NULL, N'', 0, CAST(N'2019-09-29T09:14:13.170' AS DateTime))
INSERT [dbo].[tbl_users] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName], [CreatedDateUtc], [PasswordHash2], [FullName], [DisplayName], [Avatar], [OTPType], [Birthday], [Sex], [Address], [IDCard], [Note], [SocialProviderId], [LastOnline]) VALUES (30, N'huynhtuanhuy1997@gmail.com', 0, N'6f8f57715090da2632453988d9a1501b', N'753b064dab451d92001f276cfd11c64c.aHV5bmh0dWFuaHV5MTk5N0BnbWFpbC5jb218aHV5bmh0dWFuaHV5MTk5N0BnbWFpbC5jb20=.b071060c749cd44d8ac32b5119194023', NULL, 0, 0, NULL, 0, 0, N'huynhtuanhuy1997@gmail.com', CAST(N'2019-10-03T09:13:48.730' AS DateTime), NULL, NULL, NULL, NULL, N'OTPSMS', CAST(N'2019-10-03T09:13:48.733' AS DateTime), 0, NULL, NULL, NULL, 0, CAST(N'2019-10-03T09:13:48.730' AS DateTime))
INSERT [dbo].[tbl_users] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName], [CreatedDateUtc], [PasswordHash2], [FullName], [DisplayName], [Avatar], [OTPType], [Birthday], [Sex], [Address], [IDCard], [Note], [SocialProviderId], [LastOnline]) VALUES (31, N'huynhtuanhuy1238@gmail.com', 0, N'e10adc3949ba59abbe56e057f20f883e', N'2c2789d14ab8be67e6d628c2c30dc4ea.aHV5bmh0dWFuaHV5MTIzOEBnbWFpbC5jb218aHV5bmh0dWFuaHV5MTIzOEBnbWFpbC5jb20=.b482aed771182d0345260befb3eb0109', NULL, 0, 0, NULL, 0, 0, N'huynhtuanhuy1238@gmail.com', CAST(N'2019-10-03T09:29:08.940' AS DateTime), NULL, NULL, NULL, NULL, N'OTPSMS', CAST(N'2019-10-03T09:29:08.927' AS DateTime), 0, NULL, NULL, NULL, 0, CAST(N'2019-10-03T09:29:08.940' AS DateTime))
INSERT [dbo].[tbl_users] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName], [CreatedDateUtc], [PasswordHash2], [FullName], [DisplayName], [Avatar], [OTPType], [Birthday], [Sex], [Address], [IDCard], [Note], [SocialProviderId], [LastOnline]) VALUES (32, N'huynhtuanhuy1097@gmail.com', 0, N'6db4ef0c498f805460d4db55d103c4de', N'85dd8f62299c144d0f76662a5f0046b1.aHV5bmh0dWFuaHV5MTA5N0BnbWFpbC5jb218aHV5bmh0dWFuaHV5MTA5N0BnbWFpbC5jb20=.3d624da5ab7ef4b5591bb4cd7bd4d870', NULL, 0, 0, NULL, 0, 0, N'huynhtuanhuy1097@gmail.com', CAST(N'2019-10-03T14:38:59.023' AS DateTime), NULL, NULL, NULL, NULL, N'OTPSMS', CAST(N'2019-10-03T14:38:59.010' AS DateTime), 0, NULL, NULL, NULL, 0, CAST(N'2019-10-03T14:38:59.023' AS DateTime))
INSERT [dbo].[tbl_users] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName], [CreatedDateUtc], [PasswordHash2], [FullName], [DisplayName], [Avatar], [OTPType], [Birthday], [Sex], [Address], [IDCard], [Note], [SocialProviderId], [LastOnline]) VALUES (33, N'123123231@gmail.com', 1, NULL, NULL, NULL, 1, 0, NULL, 0, 0, N'12321323213', NULL, NULL, N'Nguyen Van A', N'Nguyen Van A', NULL, NULL, NULL, 1, NULL, NULL, NULL, 1, CAST(N'2019-10-05T17:58:21.600' AS DateTime))
INSERT [dbo].[tbl_users] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName], [CreatedDateUtc], [PasswordHash2], [FullName], [DisplayName], [Avatar], [OTPType], [Birthday], [Sex], [Address], [IDCard], [Note], [SocialProviderId], [LastOnline]) VALUES (34, N'mr.tungvx@gmail.com', 1, N'bc25d3f216239ea36aa6275ee9752525', NULL, NULL, 0, 0, NULL, 0, 0, N'mr.tungvx@gmail.com', CAST(N'2019-10-06T18:38:58.123' AS DateTime), NULL, NULL, N'mr.tungvx@gmail.com', NULL, N'OTPSMS', CAST(N'2019-10-06T18:38:58.097' AS DateTime), 0, NULL, NULL, NULL, 0, CAST(N'2019-10-06T18:38:58.123' AS DateTime))
INSERT [dbo].[tbl_users] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName], [CreatedDateUtc], [PasswordHash2], [FullName], [DisplayName], [Avatar], [OTPType], [Birthday], [Sex], [Address], [IDCard], [Note], [SocialProviderId], [LastOnline]) VALUES (35, N'thuyngocjp92@gmail.com', 1, N'b44b0fc0c06383ba1ba7c6aeb926287c', NULL, NULL, 0, 0, NULL, 0, 0, N'thuyngocjp92@gmail.com', CAST(N'2019-10-07T11:19:55.990' AS DateTime), NULL, NULL, N'thuyngocjp92@gmail.com', NULL, N'OTPSMS', CAST(N'2019-10-07T11:19:55.993' AS DateTime), 0, NULL, NULL, NULL, 0, CAST(N'2019-10-07T11:19:55.990' AS DateTime))
INSERT [dbo].[tbl_users] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName], [CreatedDateUtc], [PasswordHash2], [FullName], [DisplayName], [Avatar], [OTPType], [Birthday], [Sex], [Address], [IDCard], [Note], [SocialProviderId], [LastOnline]) VALUES (1030, NULL, 1, NULL, NULL, NULL, 1, 0, NULL, 0, 0, N'huynhtuanhuy1996@gmail.com', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, 2, CAST(N'2019-10-10T17:41:30.057' AS DateTime))
INSERT [dbo].[tbl_users] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName], [CreatedDateUtc], [PasswordHash2], [FullName], [DisplayName], [Avatar], [OTPType], [Birthday], [Sex], [Address], [IDCard], [Note], [SocialProviderId], [LastOnline]) VALUES (1031, N'prince.of.sun.9x@gmail.com', 1, NULL, NULL, NULL, 1, 0, NULL, 0, 0, N'prince.of.sun.9x@gmail.com', NULL, NULL, N'anh nguyen', N'anh nguyen', NULL, NULL, NULL, 1, NULL, NULL, NULL, 2, CAST(N'2019-10-10T17:57:11.887' AS DateTime))
INSERT [dbo].[tbl_users] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName], [CreatedDateUtc], [PasswordHash2], [FullName], [DisplayName], [Avatar], [OTPType], [Birthday], [Sex], [Address], [IDCard], [Note], [SocialProviderId], [LastOnline]) VALUES (1032, N'huynhtuanhuy1996@gmail.com', 1, NULL, NULL, NULL, 1, 0, NULL, 0, 0, N'1261994027340506', NULL, NULL, N'Huỳnh Tuấn Huy', N'Huỳnh Tuấn Huy', NULL, NULL, NULL, 1, NULL, NULL, NULL, 1, CAST(N'2019-10-10T18:28:35.077' AS DateTime))
INSERT [dbo].[tbl_users] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName], [CreatedDateUtc], [PasswordHash2], [FullName], [DisplayName], [Avatar], [OTPType], [Birthday], [Sex], [Address], [IDCard], [Note], [SocialProviderId], [LastOnline]) VALUES (1033, N'minhnv543@gmail.com', 1, NULL, NULL, NULL, 1, 0, NULL, 0, 0, N'minhnv543@gmail.com', NULL, NULL, N'Impossible Nothing''s', N'Impossible Nothing''s', NULL, NULL, NULL, 1, NULL, NULL, NULL, 2, CAST(N'2019-10-11T08:28:54.003' AS DateTime))
INSERT [dbo].[tbl_users] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName], [CreatedDateUtc], [PasswordHash2], [FullName], [DisplayName], [Avatar], [OTPType], [Birthday], [Sex], [Address], [IDCard], [Note], [SocialProviderId], [LastOnline]) VALUES (2030, N'ghitacluber@gmail.com', 0, N'z1x2c3v4', N'', NULL, 0, 0, NULL, 0, 0, N'quangdvn', CAST(N'2019-10-14T15:07:29.270' AS DateTime), NULL, NULL, NULL, NULL, N'OTPSMS', CAST(N'2019-10-14T15:07:29.250' AS DateTime), 0, NULL, NULL, NULL, 0, CAST(N'2019-10-14T15:07:29.270' AS DateTime))
INSERT [dbo].[tbl_users] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName], [CreatedDateUtc], [PasswordHash2], [FullName], [DisplayName], [Avatar], [OTPType], [Birthday], [Sex], [Address], [IDCard], [Note], [SocialProviderId], [LastOnline]) VALUES (2031, N'registeronly12892@gmail.com', 1, N'e10adc3949ba59abbe56e057f20f883e', NULL, NULL, 0, 0, NULL, 0, 0, N'registeronly12892@gmail.com', CAST(N'2019-10-14T15:45:28.700' AS DateTime), NULL, NULL, N'registeronly12892@gmail.com', NULL, N'OTPSMS', CAST(N'2019-10-14T15:45:28.700' AS DateTime), 0, NULL, NULL, NULL, 0, CAST(N'2019-10-14T15:45:28.700' AS DateTime))
INSERT [dbo].[tbl_users] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName], [CreatedDateUtc], [PasswordHash2], [FullName], [DisplayName], [Avatar], [OTPType], [Birthday], [Sex], [Address], [IDCard], [Note], [SocialProviderId], [LastOnline]) VALUES (2032, N'bpi84362@eanok.com', 0, N'a10c109915e36770c291dcd3880f8517', N'5aea3c845ae58feeaa40f375ff226567.YnBpODQzNjJAZWFub2suY29tfGJwaTg0MzYyQGVhbm9rLmNvbQ==.19a2c4d57802fdceb8304302240be207', NULL, 0, 0, NULL, 0, 0, N'bpi84362@eanok.com', CAST(N'2019-10-24T01:38:24.583' AS DateTime), NULL, NULL, N'bpi84362@eanok.com', NULL, N'OTPSMS', CAST(N'2019-10-24T01:38:24.583' AS DateTime), 0, NULL, NULL, NULL, 0, CAST(N'2019-10-24T01:38:24.583' AS DateTime))
INSERT [dbo].[tbl_users] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName], [CreatedDateUtc], [PasswordHash2], [FullName], [DisplayName], [Avatar], [OTPType], [Birthday], [Sex], [Address], [IDCard], [Note], [SocialProviderId], [LastOnline]) VALUES (2033, N'agdahtahp@mailnesia.com', 1, N'e10adc3949ba59abbe56e057f20f883e', NULL, NULL, 0, 0, NULL, 0, 0, N'agdahtahp@mailnesia.com', CAST(N'2019-10-24T01:40:03.353' AS DateTime), NULL, NULL, N'agdahtahp@mailnesia.com', NULL, N'OTPSMS', CAST(N'2019-10-24T01:40:03.353' AS DateTime), 0, NULL, NULL, NULL, 0, CAST(N'2019-10-24T01:40:03.353' AS DateTime))
INSERT [dbo].[tbl_users] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName], [CreatedDateUtc], [PasswordHash2], [FullName], [DisplayName], [Avatar], [OTPType], [Birthday], [Sex], [Address], [IDCard], [Note], [SocialProviderId], [LastOnline]) VALUES (2034, N'ynaka5678@gmail.com', 1, N'cba8891e98dbe6a4c11e00ca189548fb', NULL, NULL, 0, 0, NULL, 0, 0, N'ynaka5678@gmail.com', CAST(N'2019-11-12T13:06:52.350' AS DateTime), NULL, NULL, NULL, NULL, N'OTPSMS', CAST(N'2019-11-12T13:06:52.350' AS DateTime), 0, NULL, NULL, NULL, 0, CAST(N'2019-11-12T13:06:52.350' AS DateTime))
INSERT [dbo].[tbl_users] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName], [CreatedDateUtc], [PasswordHash2], [FullName], [DisplayName], [Avatar], [OTPType], [Birthday], [Sex], [Address], [IDCard], [Note], [SocialProviderId], [LastOnline]) VALUES (2035, N'prince.of.sun.96@gmail.com', 1, N'a10c109915e36770c291dcd3880f8517', NULL, NULL, 0, 0, NULL, 0, 0, N'prince.of.sun.96@gmail.com', CAST(N'2019-11-16T04:31:16.367' AS DateTime), NULL, NULL, NULL, NULL, N'OTPSMS', CAST(N'2019-11-16T04:31:16.370' AS DateTime), 0, NULL, NULL, NULL, 0, CAST(N'2019-11-16T04:31:16.367' AS DateTime))
INSERT [dbo].[tbl_users] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName], [CreatedDateUtc], [PasswordHash2], [FullName], [DisplayName], [Avatar], [OTPType], [Birthday], [Sex], [Address], [IDCard], [Note], [SocialProviderId], [LastOnline]) VALUES (2036, N'huyht@mindx.edu.vn', 1, N'e10adc3949ba59abbe56e057f20f883e', NULL, NULL, 0, 0, NULL, 0, 0, N'huyht@mindx.edu.vn', CAST(N'2019-11-16T04:33:58.513' AS DateTime), NULL, NULL, NULL, NULL, N'OTPSMS', CAST(N'2019-11-16T04:33:58.513' AS DateTime), 0, NULL, NULL, NULL, 0, CAST(N'2019-11-16T04:33:58.513' AS DateTime))
INSERT [dbo].[tbl_users] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName], [CreatedDateUtc], [PasswordHash2], [FullName], [DisplayName], [Avatar], [OTPType], [Birthday], [Sex], [Address], [IDCard], [Note], [SocialProviderId], [LastOnline]) VALUES (2037, N'vuluongbang_nd1991@yahoo.com', 0, N'e10adc3949ba59abbe56e057f20f883e', N'b36bb62e04648ce618a529a328a07b70.dnVsdW9uZ2JhbmdfbmQxOTkxQHlhaG9vLmNvbXx2dWx1b25nYmFuZ19uZDE5OTFAeWFob28uY29t.0f86387e25db268abc5e9e7f5d6c0f7d', NULL, 0, 0, NULL, 0, 0, N'vuluongbang_nd1991@yahoo.com', CAST(N'2019-11-16T06:49:09.440' AS DateTime), NULL, NULL, N'vuluongbang_nd1991@yahoo.com', NULL, N'OTPSMS', CAST(N'2019-11-16T06:49:09.443' AS DateTime), 0, NULL, NULL, NULL, 0, CAST(N'2019-11-16T06:49:09.440' AS DateTime))
INSERT [dbo].[tbl_users] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName], [CreatedDateUtc], [PasswordHash2], [FullName], [DisplayName], [Avatar], [OTPType], [Birthday], [Sex], [Address], [IDCard], [Note], [SocialProviderId], [LastOnline]) VALUES (2038, N'11141768@st.neu.edu.vn', 1, N'a10c109915e36770c291dcd3880f8517', NULL, NULL, 0, 0, NULL, 0, 0, N'11141768@st.neu.edu.vn', CAST(N'2019-11-22T02:39:21.190' AS DateTime), NULL, NULL, N'11141768@st.neu.edu.vn', NULL, N'OTPSMS', CAST(N'2019-11-22T02:39:21.193' AS DateTime), 0, NULL, NULL, NULL, 0, CAST(N'2019-11-22T02:39:21.190' AS DateTime))
SET IDENTITY_INSERT [dbo].[tbl_users] OFF
/****** Object:  Index [aspnetuserclaims$Id]    Script Date: 11/22/2019 10:12:44 AM ******/
ALTER TABLE [dbo].[tbl_user_claims] ADD  CONSTRAINT [aspnetuserclaims$Id] UNIQUE NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[tbl_accesses] ADD  CONSTRAINT [DF__aspnetacc__Activ__0F975522]  DEFAULT ((1)) FOR [Active]
GO
ALTER TABLE [dbo].[tbl_accesses] ADD  CONSTRAINT [DF__aspnetacc__Descr__108B795B]  DEFAULT (NULL) FOR [Description]
GO
ALTER TABLE [dbo].[tbl_activitylogs] ADD  DEFAULT (NULL) FOR [TargetType]
GO
ALTER TABLE [dbo].[tbl_activitylogs] ADD  DEFAULT (NULL) FOR [TargetId]
GO
ALTER TABLE [dbo].[tbl_activitylogs] ADD  DEFAULT (NULL) FOR [IPAddress]
GO
ALTER TABLE [dbo].[tbl_activitylogs] ADD  DEFAULT (getdate()) FOR [ActivityDate]
GO
ALTER TABLE [dbo].[tbl_activitylogs] ADD  DEFAULT (NULL) FOR [ActivityType]
GO
ALTER TABLE [dbo].[tbl_categories] ADD  CONSTRAINT [DF_tbl_category_Status]  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[tbl_cmn_settings] ADD  DEFAULT (NULL) FOR [SettingValue]
GO
ALTER TABLE [dbo].[tbl_log4netfiles] ADD  DEFAULT (NULL) FOR [DateCreated]
GO
ALTER TABLE [dbo].[tbl_log4netfiles] ADD  DEFAULT (NULL) FOR [CurrentItem]
GO
ALTER TABLE [dbo].[tbl_log4netfiles] ADD  DEFAULT (NULL) FOR [FileStatus]
GO
ALTER TABLE [dbo].[tbl_log4netfiles] ADD  DEFAULT (NULL) FOR [MachineName]
GO
ALTER TABLE [dbo].[tbl_log4netfiles] ADD  DEFAULT (NULL) FOR [MachineIP]
GO
ALTER TABLE [dbo].[tbl_log4netfiles] ADD  DEFAULT (NULL) FOR [LastUpdated]
GO
ALTER TABLE [dbo].[tbl_log4netfiles] ADD  DEFAULT (NULL) FOR [AppName]
GO
ALTER TABLE [dbo].[tbl_log4netrecords] ADD  DEFAULT (NULL) FOR [Item]
GO
ALTER TABLE [dbo].[tbl_log4netrecords] ADD  DEFAULT (NULL) FOR [TimeStamp]
GO
ALTER TABLE [dbo].[tbl_log4netrecords] ADD  DEFAULT (NULL) FOR [Level]
GO
ALTER TABLE [dbo].[tbl_log4netrecords] ADD  DEFAULT (NULL) FOR [Thread]
GO
ALTER TABLE [dbo].[tbl_log4netrecords] ADD  DEFAULT (NULL) FOR [MachineName]
GO
ALTER TABLE [dbo].[tbl_log4netrecords] ADD  DEFAULT (NULL) FOR [UserName]
GO
ALTER TABLE [dbo].[tbl_log4netrecords] ADD  DEFAULT (NULL) FOR [HostName]
GO
ALTER TABLE [dbo].[tbl_log4netrecords] ADD  DEFAULT (NULL) FOR [App]
GO
ALTER TABLE [dbo].[tbl_log4netrecords] ADD  DEFAULT (NULL) FOR [Class]
GO
ALTER TABLE [dbo].[tbl_log4netrecords] ADD  DEFAULT (NULL) FOR [Method]
GO
ALTER TABLE [dbo].[tbl_log4netrecords] ADD  DEFAULT (NULL) FOR [Line]
GO
ALTER TABLE [dbo].[tbl_log4netrecords] ADD  DEFAULT (NULL) FOR [DateCreated]
GO
ALTER TABLE [dbo].[tbl_log4netrecords] ADD  DEFAULT (NULL) FOR [FileId]
GO
ALTER TABLE [dbo].[tbl_log4netrecords_exceptions] ADD  DEFAULT (NULL) FOR [TimeStamp]
GO
ALTER TABLE [dbo].[tbl_log4netrecords_exceptions] ADD  DEFAULT (NULL) FOR [MachineName]
GO
ALTER TABLE [dbo].[tbl_log4netrecords_exceptions] ADD  DEFAULT (NULL) FOR [HostName]
GO
ALTER TABLE [dbo].[tbl_log4netrecords_exceptions] ADD  DEFAULT (NULL) FOR [AppName]
GO
ALTER TABLE [dbo].[tbl_system_emails] ADD  CONSTRAINT [DF_tbl_system_emails_ReceiverId]  DEFAULT ((0)) FOR [ReceiverId]
GO
ALTER TABLE [dbo].[tbl_system_emails] ADD  CONSTRAINT [DF_tbl_system_emails_IsSent]  DEFAULT ((0)) FOR [IsSent]
GO
ALTER TABLE [dbo].[tbl_system_emails] ADD  CONSTRAINT [DF_tbl_system_emails_IsRead]  DEFAULT ((0)) FOR [IsRead]
GO
ALTER TABLE [dbo].[tbl_system_emails] ADD  CONSTRAINT [DF_tbl_system_emails_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[tbl_user_actions] ADD  CONSTRAINT [DF_tbl_user_actions_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[tbl_user_actions] ADD  CONSTRAINT [DF_tbl_user_actions_ModifiedDate]  DEFAULT (getdate()) FOR [ModifiedDate]
GO
ALTER TABLE [dbo].[tbl_user_codes] ADD  CONSTRAINT [DF_aspnetusercodes_IsUsed]  DEFAULT ((0)) FOR [IsUsed]
GO
ALTER TABLE [dbo].[tbl_user_codes_history] ADD  CONSTRAINT [DF_aspnetusercodes_history_IsUsed]  DEFAULT ((0)) FOR [IsUsed]
GO
ALTER TABLE [dbo].[tbl_user_data] ADD  CONSTRAINT [DF_tbl_user_data_FollowingCount]  DEFAULT ((0)) FOR [FollowingCount]
GO
ALTER TABLE [dbo].[tbl_user_data] ADD  CONSTRAINT [DF_tbl_user_data_MessageCount]  DEFAULT ((0)) FOR [MessageCount]
GO
ALTER TABLE [dbo].[tbl_user_data] ADD  CONSTRAINT [DF_tbl_user_data_LikePostCount]  DEFAULT ((0)) FOR [LikePostCount]
GO
ALTER TABLE [dbo].[tbl_user_data] ADD  CONSTRAINT [DF_tbl_user_data_PhotoCount]  DEFAULT ((0)) FOR [PhotoCount]
GO
ALTER TABLE [dbo].[tbl_user_data] ADD  CONSTRAINT [DF_tbl_user_data_FollowerCount]  DEFAULT ((0)) FOR [FollowerCount]
GO
ALTER TABLE [dbo].[tbl_user_data] ADD  CONSTRAINT [DF_tbl_user_data_PostCount]  DEFAULT ((0)) FOR [PostCount]
GO
ALTER TABLE [dbo].[tbl_user_devices] ADD  CONSTRAINT [DF_tbl_user_devices_UserId]  DEFAULT ((0)) FOR [UserId]
GO
ALTER TABLE [dbo].[tbl_user_devices] ADD  CONSTRAINT [DF_tbl_user_devices_iosDevice]  DEFAULT ((0)) FOR [iosDevice]
GO
ALTER TABLE [dbo].[tbl_user_devices] ADD  CONSTRAINT [DF_tbl_user_devices_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[tbl_user_devices] ADD  CONSTRAINT [DF_tbl_user_devices_Status]  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[tbl_user_devices] ADD  CONSTRAINT [DF_tbl_user_devices_LastConnected]  DEFAULT (getdate()) FOR [LastConnected]
GO
ALTER TABLE [dbo].[tbl_user_devices] ADD  CONSTRAINT [DF_tbl_user_devices_LangCode]  DEFAULT (N'vi-VN') FOR [LangCode]
GO
ALTER TABLE [dbo].[tbl_user_otpactions] ADD  CONSTRAINT [DF_aspnetoptaction_IsDone]  DEFAULT ((0)) FOR [IsDone]
GO
ALTER TABLE [dbo].[tbl_users] ADD  CONSTRAINT [DF__aspnetuse__Email__2E1BDC42]  DEFAULT (NULL) FOR [Email]
GO
ALTER TABLE [dbo].[tbl_users] ADD  CONSTRAINT [DF_aspnetusers_EmailConfirmed]  DEFAULT ((0)) FOR [EmailConfirmed]
GO
ALTER TABLE [dbo].[tbl_users] ADD  CONSTRAINT [DF_aspnetusers_PhoneNumberConfirmed]  DEFAULT ((0)) FOR [PhoneNumberConfirmed]
GO
ALTER TABLE [dbo].[tbl_users] ADD  CONSTRAINT [DF_aspnetusers_TwoFactorEnabled]  DEFAULT ((0)) FOR [TwoFactorEnabled]
GO
ALTER TABLE [dbo].[tbl_users] ADD  CONSTRAINT [DF__aspnetuse__Locko__2F10007B]  DEFAULT (NULL) FOR [LockoutEndDateUtc]
GO
ALTER TABLE [dbo].[tbl_users] ADD  CONSTRAINT [DF_aspnetusers_LockoutEnabled]  DEFAULT ((0)) FOR [LockoutEnabled]
GO
ALTER TABLE [dbo].[tbl_users] ADD  CONSTRAINT [DF_aspnetusers_AccessFailedCount]  DEFAULT ((0)) FOR [AccessFailedCount]
GO
ALTER TABLE [dbo].[tbl_users] ADD  CONSTRAINT [DF__aspnetuse__Creat__300424B4]  DEFAULT (NULL) FOR [CreatedDateUtc]
GO
ALTER TABLE [dbo].[tbl_users] ADD  CONSTRAINT [DF_tbl_users_Sex]  DEFAULT ((1)) FOR [Sex]
GO
ALTER TABLE [dbo].[tbl_users] ADD  CONSTRAINT [DF_tbl_users_SocialProviderId]  DEFAULT ((0)) FOR [SocialProviderId]
GO
ALTER TABLE [dbo].[tbl_users] ADD  CONSTRAINT [DF_tbl_users_LastOnline]  DEFAULT (getdate()) FOR [LastOnline]
GO
ALTER TABLE [dbo].[tbl_user_roles]  WITH NOCHECK ADD  CONSTRAINT [aspnetuserroles$IdentityRole_Users] FOREIGN KEY([RoleId])
REFERENCES [dbo].[tbl_roles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[tbl_user_roles] CHECK CONSTRAINT [aspnetuserroles$IdentityRole_Users]
GO
/****** Object:  StoredProcedure [dbo].[Document_Api_Insert]    Script Date: 11/22/2019 10:12:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[Document_Api_Insert]
@LinkUrl nvarchar(500),
@Data ntext
AS
BEGIN
	IF(Exists (SELECT * FROM tbl_document_api WHERE LinkUrl=@LinkUrl))
	BEGIN
		UPDATE tbl_document_api SET Data=@Data WHERE LinkUrl=@LinkUrl
	END
	ELSE
	BEGIN
		INSERT tbl_document_api(LinkUrl,[Data]) Values(@LinkUrl,@Data)
	END
END
GO
/****** Object:  StoredProcedure [dbo].[DocumentApi_GetByLinkUrl]    Script Date: 11/22/2019 10:12:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[DocumentApi_GetByLinkUrl]
@LinkUrl nvarchar(128)
AS
BEGIN
SELECT Top 1 * FROM tbl_document_api WHERE LinkUrl like '%'+@LinkUrl
END
GO
/****** Object:  StoredProcedure [dbo].[Log4net_AutoFlush]    Script Date: 11/22/2019 10:12:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*
*   SSMA informational messages:
*   M2SS0003: The following SQL clause was ignored during conversion:
*   DEFINER = `kplus`@`%`.
*/

CREATE PROCEDURE [dbo].[Log4net_AutoFlush]  
   @p_Daynum int
AS 
   BEGIN

      SET  XACT_ABORT  ON

      SET  NOCOUNT  ON

      /*
      *   SSMA informational messages:
      *   M2SS0134: Conversion of following Comment(s) is not supported :  select subdate(now(),interval p_Daynum day);
      *
      */

      DELETE 
      FROM dbo.log4netfiles
      WHERE 1 = 1 AND log4netfiles.DateCreated <= dateadd(day, -(@p_Daynum), CAST(getdate() AS datetime2))

   END
GO
/****** Object:  StoredProcedure [dbo].[Log4net_AutoGenFiles]    Script Date: 11/22/2019 10:12:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*
*   SSMA informational messages:
*   M2SS0003: The following SQL clause was ignored during conversion:
*   DEFINER = `kplus`@`%`.
*/

CREATE PROCEDURE [dbo].[Log4net_AutoGenFiles]
AS 
   BEGIN

      SET  XACT_ABORT  ON

      SET  NOCOUNT  ON

      DECLARE
         @done int 
            /*
            *   SSMA informational messages:
            *   M2SS0052: BOOLEAN literal was converted to INT literal
            */
= 0

      DECLARE
         @v_EntryId nvarchar(128)

      DECLARE
         @v_TimeStamp datetime2(0)

      DECLARE
         @v_Message nvarchar(max)

      DECLARE
         @v_MachineName nvarchar(200)

      DECLARE
         @v_HostName nvarchar(200)

      DECLARE
         @v_AppName nvarchar(200)

      DECLARE
         @v_Throwable nvarchar(max)

      DECLARE
         @v_Body nvarchar(max)

      DECLARE
         @hasData int

      SELECT TOP (1) @hasData = count_big(1)
      FROM dbo.log4netrecords_exceptions  AS a

      /*
      *   SSMA informational messages:
      *   M2SS0134: Conversion of following Comment(s) is not supported :  SELECT v_EntryId;
      *   
      *   	SET @receivers = "vlbang@vstv.vn";
      *   	SET @sender = "thuan.tran@vstv.vn";
      *   	SET @subject = "We have received a new exception";
      *   	SET @fileName = CONCAT('C:\\\\Batchs\\\\OTTManager\\\\LogsException\\\\log_',v_EntryId,'.txt');
      *   		
      *   	
      *   	-- Append receivers
      *   	SET @finalQuery = CONCAT('SELECT "To: ',@receivers,'"',',');
      *   	-- Append sender
      *   	SET @finalQuery = CONCAT(@finalQuery,'"From: ',@sender,'"',',');
      *   	-- Append subject
      *   	SET @finalQuery = CONCAT(@finalQuery,'"Subject: ',@subject,'"',',');
      *   		
      *   	-- Append body
      *   	SET @finalQuery = CONCAT(@finalQuery,'HTMLBody("',v_Body,'")');
      *   	
      *   	
      *
      */

      IF @hasData > 0
         BEGIN

            DECLARE
                logs_data CURSOR LOCAL FORWARD_ONLY FOR 
                  /*
                  *   SSMA informational messages:
                  *   M2SS0134: Conversion of following Comment(s) is not supported :  Get all logs exception
                  *
                  */

                  SELECT TOP (1) 
                     a.EntryId, 
                     a.TimeStamp, 
                     a.Message, 
                     a.MachineName, 
                     a.HostName, 
                     a.AppName, 
                     a.Throwable
                  FROM dbo.log4netrecords_exceptions  AS a
                  WHERE 1 = 1
                     ORDER BY a.TimeStamp DESC

            OPEN logs_data

            /*
            *   SSMA informational messages:
            *   M2SS0003: The following SQL clause was ignored during conversion:
            *   logs_loop : .
            */

            WHILE (1 = 1)
            
               BEGIN

                  FETCH logs_data
                      INTO 
                        @v_EntryId, 
                        @v_TimeStamp, 
                        @v_Message, 
                        @v_MachineName, 
                        @v_HostName, 
                        @v_AppName, 
                        @v_Throwable

                  IF @@FETCH_STATUS <> 0
                     /*
                     *   SSMA informational messages:
                     *   M2SS0052: BOOLEAN literal was converted to INT literal
                     */

                     SET @done = 1

                  IF @done <> 0
                     BREAK

                  SET @v_Body = N'Entry: ' + @v_EntryId + N'\'+NCHAR(13)+N'\'+NCHAR(10)

                  SET @v_Body = @v_Body + N'MachineName: ' + @v_MachineName + N'\'+NCHAR(13)+N'\'+NCHAR(10)

                  SET @v_Body = @v_Body + N'MachineIP: ' + @v_HostName + N'\'+NCHAR(13)+N'\'+NCHAR(10)

                  SET @v_Body = @v_Body + N'AppName: ' + @v_AppName + N'\'+NCHAR(13)+N'\'+NCHAR(10)

                  SET @v_Body = @v_Body + N'TimeStamp: ' + CONVERT(varchar(20), @v_TimeStamp, 120) + N'\'+NCHAR(13)+N'\'+NCHAR(10)

                  SET @v_Body = @v_Body + N'Message: ' + @v_Message + N'\'+NCHAR(13)+N'\'+NCHAR(10)

                  SET @v_Body = @v_Body + N'Throwable (Exception): ' + @v_Throwable + N'\'+NCHAR(13)+N'\'+NCHAR(10)

               END

            CLOSE logs_data

            DEALLOCATE logs_data

            /*
            *   SSMA informational messages:
            *   M2SS0134: Conversion of following Comment(s) is not supported :  Delete after reading data
            *
            */

            DELETE 
            FROM dbo.log4netrecords_exceptions
            WHERE 1 = 1 AND log4netrecords_exceptions.EntryId = @v_EntryId

            /* 
            *   SSMA error messages:
            *   M2SS0198: SSMA for MySQL  does not support user variables conversion

            SET [@fileName] = (N'C:\\Batchs\\OTTManagerDB\\Log4netException\\Logs\\log_') + (@v_EntryId) + (N'.txt')
            */



            /* 
            *   SSMA error messages:
            *   M2SS0198: SSMA for MySQL  does not support user variables conversion

            SET [@finalQuery] = (N'SELECT ') + (N'''') + (@v_Body) + (N'''')
            */



            /* 
            *   SSMA error messages:
            *   M2SS0198: SSMA for MySQL  does not support user variables conversion
            *   M2SS0198: SSMA for MySQL  does not support user variables conversion
            *   M2SS0198: SSMA for MySQL  does not support user variables conversion

            SET [@finalQuery] = ([@finalQuery]) + (N' INTO OUTFILE ') + (N'''') + ([@fileName]) + (N'''')
            */



            /* 
            *   SSMA error messages:
            *   M2SS0198: SSMA for MySQL  does not support user variables conversion
            *   M2SS0198: SSMA for MySQL  does not support user variables conversion

            SET [@finalQuery] = ([@finalQuery]) + (N' FIELDS TERMINATED BY "\'+NCHAR(13)+N'\'+NCHAR(10)+N'" ESCAPED BY ""')
            */



            /* 
            *   SSMA error messages:
            *   M2SS0082: SSMA for MySQL does not support conversion of this statement  
            *   / * select @finalQuery;* / 
            *   PREPARE stmt
            *      FROM @finalQuery

            DECLARE
               @db_null_statement int
            */



            /* 
            *   SSMA error messages:
            *   M2SS0082: SSMA for MySQL does not support conversion of this statement  
            *   EXECUTE stmt

            DECLARE
               @db_null_statement$2 int
            */



            /* 
            *   SSMA error messages:
            *   M2SS0082: SSMA for MySQL does not support conversion of this statement   DROP PREPARE stmt

            DECLARE
               @db_null_statement$3 int
            */



         END

   END
GO
/****** Object:  StoredProcedure [dbo].[Log4net_GetNewestRecords]    Script Date: 11/22/2019 10:12:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*
*   SSMA informational messages:
*   M2SS0003: The following SQL clause was ignored during conversion:
*   DEFINER = `root`@`localhost`.
*/

CREATE PROCEDURE [dbo].[Log4net_GetNewestRecords]  
   @p_Level nvarchar(50),
   @p_MaxId nvarchar(128),
   @p_pagesize int,
   @p_offset int
AS 
   BEGIN

      SET  XACT_ABORT  ON

      SET  NOCOUNT  ON

      /* 
      *   SSMA error messages:
      *   M2SS0198: SSMA for MySQL  does not support user variables conversion

      SET [@queryTable] = N'SELECT a.*,b.AppName FROM (SELECT * FROM  log4netrecords a WHERE 1=1 '
      */



      /* 
      *   SSMA error messages:
      *   M2SS0198: SSMA for MySQL  does not support user variables conversion

      SET [@MaxIdFilter] = N''
      */



      IF ((1 <> 1))
         /* 
         *   SSMA error messages:
         *   M2SS0198: SSMA for MySQL  does not support user variables conversion

         SET [@MaxIdFilter] = (N' AND a.EntryId <>') + (N'''') + (@p_MaxId) + (N'''')
         */


         DECLARE
            @db_null_statement int

      /* 
      *   SSMA error messages:
      *   M2SS0198: SSMA for MySQL  does not support user variables conversion

      SET [@LevelFilter] = (N' AND FIND_IN_SET(a.Level, ''') + (@p_Level) + (N''')')
      */



      /* 
      *   SSMA error messages:
      *   M2SS0198: SSMA for MySQL  does not support user variables conversion
      *   M2SS0198: SSMA for MySQL  does not support user variables conversion

      SET [@LevelFilter] = ([@LevelFilter]) + (N' ORDER BY a.TimeStamp DESC')
      */



      /* 
      *   SSMA error messages:
      *   M2SS0198: SSMA for MySQL  does not support user variables conversion

      SET [@limitFilter] = N' LIMIT ? OFFSET ? ) a'
      */



      /* 
      *   SSMA error messages:
      *   M2SS0198: SSMA for MySQL  does not support user variables conversion

      SET [@joinTable] = N' LEFT JOIN log4netfiles b ON a.FileId = b.FileId'
      */



      /* 
      *   SSMA error messages:
      *   M2SS0198: SSMA for MySQL  does not support user variables conversion
      *   M2SS0198: SSMA for MySQL  does not support user variables conversion
      *   M2SS0198: SSMA for MySQL  does not support user variables conversion
      *   M2SS0198: SSMA for MySQL  does not support user variables conversion
      *   M2SS0198: SSMA for MySQL  does not support user variables conversion
      *   M2SS0198: SSMA for MySQL  does not support user variables conversion

      SET [@queryTable] = ([@queryTable]) + ([@MaxIdFilter]) + ([@LevelFilter]) + ([@limitFilter]) + ([@joinTable])
      */



      /* 
      *   SSMA error messages:
      *   M2SS0198: SSMA for MySQL  does not support user variables conversion

      SET [@v_limit] = @p_pagesize
      */



      /* 
      *   SSMA error messages:
      *   M2SS0198: SSMA for MySQL  does not support user variables conversion

      SET [@v_offset] = @p_offset
      */



      /* 
      *   SSMA error messages:
      *   M2SS0082: SSMA for MySQL does not support conversion of this statement  
      *   PREPARE stmt
      *      FROM @queryTable

      DECLARE
         @db_null_statement$2 int
      */



      /* 
      *   SSMA error messages:
      *   M2SS0082: SSMA for MySQL does not support conversion of this statement  
      *   EXECUTE stmt
      *       USING @v_limit, @v_offset

      DECLARE
         @db_null_statement$3 int
      */



      /* 
      *   SSMA error messages:
      *   M2SS0082: SSMA for MySQL does not support conversion of this statement   DROP PREPARE stmt

      DECLARE
         @db_null_statement$4 int
      */



   END
GO
/****** Object:  StoredProcedure [dbo].[Log4net_GetRecordDetails]    Script Date: 11/22/2019 10:12:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*
*   SSMA informational messages:
*   M2SS0003: The following SQL clause was ignored during conversion:
*   DEFINER = `root`@`localhost`.
*/

CREATE PROCEDURE [dbo].[Log4net_GetRecordDetails]  
   @p_Id nvarchar(128)
AS 
   BEGIN

      SET  XACT_ABORT  ON

      SET  NOCOUNT  ON

      /* 
      *   SSMA error messages:
      *   M2SS0198: SSMA for MySQL  does not support user variables conversion

      SET [@FileId] = N''
      */



      /* 
      *   SSMA error messages:
      *   M2SS0084: The INTO clause cannot be converted in the current context. 
      *   INTO @FileId

      SELECT a.FileId
      FROM dbo.log4netrecords  AS a
      WHERE 1 = 1 AND a.EntryId = @p_Id
      */



      SELECT 
         a.EntryId, 
         a.Item, 
         a.TimeStamp, 
         a.Level, 
         a.Thread, 
         a.Message, 
         a.MachineName, 
         a.UserName, 
         a.HostName, 
         a.App, 
         a.Throwable, 
         a.Class, 
         a.Method, 
         a.[File], 
         a.Line, 
         a.DateCreated, 
         a.LogPath, 
         a.FileId
      FROM dbo.log4netrecords  AS a
      WHERE 1 = 1 AND a.EntryId = @p_Id

      /* 
      *   SSMA error messages:
      *   M2SS0198: SSMA for MySQL  does not support user variables conversion

      SELECT 
         a.FileId, 
         a.FolderPath, 
         a.FileName, 
         a.FullPath, 
         a.DateCreated, 
         a.CurrentItem, 
         a.FileStatus, 
         a.MachineName, 
         a.MachineIP, 
         a.LastUpdated, 
         a.Comments, 
         a.AppName
      FROM dbo.log4netfiles  AS a
      WHERE 1 = 1 AND CAST(a.FileId AS float(53)) = [@FileId]
      */



   END
GO
/****** Object:  StoredProcedure [dbo].[Log4net_GetRecordsPaging]    Script Date: 11/22/2019 10:12:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*
*   SSMA informational messages:
*   M2SS0003: The following SQL clause was ignored during conversion:
*   DEFINER = `root`@`localhost`.
*/

/*
*   SSMA informational messages:
*   M2SS0134: Conversion of following Comment(s) is not supported :  Build filter condition
*
*/

CREATE PROCEDURE [dbo].[Log4net_GetRecordsPaging]  
   @p_Level nvarchar(50),
   @p_AppName nvarchar(50),
   @p_MachineName nvarchar(50),
   @p_Message nvarchar(max),
   @p_Throwable nvarchar(max),
   @p_FromDate nvarchar(50),
   @p_ToDate nvarchar(50),
   @p_HaveException nvarchar(1),
   @p_pagesize int,
   @p_offset int
AS 
   BEGIN

      SET  XACT_ABORT  ON

      SET  NOCOUNT  ON

      /*
      *   SSMA warning messages:
      *   M2SS0172: The following SQL clause was ignored during conversion:
      *   SET  TRANSACTION ISOLATION LEVEL  READ UNCOMMITTED.
      */

      DECLARE
         @db_null_statement int

      /* 
      *   SSMA error messages:
      *   M2SS0198: SSMA for MySQL  does not support user variables conversion

      SET [@LevelFilter] = N''
      */



      IF (@p_Level IS NOT NULL AND @p_Level <> '')
         /* 
         *   SSMA error messages:
         *   M2SS0198: SSMA for MySQL  does not support user variables conversion

         SET [@LevelFilter] = (N' AND FIND_IN_SET(a.Level, ''') + (@p_Level) + (N''')')
         */


         DECLARE
            @db_null_statement$2 int

      /* 
      *   SSMA error messages:
      *   M2SS0198: SSMA for MySQL  does not support user variables conversion

      SET [@MachineNameFilter] = N''
      */



      IF (@p_MachineName IS NOT NULL AND @p_MachineName <> '')
         /* 
         *   SSMA error messages:
         *   M2SS0198: SSMA for MySQL  does not support user variables conversion

         SET [@MachineNameFilter] = (N' AND a.MachineName LIKE ''%') + (@p_MachineName) + (N'%''')
         */


         DECLARE
            @db_null_statement$3 int

      /* 
      *   SSMA error messages:
      *   M2SS0198: SSMA for MySQL  does not support user variables conversion

      SET [@MessageFilter] = N''
      */



      IF (@p_Message IS NOT NULL AND @p_Message <> '')
         /* 
         *   SSMA error messages:
         *   M2SS0198: SSMA for MySQL  does not support user variables conversion

         SET [@MessageFilter] = (N' AND a.Message LIKE ''%') + (@p_Message) + (N'%''')
         */


         DECLARE
            @db_null_statement$4 int

      /* 
      *   SSMA error messages:
      *   M2SS0198: SSMA for MySQL  does not support user variables conversion

      SET [@HaveExceptionFilter] = N''
      */



      IF (@p_HaveException IS NOT NULL AND @p_HaveException <> '')
         /* 
         *   SSMA error messages:
         *   M2SS0198: SSMA for MySQL  does not support user variables conversion

         SET [@HaveExceptionFilter] = (N' AND LENGTH(a.Throwable) > 0')
         */


         DECLARE
            @db_null_statement$5 int

      /* 
      *   SSMA error messages:
      *   M2SS0198: SSMA for MySQL  does not support user variables conversion

      SET [@ThrowableFilter] = N''
      */



      IF (@p_Throwable IS NOT NULL AND @p_Throwable <> '')
         /* 
         *   SSMA error messages:
         *   M2SS0198: SSMA for MySQL  does not support user variables conversion

         SET [@ThrowableFilter] = (N' AND a.Throwable LIKE ''%') + (@p_Throwable) + (N'%''')
         */


         DECLARE
            @db_null_statement$6 int

      /* 
      *   SSMA error messages:
      *   M2SS0198: SSMA for MySQL  does not support user variables conversion

      SET [@TimeStampFilter] = N''
      */



      /*
      *   SSMA informational messages:
      *   M2SS0134: Conversion of following Comment(s) is not supported : 
      *   	IF (p_FromDate IS NOT NULL) THEN	
      *   	    if (p_ToDate is NOT NULL) then
      *   		-- SET @TimeStampFilter = CONCAT(' AND DATE(a.TimeStamp) BETWEEN ','''', DATE(STR_TO_DATE(p_FromDate,'%Y-%m-%d')),'''', ' AND ''',DATE(STR_TO_DATE(p_ToDate,'%Y-%m-%d')),'''');
      *   		SET @TimeStampFilter = 'AND (a.TimeStamp BETWEEN p_FromDate AND p_ToDate)';
      *   	    else
      *   		-- SET @TimeStampFilter = CONCAT(' AND DATE(a.TimeStamp) BETWEEN ','''', DATE(STR_TO_DATE(p_FromDate,'%Y-%m-%d')),'''', ' AND NOW()');		
      *   		SET @TimeStampFilter = 'AND (a.TimeStamp BETWEEN p_FromDate AND NOW())';		
      *   	    end if;
      *   	END IF;	
      *   	
      *
      */

      IF (@p_FromDate IS NOT NULL AND @p_ToDate IS NOT NULL)
         /* 
         *   SSMA error messages:
         *   M2SS0198: SSMA for MySQL  does not support user variables conversion

         SET [@TimeStampFilter] = 
            (N' AND (a.TimeStamp BETWEEN ')
             + 
            (N'''')
             + 
            (@p_FromDate)
             + 
            (N'''')
             + 
            (N' AND ')
             + 
            (N'''')
             + 
            (@p_ToDate)
             + 
            (N''')')
         */


         DECLARE
            @db_null_statement$7 int
      ELSE 
         IF (@p_FromDate IS NULL AND @p_ToDate IS NULL)
            /* 
            *   SSMA error messages:
            *   M2SS0198: SSMA for MySQL  does not support user variables conversion

            SET [@TimeStampFilter] = N''
            */


            DECLARE
               @db_null_statement$8 int
         ELSE 
            IF (@p_FromDate IS NOT NULL)
               /* 
               *   SSMA error messages:
               *   M2SS0198: SSMA for MySQL  does not support user variables conversion

               SET [@TimeStampFilter] = (N' AND a.TimeStamp >= ') + (N'''') + (@p_FromDate) + (N'''')
               */


               DECLARE
                  @db_null_statement$9 int
            ELSE 
               /* 
               *   SSMA error messages:
               *   M2SS0198: SSMA for MySQL  does not support user variables conversion

               SET [@TimeStampFilter] = (N' AND a.TimeStamp <= ') + (N'''') + (@p_ToDate) + (N'''')
               */


               DECLARE
                  @db_null_statement$10 int

      /* 
      *   SSMA error messages:
      *   M2SS0198: SSMA for MySQL  does not support user variables conversion

      SET [@queryTable] = N' SELECT a.*,b.AppName FROM (SELECT * FROM  log4netrecords a WHERE 1=1'
      */



      /* 
      *   SSMA error messages:
      *   M2SS0198: SSMA for MySQL  does not support user variables conversion

      SET [@orderFilter] = N' ORDER BY TimeStamp DESC'
      */



      /* 
      *   SSMA error messages:
      *   M2SS0198: SSMA for MySQL  does not support user variables conversion

      SET [@endQuery] = N') a LEFT JOIN log4netfiles b ON a.FileId = b.FileId'
      */



      /* 
      *   SSMA error messages:
      *   M2SS0198: SSMA for MySQL  does not support user variables conversion
      *   M2SS0198: SSMA for MySQL  does not support user variables conversion
      *   M2SS0198: SSMA for MySQL  does not support user variables conversion
      *   M2SS0198: SSMA for MySQL  does not support user variables conversion
      *   M2SS0198: SSMA for MySQL  does not support user variables conversion
      *   M2SS0198: SSMA for MySQL  does not support user variables conversion
      *   M2SS0198: SSMA for MySQL  does not support user variables conversion
      *   M2SS0198: SSMA for MySQL  does not support user variables conversion
      *   M2SS0198: SSMA for MySQL  does not support user variables conversion

      SET [@queryTable] = 
         ([@queryTable])
          + 
         ([@LevelFilter])
          + 
         ([@MachineNameFilter])
          + 
         ([@MessageFilter])
          + 
         ([@HaveExceptionFilter])
          + 
         ([@ThrowableFilter])
          + 
         ([@TimeStampFilter])
          + 
         ([@endQuery])
      */



      /* 
      *   SSMA error messages:
      *   M2SS0198: SSMA for MySQL  does not support user variables conversion

      SET [@limitFilter] = N' LIMIT ? OFFSET ? '
      */



      /* 
      *   SSMA error messages:
      *   M2SS0198: SSMA for MySQL  does not support user variables conversion

      SET [@AppNameFilter] = N''
      */



      /*
      *   SSMA informational messages:
      *   M2SS0134: Conversion of following Comment(s) is not supported :  SET @AppNameFilter = CONCAT(' AND AppName LIKE CONCAT(''%',p_AppName,'%'')');
      *
      */

      IF (@p_AppName IS NOT NULL AND @p_AppName <> '')
         /* 
         *   SSMA error messages:
         *   M2SS0198: SSMA for MySQL  does not support user variables conversion

         SET [@AppNameFilter] = (N' AND AppName LIKE ''%') + (@p_AppName) + (N'%''')
         */


         DECLARE
            @db_null_statement$11 int

      /* 
      *   SSMA error messages:
      *   M2SS0198: SSMA for MySQL  does not support user variables conversion
      *   M2SS0198: SSMA for MySQL  does not support user variables conversion
      *   M2SS0198: SSMA for MySQL  does not support user variables conversion
      *   M2SS0198: SSMA for MySQL  does not support user variables conversion
      *   M2SS0198: SSMA for MySQL  does not support user variables conversion

      SET [@queryPaging] = 
         (N'SELECT SQL_CALC_FOUND_ROWS * FROM ( ')
          + 
         ([@queryTable])
          + 
         (N' ) usrs WHERE 1=1 ')
          + 
         ([@AppNameFilter])
          + 
         ([@orderFilter])
          + 
         ([@limitFilter])
      */



      /* 
      *   SSMA error messages:
      *   M2SS0198: SSMA for MySQL  does not support user variables conversion

      SET [@v_limit] = @p_pagesize
      */



      /* 
      *   SSMA error messages:
      *   M2SS0198: SSMA for MySQL  does not support user variables conversion

      SET [@v_offset] = @p_offset
      */



      /* 
      *   SSMA error messages:
      *   M2SS0082: SSMA for MySQL does not support conversion of this statement  
      *   / * int offset = (parms.CurrentPage - 1) * parms.PageSize;* / 
      *   PREPARE stmt
      *      FROM @queryPaging

      DECLARE
         @db_null_statement$12 int
      */



      /* 
      *   SSMA error messages:
      *   M2SS0082: SSMA for MySQL does not support conversion of this statement  
      *   EXECUTE stmt
      *       USING @v_limit, @v_offset

      DECLARE
         @db_null_statement$13 int
      */



      /* 
      *   SSMA error messages:
      *   M2SS0198: SSMA for MySQL  does not support user variables conversion
      *   M2SS0201: MySQL standard function FOUND_ROWS is not supported in current SSMA version

      SET [@o_totalcount] = FOUND_ROWS
      */



      /* 
      *   SSMA error messages:
      *   M2SS0198: SSMA for MySQL  does not support user variables conversion

      SELECT [@o_totalcount]
      */



      /* 
      *   SSMA error messages:
      *   M2SS0082: SSMA for MySQL does not support conversion of this statement   DROP PREPARE stmt

      DECLARE
         @db_null_statement$14 int
      */



   END
GO
/****** Object:  StoredProcedure [dbo].[Log4net_InsertLogEntry]    Script Date: 11/22/2019 10:12:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*
*   SSMA informational messages:
*   M2SS0003: The following SQL clause was ignored during conversion:
*   DEFINER = `kplus`@`%`.
*/

CREATE PROCEDURE [dbo].[Log4net_InsertLogEntry]  
   @p_Item int,
   @p_TimeStamp datetime2(0),
   @p_Level nvarchar(15),
   @p_Thread nvarchar(30),
   @p_Message nvarchar(max),
   @p_MachineName nvarchar(200),
   @p_UserName nvarchar(200),
   @p_HostName nvarchar(200),
   @p_App nvarchar(200),
   @p_Throwable nvarchar(max),
   @p_Class nvarchar(500),
   @p_Method nvarchar(100),
   @p_File nvarchar(max),
   @p_Line nvarchar(20),
   @p_LogPath nvarchar(max),
   @p_FileId nvarchar(128)
AS 
   BEGIN

      SET  XACT_ABORT  ON

      SET  NOCOUNT  ON

      IF (@p_Level <> 'DEBUG')
         INSERT dbo.log4netrecords(
            dbo.log4netrecords.EntryId, 
            dbo.log4netrecords.Item, 
            dbo.log4netrecords.TimeStamp, 
            dbo.log4netrecords.Level, 
            dbo.log4netrecords.Thread, 
            dbo.log4netrecords.Message, 
            dbo.log4netrecords.MachineName, 
            dbo.log4netrecords.UserName, 
            dbo.log4netrecords.HostName, 
            dbo.log4netrecords.App, 
            dbo.log4netrecords.Throwable, 
            dbo.log4netrecords.Class, 
            dbo.log4netrecords.Method, 
            dbo.log4netrecords.[File], 
            dbo.log4netrecords.Line, 
            dbo.log4netrecords.DateCreated, 
            dbo.log4netrecords.LogPath, 
            dbo.log4netrecords.FileId)
            VALUES (
               newid(), 
               @p_Item, 
               @p_TimeStamp, 
               @p_Level, 
               @p_Thread, 
               @p_Message, 
               @p_MachineName, 
               @p_UserName, 
               @p_HostName, 
               @p_App, 
               @p_Throwable, 
               @p_Class, 
               @p_Method, 
               @p_File, 
               @p_Line, 
               getdate(), 
               @p_LogPath, 
               @p_FileId)

      UPDATE dbo.log4netfiles
         SET 
            CurrentItem = log4netfiles.CurrentItem + 1, 
            FileStatus = 0, 
            LastUpdated = getdate(), 
            Comments = NULL
      WHERE log4netfiles.FileId = @p_FileId

   END
GO
/****** Object:  StoredProcedure [dbo].[M_Post_GetByPage]    Script Date: 11/22/2019 10:12:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[M_Post_GetByPage]
(
	@Keyword nvarchar(100),
	@Offset int,
	@PageSize int,
	@SortField nvarchar(20),
	@SortType nvarchar(10),
	@Status int
)
AS
BEGIN	
	SELECT TotalCount = COUNT(*) OVER(), a.*, b.Avatar,b.FullName, b.DisplayName, b.UserName , c.Name as CategoryName,  d.Locations, d.Images
	FROM tbl_posts a 
	LEFT JOIN tbl_users b on a.UserId = b.Id
	LEFT JOIN tbl_categories c on a.CategoryId = c.Id
	LEFT JOIN tbl_post_data d on a.Id = d.PostId
	WHERE 1=1 
	AND (
		a.Title like '%'+@Keyword+'%' 
		OR a.ShortDescription like '%'+@Keyword+'%' OR a.Description like '%'+@Keyword+'%'  OR @Keyword IS NULL
		OR b.DisplayName like '%'+@Keyword+'%' 
	)	

	AND a.Status = CASE 
        WHEN @Status != -1
            THEN @Status
		ELSE a.Status
    END 
	AND a.Status != 9
	ORDER BY 
		CASE WHEN @SortField = 'Id' AND @SortType = 'asc' THEN a.Id END ASC,
		CASE WHEN @SortField = 'Id' AND @SortType = 'desc' THEN a.Id END DESC,

		CASE WHEN @SortField = 'Title' AND @SortType = 'asc' THEN a.Title END ASC,
        CASE WHEN @SortField = 'Title' AND @SortType = 'desc' THEN a.Title END DESC,

		CASE WHEN @SortField = 'DisplayName' AND @SortType = 'asc' THEN b.DisplayName END ASC,
		CASE WHEN @SortField = 'DisplayName' AND @SortType = 'desc' THEN b.DisplayName END DESC,

		CASE WHEN @SortField = 'CreatedDate' AND @SortType = 'asc' THEN a.CreatedDate END ASC,
        CASE WHEN @SortField = 'CreatedDate' AND @SortType = 'desc' THEN a.CreatedDate END DESC,

		CASE WHEN @SortField = 'Status' AND @SortType = 'asc' THEN a.Status END ASC,
        CASE WHEN @SortField = 'Status' AND @SortType = 'desc' THEN a.Status END DESC
	OFFSET @Offset ROWS		
	FETCH NEXT @PageSize ROWS ONLY
END 
GO
/****** Object:  StoredProcedure [dbo].[Search_User]    Script Date: 11/22/2019 10:12:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[Search_User]
@Keyword nvarchar(50),
@Offset int,
@PageSize int
AS
BEGIN
	SELECT *  FROM tbl_users WHERE DisplayName like '%'+@Keyword+'%'
	ORDER BY Id
	OFFSET @Offset ROWS		
	FETCH NEXT @PageSize ROWS ONLY
END
GO
/****** Object:  StoredProcedure [dbo].[Settings_LoadSettings]    Script Date: 11/22/2019 10:12:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Settings_LoadSettings]
	@pType nvarchar (50)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT * FROM tbl_cmn_settings t 
	where t.SettingType = @pType
	;
END
GO
/****** Object:  StoredProcedure [dbo].[SQL_WriteLog]    Script Date: 11/22/2019 10:12:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SQL_WriteLog]
AS
BEGIN
	BEGIN TRY
    BEGIN TRANSACTION

		INSERT INTO tbl_cmn_sql_errors (ErrorMessage, ErrorServerity, ErrorState, ErrorLine, Actor, DateOfIssue)
		SELECT 
			ERROR_MESSAGE() AS ErrorMessage,
			ERROR_SEVERITY() AS ErrorSeverity, 									 
			ERROR_STATE() AS ErrorState,  
			ERROR_LINE() AS ErrorLine,
			ERROR_PROCEDURE() AS ErrorProcedure,  
			GETDATE() AS DateOfIssue			
		WHERE 1=1
		;
    COMMIT TRAN -- Transaction Success!
	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0
			ROLLBACK TRAN --RollBack in case of Error
	END CATCH	
END
GO
/****** Object:  StoredProcedure [dbo].[SystemEmail_GetEmailToResend]    Script Date: 11/22/2019 10:12:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SystemEmail_GetEmailToResend]	@Receiver nvarchar(128),	@Action nvarchar(50)AS
BEGIN
	BEGIN TRY	BEGIN TRANSACTION
		SELECT TOP 1 * FROM tbl_system_emails 
		WHERE 1=1
		AND Receiver = @Receiver
		AND Action = @Action
		ORDER BY CreatedDate DESC
		;
	COMMIT TRAN	END TRY	BEGIN CATCH	IF @@TRANCOUNT > 0	ROLLBACK TRAN		exec dbo.SQL_WriteLog;	END CATCH
END

GO
/****** Object:  StoredProcedure [dbo].[SystemEmail_Insert]    Script Date: 11/22/2019 10:12:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SystemEmail_Insert]	@Subject nvarchar(500), 	@Body ntext,	@Sender nvarchar(128),	@Action nvarchar(50),	@Receiver nvarchar(128),	@ReceiverId intAS
BEGIN
	BEGIN TRY	BEGIN TRANSACTION
		INSERT INTO tbl_system_emails (Subject,Body,Sender,Receiver,Action,ReceiverId) VALUES (@Subject,@Body,@Sender,@Receiver,@Action,@ReceiverId);

		SELECT SCOPE_IDENTITY();
	COMMIT TRAN	END TRY	BEGIN CATCH	IF @@TRANCOUNT > 0	ROLLBACK TRAN		exec dbo.SQL_WriteLog;	END CATCH
END

GO
/****** Object:  StoredProcedure [dbo].[SystemEmail_ReceiverRead]    Script Date: 11/22/2019 10:12:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SystemEmail_ReceiverRead]	@Action nvarchar(50),	@UserId intAS
BEGIN
	BEGIN TRY	BEGIN TRANSACTION
		UPDATE tbl_system_emails
		SET IsSent = 1,
			IsRead = 1,
			ReadDate = GETDATE()
		WHERE 1=1 
		AND Action = @Action
		AND ReceiverId = @UserId
		;

		--Save to history
		INSERT INTO tbl_system_emails_history
		SELECT * FROM tbl_system_emails t
		WHERE 1=1
		AND t.Action = @Action
		AND t.ReceiverId = @UserId
		;

		-- Delete old email 
		DELETE FROM tbl_system_emails 
		WHERE 1=1 
		AND Action = @Action
		AND ReceiverId = @UserId
		;
				
	COMMIT TRAN	END TRY	BEGIN CATCH	IF @@TRANCOUNT > 0	ROLLBACK TRAN		exec dbo.SQL_WriteLog;	END CATCH
END

GO
/****** Object:  StoredProcedure [dbo].[SystemEmail_Update]    Script Date: 11/22/2019 10:12:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SystemEmail_Update]	@Id int,	@Subject nvarchar(500), 	@Body ntext,	@Sender nvarchar(128),	@Receiver nvarchar(128),	@ReceiverId intAS
BEGIN
	BEGIN TRY	BEGIN TRANSACTION
		UPDATE tbl_system_emails
		SET Subject = @Subject,
			Body = @Body,
			Sender = @Sender,
			Receiver = @Receiver,
			ReceiverId = @ReceiverId
		WHERE 1=1 
		AND Id = @Id
		;
	COMMIT TRAN	END TRY	BEGIN CATCH	IF @@TRANCOUNT > 0	ROLLBACK TRAN		exec dbo.SQL_WriteLog;	END CATCH
END

GO
/****** Object:  StoredProcedure [dbo].[Test_UsertInsert]    Script Date: 11/22/2019 10:12:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[Test_UsertInsert]
@Row int
AS
BEGIN
DECLARE @count int
SET @Count=1
	While( @count<@Row)
	BEGIN
		Insert into tbl_users(UserName) Values(@count)
		SET @count=@Count+1
	END
END
GO
/****** Object:  StoredProcedure [dbo].[User_ActiveAccountByEmail]    Script Date: 11/22/2019 10:12:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[User_ActiveAccountByEmail]
	-- Add the parameters for the stored procedure here
	@UserName nvarchar(128),
	@HashingData nvarchar(500)
	AS
BEGIN
	DECLARE @UserId int;
	DECLARE @CurrentHashingData nvarchar(500);

	SET @UserId = (SELECT TOP 1 Id
		FROM tbl_users WHERE 1=1
		AND UserName = @UserName
	)	
	;

	IF(@UserId IS NOT NULL)
	BEGIN
		SET @CurrentHashingData = (SELECT TOP 1 SecurityStamp
			FROM tbl_users WHERE 1=1
			AND Id = @UserId
		);

		IF(@CurrentHashingData IS NULL OR @CurrentHashingData = '')
		BEGIN
			--Error_Info_NotFound = -2
			SELECT -2;
			RETURN;
		END

		IF(@HashingData <> @CurrentHashingData)
		BEGIN
			--Error_Info_NotFound = -2
			SELECT -2;
			RETURN;
		END
		ELSE
		BEGIN
			UPDATE tbl_users 
			SET EmailConfirmed = 1,
				SecurityStamp = null 
			WHERE 1=1
			AND Id = @UserId
			;

			EXEC SystemEmail_ReceiverRead 'active_account', @UserId;
				
			-- Success = 1;
			SELECT 1;
			RETURN;
		END
	END
	ELSE
	BEGIN
		--Error_Info_NotFound = -2
		SELECT -2;
		RETURN;
	END
END
GO
/****** Object:  StoredProcedure [dbo].[User_ActiveAccountByOTP]    Script Date: 11/22/2019 10:12:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[User_ActiveAccountByOTP]
	-- Add the parameters for the stored procedure here
	@UserId int
AS
BEGIN
	IF(@UserId IS NOT NULL)
	BEGIN
		UPDATE tbl_users SET PhoneNumberConfirmed = 1 
		WHERE 1=1
		AND Id = @UserId
		;
				
		-- Success = 1;
		SELECT 1;
		RETURN;
	END
	ELSE
	BEGIN
		--Error_Info_NotFound = -2
		SELECT -2;
		RETURN;
	END
END
GO
/****** Object:  StoredProcedure [dbo].[User_ApiGetInfo]    Script Date: 11/22/2019 10:12:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[User_ApiGetInfo]
	-- Add the parameters for the stored procedure here
	@UserId int,
	@TokenKey nvarchar(128)
AS
BEGIN
	SELECT TOP 1 t.*, k.TokenKey, k.ExpiredDate as TokenExpiredDate, k.CreatedDate as TokenCreatedDate
	from tbl_users t 
	left join tbl_user_tokenkeys k on t.Id = k.UserId
	where 1=1	
	and t.Id = @UserId
	--and k.TokenKey = @TokenKey
	ORDER BY k.CreatedDate DESC
	;
END
GO
/****** Object:  StoredProcedure [dbo].[User_ApiRegister]    Script Date: 11/22/2019 10:12:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[User_ApiRegister]
	-- Add the parameters for the stored procedure here
	@UserName nvarchar(128),
	@PasswordHash nvarchar(128),
	@Email nvarchar(256),
	@Birthday datetime,
	@Sex tinyint,
	@Address nvarchar(256),
	@FullName nvarchar(128),
	@DisplayName nvarchar(128),
	@IDCard nvarchar(50),
	@PhoneNumber nvarchar(50),
	@Note nvarchar(1000),
	@OTPType nvarchar(20),
	@SecurityStamp nvarchar(500)
	 
AS
BEGIN
	DECLARE @Checker int;
	DECLARE @newId int;
	SET @Checker = (SELECT dbo.CheckDuplicateUserInfo(@UserName, @Email, @PhoneNumber, @IDCard));

	IF(@Checker = 1)
	BEGIN
		INSERT INTO tbl_users(UserName, PasswordHash, Email, Birthday, Sex, Address, FullName, DisplayName, IDCard, PhoneNumber, Note, OTPType, CreatedDateUtc, SecurityStamp)
		values(@UserName, @PasswordHash, @Email, @Birthday, @Sex, @Address, @FullName, @DisplayName, @IDCard, @PhoneNumber, @Note, @OTPType, GETDATE(),@SecurityStamp)
		;

		SELECT 1;

		SET @newId = (SELECT SCOPE_IDENTITY());

		SELECT @newId;
		RETURN;
	END
	ELSE
	BEGIN
		SELECT @Checker;

		SELECT 0;
		RETURN;
	END
END
GO
/****** Object:  StoredProcedure [dbo].[User_ChangeAuthMethod]    Script Date: 11/22/2019 10:12:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[User_ChangeAuthMethod]
	-- Add the parameters for the stored procedure here
	@UserId int,
	@OTPType nvarchar(20)
AS
BEGIN
	UPDATE tbl_users 
	SET OTPType = @OTPType
	WHERE 1=1
	AND Id = @UserId
	;
END
GO
/****** Object:  StoredProcedure [dbo].[User_ChangePassword]    Script Date: 11/22/2019 10:12:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[User_ChangePassword]
	-- Add the parameters for the stored procedure here
	@UserId int,	
	@NewPassword nvarchar(128),
	@OldPassword nvarchar(128),
	@Level int,
	@TokenKey nvarchar(128)
AS
BEGIN
	DECLARE @Checker int;
	DECLARE @CurrentUser nvarchar(128);
	DECLARE @CurrentPwd nvarchar(128);
	
	SET @Checker = (SELECT dbo.CheckUserToken(@UserId, @TokenKey));
	IF(@Checker > 0)
	BEGIN
		--Begin change password
		IF(@Level = 1)
		BEGIN
			--Change pwd 1
			SET @CurrentPwd = (SELECT TOP 1 t.PasswordHash FROM tbl_users t WHERE 1=1 AND t.Id = @UserId AND t.PasswordHash = @OldPassword)
			IF(@CurrentPwd IS NOT NULL)
			BEGIN
				--The new pwd = current pwd: 111
				IF(@CurrentPwd = @NewPassword)
				BEGIN
					SELECT 111;
					RETURN;
				END
				ELSE
				BEGIN
					--OldPwd1 not matches with current password1 -> Change password
					UPDATE tbl_users SET PasswordHash = @NewPassword WHERE 1=1 AND Id = @UserId;
					SELECT 1;
					RETURN;
				END
			END
			ELSE
			BEGIN
				--OldPwd1 doesn't match with current password1: 109
				SELECT 109;
				RETURN;
				--UPDATE tbl_users SET PasswordHash = @NewPassword WHERE 1=1 AND Id = @UserId;
				--SELECT 1;
				--RETURN;
			END
		END
		
		--Begin change password2
		IF(@Level = 2)
		BEGIN			

			-- Get CurrentUser and CurrentPwd2
			SELECT TOP 1 
				@CurrentUser = t.Id,
				@CurrentPwd = t.PasswordHash2
			FROM tbl_users t 
			WHERE 1=1 
			AND t.Id = @UserId
			;
			--Change pwd 2
			IF(@CurrentPwd IS NOT NULL)
			BEGIN
				--The old pwd != current pwd: 110
				IF(@CurrentPwd <> @OldPassword)
				BEGIN
					SELECT 110;
					RETURN;
				END

				--The new pwd = current pwd: 111
				IF(@CurrentPwd = @NewPassword)
				BEGIN
					SELECT 111;
					RETURN;
				END
				ELSE
				BEGIN
					--OldPwd2 matches with current password2 -> Update password2 to temp field and waiting for OTP confirm
					-- UPDATE tbl_users SET SecurityStamp = @NewPassword WHERE 1=1 AND Id = @UserId;
					SELECT 1;
					RETURN;
				END
			END
			ELSE
			BEGIN
				--For the first time change pwd2
				UPDATE tbl_users SET SecurityStamp = @NewPassword WHERE 1=1 AND Id = @UserId;
				SELECT 1;
				RETURN;
			END
		END

	END	
	ELSE
	BEGIN
		--User or Token not found: 103
		SELECT 103;
		RETURN;
	END;
END
GO
/****** Object:  StoredProcedure [dbo].[User_ChangePassword2]    Script Date: 11/22/2019 10:12:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[User_ChangePassword2]
	-- Add the parameters for the stored procedure here
	@UserId int,
	@PasswordHash2 nvarchar(128)
AS
BEGIN
	UPDATE tbl_users 
	SET PasswordHash2 = @PasswordHash2
	WHERE 1=1
	AND Id = @UserId
	;
END
GO
/****** Object:  StoredProcedure [dbo].[User_CheckPwd2IsValid]    Script Date: 11/22/2019 10:12:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[User_CheckPwd2IsValid]
	-- Add the parameters for the stored procedure here
	@UserId int,
	@TokenKey nvarchar(128),
	@Pwd nvarchar(128)
AS
BEGIN
	DECLARE @Checker int;
	DECLARE @CurrentUser nvarchar(128);
	-- Check user + token valid
	SET @Checker = (SELECT dbo.CheckUserToken(@UserId, @TokenKey));
	IF(@Checker = 0)	
	-- Return code: 103 - User or token not correct
	BEGIN
		SELECT 103;
		RETURN;
	END

	SET @CurrentUser = (SELECT TOP 1 Id FROM tbl_users WHERE 1=1 AND Id = @UserId AND PasswordHash2 = @Pwd);

	IF(@CurrentUser IS NOT NULL)
	BEGIN
		SELECT 1;
		RETURN;
	END
	
	SELECT -1;
	RETURN;
END
GO
/****** Object:  StoredProcedure [dbo].[User_CreateOTPCode]    Script Date: 11/22/2019 10:12:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[User_CreateOTPCode]
	-- Add the parameters for the stored procedure here
	@TranId nvarchar(128),
	@UserId int,
	@Code nvarchar(50),
	@Time datetime,
	@TokenKey nvarchar(128),
	@Action nvarchar(50),
	@TargetData nvarchar(256),
	@OTPCodeType nvarchar(20)
AS
BEGIN
	DECLARE @Checker int;
	DECLARE @CurrentCode nvarchar(128);
	DECLARE @ExpiredDate datetime;
	DECLARE @CurrentUser nvarchar(128);
	DECLARE @UserOTPCodeType nvarchar(20);
	DECLARE @Result int;

	/*
	SET @Checker = (SELECT dbo.CheckUserToken(@UserId, @TokenKey));
	IF(@Checker = 0)	
	-- Return code: 103 - User or token not correct
	BEGIN
		SELECT 103;
		RETURN;
	END
	*/

	SELECT TOP 1 
		@UserOTPCodeType = t.OTPType,
		@CurrentUser = t.Id
	FROM tbl_users t 
	LEFT JOIN tbl_user_tokenkeys k on t.Id = k.UserId
	WHERE 1=1 
	AND t.Id = @UserId
	--AND (k.TokenKey = @TokenKey OR @TokenKey IS NULL OR @TokenKey = '')
	ORDER BY k.CreatedDate DESC
	;

	IF(@CurrentUser IS NULL)	
	-- Return code: 103 - User or token not correct
	BEGIN
		SELECT 103;
		RETURN;
	END

	IF(@UserOTPCodeType = 'password2' OR @UserOTPCodeType = 'PASSWORD2')
	BEGIN
		SELECT 1;
		RETURN;
	END

	IF (@UserOTPCodeType IS NULL) SET @UserOTPCodeType = 'OTPSMS';

	IF(@UserOTPCodeType LIKE '%ODP%') SET @ExpiredDate = CAST(CONVERT(VARCHAR(10), @Time, 110) + ' 23:59:59' AS DATETIME);

	IF (@OTPCodeType IS NOT NULL AND @OTPCodeType <> '') SET @UserOTPCodeType = @OTPCodeType;

	-- Move old codes (with same CodeType) to history table
	INSERT INTO tbl_user_codes_history
	SELECT * FROM tbl_user_codes t
	WHERE 1=1
	AND t.UserId = @UserId
	AND t.Code != @Code
	--AND t.CodeType = @UserOTPCodeType
	; 

	-- Delete old codes 
	DELETE FROM tbl_user_codes 
	WHERE 1=1 
	AND UserId = @UserId
	AND Code != @Code
	--AND CodeType = @UserOTPCodeType
	;
		
	-- Create new code
	INSERT INTO tbl_user_codes(Id, UserId, Code, CodeType, CreatedDate, ExpiredDate) 
	VALUES(@TranId, @UserId, @Code, @UserOTPCodeType, @Time, @ExpiredDate)
	;

	-- Create action for this code
	EXEC dbo.User_CreateOTPCodeAction @UserId, @TranId, @TargetData, @Action;

	-- Success
	SELECT 1;
	RETURN;
END
GO
/****** Object:  StoredProcedure [dbo].[User_CreateOTPCodeAction]    Script Date: 11/22/2019 10:12:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[User_CreateOTPCodeAction]
	-- Add the parameters for the stored procedure here
	@UserId int,
	@CodeId nvarchar(128),
	@TargetData nvarchar(256),
	@Action nvarchar(50)
AS
BEGIN
	-- Move old actions to history table
	INSERT INTO tbl_user_otpactions_history
	SELECT * FROM tbl_user_otpactions t
	WHERE 1=1
	AND t.UserId = @UserId
	-- AND t.Action = @Action
	; 

	-- Delete old actions 
	DELETE FROM tbl_user_otpactions 
	WHERE 1=1 
	AND UserId = @UserId
	-- AND Action = @Action
	;
	
	INSERT INTO tbl_user_otpactions(UserId,CodeId,TargetData,CreatedDate,Action)
	values (@UserId,@CodeId,@TargetData,GETDATE(),@Action);

	-- Success
	SELECT 1;
	RETURN;
END
GO
/****** Object:  StoredProcedure [dbo].[User_ExecOTPCodeAction]    Script Date: 11/22/2019 10:12:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[User_ExecOTPCodeAction]
	-- Add the parameters for the stored procedure here
	@UserId int,
	@Action nvarchar(50)
AS
BEGIN
	DECLARE @CurrentOTPCode nvarchar(128);
	DECLARE @CurrentTargeData nvarchar(256);

	SELECT TOP 1 
		@CurrentOTPCode = t.CodeId,
		@CurrentTargeData = t.TargetData
	FROM tbl_user_otpactions t WHERE 1=1
	AND t.UserId = @UserId
	-- AND t.Action = @Action
	ORDER by t.CreatedDate DESC
	;

	IF(@Action IS NOT NULL)
	BEGIN
		IF(@CurrentOTPCode IS NOT NULL)
		BEGIN
			IF(@Action = 'changepwd2')
			BEGIN
				EXEC dbo.User_ChangePassword2 @UserId, @CurrentTargeData;
			END
			ELSE IF(@Action = 'changeauthmethod')
			BEGIN
				EXEC dbo.User_ChangeAuthMethod @UserId, @CurrentTargeData;
			END
			ELSE IF(@Action = 'active_account')
			BEGIN
				EXEC dbo.User_ActiveAccountByOTP @UserId;
			END
			ELSE IF(@Action = 'recover_password1')
			BEGIN
				EXEC dbo.User_RecoverPassword @UserId, @CurrentTargeData, 'level1';
			END
			ELSE IF(@Action = 'recover_password2')
			BEGIN
				EXEC dbo.User_RecoverPassword @UserId, @CurrentTargeData, 'level2';
			END

			-- Update status
			UPDATE tbl_user_otpactions 
			SET 
				IsDone = 1,
				ImplementTime = GETDATE()
			WHERE 1=1 
			AND CodeId = @CurrentOTPCode
			;

			-- Move old actions to history table
			INSERT INTO tbl_user_otpactions_history
			SELECT * FROM tbl_user_otpactions t
			WHERE 1=1
			AND t.CodeId = @CurrentOTPCode			
			; 

			-- Delete old actions 
			DELETE FROM tbl_user_otpactions 
			WHERE 1=1 
			AND CodeId = @CurrentOTPCode
			;
		END
	END	
END
GO
/****** Object:  StoredProcedure [dbo].[User_GetAllDevices]    Script Date: 11/22/2019 10:12:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[User_GetAllDevices]
	-- Add the parameters for the stored procedure here
	@UserId int	
AS
BEGIN	
	BEGIN TRY
    BEGIN TRANSACTION

		SELECT * FROM tbl_user_devices 
		WHERE 1=1
		AND UserId = @UserId
		AND Status = 1
		;

    COMMIT TRAN -- Transaction Success!
	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0
			ROLLBACK TRAN --RollBack in case of Error

		-- you can Raise ERROR with RAISEERROR() Statement including the details of the exception
		exec dbo.SQL_WriteLog;
	END CATCH	
	
END
GO
/****** Object:  StoredProcedure [dbo].[User_GetByEmail]    Script Date: 11/22/2019 10:12:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[User_GetByEmail]
	-- Add the parameters for the stored procedure here
	@Email nvarchar(128)
AS
BEGIN	
	SET NOCOUNT ON;
	
	SELECT TOP 1 * FROM tbl_users t 
	WHERE 1=1	
	AND (t.Email = @Email)
	;	
END
GO
/****** Object:  StoredProcedure [dbo].[User_GetById]    Script Date: 11/22/2019 10:12:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[User_GetById]
	-- Add the parameters for the stored procedure here
	@UserId int
AS
BEGIN	
	SET NOCOUNT ON;
	
	SELECT TOP 1 t.*, k.TokenKey, k.ExpiredDate as TokenExpiredDate, k.CreatedDate as TokenCreatedDate
	from tbl_users t 
	left join tbl_user_tokenkeys k on t.Id = k.UserId
	where 1=1	
	and t.Id = @UserId
	ORDER BY k.CreatedDate DESC
	;
	
END
GO
/****** Object:  StoredProcedure [dbo].[User_GetByInfo]    Script Date: 11/22/2019 10:12:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[User_GetByInfo]
	-- Add the parameters for the stored procedure here
	@Info nvarchar(128)
AS
BEGIN	
	SET NOCOUNT ON;
	
	SELECT TOP 1 * FROM tbl_users t 
	WHERE 1=1	
	AND (t.Email = @Info OR t.UserName = @Info OR PhoneNumber = @Info OR Id = @Info)
	;	
END
GO
/****** Object:  StoredProcedure [dbo].[User_GetByPhoneNumber]    Script Date: 11/22/2019 10:12:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[User_GetByPhoneNumber]
	-- Add the parameters for the stored procedure here
	@PhoneNumber nvarchar(128)
AS
BEGIN	
	SET NOCOUNT ON;
	
	SELECT TOP 1 * FROM tbl_users t 
	WHERE 1=1	
	AND (t.PhoneNumber = @PhoneNumber)
	;	
END
GO
/****** Object:  StoredProcedure [dbo].[User_GetByUserName]    Script Date: 11/22/2019 10:12:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[User_GetByUserName]
	-- Add the parameters for the stored procedure here
	@UserName nvarchar(128)
AS
BEGIN	
	SET NOCOUNT ON;
	
	SELECT TOP 1 t.*, k.TokenKey, k.ExpiredDate as TokenExpiredDate, k.CreatedDate as TokenCreatedDate
	from tbl_users t 
	left join tbl_user_tokenkeys k on t.Id = k.UserId
	where 1=1	
	-- and (t.Email = @UserName or t.UserName = @UserName or PhoneNumber = @UserName)
	and t.UserName = @UserName 
	ORDER BY k.CreatedDate DESC
	;
	
END
GO
/****** Object:  StoredProcedure [dbo].[User_GetCurrentOTP]    Script Date: 11/22/2019 10:12:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[User_GetCurrentOTP]
	-- Add the parameters for the stored procedure here
	@UserId int,
	@Action nvarchar(50)
AS
BEGIN
	SELECT TOP 1 a.Id,a.Code,a.UserId,a.CodeType,b.TargetData from tbl_user_codes a
	LEFT JOIN tbl_user_otpactions b on a.Id = b.CodeId
	WHERE 1=1
	AND a.UserId = @UserId
	AND b.Action = @Action
	;
END
GO
/****** Object:  StoredProcedure [dbo].[User_GetCurrentTokenKey]    Script Date: 11/22/2019 10:12:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[User_GetCurrentTokenKey]
	@UserId int
AS
BEGIN
	SET NOCOUNT ON;
		
	SELECT TOP 1 * FROM tbl_user_tokenkeys t 
	WHERE 1=1 
	AND t.UserId = @UserId 
	ORDER BY t.CreatedDate DESC
	;
END
GO
/****** Object:  StoredProcedure [dbo].[User_GetListUser]    Script Date: 11/22/2019 10:12:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[User_GetListUser]
@ListUserId nvarchar(MAX)
AS
BEGIN
	DECLARE @sqllike nvarchar(MAX)
	SET @sqllike='SELECT * FROM tbl_users WHERE Id IN ('+@ListUserId+')';

	exec sp_executesql @sqllike
END
GO
/****** Object:  StoredProcedure [dbo].[User_GetProfile]    Script Date: 11/22/2019 10:12:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[User_GetProfile]
	-- Add the parameters for the stored procedure here
	@UserId int
AS
BEGIN	
	SET NOCOUNT ON;
	
	-- Get base info
	SELECT TOP 1 a.*
	FROM tbl_users a
	WHERE 1=1	
	AND a.Id = @UserId
	;	
END
GO
/****** Object:  StoredProcedure [dbo].[User_Login]    Script Date: 11/22/2019 10:12:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[User_Login]
	-- Add the parameters for the stored procedure here
	@UserName nvarchar(128),
	@Password nvarchar(128),
	@Domain nvarchar(128)
AS
BEGIN	
	SET NOCOUNT ON;
	DECLARE @LoginDurations int;
	SET @LoginDurations = (SELECT TOP 1 t.LoginDurations FROM tbl_domains t where 1=1 AND DomainKey = @Domain );
	
	IF(@LoginDurations IS NULL OR @LoginDurations <= 0)
	BEGIN
		--Default login timeout is 1 week
		SET @LoginDurations = 10080;
	END

	SELECT TOP 1 t.*,@LoginDurations as LoginDurations, k.TokenKey, k.ExpiredDate as TokenExpiredDate, k.CreatedDate as TokenCreatedDate
	from tbl_users t 
	left join tbl_user_tokenkeys k on t.Id = k.UserId
	where 1=1	
	--AND (t.UserName = @UserName OR t.Email =@UserName OR t.PhoneNumber=@UserName)
	AND t.UserName = @UserName
	AND t.SocialProviderId = 0
	AND t.PasswordHash = @Password
	ORDER BY k.CreatedDate DESC
	;
	
END
GO
/****** Object:  StoredProcedure [dbo].[User_LoginWith]    Script Date: 11/22/2019 10:12:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[User_LoginWith]
	-- Add the parameters for the stored procedure here
	@UserName nvarchar(128),
	@Domain nvarchar(128),
	@SocialProvider nvarchar(128),
	@Email nvarchar(128),
	@DisplayName nvarchar(128),
	@Avatar nvarchar(4000)
AS
BEGIN	
	SET NOCOUNT ON;
	DECLARE @LoginDurations int;
	DECLARE @SocialProviderId int;
	DECLARE @UserId int;

	SET @SocialProviderId=(SELECT TOP 1 Id FROM tbl_social_provider WHERE  Code=@SocialProvider)
	SET @LoginDurations = (SELECT TOP 1 t.LoginDurations FROM tbl_domains t where 1=1 AND DomainKey = @Domain );
	
	IF(@LoginDurations IS NULL OR @LoginDurations <= 0)
	BEGIN
		--Default login timeout is 1 week
		SET @LoginDurations = 10080;
	END

	IF(NOT EXISTS (SELECT Id FROM tbl_users a WHERE UserName=@UserName AND SocialProviderId=@SocialProviderId))
	BEGIN
		INSERT INTO tbl_users(UserName,SocialProviderId,DisplayName,FullName,Email,EmailConfirmed,PhoneNumberConfirmed,Avatar) VAlUES(@UserName,@SocialProviderId,@DisplayName,@DisplayName,@Email,1,1,@Avatar)

		SET @UserId= (SELECT SCOPE_IDENTITY());

		SELECT TOP 1 t.*,@LoginDurations as LoginDurations, k.TokenKey, k.ExpiredDate as TokenExpiredDate, k.CreatedDate as TokenCreatedDate, 1 as IsNew
		FROM tbl_users t 
		left join tbl_user_tokenkeys k on t.Id = k.UserId
		WHERE 1=1	
		AND Id= @UserId
		ORDER BY k.CreatedDate DESC;
	END

	ELSE
	BEGIN
		SELECT TOP 1 t.*,@LoginDurations as LoginDurations, k.TokenKey, k.ExpiredDate as TokenExpiredDate, k.CreatedDate as TokenCreatedDate, 0 as IsNew
		FROM tbl_users t 
		left join tbl_user_tokenkeys k on t.Id = k.UserId
		WHERE 1=1	
		AND ( t.UserName = @UserName)
		AND t.SocialProviderId = @SocialProviderId
		ORDER BY k.CreatedDate DESC
		;
	END
	
END
GO
/****** Object:  StoredProcedure [dbo].[User_ProvideTokenKey]    Script Date: 11/22/2019 10:12:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author: BangVL>
-- Create date: <Create Date: 2017-07-27>
-- Description:	<Description: Create token after login successfully. Write data to log>
-- =============================================
CREATE PROCEDURE [dbo].[User_ProvideTokenKey]
	-- Add the parameters for the stored procedure here
	@UserId int,
	@TokenKey nvarchar(128),
	@Method nvarchar(50),
	@Domain nvarchar(128)
AS
BEGIN
	DECLARE @LoginDurations int;
	DECLARE @DTNow datetime;
	DECLARE @UserTokenCurrent nvarchar(128);
	DECLARE @UserTokenExpiredDate datetime;

	SET @LoginDurations = (SELECT TOP 1 t.LoginDurations FROM tbl_domains t where 1=1 AND DomainKey = @Domain );
	
	SET @DTNow = GETDATE();

	IF( @LoginDurations IS NULL)
	BEGIN	
		SET @LoginDurations = 120;
	END

	-- Move old tokens to history table
	INSERT INTO tbl_user_tokenkeys_history
	SELECT * FROM tbl_user_tokenkeys t
	WHERE 1=1
	AND t.UserId = @UserId
	AND t.Domain = @Domain
	; 

	-- Delete old tokens
	DELETE FROM tbl_user_tokenkeys 
	WHERE 1=1 
	AND UserId = @UserId
	AND Domain = @Domain
	;

	INSERT INTO tbl_user_tokenkeys(UserId,TokenKey,CreatedDate,ExpiredDate,Method,Domain) values(@UserId,@TokenKey,@DTNow, DATEADD(minute,@LoginDurations,@DTNow), @Method, @Domain);

END
GO
/****** Object:  StoredProcedure [dbo].[User_ProvideTokenKey_Old]    Script Date: 11/22/2019 10:12:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author: BangVL>
-- Create date: <Create Date: 2017-07-27>
-- Description:	<Description: Create token after login successfully. Write data to log>
-- =============================================
CREATE PROCEDURE [dbo].[User_ProvideTokenKey_Old]
	-- Add the parameters for the stored procedure here
	@UserId int,
	@TokenKey nvarchar(128),
	@Method nvarchar(50),
	@Domain nvarchar(128)
AS
BEGIN
	DECLARE @LoginDurations int;
	DECLARE @DTNow datetime;
	DECLARE @UserTokenCurrent nvarchar(128);
	DECLARE @UserTokenExpiredDate datetime;

	SET @LoginDurations = (SELECT TOP 1 t.LoginDurations FROM tbl_domains t where 1=1 AND DomainKey = @Domain );
	
	SET @DTNow = GETDATE();

	IF( @LoginDurations IS NULL)
	BEGIN	
		SET @LoginDurations = 120;
	END

	-- Check duplicate token with UserId + Domain
	SELECT TOP 1 
		@UserTokenCurrent = t.UserId, 
		@UserTokenExpiredDate  = t.ExpiredDate
	FROM tbl_user_tokenkeys t 
	where 1=1 and t.UserId = @UserId and t.Domain = @Domain 
	ORDER BY t.CreatedDate DESC

	-- If UserToken with this domain not exsited
	IF(@UserTokenCurrent IS NULL)
	BEGIN
		INSERT INTO tbl_user_tokenkeys(UserId,TokenKey,CreatedDate,ExpiredDate,Method,Domain) values(@UserId,@TokenKey,@DTNow, DATEADD(minute,@LoginDurations,@DTNow), @Method, @Domain);
    END
	ELSE
	BEGIN
		
		-- If UserToken existed -> Check expired date
		IF(@UserTokenExpiredDate IS NOT NULL AND @UserTokenExpiredDate <= @DTNow)
		BEGIN 
			-- If token has been expired -> Create new
			INSERT INTO tbl_user_tokenkeys(UserId,TokenKey,CreatedDate,ExpiredDate,Method,Domain) values(@UserId,@TokenKey,@DTNow, DATEADD(minute,@LoginDurations,@DTNow), @Method, @Domain);
		END

		-- Move old tokens to history table
		INSERT INTO tbl_user_tokenkeys_history
		SELECT * FROM tbl_user_tokenkeys t
		WHERE 1=1
		AND t.UserId = @UserId
		AND t.Domain = @Domain
		AND t.TokenKey != @TokenKey
		; 

		-- Delete old tokens
		DELETE FROM tbl_user_tokenkeys 
		WHERE 1=1 
		AND UserId = @UserId
		AND Domain = @Domain
		AND TokenKey != @TokenKey
		;
	END	
END
GO
/****** Object:  StoredProcedure [dbo].[User_RecoverPassword]    Script Date: 11/22/2019 10:12:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[User_RecoverPassword]
	-- Add the parameters for the stored procedure here
	@UserId int,	
	@NewPassword nvarchar(128),
	@Level nvarchar(20) --level1 or level2
AS
BEGIN
		IF(@Level = 'level1')
		BEGIN
			UPDATE tbl_users SET PasswordHash = @NewPassword
			WHERE 1=1
			AND Id = @UserId
			;
		END
		ELSE IF(@Level = 'level2')
		BEGIN
			UPDATE tbl_users SET PasswordHash2 = @NewPassword
			WHERE 1=1
			AND Id = @UserId
			;
		END	
END
GO
/****** Object:  StoredProcedure [dbo].[User_RecoverPasswordByNewPwd]    Script Date: 11/22/2019 10:12:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[User_RecoverPasswordByNewPwd]
	-- Add the parameters for the stored procedure here
	@UserId int,	
	@PasswordHash nvarchar(256),
	@HashingData nvarchar(MAX) 
AS
BEGIN
		UPDATE tbl_users 
		SET PasswordHash = @PasswordHash,
			SecurityStamp = NULL
		WHERE 1=1
		AND Id = @UserId
		--AND HashingData = @HashingData
		;
END
GO
/****** Object:  StoredProcedure [dbo].[User_RecoverPasswordStep1]    Script Date: 11/22/2019 10:12:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[User_RecoverPasswordStep1]
	-- Add the parameters for the stored procedure here
	@UserId int,
	@Pwd nvarchar(128)	
AS
BEGIN
	UPDATE tbl_users 
	SET  SecurityStamp = @Pwd 
	WHERE 1=1
	AND Id = @UserId
	;

	--Begin recover pwd step 1 success 
	SELECT 1;
	RETURN;
END
GO
/****** Object:  StoredProcedure [dbo].[User_RecoverPasswordStep2]    Script Date: 11/22/2019 10:12:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[User_RecoverPasswordStep2]
	-- Add the parameters for the stored procedure here
	@UserId int,
	@PwdType nvarchar(20)	
AS
BEGIN
	DECLARE @CurrentUser nvarchar(128);
	DECLARE @SecurityStamp nvarchar(128);

	SELECT TOP 1 @CurrentUser = t.Id, @SecurityStamp = SecurityStamp 
	FROM tbl_users t 
	WHERE 1=1 
	AND t.Id = @UserId
	;

	-- Check user is existed
	SET @CurrentUser = (SELECT TOP 1 Id FROM tbl_users t WHERE 1=1 AND t.Id = @UserId );
	IF(@CurrentUser IS NULL)	
	-- Return code: 103 - User not exists
	BEGIN
		SELECT 103;
		RETURN;
	END

	IF(@SecurityStamp IS NULL OR @SecurityStamp = '')
	BEGIN 
		--Not found information: -2
		SELECT -2;
		RETURN;
	END

	IF(@PwdType IS NOT NULL)
	BEGIN
		IF(@PwdType = 'level1')
		BEGIN
			UPDATE tbl_users SET PasswordHash = SecurityStamp, SecurityStamp = NULL 
			WHERE 1=1
			AND Id = @UserId
			;

			EXEC SystemEmail_ReceiverRead 'recover_password1', @UserId;

			--Change pwd successfully 
			SELECT 1;
			RETURN;
		END

		IF(@PwdType = 'level2')
		BEGIN
			UPDATE tbl_users SET PasswordHash2 = SecurityStamp, SecurityStamp = NULL 
			WHERE 1=1
			AND Id = @UserId
			;

			EXEC SystemEmail_ReceiverRead 'recover_password2', @UserId;

			--Change pwd successfully 
			SELECT 1;
			RETURN;
		END
		
	END
END
GO
/****** Object:  StoredProcedure [dbo].[User_RefreshTokenKey]    Script Date: 11/22/2019 10:12:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author: BangVL>
-- Create date: <Create Date: 2017-07-27>
-- Description:	<Description: Refresh token>
-- =============================================
CREATE PROCEDURE [dbo].[User_RefreshTokenKey]
	-- Add the parameters for the stored procedure here
	@UserId int,
	@TokenKey nvarchar(128),
	@Domain nvarchar(128),
	@Time datetime
AS
BEGIN
	DECLARE @LoginDurations int;	
	DECLARE @DTNow datetime;

	SET @LoginDurations = (SELECT TOP 1 t.LoginDurations FROM tbl_domains t where 1=1 AND DomainKey = @Domain );
	IF( @LoginDurations IS NULL)
	BEGIN	
		SET @LoginDurations = 120;
	END

	SET @DTNow = GETDATE();
	IF(@Time < GETDATE())
	BEGIN
		SET @Time = @DTNow;
	END

	UPDATE tbl_user_tokenkeys
	SET CreatedDate = @Time,
	    ExpiredDate = DATEADD(minute,@LoginDurations,@Time)
	WHERE 1=1
	AND UserId = @UserId
	AND TokenKey = @TokenKey
	;	

	 SELECT @@ROWCOUNT;
END
GO
/****** Object:  StoredProcedure [dbo].[User_ResendEmailActive]    Script Date: 11/22/2019 10:12:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[User_ResendEmailActive]
	-- Add the parameters for the stored procedure here
	@UserName nvarchar(128),
	@HashingData nvarchar(MAX)
	AS
BEGIN
	UPDATE tbl_users SET SecurityStamp = @HashingData
	WHERE 1=1
	AND UserName = @UserName
	;

	SELECT 1;
END
GO
/****** Object:  StoredProcedure [dbo].[User_SendEmailRecoverPassword]    Script Date: 11/22/2019 10:12:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[User_SendEmailRecoverPassword]
	-- Add the parameters for the stored procedure here
	@UserName nvarchar(128),
	@HashingData nvarchar(MAX)
	AS
BEGIN
	UPDATE tbl_users SET SecurityStamp = @HashingData
	WHERE 1=1
	AND UserName = @UserName
	;

	SELECT 1;
END
GO
/****** Object:  StoredProcedure [dbo].[User_UpdateAvatar]    Script Date: 11/22/2019 10:12:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[User_UpdateAvatar]
	-- Add the parameters for the stored procedure here
	@UserId int,
	@Avatar nvarchar(4000)
AS
BEGIN	
	BEGIN TRY
    BEGIN TRANSACTION

		UPDATE tbl_users SET Avatar = @Avatar WHERE 1=1 AND Id = @UserId;

    COMMIT TRAN -- Transaction Success!
	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0
			ROLLBACK TRAN --RollBack in case of Error

		-- you can Raise ERROR with RAISEERROR() Statement including the details of the exception
		exec dbo.SQL_WriteLog;
	END CATCH	
	
END
GO
/****** Object:  StoredProcedure [dbo].[User_UpdateCounter]    Script Date: 11/22/2019 10:12:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[User_UpdateCounter]
(
 @FieldName nvarchar(128),
 @FieldValue int,
 @UserId int
 )
 AS
 BEGIN
	DECLARE @Checker int;
	SET @Checker = (SELECT TOP 1 UserId FROM tbl_user_data WHERE 1=1 AND UserId = @UserId);
	BEGIN TRY
    BEGIN TRANSACTION
		IF(@Checker > 0)
		BEGIN
			--If user meta data existed
			IF @FieldName = 'FollowingCount'
				UPDATE tbl_user_data SET FollowingCount = FollowingCount + @FieldValue
				WHERE 1=1
				AND UserId = @UserId
				;

			IF @FieldName = 'PostCount'
				UPDATE tbl_user_data SET PostCount = PostCount + @FieldValue
				WHERE 1=1
				AND UserId = @UserId
				;

			IF @FieldName = 'FollowerCount'
				UPDATE tbl_user_data SET FollowerCount = FollowerCount + @FieldValue
				WHERE 1=1
				AND UserId = @UserId
				;

			IF @FieldName = 'LikePostCount'
				UPDATE tbl_user_data SET LikePostCount = LikePostCount + @FieldValue
				WHERE 1=1
				AND UserId = @UserId
				;

			IF @FieldName = 'PhotoCount'
				UPDATE tbl_user_data SET PhotoCount = PhotoCount + @FieldValue
				WHERE 1=1
				AND UserId = @UserId
				;

			IF @FieldName = 'MessageCount'
				UPDATE tbl_user_data SET MessageCount = MessageCount + @FieldValue
				WHERE 1=1
				AND UserId = @UserId
				;
		END	
		ELSE
		BEGIN
			--If user meta not existed
			IF @FieldName = 'FollowingCount'
				INSERT INTO tbl_user_data(UserId,FollowingCount) VALUES(@UserId, @FieldValue);

			IF @FieldName = 'PostCount'
				INSERT INTO tbl_user_data(UserId,PostCount) VALUES(@UserId, @FieldValue);

			IF @FieldName = 'FollowerCount'
				INSERT INTO tbl_user_data(UserId,FollowerCount) VALUES(@UserId, @FieldValue);

			IF @FieldName = 'LikePostCount'
				INSERT INTO tbl_user_data(UserId,LikePostCount) VALUES(@UserId, @FieldValue);

			IF @FieldName = 'PhotoCount'
				INSERT INTO tbl_user_data(UserId,PhotoCount) VALUES(@UserId, @FieldValue);

			IF @FieldName = 'MessageCount'
				INSERT INTO tbl_user_data(UserId,MessageCount) VALUES(@UserId, @FieldValue);
		END;

    COMMIT TRAN -- Transaction Success!
	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0
			ROLLBACK TRAN --RollBack in case of Error

		-- you can Raise ERROR with RAISEERROR() Statement including the details of the exception
		exec dbo.SQL_WriteLog;
	END CATCH	
END
GO
/****** Object:  StoredProcedure [dbo].[User_UpdateDevice]    Script Date: 11/22/2019 10:12:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[User_UpdateDevice]
	-- Add the parameters for the stored procedure here
	@UserId int,
	@DeviceName nvarchar(256),
	@DeviceID nvarchar(128),
	@RegistrationID nvarchar(500),
	@iosDevice int,
	@LangCode nvarchar(10)
AS
BEGIN	
	DECLARE @CurrentId int;
	BEGIN TRY
    BEGIN TRANSACTION
		SET @CurrentId = (SELECT TOP 1 Id FROM tbl_user_devices WHERE 1=1 AND UserId = @UserId AND DeviceID = @DeviceID);

		IF @CurrentId IS NOT NULL
		BEGIN
			UPDATE tbl_user_devices 
			SET RegistrationID = @RegistrationID,
				LangCode = @LangCode
			WHERE 1=1 
			AND Id = @CurrentId
			;
		END
		ELSE
		BEGIN
			INSERT INTO tbl_user_devices(UserId, DeviceName, DeviceID, RegistrationID, iosDevice, LangCode) 
			VALUES(@UserId, @DeviceName, @DeviceID, @RegistrationID, @iosDevice, @LangCode)
			;

			SET @CurrentId = (SELECT SCOPE_IDENTITY());
		END
		
		SELECT @CurrentId;
    COMMIT TRAN -- Transaction Success!
	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0
			ROLLBACK TRAN --RollBack in case of Error

		-- you can Raise ERROR with RAISEERROR() Statement including the details of the exception
		exec dbo.SQL_WriteLog;
	END CATCH	
	
END
GO
/****** Object:  StoredProcedure [dbo].[User_UpdateOnlineTime]    Script Date: 11/22/2019 10:12:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[User_UpdateOnlineTime]
	-- Add the parameters for the stored procedure here
	@UserId int,
	@LastOnline DateTime
AS
BEGIN	
	BEGIN TRY
    BEGIN TRANSACTION

		UPDATE tbl_users SET LastOnline = @LastOnline
		WHERE 1=1 
		AND Id = @UserId
		;

    COMMIT TRAN -- Transaction Success!
	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0
			ROLLBACK TRAN --RollBack in case of Error

		-- you can Raise ERROR with RAISEERROR() Statement including the details of the exception
		exec dbo.SQL_WriteLog;
	END CATCH	
	
END
GO
/****** Object:  StoredProcedure [dbo].[User_UpdateProfile]    Script Date: 11/22/2019 10:12:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[User_UpdateProfile]
	-- Add the parameters for the stored procedure here
	@UserId int,	
	@TokenKey nvarchar(128),
	@FullName nvarchar(128),
	@DisplayName nvarchar(128),
	@Email nvarchar(256),
	@PhoneNumber nvarchar(50),
	@Birthday datetime,
	@Sex tinyint,
	@Address nvarchar(256),
	@Note nvarchar(1000),
	@Avatar nvarchar(1000)
AS
BEGIN
	DECLARE @Checker int;
	DECLARE @DuplicateCode int;

	--SET @DuplicateCode = (SELECT dbo.CheckDuplicateUpdateUserInfo(@UserId, @Email, @PhoneNumber));

	--IF(@DuplicateCode > 0)
	--BEGIN
	--	SELECT @DuplicateCode;
	--	RETURN;
	--END

	SET @Checker = (SELECT dbo.CheckUserToken(@UserId, @TokenKey));
	IF(@Checker > 0)
	BEGIN
		--Begin updating
		UPDATE tbl_users
		SET FullName = @FullName,
			DisplayName = @DisplayName,
			Birthday = @Birthday,
			Sex = @Sex,
			Address = @Address,
			PhoneNumber = @PhoneNumber,
			Note = @Note,
			Email = @Email,
			Avatar = IsNull(@Avatar, Avatar)
		WHERE 1=1
		AND Id = @UserId
		;

		SELECT 1;
		RETURN;
	END	
	ELSE
	BEGIN
		--User or Token not found: 103
		SELECT 103;
		RETURN;
	END;
END
GO
/****** Object:  StoredProcedure [dbo].[User_VerifyOTPCode]    Script Date: 11/22/2019 10:12:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[User_VerifyOTPCode]
	-- Add the parameters for the stored procedure here
	@UserId int,
	@Code nvarchar(128),
	@Time datetime,
	@TokenKey nvarchar(128),
	@Action nvarchar(50)
AS
BEGIN
	--User data
	DECLARE @CurrentUser int;
	DECLARE @CurrentOTPType nvarchar(50);
	DECLARE @UserAuthWithPw2 int;

	--OTP data
	DECLARE @CurrentOTP nvarchar(128);
	DECLARE @IsUsed int;
	DECLARE @CurrentCodeType nvarchar(50);
	DECLARE @CurrentExpiredDate datetime;
	DECLARE @CurrentCreatedDate datetime;
	DECLARE @DTNow datetime;
	

	SET @DTNow = GETDATE();
	-- Check user + token valid
	SELECT 
		@CurrentUser = t.Id,
		@CurrentOTPType = t.OTPType
	FROM tbl_users t 
	LEFT JOIN tbl_user_tokenkeys k on t.Id = k.UserId
	WHERE 1=1 
	AND t.Id = @UserId
	AND (k.TokenKey = @TokenKey OR @TokenKey IS NULL OR @TokenKey = '')
	;

	IF(@CurrentUser IS NULL)	
	-- Return code: 103 - User or token not correct
	BEGIN
		SELECT 103;
		RETURN;
	END

	IF(@CurrentOTPType = 'password2' OR @CurrentOTPType = 'PASSWORD2')
	BEGIN
		SELECT @UserAuthWithPw2 = t.Id FROM tbl_users t where 1=1
		and t.Id = @UserId 
		and t.PasswordHash2 = @code
		;

		-- Password2 not correct: 303
		IF(@UserAuthWithPw2 IS NULL)
		BEGIN
			SELECT 303;
			RETURN;
		END
		ELSE
		BEGIN
			SELECT 1;
			RETURN;
		END
	END

	-- Check code exists and unused
	SELECT TOP 1 
		@CurrentOTP = t.Id,
		@IsUsed = t.IsUsed,
		@CurrentExpiredDate = t.ExpiredDate,
		@CurrentCreatedDate = t.CreatedDate,
		@CurrentCodeType = t.CodeType
	FROM tbl_user_codes t 
	LEFT JOIN tbl_user_otpactions p ON t.Id = p.CodeId
	WHERE 1=1 
	AND t.UserId = @UserId
	AND t.Code = @Code
	--AND t.CodeType = @CurrentOTPType
	-- AND p.Action = @Action
	ORDER BY t.CreatedDate DESC
	;

	-- The OTP Code not found: Return 301
	IF(@CurrentOTP IS NULL)
	BEGIN 
		SELECT 301;
		RETURN;
	END
	ELSE
	BEGIN
		IF(@CurrentCodeType LIKE '%ODP%')
		BEGIN
			-- If the codetype is ODP
			-- If expireddate < now: The otp code has been expired
			IF(@DTNow > @CurrentExpiredDate)
			BEGIN
					
				--Remove it and other code with same type to history table
				INSERT INTO tbl_user_codes_history
				SELECT * FROM tbl_user_codes t
				WHERE 1=1
				AND t.UserId = @UserId
				--AND t.CodeType = @CurrentOTPType
				; 

				-- Delete old codes 
				DELETE FROM tbl_user_codes 
				WHERE 1=1 
				AND UserId = @UserId
				--AND CodeType = @CurrentOTPType
				;
					
				-- The OTP Code has been expired: Return 302
				SELECT 302;
				RETURN;
			END
		END
		ELSE
		BEGIN 
			-- Check status of OTPCode (different ODP)
			IF(@IsUsed > 0)			
			BEGIN
				-- The OTP Code already used: Return 301
				SELECT 301;
				RETURN;
			END
		END
			
		-- Update data
		-- UPDATE OTPCode status
		UPDATE tbl_user_codes 
		SET IsUsed = IsUsed + 1,
			UsedDate = GETDATE(),
			Action = @Action
		WHERE 1=1
		AND UserId = @UserId
		--AND Code = @Code
		-- AND CodeType = @CurrentOTPType
		;

		--Do some actions here
		EXEC dbo.User_ExecOTPCodeAction @UserId, @Action;

		IF(@CurrentCodeType NOT LIKE '%ODP%')
		BEGIN
			--Remove it and other code with same type to history table
			INSERT INTO tbl_user_codes_history
			SELECT * FROM tbl_user_codes t
			WHERE 1=1
			AND t.UserId = @UserId
			-- AND t.CodeType = @CurrentOTPType
			; 

			-- Delete old codes 
			DELETE FROM tbl_user_codes 
			WHERE 1=1 
			AND UserId = @UserId			
			-- AND CodeType = @CurrentOTPType
			;
		END

		SELECT 1;
		RETURN;

		
	END
END
GO
/****** Object:  StoredProcedure [dbo].[User_WebRegister]    Script Date: 11/22/2019 10:12:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[User_WebRegister]
	-- Add the parameters for the stored procedure here
	@UserName nvarchar (128),
	@PasswordHash nvarchar(128),
	@Email nvarchar(256),
	@Birthday datetime,
	@Sex tinyint,
	@Address nvarchar(256),
	@FullName nvarchar(128),
	@DisplayName nvarchar(128),
	@IDCard nvarchar(50),
	@PhoneNumber nvarchar(50),
	@Note nvarchar(1000),
	@OTPType nvarchar(20)
	 
AS
BEGIN
	DECLARE @Checker int;
	DECLARE @newId int;
	SET @Checker = (SELECT dbo.CheckDuplicateUserInfo(@UserName, @Email, @PhoneNumber, @IDCard));

	IF(@Checker = 1)
	BEGIN
		INSERT INTO tbl_users(UserName, PasswordHash, Email, Birthday, Sex, Address, FullName, DisplayName, IDCard, PhoneNumber, Note, OTPType, CreatedDateUtc)
		values(@UserName, @PasswordHash, @Email, @Birthday, @Sex, @Address, @FullName, @DisplayName, @IDCard, @PhoneNumber, @Note, @OTPType, GETDATE())
		;

		SELECT 1;

		SET @newId = (SELECT SCOPE_IDENTITY());

		RETURN;
	END
	ELSE
	BEGIN
		SELECT @Checker;

		SELECT 0;
		RETURN;
	END
END
GO
/****** Object:  StoredProcedure [dbo].[User_WriteLog_Action]    Script Date: 11/22/2019 10:12:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[User_WriteLog_Action]
	@ActionType nvarchar(50),
	@UserIp nvarchar(20),
	@UserAgent nvarchar(2000),
	@Method nvarchar(50),
	@Domain nvarchar(128),
	@ActionDesc nvarchar(MAX),
	@RawData nvarchar(MAX)
AS
BEGIN
	INSERT INTO dbo.tbl_traces(ActionType,UserIp,UserAgent,Method,Domain,CreatedDate,ActionDesc,RawData)
		VALUES
           (
		    @ActionType,
		    @UserIp,
            @UserAgent,
            @Method,
            @Domain, 
			GETDATE(),
            @ActionDesc,
			@RawData
		   )
	;
END
GO
/****** Object:  StoredProcedure [dbo].[WinTrack_AutoFlush]    Script Date: 11/22/2019 10:12:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*
*   SSMA informational messages:
*   M2SS0003: The following SQL clause was ignored during conversion:
*   DEFINER = `kplus`@`%`.
*/

CREATE PROCEDURE [dbo].[WinTrack_AutoFlush]
AS 
   BEGIN

      SET  XACT_ABORT  ON

      SET  NOCOUNT  ON

      /* 
      *   SSMA error messages:
      *   M2SS0016: Identifier `wintracking` cannot be converted because it was not resolved.
      *   M2SS0016: Identifier SamplingTime cannot be converted because it was not resolved.

      DELETE 
      FROM wintracking
      WHERE 1 = 1 AND SamplingTime <= CAST(REPLACE((REPLACE((REPLACE((CONVERT(varchar(20), dateadd(hour, -1, CAST(getdate() AS datetime2)), 120)), '-', '')), ' ', '')), ':', '') AS bigint)
      */



   END
GO
EXEC sys.sp_addextendedproperty @name=N'MS_SSMA_SOURCE', @value=N'manager.Log4net_AutoFlush' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'PROCEDURE',@level1name=N'Log4net_AutoFlush'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_SSMA_SOURCE', @value=N'manager.Log4net_AutoGenFiles' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'PROCEDURE',@level1name=N'Log4net_AutoGenFiles'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_SSMA_SOURCE', @value=N'manager.Log4net_GetNewestRecords' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'PROCEDURE',@level1name=N'Log4net_GetNewestRecords'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_SSMA_SOURCE', @value=N'manager.Log4net_GetRecordDetails' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'PROCEDURE',@level1name=N'Log4net_GetRecordDetails'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_SSMA_SOURCE', @value=N'manager.Log4net_GetRecordsPaging' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'PROCEDURE',@level1name=N'Log4net_GetRecordsPaging'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_SSMA_SOURCE', @value=N'manager.Log4net_InsertLogEntry' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'PROCEDURE',@level1name=N'Log4net_InsertLogEntry'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_SSMA_SOURCE', @value=N'manager.WinTrack_AutoFlush' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'PROCEDURE',@level1name=N'WinTrack_AutoFlush'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_SSMA_SOURCE', @value=N'manager.HTMLBody' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'FUNCTION',@level1name=N'HTMLBody'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_SSMA_SOURCE', @value=N'manager.aspnetaccessroles' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'tbl_access_roles'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_SSMA_SOURCE', @value=N'manager.aspnetaccess' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'tbl_accesses'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_SSMA_SOURCE', @value=N'manager.aspnetactivitylog' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'tbl_activitylogs'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'0: Khong kha dung, 1: Dang hoat dong, 9: Xoa logic' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'tbl_categories', @level2type=N'COLUMN',@level2name=N'Status'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_SSMA_SOURCE', @value=N'manager.cmn_settings' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'tbl_cmn_settings'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_SSMA_SOURCE', @value=N'manager.log4netfiles' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'tbl_log4netfiles'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_SSMA_SOURCE', @value=N'manager.log4netrecords' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'tbl_log4netrecords'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_SSMA_SOURCE', @value=N'manager.log4netrecords_exceptions' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'tbl_log4netrecords_exceptions'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_SSMA_SOURCE', @value=N'manager.aspnetroles' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'tbl_roles'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_SSMA_SOURCE', @value=N'manager.aspnetuserclaims' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'tbl_user_claims'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_SSMA_SOURCE', @value=N'manager.aspnetuserroles' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'tbl_user_roles'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_SSMA_SOURCE', @value=N'manager.aspnetusers' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'tbl_users'
GO
