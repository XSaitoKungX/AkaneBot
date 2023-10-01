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
    }
}
