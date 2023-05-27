﻿using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SObject = StardewValley.Object;
using AchtuurCore.Patches;
using HarmonyLib;

namespace StardewTravelSkill.Patches
{
    internal class ReduceActiveItemPatch : GenericPatcher
    {
        private static IMonitor Monitor;

        public override void Patch(Harmony harmony, IMonitor monitor)
        {
            Monitor = monitor;
            harmony.Patch(
                original: this.getOriginalMethod<Farmer>(nameof(Farmer.reduceActiveItemByOne)),
                prefix: this.getHarmonyMethod(nameof(this.Prefix_ReduceActiveItemByOne))
            ); ;
        }


        /// <summary>
        /// Postfix patch to <see cref="StardewValley.Farmer.reduceActiveItemByOne"/>.
        /// </summary>
        /// <param name="__result"></param>
        internal static bool Prefix_ReduceActiveItemByOne()
        {
            try
            {
                // Check if the held item that was used is a totem
                SObject held_item = Game1.player.ActiveObject;
                if (!isTotem(held_item.ParentSheetIndex))
                    return true;

                Random rnd = new Random();
                // Randomly decide if warp totem should be consumed
                // GetWarpTotemConsumeChance() returns 1 if the profession is unlocked, meaning the totem is always consumed
                if (rnd.NextDouble() > ModEntry.GetWarpTotemConsumeChance())
                {
                    Monitor.Log("Warp totem not consumed!", LogLevel.Debug);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(Prefix_ReduceActiveItemByOne)}:\n{ex}", LogLevel.Error);
                return true;
            }
        }


        static bool isTotem(int item_id)
        {
            switch (item_id)
            {
                case 261:
                case 688:
                case 689:
                case 690:
                case 886: return true;
                default: return false;
            }
        }
    }
}