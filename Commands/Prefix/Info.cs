using Akane.Engine.LevelSystem;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Akane.Commands.Prefix
{
    internal class Info : BaseCommandModule
    {
        private readonly Random random = new Random();

        [Command("testmsg")]
        [Cooldown(5, 10, CooldownBucketType.User)]
        public async Task TestCommand(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("Test Nachricht");
        }

        [Command("embedmessage1")]
        public async Task SendEmbedMessage2(CommandContext ctx) //Example 1
        {
            var embedMessage = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.Azure)
                    .WithTitle("Das ist der Titel")
                    .WithDescription("Das ist die Beschreibung"));

            await ctx.Channel.SendMessageAsync(embedMessage);
        }

        [Command("embedmessage2")]
        public async Task SendEmbedMessage1(CommandContext ctx) //Example 2
        {
            var embedMessage = new DiscordEmbedBuilder()
            {
                Title = "Das ist der Titel",
                Description = "Das ist die Beschreibung",
                Color = DiscordColor.Azure,
            };

            await ctx.Channel.SendMessageAsync(embed: embedMessage);
        }

        [Command("help")]
        [Description("Liste alle Commands von Bot auf")]
        public async Task HelpCommand(CommandContext ctx)
        {
            var randomColor = new DiscordColor((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
            // Obtaining all registered commands
            var commands = ctx.CommandsNext.RegisteredCommands;

            // Make a list of commands and their descriptions
            var categories = new Dictionary<string, List<Command>>();
            var allCommands = ctx.CommandsNext.RegisteredCommands;

            var page = 0;
            var pageSize = 10; // Anzahl der Befehle pro Seite
            var totalPages = (int)Math.Ceiling(commands.Count() / (double)pageSize);

            // Main-Page (Info)
            var helpMessage = new DiscordMessageBuilder()
                .WithEmbed(new DiscordEmbedBuilder()
                {
                    Title = $"{ctx.Client.CurrentUser.Username}'s Help Menu",
                    Description = "Informationen über __**Akane**__ \n" +
                    "__**Funktionen**__ \n" +
                    "> 5+ Systeme, z.B.: Welcome, ChatGPT, Levelsystem, Musik und vieles mehr \n\n" +
                    "Wie benutze ich __**Akane**__? \n" +
                    "> Um Befehle auszuführen, gebe `a![Command]` ein oder um alle Befehle zu sehen, gebe `a!help` ein \n\n" +
                    "__**Akane's Stats**__ \n" +
                    $"> {commands.Count()} Commands \n" +
                    $"> Seit {DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")} Online \n" +
                    $"> Ping: {ctx.Client.Ping}ms \n" +
                    "> Entwickelt von [Splay Development]() \n" +
                    $"> Developer: `꧁Saito꧂` <@{ctx.User.Id}> \n\n" +
                    "__**Wie kann ich Hilfe bekommen?**__ \n" +
                    "> Du kannst die untenstehenden Buttons verwenden, um zwischen Seiten zu wechseln",
                    Color = randomColor
                });

            var buttons = new DiscordComponent[]
            {
                new DiscordButtonComponent(ButtonStyle.Primary, "back", "◀", false),
                new DiscordButtonComponent(ButtonStyle.Primary, "next", "▶", false)
            };

            helpMessage.AddComponents(buttons);

            var msg = await ctx.Channel.SendMessageAsync(helpMessage);

            // Wait for button interactive
            while (true)
            {
                var interactivity = ctx.Client.GetInteractivity();
                var result = await interactivity.WaitForButtonAsync(msg, TimeSpan.FromMinutes(5));

                if (result.Result == null)
                    break; // Timeout

                if (result.Result.Id == "back")
                {
                    page = (page - 1 + totalPages) % totalPages;
                }
                else if (result.Result.Id == "next")
                {
                    page = (page + 1) % totalPages;
                }

                // Update the embedded message with the commands for the current page
                var pageCommands = allCommands.Skip(page * pageSize).Take(pageSize);
                var pageDescription = string.Join("\n", pageCommands.Select(cmd => $"**{cmd.Value.Name}**: {cmd.Value.Description ?? "Keine Beschreibung"}"));

                helpMessage = new DiscordMessageBuilder()
                    .WithEmbed(new DiscordEmbedBuilder()
                    {
                        Title = $"{ctx.Client.CurrentUser.Username}'s Help Menu (Seite {page + 1}/{totalPages})",
                        Description = pageDescription,
                        Color = randomColor
                    });

                helpMessage.AddComponents(buttons);
                await msg.ModifyAsync(helpMessage);
            }
        }

        [Command("profile")]
        public async Task ProfileCommand(CommandContext ctx)
        {
            string username = ctx.User.Username;
            ulong guildID = ctx.Guild.Id;

            var userDetails = new DUser()
            {
                UserName = ctx.User.Username,
                guildID = ctx.Guild.Id,
                avatarURL = ctx.User.AvatarUrl,
                Level = 1,
                XP = 0
            };

            var levelEngine = new LevelEngine();
            var doesExist = levelEngine.CheckUserExists(username, guildID);

            if (doesExist == false)
            {
                var isStored = levelEngine.StoreUserDetails(userDetails);
                if (isStored == true)
                {
                    var randomColor = new DiscordColor((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
                    var successMessage = new DiscordEmbedBuilder()
                    {
                        Title = "✅ Profil erfolgreich erstellt",
                        Description = "Bitte gebe `a!profile` nochmal, um dein Profil anzusehen",
                        Color = randomColor
                    };

                    await ctx.Channel.SendMessageAsync(successMessage);

                    var pulledUser = levelEngine.GetUser(username, guildID);

                    var profile = new DiscordMessageBuilder()
                        .AddEmbed(new DiscordEmbedBuilder()
                        .WithTitle(pulledUser.UserName + "'s Profil")
                        .WithColor(randomColor)
                        .WithThumbnail(pulledUser.avatarURL)
                        .AddField("Level", pulledUser.Level.ToString(), true)
                        .AddField("XP", pulledUser.XP.ToString(), true)
                        );

                    await ctx.Channel.SendMessageAsync(profile);
                }
                else
                {
                    var randomColor = new DiscordColor((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
                    var failedMessage = new DiscordEmbedBuilder()
                    {
                        Title = "Etwas ist beim Erstellen des Profils schiefgelaufen! Versuche es später nochmal.",
                        Color = randomColor
                    };

                    await ctx.Channel.SendMessageAsync(failedMessage);
                }
            }
            else
            {
                var pulledUser = levelEngine.GetUser(username, guildID);
                var randomColor = new DiscordColor((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));

                var profile = new DiscordMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithTitle(pulledUser.UserName + "'s Profil")
                        .WithColor(randomColor)
                        .WithThumbnail(pulledUser.avatarURL)
                        .AddField("Level", pulledUser.Level.ToString(), true)
                        .AddField("XP", pulledUser.XP.ToString(), true)
                        );

                await ctx.Channel.SendMessageAsync(profile);
            }
        }

        [Command("ping")]
        public async Task Ping(CommandContext ctx)
        {
            // Bot-Ping
            var botPing = ctx.Client.Ping;

            // Host-Ping
            var sw = Stopwatch.StartNew();
            await ctx.TriggerTypingAsync();
            sw.Stop();
            var hostPing = sw.ElapsedMilliseconds;

            // API-Latenz
            var apiLatency = hostPing - ctx.Client.Ping;

            // Bot-Version
            var botVersion = "1.0";

            // RAM
            var ramUsage = $"{(GC.GetTotalMemory(true) / (1024 * 1024)):F2} MB";

            var randomColor = new DiscordColor((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
            // Create a Embed
            var embed = new DiscordEmbedBuilder
            {
                Title = "🏓 Pong!",
                Color = randomColor,
            };

            embed.AddField("Bot Ping", $"🏓 {botPing}ms", inline: true);
            embed.AddField("Host Ping", $"🏠 {hostPing}ms", inline: true);
            embed.AddField("API Latenz", $"🌐 {apiLatency}ms", inline: true);
            embed.AddField("Bot Version", $"🤖 {botVersion}", inline: true);
            embed.AddField("RAM", $"🧠 {ramUsage}", inline: true);

            embed.WithFooter($"Angefordert von: {ctx.User.Username} am {DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}");

            await ctx.RespondAsync(embed);
        }

        private DateTime StartTime { get; set; } = DateTime.Now;

        [Command("uptime")]
        public async Task Uptime(CommandContext ctx)
        {
            // Current time
            var currentTime = DateTime.Now;

            // Calculation of uptime
            var uptime = currentTime - StartTime;

            var randomColor = new DiscordColor((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
            // Create a Embed
            var embed = new DiscordEmbedBuilder
            {
                Title = "⏳ Bot Uptime",
                Color = randomColor,
            };

            embed.AddField("Online seit", $"{uptime.Days} Tage, {uptime.Hours} Stunden, {uptime.Minutes} Minuten, {uptime.Seconds} Sekunden");
            embed.WithFooter($"Angefordert von: {ctx.User.Username} am {DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}");

            await ctx.RespondAsync(embed);
        }
    }
}
