﻿using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Utilities;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AchtuurCore.Utility;
using AchtuurCore.Integrations;

namespace BetterPlanting
{
    internal class ModConfig
    {

        public SButton IncrementModeKey { get; set; }
        public SButton DecrementModeKey { get; set; }

        public ModConfig()
        {
            // Initialise variables here

            IncrementModeKey = SButton.LeftControl;
            DecrementModeKey = SButton.RightControl;

        }

        /// <summary>
        /// Constructs config menu for GenericConfigMenu mod
        /// </summary>
        /// <param name="instance"></param>
        public void createMenu()
        {
            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = ModEntry.Instance.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            // register mod
            configMenu.Register(
                mod: ModEntry.Instance.ModManifest,
                reset: () => ModEntry.Instance.Config = new ModConfig(),
                save: () => ModEntry.Instance.Helper.WriteConfig(ModEntry.Instance.Config)
            );

            /// General travel skill settings header
            configMenu.AddSectionTitle(
                mod: ModEntry.Instance.ModManifest,
                text: I18n.CfgSection_General,
                tooltip: null
            );

            // increment
            configMenu.AddKeybind(
                mod: ModEntry.Instance.ModManifest,
                name: I18n.CfgIncrementKeyBind_Name,
                tooltip: I18n.CfgIncrementKeyBind_Desc,
                getValue: () => this.IncrementModeKey,
                setValue: (key) => this.IncrementModeKey = key
            );

            // decrement
            configMenu.AddKeybind(
                mod: ModEntry.Instance.ModManifest,
                name: I18n.CfgDecrementKeyBind_Name,
                tooltip: I18n.CfgDecrementKeyBind_Desc,
                getValue: () => this.DecrementModeKey,
                setValue: (key) => this.DecrementModeKey = key
            );

        }
    }
}

