﻿using System;
using System.Collections.Generic;
using Microsoft.AspNet.Identity;

namespace MsSql.AspNet.Identity
{
    public class IdentityUser : IUser
    {
        public virtual string Id { get; set; }
        public virtual string UserName { get; set; }
        public virtual string PasswordHash { get; set; }
        public virtual string SecurityStamp { get; set; }
        public virtual string Email { get; set; }
        public virtual string PhoneNumber { get; set; }
        public virtual bool EmailConfirmed { get; set; }
        public virtual bool PhoneNumberConfirmed { get; set; }
        public virtual int AccessFailedCount { get; set; }
        public virtual bool LockoutEnabled { get; set; }
        public virtual DateTime? LockoutEndDateUtc { get; set; }
        public virtual DateTime CreatedDateUtc { get; set; }
        public virtual bool TwoFactorEnabled { get; set; }
        public virtual int ProviderId { get; set; }
       
        public virtual string Code { get; set; }
        public virtual string Avatar { get; set; }
        public virtual List<string> Roles { get; set; }
        public virtual List<IdentityUserClaim> Claims { get; set; }
        public virtual List<UserLoginInfo> Logins { get; set; }

        public virtual string FullName { get; set; }
        public virtual int StaffId { get; set; }

        public virtual int ParentId { get; set; }
        public string HashingData { get; set; }
        public int TotalCount { get; set; }
        public IdentityUser()
        {
            this.Claims = new List<IdentityUserClaim>();
            this.Roles = new List<string>();
            this.Logins = new List<UserLoginInfo>();            
            this.Id = Guid.NewGuid().ToString();
            LockoutEnabled = true;
        }

        public IdentityUser(string userName)
            : this()
        {
            this.UserName = userName;
        }
    }

    public sealed class IdentityUserLogin
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Provider { get; set; }
        public string ProviderKey { get; set; }
    }

    public class IdentityUserClaim
    {
        public virtual string ClaimType { get; set; }
        public virtual string ClaimValue { get; set; }
    }
}
