using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace RelationshipsReworked.Util
{

    // Ideology Utility stuff
    public static class IdeoUtil
    {
        // Precepts for spouse count
        [MayRequireIdeology]
        public static readonly PreceptDef SpouseCount_Male_MaxTwo = DefDatabase<PreceptDef>.GetNamed("SpouseCount_Male_MaxTwo", false);

        [MayRequireIdeology]
        public static readonly PreceptDef SpouseCount_Female_MaxTwo = DefDatabase<PreceptDef>.GetNamed("SpouseCount_Female_MaxTwo", false);

        [MayRequireIdeology]
        public static readonly PreceptDef SpouseCount_Male_MaxThree = DefDatabase<PreceptDef>.GetNamed("SpouseCount_Male_MaxThree", false);

        [MayRequireIdeology]
        public static readonly PreceptDef SpouseCount_Female_MaxThree = DefDatabase<PreceptDef>.GetNamed("SpouseCount_Female_MaxThree", false);

        [MayRequireIdeology]
        public static readonly PreceptDef SpouseCount_Male_MaxFour = DefDatabase<PreceptDef>.GetNamed("SpouseCount_Male_MaxFour", false);

        [MayRequireIdeology]
        public static readonly PreceptDef SpouseCount_Female_MaxFour = DefDatabase<PreceptDef>.GetNamed("SpouseCount_Female_MaxFour", false);

        [MayRequireIdeology]
        public static readonly PreceptDef SpouseCount_Male_Unlimited = DefDatabase<PreceptDef>.GetNamed("SpouseCount_Male_Unlimited", false);

        [MayRequireIdeology]
        public static readonly PreceptDef SpouseCount_Female_Unlimited = DefDatabase<PreceptDef>.GetNamed("SpouseCount_Female_Unlimited", false);

        [MayRequireIdeology]
        public static readonly PreceptDef Lovin_FreeApproved = DefDatabase<PreceptDef>.GetNamed("Lovin_FreeApproved", false);

        [MayRequireIdeology]
        public static readonly PreceptDef Lovin_Free = DefDatabase<PreceptDef>.GetNamed("Lovin_Free", false);

        public static bool canHaveUnlimitedSpouses(Pawn pawn)
        {
            return ModsConfig.IdeologyActive && (pawn.gender == Gender.Female ? pawn.Ideo.HasPrecept(SpouseCount_Female_Unlimited) : pawn.Ideo.HasPrecept(SpouseCount_Male_Unlimited));
        }


        public static bool canHaveAnotherSpouse(Pawn pawn) {
            // Default a pawn can have 1 spouse
            if (!ModsConfig.IdeologyActive || pawn.Ideo == null)
            {
                return pawn.GetSpouseCount(false) < 1;
            }
            return IdeoUtility.DoerWillingToDo(SpouseRelationUtility.GetHistoryEventForSpouseAndFianceCountPlusOne(pawn), pawn);
        }


    }
}
