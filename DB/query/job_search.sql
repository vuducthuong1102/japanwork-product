USE [job_market]
GO

DECLARE	@return_value int

EXEC	@return_value = [dbo].[Job_SearchByPage]
		@keyword = N'',
		@employment_type_id = 0,
		@field_id = 0,
		@language_code = N'vi-VN',
		@salary_min = 0,
		@salary_max = 0,
		@city_ids = N'',
		@prefecture_ids = N'',
		@japanese_level_number = 0,
		@station_ids = N'',		
		@sub_industry_ids = N'',
		@sub_field_ids = N'',
		@job_seeker_id = 0,
		@sorting_date = 'asc',
		@offset = 0,
		@page_size = 20

SELECT	'Return Value' = @return_value

GO
