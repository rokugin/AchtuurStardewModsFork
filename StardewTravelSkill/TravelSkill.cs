﻿using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewTravelSkill
{
    internal class TravelSkill : SpaceCore.Skills.Skill
    {
        public const string TravelSkillID = "Achtuur.Travelling";
        public const float ExpPerStep = 0.05f;


        /// <summary>
        /// Lvl 5: Grants additional x% movespeed
        /// </summary>-
        public static TravelProfession ProfessionMovespeed;


        /// <summary>
        /// Lvl 10: Passively restore stamina
        /// </summary>
        public static TravelProfession ProfessionRestoreStamina;


        /// <summary>
        /// Lvl 10: Walking in one direction for a set amount of time gives a speed boost, requires movespeed
        /// </summary>
        public static TravelProfession ProfessionSprint;

        /// <summary>
        /// Lvl 5: Warp totem recipe is cheaper
        /// </summary>
        public static TravelProfession ProfessionCheapWarpTotem;

        /// <summary>
        /// Lvl 10: Obelisk is cheaper, requires cheapwarptotem profession
        /// </summary>
        public static TravelProfession ProfessionCheapObelisk;

        /// <summary>
        /// Lvl 10: Totems have a 50% chance of not being used up
        /// </summary>
        public static TravelProfession ProfessionTotemReuse;

        public TravelSkill()
        : base(TravelSkillID) 
        {

            this.Icon = ModEntry.Instance.Helper.ModContent.Load<Texture2D>("assets/skillicon.png");
            this.SkillsPageIcon = ModEntry.Instance.Helper.ModContent.Load<Texture2D>("assets/skillpageicon.png");

            this.ExperienceCurve = new[] { 100, 380, 770, 1300, 2150, 3300, 4800, 6900, 10000, 15000 };
            this.ExperienceBarColor = new Microsoft.Xna.Framework.Color(123, 123, 123);

            // Level 5 professions
            TravelSkill.ProfessionMovespeed = new TravelProfession(skill: this, id: "Movespeed", name: I18n.Movespeed_Name, desc: I18n.Movespeed_Desc, path_to_icon: "assets/professions/movespeed.png");
            this.Professions.Add(TravelSkill.ProfessionMovespeed);

            TravelSkill.ProfessionCheapWarpTotem = new TravelProfession(skill: this, id: "CheapWarpTotem", name: I18n.Cheapwarptotem_Name, desc: I18n.Cheapwarptotem_Desc, path_to_icon: "assets/professions/cheaptotem.png");
            this.Professions.Add(TravelSkill.ProfessionCheapWarpTotem);

            this.ProfessionsForLevels.Add(new ProfessionPair(5, TravelSkill.ProfessionMovespeed, TravelSkill.ProfessionCheapWarpTotem));

            // Level 10 professions A
            TravelSkill.ProfessionRestoreStamina = new TravelProfession(skill: this, id: "RestoreStamina", name: I18n.Restorestamine_Name, desc: I18n.Restorestamina_Desc, path_to_icon: "assets/professions/restorestamina.png");
            this.Professions.Add(TravelSkill.ProfessionRestoreStamina);

            TravelSkill.ProfessionSprint = new TravelProfession(skill: this, id: "Sprint", name: I18n.Sprint_Name, desc: I18n.Sprint_Desc, path_to_icon: "assets/professions/sprint.png");
            this.Professions.Add(TravelSkill.ProfessionSprint);

            this.ProfessionsForLevels.Add(new ProfessionPair(10, TravelSkill.ProfessionRestoreStamina, TravelSkill.ProfessionSprint, TravelSkill.ProfessionMovespeed));

            // Level 10 professions B
            TravelSkill.ProfessionCheapObelisk = new TravelProfession(skill: this, id: "CheapObelisk", name: I18n.Cheapobelisk_Name, desc: I18n.Cheapobelisk_Desc, path_to_icon: "assets/professions/cheapobelisk.png");
            this.Professions.Add(TravelSkill.ProfessionCheapObelisk);

            TravelSkill.ProfessionTotemReuse = new TravelProfession(skill: this, id: "TotemReuse", name: I18n.Totemreuse_Name, desc: I18n.Totemreuse_Desc, path_to_icon: "assets/professions/totemreuse.png");
            this.Professions.Add(TravelSkill.ProfessionTotemReuse);

            this.ProfessionsForLevels.Add(new ProfessionPair(10, TravelSkill.ProfessionCheapObelisk, TravelSkill.ProfessionTotemReuse, TravelSkill.ProfessionCheapWarpTotem));
        }

        public override string GetName()
        {
            return I18n.Travelskill_Name();
        }

        public override List<string> GetExtraLevelUpInfo(int level)
        {
            return new()
            {
                I18n.Travelskill_LevelUpPerk(bonus: Math.Round(ModConfig.LevelMovespeedBonus * 100.0f, 2))
            };
        }

        public override string GetSkillPageHoverText(int level)
        {
            return I18n.Travelskill_LevelUpPerk(bonus: Math.Round(100.0f * ModConfig.LevelMovespeedBonus * level, 2));
        }
    }
}
