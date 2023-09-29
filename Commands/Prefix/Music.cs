using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Akane.Commands.Prefix
{
    internal class Music : BaseCommandModule
    {
        private readonly Random random = new Random();

        [Command("play")]
        public async Task PlayMusic(CommandContext ctx, [RemainingText] string query)
        {
            var userVC = ctx.Member.VoiceState.Channel;
            var lavalinkInstance = ctx.Client.GetLavalink();

            // Pre-Execution checks
            if (ctx.Member.VoiceState == null || userVC == null)
            {
                var randomColor = new DiscordColor((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
                var embed1 = new DiscordEmbedBuilder()
                {
                    Title = "Bitte gib einen Voice Channel (VC) ein.",
                    Color = randomColor,
                };

                await ctx.Channel.SendMessageAsync(embed1);
                return;
            }

            if (!lavalinkInstance.ConnectedNodes.Any())
            {
                var randomColor = new DiscordColor((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
                var embed2 = new DiscordEmbedBuilder()
                {
                    Title = "Ein Fehler ist aufgetreten. Die Verbindung wurde nicht hergestellt.",
                    Color = randomColor,
                };

                await ctx.Channel.SendMessageAsync(embed2);
                return;
            }

            if (userVC.Type != DSharpPlus.ChannelType.Voice)
            {
                var randomColor = new DiscordColor((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
                var embed3 = new DiscordEmbedBuilder()
                {
                    Title = "Bitte gib einen gültigen Voice Channel (VC) ein.",
                    Color = randomColor,
                };

                await ctx.Channel.SendMessageAsync(embed3);
                return;
            }

            // Connection to the VC and playing Music
            var node = lavalinkInstance.ConnectedNodes.Values.First();
            await node.ConnectAsync(userVC);

            var connect = node.GetGuildConnection(ctx.Member.VoiceState.Guild);
            if (connect == null)
            {
                var randomColor = new DiscordColor((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
                var embed4 = new DiscordEmbedBuilder()
                {
                    Title = "Die Verbindung zum Lavalink kann nicht hergestellt werden! Bitte versuche später nochmal.",
                    Color = randomColor,
                };

                await ctx.Channel.SendMessageAsync(embed4);
                return;
            }

            var searchQuery = await node.Rest.GetTracksAsync(query);
            if (searchQuery.LoadResultType == LavalinkLoadResultType.NoMatches || searchQuery.LoadResultType == LavalinkLoadResultType.LoadFailed)
            {
                var randomColor = new DiscordColor((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
                var embed5 = new DiscordEmbedBuilder()
                {
                    Title = $"Musik konnte nicht gefunden werden mit der Anfrage: {query}",
                    Color = randomColor,
                };

                await ctx.Channel.SendMessageAsync(embed5);
                return;
            }

            var musicTrack = searchQuery.Tracks.First();

            await connect.PlayAsync(musicTrack);

            string musicDescription = $"Gerade spielt: {musicTrack.Title} \n" +
                                      $"Author: {musicTrack.Author} \n" +
                                      $"Link: {musicTrack.Uri}";

            var embed6 = new DiscordEmbedBuilder()
            {
                Title = $"Erfolgreich den Voice Channel begetreten - {userVC.Name} - und spielt Musik ab",
                Description = musicDescription,
                Color = DiscordColor.Green,
            };

            await ctx.Channel.SendMessageAsync(embed6);
        }

        [Command("pause")]
        public async Task PuaseMusic(CommandContext ctx)
        {
            var userVC = ctx.Member.VoiceState.Channel;
            var lavalinkInstance = ctx.Client.GetLavalink();

            // Pre-Executeion checks
            if (ctx.Member.VoiceState == null || userVC == null)
            {
                var randomColor = new DiscordColor((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
                var embed7 = new DiscordEmbedBuilder()
                {
                    Title = "Bitte gib einen Voice Channel (VC) ein.",
                    Color = randomColor,
                };

                await ctx.Channel.SendMessageAsync(embed7);
                return;
            }

            if (!lavalinkInstance.ConnectedNodes.Any())
            {
                var randomColor = new DiscordColor((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
                var embed8 = new DiscordEmbedBuilder()
                {
                    Title = "Ein Fehler ist aufgetreten. Die Verbindung wurde nicht hergestellt.",
                    Color = randomColor,
                };

                await ctx.Channel.SendMessageAsync(embed8);
                return;
            }

            if (userVC.Type != DSharpPlus.ChannelType.Voice)
            {
                var randomColor = new DiscordColor((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
                var embed9 = new DiscordEmbedBuilder()
                {
                    Title = "Bitte gib einen gültigen Voice Channel (VC) ein.",
                    Color = randomColor,
                };

                await ctx.Channel.SendMessageAsync(embed9);
                return;
            }

            // Connection to the VC and playing Music
            var node = lavalinkInstance.ConnectedNodes.Values.First();
            var connect = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            if (connect == null)
            {
                var randomColor = new DiscordColor((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
                var embed10 = new DiscordEmbedBuilder()
                {
                    Title = "Die Verbindung zum Lavalink kann nicht hergestellt werden! Bitte versuche später nochmal.",
                    Color = randomColor,
                };

                await ctx.Channel.SendMessageAsync(embed10);
                return;
            }

            if (connect.CurrentState.CurrentTrack == null)
            {
                var randomColor = new DiscordColor((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
                var embed11 = new DiscordEmbedBuilder()
                {
                    Title = "Momentan wird keine Musik abgespielt!",
                    Color = randomColor,
                };

                await ctx.Channel.SendMessageAsync(embed11);
                return;
            }

            await connect.PauseAsync();

            var embed12 = new DiscordEmbedBuilder()
            {
                Title = "Musik pausiert",
                Color = DiscordColor.Black,
            };

            await ctx.Channel.SendMessageAsync(embed12);
        }

        [Command("resume")]
        public async Task ResumeMusic(CommandContext ctx)
        {
            var userVC = ctx.Member.VoiceState.Channel;
            var lavalinkInstance = ctx.Client.GetLavalink();

            // Pre-Executeion checks
            if (ctx.Member.VoiceState == null || userVC == null)
            {
                var randomColor = new DiscordColor((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
                var embed13 = new DiscordEmbedBuilder()
                {
                    Title = "Bitte gib einen Voice Channel (VC) ein.",
                    Color = randomColor,
                };

                await ctx.Channel.SendMessageAsync(embed13);
                return;
            }

            if (!lavalinkInstance.ConnectedNodes.Any())
            {
                var randomColor = new DiscordColor((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
                var embed14 = new DiscordEmbedBuilder()
                {
                    Title = "Ein Fehler ist aufgetreten. Die Verbindung wurde nicht hergestellt.",
                    Color = randomColor,
                };

                await ctx.Channel.SendMessageAsync(embed14);
                return;
            }

            if (userVC.Type != DSharpPlus.ChannelType.Voice)
            {
                var randomColor = new DiscordColor((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
                var embed15 = new DiscordEmbedBuilder()
                {
                    Title = "Bitte gib einen gültigen Voice Channel (VC) ein.",
                    Color = randomColor,
                };

                await ctx.Channel.SendMessageAsync(embed15);
                return;
            }

            // Connection to the VC and playing Music
            var node = lavalinkInstance.ConnectedNodes.Values.First();
            var connect = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            if (connect == null)
            {
                var randomColor = new DiscordColor((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
                var embed16 = new DiscordEmbedBuilder()
                {
                    Title = "Die Verbindung zum Lavalink kann nicht hergestellt werden! Bitte versuche später nochmal.",
                    Color = randomColor,
                };

                await ctx.Channel.SendMessageAsync(embed16);
                return;
            }

            if (connect.CurrentState.CurrentTrack == null)
            {
                var randomColor = new DiscordColor((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
                var embed17 = new DiscordEmbedBuilder()
                {
                    Title = "Momentan wird keine Musik abgespielt!",
                    Color = randomColor,
                };

                await ctx.Channel.SendMessageAsync(embed17);
                return;
            }

            await connect.ResumeAsync();

            var embed18 = new DiscordEmbedBuilder()
            {
                Title = "Musik fortgesetzt",
                Color = DiscordColor.Green,
            };

            await ctx.Channel.SendMessageAsync(embed18);
        }

        [Command("stop")]
        public async Task StopMusic(CommandContext ctx)
        {
            var userVC = ctx.Member.VoiceState.Channel;
            var lavalinkInstance = ctx.Client.GetLavalink();

            // Pre-Executeion checks
            if (ctx.Member.VoiceState == null || userVC == null)
            {
                var randomColor = new DiscordColor((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
                var embed18 = new DiscordEmbedBuilder()
                {
                    Title = "Bitte gib einen Voice Channel (VC) ein.",
                    Color = randomColor,
                };

                await ctx.Channel.SendMessageAsync(embed18);
                return;
            }

            if (!lavalinkInstance.ConnectedNodes.Any())
            {
                var randomColor = new DiscordColor((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
                var embed19 = new DiscordEmbedBuilder()
                {
                    Title = "Ein Fehler ist aufgetreten. Die Verbindung wurde nicht hergestellt.",
                    Color = randomColor,
                };

                await ctx.Channel.SendMessageAsync(embed19);
                return;
            }

            if (userVC.Type != DSharpPlus.ChannelType.Voice)
            {
                var randomColor = new DiscordColor((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
                var embed20 = new DiscordEmbedBuilder()
                {
                    Title = "Bitte gib einen gültigen Voice Channel (VC) ein.",
                    Color = randomColor,
                };

                await ctx.Channel.SendMessageAsync(embed20);
                return;
            }

            // Connection to the VC and playing Music
            var node = lavalinkInstance.ConnectedNodes.Values.First();
            var connect = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            if (connect == null)
            {
                var randomColor = new DiscordColor((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
                var embed21 = new DiscordEmbedBuilder()
                {
                    Title = "Die Verbindung zum Lavalink kann nicht hergestellt werden! Bitte versuche später nochmal.",
                    Color = randomColor,
                };

                await ctx.Channel.SendMessageAsync(embed21);
                return;
            }

            if (connect.CurrentState.CurrentTrack == null)
            {
                var randomColor = new DiscordColor((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
                var embed21 = new DiscordEmbedBuilder()
                {
                    Title = "Momentan wird keine Musik abgespielt!",
                    Color = randomColor,
                };

                await ctx.Channel.SendMessageAsync(embed21);
                return;
            }

            await connect.StopAsync();
            await connect.DisconnectAsync();

            var embed22 = new DiscordEmbedBuilder()
            {
                Title = "Musik pausiert!",
                Description = "Die Verbindung vom Voice Channel erfolgreich getrennt",
                Color = DiscordColor.Red,
            };

            await ctx.Channel.SendMessageAsync(embed22);
        }
    }
}
