﻿using GreyOTron.Library.Helpers;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;

namespace GreyOTron.Library.ApiClients
{
    public class DiscordBotsApi
    {
        private readonly IConfiguration configuration;
        private const string BaseUrl = "https://discordbots.org/api";

        public DiscordBotsApi(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void UpdateStatistics(Statistics statistics)
        {
            var client = new RestClient(BaseUrl);
            client.AddDefaultHeader("Authorization", configuration["DiscordBotsToken"]);
            var request = new RestRequest($"bots/{configuration["DiscordBotId"]}/stats") {JsonSerializer = new JsonConvertRestSharpSerializer()};
            request.AddJsonBody(statistics);
            client.Execute(request, Method.POST);
        }
    }

    public class Statistics
    {
        [JsonProperty("server_count")]
        public int ServerCount { get; set; }
    }
}
