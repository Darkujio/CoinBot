using CoinBotCore.Services.UserInfos;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserDateBase;
using UserDateBase.Models;

namespace Corny_Bot.Commands
{
    public class CoinCommands:BaseCommandModule
    {
        private readonly IUserInfosService _UserInfoService;

        public CoinCommands(IUserInfosService UserInfoService)
        {
            _UserInfoService = UserInfoService;
        }

        [Command("CoinsDm")]
        [RequireDirectMessage]
        public async Task DisplayMyCoinsDM(CommandContext ctx)
        {
            UserInfo userInfo = await _UserInfoService.GetOrCreateUserInfo(ctx.User.Id).ConfigureAwait(false);
            await ctx.Channel.SendMessageAsync($"Your Coins: {userInfo.coins.ToString()}").ConfigureAwait(false);
        }


        [Command("Coins")]
        [RequireGuild]
        public async Task DisplayMyCoinsAdmin(CommandContext ctx, DiscordMember member)
        {
            await GetUserInfo(ctx, member.Id);
        }

        [Command("Coins")]
        [RequireGuild]
        public async Task DisplayMyCoins(CommandContext ctx)
        {
            await GetUserInfo(ctx, ctx.Member.Id);
        }

        [Command("RemoveCoins")]
        [RequireRoles(RoleCheckMode.Any, "OWNER", "ADMIN")]
        public async Task RemoveCoins(CommandContext ctx, DiscordMember member, int coinAmount)
        {
            ulong memberId = member.Id;
            ulong GuildId = ctx.Guild.Id;
            UserInfo userInfo = await _UserInfoService.GetOrCreateUserInfo(memberId).ConfigureAwait(false);
            bool Result = await _UserInfoService.DeleteCoinsFromUser(userInfo, coinAmount);
            if (Result) await ctx.Channel.SendMessageAsync($"Removing {coinAmount.ToString()} coins from {member.DisplayName}.").ConfigureAwait(false);
            else await ctx.Channel.SendMessageAsync($"Not enough coins!").ConfigureAwait(false);
        }

        [Command("AddCoins")]
        [RequireRoles(RoleCheckMode.Any, "OWNER", "ADMIN")]
        public async Task AddCoins(CommandContext ctx, DiscordMember member, int coinAmount)
        {
            ulong memberId = member.Id;
            ulong GuildId = ctx.Guild.Id;
            UserInfo userInfo = await _UserInfoService.GetOrCreateUserInfo(memberId).ConfigureAwait(false);
            await _UserInfoService.AddCoinsToUser(userInfo, coinAmount);
            await ctx.Channel.SendMessageAsync($"Adding {coinAmount.ToString()} coins to {member.DisplayName}.").ConfigureAwait(false);
        }

        [Command("TransferCoins")]
        [RequireGuild]
        public async Task TransferCoins(CommandContext ctx, DiscordMember member, int coinAmount)
        {
            ulong member1Id = ctx.Member.Id;
            ulong member2Id = member.Id;
            ulong GuildId = ctx.Guild.Id;
            UserInfo user1Info = await _UserInfoService.GetOrCreateUserInfo(member1Id).ConfigureAwait(false);
            UserInfo user2Info = await _UserInfoService.GetOrCreateUserInfo(member2Id).ConfigureAwait(false);
            bool Result = await _UserInfoService.DeleteCoinsFromUser(user1Info, coinAmount);
            if (Result)
            {
                await ctx.Channel.SendMessageAsync($"Removing {coinAmount.ToString()} coins from {ctx.Member.DisplayName}.").ConfigureAwait(false);
                await AddCoins(ctx, member, coinAmount);
                var LogChannel = ctx.Guild.GetChannel(649671401443295243);
                await LogChannel.SendMessageAsync($"{ctx.Member.DisplayName} > {coinAmount.ToString()} > {member.DisplayName}").ConfigureAwait(false);
            }
            else await ctx.Channel.SendMessageAsync($"Not enough coins!").ConfigureAwait(false);
        }

        [Command("PayOut")]
        [RequireRoles(RoleCheckMode.Any, "OWNER", "ADMIN")]
        public async Task Testing(CommandContext ctx)
        {
            await _UserInfoService.AddCoinsMonthly(ctx).ConfigureAwait(false);
        }
        private async Task GetUserInfo(CommandContext ctx, ulong memberId)
        {
            UserInfo userInfo = await _UserInfoService.GetOrCreateUserInfo(memberId).ConfigureAwait(false);
            DiscordMember member = ctx.Guild.Members[userInfo.DiscordId];
            Console.WriteLine("Here2");

            var profileEmbed = new DiscordEmbedBuilder
            {
                Title = $"{member.DisplayName}'s Corny info"
            };
            profileEmbed.WithThumbnail(member.AvatarUrl,10,10);
            profileEmbed.AddField("Coins", userInfo.coins.ToString());

            await ctx.Channel.SendMessageAsync(embed: profileEmbed).ConfigureAwait(false);
        }
    }
}
