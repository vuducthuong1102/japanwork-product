using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailConsoler.DataStorage
{
    public class VodXMLEntity 
    {
        public string XMLFileId { get; set; }
        public string XmlFileName { get; set; }
        public DateTime XMLCreatedDate { get; set; }
        public DateTime? XMLUpdatedDate { get; set; }
        public string XMLAckStatus { get; set; }
        public DateTime? XMLAckDate { get; set; }
        public string XMLAckMessage { get; set; }

        public int Status { get; set; }
        public string StatusDesc { get; set; }
        public int NumLine { get; set; }
    }
   
    public class GenreXMLEntity : VodXMLEntity
    {
        public string GenreIdValues { get; set; }
    }

    public class GenreEntity : GenreXMLEntity
    {
        public string GenreId { get; set; }
        public string ParentId { get; set; }
        public string Title_EN { get; set; }
        public string Title_VI { get; set; }
        public DateTime? CreationDateTime { get; set; }
        public DateTime? PublishDateTime { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }        
    }

    public class ContainerEntity : VodXMLEntity
    {
        public string ContainerId { get; set; }
        public string Name { get; set; }
        public int IsHD { get; set; }
        public int IsFree { get; set; }
        public DateTime? CreationDateTime { get; set; }
        public DateTime? PublishDateTime { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public string Title_EN { get; set; }
        public string Title_VI { get; set; }
        public string Summary_EN { get; set; }
        public string Summary_VI { get; set; }
        public string Rating { get; set; }
        public string DisplayRunTime { get; set; }
        public int Year { get; set; }
        public string CountryOfOrigin { get; set; }
        public string Genre { get; set; }
        public string Subgenre { get; set; }
        public string SubGenres { get; set; }
        public string Image { get; set; }      
    }

    public class CollectionEntity : VodXMLEntity
    {
        public string CollectionId { get; set; }
        public string ContainerId { get; set; }
        public string Name { get; set; }
        public DateTime? CreationDateTime { get; set; }
        public DateTime? PublishDateTime { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public string Title_EN { get; set; }
        public string Title_VI { get; set; }
        public string Summary_EN { get; set; }
        public string Summary_VI { get; set; }
        public string Rating { get; set; }
        public string DisplayRunTime { get; set; }
        public int Year { get; set; }
        public string CountryOfOrigin { get; set; }
        public string Genre { get; set; }
        public string Subgenre { get; set; }
        public string SubGenres { get; set; }
        public string Image { get; set; }
    }

    public class ProgramEntity : VodXMLEntity
    {
        public string ProgramId { get; set; }
        public string CollectionId { get; set; }
        public string ContainerId { get; set; }
        public string Name { get; set; }
        public int IsHD { get; set; }
        public int IsFree { get; set; }
        public DateTime? CreationDateTime { get; set; }
        public DateTime? PublishDateTime { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public DateTime? TopContentStart { get; set; }
        public DateTime? TopContentEnd { get; set; }
        public string Title_EN { get; set; }
        public string Title_VI { get; set; }
        public string Summary_EN { get; set; }
        public string Summary_VI { get; set; }
        public string Rating { get; set; }
        public string DisplayRunTime { get; set; }
        public int Year { get; set; }
        public string CountryOfOrigin { get; set; }
        public string Genre { get; set; }
        public string Subgenre { get; set; }
        public string SubGenres { get; set; }
        public string Image { get; set; }

        public string Actors { get; set; }
        public string Directors { get; set; }
        public string Producers { get; set; }
        public string EpisodeName_EN { get; set; }
        public string EpisodeName_VI { get; set; }
        public string EpisodeID { get; set; }
        public string MovieFileName { get; set; }
        public string MovieFileSize { get; set; }
        public string MovieCheckSum { get; set; }
        public int MovieDuration { get; set; }      
    }

    public class SubscriptionPackageEntity {
        public string PackageId { get; set; }
        public int IsHD { get; set; }
        public int IsFree { get; set; }
        public string PackageRef { get; set; }
        public int Active { get; set; }
    }

    public class ContainerEntityXml : ContainerEntity
    {
        public ContainerEntityXml()
        {
            Collections = new List<CollectionEntity>();
        }

        public List<CollectionEntity> Collections { get; set; }
    }

    public class CollectionEntityXml : CollectionEntity
    {
        public CollectionEntityXml()
        {
            Programs = new List<ProgramEntity>();
            Container = new ContainerEntity();
        }

        public List<ProgramEntity> Programs { get; set; }
        public ContainerEntity Container { get; set; }
    }

    public class ProgramEntityXml : ProgramEntity
    {
        public ProgramEntityXml()
        {
            Collection = new CollectionEntity();
        }

        public CollectionEntity Collection { get; set; }
    }

    public class VodMetadata
    {
        public VodMetadata()
        {
            Genres = new List<GenreEntity>();
            Packages = new List<SubscriptionPackageEntity>();
        }

        public List<GenreEntity> Genres { get; set; }
        public List<SubscriptionPackageEntity> Packages { get; set; }
    }
}
