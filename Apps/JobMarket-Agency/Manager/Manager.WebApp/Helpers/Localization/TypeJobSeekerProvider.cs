using Manager.WebApp.Caching;
using Manager.WebApp.Resources;
using System.Collections.Generic;

namespace Manager.WebApp
{
    public class TypeJobSeekerProvider
    {
        public static List<TypeJobSeeker> GetLists()
        {
            return new List<TypeJobSeeker> {
                new TypeJobSeeker {
                    TypeName = ManagerResource.LB_CANDIDATE_ONLINE, Id = 0
                },
                new TypeJobSeeker {
                    TypeName = ManagerResource.LB_CANDIDATE_OFFLINE, Id = 1
                }
            };
        }

        public class TypeJobSeeker
        {
            public string TypeName
            {
                get;
                set;
            }
            public int Id { get; set; }

        }
    }
}