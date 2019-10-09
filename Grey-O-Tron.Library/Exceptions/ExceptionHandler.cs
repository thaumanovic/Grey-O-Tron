﻿using System;
using System.Collections.Generic;
using System.Linq;
using Discord;
using Discord.WebSocket;
using GreyOTron.Library.Helpers;
using Microsoft.ApplicationInsights;

namespace GreyOTron.Library.Exceptions
{
    public static class ExceptionHandler
    {
        public static void HandleException(TelemetryClient log, Exception e, IUser user, string content = null)
        {
            var properties = new Dictionary<string, string>();

            if (e is InvalidKeyException invalidKeyException)
            {
                properties = invalidKeyException.AsDictionary();
                properties.Add("UserId", user.UserId());
                if (content != null) { properties.Add("Command", content); }
                log.TrackTrace("Invalid key", properties);
                return;
            }

            if (e is ApiInformationForUserByKeyException apiInformationForUserByKeyException)
            {
                properties = apiInformationForUserByKeyException.AsDictionary();
            }

            properties.Add("UserId", user.UserId());
            if (content != null) { properties.Add("Command", content); }
            if (user is SocketGuildUser guildUser)
            {
                properties.Add("ServerName", guildUser.Guild.Name);
                properties.Add("ServerId", guildUser.Guild.Id.ToString());
                properties.Add("IsAdmin", guildUser.IsAdmin().ToString());
                properties.Add("Roles", guildUser.Roles.Aggregate("", (s, role) => $"{s}{role.Name}, ", r => r.TrimEnd(' ', ',')));
            }

            log.TrackException(e, properties);
        }
    }
}
