using Akane.Engine.ExternalClasses;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using Google.Apis.CustomSearchAPI.v1;
using Google.Apis.Services;
using OpenAI_API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Akane.Commands.Prefix
{
    internal class Fun : BaseCommandModule
    {
        private readonly Random random = new Random();

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
        [Description("Erstellt eine Umfrage")]
        public async Task PollCommand(CommandContext ctx)
        {
            // Schritt 1: Umfrage-Namen abfragen
            await ctx.RespondAsync("Wie soll die Umfrage heißen? (Antworte in 3 Minuten)");
            var pollNameResponse = await WaitForUserResponse(ctx, TimeSpan.FromMinutes(3));

            if (pollNameResponse == null)
            {
                await ctx.RespondAsync("Zeit überschritten. Die Umfrage wurde abgebrochen.");
                return;
            }

            var pollName = pollNameResponse.Content;

            // Schritt 2: Anzahl der Optionen abfragen
            await ctx.RespondAsync("Wie viele Optionen soll die Umfrage haben?");
            var optionsCountResponse = await WaitForUserResponse(ctx, TimeSpan.FromMinutes(3));

            if (optionsCountResponse == null)
            {
                await ctx.RespondAsync("Zeit überschritten. Die Umfrage wurde abgebrochen.");
                return;
            }

            if (!int.TryParse(optionsCountResponse.Content, out int optionsCount) || optionsCount <= 0)
            {
                await ctx.RespondAsync("Ungültige Eingabe. Die Anzahl der Optionen muss eine positive ganze Zahl sein.");
                return;
            }

            // Schritt 3: Optionen abfragen
            var options = new List<string>();
            for (int i = 1; i <= optionsCount; i++)
            {
                await ctx.RespondAsync($"Gib Option {i} ein:");
                var optionResponse = await WaitForUserResponse(ctx, TimeSpan.FromMinutes(3));

                if (optionResponse == null)
                {
                    await ctx.RespondAsync("Zeit überschritten. Die Umfrage wurde abgebrochen.");
                    return;
                }

                options.Add(optionResponse.Content);
            }

            // Schritt 4: Umfragedauer abfragen
            await ctx.RespondAsync("Wie lange soll die Umfrage dauern? `(z.B. '30min' für 30 Minute)`");
            var durationResponse = await WaitForUserResponse(ctx, TimeSpan.FromMinutes(3));

            if (durationResponse == null)
            {
                await ctx.RespondAsync("Zeit überschritten. Die Umfrage wurde abgebrochen.");
                return;
            }

            if (!ParseDuration(durationResponse.Content, out TimeSpan pollDuration))
            {
                await ctx.RespondAsync("Ungültige Eingabe. Verwende 's' für Sekunden, 'min' für Minuten, 'h' für Stunden, 'd' für Tage oder 'month' für Monate.");
                return;
            }

            // Schritt 5: Ziel-Channel abfragen
            await ctx.RespondAsync("In welchen Channel soll die Umfrage gesendet werden? (Antworte mit '@Channel' oder 'Channel-ID')");
            var channelResponse = await WaitForUserResponse(ctx, TimeSpan.FromMinutes(3));

            if (channelResponse == null)
            {
                await ctx.RespondAsync("Zeit überschritten. Die Umfrage wurde abgebrochen.");
                return;
            }

            ulong channelId = 0;
            if (channelResponse.MentionedChannels.Count > 0)
            {
                channelId = channelResponse.MentionedChannels.First().Id;
            }
            else if (ulong.TryParse(channelResponse.Content, out ulong parsedChannelId))
            {
                channelId = parsedChannelId;
            }
            else
            {
                await ctx.RespondAsync("Ungültige Eingabe. Bitte ping einen Channel oder gib seine ID ein.");
                return;
            }

            // Schritt 6: Umfrage erstellen und senden
            var pollEmbed = CreatePollEmbed(pollName, options, ctx.Member.Username, pollDuration);
            var pollMessage = await ctx.Client.GetChannelAsync(channelId).Result.SendMessageAsync(embed: pollEmbed);

            await ctx.RespondAsync($"Die Umfrage **{pollName}** wurde erfolgreich in {ctx.Channel.Mention} erstellt!");

            // Schritt 7: Timer für das Beenden der Umfrage einrichten
            for (int i = 0; i < options.Count; i++)
            {
                var optionEmojis = GetOptionEmoji(i + 1);
                await pollMessage.CreateReactionAsync(DiscordEmoji.FromUnicode(optionEmojis));
            }

            // Schritt 8: Timer für das Beenden der Umfrage einrichten
            var timer = new Timer(async _ =>
            {
                // Umfrageergebnisse sammeln
                var pollResults = new Dictionary<string, int>();
                foreach (var option in options)
                {
                    pollResults[option] = 0;
                }

                foreach (var option in options)
                {
                    var emoji = DiscordEmoji.FromUnicode(GetOptionEmoji(options.IndexOf(option) + 1));
                    var reactionUsers = await pollMessage.GetReactionsAsync(emoji).ConfigureAwait(false);
                    pollResults[option] = reactionUsers.Count;
                }

                // Umfrage-Embed bearbeiten und Ergebnisse anzeigen
                var updatedEmbed = UpdatePollEmbed(pollEmbed, pollResults);
                await pollMessage.ModifyAsync(updatedEmbed).ConfigureAwait(false);
            }, null, pollDuration, Timeout.InfiniteTimeSpan);
        }

        private DiscordEmbed CreatePollEmbed(string pollName, List<string> options, string author, TimeSpan duration)
        {
            var randomColor = new DiscordColor((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));

            var pollEmbed = new DiscordEmbedBuilder
            {
                Title = pollName,
                Description = string.Join("\n", options.Select((option, index) => $"{GetOptionEmoji(index + 1)} {option}")),
                Color = randomColor,
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = $"Erstellt von {author}",
                },
            };

            pollEmbed.AddField("Dauer", FormatDuration(duration), true);

            return pollEmbed;
        }

        private DiscordEmbed UpdatePollEmbed(DiscordEmbed originalEmbed, Dictionary<string, int> results)
        {
            var updatedEmbed = new DiscordEmbedBuilder(originalEmbed);

            var totalVotes = results.Values.Sum();
            var resultsText = string.Join("\n", results.Select(kv => $"{kv.Key} - {kv.Value} Stimmen ({(double)kv.Value / totalVotes * 100:F2}%)"));

            updatedEmbed.AddField("Ergebnisse", resultsText);

            return updatedEmbed;
        }

        private string GetOptionEmoji(int optionNumber)
        {
            // Hier kannst du die Emojis für die Optionen anpassen oder erweitern
            string[] emojis = { "1️⃣", "2️⃣", "3️⃣", "4️⃣", "5️⃣", "6️⃣", "7️⃣", "8️⃣", "9️⃣", "🔟" };

            if (optionNumber >= 1 && optionNumber <= emojis.Length)
            {
                return emojis[optionNumber - 1];
            }
            else
            {
                return optionNumber.ToString();
            }
        }

        private string FormatDuration(TimeSpan duration)
        {
            string formattedDuration = "";

            if (duration.Days > 0)
            {
                formattedDuration += $"{duration.Days}d ";
            }

            if (duration.Hours > 0)
            {
                formattedDuration += $"{duration.Hours}h ";
            }

            if (duration.Minutes > 0)
            {
                formattedDuration += $"{duration.Minutes}min ";
            }

            if (duration.Seconds > 0)
            {
                formattedDuration += $"{duration.Seconds}s";
            }

            return formattedDuration.Trim();
        }

        private bool ParseDuration(string input, out TimeSpan duration)
        {
            duration = TimeSpan.Zero;

            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }

            input = input.Trim().ToLower();

            if (input.EndsWith("s"))
            {
                if (double.TryParse(input.Substring(0, input.Length - 1), out double seconds))
                {
                    duration = TimeSpan.FromSeconds(seconds);
                    return true;
                }
            }
            else if (input.EndsWith("min"))
            {
                if (double.TryParse(input.Substring(0, input.Length - 3), out double minutes))
                {
                    duration = TimeSpan.FromMinutes(minutes);
                    return true;
                }
            }
            else if (input.EndsWith("h"))
            {
                if (double.TryParse(input.Substring(0, input.Length - 1), out double hours))
                {
                    duration = TimeSpan.FromHours(hours);
                    return true;
                }
            }
            else if (input.EndsWith("d"))
            {
                if (double.TryParse(input.Substring(0, input.Length - 1), out double days))
                {
                    duration = TimeSpan.FromDays(days);
                    return true;
                }
            }
            else if (input.EndsWith("month"))
            {
                if (double.TryParse(input.Substring(0, input.Length - 5), out double months))
                {
                    duration = TimeSpan.FromDays(months * 30); // Annähernde Anzahl von Tagen in einem Monat
                    return true;
                }
            }

            return false;
        }

        private async Task<DiscordMessage> WaitForUserResponse(CommandContext ctx, TimeSpan timeout)
        {
            try
            {
                var interactivity = ctx.Client.GetInteractivity();
                var response = await interactivity.WaitForMessageAsync(x => 
                x.Author.Id == ctx.User.Id && x.Channel.Id == ctx.Channel.Id && !x.Author.IsBot, timeout);
                return response.Result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        [Command("image")]
        [Description("Sucht in Google Images nach der angegebenen Suchanfrage")]
        public async Task ImageSearch(CommandContext ctx, [RemainingText] string query)
        {
            // Clearing the dictionary so we can populate it with new images
            Program.imageHandler.images.Clear();
            int IDCount = 0;

            // Replace with your own Custom Search Engine ID and API Key
            // https://cse.google.com/cse.js?cx=34e1a705bcc3a4f02
            string cseId = "34e1a705bcc3a4f02";
            string apiKey = "AIzaSyBjSnKalOVzUwTech1xK67u9HxlGP1aVeQ";

            // Initialise the API
            var customSearchService = new CustomSearchAPIService(new BaseClientService.Initializer
            {
                ApplicationName = "Akane",
                ApiKey = apiKey,
            });

            // Create search request
            var listRequest = customSearchService.Cse.List();
            listRequest.Cx = cseId;
            listRequest.Num = 10;
            listRequest.SearchType = CseResource.ListRequest.SearchTypeEnum.Image;
            listRequest.Q = query;

            // Execute the search request & get the results
            var search = await listRequest.ExecuteAsync();
            var results = search.Items;

            // Foreach through the results and add each image link into the dictionary
            foreach (var result in results)
            {
                Program.imageHandler.images.Add(IDCount, result.Link);
                IDCount++;
            }

            // If there are no results, then display an error message, else show the images
            if (results == null || !results.Any())
            {
                var randomColor = new DiscordColor((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
                var embed = new DiscordEmbedBuilder
                {
                    Title = "Keine Ergebnisse gefunden!",
                    Color = randomColor,
                };

                await ctx.RespondAsync(embed);
                return;
            }
            else
            {
                // Create the buttons for this Embed
                var previousEmoji = new DiscordComponentEmoji(DiscordEmoji.FromName(ctx.Client, ":track_previous:"));
                var previousButton = new DiscordButtonComponent(ButtonStyle.Primary, "previousButton", "Previous", false, previousEmoji);

                var nextEmoji = new DiscordComponentEmoji(DiscordEmoji.FromName(ctx.Client, ":track_next:"));
                var nextButton = new DiscordButtonComponent(ButtonStyle.Primary, "nextButton", "Next", false, nextEmoji);

                // Display the First Result
                var firstResult = results.First();

                var randomColor = new DiscordColor((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
                var imageMessage = new DiscordMessageBuilder()
                    .WithEmbed(new DiscordEmbedBuilder()
                    {
                        Title = $"Ergebnis für Suchanfrage: {query}",
                        Color = randomColor,
                        ImageUrl = firstResult.Link
                    })
                    .AddComponents(previousButton, nextButton);

                await ctx.Channel.SendMessageAsync(imageMessage);
            }
        }

        [Command("chatgpt")]
        public async Task ChatGPT(CommandContext ctx, params string[] message)
        {
            // Stelle sicher, dass eine Nachricht eingegeben wurde
            if (message.Length == 0)
            {
                await ctx.RespondAsync("Du musst eine Nachricht eingeben, damit ich antworten kann.");
                return;
            }

            try
            {
                // API-Schlüssel für OpenAI
                string apiKey = "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";

                // Initialisiere die OpenAIAPI
                var api = new OpenAIAPI(apiKey);

                // Initialisiere eine neue Chat-Konversation
                var chat = api.Chat.CreateConversation();
                chat.AppendSystemMessage("Gib eine Suchanfrage ein.");

                // Füge die Benutzereingabe als Nachricht hinzu
                chat.AppendUserInput(string.Join(" ", message));

                // Hole die Antwort von ChatGPT
                string response = await chat.GetResponseFromChatbot();

                // Sende die Antwort als Discord Embed Message
                var randomColor = new DiscordColor((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
                var responseMsg = new DiscordEmbedBuilder()
                {
                    Title = string.Join(" ", message),
                    Description = response,
                    Color = randomColor
                };

                await ctx.RespondAsync(embed: responseMsg);
            }
            catch (Exception ex)
            {
                // Behandle Fehler und gebe eine Fehlermeldung aus
                var randomColor = new DiscordColor((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
                var errorMessage = new DiscordEmbedBuilder()
                {
                    Title = "Ein Fehler ist aufgetreten:",
                    Description = ex.Message,
                    Color = randomColor
                };
                await ctx.RespondAsync(embed: errorMessage);
            }
        }
    }
}
