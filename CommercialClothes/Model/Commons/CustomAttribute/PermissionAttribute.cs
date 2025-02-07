﻿using Microsoft.AspNetCore.Mvc;

namespace CommercialClothes.Commons.CustomAttribute
{
    public class PermissionAttribute : TypeFilterAttribute
    {
        public PermissionAttribute(string role) : base(typeof(PermissionFilter))
        {
            Arguments = new object[] { role };
        }
    }
}