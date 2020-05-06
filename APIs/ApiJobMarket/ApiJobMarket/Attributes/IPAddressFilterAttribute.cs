using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http.Controllers;
using System.Web;
using System.Web.Http;
using System.Net.Sockets;

using ApiJobMarket.Logging;
using ApiJobMarket.Settings;

using Microsoft.Owin;

namespace ApiJobMarket.Attributes
{

    public class IPAddressFilterAttribute : AuthorizeAttribute
    {
        private readonly ILog logger = LogProvider.GetCurrentClassLogger();

        private IEnumerable<IPAddress> ipAddresses;
        private IEnumerable<IPAddressRange> ipAddressRanges;
        private IPAddressFilteringAction filteringType;

        public IEnumerable<IPAddress> IPAddresses
        {
            get
            {
                return this.ipAddresses;
            }
        }

        public IEnumerable<IPAddressRange> IPAddressRanges
        {
            get
            {
                return this.ipAddressRanges;
            }
        }

        public IPAddressFilterAttribute()
            : this( IPAddressFilteringAction.Allow)
        {

        }

        public IPAddressFilterAttribute(IPAddressFilteringAction filteringType)
        {
            string[] addresses = ApiJobMarketSettings.AllowedIPAddresses.Split(new char[] { ',', ';' });
            ipAddresses = addresses.Select(a => IPAddress.Parse(a));
            this.filteringType = filteringType;
        }

        public IPAddressFilterAttribute(string ipAddress, IPAddressFilteringAction filteringType)
           : this(new IPAddress[] { IPAddress.Parse(ipAddress) }, filteringType)
        {

        }

        public IPAddressFilterAttribute(IPAddress ipAddress, IPAddressFilteringAction filteringType)
            : this(new IPAddress[] { ipAddress }, filteringType)
        {

        }

        public IPAddressFilterAttribute(IEnumerable<string> ipAddresses, IPAddressFilteringAction filteringType)
            : this(ipAddresses.Select(a => IPAddress.Parse(a)), filteringType)
        {

        }

        public IPAddressFilterAttribute(IEnumerable<IPAddress> ipAddresses, IPAddressFilteringAction filteringType)
        {
            this.ipAddresses = ipAddresses;
            this.filteringType = filteringType;
        }

        public IPAddressFilterAttribute(string ipAddressRangeStart, string ipAddressRangeEnd, IPAddressFilteringAction filteringType)
            : this(new IPAddressRange[] { new IPAddressRange(ipAddressRangeStart, ipAddressRangeEnd) }, filteringType)
        {

        }

        public IPAddressFilterAttribute(IPAddressRange ipAddressRange, IPAddressFilteringAction filteringType)
            : this(new IPAddressRange[] { ipAddressRange }, filteringType)
        {

        }

        public IPAddressFilterAttribute(IEnumerable<IPAddressRange> ipAddressRanges, IPAddressFilteringAction filteringType)
        {
            this.ipAddressRanges = ipAddressRanges;
            this.filteringType = filteringType;
        }

        protected override bool IsAuthorized(HttpActionContext context)
        {
            var request = context.Request;
            const string OWIN_CONTEXT = "MS_OwinContext"; //incase of WebAPI
            const string HTTP_CONTEXT = "MS_HttpContext"; //incase of WebMVC

            string ipAddressString = null;
            if (request.Properties.ContainsKey(OWIN_CONTEXT))
            {
                OwinContext owinContext = request.Properties[OWIN_CONTEXT] as OwinContext;
                if (owinContext != null)
                    ipAddressString = owinContext.Request.RemoteIpAddress;
            }

            if (string.IsNullOrEmpty(ipAddressString) && request.Properties.ContainsKey(OWIN_CONTEXT))
            {
                HttpContextWrapper httpContext = context.Request.Properties[HTTP_CONTEXT] as HttpContextWrapper;
                if (httpContext!=null)
                    ipAddressString = httpContext.Request.UserHostAddress;
            }
                                    
            var isAllowed = string.IsNullOrEmpty(ipAddressString) || IsIPAddressAllowed(ipAddressString);

            if (!isAllowed)
            {
                var actionName = context.ActionDescriptor.ActionName;
                var controllerName = context.ActionDescriptor.ControllerDescriptor.ControllerName;

                logger.ErrorFormat("The IP Address [{0}] is noth authorized for method {1}/{2}", ipAddressString, controllerName, actionName);
            }

            return isAllowed;
        }

        private bool IsIPAddressAllowed(string ipAddressString)
        {
            IPAddress ipAddress = IPAddress.Parse(ipAddressString);

            if (this.filteringType == IPAddressFilteringAction.Allow)
            {
                if (this.ipAddresses != null && this.ipAddresses.Any() &&
                    !IsIPAddressInList(ipAddressString.Trim()))
                {
                    return false;
                }
                else if (this.ipAddressRanges != null && this.ipAddressRanges.Any() &&
                    !this.ipAddressRanges.Where(r => ipAddress.IsInRange(r.StartIPAddress, r.EndIPAddress)).Any())
                {
                    return false;
                }

            }
            else
            {
                if (this.ipAddresses != null && this.ipAddresses.Any() &&
                   IsIPAddressInList(ipAddressString.Trim()))
                {
                    return false;
                }
                else if (this.ipAddressRanges != null && this.ipAddressRanges.Any() &&
                    this.ipAddressRanges.Where(r => ipAddress.IsInRange(r.StartIPAddress, r.EndIPAddress)).Any())
                {
                    return false;
                }

            }

            return true;

        }

        private bool IsIPAddressInList(string ipAddress)
        {
            if (!string.IsNullOrWhiteSpace(ipAddress))
            {
                IEnumerable<string> addresses = this.ipAddresses.Select(a => a.ToString());
                return addresses.Where(a => a.Trim().Equals(ipAddress, StringComparison.InvariantCultureIgnoreCase)).Any();
            }
            return false;
        }

    }


    public class IPAddressRange
    {
        private IPAddress startIPAddress;
        private IPAddress endIPAddress;

        public IPAddress StartIPAddress
        {
            get
            {
                return this.startIPAddress;
            }
        }

        public IPAddress EndIPAddress
        {
            get
            {
                return this.endIPAddress;
            }
        }

        public IPAddressRange(string startIPAddress, string endIPAddress)
            : this(IPAddress.Parse(startIPAddress), IPAddress.Parse(endIPAddress))
        {

        }
        public IPAddressRange(IPAddress startIPAddress, IPAddress endIPAddress)
        {
            this.startIPAddress = startIPAddress;
            this.endIPAddress = endIPAddress;
        }
    }

    public enum IPAddressFilteringAction
    {
        Allow,
        Restrict
    }

    public static class IPAddressExtensions
    {
        public static bool IsInRange(this IPAddress address, IPAddress start, IPAddress end)
        {

            AddressFamily addressFamily = start.AddressFamily;
            byte[] lowerBytes = start.GetAddressBytes();
            byte[] upperBytes = end.GetAddressBytes();

            if (address.AddressFamily != addressFamily)
            {
                return false;
            }

            byte[] addressBytes = address.GetAddressBytes();

            bool lowerBoundary = true, upperBoundary = true;

            for (int i = 0; i < lowerBytes.Length &&
                (lowerBoundary || upperBoundary); i++)
            {
                if ((lowerBoundary && addressBytes[i] < lowerBytes[i]) ||
                    (upperBoundary && addressBytes[i] > upperBytes[i]))
                {
                    return false;
                }

                lowerBoundary &= (addressBytes[i] == lowerBytes[i]);
                upperBoundary &= (addressBytes[i] == upperBytes[i]);
            }

            return true;
        }
    }
}