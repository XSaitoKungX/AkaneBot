using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Google.Apis.CustomSearchAPI.v1;
using Google.Apis.Services;
using OpenAI_API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using static Google.Apis.Requests.BatchRequest;

namespace Akane.Commands.Prefix
{
    internal class UserRequestedCommands : BaseCommandModule
    {
        private readonly Random random = new Random();

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
