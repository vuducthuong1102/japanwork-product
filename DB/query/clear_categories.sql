delete from field;
delete from field_lang;
delete from sub_field;
delete from sub_field_lang;
delete from industry;
delete from industry_lang;
delete from sub_industry;
delete from sub_industry_lang;

DBCC CHECKIDENT('field', RESEED, 0)
DBCC CHECKIDENT('field_lang', RESEED, 0)
DBCC CHECKIDENT('sub_field', RESEED, 0)
DBCC CHECKIDENT('sub_field_lang', RESEED, 0)
DBCC CHECKIDENT('industry', RESEED, 0)
DBCC CHECKIDENT('industry_lang', RESEED, 0)
DBCC CHECKIDENT('sub_industry', RESEED, 0)
DBCC CHECKIDENT('sub_industry_lang', RESEED, 0)