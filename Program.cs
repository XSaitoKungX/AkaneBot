using Akane.Commands.Prefix;
using Akane.Commands.SlashCommands;
using Akane.config;
using Akane.Engine;
using Akane.Engine.ImageHandler;
using Akane.Engine.LevelSystem;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Lavalink;
using DSharpPlus.Net;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Akane
{
    internal class Program
    {
        // Main Discord Properties
        public static DiscordClient Client { get; set; }
        public static CommandsNextExtension Commands {  get; set; }

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
            Commands.RegisterCommands<Music>();
            Commands.RegisterCommands<DiscordComponentCommands>();
            Commands.RegisterCommands<UserRequestedCommands>();

            // Slash Commands
            slashCommandsConfig.RegisterCommands<FunSL>();
            slashCommandsConfig.RegisterCommands<ModerationSL>();

            // Error event handlers
            Commands.CommandErrored += OnCommandError;

            // Lavalink Configuration
            var endpoint = new ConnectionEndpoint
            {
                Hostname = "oce-lavalink.lexnet.cc",
                Port = 443,
                Secured = true,
            };

            var lavalinkConfig = new LavalinkConfiguration
            {
                Password = "lexn3tl@val!nk",
                RestEndpoint = endpoint,
                SocketEndpoint = endpoint,
            };

            var lavalink = Client.UseLavalink();

            // Connect to the Client and get the Bot online
            await Client.ConnectAsync();
            await lavalink.ConnectAsync(lavalinkConfig);
            await Task.Delay(-1);
        }

        private static Task Client_Ready(DiscordClient sender, DSharpPlus.EventArgs.ReadyEventArgs args)
        {
            return Task.CompletedTask;
        }

        private static async Task UserJoinHandler(DiscordClient sender, DSharpPlus.EventArgs.GuildMemberAddEventArgs args)
        {
            var defaultChannel = args.Guild.GetDefaultChannel();

            var welcomeEmbed = new DiscordEmbedBuilder()
            {
                Title = $"Willkommen {args.Member.Username} auf dem Server!",
                Description = "Wir hoffen, dass du bei uns einen guten Start bei uns hast. Vergiss nicht unsere Regeln durchzulesen!",
                Color = DiscordColor.Green,
            };

            await defaultChannel.SendMessageAsync(welcomeEmbed);
        }

        private static async Task VoiceChannelHandler(DiscordClient sender, DSharpPlus.EventArgs.VoiceStateUpdateEventArgs args)
        {
            var channel = args.Channel;
            var mainUserName = args.User.Username;

            if (channel != null && channel.Name == "Create" && args.Before == null) // Joining a VC
            {
                Console.WriteLine($"Voice Channel {channel.Name} beigetreten");

                // Creating the Voice Channel
                var userVC = await args.Guild.CreateVoiceChannelAsync($"{args.User.Username}'s Channel", args.Channel.Parent);
                voiceChannelIDs.Add(args.User.Username, userVC.Id);

                var member = await args.Guild.GetMemberAsync(args.User.Id);
                await member.ModifyAsync(x => x.VoiceChannel = userVC);
            }
            if (args.User.Username == mainUserName && channel == null && args.Before != null && args.Before.Channel != null && args.Before.Channel.Name == $"{args.User.Username}'s Channel") // Leaving the VC
            {
                Console.WriteLine($"Aus dem Voice Channel {args.Before.Channel.Name} gegangen!");
                var channelID = voiceChannelIDs.TryGetValue(args.User.Username, out ulong channelToDelete);
                var leftChannel = args.Guild.GetChannel(channelToDelete);
                await leftChannel.DeleteAsync();

                voiceChannelIDs.Remove(args.User.Username);
            }
        }

        private static async Task ModalEventHandler(DiscordClient sender, DSharpPlus.EventArgs.ModalSubmitEventArgs args)
        {
            if (args.Interaction.Type == InteractionType.ModalSubmit && args.Interaction.Data.CustomId == "modal")
            {
                var values = args.Values;
                await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"{args.Interaction.User.Username} hat ein Modal mit der Eingabe {values.Values.First()} übermittelt."));
            }
        }

        private static async Task MessageSendHandler(DiscordClient sender, DSharpPlus.EventArgs.MessageCreateEventArgs args)
        {
            // Swear Filter
            var swearFilter = new SwearFilter();
            foreach (var word in swearFilter.filter)
            {
                if (args.Message.Content.Contains(word))
                {
                    await args.Channel.SendMessageAsync("Diese Nachricht enthielt ein Schimpfwort. Du wurdest gewarnt!");
                }
            }

            // Image Counter Reset
            if (args.Message.Content == "a!image")
            {
                ImageIDCounter = 0; // Reset the counter when someone uses this command
            }

            // Profile System
            var levelEngine = new LevelEngine();
            var addedXP = levelEngine.AddXP(args.Author.Username, args.Guild.Id);

            if (levelEngine.levelledUp == true)
            {
                var levelledUpEmbed = new DiscordEmbedBuilder()
                {
                    Title = $"Glückwunsch, {args.Author.Username}!! Dein Level ist aufgestiegen!! 🥳🎉",
                    Description = "Level: " + levelEngine.GetUser(args.Author.Username, args.Guild.Id).Level.ToString(),
                    Color = DiscordColor.Chartreuse
                };

                await args.Channel.SendMessageAsync(args.Author.Mention, levelledUpEmbed);
            }
        }

        private static async Task InteractionEventHandler(DiscordClient sender, DSharpPlus.EventArgs.ComponentInteractionCreateEventArgs args)
        {
            // Drop-Down Events
            if (args.Id == "dropDownList" && args.Interaction.Data.ComponentType == ComponentType.StringSelect)
            {
                var options = args.Values;

                foreach (var option in options)
                {
                    switch (option)
                    {
                        case "option1":
                            await args.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().WithContent($"{args.User.Username} hat die Option 1 gewählt"));
                            break;

                        case "option2":
                            await args.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().WithContent($"{args.User.Username} hat die Option 2 gewählt"));
                            break;

                        case "option3":
                            await args.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().WithContent($"{args.User.Username} hat die Option 3 gewählt"));
                            break;
                    }
                }
            }

            else if (args.Id == "channelDropDownList")
            {
                var options = args.Values;

                foreach (var channel in options)
                {
                    var selectedChannel = await Client.GetChannelAsync(ulong.Parse(channel));
                    await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"{args.User.Username} hat den Channel {selectedChannel.Name} ausgewählt"));
                }
            }

            else if (args.Id == "mentionDropDownList")
            {
                var options = args.Values;
                foreach (var user in options)
                {
                    var selectedUser = await Client.GetUserAsync(ulong.Parse(user));
                    await args.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().WithContent($"{selectedUser.Mention} wurde erwähnt"));
                }
            }

            // Button Events
            if (args.Interaction.Data.CustomId == "1")
            {
                var options = args.Values;
                
                foreach (var option in options)
                {
                    switch (option)
                    {
                        case "option1":
                            await args.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().WithContent($"Du hast die Option 1 gewählt"));
                            break;

                        case "option2":
                            await args.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().WithContent($"Du hast die Option 2 gewählt"));
                            break;

                        case "option3":
                            await args.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().WithContent($"Du hast die Option 3 gewählt"));
                            break;
                    }
                }
            }

            // Buttons
            switch (args.Interaction.Data.CustomId)
            {
                case "1":
                    await args.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().WithContent("Du hast der 1. Button geklickt"));
                    break;

                case "2":
                    await args.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().WithContent("Du hast der 2. Button geklickt"));
                    break;

                case "funButton":
                    string funCommandsList = "a!message -> Send a message \n" +
                        "a!embedmessage1 -> Sends an embed message \n" +
                        "a!poll -> Starts a poll";

                    await args.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().WithContent(funCommandsList));
                    break;

                case "previousButton":
                    ImageIDCounter--; // Decrement the ID by 1 to get the ID for the previous image
                    string imageURL = Program.imageHandler.GetImageAtId(ImageIDCounter); // Get the image from the Dictionary

                    // Initialise the Buttons again
                    var previousEmoji = new DiscordComponentEmoji(DiscordEmoji.FromName(Client, ":track_previous:"));
                    var previousButton = new DiscordButtonComponent(ButtonStyle.Primary, "previousButton", "Previous", false, previousEmoji);

                    var nextEmoji = new DiscordComponentEmoji(DiscordEmoji.FromName(Client, ":track_next"));
                    var nextButton = new DiscordButtonComponent(ButtonStyle.Primary, "nextButton", "Next", false, nextEmoji);

                    // Send the new image as a response to the button press, replacing the previous image
                    var imageMessage = new DiscordMessageBuilder()
                        .AddEmbed(new DiscordEmbedBuilder()
                        .WithColor(DiscordColor.Azure)
                        .WithTitle("Results")
                        .WithImageUrl(imageURL)
                        .WithFooter("Page " + ImageIDCounter))
                        .AddComponents(previousButton, nextButton);

                    await args.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().AddEmbed(imageMessage.Embed).AddComponents(imageMessage.Components));
                    break;

                case "nextButton":
                    ImageIDCounter++; //Same idea but this time you increment the counter by 1 to get the next image
                    string imageURL1 = Program.imageHandler.GetImageAtId(ImageIDCounter);

                    var previousEmoji1 = new DiscordComponentEmoji(DiscordEmoji.FromName(Client, ":track_previous:"));
                    var previousButton1 = new DiscordButtonComponent(ButtonStyle.Primary, "previousButton", "Previous", false, previousEmoji1);

                    var nextEmoji1 = new DiscordComponentEmoji(DiscordEmoji.FromName(Client, ":track_next:"));
                    var nextButton1 = new DiscordButtonComponent(ButtonStyle.Primary, "nextButton", "Next", false, nextEmoji1);

                    var imageMessage1 = new DiscordMessageBuilder()
                        .AddEmbed(new DiscordEmbedBuilder()
                        .WithColor(DiscordColor.Azure)
                        .WithTitle("Results")
                        .WithImageUrl(imageURL1)
                        .WithFooter("Page " + ImageIDCounter)
                        )
                        .AddComponents(previousButton1, nextButton1);
                    await args.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().AddEmbed(imageMessage1.Embed).AddComponents(imageMessage1.Components));

                    break;

                default:
                    Console.WriteLine("Keine Buttons mit dieser ID gefunden!");
                    break;
            }
        }

        private static async Task OnCommandError(CommandsNextExtension sender, CommandErrorEventArgs args)
        {
            // Casting ErrorEventArgs as a ChecksFailedException
            if (args.Exception is ChecksFailedException castedException)
            {
                string cooldownTimer = string.Empty;

                foreach (var check in castedException.FailedChecks)
                {
                    var cooldown = (CooldownAttribute)check; // The cooldown that has triggered this method
                    TimeSpan timeLeft = cooldown.GetRemainingCooldown(args.Context); // Getting the remaining time on this cooldown
                    cooldownTimer = timeLeft.ToString(@"hh\:mm\:ss");
                }

                var cooldownMessage = new DiscordEmbedBuilder()
                {
                    Title = "Du bist im Cooldown-Modus. Bitte warte, bis der Cooldown abläuft",
                    Description = "Verbleibende Zeit: " + cooldownTimer,
                    Color = DiscordColor.Red
                };

                await args.Context.Channel.SendMessageAsync(cooldownMessage);
            }
        }
    }
}
