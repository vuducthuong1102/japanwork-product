﻿using System;
using Microsoft.AspNet.Identity;

namespace MsSql.AspNet.Identity
{
    public class IdentityRole : IRole
    {
        public IdentityRole()
        {
            Id = Guid.NewGuid().ToString();
        }

        public IdentityRole(string name)
            : this()
        {
            Name = name;
        }

        public IdentityRole(string name, string id)
        {
            Name = name;
            Id = id;
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public string UserId { get; set; }
        public int Agency_Id { get; set; }
    }
}
