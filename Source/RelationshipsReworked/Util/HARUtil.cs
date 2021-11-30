using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace RelationshipsReworked.Util
{
    // Util class meant to be used for humanoid alien races compatibility.
    // This mod should still work fine without HAR.
    // TODO figure out how I can check the raceprops for immune to aging without adding a dependency on the mod.
    public static class HARUtil
    {
        [MayRequireIdeology]
        public static HistoryEventDef HAR_AlienDating_Dating = DefDatabase<HistoryEventDef>.GetNamedSilentFail("HAR_AlienDating_Dating");

        [MayRequireIdeology]
        public static HistoryEventDef HAR_AlienDating_BeginRomance = DefDatabase<HistoryEventDef>.GetNamedSilentFail("HAR_AlienDating_BeginRomance");

        [MayRequireIdeology]
        public static HistoryEventDef HAR_AlienDating_SharedBed = DefDatabase<HistoryEventDef>.GetNamedSilentFail("HAR_AlienDating_SharedBed");

        public static TraitDef Xenophobia = DefDatabase<TraitDef>.GetNamedSilentFail("Xenophobia");

        public static bool isHARActive = ModLister.HasActiveModWithName("Humanoid Alien Races");

        // Should cover xenophobe memes / precepts
        public static bool wouldPawnDateAlien(Pawn pawn) {
            return isHARActive && IdeoUtility.DoerWillingToDo(HAR_AlienDating_BeginRomance, pawn) && !isXenophobe(pawn);
        }

        public static bool isXenophobe(Pawn pawn)
        {
            return isHARActive && pawn.story != null && pawn.story.traits != null && pawn.story.traits.DegreeOfTrait(Xenophobia) == 1;
        }

        public static bool isXenophile(Pawn pawn)
        {
            return isHARActive && pawn.story != null && pawn.story.traits != null && pawn.story.traits.DegreeOfTrait(Xenophobia) == -1;
        }

        public static float getInterspeciesRelationsFactor(Pawn initiator, Pawn recipient)
        {
            // If HAR isn't active we'll just set the factor to 0
            if (!HARUtil.isHARActive)
            {
                return initiator.def == recipient.def ? 1f : 0f;
            }

            float speciesFactor = 1f;
            if (initiator.def != recipient.def)
            {
                if (!HARUtil.wouldPawnDateAlien(initiator))
                {
                    speciesFactor *= 0.01f; // Keeping this 0.01 instead of 0 to keep things spicy :)
                }
                else if (!HARUtil.isXenophile(initiator)) {
                    speciesFactor *= 0.85f; // slightly less likely to date an alien if you're not a xenophile.
                }
            }
            // TODO factor in Xenophile to increase the chance if pawn races are different.

            return speciesFactor;

        }

    }
}
