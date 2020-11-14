using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Corny_Bot.Commands;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Corny_Bot
{
    public class Bot
    {
        public DiscordClient Client { get; private set; }
        public CommandsNextExtension Commands { get; private set; }

        public Bot(IServiceProvider services)
        {
            var json = string.Empty;

            using (var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json =  sr.ReadToEnd();

            var configJson = JsonConvert.DeserializeObject<ConfigJson>(json);

            var config = new DiscordConfiguration
            {
                Token = configJson.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                MinimumLogLevel = LogLevel.Debug,
                //UseInternalLogHandler = true
            };
            Client = new DiscordClient(config);

            //Client.Ready += OnClientReady;

            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new string[] { configJson.Prefix },
                EnableMentionPrefix = true,
                EnableDms = true,
                DmHelp = true,
                Services = services
            };

            Commands = Client.UseCommandsNext(commandsConfig);

            Commands.RegisterCommands<PingPong>();
            Commands.RegisterCommands<CoinCommands>();

            Client.ConnectAsync();
        }

        private Task OnClientReady(ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }
    }
}
