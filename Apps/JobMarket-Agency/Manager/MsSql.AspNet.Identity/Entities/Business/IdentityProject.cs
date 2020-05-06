using System;
using System.Collections.Generic;

namespace MsSql.AspNet.Identity.Entities
{
    public class IdentityProject : CommonIdentity
    {
        public int Id { get; set; }       

        public string Cover { get; set; }

        public int CategoryId { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public int Status { get; set; }

        //Extends
        public string UrlFriendly { get; set; }
        public string CreatedDateLabel { get; set; }
        public string CategoryLabel { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string BodyContent { get; set; }
        public string LangCode { get; set; }

        public List<IdentityProjectLang> MyLanguages { get; set; }
        public List<IdentityProjectImage> Images { get; set; }

        public List<IdentityProductProperty> Properties { get; set; }
        public string[] SelectedProperties { get; set; }
        public string PropertyList { get; set; }

        public MetaDataProject MetaData { get; set; }

        public IdentityProject()
        {
            MetaData = new MetaDataProject();
            MyLanguages = new List<IdentityProjectLang>();
            Images = new List<IdentityProjectImage>();
        }     
    }

    public class MetaDataProject
    {
        public DateTime? BeginDate { get; set; }
        public DateTime? FinishDate { get; set; }
        public string PersonInCharge { get; set; }
        public string FrameWork { get; set; }
        public string Customer { get; set; }
    }
   
    public class IdentityProjectData
    {
        public int ProjectId { get; set; }
    }

    public class IdentityProjectImage : IdentityImage
    {
        public int ProjectId { get; set; }
    }

    public class IdentityProjectDetail 
    {
        public IdentityProject Project { get; set; }

        public IdentityProjectData ProjectData { get; set; }
    }


    public class IdentityProjectLang
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string BodyContent { get; set; }
        public string LangCode { get; set; }
        public string UrlFriendly { get; set; }
    }
}
