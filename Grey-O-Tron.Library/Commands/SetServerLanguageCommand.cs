﻿using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using GreyOTron.Library.Attributes;
using GreyOTron.Library.Extensions;
using GreyOTron.Library.Interfaces;
using GreyOTron.Library.Models;
using GreyOTron.Library.Services;
using GreyOTron.Resources;

namespace GreyOTron.Library.Commands
{
    [Command("set-server-language", CommandDescription = "Set this server's preferred language. (Ignored when user has preferred language set!)", CommandArguments = "{language (en|fr|nl|de)}", CommandOptions = CommandOptions.DiscordServer | CommandOptions.RequiresAdmin)]
    public class SetServerLanguageCommand : ICommand
    {
        private readonly IDiscordServerRepository discordServerRepository;
        private readonly LanguagesService languages;

        public SetServerLanguageCommand(IDiscordServerRepository discordServerRepository, LanguagesService languages)
        {
            this.discordServerRepository = discordServerRepository;
            this.languages = languages;
        }


        public async Task Execute(IMessage message, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested) return;
            var guildUser = (IGuildUser)message.Author;
            var language = Arguments.Trim().ToLowerInvariant();
            var languageExists = languages.Exists(language);
            if (languageExists)
            {
                await discordServerRepository.InsertOrUpdate(new DiscordServerDto
                { Id = guildUser.Guild.Id, Name = guildUser.Guild.Name, PreferredLanguage = language });
                languages.UpdateForServerId(guildUser.Guild.Id, language);
                await guildUser.InternalSendMessageAsync(nameof(GreyOTronResources.LanguageSet), language, guildUser.Guild.Name);
            }
            else
            {
                await guildUser.InternalSendMessageAsync(nameof(GreyOTronResources.InvalidLanguage), language);
            }
        }

        public string Arguments { get; set; }
        public DiscordSocketClient Client { get; set; }
    }
}
