using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;

namespace Corny_Bot.Commands
{
    public class PingPong:BaseCommandModule
    {
        [Command ("ping")]
        [Description ("Testing command and showcase of your ID")]
        [RequireOwner]
        public async Task Ping(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("Pong!").ConfigureAwait(false);
        }
    }
}
