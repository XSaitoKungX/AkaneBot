using Akane.Engine.ExternalClasses;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Akane.Commands.Prefix
{
    internal class Fun : BaseCommandModule
    {
        private readonly Random random = new Random();

        private readonly string giphyApiKey = "Z1PXsFOooGkWoH9QbpnUCJJvx6I1vQGb";

        [Command("calculate")]
        public async Task Calculate(CommandContext ctx, double number1, string operation, double number2)
        {
            double result = 0;

            switch (operation)
            {
                case "+":
                    result = number1 + number2;
                    break;

                case "-":
                    result = number1 - number2;
                    break;

                case "*":
                    result = number1 * number2;
                    break;

                case "/":
                    if (number2 != 0)
                    {
                        result = number1 / number2;
                    } else
                    {
                        await ctx.Channel.SendMessageAsync("Division durch 0 ist mathematisch nicht möglich!");
                        return;
                    }
                    break;

                default:
                    await ctx.Channel.SendMessageAsync("Ungültiger Operator! Unterstützte Operatoren sind: +, -, * und /.");
                    return;
            }

            var randomColor = new DiscordColor((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
            // Erstelle ein Embed für die Antwort
            var embed = new DiscordEmbedBuilder
            {
                Title = "Taschenrechner",
                Description = $"Rechnung: {number1} {operation} {number2}\nErgebnis: {result}", // "F2", um zwei Nachkommastellen anzuzeigen
                Color = randomColor,
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = $"Angefordert von: {ctx.User.Username} am {DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}"
                }
            };

            await ctx.RespondAsync(embed);
        }

        [Command("action")]
        public async Task PerformAction(CommandContext ctx, string action, DiscordMember member)
        {
            string actionText = GetActionText(action.ToLower());
            string actionEmoji = GetActionEmoji(action.ToLower());

            if (actionText == null)
            {
                await ctx.Channel.SendMessageAsync("Ungültige Aktion. Unterstützte Aktionen sind: kiss, hug, cuddle, punch, baka, highfive, lapsit, lick, love, marry, massage, pat, poke, slap, steal, tickle, wave, yeet.");
                return;
            }

            // Verwende Platzhalter {0} und {1} im Texttemplate
            string textTemplate = $"{actionEmoji} {{1}}, {{0}} {actionText}";
            string formattedText = string.Format(textTemplate, ctx.Member.DisplayName, member.DisplayName );

            using (var httpClient = new HttpClient())
            {
                var giphyUrl = $"https://api.giphy.com/v1/gifs/random?api_key={giphyApiKey}&tag=anime {action}&rating=g";

                try
                {
                    var response = await httpClient.GetStringAsync(giphyUrl);
                    var gifData = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(response);

                    string gifUrl = gifData.data?.images?.original?.url.ToString();

                    if (!string.IsNullOrEmpty(gifUrl))
                    {
                        var randomColor = new DiscordColor((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
                        var embed = new DiscordEmbedBuilder
                        {
                            Title = formattedText,
                            Color = randomColor
                        };

                        embed.ImageUrl = gifUrl;

                        await ctx.RespondAsync(embed);
                    }
                    else
                    {
                        await ctx.Channel.SendMessageAsync("GIF konnte nicht gefunden werden. Bitte versuche es später erneut.");
                    }
                }
                catch (HttpRequestException)
                {
                    await ctx.Channel.SendMessageAsync("Fehler beim Abrufen des GIFs. Bitte versuche es später erneut.");
                }
                catch (Newtonsoft.Json.JsonException)
                {
                    await ctx.Channel.SendMessageAsync("Ungültige Antwort von der GIF-API. Bitte versuche es später erneut.");
                }
            }
        }

        private string GetActionText(string action)
        {
            switch (action)
            {
                case "kiss":
                    return "gibt dir einen Kuss! Voll süß 🥰";
                case "hug":
                    return "umarmt dich! Naww~";
                case "cuddle":
                    return "kuschelt mit dir!";
                case "punch":
                    return "verpasst dir einen Faustschlag! Richtig frech 😡";
                case "baka":
                    return "nennt dich Baka";
                case "highfive":
                    return "gibt dir einen High-Five";
                case "lapsit":
                    return "sitzt auf deinem Schoß";
                case "lick":
                    return "leckt dich! Aww~";
                case "love":
                    return "liebt dich! Uiii 😏";
                case "marry":
                    return "will dich heiraten";
                case "massage":
                    return "massiert dich";
                case "pat":
                    return "klopft dir auf die Schulter";
                case "poke":
                    return "stupst dich";
                case "slap":
                    return "gibt dir eine Ohrfeige";
                case "steal":
                    return "stiehlt von dir";
                case "tickle":
                    return "kitzelt dich";
                case "wave":
                    return "winkt dir zu";
                case "yeet":
                    return "yeetet dich";
                default:
                    return null;
            }
        }

        private string GetActionEmoji(string action)
        {
            switch (action)
            {
                case "kiss":
                    return "💋";
                case "hug":
                    return "🤗";
                case "cuddle":
                    return "🥰";
                case "punch":
                    return "👊";
                case "baka":
                    return "💢";
                case "highfive":
                    return "✋";
                case "lapsit":
                    return "👶";
                case "lick":
                    return "👅";
                case "love":
                    return "❤️";
                case "marry":
                    return "💍";
                case "massage":
                    return "💆";
                case "pat":
                    return "👏";
                case "poke":
                    return "👉";
                case "slap":
                    return "👋";
                case "steal":
                    return "🤫";
                case "tickle":
                    return "🤣";
                case "wave":
                    return "👋";
                case "yeet":
                    return "🚀";
                default:
                    return null;
            }
        }

        [Command("cardgame")]
        public async Task SimpleCardGame(CommandContext ctx)
        {
            var UserCard = new CardBuilder(); // Creating an instance of a card for the user

            var randomColor = new DiscordColor((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
            var userAvatarURl = ctx.User.AvatarUrl;
            var userCardMessage = new DiscordEmbedBuilder()
            {
                Title = "Deine Karte",
                Description = $"Du hast eine {UserCard.SelectedCard} gezogen",
                Color = randomColor,
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
                {
                    Url = userAvatarURl,
                }
            };
            
            await ctx.Channel.SendMessageAsync(userCardMessage);

            var BotCard = new CardBuilder(); // Creating an instance of a card for the bot

            var botAvatarURL = ctx.Client.CurrentUser.AvatarUrl;
            var botCardMessage = new DiscordEmbedBuilder()
            {
                Title = "Bot's Karte",
                Description = $"{ctx.Client.CurrentUser.Username} hat eine {BotCard.SelectedCard} gezogen",
                Color = randomColor,
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
                {
                    Url = botAvatarURL,
                }
            };

            await ctx.Channel.SendMessageAsync(botCardMessage);

            if (UserCard.SelectedNumber > BotCard.SelectedNumber)
            {
                // User wins
                var userWinningMessage = new DiscordEmbedBuilder()
                {
                    Title = "Glückwunsch, du hast gewonnen! 🥳🎉",
                    Color = randomColor,
                };

                await ctx.Channel.SendMessageAsync(userWinningMessage);
                return;
            }
            else if (UserCard.SelectedNumber < BotCard.SelectedNumber)
            {
                // Bot wins
                var botWinningMessage = new DiscordEmbedBuilder()
                {
                    Title = "Du hast leider verloren. Bot hat gewonnen!",
                    Color = randomColor,
                };

                await ctx.Channel.SendMessageAsync(botWinningMessage);
                return;
            }
            else
            {
                // Draw
                var drawMessage = new DiscordEmbedBuilder()
                {
                    Title = "Es ist unentschieden!",
                    Color = randomColor,
                };

                await ctx.Channel.SendMessageAsync(drawMessage);
                return;
            }
        }

        [Command("poll")]
        public async Task Poll(CommandContext ctx, string option1, string option2, string option3, string option4, [RemainingText] string pollTitle)
        {
            var interactivity = Program.Client.GetInteractivity();
            var pollTime = TimeSpan.FromSeconds(10);

            DiscordEmoji[] emojiOptions =
            {
                DiscordEmoji.FromName(Program.Client, ":one:"),
                DiscordEmoji.FromName(Program.Client, ":two:"),
                DiscordEmoji.FromName(Program.Client, ":three:"),
                DiscordEmoji.FromName(Program.Client, ":four:"),
            };

            string optionsDescription = $"{emojiOptions[0]} | {option1} \n" +
                                        $"{emojiOptions[1]} | {option2} \n" +
                                        $"{emojiOptions[2]} | {option3} \n" +
                                        $"{emojiOptions[3]} | {option4}";

            var randomColor = new DiscordColor((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
            var pollMessage = new DiscordEmbedBuilder()
            {
                Title = pollTitle,
                Description = optionsDescription,
                Color = randomColor
            };

            var sentPoll = await ctx.Channel.SendMessageAsync(pollMessage);

            foreach (var emoji in emojiOptions)
            {
                await sentPoll.CreateReactionAsync(emoji);
            }

            var totalReactions = await interactivity.CollectReactionsAsync(sentPoll, pollTime);

            int count1 = 0;
            int count2 = 0;
            int count3 = 0;
            int count4 = 0;

            foreach (var emoji in totalReactions)
            {
                if (emoji.Emoji == emojiOptions[0])
                {
                    count1++;
                }

                if (emoji.Emoji == emojiOptions[1])
                {
                    count2++;
                }

                if (emoji.Emoji == emojiOptions[2])
                {
                    count3++;
                }

                if (emoji.Emoji == emojiOptions[3])
                {
                    count4++;
                }
            }

            int totalVotes = count1 + count2 + count3 + count4;
            string resultsDescription = $"{emojiOptions[0]}: {count1} Votes \n" +
                                        $"{emojiOptions[1]}: {count2} Votes \n" +
                                        $"{emojiOptions[2]}: {count3} Votes \n" +
                                        $"{emojiOptions[3]}: {count4} Votes \n\n" +
                                        $"Total Votes: {totalVotes}";

            var resultEmbed = new DiscordEmbedBuilder()
            {
                Title = "📊 Ergebnis der Umfrage",
                Description = resultsDescription,
                Color = randomColor
            };

            await ctx.Channel.SendMessageAsync(resultEmbed);
        }
    }
}
