using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Akane.Commands.Prefix
{
    internal class DiscordComponentCommands : BaseCommandModule
    {
        // This class shows how to make implement different DiscordComponent types
        private readonly Random random = new Random();

        [Command("button")]
        public async Task ButtonExample(CommandContext ctx)
        {
            // Declare buttons before doing anything else
            DiscordButtonComponent button1 = new DiscordButtonComponent(DSharpPlus.ButtonStyle.Primary, "1", "Button 1");
            DiscordButtonComponent button2 = new DiscordButtonComponent(DSharpPlus.ButtonStyle.Primary, "2", "Button 2");

            // Make the MessageBuilder and add on the buttons
            // A Message can have up to 5x5 worth of buttons. Thats 5 rows, each with 5 buttons
            var randomColor = new DiscordColor((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
            var message = new DiscordMessageBuilder()
                .WithEmbed(new DiscordEmbedBuilder()
                {
                    Title = "Das ist eine Nachricht mit Buttons",
                    Description = "Wähle einen Button aus.",
                    Color = randomColor,
                })
                .AddComponents(button1, button2);

            await ctx.RespondAsync(message);
        }

        [Command("dropdown-list")]
        public async Task DropDownList(CommandContext ctx)
        {
            // Declare the list of options in the drop-down
            List<DiscordSelectComponentOption> optionList = new List<DiscordSelectComponentOption>();
            optionList.Add(new DiscordSelectComponentOption("Option 1", "option1"));
            optionList.Add(new DiscordSelectComponentOption("Option 2", "option2"));
            optionList.Add(new DiscordSelectComponentOption("Option 3", "option3"));

            // Turn the list into an IEnumerable for the Component
            var options = optionList.AsEnumerable();

            // Make thr drop-down component
            var dropDown = new DiscordSelectComponent("dropDownList", "Select...", options);

            // Make and send off the message with the component
            var randomColor = new DiscordColor((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
            var dropDownMessage = new DiscordMessageBuilder()
                .WithEmbed(new DiscordEmbedBuilder()
                {
                    Title = "Das ist ein Embed mit `Drop-Down-Menü`",
                    Color = randomColor
                })
                .AddComponents(dropDown);

            await ctx.RespondAsync(dropDownMessage);
        }

        [Command("channel-list")]
        public async Task ChannelList(CommandContext ctx)
        {
            var channelComponent = new DiscordChannelSelectComponent("channelDropDownList", "Select...");

            var randomColor = new DiscordColor((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
            var channelDropDownMessage = new DiscordMessageBuilder()
                .WithEmbed(new DiscordEmbedBuilder()
                {
                    Title = "Das ist ein Embed mit `Channel Drop-Down-List`"
                })
                .AddComponents(channelComponent);

            await ctx.RespondAsync(channelDropDownMessage);
        }

        [Command("mention-list")]
        public async Task MentionList(CommandContext ctx)
        {
            var mentionComponent = new DiscordMentionableSelectComponent("mentionDropDownList", "Select...");

            var randomColor = new DiscordColor((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
            var mentionDropDownMessage = new DiscordMessageBuilder()
                .WithEmbed(new DiscordEmbedBuilder()
                {
                    Title = "Das ist eine Embed mit `Mention Drop-Down-List`",
                    Color = randomColor,
                })
                .AddComponents(mentionComponent);

            await ctx.RespondAsync(mentionDropDownMessage);
        }
    }
}
