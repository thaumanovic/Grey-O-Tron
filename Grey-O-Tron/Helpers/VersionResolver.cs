﻿using System.Reflection;

namespace GreyOTron.Helpers
{
    public static class VersionResolver
    {
        public static string Get()
        {
            var v = Assembly.GetExecutingAssembly().GetName().Version;
            return $"{v.Major}.{v.Minor}.{v.Build}";
        }
    }
}