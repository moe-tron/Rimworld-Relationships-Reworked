using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using Verse;
using RelationshipsReworked.Util;
using RelationshipsReworked.Settings;


namespace RelationshipsReworked.Patches
{
    [HarmonyPatch(typeof(Pawn_RelationsTracker), "SecondaryLovinChanceFactor", null)]
    public class Pawn_RelationsTrackerSecondaryLovinChanceFactorPatch
    {

        // This is a factor for lovin' but it's used in vanilla rimworld for a lot of romance interaction stuff.
        [HarmonyPostfix]
        public static void SecondaryLovinChanceFactor(ref Pawn otherPawn, ref Pawn ___pawn, ref float __result) {
            Pawn pawn = ___pawn;

            // No pawn to generate factor with, or pawn is otherpawn.
            if (pawn == null || otherPawn == null || pawn == otherPawn) {
                __result = 0f;
                return;
            }

            // Non humanlike races can't generate relations with humanlikes.
            if (!pawn.RaceProps.Humanlike || !otherPawn.RaceProps.Humanlike)
            {
                __result = 0f;
                return;
            }

            // Asexual pawns always have 0 chance.
            if (pawn.story != null && pawn.story.traits != null)
            {
                if (pawn.story.traits.HasTrait(TraitDefOf.Asexual))
                {
                    __result = 0f;
                    return;
                }
            }

            // Get gender preference factor
            float genderFactor = GenUtil.getGenderPreferenceFactor(pawn, otherPawn);


            float ageBioYearsPawn = pawn.ageTracker.AgeBiologicalYearsFloat;
            float ageBioYearsOtherPawn = otherPawn.ageTracker.AgeBiologicalYearsFloat;
            
            // TODO make this overrideable in settings and use 16 like in vanilla in case there's races that aren't setup properly.
            if (ageBioYearsPawn < pawn.ageTracker.AdultMinAge || ageBioYearsOtherPawn < otherPawn.ageTracker.AdultMinAge)
            {
                __result = 0f;
                return;
            }

            // Similar to vanilla rimworld, except males and females have the same preferance.
            // Factors in the fraction of the pawn's total lifespan rather than direct ages.
            float pawnMaxLifespan = pawn.RaceProps.lifeExpectancy;
            float otherPawnMaxLifespan = otherPawn.RaceProps.lifeExpectancy;

            float pawnRatioOfLifespan = ageBioYearsPawn / pawnMaxLifespan;
            float otherPawnRatioOfLifespan = ageBioYearsOtherPawn / otherPawnMaxLifespan;

            float ageRatioMin = Math.Max(0f, pawnRatioOfLifespan - 0.4f); // Should be fine as 0 since we return early if either pawn hasn't reached adulthood.
            float ageRatioLower = Math.Max(0f, pawnRatioOfLifespan - 0.15f);
            float ageRatioUpper = pawnRatioOfLifespan + 0.15f;
            float ageRatioMax = pawnRatioOfLifespan + 0.4f;
            float ageFactor = GenMath.FlatHill(0.15f, ageRatioMin, ageRatioLower, ageRatioUpper, ageRatioMax, 0.15f, otherPawnRatioOfLifespan);

            float beautyFactor =  BeautyCurve.Evaluate(otherPawn.GetStatValue(StatDefOf.Beauty));

            // HAR specific, if species is OK this will be 1f, otherwise it will be 0.01f.
            float speciesFactor = HARUtil.getInterspeciesRelationsFactor(pawn, otherPawn);

            float factionFactor = pawn.Faction == otherPawn.Faction ? 1f : RelationshipsReworkedSettings.outside_faction_factor;

            // Not sure if any other body types should be taken into account.

            float bodyTypeFactor = otherPawn.story != null && otherPawn.story.bodyType == BodyTypeDefOf.Fat ? RelationshipsReworkedSettings.fat_pawn_attractiveness_factor : 1f;


            __result = ageFactor * beautyFactor * factionFactor * genderFactor * speciesFactor * bodyTypeFactor;
        }

        // Curve for evaluating pawn's beauty
        // Should support mods that change the beauty stat for pawns
        // Idea here is that beauty increases sharply as Pawns get prettier but tapers off at higher levels of beauty.
        // Maxes out at beauty value of 3.0 w/ a factor of 2.5
        private static readonly SimpleCurve BeautyCurve = new SimpleCurve
        {
            {
                new CurvePoint(-2f, 0.1f),
                true
            },
            {
                new CurvePoint(-1f, 0.3f),
                true
            },
            {
                new CurvePoint(0f, 1f),
                true
            },
            {
                new CurvePoint(1f, 1.75f),
                true
            },
            {
                new CurvePoint(2f, 2.3f),
                true
            },
            {
                new CurvePoint(3f, 2.5f),
                true
            }
        };

    }
}
