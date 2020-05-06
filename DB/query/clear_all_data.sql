DELETE FROM A_job_seeker;  
DBCC CHECKIDENT ('A_job_seeker', RESEED, 1);

DELETE FROM A_job_seeker_address;  
DBCC CHECKIDENT ('A_job_seeker_address', RESEED, 1);

DELETE FROM A_job_seeker_certificate;  
DBCC CHECKIDENT ('A_job_seeker_certificate', RESEED, 1);

DELETE FROM A_job_seeker_edu_history;  
DBCC CHECKIDENT ('A_job_seeker_edu_history', RESEED, 1);

DELETE FROM A_job_seeker_wish;  
DBCC CHECKIDENT ('A_job_seeker_wish', RESEED, 1);

DELETE FROM A_job_seeker_work_history;  
DBCC CHECKIDENT ('A_job_seeker_work_history', RESEED, 1);

DELETE FROM A_notification;  
DBCC CHECKIDENT ('A_notification', RESEED, 1);

DELETE FROM A_notification_config;  
DBCC CHECKIDENT ('A_notification_config', RESEED, 1);

DELETE FROM A_notification_detail;  
DBCC CHECKIDENT ('A_notification_detail', RESEED, 1);

DELETE FROM A_schedule;  
DBCC CHECKIDENT ('A_schedule', RESEED, 1);

DELETE FROM agency_company;  
DBCC CHECKIDENT ('agency_company', RESEED, 1);

DELETE FROM application;  
DBCC CHECKIDENT ('application', RESEED, 1);

DELETE FROM candidate;  
DBCC CHECKIDENT ('candidate', RESEED, 1);

DELETE FROM company;  
DBCC CHECKIDENT ('company', RESEED, 1);

DELETE FROM company_lang;  
DBCC CHECKIDENT ('company_lang', RESEED, 1);

DELETE FROM company_note;  
DBCC CHECKIDENT ('company_note', RESEED, 1);

--DELETE FROM cs;  
--DBCC CHECKIDENT ('cs', RESEED, 1);

--DELETE FROM cs_address;  
--DBCC CHECKIDENT ('cs_address', RESEED, 1);

--DELETE FROM cs_certificate;  
--DBCC CHECKIDENT ('cs_certificate', RESEED, 1);

--DELETE FROM cs_edu_history;  
--DBCC CHECKIDENT ('cs_edu_history', RESEED, 1);

--DELETE FROM cs_work_history;  
--DBCC CHECKIDENT ('cs_work_history', RESEED, 1);

--DELETE FROM cs_work_history_detail;  
--DBCC CHECKIDENT ('cs_work_history_detail', RESEED, 1);

DELETE FROM friend_invitation;  
DBCC CHECKIDENT ('friend_invitation', RESEED, 1);

DELETE FROM friend_invitation_master;  
DBCC CHECKIDENT ('friend_invitation_master', RESEED, 1);

DELETE FROM interview_process;  
DBCC CHECKIDENT ('interview_process', RESEED, 1);

DELETE FROM invitation;  
DBCC CHECKIDENT ('invitation', RESEED, 1);

DELETE FROM invitation_master;  
DBCC CHECKIDENT ('invitation_master', RESEED, 1);

DELETE FROM job;  
DBCC CHECKIDENT ('job', RESEED, 1);

DELETE FROM job_address;  
DBCC CHECKIDENT ('job_address', RESEED, 1);

DELETE FROM job_address_station;  
DBCC CHECKIDENT ('job_address_station', RESEED, 1);

DELETE FROM job_alert;  
DBCC CHECKIDENT ('job_alert', RESEED, 1);

DELETE FROM job_sub_industry;  
DBCC CHECKIDENT ('job_sub_industry', RESEED, 1);

DELETE FROM job_sub_field;  
DBCC CHECKIDENT ('job_sub_field', RESEED, 1);

DELETE FROM job_tag;  
DBCC CHECKIDENT ('job_tag', RESEED, 1);

DELETE FROM job_translation;  
DBCC CHECKIDENT ('job_translation', RESEED, 1);

DELETE FROM notification;  
DBCC CHECKIDENT ('notification', RESEED, 1);

DELETE FROM notification_detail;  
DBCC CHECKIDENT ('notification_detail', RESEED, 1);

DELETE FROM tag;  
DBCC CHECKIDENT ('tag', RESEED, 1);

DELETE FROM saved_job;  
DBCC CHECKIDENT ('saved_job', RESEED, 1);

