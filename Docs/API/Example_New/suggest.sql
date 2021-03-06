-- phpMyAdmin SQL Dump
-- version 4.9.0.1
-- https://www.phpmyadmin.net/
--
-- Host: localhost
-- Generation Time: Sep 25, 2019 at 07:16 AM
-- Server version: 5.6.45
-- PHP Version: 7.2.19

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET AUTOCOMMIT = 0;
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `job_japan_old`
--

-- --------------------------------------------------------

--
-- Table structure for table `suggest`
--

CREATE TABLE `suggest` (
  `id` int(11) NOT NULL,
  `form` int(11) DEFAULT NULL,
  `type` int(11) DEFAULT NULL,
  `title` varchar(255) DEFAULT NULL,
  `content` text,
  `createdAt` datetime NOT NULL,
  `updatedAt` datetime NOT NULL,
  `isDescription` tinyint(1) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- Dumping data for table `suggest`
--

INSERT INTO `suggest` (`id`, `form`, `type`, `title`, `content`, `createdAt`, `updatedAt`, `isDescription`) VALUES
(7, 4, 0, '接客業 ー Công việc tiếp khách ①', '私は人と関わることが好きです。この仕事を通してたくさんの日本人と関わり、日本語を上手く話せるようになりたいです。また、接客の仕事なので正しいマナーを身に着けたいです。ですので、この仕事に応募しました。', '2019-04-18 14:25:30', '2019-04-18 14:25:30', NULL),
(8, 4, 0, '接客業 ー Công việc tiếp khách ②', '私は日本語の勉強をしています。この仕事を通して実際に日本語を使いたいです。また、たくさんの日本人と関わることで日本語を伸ばしたいです。ですので、この仕事に応募しました。', '2019-04-18 14:26:03', '2019-04-18 14:26:03', NULL),
(9, 4, 0, '仕分け ー phân loại hàng hóa', '私は生活費を稼がなければいけません。しかし、あまり日本語が上手ではないので、接客の仕事は少し難しいです。また、私は細かい作業が得意なので、自信があります。ですので、この仕事に応募しました。', '2019-04-18 14:26:37', '2019-04-18 14:26:37', NULL),
(11, 2, 0, '接客業 - Tiếp khách', '私は日本語の勉強をしています。この仕事を通して実際に日本語を使いたいです。また、たくさんの日本人と関わることで日本語を伸ばしたいです。ですので、この仕事に応募しました。', '2019-04-25 09:55:30', '2019-07-20 04:16:35', 0),
(14, 2, 0, '接客業 - Tiếp khách', '私は人と関わることが好きです。この仕事を通してたくさんの日本人と関わり、日本語を上手く話せるようになりたいです。また、接客の仕事なので正しいマナーを身に着けたいです。ですので、この仕事に応募しました。', '2019-04-30 05:42:10', '2019-07-20 03:30:24', 0),
(15, 2, 0, '仕分け - Phân loại hàng hóa', '私は生活費を稼がなければいけません。しかし、あまり日本語が上手ではないので、接客の仕事は少し難しいです。また、私は細かい作業が得意なので、自信があります。ですので、この仕事に応募しました。', '2019-04-30 05:42:27', '2019-07-20 04:15:27', 0),
(16, 2, 0, 'とにかく稼ぎたい - Muốn kiếm tiền', '私は日本へ留学に来ており、学費を稼がなければいけません。ですので、この仕事に応募しました。真面目に頑張りますのでよろしくお願いします。', '2019-04-30 05:42:44', '2019-07-20 04:14:46', 0),
(17, 0, 0, 'Sở thích kĩ năng - 1', 'プログラミング 　大学1年生のころから勉強をし始め、Web制作をしておりました。現在は、空いた時間にクラウドワークスなどを活用し、プログラミングのスキルを高めつつ、1つの趣味としても楽しんでおります。', '2019-04-30 05:43:26', '2019-04-30 05:43:26', NULL),
(18, 0, 0, 'Sở thích kĩ năng - 2', 'ハイキング 　私は冒険をすることが好きで、そのためハイキングへよく行っております。ハイキングは景色を楽しむだけではなく挑戦することもできます。また、最近では一度も行ったことのない土地へ訪ね、100キロのハイキングをしました。', '2019-04-30 05:43:45', '2019-04-30 05:43:45', NULL),
(19, 0, 0, 'Sở thích kĩ năng - 3', 'で旅行を企画すること 　私は旅行がとても好きです。その反面、持っているお金は少ないので、どうしたら安く旅行へ行くことがいつもできるか考えおりました。その結果、今では予算をなるべく抑えることが一つの特技になりました。', '2019-04-30 05:43:59', '2019-04-30 05:43:59', NULL),
(20, 0, 2, 'マーケティング', '貴社のマーケティング部門で、法人営業に取り組みたいので応募しました。企業説明会で法人営業担当の○○様に業務内容や、やりがいなどを伺い、是非貴社で働きたいと考えています。 　私は、最新のニュースや記事などの情報をいち早く把握することが好きです。また、大学でのレポートでは、情報の質についてはクラスで一番良い評価をいただきました。 　自分の強みである情報感度や課題追及を活かして、貴社では顧客の課題解決の仕事で活躍したいと考えております。', '2019-04-30 05:45:10', '2019-04-30 05:45:10', NULL),
(21, 0, 2, 'IT', '私は、貴社でエンジニアとしてアプリ開発に携わりたいので応募しました。貴社のホームページで企業理念を拝見した際、「ITで世界を変える」という理念に心を強く動かされました。 　私は、実際に○○という会社でインターンシップに参加し、そこではアプリ開発をいていました。加えてそこではアプリをより良くするために、新しい機能を提案し、開発の面もすべて任せてもらいました。 　自分のアプリ開発の経験を活かして、貴社でもより良いアプリを作るとともに、理念の達成のために貢献していきたいと考えております。', '2019-04-30 05:45:24', '2019-04-30 05:45:24', NULL),
(22, 0, 2, '事務職', '私は、社員が気持ち良く働ける環境を作るために事務職を希望しています。高校から大学在学時にかけて、運動部でマネージャーをしており、陰ながら選手のサポートに努めていました。貴社では他部門との交流が活発で風通しの良い職場であり、より広い視野を持って会社全体をサポートできる点に惹かれています。 　マネージャーの経験を活かし、より活発なコミュニケーションを促進したたり、より働きやすい会社作りをしていきたいと考えております。', '2019-04-30 05:45:40', '2019-04-30 05:45:40', NULL),
(23, 0, 1, 'PR bản thân  - 1', '私は「目標を達成するためにコツコツと努力をすること」に自信があります。この力を発揮したことは、2年間でTOEIC900点を取得したときのことです。 　この目標を立てた時、私のTOEICのスコアは600点でした。このままでは語学力が伸びず、グローバルな環境で働くことが難しくなると考え、900点を取るといった目標を立てました。これを実現するために、小さな目標をいくつか立てて自分をコントロールしました。継続的に努力をする結果、2年間で目標をことができました 　貴社で働く際でも、大きな目標に対してコツコツと努力をすることで、確実に目標を達成し大きな成果を上げたいと考えています。', '2019-04-30 05:46:12', '2019-04-30 05:46:12', NULL),
(24, 0, 1, 'PR bản thân - 2', '私の強みは、責任感をもって、与えられた役割をこなすことです。この強みを発揮したのが、インターンシップに参加し、メルマガ作成をしていたときです。 　私は、サービスを宣伝するために会員制のメルマガを作成しており、会社とユーザをつなぐほぼ唯一のチャネルとして責任感を持って作成していました。初めは、効果的なメルマガのコンテンツを作成することができませんでした。どうにか良い成果を残すために、問題点を洗い出し改善案を出しました。具体的には、過去のデータを読み込んで傾向をつかむこと、ユーザと関わることがある社員とミーティングを重ねることを行いました。その結果、メルマガの開封率やクリック数をあげることができました。 　この経験を活かして、貴社ではより自分の役割に責任を持って、成果を出すために主体的に行動していきたいです。', '2019-04-30 05:46:29', '2019-04-30 05:46:29', NULL),
(25, 0, 1, 'PR bản thân  -  3', '「未知のことに挑戦し続けること」が私のモットーです。自分をより面白い人間にするために、分からないことや困難に対して前向きに行動することができます。 　大学在学時は海外のインターンシップへ一人で行き、専門性を高めることやたくさんの人と話すことを目標にしていました。当時は、初めて一人で海外に行くことに加え、言葉が通じない環境での生活であったため、精神的にも大変でした。その中でも現地の人50人とフェイスブックを交換するという目標を立て、多くの人々に率先して話しかけました。このように、分からないことや困難に対しての姿勢という点では優れていると自負しております。 　貴社においても、この強みを活かして研鑽を積み、プロとして成果を出したいです。', '2019-04-30 05:46:51', '2019-07-10 22:08:46', 0),
(26, 2, 0, 'Giải thích', '1.　với mục này bạn sẽ viết về lý do mà bạn muốn xin vào nơi dự định xin và những điểm mạnh hay kinh nghiệm của bạn có thể phát huy được tại nơi muốn xin vào nếu có. \n2.   Bạn nên viết thành thật nhưng cũng không nên quá đơn giản, nên lựa chọn từ cho phù hợp. Ví dụ không nên viết là “vì cần tiền” mà có thể thay đổi thành “ cần phải tri trả cho cuộc sống và một phần học phí trường tiếng”.', '2019-05-01 10:17:42', '2019-07-20 04:13:15', 1),
(27, 2, 0, '家・学校から近い - Gần nhà , gần trường', '家から近いため、学校帰りや休日も通いやすいので志望しました。こちらのお店は普段から利用しており、明るい接客でしたので、私も接客の仕事を通して、コミュニケーション力やマナーを学び楽しく働きたいと考え、応募しました。週○日、授業から戻ってくる○時以降での勤務が調整しやすいです。', '2019-07-20 03:25:01', '2019-07-20 03:31:09', 0),
(28, 2, 0, '学費のため - Cần tiền học phí', '留学生のため、現在ひとり暮らしの家賃と学費で親に負担をかけており、少しでもその負担を軽くしたいと思い応募しました。アルバイトをすることで、少しでも学費の足しにできればと考えています。', '2019-07-20 03:37:21', '2019-07-20 03:37:21', NULL),
(29, 2, 0, '飲食店 - quán ăn', '私がこのお店で働きたいと思った理由は、お店の料理が大好きだからです。私は以前からこのお店に通っており、お店のファンでした。私は、料理がおいしいのは当然ですが、接客態度も飲食店の評価にはとても重要だと思います。おいしいお料理と明るい接客で、満足になって頂き、リピートしていただけるよう頑張りたいと思っています。', '2019-07-20 04:22:11', '2019-07-20 04:22:11', NULL);

--
-- Indexes for dumped tables
--

--
-- Indexes for table `suggest`
--
ALTER TABLE `suggest`
  ADD PRIMARY KEY (`id`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `suggest`
--
ALTER TABLE `suggest`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=30;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
