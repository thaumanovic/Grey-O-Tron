﻿using System.Globalization;
using System.Linq;
using System.Resources;
using GreyOTron.Resources;

namespace GreyOTron.Library.Helpers
{
    public class TranslationHelper
    {
        private readonly Languages languages;

        public TranslationHelper(Languages languages)
        {
            this.languages = languages;
        }

        public string Translate(ulong userId, ulong? serverId, string key, params string[] formatParameters)
        {
            var ci = languages.GetForUserId(userId);
            if (ci == null && serverId.HasValue)
            {
                ci = languages.GetForServerId(serverId.Value);
            }

            if (ci == null)
            {
                ci = CultureInfo.InvariantCulture;
            }
            
            var message = GreyOTronResources.ResourceManager.GetString(key, ci) ?? key;
            var translatedFormatParameters = formatParameters.Select(formatParameter => GreyOTronResources.ResourceManager.GetString(formatParameter, ci) ?? formatParameter).ToList();
            return string.Format(message, translatedFormatParameters.ToArray<object>());
        }
    }
}