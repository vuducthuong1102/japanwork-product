/*
SQLyog Ultimate v11.33 (64 bit)
MySQL - 10.1.28-MariaDB 
*********************************************************************
*/
/*!40101 SET NAMES utf8 */;

create table `region` (
	`region` varchar (765),
	`furigana` varchar (765)
); 
insert into `region` (`region`, `furigana`) values('北海道','Hokkaido');
insert into `region` (`region`, `furigana`) values('東北','Tohoku');
insert into `region` (`region`, `furigana`) values('関東','Kanto');
insert into `region` (`region`, `furigana`) values('中部','Chubu');
insert into `region` (`region`, `furigana`) values('近畿','Kinki');
insert into `region` (`region`, `furigana`) values('中国','Chugoku');
insert into `region` (`region`, `furigana`) values('四国','Shikoku');
insert into `region` (`region`, `furigana`) values('九州','Kyusyu');
