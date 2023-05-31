﻿using AchtuurCore.Events;
using AchtuurCore.Patches;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MultiplayerExpShare.Patches;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MultiplayerExpShare
{
    internal class ModEntry : Mod
    {
        public static ModEntry Instance;
        public ModConfig Config;

        private float AccumulatedExp;

        /// <summary>
        /// Returns whether <paramref name="other_farmer"/> is nearby <c>Game1.player</c>, based on <see cref="ModConfig.ExpShareType"/>
        /// </summary>
        /// <param name="other_farmer"></param>
        /// <returns></returns>
        public static bool FarmerIsNearby(Farmer other_farmer)
        {
            switch (ModConfig.ExpShareType)
            {
                case ExpShareRangeType.Tile:
                    return other_farmer.currentLocation == Game1.player.currentLocation && IsInTileRange(other_farmer);
                case ExpShareRangeType.Map:
                    return other_farmer.currentLocation == Game1.player.currentLocation;
                case ExpShareRangeType.Global:
                    return true;
                default: return false;
            }
        }

        /// <summary>
        /// Calculates euclidian distance between current tile of <c> name="Game1.player" </c> and <paramref name="other_farmer"/> and returns true if that value is less than or equal to <see cref="ModConfig.NearbyPlayerTileRange"/>
        /// </summary>
        /// <param name="other_farmer"></param>
        /// <returns></returns>
        public static bool IsInTileRange(Farmer other_farmer)
        {
            int dx = Game1.player.getTileX() - other_farmer.getTileX();
            int dy = Game1.player.getTileY() - other_farmer.getTileY();

            return dx*dx + dy*dy <= ModConfig.NearbyPlayerTileRange * ModConfig.NearbyPlayerTileRange;
        }

        public static Farmer[] GetNearbyPlayers() 
        {
            // return all players that are close to the main player
            List<Farmer> nearbyFarmers = new List<Farmer>();
            foreach (Farmer online_farmer in Game1.getOnlineFarmers())
            {
                // Skip if player is current player
                if (online_farmer.IsLocalPlayer)
                    continue;
                
                // Add other player to list if they are close enough to main player
                if (FarmerIsNearby(online_farmer))
                {
                    AchtuurCore.Debug.DebugLog(Instance.Monitor, $"{online_farmer.Name} is nearby to {Game1.player.Name}");
                    nearbyFarmers.Add(online_farmer);
                }
            }

            return nearbyFarmers.ToArray();
        }

        public override void Entry(IModHelper helper)
        {

            HarmonyPatcher.ApplyPatches(this,
                new GainExperiencePatch()
            );

            ModEntry.Instance = this;
            I18n.Init(helper.Translation);


            this.Config = helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.GameLaunched += OnGameLaunch;
            helper.Events.Multiplayer.ModMessageReceived += this.OnModMessageReceived;
        }

        private void OnModMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            if (e.FromModID == this.ModManifest.UniqueID && e.Type == "SharedExpGained" && e.FromPlayerID != Game1.player.UniqueMultiplayerID)
            {
                ExpGainData msg_expdata = e.ReadAs<ExpGainData>();

                // if this farmer was not nearby, don't add exp
                if (!msg_expdata.nearby_farmer_ids.Contains(Game1.player.UniqueMultiplayerID))
                    return;

                AchtuurCore.Debug.DebugLog(Instance.Monitor, $"Received {msg_expdata.amount} exp in {AchtuurCore.Debug.GetSkillNameFromId(msg_expdata.skill_id)} from ({msg_expdata.actor_multiplayerid})!");
                GainExperiencePatch.InvokeGainExperience(Game1.player, msg_expdata.skill_id, msg_expdata.amount, isSharedExp: true);
            }
        }

        private void OnGameLaunch(object sender, GameLaunchedEventArgs e)
        {
            this.Config.createMenu(this);
        }
    }
}
