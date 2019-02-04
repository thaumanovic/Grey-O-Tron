﻿using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using GreyOTron.Library.Helpers;

namespace GreyOTron.Library.Commands
{
    [Command("version", CommandDescription = "Get the current bot version.", CommandOptions = CommandOptions.DirectMessage | CommandOptions.DiscordServer)]
    public class VersionCommand : ICommand
    {
        public async Task Execute(SocketMessage message)
        {
            await message.Author.SendMessageAsync($"Current version: {VersionResolver.Get()}");
        }

        public string Arguments { get; set; }
    }
}
