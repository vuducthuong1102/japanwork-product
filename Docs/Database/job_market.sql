USE [job_market]
GO
/****** Object:  Table [dbo].[agency]    Script Date: 8/13/2019 1:44:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[agency](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[user_id] [int] NULL,
	[constract_id] [int] NULL,
	[company_id] [int] NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
 CONSTRAINT [PK_agency] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[application]    Script Date: 8/13/2019 1:44:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[application](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[status] [tinyint] NULL,
	[interview_accept_time] [datetime] NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
	[cancelled_time] [datetime] NULL,
	[cv_id] [int] NULL,
	[job_id] [int] NULL,
	[job_seeker_id] [int] NULL,
 CONSTRAINT [PK_application] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[candidate]    Script Date: 8/13/2019 1:44:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[candidate](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[status] [tinyint] NULL,
	[request_time] [datetime] NULL,
	[applied_time] [datetime] NULL,
	[cv_id] [int] NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
	[job_id] [int] NULL,
	[job_seeker_id] [int] NULL,
 CONSTRAINT [PK_candidate] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[certificate]    Script Date: 8/13/2019 1:44:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[certificate](
	[id] [int] NOT NULL,
	[name] [nvarchar](255) NULL,
	[work_background_id] [int] NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
 CONSTRAINT [PK_certificate] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[certificate_cv]    Script Date: 8/13/2019 1:44:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[certificate_cv](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[cv_id] [int] NULL,
	[name] [nvarchar](255) NULL,
	[start_date] [datetime] NULL,
	[status] [int] NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
	[point] [nvarchar](255) NULL,
	[pass] [tinyint] NULL,
 CONSTRAINT [PK_certificate_cv] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[city]    Script Date: 8/13/2019 1:44:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[city](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[city] [nvarchar](255) NULL,
	[furigana] [nvarchar](255) NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
	[prefecture_id] [int] NULL,
 CONSTRAINT [PK_city] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[company]    Script Date: 8/13/2019 1:44:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[company](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[company_name] [nvarchar](255) NULL,
	[description] [nvarchar](2048) NULL,
	[company_size_id] [int] NULL,
	[logo_path] [nvarchar](4000) NULL,
	[sub_industry_id] [int] NULL,
	[establish_year] [int] NULL,
	[website] [nvarchar](256) NULL,
	[phone] [nvarchar](64) NULL,
	[fax] [nvarchar](64) NULL,
	[branch] [int] NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
	[agency_id] [int] NULL,
	[desciption_trans] [int] NULL,
	[headquater_id] [nchar](10) NULL,
	[region_id] [int] NULL,
	[prefecture_id] [int] NULL,
	[city_id] [int] NULL,
	[lat] [nvarchar](50) NULL,
	[lng] [nvarchar](50) NULL,
	[map] [geography] NULL,
 CONSTRAINT [PK_company] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[constract]    Script Date: 8/13/2019 1:44:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[constract](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[constract_time] [datetime] NULL,
	[constract_number] [nvarchar](45) NULL,
	[duration] [datetime] NULL,
	[remark] [nvarchar](1024) NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
 CONSTRAINT [PK_constract] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[content_example]    Script Date: 8/13/2019 1:44:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[content_example](
	[content_example_id] [int] IDENTITY(1,1) NOT NULL,
	[title] [nvarchar](255) NULL,
	[content] [ntext] NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
	[example_id] [int] NULL,
 CONSTRAINT [PK_content_example] PRIMARY KEY CLUSTERED 
(
	[content_example_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[cv]    Script Date: 8/13/2019 1:44:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[cv](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[job_seeker_id] [int] NULL,
	[cv_title] [nvarchar](255) NULL,
	[date] [datetime] NULL,
	[fullname] [nvarchar](255) NULL,
	[fullname_furigana] [nvarchar](255) NULL,
	[gender] [tinyint] NULL,
	[birthday] [datetime] NULL,
	[email] [nvarchar](255) NULL,
	[phone] [nvarchar](255) NULL,
	[mariage] [tinyint] NULL,
	[dependent_num] [tinyint] NULL,
	[higest_edu] [int] NULL,
	[pr] [ntext] NULL,
	[hobby_skills] [ntext] NULL,
	[reason] [ntext] NULL,
	[time_work] [ntext] NULL,
	[aspiration] [ntext] NULL,
	[form] [int] NULL,
	[image] [nvarchar](255) NULL,
	[pdf] [nvarchar](255) NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
	[reason_pr] [ntext] NULL,
	[contact_phone] [nvarchar](255) NULL,
	[check_address] [bit] NULL,
	[check_work] [bit] NULL,
	[check_ceti] [bit] NULL,
	[check_timework] [bit] NULL,
	[address] [nvarchar](500) NULL,
	[region_id] [int] NULL,
	[perfecture_id] [int] NULL,
	[city_id] [int] NULL,
	[contact_address] [nvarchar](500) NULL,
	[main_cv] [int] NULL,
	[station_id] [int] NULL,
 CONSTRAINT [PK_cv] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[edu_history]    Script Date: 8/13/2019 1:44:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[edu_history](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[cv_id] [int] NULL,
	[shool] [nvarchar](255) NULL,
	[start_date] [datetime] NULL,
	[end_date] [datetime] NULL,
	[status] [tinyint] NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
	[address] [nvarchar](255) NULL,
	[qualification_id] [int] NULL,
 CONSTRAINT [PK_edu_history] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[employment_type]    Script Date: 8/13/2019 1:44:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[employment_type](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[employment_type] [nvarchar](255) NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
	[translation_id] [int] NULL,
 CONSTRAINT [PK_employment_type] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[example]    Script Date: 8/13/2019 1:44:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[example](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](255) NULL,
	[description] [ntext] NULL,
	[type] [tinyint] NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
 CONSTRAINT [PK_example] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[expected_location]    Script Date: 8/13/2019 1:44:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[expected_location](
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
	[job_seeker_id] [int] NULL,
	[region_id] [int] NULL,
	[prefecture_id] [int] NULL,
	[city_id] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[expected_station]    Script Date: 8/13/2019 1:44:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[expected_station](
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
	[job_seeker_id] [int] NULL,
	[station_id] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[field]    Script Date: 8/13/2019 1:44:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[field](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[field] [nvarchar](255) NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
	[translation_id] [int] NULL,
 CONSTRAINT [PK_field] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[industry]    Script Date: 8/13/2019 1:44:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[industry](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[industry] [nvarchar](255) NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
	[translation_id] [int] NULL,
 CONSTRAINT [PK_industry] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[job]    Script Date: 8/13/2019 1:44:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[job](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[company_id] [int] NULL,
	[quantity] [smallint] NULL,
	[age_min] [smallint] NULL,
	[age_max] [smallint] NULL,
	[salary_min] [int] NULL,
	[salary_max] [int] NULL,
	[salary_type_id] [int] NULL,
	[work_start_time] [time](7) NULL,
	[work_end_time] [time](7) NULL,
	[probation_duration] [int] NULL,
	[status] [tinyint] NULL,
	[closed_time] [datetime] NULL,
	[employment_type_id] [int] NULL,
	[flexible_time] [bit] NULL,
	[language_level] [nvarchar](10) NULL,
	[work_experience_doc_required] [bit] NULL,
	[view_count] [int] NULL,
	[duration] [int] NULL,
	[view_company] [bit] NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
	[qualification_id] [int] NULL,
	[station_id] [int] NULL,
 CONSTRAINT [PK_job] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[job_address]    Script Date: 8/13/2019 1:44:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[job_address](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[job_id] [int] NULL,
	[region_id] [int] NULL,
	[prefecture_id] [int] NULL,
	[city_id] [int] NULL,
	[address] [nvarchar](255) NULL,
	[note] [nvarchar](255) NULL,
 CONSTRAINT [PK_job_address] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[job_alert]    Script Date: 8/13/2019 1:44:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[job_alert](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[job_title] [nvarchar](255) NULL,
	[year_salary_min] [nvarchar](255) NULL,
	[prefecture] [nvarchar](255) NULL,
	[subfield] [nvarchar](255) NULL,
	[job_seeker_user_id] [int] NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
	[job_seeker_id] [int] NULL,
 CONSTRAINT [PK_job_alert] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[job_seeker]    Script Date: 8/13/2019 1:44:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[job_seeker](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[user_id] [int] NULL,
	[video_path] [nvarchar](max) NULL,
	[expected_job_title] [nvarchar](255) NULL,
	[expected_salary_min] [int] NULL,
	[expected_salary_max] [int] NULL,
	[work_status] [int] NULL,
	[google_id] [nvarchar](255) NULL,
	[facebook_id] [nvarchar](255) NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
	[job_seeking_status_id] [int] NULL,
	[salary_type_id] [int] NULL,
 CONSTRAINT [PK_job_seeker] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[job_seeking_status]    Script Date: 8/13/2019 1:44:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[job_seeking_status](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](255) NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
 CONSTRAINT [PK_job_seeking_status] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[job_sub_field]    Script Date: 8/13/2019 1:44:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[job_sub_field](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[job_id] [int] NULL,
	[sub_field_id] [int] NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
 CONSTRAINT [PK_job_sub_field] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[job_tag]    Script Date: 8/13/2019 1:44:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[job_tag](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[tag_id] [int] NULL,
	[job_id] [int] NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
 CONSTRAINT [PK_job_tag] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[job_translation]    Script Date: 8/13/2019 1:44:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[job_translation](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[title] [nvarchar](255) NULL,
	[subsidy] [nvarchar](255) NULL,
	[paid_holiday] [nvarchar](128) NULL,
	[bonus] [nvarchar](128) NULL,
	[certificate] [nvarchar](1024) NULL,
	[work_content] [nvarchar](4000) NULL,
	[requirement] [nvarchar](2048) NULL,
	[plus] [nvarchar](128) NULL,
	[welfare] [nvarchar](128) NULL,
	[training] [nvarchar](512) NULL,
	[recruiment_procedure] [nvarchar](512) NULL,
	[remark] [nvarchar](1024) NULL,
	[job_id] [int] NULL,
	[language_code] [nvarchar](10) NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
 CONSTRAINT [PK_job_translation] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[language]    Script Date: 8/13/2019 1:44:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[language](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](255) NULL,
	[lang_code] [nvarchar](10) NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
 CONSTRAINT [PK_language] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[language_background]    Script Date: 8/13/2019 1:44:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[language_background](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[work_background_id] [int] NULL,
	[language_id] [int] NULL,
	[language_level_id] [int] NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
 CONSTRAINT [PK_language_background] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[language_level]    Script Date: 8/13/2019 1:44:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[language_level](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[language_level] [nvarchar](128) NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
 CONSTRAINT [PK_language_level] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[notification]    Script Date: 8/13/2019 1:44:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[notification](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[title] [nvarchar](255) NULL,
	[content] [nvarchar](255) NULL,
	[isread] [bit] NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
	[notification_type] [tinyint] NULL,
	[user_id] [int] NULL,
 CONSTRAINT [PK_notification] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[notification_type]    Script Date: 8/13/2019 1:44:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[notification_type](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[type_name] [nvarchar](255) NULL,
	[description] [nvarchar](255) NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
 CONSTRAINT [PK_notification_type] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[pdf_code_id]    Script Date: 8/13/2019 1:44:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[pdf_code_id](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[code_id] [nvarchar](255) NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
	[user_id] [int] NULL,
	[form] [int] NULL,
	[title] [nvarchar](255) NULL,
	[cv_id] [int] NULL,
	[work_background_id] [int] NULL,
 CONSTRAINT [PK_pdf_code_id] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[prefecture]    Script Date: 8/13/2019 1:44:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[prefecture](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[prefecture] [nvarchar](255) NULL,
	[region_id] [int] NULL,
	[furigana] [nvarchar](255) NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
 CONSTRAINT [PK_prefecture] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[qualification]    Script Date: 8/13/2019 1:44:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[qualification](
	[id] [int] NULL,
	[qualification] [nvarchar](45) NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
	[translation_id] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[recommended_job]    Script Date: 8/13/2019 1:44:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[recommended_job](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
	[job_seeker_id] [int] NULL,
 CONSTRAINT [PK_recommended_job] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[region]    Script Date: 8/13/2019 1:44:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[region](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[region] [nvarchar](255) NULL,
	[furigana] [nvarchar](255) NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
 CONSTRAINT [PK_region] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[salary_type]    Script Date: 8/13/2019 1:44:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[salary_type](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[salary_type] [nvarchar](255) NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
	[translation_id] [int] NULL,
 CONSTRAINT [PK_salary_type] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[saved_job]    Script Date: 8/13/2019 1:44:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[saved_job](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
	[job_id] [int] NULL,
	[job_seeker_id] [int] NULL,
 CONSTRAINT [PK_saved_job] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[search]    Script Date: 8/13/2019 1:44:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[search](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[title] [nvarchar](255) NULL,
	[salary_min] [int] NULL,
	[prefecture_id] [int] NULL,
	[city_id] [int] NULL,
	[region_id] [int] NULL,
	[sub_field_id] [int] NULL,
	[sub_industry_id] [int] NULL,
	[station_id] [int] NULL,
	[employment_type_id] [int] NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
 CONSTRAINT [PK_search] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[sequelizemeta]    Script Date: 8/13/2019 1:44:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sequelizemeta](
	[name] [nvarchar](255) NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[station]    Script Date: 8/13/2019 1:44:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[station](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[station] [nvarchar](255) NULL,
	[furigana] [nvarchar](255) NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
	[address] [nvarchar](255) NULL,
	[region_id] [int] NULL,
	[prefecture_id] [int] NULL,
	[city_id] [int] NULL,
	[lat] [nvarchar](50) NULL,
	[lng] [nvarchar](50) NULL,
	[map] [geography] NULL,
 CONSTRAINT [PK_station] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[sub_field]    Script Date: 8/13/2019 1:44:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sub_field](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[field_id] [int] NULL,
	[sub_field] [nvarchar](255) NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
	[translation_id] [int] NULL,
 CONSTRAINT [PK_sub_field] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[sub_industry]    Script Date: 8/13/2019 1:44:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sub_industry](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[industry_id] [int] NULL,
	[sub_industry] [nvarchar](255) NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
	[translation_id] [int] NULL,
 CONSTRAINT [PK_sub_industry] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[suggest]    Script Date: 8/13/2019 1:44:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[suggest](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[form] [int] NULL,
	[type] [smallint] NULL,
	[title] [nvarchar](255) NULL,
	[content] [ntext] NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
	[isdescription] [bit] NULL,
	[field_id] [int] NULL,
 CONSTRAINT [PK_suggest] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tag]    Script Date: 8/13/2019 1:44:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tag](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[tag] [nvarchar](128) NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
 CONSTRAINT [PK_tag] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[token_firebase]    Script Date: 8/13/2019 1:44:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[token_firebase](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[token] [nvarchar](255) NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
	[user_id] [int] NULL,
 CONSTRAINT [PK_token_firebase] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[train_line]    Script Date: 8/13/2019 1:44:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[train_line](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[train_line] [nvarchar](128) NULL,
	[furigana] [nvarchar](128) NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
	[translation_id] [int] NULL,
 CONSTRAINT [PK_train_line] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[train_line_station]    Script Date: 8/13/2019 1:44:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[train_line_station](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
	[station_id] [int] NULL,
	[train_line_id] [int] NULL,
 CONSTRAINT [PK_train_line_station] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[translation]    Script Date: 8/13/2019 1:44:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[translation](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
 CONSTRAINT [PK_translation] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[translation_language]    Script Date: 8/13/2019 1:44:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[translation_language](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[text] [nvarchar](2000) NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
	[translation_id] [int] NULL,
	[language_id] [int] NULL,
	[language_code] [nvarchar](10) NULL,
 CONSTRAINT [PK_translation_language] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[work_background]    Script Date: 8/13/2019 1:44:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[work_background](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[job_seeker_id] [int] NULL,
	[work_background_title] [nvarchar](255) NULL,
	[date] [datetime] NULL,
	[fullname] [nvarchar](255) NULL,
	[fullname_hira] [nvarchar](255) NULL,
	[summary] [ntext] NULL,
	[experience] [ntext] NULL,
	[pr] [ntext] NULL,
	[pdf] [nvarchar](255) NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
	[cv_id] [int] NULL,
 CONSTRAINT [PK_work_background] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[work_history_background]    Script Date: 8/13/2019 1:44:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[work_history_background](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[content_work] [ntext] NULL,
	[start_date] [datetime] NULL,
	[end_date] [datetime] NULL,
	[status] [smallint] NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
	[work_progress_id] [int] NULL,
 CONSTRAINT [PK_work_history_background] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[work_history_cv]    Script Date: 8/13/2019 1:44:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[work_history_cv](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[cv_id] [int] NULL,
	[company] [nvarchar](255) NULL,
	[content_work] [ntext] NULL,
	[form] [int] NULL,
	[start_date] [datetime] NULL,
	[end_date] [datetime] NULL,
	[status] [smallint] NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
	[address] [nvarchar](255) NULL,
 CONSTRAINT [PK_work_history_cv] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[work_progress]    Script Date: 8/13/2019 1:44:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[work_progress](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[work_background_id] [int] NULL,
	[company] [nvarchar](255) NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
 CONSTRAINT [PK_work_progress] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[agency] ADD  CONSTRAINT [DF_agency_user_id]  DEFAULT ((0)) FOR [user_id]
GO
ALTER TABLE [dbo].[agency] ADD  CONSTRAINT [DF_agency_constract_id]  DEFAULT ((0)) FOR [constract_id]
GO
ALTER TABLE [dbo].[agency] ADD  CONSTRAINT [DF_agency_company_id]  DEFAULT ((0)) FOR [company_id]
GO
ALTER TABLE [dbo].[agency] ADD  CONSTRAINT [DF_agency_created_at]  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[application] ADD  CONSTRAINT [DF_application_status]  DEFAULT ((0)) FOR [status]
GO
ALTER TABLE [dbo].[application] ADD  CONSTRAINT [DF_application_created_at]  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[application] ADD  CONSTRAINT [DF_application_cv_id]  DEFAULT ((0)) FOR [cv_id]
GO
ALTER TABLE [dbo].[application] ADD  CONSTRAINT [DF_application_job_id]  DEFAULT ((0)) FOR [job_id]
GO
ALTER TABLE [dbo].[application] ADD  CONSTRAINT [DF_application_job_seeker_id]  DEFAULT ((0)) FOR [job_seeker_id]
GO
ALTER TABLE [dbo].[candidate] ADD  CONSTRAINT [DF_candidate_status]  DEFAULT ((0)) FOR [status]
GO
ALTER TABLE [dbo].[candidate] ADD  CONSTRAINT [DF_candidate_cv_id]  DEFAULT ((0)) FOR [cv_id]
GO
ALTER TABLE [dbo].[candidate] ADD  CONSTRAINT [DF_candidate_created_at]  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[candidate] ADD  CONSTRAINT [DF_candidate_job_id]  DEFAULT ((0)) FOR [job_id]
GO
ALTER TABLE [dbo].[candidate] ADD  CONSTRAINT [DF_candidate_job_seeker_id]  DEFAULT ((0)) FOR [job_seeker_id]
GO
ALTER TABLE [dbo].[certificate] ADD  CONSTRAINT [DF_certificate_work_background_id]  DEFAULT ((0)) FOR [work_background_id]
GO
ALTER TABLE [dbo].[certificate] ADD  CONSTRAINT [DF_certificate_created_at]  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[certificate_cv] ADD  CONSTRAINT [DF_certificate_cv_cv_id]  DEFAULT ((0)) FOR [cv_id]
GO
ALTER TABLE [dbo].[certificate_cv] ADD  CONSTRAINT [DF_certificate_cv_status]  DEFAULT ((1)) FOR [status]
GO
ALTER TABLE [dbo].[certificate_cv] ADD  CONSTRAINT [DF_certificate_cv_created_at]  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[certificate_cv] ADD  CONSTRAINT [DF_certificate_cv_pass]  DEFAULT ((0)) FOR [pass]
GO
ALTER TABLE [dbo].[city] ADD  CONSTRAINT [DF_city_created_at]  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[city] ADD  CONSTRAINT [DF_city_prefecture_id]  DEFAULT ((0)) FOR [prefecture_id]
GO
ALTER TABLE [dbo].[company] ADD  CONSTRAINT [DF_company_company_size_id]  DEFAULT ((0)) FOR [company_size_id]
GO
ALTER TABLE [dbo].[company] ADD  CONSTRAINT [DF_company_sub_industry_id]  DEFAULT ((0)) FOR [sub_industry_id]
GO
ALTER TABLE [dbo].[company] ADD  CONSTRAINT [DF_company_establish_year]  DEFAULT ((0)) FOR [establish_year]
GO
ALTER TABLE [dbo].[company] ADD  CONSTRAINT [DF_company_branch]  DEFAULT ((0)) FOR [branch]
GO
ALTER TABLE [dbo].[company] ADD  CONSTRAINT [DF_company_created_at]  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[company] ADD  CONSTRAINT [DF_company_agency_id]  DEFAULT ((0)) FOR [agency_id]
GO
ALTER TABLE [dbo].[company] ADD  CONSTRAINT [DF_company_desciption_trans]  DEFAULT ((0)) FOR [desciption_trans]
GO
ALTER TABLE [dbo].[company] ADD  CONSTRAINT [DF_company_region_id]  DEFAULT ((0)) FOR [region_id]
GO
ALTER TABLE [dbo].[company] ADD  CONSTRAINT [DF_company_prefecture_id]  DEFAULT ((0)) FOR [prefecture_id]
GO
ALTER TABLE [dbo].[company] ADD  CONSTRAINT [DF_company_city_id]  DEFAULT ((0)) FOR [city_id]
GO
ALTER TABLE [dbo].[constract] ADD  CONSTRAINT [DF_constract_created_at]  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[content_example] ADD  CONSTRAINT [DF_content_example_created_at]  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[content_example] ADD  CONSTRAINT [DF_content_example_example_id]  DEFAULT ((0)) FOR [example_id]
GO
ALTER TABLE [dbo].[cv] ADD  CONSTRAINT [DF_cv_job_seeker_id]  DEFAULT ((0)) FOR [job_seeker_id]
GO
ALTER TABLE [dbo].[cv] ADD  CONSTRAINT [DF_cv_gender]  DEFAULT ((0)) FOR [gender]
GO
ALTER TABLE [dbo].[cv] ADD  CONSTRAINT [DF_cv_mariage]  DEFAULT ((0)) FOR [mariage]
GO
ALTER TABLE [dbo].[cv] ADD  CONSTRAINT [DF_cv_dependent_num]  DEFAULT ((0)) FOR [dependent_num]
GO
ALTER TABLE [dbo].[cv] ADD  CONSTRAINT [DF_cv_created_at]  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[cv] ADD  CONSTRAINT [DF_cv_check_address]  DEFAULT ((0)) FOR [check_address]
GO
ALTER TABLE [dbo].[cv] ADD  CONSTRAINT [DF_cv_check_work]  DEFAULT ((0)) FOR [check_work]
GO
ALTER TABLE [dbo].[cv] ADD  CONSTRAINT [DF_cv_check_ceti]  DEFAULT ((0)) FOR [check_ceti]
GO
ALTER TABLE [dbo].[cv] ADD  CONSTRAINT [DF_cv_check_timework]  DEFAULT ((0)) FOR [check_timework]
GO
ALTER TABLE [dbo].[cv] ADD  CONSTRAINT [DF_Table_1_region]  DEFAULT ((0)) FOR [region_id]
GO
ALTER TABLE [dbo].[cv] ADD  CONSTRAINT [DF_cv_perfecture_id]  DEFAULT ((0)) FOR [perfecture_id]
GO
ALTER TABLE [dbo].[cv] ADD  CONSTRAINT [DF_cv_city_id]  DEFAULT ((0)) FOR [city_id]
GO
ALTER TABLE [dbo].[cv] ADD  CONSTRAINT [DF_cv_main_cv]  DEFAULT ((0)) FOR [main_cv]
GO
ALTER TABLE [dbo].[cv] ADD  CONSTRAINT [DF_cv_station_id]  DEFAULT ((0)) FOR [station_id]
GO
ALTER TABLE [dbo].[edu_history] ADD  CONSTRAINT [DF_edu_history_cv_id]  DEFAULT ((0)) FOR [cv_id]
GO
ALTER TABLE [dbo].[edu_history] ADD  CONSTRAINT [DF_edu_history_status]  DEFAULT ((1)) FOR [status]
GO
ALTER TABLE [dbo].[edu_history] ADD  CONSTRAINT [DF_edu_history_created_at]  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[edu_history] ADD  CONSTRAINT [DF_edu_history_qualification_id]  DEFAULT ((0)) FOR [qualification_id]
GO
ALTER TABLE [dbo].[employment_type] ADD  CONSTRAINT [DF_employment_type_created_at]  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[employment_type] ADD  CONSTRAINT [DF_employment_type_translation_id]  DEFAULT ((0)) FOR [translation_id]
GO
ALTER TABLE [dbo].[example] ADD  CONSTRAINT [DF_example_type]  DEFAULT ((0)) FOR [type]
GO
ALTER TABLE [dbo].[example] ADD  CONSTRAINT [DF_example_created_at]  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[expected_location] ADD  CONSTRAINT [DF_expected_location_created_at]  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[expected_location] ADD  CONSTRAINT [DF_expected_location_job_seeker_id]  DEFAULT ((0)) FOR [job_seeker_id]
GO
ALTER TABLE [dbo].[expected_location] ADD  CONSTRAINT [DF_expected_location_region_id]  DEFAULT ((0)) FOR [region_id]
GO
ALTER TABLE [dbo].[expected_location] ADD  CONSTRAINT [DF_expected_location_prefecture_id]  DEFAULT ((0)) FOR [prefecture_id]
GO
ALTER TABLE [dbo].[expected_location] ADD  CONSTRAINT [DF_expected_location_city_id]  DEFAULT ((0)) FOR [city_id]
GO
ALTER TABLE [dbo].[expected_station] ADD  CONSTRAINT [DF_expected_station_job_seeker_id]  DEFAULT ((0)) FOR [job_seeker_id]
GO
ALTER TABLE [dbo].[expected_station] ADD  CONSTRAINT [DF_expected_station_station_id]  DEFAULT ((0)) FOR [station_id]
GO
ALTER TABLE [dbo].[field] ADD  CONSTRAINT [DF_field_created_at]  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[field] ADD  CONSTRAINT [DF_field_translation_id]  DEFAULT ((0)) FOR [translation_id]
GO
ALTER TABLE [dbo].[industry] ADD  CONSTRAINT [DF_industry_created_at]  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[industry] ADD  CONSTRAINT [DF_industry_translation_id]  DEFAULT ((0)) FOR [translation_id]
GO
ALTER TABLE [dbo].[job] ADD  CONSTRAINT [DF_job_company_id]  DEFAULT ((0)) FOR [company_id]
GO
ALTER TABLE [dbo].[job] ADD  CONSTRAINT [DF_job_quantity]  DEFAULT ((1)) FOR [quantity]
GO
ALTER TABLE [dbo].[job] ADD  CONSTRAINT [DF_job_age_min]  DEFAULT ((18)) FOR [age_min]
GO
ALTER TABLE [dbo].[job] ADD  CONSTRAINT [DF_job_age_max]  DEFAULT ((65)) FOR [age_max]
GO
ALTER TABLE [dbo].[job] ADD  CONSTRAINT [DF_job_salary_min]  DEFAULT ((0)) FOR [salary_min]
GO
ALTER TABLE [dbo].[job] ADD  CONSTRAINT [DF_job_salary_max]  DEFAULT ((0)) FOR [salary_max]
GO
ALTER TABLE [dbo].[job] ADD  CONSTRAINT [DF_job_salary_type_id]  DEFAULT ((0)) FOR [salary_type_id]
GO
ALTER TABLE [dbo].[job] ADD  CONSTRAINT [DF_job_probation_duration]  DEFAULT ((0)) FOR [probation_duration]
GO
ALTER TABLE [dbo].[job] ADD  CONSTRAINT [DF_job_status]  DEFAULT ((0)) FOR [status]
GO
ALTER TABLE [dbo].[job] ADD  CONSTRAINT [DF_job_flexible_time]  DEFAULT ((1)) FOR [flexible_time]
GO
ALTER TABLE [dbo].[job] ADD  CONSTRAINT [DF_job_work_experience_doc_required]  DEFAULT ((0)) FOR [work_experience_doc_required]
GO
ALTER TABLE [dbo].[job] ADD  CONSTRAINT [DF_job_view_count]  DEFAULT ((0)) FOR [view_count]
GO
ALTER TABLE [dbo].[job] ADD  CONSTRAINT [DF_job_duration]  DEFAULT ((0)) FOR [duration]
GO
ALTER TABLE [dbo].[job] ADD  CONSTRAINT [DF_job_view_company]  DEFAULT ((1)) FOR [view_company]
GO
ALTER TABLE [dbo].[job] ADD  CONSTRAINT [DF_job_created_at]  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[job] ADD  CONSTRAINT [DF_job_qualification_id]  DEFAULT ((0)) FOR [qualification_id]
GO
ALTER TABLE [dbo].[job] ADD  CONSTRAINT [DF_job_station_id]  DEFAULT ((0)) FOR [station_id]
GO
ALTER TABLE [dbo].[job_address] ADD  CONSTRAINT [DF_job_address_job_id]  DEFAULT ((0)) FOR [job_id]
GO
ALTER TABLE [dbo].[job_address] ADD  CONSTRAINT [DF_job_address_region_id]  DEFAULT ((0)) FOR [region_id]
GO
ALTER TABLE [dbo].[job_address] ADD  CONSTRAINT [DF_job_address_prefecture_id]  DEFAULT ((0)) FOR [prefecture_id]
GO
ALTER TABLE [dbo].[job_address] ADD  CONSTRAINT [DF_job_address_city_id]  DEFAULT ((0)) FOR [city_id]
GO
ALTER TABLE [dbo].[job_alert] ADD  CONSTRAINT [DF_job_alert_job_seeker_user_id]  DEFAULT ((0)) FOR [job_seeker_user_id]
GO
ALTER TABLE [dbo].[job_alert] ADD  CONSTRAINT [DF_job_alert_created_at]  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[job_alert] ADD  CONSTRAINT [DF_job_alert_job_seeker_id]  DEFAULT ((0)) FOR [job_seeker_id]
GO
ALTER TABLE [dbo].[job_seeker] ADD  CONSTRAINT [DF_job_seeker_user_id]  DEFAULT ((0)) FOR [user_id]
GO
ALTER TABLE [dbo].[job_seeker] ADD  CONSTRAINT [DF_job_seeker_expected_salary_min]  DEFAULT ((0)) FOR [expected_salary_min]
GO
ALTER TABLE [dbo].[job_seeker] ADD  CONSTRAINT [DF_job_seeker_expected_salary_max]  DEFAULT ((0)) FOR [expected_salary_max]
GO
ALTER TABLE [dbo].[job_seeker] ADD  CONSTRAINT [DF_job_seeker_work_status]  DEFAULT ((0)) FOR [work_status]
GO
ALTER TABLE [dbo].[job_seeker] ADD  CONSTRAINT [DF_job_seeker_created_at]  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[job_seeker] ADD  CONSTRAINT [DF_job_seeker_job_seeking_status_id]  DEFAULT ((0)) FOR [job_seeking_status_id]
GO
ALTER TABLE [dbo].[job_seeker] ADD  CONSTRAINT [DF_job_seeker_salary_type_id]  DEFAULT ((0)) FOR [salary_type_id]
GO
ALTER TABLE [dbo].[job_seeking_status] ADD  CONSTRAINT [DF_job_seeking_status_created_at]  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[job_sub_field] ADD  CONSTRAINT [DF_job_sub_field_job_id]  DEFAULT ((0)) FOR [job_id]
GO
ALTER TABLE [dbo].[job_sub_field] ADD  CONSTRAINT [DF_job_sub_field_sub_field_id]  DEFAULT ((0)) FOR [sub_field_id]
GO
ALTER TABLE [dbo].[job_sub_field] ADD  CONSTRAINT [DF_job_sub_field_created_at]  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[job_tag] ADD  CONSTRAINT [DF_job_tag_tag_id]  DEFAULT ((0)) FOR [tag_id]
GO
ALTER TABLE [dbo].[job_tag] ADD  CONSTRAINT [DF_job_tag_job_id]  DEFAULT ((0)) FOR [job_id]
GO
ALTER TABLE [dbo].[job_tag] ADD  CONSTRAINT [DF_job_tag_created_at]  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[job_translation] ADD  CONSTRAINT [DF_job_translation_job_id]  DEFAULT ((0)) FOR [job_id]
GO
ALTER TABLE [dbo].[job_translation] ADD  CONSTRAINT [DF_Table_1_language_id]  DEFAULT ((0)) FOR [language_code]
GO
ALTER TABLE [dbo].[job_translation] ADD  CONSTRAINT [DF_job_translation_created_at]  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[language] ADD  CONSTRAINT [DF_language_created_at]  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[language_background] ADD  CONSTRAINT [DF_language_background_work_background_id]  DEFAULT ((0)) FOR [work_background_id]
GO
ALTER TABLE [dbo].[language_background] ADD  CONSTRAINT [DF_language_background_language_id]  DEFAULT ((0)) FOR [language_id]
GO
ALTER TABLE [dbo].[language_background] ADD  CONSTRAINT [DF_language_background_language_level_id]  DEFAULT ((0)) FOR [language_level_id]
GO
ALTER TABLE [dbo].[language_background] ADD  CONSTRAINT [DF_language_background_created_at]  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[language_level] ADD  CONSTRAINT [DF_language_level_created_at]  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[notification] ADD  CONSTRAINT [DF_notification_isread]  DEFAULT ((0)) FOR [isread]
GO
ALTER TABLE [dbo].[notification] ADD  CONSTRAINT [DF_notification_created_at]  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[notification] ADD  CONSTRAINT [DF_notification_notification_type]  DEFAULT ((0)) FOR [notification_type]
GO
ALTER TABLE [dbo].[notification] ADD  CONSTRAINT [DF_notification_user_id]  DEFAULT ((0)) FOR [user_id]
GO
ALTER TABLE [dbo].[notification_type] ADD  CONSTRAINT [DF_notification_type_created_at]  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[pdf_code_id] ADD  CONSTRAINT [DF_pdf_code_id_created_at]  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[pdf_code_id] ADD  CONSTRAINT [DF_pdf_code_id_user_id]  DEFAULT ((0)) FOR [user_id]
GO
ALTER TABLE [dbo].[pdf_code_id] ADD  CONSTRAINT [DF_pdf_code_id_form]  DEFAULT ((0)) FOR [form]
GO
ALTER TABLE [dbo].[pdf_code_id] ADD  CONSTRAINT [DF_pdf_code_id_cv_id]  DEFAULT ((0)) FOR [cv_id]
GO
ALTER TABLE [dbo].[pdf_code_id] ADD  CONSTRAINT [DF_pdf_code_id_work_background_id]  DEFAULT ((0)) FOR [work_background_id]
GO
ALTER TABLE [dbo].[prefecture] ADD  CONSTRAINT [DF_prefecture_region_id]  DEFAULT ((0)) FOR [region_id]
GO
ALTER TABLE [dbo].[prefecture] ADD  CONSTRAINT [DF_prefecture_created_at]  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[qualification] ADD  CONSTRAINT [DF_qualification_created_at]  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[qualification] ADD  CONSTRAINT [DF_qualification_translation_id]  DEFAULT ((0)) FOR [translation_id]
GO
ALTER TABLE [dbo].[recommended_job] ADD  CONSTRAINT [DF_recommended_job_created_at]  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[recommended_job] ADD  CONSTRAINT [DF_recommended_job_job_seeker_id]  DEFAULT ((0)) FOR [job_seeker_id]
GO
ALTER TABLE [dbo].[region] ADD  CONSTRAINT [DF_region_created_at]  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[salary_type] ADD  CONSTRAINT [DF_salary_type_created_at]  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[salary_type] ADD  CONSTRAINT [DF_salary_type_translation_id]  DEFAULT ((0)) FOR [translation_id]
GO
ALTER TABLE [dbo].[saved_job] ADD  CONSTRAINT [DF_saved_job_created_at]  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[saved_job] ADD  CONSTRAINT [DF_saved_job_job_id]  DEFAULT ((0)) FOR [job_id]
GO
ALTER TABLE [dbo].[saved_job] ADD  CONSTRAINT [DF_saved_job_job_seeker_id]  DEFAULT ((0)) FOR [job_seeker_id]
GO
ALTER TABLE [dbo].[search] ADD  CONSTRAINT [DF_search_salary_min]  DEFAULT ((0)) FOR [salary_min]
GO
ALTER TABLE [dbo].[search] ADD  CONSTRAINT [DF_search_prefecture_id]  DEFAULT ((0)) FOR [prefecture_id]
GO
ALTER TABLE [dbo].[search] ADD  CONSTRAINT [DF_search_city_id]  DEFAULT ((0)) FOR [city_id]
GO
ALTER TABLE [dbo].[search] ADD  CONSTRAINT [DF_search_region_id]  DEFAULT ((0)) FOR [region_id]
GO
ALTER TABLE [dbo].[search] ADD  CONSTRAINT [DF_search_sub_field_id]  DEFAULT ((0)) FOR [sub_field_id]
GO
ALTER TABLE [dbo].[search] ADD  CONSTRAINT [DF_search_sub_industry_id]  DEFAULT ((0)) FOR [sub_industry_id]
GO
ALTER TABLE [dbo].[search] ADD  CONSTRAINT [DF_search_station_id]  DEFAULT ((0)) FOR [station_id]
GO
ALTER TABLE [dbo].[search] ADD  CONSTRAINT [DF_search_employment_type_id]  DEFAULT ((0)) FOR [employment_type_id]
GO
ALTER TABLE [dbo].[search] ADD  CONSTRAINT [DF_search_created_at]  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[sequelizemeta] ADD  CONSTRAINT [DF_sequelizemeta_created_at]  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[station] ADD  CONSTRAINT [DF_station_created_at]  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[station] ADD  CONSTRAINT [DF_station_region_id]  DEFAULT ((0)) FOR [region_id]
GO
ALTER TABLE [dbo].[station] ADD  CONSTRAINT [DF_station_prefecture_id]  DEFAULT ((0)) FOR [prefecture_id]
GO
ALTER TABLE [dbo].[station] ADD  CONSTRAINT [DF_station_city_id]  DEFAULT ((0)) FOR [city_id]
GO
ALTER TABLE [dbo].[sub_field] ADD  CONSTRAINT [DF_sub_field_field_id]  DEFAULT ((0)) FOR [field_id]
GO
ALTER TABLE [dbo].[sub_field] ADD  CONSTRAINT [DF_sub_field_sub_field_id]  DEFAULT ((0)) FOR [sub_field]
GO
ALTER TABLE [dbo].[sub_industry] ADD  CONSTRAINT [DF_sub_industry_industry_id]  DEFAULT ((0)) FOR [industry_id]
GO
ALTER TABLE [dbo].[sub_industry] ADD  CONSTRAINT [DF_Table_1_sub_industry_id]  DEFAULT ((0)) FOR [sub_industry]
GO
ALTER TABLE [dbo].[sub_industry] ADD  CONSTRAINT [DF_sub_industry_created_at]  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[sub_industry] ADD  CONSTRAINT [DF_sub_industry_translation_id]  DEFAULT ((0)) FOR [translation_id]
GO
ALTER TABLE [dbo].[suggest] ADD  CONSTRAINT [DF_suggest_form]  DEFAULT ((0)) FOR [form]
GO
ALTER TABLE [dbo].[suggest] ADD  CONSTRAINT [DF_suggest_type]  DEFAULT ((0)) FOR [type]
GO
ALTER TABLE [dbo].[suggest] ADD  CONSTRAINT [DF_suggest_created_at]  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[suggest] ADD  CONSTRAINT [DF_suggest_isdescription]  DEFAULT ((0)) FOR [isdescription]
GO
ALTER TABLE [dbo].[suggest] ADD  CONSTRAINT [DF_suggest_field_id]  DEFAULT ((0)) FOR [field_id]
GO
ALTER TABLE [dbo].[tag] ADD  CONSTRAINT [DF_tag_created_at]  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[token_firebase] ADD  CONSTRAINT [DF_token_firebase_created_at]  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[token_firebase] ADD  CONSTRAINT [DF_token_firebase_user_id]  DEFAULT ((0)) FOR [user_id]
GO
ALTER TABLE [dbo].[train_line] ADD  CONSTRAINT [DF_train_line_created_at]  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[train_line] ADD  CONSTRAINT [DF_train_line_translation_id]  DEFAULT ((0)) FOR [translation_id]
GO
ALTER TABLE [dbo].[train_line_station] ADD  CONSTRAINT [DF_train_line_station_created_at]  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[train_line_station] ADD  CONSTRAINT [DF_train_line_station_station_id]  DEFAULT ((0)) FOR [station_id]
GO
ALTER TABLE [dbo].[train_line_station] ADD  CONSTRAINT [DF_train_line_station_train_line_id]  DEFAULT ((0)) FOR [train_line_id]
GO
ALTER TABLE [dbo].[translation] ADD  CONSTRAINT [DF_translation_created_at]  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[translation_language] ADD  CONSTRAINT [DF_translation_language_created_at]  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[translation_language] ADD  CONSTRAINT [DF_translation_language_translation_id]  DEFAULT ((0)) FOR [translation_id]
GO
ALTER TABLE [dbo].[translation_language] ADD  CONSTRAINT [DF_translation_language_language_id]  DEFAULT ((0)) FOR [language_id]
GO
ALTER TABLE [dbo].[work_background] ADD  CONSTRAINT [DF_work_background_job_seeker_id]  DEFAULT ((0)) FOR [job_seeker_id]
GO
ALTER TABLE [dbo].[work_background] ADD  CONSTRAINT [DF_work_background_created_at]  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[work_background] ADD  CONSTRAINT [DF_work_background_cv_id]  DEFAULT ((0)) FOR [cv_id]
GO
ALTER TABLE [dbo].[work_history_background] ADD  CONSTRAINT [DF_work_history_background_status]  DEFAULT ((0)) FOR [status]
GO
ALTER TABLE [dbo].[work_history_background] ADD  CONSTRAINT [DF_work_history_background_created_at]  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[work_history_background] ADD  CONSTRAINT [DF_work_history_background_work_progress_id]  DEFAULT ((0)) FOR [work_progress_id]
GO
ALTER TABLE [dbo].[work_history_cv] ADD  CONSTRAINT [DF_work_history_cv_cv_id]  DEFAULT ((0)) FOR [cv_id]
GO
ALTER TABLE [dbo].[work_history_cv] ADD  CONSTRAINT [DF_work_history_cv_form]  DEFAULT ((0)) FOR [form]
GO
ALTER TABLE [dbo].[work_history_cv] ADD  CONSTRAINT [DF_work_history_cv_status]  DEFAULT ((0)) FOR [status]
GO
ALTER TABLE [dbo].[work_history_cv] ADD  CONSTRAINT [DF_work_history_cv_created_at]  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[work_progress] ADD  CONSTRAINT [DF_work_progress_work_background_id]  DEFAULT ((0)) FOR [work_background_id]
GO
ALTER TABLE [dbo].[work_progress] ADD  CONSTRAINT [DF_work_progress_created_at]  DEFAULT (getdate()) FOR [created_at]
GO
