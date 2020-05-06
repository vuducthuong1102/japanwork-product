using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Threading.Tasks;

namespace ApiJobMarket.Services
{
    public class SampleService : ISampleService
    {
        public void DoAnyThing()
        {
            //Nothing to do
        }

        public virtual async Task<bool> IsValidAsync()
        {
            return await Task.FromResult(false);
        }
    }

    public interface ISampleService
    {
        void DoAnyThing();

        Task<bool> IsValidAsync();
    }
}