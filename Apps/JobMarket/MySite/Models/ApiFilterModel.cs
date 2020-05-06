using System.Dynamic;


namespace MySite.Models
{
    public class ApiCommonFilterModel : ApiCommonModel
    {
        public dynamic Extensions { get; set; }

        public ApiCommonFilterModel()
        {
            Extensions = new ExpandoObject();
        }
    }
}