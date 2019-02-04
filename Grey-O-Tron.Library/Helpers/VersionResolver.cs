﻿using System.Reflection;

namespace GreyOTron.Library.Helpers
{
    public static class VersionResolver
    {
        public static string Get()
        {
            var v = Assembly.GetEntryAssembly().GetName().Version;
            return $"{v.Major}.{v.Minor}.{v.Build}";
        }
    }
}
