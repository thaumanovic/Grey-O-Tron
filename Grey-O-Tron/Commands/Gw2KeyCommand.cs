﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using GreyOTron.ApiClients;
using GreyOTron.Helpers;
using GreyOTron.TableStorage;
using Microsoft.Extensions.Configuration;

namespace GreyOTron.Commands
{
    [Command("gw2-key")]
    public class Gw2KeyCommand : ICommand
    {
        private readonly Gw2Api _gw2Api;
        private readonly KeyRepository _gw2KeyRepository;
        private readonly IConfigurationRoot _configuration;
        private readonly VerifyUser _verifyUser;

        public Gw2KeyCommand(Gw2Api gw2Api, KeyRepository gw2KeyRepository, IConfigurationRoot configuration, VerifyUser verifyUser)
        {
            _gw2Api = gw2Api;
            _gw2KeyRepository = gw2KeyRepository;
            _configuration = configuration;
            _verifyUser = verifyUser;
        }

        public async Task Execute(SocketMessage socketMessage)
        {
            if (socketMessage.Author.Id == 291207609791283212)
            {
                await socketMessage.Author.SendMessageAsync("Go back to your own corner pleb!");
            }

            var key = Arguments;
            var acInfo = _gw2Api.GetInformationForUserByKey(key);
            if (acInfo.TokenInfo != null && acInfo.TokenInfo.Name == $"{socketMessage.Author.Username}#{socketMessage.Author.Discriminator}")
            {
                await _gw2KeyRepository.Set(new DiscordClientWithKey("Gw2", socketMessage.Author.Id.ToString(),
    $"{socketMessage.Author.Username}#{socketMessage.Author.Discriminator}",
    key));

                if (socketMessage.Author is SocketGuildUser guildUser)
                {
                    await _verifyUser.Verify(acInfo, guildUser);
                }
                else
                {
                    await socketMessage.Author.SendMessageAsync($"Your key has been stored, don't forget to use {_configuration["command-prefix"]}gw2-verify on the server you whish to get verified on.");
                }
            }
            else
            {
                await socketMessage.Author.SendMessageAsync($"Please make sure your GW2 application key's name is the same as your discord username: {socketMessage.Author.Username}#{socketMessage.Author.Discriminator}");
                await socketMessage.Author.SendMessageAsync("You can view, create and edit your GW2 application key's on https://account.arena.net/applications");
            }

            await socketMessage.Channel.DeleteMessagesAsync(new List<SocketMessage> { socketMessage });
        }

        public string Arguments { get; set; }
    }
}
