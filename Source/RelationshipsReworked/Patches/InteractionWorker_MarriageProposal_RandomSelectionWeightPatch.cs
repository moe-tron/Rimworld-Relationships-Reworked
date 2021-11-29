using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using Verse;

namespace RelationshipsReworked.Patches
{
    [HarmonyPatch(typeof(InteractionWorker_MarriageProposal), "RandomSelectionWeight", null)]
    public class InteractionWorker_MarriageProposal_RandomSelectionWeightPatch
    {
        // All we're doing here is just unsetting the mult done if the initiator is female, these means that woman are just as likely to propose as men.
        // TODO factor in ideologies maybe? If initiator has male superemacy ideo they'll be less likely to propose.
        [HarmonyPostfix]
        public static void RandomSelectionWeight(ref Pawn initiator, ref Pawn recipient, ref float __result) {
            if (initiator.gender == Gender.Female) {
                __result = __result / 0.2f;
            }
        }
    }
}
