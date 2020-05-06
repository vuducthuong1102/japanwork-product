USE [job_market]
GO

DECLARE	@return_value int

EXEC	@return_value = [dbo].A_Report_GetStatisticsApplicationByWeek
		@AgencyId = 7,
		@FromDate = N'2020-02-03 00:00:00',
		@ToDate = N'2020-02-09 00:00:00',
		@ListDays = N'13,14,15,16,17,18,19'

SELECT	'Return Value' = @return_value

GO
