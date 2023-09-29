using Akane.Engine.LevelSystem;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public async Task HelpCommand(CommandContext ctx)
        {
            var funButton = new DiscordButtonComponent(DSharpPlus.ButtonStyle.Success, "funButton", "Fun");
            var infoButton = new DiscordButtonComponent(DSharpPlus.ButtonStyle.Success, "InfoButton", "Info");
            var musicButton = new DiscordButtonComponent(DSharpPlus.ButtonStyle.Success, "MusicButton", "Music");
            var aiButton = new DiscordButtonComponent(DSharpPlus.ButtonStyle.Success, "UserRequestButton", "AI");

            var randomColor = new DiscordColor((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
            var helpMessage = new DiscordMessageBuilder()
                .WithEmbed(new DiscordEmbedBuilder()
                {
                    Title = $"{ctx.Client.CurrentUser.Username}'s Help Menu",
                    Description = "Bitte wähle eine Schaltfläche aus, um weitere Informationen zu den Befehlen zu erhalten",
                    Color = randomColor
                })
                .AddComponents(funButton, infoButton, musicButton, aiButton);

            await ctx.Channel.SendMessageAsync(helpMessage);
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
            // Erstelle ein Embed
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
            // Aktuelle Zeit
            var currentTime = DateTime.Now;

            // Berechnung der Uptime
            var uptime = currentTime - StartTime;

            var randomColor = new DiscordColor((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
            // Erstelle ein Embed
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
