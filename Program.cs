using Akane.commands;
using Akane.config;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Akane
{
    internal class Program
    {
        // Main Discord Properties
        private static DiscordClient Client { get; set; }
        private static CommandsNextExtension Commands {  get; set; }

        // Miscaleneous Properties
        private static int ImageIDCounter = 0;
        public static GoogleImageHandler imageHandler;
        private static Dictionary<string, ulong> voiceChannelIDs = new Dictionary<string, ulong>();

        static async Task Main(string[] args)
        {
            // Instantiating the class with the Instance property
            imageHandler = GoogleImageHandler.Instance;

            // Reading the Token & Prefix
            var jsonReader = new JSONReader();
            await jsonReader.ReadJSON();

            // Making a Bot Configuration with token & additional settings
            var discordConfig = new DiscordConfiguration()
            {
                Intents = DiscordIntents.All,
                Token = jsonReader.token,
                TokenType = TokenType.Bot,
                AutoReconnect = true
            };

            // Initializing the client with this config
            Client = new DiscordClient(discordConfig);

            // Setting default timeout for Interactivity based commands
            Client.UseInteractivity(new InteractivityConfiguration()
            {
                Timeout = TimeSpan.FromMinutes(2)
            });

            // Event Handlers
            Client.Ready += Client_Ready;
            Client.ComponentInteractionCreated += InteractionEventHandler;
            Client.MessageCreated += MessageSendHandler;
            Client.ModalSubmitted += ModalEventHandler;
            Client.VoiceStateUpdated += VoiceChannelHandler;
            Client.GuildMemberAdded += UserJoinHandler;

            // Setting up Commands Configuration with Prefix
            var commandsConfig = new CommandsNextConfiguration()
            {
                StringPrefixes = new string[] { jsonReader.prefix },
                EnableMentionPrefix = true,
                EnableDms = true,
                EnableDefaultHelp = false,
            };

            // Enabling the use of commands with config & also enabling use of Slash Commands
            Commands = Client.UseCommandsNext(commandsConfig);
            var slashCommandsConfig = Client.UseSlashCommands();

            // Prefix Based Commands
            Commands.RegisterCommands<Fun>();
            Commands.RegisterCommands<Info>();
            Commands.RegisterCommands<MusicCommands>();
            Commands.RegisterCommands<DiscordComponentCommands>();

            // Slash Commands
            slashCommandsConfig.RegisterCommands<FunSL>();
            slashCommandsConfig.RegisterCommands<ModerationSL>();

            // Error event handlers
            Commands.CommandErrored += OnCommandError;

            await Client.ConnectAsync();
            await Task.Delay(-1);
        }

        private static Task OnCommandError(CommandsNextExtension sender, CommandErrorEventArgs args)
        {
            throw new NotImplementedException();
        }

        private static Task UserJoinHandler(DiscordClient sender, DSharpPlus.EventArgs.GuildMemberAddEventArgs args)
        {
            throw new NotImplementedException();
        }

        private static Task VoiceChannelHandler(DiscordClient sender, DSharpPlus.EventArgs.VoiceStateUpdateEventArgs args)
        {
            throw new NotImplementedException();
        }

        private static Task ModalEventHandler(DiscordClient sender, DSharpPlus.EventArgs.ModalSubmitEventArgs args)
        {
            throw new NotImplementedException();
        }

        private static Task MessageSendHandler(DiscordClient sender, DSharpPlus.EventArgs.MessageCreateEventArgs args)
        {
            throw new NotImplementedException();
        }

        private static Task InteractionEventHandler(DiscordClient sender, DSharpPlus.EventArgs.ComponentInteractionCreateEventArgs args)
        {
            throw new NotImplementedException();
        }

        private static Task Client_Ready(DiscordClient sender, DSharpPlus.EventArgs.ReadyEventArgs args)
        {
            return Task.CompletedTask;
        }
    }
}
