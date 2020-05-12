﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord.WebSocket;
using GreyOTron.Library.ApiClients;
using GreyOTron.Library.Exceptions;
using GreyOTron.Library.Helpers;
using GreyOTron.Library.TableStorage;
using GreyOTron.Library.Translations;
using Microsoft.ApplicationInsights;
using Newtonsoft.Json;
using Polly.CircuitBreaker;

namespace GreyOTron.Library.Commands
{
    [Command("gw2-key", CommandDescription = "Stores Guild Wars 2 key in the database.", CommandArguments = "{key}", CommandOptions = CommandOptions.DirectMessage | CommandOptions.DiscordServer)]
    public class Gw2KeyCommand : ICommand
    {
        private readonly Gw2Api gw2Api;
        private readonly KeyRepository gw2KeyRepository;
        private readonly VerifyUser verifyUser;
        private readonly TelemetryClient log;

        public Gw2KeyCommand(Gw2Api gw2Api, KeyRepository gw2KeyRepository, VerifyUser verifyUser, TelemetryClient log)
        {
            this.gw2Api = gw2Api;
            this.gw2KeyRepository = gw2KeyRepository;
            this.verifyUser = verifyUser;
            this.log = log;
        }

        public async Task Execute(SocketMessage message, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested) return;
            var key = Arguments;
            if (string.IsNullOrWhiteSpace(key))
            {
                await message.Author.InternalSendMessageAsync(nameof(GreyOTronResources.EmptyKeyTryAgain));
            }
            else
            {
                AccountInfo acInfo;
                try
                {
                    acInfo = gw2Api.GetInformationForUserByKey(key);
                }
                catch (BrokenCircuitException)
                {
                    await message.Author.InternalSendMessageAsync(nameof(GreyOTronResources.GW2ApiUnableToProcess));
                    throw;
                }
                catch (InvalidKeyException)
                {
                    await message.Author.InternalSendMessageAsync(nameof(GreyOTronResources.InvalidApiKey));
                    throw;
                }
                log.TrackTrace(message.Content, new Dictionary<string, string> { { "DiscordUser", message.Author.UserId() }, { "AccountInfo", JsonConvert.SerializeObject(acInfo, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }) } });
                if (acInfo?.TokenInfo?.Name == $"{message.Author.Username}#{message.Author.Discriminator}")
                {
                    await gw2KeyRepository.Set(new DiscordClientWithKey("Gw2", message.Author.Id.ToString(),
                        $"{message.Author.Username}#{message.Author.Discriminator}", key));

                    if (message.Author is SocketGuildUser guildUser)
                    {
                        await verifyUser.Execute(acInfo, guildUser, guildUser);
                    }
                    else
                    {
                        foreach (var guild in message.Author.MutualGuilds)
                        {
                            guildUser = guild.GetUser(message.Author.Id);
                            await verifyUser.Execute(acInfo, guildUser, guildUser);
                        }
                        await message.Author.InternalSendMessageAsync(nameof(GreyOTronResources.SuccessfullyVerified), message.Author.MutualGuilds.Aggregate("", (x, y) => $"{x}{y.Name}\n"));
                    }
                }
                else
                {
                    await message.Author.InternalSendMessageAsync(nameof(GreyOTronResources.DiscordNameDifferentFromGw2Key), message.Author.Username, message.Author.Discriminator);
                    await message.Author.InternalSendMessageAsync(nameof(GreyOTronResources.FindYourGW2ApplicationKey));
                }
            }

            if (!(message.Channel is SocketDMChannel))
            {
                await message.DeleteAsync();
            }
        }

        public string Arguments { get; set; }
        public DiscordSocketClient Client { get; set; }
    }
}
