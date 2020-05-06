using MsSql.AspNet.Identity.Entities;
using System.Collections.Generic;

namespace Manager.WebApp.Models
{
    public class ProjectModel
    {
        public IdentityProject ProjectInfo { get; set; }
    }

    public class NewestProjectsModel
    {
        public List<IdentityProject> Projects { get; set; }
        public List<IdentityProjectCategory> Categories { get; set; }
        public string LangCode { get; set; }
    }

    public class RelatedProjectsModel
    {
        public List<IdentityProject> Projects { get; set; }
        public string LangCode { get; set; }
    }
}