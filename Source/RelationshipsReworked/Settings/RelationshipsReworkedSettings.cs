using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using UnityEngine;

namespace RelationshipsReworked.Settings
{
    class RelationshipsReworkedSettings : ModSettings
    {
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<bool>(ref RelationshipsReworkedSettings.default_gay, "default_gay", false, true);
			Scribe_Values.Look<bool>(ref RelationshipsReworkedSettings.default_bisexual, "default_bisexual", false, true);
			Scribe_Values.Look<bool>(ref RelationshipsReworkedSettings.default_hetero, "default_hetero", true, true);
			Scribe_Values.Look<bool>(ref RelationshipsReworkedSettings.default_asexual, "default_asexual", false, true);
			Scribe_Values.Look<float>(ref RelationshipsReworkedSettings.outside_faction_factor, "outside_faction_factor", 0.1f, true);
			Scribe_Values.Look<float>(ref RelationshipsReworkedSettings.romance_attempt_factor, "romance_attempt_factor", 1f, true);
			Scribe_Values.Look<bool>(ref RelationshipsReworkedSettings.vanilla_age_requirements, "vanilla_age_requirements", false, true);
			Scribe_Values.Look<bool>(ref RelationshipsReworkedSettings.debug_logging, "debug_logging", false, true);
		}

		public static void DoSettingsWindowContents(Rect inRect)
		{
			Listing_Standard listing_Standard = new Listing_Standard();
			listing_Standard.ColumnWidth = Math.Min(400f, inRect.width / 2f);
			listing_Standard.Begin(inRect);
			listing_Standard.Label("Default sexuality options, choose one. These will be overriden by pawn triats like Gay, Asexual, etc.");
			listing_Standard.Gap(4f);
			listing_Standard.CheckboxLabeled("Default Sexuality Gay", ref default_gay);
			if (default_gay) { // There's gotta be a better way to do this..
				default_bisexual = false;
				default_hetero = false;
				default_asexual = false;
			}
			listing_Standard.Gap(4f);
			listing_Standard.CheckboxLabeled("Default Sexuality Bisexual", ref default_bisexual);
			if (default_bisexual)
			{
				default_gay = false;
				default_hetero = false;
				default_asexual = false;
			}
			listing_Standard.Gap(4f);
			listing_Standard.CheckboxLabeled("Default Sexuality Asexual", ref default_asexual);
			if (default_asexual)
			{
				default_bisexual = false;
				default_hetero = false;
				default_gay = false;
			}
			listing_Standard.Gap(4f);
			listing_Standard.CheckboxLabeled("Default Sexuality Heterosexual", ref default_hetero);
			if (default_hetero)
			{
				default_bisexual = false;
				default_gay = false;
				default_asexual = false;
			}
			if (!default_bisexual && !default_gay && !default_asexual)
			{
				default_hetero = true;
				default_bisexual = false;
				default_gay = false;
				default_asexual = false;
			}
			listing_Standard.Gap(4f);
			listing_Standard.Label("Other configurable options");
			listing_Standard.Gap(4f);
			listing_Standard.Label("Mixed faction romance factor: " + outside_faction_factor.ToString("0.00"), -1f, "Higher values increase the likelihood of romance attempts with pawns of differnet factions.");
			outside_faction_factor = listing_Standard.Slider(outside_faction_factor, 0f, 1f);
			listing_Standard.Gap(4f);
			// Disabling this for now! Might enable later
			//listing_Standard.Label("Likeliness of romance attempt factor: " + romance_attempt_factor.ToString("0.00"), -1, "Higher values mean that pawns are more likely to try and romance others. 1 is the default.");
			//romance_attempt_factor = listing_Standard.Slider(romance_attempt_factor, 0f, 2f);
			listing_Standard.CheckboxLabeled("Use vanilla age requirements for romance", ref vanilla_age_requirements, "This mod uses the race's configured adult age, for humans it is 18. Vanilla rimworld uses 16 as the min age for romancing / loving, check this if you'd rather the min age be 16 regardless of race.");
			listing_Standard.Gap(4f);
			listing_Standard.CheckboxLabeled("Enable Debug Logging", ref debug_logging, "You probably don't want this enabled.");
			listing_Standard.End();
		}



		// Sexualitites
		public static bool default_gay = false;
		public static bool default_bisexual = false;
		public static bool default_hetero = true;
		public static bool default_asexual = false;
		
		// Misc options
		public static float outside_faction_factor = 0.1f;
		public static float romance_attempt_factor = 1f;
		public static bool vanilla_age_requirements = false;
		public static bool debug_logging = false;
	}
}
