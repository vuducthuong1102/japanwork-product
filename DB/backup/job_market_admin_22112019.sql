USE [job_market_admin]
GO
/****** Object:  UserDefinedFunction [dbo].[F_Product_GetMinInventory]    Script Date: 11/22/2019 10:11:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[F_Product_GetMinInventory]
(  
  @ProductId int
)  
RETURNS float
AS  
BEGIN 
	DECLARE @nums float;
	SET @nums = 0;

	SET @nums = (SELECT TOP 1 MinInventory FROM tbl_product WHERE 1=1 AND Id = @ProductId);

	IF @nums IS NULL SET @nums = 0;

	RETURN @nums;
END  
GO
/****** Object:  UserDefinedFunction [dbo].[F_Product_GetStockTakeQTY]    Script Date: 11/22/2019 10:11:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[F_Product_GetStockTakeQTY]
(  
  @ProductId int
)  
RETURNS float
AS  
BEGIN 
	DECLARE @nums float;
	SET @nums = 0;

	SET @nums = (SELECT TOP 1 StockTakeQTY FROM tbl_warehouse WHERE 1=1 AND ProductId = @ProductId);

	IF @nums IS NULL SET @nums = 0;

	RETURN @nums;
END  
GO
/****** Object:  UserDefinedFunction [dbo].[F_Product_GetWarehouseNum]    Script Date: 11/22/2019 10:11:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[F_Product_GetWarehouseNum]
(  
  @ProductId int
)  
RETURNS float
AS  
BEGIN 
	DECLARE @nums float;
	SET @nums = 0;

	SET @nums = (SELECT TOP 1 WarehouseNum FROM tbl_warehouse WHERE 1=1 AND ProductId = @ProductId);

	IF @nums IS NULL SET @nums = 0;

	RETURN @nums;
END  
GO
/****** Object:  UserDefinedFunction [dbo].[F_Product_IsIncludedProperties]    Script Date: 11/22/2019 10:11:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[F_Product_IsIncludedProperties]
(  
  @ProductId int,
  @PropertyList nvarchar(max)
)  
RETURNS int
AS  
BEGIN 
	DECLARE @Counter int = 0;
	DECLARE @item nvarchar(500);    
	DECLARE @ExistedId int ;
  
	DECLARE emp_cursor CURSOR FOR     
	SELECT value FROM STRING_SPLIT(@PropertyList,'#')
	WHERE value IS NOT NULL AND value <> ''
	;
  
	OPEN emp_cursor    
  
	FETCH NEXT FROM emp_cursor     
	INTO @item    
  
	WHILE @@FETCH_STATUS = 0    
	BEGIN    		
		SET @ExistedId = (SELECT TOP 1 Id FROM tbl_product_property WHERE ProductId = @ProductId AND PropertyId IN (SELECT * FROM dbo.fnStringList2Table(@item)));

		IF(@ExistedId IS NOT NULL) 
		BEGIN
			SET @Counter = @Counter + 1;
		END
	FETCH NEXT FROM emp_cursor     
	INTO @item  
	END     
	CLOSE emp_cursor;    
	DEALLOCATE emp_cursor;  
	
	 RETURN @Counter;
END  
GO
/****** Object:  UserDefinedFunction [dbo].[fnStringList2Table]    Script Date: 11/22/2019 10:11:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[fnStringList2Table]
(
    @List varchar(MAX)
)
RETURNS 
@ParsedList table
(
    item int
)
AS
BEGIN
    DECLARE @item varchar(4000), @Pos int

    SET @List = LTRIM(RTRIM(@List))+ ','
    SET @Pos = CHARINDEX(',', @List, 1)

    WHILE @Pos > 0
    BEGIN
        SET @item = LTRIM(RTRIM(LEFT(@List, @Pos - 1)))
        IF @item <> ''
        BEGIN
            INSERT INTO @ParsedList (item) 
            VALUES (CAST(@item AS int))
        END
        SET @List = RIGHT(@List, LEN(@List) - @Pos)
        SET @Pos = CHARINDEX(',', @List, 1)
    END

    RETURN
END
GO
/****** Object:  Table [dbo].[aspnetaccess]    Script Date: 11/22/2019 10:11:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[aspnetaccess](
	[Id] [nvarchar](128) NOT NULL,
	[AccessName] [nvarchar](50) NOT NULL,
	[Active] [smallint] NULL,
	[Description] [nvarchar](255) NULL,
 CONSTRAINT [PK_aspnetaccess_1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[aspnetaccessroles]    Script Date: 11/22/2019 10:11:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[aspnetaccessroles](
	[RoleId] [nvarchar](128) NOT NULL,
	[OperationId] [int] NOT NULL,
 CONSTRAINT [PK_aspnetaccessroles_roleid] PRIMARY KEY CLUSTERED 
(
	[RoleId] ASC,
	[OperationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[aspnetactivitylog]    Script Date: 11/22/2019 10:11:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[aspnetactivitylog](
	[ActivityLogId] [int] IDENTITY(4962,1) NOT NULL,
	[UserId] [nvarchar](128) NOT NULL,
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
/****** Object:  Table [dbo].[aspnetdomains]    Script Date: 11/22/2019 10:11:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[aspnetdomains](
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
/****** Object:  Table [dbo].[aspnetmenus]    Script Date: 11/22/2019 10:11:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[aspnetmenus](
	[Id] [int] IDENTITY(47,1) NOT NULL,
	[ParentId] [int] NULL,
	[Area] [nvarchar](20) NULL,
	[Name] [nvarchar](20) NULL,
	[Title] [nvarchar](100) NULL,
	[Desc] [nvarchar](255) NULL,
	[Action] [nvarchar](50) NULL,
	[Controller] [nvarchar](50) NULL,
	[Visible] [smallint] NULL,
	[Authenticate] [smallint] NULL,
	[CssClass] [nvarchar](100) NULL,
	[SortOrder] [int] NULL,
	[AbsoluteUri] [nvarchar](255) NULL,
	[Active] [smallint] NULL,
	[IconCss] [nvarchar](50) NULL,
 CONSTRAINT [PK_aspnetmenus_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[aspnetmenus_lang]    Script Date: 11/22/2019 10:11:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[aspnetmenus_lang](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MenuId] [int] NULL,
	[Title] [nvarchar](100) NULL,
	[LangCode] [nvarchar](10) NULL,
 CONSTRAINT [PK_aspnetmenus_lang] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[aspnetoperations]    Script Date: 11/22/2019 10:11:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[aspnetoperations](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OperationName] [nvarchar](50) NOT NULL,
	[Enabled] [smallint] NULL,
	[AccessId] [nvarchar](128) NOT NULL,
	[ActionName] [nvarchar](20) NULL,
 CONSTRAINT [PK_aspnetoperations] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[aspnetroles]    Script Date: 11/22/2019 10:11:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[aspnetroles](
	[Id] [nvarchar](128) NOT NULL,
	[Name] [nvarchar](256) NOT NULL,
 CONSTRAINT [PK_aspnetroles_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[aspnettraces]    Script Date: 11/22/2019 10:11:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[aspnettraces](
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
/****** Object:  Table [dbo].[aspnetuserclaims]    Script Date: 11/22/2019 10:11:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[aspnetuserclaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](128) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_aspnetuserclaims_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[aspnetuserlogins]    Script Date: 11/22/2019 10:11:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[aspnetuserlogins](
	[LoginProvider] [nvarchar](128) NOT NULL,
	[ProviderKey] [nvarchar](128) NOT NULL,
	[UserId] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_aspnetuserlogins_LoginProvider] PRIMARY KEY CLUSTERED 
(
	[LoginProvider] ASC,
	[ProviderKey] ASC,
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[aspnetuserroles]    Script Date: 11/22/2019 10:11:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[aspnetuserroles](
	[UserId] [nvarchar](128) NOT NULL,
	[RoleId] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_aspnetuserroles_UserId] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[aspnetusers]    Script Date: 11/22/2019 10:11:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[aspnetusers](
	[Id] [nvarchar](128) NOT NULL,
	[StaffId] [int] IDENTITY(1,1) NOT NULL,
	[ProviderId] [int] NULL,
	[Email] [nvarchar](256) NULL,
	[EmailConfirmed] [smallint] NOT NULL,
	[PasswordHash] [nvarchar](max) NULL,
	[SecurityStamp] [nvarchar](max) NULL,
	[PhoneNumber] [nvarchar](50) NULL,
	[PhoneNumberConfirmed] [smallint] NOT NULL,
	[TwoFactorEnabled] [smallint] NOT NULL,
	[LockoutEndDateUtc] [datetime2](0) NULL,
	[LockoutEnabled] [smallint] NOT NULL,
	[AccessFailedCount] [int] NOT NULL,
	[UserName] [nvarchar](256) NOT NULL,
	[CreatedDateUtc] [datetime2](0) NULL,
	[PasswordHash2] [nvarchar](128) NULL,
	[FullName] [nvarchar](128) NULL,
	[DisplayName] [nvarchar](128) NULL,
	[Avatar] [nvarchar](1000) NULL,
	[OTPType] [nvarchar](20) NULL,
	[Birthday] [datetime] NULL,
	[Sex] [tinyint] NULL,
	[Address] [nvarchar](256) NULL,
	[Note] [nvarchar](1000) NULL,
	[Code] [nchar](10) NULL,
	[StaffCategoryId] [int] NULL,
	[Married] [bit] NULL,
	[IdCard] [nvarchar](20) NULL,
	[Passport] [nvarchar](20) NULL,
	[TaxInfo] [nvarchar](256) NULL,
	[InsurranceInfo] [nvarchar](256) NULL,
	[BankInfo] [nvarchar](500) NULL,
	[Status] [smallint] NULL,
 CONSTRAINT [PK_aspnetusers_Id] PRIMARY KEY CLUSTERED 
(
	[StaffId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[aspnetusertokenkeys]    Script Date: 11/22/2019 10:11:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[aspnetusertokenkeys](
	[UserId] [nvarchar](128) NOT NULL,
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
/****** Object:  Table [dbo].[aspnetusertokenkeys_history]    Script Date: 11/22/2019 10:11:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[aspnetusertokenkeys_history](
	[UserId] [nvarchar](128) NOT NULL,
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
/****** Object:  Table [dbo].[cmn_settings]    Script Date: 11/22/2019 10:11:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[cmn_settings](
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
/****** Object:  Table [dbo].[cmn_sql_errors]    Script Date: 11/22/2019 10:11:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[cmn_sql_errors](
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
/****** Object:  Table [dbo].[tbl_currency]    Script Date: 11/22/2019 10:11:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_currency](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](20) NULL,
	[Name] [nvarchar](128) NULL,
	[CreatedDate] [datetime] NULL,
	[Status] [smallint] NULL,
 CONSTRAINT [PK_tbl_currency] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_device]    Script Date: 11/22/2019 10:11:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_device](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](20) NULL,
	[Name] [nvarchar](128) NULL,
	[CreatedDate] [datetime] NULL,
	[Status] [smallint] NULL,
 CONSTRAINT [PK_tbl_device] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_footer]    Script Date: 11/22/2019 10:11:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_footer](
	[BodyContent] [nvarchar](max) NULL,
	[LangCode] [nvarchar](10) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_ht_resource]    Script Date: 11/22/2019 10:11:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_ht_resource](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ResKey] [nvarchar](128) NULL,
	[ResValue] [nvarchar](1000) NULL,
	[LangCode] [nchar](10) NULL,
 CONSTRAINT [PK_tbl_ht_resource] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_navigation]    Script Date: 11/22/2019 10:11:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_navigation](
	[Id] [int] IDENTITY(47,1) NOT NULL,
	[ParentId] [int] NULL,
	[Area] [nvarchar](20) NULL,
	[Name] [nvarchar](20) NULL,
	[Title] [nvarchar](100) NULL,
	[Desc] [nvarchar](255) NULL,
	[Action] [nvarchar](50) NULL,
	[Controller] [nvarchar](50) NULL,
	[Visible] [smallint] NULL,
	[Authenticate] [smallint] NULL,
	[CssClass] [nvarchar](100) NULL,
	[SortOrder] [int] NULL,
	[AbsoluteUri] [nvarchar](500) NULL,
	[Active] [smallint] NULL,
	[IconCss] [nvarchar](50) NULL,
 CONSTRAINT [PK_tbl_navigation_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_navigation_lang]    Script Date: 11/22/2019 10:11:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_navigation_lang](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[NavigationId] [int] NULL,
	[Title] [nvarchar](100) NULL,
	[AbsoluteUri] [nvarchar](500) NULL,
	[LangCode] [nvarchar](10) NULL,
 CONSTRAINT [PK_navigation_lang] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_page]    Script Date: 11/22/2019 10:11:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_page](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](256) NULL,
	[Controller] [nvarchar](50) NULL,
	[Action] [nvarchar](50) NULL,
	[IsBlankPage] [bit] NULL,
	[PageTemplateId] [int] NULL,
	[CustomTemplate] [nvarchar](max) NULL,
	[SortOrder] [int] NULL,
	[CreatedBy] [int] NULL,
	[CreatedDate] [datetime] NULL,
	[LastUpdatedBy] [int] NULL,
	[LastUpdated] [datetime] NULL,
	[Status] [smallint] NULL,
 CONSTRAINT [PK_tbl_page_layou] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_page_lang]    Script Date: 11/22/2019 10:11:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_page_lang](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PageId] [int] NULL,
	[Title] [nvarchar](256) NULL,
	[Description] [nvarchar](500) NULL,
	[BodyContent] [nvarchar](max) NULL,
	[UrlFriendly] [nvarchar](500) NULL,
	[LangCode] [nvarchar](20) NULL,
 CONSTRAINT [PK_tbl_page_lang] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_page_template]    Script Date: 11/22/2019 10:11:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_page_template](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Widgets] [nvarchar](1000) NULL,
	[IsDefault] [bit] NULL,
	[Status] [smallint] NULL,
 CONSTRAINT [PK_tbl_page_template] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_page_template_lang]    Script Date: 11/22/2019 10:11:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_page_template_lang](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](128) NULL,
	[PageTemplateId] [int] NULL,
	[LangCode] [nvarchar](20) NULL,
 CONSTRAINT [PK_tbl_page_template_lang] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_post]    Script Date: 11/22/2019 10:11:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_post](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](256) NULL,
	[IsHighlights] [bit] NOT NULL,
	[Cover] [nvarchar](500) NULL,
	[CategoryId] [int] NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedBy] [nvarchar](128) NULL,
	[Status] [smallint] NULL,
 CONSTRAINT [PK_tbl_post] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_post_lang]    Script Date: 11/22/2019 10:11:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_post_lang](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PostId] [int] NULL,
	[Title] [nvarchar](256) NULL,
	[Description] [nvarchar](256) NULL,
	[BodyContent] [ntext] NULL,
	[UrlFriendly] [nvarchar](500) NULL,
	[LangCode] [nvarchar](10) NULL,
	[Status] [smallint] NULL,
 CONSTRAINT [PK_tbl_post_lang] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_product]    Script Date: 11/22/2019 10:11:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_product](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](20) NULL,
	[ProductCategoryId] [int] NULL,
	[ProviderId] [int] NULL,
	[Name] [nvarchar](256) NULL,
	[ShortDescription] [nvarchar](256) NULL,
	[Detail] [ntext] NULL,
	[OtherInfo] [nvarchar](1000) NULL,
	[Cost] [float] NULL,
	[SaleOffCost] [float] NULL,
	[UnitId] [int] NULL,
	[CurrencyId] [int] NULL,
	[CreatedBy] [int] NULL,
	[CreatedDate] [datetime] NULL,
	[LastUpdatedBy] [int] NULL,
	[LastUpdated] [datetime] NULL,
	[Status] [smallint] NULL,
	[MinInventory] [float] NULL,
 CONSTRAINT [PK_tbl_product] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_product_category]    Script Date: 11/22/2019 10:11:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_product_category](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](20) NULL,
	[Name] [nvarchar](128) NULL,
	[CreatedDate] [datetime] NULL,
	[Status] [smallint] NULL,
 CONSTRAINT [PK_tbl_product_category] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_product_image]    Script Date: 11/22/2019 10:11:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_product_image](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProductId] [int] NULL,
	[Name] [nvarchar](256) NULL,
	[Url] [nvarchar](500) NULL,
	[CreatedDate] [nvarchar](1000) NULL,
 CONSTRAINT [PK_tbl_product_image] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_product_property]    Script Date: 11/22/2019 10:11:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_product_property](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProductId] [int] NULL,
	[PropertyCategoryId] [int] NULL,
	[PropertyId] [int] NULL,
 CONSTRAINT [PK_tbl_product_property] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_productcat_propertycat]    Script Date: 11/22/2019 10:11:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_productcat_propertycat](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProductCategoryId] [int] NULL,
	[PropertyCategoryId] [int] NULL,
 CONSTRAINT [PK_tbl_productcat_propertycat] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_project]    Script Date: 11/22/2019 10:11:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_project](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](256) NULL,
	[Cover] [nvarchar](500) NULL,
	[CategoryId] [int] NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedBy] [nvarchar](128) NULL,
	[MetaData] [nvarchar](max) NULL,
	[Status] [smallint] NULL,
 CONSTRAINT [PK_tbl_project] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_project_category]    Script Date: 11/22/2019 10:11:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_project_category](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](128) NOT NULL,
	[Code] [nvarchar](20) NULL,
	[Icon] [nvarchar](20) NULL,
	[CreatedBy] [nvarchar](128) NULL,
	[CreatedDate] [datetime] NULL,
	[LastUpdated] [datetime] NULL,
	[LastUpdatedBy] [nvarchar](128) NULL,
	[Status] [smallint] NULL,
	[Description] [nvarchar](1000) NULL,
	[ParentId] [int] NULL,
	[Cover] [nvarchar](500) NULL,
 CONSTRAINT [PK_tbl_project_category] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_project_category_lang]    Script Date: 11/22/2019 10:11:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_project_category_lang](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[LangCode] [nvarchar](20) NULL,
	[Name] [nvarchar](128) NULL,
	[ProjectCategoryId] [int] NULL,
	[Description] [nvarchar](1000) NULL,
	[UrlFriendly] [nvarchar](500) NULL,
 CONSTRAINT [PK_tbl_project_category_lang] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_project_image]    Script Date: 11/22/2019 10:11:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_project_image](
	[Id] [nvarchar](128) NOT NULL,
	[ProjectId] [int] NULL,
	[Name] [nvarchar](256) NULL,
	[Url] [nvarchar](500) NULL,
	[CreatedDate] [datetime] NULL,
 CONSTRAINT [PK_tbl_project_image] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_project_lang]    Script Date: 11/22/2019 10:11:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_project_lang](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProjectId] [int] NULL,
	[Title] [nvarchar](256) NULL,
	[Description] [nvarchar](256) NULL,
	[BodyContent] [ntext] NULL,
	[UrlFriendly] [nvarchar](500) NULL,
	[LangCode] [nvarchar](10) NULL,
	[Status] [smallint] NULL,
 CONSTRAINT [PK_tbl_project_lang] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_property]    Script Date: 11/22/2019 10:11:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_property](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PropertyCategoryId] [int] NOT NULL,
	[Code] [nvarchar](20) NULL,
	[Name] [nvarchar](128) NULL,
	[CreatedDate] [datetime] NULL,
	[Status] [smallint] NULL,
 CONSTRAINT [PK_tbl_property] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_property_category]    Script Date: 11/22/2019 10:11:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_property_category](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](20) NULL,
	[Name] [nvarchar](128) NULL,
	[CreatedDate] [datetime] NULL,
	[Status] [smallint] NULL,
 CONSTRAINT [PK_tbl_property_category] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_provider]    Script Date: 11/22/2019 10:11:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_provider](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](20) NULL,
	[Name] [nvarchar](128) NULL,
	[Address] [nvarchar](256) NULL,
	[Email] [nvarchar](50) NULL,
	[Phone] [nvarchar](20) NULL,
	[Lat] [nvarchar](30) NULL,
	[Long] [nvarchar](30) NULL,
	[CreatedDate] [datetime] NULL,
	[Status] [smallint] NULL,
 CONSTRAINT [PK_tbl_provider] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_unit]    Script Date: 11/22/2019 10:11:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_unit](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](20) NULL,
	[Name] [nvarchar](128) NULL,
	[CreatedDate] [datetime] NULL,
	[Status] [smallint] NULL,
 CONSTRAINT [PK_tbl_unit] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_widget]    Script Date: 11/22/2019 10:11:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_widget](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Controller] [nvarchar](50) NULL,
	[Action] [nvarchar](50) NULL,
	[Status] [smallint] NULL,
 CONSTRAINT [PK_tbl_widget] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_widget_lang]    Script Date: 11/22/2019 10:11:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_widget_lang](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[WidgetId] [int] NULL,
	[Name] [nvarchar](128) NULL,
	[Description] [nvarchar](256) NULL,
	[LangCode] [nvarchar](20) NULL,
 CONSTRAINT [PK_tbl_widget_lang] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[aspnetaccess] ([Id], [AccessName], [Active], [Description]) VALUES (N'141C6B6F-8E1C-4806-A62F-59FC654BC731', N'MyAccount', 1, N'Account detail')
INSERT [dbo].[aspnetaccess] ([Id], [AccessName], [Active], [Description]) VALUES (N'29CD28D0-7701-4425-B0C2-BD1C609B1661', N'Access', 1, N'Quản lý các controllers')
INSERT [dbo].[aspnetaccess] ([Id], [AccessName], [Active], [Description]) VALUES (N'2FDE3424-C325-4391-9A36-CE18B1F829E1', N'Company', 1, N'Quản lý công ty')
INSERT [dbo].[aspnetaccess] ([Id], [AccessName], [Active], [Description]) VALUES (N'32B7D7E6-6EA3-4AD6-8B20-8F047F4F45FA', N'Footer', 1, N'Quản lý chân trang')
INSERT [dbo].[aspnetaccess] ([Id], [AccessName], [Active], [Description]) VALUES (N'357BEB69-0425-4DE7-B425-B05C44881F6E', N'ProjectCategory', 1, N'Danh mục loại dự án')
INSERT [dbo].[aspnetaccess] ([Id], [AccessName], [Active], [Description]) VALUES (N'4F76273F-33D6-4282-B786-0E29103C03F5', N'SalaryFilter', 1, N'Quản lý mức lương')
INSERT [dbo].[aspnetaccess] ([Id], [AccessName], [Active], [Description]) VALUES (N'59003A6E-9C0B-4E45-B8D8-A932EDDB2F26', N'FrontEndSystem', 1, N'Quản lý cấu hình trang ngoài')
INSERT [dbo].[aspnetaccess] ([Id], [AccessName], [Active], [Description]) VALUES (N'6823D1F2-3B68-4ADF-B324-624D47A16EB5', N'Suggest', 1, N'Quản lý biểu mẫu')
INSERT [dbo].[aspnetaccess] ([Id], [AccessName], [Active], [Description]) VALUES (N'6D9B1DF1-BCD0-4A73-8DBF-6DF2A313E3BC', N'Page', 1, N'Quản lý trang')
INSERT [dbo].[aspnetaccess] ([Id], [AccessName], [Active], [Description]) VALUES (N'7947E54F-9CA8-4725-B5BD-8C9C2611171F', N'Job', 1, N'Quản lý công việc')
INSERT [dbo].[aspnetaccess] ([Id], [AccessName], [Active], [Description]) VALUES (N'7FDA2C88-0928-4625-A83A-E2D716C14359', N'Home', 1, N'Trang chủ ')
INSERT [dbo].[aspnetaccess] ([Id], [AccessName], [Active], [Description]) VALUES (N'824085C0-CBC0-4CE0-AAA5-9D730781E1D5', N'UsersAdmin', 1, N'Quản lý users')
INSERT [dbo].[aspnetaccess] ([Id], [AccessName], [Active], [Description]) VALUES (N'85D4CBB4-D2BB-4535-91C4-5B95B3442B99', N'Post', 1, N'Quản lý bài viết')
INSERT [dbo].[aspnetaccess] ([Id], [AccessName], [Active], [Description]) VALUES (N'99648A3A-A3F8-497E-9299-75E41DBB2E8A', N'Menu', 1, N'Quản lý menu')
INSERT [dbo].[aspnetaccess] ([Id], [AccessName], [Active], [Description]) VALUES (N'A8C7D090-B6CF-4A0E-86B3-448FB7F13E37', N'System', 1, N'Hệ thống')
INSERT [dbo].[aspnetaccess] ([Id], [AccessName], [Active], [Description]) VALUES (N'B229CAE9-333E-49B2-948B-ED160D244BB5', N'AccessRoles', 1, N'Phân quyền')
INSERT [dbo].[aspnetaccess] ([Id], [AccessName], [Active], [Description]) VALUES (N'BE146B14-1A86-49F3-8344-C5C9D75D839A', N'Navigation', 1, N'Quản lý điều hướng')
INSERT [dbo].[aspnetaccess] ([Id], [AccessName], [Active], [Description]) VALUES (N'C2FB41CC-4ED8-46FF-84C6-1164D8E598FF', N'Project', 1, N'Quản lý dự án')
INSERT [dbo].[aspnetaccess] ([Id], [AccessName], [Active], [Description]) VALUES (N'CB21302B-7347-41F3-B716-1F9E3F173C05', N'Function', 1, N'Quản lý chức năng')
INSERT [dbo].[aspnetaccess] ([Id], [AccessName], [Active], [Description]) VALUES (N'D120F343-8F75-41BA-A8CB-97410C122570', N'Product', 1, N'Item manage')
INSERT [dbo].[aspnetaccess] ([Id], [AccessName], [Active], [Description]) VALUES (N'D3C3C55D-DFD2-4805-8839-6A7F965C808F', N'RolesAdmin', 1, N'Quản lý nhóm users')
INSERT [dbo].[aspnetaccess] ([Id], [AccessName], [Active], [Description]) VALUES (N'E81AF9AD-5269-4C72-BC0E-47DC2EECC2EA', N'PropertyCategory', 1, N'Quản lý loại thuộc tính')
INSERT [dbo].[aspnetaccess] ([Id], [AccessName], [Active], [Description]) VALUES (N'E94B2CC5-00EE-4922-AA9B-4CAB9667E915', N'Unit', 1, N'Danh mục đơn vị')
INSERT [dbo].[aspnetaccess] ([Id], [AccessName], [Active], [Description]) VALUES (N'F7B02B30-723D-4524-B044-A27ACD58CA8F', N'TypeSuggest', 1, N'Quản lý loại biểu mẫu')
INSERT [dbo].[aspnetaccess] ([Id], [AccessName], [Active], [Description]) VALUES (N'FCE958A1-2DA7-418D-A821-1A862A8D2483', N'Agency', 1, N'Quản lý đối tác')
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'ae7bdc1f-592b-4742-8ddb-a0b6e174045a', 9)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'ae7bdc1f-592b-4742-8ddb-a0b6e174045a', 2114)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'ae7bdc1f-592b-4742-8ddb-a0b6e174045a', 2115)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'ae7bdc1f-592b-4742-8ddb-a0b6e174045a', 2116)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'ae7bdc1f-592b-4742-8ddb-a0b6e174045a', 2119)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'ae7bdc1f-592b-4742-8ddb-a0b6e174045a', 2120)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'ae7bdc1f-592b-4742-8ddb-a0b6e174045a', 2127)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'b771e7d7-5b2b-4b77-a8b8-272392c6e411', 2151)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'd0732bf3-f54a-44b0-8f3a-12606e977e34', 1)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'd0732bf3-f54a-44b0-8f3a-12606e977e34', 2)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'd0732bf3-f54a-44b0-8f3a-12606e977e34', 3)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'd0732bf3-f54a-44b0-8f3a-12606e977e34', 9)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'd0732bf3-f54a-44b0-8f3a-12606e977e34', 14)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'd0732bf3-f54a-44b0-8f3a-12606e977e34', 15)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'd0732bf3-f54a-44b0-8f3a-12606e977e34', 17)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'd0732bf3-f54a-44b0-8f3a-12606e977e34', 18)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'd0732bf3-f54a-44b0-8f3a-12606e977e34', 19)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'd0732bf3-f54a-44b0-8f3a-12606e977e34', 20)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'd0732bf3-f54a-44b0-8f3a-12606e977e34', 22)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'd0732bf3-f54a-44b0-8f3a-12606e977e34', 23)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'd0732bf3-f54a-44b0-8f3a-12606e977e34', 24)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'd0732bf3-f54a-44b0-8f3a-12606e977e34', 25)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'd0732bf3-f54a-44b0-8f3a-12606e977e34', 26)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'd0732bf3-f54a-44b0-8f3a-12606e977e34', 27)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'd0732bf3-f54a-44b0-8f3a-12606e977e34', 28)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'd0732bf3-f54a-44b0-8f3a-12606e977e34', 29)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'd0732bf3-f54a-44b0-8f3a-12606e977e34', 30)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'd0732bf3-f54a-44b0-8f3a-12606e977e34', 32)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'd0732bf3-f54a-44b0-8f3a-12606e977e34', 33)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'd0732bf3-f54a-44b0-8f3a-12606e977e34', 1094)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'd0732bf3-f54a-44b0-8f3a-12606e977e34', 2101)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'd0732bf3-f54a-44b0-8f3a-12606e977e34', 2102)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'd0732bf3-f54a-44b0-8f3a-12606e977e34', 2104)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'd0732bf3-f54a-44b0-8f3a-12606e977e34', 2110)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'd0732bf3-f54a-44b0-8f3a-12606e977e34', 2111)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'd0732bf3-f54a-44b0-8f3a-12606e977e34', 2112)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'd0732bf3-f54a-44b0-8f3a-12606e977e34', 2113)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'd0732bf3-f54a-44b0-8f3a-12606e977e34', 2114)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'd0732bf3-f54a-44b0-8f3a-12606e977e34', 2115)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'd0732bf3-f54a-44b0-8f3a-12606e977e34', 2116)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'd0732bf3-f54a-44b0-8f3a-12606e977e34', 2119)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'd0732bf3-f54a-44b0-8f3a-12606e977e34', 2120)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'd0732bf3-f54a-44b0-8f3a-12606e977e34', 2125)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'd0732bf3-f54a-44b0-8f3a-12606e977e34', 2126)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'd0732bf3-f54a-44b0-8f3a-12606e977e34', 2127)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'd0732bf3-f54a-44b0-8f3a-12606e977e34', 2130)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'd0732bf3-f54a-44b0-8f3a-12606e977e34', 2131)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'd0732bf3-f54a-44b0-8f3a-12606e977e34', 2132)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'd0732bf3-f54a-44b0-8f3a-12606e977e34', 2133)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'd0732bf3-f54a-44b0-8f3a-12606e977e34', 2134)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'd0732bf3-f54a-44b0-8f3a-12606e977e34', 2135)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'd0732bf3-f54a-44b0-8f3a-12606e977e34', 2136)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'd0732bf3-f54a-44b0-8f3a-12606e977e34', 2137)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'd0732bf3-f54a-44b0-8f3a-12606e977e34', 2138)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'd0732bf3-f54a-44b0-8f3a-12606e977e34', 2139)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'd0732bf3-f54a-44b0-8f3a-12606e977e34', 2140)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'd0732bf3-f54a-44b0-8f3a-12606e977e34', 2141)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'd0732bf3-f54a-44b0-8f3a-12606e977e34', 2142)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'd0732bf3-f54a-44b0-8f3a-12606e977e34', 2143)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'd0732bf3-f54a-44b0-8f3a-12606e977e34', 2144)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'd0732bf3-f54a-44b0-8f3a-12606e977e34', 2145)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'd0732bf3-f54a-44b0-8f3a-12606e977e34', 2146)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'd0732bf3-f54a-44b0-8f3a-12606e977e34', 2147)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'd0732bf3-f54a-44b0-8f3a-12606e977e34', 2148)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'd0732bf3-f54a-44b0-8f3a-12606e977e34', 2149)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'd0732bf3-f54a-44b0-8f3a-12606e977e34', 2150)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'd0732bf3-f54a-44b0-8f3a-12606e977e34', 2151)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'd0732bf3-f54a-44b0-8f3a-12606e977e34', 2152)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'd0732bf3-f54a-44b0-8f3a-12606e977e34', 2153)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'd0732bf3-f54a-44b0-8f3a-12606e977e34', 2154)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'd0732bf3-f54a-44b0-8f3a-12606e977e34', 2155)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'd8a6f35e-1870-452b-bd88-9794d3fce6b2', 9)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'fe537142-5123-4dc1-8051-53802ce6ad7d', 9)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'fe537142-5123-4dc1-8051-53802ce6ad7d', 2119)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'fe537142-5123-4dc1-8051-53802ce6ad7d', 2120)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'fe537142-5123-4dc1-8051-53802ce6ad7d', 2133)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'fe537142-5123-4dc1-8051-53802ce6ad7d', 2134)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'fe537142-5123-4dc1-8051-53802ce6ad7d', 2135)
INSERT [dbo].[aspnetaccessroles] ([RoleId], [OperationId]) VALUES (N'fe537142-5123-4dc1-8051-53802ce6ad7d', 2141)
SET IDENTITY_INSERT [dbo].[aspnetactivitylog] ON 

INSERT [dbo].[aspnetactivitylog] ([ActivityLogId], [UserId], [ActivityText], [TargetType], [TargetId], [IPAddress], [ActivityDate], [ActivityType]) VALUES (4962, N'94E7515B-09B1-4B90-872E-6D544BA4A339', N'Updated the access [Name: Admin]', N'Access', N'7FDA2C88-0928-4625-A83A-E2D716C14359', N'::1', CAST(N'2019-06-18T02:59:28.763' AS DateTime), N'UpdateAccess')
INSERT [dbo].[aspnetactivitylog] ([ActivityLogId], [UserId], [ActivityText], [TargetType], [TargetId], [IPAddress], [ActivityDate], [ActivityType]) VALUES (4963, N'94E7515B-09B1-4B90-872E-6D544BA4A339', N'Updated the access [Name: Admin]', N'Access', N'7FDA2C88-0928-4625-A83A-E2D716C14359', N'::1', CAST(N'2019-06-18T03:01:08.580' AS DateTime), N'UpdateAccess')
INSERT [dbo].[aspnetactivitylog] ([ActivityLogId], [UserId], [ActivityText], [TargetType], [TargetId], [IPAddress], [ActivityDate], [ActivityType]) VALUES (4964, N'94E7515B-09B1-4B90-872E-6D544BA4A339', N'Updated the access [Name: Admin]', N'Access', N'7FDA2C88-0928-4625-A83A-E2D716C14359', N'::1', CAST(N'2019-06-18T03:03:24.773' AS DateTime), N'UpdateAccess')
INSERT [dbo].[aspnetactivitylog] ([ActivityLogId], [UserId], [ActivityText], [TargetType], [TargetId], [IPAddress], [ActivityDate], [ActivityType]) VALUES (4965, N'94E7515B-09B1-4B90-872E-6D544BA4A339', N'Create new access [Name: Demo]', N'Access', NULL, N'::1', CAST(N'2019-06-18T03:05:58.900' AS DateTime), N'CreateAccess')
INSERT [dbo].[aspnetactivitylog] ([ActivityLogId], [UserId], [ActivityText], [TargetType], [TargetId], [IPAddress], [ActivityDate], [ActivityType]) VALUES (4966, N'94E7515B-09B1-4B90-872E-6D544BA4A339', N'Updated the access [Name: Demo]', N'Access', N'14F886B0-E96D-4015-A176-6BE2C5EE757C', N'::1', CAST(N'2019-06-18T03:06:42.817' AS DateTime), N'UpdateAccess')
INSERT [dbo].[aspnetactivitylog] ([ActivityLogId], [UserId], [ActivityText], [TargetType], [TargetId], [IPAddress], [ActivityDate], [ActivityType]) VALUES (4967, N'94E7515B-09B1-4B90-872E-6D544BA4A339', N'Delete the Access [Id: 14F886B0-E96D-4015-A176-6BE2C5EE757C]', N'Access', N'14F886B0-E96D-4015-A176-6BE2C5EE757C', N'::1', CAST(N'2019-06-18T03:06:50.750' AS DateTime), N'DeleteAccess')
INSERT [dbo].[aspnetactivitylog] ([ActivityLogId], [UserId], [ActivityText], [TargetType], [TargetId], [IPAddress], [ActivityDate], [ActivityType]) VALUES (4968, N'94E7515B-09B1-4B90-872E-6D544BA4A339', N'Create new role device [Name: Tester]', N'RolesAdmin', N'0', N'::1', CAST(N'2019-06-18T03:10:28.620' AS DateTime), N'CreateRole')
INSERT [dbo].[aspnetactivitylog] ([ActivityLogId], [UserId], [ActivityText], [TargetType], [TargetId], [IPAddress], [ActivityDate], [ActivityType]) VALUES (4969, N'94E7515B-09B1-4B90-872E-6D544BA4A339', N'Updated the role [Name: Tester]', N'RolesAdmin', N'a5866dfa-08f0-4605-857f-827782df025a', N'::1', CAST(N'2019-06-18T03:12:13.057' AS DateTime), N'UpdateRole')
INSERT [dbo].[aspnetactivitylog] ([ActivityLogId], [UserId], [ActivityText], [TargetType], [TargetId], [IPAddress], [ActivityDate], [ActivityType]) VALUES (4970, N'94E7515B-09B1-4B90-872E-6D544BA4A339', N'Create new role device [Name: Content]', N'RolesAdmin', N'0', N'::1', CAST(N'2019-06-18T03:12:48.103' AS DateTime), N'CreateRole')
INSERT [dbo].[aspnetactivitylog] ([ActivityLogId], [UserId], [ActivityText], [TargetType], [TargetId], [IPAddress], [ActivityDate], [ActivityType]) VALUES (4971, N'94E7515B-09B1-4B90-872E-6D544BA4A339', N'Create new function [Helo of Unit]', N'Function', N'0', N'::1', CAST(N'2019-06-18T03:14:01.827' AS DateTime), N'CreateFunction')
INSERT [dbo].[aspnetactivitylog] ([ActivityLogId], [UserId], [ActivityText], [TargetType], [TargetId], [IPAddress], [ActivityDate], [ActivityType]) VALUES (4972, N'94E7515B-09B1-4B90-872E-6D544BA4A339', N'Updated the function [Helo of Unit]', N'Function', N'2129', N'::1', CAST(N'2019-06-18T03:14:08.783' AS DateTime), N'UpdateFunction')
INSERT [dbo].[aspnetactivitylog] ([ActivityLogId], [UserId], [ActivityText], [TargetType], [TargetId], [IPAddress], [ActivityDate], [ActivityType]) VALUES (4973, N'94E7515B-09B1-4B90-872E-6D544BA4A339', N'Delete the function [Id: 2129]', N'Function', N'2129', N'::1', CAST(N'2019-06-18T03:14:16.127' AS DateTime), N'DeleteFunction')
INSERT [dbo].[aspnetactivitylog] ([ActivityLogId], [UserId], [ActivityText], [TargetType], [TargetId], [IPAddress], [ActivityDate], [ActivityType]) VALUES (4974, N'94E7515B-09B1-4B90-872E-6D544BA4A339', N'Delete the Access [Id: 4E77F4EA-395B-4EB8-BA44-14EB3E875D04]', N'Access', N'4E77F4EA-395B-4EB8-BA44-14EB3E875D04', N'::1', CAST(N'2019-06-20T07:37:34.567' AS DateTime), N'DeleteAccess')
INSERT [dbo].[aspnetactivitylog] ([ActivityLogId], [UserId], [ActivityText], [TargetType], [TargetId], [IPAddress], [ActivityDate], [ActivityType]) VALUES (4975, N'94E7515B-09B1-4B90-872E-6D544BA4A339', N'Delete the Access [Id: 12938940-68D8-448B-A2F0-C5D8CD83C358]', N'Access', N'12938940-68D8-448B-A2F0-C5D8CD83C358', N'::1', CAST(N'2019-06-20T07:37:47.293' AS DateTime), N'DeleteAccess')
INSERT [dbo].[aspnetactivitylog] ([ActivityLogId], [UserId], [ActivityText], [TargetType], [TargetId], [IPAddress], [ActivityDate], [ActivityType]) VALUES (4976, N'94E7515B-09B1-4B90-872E-6D544BA4A339', N'Create new access [Name: Navigation]', N'Access', NULL, N'::1', CAST(N'2019-06-20T07:38:27.777' AS DateTime), N'CreateAccess')
INSERT [dbo].[aspnetactivitylog] ([ActivityLogId], [UserId], [ActivityText], [TargetType], [TargetId], [IPAddress], [ActivityDate], [ActivityType]) VALUES (4977, N'94E7515B-09B1-4B90-872E-6D544BA4A339', N'Create new function [Index of Navigation]', N'Function', N'0', N'::1', CAST(N'2019-06-20T07:39:06.500' AS DateTime), N'CreateFunction')
INSERT [dbo].[aspnetactivitylog] ([ActivityLogId], [UserId], [ActivityText], [TargetType], [TargetId], [IPAddress], [ActivityDate], [ActivityType]) VALUES (4978, N'94E7515B-09B1-4B90-872E-6D544BA4A339', N'Create new access [Name: Post]', N'Access', NULL, N'::1', CAST(N'2019-06-21T02:58:10.247' AS DateTime), N'CreateAccess')
INSERT [dbo].[aspnetactivitylog] ([ActivityLogId], [UserId], [ActivityText], [TargetType], [TargetId], [IPAddress], [ActivityDate], [ActivityType]) VALUES (4979, N'94E7515B-09B1-4B90-872E-6D544BA4A339', N'Create new function [Index of Post]', N'Function', N'0', N'::1', CAST(N'2019-06-21T02:58:53.803' AS DateTime), N'CreateFunction')
INSERT [dbo].[aspnetactivitylog] ([ActivityLogId], [UserId], [ActivityText], [TargetType], [TargetId], [IPAddress], [ActivityDate], [ActivityType]) VALUES (4980, N'94E7515B-09B1-4B90-872E-6D544BA4A339', N'Create new access [Name: Page]', N'Access', NULL, N'::1', CAST(N'2019-06-25T02:31:40.743' AS DateTime), N'CreateAccess')
INSERT [dbo].[aspnetactivitylog] ([ActivityLogId], [UserId], [ActivityText], [TargetType], [TargetId], [IPAddress], [ActivityDate], [ActivityType]) VALUES (4981, N'94E7515B-09B1-4B90-872E-6D544BA4A339', N'Create new function [Index of Page]', N'Function', N'0', N'::1', CAST(N'2019-06-25T02:32:00.967' AS DateTime), N'CreateFunction')
INSERT [dbo].[aspnetactivitylog] ([ActivityLogId], [UserId], [ActivityText], [TargetType], [TargetId], [IPAddress], [ActivityDate], [ActivityType]) VALUES (4982, N'94E7515B-09B1-4B90-872E-6D544BA4A339', N'Create new access [Name: Project]', N'Access', NULL, N'::1', CAST(N'2019-07-09T02:26:07.710' AS DateTime), N'CreateAccess')
INSERT [dbo].[aspnetactivitylog] ([ActivityLogId], [UserId], [ActivityText], [TargetType], [TargetId], [IPAddress], [ActivityDate], [ActivityType]) VALUES (4983, N'94E7515B-09B1-4B90-872E-6D544BA4A339', N'Create new function [Index of Project]', N'Function', N'0', N'::1', CAST(N'2019-07-09T02:26:32.860' AS DateTime), N'CreateFunction')
INSERT [dbo].[aspnetactivitylog] ([ActivityLogId], [UserId], [ActivityText], [TargetType], [TargetId], [IPAddress], [ActivityDate], [ActivityType]) VALUES (4984, N'94E7515B-09B1-4B90-872E-6D544BA4A339', N'Create new function [Create of Project]', N'Function', N'0', N'::1', CAST(N'2019-07-09T02:26:43.947' AS DateTime), N'CreateFunction')
INSERT [dbo].[aspnetactivitylog] ([ActivityLogId], [UserId], [ActivityText], [TargetType], [TargetId], [IPAddress], [ActivityDate], [ActivityType]) VALUES (4985, N'94E7515B-09B1-4B90-872E-6D544BA4A339', N'Create new function [Edit of Project]', N'Function', N'0', N'::1', CAST(N'2019-07-09T02:26:59.330' AS DateTime), N'CreateFunction')
INSERT [dbo].[aspnetactivitylog] ([ActivityLogId], [UserId], [ActivityText], [TargetType], [TargetId], [IPAddress], [ActivityDate], [ActivityType]) VALUES (4986, N'94E7515B-09B1-4B90-872E-6D544BA4A339', N'Create new access [Name: ProjectCategory]', N'Access', NULL, N'::1', CAST(N'2019-07-10T02:50:12.900' AS DateTime), N'CreateAccess')
INSERT [dbo].[aspnetactivitylog] ([ActivityLogId], [UserId], [ActivityText], [TargetType], [TargetId], [IPAddress], [ActivityDate], [ActivityType]) VALUES (4987, N'94E7515B-09B1-4B90-872E-6D544BA4A339', N'Create new function [Index of ProjectCategory]', N'Function', N'0', N'::1', CAST(N'2019-07-10T02:50:32.870' AS DateTime), N'CreateFunction')
INSERT [dbo].[aspnetactivitylog] ([ActivityLogId], [UserId], [ActivityText], [TargetType], [TargetId], [IPAddress], [ActivityDate], [ActivityType]) VALUES (4988, N'94E7515B-09B1-4B90-872E-6D544BA4A339', N'Create new function [Create of ProjectCategory]', N'Function', N'0', N'::1', CAST(N'2019-07-10T02:50:44.027' AS DateTime), N'CreateFunction')
INSERT [dbo].[aspnetactivitylog] ([ActivityLogId], [UserId], [ActivityText], [TargetType], [TargetId], [IPAddress], [ActivityDate], [ActivityType]) VALUES (4989, N'94E7515B-09B1-4B90-872E-6D544BA4A339', N'Create new function [Edit of ProjectCategory]', N'Function', N'0', N'::1', CAST(N'2019-07-10T02:50:51.897' AS DateTime), N'CreateFunction')
INSERT [dbo].[aspnetactivitylog] ([ActivityLogId], [UserId], [ActivityText], [TargetType], [TargetId], [IPAddress], [ActivityDate], [ActivityType]) VALUES (4990, N'94E7515B-09B1-4B90-872E-6D544BA4A339', N'Create new access [Name: Footer]', N'Access', NULL, N'::1', CAST(N'2019-08-07T06:47:31.473' AS DateTime), N'CreateAccess')
INSERT [dbo].[aspnetactivitylog] ([ActivityLogId], [UserId], [ActivityText], [TargetType], [TargetId], [IPAddress], [ActivityDate], [ActivityType]) VALUES (4991, N'94E7515B-09B1-4B90-872E-6D544BA4A339', N'Create new function [Index of Footer]', N'Function', N'0', N'::1', CAST(N'2019-08-07T06:48:08.400' AS DateTime), N'CreateFunction')
INSERT [dbo].[aspnetactivitylog] ([ActivityLogId], [UserId], [ActivityText], [TargetType], [TargetId], [IPAddress], [ActivityDate], [ActivityType]) VALUES (4992, N'94E7515B-09B1-4B90-872E-6D544BA4A339', N'Create new access [Name: FrontEndSystem]', N'Access', NULL, N'::1', CAST(N'2019-08-23T01:52:29.063' AS DateTime), N'CreateAccess')
INSERT [dbo].[aspnetactivitylog] ([ActivityLogId], [UserId], [ActivityText], [TargetType], [TargetId], [IPAddress], [ActivityDate], [ActivityType]) VALUES (4993, N'94E7515B-09B1-4B90-872E-6D544BA4A339', N'Create new function [FrontEndSettings of FrontEndSystem]', N'Function', N'0', N'::1', CAST(N'2019-08-23T01:52:59.147' AS DateTime), N'CreateFunction')
INSERT [dbo].[aspnetactivitylog] ([ActivityLogId], [UserId], [ActivityText], [TargetType], [TargetId], [IPAddress], [ActivityDate], [ActivityType]) VALUES (4994, N'94E7515B-09B1-4B90-872E-6D544BA4A339', N'Create new access [Name: Agency]', N'Access', NULL, N'::1', CAST(N'2019-08-23T02:40:22.177' AS DateTime), N'CreateAccess')
INSERT [dbo].[aspnetactivitylog] ([ActivityLogId], [UserId], [ActivityText], [TargetType], [TargetId], [IPAddress], [ActivityDate], [ActivityType]) VALUES (4995, N'94E7515B-09B1-4B90-872E-6D544BA4A339', N'Create new function [Index of Agency]', N'Function', N'0', N'::1', CAST(N'2019-08-23T02:40:44.477' AS DateTime), N'CreateFunction')
INSERT [dbo].[aspnetactivitylog] ([ActivityLogId], [UserId], [ActivityText], [TargetType], [TargetId], [IPAddress], [ActivityDate], [ActivityType]) VALUES (4996, N'94E7515B-09B1-4B90-872E-6D544BA4A339', N'Updated the access [Name: Home]', N'Access', N'7FDA2C88-0928-4625-A83A-E2D716C14359', N'::1', CAST(N'2019-09-25T08:38:09.867' AS DateTime), N'UpdateAccess')
INSERT [dbo].[aspnetactivitylog] ([ActivityLogId], [UserId], [ActivityText], [TargetType], [TargetId], [IPAddress], [ActivityDate], [ActivityType]) VALUES (4997, N'94E7515B-09B1-4B90-872E-6D544BA4A339', N'Create new access [Name: Company]', N'Access', NULL, N'::1', CAST(N'2019-09-25T08:51:10.843' AS DateTime), N'CreateAccess')
INSERT [dbo].[aspnetactivitylog] ([ActivityLogId], [UserId], [ActivityText], [TargetType], [TargetId], [IPAddress], [ActivityDate], [ActivityType]) VALUES (4998, N'94E7515B-09B1-4B90-872E-6D544BA4A339', N'Create new function [Index of Company]', N'Function', N'0', N'::1', CAST(N'2019-09-25T08:53:24.020' AS DateTime), N'CreateFunction')
INSERT [dbo].[aspnetactivitylog] ([ActivityLogId], [UserId], [ActivityText], [TargetType], [TargetId], [IPAddress], [ActivityDate], [ActivityType]) VALUES (4999, N'94E7515B-09B1-4B90-872E-6D544BA4A339', N'Create new access [Name: SalaryFilter]', N'Access', NULL, N'::1', CAST(N'2019-10-02T04:19:42.803' AS DateTime), N'CreateAccess')
INSERT [dbo].[aspnetactivitylog] ([ActivityLogId], [UserId], [ActivityText], [TargetType], [TargetId], [IPAddress], [ActivityDate], [ActivityType]) VALUES (5000, N'94E7515B-09B1-4B90-872E-6D544BA4A339', N'Create new function [Index of SalaryFilter]', N'Function', N'0', N'::1', CAST(N'2019-10-02T04:19:57.193' AS DateTime), N'CreateFunction')
INSERT [dbo].[aspnetactivitylog] ([ActivityLogId], [UserId], [ActivityText], [TargetType], [TargetId], [IPAddress], [ActivityDate], [ActivityType]) VALUES (5001, N'94E7515B-09B1-4B90-872E-6D544BA4A339', N'Create new function [Edit of SalaryFilter]', N'Function', N'0', N'::1', CAST(N'2019-10-02T04:41:06.097' AS DateTime), N'CreateFunction')
INSERT [dbo].[aspnetactivitylog] ([ActivityLogId], [UserId], [ActivityText], [TargetType], [TargetId], [IPAddress], [ActivityDate], [ActivityType]) VALUES (5002, N'94E7515B-09B1-4B90-872E-6D544BA4A339', N'Create new function [Create of SalaryFilter]', N'Function', N'0', N'::1', CAST(N'2019-10-02T04:41:22.347' AS DateTime), N'CreateFunction')
INSERT [dbo].[aspnetactivitylog] ([ActivityLogId], [UserId], [ActivityText], [TargetType], [TargetId], [IPAddress], [ActivityDate], [ActivityType]) VALUES (5003, N'94E7515B-09B1-4B90-872E-6D544BA4A339', N'Create new function [Delete of SalaryFilter]', N'Function', N'0', N'::1', CAST(N'2019-10-02T04:41:32.330' AS DateTime), N'CreateFunction')
INSERT [dbo].[aspnetactivitylog] ([ActivityLogId], [UserId], [ActivityText], [TargetType], [TargetId], [IPAddress], [ActivityDate], [ActivityType]) VALUES (5004, N'94E7515B-09B1-4B90-872E-6D544BA4A339', N'Create new access [Name: Suggest]', N'Access', NULL, N'::1', CAST(N'2019-10-02T04:41:48.543' AS DateTime), N'CreateAccess')
INSERT [dbo].[aspnetactivitylog] ([ActivityLogId], [UserId], [ActivityText], [TargetType], [TargetId], [IPAddress], [ActivityDate], [ActivityType]) VALUES (5005, N'94E7515B-09B1-4B90-872E-6D544BA4A339', N'Create new function [Index of Suggest]', N'Function', N'0', N'::1', CAST(N'2019-10-02T04:42:05.597' AS DateTime), N'CreateFunction')
INSERT [dbo].[aspnetactivitylog] ([ActivityLogId], [UserId], [ActivityText], [TargetType], [TargetId], [IPAddress], [ActivityDate], [ActivityType]) VALUES (5006, N'94E7515B-09B1-4B90-872E-6D544BA4A339', N'Create new function [Edit of Suggest]', N'Function', N'0', N'::1', CAST(N'2019-10-02T04:42:17.963' AS DateTime), N'CreateFunction')
INSERT [dbo].[aspnetactivitylog] ([ActivityLogId], [UserId], [ActivityText], [TargetType], [TargetId], [IPAddress], [ActivityDate], [ActivityType]) VALUES (5007, N'94E7515B-09B1-4B90-872E-6D544BA4A339', N'Create new function [Create of Suggest]', N'Function', N'0', N'::1', CAST(N'2019-10-02T04:42:27.600' AS DateTime), N'CreateFunction')
INSERT [dbo].[aspnetactivitylog] ([ActivityLogId], [UserId], [ActivityText], [TargetType], [TargetId], [IPAddress], [ActivityDate], [ActivityType]) VALUES (5008, N'94E7515B-09B1-4B90-872E-6D544BA4A339', N'Create new function [Delete of Suggest]', N'Function', N'0', N'::1', CAST(N'2019-10-02T04:42:38.250' AS DateTime), N'CreateFunction')
INSERT [dbo].[aspnetactivitylog] ([ActivityLogId], [UserId], [ActivityText], [TargetType], [TargetId], [IPAddress], [ActivityDate], [ActivityType]) VALUES (5009, N'94E7515B-09B1-4B90-872E-6D544BA4A339', N'Create new access [Name: Access]', N'Access', NULL, N'::1', CAST(N'2019-10-02T09:09:44.673' AS DateTime), N'CreateAccess')
INSERT [dbo].[aspnetactivitylog] ([ActivityLogId], [UserId], [ActivityText], [TargetType], [TargetId], [IPAddress], [ActivityDate], [ActivityType]) VALUES (5010, N'94E7515B-09B1-4B90-872E-6D544BA4A339', N'Updated the access [Name: Job]', N'Access', N'7947E54F-9CA8-4725-B5BD-8C9C2611171F', N'::1', CAST(N'2019-10-02T09:10:02.630' AS DateTime), N'UpdateAccess')
INSERT [dbo].[aspnetactivitylog] ([ActivityLogId], [UserId], [ActivityText], [TargetType], [TargetId], [IPAddress], [ActivityDate], [ActivityType]) VALUES (5011, N'94E7515B-09B1-4B90-872E-6D544BA4A339', N'Create new function [Index of Job]', N'Function', N'0', N'::1', CAST(N'2019-10-02T09:10:20.937' AS DateTime), N'CreateFunction')
INSERT [dbo].[aspnetactivitylog] ([ActivityLogId], [UserId], [ActivityText], [TargetType], [TargetId], [IPAddress], [ActivityDate], [ActivityType]) VALUES (5012, N'94E7515B-09B1-4B90-872E-6D544BA4A339', N'Create new access [Name: TypeSuggest]', N'Access', NULL, N'::1', CAST(N'2019-10-03T08:32:27.653' AS DateTime), N'CreateAccess')
INSERT [dbo].[aspnetactivitylog] ([ActivityLogId], [UserId], [ActivityText], [TargetType], [TargetId], [IPAddress], [ActivityDate], [ActivityType]) VALUES (5013, N'94E7515B-09B1-4B90-872E-6D544BA4A339', N'Create new function [Index of TypeSuggest]', N'Function', N'0', N'::1', CAST(N'2019-10-03T08:32:51.550' AS DateTime), N'CreateFunction')
INSERT [dbo].[aspnetactivitylog] ([ActivityLogId], [UserId], [ActivityText], [TargetType], [TargetId], [IPAddress], [ActivityDate], [ActivityType]) VALUES (5014, N'94E7515B-09B1-4B90-872E-6D544BA4A339', N'Create new function [Edit of TypeSuggest]', N'Function', N'0', N'::1', CAST(N'2019-10-03T10:02:19.447' AS DateTime), N'CreateFunction')
INSERT [dbo].[aspnetactivitylog] ([ActivityLogId], [UserId], [ActivityText], [TargetType], [TargetId], [IPAddress], [ActivityDate], [ActivityType]) VALUES (5015, N'94E7515B-09B1-4B90-872E-6D544BA4A339', N'Create new function [Create of TypeSuggest]', N'Function', N'0', N'::1', CAST(N'2019-10-03T10:02:35.327' AS DateTime), N'CreateFunction')
INSERT [dbo].[aspnetactivitylog] ([ActivityLogId], [UserId], [ActivityText], [TargetType], [TargetId], [IPAddress], [ActivityDate], [ActivityType]) VALUES (5016, N'94E7515B-09B1-4B90-872E-6D544BA4A339', N'Create new function [Delete of TypeSuggest]', N'Function', N'0', N'::1', CAST(N'2019-10-03T10:02:53.727' AS DateTime), N'CreateFunction')
INSERT [dbo].[aspnetactivitylog] ([ActivityLogId], [UserId], [ActivityText], [TargetType], [TargetId], [IPAddress], [ActivityDate], [ActivityType]) VALUES (5017, N'94E7515B-09B1-4B90-872E-6D544BA4A339', N'Create new role device [Name: Translate]', N'RolesAdmin', N'0', N'118.71.96.49', CAST(N'2019-11-11T03:35:43.277' AS DateTime), N'CreateRole')
SET IDENTITY_INSERT [dbo].[aspnetactivitylog] OFF
INSERT [dbo].[aspnetdomains] ([DomainKey], [DomainName], [LoginDurations], [Status], [Des]) VALUES (N'123.24.199.154', N'local', 60, 1, N'Current local domain')
SET IDENTITY_INSERT [dbo].[aspnetmenus] ON 

INSERT [dbo].[aspnetmenus] ([Id], [ParentId], [Area], [Name], [Title], [Desc], [Action], [Controller], [Visible], [Authenticate], [CssClass], [SortOrder], [AbsoluteUri], [Active], [IconCss]) VALUES (1, 0, NULL, NULL, N'Home', NULL, N'Index', N'Home', 1, 0, NULL, 1, NULL, 1, N'menu-icon fa fa-home')
INSERT [dbo].[aspnetmenus] ([Id], [ParentId], [Area], [Name], [Title], [Desc], [Action], [Controller], [Visible], [Authenticate], [CssClass], [SortOrder], [AbsoluteUri], [Active], [IconCss]) VALUES (4, 0, NULL, NULL, N'User', NULL, NULL, NULL, 1, 0, NULL, 7, NULL, 1, N'menu-icon fa fa-users')
INSERT [dbo].[aspnetmenus] ([Id], [ParentId], [Area], [Name], [Title], [Desc], [Action], [Controller], [Visible], [Authenticate], [CssClass], [SortOrder], [AbsoluteUri], [Active], [IconCss]) VALUES (6, 0, NULL, N'Cấu hình', N'Configuration', NULL, NULL, NULL, 1, 0, NULL, 10, NULL, 1, N'menu-icon fa fa-gears')
INSERT [dbo].[aspnetmenus] ([Id], [ParentId], [Area], [Name], [Title], [Desc], [Action], [Controller], [Visible], [Authenticate], [CssClass], [SortOrder], [AbsoluteUri], [Active], [IconCss]) VALUES (11, 4, NULL, NULL, N'User list', NULL, N'Index', N'UsersAdmin', 1, 0, NULL, 1, NULL, 1, N'menu-icon fa fa-caret-right')
INSERT [dbo].[aspnetmenus] ([Id], [ParentId], [Area], [Name], [Title], [Desc], [Action], [Controller], [Visible], [Authenticate], [CssClass], [SortOrder], [AbsoluteUri], [Active], [IconCss]) VALUES (12, 11, NULL, N'Chi tiết', N'Detail', NULL, N'Details', N'UsersAdmin', 0, 0, NULL, 1, NULL, 1, N'menu-icon fa fa-caret-right')
INSERT [dbo].[aspnetmenus] ([Id], [ParentId], [Area], [Name], [Title], [Desc], [Action], [Controller], [Visible], [Authenticate], [CssClass], [SortOrder], [AbsoluteUri], [Active], [IconCss]) VALUES (13, 11, NULL, NULL, N'Update user info', NULL, N'Edit', N'UsersAdmin', 0, 0, NULL, 2, NULL, 1, N'menu-icon fa fa-caret-right')
INSERT [dbo].[aspnetmenus] ([Id], [ParentId], [Area], [Name], [Title], [Desc], [Action], [Controller], [Visible], [Authenticate], [CssClass], [SortOrder], [AbsoluteUri], [Active], [IconCss]) VALUES (14, 11, NULL, NULL, N'Add user', NULL, N'Create', N'UsersAdmin', 0, 0, NULL, 3, NULL, 1, N'menu-icon fa fa-caret-right')
INSERT [dbo].[aspnetmenus] ([Id], [ParentId], [Area], [Name], [Title], [Desc], [Action], [Controller], [Visible], [Authenticate], [CssClass], [SortOrder], [AbsoluteUri], [Active], [IconCss]) VALUES (15, 4, NULL, N'Nhóm nhân viên', N'Roles', NULL, N'Index', N'RolesAdmin', 1, 0, NULL, 2, NULL, 1, N'menu-icon fa fa-caret-right')
INSERT [dbo].[aspnetmenus] ([Id], [ParentId], [Area], [Name], [Title], [Desc], [Action], [Controller], [Visible], [Authenticate], [CssClass], [SortOrder], [AbsoluteUri], [Active], [IconCss]) VALUES (16, 4, NULL, N'Phân quyền', N'Grant Permission', NULL, N'Index', N'AccessRoles', 1, 0, NULL, 3, NULL, 1, N'menu-icon fa fa-caret-right')
INSERT [dbo].[aspnetmenus] ([Id], [ParentId], [Area], [Name], [Title], [Desc], [Action], [Controller], [Visible], [Authenticate], [CssClass], [SortOrder], [AbsoluteUri], [Active], [IconCss]) VALUES (17, 6, NULL, N'Hệ thống', N'System', NULL, N'Settings', N'System', 1, 0, NULL, 4, NULL, 1, NULL)
INSERT [dbo].[aspnetmenus] ([Id], [ParentId], [Area], [Name], [Title], [Desc], [Action], [Controller], [Visible], [Authenticate], [CssClass], [SortOrder], [AbsoluteUri], [Active], [IconCss]) VALUES (23, 0, NULL, NULL, N'Your Profile', NULL, N'Profile', N'MyAccount', 0, 0, N'hidden', 11, NULL, 1, N'menu-icon fa fa-caret-right')
INSERT [dbo].[aspnetmenus] ([Id], [ParentId], [Area], [Name], [Title], [Desc], [Action], [Controller], [Visible], [Authenticate], [CssClass], [SortOrder], [AbsoluteUri], [Active], [IconCss]) VALUES (24, 23, NULL, NULL, N'Change your password', NULL, N'ChangePassword', N'MyAccount', 0, 0, NULL, 1, NULL, 1, N'menu-icon fa fa-caret-right')
INSERT [dbo].[aspnetmenus] ([Id], [ParentId], [Area], [Name], [Title], [Desc], [Action], [Controller], [Visible], [Authenticate], [CssClass], [SortOrder], [AbsoluteUri], [Active], [IconCss]) VALUES (25, 0, NULL, N'Tools', N'Tools', NULL, NULL, NULL, 1, 0, NULL, 8, NULL, 1, N'menu-icon fa fa fa-cog')
INSERT [dbo].[aspnetmenus] ([Id], [ParentId], [Area], [Name], [Title], [Desc], [Action], [Controller], [Visible], [Authenticate], [CssClass], [SortOrder], [AbsoluteUri], [Active], [IconCss]) VALUES (26, 25, NULL, N'Send SMS Test', N'Send SMS Test', NULL, N'TestSms', N'Tools', 1, 1, NULL, 1, NULL, 1, N'menu-icon fa fa-caret-right')
INSERT [dbo].[aspnetmenus] ([Id], [ParentId], [Area], [Name], [Title], [Desc], [Action], [Controller], [Visible], [Authenticate], [CssClass], [SortOrder], [AbsoluteUri], [Active], [IconCss]) VALUES (35, 0, NULL, N'Thống kê', N'Statistics', NULL, NULL, NULL, 0, 0, NULL, 9, NULL, 1, N'menu-icon fa fa-signal')
INSERT [dbo].[aspnetmenus] ([Id], [ParentId], [Area], [Name], [Title], [Desc], [Action], [Controller], [Visible], [Authenticate], [CssClass], [SortOrder], [AbsoluteUri], [Active], [IconCss]) VALUES (58, 0, NULL, NULL, N'Master', NULL, NULL, NULL, 1, 0, NULL, 6, NULL, 1, N'menu-icon fa fa-puzzle-piece')
INSERT [dbo].[aspnetmenus] ([Id], [ParentId], [Area], [Name], [Title], [Desc], [Action], [Controller], [Visible], [Authenticate], [CssClass], [SortOrder], [AbsoluteUri], [Active], [IconCss]) VALUES (106, 35, NULL, N'User online', N'User online', NULL, N'UsersOnline', N'Statistics', 1, 0, NULL, 1, NULL, 1, N'menu-icon fa fa-caret-right')
INSERT [dbo].[aspnetmenus] ([Id], [ParentId], [Area], [Name], [Title], [Desc], [Action], [Controller], [Visible], [Authenticate], [CssClass], [SortOrder], [AbsoluteUri], [Active], [IconCss]) VALUES (1124, 0, NULL, NULL, N'Posts', NULL, NULL, NULL, 1, 0, NULL, 4, NULL, 1, N'fa fa-file-text-o')
INSERT [dbo].[aspnetmenus] ([Id], [ParentId], [Area], [Name], [Title], [Desc], [Action], [Controller], [Visible], [Authenticate], [CssClass], [SortOrder], [AbsoluteUri], [Active], [IconCss]) VALUES (1132, 6, NULL, NULL, N'Config frontend', NULL, N'FrontEndSettings', N'FrontEndSystem', 1, 0, NULL, 3, NULL, 1, NULL)
INSERT [dbo].[aspnetmenus] ([Id], [ParentId], [Area], [Name], [Title], [Desc], [Action], [Controller], [Visible], [Authenticate], [CssClass], [SortOrder], [AbsoluteUri], [Active], [IconCss]) VALUES (1141, 6, NULL, NULL, N'Menu', NULL, N'Index', N'Menu', 1, 0, NULL, 2, NULL, 1, NULL)
INSERT [dbo].[aspnetmenus] ([Id], [ParentId], [Area], [Name], [Title], [Desc], [Action], [Controller], [Visible], [Authenticate], [CssClass], [SortOrder], [AbsoluteUri], [Active], [IconCss]) VALUES (1153, 0, NULL, NULL, N'Projects', NULL, N'Index', N'Project', 0, 0, NULL, 5, NULL, 1, N'fa fa-cubes')
INSERT [dbo].[aspnetmenus] ([Id], [ParentId], [Area], [Name], [Title], [Desc], [Action], [Controller], [Visible], [Authenticate], [CssClass], [SortOrder], [AbsoluteUri], [Active], [IconCss]) VALUES (1154, 1153, NULL, NULL, N'Add new', NULL, N'Create', N'Project', 0, 0, NULL, 1, NULL, 1, NULL)
INSERT [dbo].[aspnetmenus] ([Id], [ParentId], [Area], [Name], [Title], [Desc], [Action], [Controller], [Visible], [Authenticate], [CssClass], [SortOrder], [AbsoluteUri], [Active], [IconCss]) VALUES (1155, 1153, NULL, NULL, N'Edit', NULL, N'Edit', N'Project', 0, 0, NULL, 2, NULL, 1, NULL)
INSERT [dbo].[aspnetmenus] ([Id], [ParentId], [Area], [Name], [Title], [Desc], [Action], [Controller], [Visible], [Authenticate], [CssClass], [SortOrder], [AbsoluteUri], [Active], [IconCss]) VALUES (1159, 6, NULL, NULL, N'Footer manage', NULL, N'Index', N'Footer', 1, 0, NULL, 1, NULL, 1, NULL)
INSERT [dbo].[aspnetmenus] ([Id], [ParentId], [Area], [Name], [Title], [Desc], [Action], [Controller], [Visible], [Authenticate], [CssClass], [SortOrder], [AbsoluteUri], [Active], [IconCss]) VALUES (1161, 0, NULL, NULL, N'Công ty', NULL, N'Index', N'Company', 1, 0, NULL, 2, NULL, 1, N'fa fa-building')
INSERT [dbo].[aspnetmenus] ([Id], [ParentId], [Area], [Name], [Title], [Desc], [Action], [Controller], [Visible], [Authenticate], [CssClass], [SortOrder], [AbsoluteUri], [Active], [IconCss]) VALUES (1162, 58, NULL, NULL, N'Salary filter', NULL, N'Index', N'SalaryFilter', 1, 0, NULL, 1, NULL, 1, NULL)
INSERT [dbo].[aspnetmenus] ([Id], [ParentId], [Area], [Name], [Title], [Desc], [Action], [Controller], [Visible], [Authenticate], [CssClass], [SortOrder], [AbsoluteUri], [Active], [IconCss]) VALUES (1163, 58, NULL, NULL, N'Biểu mẫu', NULL, N'Index', N'Suggest', 1, 0, NULL, 2, NULL, 1, NULL)
INSERT [dbo].[aspnetmenus] ([Id], [ParentId], [Area], [Name], [Title], [Desc], [Action], [Controller], [Visible], [Authenticate], [CssClass], [SortOrder], [AbsoluteUri], [Active], [IconCss]) VALUES (1164, 0, NULL, NULL, N'Công việc', NULL, N'Index', N'Job', 1, 0, NULL, 3, NULL, 1, N'fa fa-file-word-o')
INSERT [dbo].[aspnetmenus] ([Id], [ParentId], [Area], [Name], [Title], [Desc], [Action], [Controller], [Visible], [Authenticate], [CssClass], [SortOrder], [AbsoluteUri], [Active], [IconCss]) VALUES (1165, 58, NULL, NULL, N'Loại biểu mẫu', NULL, N'Index', N'TypeSuggest', 1, 0, NULL, 3, NULL, 1, NULL)
SET IDENTITY_INSERT [dbo].[aspnetmenus] OFF
SET IDENTITY_INSERT [dbo].[aspnetmenus_lang] ON 

INSERT [dbo].[aspnetmenus_lang] ([Id], [MenuId], [Title], [LangCode]) VALUES (1, 1, N'Home', N'en-US')
INSERT [dbo].[aspnetmenus_lang] ([Id], [MenuId], [Title], [LangCode]) VALUES (2, 1, N'Trang chủ', N'vi-VN')
INSERT [dbo].[aspnetmenus_lang] ([Id], [MenuId], [Title], [LangCode]) VALUES (4, 104, N'Kho', N'vi-VN')
INSERT [dbo].[aspnetmenus_lang] ([Id], [MenuId], [Title], [LangCode]) VALUES (5, 1147, N'Quản lý kho', N'vi-VN')
INSERT [dbo].[aspnetmenus_lang] ([Id], [MenuId], [Title], [LangCode]) VALUES (6, 1126, N'Sản phẩm', N'vi-VN')
INSERT [dbo].[aspnetmenus_lang] ([Id], [MenuId], [Title], [LangCode]) VALUES (7, 1144, N'Danh sách sản phẩm', N'vi-VN')
INSERT [dbo].[aspnetmenus_lang] ([Id], [MenuId], [Title], [LangCode]) VALUES (8, 58, N'Danh mục', N'vi-VN')
INSERT [dbo].[aspnetmenus_lang] ([Id], [MenuId], [Title], [LangCode]) VALUES (9, 1127, N'Nhà cung cấp', N'vi-VN')
INSERT [dbo].[aspnetmenus_lang] ([Id], [MenuId], [Title], [LangCode]) VALUES (10, 1133, N'Đơn vị', N'vi-VN')
INSERT [dbo].[aspnetmenus_lang] ([Id], [MenuId], [Title], [LangCode]) VALUES (11, 121, N'Loại sản phẩm', N'vi-VN')
INSERT [dbo].[aspnetmenus_lang] ([Id], [MenuId], [Title], [LangCode]) VALUES (12, 123, N'Thuộc tính', N'vi-VN')
INSERT [dbo].[aspnetmenus_lang] ([Id], [MenuId], [Title], [LangCode]) VALUES (13, 1145, N'Thêm sản phẩm', N'vi-VN')
INSERT [dbo].[aspnetmenus_lang] ([Id], [MenuId], [Title], [LangCode]) VALUES (14, 1146, N'Sửa sản phẩm', N'vi-VN')
INSERT [dbo].[aspnetmenus_lang] ([Id], [MenuId], [Title], [LangCode]) VALUES (15, 1135, N'Thêm đơn vị', N'vi-VN')
INSERT [dbo].[aspnetmenus_lang] ([Id], [MenuId], [Title], [LangCode]) VALUES (16, 1134, N'Sửa đơn vị', N'vi-VN')
INSERT [dbo].[aspnetmenus_lang] ([Id], [MenuId], [Title], [LangCode]) VALUES (17, 1139, N'Thêm loại mới', N'vi-VN')
INSERT [dbo].[aspnetmenus_lang] ([Id], [MenuId], [Title], [LangCode]) VALUES (18, 1140, N'Sửa loại sản phẩm', N'vi-VN')
INSERT [dbo].[aspnetmenus_lang] ([Id], [MenuId], [Title], [LangCode]) VALUES (19, 1142, N'Thêm loại thuộc tính', N'vi-VN')
INSERT [dbo].[aspnetmenus_lang] ([Id], [MenuId], [Title], [LangCode]) VALUES (20, 1143, N'Sửa loại thuộc tính', N'vi-VN')
INSERT [dbo].[aspnetmenus_lang] ([Id], [MenuId], [Title], [LangCode]) VALUES (21, 17, N'Cấu hình', N'vi-VN')
INSERT [dbo].[aspnetmenus_lang] ([Id], [MenuId], [Title], [LangCode]) VALUES (22, 6, N'Hệ thống', N'vi-VN')
INSERT [dbo].[aspnetmenus_lang] ([Id], [MenuId], [Title], [LangCode]) VALUES (23, 23, N'Tài khoản của tôi', N'vi-VN')
INSERT [dbo].[aspnetmenus_lang] ([Id], [MenuId], [Title], [LangCode]) VALUES (24, 4, N'Nhân viên', N'vi-VN')
INSERT [dbo].[aspnetmenus_lang] ([Id], [MenuId], [Title], [LangCode]) VALUES (25, 11, N'Danh sách', N'vi-VN')
INSERT [dbo].[aspnetmenus_lang] ([Id], [MenuId], [Title], [LangCode]) VALUES (26, 12, N'Chi tiết', N'vi-VN')
INSERT [dbo].[aspnetmenus_lang] ([Id], [MenuId], [Title], [LangCode]) VALUES (27, 13, N'Sửa thông tin', N'vi-VN')
INSERT [dbo].[aspnetmenus_lang] ([Id], [MenuId], [Title], [LangCode]) VALUES (28, 14, N'Thêm mới', N'vi-VN')
INSERT [dbo].[aspnetmenus_lang] ([Id], [MenuId], [Title], [LangCode]) VALUES (29, 15, N'Phân nhóm', N'vi-VN')
INSERT [dbo].[aspnetmenus_lang] ([Id], [MenuId], [Title], [LangCode]) VALUES (30, 16, N'Phân quyền', N'vi-VN')
INSERT [dbo].[aspnetmenus_lang] ([Id], [MenuId], [Title], [LangCode]) VALUES (31, 1148, N'Lịch sử', N'vi-VN')
INSERT [dbo].[aspnetmenus_lang] ([Id], [MenuId], [Title], [LangCode]) VALUES (32, 1149, N'Cấu hình máy quét', N'vi-VN')
INSERT [dbo].[aspnetmenus_lang] ([Id], [MenuId], [Title], [LangCode]) VALUES (33, 24, N'Đổi mật khẩu', N'vi-VN')
INSERT [dbo].[aspnetmenus_lang] ([Id], [MenuId], [Title], [LangCode]) VALUES (34, 1151, N'Quản lý trang', N'vi-VN')
INSERT [dbo].[aspnetmenus_lang] ([Id], [MenuId], [Title], [LangCode]) VALUES (35, 1152, N'Danh sách', N'vi-VN')
INSERT [dbo].[aspnetmenus_lang] ([Id], [MenuId], [Title], [LangCode]) VALUES (36, 1124, N'Bài viết', N'vi-VN')
INSERT [dbo].[aspnetmenus_lang] ([Id], [MenuId], [Title], [LangCode]) VALUES (37, 1125, N'Danh sách', N'vi-VN')
INSERT [dbo].[aspnetmenus_lang] ([Id], [MenuId], [Title], [LangCode]) VALUES (38, 1156, N'Loại dự án', N'vi-VN')
INSERT [dbo].[aspnetmenus_lang] ([Id], [MenuId], [Title], [LangCode]) VALUES (39, 1157, N'Thêm mới', N'vi-VN')
INSERT [dbo].[aspnetmenus_lang] ([Id], [MenuId], [Title], [LangCode]) VALUES (40, 1158, N'Sửa', N'vi-VN')
INSERT [dbo].[aspnetmenus_lang] ([Id], [MenuId], [Title], [LangCode]) VALUES (41, 1159, N'Quản lý chân trang', N'vi-VN')
INSERT [dbo].[aspnetmenus_lang] ([Id], [MenuId], [Title], [LangCode]) VALUES (42, 1159, N'フッター管理', N'ja-JP')
INSERT [dbo].[aspnetmenus_lang] ([Id], [MenuId], [Title], [LangCode]) VALUES (43, 1162, N'Danh mục mức lương', N'vi-VN')
SET IDENTITY_INSERT [dbo].[aspnetmenus_lang] OFF
SET IDENTITY_INSERT [dbo].[aspnetoperations] ON 

INSERT [dbo].[aspnetoperations] ([Id], [OperationName], [Enabled], [AccessId], [ActionName]) VALUES (1, N'View Roles', 1, N'D3C3C55D-DFD2-4805-8839-6A7F965C808F', N'Index')
INSERT [dbo].[aspnetoperations] ([Id], [OperationName], [Enabled], [AccessId], [ActionName]) VALUES (2, N'Edit Role', 1, N'D3C3C55D-DFD2-4805-8839-6A7F965C808F', N'Update')
INSERT [dbo].[aspnetoperations] ([Id], [OperationName], [Enabled], [AccessId], [ActionName]) VALUES (3, N'Create Role', 1, N'D3C3C55D-DFD2-4805-8839-6A7F965C808F', N'Create')
INSERT [dbo].[aspnetoperations] ([Id], [OperationName], [Enabled], [AccessId], [ActionName]) VALUES (9, N'Show Dashboard', 1, N'7FDA2C88-0928-4625-A83A-E2D716C14359', N'Index')
INSERT [dbo].[aspnetoperations] ([Id], [OperationName], [Enabled], [AccessId], [ActionName]) VALUES (14, N'View Access List', 1, N'B229CAE9-333E-49B2-948B-ED160D244BB5', N'Index')
INSERT [dbo].[aspnetoperations] ([Id], [OperationName], [Enabled], [AccessId], [ActionName]) VALUES (15, N'Update permissions', 1, N'B229CAE9-333E-49B2-948B-ED160D244BB5', N'UpdateAccessRoles')
INSERT [dbo].[aspnetoperations] ([Id], [OperationName], [Enabled], [AccessId], [ActionName]) VALUES (17, N'View Users List', 1, N'824085C0-CBC0-4CE0-AAA5-9D730781E1D5', N'Index')
INSERT [dbo].[aspnetoperations] ([Id], [OperationName], [Enabled], [AccessId], [ActionName]) VALUES (18, N'View User Detail', 1, N'824085C0-CBC0-4CE0-AAA5-9D730781E1D5', N'Details')
INSERT [dbo].[aspnetoperations] ([Id], [OperationName], [Enabled], [AccessId], [ActionName]) VALUES (19, N'Edit user information', 1, N'824085C0-CBC0-4CE0-AAA5-9D730781E1D5', N'Edit')
INSERT [dbo].[aspnetoperations] ([Id], [OperationName], [Enabled], [AccessId], [ActionName]) VALUES (20, N'Reset User Password', 1, N'824085C0-CBC0-4CE0-AAA5-9D730781E1D5', N'ResetPassword')
INSERT [dbo].[aspnetoperations] ([Id], [OperationName], [Enabled], [AccessId], [ActionName]) VALUES (22, N'System functions', 1, N'CB21302B-7347-41F3-B716-1F9E3F173C05', N'Index')
INSERT [dbo].[aspnetoperations] ([Id], [OperationName], [Enabled], [AccessId], [ActionName]) VALUES (23, N'Create new function', 1, N'CB21302B-7347-41F3-B716-1F9E3F173C05', N'Create')
INSERT [dbo].[aspnetoperations] ([Id], [OperationName], [Enabled], [AccessId], [ActionName]) VALUES (24, N'Update function ', 1, N'CB21302B-7347-41F3-B716-1F9E3F173C05', N'Update')
INSERT [dbo].[aspnetoperations] ([Id], [OperationName], [Enabled], [AccessId], [ActionName]) VALUES (25, N'Delete function', 1, N'CB21302B-7347-41F3-B716-1F9E3F173C05', N'Delete')
INSERT [dbo].[aspnetoperations] ([Id], [OperationName], [Enabled], [AccessId], [ActionName]) VALUES (26, N'View access list', 1, N'29CD28D0-7701-4425-B0C2-BD1C609B1661', N'Index')
INSERT [dbo].[aspnetoperations] ([Id], [OperationName], [Enabled], [AccessId], [ActionName]) VALUES (27, N'Create access', 1, N'29CD28D0-7701-4425-B0C2-BD1C609B1661', N'Create')
INSERT [dbo].[aspnetoperations] ([Id], [OperationName], [Enabled], [AccessId], [ActionName]) VALUES (28, N'Delete access', 1, N'29CD28D0-7701-4425-B0C2-BD1C609B1661', N'DeleteAccess')
INSERT [dbo].[aspnetoperations] ([Id], [OperationName], [Enabled], [AccessId], [ActionName]) VALUES (29, N'Update access', 1, N'29CD28D0-7701-4425-B0C2-BD1C609B1661', N'Update')
INSERT [dbo].[aspnetoperations] ([Id], [OperationName], [Enabled], [AccessId], [ActionName]) VALUES (30, N'Delete role', 1, N'D3C3C55D-DFD2-4805-8839-6A7F965C808F', N'DeleteRole')
INSERT [dbo].[aspnetoperations] ([Id], [OperationName], [Enabled], [AccessId], [ActionName]) VALUES (32, N'Create user', 1, N'824085C0-CBC0-4CE0-AAA5-9D730781E1D5', N'Create')
INSERT [dbo].[aspnetoperations] ([Id], [OperationName], [Enabled], [AccessId], [ActionName]) VALUES (33, N'Delete User', 1, N'824085C0-CBC0-4CE0-AAA5-9D730781E1D5', N'DeleteUser')
INSERT [dbo].[aspnetoperations] ([Id], [OperationName], [Enabled], [AccessId], [ActionName]) VALUES (1094, N'Cấu hình hệ thống', 1, N'A8C7D090-B6CF-4A0E-86B3-448FB7F13E37', N'Settings')
INSERT [dbo].[aspnetoperations] ([Id], [OperationName], [Enabled], [AccessId], [ActionName]) VALUES (2101, N'Danh sách', 1, N'E94B2CC5-00EE-4922-AA9B-4CAB9667E915', N'Index')
INSERT [dbo].[aspnetoperations] ([Id], [OperationName], [Enabled], [AccessId], [ActionName]) VALUES (2102, N'Sửa đơn vị', 1, N'E94B2CC5-00EE-4922-AA9B-4CAB9667E915', N'Edit')
INSERT [dbo].[aspnetoperations] ([Id], [OperationName], [Enabled], [AccessId], [ActionName]) VALUES (2104, N'Thêm mới', 1, N'E94B2CC5-00EE-4922-AA9B-4CAB9667E915', N'Create')
INSERT [dbo].[aspnetoperations] ([Id], [OperationName], [Enabled], [AccessId], [ActionName]) VALUES (2110, N'Cập nhật menu', 1, N'99648A3A-A3F8-497E-9299-75E41DBB2E8A', N'Index')
INSERT [dbo].[aspnetoperations] ([Id], [OperationName], [Enabled], [AccessId], [ActionName]) VALUES (2111, N'Danh sách', 1, N'E81AF9AD-5269-4C72-BC0E-47DC2EECC2EA', N'Index')
INSERT [dbo].[aspnetoperations] ([Id], [OperationName], [Enabled], [AccessId], [ActionName]) VALUES (2112, N'Thêm mới', 1, N'E81AF9AD-5269-4C72-BC0E-47DC2EECC2EA', N'Create')
INSERT [dbo].[aspnetoperations] ([Id], [OperationName], [Enabled], [AccessId], [ActionName]) VALUES (2113, N'Sửa thuộc tính', 1, N'E81AF9AD-5269-4C72-BC0E-47DC2EECC2EA', N'Edit')
INSERT [dbo].[aspnetoperations] ([Id], [OperationName], [Enabled], [AccessId], [ActionName]) VALUES (2114, N'Danh sách sản phẩm', 1, N'D120F343-8F75-41BA-A8CB-97410C122570', N'Index')
INSERT [dbo].[aspnetoperations] ([Id], [OperationName], [Enabled], [AccessId], [ActionName]) VALUES (2115, N'Thêm mới', 1, N'D120F343-8F75-41BA-A8CB-97410C122570', N'Create')
INSERT [dbo].[aspnetoperations] ([Id], [OperationName], [Enabled], [AccessId], [ActionName]) VALUES (2116, N'Sửa sản phẩm', 1, N'D120F343-8F75-41BA-A8CB-97410C122570', N'Edit')
INSERT [dbo].[aspnetoperations] ([Id], [OperationName], [Enabled], [AccessId], [ActionName]) VALUES (2119, N'View profile', 1, N'141C6B6F-8E1C-4806-A62F-59FC654BC731', N'Profile')
INSERT [dbo].[aspnetoperations] ([Id], [OperationName], [Enabled], [AccessId], [ActionName]) VALUES (2120, N'Thay đổi mật khẩu', 1, N'141C6B6F-8E1C-4806-A62F-59FC654BC731', N'ChangePassword')
INSERT [dbo].[aspnetoperations] ([Id], [OperationName], [Enabled], [AccessId], [ActionName]) VALUES (2125, N'Xóa', 1, N'E94B2CC5-00EE-4922-AA9B-4CAB9667E915', N'Delete')
INSERT [dbo].[aspnetoperations] ([Id], [OperationName], [Enabled], [AccessId], [ActionName]) VALUES (2126, N'Xóa', 1, N'E81AF9AD-5269-4C72-BC0E-47DC2EECC2EA', N'Delete')
INSERT [dbo].[aspnetoperations] ([Id], [OperationName], [Enabled], [AccessId], [ActionName]) VALUES (2127, N'Xóa', 1, N'D120F343-8F75-41BA-A8CB-97410C122570', N'Delete')
INSERT [dbo].[aspnetoperations] ([Id], [OperationName], [Enabled], [AccessId], [ActionName]) VALUES (2130, N'Quản lý điều hướng', 1, N'BE146B14-1A86-49F3-8344-C5C9D75D839A', N'Index')
INSERT [dbo].[aspnetoperations] ([Id], [OperationName], [Enabled], [AccessId], [ActionName]) VALUES (2131, N'Danh sách', 1, N'85D4CBB4-D2BB-4535-91C4-5B95B3442B99', N'Index')
INSERT [dbo].[aspnetoperations] ([Id], [OperationName], [Enabled], [AccessId], [ActionName]) VALUES (2132, N'Danh sách', 1, N'6D9B1DF1-BCD0-4A73-8DBF-6DF2A313E3BC', N'Index')
INSERT [dbo].[aspnetoperations] ([Id], [OperationName], [Enabled], [AccessId], [ActionName]) VALUES (2133, N'Danh sách', 1, N'C2FB41CC-4ED8-46FF-84C6-1164D8E598FF', N'Index')
INSERT [dbo].[aspnetoperations] ([Id], [OperationName], [Enabled], [AccessId], [ActionName]) VALUES (2134, N'Thêm mới', 1, N'C2FB41CC-4ED8-46FF-84C6-1164D8E598FF', N'Create')
INSERT [dbo].[aspnetoperations] ([Id], [OperationName], [Enabled], [AccessId], [ActionName]) VALUES (2135, N'Sửa', 1, N'C2FB41CC-4ED8-46FF-84C6-1164D8E598FF', N'Edit')
INSERT [dbo].[aspnetoperations] ([Id], [OperationName], [Enabled], [AccessId], [ActionName]) VALUES (2136, N'Danh sách', 1, N'357BEB69-0425-4DE7-B425-B05C44881F6E', N'Index')
INSERT [dbo].[aspnetoperations] ([Id], [OperationName], [Enabled], [AccessId], [ActionName]) VALUES (2137, N'Thêm mới', 1, N'357BEB69-0425-4DE7-B425-B05C44881F6E', N'Create')
INSERT [dbo].[aspnetoperations] ([Id], [OperationName], [Enabled], [AccessId], [ActionName]) VALUES (2138, N'Sửa', 1, N'357BEB69-0425-4DE7-B425-B05C44881F6E', N'Edit')
INSERT [dbo].[aspnetoperations] ([Id], [OperationName], [Enabled], [AccessId], [ActionName]) VALUES (2139, N'Quản lý chân trang', 1, N'32B7D7E6-6EA3-4AD6-8B20-8F047F4F45FA', N'Index')
INSERT [dbo].[aspnetoperations] ([Id], [OperationName], [Enabled], [AccessId], [ActionName]) VALUES (2140, N'Cấu hình', 1, N'59003A6E-9C0B-4E45-B8D8-A932EDDB2F26', N'FrontEndSettings')
INSERT [dbo].[aspnetoperations] ([Id], [OperationName], [Enabled], [AccessId], [ActionName]) VALUES (2141, N'Danh sách', 1, N'FCE958A1-2DA7-418D-A821-1A862A8D2483', N'Index')
INSERT [dbo].[aspnetoperations] ([Id], [OperationName], [Enabled], [AccessId], [ActionName]) VALUES (2142, N'Danh sách công ty', 1, N'2FDE3424-C325-4391-9A36-CE18B1F829E1', N'Index')
INSERT [dbo].[aspnetoperations] ([Id], [OperationName], [Enabled], [AccessId], [ActionName]) VALUES (2143, N'Danh sách mức lương', 1, N'4F76273F-33D6-4282-B786-0E29103C03F5', N'Index')
INSERT [dbo].[aspnetoperations] ([Id], [OperationName], [Enabled], [AccessId], [ActionName]) VALUES (2144, N'Cập nhật mức lương', 1, N'4F76273F-33D6-4282-B786-0E29103C03F5', N'Edit')
INSERT [dbo].[aspnetoperations] ([Id], [OperationName], [Enabled], [AccessId], [ActionName]) VALUES (2145, N'Thêm mới mức lương', 1, N'4F76273F-33D6-4282-B786-0E29103C03F5', N'Create')
INSERT [dbo].[aspnetoperations] ([Id], [OperationName], [Enabled], [AccessId], [ActionName]) VALUES (2146, N'Xóa mức lương', 1, N'4F76273F-33D6-4282-B786-0E29103C03F5', N'Delete')
INSERT [dbo].[aspnetoperations] ([Id], [OperationName], [Enabled], [AccessId], [ActionName]) VALUES (2147, N'Danh sách biểu mẫu', 1, N'6823D1F2-3B68-4ADF-B324-624D47A16EB5', N'Index')
INSERT [dbo].[aspnetoperations] ([Id], [OperationName], [Enabled], [AccessId], [ActionName]) VALUES (2148, N'Cập nhật biểu mẫu', 1, N'6823D1F2-3B68-4ADF-B324-624D47A16EB5', N'Edit')
INSERT [dbo].[aspnetoperations] ([Id], [OperationName], [Enabled], [AccessId], [ActionName]) VALUES (2149, N'Thêm mới biểu mẫu', 1, N'6823D1F2-3B68-4ADF-B324-624D47A16EB5', N'Create')
INSERT [dbo].[aspnetoperations] ([Id], [OperationName], [Enabled], [AccessId], [ActionName]) VALUES (2150, N'Xóa biểu mẫu', 1, N'6823D1F2-3B68-4ADF-B324-624D47A16EB5', N'Delete')
INSERT [dbo].[aspnetoperations] ([Id], [OperationName], [Enabled], [AccessId], [ActionName]) VALUES (2151, N'Danh sách công việc', 1, N'7947E54F-9CA8-4725-B5BD-8C9C2611171F', N'Index')
INSERT [dbo].[aspnetoperations] ([Id], [OperationName], [Enabled], [AccessId], [ActionName]) VALUES (2152, N'Danh sách loại biểu mẫu', 1, N'F7B02B30-723D-4524-B044-A27ACD58CA8F', N'Index')
INSERT [dbo].[aspnetoperations] ([Id], [OperationName], [Enabled], [AccessId], [ActionName]) VALUES (2153, N'Cập nhật loại biểu mẫu', 1, N'F7B02B30-723D-4524-B044-A27ACD58CA8F', N'Edit')
INSERT [dbo].[aspnetoperations] ([Id], [OperationName], [Enabled], [AccessId], [ActionName]) VALUES (2154, N'Thêm mới loại biểu mẫu', 1, N'F7B02B30-723D-4524-B044-A27ACD58CA8F', N'Create')
INSERT [dbo].[aspnetoperations] ([Id], [OperationName], [Enabled], [AccessId], [ActionName]) VALUES (2155, N'Xóa loại biểu mẫu', 1, N'F7B02B30-723D-4524-B044-A27ACD58CA8F', N'Delete')
SET IDENTITY_INSERT [dbo].[aspnetoperations] OFF
INSERT [dbo].[aspnetroles] ([Id], [Name]) VALUES (N'a5866dfa-08f0-4605-857f-827782df025a', N'Tester')
INSERT [dbo].[aspnetroles] ([Id], [Name]) VALUES (N'ae7bdc1f-592b-4742-8ddb-a0b6e174045a', N'Users')
INSERT [dbo].[aspnetroles] ([Id], [Name]) VALUES (N'b771e7d7-5b2b-4b77-a8b8-272392c6e411', N'Translate')
INSERT [dbo].[aspnetroles] ([Id], [Name]) VALUES (N'd0732bf3-f54a-44b0-8f3a-12606e977e34', N'Admin')
INSERT [dbo].[aspnetroles] ([Id], [Name]) VALUES (N'd8a6f35e-1870-452b-bd88-9794d3fce6b2', N'Content')
INSERT [dbo].[aspnetroles] ([Id], [Name]) VALUES (N'fe537142-5123-4dc1-8051-53802ce6ad7d', N'Manager')
INSERT [dbo].[aspnetuserroles] ([UserId], [RoleId]) VALUES (N'0c99f546-1d85-4c87-a7cf-995461c67d2b', N'b771e7d7-5b2b-4b77-a8b8-272392c6e411')
INSERT [dbo].[aspnetuserroles] ([UserId], [RoleId]) VALUES (N'63454fc4-58c6-4224-ad98-a66f974b1774', N'fe537142-5123-4dc1-8051-53802ce6ad7d')
INSERT [dbo].[aspnetuserroles] ([UserId], [RoleId]) VALUES (N'94E7515B-09B1-4B90-872E-6D544BA4A339', N'd0732bf3-f54a-44b0-8f3a-12606e977e34')
INSERT [dbo].[aspnetuserroles] ([UserId], [RoleId]) VALUES (N'db799393-4290-421a-945f-d0f5caf9202c', N'b771e7d7-5b2b-4b77-a8b8-272392c6e411')
SET IDENTITY_INSERT [dbo].[aspnetusers] ON 

INSERT [dbo].[aspnetusers] ([Id], [StaffId], [ProviderId], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName], [CreatedDateUtc], [PasswordHash2], [FullName], [DisplayName], [Avatar], [OTPType], [Birthday], [Sex], [Address], [Note], [Code], [StaffCategoryId], [Married], [IdCard], [Passport], [TaxInfo], [InsurranceInfo], [BankInfo], [Status]) VALUES (N'94E7515B-09B1-4B90-872E-6D544BA4A339', 7, 0, N'bangkhmt3@gmail.com', 1, N'e10adc3949ba59abbe56e057f20f883e', N'bb943eab-3c65-4af8-90c7-16f6c1be015f', N'', 1, 0, CAST(N'4757-03-13T09:18:24.0000000' AS DateTime2), 0, 0, N'admin', CAST(N'2016-03-24T02:39:27.0000000' AS DateTime2), NULL, N'Admin', N'Admin', NULL, NULL, NULL, 0, NULL, NULL, NULL, 0, 0, NULL, NULL, NULL, NULL, NULL, 1)
INSERT [dbo].[aspnetusers] ([Id], [StaffId], [ProviderId], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName], [CreatedDateUtc], [PasswordHash2], [FullName], [DisplayName], [Avatar], [OTPType], [Birthday], [Sex], [Address], [Note], [Code], [StaffCategoryId], [Married], [IdCard], [Passport], [TaxInfo], [InsurranceInfo], [BankInfo], [Status]) VALUES (N'63454fc4-58c6-4224-ad98-a66f974b1774', 8, 0, N'manager@email.com', 1, N'e10adc3949ba59abbe56e057f20f883e', N'22359df7-3934-4a9a-a8e6-8cdddcca24ff', N'', 0, 0, NULL, 1, 0, N'manager', CAST(N'2019-06-13T13:49:12.0000000' AS DateTime2), NULL, N'Manager', NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, 0, 0, NULL, NULL, NULL, NULL, NULL, 1)
INSERT [dbo].[aspnetusers] ([Id], [StaffId], [ProviderId], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName], [CreatedDateUtc], [PasswordHash2], [FullName], [DisplayName], [Avatar], [OTPType], [Birthday], [Sex], [Address], [Note], [Code], [StaffCategoryId], [Married], [IdCard], [Passport], [TaxInfo], [InsurranceInfo], [BankInfo], [Status]) VALUES (N'0c99f546-1d85-4c87-a7cf-995461c67d2b', 9, 0, N'translate001@email.com', 1, N'e10adc3949ba59abbe56e057f20f883e', N'68370f44-988d-4b6e-8e2c-6c5021d0fccb', N'', 0, 0, NULL, 1, 0, N'translate001', CAST(N'2019-11-11T10:38:01.0000000' AS DateTime2), NULL, N'Người biên dịch 1', NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, 0, 0, NULL, NULL, NULL, NULL, NULL, 1)
INSERT [dbo].[aspnetusers] ([Id], [StaffId], [ProviderId], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName], [CreatedDateUtc], [PasswordHash2], [FullName], [DisplayName], [Avatar], [OTPType], [Birthday], [Sex], [Address], [Note], [Code], [StaffCategoryId], [Married], [IdCard], [Passport], [TaxInfo], [InsurranceInfo], [BankInfo], [Status]) VALUES (N'db799393-4290-421a-945f-d0f5caf9202c', 10, 0, N'translate002@email.com', 1, N'0b3bc9ce555f07d127c6da44337e364f', N'e3925028-ca93-4859-bd2c-96373ef0c1dc', N'', 0, 0, NULL, 1, 0, N'translate002', CAST(N'2019-11-11T10:48:25.0000000' AS DateTime2), NULL, N'Người biên dịch 2', NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, 0, 0, NULL, NULL, NULL, NULL, NULL, 1)
SET IDENTITY_INSERT [dbo].[aspnetusers] OFF
INSERT [dbo].[cmn_settings] ([SettingName], [SettingType], [SettingValue]) VALUES (N'AdminEmail', N'GeneralSettings', N'')
INSERT [dbo].[cmn_settings] ([SettingName], [SettingType], [SettingValue]) VALUES (N'CacheProvider', N'CacheSettings', N'')
INSERT [dbo].[cmn_settings] ([SettingName], [SettingType], [SettingValue]) VALUES (N'EmbeddedScripts', N'GeneralFrontEndSettings', N'<script>
      
</script>')
INSERT [dbo].[cmn_settings] ([SettingName], [SettingType], [SettingValue]) VALUES (N'SiteLogo', N'GeneralFrontEndSettings', N'Media/Uploads/images/logo.jpg')
INSERT [dbo].[cmn_settings] ([SettingName], [SettingType], [SettingValue]) VALUES (N'SiteName', N'GeneralFrontEndSettings', N'Job Market')
INSERT [dbo].[cmn_settings] ([SettingName], [SettingType], [SettingValue]) VALUES (N'SiteName', N'GeneralSettings', N'RIREKI')
INSERT [dbo].[cmn_settings] ([SettingName], [SettingType], [SettingValue]) VALUES (N'StoragePeriodTime', N'GeneralSettings', N'0')
INSERT [dbo].[cmn_settings] ([SettingName], [SettingType], [SettingValue]) VALUES (N'SystemDefaultCacheDuration', N'CacheSettings', N'0')
INSERT [dbo].[cmn_settings] ([SettingName], [SettingType], [SettingValue]) VALUES (N'TimeZoneId', N'GeneralSettings', N'')
INSERT [dbo].[cmn_settings] ([SettingName], [SettingType], [SettingValue]) VALUES (N'WebsiteCacheDuration', N'CacheSettings', N'0')
SET IDENTITY_INSERT [dbo].[tbl_currency] ON 

INSERT [dbo].[tbl_currency] ([Id], [Code], [Name], [CreatedDate], [Status]) VALUES (1, NULL, N'$', CAST(N'2019-04-05T14:20:21.050' AS DateTime), 1)
INSERT [dbo].[tbl_currency] ([Id], [Code], [Name], [CreatedDate], [Status]) VALUES (2, NULL, N'VND', CAST(N'2019-04-05T14:20:28.970' AS DateTime), 1)
SET IDENTITY_INSERT [dbo].[tbl_currency] OFF
SET IDENTITY_INSERT [dbo].[tbl_device] ON 

INSERT [dbo].[tbl_device] ([Id], [Code], [Name], [CreatedDate], [Status]) VALUES (1, NULL, N'PC', CAST(N'2019-06-24T15:46:09.070' AS DateTime), 1)
INSERT [dbo].[tbl_device] ([Id], [Code], [Name], [CreatedDate], [Status]) VALUES (2, N'HT-01', N'HandyTerminal 1', CAST(N'2019-06-24T15:46:20.387' AS DateTime), 1)
SET IDENTITY_INSERT [dbo].[tbl_device] OFF
INSERT [dbo].[tbl_footer] ([BodyContent], [LangCode]) VALUES (N'<div class="col_three_fourth col_last">
<div class="col_one_fourth">
<div class="widget widget_links clearfix">
<h4>Công ty ChieSe</h4>
<ul>
<li><a href="#">Documentation</a></li>
<li><a href="#">Feedback</a></li>
<li><a href="#">Plugins</a></li>
<li><a href="#">Support Forums</a></li>
<li><a href="#">Themes</a></li>
</ul>
</div>
</div>
<div class="col_one_fourth">
<div class="widget widget_links clearfix">
<h4>Community</h4>
<ul>
<li><a href="#">Documentation</a></li>
<li><a href="#">Feedback</a></li>
<li><a href="#">Plugins</a></li>
<li><a href="#">Support Forums</a></li>
<li><a href="#">Themes</a></li>
</ul>
</div>
</div>
<div class="col_one_fourth">
<div class="widget widget_links clearfix">
<h4>Learn</h4>
<ul>
<li><a href="#">Documentation</a></li>
<li><a href="#">Feedback</a></li>
<li><a href="#">Plugins</a></li>
<li><a href="#">Support Forums</a></li>
<li><a href="#">Themes</a></li>
</ul>
</div>
</div>
<div class="col_one_fourth col_last">
<div class="widget widget_links clearfix">
<h4>About</h4>
<ul>
<li><a href="#">Documentation</a></li>
<li><a href="#">Feedback</a></li>
<li><a href="#">Plugins</a></li>
<li><a href="#">Support Forums</a></li>
<li><a href="#">Themes</a></li>
</ul>
</div>
</div>
<div class="clear"> </div>
<div class="line line-sm"> </div>
<div class="col_two_third"><small class="t300" style="color: #aaa;">Copyrights © 2017 All Rights Reserved by ChieSe.</small></div>
<div class="col_one_third col_last">
<div class="fright clearfix"> </div>
</div>
</div>', N'vi-VN')
INSERT [dbo].[tbl_footer] ([BodyContent], [LangCode]) VALUES (N'<div class="col_three_fourth col_last">
<div class="col_one_fourth">
<div class="widget widget_links clearfix">
<h4>ChieSe 会社</h4>
<ul>
<li><a href="#">ドキュメンテーション</a></li>
<li><a href="#">Feedback</a></li>
<li><a href="#">Plugins</a></li>
<li><a href="#">Support Forums</a></li>
<li><a href="#">Themes</a></li>
</ul>
</div>
</div>
<div class="col_one_fourth">
<div class="widget widget_links clearfix">
<h4>コミュニティ</h4>
<ul>
<li><a href="#">Documentation</a></li>
<li><a href="#">Feedback</a></li>
<li><a href="#">Plugins</a></li>
<li><a href="#">Support Forums</a></li>
<li><a href="#">Themes</a></li>
</ul>
</div>
</div>
<div class="col_one_fourth">
<div class="widget widget_links clearfix">
<h4>Learn</h4>
<ul>
<li><a href="#">Documentation</a></li>
<li><a href="#">Feedback</a></li>
<li><a href="#">Plugins</a></li>
<li><a href="#">Support Forums</a></li>
<li><a href="#">Themes</a></li>
</ul>
</div>
</div>
<div class="col_one_fourth col_last">
<div class="widget widget_links clearfix">
<h4>About</h4>
<ul>
<li><a href="#">Documentation</a></li>
<li><a href="#">Feedback</a></li>
<li><a href="#">Plugins</a></li>
<li><a href="#">Support Forums</a></li>
<li><a href="#">Themes</a></li>
</ul>
</div>
</div>
<div class="clear"> </div>
<div class="line line-sm"> </div>
<div class="col_two_third"><small class="t300" style="color: #aaa;">Copyrights © 2017 All Rights Reserved by ChieSe.</small></div>
<div class="col_one_third col_last">
<div class="fright clearfix"> </div>
</div>
</div>', N'ja-JP')
SET IDENTITY_INSERT [dbo].[tbl_ht_resource] ON 

INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (1, N'LB_SYSTEM_STARTING', N'System starting', N'en-US     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (2, N'LB_SYSTEM_STARTING', N'Khởi động', N'vi-VN     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (3, N'LB_ACCOUNT', N'User Code', N'en-US     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (4, N'LB_ACCOUNT', N'Tài khoản', N'vi-VN     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (5, N'LB_PASSWORD', N'Password', N'en-US     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (6, N'LB_PASSWORD', N'Mật khẩu', N'vi-VN     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (7, N'LB_LOGIN', N'Login', N'en-US     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (8, N'LB_LOGIN', N'OK', N'vi-VN     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (9, N'LB_EXIT', N'Exit', N'en-US     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (10, N'LB_EXIT', N'Thoát', N'vi-VN     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (11, N'LB_ERROR', N'Error', N'en-US     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (12, N'LB_ERROR', N'Lỗi', N'vi-VN     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (13, N'ERROR_ACCOUNT_NULL', N'Please fill in your user code!', N'en-US     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (14, N'ERROR_ACCOUNT_NULL', N'Vui lòng nhập tài khoản !', N'vi-VN     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (15, N'ERROR_AN_ERROR_OCCURED', N'An error has occurred. Please see the log !', N'en-US     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (16, N'ERROR_AN_ERROR_OCCURED', N'Đã có lỗi xảy ra. Vui lòng kiểm tra log !', N'vi-VN     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (17, N'ERROR_DB_CONNECTION', N'Could not connect to database, please re-check your configuration !', N'en-US     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (18, N'ERROR_DB_CONNECTION', N'Không thể kết nối cơ sở dữ liệu, vui lòng kiểm tra cấu hình !', N'vi-VN     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (19, N'ERROR_GOOS_ISSUE_QTY_NOT_ENOUGH_FORMAT', N'Current QTY [{0}] is not enough [{1}] !', N'en-US     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (20, N'ERROR_GOOS_ISSUE_QTY_NOT_ENOUGH_FORMAT', N'Số lượng hiện tại [{0}] không đủ [{1}] !', N'vi-VN     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (21, N'ERROR_INPUT_QTY', N'QTY must be larger than 0 !', N'en-US     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (22, N'ERROR_INPUT_QTY', N'Số lượng nhập vào phải lớn hơn 0 !', N'vi-VN     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (23, N'ERROR_LOGIN_ACCOUNT_LOCKED', N'Your account has been locked !', N'en-US     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (24, N'ERROR_LOGIN_ACCOUNT_LOCKED', N'Tài khoản của bạn đã bị khóa !', N'vi-VN     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (25, N'ERROR_LOGIN_INVALID', N'User Code or password is incorrect !', N'en-US     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (26, N'ERROR_LOGIN_INVALID', N'Mã nhân viên hoặc mật khẩu không đúng !', N'vi-VN     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (27, N'ERROR_PASSWORD_NULL', N'Please fill in your password !', N'en-US     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (28, N'ERROR_PASSWORD_NULL', N'Vui lòng nhập mật khẩu !', N'vi-VN     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (29, N'LB_MENU', N'Menu', N'en-US     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (30, N'LB_MENU', N'Menu', N'vi-VN     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (31, N'LB_GOODS_RECEIPT', N'Goods Receipt', N'en-US     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (32, N'LB_GOODS_RECEIPT', N'Nhập kho', N'vi-VN     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (33, N'LB_GOODS_ISSUE', N'Goods Issue', N'en-US     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (34, N'LB_GOODS_ISSUE', N'Xuất kho', N'vi-VN     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (35, N'LB_STOCK_TAKE', N'Stock Take', N'en-US     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (36, N'LB_STOCK_TAKE', N'Kiểm kê', N'vi-VN     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (37, N'LB_SETTINGS', N'Settings', N'en-US     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (38, N'LB_SETTINGS', N'Cài đặt', N'vi-VN     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (39, N'LB_SCAN_CODE', N'Scan code', N'en-US     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (40, N'LB_SCAN_CODE', N'Quét mã', N'vi-VN     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (41, N'LB_ITEM_CODE', N'Item Code', N'en-US     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (42, N'LB_ITEM_CODE', N'Mã SP', N'vi-VN     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (43, N'LB_ITEM_NAME', N'Item Name', N'en-US     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (44, N'LB_ITEM_NAME', N'Tên SP', N'vi-VN     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (45, N'LB_QTY', N'QTY', N'en-US     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (46, N'LB_QTY', N'S.lượng', N'vi-VN     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (47, N'LB_BACK', N'Back', N'en-US     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (48, N'LB_BACK', N'Menu', N'vi-VN     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (49, N'LB_CLEAR', N'Clear', N'en-US     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (50, N'LB_CLEAR', N'Xóa', N'vi-VN     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (51, N'LB_REGISTER', N'Register', N'en-US     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (52, N'LB_REGISTER', N'Lưu', N'vi-VN     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (53, N'LB_STORAGE_PERIOD_LOG', N'Storage period log', N'en-US     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (54, N'LB_STORAGE_PERIOD', N'Lưu log', N'vi-VN     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (55, N'LB_SCAN_SETTING', N'Scan setting', N'en-US     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (56, N'LB_SCAN_SETTING', N'Cấu hình quét', N'vi-VN     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (57, N'LB_START_POSITION', N'Start position of characters', N'en-US     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (58, N'LB_START_POSITION', N'Vị trí bắt đầu', N'vi-VN     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (59, N'LB_NUMBER_OF_CHARACTERS', N'Number of characters', N'en-US     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (60, N'LB_NUMBER_OF_CHARACTERS', N'Số ký tự lấy', N'vi-VN     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (61, N'LB_DEFAULT', N'Default', N'en-US     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (62, N'LB_DEFAULT', N'Mặc định', N'vi-VN     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (63, N'LB_CUSTOM', N'Custom', N'en-US     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (64, N'LB_CUSTOM', N'Tùy chỉnh', N'vi-VN     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (65, N'LB_CODE_MAX_LENGTH', N'Length of code', N'en-US     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (66, N'LB_CODE_MAX_LENGTH', N'Độ dài mã', N'vi-VN     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (67, N'LB_LANGUAGE', N'Language', N'en-US     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (68, N'LB_LANGUAGE', N'Ngôn ngữ', N'vi-VN     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (69, N'LB_DAYS', N'Days', N'en-US     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (70, N'LB_DAYS', N'Ngày', N'vi-VN     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (71, N'ERROR_ITEM_CODE_NOT_FOUND', N'Item code not found !', N'en-US     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (72, N'ERROR_ITEM_CODE_NOT_FOUND', N'Không tìm thấy sản phẩm !', N'vi-VN     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (73, N'LB_ITEM_CODE_TYPE', N'Code type', N'en-US     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (74, N'LB_ITEM_CODE_TYPE', N'Loại mã', N'vi-VN     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (75, N'ERROR_LOGIN_PERMISSION_DENNIED', N'You don''t have permission for using. Please contact to Admin !', N'en-US     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (76, N'ERROR_LOGIN_PERMISSION_DENNIED', N'Bạn không có quyền sử dụng phần mềm này, vui lòng liên hệ Admin !', N'vi-VN     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (77, N'LB_LOGOUT', N'Logout', N'en-US     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (78, N'LB_LOGOUT', N'Đ.xuất', N'vi-VN     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (79, N'LB_CONFIRMATION', N'Confirmation', N'en-US     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (80, N'LB_CONFIRMATION', N'Xác nhận thông tin', N'vi-VN     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (81, N'LB_LOGOUT_CONFIRM', N'Are you sure you want to logout ?', N'en-US     ')
INSERT [dbo].[tbl_ht_resource] ([Id], [ResKey], [ResValue], [LangCode]) VALUES (82, N'LB_LOGOUT_CONFIRM', N'Bạn có chắc chắn muốn đăng xuất ?', N'vi-VN     ')
SET IDENTITY_INSERT [dbo].[tbl_ht_resource] OFF
SET IDENTITY_INSERT [dbo].[tbl_navigation] ON 

INSERT [dbo].[tbl_navigation] ([Id], [ParentId], [Area], [Name], [Title], [Desc], [Action], [Controller], [Visible], [Authenticate], [CssClass], [SortOrder], [AbsoluteUri], [Active], [IconCss]) VALUES (47, 0, NULL, NULL, N'HOME', NULL, NULL, NULL, 1, 0, NULL, 1, N'/', 1, NULL)
INSERT [dbo].[tbl_navigation] ([Id], [ParentId], [Area], [Name], [Title], [Desc], [Action], [Controller], [Visible], [Authenticate], [CssClass], [SortOrder], [AbsoluteUri], [Active], [IconCss]) VALUES (48, 0, NULL, NULL, N'RECRUITMENT', NULL, NULL, NULL, 1, 0, NULL, 6, NULL, 1, NULL)
INSERT [dbo].[tbl_navigation] ([Id], [ParentId], [Area], [Name], [Title], [Desc], [Action], [Controller], [Visible], [Authenticate], [CssClass], [SortOrder], [AbsoluteUri], [Active], [IconCss]) VALUES (51, 0, NULL, NULL, N'SERVICE', NULL, NULL, NULL, 1, 0, NULL, 3, NULL, 1, NULL)
INSERT [dbo].[tbl_navigation] ([Id], [ParentId], [Area], [Name], [Title], [Desc], [Action], [Controller], [Visible], [Authenticate], [CssClass], [SortOrder], [AbsoluteUri], [Active], [IconCss]) VALUES (52, 0, NULL, NULL, N'WHY US', NULL, NULL, NULL, 1, 0, NULL, 2, NULL, 1, NULL)
INSERT [dbo].[tbl_navigation] ([Id], [ParentId], [Area], [Name], [Title], [Desc], [Action], [Controller], [Visible], [Authenticate], [CssClass], [SortOrder], [AbsoluteUri], [Active], [IconCss]) VALUES (53, 0, NULL, NULL, N'PRODUCT', NULL, NULL, NULL, 1, 0, NULL, 4, NULL, 1, NULL)
INSERT [dbo].[tbl_navigation] ([Id], [ParentId], [Area], [Name], [Title], [Desc], [Action], [Controller], [Visible], [Authenticate], [CssClass], [SortOrder], [AbsoluteUri], [Active], [IconCss]) VALUES (54, 0, NULL, NULL, N'ABOUT US', NULL, NULL, NULL, 1, 0, NULL, 5, NULL, 1, NULL)
INSERT [dbo].[tbl_navigation] ([Id], [ParentId], [Area], [Name], [Title], [Desc], [Action], [Controller], [Visible], [Authenticate], [CssClass], [SortOrder], [AbsoluteUri], [Active], [IconCss]) VALUES (1055, 0, NULL, NULL, N'CONTACT - QUOTATION', NULL, NULL, NULL, 1, 0, NULL, 7, NULL, 1, NULL)
INSERT [dbo].[tbl_navigation] ([Id], [ParentId], [Area], [Name], [Title], [Desc], [Action], [Controller], [Visible], [Authenticate], [CssClass], [SortOrder], [AbsoluteUri], [Active], [IconCss]) VALUES (1060, 53, NULL, NULL, N'Phần mềm theo yêu cầu ', NULL, NULL, NULL, 1, 0, NULL, 0, NULL, 1, NULL)
INSERT [dbo].[tbl_navigation] ([Id], [ParentId], [Area], [Name], [Title], [Desc], [Action], [Controller], [Visible], [Authenticate], [CssClass], [SortOrder], [AbsoluteUri], [Active], [IconCss]) VALUES (1061, 53, NULL, NULL, N'Phần mềm đóng gói ', NULL, NULL, NULL, 1, 0, NULL, 1, NULL, 1, NULL)
INSERT [dbo].[tbl_navigation] ([Id], [ParentId], [Area], [Name], [Title], [Desc], [Action], [Controller], [Visible], [Authenticate], [CssClass], [SortOrder], [AbsoluteUri], [Active], [IconCss]) VALUES (1062, 53, NULL, NULL, N'Outsourcing', NULL, NULL, NULL, 1, 0, NULL, 3, NULL, 1, NULL)
SET IDENTITY_INSERT [dbo].[tbl_navigation] OFF
SET IDENTITY_INSERT [dbo].[tbl_navigation_lang] ON 

INSERT [dbo].[tbl_navigation_lang] ([Id], [NavigationId], [Title], [AbsoluteUri], [LangCode]) VALUES (1, 47, N'TRANG CHỦ', N'/', N'vi-VN')
INSERT [dbo].[tbl_navigation_lang] ([Id], [NavigationId], [Title], [AbsoluteUri], [LangCode]) VALUES (3, 54, N'VỀ CHÚNG TÔI', N'http://localhost:3130/article/detail/10/ve-chung-toi', N'vi-VN')
INSERT [dbo].[tbl_navigation_lang] ([Id], [NavigationId], [Title], [AbsoluteUri], [LangCode]) VALUES (5, 48, N'TUYỂN DỤNG', NULL, N'vi-VN')
INSERT [dbo].[tbl_navigation_lang] ([Id], [NavigationId], [Title], [AbsoluteUri], [LangCode]) VALUES (6, 49, N'Đa nền tảng', N'da-nen-tang', N'vi-VN')
INSERT [dbo].[tbl_navigation_lang] ([Id], [NavigationId], [Title], [AbsoluteUri], [LangCode]) VALUES (1004, 51, N'DỊCH VỤ', N'http://localhost:3130/article/detail/20/dich-vu', N'vi-VN')
INSERT [dbo].[tbl_navigation_lang] ([Id], [NavigationId], [Title], [AbsoluteUri], [LangCode]) VALUES (1005, 1055, N'LIÊN HỆ - BÁO GIÁ', N'http://localhost:3130/article/detail/13/bao-gia', N'vi-VN')
INSERT [dbo].[tbl_navigation_lang] ([Id], [NavigationId], [Title], [AbsoluteUri], [LangCode]) VALUES (1006, 47, N'ホーム', N'/', N'ja-JP')
INSERT [dbo].[tbl_navigation_lang] ([Id], [NavigationId], [Title], [AbsoluteUri], [LangCode]) VALUES (1007, 54, N'企業情報', N'http://localhost:3130/article/detail/10/about-us', N'ja-JP')
INSERT [dbo].[tbl_navigation_lang] ([Id], [NavigationId], [Title], [AbsoluteUri], [LangCode]) VALUES (1008, 52, N'選ばれる理由', N'http://localhost:3130/article/detail/12/invalid', N'ja-JP')
INSERT [dbo].[tbl_navigation_lang] ([Id], [NavigationId], [Title], [AbsoluteUri], [LangCode]) VALUES (1009, 52, N'TẠI SAO CHỌN CHÚNG TÔI', N'http://localhost:3130/article/detail/12/tai-sao-chon-chie-se', N'vi-VN')
INSERT [dbo].[tbl_navigation_lang] ([Id], [NavigationId], [Title], [AbsoluteUri], [LangCode]) VALUES (1010, 51, N'事業内容', N'http://localhost:3130/article/detail/20/services', N'ja-JP')
INSERT [dbo].[tbl_navigation_lang] ([Id], [NavigationId], [Title], [AbsoluteUri], [LangCode]) VALUES (1011, 53, N'開発事例', N'http://localhost:3130/article/detail/10/invalid', N'ja-JP')
INSERT [dbo].[tbl_navigation_lang] ([Id], [NavigationId], [Title], [AbsoluteUri], [LangCode]) VALUES (1012, 48, N'採用情報', NULL, N'ja-JP')
INSERT [dbo].[tbl_navigation_lang] ([Id], [NavigationId], [Title], [AbsoluteUri], [LangCode]) VALUES (1013, 1055, N'お問合せ・お見積', N'http://localhost:3130/article/detail/13/contact', N'ja-JP')
INSERT [dbo].[tbl_navigation_lang] ([Id], [NavigationId], [Title], [AbsoluteUri], [LangCode]) VALUES (1014, 53, N'SẢN PHẨM', N'http://localhost:3130/article/detail/10/san-pham', N'vi-VN')
INSERT [dbo].[tbl_navigation_lang] ([Id], [NavigationId], [Title], [AbsoluteUri], [LangCode]) VALUES (1015, 1060, N'Phần mềm theo yêu cầu', N'http://localhost:3130/article/detail/3/thiet-ke-va-tuy-chinh-phan-mem-theo-yeu-cau', N'vi-VN')
INSERT [dbo].[tbl_navigation_lang] ([Id], [NavigationId], [Title], [AbsoluteUri], [LangCode]) VALUES (1016, 1060, N'システム開発', N'http://localhost:3130/article/detail/3/development', N'ja-JP')
INSERT [dbo].[tbl_navigation_lang] ([Id], [NavigationId], [Title], [AbsoluteUri], [LangCode]) VALUES (1017, 1061, N'Phần mềm đóng gói', N'http://localhost:3130/article/detail/2/phan-mem-dong-goi', N'vi-VN')
INSERT [dbo].[tbl_navigation_lang] ([Id], [NavigationId], [Title], [AbsoluteUri], [LangCode]) VALUES (1018, 1061, N'パッケージソフト', N'http://localhost:3130/article/detail/2/package-software', N'ja-JP')
INSERT [dbo].[tbl_navigation_lang] ([Id], [NavigationId], [Title], [AbsoluteUri], [LangCode]) VALUES (1019, 1062, N'Outsourcing', N'http://localhost:3130/article/detail/1/dich-vu', N'vi-VN')
INSERT [dbo].[tbl_navigation_lang] ([Id], [NavigationId], [Title], [AbsoluteUri], [LangCode]) VALUES (1020, 1062, N'オフショア開発', N'http://localhost:3130/article/detail/1/service', N'ja-JP')
SET IDENTITY_INSERT [dbo].[tbl_navigation_lang] OFF
SET IDENTITY_INSERT [dbo].[tbl_page] ON 

INSERT [dbo].[tbl_page] ([Id], [Title], [Controller], [Action], [IsBlankPage], [PageTemplateId], [CustomTemplate], [SortOrder], [CreatedBy], [CreatedDate], [LastUpdatedBy], [LastUpdated], [Status]) VALUES (1, NULL, N'Tester', N'Index', 0, 1, NULL, NULL, NULL, NULL, NULL, CAST(N'2019-06-25T10:31:23.313' AS DateTime), 1)
SET IDENTITY_INSERT [dbo].[tbl_page] OFF
SET IDENTITY_INSERT [dbo].[tbl_page_lang] ON 

INSERT [dbo].[tbl_page_lang] ([Id], [PageId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode]) VALUES (1, 1, N'Home', N'Home page', N'<p>Bla bla bla</p>', N'/', N'en-US')
INSERT [dbo].[tbl_page_lang] ([Id], [PageId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode]) VALUES (2, 1, N'Trang chủ', N'Trang chủ', N'<p>Bla bla bla</p>
<p>&nbsp;</p>
<p><img src="../../../Media/Uploads/Thu%20vien/3.jpg" alt="" width="500" height="262" /></p>
<p>&nbsp;</p>', N'trang-chu', N'vi-VN')
SET IDENTITY_INSERT [dbo].[tbl_page_lang] OFF
SET IDENTITY_INSERT [dbo].[tbl_page_template] ON 

INSERT [dbo].[tbl_page_template] ([Id], [Widgets], [IsDefault], [Status]) VALUES (1, N'1,2,-1', 1, 1)
SET IDENTITY_INSERT [dbo].[tbl_page_template] OFF
SET IDENTITY_INSERT [dbo].[tbl_page_template_lang] ON 

INSERT [dbo].[tbl_page_template_lang] ([Id], [Name], [PageTemplateId], [LangCode]) VALUES (1, N'Mặc định', 1, N'vi-VN')
SET IDENTITY_INSERT [dbo].[tbl_page_template_lang] OFF
SET IDENTITY_INSERT [dbo].[tbl_post] ON 

INSERT [dbo].[tbl_post] ([Id], [Title], [IsHighlights], [Cover], [CategoryId], [CreatedDate], [CreatedBy], [Status]) VALUES (1, NULL, 0, N'Media/Uploads/mang.jpg', 3, CAST(N'2019-06-21T08:50:16.437' AS DateTime), NULL, 1)
INSERT [dbo].[tbl_post] ([Id], [Title], [IsHighlights], [Cover], [CategoryId], [CreatedDate], [CreatedBy], [Status]) VALUES (2, NULL, 0, N'Media/Uploads/Slide0_1.PNG', 3, CAST(N'2019-06-21T10:37:45.423' AS DateTime), NULL, 1)
INSERT [dbo].[tbl_post] ([Id], [Title], [IsHighlights], [Cover], [CategoryId], [CreatedDate], [CreatedBy], [Status]) VALUES (3, NULL, 0, N'Media/Uploads/featured/3.jpg', 3, CAST(N'2019-06-21T14:55:59.950' AS DateTime), N'94E7515B-09B1-4B90-872E-6D544BA4A339', 1)
INSERT [dbo].[tbl_post] ([Id], [Title], [IsHighlights], [Cover], [CategoryId], [CreatedDate], [CreatedBy], [Status]) VALUES (4, NULL, 0, N'Media/Uploads/slider/2.jpg', 2, CAST(N'2019-06-21T15:27:13.220' AS DateTime), N'94E7515B-09B1-4B90-872E-6D544BA4A339', 1)
INSERT [dbo].[tbl_post] ([Id], [Title], [IsHighlights], [Cover], [CategoryId], [CreatedDate], [CreatedBy], [Status]) VALUES (5, N'Customers', 0, N'Media/Uploads/slider/3.jpg', 2, CAST(N'2019-06-25T10:29:13.550' AS DateTime), N'94E7515B-09B1-4B90-872E-6D544BA4A339', 1)
INSERT [dbo].[tbl_post] ([Id], [Title], [IsHighlights], [Cover], [CategoryId], [CreatedDate], [CreatedBy], [Status]) VALUES (6, N'Test', 0, N'Media/Uploads/slider/1.jpg', 2, CAST(N'2019-07-08T13:06:15.093' AS DateTime), N'94E7515B-09B1-4B90-872E-6D544BA4A339', 1)
INSERT [dbo].[tbl_post] ([Id], [Title], [IsHighlights], [Cover], [CategoryId], [CreatedDate], [CreatedBy], [Status]) VALUES (7, N'Great Team Work.', 0, N'Media/Uploads/featured/3.jpg', 4, CAST(N'2019-07-08T15:50:32.687' AS DateTime), N'94E7515B-09B1-4B90-872E-6D544BA4A339', 1)
INSERT [dbo].[tbl_post] ([Id], [Title], [IsHighlights], [Cover], [CategoryId], [CreatedDate], [CreatedBy], [Status]) VALUES (8, N'Amazing Career Prospects.', 0, N'Media/Uploads/carousel/1.jpg', 4, CAST(N'2019-07-08T15:52:38.243' AS DateTime), N'94E7515B-09B1-4B90-872E-6D544BA4A339', 1)
INSERT [dbo].[tbl_post] ([Id], [Title], [IsHighlights], [Cover], [CategoryId], [CreatedDate], [CreatedBy], [Status]) VALUES (9, N'Beautiful Workspace.', 0, N'Media/Uploads/tri-an(1).jpg', 4, CAST(N'2019-07-08T15:53:43.767' AS DateTime), N'94E7515B-09B1-4B90-872E-6D544BA4A339', 9)
INSERT [dbo].[tbl_post] ([Id], [Title], [IsHighlights], [Cover], [CategoryId], [CreatedDate], [CreatedBy], [Status]) VALUES (10, N'Bloomberg smart cities; change-makers economic security', 0, N'Media/Uploads/IMG_5568.jpg', 1, CAST(N'2019-07-10T14:23:09.927' AS DateTime), N'94E7515B-09B1-4B90-872E-6D544BA4A339', 1)
INSERT [dbo].[tbl_post] ([Id], [Title], [IsHighlights], [Cover], [CategoryId], [CreatedDate], [CreatedBy], [Status]) VALUES (11, N'Medicine new approaches communities, outcomes partnership', 0, N'Media/Uploads/Blog/11.jpg', 1, CAST(N'2019-07-10T14:36:13.653' AS DateTime), N'94E7515B-09B1-4B90-872E-6D544BA4A339', 9)
INSERT [dbo].[tbl_post] ([Id], [Title], [IsHighlights], [Cover], [CategoryId], [CreatedDate], [CreatedBy], [Status]) VALUES (12, N'Test artilce', 0, N'Media/Uploads/why-us-png-6.png', 1, CAST(N'2019-07-10T16:19:19.320' AS DateTime), N'94E7515B-09B1-4B90-872E-6D544BA4A339', 1)
INSERT [dbo].[tbl_post] ([Id], [Title], [IsHighlights], [Cover], [CategoryId], [CreatedDate], [CreatedBy], [Status]) VALUES (13, N'Báo giá', 0, N'Media/Uploads/lien-he-new-wings-696x696.jpg', 1, CAST(N'2019-07-18T15:26:21.020' AS DateTime), N'94E7515B-09B1-4B90-872E-6D544BA4A339', 1)
INSERT [dbo].[tbl_post] ([Id], [Title], [IsHighlights], [Cover], [CategoryId], [CreatedDate], [CreatedBy], [Status]) VALUES (14, N'bản sắc công ty', 0, NULL, 1, CAST(N'2019-07-23T09:59:42.630' AS DateTime), N'94E7515B-09B1-4B90-872E-6D544BA4A339', 9)
INSERT [dbo].[tbl_post] ([Id], [Title], [IsHighlights], [Cover], [CategoryId], [CreatedDate], [CreatedBy], [Status]) VALUES (15, N'THƯ NGỎ', 0, NULL, 1, CAST(N'2019-07-23T10:12:56.090' AS DateTime), N'94E7515B-09B1-4B90-872E-6D544BA4A339', 9)
INSERT [dbo].[tbl_post] ([Id], [Title], [IsHighlights], [Cover], [CategoryId], [CreatedDate], [CreatedBy], [Status]) VALUES (16, N'HỒ SƠ CHIE SE', 0, NULL, 1, CAST(N'2019-07-23T10:15:29.920' AS DateTime), N'94E7515B-09B1-4B90-872E-6D544BA4A339', 9)
INSERT [dbo].[tbl_post] ([Id], [Title], [IsHighlights], [Cover], [CategoryId], [CreatedDate], [CreatedBy], [Status]) VALUES (17, N'LỊCH SỬ CHIE SE ', 0, N'Media/Uploads/IMG_5568.jpg', 1, CAST(N'2019-07-23T10:46:17.770' AS DateTime), N'94E7515B-09B1-4B90-872E-6D544BA4A339', 9)
INSERT [dbo].[tbl_post] ([Id], [Title], [IsHighlights], [Cover], [CategoryId], [CreatedDate], [CreatedBy], [Status]) VALUES (18, N'BẢN SẮC CHIE SE', 0, NULL, 1, CAST(N'2019-07-23T10:51:59.033' AS DateTime), N'94E7515B-09B1-4B90-872E-6D544BA4A339', 9)
INSERT [dbo].[tbl_post] ([Id], [Title], [IsHighlights], [Cover], [CategoryId], [CreatedDate], [CreatedBy], [Status]) VALUES (19, N'DỊCH VỤ', 0, NULL, 1, CAST(N'2019-07-24T10:53:24.587' AS DateTime), N'94E7515B-09B1-4B90-872E-6D544BA4A339', 9)
INSERT [dbo].[tbl_post] ([Id], [Title], [IsHighlights], [Cover], [CategoryId], [CreatedDate], [CreatedBy], [Status]) VALUES (20, N'DỊCH VỤ', 0, N'Media/Uploads/20201502087750.jpg', 1, CAST(N'2019-07-24T11:24:27.110' AS DateTime), N'94E7515B-09B1-4B90-872E-6D544BA4A339', 1)
INSERT [dbo].[tbl_post] ([Id], [Title], [IsHighlights], [Cover], [CategoryId], [CreatedDate], [CreatedBy], [Status]) VALUES (21, N'お問合せ・お見積り', 0, NULL, 1, CAST(N'2019-07-24T11:32:08.183' AS DateTime), N'94E7515B-09B1-4B90-872E-6D544BA4A339', 9)
INSERT [dbo].[tbl_post] ([Id], [Title], [IsHighlights], [Cover], [CategoryId], [CreatedDate], [CreatedBy], [Status]) VALUES (22, N'VỀ CHÚNG TÔI', 0, N'Media/Uploads/carousel/2.jpg', 1, CAST(N'2019-07-25T09:07:47.117' AS DateTime), N'94E7515B-09B1-4B90-872E-6D544BA4A339', 1)
SET IDENTITY_INSERT [dbo].[tbl_post] OFF
SET IDENTITY_INSERT [dbo].[tbl_post_lang] ON 

INSERT [dbo].[tbl_post_lang] ([Id], [PostId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (1, 1, N'1000+ HTML Pages Included', N'We provide multiple good servies for all users', N'<p>Lorem Ipsum is simply dummy text of the printing and typesetting industry.</p>
<p><img src="../../../Media/Uploads/featured/2.jpg" alt="" width="300" height="188" /></p>
<p>Lorem Ipsum has been the industry''s standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum</p>
<p><img src="../../../Media/Uploads/featured/1.jpg" alt="" width="800" height="500" /></p>', N'1000-html-pages-included', N'en-US', 1)
INSERT [dbo].[tbl_post_lang] ([Id], [PostId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (2, 1, N'OUTSOURCING', NULL, N'<p> </p>
<p> </p>', N'outsourcing', N'vi-VN', 1)
INSERT [dbo].[tbl_post_lang] ([Id], [PostId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (3, 1, N'オフショア開発', NULL, N'<p style="background-color: transparent; color: #000000; cursor: text; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; outline-color: transparent; outline-style: none; outline-width: 0px; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"> </p>
<p style="background-color: transparent; color: #000000; cursor: text; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; outline-color: transparent; outline-style: none; outline-width: 0px; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"> </p>', N'invalid', N'ja-JP', 1)
INSERT [dbo].[tbl_post_lang] ([Id], [PostId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (4, 2, N'Retina Ready Display', N'Globally parallel task premium infomediaries', N'<p>Globally parallel task premium infomediaries</p>
<p>Globally parallel task premium infomediaries</p>
<p>Globally parallel task premium infomediaries</p>', N'retina-ready-display', N'en-US', 1)
INSERT [dbo].[tbl_post_lang] ([Id], [PostId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (5, 3, N'THIẾT KẾ VÀ TÙY CHỈNH PHẦN MỀM ', NULL, N'<p><span style="color: #000000;">Chúng tôi chuyên thiết kế các phần mềm theo yêu cầu của khách hàng, </span><br /><span style="color: #000000;">đặc biệt là những phần mềm liên quan đến ứng dụng nghiệp vụ: <span style="text-align: left; text-transform: none; text-indent: 0px; letter-spacing: normal; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; text-decoration: none; word-spacing: 0px; display: inline !important; white-space: normal; cursor: text; orphans: 2; float: none; -webkit-text-stroke-width: 0px; background-color: transparent;">phần mềm</span> quản lý kho, <span style="background-color: transparent; color: #000000; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;">quản lý tài sản</span>, nhân sự, bán hàng....</span><br /><span style="color: #000000;">hoặc những phần mềm ứng dụng điều khiển: phần mềm quản lý sản xuất ở nhà máy, phần mềm điều khiển dây chuyền sản xuất,</span><br /><span style="color: #000000;">điều khiển theo quy trình, quản lý bãi đỗ xe, giao diện DCS, PLC....</span></p>
<p><span style="color: #000000;">Một số phần mềm tiêu biểu như: </span></p>
<hr />
<p><strong><span style="text-decoration: underline;">1. PHẦN MỀM TRUY XUẤT NGUỒN GỐC LINH KIỆN</span> </strong></p>
<p><a href="../../../ourproject/detail/6/phan-mem-truy-xuat-nguon-goc-linh-kien"><img src="../../../Media/Uploads/quet-ma-qr-de-nhan-dien-san-pham-nem-kim-cuong-chinh-hang-qua-tien-loi.png" alt="" width="300" height="300" /></a></p>
<hr />
<p><span style="text-decoration: underline;"><strong>2. PHẦN MỀM TẠO FILE IN TỰ ĐỘNG BẰNG MÁY IN KỸ THUẬT SỐ </strong></span></p>
<p><a href="../../../ourproject/detail/5/phan-mem-tao-file-in-tu-dong-pass"><img src="../../../Media/Uploads/may-in-ma-vach-chinh-hang.jpg" alt="" width="396" height="263" /></a></p>
<p> </p>
<hr />
<p><span style="text-decoration: underline;"><strong>3. PHẦN MỀM HỖ TRỢ KÊ KHAI HẢI QUAN ECUS SUB SYSTEM</strong></span></p>
<p><a href="../../../ourproject/detail/4/suzumoto-system"><img src="../../../Media/Uploads/Slide4_1.JPG" alt="" width="760" height="570" /></a></p>', N'thiet-ke-va-tuy-chinh-phan-mem', N'vi-VN', 1)
INSERT [dbo].[tbl_post_lang] ([Id], [PostId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (6, 2, N'PHẦN MỀM ĐÓNG GÓI', NULL, N'<p><span style="text-decoration: underline;"><strong>PHẦN MỀM ĐÓNG GÓI</strong></span></p>
<p><a href="../../../ourproject/detail/1/goi-phan-mem-quan-ly-kho-zaikan"><img src="../../../Media/Uploads/Slide0.PNG" alt="Slide0" width="781" height="541" /></a></p>', N'phan-mem-dong-goi', N'vi-VN', 1)
INSERT [dbo].[tbl_post_lang] ([Id], [PostId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (8, 4, N'Chúng tôi nỗ lực mỗi ngày để cung cấp dịch vụ chất lượng với giá trị gia tăng cao.', NULL, N'<p> </p>
<h5> </h5>', N'http://localhost:3130/article/detail/12/tai-sao-chon-chie-se', N'vi-VN', 1)
INSERT [dbo].[tbl_post_lang] ([Id], [PostId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (9, 5, N'Flexible Works.', N'Completely incubate worldwide users before imperatives.', N'<p><img src="../../Media/Uploads/Thu%20vien/chiese_logo.png" alt="" width="500" height="637" />Completely incubate worldwide users before imperatives.</p>
<p>Completely incubate worldwide users before imperatives.</p>
<p>Completely incubate worldwide users before imperatives.</p>
<p>Completely incubate worldwide users before imperatives.</p>', N'flexible-works', N'en-US', 1)
INSERT [dbo].[tbl_post_lang] ([Id], [PostId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (10, 5, N'Chúng tôi thiết kế phần mềm quy mô vừa và nhỏ theo sát yêu cầu.', NULL, NULL, N'http://localhost:3130/article/detail/20/dich-vu', N'vi-VN', 1)
INSERT [dbo].[tbl_post_lang] ([Id], [PostId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (11, 3, N'Ultra Responsive Design', N'Energistically visualize market-driven.', N'<p>Energistically visualize market-driven.</p>
<p><img src="../../../Media/Uploads/featured/3.jpg" alt="" width="800" height="500" /></p>
<p>&nbsp;</p>', N'ultra-responsive-design', N'en-US', 1)
INSERT [dbo].[tbl_post_lang] ([Id], [PostId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (12, 4, N'Amazing Prospects.', N'Seamlessly engineer effective synergy after e-business experiences.', N'<p>Seamlessly engineer effective synergy after e-business experiences.</p>
<p>&nbsp;</p>
<p>Seamlessly engineer effective synergy after e-business experiences.</p>
<p>&nbsp;</p>
<p>Seamlessly engineer effective synergy after e-business experiences.</p>
<p>Seamlessly engineer effective synergy after e-business experiences.</p>
<p>Seamlessly engineer effective synergy after e-business experiences.</p>
<p><img src="../../../Media/Uploads/slider/1.jpg" alt="" width="500" height="333" /></p>', N'amazing-prospects', N'en-US', 1)
INSERT [dbo].[tbl_post_lang] ([Id], [PostId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (13, 6, N'Chiese Company', N'Quickly support 24/7', N'<p>Quickly communicate bleeding-edge best practices.</p>
<p>&nbsp;</p>
<p><video controls="controls" width="300" height="150">
<source src="https://www.youtube.com/" /></video></p>
<p>&nbsp;</p>
<p>Quickly communicate bleeding-edge best practices.</p>
<p>&nbsp;</p>
<p>Quickly communicate bleeding-edge best practices.</p>
<p>&nbsp;</p>
<p>Quickly communicate bleeding-edge best practices.</p>
<p>&nbsp;</p>
<p>Quickly communicate bleeding-edge best practices.</p>
<p>&nbsp;</p>
<p>Quickly communicate bleeding-edge best practices.</p>', N'http://localhost:3130/article/detail/17/history', N'en-US', 1)
INSERT [dbo].[tbl_post_lang] ([Id], [PostId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (14, 6, N'弊社はシステム開発会社です。', NULL, NULL, N'http://localhost:3130/article/detail/10/about-us', N'ja-JP', 1)
INSERT [dbo].[tbl_post_lang] ([Id], [PostId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (15, 5, N'最適な中小規模システムをオーダーメイドで開発します。', NULL, NULL, N'http://localhost:3130/article/detail/20/service', N'ja-JP', 1)
INSERT [dbo].[tbl_post_lang] ([Id], [PostId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (16, 6, N'Chúng tôi là công ty phần mềm', NULL, NULL, N'http://localhost:3130/article/detail/10/ve-chung-toi', N'vi-VN', 1)
INSERT [dbo].[tbl_post_lang] ([Id], [PostId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (17, 7, N'Great Team Work.', N'Uniquely plagiarize dynamic convergence after equity invested experiences. Holisticly repurpose installed base infomediaries before web-enabled methods of empowerment.', N'<p>Uniquely plagiarize dynamic convergence after equity invested experiences. Holisticly repurpose installed base infomediaries before web-enabled methods of empowerment.</p>
<p><img src="../../Media/Uploads/carousel/1.jpg" alt="" width="1440" height="820" /></p>', N'great-team-work', N'en-US', 1)
INSERT [dbo].[tbl_post_lang] ([Id], [PostId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (18, 8, N'Amazing Career Prospects.', N'Lorem ipsum dolor sit amet, consectetur adipisicing elit. Dolor mollitia dignissimos, assumenda consequuntur consectetur! Laborum reiciendis, accusamus possimus et similique nisi obcaecati ex doloremque ea odio.', N'<p><iframe src="//www.youtube.com/embed/coQ2tllkGms" width="560" height="314" allowfullscreen="allowfullscreen"></iframe></p>
<p>&nbsp;</p>
<p>Lorem ipsum dolor sit amet, consectetur adipisicing elit. Dolor mollitia dignissimos,</p>
<p>&nbsp;</p>', N'amazing-career-prospects', N'en-US', 1)
INSERT [dbo].[tbl_post_lang] ([Id], [PostId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (19, 9, N'Beautiful Workspace 111.', N'Dolor mollitia dignissimos, assumenda consequuntur consectetur! Laborum reiciendis, error explicabo consectetur adipisci, accusamus possimus et similique nisi obcaecati ex doloremque ea odio.', N'<p>Dolor mollitia dignissimos, assumenda consequuntur consectetur! Laborum reiciendis, error explicabo consectetur adipisci, accusamus possimus et similique nisi obcaecati ex doloremque ea odio.</p>
<p>&nbsp;</p>
<p>Dolor mollitia dignissimos, assumenda consequuntur consectetur! Laborum reiciendis, error explicabo consectetur adipisci, accusamus possimus et similique nisi obcaecati ex doloremque ea odio.</p>
<p>&nbsp;</p>
<p><iframe src="//www.youtube.com/embed/6bWyBNnyZm4" width="800" height="449" allowfullscreen="allowfullscreen"></iframe></p>', N'beautiful-workspace-111', N'en-US', 1)
INSERT [dbo].[tbl_post_lang] ([Id], [PostId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (20, 10, N'Train crashes into taxi in central Vietnam, killing two', N'Railway staff and local residents join a rescue mission after a taxi cab was hit by a train in Quang Ngai Province', N'<div class="author">By&nbsp;<a>Pham Linh</a>&nbsp; &nbsp;July 9, 2019 | 09:28 pm GMT+7
<div class="nav_share">&nbsp;</div>
</div>
<div class="thumb_detail_top"><img class="vne_lazy_image lazyloaded" src="https://i-english.vnecdn.net/2019/07/09/settop179811562677074-15626808-8644-9324-1562681020_680x0.jpg" alt="Train crashes into taxi in central Vietnam, killing two" data-original="https://i-english.vnecdn.net/2019/07/09/settop179811562677074-15626808-8644-9324-1562681020_680x0.jpg" />
<div class="caption_thumb_detail_top">&nbsp;</div>
<div class="caption_thumb_detail_top">&nbsp;</div>
<div class="caption_thumb_detail_top" style="text-align: left;">Railway staff and local residents join a rescue mission after a taxi cab was hit by a train in Quang Ngai Province, July 9, 2019. Photo by VnExpress/Binh Son.</div>
</div>
<h2 class="lead_post_detail row" style="text-align: left;">Two taxi passengers were killed when their car was hit by a train at a level crossing in Quang Ngai Province on Tuesday afternoon.</h2>
<div class="fck_detail">
<p class="Normal" style="text-align: left;">The taxi cab carrying four passengers was thrown more than 10 meters from the tracks after it was hit by the north-bound train.</p>
<p class="Normal" style="text-align: left;">A 65-year-old woman and a one-year-old girl died on the spot.</p>
<p class="Normal" style="text-align: left;">The taxi driver, 23, and the other two passengers, a 22-year-old woman and a two-year-old boy, were rushed to hospital with serious injuries.</p>
<p class="Normal" style="text-align: left;">The train resumed its journey half an hour later. Railway traffic police are investigating the cause of the accident.</p>
<p class="Normal" style="text-align: left;">Official statistics shows about 267 railroad incidents occurred across the country last year, down 22.8 percent against 2017. 124 people were killed and 184 others injured.</p>
<p class="Normal" style="text-align: left;">A report by the Vietnam Railways showed that there are 5,793 level crossings across the country. Out of those, only 641 have stationed guards, 366 have automatic alarms and 507 have warning signs.</p>
</div>', N'train-crashes-into-taxi-in-central-vietnam-killing-two', N'en-US', 1)
INSERT [dbo].[tbl_post_lang] ([Id], [PostId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (21, 11, N'Hanoi opens West Lake floodgate', N'Hanoi opens West Lake floodgate to maintain level, clean river', N'<div class="author"><br />
<div class="nav_share">&nbsp;</div>
</div>
<div class="thumb_detail_top"><img class="vne_lazy_image lazyloaded" src="https://i-english.vnecdn.net/2019/07/09/Tolich63051562657290-156267345-7209-9369-1562673486_680x0.jpg" alt="Hanoi opens West Lake floodgate to maintain level, clean river" data-original="https://i-english.vnecdn.net/2019/07/09/Tolich63051562657290-156267345-7209-9369-1562673486_680x0.jpg" />
<div class="caption_thumb_detail_top">&nbsp;</div>
<div class="caption_thumb_detail_top">&nbsp;</div>
<div class="caption_thumb_detail_top">A floodgate from the West Lake near Trich Sai Street is opened to reduce its level and to help clean pollution in the To Lich River on July 9, 2019. Photo by VnExpress/Ba Do.</div>
</div>
<h2 class="lead_post_detail row">The floodgate in Hanoi&rsquo;s West Lake was opened on Tuesday to regulate its level and let water into the To Lich River to clean it.</h2>
<div class="fck_detail">
<p class="Normal">The opening of the floodgate, near Trich Sai Street, depends on the water level in the lake, a spokesperson for the Hanoi Sewage and Water Drainage Company said.</p>
<p class="Normal">"As per regulations, the West Lake&rsquo;s water level must be maintained at 5.7 meters, but the current level is almost six meters."</p>
<p class="Normal">An ongoing trial to improve the To Lich River&rsquo;s water quality using technologies from Japan and Germany would continue, he added.</p>
<p class="Normal">After nearly two months, the water in the river has changed color from black to green in some areas and less foul-smelling than usual. The sludge at the bottom has also been decomposed.</p>
<p class="Normal">"It&rsquo;s been a long time since I last saw green water in the To Lich," Nguyen Thi Xuan, a local resident, said.</p>
<p class="Normal">"If the floodgate can be opened once a week to clean the river, it would be great."</p>
<p class="Normal">The final results from the foreign intervention are expected to be announced at the end of this month.</p>
<p class="Normal">This is not the first time water from the West Lake, Hanoi''s largest freshwater body, has been let into the To Lich to clean it. The company did it once last year and the results were encouraging.</p>
<p class="Normal">&nbsp;</p>
<p class="Normal">The To Lich used to be a branch of the Red River but was delinked by the French by filling a section as part of a city plan in 1889.</p>
<p class="Normal">Over 200 sewage outlets empty 150,000 cubic meters of untreated household wastewater every day into it, according to the city Department of Natural Resources and Environment.&nbsp;Wastewater from factories also contribute to the river&rsquo;s pollution.</p>
</div>', N'hanoi-opens-west-lake-floodgate', N'en-US', 1)
INSERT [dbo].[tbl_post_lang] ([Id], [PostId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (22, 12, N'Test artilce', N'Test artilce Test artilce Test artilce Test artilce Test artilce Test artilce', N'<p>Test artilce Test artilce Test artilce Test artilce Test artilce Test artilce</p>
<p>Test artilce Test artilce Test artilce Test artilce Test artilce Test artilce</p>
<p>Test artilce Test artilce Test artilce Test artilce Test artilce Test artilce</p>
<p>&nbsp;</p>
<p>Test artilce Test artilce Test artilce Test artilce Test artilce Test artilce</p>
<p>Test artilce Test artilce Test artilce Test artilce Test artilce Test artilce</p>
<p>&nbsp;</p>', N'test-artilce', N'en-US', 1)
INSERT [dbo].[tbl_post_lang] ([Id], [PostId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (23, 11, N'DỊCH VỤ', NULL, N'<p><img src="../../../Media/Uploads/Slide2.JPG" alt="" width="940" height="705" /></p>
<p>※ Ngo&agrave;i ra ch&uacute;ng t&ocirc;i c&ograve;n cung cấp c&aacute;c dịch vụ kh&aacute;c như:<br />・Cung cấp c&aacute;c phần mềm đ&oacute;ng g&oacute;i tự sản xuất với chi ph&iacute; hợp l&yacute;<br />・Thiết kế website c&ocirc;ng ty<br />・Bi&ecirc;n dịch ng&ocirc;n ngữ phần mềm (tiếng Việt, tiếng Nhật) <br />・Đ&ocirc;i khi ch&uacute;ng t&ocirc;i cũng đưa ra những đề xuất "kh&ocirc;ng cần tạo phần mềm" sau khi nghi&ecirc;n cứu y&ecirc;u cầu của kh&aacute;ch h&agrave;ng.</p>
<p><img src="../../../Media/Uploads/Slide3.JPG" alt="" width="960" height="720" /></p>', N'dich-vu', N'vi-VN', 1)
INSERT [dbo].[tbl_post_lang] ([Id], [PostId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (24, 13, N'LIÊN HỆ - BÁO GIÁ', NULL, N'<p style="text-align: center;"><span style="font-size: 14pt; font-family: arial, helvetica, sans-serif;"><strong><span style="text-decoration: underline;">LIÊN HỆ VỚI CHÚNG TÔI</span> </strong></span></p>
<table style="height: 136px; width: 771px;" width="695">
<tbody>
<tr style="height: 18px;">
<td style="width: 761px; height: 10px; text-align: left;" colspan="2">
<p><span style="color: #3366ff; font-family: arial, helvetica, sans-serif; font-size: 12pt;">                        CÔNG TY TNHH CHIA SẺ TRÍ TUỆ (CHIE SE)</span></p>
</td>
</tr>
<tr style="height: 36px;">
<td style="width: 226px; height: 36px; text-align: left;"><span style="color: #000000; font-family: arial, helvetica, sans-serif; font-size: 12pt;">                        Địa chỉ: </span></td>
<td style="width: 529px; height: 36px; text-align: left;"><span style="color: #000000; font-family: arial, helvetica, sans-serif; font-size: 12pt;">Tầng 19, tòa nhà ICON4, số 243A Đê La Thành, phường Láng Thượng, quận Đống Đa, thành phố Hà Nội, Việt Nam </span></td>
</tr>
<tr style="height: 36px;">
<td style="width: 226px; height: 36px; text-align: left;"><span style="color: #000000; font-family: arial, helvetica, sans-serif; font-size: 12pt;">                        Điện thoại:</span></td>
<td style="width: 529px; height: 36px; text-align: left;"><span style="color: #000000; font-family: arial, helvetica, sans-serif; font-size: 12pt;">(084) 43 200 2128</span></td>
</tr>
<tr style="height: 18px;">
<td style="width: 226px; height: 18px; text-align: left;"><span style="color: #000000; font-family: arial, helvetica, sans-serif; font-size: 12pt;">                        Fax:</span></td>
<td style="width: 529px; height: 18px; text-align: left;"><span style="color: #000000; font-family: arial, helvetica, sans-serif; font-size: 12pt;">(084) 43 726 5493</span></td>
</tr>
<tr style="height: 18px;">
<td style="width: 226px; height: 18px; text-align: left;"><span style="color: #000000; font-family: arial, helvetica, sans-serif; font-size: 12pt;">                        Email: </span></td>
<td style="width: 529px; height: 18px; text-align: left;"><span style="color: #000000; font-family: arial, helvetica, sans-serif; font-size: 12pt;"><a style="color: #000000;" href="mailto:maihuong@chiese.vn%20%20(Tiếng%20Việt)">maihuong@chiese.vn   (Tiếng Việt)</a></span></td>
</tr>
<tr style="height: 18px;">
<td style="width: 226px; height: 18px; text-align: left;"> </td>
<td style="width: 529px; height: 18px; text-align: left;"><span style="color: #000000; font-family: arial, helvetica, sans-serif; font-size: 12pt;"><a style="color: #000000;" href="mailto:kokuwano@chiese.vn%20%20%20(Tiếng%20Nhật)">kokuwano@chiese.vn   (Tiếng Nhật)</a></span></td>
</tr>
</tbody>
</table>
<hr />
<p><iframe style="border: 0;" src="https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d3724.1196623807623!2d105.80225791440736!3d21.02789749318482!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x3135ab42139e9c5f%3A0x6eca1d6b8b7323a4!2zVMOyYSBuaMOgIEljb240!5e0!3m2!1svi!2s!4v1563875277551!5m2!1svi!2s" width="400" height="300" frameborder="0" allowfullscreen="allowfullscreen"></iframe></p>', N'lien-he-bao-gia', N'vi-VN', 1)
INSERT [dbo].[tbl_post_lang] ([Id], [PostId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (25, 13, N'Pricing', N'Package Pricing', N'<p><img src="../../../Media/Uploads/Project/1562739808_download.png" alt="" width="800" height="600" /></p>
<p>&nbsp;</p>
<p>Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry''s standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.</p>
<p>&nbsp;</p>
<p><img src="../../../Media/Uploads/carousel/1.jpg" alt="" width="800" height="456" /></p>
<p>It is a long established fact that a reader will be distracted by the readable content of a page when looking at its layout. The point of using Lorem Ipsum is that it has a more-or-less normal distribution of letters, as opposed to using ''Content here, content here'', making it look like readable English. Many desktop publishing packages and web page editors now use Lorem Ipsum as their default model text, and a search for ''lorem ipsum'' will uncover many web sites still in their infancy. Various versions have evolved over the years, sometimes by accident, sometimes on purpose (injected humour and the like).</p>', N'pricing', N'en-US', 1)
INSERT [dbo].[tbl_post_lang] ([Id], [PostId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (26, 12, N'TẠI SAO CHỌN CHIE SE', NULL, N'<table style="width: 100.68%; height: 227px; border-collapse: collapse;" border="1">
<tbody>
<tr style="height: 220px;">
<td style="width: 54.98%; height: 227px;">
<p><strong><em>1. Thế mạnh của chúng tôi là cung cấp các phần mềm quy mô vừa và nhỏ trong thời gian ngắn với chi phí hợp lý. Chúng tôi không cung cấp các phần mềm đa chức năng chi phí lớn hướng đến nhiều đối tượng doanh nghiệp, mà được đánh giá cao bởi các doanh nghiệp muốn tùy chỉnh phần mềm sẵn có theo yêu cầu thực tế hoặc chỉ cần mua các phần mềm tối giản thiết kế theo đúng nhu cầu. </em></strong><br /><strong><em>Ngoài ra chúng tôi cũng có thể thiết kế các phần mềm phụ phù hợp để khách hàng có thể sử dụng tại Việt Nam.</em></strong></p>
</td>
<td style="width: 6.34%; height: 227px;"><em> </em></td>
<td style="width: 42.39%; height: 227px;"><img src="../../../Media/Uploads/a.jpg" alt="" width="524" height="294" /></td>
</tr>
</tbody>
</table>
<p> </p>
<table style="width: 100.57%; height: 381px; border-collapse: collapse;" border="1">
<tbody>
<tr style="height: 56px;">
<td style="width: 36.14%; height: 56px;"><img src="../../../Media/Uploads/b.jpg" alt="" width="527" height="407" /></td>
<td style="width: 6.36%;"> </td>
<td style="width: 50.14%; height: 56px;"><em><strong><i>2. </i>Chúng tôi là công ty Nhật nên hiểu rõ những yêu cầu và mong đợi của khách hàng là các doanh nghiệp Nhật Bản. Chúng tôi luôn nỗ lực không ngừng đóng góp công sức nhỏ bé của mình để mang giá trị thẩm mỹ và chất lượng Nhật Bản ra thế giới.</strong></em></td>
</tr>
</tbody>
</table>
<p> </p>
<table style="width: 100.73%; height: 18px; border-collapse: collapse;" border="1">
<tbody>
<tr style="height: 18px;">
<td style="width: 54.49%; height: 18px;"><strong><em><i>3. </i>Chúng tôi có một đội nhóm không chỉ có năng lực chuyên môn kỹ thuật mà còn rất trẻ trung, nhiệt huyết, lạc quan, cầu tiến; biết lắng nghe, tích cực giải quyết vấn đề và tu dưỡng bản thân.</em></strong></td>
<td style="width: 6.66%;"> </td>
<td style="width: 39.33%; height: 18px;"><img src="../../../Media/Uploads/c.jpg" alt="" width="525" height="349" /></td>
</tr>
</tbody>
</table>
<p> </p>
<table style="width: 101.16%; height: 244px; border-collapse: collapse;" border="1">
<tbody>
<tr>
<td style="width: 38.85%;"><img src="../../../Media/Uploads/e.jpg" alt="" width="526" height="296" /></td>
<td style="width: 6.78%;"><em> </em></td>
<td style="width: 62.33%;"><strong><em><i>4. </i>Chúng tôi luôn ý thức bảo mật thông tin khách hàng và thiết lập môi trường để thực hiện mục tiêu đó.</em></strong></td>
</tr>
</tbody>
</table>
<p> </p>
<table style="width: 100.94%; border-collapse: collapse;" border="1">
<tbody>
<tr>
<td style="width: 54.29%;">
<p><strong><em>5. Chúng tôi nỗ lực mỗi ngày để có thể cung cấp cho khách hàng những dịch vụ tốt nhất vượt mong đợi với giá trị gia tăng cao bằng cách tận dụng và phát huy mọi nguồn lực có thể như: năng lực chuyên môn kỹ thuật, kinh nghiệm làm việc, khả năng sáng tạo,...</em></strong><br /><strong><em>Chúng tôi vinh hạnh và tự hào khi được trao cho cơ hội để giải quyết các vấn đề và đóng góp vào </em><em>sự phát triển của quý khách hàng.</em></strong></p>
</td>
<td style="width: 6.89%;">
<p><em> </em></p>
</td>
<td style="width: 39.7%;"><img src="../../../Media/Uploads/d.jpg" alt="" width="526" height="307" /></td>
</tr>
</tbody>
</table>
<p> </p>', N'tai-sao-chon-chie-se', N'vi-VN', 1)
INSERT [dbo].[tbl_post_lang] ([Id], [PostId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (27, 14, N'BẢN SẮC CÔNG TY', NULL, N'<p>・Ch&uacute;ng t&ocirc;i độc lập v&agrave; phụ tr&aacute;ch dự &aacute;n trực tiếp từ đầu đến cuối:<br />Ch&uacute;ng t&ocirc;i c&oacute; khả năng độc lập đưa ra c&aacute;c phương &aacute;n phần mềm ph&ugrave; hợp nhất với kh&aacute;ch h&agrave;ng kh&ocirc;ng phụ thuộc v&agrave;o nh&agrave; sản xuất hay c&ocirc;ng ty mẹ.<br />・Thế mạnh của ch&uacute;ng t&ocirc;i l&agrave; cung cấp c&aacute;c phần mềm quy m&ocirc; vừa v&agrave; nhỏ trong thời gian ngắn với chi ph&iacute; hợp l&yacute;. Ch&uacute;ng t&ocirc;i kh&ocirc;ng cung cấp c&aacute;c phần mềm đa chức năng chi ph&iacute; lớn hướng đến nhiều đối tượng doanh nghiệp m&agrave; được đ&aacute;nh gi&aacute; cao bởi c&aacute;c doanh nghiệp muốn t&ugrave;y chỉnh c&aacute;c phần mềm sẵn c&oacute; theo nhu cầu thực tế hoặc chỉ cần mua c&aacute;c phần mềm tối giản thiết kế theo đ&uacute;ng y&ecirc;u cầu. Ngo&agrave;i ra ch&uacute;ng t&ocirc;i cũng c&oacute; thể thiết kế c&aacute;c phần mềm phụ ph&ugrave; hợp để kh&aacute;ch h&agrave;ng c&oacute; thể sử dụng tại Việt Nam.<br />・Ch&uacute;ng t&ocirc;i lu&ocirc;n khuyến kh&iacute;ch c&aacute;c &yacute; tưởng s&aacute;ng tạo, tranh luận mang t&iacute;nh chất x&acirc;y dựng trong c&ocirc;ng ty để đưa ra được những lựa chọn tối ưu v&agrave; tạo ra những sản phẩm c&oacute; chất lượng cao.</p>', N'ban-sac-cong-ty', N'vi-VN', 1)
INSERT [dbo].[tbl_post_lang] ([Id], [PostId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (28, 15, N'VỀ CHÚNG TÔI', NULL, N'<p><span style="font-size: 14pt; color: #3366ff;"><em><strong>THƯ NGỎ&nbsp;</strong></em></span></p>
<table style="height: 468px; width: 989px;" width="862">
<tbody>
<tr style="height: 23px;">
<td style="width: 687px; height: 23px;"><em>Xin ch&agrave;o c&aacute;c bạn,</em></td>
<td style="width: 286px; height: 468px;" rowspan="7"><img src="../../../Media/Uploads/president.jpg" alt="" width="286" height="411" /></td>
</tr>
<tr style="height: 23px;">
<td style="width: 687px; height: 23px;"><em>T&ocirc;i l&agrave; Yamasaki Hirofumi - Gi&aacute;m đốc c&ocirc;ng ty phần mềm CHIE SE.</em></td>
</tr>
<tr style="height: 41px;">
<td style="width: 687px; height: 41px;"><em>Cảm ơn c&aacute;c bạn đ&atilde; ưu &aacute;i gh&eacute; thăm trang web của ch&uacute;ng t&ocirc;i trong số rất nhiều trang web kh&aacute;c nhau.</em></td>
</tr>
<tr style="height: 41px;">
<td style="width: 687px; height: 41px;"><em>Năm 2015 ch&uacute;ng t&ocirc;i đ&atilde; c&acirc;n nhắc mở rộng sang thị trường Việt Nam v&agrave; năm 2016 ch&iacute;nh thức th&agrave;nh lập c&ocirc;ng ty phần mềm mang t&ecirc;n "CHIE SE" tại H&agrave; Nội.&nbsp;</em></td>
</tr>
<tr style="height: 59px;">
<td style="width: 687px; height: 59px;"><em>T&ecirc;n c&ocirc;ng ty l&agrave; từ gh&eacute;p của từ "CHIE" (nghĩa l&agrave; "Tr&iacute; tuệ") trong tiếng Nhật v&agrave; "SE" (l&agrave; từ "Sẻ" trong cụm từ "Chia sẻ") trong tiếng Việt; mang th&ocirc;ng điệp Việt Nam v&agrave; Nhật Bản c&ugrave;ng chia sẻ kiến thức, kinh nghiệm v&agrave; s&aacute;ng kiến.</em></td>
</tr>
<tr style="height: 41px;">
<td style="width: 687px; height: 41px;"><em>C&ocirc;ng ty mẹ của CHIE SE cũng l&agrave; c&ocirc;ng ty phần mềm mang t&ecirc;n "GAINSHARING" được th&agrave;nh lập v&agrave;o năm 2003 tại Osaka, Nhật Bản.&nbsp;</em></td>
</tr>
<tr style="height: 240px;">
<td style="width: 687px; height: 240px;">
<p><em>Ở Nhật Bản, ch&uacute;ng t&ocirc;i cung cấp phần mềm cho rất nhiều kh&aacute;ch h&agrave;ng doanh nghiệp kh&aacute;c nhau. Với đối tượng kh&aacute;ch h&agrave;ng l&agrave; c&aacute;c doanh nghiệp (nh&agrave; m&aacute;y) sản xuất, ch&uacute;ng t&ocirc;i cung cấp c&aacute;c loại phần mềm điều khiển, gi&aacute;m s&aacute;t, quản l&yacute; sản xuất,.... Ch&uacute;ng t&ocirc;i c&ograve;n cung cấp c&aacute;c phần mềm nghiệp vụ cho kh&aacute;ch h&agrave;ng l&agrave; c&aacute;c doanh nghiệp vận tải, đ&agrave;i ph&aacute;t thanh,... Kh&aacute;ch h&agrave;ng đang sử dụng đa dạng c&aacute;c dịch vụ của ch&uacute;ng t&ocirc;i trong suốt qu&aacute; tr&igrave;nh từ tư vấn đến sản xuất v&agrave; bảo tr&igrave; phần mềm. Ch&uacute;ng t&ocirc;i đang ho&agrave;n thiện thể chế để ph&iacute;a Việt Nam cũng giống như ph&iacute;a Nhật Bản, c&oacute; thể cung cấp cho c&aacute;c doanh nghiệp sản xuất Nhật Bản phần mềm với chi ph&iacute; v&agrave; thời gian hợp l&yacute; dựa tr&ecirc;n những kiến thức, kinh nghiệm v&agrave; t&agrave;i sản đ&atilde; được t&iacute;ch lũy b&ecirc;n Nhật. C&ocirc;ng ty ch&uacute;ng t&ocirc;i tuy vẫn c&ograve;n trong giai đoạn đang ph&aacute;t triển nhưng sẽ lu&ocirc;n cố gắng để ho&agrave;n thiện; ph&aacute;t triển c&ugrave;ng v&agrave; đ&oacute;ng g&oacute;p v&agrave;o sự ph&aacute;t triển của Việt Nam. </em></p>
<p><em>Rất mong nhận được sự chỉ dẫn v&agrave; ủng hộ từ c&aacute;c bạn.</em></p>
</td>
</tr>
</tbody>
</table>
<p>&nbsp;</p>
<p><span style="font-size: 14pt; color: #3366ff;"><strong>HỒ SƠ CHIE SE</strong>&nbsp;</span></p>
<table style="width: 1689px; height: 90px;" width="998">
<tbody>
<tr style="height: 25px;">
<td style="width: 130px; height: 10px;">T&ecirc;n c&ocirc;ng ty:</td>
<td style="width: 502px; height: 10px;">C&ocirc;ng ty TNHH Chia Sẻ Tr&iacute; Tuệ (Chie Se)</td>
<td style="width: 18px; height: 90px;" rowspan="9">&nbsp;</td>
<td style="width: 1011px; height: 90px;" rowspan="9">&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;<img src="../../../Media/Uploads/images/2.jpg" alt="" width="425" height="551" /></td>
</tr>
<tr style="height: 74px;">
<td style="width: 130px; height: 10px;">Địa chỉ:</td>
<td style="width: 502px; height: 10px;">Tầng 19, t&ograve;a nh&agrave; ICON4, số 243A Đ&ecirc; La Th&agrave;nh, phường L&aacute;ng Thượng, quận Đống Đa, th&agrave;nh phố H&agrave; Nội, Việt Nam&nbsp;</td>
</tr>
<tr style="height: 26px;">
<td style="width: 130px; height: 10px;">Ng&agrave;y th&agrave;nh lập:</td>
<td style="width: 502px; height: 10px;">&nbsp;23/03/2016</td>
</tr>
<tr style="height: 196px;">
<td style="width: 130px; height: 10px;">Lĩnh vực hoạt động ch&iacute;nh <br />/Dịch vụ cung cấp ch&iacute;nh:</td>
<td style="width: 502px; height: 10px;">- Thực hiện v&agrave; sản xuất phần mềm đ&oacute;ng g&oacute;i, phần mềm theo y&ecirc;u cầu của kh&aacute;ch h&agrave;ng<br />&nbsp;- Thực hiện c&aacute;c dịch vụ bảo tr&igrave; hệ thống <br />&nbsp;- Tư vấn, hỗ trợ mua v&agrave; c&agrave;i đặt thiết bị phần cứng<br />&nbsp;- Bi&ecirc;n dịch ng&ocirc;n ngữ phần mềm (tiếng Việt, tiếng Nhật)</td>
</tr>
<tr style="height: 25px;">
<td style="width: 130px; height: 10px;">Người đại diện:</td>
<td style="width: 502px; height: 10px;">Gi&aacute;m đốc Yamasaki Hirofumi&nbsp;</td>
</tr>
<tr style="height: 26px;">
<td style="width: 130px; height: 10px;">Quy m&ocirc; nh&acirc;n sự:</td>
<td style="width: 502px; height: 10px;">6 người (t&iacute;nh đến 07/2019)</td>
</tr>
<tr style="height: 147px;">
<td style="width: 130px; height: 10px;">Kh&aacute;ch h&agrave;ng ti&ecirc;u biểu:</td>
<td style="width: 502px; height: 10px;">C&ocirc;ng ty Cổ phần Gainsharing<br />C&ocirc;ng ty TNHH Shihen Việt Nam<br />C&ocirc;ng ty TNHH Suzumoto Việt Nam <br />C&ocirc;ng ty TNHH KDDI Việt Nam<br />C&ocirc;ng ty TNHH RHYTHM PRECISION Việt Nam</td>
</tr>
<tr style="height: 50px;">
<td style="width: 130px; height: 10px;">Ng&acirc;n h&agrave;ng giao dịch:</td>
<td style="width: 502px; height: 10px;">Ng&acirc;n h&agrave;ng BIDV, Ng&acirc;n h&agrave;ng Mizuho</td>
</tr>
<tr style="height: 26px;">
<td style="width: 130px; height: 10px;">TEL:</td>
<td style="width: 502px; height: 10px;">(084) 43 200 2128</td>
</tr>
</tbody>
</table>
<p>&nbsp;</p>
<p><span style="font-size: 14pt; color: #3366ff;"><strong>BẢN SẮC CHIE SE</strong></span></p>
<table style="height: 384px; width: 1645px;" width="870">
<tbody>
<tr style="height: 102px;">
<td style="height: 102px; width: 633px;">
<p>・Ch&uacute;ng t&ocirc;i độc lập v&agrave; phụ tr&aacute;ch dự &aacute;n trực tiếp từ đầu đến cuối:&nbsp;</p>
<p>Ch&uacute;ng t&ocirc;i c&oacute; khả năng độc lập đưa ra c&aacute;c phương &aacute;n phần mềm ph&ugrave; hợp nhất với kh&aacute;ch h&agrave;ng kh&ocirc;ng phụ thuộc v&agrave;o nh&agrave; sản xuất hay c&ocirc;ng ty mẹ.</p>
</td>
<td style="width: 141px;">&nbsp;</td>
<td style="height: 102px; width: 849px;"><img src="../../../Media/Uploads/4.jpg" alt="" width="410" height="256" /></td>
</tr>
<tr style="height: 29px;">
<td style="width: 633px; height: 29px;">&nbsp;</td>
<td style="width: 141px;">&nbsp;</td>
<td style="width: 849px; height: 29px;">&nbsp;</td>
</tr>
<tr style="height: 162px;">
<td style="height: 162px; width: 633px;">・Thế mạnh của ch&uacute;ng t&ocirc;i l&agrave; cung cấp c&aacute;c phần mềm quy m&ocirc; vừa v&agrave; nhỏ trong thời gian ngắn với chi ph&iacute; hợp l&yacute;. Ch&uacute;ng t&ocirc;i kh&ocirc;ng cung cấp c&aacute;c phần mềm đa chức năng chi ph&iacute; lớn hướng đến nhiều đối tượng doanh nghiệp m&agrave; được đ&aacute;nh gi&aacute; cao bởi c&aacute;c doanh nghiệp muốn t&ugrave;y chỉnh c&aacute;c phần mềm sẵn c&oacute; theo nhu cầu thực tế hoặc chỉ cần mua c&aacute;c phần mềm tối giản thiết kế theo đ&uacute;ng y&ecirc;u cầu. Ngo&agrave;i ra ch&uacute;ng t&ocirc;i cũng c&oacute; thể thiết kế c&aacute;c phần mềm phụ ph&ugrave; hợp để kh&aacute;ch h&agrave;ng c&oacute; thể sử dụng tại Việt Nam.</td>
<td style="width: 141px;">&nbsp;</td>
<td style="height: 162px; width: 849px;"><img src="../../../Media/Uploads/3.jpg" alt="" width="400" height="256" /></td>
</tr>
<tr style="height: 37px;">
<td style="width: 633px; height: 37px;">&nbsp;</td>
<td style="width: 141px;">&nbsp;</td>
<td style="width: 849px; height: 37px;">&nbsp;</td>
</tr>
<tr style="height: 54px;">
<td style="height: 54px; width: 633px;">・Ch&uacute;ng t&ocirc;i lu&ocirc;n khuyến kh&iacute;ch c&aacute;c &yacute; tưởng s&aacute;ng tạo, tranh luận mang t&iacute;nh chất x&acirc;y dựng trong c&ocirc;ng ty để đưa ra được những lựa chọn tối ưu v&agrave; tạo ra những sản phẩm c&oacute; chất lượng cao.</td>
<td style="width: 141px;">&nbsp;</td>
<td style="height: 54px; width: 849px;"><img src="../../../Media/Uploads/5.jpg" alt="" width="390" height="312" /></td>
</tr>
</tbody>
</table>
<p><img src="../../../Media/Uploads/Slide6.JPG" alt="" width="1222" height="916" /></p>', N've-chung-toi', N'vi-VN', 1)
INSERT [dbo].[tbl_post_lang] ([Id], [PostId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (29, 16, N'HỒ SƠ CHIE SE', NULL, N'<table style="width: 830px;" width="694">
<tbody>
<tr>
<td style="width: 200px; text-align: left;">
<p><strong>T&ecirc;n c&ocirc;ng ty</strong></p>
</td>
<td style="width: 614px; text-align: left;">
<p>C&ocirc;ng ty TNHH Chia Sẻ Tr&iacute; Tuệ (Chie Se)</p>
</td>
</tr>
<tr>
<td style="text-align: left; width: 200px;"><strong>Địa chỉ</strong></td>
<td style="width: 614px; text-align: left;">
<p>Tầng 19, t&ograve;a nh&agrave; ICON4, số 243A Đ&ecirc; La Th&agrave;nh, phường L&aacute;ng Thượng, quận Đống Đa, th&agrave;nh phố H&agrave; Nội, Việt Nam&nbsp;</p>
</td>
</tr>
<tr>
<td style="width: 200px; text-align: left;"><strong>Ng&agrave;y th&agrave;nh lập</strong></td>
<td style="width: 614px; text-align: left;">
<p>23/03/2016</p>
</td>
</tr>
<tr>
<td style="width: 200px; text-align: left;"><strong>Lĩnh vực hoạt động ch&iacute;nh </strong><br /><strong>/Dịch vụ cung cấp ch&iacute;nh</strong></td>
<td style="width: 614px; text-align: left;">&nbsp;- Thực hiện v&agrave; sản xuất phần mềm đ&oacute;ng g&oacute;i, phần mềm theo y&ecirc;u cầu của kh&aacute;ch h&agrave;ng<br />&nbsp;- Thực hiện c&aacute;c dịch vụ bảo tr&igrave; hệ thống <br />&nbsp;- Tư vấn, hỗ trợ mua v&agrave; c&agrave;i đặt thiết bị phần cứng<br />&nbsp;- Bi&ecirc;n dịch ng&ocirc;n ngữ phần mềm (tiếng Việt, tiếng Nhật) <br />&nbsp;</td>
</tr>
<tr>
<td style="width: 200px; text-align: left;"><strong>Người đại diện</strong></td>
<td style="width: 614px; text-align: left;">
<p>&nbsp;- Gi&aacute;m đốc Yamasaki Hirofumi&nbsp;</p>
</td>
</tr>
<tr>
<td style="width: 200px; text-align: left;"><strong>Quy m&ocirc; nh&acirc;n sự</strong></td>
<td style="width: 614px; text-align: left;">
<p>&nbsp;- 6 người (t&iacute;nh đến 07/2019)</p>
</td>
</tr>
<tr>
<td style="width: 200px; text-align: left;"><strong>Kh&aacute;ch h&agrave;ng ti&ecirc;u biểu</strong></td>
<td style="width: 614px; text-align: left;">
<p>&nbsp;- C&ocirc;ng ty Cổ phần Gainsharing<br />&nbsp;- C&ocirc;ng ty TNHH Shihen Việt Nam<br />&nbsp;- C&ocirc;ng ty TNHH Suzumoto Việt Nam <br />&nbsp;- C&ocirc;ng ty TNHH KDDI Việt Nam<br />&nbsp;- C&ocirc;ng ty TNHH RHYTHM PRECISION Việt Nam</p>
</td>
</tr>
<tr>
<td style="width: 200px; text-align: left;"><strong>Ng&acirc;n h&agrave;ng giao dịch</strong></td>
<td style="width: 614px; text-align: left;">
<p>&nbsp;- Ng&acirc;n h&agrave;ng BIDV, Ng&acirc;n h&agrave;ng Mizuho</p>
</td>
</tr>
<tr>
<td style="width: 200px; text-align: left;"><strong>TEL&nbsp;</strong></td>
<td style="width: 614px; text-align: left;">
<p>&nbsp; (084) 43 200 2128</p>
</td>
</tr>
</tbody>
</table>', N'ho-so-chie-se', N'vi-VN', 1)
INSERT [dbo].[tbl_post_lang] ([Id], [PostId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (30, 17, N'企業情報', NULL, N'<p style="background-color: transparent; color: #000000; cursor: text; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; outline-color: transparent; outline-style: none; outline-width: 0px; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"><span style="color: #3366ff; font-size: 18.66px;"><em><strong>社長挨拶</strong></em></span></p>
<table style="width: 1007px; height: 10px; text-align: left; color: #000000; text-transform: none; text-indent: 0px; letter-spacing: normal; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; text-decoration: none; word-spacing: 0px; white-space: normal; orphans: 2; -webkit-text-stroke-width: 0px; background-color: transparent;" width="862">
<tbody style="cursor: text; outline-color: transparent; outline-style: none; outline-width: 0px;">
<tr style="height: 240px;">
<td style="width: 705px; height: 10px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">
<p>始めまして。<br />CHIE SEの代表 山崎博文です。<br />数あるサイトの中から当社ホームページにお越しいただきありがとうございます。<br />当社は2015年からベトナム進出を検討し、2016年にハノイにシステム開発会社<br />「CHIE SE」を設立しました。<br />日本とベトナムで「知恵」を「共有する」という意味で、日本語の「知恵」の「CHIE」<br />とベトナム語の「Chia sẻ」の「SE」を組み合せた造語を会社名としました。<br />親会社は日本の大阪で2003年から創業している「GAINSHARING」というシステム会社です。<br />日本では様々な会社にシステム開発を提供しています。製造業を中心とした会社（工場）には<br />各種制御システムや監視システム、そして生産管理などを納入しています。<br />また運送業や放送局には業務系のシステムを納入し、提案から開発そして保守に至るまで<br />幅広いサービスを活用いただいています。<br />ベトナムにおいても日本同様に日系の製造業のお客様に対して日本で培ったノウハウや資産<br />を適切な価格と納期で提供出来る様に体制を整えています。<br />まだまだ発展途上の会社ではありますが、ベトナムの成長とともに当社も成長し、<br />貢献出来る様に努めていきますので、ご指導ご鞭撻のほど、よろしくお願いいたします。</p>
</td>
<td style="width: 286px; height: 10px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;"><img style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;" src="../../../Media/Uploads/president.jpg" alt="" width="270" height="388" /></td>
</tr>
</tbody>
</table>
<p style="background-color: transparent; color: #000000; cursor: text; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; outline-color: transparent; outline-style: none; outline-width: 0px; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"> </p>
<p style="background-color: transparent; color: #000000; cursor: text; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; outline-color: transparent; outline-style: none; outline-width: 0px; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"><span style="color: #3366ff; font-size: 18.66px;"><strong>企業概要</strong></span></p>
<table style="width: 1689px; height: 221px; text-align: left; color: #000000; text-transform: none; text-indent: 0px; letter-spacing: normal; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; text-decoration: none; word-spacing: 0px; white-space: normal; orphans: 2; -webkit-text-stroke-width: 0px; background-color: transparent;" width="998">
<tbody style="cursor: text; outline-color: transparent; outline-style: none; outline-width: 0px;">
<tr style="height: 25px;">
<td style="width: 129.16px; height: 18px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">社名：</td>
<td style="width: 499.6px; height: 18px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">CHIE SE CO.,LTD</td>
<td style="width: 63.08px; height: 221px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;" rowspan="8"> </td>
<td style="width: 969.16px; height: 221px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;" rowspan="8">                         <img style="float: left;" src="../../../Media/Uploads/file.348163.jpg" alt="file.348163" width="388" height="551" /></td>
</tr>
<tr style="height: 74px;">
<td style="width: 129.16px; height: 18px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">所在地：</td>
<td style="width: 499.6px; height: 18px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">
<p>19th Floor, ICON4 Tower, 243A De La Thanh Str,<br />Lang Thuong Ward, Dong Da Dist, Ha Noi, Viet Nam</p>
</td>
</tr>
<tr style="height: 26px;">
<td style="width: 129.16px; height: 18px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">設立日：</td>
<td style="width: 499.6px; height: 18px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">2016年03月23日</td>
</tr>
<tr style="height: 196px;">
<td style="width: 129.16px; height: 72px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">
<p>事業概要：</p>
</td>
<td style="width: 499.6px; height: 72px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">
<p>CPC842：ソフトウェアの製造・開発<br />CPC841：ハード機器設置のコンサルティング<br />CPC843：データ処理<br />CPC865： 管理コンサルティング</p>
</td>
</tr>
<tr style="height: 25px;">
<td style="width: 129.16px; height: 18px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">代表者：</td>
<td style="width: 499.6px; height: 18px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">社長　山崎博文</td>
</tr>
<tr style="height: 26px;">
<td style="width: 129.16px; height: 18px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">従業員数：</td>
<td style="width: 499.6px; height: 18px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">6人（2019年07月まで)</td>
</tr>
<tr style="height: 50px;">
<td style="width: 129.16px; height: 36px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">取引銀行：</td>
<td style="width: 499.6px; height: 36px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">BIDV, MIZUHO</td>
</tr>
<tr style="height: 26px;">
<td style="width: 129.16px; height: 23px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">電話番号：</td>
<td style="width: 499.6px; height: 23px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">(084) 243 200 2128</td>
</tr>
</tbody>
</table>
<p style="background-color: transparent; color: #000000; cursor: text; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; outline-color: transparent; outline-style: none; outline-width: 0px; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"> </p>
<p style="background-color: transparent; color: #000000; cursor: text; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; outline-color: transparent; outline-style: none; outline-width: 0px; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"><span style="color: #3366ff; font-size: 18.66px;"><strong>CHIE SEの特徴</strong></span></p>
<table style="background-color: transparent; color: #000000; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; height: 922.62px; letter-spacing: normal; orphans: 2; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; width: 1645px; word-spacing: 0px;" width="870">
<tbody style="cursor: text; outline-color: transparent; outline-style: none; outline-width: 0px;">
<tr style="height: 102px;">
<td style="width: 634.4px; height: 96.6px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">
<p style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">・独立系でワンステップ<br />メーカーや親会社に依存しないポジションでお客様にとって最適なシステムを提案します。<br />業務コンサルティング、提案業務から設計、開発及び納入、保守に至るまで<br />全てを自社内スタッフで対応させていただきます。</p>
<p style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;"> </p>
</td>
<td style="width: 69.04px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;"> </td>
<td style="width: 919.56px; height: 259.54px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;"><img style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;" src="../../../Media/Uploads/4.jpg" alt="" width="388" height="242" /></td>
</tr>
<tr style="height: 29px;">
<td style="width: 634.4px; height: 18.2px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;"> </td>
<td style="width: 69.04px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;"> </td>
<td style="width: 919.56px; height: 18.2px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;"> </td>
</tr>
<tr style="height: 162px;">
<td style="width: 634.4px; height: 109.2px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">
<p>・弊社の強みは中小規模のシステム開発が短期間かつ、適正価格で提供することができます。<br />高価で高機能なエンタープライズシステムを提供するのではなく、カスタマイズが必要であったり、エンタープライズまでも必要としない企業様に当社の立ち位置を重宝して頂いております。<br />また、ベトナムローカルでのサブシステムの構築には当社の資産を活用していただける<br />と考えています。</p>
</td>
<td style="width: 69.04px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;"> </td>
<td style="width: 919.56px; height: 259.54px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;"><img style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;" src="../../../Media/Uploads/3.jpg" alt="" width="389" height="249" /></td>
</tr>
<tr style="height: 37px;">
<td style="width: 634.4px; height: 18.2px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;"> </td>
<td style="width: 69.04px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;"> </td>
<td style="width: 919.56px; height: 18.2px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;"> </td>
</tr>
<tr style="height: 54px;">
<td style="width: 634.4px; height: 54.6px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">
<p>・弊社は最適な選択支を選び、高品質な製品を作る為に、社内で創造的なアイデア<br />や建設的な討論を奨励しています。</p>
</td>
<td style="width: 69.04px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;"> </td>
<td style="width: 919.56px; height: 315.54px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;"><img style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;" src="../../../Media/Uploads/5.jpg" alt="" width="390" height="312" /></td>
</tr>
</tbody>
</table>
<p style="background-color: transparent; color: #000000; cursor: text; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; outline-color: transparent; outline-style: none; outline-width: 0px; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"><img src="../../../Media/Uploads/Slide6_1.JPG" alt="Slide6_1" width="1110" height="833" /></p>', N'invalid', N'vi-VN', 1)
INSERT [dbo].[tbl_post_lang] ([Id], [PostId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (31, 18, N'BẢN SẮC CHIE SE', NULL, N'<table style="width: 784px;" width="810">
<tbody>
<tr>
<td style="width: 142px;">Đối với kh&aacute;ch h&agrave;ng&nbsp;</td>
<td style="width: 626px;">
<p>&nbsp;- Ch&uacute;ng t&ocirc;i lu&ocirc;n đặt m&igrave;nh v&agrave;o vị tr&iacute; của kh&aacute;ch h&agrave;ng để lắng nghe, thấu hiểu những y&ecirc;u cầu, nguyện vọng của kh&aacute;ch h&agrave;ng một c&aacute;ch tốt nhất c&oacute; thể. <br />&nbsp;- Đồng h&agrave;nh để giải quyết mọi vấn đề của kh&aacute;ch h&agrave;ng <br />&nbsp;- Lu&ocirc;n ch&uacute; trọng tới mục ti&ecirc;u tối ưu h&oacute;a Chi ph&iacute; - chất lượng - b&agrave;n giao sản phẩm cho kh&aacute;ch h&agrave;ng</p>
</td>
</tr>
<tr>
<td style="width: 142px;">Gia đ&igrave;nh Chie Se&nbsp;</td>
<td style="width: 626px;">
<p>&nbsp;- C&aacute;c th&agrave;nh vi&ecirc;n của ch&uacute;ng t&ocirc;i kh&ocirc;ng chỉ l&agrave;m tốt c&ocirc;ng vệc của m&igrave;nh m&agrave; c&ograve;n Đo&agrave;n kết, c&oacute; tinh thần chia sẻ, v&agrave; gi&uacute;p đỡ nhau c&ugrave;ng tiến bộ.<br />&nbsp;- Đặc biệt Chie Se lu&ocirc;n ch&uacute; trọng s&aacute;ng tạo, đổi mới kh&ocirc;ng ngừng để đem lại c&aacute;c gi&aacute; trị mới v&agrave; hữu &iacute;ch hơn cho kh&aacute;ch h&agrave;ng.</p>
</td>
</tr>
<tr>
<td style="width: 142px;">Với x&atilde; hội</td>
<td style="width: 626px;">&nbsp;- T&ocirc;n trọng v&agrave; thực hiện đ&uacute;ng ph&aacute;p luật<br />&nbsp;- Ph&aacute;t triển bền vững, s&aacute;ng tạo v&agrave; v&igrave; quyền lợi quốc gia.<br />&nbsp;- Kết hợp văn h&oacute;a kinh doanh của 2 quốc gia Việt Nam v&agrave; Nhật Bản</td>
</tr>
</tbody>
</table>
<p>&nbsp;</p>
<h1><a title="Lịch sử c&ocirc;ng ty" href="../../../article/detail/17/lich-su-chie-se">Lịch sử c&ocirc;ng ty</a></h1>
<p>&nbsp;</p>
<p><a title="Lịch sử c&ocirc;ng ty" href="../../../article/detail/6/chung-toi-la-cong-ty-phan-mem"><img src="../../../Media/Uploads/chiese_logo.png" alt="" width="500" height="637" /></a></p>', N'ban-sac-chie-se', N'vi-VN', 1)
INSERT [dbo].[tbl_post_lang] ([Id], [PostId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (32, 15, N'社長挨拶', NULL, N'<p>始めまして。CHIE SEの代表 山崎博文です。<br />数あるサイトの中から当社ホームページにお越しいただきありがとうございます。<br />当社は2015年からベトナム進出を検討し、2016年にハノイにシステム開発会社「CHIE SE」を設立しました。<br />日本とベトナムで「知恵」を「共有する」という意味で、日本語の「知恵」の「CHIE」<br />とベトナム語の「Chia sẻ」の「SE」を組み合せた造語を会社名としました。<br />親会社は日本の大阪で2003年から創業している「GAINSHARING」というシステム会社です。<br />日本では様々な会社にシステム開発を提供しています。<br />製造業を中心とした会社（工場）には各種制御システムや監視システム、そして生産管理などを納入しています。<br />また運送業や放送局には業務系のシステムを納入し、提案から開発そして保守に至るまで幅広いサービスを活用頂いています。<br />ベトナムにおいても日本同様に日系の製造業のお客様に対して日本で培ったノウハウや資産<br />を適切な価格と納期で提供出来る様に体制を整えています。<br />まだまだ発展途上の会社ではありますが、ベトナムの成長とともに当社も成長し、貢献出来る様に努めていきますので、<br />ご指導ご鞭撻のほど、よろしくお願い致します。</p>', N'invalid', N'ja-JP', 1)
INSERT [dbo].[tbl_post_lang] ([Id], [PostId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (33, 10, N'VỀ CHÚNG TÔI', NULL, N'<p style="background-color: transparent; color: #000000; cursor: text; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; outline-color: transparent; outline-style: none; outline-width: 0px; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"><span style="color: #3366ff; font-size: 18.66px;"><em><strong>LỜI NGỎ</strong></em></span></p>
<table style="background-color: transparent; color: #000000; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; height: 434.4px; letter-spacing: normal; orphans: 2; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; width: 1007px; word-spacing: 0px;" width="862">
<tbody style="cursor: text; outline-color: transparent; outline-style: none; outline-width: 0px;">
<tr style="height: 240px;">
<td style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; height: 428.4px; width: 709.26px;">
<p style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;"><em>Xin chào các bạn,</em><br /><em>Tôi là Yamasaki Hirofumi - Giám đốc công ty phần mềm CHIE SE.</em><br /><em>Cảm ơn các bạn đã ưu ái ghé thăm trang web của chúng tôi trong số rất nhiều trang web khác nhau.</em><br /><em>Năm 2015 chúng tôi đã cân nhắc mở rộng sang thị trường Việt Nam và năm 2016 chính thức thành lập công ty phần mềm mang tên "CHIE SE" tại Hà Nội. </em><br /><em>Tên công ty là từ ghép của từ "CHIE" (nghĩa là "Trí tuệ") trong tiếng Nhật và "SE" (là từ "Sẻ" trong cụm từ "Chia sẻ") trong tiếng Việt; mang thông điệp Việt Nam và Nhật Bản cùng chia sẻ kiến thức, kinh nghiệm và sáng kiến.</em><br /><em>Công ty mẹ của CHIE SE cũng là công ty phần mềm mang tên "GAINSHARING" được thành lập vào năm 2003 tại Osaka, Nhật Bản. </em><br /><em>Ở Nhật Bản, chúng tôi cung cấp phần mềm cho rất nhiều khách hàng doanh nghiệp khác nhau. </em><br /><em>Với đối tượng khách hàng là các doanh nghiệp (nhà máy) sản xuất, chúng tôi cung cấp các loại phần mềm điều khiển, giám sát, quản lý sản xuất,.... Chúng tôi còn cung cấp các phần mềm nghiệp vụ cho khách hàng là các doanh nghiệp vận tải, đài phát thanh,... Khách hàng đang sử dụng đa dạng các dịch vụ của chúng tôi trong suốt quá trình từ tư vấn đến sản xuất và bảo trì phần mềm. </em><br /><em>Chúng tôi đang hoàn thiện thể chế để phía Việt Nam cũng giống như phía Nhật Bản, có thể cung cấp cho các doanh nghiệp sản xuất Nhật Bản phần mềm với chi phí và thời gian hợp lý dựa trên những kiến thức, kinh nghiệm và tài sản đã được tích lũy bên Nhật. Công ty chúng tôi tuy vẫn còn trong giai đoạn đang phát triển nhưng sẽ luôn cố gắng để hoàn thiện; phát triển cùng và đóng góp vào sự phát triển của Việt Nam. </em><br /><em>Rất mong nhận được sự chỉ dẫn và ủng hộ từ các bạn.</em></p>
</td>
<td style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; height: 391.54px; width: 287.74px;"><img style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;" src="../../../Media/Uploads/president.jpg" alt="" width="270" height="388" /></td>
</tr>
</tbody>
</table>
<p style="background-color: transparent; color: #000000; cursor: text; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; outline-color: transparent; outline-style: none; outline-width: 0px; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"><span style="color: #0000ff; font-family: arial,helvetica,sans-serif; text-decoration: underline;"><strong><span style="font-size: 18.66px;">TRIẾT LÝ DOANH NGHIỆP</span></strong></span></p>
<p style="background-color: transparent; color: #000000; cursor: text; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; outline-color: transparent; outline-style: none; outline-width: 0px; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"><img src="../../../Media/Uploads/Presentation1_1.pptx" alt="" /><img src="../../../Media/Uploads/Presentation1.pptx" alt="" /><img src="../../../Media/Uploads/Presentation1.png" alt="" width="1005" height="565" /></p>
<p style="background-color: transparent; color: #000000; cursor: text; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; outline-color: transparent; outline-style: none; outline-width: 0px; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"><span style="color: #3366ff; font-size: 18.66px;"><strong>HỒ SƠ CHIE SE</strong></span></p>
<table style="width: 1689px; height: 240px; text-align: left; color: #000000; text-transform: none; text-indent: 0px; letter-spacing: normal; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; text-decoration: none; word-spacing: 0px; white-space: normal; orphans: 2; -webkit-text-stroke-width: 0px; background-color: transparent;" width="998">
<tbody style="cursor: text; outline-color: transparent; outline-style: none; outline-width: 0px;">
<tr style="height: 25px;">
<td style="width: 129.11px; height: 18px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">Tên công ty:</td>
<td style="width: 498.62px; height: 18px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">CÔNG TY TNHH CHIA SẺ TRÍ TUỆ (CHIE SE)</td>
<td style="width: 65.87px; height: 240px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;" rowspan="8"> </td>
<td style="width: 967.4px; height: 240px; text-align: left; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;" rowspan="8"><img src="../../../Media/Uploads/IMG_7571.jpg" alt="" width="384" height="512" /></td>
</tr>
<tr style="height: 74px;">
<td style="width: 129.11px; height: 18px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">Địa chỉ:</td>
<td style="width: 498.62px; height: 18px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">
<p style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">Tầng 19, Tòa nhà ICON4, Số 243A Đê La Thành,<br />Phường Láng Thượng, Quận Đống Đa, Thành phố Hà Nội, Việt Nam</p>
</td>
</tr>
<tr style="height: 26px;">
<td style="width: 129.11px; height: 18px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">Ngày thành lập:</td>
<td style="width: 498.62px; height: 18px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">23/03/2016</td>
</tr>
<tr style="height: 196px;">
<td style="width: 129.11px; height: 96px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">
<p style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">Lĩnh vực hoạt động chính:</p>
<p style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;"> </p>
</td>
<td style="width: 498.62px; height: 96px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">
<p style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">CPC842 : Dịch vụ thực hiện và sản xuất phần mềm<br />CPC841 : Dịch vụ tư vấn liên quan đến lắp đặt phần cứng máy tính<br />CPC843 : Dịch vụ xử lý dữ liệu trong lĩnh vực công nghệ thông tin<br />CPC865 : Dịch vụ tư vấn quản lý</p>
</td>
</tr>
<tr style="height: 25px;">
<td style="width: 129.11px; height: 18px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;"><span style="background-color: transparent; color: #000000; cursor: text; display: inline; float: none; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;">Người đại diện:</span></td>
<td style="width: 498.62px; height: 18px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">Giám đốc Yamasaki Hirofumi</td>
</tr>
<tr style="height: 26px;">
<td style="width: 129.11px; height: 18px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">Quy mô nhân sự:</td>
<td style="width: 498.62px; height: 18px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">6 người (Tính đến 07/2019)</td>
</tr>
<tr style="height: 50px;">
<td style="width: 129.11px; height: 36px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">Ngân hàng giao dịch:</td>
<td style="width: 498.62px; height: 36px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">BIDV, MIZUHO</td>
</tr>
<tr style="height: 26px;">
<td style="width: 129.11px; height: 18px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">Số điện thoại:</td>
<td style="width: 498.62px; height: 18px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">(084) 243 200 2128</td>
</tr>
</tbody>
</table>
<p style="background-color: transparent; color: #000000; cursor: text; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; outline-color: transparent; outline-style: none; outline-width: 0px; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"> </p>
<p style="background-color: transparent; color: #000000; cursor: text; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; outline-color: transparent; outline-style: none; outline-width: 0px; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"><strong><span style="font-size: 24px;"><span style="color: #0b0130;">BẢN SẮC CHIE SE</span></span></strong></p>
<table style="background-color: transparent; color: #000000; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; height: 922.6px; letter-spacing: normal; orphans: 2; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; width: 1645px; word-spacing: 0px;" width="870">
<tbody style="cursor: text; outline-color: transparent; outline-style: none; outline-width: 0px;">
<tr style="height: 102px;">
<td style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; height: 114.8px; width: 637.52px;">
<p style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">・Chúng tôi độc lập và phụ trách dự án trực tiếp từ đầu đến cuối:<br />Chúng tôi có khả năng độc lập đưa ra các phương án phần mềm phù hợp nhất với khách hàng không phụ thuộc vào nhà sản xuất hay công ty mẹ.</p>
<p style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;"> </p>
</td>
<td style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; width: 64.36px;"> </td>
<td style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; height: 245.54px; width: 929.12px;"><img style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;" src="../../../Media/Uploads/4.jpg" alt="" width="388" height="242" /></td>
</tr>
<tr style="height: 29px;">
<td style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; height: 18.2px; width: 637.52px;"> </td>
<td style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; width: 64.36px;"> </td>
<td style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; height: 18.2px; width: 929.12px;"> </td>
</tr>
<tr style="height: 162px;">
<td style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; height: 137.2px; width: 637.52px;">
<p style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">・Thế mạnh của chúng tôi là cung cấp các phần mềm quy mô vừa và nhỏ trong thời gian ngắn với chi phí hợp lý. Chúng tôi không cung cấp các phần mềm đa chức năng chi phí lớn hướng đến nhiều đối tượng doanh nghiệp mà được đánh giá cao bởi các doanh nghiệp muốn tùy chỉnh các phần mềm sẵn có theo nhu cầu thực tế hoặc chỉ cần mua các phần mềm tối giản thiết kế theo đúng yêu cầu. Ngoài ra chúng tôi cũng có thể thiết kế các phần mềm phụ phù hợp để khách hàng có thể sử dụng tại Việt Nam.</p>
</td>
<td style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; width: 64.36px;"> </td>
<td style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; height: 252.54px; width: 929.12px;"><img style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;" src="../../../Media/Uploads/3.jpg" alt="" width="389" height="249" /></td>
</tr>
<tr style="height: 37px;">
<td style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; height: 18.2px; width: 637.52px;"> </td>
<td style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; width: 64.36px;"> </td>
<td style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; height: 18.2px; width: 929.12px;"> </td>
</tr>
<tr style="height: 54px;">
<td style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; height: 82.6px; width: 637.52px;">
<p style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">・Chúng tôi luôn khuyến khích các ý tưởng sáng tạo, tranh luận mang tính chất xây dựng trong công ty để đưa ra được những lựa chọn tối ưu và tạo ra những sản phẩm có chất lượng cao.</p>
</td>
<td style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; width: 64.36px;"> </td>
<td style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; height: 315.54px; width: 929.12px;"><img style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;" src="../../../Media/Uploads/5.jpg" alt="" width="390" height="312" /></td>
</tr>
</tbody>
</table>
<p style="background-color: transparent; color: #000000; cursor: text; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; outline-color: transparent; outline-style: none; outline-width: 0px; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"><img src="../../../Media/Uploads/Slide6.JPG" alt="" width="1104" height="828" /></p>', N'http://localhost:3130/article/detail/10/ve-chung-toi', N'vi-VN', 1)
INSERT [dbo].[tbl_post_lang] ([Id], [PostId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (34, 17, N'History', NULL, N'<p>Scalable on Devices.</p>
<div class="section nobottommargin">
<div class="container clear-bottommargin clearfix">
<div class="row topmargin-sm clearfix">
<div class="col-lg-4 bottommargin"><i class="i-plain color i-large icon-line2-screen-desktop inline-block"></i>
<div class="heading-block nobottomborder" style="margin-bottom: 15px;">
<h4>Responsive &amp; Retina</h4>
</div>
<p>Employment respond committed meaningful fight against oppression social challenges rural legal aid governance. Meaningful work, implementation, process cooperation, campaign inspire.</p>
</div>
<div class="col-lg-4 bottommargin"><i class="i-plain color i-large icon-line2-energy inline-block"></i>
<div class="heading-block nobottomborder" style="margin-bottom: 15px;"><span class="before-heading">Smartly Coded &amp; Maintained.</span>
<h4>Powerful Performance</h4>
</div>
<p>Medecins du Monde Jane Addams reduce child mortality challenges Ford Foundation. Diversification shifting landscape advocate pathway to a better life rights international. Assessment.</p>
</div>
<div class="col-lg-4 bottommargin"><i class="i-plain color i-large icon-line2-equalizer inline-block"></i>
<div class="heading-block nobottomborder" style="margin-bottom: 15px;"><span class="before-heading">Flexible &amp; Customizable.</span>
<h4>Truly Multi-Purpose</h4>
</div>
<p>Democracy inspire breakthroughs, Rosa Parks; inspiration raise awareness natural resources. Governance impact; transformative donation philanthropy, respect reproductive.</p>
<p> </p>
</div>
</div>
</div>
</div>
<div class="container clearfix">
<div class="heading-block topmargin-lg center">
<h2>Even more Feature Rich</h2>
<span class="divcenter">Philanthropy convener livelihoods, initiative end hunger gender rights local. John Lennon storytelling; advocate, altruism impact catalyst.</span></div>
<div class="row bottommargin-sm">
<div class="col-lg-4 col-md-6 bottommargin">
<div class="feature-box fbox-right topmargin fadeIn animated" data-animate="fadeIn">
<div class="fbox-icon"> </div>
<h3>Boxed &amp; Wide Layouts</h3>
<p>Stretch your Website to the Full Width or make it boxed to surprise your visitors.</p>
</div>
<div class="feature-box fbox-right topmargin fadeIn animated" data-animate="fadeIn" data-delay="200">
<div class="fbox-icon"> </div>
<h3>Extensive Documentation</h3>
<p>We have covered each &amp; everything in our Docs including Videos &amp; Screenshots.</p>
</div>
<div class="feature-box fbox-right topmargin fadeIn animated" data-animate="fadeIn" data-delay="400">
<div class="fbox-icon"> </div>
<h3>Parallax Support</h3>
<p>Display your Content attractively using Parallax Sections with HTML5 Videos.</p>
</div>
</div>
<div class="col-lg-4 d-md-none d-lg-block bottommargin center"><img src="../../../themes/default/images/services/iphone7.png" alt="iphone 2" /></div>
<div class="col-lg-4 col-md-6 bottommargin">
<div class="feature-box topmargin fadeIn animated" data-animate="fadeIn">
<div class="fbox-icon"> </div>
<h3>HTML5 Video</h3>
<p>Canvas provides support for Native HTML5 Videos that can be added to a Background.</p>
</div>
<div class="feature-box topmargin fadeIn animated" data-animate="fadeIn" data-delay="200">
<div class="fbox-icon"> </div>
<h3>Endless Possibilities</h3>
<p>Complete control on each &amp; every element that provides endless customization.</p>
</div>
<div class="feature-box topmargin fadeIn animated" data-animate="fadeIn" data-delay="400">
<div class="fbox-icon"> </div>
<h3>Light &amp; Dark Color Schemes</h3>
<p>Change your Website''s Primary Scheme instantly by simply adding the dark class.</p>
</div>
</div>
</div>
</div>
<div class="row nopadding align-items-stretch">
<div class="col-lg-4 dark col-padding ohidden" style="background-color: #1abc9c;">
<div>
<h3 class="uppercase" style="font-weight: 600;">Why choose Us</h3>
<p style="line-height: 1.8;">Transform, agency working families thinkers who make change happen communities. Developing nations legal aid public sector our ambitions future aid The Elders economic security Rosa.</p>
<a class="button button-border button-light button-rounded uppercase nomargin" href="#">Read More</a> <i class="icon-bulb bgicon"></i></div>
</div>
<div class="col-lg-4 dark col-padding ohidden" style="background-color: #34495e;">
<div>
<h3 class="uppercase" style="font-weight: 600;">Our Mission</h3>
<p style="line-height: 1.8;">Frontline respond, visionary collaborative cities advancement overcome injustice, UNHCR public-private partnerships cause. Giving, country educate rights-based approach; leverage disrupt solution.</p>
<a class="button button-border button-light button-rounded uppercase nomargin" href="#">Read More</a> <i class="icon-cog bgicon"></i></div>
</div>
<div class="col-lg-4 dark col-padding ohidden" style="background-color: #e74c3c;">
<div>
<h3 class="uppercase" style="font-weight: 600;">What you get</h3>
<p style="line-height: 1.8;">Sustainability involvement fundraising campaign connect carbon rights, collaborative cities convener truth. Synthesize change lives treatment fluctuation participatory monitoring underprivileged equal.</p>
<a class="button button-border button-light button-rounded uppercase nomargin" href="#">Read More</a> <i class="icon-thumbs-up bgicon"></i></div>
</div>
</div>', N'http://localhost:3130/article/detail/17/history', N'en-US', 1)
INSERT [dbo].[tbl_post_lang] ([Id], [PostId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (35, 17, N'企業情報', NULL, N'<p style="background-color: transparent; color: #000000; cursor: text; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; outline-color: transparent; outline-style: none; outline-width: 0px; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"><span style="color: #3366ff; font-size: 18.66px;"><em><strong>社長挨拶</strong></em></span></p>
<table style="width: 1007px; height: 10px; text-align: left; color: #000000; text-transform: none; text-indent: 0px; letter-spacing: normal; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; text-decoration: none; word-spacing: 0px; white-space: normal; orphans: 2; -webkit-text-stroke-width: 0px; background-color: transparent;" width="862">
<tbody style="cursor: text; outline-color: transparent; outline-style: none; outline-width: 0px;">
<tr style="height: 240px;">
<td style="width: 705px; height: 10px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">
<p>始めまして。<br />CHIE SEの代表 山崎博文です。<br />数あるサイトの中から当社ホームページにお越しいただきありがとうございます。<br />当社は2015年からベトナム進出を検討し、2016年にハノイにシステム開発会社<br />「CHIE SE」を設立しました。<br />日本とベトナムで「知恵」を「共有する」という意味で、日本語の「知恵」の「CHIE」<br />とベトナム語の「Chia sẻ」の「SE」を組み合せた造語を会社名としました。<br />親会社は日本の大阪で2003年から創業している「GAINSHARING」というシステム会社です。<br />日本では様々な会社にシステム開発を提供しています。製造業を中心とした会社（工場）には<br />各種制御システムや監視システム、そして生産管理などを納入しています。<br />また運送業や放送局には業務系のシステムを納入し、提案から開発そして保守に至るまで<br />幅広いサービスを活用いただいています。<br />ベトナムにおいても日本同様に日系の製造業のお客様に対して日本で培ったノウハウや資産<br />を適切な価格と納期で提供出来る様に体制を整えています。<br />まだまだ発展途上の会社ではありますが、ベトナムの成長とともに当社も成長し、<br />貢献出来る様に努めていきますので、ご指導ご鞭撻のほど、よろしくお願いいたします。</p>
</td>
<td style="width: 286px; height: 10px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;"><img style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;" src="../../../Media/Uploads/president.jpg" alt="" width="270" height="388" /></td>
</tr>
</tbody>
</table>
<p style="background-color: transparent; color: #000000; cursor: text; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; outline-color: transparent; outline-style: none; outline-width: 0px; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"> </p>
<p style="background-color: transparent; color: #000000; cursor: text; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; outline-color: transparent; outline-style: none; outline-width: 0px; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"><span style="color: #3366ff; font-size: 18.66px;"><strong>企業概要</strong></span></p>
<table style="width: 1689px; height: 221px; text-align: left; color: #000000; text-transform: none; text-indent: 0px; letter-spacing: normal; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; text-decoration: none; word-spacing: 0px; white-space: normal; orphans: 2; -webkit-text-stroke-width: 0px; background-color: transparent;" width="998">
<tbody style="cursor: text; outline-color: transparent; outline-style: none; outline-width: 0px;">
<tr style="height: 25px;">
<td style="width: 129.08px; height: 18px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">社名：</td>
<td style="width: 499.3px; height: 18px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">CHIE SE CO.,LTD</td>
<td style="width: 66.04px; height: 221px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;" rowspan="8"> </td>
<td style="width: 966.58px; height: 221px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;" rowspan="8">                         <img style="float: left;" src="../../../Media/Uploads/file.348163.jpg" alt="file.348163" width="387" height="551" /></td>
</tr>
<tr style="height: 74px;">
<td style="width: 129.08px; height: 18px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">所在地：</td>
<td style="width: 499.3px; height: 18px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">
<p>19th Floor, ICON4 Tower, 243A De La Thanh Str,<br />Lang Thuong Ward, Dong Da Dist, Ha Noi, Viet Nam</p>
</td>
</tr>
<tr style="height: 26px;">
<td style="width: 129.08px; height: 18px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">設立日：</td>
<td style="width: 499.3px; height: 18px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">2016年03月23日</td>
</tr>
<tr style="height: 196px;">
<td style="width: 129.08px; height: 72px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">
<p>事業概要：</p>
</td>
<td style="width: 499.3px; height: 72px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">
<p>CPC842：ソフトウェアの製造・開発<br />CPC841：ハード機器設置のコンサルティング<br />CPC843：データ処理<br />CPC865： 管理コンサルティング</p>
</td>
</tr>
<tr style="height: 25px;">
<td style="width: 129.08px; height: 18px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">代表者：</td>
<td style="width: 499.3px; height: 18px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">社長　山崎博文</td>
</tr>
<tr style="height: 26px;">
<td style="width: 129.08px; height: 18px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">従業員数：</td>
<td style="width: 499.3px; height: 18px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">6人（2019年07月まで)</td>
</tr>
<tr style="height: 50px;">
<td style="width: 129.08px; height: 36px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">取引銀行：</td>
<td style="width: 499.3px; height: 36px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">BIDV, MIZUHO</td>
</tr>
<tr style="height: 26px;">
<td style="width: 129.08px; height: 23px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">電話番号：</td>
<td style="width: 499.3px; height: 23px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">(084) 243 200 2128</td>
</tr>
</tbody>
</table>
<p style="background-color: transparent; color: #000000; cursor: text; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; outline-color: transparent; outline-style: none; outline-width: 0px; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"> </p>
<p style="background-color: transparent; color: #000000; cursor: text; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; outline-color: transparent; outline-style: none; outline-width: 0px; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"><span style="color: #3366ff; font-size: 18.66px;"><strong>CHIE SEの特徴</strong></span></p>
<table style="background-color: transparent; color: #000000; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; height: 922.62px; letter-spacing: normal; orphans: 2; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; width: 1645px; word-spacing: 0px;" width="870">
<tbody style="cursor: text; outline-color: transparent; outline-style: none; outline-width: 0px;">
<tr style="height: 102px;">
<td style="width: 634.39px; height: 96.6px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">
<p style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">・独立系でワンステップ<br />メーカーや親会社に依存しないポジションでお客様にとって最適なシステムを提案します。<br />業務コンサルティング、提案業務から設計、開発及び納入、保守に至るまで<br />全てを自社内スタッフで対応させていただきます。</p>
<p style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;"> </p>
</td>
<td style="width: 64.04px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;"> </td>
<td style="width: 924.57px; height: 259.54px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;"><img style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;" src="../../../Media/Uploads/4.jpg" alt="" width="388" height="242" /></td>
</tr>
<tr style="height: 29px;">
<td style="width: 634.39px; height: 18.2px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;"> </td>
<td style="width: 64.04px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;"> </td>
<td style="width: 924.57px; height: 18.2px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;"> </td>
</tr>
<tr style="height: 162px;">
<td style="width: 634.39px; height: 109.2px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">
<p>・弊社の強みは中小規模のシステム開発が短期間かつ、適正価格で提供することができます。<br />高価で高機能なエンタープライズシステムを提供するのではなく、カスタマイズが必要であったり、エンタープライズまでも必要としない企業様に当社の立ち位置を重宝して頂いております。<br />また、ベトナムローカルでのサブシステムの構築には当社の資産を活用していただける<br />と考えています。</p>
</td>
<td style="width: 64.04px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;"> </td>
<td style="width: 924.57px; height: 259.54px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;"><img style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;" src="../../../Media/Uploads/3.jpg" alt="" width="389" height="249" /></td>
</tr>
<tr style="height: 37px;">
<td style="width: 634.39px; height: 18.2px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;"> </td>
<td style="width: 64.04px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;"> </td>
<td style="width: 924.57px; height: 18.2px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;"> </td>
</tr>
<tr style="height: 54px;">
<td style="width: 634.39px; height: 54.6px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">
<p>・弊社は最適な選択支を選び、高品質な製品を作る為に、社内で創造的なアイデア<br />や建設的な討論を奨励しています。</p>
</td>
<td style="width: 64.04px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;"> </td>
<td style="width: 924.57px; height: 315.54px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;"><img style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;" src="../../../Media/Uploads/5.jpg" alt="" width="390" height="312" /></td>
</tr>
</tbody>
</table>
<p style="background-color: transparent; color: #000000; cursor: text; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; outline-color: transparent; outline-style: none; outline-width: 0px; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"><img src="../../../Media/Uploads/Slide6_1.JPG" alt="Slide6_1" width="1110" height="833" /></p>', N'About-us', N'ja-JP', 1)
INSERT [dbo].[tbl_post_lang] ([Id], [PostId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (36, 4, N'高付加価値で良いサービスを提供するために日々取り組んでいます。', NULL, NULL, N'invalid', N'ja-JP', 1)
INSERT [dbo].[tbl_post_lang] ([Id], [PostId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (37, 9, N'TRI ÂN KHÁCH HÀNG', NULL, N'<p><strong><span style="font-size: 14pt;">1. THIỆP TẾT HÀNG NĂM</span></strong></p>
<p><img src="../../../Media/Uploads/IMG_7459.jpg" alt="IMG_7459" width="589" height="417" /></p>
<p> </p>
<p><img src="../../../Media/Uploads/IMG_7461.jpg" alt="IMG_7461" width="585" height="431" /></p>
<p> </p>
<p><strong><span style="font-size: 14pt;">2. QUÀ MỪNG KHÁCH HÀNG NHẬM CHỨC</span></strong></p>
<p><img src="../../../Media/Uploads/photofacefun_com_1564048612.jpg" alt="" width="587" height="657" /></p>
<p> </p>
<p><strong><span style="font-size: 14pt;">3. QUÀ CHIA TAY KHÁCH HÀNG VỀ NƯỚC</span></strong></p>
<p><img src="../../../Media/Uploads/IMG_4220.JPG" alt="" width="588" height="441" /></p>', N'tri-an-khach-hang', N'vi-VN', 1)
INSERT [dbo].[tbl_post_lang] ([Id], [PostId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (38, 13, N'お問合せ・お見積り', NULL, N'<p style="background-color: transparent; color: #000000; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; text-align: center; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"><span style="font-family: arial,helvetica,sans-serif; font-size: 18.66px;"><strong><span style="text-decoration: underline;">問合せ窓口</span></strong></span></p>
<table style="background-color: transparent; color: #000000; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; height: 214.8px; letter-spacing: normal; orphans: 2; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; width: 771px; word-spacing: 0px;" width="695">
<tbody style="cursor: text; outline-color: transparent; outline-style: none; outline-width: 0px;">
<tr style="height: 18px;">
<td style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; height: 48.8px; text-align: left; width: 765px;" colspan="2">
<p style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;"><span style="color: #3366ff; font-family: verdana,geneva,sans-serif; font-size: 16px;">                        CHIE SE CO.,LTD</span></p>
</td>
</tr>
<tr style="height: 36px;">
<td style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; height: 20.8px; width: 227.8px;"><span style="color: #000000; font-family: verdana,geneva,sans-serif; font-size: 16px;">                      所在地：</span></td>
<td style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; height: 41.6px; text-align: left; width: 533.2px;"><span style="font-family: verdana,geneva,sans-serif; font-size: 16px;">19th Floor, ICON4 Tower, 243A De La Thanh Str, Lang Thuong Ward, Dong Da Dist, Ha Noi, Viet Nam</span></td>
</tr>
<tr style="height: 36px;">
<td style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; height: 20.8px; width: 227.8px;"><span style="font-family: verdana,geneva,sans-serif;">                         <span style="color: #000000; font-family: verdana,geneva,sans-serif; font-size: 16px;">電話番号：</span></span></td>
<td style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; height: 20.8px; text-align: left; width: 533.2px;"><span style="color: #000000; font-family: verdana,geneva,sans-serif; font-size: 16px;">(084) 243 200 2128</span></td>
</tr>
<tr style="height: 18px;">
<td style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; height: 20.8px; width: 227.8px;"><span style="color: #000000; font-family: verdana,geneva,sans-serif; font-size: 16px;">                      Fax：</span></td>
<td style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; height: 20.8px; text-align: left; width: 533.2px;"><span style="color: #000000; font-family: verdana,geneva,sans-serif; font-size: 16px;">(084) 243 726 5493</span></td>
</tr>
<tr style="height: 18px;">
<td style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; height: 20.8px; width: 227.8px;"><span style="color: #000000; font-family: verdana,geneva,sans-serif; font-size: 16px;">                      Email：</span></td>
<td style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; height: 20.8px; text-align: left; width: 533.2px;"><span style="color: #000000; font-family: verdana,geneva,sans-serif; font-size: 16px;"><a style="color: #000000;">maihuong@chiese.vn   (ベトナム語)</a></span></td>
</tr>
<tr style="height: 18px;">
<td style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; height: 18.2px; width: 227.8px;"> </td>
<td style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; height: 20.8px; text-align: left; width: 533.2px;"><span style="color: #000000; font-family: verdana,geneva,sans-serif; font-size: 16px;"><a style="color: #000000;">kokuwano@chiese.vn   (日本語)</a></span></td>
</tr>
</tbody>
</table>
<hr style="background-color: transparent; color: #000000; cursor: default; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; text-align: center; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;" />
<p style="background-color: transparent; color: #000000; cursor: text; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; outline-color: transparent; outline-style: none; outline-width: 0px; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"><iframe style="border-image-outset: 0; border-image-repeat: stretch; border-image-slice: 100%; border-image-source: none; border-image-width: 1; border: 0px none #000000;" src="https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d3724.1196623807623!2d105.80225791440736!3d21.02789749318482!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x3135ab42139e9c5f%3A0x6eca1d6b8b7323a4!2zVMOyYSBuaMOgIEljb240!5e0!3m2!1svi!2s!4v1563875277551!5m2!1svi!2s" width="400" height="300" frameborder="0" allowfullscreen="allowfullscreen" data-mce-fragment="1"></iframe></p>', N'contact', N'ja-JP', 1)
INSERT [dbo].[tbl_post_lang] ([Id], [PostId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (39, 19, N'DỊCH VỤ', NULL, N'<p><img src="../../../Media/Uploads/Slide2.JPG" alt="" width="960" height="720" /></p>
<p> </p>
<p><img src="../../../Media/Uploads/Slide3.JPG" alt="" width="960" height="720" /></p>
<p> </p>
<p> </p>', N'dich-vu', N'vi-VN', 1)
INSERT [dbo].[tbl_post_lang] ([Id], [PostId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (40, 12, N'選ばれる理由', NULL, N'<p> </p>
<table style="width: 1152px; height: 145px; text-align: left; color: #000000; text-transform: none; text-indent: 0px; letter-spacing: normal; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; text-decoration: none; word-spacing: 0px; white-space: normal; border-collapse: collapse; orphans: 2; -webkit-text-stroke-width: 0px; background-color: transparent;" border="1">
<tbody>
<tr style="height: 220px;">
<td style="width: 635.56px; height: 145px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">
<p style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;"><strong>1．弊社の強みは中小規模のシステム開発が短期間かつ、適正価格で提供することができます。</strong><br /><strong>高価で高機能なエンタープライズシステムを提供するのではなく、カスタマイズが必要であったり、エンタープライズまでも必要としない企業様に当社の立ち位置を重宝していただいております。</strong><br /><strong>また、ベトナムローカルでのサブシステムの構築には当社の資産を活用していただける</strong><br /><strong>と考えています。</strong></p>
</td>
<td style="width: 68.06px; height: 145px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;"><em style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;"> </em></td>
<td style="width: 438.38px; height: 145px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;"><img style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;" src="../../../Media/Uploads/a.jpg" alt="" width="438" height="246" /></td>
</tr>
</tbody>
</table>
<p style="background-color: transparent; color: #000000; cursor: text; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; outline-color: transparent; outline-style: none; outline-width: 0px; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"> </p>
<table style="width: 1153px; height: 324px; text-align: left; color: #000000; text-transform: none; text-indent: 0px; letter-spacing: normal; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; text-decoration: none; word-spacing: 0px; white-space: normal; border-collapse: collapse; orphans: 2; -webkit-text-stroke-width: 0px; background-color: transparent;" border="1">
<tbody>
<tr style="height: 56px;">
<td style="width: 445.39px; height: 324px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;"><img style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;" src="../../../Media/Uploads/b.jpg" alt="" width="445" height="344" /></td>
<td style="width: 70.06px; height: 324px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;"> </td>
<td style="width: 627.55px; height: 324px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">
<p><strong>2．日系企業なので、日本企業のご要望・ご期待がよく理解できて、日本の美的価値観・</strong><br /><strong>日本クオリティを世界に届けるように微力ながら最善を尽くしています。</strong></p>
</td>
</tr>
</tbody>
</table>
<p style="background-color: transparent; color: #000000; cursor: text; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; outline-color: transparent; outline-style: none; outline-width: 0px; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"> </p>
<table style="width: 1151px; height: 54px; text-align: left; color: #000000; text-transform: none; text-indent: 0px; letter-spacing: normal; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; text-decoration: none; word-spacing: 0px; white-space: normal; border-collapse: collapse; orphans: 2; -webkit-text-stroke-width: 0px; background-color: transparent;" border="1">
<tbody>
<tr style="height: 18px;">
<td style="width: 617.54px; height: 54px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;"><strong><span style="text-align: left; color: #000000; text-transform: none; text-indent: 0px; letter-spacing: normal; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; text-decoration: none; word-spacing: 0px; display: inline !important; white-space: normal; cursor: text; orphans: 2; float: none; -webkit-text-stroke-width: 0px; background-color: transparent;">3．技術力のみならず、若々しくて熱心で前向きで向上心あり、傾聴で常に問題解決及び自分磨きに取り組んでいるチームを持っております。</span></strong></td>
<td style="width: 70.06px; height: 54px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;"> </td>
<td style="width: 453.4px; height: 54px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;"><img style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;" src="../../../Media/Uploads/c.jpg" alt="" width="453" height="301" /></td>
</tr>
</tbody>
</table>
<p style="background-color: transparent; color: #000000; cursor: text; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; outline-color: transparent; outline-style: none; outline-width: 0px; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"> </p>
<table style="width: 1151px; height: 251px; text-align: left; color: #000000; text-transform: none; text-indent: 0px; letter-spacing: normal; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; text-decoration: none; word-spacing: 0px; white-space: normal; border-collapse: collapse; orphans: 2; -webkit-text-stroke-width: 0px; background-color: transparent;" border="1">
<tbody>
<tr style="height: 251px;">
<td style="width: 448.4px; height: 251px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;"><img style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;" src="../../../Media/Uploads/e.jpg" alt="" width="448" height="252" /></td>
<td style="width: 71.06px; height: 251px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;"><em style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;"> </em></td>
<td style="width: 621.54px; height: 251px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;"><strong><span style="text-align: left; color: #000000; text-transform: none; text-indent: 0px; letter-spacing: normal; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; text-decoration: none; word-spacing: 0px; display: inline !important; white-space: normal; cursor: text; orphans: 2; float: none; -webkit-text-stroke-width: 0px; background-color: transparent;">4．お客様の機密情報を保持することを常に意識して環境を整えています。</span></strong></td>
</tr>
</tbody>
</table>
<p style="background-color: transparent; color: #000000; cursor: text; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; outline-color: transparent; outline-style: none; outline-width: 0px; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"> </p>
<table style="width: 1150px; text-align: left; color: #000000; text-transform: none; text-indent: 0px; letter-spacing: normal; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; text-decoration: none; word-spacing: 0px; white-space: normal; border-collapse: collapse; orphans: 2; -webkit-text-stroke-width: 0px; background-color: transparent;" border="1">
<tbody>
<tr>
<td style="width: 619.1px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">
<p style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;"><strong>5．技術力や経験値や創造性など弊社の全リソースを生かして、高付加価値でお客様のご期待を</strong><br /><strong>超える質の高いサービスを提供するのに日々努力しております。</strong><br /><strong>弊社のご提案でお客様の問題を解いて、お客様の益々のご発展に貢献出来る機会に恵まれたことを光栄に存じます。</strong></p>
</td>
<td style="width: 70.9px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">
<p style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;"><em style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;"> </em></p>
</td>
<td style="width: 450px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;"><img style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;" src="../../../Media/Uploads/d.jpg" alt="" width="450" height="263" /></td>
</tr>
</tbody>
</table>', N'why-us', N'ja-JP', 1)
INSERT [dbo].[tbl_post_lang] ([Id], [PostId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (41, 10, N'企業情報', NULL, N'<p style="background-color: transparent; color: #000000; cursor: text; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; outline-color: transparent; outline-style: none; outline-width: 0px; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"><span style="color: #3366ff; font-size: 18.66px;"><em><strong>社長挨拶</strong></em></span></p>
<table style="background-color: transparent; color: #000000; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; height: 397.54px; letter-spacing: normal; orphans: 2; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; width: 1007px; word-spacing: 0px;" width="862">
<tbody style="cursor: text; outline-color: transparent; outline-style: none; outline-width: 0px;">
<tr style="height: 240px;">
<td style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; height: 319.2px; width: 709.26px;">
<p style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">始めまして。<br style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;" />CHIE SEの代表 山崎博文です。<br style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;" />数あるサイトの中から当社ホームページにお越しいただきありがとうございます。<br style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;" />当社は2015年からベトナム進出を検討し、2016年にハノイにシステム開発会社<br style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;" />「CHIE SE」を設立しました。<br style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;" />日本とベトナムで「知恵」を「共有する」という意味で、日本語の「知恵」の「CHIE」<br style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;" />とベトナム語の「Chia sẻ」の「SE」を組み合せた造語を会社名としました。<br style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;" />親会社は日本の大阪で2003年から創業している「GAINSHARING」というシステム会社です。<br style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;" />日本では様々な会社にシステム開発を提供しています。製造業を中心とした会社（工場）には<br style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;" />各種制御システムや監視システム、そして生産管理などを納入しています。<br style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;" />また運送業や放送局には業務系のシステムを納入し、提案から開発そして保守に至るまで<br style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;" />幅広いサービスを活用いただいています。<br style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;" />ベトナムにおいても日本同様に日系の製造業のお客様に対して日本で培ったノウハウや資産<br style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;" />を適切な価格と納期で提供出来る様に体制を整えています。<br style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;" />まだまだ発展途上の会社ではありますが、ベトナムの成長とともに当社も成長し、<br style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;" />貢献出来る様に努めていきますので、ご指導ご鞭撻のほど、よろしくお願いいたします。</p>
</td>
<td style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; height: 391.54px; width: 287.74px;"><img style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;" src="../../../Media/Uploads/president.jpg" alt="" width="270" height="388" /></td>
</tr>
</tbody>
</table>
<p style="background-color: transparent; color: #000000; cursor: text; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; outline-color: transparent; outline-style: none; outline-width: 0px; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"><span style="color: #0000ff; font-family: arial,helvetica,sans-serif; text-decoration: underline;"><strong><span style="font-size: 18.66px;">企業理念</span></strong></span></p>
<p style="background-color: transparent; color: #000000; cursor: text; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; outline-color: transparent; outline-style: none; outline-width: 0px; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"><img src="../../../Media/Uploads/Presentation1_1.pptx" alt="" /><img src="../../../Media/Uploads/Presentation1.pptx" alt="" /><img src="../../../Media/Uploads/16ef08cf65de45a06a6551199a55504e.jpg" alt="" width="1007" height="566" /></p>
<p style="background-color: transparent; color: #000000; cursor: text; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; outline-color: transparent; outline-style: none; outline-width: 0px; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"><span style="color: #3366ff; font-size: 18.66px;"><strong>企業概要</strong></span></p>
<table style="width: 1689px; height: 172px; text-align: left; color: #000000; text-transform: none; text-indent: 0px; letter-spacing: normal; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; text-decoration: none; word-spacing: 0px; white-space: normal; orphans: 2; -webkit-text-stroke-width: 0px; background-color: transparent;" width="998">
<tbody style="cursor: text; outline-color: transparent; outline-style: none; outline-width: 0px;">
<tr style="height: 25px;">
<td style="width: 129.02px; height: 18px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">社名：</td>
<td style="width: 498.67px; height: 18px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">CHIE SE CO.,LTD</td>
<td style="width: 65.89px; height: 172px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;" rowspan="8"> </td>
<td style="width: 967.42px; height: 172px; text-align: left; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;" rowspan="8"><img src="../../../Media/Uploads/IMG_7571.jpg" alt="" width="384" height="512" /></td>
</tr>
<tr style="height: 74px;">
<td style="width: 129.02px; height: 18px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">所在地：</td>
<td style="width: 498.67px; height: 18px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">
<p style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">19th Floor, ICON4 Tower, 243A De La Thanh Str,<br style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;" />Lang Thuong Ward, Dong Da Dist, Ha Noi, Viet Nam</p>
</td>
</tr>
<tr style="height: 26px;">
<td style="width: 129.02px; height: 18px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">設立日：</td>
<td style="width: 498.67px; height: 18px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">2016年03月23日</td>
</tr>
<tr style="height: 196px;">
<td style="width: 129.02px; height: 46px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">
<p style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">事業概要：</p>
</td>
<td style="width: 498.67px; height: 46px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">
<p style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">CPC842：ソフトウェアの製造・開発<br style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;" />CPC841：ハード機器設置のコンサルティング<br style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;" />CPC843：データ処理<br style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;" />CPC865： 管理コンサルティング</p>
</td>
</tr>
<tr style="height: 25px;">
<td style="width: 129.02px; height: 18px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">代表者：</td>
<td style="width: 498.67px; height: 18px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">社長　山崎博文</td>
</tr>
<tr style="height: 26px;">
<td style="width: 129.02px; height: 18px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">従業員数：</td>
<td style="width: 498.67px; height: 18px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">6人（2019年07月まで)</td>
</tr>
<tr style="height: 50px;">
<td style="width: 129.02px; height: 18px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">取引銀行：</td>
<td style="width: 498.67px; height: 18px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">BIDV, MIZUHO</td>
</tr>
<tr style="height: 26px;">
<td style="width: 129.02px; height: 18px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">電話番号：</td>
<td style="width: 498.67px; height: 18px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">(084) 243 200 2128</td>
</tr>
</tbody>
</table>
<p style="background-color: transparent; color: #000000; cursor: text; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; outline-color: transparent; outline-style: none; outline-width: 0px; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"> </p>
<p style="background-color: transparent; color: #000000; cursor: text; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; outline-color: transparent; outline-style: none; outline-width: 0px; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"><span style="color: #3366ff; font-size: 18.66px;"><strong>当社の特徴</strong></span></p>
<table style="background-color: transparent; color: #000000; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; height: 922.6px; letter-spacing: normal; orphans: 2; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; width: 1645px; word-spacing: 0px;" width="870">
<tbody style="cursor: text; outline-color: transparent; outline-style: none; outline-width: 0px;">
<tr style="height: 102px;">
<td style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; height: 133px; width: 637.52px;">
<p style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">・独立系でワンステップ<br />メーカーや親会社に依存しないポジションでお客様にとって最適なシステムを提案します。<br />業務コンサルティング、提案業務から設計、開発及び納入、保守に至るまで<br />全てを自社内スタッフで対応させていただきます。</p>
<p style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;"> </p>
</td>
<td style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; width: 64.36px;"> </td>
<td style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; height: 245.54px; width: 929.12px;"><img style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;" src="../../../Media/Uploads/4.jpg" alt="" width="388" height="242" /></td>
</tr>
<tr style="height: 29px;">
<td style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; height: 18.2px; width: 637.52px;"> </td>
<td style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; width: 64.36px;"> </td>
<td style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; height: 18.2px; width: 929.12px;"> </td>
</tr>
<tr style="height: 162px;">
<td style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; height: 119px; width: 637.52px;">
<p style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">・弊社の強みは中小規模のシステム開発が短期間かつ、適正価格で提供することができます。<br style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;" />高価で高機能なエンタープライズシステムを提供するのではなく、カスタマイズが必要であったり、エンタープライズまでも必要としない企業様に当社の立ち位置を重宝して頂いております。<br style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;" />また、ベトナムローカルでのサブシステムの構築には当社の資産を活用していただける<br style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;" />と考えています。</p>
</td>
<td style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; width: 64.36px;"> </td>
<td style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; height: 252.54px; width: 929.12px;"><img style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;" src="../../../Media/Uploads/3.jpg" alt="" width="389" height="249" /></td>
</tr>
<tr style="height: 37px;">
<td style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; height: 18.2px; width: 637.52px;"> </td>
<td style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; width: 64.36px;"> </td>
<td style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; height: 18.2px; width: 929.12px;"> </td>
</tr>
<tr style="height: 54px;">
<td style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; height: 64.4px; width: 637.52px;">
<p style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;">・弊社は最適な選択支を選び、高品質な製品を作る為に、社内で創造的なアイデア<br style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;" />や建設的な討論を奨励しています。</p>
</td>
<td style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; width: 64.36px;"> </td>
<td style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; height: 315.54px; width: 929.12px;"><img style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;" src="../../../Media/Uploads/5.jpg" alt="" width="390" height="312" /></td>
</tr>
</tbody>
</table>
<p style="background-color: transparent; color: #000000; cursor: text; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; outline-color: transparent; outline-style: none; outline-width: 0px; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"><img src="../../../Media/Uploads/Slide6_1.JPG" alt="Slide6_1" width="1110" height="833" /></p>', N'about-us', N'ja-JP', 1)
INSERT [dbo].[tbl_post_lang] ([Id], [PostId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (42, 20, N'DỊCH VỤ', NULL, N'<p><img src="../../../Media/Uploads/Slide2.JPG" alt="" width="960" height="720" /></p>
<p>※ Ngoài ra chúng tôi còn cung cấp các dịch vụ khác như:<br />・Cung cấp các phần mềm đóng gói tự sản xuất với chi phí hợp lý<br />・Thiết kế website công ty<br />・Biên dịch ngôn ngữ phần mềm (tiếng Việt, tiếng Nhật) <br />・Đôi khi chúng tôi cũng đưa ra những đề xuất "không cần tạo phần mềm" sau khi nghiên cứu yêu cầu của khách hàng.</p>
<p><img src="../../../Media/Uploads/Slide3.JPG" alt="" width="960" height="720" /></p>', N'dich-vu', N'vi-VN', 1)
INSERT [dbo].[tbl_post_lang] ([Id], [PostId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (43, 21, N'お問合せ・お見積り', NULL, N'<p style="background-color: transparent; color: #000000; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; text-align: center; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"><span style="font-family: arial,helvetica,sans-serif; font-size: 18.66px;"><strong><span style="text-decoration: underline;">問合せ窓口</span></strong></span></p>
<table style="background-color: transparent; color: #000000; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; height: 214.8px; letter-spacing: normal; orphans: 2; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; width: 771px; word-spacing: 0px;" width="695">
<tbody style="cursor: text; outline-color: transparent; outline-style: none; outline-width: 0px;">
<tr style="height: 18px;">
<td style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; height: 48.8px; text-align: left; width: 765px;" colspan="2">
<p style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;"><span style="color: #3366ff; font-family: verdana,geneva,sans-serif; font-size: 16px;">                        CHIE SE CO.,LTD</span></p>
</td>
</tr>
<tr style="height: 36px;">
<td style="width: 227.8px; height: 20.8px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;"><span style="color: #000000; font-family: verdana,geneva,sans-serif; font-size: 16px;">                      所在地：</span></td>
<td style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; height: 41.6px; text-align: left; width: 533.2px;"><span style="font-family: verdana,geneva,sans-serif; font-size: 12pt;">19th Floor, ICON4 Tower, 243A De La Thanh Str, Lang Thuong Ward, Dong Da Dist, Ha Noi, Viet Nam</span></td>
</tr>
<tr style="height: 36px;">
<td style="width: 227.8px; height: 20.8px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;"><span style="font-family: verdana,geneva,sans-serif;">                         <span style="color: #000000; font-family: verdana,geneva,sans-serif; font-size: 16px;">電話番号：</span></span></td>
<td style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; height: 20.8px; text-align: left; width: 533.2px;"><span style="color: #000000; font-family: verdana,geneva,sans-serif; font-size: 16px;">(084) 243 200 2128</span></td>
</tr>
<tr style="height: 18px;">
<td style="width: 227.8px; height: 20.8px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;"><span style="color: #000000; font-family: verdana,geneva,sans-serif; font-size: 16px;">                      Fax：</span></td>
<td style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; height: 20.8px; text-align: left; width: 533.2px;"><span style="color: #000000; font-family: verdana,geneva,sans-serif; font-size: 16px;">(084) 243 726 5493</span></td>
</tr>
<tr style="height: 18px;">
<td style="width: 227.8px; height: 20.8px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;"><span style="color: #000000; font-family: verdana,geneva,sans-serif; font-size: 16px;">                      Email：</span></td>
<td style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; height: 20.8px; text-align: left; width: 533.2px;"><span style="color: #000000; font-family: verdana,geneva,sans-serif; font-size: 16px;"><a style="color: #000000;">maihuong@chiese.vn   (ベトナム語)</a></span></td>
</tr>
<tr style="height: 18px;">
<td style="width: 227.8px; height: 18.2px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;"> </td>
<td style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; height: 20.8px; text-align: left; width: 533.2px;"><span style="color: #000000; font-family: verdana,geneva,sans-serif; font-size: 16px;"><a style="color: #000000;">kokuwano@chiese.vn   (日本語)</a></span></td>
</tr>
</tbody>
</table>
<hr style="background-color: transparent; color: #000000; cursor: default; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; text-align: center; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;" />
<p style="background-color: transparent; color: #000000; cursor: text; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; outline-color: transparent; outline-style: none; outline-width: 0px; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"><iframe style="border-image-outset: 0; border-image-repeat: stretch; border-image-slice: 100%; border-image-source: none; border-image-width: 1; border: 0px none #000000;" src="https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d3724.1196623807623!2d105.80225791440736!3d21.02789749318482!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x3135ab42139e9c5f%3A0x6eca1d6b8b7323a4!2zVMOyYSBuaMOgIEljb240!5e0!3m2!1svi!2s!4v1563875277551!5m2!1svi!2s" width="400" height="300" frameborder="0" allowfullscreen="allowfullscreen" data-mce-fragment="1"></iframe></p>', N'invalid', N'ja-JP', 1)
INSERT [dbo].[tbl_post_lang] ([Id], [PostId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (44, 21, N'お問合せ・お見積り', NULL, N'<p style="background-color: transparent; color: #000000; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; text-align: center; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"><span style="font-family: arial,helvetica,sans-serif; font-size: 18.66px;"><strong><span style="text-decoration: underline;">問合せ窓口</span></strong></span></p>
<table style="background-color: transparent; color: #000000; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; height: 214.8px; letter-spacing: normal; orphans: 2; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; width: 771px; word-spacing: 0px;" width="695">
<tbody style="cursor: text; outline-color: transparent; outline-style: none; outline-width: 0px;">
<tr style="height: 18px;">
<td style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; height: 48.8px; text-align: left; width: 765px;" colspan="2">
<p style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;"><span style="color: #3366ff; font-family: verdana,geneva,sans-serif; font-size: 16px;">                        CHIE SE CO.,LTD</span></p>
</td>
</tr>
<tr style="height: 36px;">
<td style="width: 227.8px; height: 20.8px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;"><span style="color: #000000; font-family: verdana,geneva,sans-serif; font-size: 16px;">                      所在地：</span></td>
<td style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; height: 41.6px; text-align: left; width: 533.2px;"><span style="font-family: verdana,geneva,sans-serif; font-size: 12pt;">19th Floor, ICON4 Tower, 243A De La Thanh Str, Lang Thuong Ward, Dong Da Dist, Ha Noi, Viet Nam</span></td>
</tr>
<tr style="height: 36px;">
<td style="width: 227.8px; height: 20.8px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;"><span style="font-family: verdana,geneva,sans-serif;">                         <span style="color: #000000; font-family: verdana,geneva,sans-serif; font-size: 16px;">電話番号：</span></span></td>
<td style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; height: 20.8px; text-align: left; width: 533.2px;"><span style="color: #000000; font-family: verdana,geneva,sans-serif; font-size: 16px;">(084) 243 200 2128</span></td>
</tr>
<tr style="height: 18px;">
<td style="width: 227.8px; height: 20.8px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;"><span style="color: #000000; font-family: verdana,geneva,sans-serif; font-size: 16px;">                      Fax：</span></td>
<td style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; height: 20.8px; text-align: left; width: 533.2px;"><span style="color: #000000; font-family: verdana,geneva,sans-serif; font-size: 16px;">(084) 243 726 5493</span></td>
</tr>
<tr style="height: 18px;">
<td style="width: 227.8px; height: 20.8px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;"><span style="color: #000000; font-family: verdana,geneva,sans-serif; font-size: 16px;">                      Email：</span></td>
<td style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; height: 20.8px; text-align: left; width: 533.2px;"><span style="color: #000000; font-family: verdana,geneva,sans-serif; font-size: 16px;"><a style="color: #000000;">maihuong@chiese.vn   (ベトナム語)</a></span></td>
</tr>
<tr style="height: 18px;">
<td style="width: 227.8px; height: 18.2px; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px;"> </td>
<td style="font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; height: 20.8px; text-align: left; width: 533.2px;"><span style="color: #000000; font-family: verdana,geneva,sans-serif; font-size: 16px;"><a style="color: #000000;">kokuwano@chiese.vn   (日本語)</a></span></td>
</tr>
</tbody>
</table>
<hr style="background-color: transparent; color: #000000; cursor: default; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; text-align: center; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;" />
<p style="background-color: transparent; color: #000000; cursor: text; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; outline-color: transparent; outline-style: none; outline-width: 0px; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"><iframe style="border-image-outset: 0; border-image-repeat: stretch; border-image-slice: 100%; border-image-source: none; border-image-width: 1; border: 0px none #000000;" src="https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d3724.1196623807623!2d105.80225791440736!3d21.02789749318482!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x3135ab42139e9c5f%3A0x6eca1d6b8b7323a4!2zVMOyYSBuaMOgIEljb240!5e0!3m2!1svi!2s!4v1563875277551!5m2!1svi!2s" width="400" height="300" frameborder="0" allowfullscreen="allowfullscreen" data-mce-fragment="1"></iframe></p>', N'invalid', N'vi-VN', 1)
INSERT [dbo].[tbl_post_lang] ([Id], [PostId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (45, 20, N'事業内容', NULL, N'<p style="background-color: transparent; color: #000000; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"><img src="../../../Media/Uploads/Slide2_1.JPG" alt="" width="960" height="720" /></p>
<p style="background-color: transparent; color: #000000; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;">※ 他に、弊社も下記のサービスを提供しています：<br />・低額な自社開発のパッケージソフトの提供<br />・ホームページ制作<br />・システム言語の翻訳（ベトナム語・日本語）<br />・お客様のご要望を検討した結果、時には「システムを作らない」提案をさせていただきます。</p>
<p style="background-color: transparent; color: #000000; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"><img src="../../../Media/Uploads/Slide3_1.JPG" alt="" width="960" height="720" /></p>', N'service', N'ja-JP', 1)
INSERT [dbo].[tbl_post_lang] ([Id], [PostId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (46, 22, N'SẢN PHẨM', NULL, NULL, N'san-pham', N'vi-VN', 1)
INSERT [dbo].[tbl_post_lang] ([Id], [PostId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (47, 22, N'SẢN PHẨM', NULL, NULL, N'san-pham', N'ja-JP', 1)
INSERT [dbo].[tbl_post_lang] ([Id], [PostId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (48, 8, N'TEAM BUILDING', NULL, N'<p><span style="font-size: 14pt;"><strong>1. DU LỊCH CÔNG TY</strong></span></p>
<p><img src="../../../Media/Uploads/-4733415448873569400_IMG_0289.jpg" alt="-4733415448873569400_IMG_0289" width="623" height="378" /></p>
<p> </p>
<p><span style="font-size: 14pt;"><strong>2. TIỆC TẠI GIA</strong></span></p>
<p><img src="../../../Media/Uploads/IMG_7462.JPG" alt="IMG_7462" width="620" height="465" /></p>
<p> </p>
<p><span style="font-size: 14pt;"><strong>3. TIỆC SINH NHẬT</strong></span></p>
<p><img src="../../../Media/Uploads/IMG_7336.jpg" alt="IMG_7336" width="620" height="432" /></p>
<p> </p>', N'team-building', N'vi-VN', 1)
INSERT [dbo].[tbl_post_lang] ([Id], [PostId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (49, 7, N'TỔNG VỆ SINH HÀNG TUẦN ', NULL, N'<p>・Chiều thứ sáu hàng tuần tất cả nhân viên trong công ty sẽ dọn dẹp văn phòng.<br />Ở Việt Nam thuê người dọn dẹp thì nhàn hơn nhưng chúng tôi quyết định<br />tự phân công nhau dọn dẹp để cảm ơn văn phòng - nơi chiến đấu không mệt mỏi mỗi ngày.<br />Nhân viên người Nhật và người Việt cùng nhau dọn dẹp nên đây cũng là những giây phút giao tiếp<br />quý giá của chúng tôi với đồng nghiệp của mình.</p>
<p><img src="../../../Media/Uploads/IMG_7467_1.jpg" alt="" width="699" height="590" /></p>
<p> </p>
<p><img src="../../../Media/Uploads/IMG_7469.JPG" alt="IMG_7469" width="699" height="524" /></p>
<p> </p>
<p>・Lau kính cửa sổ vào những hôm trời trong xanh như thế này tâm trạng càng thêm phơi phới.</p>
<p><img src="../../../Media/Uploads/IMG_7018.jpg" alt="IMG_7018" width="699" height="524" /></p>
<p> </p>
<p>・Sau khi dọn dẹp xong, vào những hôm trời đẹp không ngờ còn có thể thu trọn vào tầm mắt <br />khoảnh khắc hoàng hôn rực rỡ này, không gian lắng đọng quá ! <br />Thiên nhiên thật kỳ diệu phải không các bạn?</p>
<p><img src="../../../Media/Uploads/IMG_7294.jpg" alt="" width="698" height="523" /></p>
<p> </p>', N'tong-ve-sinh-hang-tuan', N'vi-VN', 1)
INSERT [dbo].[tbl_post_lang] ([Id], [PostId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (50, 3, N'システム開発・改造', NULL, N'<p style="background-color: transparent; color: #000000; cursor: text; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; outline-color: transparent; outline-style: none; outline-width: 0px; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;">弊社はお客様の業務に対応したシステムをオーダーメイドで開発します。<br />例えば、在庫管理や資産管理や購買管理や人事管理などの業務系アプリケーション<br />そして、工場向けの各種生産管理やライン制御やプロセス制御や駐車場/駐輪場管理やDCS/PLCインターフフェースなど<br />の制御系アプリケーションです。</p>
<p style="background-color: transparent; color: #000000; cursor: text; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; outline-color: transparent; outline-style: none; outline-width: 0px; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;">典型的な開発事例は下記の通りです。</p>
<hr style="background-color: transparent; color: #000000; cursor: default; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; text-align: center; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;" />
<p style="background-color: transparent; color: #000000; cursor: text; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; outline-color: transparent; outline-style: none; outline-width: 0px; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"><strong style="cursor: text; outline-color: transparent; outline-style: none; outline-width: 0px;"><span style="text-decoration: underline;">1. 部品トレーサビリティシステム</span>：</strong></p>
<p style="background-color: transparent; color: #000000; cursor: text; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; outline-color: transparent; outline-style: none; outline-width: 0px; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"><a style="cursor: auto; outline-color: transparent; outline-style: none; outline-width: 0px;" href="../../../ourproject/detail/6/phan-mem-truy-xuat-nguon-goc-linh-kien"><img style="cursor: auto; outline-color: transparent; outline-style: none; outline-width: 0px;" src="../../../Media/Uploads/quet-ma-qr-de-nhan-dien-san-pham-nem-kim-cuong-chinh-hang-qua-tien-loi.png" alt="" width="300" height="300" /></a></p>
<hr style="background-color: transparent; color: #000000; cursor: default; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; text-align: center; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;" />
<p style="background-color: transparent; color: #000000; cursor: text; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; outline-color: transparent; outline-style: none; outline-width: 0px; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"><span style="text-decoration: underline;"><strong style="cursor: text; outline-color: transparent; outline-style: none; outline-width: 0px;">2. デジタルプリンター用ファイル自動作成システム（PASS）</strong></span><strong style="cursor: text; outline-color: transparent; outline-style: none; outline-width: 0px;">：</strong></p>
<p style="background-color: transparent; color: #000000; cursor: text; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; outline-color: transparent; outline-style: none; outline-width: 0px; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"><a style="cursor: auto; outline-color: transparent; outline-style: none; outline-width: 0px;" href="../../../ourproject/detail/5/phan-mem-tao-file-in-tu-dong-pass"><img style="cursor: auto; outline-color: transparent; outline-style: none; outline-width: 0px;" src="../../../Media/Uploads/may-in-ma-vach-chinh-hang.jpg" alt="" width="396" height="263" /></a></p>
<p style="background-color: transparent; color: #000000; cursor: text; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; outline-color: transparent; outline-style: none; outline-width: 0px; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"> </p>
<hr style="background-color: transparent; color: #000000; cursor: default; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; text-align: center; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;" />
<p style="background-color: transparent; color: #000000; cursor: text; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; outline-color: transparent; outline-style: none; outline-width: 0px; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"><span style="text-decoration: underline;"><strong style="cursor: text; outline-color: transparent; outline-style: none; outline-width: 0px;">3. 通関システムのECUS SUB SYSTEM</strong></span></p>
<p style="background-color: transparent; color: #000000; cursor: text; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; outline-color: transparent; outline-style: none; outline-width: 0px; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"><a href="../../../ourproject/detail/4/ecus-sub-system"><img src="../../../Media/Uploads/Slide4.JPG" alt="" width="960" height="720" /></a></p>', N'development', N'ja-JP', 1)
INSERT [dbo].[tbl_post_lang] ([Id], [PostId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (51, 2, N'パッケージソフト', NULL, N'<p><span style="text-decoration: underline;"><strong>パッケージソフト</strong></span></p>
<p><a href="../../../ourproject/detail/1/zaikan"><img src="../../../Media/Uploads/Slide0_1.PNG" alt="" width="707" height="489" /></a></p>', N'package-software', N'ja-JP', 1)
INSERT [dbo].[tbl_post_lang] ([Id], [PostId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (52, 9, N'お客様へのお礼', NULL, N'<p style="background-color: transparent; color: #000000; cursor: text; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; outline-color: transparent; outline-style: none; outline-width: 0px; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"><strong style="cursor: text; outline-color: transparent; outline-style: none; outline-width: 0px;"><span style="font-size: 18.66px;">1. 毎年の年賀状</span></strong></p>
<p style="background-color: transparent; color: #000000; cursor: text; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; outline-color: transparent; outline-style: none; outline-width: 0px; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"><img style="cursor: text; outline-color: transparent; outline-style: none; outline-width: 0px;" src="../../../Media/Uploads/IMG_7459.jpg" alt="IMG_7459" width="589" height="417" /></p>
<p style="background-color: transparent; color: #000000; cursor: text; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; outline-color: transparent; outline-style: none; outline-width: 0px; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"> </p>
<p style="background-color: transparent; color: #000000; cursor: text; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; outline-color: transparent; outline-style: none; outline-width: 0px; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"><img style="cursor: text; outline-color: transparent; outline-style: none; outline-width: 0px;" src="../../../Media/Uploads/IMG_7461.jpg" alt="IMG_7461" width="585" height="431" /></p>
<p style="background-color: transparent; color: #000000; cursor: text; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; outline-color: transparent; outline-style: none; outline-width: 0px; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"> </p>
<p style="background-color: transparent; color: #000000; cursor: text; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; outline-color: transparent; outline-style: none; outline-width: 0px; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"><strong style="cursor: text; outline-color: transparent; outline-style: none; outline-width: 0px;"><span style="font-size: 18.66px;">2. お客様の就任のお祝い</span></strong></p>
<p style="background-color: transparent; color: #000000; cursor: text; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; outline-color: transparent; outline-style: none; outline-width: 0px; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"><img style="cursor: text; outline-color: transparent; outline-style: none; outline-width: 0px;" src="../../../Media/Uploads/photofacefun_com_1564048612.jpg" alt="" width="587" height="657" /></p>
<p style="background-color: transparent; color: #000000; cursor: text; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; outline-color: transparent; outline-style: none; outline-width: 0px; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"> </p>
<p style="background-color: transparent; color: #000000; cursor: text; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; outline-color: transparent; outline-style: none; outline-width: 0px; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"><strong style="cursor: text; outline-color: transparent; outline-style: none; outline-width: 0px;"><span style="font-size: 18.66px;">3. お客様の帰国時のお土産</span></strong></p>
<p style="background-color: transparent; color: #000000; cursor: text; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; outline-color: transparent; outline-style: none; outline-width: 0px; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"><img style="cursor: text; outline-color: transparent; outline-style: none; outline-width: 0px;" src="../../../Media/Uploads/IMG_4220.JPG" alt="" width="588" height="441" /></p>', N'invalid', N'ja-JP', 1)
INSERT [dbo].[tbl_post_lang] ([Id], [PostId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (53, 8, N'チームビルディング', NULL, N'<p><span style="font-size: 14pt;"><strong>1. 社内旅行</strong></span></p>
<p><img src="../../../Media/Uploads/-4733415448873569400_IMG_0289.jpg" alt="-4733415448873569400_IMG_0289" width="623" height="378" /></p>
<p> </p>
<p><span style="font-size: 14pt;"><strong>2. ホームパーティー</strong></span></p>
<p><img src="../../../Media/Uploads/IMG_7462.JPG" alt="IMG_7462" width="620" height="465" /></p>
<p> </p>
<p><span style="font-size: 14pt;"><strong>3. 誕生日パーティー</strong></span></p>
<p><img src="../../../Media/Uploads/IMG_7336.jpg" alt="IMG_7336" width="620" height="432" /></p>
<p> </p>', N'invalid', N'ja-JP', 1)
INSERT [dbo].[tbl_post_lang] ([Id], [PostId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (54, 7, N'毎週のお掃除', NULL, N'<p style="background-color: transparent; color: #000000; cursor: text; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; outline-color: transparent; outline-style: none; outline-width: 0px; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;">・毎週金曜日の夕方からは、スタッフ全員で事務所の掃除をしています。<br />ベトナムでは、掃除をアウトソーシングすると楽なのですが、日々戦っているフィールド（事務所）<br />に感謝の気持ちを持つため、自分たちで掃除します。<br />日本人ベトナム人関係なく掃除するため、この掃除時間も大切なコミュニケーションタイムです。</p>
<p style="background-color: transparent; color: #000000; cursor: text; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; outline-color: transparent; outline-style: none; outline-width: 0px; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"><img style="cursor: text; outline-color: transparent; outline-style: none; outline-width: 0px;" src="../../../Media/Uploads/IMG_7467_1.jpg" alt="" width="699" height="590" /></p>
<p style="background-color: transparent; color: #000000; cursor: text; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; outline-color: transparent; outline-style: none; outline-width: 0px; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"> </p>
<p style="background-color: transparent; color: #000000; cursor: text; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; outline-color: transparent; outline-style: none; outline-width: 0px; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"><img style="cursor: text; outline-color: transparent; outline-style: none; outline-width: 0px;" src="../../../Media/Uploads/IMG_7469.JPG" alt="IMG_7469" width="699" height="524" /></p>
<p style="background-color: transparent; color: #000000; cursor: text; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; outline-color: transparent; outline-style: none; outline-width: 0px; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"> </p>
<p style="background-color: transparent; color: #000000; cursor: text; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; outline-color: transparent; outline-style: none; outline-width: 0px; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;">・こんなにきれいな晴天の日に窓ガラスを拭いたら、気持ちが高揚します。</p>
<p style="background-color: transparent; color: #000000; cursor: text; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; outline-color: transparent; outline-style: none; outline-width: 0px; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"><img style="cursor: text; outline-color: transparent; outline-style: none; outline-width: 0px;" src="../../../Media/Uploads/IMG_7018.jpg" alt="IMG_7018" width="699" height="524" /></p>
<p style="background-color: transparent; color: #000000; cursor: text; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; outline-color: transparent; outline-style: none; outline-width: 0px; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"> </p>
<p style="background-color: transparent; color: #000000; cursor: text; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; outline-color: transparent; outline-style: none; outline-width: 0px; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;">・晴天日のお掃除後、こんなに華やかな夕焼けを目に焼き付けることが出来るとは。<br /><span style="display: inline !important; float: none; background-color: transparent; color: #000000; cursor: text; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;">なんと落ち着く空間！</span>自然って素晴らしいものですね。</p>
<p style="background-color: transparent; color: #000000; cursor: text; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; outline-color: transparent; outline-style: none; outline-width: 0px; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"><img style="cursor: text; outline-color: transparent; outline-style: none; outline-width: 0px;" src="../../../Media/Uploads/IMG_7294.jpg" alt="" width="698" height="523" /></p>', N'invalid', N'ja-JP', 1)
SET IDENTITY_INSERT [dbo].[tbl_post_lang] OFF
SET IDENTITY_INSERT [dbo].[tbl_project] ON 

INSERT [dbo].[tbl_project] ([Id], [Title], [Cover], [CategoryId], [CreatedDate], [CreatedBy], [MetaData], [Status]) VALUES (1, N'ZaiKan software', N'Media/Uploads/Slide0_1.PNG', 4, CAST(N'2019-07-09T09:50:43.893' AS DateTime), N'94E7515B-09B1-4B90-872E-6D544BA4A339', N'{"BeginDate":null,"FinishDate":null,"PersonInCharge":null,"FrameWork":null,"Customer":null}', 1)
INSERT [dbo].[tbl_project] ([Id], [Title], [Cover], [CategoryId], [CreatedDate], [CreatedBy], [MetaData], [Status]) VALUES (2, N'ChieSe Website', N'Media/Uploads/CHIE%20SE.png', 1, CAST(N'2019-07-10T13:25:06.013' AS DateTime), N'94E7515B-09B1-4B90-872E-6D544BA4A339', N'{"BeginDate":"2019-07-01T00:00:00","FinishDate":"2019-07-31T00:00:00","PersonInCharge":null,"FrameWork":"Asp.NET / HTML5","Customer":"Chie Se"}', 1)
INSERT [dbo].[tbl_project] ([Id], [Title], [Cover], [CategoryId], [CreatedDate], [CreatedBy], [MetaData], [Status]) VALUES (3, N'Shihen maintenance', N'Media/Uploads/Van-hanh-bao-tri2.jpg', 3, CAST(N'2019-07-11T11:23:40.260' AS DateTime), N'94E7515B-09B1-4B90-872E-6D544BA4A339', N'{"BeginDate":"2016-04-01T00:00:00","FinishDate":null,"PersonInCharge":"Oai Nguyen","FrameWork":null,"Customer":null}', 1)
INSERT [dbo].[tbl_project] ([Id], [Title], [Cover], [CategoryId], [CreatedDate], [CreatedBy], [MetaData], [Status]) VALUES (4, N'Suzumoto system', N'Media/Uploads/acd457c0-283f-4ee5-b94c-3736849cacc1.jpg', 2, CAST(N'2019-07-11T13:05:46.150' AS DateTime), N'94E7515B-09B1-4B90-872E-6D544BA4A339', N'{"BeginDate":null,"FinishDate":null,"PersonInCharge":null,"FrameWork":null,"Customer":null}', 1)
INSERT [dbo].[tbl_project] ([Id], [Title], [Cover], [CategoryId], [CreatedDate], [CreatedBy], [MetaData], [Status]) VALUES (5, N'PHẦN MỀM TẠO FILE IN TỰ ĐỘNG BẰNG MÁY IN KỸ THUẬT SỐ', N'Media/Uploads/PASS-JP1.png', 2, CAST(N'2019-07-24T16:14:22.497' AS DateTime), N'94E7515B-09B1-4B90-872E-6D544BA4A339', N'{"BeginDate":null,"FinishDate":null,"PersonInCharge":null,"FrameWork":null,"Customer":null}', 1)
INSERT [dbo].[tbl_project] ([Id], [Title], [Cover], [CategoryId], [CreatedDate], [CreatedBy], [MetaData], [Status]) VALUES (6, N'PHẦN MỀM TRUY XUẤT NGUỒN GỐC LINH KIỆN', N'Media/Uploads/400px-Componentes.jpg', 2, CAST(N'2019-07-24T16:18:38.130' AS DateTime), N'94E7515B-09B1-4B90-872E-6D544BA4A339', N'{"BeginDate":null,"FinishDate":null,"PersonInCharge":null,"FrameWork":null,"Customer":null}', 1)
INSERT [dbo].[tbl_project] ([Id], [Title], [Cover], [CategoryId], [CreatedDate], [CreatedBy], [MetaData], [Status]) VALUES (7, N'PHẦN MỀM QUẢN LÝ SẢN PHẨM Ở CÔNG ĐOẠN ĐÓNG GÓI', N'Media/Uploads/packaging-banner.jpg', 2, CAST(N'2019-07-24T16:20:40.180' AS DateTime), N'94E7515B-09B1-4B90-872E-6D544BA4A339', N'{"BeginDate":null,"FinishDate":null,"PersonInCharge":null,"FrameWork":null,"Customer":null}', 1)
INSERT [dbo].[tbl_project] ([Id], [Title], [Cover], [CategoryId], [CreatedDate], [CreatedBy], [MetaData], [Status]) VALUES (8, N'XUẤT QR CODE LÊN PHIẾU SẢN PHẨM', N'Media/Uploads/su-khac-nhau-giua-bien-dich-va-phien-dich-vien.png', 7, CAST(N'2019-07-24T16:45:03.003' AS DateTime), N'94E7515B-09B1-4B90-872E-6D544BA4A339', N'{"BeginDate":null,"FinishDate":null,"PersonInCharge":null,"FrameWork":null,"Customer":null}', 1)
SET IDENTITY_INSERT [dbo].[tbl_project] OFF
SET IDENTITY_INSERT [dbo].[tbl_project_category] ON 

INSERT [dbo].[tbl_project_category] ([Id], [Name], [Code], [Icon], [CreatedBy], [CreatedDate], [LastUpdated], [LastUpdatedBy], [Status], [Description], [ParentId], [Cover]) VALUES (1, N'Warehouse management software', NULL, NULL, N'94E7515B-09B1-4B90-872E-6D544BA4A339', CAST(N'2019-07-10T10:09:00.000' AS DateTime), NULL, NULL, 1, NULL, 0, N'Media/Uploads/Icons/home.svg')
INSERT [dbo].[tbl_project_category] ([Id], [Name], [Code], [Icon], [CreatedBy], [CreatedDate], [LastUpdated], [LastUpdatedBy], [Status], [Description], [ParentId], [Cover]) VALUES (2, N'Off Shore', NULL, NULL, N'94E7515B-09B1-4B90-872E-6D544BA4A339', CAST(N'2019-07-10T10:14:14.503' AS DateTime), NULL, NULL, 1, NULL, 0, N'Media/Uploads/Icons/doc.svg')
INSERT [dbo].[tbl_project_category] ([Id], [Name], [Code], [Icon], [CreatedBy], [CreatedDate], [LastUpdated], [LastUpdatedBy], [Status], [Description], [ParentId], [Cover]) VALUES (3, N'System maintenance', NULL, NULL, N'94E7515B-09B1-4B90-872E-6D544BA4A339', CAST(N'2019-07-11T14:48:29.360' AS DateTime), NULL, NULL, 1, NULL, 0, N'Media/Uploads/Icons/card.svg')
INSERT [dbo].[tbl_project_category] ([Id], [Name], [Code], [Icon], [CreatedBy], [CreatedDate], [LastUpdated], [LastUpdatedBy], [Status], [Description], [ParentId], [Cover]) VALUES (4, N'Payment system', NULL, NULL, N'94E7515B-09B1-4B90-872E-6D544BA4A339', CAST(N'2019-07-11T14:50:58.813' AS DateTime), NULL, NULL, 1, NULL, 0, N'Media/Uploads/Icons/buy.svg')
INSERT [dbo].[tbl_project_category] ([Id], [Name], [Code], [Icon], [CreatedBy], [CreatedDate], [LastUpdated], [LastUpdatedBy], [Status], [Description], [ParentId], [Cover]) VALUES (5, N'Document management', NULL, NULL, N'94E7515B-09B1-4B90-872E-6D544BA4A339', CAST(N'2019-07-11T14:52:52.827' AS DateTime), NULL, NULL, 1, NULL, 0, N'Media/Uploads/Icons/dropbox.svg')
INSERT [dbo].[tbl_project_category] ([Id], [Name], [Code], [Icon], [CreatedBy], [CreatedDate], [LastUpdated], [LastUpdatedBy], [Status], [Description], [ParentId], [Cover]) VALUES (6, N'Offer calculator', NULL, NULL, N'94E7515B-09B1-4B90-872E-6D544BA4A339', CAST(N'2019-07-11T14:54:53.687' AS DateTime), NULL, NULL, 9, NULL, 0, N'Media/Uploads/Icons/offers.svg')
INSERT [dbo].[tbl_project_category] ([Id], [Name], [Code], [Icon], [CreatedBy], [CreatedDate], [LastUpdated], [LastUpdatedBy], [Status], [Description], [ParentId], [Cover]) VALUES (7, N'Biên dịch ngôn ngữ phần mềm', NULL, NULL, N'94E7515B-09B1-4B90-872E-6D544BA4A339', CAST(N'2019-07-24T13:47:13.273' AS DateTime), NULL, NULL, 1, NULL, 0, N'Media/Uploads/Icons/offers.svg')
INSERT [dbo].[tbl_project_category] ([Id], [Name], [Code], [Icon], [CreatedBy], [CreatedDate], [LastUpdated], [LastUpdatedBy], [Status], [Description], [ParentId], [Cover]) VALUES (8, N'Tùy chỉnh phần mềm theo yêu cầu', NULL, NULL, N'94E7515B-09B1-4B90-872E-6D544BA4A339', CAST(N'2019-07-24T16:42:54.477' AS DateTime), NULL, NULL, 9, NULL, 0, N'Media/Uploads/Icons/barcode.svg')
SET IDENTITY_INSERT [dbo].[tbl_project_category] OFF
SET IDENTITY_INSERT [dbo].[tbl_project_category_lang] ON 

INSERT [dbo].[tbl_project_category_lang] ([Id], [LangCode], [Name], [ProjectCategoryId], [Description], [UrlFriendly]) VALUES (1, N'en-US', N'Stock Manage', 1, N'Looks beautiful & ultra-sharp on Retina Displays with Retina Icons, Fonts & Images. Looks beautiful ', N'stock-manage')
INSERT [dbo].[tbl_project_category_lang] ([Id], [LangCode], [Name], [ProjectCategoryId], [Description], [UrlFriendly]) VALUES (2, N'vi-VN', N'Thiết kế Website', 1, NULL, N'thiet-ke-website')
INSERT [dbo].[tbl_project_category_lang] ([Id], [LangCode], [Name], [ProjectCategoryId], [Description], [UrlFriendly]) VALUES (3, N'en-US', N'Off Shore', 2, N'Off shore projects Powerful Layout with Responsive functionality that can be adapted to any screen size.', N'off-shore')
INSERT [dbo].[tbl_project_category_lang] ([Id], [LangCode], [Name], [ProjectCategoryId], [Description], [UrlFriendly]) VALUES (4, N'vi-VN', N'Thiết kế và tùy chỉnh phần mềm', 2, NULL, N'thiet-ke-va-tuy-chinh-phan-mem')
INSERT [dbo].[tbl_project_category_lang] ([Id], [LangCode], [Name], [ProjectCategoryId], [Description], [UrlFriendly]) VALUES (5, N'en-US', N'System maintenance', 3, N'This package is built for companies who require IT maintenance support in order to strenghten their business', N'system-maintenance')
INSERT [dbo].[tbl_project_category_lang] ([Id], [LangCode], [Name], [ProjectCategoryId], [Description], [UrlFriendly]) VALUES (6, N'en-US', N'Payment system', 4, N'A payment system is any system used to settle financial transactions through the transfer of monetary value. ', N'payment-system')
INSERT [dbo].[tbl_project_category_lang] ([Id], [LangCode], [Name], [ProjectCategoryId], [Description], [UrlFriendly]) VALUES (7, N'en-US', N'Document management', 5, N'Document management software (DMS) came about to provide an automated way of organizing, capturing, digitizing, ...', N'document-management')
INSERT [dbo].[tbl_project_category_lang] ([Id], [LangCode], [Name], [ProjectCategoryId], [Description], [UrlFriendly]) VALUES (8, N'en-US', N'Offer calculator', 6, N'If you''re looking for offer in compromise software and support within your ... Quickly use the Offer in Compromise Calculator', N'offer-calculator')
INSERT [dbo].[tbl_project_category_lang] ([Id], [LangCode], [Name], [ProjectCategoryId], [Description], [UrlFriendly]) VALUES (9, N'vi-VN', N'Thiết kế phần mềm phụ', 6, NULL, N'thiet-ke-phan-mem-phu')
INSERT [dbo].[tbl_project_category_lang] ([Id], [LangCode], [Name], [ProjectCategoryId], [Description], [UrlFriendly]) VALUES (10, N'vi-VN', N'Outsourcing', 5, NULL, N'outsourcing')
INSERT [dbo].[tbl_project_category_lang] ([Id], [LangCode], [Name], [ProjectCategoryId], [Description], [UrlFriendly]) VALUES (11, N'vi-VN', N'Phần mềm đóng gói', 4, NULL, N'phan-mem-dong-goi')
INSERT [dbo].[tbl_project_category_lang] ([Id], [LangCode], [Name], [ProjectCategoryId], [Description], [UrlFriendly]) VALUES (12, N'vi-VN', N'Dịch vụ bảo trì', 3, NULL, N'dich-vu-bao-tri')
INSERT [dbo].[tbl_project_category_lang] ([Id], [LangCode], [Name], [ProjectCategoryId], [Description], [UrlFriendly]) VALUES (13, N'vi-VN', N'Biên dịch ngôn ngữ phần mềm', 7, NULL, N'bien-dich-ngon-ngu-phan-mem')
INSERT [dbo].[tbl_project_category_lang] ([Id], [LangCode], [Name], [ProjectCategoryId], [Description], [UrlFriendly]) VALUES (14, N'vi-VN', N'Tùy chỉnh phần mềm theo yêu cầu', 8, NULL, N'tuy-chinh-phan-mem-theo-yeu-cau')
INSERT [dbo].[tbl_project_category_lang] ([Id], [LangCode], [Name], [ProjectCategoryId], [Description], [UrlFriendly]) VALUES (15, N'ja-JP', N'システム改造', 8, NULL, N'invalid')
INSERT [dbo].[tbl_project_category_lang] ([Id], [LangCode], [Name], [ProjectCategoryId], [Description], [UrlFriendly]) VALUES (16, N'ja-JP', N'システム言語の翻訳', 7, NULL, N'invalid')
INSERT [dbo].[tbl_project_category_lang] ([Id], [LangCode], [Name], [ProjectCategoryId], [Description], [UrlFriendly]) VALUES (17, N'ja-JP', N'サブシステム開発', 6, NULL, N'invalid')
INSERT [dbo].[tbl_project_category_lang] ([Id], [LangCode], [Name], [ProjectCategoryId], [Description], [UrlFriendly]) VALUES (18, N'ja-JP', N'オフショア開発', 5, NULL, N'invalid')
INSERT [dbo].[tbl_project_category_lang] ([Id], [LangCode], [Name], [ProjectCategoryId], [Description], [UrlFriendly]) VALUES (19, N'ja-JP', N'パッケージソフト', 4, NULL, N'invalid')
INSERT [dbo].[tbl_project_category_lang] ([Id], [LangCode], [Name], [ProjectCategoryId], [Description], [UrlFriendly]) VALUES (20, N'ja-JP', N'保守サービス', 3, NULL, N'invalid')
INSERT [dbo].[tbl_project_category_lang] ([Id], [LangCode], [Name], [ProjectCategoryId], [Description], [UrlFriendly]) VALUES (21, N'ja-JP', N'システム開発・改造', 2, NULL, N'invalid')
INSERT [dbo].[tbl_project_category_lang] ([Id], [LangCode], [Name], [ProjectCategoryId], [Description], [UrlFriendly]) VALUES (22, N'ja-JP', N'ホームページ制作', 1, NULL, N'invalid')
SET IDENTITY_INSERT [dbo].[tbl_project_category_lang] OFF
INSERT [dbo].[tbl_project_image] ([Id], [ProjectId], [Name], [Url], [CreatedDate]) VALUES (N'1_cca1f49142332e33b0e90e3cc3a1c79c', 1, N'', N'Media/Uploads/Slide0_1.PNG', CAST(N'2019-07-25T11:31:15.360' AS DateTime))
INSERT [dbo].[tbl_project_image] ([Id], [ProjectId], [Name], [Url], [CreatedDate]) VALUES (N'2_034539364ae75e361770a9d4e0877752', 2, N'', N'Media/Uploads/thiet-ke-website-tai-nghe-an%20(2)_1.jpg', CAST(N'2019-07-25T16:51:28.370' AS DateTime))
INSERT [dbo].[tbl_project_image] ([Id], [ProjectId], [Name], [Url], [CreatedDate]) VALUES (N'2_092314ef7f74a985598ace38def9f6eb', 2, N'', N'Media/Uploads/CHIE%20SE.jpg', CAST(N'2019-07-25T16:51:34.397' AS DateTime))
INSERT [dbo].[tbl_project_image] ([Id], [ProjectId], [Name], [Url], [CreatedDate]) VALUES (N'2_b90770a3d782c0a1640469e5bbfb2b36', 2, N'', N'Media/Uploads/cong-ty-thiet-ke-website-uy-tin-chuyen-nghiep-tai-tphcm-6.jpg', CAST(N'2019-07-25T16:51:19.120' AS DateTime))
INSERT [dbo].[tbl_project_image] ([Id], [ProjectId], [Name], [Url], [CreatedDate]) VALUES (N'3_bb1b8295b7ea10ac98df877d466618b6', 3, N'', N'Media/Uploads/Van-hanh-bao-tri2.jpg', CAST(N'2019-07-24T15:42:14.817' AS DateTime))
INSERT [dbo].[tbl_project_image] ([Id], [ProjectId], [Name], [Url], [CreatedDate]) VALUES (N'3_bf68b6efeb6b7314680c52fb2e565d53', 3, N'', N'Media/Uploads/bao%20mat%20du%20lieu.jpg', CAST(N'2019-07-24T15:42:01.907' AS DateTime))
INSERT [dbo].[tbl_project_image] ([Id], [ProjectId], [Name], [Url], [CreatedDate]) VALUES (N'4_55187204b2a426a39ea978dba21dc2e2', 4, N'', N'Media/Uploads/ctythaison.png', CAST(N'2019-07-24T15:45:37.997' AS DateTime))
INSERT [dbo].[tbl_project_image] ([Id], [ProjectId], [Name], [Url], [CreatedDate]) VALUES (N'4_a758a072d82c3fdba54aacf7b39c908b', 4, N'', N'Media/Uploads/acd457c0-283f-4ee5-b94c-3736849cacc1.jpg', CAST(N'2019-07-24T15:45:44.457' AS DateTime))
INSERT [dbo].[tbl_project_image] ([Id], [ProjectId], [Name], [Url], [CreatedDate]) VALUES (N'5_70300c6cbc4f611341a153eddf169393', 5, N'', N'Media/Uploads/PASS-JP1.png', CAST(N'2019-07-25T11:42:13.867' AS DateTime))
INSERT [dbo].[tbl_project_image] ([Id], [ProjectId], [Name], [Url], [CreatedDate]) VALUES (N'5_788176a889738b8758750ab50e640a73', 5, N'', N'Media/Uploads/kyhieu.png', CAST(N'2019-07-25T11:42:06.077' AS DateTime))
INSERT [dbo].[tbl_project_image] ([Id], [ProjectId], [Name], [Url], [CreatedDate]) VALUES (N'6_39c2d0dfda23f9ddd38c3838776aacae', 6, N'', N'Media/Uploads/400px-Componentes.jpg', CAST(N'2019-07-24T17:07:38.067' AS DateTime))
INSERT [dbo].[tbl_project_image] ([Id], [ProjectId], [Name], [Url], [CreatedDate]) VALUES (N'6_dd7159d99abbfb1cd1d68c7a582e26f0', 6, N'', N'Media/Uploads/may-in-ma-vach-chinh-hang.jpg', CAST(N'2019-07-24T17:07:30.007' AS DateTime))
INSERT [dbo].[tbl_project_image] ([Id], [ProjectId], [Name], [Url], [CreatedDate]) VALUES (N'7_825f5dc90cb9ea4a134c24fd7b1941d4', 7, N'', N'Media/Uploads/packaging-banner.jpg', CAST(N'2019-07-24T16:56:07.520' AS DateTime))
INSERT [dbo].[tbl_project_image] ([Id], [ProjectId], [Name], [Url], [CreatedDate]) VALUES (N'8_095c52e617c592e22ab3fcd16cb9d2b5', 8, N'', N'Media/Uploads/3-1489146553855.jpg', CAST(N'2019-07-25T15:48:07.237' AS DateTime))
INSERT [dbo].[tbl_project_image] ([Id], [ProjectId], [Name], [Url], [CreatedDate]) VALUES (N'8_bfeb335359ad652059f441335a79bb2e', 8, N'', N'Media/Uploads/su-khac-nhau-giua-bien-dich-va-phien-dich-vien.png', CAST(N'2019-07-25T15:48:00.363' AS DateTime))
SET IDENTITY_INSERT [dbo].[tbl_project_lang] ON 

INSERT [dbo].[tbl_project_lang] ([Id], [ProjectId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (1, 1, N'ZaiKan software', N'Simple system', NULL, N'zaikan-software', N'en-US', 1)
INSERT [dbo].[tbl_project_lang] ([Id], [ProjectId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (2, 1, N'GÓI PHẦN MỀM QUẢN LÝ KHO ZAIKAN', N'Phần mềm quản lý kho', N'<p style="background-color: transparent; color: #000000; cursor: text; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; outline-color: transparent; outline-style: none; outline-width: 0px; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"> </p>
<p style="text-align: left;"><img src="../../../Media/Uploads/Slide3_1.PNG" alt="Slide3_1" /><img src="../../../Media/Uploads/Slide2_1.PNG" alt="Slide2_1" /></p>', N'goi-phan-mem-quan-ly-kho-zaikan', N'vi-VN', 1)
INSERT [dbo].[tbl_project_lang] ([Id], [ProjectId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (3, 2, N'ChieSe Website', N'Landing page for company', N'<p>Lorem ipsum dolor sit amet, consectetur adipisicing elit. Aliquam, labore deserunt ex cupiditate est blanditiis dignissimos nesciunt doloremque laboriosam ullam necessitatibus voluptatum tempora itaque quia porro voluptates quo excepturi veritatis!</p>', N'chiese-website', N'en-US', 1)
INSERT [dbo].[tbl_project_lang] ([Id], [ProjectId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (4, 2, N'THIẾT KẾ WEBSITE CÔNG TY', N'Trang web công ty CHIE SE', NULL, N'thiet-ke-website-cong-ty', N'vi-VN', 1)
INSERT [dbo].[tbl_project_lang] ([Id], [ProjectId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (5, 3, N'Shihen maintenance', NULL, N'<p><img src="../../../Media/Uploads/chiese_logo.png" alt="" width="500" height="637" /></p>', N'shihen-maintenance', N'en-US', 1)
INSERT [dbo].[tbl_project_lang] ([Id], [ProjectId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (6, 4, N'Suzumoto system', N'Website, backend system', NULL, N'suzumoto-system', N'en-US', 1)
INSERT [dbo].[tbl_project_lang] ([Id], [ProjectId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (7, 4, N'PHẦN MỀM PHỤ KÊ KHAI HẢI QUAN ECUS SUB SYSTEM ', NULL, N'<p><img src="../../../Media/Uploads/Corporate%20Profile_Base_VN.png" alt="Corporate Profile_Base_VN" /></p>', N'phan-mem-phu-ke-khai-hai-quan-ecus-sub-system', N'vi-VN', 1)
INSERT [dbo].[tbl_project_lang] ([Id], [ProjectId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (8, 3, N'DỊCH VỤ BẢO TRÌ HỆ THỐNG IT', NULL, NULL, N'dich-vu-bao-tri-he-thong-it', N'vi-VN', 1)
INSERT [dbo].[tbl_project_lang] ([Id], [ProjectId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (9, 5, N'PHẦN MỀM TẠO FILE IN TỰ ĐỘNG PASS', NULL, N'<p><img src="../../../Media/Uploads/PASS-VN1.png" alt="PASS-VN1" /><img src="../../../Media/Uploads/PASS-VN2.png" alt="PASS-VN2" /></p>', N'phan-mem-tao-file-in-tu-dong-pass', N'vi-VN', 1)
INSERT [dbo].[tbl_project_lang] ([Id], [ProjectId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (10, 6, N'PHẦN MỀM TRUY XUẤT NGUỒN GỐC LINH KIỆN', NULL, N'<p style="background-color: transparent; color: #000000; cursor: text; font-family: Verdana,Arial,Helvetica,sans-serif; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; outline-color: transparent; outline-style: none; outline-width: 0px; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;">Sử dụng máy đọc mã vạch Handy Terminal để ghi lại thông tin truy xuất nguồn gốc (khi nào, ai, đã làm gì), sau đó xuất thông tin truy xuất nguồn gốc ra file CSV để phần mềm khác sử dụng.<br />Các chức năng chính:<br />・Ghi lại thông tin xuất kho<br />・Chống lỗi xuất kho (Đối chiếu mã QR trên danh sách xuất kho và trên linh kiện)<br />・Kiểm tra linh kiện được tiếp nhận (Đối chiếu mã QR trên danh sách và trên linh kiện)<br />・Kiểm tra linh kiện được cài đặt vào máy (Đối chiếu mã QR trên danh sách và trên linh kiện)<br />・Kiểm tra chất lượng (Đối chiếu mã QR trên danh sách và trên linh kiện)</p>', N'phan-mem-truy-xuat-nguon-goc-linh-kien', N'vi-VN', 1)
INSERT [dbo].[tbl_project_lang] ([Id], [ProjectId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (11, 7, N'PHẦN MỀM QUẢN LÝ SẢN PHẨM Ở CÔNG ĐOẠN ĐÓNG GÓI', NULL, NULL, N'phan-mem-quan-ly-san-pham-o-cong-doan-dong-goi', N'vi-VN', 1)
INSERT [dbo].[tbl_project_lang] ([Id], [ProjectId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (12, 8, N'BIÊN DỊCH NGÔN NGỮ PHẦN MỀM ', NULL, NULL, N'bien-dich-ngon-ngu-phan-mem', N'vi-VN', 1)
INSERT [dbo].[tbl_project_lang] ([Id], [ProjectId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (13, 8, N'棚卸作業支援システム QRコード出力対応', NULL, NULL, N'qr', N'ja-JP', 1)
INSERT [dbo].[tbl_project_lang] ([Id], [ProjectId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (14, 7, N'箱詰め工程 製品管理システム', NULL, NULL, N'-', N'ja-JP', 1)
INSERT [dbo].[tbl_project_lang] ([Id], [ProjectId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (15, 6, N'部品トレーサビリティシステム開発', NULL, N'<p>ハンディーターミナルを使用し、トレーサビリティの情報（いつ・だれが・なにをしたか）を記録します。<br />記録したトレーサビリティの情報は、CSVファイルに出力し、既存のシステムで使用します。<br />主な機能：<br />・出庫情報の記録<br />・誤出庫検査（出庫リストと各部品のQRコード照合）<br />・部品受け取り検査（リストとQRコードの照合）<br />・部品セット検査（リストとQRコードの照合）<br />・品質検査（リストとQRコードの照合）</p>', N'invalid', N'ja-JP', 1)
INSERT [dbo].[tbl_project_lang] ([Id], [ProjectId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (16, 5, N'デジタルプリンター用ファイル自動作成システム（PASS）', NULL, N'<p><img src="../../../Media/Uploads/PASS-JP1.png" alt="PASS-JP1" /><img src="../../../Media/Uploads/PASS-JP2.png" alt="PASS-JP2" /></p>', N'pass', N'ja-JP', 1)
INSERT [dbo].[tbl_project_lang] ([Id], [ProjectId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (17, 4, N'通関システムのサブシステム（ECUS SUB SYSTEM） ', NULL, NULL, N'ecus-sub-system', N'ja-JP', 1)
INSERT [dbo].[tbl_project_lang] ([Id], [ProjectId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (18, 3, N'ITインフラ保守サービス', NULL, NULL, N'it', N'ja-JP', 1)
INSERT [dbo].[tbl_project_lang] ([Id], [ProjectId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (19, 2, N'企業向けのホームページ制作', NULL, NULL, N'invalid', N'ja-JP', 1)
INSERT [dbo].[tbl_project_lang] ([Id], [ProjectId], [Title], [Description], [BodyContent], [UrlFriendly], [LangCode], [Status]) VALUES (20, 1, N'在庫管理パッケージソフト（ZAIKAN）', NULL, N'<p><img src="../../../Media/Uploads/Slide3.PNG" alt="Slide3" /><img src="../../../Media/Uploads/Slide2.PNG" alt="Slide2" /></p>', N'zaikan', N'ja-JP', 1)
SET IDENTITY_INSERT [dbo].[tbl_project_lang] OFF
SET IDENTITY_INSERT [dbo].[tbl_unit] ON 

INSERT [dbo].[tbl_unit] ([Id], [Code], [Name], [CreatedDate], [Status]) VALUES (1, N'kg', N'Kilogram', CAST(N'2019-06-24T10:57:23.543' AS DateTime), 1)
INSERT [dbo].[tbl_unit] ([Id], [Code], [Name], [CreatedDate], [Status]) VALUES (2, N'l', N'Lit', CAST(N'2019-06-24T10:57:34.597' AS DateTime), 1)
INSERT [dbo].[tbl_unit] ([Id], [Code], [Name], [CreatedDate], [Status]) VALUES (3, N'mile', N'Mile', CAST(N'2019-06-24T10:57:46.360' AS DateTime), 1)
SET IDENTITY_INSERT [dbo].[tbl_unit] OFF
SET IDENTITY_INSERT [dbo].[tbl_widget] ON 

INSERT [dbo].[tbl_widget] ([Id], [Controller], [Action], [Status]) VALUES (1, N'DynamicWidget', N'GetListUnit', 1)
INSERT [dbo].[tbl_widget] ([Id], [Controller], [Action], [Status]) VALUES (2, N'DynamicWidget', N'GetListDevice', 1)
SET IDENTITY_INSERT [dbo].[tbl_widget] OFF
SET IDENTITY_INSERT [dbo].[tbl_widget_lang] ON 

INSERT [dbo].[tbl_widget_lang] ([Id], [WidgetId], [Name], [Description], [LangCode]) VALUES (1, 1, N'Danh sách đơn vị', N'Hiển thị danh sách đơn vị', N'vi-VN')
SET IDENTITY_INSERT [dbo].[tbl_widget_lang] OFF
/****** Object:  Index [aspnetuserclaims$Id]    Script Date: 11/22/2019 10:11:20 AM ******/
ALTER TABLE [dbo].[aspnetuserclaims] ADD  CONSTRAINT [aspnetuserclaims$Id] UNIQUE NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[aspnetaccess] ADD  DEFAULT ((1)) FOR [Active]
GO
ALTER TABLE [dbo].[aspnetaccess] ADD  DEFAULT (NULL) FOR [Description]
GO
ALTER TABLE [dbo].[aspnetactivitylog] ADD  DEFAULT (NULL) FOR [TargetType]
GO
ALTER TABLE [dbo].[aspnetactivitylog] ADD  DEFAULT (NULL) FOR [TargetId]
GO
ALTER TABLE [dbo].[aspnetactivitylog] ADD  DEFAULT (NULL) FOR [IPAddress]
GO
ALTER TABLE [dbo].[aspnetactivitylog] ADD  DEFAULT (getdate()) FOR [ActivityDate]
GO
ALTER TABLE [dbo].[aspnetactivitylog] ADD  DEFAULT (NULL) FOR [ActivityType]
GO
ALTER TABLE [dbo].[aspnetmenus] ADD  DEFAULT (NULL) FOR [ParentId]
GO
ALTER TABLE [dbo].[aspnetmenus] ADD  DEFAULT (NULL) FOR [Area]
GO
ALTER TABLE [dbo].[aspnetmenus] ADD  DEFAULT (NULL) FOR [Name]
GO
ALTER TABLE [dbo].[aspnetmenus] ADD  DEFAULT (NULL) FOR [Title]
GO
ALTER TABLE [dbo].[aspnetmenus] ADD  DEFAULT (NULL) FOR [Desc]
GO
ALTER TABLE [dbo].[aspnetmenus] ADD  DEFAULT (NULL) FOR [Action]
GO
ALTER TABLE [dbo].[aspnetmenus] ADD  DEFAULT (NULL) FOR [Controller]
GO
ALTER TABLE [dbo].[aspnetmenus] ADD  DEFAULT ((1)) FOR [Visible]
GO
ALTER TABLE [dbo].[aspnetmenus] ADD  DEFAULT ((1)) FOR [Authenticate]
GO
ALTER TABLE [dbo].[aspnetmenus] ADD  DEFAULT (NULL) FOR [CssClass]
GO
ALTER TABLE [dbo].[aspnetmenus] ADD  DEFAULT (NULL) FOR [SortOrder]
GO
ALTER TABLE [dbo].[aspnetmenus] ADD  DEFAULT (NULL) FOR [AbsoluteUri]
GO
ALTER TABLE [dbo].[aspnetmenus] ADD  DEFAULT ((1)) FOR [Active]
GO
ALTER TABLE [dbo].[aspnetmenus] ADD  DEFAULT (NULL) FOR [IconCss]
GO
ALTER TABLE [dbo].[aspnetmenus_lang] ADD  CONSTRAINT [DF_aspnetmenus_lang_MenuId]  DEFAULT ((0)) FOR [MenuId]
GO
ALTER TABLE [dbo].[aspnetoperations] ADD  DEFAULT (NULL) FOR [Enabled]
GO
ALTER TABLE [dbo].[aspnetoperations] ADD  DEFAULT (NULL) FOR [ActionName]
GO
ALTER TABLE [dbo].[aspnetusers] ADD  CONSTRAINT [DF_aspnetusers_ProviderId]  DEFAULT ((0)) FOR [ProviderId]
GO
ALTER TABLE [dbo].[aspnetusers] ADD  CONSTRAINT [DF__aspnetuse__Email__02084FDA]  DEFAULT (NULL) FOR [Email]
GO
ALTER TABLE [dbo].[aspnetusers] ADD  CONSTRAINT [DF__aspnetuse__Locko__02FC7413]  DEFAULT (NULL) FOR [LockoutEndDateUtc]
GO
ALTER TABLE [dbo].[aspnetusers] ADD  CONSTRAINT [DF__aspnetuse__Creat__03F0984C]  DEFAULT (NULL) FOR [CreatedDateUtc]
GO
ALTER TABLE [dbo].[aspnetusers] ADD  CONSTRAINT [DF_aspnetusers_Sex]  DEFAULT ((0)) FOR [Sex]
GO
ALTER TABLE [dbo].[aspnetusers] ADD  CONSTRAINT [DF_Staff_StaffCategoryId]  DEFAULT ((0)) FOR [StaffCategoryId]
GO
ALTER TABLE [dbo].[aspnetusers] ADD  CONSTRAINT [DF_Staff_Married]  DEFAULT ((0)) FOR [Married]
GO
ALTER TABLE [dbo].[aspnetusers] ADD  CONSTRAINT [DF_Staff_Status]  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[cmn_settings] ADD  DEFAULT (NULL) FOR [SettingValue]
GO
ALTER TABLE [dbo].[tbl_currency] ADD  CONSTRAINT [DF_tbl_currency_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[tbl_currency] ADD  CONSTRAINT [DF_tbl_currency_Status]  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[tbl_device] ADD  CONSTRAINT [DF_tbl_device_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[tbl_device] ADD  CONSTRAINT [DF_tbl_device_Status]  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[tbl_ht_resource] ADD  CONSTRAINT [DF_tbl_ht_resource_LangCode]  DEFAULT (N'en-US') FOR [LangCode]
GO
ALTER TABLE [dbo].[tbl_navigation] ADD  CONSTRAINT [DF__tbl_navig__Paren__5B78929E]  DEFAULT (NULL) FOR [ParentId]
GO
ALTER TABLE [dbo].[tbl_navigation] ADD  CONSTRAINT [DF__tbl_naviga__Area__5C6CB6D7]  DEFAULT (NULL) FOR [Area]
GO
ALTER TABLE [dbo].[tbl_navigation] ADD  CONSTRAINT [DF__tbl_naviga__Name__5D60DB10]  DEFAULT (NULL) FOR [Name]
GO
ALTER TABLE [dbo].[tbl_navigation] ADD  CONSTRAINT [DF__tbl_navig__Title__5E54FF49]  DEFAULT (NULL) FOR [Title]
GO
ALTER TABLE [dbo].[tbl_navigation] ADD  CONSTRAINT [DF__tbl_naviga__Desc__5F492382]  DEFAULT (NULL) FOR [Desc]
GO
ALTER TABLE [dbo].[tbl_navigation] ADD  CONSTRAINT [DF__tbl_navig__Actio__603D47BB]  DEFAULT (NULL) FOR [Action]
GO
ALTER TABLE [dbo].[tbl_navigation] ADD  CONSTRAINT [DF__tbl_navig__Contr__61316BF4]  DEFAULT (NULL) FOR [Controller]
GO
ALTER TABLE [dbo].[tbl_navigation] ADD  CONSTRAINT [DF__tbl_navig__Visib__6225902D]  DEFAULT ((1)) FOR [Visible]
GO
ALTER TABLE [dbo].[tbl_navigation] ADD  CONSTRAINT [DF__tbl_navig__Authe__6319B466]  DEFAULT ((1)) FOR [Authenticate]
GO
ALTER TABLE [dbo].[tbl_navigation] ADD  CONSTRAINT [DF__tbl_navig__CssCl__640DD89F]  DEFAULT (NULL) FOR [CssClass]
GO
ALTER TABLE [dbo].[tbl_navigation] ADD  CONSTRAINT [DF__tbl_navig__SortO__6501FCD8]  DEFAULT (NULL) FOR [SortOrder]
GO
ALTER TABLE [dbo].[tbl_navigation] ADD  CONSTRAINT [DF__tbl_navig__Absol__65F62111]  DEFAULT (NULL) FOR [AbsoluteUri]
GO
ALTER TABLE [dbo].[tbl_navigation] ADD  CONSTRAINT [DF__tbl_navig__Activ__66EA454A]  DEFAULT ((1)) FOR [Active]
GO
ALTER TABLE [dbo].[tbl_navigation] ADD  CONSTRAINT [DF__tbl_navig__IconC__67DE6983]  DEFAULT (NULL) FOR [IconCss]
GO
ALTER TABLE [dbo].[tbl_navigation_lang] ADD  CONSTRAINT [DF_navigation_lang_NavigationId]  DEFAULT ((0)) FOR [NavigationId]
GO
ALTER TABLE [dbo].[tbl_navigation_lang] ADD  CONSTRAINT [DF_tbl_navigation_lang_AbsoluteUri]  DEFAULT (NULL) FOR [AbsoluteUri]
GO
ALTER TABLE [dbo].[tbl_page] ADD  CONSTRAINT [DF_tbl_page_layout_WidgetId]  DEFAULT ((0)) FOR [PageTemplateId]
GO
ALTER TABLE [dbo].[tbl_page] ADD  CONSTRAINT [DF_tbl_page_SortOrder]  DEFAULT ((0)) FOR [SortOrder]
GO
ALTER TABLE [dbo].[tbl_page] ADD  CONSTRAINT [DF_tbl_page_CreatedBy]  DEFAULT ((0)) FOR [CreatedBy]
GO
ALTER TABLE [dbo].[tbl_page] ADD  CONSTRAINT [DF_tbl_page_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[tbl_page] ADD  CONSTRAINT [DF_tbl_page_LastUpdatedBy]  DEFAULT ((0)) FOR [LastUpdatedBy]
GO
ALTER TABLE [dbo].[tbl_page] ADD  CONSTRAINT [DF_tbl_page_Status]  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[tbl_page_lang] ADD  CONSTRAINT [DF_tbl_page_lang_PageId]  DEFAULT ((0)) FOR [PageId]
GO
ALTER TABLE [dbo].[tbl_page_template] ADD  CONSTRAINT [DF_tbl_page_template_IsDefault]  DEFAULT ((0)) FOR [IsDefault]
GO
ALTER TABLE [dbo].[tbl_page_template] ADD  CONSTRAINT [DF_tbl_page_template_Status]  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[tbl_page_template_lang] ADD  CONSTRAINT [DF_tbl_page_template_lang_PageTemplateId]  DEFAULT ((0)) FOR [PageTemplateId]
GO
ALTER TABLE [dbo].[tbl_post] ADD  CONSTRAINT [DF_tbl_post_IsHighlights]  DEFAULT ((0)) FOR [IsHighlights]
GO
ALTER TABLE [dbo].[tbl_post] ADD  CONSTRAINT [DF_tbl_post_PostType]  DEFAULT ((0)) FOR [CategoryId]
GO
ALTER TABLE [dbo].[tbl_post] ADD  CONSTRAINT [DF_tbl_post_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[tbl_post] ADD  CONSTRAINT [DF_tbl_post_Status]  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[tbl_post_lang] ADD  CONSTRAINT [DF_tbl_post_lang_PostId]  DEFAULT ((0)) FOR [PostId]
GO
ALTER TABLE [dbo].[tbl_post_lang] ADD  CONSTRAINT [DF_tbl_post_lang_Status]  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[tbl_product] ADD  CONSTRAINT [DF_tbl_product_ProductCategoryId]  DEFAULT ((0)) FOR [ProductCategoryId]
GO
ALTER TABLE [dbo].[tbl_product] ADD  CONSTRAINT [DF_tbl_product_ProviderId]  DEFAULT ((0)) FOR [ProviderId]
GO
ALTER TABLE [dbo].[tbl_product] ADD  CONSTRAINT [DF_tbl_product_Cost]  DEFAULT ((0)) FOR [Cost]
GO
ALTER TABLE [dbo].[tbl_product] ADD  CONSTRAINT [DF_tbl_product_SaleOffCost]  DEFAULT ((0)) FOR [SaleOffCost]
GO
ALTER TABLE [dbo].[tbl_product] ADD  CONSTRAINT [DF_tbl_product_UnitId]  DEFAULT ((0)) FOR [UnitId]
GO
ALTER TABLE [dbo].[tbl_product] ADD  CONSTRAINT [DF_tbl_product_CurrencyId]  DEFAULT ((0)) FOR [CurrencyId]
GO
ALTER TABLE [dbo].[tbl_product] ADD  CONSTRAINT [DF_tbl_product_CreatedBy]  DEFAULT ((0)) FOR [CreatedBy]
GO
ALTER TABLE [dbo].[tbl_product] ADD  CONSTRAINT [DF_tbl_product_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[tbl_product] ADD  CONSTRAINT [DF_tbl_product_LastUpdatedBy]  DEFAULT ((0)) FOR [LastUpdatedBy]
GO
ALTER TABLE [dbo].[tbl_product] ADD  CONSTRAINT [DF_tbl_product_Status]  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[tbl_product] ADD  CONSTRAINT [DF_tbl_product_MinInventory]  DEFAULT ((0)) FOR [MinInventory]
GO
ALTER TABLE [dbo].[tbl_product_category] ADD  CONSTRAINT [DF_tbl_product_category_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[tbl_product_category] ADD  CONSTRAINT [DF_tbl_product_category_Status]  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[tbl_product_property] ADD  CONSTRAINT [DF_tbl_product_property_ProductId]  DEFAULT ((0)) FOR [ProductId]
GO
ALTER TABLE [dbo].[tbl_product_property] ADD  CONSTRAINT [DF_tbl_product_property_PropertyId]  DEFAULT ((0)) FOR [PropertyCategoryId]
GO
ALTER TABLE [dbo].[tbl_productcat_propertycat] ADD  CONSTRAINT [DF_tbl_productcat_propertycat_ProductCategoryId]  DEFAULT ((0)) FOR [ProductCategoryId]
GO
ALTER TABLE [dbo].[tbl_productcat_propertycat] ADD  CONSTRAINT [DF_tbl_productcat_propertycat_PropertyCategoryId]  DEFAULT ((0)) FOR [PropertyCategoryId]
GO
ALTER TABLE [dbo].[tbl_project] ADD  CONSTRAINT [DF_tbl_project_CategoryId]  DEFAULT ((0)) FOR [CategoryId]
GO
ALTER TABLE [dbo].[tbl_project] ADD  CONSTRAINT [DF_tbl_project_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[tbl_project] ADD  CONSTRAINT [DF_tbl_project_Status]  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[tbl_project_category] ADD  CONSTRAINT [DF_tbl_project_category_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[tbl_project_category] ADD  CONSTRAINT [DF_tbl_project_category_Status]  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[tbl_project_category] ADD  CONSTRAINT [DF_tbl_project_category_ParentId]  DEFAULT ((0)) FOR [ParentId]
GO
ALTER TABLE [dbo].[tbl_project_category_lang] ADD  CONSTRAINT [DF_tbl_project_category_lang_projectCategoryId]  DEFAULT ((0)) FOR [ProjectCategoryId]
GO
ALTER TABLE [dbo].[tbl_project_lang] ADD  CONSTRAINT [DF_tbl_project_lang_ProjectId]  DEFAULT ((0)) FOR [ProjectId]
GO
ALTER TABLE [dbo].[tbl_project_lang] ADD  CONSTRAINT [DF_tbl_project_lang_Status]  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[tbl_property] ADD  CONSTRAINT [DF_tbl_property_PropertyCategoryId]  DEFAULT ((0)) FOR [PropertyCategoryId]
GO
ALTER TABLE [dbo].[tbl_property] ADD  CONSTRAINT [DF_tbl_property_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[tbl_property] ADD  CONSTRAINT [DF_tbl_property_Status]  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[tbl_property_category] ADD  CONSTRAINT [DF_tbl_property_category_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[tbl_property_category] ADD  CONSTRAINT [DF_tbl_property_category_Status]  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[tbl_provider] ADD  CONSTRAINT [DF_tbl_provider_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[tbl_provider] ADD  CONSTRAINT [DF_tbl_provider_Status]  DEFAULT ((0)) FOR [Status]
GO
ALTER TABLE [dbo].[tbl_unit] ADD  CONSTRAINT [DF_tbl_unit_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[tbl_unit] ADD  CONSTRAINT [DF_tbl_unit_Status]  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[tbl_widget] ADD  CONSTRAINT [DF_tbl_widget_Status]  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[tbl_widget_lang] ADD  CONSTRAINT [DF_tbl_widget_lang_WidgetId]  DEFAULT ((0)) FOR [WidgetId]
GO
ALTER TABLE [dbo].[aspnetaccessroles]  WITH CHECK ADD  CONSTRAINT [FK_aspnetaccessroles_aspnetoperations] FOREIGN KEY([OperationId])
REFERENCES [dbo].[aspnetoperations] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[aspnetaccessroles] CHECK CONSTRAINT [FK_aspnetaccessroles_aspnetoperations]
GO
ALTER TABLE [dbo].[aspnetaccessroles]  WITH CHECK ADD  CONSTRAINT [FK_aspnetaccessroles_aspnetroles] FOREIGN KEY([RoleId])
REFERENCES [dbo].[aspnetroles] ([Id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[aspnetaccessroles] CHECK CONSTRAINT [FK_aspnetaccessroles_aspnetroles]
GO
ALTER TABLE [dbo].[aspnetoperations]  WITH CHECK ADD  CONSTRAINT [FK_aspnetoperations_aspnetaccess1] FOREIGN KEY([AccessId])
REFERENCES [dbo].[aspnetaccess] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[aspnetoperations] CHECK CONSTRAINT [FK_aspnetoperations_aspnetaccess1]
GO
ALTER TABLE [dbo].[aspnetuserroles]  WITH NOCHECK ADD  CONSTRAINT [aspnetuserroles$IdentityRole_Users] FOREIGN KEY([RoleId])
REFERENCES [dbo].[aspnetroles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[aspnetuserroles] CHECK CONSTRAINT [aspnetuserroles$IdentityRole_Users]
GO
ALTER TABLE [dbo].[tbl_product_image]  WITH CHECK ADD  CONSTRAINT [FK_tbl_product_image_tbl_product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[tbl_product] ([Id])
GO
ALTER TABLE [dbo].[tbl_product_image] CHECK CONSTRAINT [FK_tbl_product_image_tbl_product]
GO
ALTER TABLE [dbo].[tbl_project_image]  WITH CHECK ADD  CONSTRAINT [FK_tbl_project_image_tbl_project] FOREIGN KEY([ProjectId])
REFERENCES [dbo].[tbl_project] ([Id])
GO
ALTER TABLE [dbo].[tbl_project_image] CHECK CONSTRAINT [FK_tbl_project_image_tbl_project]
GO
/****** Object:  StoredProcedure [dbo].[Category_Delete]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Category_Delete]
	@Id int	
AS
BEGIN
	BEGIN TRY
    BEGIN TRANSACTION
		UPDATE tbl_category
		SET			
			Status = 9
		WHERE 1=1
		AND Id = @Id
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
/****** Object:  StoredProcedure [dbo].[Category_GetAll]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Category_GetAll]
AS
BEGIN
	--Begin select the data
	SELECT a.*
	FROM tbl_category a
	WHERE 1=1	
	AND a.Status != 9
END



GO
/****** Object:  StoredProcedure [dbo].[Category_GetById]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Category_GetById]
	@Id int	
AS
BEGIN
		SELECT TOP 1 * FROM tbl_category WHERE 1=1 AND Id=@Id
END



GO
/****** Object:  StoredProcedure [dbo].[Category_GetByPage]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Category_GetByPage]
	@Keyword nvarchar(200),
	@Status smallint,
	@Offset int,
	@PageSize int
AS
BEGIN
	--Begin select the data
	SELECT TotalCount = COUNT(*) OVER(),
		a.*
	FROM tbl_category a
	WHERE 1=1	
	AND (@Keyword IS NULL OR a.CategoryName LIKE Concat('%',@Keyword,'%'))
	AND (((a.Status = CASE WHEN (@Status = -1 OR @Status IS NULL) THEN a.Status ELSE @Status END)))
	AND a.Status != 9

	ORDER BY a.Id ASC
	OFFSET @Offset ROWS		
	FETCH NEXT @PageSize ROWS ONLY
	;
END



GO
/****** Object:  StoredProcedure [dbo].[Category_Insert]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Category_Insert]	@CategoryName     nvarchar(100) = NULL,	@Index            int = NULL,	@Status           smallint = NULL,	@IsSimple         bit = NULL,	@ParentId         int = NULL,	@LinkUrl          nvarchar(500) = NULL,	@KeyUrl           nvarchar(100) = NULLAS
BEGIN
	BEGIN TRY	BEGIN TRANSACTION
		INSERT INTO dbo.tbl_category		(CategoryName,[Index],Status,ParentId,LinkUrl,KeyUrl)		VALUES( @CategoryName,@Index,@Status,@ParentId,@LinkUrl,@KeyUrl)
	COMMIT TRAN	END TRY	BEGIN CATCH	IF @@TRANCOUNT > 0	ROLLBACK TRAN	exec dbo.SQL_WriteLog;	END CATCH
END




GO
/****** Object:  StoredProcedure [dbo].[Category_Update]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 
CREATE PROCEDURE [dbo].[Category_Update]	@Id  int,	@CategoryName     nvarchar(100) = NULL,	@Index            int = NULL,	@Status           smallint = NULL,	@ParentId         int = NULL,	@LinkUrl          nvarchar(500) = NULL,	@KeyUrl           nvarchar(100) = NULLAS
BEGIN
	BEGIN TRY	BEGIN TRANSACTION
		UPDATE dbo.tbl_category		SET 	CategoryName     = @CategoryName,		[Index]            = @Index,		Status           = @Status,		ParentId         = @ParentId,		LinkUrl          = @LinkUrl,		KeyUrl           = @KeyUrl		WHERE 1=1 AND Id = @Id 
	COMMIT TRAN	END TRY	BEGIN CATCH	IF @@TRANCOUNT > 0	ROLLBACK TRAN	exec dbo.SQL_WriteLog;	END CATCH
END


GO
/****** Object:  StoredProcedure [dbo].[Country_GetByArea]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [dbo].[Country_GetByArea]
(
	@AreaId int
)	
AS
BEGIN
	SET NOCOUNT ON;

	SELECT * FROM tbl_country  
	WHERE 1=1
	AND AreaId = @AreaId
	AND Status = 1
	ORDER BY Name ASC
	;
END



GO
/****** Object:  StoredProcedure [dbo].[Credit_GetList]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Credit_GetList]	
AS
BEGIN
	SET NOCOUNT ON;

	SELECT * FROM tbl_credit 
	WHERE 1=1
	AND Status = 1
	-- ORDER BY Name ASC
	;
END



GO
/****** Object:  StoredProcedure [dbo].[Currency_Delete]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Currency_Delete]
	 @Id int
AS
BEGIN
	 UPDATE tbl_currency
		SET Status = 9
	 WHERE 1=1
	 AND Id = @Id
	 ;
END



GO
/****** Object:  StoredProcedure [dbo].[Currency_GetById]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Currency_GetById]
	 @Id int
AS
BEGIN
	 SELECT * FROM tbl_currency
	 WHERE 1=1
	 AND Id = @Id
	 AND Status != 9
	 ;
END



GO
/****** Object:  StoredProcedure [dbo].[Currency_GetByPage]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Currency_GetByPage]
	 @Keyword nvarchar(128),
	 @Status int,
	 @Offset int,
	 @PageSize int
AS
BEGIN
	 SET NOCOUNT ON;

	 DECLARE @TotalCount int;
	 SET @TotalCount = (SELECT COUNT(1) FROM tbl_currency a
	 	 WHERE 1=1
	 	 AND (a.Status = CASE WHEN (@Status = -1 OR @Status IS NULL) THEN a.Status ELSE @Status END)
		 AND (@Keyword IS NULL OR a.Name LIKE '%' + @Keyword + '%' OR a.Name LIKE '%' + @Keyword + '%')
		 AND a.Status != 9
	 );

	 SELECT @TotalCount as TotalCount, a.*
	 	 FROM tbl_currency a
	 	 WHERE 1=1
	 	 AND (a.Status = CASE WHEN (@Status = -1 OR @Status IS NULL) THEN a.Status ELSE @Status END)
		 AND (@Keyword IS NULL OR a.Name LIKE '%' + @Keyword + '%' OR a.Name LIKE '%' + @Keyword + '%')
		 AND a.Status != 9
	 	 ORDER BY a.Id DESC
	 	 OFFSET @Offset ROWS
	 	 FETCH NEXT @PageSize ROWS ONLY
	 ;
END



GO
/****** Object:  StoredProcedure [dbo].[Currency_GetList]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Currency_GetList]
	 
AS
BEGIN
	 SELECT * FROM tbl_currency
	 WHERE 1=1
	 AND Status = 1
	 ;
END



GO
/****** Object:  StoredProcedure [dbo].[Currency_Insert]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Currency_Insert]
	@Code nvarchar(20),
	@Name nvarchar(128),
	@Status smallint
AS
BEGIN
	 INSERT INTO tbl_currency(Code,Name,Status) 
	 VALUES(@Code,@Name,@Status);

	 SELECT SCOPE_IDENTITY();
END



GO
/****** Object:  StoredProcedure [dbo].[Currency_Update]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Currency_Update]
	@Id int,
	@Code nvarchar(20),
	@Name nvarchar(128),
	@Status smallint
AS
BEGIN
	 UPDATE tbl_currency
	 SET 
	 	 Code= @Code,
	 	 Name= @Name,
	 	 Status= @Status
	 WHERE 1=1
	 AND Id = @Id
	 ;
END



GO
/****** Object:  StoredProcedure [dbo].[Device_GetList]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Device_GetList]
	 
AS
BEGIN
	 SELECT * FROM tbl_device
	 WHERE 1=1
	 AND Status = 1
	 ;
END



GO
/****** Object:  StoredProcedure [dbo].[Device_Insert]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Device_Insert]
	@Code nvarchar(20),
	@Name nvarchar(128)
AS
BEGIN
	 IF NOT EXISTS(SELECT TOP 1 Id FROM tbl_device WHERE 1=1 AND Name = @Name)
	 BEGIN
		 INSERT INTO tbl_device(Code,Name) 
		 VALUES(@Code,@Name);

		 SELECT SCOPE_IDENTITY();
		 RETURN;
	 END
END



GO
/****** Object:  StoredProcedure [dbo].[Device_RegisterNewDevice]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Device_RegisterNewDevice]
	@Code nvarchar(20),
	@Name nvarchar(128)
AS
BEGIN
	 DECLARE @CurrentId int;

	 SET @CurrentId = (SELECT TOP 1 Id FROM tbl_device WHERE 1=1 AND Name = @Name)
	 IF @CurrentId IS NULL
	 BEGIN
		 INSERT INTO tbl_device(Code,Name) 
		 VALUES(@Code,@Name);

		 SELECT SCOPE_IDENTITY();
		 
		 SELECT 1;
	 END
	 ELSE
	 BEGIN
		SELECT @CurrentId;

		SELECT 0;
	 END
END



GO
/****** Object:  StoredProcedure [dbo].[District_GetByProvince]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [dbo].[District_GetByProvince]
(
	@ProvinceId int
)	
AS
BEGIN
	SET NOCOUNT ON;

	SELECT * FROM tbl_district  
	WHERE 1=1
	AND ProvinceId = @ProvinceId
	AND Status = 1
	ORDER BY Name ASC
	;
END



GO
/****** Object:  StoredProcedure [dbo].[District_GetList]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [dbo].[District_GetList]	
AS
BEGIN
	SET NOCOUNT ON;

	SELECT * FROM tbl_district 
	WHERE 1=1
	AND Status = 1
	ORDER BY Name ASC
	;
END



GO
/****** Object:  StoredProcedure [dbo].[F_Navigation_GetList]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[F_Navigation_GetList]
AS
BEGIN	

	SELECT *
	INTO #NaviagtionTemp
	FROM
	(
		SELECT * FROM tbl_navigation a
		WHERE 1=1
		AND a.Active = 1
		AND a.Visible = 1
	) as x;
	
	SELECT * FROM  #NaviagtionTemp ORDER BY SortOrder; 
	   
	SELECT * FROM tbl_navigation_lang a
	WHERE 1=1
	AND a.NavigationId IN (SELECT Id FROM #NaviagtionTemp)	
	;
	
	DROP TABLE #NaviagtionTemp;
END

GO
/****** Object:  StoredProcedure [dbo].[F_Page_GetPageByOperation]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[F_Page_GetPageByOperation]
	@Controller nvarchar(50),
	@Action nvarchar(50)
AS
BEGIN
	DECLARE @CurrentPageId int;
	DECLARE @CurrentPageTemplateId int;
	
	SELECT @CurrentPageId = Id, @CurrentPageTemplateId = PageTemplateId FROM tbl_page WHERE 1=1 AND Controller = @Controller AND Action = @Action;
	
	--SELECT *
	--INTO #MyPageTemplate
	--FROM 
	--(SELECT TOP 1 * FROM tbl_page_template WHERE 1=1 AND Id = @CurrentPageTemplateId) as x
	--;

	SELECT * FROM tbl_page WHERE 1=1 AND Id = @CurrentPageId;

	IF @CurrentPageTemplateId > 0
	BEGIN
		SELECT TOP 1 * FROM tbl_page_template WHERE 1=1
		AND Id = @CurrentPageTemplateId
		AND Status = 1
		;
	END
	ELSE
	BEGIN
		SELECT TOP 1 * FROM tbl_page_template WHERE 1=1
		AND IsDefault = 1
		AND Status = 1
		;
	END
	
END



GO
/****** Object:  StoredProcedure [dbo].[F_Post_GetByCategory]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[F_Post_GetByCategory]
	@Keyword nvarchar(200),
	@CategoryId int,
	@LangCode nvarchar(20),
	@Offset int,
	@PageSize int
AS
BEGIN
	SELECT *
	INTO #PostByCatPaging
	FROM
	(
		SELECT * FROM tbl_post a
		WHERE 1=1		
		AND a.Status = 1
		AND (((a.CategoryId = CASE WHEN (@CategoryId = 0 OR @CategoryId = -1) THEN a.CategoryId ELSE @CategoryId END)))
		ORDER BY a.CreatedDate DESC			
	 	OFFSET @Offset ROWS
	 	FETCH NEXT @PageSize ROWS ONLY
	) as x;
	
	SELECT * FROM  #PostByCatPaging; 
	   
	SELECT * FROM tbl_post_lang a
	WHERE 1=1
	AND a.PostId IN (SELECT Id FROM #PostByCatPaging)	
	--AND (@LangCode = CASE WHEN @LangCode = LangCode THEN @LangCode ELSE 'en-US' END )
	AND LangCode = @LangCode
	;
	
	DROP TABLE #PostByCatPaging;
END



GO
/****** Object:  StoredProcedure [dbo].[F_Post_GetDetailById]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[F_Post_GetDetailById]
	@Id nvarchar(128)
AS
BEGIN
	SELECT *
	INTO #PostDetailTmp
	FROM
	(
		SELECT TOP 1 * FROM tbl_post WHERE 1=1 
		AND Id=@Id
		AND Status = 1
	) as x;
	
	SELECT * FROM  #PostDetailTmp; 
	   
	SELECT * FROM tbl_post_lang a
	WHERE 1=1
	AND a.PostId = (SELECT TOP 1 Id FROM #PostDetailTmp)	
	;
	
	DROP TABLE #PostDetailTmp;
END



GO
/****** Object:  StoredProcedure [dbo].[F_Post_GetRelated]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[F_Post_GetRelated]
	@Keyword nvarchar(200),
	@Id int,
	@LangCode nvarchar(20),
	@Offset int,
	@PageSize int
AS
BEGIN
	DECLARE @CurrentCategoryId int = 0;

	SET @CurrentCategoryId = (SELECT TOP 1 CategoryId FROM tbl_post WHERE 1=1 AND Id = @Id);
	
	SELECT *
	INTO #RelatedPostsPaging
	FROM
	(
		SELECT * FROM tbl_post a
		WHERE 1=1		
		--AND (((a.CategoryId = CASE WHEN (@CategoryId = 0 OR @CategoryId = -1) THEN a.CategoryId ELSE @CategoryId END)))	
		AND a.Status = 1
		AND (((a.CategoryId = CASE WHEN (@CurrentCategoryId = 0 OR @CurrentCategoryId = -1) THEN a.CategoryId ELSE @CurrentCategoryId END)))
		AND a.Id != @Id
		ORDER BY a.CreatedDate DESC			
	 	OFFSET @Offset ROWS
	 	FETCH NEXT @PageSize ROWS ONLY
	) as x;
	
	SELECT * FROM  #RelatedPostsPaging; 
	   
	SELECT * FROM tbl_post_lang a
	WHERE 1=1
	AND a.PostId IN (SELECT Id FROM #RelatedPostsPaging)	
	AND (@LangCode = CASE WHEN @LangCode = LangCode THEN @LangCode ELSE 'en-US' END )
	;
	
	DROP TABLE #RelatedPostsPaging;
END



GO
/****** Object:  StoredProcedure [dbo].[F_Post_SearchByPage]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[F_Post_SearchByPage]
	@Keyword nvarchar(200),
	@Offset int,
	@PageSize int,
	@LangCode nvarchar(10)
AS
BEGIN
	DECLARE @TotalCount int;

	SET NOCOUNT ON;

	SET @TotalCount = (SELECT COUNT(1) 
		FROM tbl_post a
	 	WHERE 1=1		
		AND (
			@Keyword IS NULL OR a.Title LIKE '%' + @Keyword + '%'
			OR ( SELECT TOP 1 PostId FROM tbl_post_lang b WHERE 1=1 
					AND (b.Title LIKE '%' + @Keyword + '%'
						OR b.Description LIKE '%' + @Keyword + '%'
						OR b.UrlFriendly LIKE '%' + @Keyword + '%'
					)
				) = a.Id
		)

		AND a.CategoryId = 1		
		AND a.Status = 1
	 );

	SELECT *
	INTO #PostSearchPaging
	FROM
	(
		SELECT @TotalCount as TotalCount, a.* FROM tbl_post a
		WHERE 1=1
		AND (
			@Keyword IS NULL OR a.Title LIKE '%' + @Keyword + '%'
			OR ( SELECT TOP 1 PostId FROM tbl_post_lang b WHERE 1=1 
					AND (b.Title LIKE '%' + @Keyword + '%'
						OR b.Description LIKE '%' + @Keyword + '%'
						OR b.UrlFriendly LIKE '%' + @Keyword + '%'
					)
				) = a.Id
		)
		AND a.CategoryId = 1
		AND a.Status = 1

		ORDER BY CreatedDate DESC
	 	OFFSET @Offset ROWS
	 	FETCH NEXT @PageSize ROWS ONLY
	) as x;
	
	SELECT * FROM  #PostSearchPaging; 
	   
	SELECT * FROM tbl_post_lang a
	WHERE 1=1
	AND a.PostId IN (SELECT Id FROM #PostSearchPaging)	
	AND (@LangCode = LangCode )
	;
	
	DROP TABLE #PostSearchPaging;
END



GO
/****** Object:  StoredProcedure [dbo].[F_Project_GetDetailById]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[F_Project_GetDetailById]
	@Id nvarchar(128)
AS
BEGIN
		SELECT TOP 1 * FROM tbl_project WHERE 1=1 AND Id=@Id

		SELECT * FROM tbl_project_lang WHERE 1=1 
		AND ProjectId = @Id
		;

		SELECT * FROM tbl_project_image WHERE 1=1 AND ProjectId = @Id
		ORDER BY CreatedDate DESC
		;
END



GO
/****** Object:  StoredProcedure [dbo].[F_Project_GetNewest]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[F_Project_GetNewest]
	@Offset int,
	@PageSize int
AS
BEGIN
	SELECT *
	INTO #ProjectNewestPaging
	FROM
	(
		SELECT * FROM tbl_project a
		WHERE 1=1		
		AND a.Status = 1
		ORDER BY a.CreatedDate DESC			
	 	OFFSET @Offset ROWS
	 	FETCH NEXT @PageSize ROWS ONLY
	) as x;
	
	SELECT * FROM  #ProjectNewestPaging; 
	
	DROP TABLE #ProjectNewestPaging;
END



GO
/****** Object:  StoredProcedure [dbo].[F_Project_GetRelated]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[F_Project_GetRelated]
	@Keyword nvarchar(200),
	@Id int,
	@LangCode nvarchar(20),
	@Offset int,
	@PageSize int
AS
BEGIN
	DECLARE @CurrentCategoryId int = 0;

	SET @CurrentCategoryId = (SELECT TOP 1 CategoryId FROM tbl_project WHERE 1=1 AND Id = @Id);
	
	SELECT *
	INTO #RelatedProjectsPaging
	FROM
	(
		SELECT * FROM tbl_project a
		WHERE 1=1		
		--AND (((a.CategoryId = CASE WHEN (@CategoryId = 0 OR @CategoryId = -1) THEN a.CategoryId ELSE @CategoryId END)))	
		AND a.Status = 1
		AND (((a.CategoryId = CASE WHEN (@CurrentCategoryId = 0 OR @CurrentCategoryId = -1) THEN a.CategoryId ELSE @CurrentCategoryId END)))
		AND a.Id != @Id
		ORDER BY a.CreatedDate DESC			
	 	OFFSET @Offset ROWS
	 	FETCH NEXT @PageSize ROWS ONLY
	) as x;
	
	SELECT * FROM  #RelatedProjectsPaging; 
	   
	SELECT * FROM tbl_project_lang a
	WHERE 1=1
	AND a.ProjectId IN (SELECT Id FROM #RelatedProjectsPaging)	
	--AND (@LangCode = CASE WHEN @LangCode = LangCode THEN @LangCode ELSE 'en-US' END )
	;

	SELECT * FROM tbl_project_image 
	WHERE 1=1
	AND ProjectId IN (SELECT Id FROM #RelatedProjectsPaging)
	; 
	
	DROP TABLE #RelatedProjectsPaging;
END



GO
/****** Object:  StoredProcedure [dbo].[F_Widget_GetListInString]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[F_Widget_GetListInString]
	 @List nvarchar(max)
AS
BEGIN
	 SELECT * FROM tbl_widget
	 WHERE 1=1
	 AND Id IN (SELECT * FROM dbo.fnStringList2Table(@List))
	 ;
END


GO
/****** Object:  StoredProcedure [dbo].[Footer_GetAll]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Footer_GetAll]
AS
BEGIN
	--Begin select the data
	SELECT *
	FROM tbl_footer
	WHERE 1=1		
	;
END



GO
/****** Object:  StoredProcedure [dbo].[Footer_GetByLangCode]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Footer_GetByLangCode]
@LangCode nvarchar(10)
AS
BEGIN
	--Begin select the data
	SELECT *
	FROM tbl_footer
	WHERE 1=1	
	AND LangCode = @LangCode
	;
END



GO
/****** Object:  StoredProcedure [dbo].[Footer_Update]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Footer_Update]
	@BodyContent nvarchar(max),
	@LangCode nvarchar(10)
AS
BEGIN
	 IF NOT EXISTS(SELECT TOP 1 LangCode FROM tbl_footer WHERE 1=1 AND LangCode = @LangCode)
	 BEGIN
		 INSERT INTO tbl_footer(BodyContent,LangCode) 
		 VALUES(@BodyContent,@LangCode);
		 RETURN;
	 END
	 ELSE
	 BEGIN
		UPDATE tbl_footer
		SET BodyContent = @BodyContent
		WHERE 1=1
		AND LangCode = @LangCode
		;
	 END
END



GO
/****** Object:  StoredProcedure [dbo].[GenerateSPforInsertUpdateDelete]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 
CREATE PROC [dbo].[GenerateSPforInsertUpdateDelete] 
 @Schemaname Sysname = 'dbo' 
,@Tablename  Sysname 
,@ProcName     Sysname = '' 
,@IdentityInsert  bit  = 0  
AS 
 
SET NOCOUNT ON 
 
/* 
Parameters 
@Schemaname            - SchemaName to which the table belongs to. Default value 'dbo'. 
@Tablename            - TableName for which the procs needs to be generated. 
@ProcName            - Procedure name. Default is blank and when blank the procedure name generated will be sp_<Tablename> 
@IdentityInsert        - Flag to say if the identity insert needs to be done to the table or not if identity column exists in the table. 
                      Default value is 0. 
*/ 
 
DECLARE @PKTable TABLE 
( 
TableQualifier SYSNAME 
,TableOwner       SYSNAME 
,TableName       SYSNAME 
,ColumnName       SYSNAME 
,KeySeq           int 
,PKName           SYSNAME 
) 
 
INSERT INTO @PKTable 
EXEC sp_pkeys @Tablename,@Schemaname 
 
--SELECT * FROM @PKTable 
 
DECLARE @columnNames              VARCHAR(MAX) 
DECLARE @columnNamesWithDatatypes VARCHAR(MAX) 
DECLARE @columnNamesWithDatatypeUpdate VARCHAR(MAX) 
DECLARE @columnNamesWithDatatypeDelete VARCHAR(MAX) 
DECLARE @InsertcolumnNames          VARCHAR(MAX) 
DECLARE @UpdatecolumnNames          VARCHAR(MAX) 
DECLARE @IdentityExists              BIT 

DECLARE @BEGINTRANSACTION VARCHAR(200)
DECLARE @ENDTRANSACTION VARCHAR(200)

SET @BEGINTRANSACTION='	BEGIN TRY'+CHAR(13)+
    '	BEGIN TRANSACTION'
SET @ENDTRANSACTION='	COMMIT TRAN'+CHAR(13)+
		'	END TRY'+CHAR(13)+
		'	BEGIN CATCH'+CHAR(13)+
			'	IF @@TRANCOUNT > 0'+CHAR(13)+
				'	ROLLBACK TRAN'+CHAR(13)+
			'	exec dbo.SQL_WriteLog;'+CHAR(13)+
		'	END CATCH'+CHAR(13)

SELECT @columnNames = '' 
SELECT @columnNamesWithDatatypes = '' 
SELECT @columnNamesWithDatatypeUpdate = '' 
SELECT @columnNamesWithDatatypeDelete = '' 
SELECT @InsertcolumnNames = '' 
SELECT @UpdatecolumnNames = '' 
SELECT @IdentityExists = 0 
 
DECLARE @MaxLen INT 
 
 
 
SELECT @MaxLen =  MAX(LEN(SC.NAME)) 
  FROM sys.schemas SCH 
  JOIN sys.tables  ST 
    ON SCH.schema_id =ST.schema_id 
  JOIN sys.columns SC 
    ON ST.object_id = SC.object_id 
 WHERE SCH.name = @Schemaname 
   AND ST.name  = @Tablename  
   AND SC.is_identity = CASE 
                        WHEN @IdentityInsert = 1 THEN SC.is_identity 
                        ELSE 0 
                        END 
   AND SC.is_computed = 0 
 
 
SELECT @columnNames = @columnNames + SC.name + ',' 
      ,@columnNamesWithDatatypes = @columnNamesWithDatatypes +'	@' + SC.name  
                                                             + REPLICATE(' ',@MaxLen + 5 - LEN(SC.NAME)) + STY.name  
                                                             + CASE  
                                                               WHEN STY.NAME IN ('Char','Varchar') AND SC.max_length <> -1 THEN '(' + CONVERT(VARCHAR(4),SC.max_length) + ')' 
                                                               WHEN STY.NAME IN ('Nchar','Nvarchar') AND SC.max_length <> -1 THEN '(' + CONVERT(VARCHAR(4),SC.max_length / 2 ) + ')' 
                                                               WHEN STY.NAME IN ('Char','Varchar','Nchar','Nvarchar') AND SC.max_length = -1 THEN '(Max)' 
                                                               ELSE '' 
                                                               END  
                                                               + CASE 
                                                                 WHEN NOT EXISTS(SELECT 1 FROM @PKTable WHERE ColumnName=SC.name) THEN  ' = NULL,' + CHAR(13) 
                                                                 ELSE ',' + CHAR(13) 
                                                                 END 
       ,@InsertcolumnNames = @InsertcolumnNames + '@' + SC.name + ',' 
       ,@UpdatecolumnNames = @UpdatecolumnNames  
                             + CASE 
                               WHEN NOT EXISTS(SELECT 1 FROM @PKTable WHERE ColumnName=SC.name) THEN  
                                    CASE  
                                    WHEN @UpdatecolumnNames ='' THEN '' 
                                    ELSE '	' 
                                    END +'	'+  SC.name +  + REPLICATE(' ',@MaxLen + 5 - LEN(SC.NAME)) + '= ' + '@' + SC.name + ',' + CHAR(13) 
                               ELSE '' 
                               END  
      ,@IdentityExists  = CASE  
                          WHEN SC.is_identity = 1 OR @IdentityExists = 1 THEN 1  
                          ELSE 0 
                          END 
  FROM sys.schemas SCH 
  JOIN sys.tables  ST 
    ON SCH.schema_id =ST.schema_id 
  JOIN sys.columns SC 
    ON ST.object_id = SC.object_id 
  JOIN sys.types STY 
    ON SC.user_type_id     = STY.user_type_id 
   AND SC.system_type_id = STY.system_type_id 
 WHERE SCH.name = @Schemaname 
   AND ST.name  = @Tablename 
   AND SC.is_identity = CASE 
                        WHEN @IdentityInsert = 1 THEN SC.is_identity 
                        ELSE 0 
                        END 
   AND SC.is_computed = 0 
 
DECLARE @InsertSQL VARCHAR(MAX) 
DECLARE @UpdateSQL VARCHAR(MAX) 
DECLARE @DeleteSQL VARCHAR(MAX)
DECLARE @GetByIdSQL VARCHAR(MAX) 
DECLARE @PKWhereClause VARCHAR(MAX) 
 
SELECT @PKWhereClause = '1=1 AND ' 
 
SELECT @PKWhereClause = @PKWhereClause + ColumnName + ' = @' + ColumnName 
  FROM @PKTable 
ORDER BY KeySeq 
 
SELECT @columnNames          = SUBSTRING(@columnNames,1,LEN(@columnNames)-1) 
SELECT @InsertcolumnNames = SUBSTRING(@InsertcolumnNames,1,LEN(@InsertcolumnNames)-1) 
SELECT @UpdatecolumnNames = SUBSTRING(@UpdatecolumnNames,1,LEN(@UpdatecolumnNames)-2) 
SELECT @PKWhereClause      = SUBSTRING(@PKWhereClause,1,LEN(@PKWhereClause)-5) 
SELECT @columnNamesWithDatatypes = SUBSTRING(@columnNamesWithDatatypes,1,LEN(@columnNamesWithDatatypes)-2) 
SELECT @columnNamesWithDatatypes = @columnNamesWithDatatypes

SELECT @columnNamesWithDatatypeUpdate = +'	@Id  int,'+ CHAR(13)+   + @columnNamesWithDatatypes 
SELECT @columnNamesWithDatatypeDelete = +'	@Id  int'
 
SELECT @InsertSQL = '		INSERT INTO ' + @Schemaname +'.' + @Tablename  
                                   + CHAR(13) + '		(' + @columnNames + ')' +  
                                   + CHAR(13) + '		SELECT ' + @InsertcolumnNames
 
SELECT @UpdateSQL = '		UPDATE '  + @Schemaname +'.' + @Tablename  
                               + CHAR (13) + '		SET ' + @UpdatecolumnNames  
                               + CHAR (13) + '		WHERE 1=1 AND ' + @PKWhereClause

SELECT @DeleteSQL = '		UPDATE '  + @Schemaname +'.' + @Tablename  
                               + CHAR (13) + '		SET Status=9 '   
                               + CHAR (13) + '		WHERE 1=1 AND ' + @PKWhereClause 

SELECT @GetByIdSQL = '	SELECT * FROM '  + @Schemaname +'.' + @Tablename  
                               + CHAR (13) + '	WHERE 1=1 AND ' + @PKWhereClause 
 
 IF LTRIM(RTRIM(@ProcName)) = ''  
    SELECT @ProcName = 'SP_' + @Tablename 

 PRINT 'CREATE PROCEDURE ['+@Schemaname+'].[' + @ProcName+'_Insert]' +  CHAR (13) +  @columnNamesWithDatatypes +  CHAR (13) + 'AS' 
 PRINT 'BEGIN' 
 PRINT @BEGINTRANSACTION
 IF @IdentityExists = 1 AND @IdentityInsert = 1  
 PRINT 'SET IDENTITY_INSERT ' + @Schemaname + '.' + @Tablename + ' ON ' 
 PRINT @InsertSQL 
 IF @IdentityExists = 1 AND @IdentityInsert = 1  
 PRINT 'SET IDENTITY_INSERT ' + @Schemaname + '.' + @Tablename + ' OFF ' 
 PRINT @ENDTRANSACTION
 PRINT 'END' 
 PRINT 'GO' 

 PRINT '' 
 PRINT 'CREATE PROCEDURE ['+@Schemaname+'].[' + @ProcName+'_Update]' +  CHAR (13) +  @columnNamesWithDatatypeUpdate +  CHAR (13) + 'AS' 
 PRINT 'BEGIN' 
 PRINT @BEGINTRANSACTION
 PRINT @UpdateSQL 
 PRINT @ENDTRANSACTION
 PRINT 'END' 
  
 PRINT '' 
 PRINT 'CREATE PROCEDURE ['+@Schemaname+'].[' + @ProcName+'_Delete]' +  CHAR (13) +  @columnNamesWithDatatypeDelete +  CHAR (13) + 'AS'
 PRINT 'BEGIN' 
 PRINT @BEGINTRANSACTION 
 PRINT @DeleteSQL 
 PRINT @ENDTRANSACTION
 PRINT 'END' 
 PRINT 'GO' 
 
 PRINT 'CREATE PROCEDURE ['+@Schemaname+'].[' + @ProcName+'_GetById]' +  CHAR (13) +  @columnNamesWithDatatypeDelete +  CHAR (13) + 'AS'
 PRINT 'BEGIN' 
 PRINT @GetByIdSQL 
 PRINT 'END' 
 PRINT 'GO' 
SET NOCOUNT OFF 
 
 



GO
/****** Object:  StoredProcedure [dbo].[GroupProperty_GetList]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GroupProperty_GetList]
@Code nvarchar(20)	
AS
BEGIN
	SET NOCOUNT ON;

	SELECT * FROM tbl_group_property 	
	WHERE 1=1
	AND (@Code IS NULL OR Code LIKE Concat('%',@Code,'%'))
	AND Status = 1
	ORDER BY Name ASC
	;
END



GO
/****** Object:  StoredProcedure [dbo].[Log4net_AutoFlush]    Script Date: 11/22/2019 10:11:20 AM ******/
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
/****** Object:  StoredProcedure [dbo].[Log4net_AutoGenFiles]    Script Date: 11/22/2019 10:11:20 AM ******/
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
/****** Object:  StoredProcedure [dbo].[Log4net_GetNewestRecords]    Script Date: 11/22/2019 10:11:20 AM ******/
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
/****** Object:  StoredProcedure [dbo].[Log4net_GetRecordDetails]    Script Date: 11/22/2019 10:11:20 AM ******/
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
/****** Object:  StoredProcedure [dbo].[Log4net_GetRecordsPaging]    Script Date: 11/22/2019 10:11:20 AM ******/
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
/****** Object:  StoredProcedure [dbo].[Log4net_InsertLogEntry]    Script Date: 11/22/2019 10:11:20 AM ******/
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
/****** Object:  StoredProcedure [dbo].[Menu_AddNewLang]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Menu_AddNewLang]	@MenuId int,	@Title nvarchar(100),	@LangCode nvarchar(100)AS
BEGIN
	DECLARE @Exsited int;
	SET @Exsited = (SELECT TOP 1 Id FROM aspnetmenus_lang WHERE 1=1 AND MenuId = @MenuId AND LangCode = @LangCode);

	IF @Exsited IS NOT NULL
	BEGIN
		SELECT -1;
		RETURN;
	END

	INSERT INTO aspnetmenus_lang	(MenuId,Title,LangCode)	VALUES(@MenuId,@Title,@LangCode)
	;

	SELECT 1;

	RETURN;
END




GO
/****** Object:  StoredProcedure [dbo].[Menu_Delete]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[Menu_Delete]
@Id int
as
begin
	if(not exists(select * from aspnetmenus where ParentId=@Id))
	begin
	delete aspnetmenus where Id=@Id
	end
end



GO
/****** Object:  StoredProcedure [dbo].[Menu_DeleteLang]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Menu_DeleteLang]	@Id intAS
BEGIN
	DELETE FROM aspnetmenus_lang WHERE 1=1
	AND Id = @Id
	;
END




GO
/****** Object:  StoredProcedure [dbo].[Menu_GetById]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[Menu_GetById]
@Id int
as
begin
select * from aspnetmenus where Id=@Id
end



GO
/****** Object:  StoredProcedure [dbo].[Menu_GetChildMenuByUserId]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Menu_GetChildMenuByUserId]
	@UserId nvarchar(128),
	@ParentId int
AS
BEGIN
	SET NOCOUNT ON;

	SELECT DISTINCT e.* 
	INTO #UserMenuChildPaging
	FROM aspnetuserroles a 
    LEFT JOIN aspnetaccessroles b ON a.RoleId = b.RoleId
    LEFT JOIN aspnetoperations c ON b.OperationId = c.Id
    LEFT JOIN aspnetaccess d ON c.AccessId = d.Id
    RIGHT JOIN aspnetmenus e ON (e.Action = c.ActionName AND e.Controller = d.AccessName) 
    AND (e.Action IS NOT NULL AND e.Controller IS NOT NULL)
    WHERE a.UserId = @UserId AND d.Active = 1 AND e.Active = 1 and e.ParentId = @ParentId
    ORDER BY e.SortOrder

	SELECT * FROM #UserMenuChildPaging ORDER BY SortOrder;

	SELECT * FROM aspnetmenus_lang WHERE 1=1
	AND MenuId IN (SELECT Id FROM #UserMenuChildPaging)
	;

	DROP TABLE #UserMenuChildPaging;
END



GO
/****** Object:  StoredProcedure [dbo].[Menu_GetDetail]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[Menu_GetDetail]
@Id int
as
begin
	SELECT TOP 1 * FROM aspnetmenus WHERE 1=1
	AND Id = @Id
	;

	SELECT * FROM aspnetmenus_lang WHERE 1=1
	AND MenuId = @Id
	;
end



GO
/****** Object:  StoredProcedure [dbo].[Menu_GetLangDetail]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[Menu_GetLangDetail]
@Id int
as
begin
	SELECT TOP 1 * FROM aspnetmenus_lang WHERE 1=1
	AND Id = @Id
	;
end



GO
/****** Object:  StoredProcedure [dbo].[Menu_GetRootMenuByUserId]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Menu_GetRootMenuByUserId]
	@UserId nvarchar(128)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT DISTINCT e.* 
	INTO #UserMenuPaging
	FROM aspnetuserroles a 
	LEFT JOIN aspnetaccessroles b ON a.RoleId = b.RoleId
	LEFT JOIN aspnetoperations c ON b.OperationId = c.Id
	LEFT JOIN aspnetaccess d ON c.AccessId = d.Id
	RIGHT JOIN aspnetmenus e ON (e.Action = c.ActionName AND e.Controller = d.AccessName) 
    OR (e.Action IS NULL AND e.Controller IS NULL)
    WHERE a.UserId = @UserId AND d.Active = 1 AND e.Active = 1 AND (e.ParentId is null OR e.ParentId = 0)
    ORDER BY e.SortOrder

	SELECT * FROM #UserMenuPaging ORDER BY SortOrder;

	SELECT * FROM aspnetmenus_lang WHERE 1=1
	AND MenuId IN (SELECT Id FROM #UserMenuPaging)
	;

	DROP TABLE #UserMenuPaging;
END



GO
/****** Object:  StoredProcedure [dbo].[Menu_Insert]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Menu_Insert]
@ParentId int,
@Area nvarchar(20),
@Name nvarchar(20),
@Title nvarchar(100),
@Desc nvarchar(255),
@Action nvarchar(50),
@Controller nvarchar(50),
@Visible smallint,
@Authenticate smallint,
@CssClass nvarchar(100),
@SortOrder int,
@AbsoluteUri nvarchar(255),
@Active smallint,
@IconCss nvarchar(50)  
AS
BEGIN
	Insert into aspnetmenus (ParentId,Area,[Name],Title,[Desc],[Action],Controller,Visible,Authenticate,CssClass,SortOrder,AbsoluteUri,Active,IconCss)
	values(@ParentId,@Area,@Name,@Title,@Desc,@Action,@Controller,@Visible,@Authenticate,@CssClass,@SortOrder,@AbsoluteUri,@Active,@IconCss)
END



GO
/****** Object:  StoredProcedure [dbo].[Menu_Update]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Menu_Update]
@Id int,
@ParentId int,
@Area nvarchar(20),
@Name nvarchar(20),
@Title nvarchar(100),
@Desc nvarchar(255),
@Action nvarchar(50),
@Controller nvarchar(50),
@Visible smallint,
@Authenticate smallint,
@CssClass nvarchar(100),
@SortOrder int,
@AbsoluteUri nvarchar(255),
@Active smallint,
@IconCss nvarchar(50)  
AS
BEGIN
	UPDATE [dbo].aspnetmenus SET ParentId=@ParentId,Area=@Area,[Name]=@Name,Title=@Title,[Desc]=@Desc,[Action]=@Action,Controller=@Controller,Visible=@Visible,Authenticate=@Authenticate,CssClass=@CssClass,SortOrder=@SortOrder,AbsoluteUri=@AbsoluteUri,Active=@Active,IconCss=@IconCss where Id=@Id
END



GO
/****** Object:  StoredProcedure [dbo].[Menu_UpdateLang]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Menu_UpdateLang]	@Id int,	@MenuId int,	@Title nvarchar(100),	@LangCode nvarchar(100)AS
BEGIN
	DECLARE @Exsited int;
	SET @Exsited = (SELECT TOP 1 Id FROM aspnetmenus_lang WHERE 1=1 AND MenuId = @MenuId AND LangCode = @LangCode AND Id != @Id);

	IF @Exsited IS NOT NULL
	BEGIN
		SELECT -1;
		RETURN;
	END

	UPDATE aspnetmenus_lang
		SET Title = @Title,
			LangCode = @LangCode
	WHERE 1=1
	AND Id = @Id
	;

	SELECT 1;

	RETURN;
END




GO
/****** Object:  StoredProcedure [dbo].[Navigation_AddNewLang]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Navigation_AddNewLang]	@NavigationId int,	@Title nvarchar(100),	@AbsoluteUri nvarchar(500),	@LangCode nvarchar(10)AS
BEGIN
	DECLARE @Exsited int;
	SET @Exsited = (SELECT TOP 1 Id FROM tbl_navigation_lang WHERE 1=1 AND NavigationId = @NavigationId AND LangCode = @LangCode);

	IF @Exsited IS NOT NULL
	BEGIN
		SELECT -1;
		RETURN;
	END

	INSERT INTO tbl_navigation_lang (NavigationId,Title,LangCode,AbsoluteUri)	VALUES(@NavigationId,@Title,@LangCode, @AbsoluteUri)
	;

	SELECT 1;

	RETURN;
END




GO
/****** Object:  StoredProcedure [dbo].[Navigation_Delete]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[Navigation_Delete]
@Id int
as
begin
	if(not exists(select * from tbl_navigation where ParentId=@Id))
	begin
	delete tbl_navigation where Id=@Id
	end
end



GO
/****** Object:  StoredProcedure [dbo].[Navigation_DeleteLang]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Navigation_DeleteLang]	@Id intAS
BEGIN
	DELETE FROM tbl_navigation_lang WHERE 1=1
	AND Id = @Id
	;
END




GO
/****** Object:  StoredProcedure [dbo].[Navigation_GetById]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Navigation_GetById]
	 @Id int
AS
BEGIN
	 SELECT * FROM tbl_navigation
	 WHERE 1=1
	 AND Id = @Id
	 ;
END

GO
/****** Object:  StoredProcedure [dbo].[Navigation_GetDetail]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[Navigation_GetDetail]
@Id int
as
begin
	SELECT TOP 1 * FROM tbl_navigation WHERE 1=1
	AND Id = @Id
	;

	SELECT * FROM tbl_navigation_lang WHERE 1=1
	AND NavigationId = @Id
	;
end



GO
/****** Object:  StoredProcedure [dbo].[Navigation_GetLangDetail]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[Navigation_GetLangDetail]
@Id int
as
begin
	SELECT TOP 1 * FROM tbl_navigation_lang WHERE 1=1
	AND Id = @Id
	;
end



GO
/****** Object:  StoredProcedure [dbo].[Navigation_GetList]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Navigation_GetList]
AS
BEGIN	
	 SELECT * FROM tbl_navigation a WHERE 1=1
	 AND a.Active != 9
	 ORDER BY a.SortOrder ASC
	 ;
END

GO
/****** Object:  StoredProcedure [dbo].[Navigation_Insert]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Navigation_Insert]
@ParentId int,
@Area nvarchar(20),
@Name nvarchar(20),
@Title nvarchar(100),
@Desc nvarchar(255),
@Action nvarchar(50),
@Controller nvarchar(50),
@Visible smallint,
@Authenticate smallint,
@CssClass nvarchar(100),
@SortOrder int,
@AbsoluteUri nvarchar(500),
@Active smallint,
@IconCss nvarchar(50)  
AS
BEGIN
	Insert into tbl_navigation (ParentId,Area,[Name],Title,[Desc],[Action],Controller,Visible,Authenticate,CssClass,SortOrder,AbsoluteUri,Active,IconCss)
	values(@ParentId,@Area,@Name,@Title,@Desc,@Action,@Controller,@Visible,@Authenticate,@CssClass,@SortOrder,@AbsoluteUri,@Active,@IconCss)
END



GO
/****** Object:  StoredProcedure [dbo].[Navigation_Update]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Navigation_Update]
@Id int,
@ParentId int,
@Area nvarchar(20),
@Name nvarchar(20),
@Title nvarchar(100),
@Desc nvarchar(255),
@Action nvarchar(50),
@Controller nvarchar(50),
@Visible smallint,
@Authenticate smallint,
@CssClass nvarchar(100),
@SortOrder int,
@AbsoluteUri nvarchar(500),
@Active smallint,
@IconCss nvarchar(50)  
AS
BEGIN
	UPDATE [dbo].tbl_navigation SET ParentId=@ParentId,Area=@Area,[Name]=@Name,Title=@Title,[Desc]=@Desc,[Action]=@Action,Controller=@Controller,Visible=@Visible,Authenticate=@Authenticate,CssClass=@CssClass,SortOrder=@SortOrder,AbsoluteUri=@AbsoluteUri,Active=@Active,IconCss=@IconCss where Id=@Id
END



GO
/****** Object:  StoredProcedure [dbo].[Navigation_UpdateLang]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Navigation_UpdateLang]	@Id int,	@NavigationId int,	@Title nvarchar(100),	@AbsoluteUri nvarchar(500),	@LangCode nvarchar(10)AS
BEGIN
	DECLARE @Exsited int;
	SET @Exsited = (SELECT TOP 1 Id FROM tbl_navigation_lang WHERE 1=1 AND NavigationId = @NavigationId AND LangCode = @LangCode AND Id != @Id);

	IF @Exsited IS NOT NULL
	BEGIN
		INSERT INTO tbl_navigation_lang (Title,LangCode,AbsoluteUri,Id) VALUES(@Title,@LangCode,@AbsoluteUri,@Id)
		SELECT -1;
		RETURN;
	END

	UPDATE tbl_navigation_lang
		SET Title = @Title,
			LangCode = @LangCode,
			AbsoluteUri = @AbsoluteUri
	WHERE 1=1
	AND Id = @Id
	;

	SELECT 1;

	RETURN;
END




GO
/****** Object:  StoredProcedure [dbo].[Operation_GetListNotUse]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create proc [dbo].[Operation_GetListNotUse]
as
begin
select b.AccessName, a.ActionName from aspnetoperations as a left join aspnetaccess as b on
a.AccessId=b.Id
 where a.ActionName not in(select [Action] from aspnetmenus where Controller=b.AccessName)
 order by b.AccessName
end



GO
/****** Object:  StoredProcedure [dbo].[Page_GetById]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Page_GetById]
	@Id nvarchar(128),
	@LangCode nvarchar(10)
AS
BEGIN
		SELECT TOP 1 * FROM tbl_page WHERE 1=1 AND Id=@Id

		SELECT * FROM tbl_page_lang WHERE 1=1 
		AND PageId = @Id
		AND LangCode = @LangCode
		;
END



GO
/****** Object:  StoredProcedure [dbo].[Page_GetByPage]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Page_GetByPage]
	@Keyword nvarchar(200),
	@Status smallint,
	@Offset int,
	@PageSize int,
	@LangCode nvarchar(10)
AS
BEGIN
	SELECT *
	INTO #PagePaging
	FROM
	(
		SELECT * FROM tbl_page a
		WHERE 1=1
		AND (a.Id IN (
			SELECT PageId FROM tbl_page_lang b WHERE 1=1 
				AND (@Keyword IS NULL OR b.Title LIKE '%' + @Keyword + '%' OR b.Description LIKE '%' + @Keyword + '%')						
		))
		--AND (((a.CategoryId = CASE WHEN (@CategoryId = 0 OR @CategoryId = -1) THEN a.CategoryId ELSE @CategoryId END)))
		--AND (@FromDate IS NULL OR a.CreatedDate >= @FromDate)
		--AND (@ToDate IS NULL OR a.CreatedDate <= @ToDate)		
		AND a.Status = CASE 
			WHEN @Status != -1
				THEN @Status
			ELSE 
				a.Status
			END 
		AND a.Status != 9
		--ORDER BY 
		--	CASE WHEN @SortField = 'Id' AND @SortType = 'asc' THEN a.Id END ASC,
		--	CASE WHEN @SortField = 'Id' AND @SortType = 'desc' THEN a.Id END DESC,

		--	CASE WHEN @SortField = 'CreatedDate' AND @SortType = 'asc' THEN a.CreatedDate END ASC,
		--	CASE WHEN @SortField = 'CreatedDate' AND @SortType = 'desc' THEN a.CreatedDate END DESC,

		--	CASE WHEN @SortField = 'Status' AND @SortType = 'asc' THEN a.Status END ASC,
		--	CASE WHEN @SortField = 'Status' AND @SortType = 'desc' THEN a.Status END DESC
		ORDER BY a.CreatedDate DESC
	 	OFFSET 0 ROWS
	 	FETCH NEXT 20 ROWS ONLY
	) as x;
	
	SELECT * FROM  #PagePaging; 
	   
	SELECT * FROM tbl_page_lang a
	WHERE 1=1
	AND a.PageId IN (SELECT Id FROM #PagePaging)	
	AND (@LangCode = CASE WHEN @LangCode = LangCode THEN @LangCode ELSE 'en-US' END )
	;
	
	DROP TABLE #PagePaging;
END
GO
/****** Object:  StoredProcedure [dbo].[Page_Insert]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Page_Insert]	@Title nvarchar(256),	@Description nvarchar(256),	@BodyContent ntext,	@UrlFriendly nvarchar(256),	@CreatedBy nvarchar(128),	@Status int,	@LangCode nvarchar(10)AS
BEGIN
	DECLARE @NewId int = 0;

	INSERT INTO tbl_page(CreatedBy,Status)	VALUES(@CreatedBy,@Status)
	;

	SET @NewId = (SELECT SCOPE_IDENTITY());

	INSERT INTO tbl_page_lang(PageId,Title,Description,BodyContent,UrlFriendly,LangCode)
	VALUES (@NewId, @Title,@Description,@BodyContent,@UrlFriendly,@LangCode)
	;

	SELECT @NewId;
END

GO
/****** Object:  StoredProcedure [dbo].[Page_Update]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Page_Update]	@Id int,	@Title nvarchar(256),	@Description nvarchar(256),	@BodyContent ntext,	@UrlFriendly nvarchar(256),	@Status smallint,	@LangCode nvarchar(10)AS
BEGIN
	BEGIN TRY	BEGIN TRANSACTION
		UPDATE tbl_page
			SET 
				LastUpdated = GETDATE(),
				Status = @Status
		WHERE 1=1
		AND Id = @Id
		;

		IF NOT EXISTS(SELECT TOP 1 PageId FROM tbl_page_lang WHERE 1=1 AND PageId = @Id AND LangCode = @LangCode)
		BEGIN
			INSERT INTO tbl_page_lang(PageId,Title,Description,BodyContent,UrlFriendly,LangCode)
			VALUES (@Id, @Title, @Description, @BodyContent, @UrlFriendly, @LangCode)
			;
		END
		ELSE
		BEGIN
			UPDATE tbl_page_lang 
			SET
				Title = @Title,
				Description = @Description,
				BodyContent = @BodyContent,
				UrlFriendly = @UrlFriendly
			WHERE 1=1
			AND PageId = @Id
			AND LangCode = @LangCode
			;
		END
	COMMIT TRAN	END TRY	BEGIN CATCH	IF @@TRANCOUNT > 0	ROLLBACK TRAN		exec dbo.SQL_WriteLog;	END CATCH
END

GO
/****** Object:  StoredProcedure [dbo].[PageLayout_GetList]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[PageLayout_GetList]
	 
AS
BEGIN
	 SELECT * FROM tbl_page_layout
	 WHERE 1=1
	 AND Status = 1
	 ;
END



GO
/****** Object:  StoredProcedure [dbo].[Post_Delete]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Post_Delete]
	@Id nvarchar(128)	
AS
BEGIN
	BEGIN TRY
    BEGIN TRANSACTION
		UPDATE tbl_post
		SET			
			Status = 9
		WHERE 1=1
		AND Id = @Id
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
/****** Object:  StoredProcedure [dbo].[Post_GetById]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Post_GetById]
	@Id nvarchar(128),
	@LangCode nvarchar(10)
AS
BEGIN
		SELECT TOP 1 * FROM tbl_post WHERE 1=1 AND Id=@Id

		SELECT * FROM tbl_post_lang WHERE 1=1 
		AND PostId = @Id
		AND LangCode = @LangCode
		;
END



GO
/****** Object:  StoredProcedure [dbo].[Post_GetByPage]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Post_GetByPage]
	@Keyword nvarchar(200),
	@Status smallint,
	@CategoryId int,
	@Offset int,
	@PageSize int,
	@LangCode nvarchar(10)
AS
BEGIN
	DECLARE @TotalCount int;

	SET NOCOUNT ON;

	SET @TotalCount = (SELECT COUNT(1) 
		FROM tbl_post a
	 	WHERE 1=1		
		AND (
			@Keyword IS NULL OR a.Title LIKE '%' + @Keyword + '%'
			OR ( SELECT TOP 1 PostId FROM tbl_post_lang b WHERE 1=1 
					AND (b.Title LIKE '%' + @Keyword + '%'
						OR b.Description LIKE '%' + @Keyword + '%'
						OR b.UrlFriendly LIKE '%' + @Keyword + '%'
					)
				) = a.Id
		)
		AND (a.CategoryId = CASE WHEN (@CategoryId = -1 OR @CategoryId IS NULL) THEN a.CategoryId ELSE @CategoryId END)
		--AND (@FromDate IS NULL OR a.CreatedDate >= @FromDate)
		--AND (@ToDate IS NULL OR a.CreatedDate <= @ToDate)		
		AND a.Status = CASE 
			WHEN @Status != -1
				THEN @Status
			ELSE 
				a.Status
			END 
		AND a.Status != 9
	 );

	SELECT *
	INTO #PostPaging
	FROM
	(
		SELECT @TotalCount as TotalCount, a.* FROM tbl_post a
		WHERE 1=1
		AND (
			@Keyword IS NULL OR a.Title LIKE '%' + @Keyword + '%'
			OR ( SELECT TOP 1 PostId FROM tbl_post_lang b WHERE 1=1 
					AND (b.Title LIKE '%' + @Keyword + '%'
						OR b.Description LIKE '%' + @Keyword + '%'
						OR b.UrlFriendly LIKE '%' + @Keyword + '%'
					)
				) = a.Id
		)
		AND (a.CategoryId = CASE WHEN (@CategoryId = -1 OR @CategoryId IS NULL) THEN a.CategoryId ELSE @CategoryId END)

		--AND (@FromDate IS NULL OR a.CreatedDate >= @FromDate)
		--AND (@ToDate IS NULL OR a.CreatedDate <= @ToDate)		
		AND a.Status = CASE 
			WHEN @Status != -1
				THEN @Status
			ELSE 
				a.Status
			END 
		AND a.Status != 9
		ORDER BY CreatedDate DESC
	 	OFFSET @Offset ROWS
	 	FETCH NEXT @PageSize ROWS ONLY
	) as x;
	
	SELECT * FROM  #PostPaging; 
	   
	SELECT * FROM tbl_post_lang a
	WHERE 1=1
	AND a.PostId IN (SELECT Id FROM #PostPaging)	
	AND (@LangCode = LangCode )
	;
	
	DROP TABLE #PostPaging;
END



GO
/****** Object:  StoredProcedure [dbo].[Post_GetByPage_Old]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Post_GetByPage_Old]
	@Keyword nvarchar(200),
	@Status smallint,
	@SortField nvarchar(20),
	@SortType nvarchar(10),
	@Offset int,
	@PageSize int,
	@LangCode nvarchar(10)
AS
BEGIN
	DECLARE @TotalCount int;

	SET NOCOUNT ON;

	SET @TotalCount = (SELECT COUNT(1) 
		FROM tbl_post a
	 	WHERE 1=1		
		AND (
			@Keyword IS NULL OR a.Title LIKE '%' + @Keyword + '%'
			OR ( SELECT TOP 1 PostId FROM tbl_post_lang b WHERE 1=1 
					AND (b.Title LIKE '%' + @Keyword + '%'
						OR b.Description LIKE '%' + @Keyword + '%'
						OR b.UrlFriendly LIKE '%' + @Keyword + '%'
					)
				) = a.Id
		)
		--AND (((a.CategoryId = CASE WHEN (@CategoryId = 0 OR @CategoryId = -1) THEN a.CategoryId ELSE @CategoryId END)))
		--AND (@FromDate IS NULL OR a.CreatedDate >= @FromDate)
		--AND (@ToDate IS NULL OR a.CreatedDate <= @ToDate)		
		AND a.Status = CASE 
			WHEN @Status != -1
				THEN @Status
			ELSE 
				a.Status
			END 
		AND a.Status != 9
	 );

	SELECT *
	INTO #PostPaging
	FROM
	(
		SELECT @TotalCount as TotalCount, a.* FROM tbl_post a
		WHERE 1=1
		AND (
			@Keyword IS NULL OR a.Title LIKE '%' + @Keyword + '%'
			OR ( SELECT TOP 1 PostId FROM tbl_post_lang b WHERE 1=1 
					AND (b.Title LIKE '%' + @Keyword + '%'
						OR b.Description LIKE '%' + @Keyword + '%'
						OR b.UrlFriendly LIKE '%' + @Keyword + '%'
					)
				) = a.Id
		)
		--AND (((a.CategoryId = CASE WHEN (@CategoryId = 0 OR @CategoryId = -1) THEN a.CategoryId ELSE @CategoryId END)))
		--AND (@FromDate IS NULL OR a.CreatedDate >= @FromDate)
		--AND (@ToDate IS NULL OR a.CreatedDate <= @ToDate)		
		AND a.Status = CASE 
			WHEN @Status != -1
				THEN @Status
			ELSE 
				a.Status
			END 
		AND a.Status != 9
		ORDER BY 
			CASE WHEN @SortField = 'Id' AND @SortType = 'asc' THEN a.Id END ASC,
			CASE WHEN @SortField = 'Id' AND @SortType = 'desc' THEN a.Id END DESC,

			CASE WHEN @SortField = 'CreatedDate' AND @SortType = 'asc' THEN a.CreatedDate END ASC,
			CASE WHEN @SortField = 'CreatedDate' AND @SortType = 'desc' THEN a.CreatedDate END DESC,

			CASE WHEN @SortField = 'Status' AND @SortType = 'asc' THEN a.Status END ASC,
			CASE WHEN @SortField = 'Status' AND @SortType = 'desc' THEN a.Status END DESC,

			CASE WHEN @SortField = 'IsHighlights' AND @SortType = 'asc' THEN a.IsHighlights END ASC,
			CASE WHEN @SortField = 'IsHighlights' AND @SortType = 'desc' THEN a.IsHighlights END DESC
	 	OFFSET @Offset ROWS
	 	FETCH NEXT @PageSize ROWS ONLY
	) as x;
	
	SELECT * FROM  #PostPaging; 
	   
	SELECT * FROM tbl_post_lang a
	WHERE 1=1
	AND a.PostId IN (SELECT Id FROM #PostPaging)	
	AND (@LangCode = LangCode )
	;
	
	DROP TABLE #PostPaging;
END



GO
/****** Object:  StoredProcedure [dbo].[Post_Insert]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Post_Insert]	@Title nvarchar(256),	@Description nvarchar(256),	@BodyContent ntext,	@IsHighlights bit,	@Cover nvarchar(500),	@UrlFriendly nvarchar(256),	@CategoryId int,	@CreatedBy nvarchar(128),	@Status int,	@LangCode nvarchar(10)AS
BEGIN
	DECLARE @NewId int = 0;

	INSERT INTO tbl_post(Title,IsHighlights,Cover,CategoryId,CreatedBy,Status)	VALUES(@Title,@IsHighlights,@Cover,@CategoryId,@CreatedBy,@Status)
	;

	SET @NewId = (SELECT SCOPE_IDENTITY());

	INSERT INTO tbl_post_lang(PostId,Title,Description,BodyContent,LangCode,UrlFriendly)
	VALUES (@NewId, @Title,@Description,@BodyContent,@LangCode,@UrlFriendly)
	;

	SELECT @NewId;
END

GO
/****** Object:  StoredProcedure [dbo].[Post_Update]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Post_Update]	@Id int,	@Title nvarchar(256),	@Description nvarchar(256),	@BodyContent ntext,	@IsHighlights bit,	@Cover nvarchar(500),	@UrlFriendly nvarchar(256),	@CategoryId int,	@Status smallint,	@LangCode nvarchar(10)AS
BEGIN
	BEGIN TRY	BEGIN TRANSACTION
		UPDATE tbl_post
			SET 
				IsHighlights = @IsHighlights,
				Cover = @Cover,				
				CategoryId = @CategoryId,
				Status = @Status
		WHERE 1=1
		AND Id = @Id
		;

		IF NOT EXISTS(SELECT TOP 1 PostId FROM tbl_post_lang WHERE 1=1 AND PostId = @Id AND LangCode = @LangCode)
		BEGIN
			INSERT INTO tbl_post_lang(PostId,Title,Description,BodyContent,LangCode,UrlFriendly)
			VALUES (@Id, @Title,@Description,@BodyContent,@LangCode,@UrlFriendly)
			;
		END
		ELSE
		BEGIN
			UPDATE tbl_post_lang 
			SET
				Title = @Title,
				Description = @Description,
				BodyContent = @BodyContent,
				UrlFriendly = @UrlFriendly
			WHERE 1=1
			AND PostId = @Id
			AND LangCode = @LangCode
			;
		END
	COMMIT TRAN	END TRY	BEGIN CATCH	IF @@TRANCOUNT > 0	ROLLBACK TRAN		exec dbo.SQL_WriteLog;	END CATCH
END

GO
/****** Object:  StoredProcedure [dbo].[Product_Delete]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Product_Delete]
	 @Id int
AS
BEGIN
	 UPDATE tbl_product
		SET Status = 9
	 WHERE 1=1
	 AND Id = @Id
	 ;
END


GO
/****** Object:  StoredProcedure [dbo].[Product_GetActiveForChoosen]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Product_GetActiveForChoosen]
	 @Keyword nvarchar(128),
	 --@ProviderId int,
	 --@ProductCategoryId int,
	 @PropertyCategoryId int,
	 @PropertyList nvarchar(MAX),
	 @Offset int,
	 @PageSize int
AS
BEGIN
	 DECLARE @TotalCount int;
	 SET @TotalCount = (SELECT COUNT(1) FROM tbl_product a
	 	 WHERE 1=1	 	 
		 AND (
			@Keyword IS NULL 
			OR a.Name LIKE Concat('%',@Keyword,'%')
			OR a.Code LIKE Concat('%',@Keyword,'%')
		 )
		 --AND (a.ProviderId = CASE WHEN (@ProviderId = 0 OR @ProviderId IS NULL) THEN a.ProviderId ELSE @ProviderId END)
		 --AND (a.ProductCategoryId = CASE WHEN (@ProductCategoryId = 0 OR @ProductCategoryId IS NULL) THEN a.ProductCategoryId ELSE @ProductCategoryId END)
		 AND (@PropertyCategoryId = CASE WHEN (@PropertyCategoryId = 0 OR @PropertyCategoryId IS NULL) THEN  @PropertyCategoryId ELSE (SELECT TOP 1 PropertyCategoryId FROM tbl_product_property WHERE 1=1 AND PropertyCategoryId = @PropertyCategoryId AND ProductId = a.Id) END)
		 AND (
			@PropertyList IS NULL OR @PropertyList = '' 
			OR a.Id IN (SELECT ProductId FROM tbl_product_property WHERE PropertyId IN (SELECT * FROM dbo.fnStringList2Table(@PropertyList)))
		)
		 AND a.Status = 1
	 );

	 SELECT @TotalCount as TotalCount, a.*, (SELECT dbo.F_Product_GetWarehouseNum(Id)) as WarehouseNum
	 	 FROM tbl_product a
	 	 WHERE 1=1
		 AND (
			@Keyword IS NULL 
			OR a.Name LIKE Concat('%',@Keyword,'%')
			OR a.Code LIKE Concat('%',@Keyword,'%')
		 )
		 --AND (a.ProviderId = CASE WHEN (@ProviderId = 0 OR @ProviderId IS NULL) THEN a.ProviderId ELSE @ProviderId END)
		 --AND (a.ProductCategoryId = CASE WHEN (@ProductCategoryId = 0 OR @ProductCategoryId IS NULL) THEN a.ProductCategoryId ELSE @ProductCategoryId END)
		 AND (@PropertyCategoryId = CASE WHEN (@PropertyCategoryId = 0 OR @PropertyCategoryId IS NULL) THEN  @PropertyCategoryId ELSE (SELECT TOP 1 PropertyCategoryId FROM tbl_product_property WHERE 1=1 AND PropertyCategoryId = @PropertyCategoryId AND ProductId = a.Id) END)
		 AND (
			@PropertyList IS NULL OR @PropertyList = '' 
			OR a.Id IN (SELECT ProductId FROM tbl_product_property WHERE PropertyId IN (SELECT * FROM dbo.fnStringList2Table(@PropertyList)))
		)

		 AND a.Status = 1
	 	 ORDER BY a.Code ASC
	 	 OFFSET @Offset ROWS
	 	 FETCH NEXT @PageSize ROWS ONLY
	 ;
END


GO
/****** Object:  StoredProcedure [dbo].[Product_GetByCode]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Product_GetByCode]
	 @Code nvarchar(128)
AS
BEGIN
	 SELECT TOP 1 * FROM tbl_product
	 WHERE 1=1
	 AND Code = @Code
	 AND Status = 1
	 ;
END


GO
/****** Object:  StoredProcedure [dbo].[Product_GetById]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Product_GetById]
	 @Id int
AS
BEGIN
	 SELECT * FROM tbl_product
	 WHERE 1=1
	 AND Id = @Id
	 ;

	 --Properties
	 SELECT * FROM tbl_product_property a WHERE 1=1
	 AND ProductId = @Id
	 AND (SELECT TOP 1 Status FROM tbl_property WHERE 1=1 AND a.PropertyCategoryId = Id) = 1
	 ;
END


GO
/****** Object:  StoredProcedure [dbo].[Product_GetByListIds]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Product_GetByListIds]
	 @ListIds nvarchar(MAX)
AS
BEGIN
	 SELECT a.*, (SELECT dbo.F_Product_GetWarehouseNum(a.Id)) as WarehouseNum
	 FROM tbl_product a WHERE 1=1
	 AND a.Id IN (SELECT * FROM dbo.fnStringList2Table(@ListIds))
	 ;	
END


GO
/****** Object:  StoredProcedure [dbo].[Product_GetByPage]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Product_GetByPage]
	 @Keyword nvarchar(128),
	 @Status int,
	 --@ProviderId int,
	 --@ProductCategoryId int,
	 @PropertyCategoryId int,
	 @PropertyList nvarchar(MAX),
	 @Offset int,
	 @PageSize int
AS
BEGIN
	 DECLARE @TotalCount int;
	 DECLARE @PropertyCatCount int = 0;

	 SET @PropertyCatCount = (SELECT COUNT(1) FROM STRING_SPLIT(@PropertyList,'#') WHERE value IS NOT NULL AND value <> '');

	 SET @TotalCount = (SELECT COUNT(1) FROM tbl_product a
	 	 WHERE 1=1
		 AND (
			@Keyword IS NULL 
			OR a.Name LIKE Concat('%',@Keyword,'%')
			OR a.Code LIKE Concat('%',@Keyword,'%')
		 )
	 	 AND (a.Status = CASE WHEN (@Status = -1 OR @Status IS NULL) THEN a.Status ELSE @Status END)
		 --AND (a.ProviderId = CASE WHEN (@ProviderId = 0 OR @ProviderId IS NULL) THEN a.ProviderId ELSE @ProviderId END)
		 --AND (a.ProductCategoryId = CASE WHEN (@ProductCategoryId = 0 OR @ProductCategoryId IS NULL) THEN a.ProductCategoryId ELSE @ProductCategoryId END)
		 --AND (@PropertyCategoryId = CASE WHEN (@PropertyCategoryId = 0 OR @PropertyCategoryId IS NULL) THEN  @PropertyCategoryId ELSE (SELECT TOP 1 PropertyCategoryId FROM tbl_product_property WHERE 1=1 AND PropertyCategoryId = @PropertyCategoryId AND ProductId = a.Id) END)
		 AND (
			(@PropertyList IS NULL OR @PropertyList = '')
			--OR a.Id IN (SELECT ProductId FROM tbl_product_property WHERE PropertyId IN (SELECT * FROM dbo.fnStringList2Table(@PropertyList)))
			--OR (SELECT dbo.F_Product_IsIncludedProperties(a.Id,@PropertyList)) = 1
			OR (SELECT dbo.F_Product_IsIncludedProperties(a.Id,@PropertyList)) = @PropertyCatCount
		)

		 AND a.Status != 9
	 );

	 SELECT @TotalCount as TotalCount, a.*, (SELECT dbo.F_Product_GetWarehouseNum(a.Id)) as WarehouseNum
	 	 FROM tbl_product a
	 	 WHERE 1=1
		 AND (
			@Keyword IS NULL 
			OR a.Name LIKE Concat('%',@Keyword,'%')
			OR a.Code LIKE Concat('%',@Keyword,'%')
		 )
	 	 AND (a.Status = CASE WHEN (@Status = -1 OR @Status IS NULL) THEN a.Status ELSE @Status END)
		 --AND (a.ProviderId = CASE WHEN (@ProviderId = 0 OR @ProviderId IS NULL) THEN a.ProviderId ELSE @ProviderId END)
		 --AND (a.ProductCategoryId = CASE WHEN (@ProductCategoryId = 0 OR @ProductCategoryId IS NULL) THEN a.ProductCategoryId ELSE @ProductCategoryId END)
		 --AND (@PropertyCategoryId = CASE WHEN (@PropertyCategoryId = 0 OR @PropertyCategoryId IS NULL) THEN  @PropertyCategoryId ELSE (SELECT TOP 1 PropertyCategoryId FROM tbl_product_property WHERE 1=1 AND PropertyCategoryId = @PropertyCategoryId AND ProductId = a.Id) END)
		 AND (
			(@PropertyList IS NULL OR @PropertyList = '')
			--OR  a.Id IN (SELECT ProductId FROM tbl_product_property WHERE PropertyId IN (SELECT * FROM dbo.fnStringList2Table(@PropertyList)))
			--OR (SELECT dbo.F_Product_IsIncludedProperties(a.Id,@PropertyList)) = 1
			OR (SELECT dbo.F_Product_IsIncludedProperties(a.Id,@PropertyList)) = @PropertyCatCount
		)
		 AND a.Status != 9
	 	 ORDER BY a.Code ASC
	 	 OFFSET @Offset ROWS
	 	 FETCH NEXT @PageSize ROWS ONLY
	 ;
END


GO
/****** Object:  StoredProcedure [dbo].[Product_GetDetail]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Product_GetDetail]
	 @Id int
AS
BEGIN
	SELECT *, (SELECT dbo.F_Product_GetWarehouseNum(Id)) as WarehouseNum
	FROM tbl_product 
	WHERE 1=1
	 AND Id = @Id
	 AND Status = 1
	 ;
END


GO
/****** Object:  StoredProcedure [dbo].[Product_GetListNeedReflectByPage]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Product_GetListNeedReflectByPage]
	 @Keyword nvarchar(256),
	 @PropertyCategoryId int,
	 @PropertyList nvarchar(MAX),
	 @Offset int,
	 @PageSize int
AS
BEGIN
	DECLARE @TotalCount int;
	
	SET NOCOUNT ON;
	 
	 SET @TotalCount = (
	 SELECT COUNT(1) FROM tbl_warehouse a
	 	WHERE 1=1	 	
		AND a.ProductId IN (
			SELECT b.Id
			FROM tbl_product b WHERE 1=1
			AND (@Keyword IS NULL OR b.Code LIKE '%' + @Keyword + '%' OR  b.Name LIKE '%' + @Keyword + '%')			
		)	 
		AND a.Reflected = 0		
	 );

	SELECT *
	INTO #WarehousePaging
	FROM
	(
		SELECT a.*
	 	FROM tbl_warehouse a
	 	WHERE 1=1
		AND a.ProductId IN (
			SELECT b.Id
			FROM tbl_product b WHERE 1=1
			AND (@Keyword IS NULL OR b.Code LIKE '%' + @Keyword + '%' OR  b.Name LIKE '%' + @Keyword + '%')			
		)	
	 	 --AND (a.Status = CASE WHEN (@Status = -1 OR @Status IS NULL) THEN a.Status ELSE @Status END)
		 --AND (@Keyword IS NULL OR a.Name LIKE '%' + @Keyword + '%' OR a.Code LIKE '%' + @Keyword + '%')
		 --AND a.Status != 9
		 AND a.Reflected = 0			
	 	 ORDER BY a.Id DESC
	 	 OFFSET @Offset ROWS
	 	 FETCH NEXT @PageSize ROWS ONLY
	) as x;
	
	SELECT @TotalCount as TotalCount, *, (SELECT dbo.F_Product_GetWarehouseNum(Id)) as WarehouseNum
	FROM tbl_product a WHERE 1=1
	AND Id IN (SELECT ProductId FROM #WarehousePaging)	
	AND Status = 1
	ORDER BY a.Code ASC
	;

	SELECT * FROM  #WarehousePaging;
	
	DROP TABLE #WarehousePaging;
END



GO
/****** Object:  StoredProcedure [dbo].[Product_Insert]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Product_Insert]
	@Code nvarchar(20),
	@ProductCategoryId int,
	@ProviderId int,
	@Name nvarchar(256),
	--@ShortDescription nvarchar(256),
	--@Detail ntext,
	--@OtherInfo nvarchar(1000),
	--@Cost float,
	--@SaleOffCost float,
	--@UnitId int,
	--@CurrencyId int,
	@MinInventory float,
	@UnitId int,
	@CreatedBy int,
	--@CreatedDate datetime,
	--@LastUpdatedBy nchar(10),
	--@LastUpdated datetime,
	@Status smallint
AS
BEGIN
	 INSERT INTO tbl_product(Code,ProductCategoryId,ProviderId,Name,MinInventory,UnitId,CreatedBy,Status) 
	 VALUES(@Code,@ProductCategoryId,@ProviderId,@Name,@MinInventory,@UnitId,@CreatedBy,@Status);

	 SELECT SCOPE_IDENTITY();
END


GO
/****** Object:  StoredProcedure [dbo].[Product_Update]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Product_Update]
	@Id int,
	@Code nvarchar(20),
	@ProductCategoryId int,
	@ProviderId int,
	@Name nvarchar(256),
	--@ShortDescription nvarchar(256),
	--@Detail ntext,
	--@OtherInfo nvarchar(1000),
	--@Cost float,
	--@SaleOffCost float,
	--@UnitId int,
	--@CurrencyId int,
	@MinInventory float,
	@UnitId int,
	@LastUpdatedBy nchar(10),
	@Status smallint
AS
BEGIN
	 UPDATE tbl_product
	 SET 
	 	 Code= @Code,
	 	 ProductCategoryId= @ProductCategoryId,
	 	 ProviderId= @ProviderId,
	 	 Name= @Name,
	 	 --ShortDescription= @ShortDescription,
	 	 --Detail= @Detail,
	 	 --OtherInfo= @OtherInfo,
	 	 --Cost= @Cost,
	 	 --SaleOffCost= @SaleOffCost,
	 	 --CurrencyId= @CurrencyId,
		 UnitId= @UnitId,
		 MinInventory = @MinInventory ,
	 	 LastUpdatedBy= @LastUpdatedBy,
	 	 LastUpdated= GETDATE(),
	 	 Status= @Status
	 WHERE 1=1
	 AND Id = @Id
	 ;
END


GO
/****** Object:  StoredProcedure [dbo].[ProductCategory_Delete]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[ProductCategory_Delete]
	 @Id int
AS
BEGIN
	 UPDATE tbl_product_category
		SET Status = 9
	 WHERE 1=1
	 AND Id = @Id
	 ;
END



GO
/****** Object:  StoredProcedure [dbo].[ProductCategory_GetById]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[ProductCategory_GetById]
	 @Id int
AS
BEGIN
	 SELECT * FROM tbl_product_category
	 WHERE 1=1
	 AND Id = @Id
	 AND Status != 9
	 ;
END



GO
/****** Object:  StoredProcedure [dbo].[ProductCategory_GetByPage]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[ProductCategory_GetByPage]
	 @Keyword nvarchar(128),
	 @Status int,
	 @Offset int,
	 @PageSize int
AS
BEGIN
	 SET NOCOUNT ON;

	 DECLARE @TotalCount int;
	 SET @TotalCount = (SELECT COUNT(1) FROM tbl_product_category a
	 	 WHERE 1=1
	 	 AND (a.Status = CASE WHEN (@Status = -1 OR @Status IS NULL) THEN a.Status ELSE @Status END)
		 AND (@Keyword IS NULL OR a.Name LIKE '%' + @Keyword + '%' OR a.Name LIKE '%' + @Keyword + '%')
		 AND a.Status != 9
	 );

	 SELECT @TotalCount as TotalCount, a.*
	 	 FROM tbl_product_category a
	 	 WHERE 1=1
	 	 AND (a.Status = CASE WHEN (@Status = -1 OR @Status IS NULL) THEN a.Status ELSE @Status END)
		 AND (@Keyword IS NULL OR a.Name LIKE '%' + @Keyword + '%' OR a.Name LIKE '%' + @Keyword + '%')
		 AND a.Status != 9
	 	 ORDER BY a.Id DESC
	 	 OFFSET @Offset ROWS
	 	 FETCH NEXT @PageSize ROWS ONLY
	 ;
END



GO
/****** Object:  StoredProcedure [dbo].[ProductCategory_GetList]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[ProductCategory_GetList]
	 
AS
BEGIN
	 SELECT * FROM tbl_product_category
	 WHERE 1=1
	 AND Status = 1
	 ;
END



GO
/****** Object:  StoredProcedure [dbo].[ProductCategory_Insert]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ProductCategory_Insert]
	@Code nvarchar(20),
	@Name nvarchar(128),
	@Status smallint
AS
BEGIN
	 INSERT INTO tbl_product_category(Code,Name,Status) 
	 VALUES(@Code,@Name,@Status);

	 SELECT SCOPE_IDENTITY();
END



GO
/****** Object:  StoredProcedure [dbo].[ProductCategory_Update]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[ProductCategory_Update]
	@Id int,
	@Code nvarchar(20),
	@Name nvarchar(128),
	@Status smallint
AS
BEGIN
	 UPDATE tbl_product_category
	 SET 
	 	 Code= @Code,
	 	 Name= @Name,
	 	 Status= @Status
	 WHERE 1=1
	 AND Id = @Id
	 ;
END



GO
/****** Object:  StoredProcedure [dbo].[Project_AddNewImage]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Project_AddNewImage]
	@Id nvarchar(128),
	@ProjectId int,
	@Name nvarchar(128),
	@Url nvarchar(500)
AS
BEGIN
	INSERT INTO tbl_project_image(Id, ProjectId, Name, Url, CreatedDate)
		VALUES(@Id, @ProjectId, @Name, @Url, GETDATE())
	;
END
GO
/****** Object:  StoredProcedure [dbo].[Project_Delete]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Project_Delete]
	@Id nvarchar(128)	
AS
BEGIN
	BEGIN TRY
    BEGIN TRANSACTION
		UPDATE tbl_project
		SET			
			Status = 9
		WHERE 1=1
		AND Id = @Id
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
/****** Object:  StoredProcedure [dbo].[Project_GetById]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Project_GetById]
	@Id nvarchar(128),
	@LangCode nvarchar(10)
AS
BEGIN
		SELECT TOP 1 * FROM tbl_project WHERE 1=1 AND Id=@Id

		SELECT * FROM tbl_project_lang WHERE 1=1 
		AND ProjectId = @Id
		AND LangCode = @LangCode
		;

		SELECT * FROM tbl_project_image WHERE 1=1 AND ProjectId = @Id
		ORDER BY CreatedDate DESC
		;
END



GO
/****** Object:  StoredProcedure [dbo].[Project_GetByPage]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Project_GetByPage]
	 @Keyword nvarchar(128),
	 @PropertyList nvarchar(MAX),
	 @CategoryId int,
	 @LangCode nvarchar(20),
	 @Status smallint,
	 @Offset int,
	 @PageSize int
AS
BEGIN
	DECLARE @TotalCount int;
	DECLARE @PropertyCatCount int = 0;

	SET NOCOUNT ON;

	SET @PropertyCatCount = (SELECT COUNT(1) FROM STRING_SPLIT(@PropertyList,'#') WHERE value IS NOT NULL AND value <> '');
	 
	 SET @TotalCount = (SELECT COUNT(1) 
		FROM tbl_project a
	 	WHERE 1=1
		
		AND (
			@Keyword IS NULL OR a.Title LIKE '%' + @Keyword + '%'
			OR ( SELECT TOP 1 ProjectId FROM tbl_project_lang b WHERE 1=1 
					AND (b.Title LIKE '%' + @Keyword + '%'
						OR b.Description LIKE '%' + @Keyword + '%'
						OR b.UrlFriendly LIKE '%' + @Keyword + '%'
					)
				) = a.Id
		)

		AND (a.Status = CASE WHEN (@Status = -1 OR @Status IS NULL) THEN a.Status ELSE @Status END)
		AND (a.CategoryId = CASE WHEN (@CategoryId = -1 OR @CategoryId IS NULL) THEN a.CategoryId ELSE @CategoryId END)
		AND (
			(@PropertyList IS NULL OR @PropertyList = '')
			--OR b.Id IN (SELECT ProductId FROM tbl_product_property WHERE PropertyId IN (SELECT * FROM dbo.fnStringList2Table(@PropertyList)))
			--OR (SELECT dbo.F_Product_IsIncludedProperties(b.Id,@PropertyList)) = 1
			--OR (SELECT dbo.F_Product_IsIncludedProperties(b.Id,@PropertyList)) = @PropertyCatCount
		)
	 );

	SELECT *
	INTO #ProjectPaging
	FROM
	(
		SELECT @TotalCount as TotalCount, a.*
	 	FROM tbl_project a
	 	WHERE 1=1		
		AND (
			@Keyword IS NULL OR a.Title LIKE '%' + @Keyword + '%'
			OR ( SELECT TOP 1 ProjectId FROM tbl_project_lang b WHERE 1=1 
					AND (b.Title LIKE '%' + @Keyword + '%'
						OR b.Description LIKE '%' + @Keyword + '%'
						OR b.UrlFriendly LIKE '%' + @Keyword + '%'
					)
				) = a.Id
		)
		AND (a.Status = CASE WHEN (@Status = -1 OR @Status IS NULL) THEN a.Status ELSE @Status END)
		AND (a.CategoryId = CASE WHEN (@CategoryId = -1 OR @CategoryId IS NULL) THEN a.CategoryId ELSE @CategoryId END)
		--AND (@PropertyCategoryId = CASE WHEN (@PropertyCategoryId = 0 OR @PropertyCategoryId IS NULL) THEN  @PropertyCategoryId ELSE (SELECT TOP 1 PropertyCategoryId FROM tbl_product_property WHERE 1=1 AND PropertyCategoryId = @PropertyCategoryId AND ProductId = b.Id) END)
		AND (
			(@PropertyList IS NULL OR @PropertyList = '')
			--OR b.Id IN (SELECT ProductId FROM tbl_product_property WHERE PropertyId IN (SELECT * FROM dbo.fnStringList2Table(@PropertyList)))
			--OR (SELECT dbo.F_Product_IsIncludedProperties(b.Id,@PropertyList)) = 1
			--OR (SELECT dbo.F_Product_IsIncludedProperties(b.Id,@PropertyList)) = @PropertyCatCount
		)

	 	 ORDER BY a.CreatedDate DESC
	 	 OFFSET @Offset ROWS
	 	 FETCH NEXT @PageSize ROWS ONLY
	) as x;
	
	SELECT * FROM  #ProjectPaging; 
	   
	SELECT * FROM tbl_project_lang a
	WHERE 1=1
	AND a.ProjectId IN (SELECT Id FROM #ProjectPaging)	
	AND (LangCode = @LangCode)
	;
	
	DROP TABLE #ProjectPaging;
	
END



GO
/****** Object:  StoredProcedure [dbo].[Project_GetDetail]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Project_GetDetail]
	@Id nvarchar(128)
AS
BEGIN
		SELECT TOP 1 * FROM tbl_project WHERE 1=1 AND Id=@Id

		SELECT * FROM tbl_project_lang WHERE 1=1 
		AND ProjectId = @Id
		;

		SELECT * FROM tbl_project_image WHERE 1=1 AND ProjectId = @Id
		ORDER BY CreatedDate DESC
		;
END



GO
/****** Object:  StoredProcedure [dbo].[Project_GetDetailById]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Project_GetDetailById]
	@Id nvarchar(128)
AS
BEGIN
		SELECT TOP 1 * FROM tbl_project WHERE 1=1 AND Id=@Id

		SELECT * FROM tbl_project_lang WHERE 1=1 
		AND ProjectId = @Id
		;

		SELECT * FROM tbl_project_image WHERE 1=1 AND ProjectId = @Id
		ORDER BY CreatedDate DESC
		;
END



GO
/****** Object:  StoredProcedure [dbo].[Project_GetListImage]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Project_GetListImage]
	@ProjectId int
AS
BEGIN
	SELECT * FROM tbl_project_image WHERE 1=1
	AND ProjectId = @ProjectId
	ORDER BY CreatedDate DESC
	;
END
GO
/****** Object:  StoredProcedure [dbo].[Project_Insert]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Project_Insert]	@Title nvarchar(256),	@Description nvarchar(256),	@BodyContent ntext,	@Cover nvarchar(500),	@UrlFriendly nvarchar(256),	@CategoryId int,	@CreatedBy nvarchar(128),	@Status int,	@LangCode nvarchar(10),	@MetaData nvarchar(max)AS
BEGIN
	DECLARE @NewId int = 0;

	INSERT INTO tbl_project(Title,Cover,CategoryId,CreatedBy,Status,MetaData)	VALUES(@Title,@Cover,@CategoryId,@CreatedBy,@Status,@MetaData)
	;

	SET @NewId = (SELECT SCOPE_IDENTITY());

	INSERT INTO tbl_project_lang(ProjectId,Title,Description,BodyContent,LangCode,UrlFriendly)
	VALUES (@NewId, @Title,@Description,@BodyContent,@LangCode,@UrlFriendly)
	;

	SELECT @NewId;
END

GO
/****** Object:  StoredProcedure [dbo].[Project_RemoveImage]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[Project_RemoveImage]
@Id nvarchar(128)
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY
    BEGIN TRANSACTION
		DELETE FROM tbl_project_image WHERE 1=1
		AND Id = @Id;
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
/****** Object:  StoredProcedure [dbo].[Project_Update]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Project_Update]	@Id int,	@Title nvarchar(256),	@Description nvarchar(256),	@BodyContent ntext,	@Cover nvarchar(500),	@UrlFriendly nvarchar(256),	@CategoryId int,	@Status smallint,	@LangCode nvarchar(10),	@MetaData nvarchar(max)AS
BEGIN
	BEGIN TRY	BEGIN TRANSACTION
		UPDATE tbl_project
			SET 
				Cover = @Cover,				
				CategoryId = @CategoryId,
				Status = @Status,
				MetaData = @MetaData
		WHERE 1=1
		AND Id = @Id
		;

		IF NOT EXISTS(SELECT TOP 1 ProjectId FROM tbl_project_lang WHERE 1=1 AND ProjectId = @Id AND LangCode = @LangCode)
		BEGIN
			INSERT INTO tbl_project_lang(ProjectId,Title,Description,BodyContent,LangCode,UrlFriendly)
			VALUES (@Id, @Title,@Description,@BodyContent,@LangCode,@UrlFriendly)
			;
		END
		ELSE
		BEGIN
			UPDATE tbl_project_lang 
			SET
				Title = @Title,
				Description = @Description,
				BodyContent = @BodyContent,
				UrlFriendly = @UrlFriendly
			WHERE 1=1
			AND ProjectId = @Id
			AND LangCode = @LangCode
			;
		END
	COMMIT TRAN	END TRY	BEGIN CATCH	IF @@TRANCOUNT > 0	ROLLBACK TRAN		exec dbo.SQL_WriteLog;	END CATCH
END

GO
/****** Object:  StoredProcedure [dbo].[ProjectCategory_Delete]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[ProjectCategory_Delete]
	@Id nvarchar(128)	
AS
BEGIN
	BEGIN TRY
    BEGIN TRANSACTION
		UPDATE tbl_project_category
		SET			
			Status = 9
		WHERE 1=1
		AND Id = @Id
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
/****** Object:  StoredProcedure [dbo].[ProjectCategory_GetById]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[ProjectCategory_GetById]
	@Id nvarchar(128),
	@LangCode nvarchar(10)
AS
BEGIN
		SELECT TOP 1 * FROM tbl_project_category WHERE 1=1 AND Id=@Id

		SELECT * FROM tbl_project_category_lang WHERE 1=1 
		AND ProjectCategoryId = @Id
		AND LangCode = @LangCode
		;
END



GO
/****** Object:  StoredProcedure [dbo].[ProjectCategory_GetByPage]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[ProjectCategory_GetByPage]
	 @Keyword nvarchar(128),
	 @LangCode nvarchar(20),
	 @Status smallint,
	 @Offset int,
	 @PageSize int
AS
BEGIN
	DECLARE @TotalCount int;
	SET NOCOUNT ON;
	 
	 SET @TotalCount = (SELECT COUNT(1) 
		FROM tbl_project_category a
	 	WHERE 1=1
		--AND (@Keyword IS NULL OR a.Title LIKE '%' + @Keyword + '%')
		AND (
			@Keyword IS NULL OR a.Name LIKE '%' + @Keyword + '%'
			OR ( SELECT TOP 1 b.ProjectCategoryId FROM tbl_project_category_lang b WHERE 1=1 
					AND (b.Name LIKE '%' + @Keyword + '%'
						OR b.Description LIKE '%' + @Keyword + '%'
						OR b.UrlFriendly LIKE '%' + @Keyword + '%'
					)
				) = a.Id
		)
		AND (a.Status = CASE WHEN (@Status = -1 OR @Status IS NULL) THEN a.Status ELSE @Status END)	
		AND a.Status != 9	
	 );

	SELECT *
	INTO #ProjectCategoryPaging
	FROM
	(
		SELECT @TotalCount as TotalCount, a.*
	 	FROM tbl_project_category a
	 	WHERE 1=1				
		AND (
			@Keyword IS NULL OR a.Name LIKE '%' + @Keyword + '%'
			OR ( SELECT TOP 1 b.ProjectCategoryId FROM tbl_project_category_lang b WHERE 1=1 
					AND (b.Name LIKE '%' + @Keyword + '%'
						OR b.Description LIKE '%' + @Keyword + '%'
						OR b.UrlFriendly LIKE '%' + @Keyword + '%'
					)
				) = a.Id
		)

		AND (a.Status = CASE WHEN (@Status = -1 OR @Status IS NULL) THEN a.Status ELSE @Status END)		
		AND a.Status != 9
	 	 ORDER BY a.CreatedDate DESC
	 	 OFFSET @Offset ROWS
	 	 FETCH NEXT @PageSize ROWS ONLY
	) as x;
	
	SELECT * FROM  #ProjectCategoryPaging; 
	   
	SELECT * FROM tbl_project_category_lang a
	WHERE 1=1
	AND a.ProjectCategoryId IN (SELECT Id FROM #ProjectCategoryPaging)	
	AND (LangCode = @LangCode)
	;
	
	DROP TABLE #ProjectCategoryPaging;
	
END



GO
/****** Object:  StoredProcedure [dbo].[ProjectCategory_GetList]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[ProjectCategory_GetList]
	
AS
BEGIN
	SET NOCOUNT ON;	 
	
	SELECT *
	INTO #ProjectCategoryListPaging
	FROM
	(
		SELECT a.*
	 	FROM tbl_project_category a
	 	WHERE 1=1
		AND Status = 1
	) as x;
	
	SELECT * FROM  #ProjectCategoryListPaging; 
	   
	SELECT * FROM tbl_project_category_lang a
	WHERE 1=1
	AND a.ProjectCategoryId IN (SELECT Id FROM #ProjectCategoryListPaging)	
	;
	
	DROP TABLE #ProjectCategoryListPaging;
	
END



GO
/****** Object:  StoredProcedure [dbo].[ProjectCategory_Insert]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ProjectCategory_Insert]	@Name nvarchar(256),	@Description nvarchar(256),	@Cover nvarchar(500),	@UrlFriendly nvarchar(256),	@ParentId int,	@CreatedBy nvarchar(128),	@Status int,	@LangCode nvarchar(10)AS
BEGIN
	DECLARE @NewId int = 0;

	INSERT INTO tbl_project_category(Name,Cover,ParentId,CreatedBy,Status)	VALUES(@Name,@Cover,@ParentId,@CreatedBy,@Status)
	;

	SET @NewId = (SELECT SCOPE_IDENTITY());

	INSERT INTO tbl_project_category_lang(ProjectCategoryId,Name,Description,LangCode,UrlFriendly)
	VALUES (@NewId, @Name,@Description,@LangCode,@UrlFriendly)
	;

	SELECT @NewId;
END

GO
/****** Object:  StoredProcedure [dbo].[ProjectCategory_Update]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ProjectCategory_Update]	@Id int,	@Name nvarchar(256),	@Description nvarchar(256),	@Cover nvarchar(500),	@UrlFriendly nvarchar(256),	@ParentId int,	@Status smallint,	@LangCode nvarchar(10)AS
BEGIN
	BEGIN TRY	BEGIN TRANSACTION
		UPDATE tbl_project_category
			SET 
				Cover = @Cover,				
				ParentId = @ParentId,
				Status = @Status
		WHERE 1=1
		AND Id = @Id
		;

		IF NOT EXISTS(SELECT TOP 1 ProjectCategoryId FROM tbl_project_category_lang WHERE 1=1 AND ProjectCategoryId = @Id AND LangCode = @LangCode)
		BEGIN
			INSERT INTO tbl_project_category_lang(ProjectCategoryId,Name,Description,LangCode,UrlFriendly)
			VALUES (@Id, @Name,@Description,@LangCode,@UrlFriendly)
			;
		END
		ELSE
		BEGIN
			UPDATE tbl_project_category_lang 
			SET
				Name = @Name,
				Description = @Description,
				UrlFriendly = @UrlFriendly
			WHERE 1=1
			AND ProjectCategoryId = @Id
			AND LangCode = @LangCode
			;
		END
	COMMIT TRAN	END TRY	BEGIN CATCH	IF @@TRANCOUNT > 0	ROLLBACK TRAN		exec dbo.SQL_WriteLog;	END CATCH
END

GO
/****** Object:  StoredProcedure [dbo].[Property_Delete]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Property_Delete]
	 @Id int
AS
BEGIN
	 UPDATE tbl_property
		SET Status = 9
	 WHERE 1=1
	 AND Id = @Id
	 ;
END



GO
/****** Object:  StoredProcedure [dbo].[Property_GetByCategory]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Property_GetByCategory]
	 @PropertyCategoryId int
AS
BEGIN
	 SET NOCOUNT ON;

	 SELECT a.*
	 	 FROM tbl_property a
	 	 WHERE 1=1
		 AND a.Status = 1
		 AND a.PropertyCategoryId = @PropertyCategoryId
	 	 ORDER BY a.Name ASC
	 ;
END



GO
/****** Object:  StoredProcedure [dbo].[Property_GetById]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Property_GetById]
	 @Id int
AS
BEGIN
	 SELECT * FROM tbl_property
	 WHERE 1=1
	 AND Id = @Id
	 AND Status != 9
	 ;
END



GO
/****** Object:  StoredProcedure [dbo].[Property_GetByPage]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Property_GetByPage]
	 @Keyword nvarchar(128),
	 @Status int,
	 @Offset int,
	 @PageSize int
AS
BEGIN
	 SET NOCOUNT ON;

	 DECLARE @TotalCount int;
	 SET @TotalCount = (SELECT COUNT(1) 
		 FROM tbl_property a
		 LEFT JOIN tbl_property b on a.Id = b.PropertyCategoryId
	 	 WHERE 1=1
	 	 AND (a.Status = CASE WHEN (@Status = -1 OR @Status IS NULL) THEN a.Status ELSE @Status END)
		 AND (@Keyword IS NULL OR a.Name LIKE '%' + @Keyword + '%' OR a.Name LIKE '%' + @Keyword + '%' OR b.Name LIKE '%' + @Keyword + '%')	 
		 AND a.Status != 9
	 );

	 SELECT @TotalCount as TotalCount, a.*
	 	 FROM tbl_property a
		 LEFT JOIN tbl_property b on a.Id = b.PropertyCategoryId
	 	 WHERE 1=1
	 	 AND (a.Status = CASE WHEN (@Status = -1 OR @Status IS NULL) THEN a.Status ELSE @Status END)
		 AND (@Keyword IS NULL OR a.Name LIKE '%' + @Keyword + '%' OR a.Name LIKE '%' + @Keyword + '%' OR b.Name LIKE '%' + @Keyword + '%')
		 AND a.Status != 9
	 	 ORDER BY a.Id DESC
	 	 OFFSET @Offset ROWS
	 	 FETCH NEXT @PageSize ROWS ONLY
	 ;
END



GO
/****** Object:  StoredProcedure [dbo].[Property_Insert]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Property_Insert]
	@Code nvarchar(20),
	@PropertyCategoryId int,
	@Name nvarchar(128),
	@Status smallint
AS
BEGIN
	 INSERT INTO tbl_property(Code,PropertyCategoryId,Name,Status) 
	 VALUES(@Code,@PropertyCategoryId,@Name,@Status);

	 SELECT SCOPE_IDENTITY();
END



GO
/****** Object:  StoredProcedure [dbo].[Property_Update]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Property_Update]
	@Id int,
	@Code nvarchar(20),
	@PropertyCategoryId int,
	@Name nvarchar(128),
	@Status smallint
AS
BEGIN
	 UPDATE tbl_property
	 SET 
	 	 Code= @Code,
	 	 Name= @Name,
	 	 Status= @Status,
		 PropertyCategoryId = @PropertyCategoryId
	 WHERE 1=1
	 AND Id = @Id
	 ;
END



GO
/****** Object:  StoredProcedure [dbo].[PropertyCategory_Delete]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[PropertyCategory_Delete]
	 @Id int
AS
BEGIN
	 UPDATE tbl_property_category
		SET Status = 9
	 WHERE 1=1
	 AND Id = @Id
	 ;
END



GO
/****** Object:  StoredProcedure [dbo].[PropertyCategory_GetById]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[PropertyCategory_GetById]
	 @Id int
AS
BEGIN
	 SELECT * FROM tbl_property_category
	 WHERE 1=1
	 AND Id = @Id
	 AND Status != 9
	 ;
END



GO
/****** Object:  StoredProcedure [dbo].[PropertyCategory_GetByPage]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[PropertyCategory_GetByPage]
	 @Keyword nvarchar(128),
	 @Status int,
	 @Offset int,
	 @PageSize int
AS
BEGIN
	 SET NOCOUNT ON;

	 DECLARE @TotalCount int;
	 SET @TotalCount = (SELECT COUNT(1) FROM tbl_property_category a
	 	 WHERE 1=1
	 	 AND (a.Status = CASE WHEN (@Status = -1 OR @Status IS NULL) THEN a.Status ELSE @Status END)
		 AND (@Keyword IS NULL OR a.Name LIKE '%' + @Keyword + '%' OR a.Code LIKE '%' + @Keyword + '%'
			OR (@Keyword LIKE CASE WHEN (@Keyword = '' OR @Keyword IS NULL) THEN  '%' + @Keyword + '%' ELSE '%' + (SELECT TOP 1 @Keyword FROM tbl_property WHERE 1=1 AND Name LIKE '%' + @Keyword + '%' AND PropertyCategoryId = a.Id) + '%' END)
		 )
		 AND a.Status != 9
	 );

	SELECT *
	INTO #PropertyCategoryPaging
	FROM
	(
		SELECT @TotalCount as TotalCount, a.*
	 	 FROM tbl_property_category a
	 	 WHERE 1=1
	 	 AND (a.Status = CASE WHEN (@Status = -1 OR @Status IS NULL) THEN a.Status ELSE @Status END)
		 AND (@Keyword IS NULL OR a.Name LIKE '%' + @Keyword + '%' OR a.Code LIKE '%' + @Keyword + '%'
			OR (@Keyword LIKE CASE WHEN (@Keyword = '' OR @Keyword IS NULL) THEN  '%' + @Keyword + '%' ELSE '%' + (SELECT TOP 1 @Keyword FROM tbl_property WHERE 1=1 AND Name LIKE '%' + @Keyword + '%' AND PropertyCategoryId = a.Id) + '%' END)
		 )
		 AND a.Status != 9
	 	 ORDER BY a.Id DESC
	 	 OFFSET @Offset ROWS
	 	 FETCH NEXT @PageSize ROWS ONLY
	) as x;
	
	SELECT * FROM  #PropertyCategoryPaging;

	SELECT * FROM tbl_property WHERE 1=1
	AND PropertyCategoryId IN (SELECT Id FROM #PropertyCategoryPaging)
	AND Status != 9
	;
	
	DROP TABLE #PropertyCategoryPaging;
END



GO
/****** Object:  StoredProcedure [dbo].[PropertyCategory_GetDetail]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[PropertyCategory_GetDetail]
(
	@Id int
)
AS
BEGIN	
	-- Select base info
	SELECT TOP 1 * FROM tbl_property_category WHERE 1=1
	AND Id = @Id
	AND Status != 9
	;

	-- Select all properties
	SELECT * FROM tbl_property WHERE 1=1
	AND PropertyCategoryId = @Id
	AND Status != 9
	; 
END 

GO
/****** Object:  StoredProcedure [dbo].[PropertyCategory_GetList]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[PropertyCategory_GetList]
	 
AS
BEGIN
	 SELECT * FROM tbl_property_category
	 WHERE 1=1
	 AND Status = 1
	 ;
END



GO
/****** Object:  StoredProcedure [dbo].[PropertyCategory_Insert]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[PropertyCategory_Insert]
	@Code nvarchar(20),
	@Name nvarchar(128),
	@Status smallint
AS
BEGIN
	 INSERT INTO tbl_property_category(Code,Name,Status) 
	 VALUES(@Code,@Name,@Status);

	 SELECT SCOPE_IDENTITY();
END



GO
/****** Object:  StoredProcedure [dbo].[PropertyCategory_Update]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[PropertyCategory_Update]
	@Id int,
	@Code nvarchar(20),
	@Name nvarchar(128),
	@Status smallint
AS
BEGIN
	 UPDATE tbl_property_category
	 SET 
	 	 Code= @Code,
	 	 Name= @Name,
	 	 Status= @Status
	 WHERE 1=1
	 AND Id = @Id
	 ;
END



GO
/****** Object:  StoredProcedure [dbo].[Provider_Delete]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[Provider_Delete]
	@Id int
AS
BEGIN
	BEGIN TRY
    BEGIN TRANSACTION
		UPDATE tbl_provider
		SET 
			Status = 9
		WHERE 1=1
		AND Id = @Id
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
/****** Object:  StoredProcedure [dbo].[Provider_GetById]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[Provider_GetById]
	@Id int
AS
BEGIN
	SELECT * FROM tbl_provider WHERE 1=1
	AND Id = @Id
	AND Status != 9
	;
END


GO
/****** Object:  StoredProcedure [dbo].[Provider_GetByPage]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[Provider_GetByPage]
(
	@Keyword nvarchar(100),
	@Status int,
	@Offset int,
	@PageSize int
)
AS
BEGIN	
	SET NOCOUNT ON;

	DECLARE @TotalCount bigint;
	SET @TotalCount=(SELECT COUNT(1) FROM tbl_provider a 
	WHERE 1=1 
		AND (
			a.Name like '%'+@Keyword+'%'
			OR a.Phone like '%'+@Keyword+'%'
			OR a.Email like '%'+@Keyword+'%'
			OR @Keyword IS NULL
		)	
		AND (((a.Status = CASE WHEN (@Status = -1 OR @Status IS NULL) THEN a.Status ELSE @Status END)))
		AND Status != 9
		)
		;

	SELECT @TotalCount as TotalCount, 
	*
	FROM tbl_provider a 
	WHERE 1=1 
	AND (
			a.Name like '%'+@Keyword+'%'
			OR a.Phone like '%'+@Keyword+'%'
			OR a.Email like '%'+@Keyword+'%'
			OR @Keyword IS NULL
	)		
	AND (((a.Status = CASE WHEN (@Status = -1 OR @Status IS NULL) THEN a.Status ELSE @Status END)))
	AND Status != 9
	ORDER BY Name ASC
	OFFSET @Offset ROWS		
	FETCH NEXT @PageSize ROWS ONLY
END 


GO
/****** Object:  StoredProcedure [dbo].[Provider_GetList]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[Provider_GetList]
as
begin
select * from tbl_provider where Status=1 
end


GO
/****** Object:  StoredProcedure [dbo].[Provider_Insert]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Provider_Insert]	@Name nvarchar(128),	@Code nvarchar(20),	@Phone nvarchar(20),	@Email nvarchar(128),	@Address nvarchar(500),		@Status intAS
BEGIN
	INSERT INTO tbl_provider (Name, Code, Phone,Email, Address, Status)		VALUES(@Name, @Code, @Phone, @Email, @Address, @Status)
	;

	SELECT SCOPE_IDENTITY();
END



GO
/****** Object:  StoredProcedure [dbo].[Provider_Update]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Provider_Update]	@Id int,	@Name nvarchar(128),	@Code nvarchar(20),	@Phone nvarchar(20),	@Email nvarchar(128),	@Address nvarchar(500),		@Status intAS
BEGIN
	BEGIN TRY
    BEGIN TRANSACTION
		UPDATE tbl_provider
		SET 
			Name = @Name,
			Code = @Code,
			Phone = Phone,
			Email = @Email,
			Address = @Address,						Status = @Status
		WHERE 1=1
		AND Id = @Id
		AND Status != 9
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
/****** Object:  StoredProcedure [dbo].[Province_GetByCountry]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [dbo].[Province_GetByCountry]
(
	@CountryId int
)	
AS
BEGIN
	SET NOCOUNT ON;

	SELECT * FROM tbl_province  
	WHERE 1=1
	AND CountryId = @CountryId
	AND Status = 1
	ORDER BY Name ASC
	;
END



GO
/****** Object:  StoredProcedure [dbo].[Province_GetList]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [dbo].[Province_GetList]	
AS
BEGIN
	SET NOCOUNT ON;

	SELECT * FROM tbl_province 
	WHERE 1=1
	AND Status = 1
	ORDER BY Name ASC
	;
END



GO
/****** Object:  StoredProcedure [dbo].[Settings_LoadSettings]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [dbo].[Settings_LoadSettings]
	@pType nvarchar (50)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT * FROM cmn_settings t 
	where t.SettingType = @pType
	;
END



GO
/****** Object:  StoredProcedure [dbo].[SQL_WriteLog]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SQL_WriteLog]
AS
BEGIN
	BEGIN TRY
    BEGIN TRANSACTION

		INSERT INTO cmn_sql_errors (ErrorMessage, ErrorServerity, ErrorState, ErrorLine, Actor, DateOfIssue)
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
/****** Object:  StoredProcedure [dbo].[System_ClearPeriod]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[System_ClearPeriod]
	 
AS
BEGIN
	DECLARE @numberOfMonth int;
	DECLARE @requiredDate datetime    

	SET @numberOfMonth = (SELECT TOP 1 CAST(SettingValue as int) FROM cmn_settings WHERE 1=1 AND SettingName = 'StoragePeriodTime')

	IF @numberOfMonth IS NULL OR @numberOfMonth = 0
	BEGIN
		--SET @numberOfMonth = 6;
		SELECT 0 RETURN;
	END

	IF @numberOfMonth > 48
	BEGIN
		SET @numberOfMonth = 48;
	END

	SET @requiredDate = (SELECT CONVERT(DATETIME,DATEADD(MM, DATEDIFF(MM, 0, GETDATE()) - @numberOfMonth, 0)))

	SELECT @requiredDate;

	--Warehouse activities
	DELETE FROM tbl_warehouse_activity WHERE 1=1 AND CreatedDate < @requiredDate;

	--Goods receipt detail
	DELETE a FROM tbl_goods_receipt_detail a
	LEFT JOIN tbl_goods_receipt b ON a.GoodsReceiptId = b.Id
	WHERE b.CreatedDate < @requiredDate
	;

	--Goods receipt
	DELETE a FROM tbl_goods_receipt a
	WHERE a.CreatedDate < @requiredDate
	;

	--Goods issue detail
	DELETE a FROM tbl_goods_issue_detail a
	LEFT JOIN tbl_goods_issue b ON a.GoodsIssueId = b.Id
	WHERE b.CreatedDate < @requiredDate
	;

	--Goods issue
	DELETE a FROM tbl_goods_issue a
	WHERE a.CreatedDate < @requiredDate
	;

	--Reflectstocktake detail
	DELETE a FROM tbl_reflectstocktake_detail a
	LEFT JOIN tbl_reflectstocktake b ON a.ReflectStockTakeId = b.Id
	WHERE b.CreatedDate < @requiredDate
	;

	--Reflectstocktake
	DELETE a FROM tbl_reflectstocktake a
	WHERE a.CreatedDate < @requiredDate
	;

	--Activity logs
	DELETE FROM aspnetactivitylog 
	WHERE 1=1 
	AND ActivityDate < @requiredDate
	;
	
	INSERT INTO [dbo].[aspnetactivitylog]
           ([UserId]
           ,[ActivityText]
           ,[TargetType]
           ,[TargetId]
           ,[IPAddress]
           ,[ActivityDate]
           ,[ActivityType])
     VALUES
        ('0'
        ,'System Clear Logs Period ' + (select convert(varchar, getdate(), 121))
        ,'System'
        ,'0'
        ,''
        ,getdate()
        ,'SystemClearLog'
		);

END



GO
/****** Object:  StoredProcedure [dbo].[Unit_Delete]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Unit_Delete]
	 @Id int
AS
BEGIN
	 UPDATE tbl_unit
		SET Status = 9
	 WHERE 1=1
	 AND Id = @Id
	 ;
END


GO
/****** Object:  StoredProcedure [dbo].[Unit_GetById]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Unit_GetById]
	 @Id int
AS
BEGIN
	 SELECT * FROM tbl_unit
	 WHERE 1=1
	 AND Id = @Id
	 AND Status != 9
	 ;
END


GO
/****** Object:  StoredProcedure [dbo].[Unit_GetByPage]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Unit_GetByPage]
	 @Keyword nvarchar(128),
	 @Status int,
	 @Offset int,
	 @PageSize int
AS
BEGIN
	 SET NOCOUNT ON;

	 DECLARE @TotalCount int;
	 SET @TotalCount = (SELECT COUNT(1) FROM tbl_unit a
	 	 WHERE 1=1
	 	 AND (a.Status = CASE WHEN (@Status = -1 OR @Status IS NULL) THEN a.Status ELSE @Status END)
		 AND (@Keyword IS NULL OR a.Name LIKE '%' + @Keyword + '%' OR a.Name LIKE '%' + @Keyword + '%')
		 AND a.Status != 9
	 );

	 SELECT @TotalCount as TotalCount, a.*
	 	 FROM tbl_unit a
	 	 WHERE 1=1
	 	 AND (a.Status = CASE WHEN (@Status = -1 OR @Status IS NULL) THEN a.Status ELSE @Status END)
		 AND (@Keyword IS NULL OR a.Name LIKE '%' + @Keyword + '%' OR a.Name LIKE '%' + @Keyword + '%')
		 AND a.Status != 9
	 	 ORDER BY a.Id DESC
	 	 OFFSET @Offset ROWS
	 	 FETCH NEXT @PageSize ROWS ONLY
	 ;
END


GO
/****** Object:  StoredProcedure [dbo].[Unit_GetList]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Unit_GetList]
	 
AS
BEGIN
	 SELECT * FROM tbl_unit
	 WHERE 1=1
	 AND Status = 1
	 ;
END


GO
/****** Object:  StoredProcedure [dbo].[Unit_Insert]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Unit_Insert]
	@Code nvarchar(20),
	@Name nvarchar(128),
	@Status smallint
AS
BEGIN
	 INSERT INTO tbl_unit(Code,Name,Status) 
	 VALUES(@Code,@Name,@Status);

	 SELECT SCOPE_IDENTITY();
END


GO
/****** Object:  StoredProcedure [dbo].[Unit_Update]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Unit_Update]
	@Id int,
	@Code nvarchar(20),
	@Name nvarchar(128),
	@Status smallint
AS
BEGIN
	 UPDATE tbl_unit
	 SET 
	 	 Code= @Code,
	 	 Name= @Name,
	 	 Status= @Status
	 WHERE 1=1
	 AND Id = @Id
	 ;
END


GO
/****** Object:  StoredProcedure [dbo].[Users_GetListUser]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[Users_GetListUser]
as
BEGIN
	SELECT * FROM aspnetusers WHERE 1=1
	AND ( (LockoutEnabled = 1 AND LockoutEndDateUtc IS NULL) OR (LockoutEnabled = 0 AND LockoutEndDateUtc IS NOT NULL))
	AND UserName != 'admin'
	;
END


GO
/****** Object:  StoredProcedure [dbo].[Users_Login]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[Users_Login]
 @UserName nvarchar(128),
 @PasswordHash nvarchar(256)
as
BEGIN
	DECLARE @UserId nvarchar(128);
	DECLARE @AccessId nvarchar(128);

	SELECT *
	INTO #HTUserInfo
	FROM aspnetusers WHERE 1=1
	AND UserName != 'admin'
	AND UserName = @UserName
	AND PasswordHash = @PasswordHash
	;
	
	SET @UserId = (SELECT Id FROM #HTUserInfo);

	IF @UserId IS NOT NULL
	BEGIN		
		SET @AccessId = (
			SELECT TOP 1 a.Id FROM aspnetaccess a
			LEFT JOIN aspnetoperations b on a.Id = b.AccessId
			LEFT JOIN aspnetaccessroles c on b.Id = c.OperationId
			LEFT JOIN aspnetuserroles d on c.RoleId = d.RoleId
			WHERE 1=1
			AND d.UserId = @UserId
			AND a.AccessName = 'Warehouse'
			AND b.ActionName = 'Index'
		);
	END
	
	SELECT * FROM #HTUserInfo;

	SELECT @AccessId;
END


GO
/****** Object:  StoredProcedure [dbo].[Widget_GetList]    Script Date: 11/22/2019 10:11:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Widget_GetList]
AS
BEGIN
	 SELECT * FROM tbl_widget
	 WHERE 1=1
	 AND Status = 1
	 ;
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
EXEC sys.sp_addextendedproperty @name=N'MS_SSMA_SOURCE', @value=N'manager.aspnetaccess' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'aspnetaccess'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_SSMA_SOURCE', @value=N'manager.aspnetaccessroles' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'aspnetaccessroles'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_SSMA_SOURCE', @value=N'manager.aspnetactivitylog' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'aspnetactivitylog'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_SSMA_SOURCE', @value=N'manager.aspnetmenus' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'aspnetmenus'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_SSMA_SOURCE', @value=N'manager.aspnetoperations' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'aspnetoperations'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_SSMA_SOURCE', @value=N'manager.aspnetroles' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'aspnetroles'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_SSMA_SOURCE', @value=N'manager.aspnetuserclaims' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'aspnetuserclaims'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_SSMA_SOURCE', @value=N'manager.aspnetuserlogins' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'aspnetuserlogins'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_SSMA_SOURCE', @value=N'manager.aspnetuserroles' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'aspnetuserroles'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_SSMA_SOURCE', @value=N'manager.aspnetusers' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'aspnetusers'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_SSMA_SOURCE', @value=N'manager.cmn_settings' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'cmn_settings'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_SSMA_SOURCE', @value=N'manager.tbl_navigation' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'tbl_navigation'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'0: Khong kha dung, 1: Dang hoat dong, 9: Xoa logic' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'tbl_project_category', @level2type=N'COLUMN',@level2name=N'Status'
GO
