using HarmonyLib;
using Verse;
using RimWorld;
using RelationshipsReworked.Util;

namespace RelationshipsReworked.Patches
{
    [HarmonyPatch(typeof(CompAbilityEffect_WordOfLove), "ValidateTarget")]
	[HarmonyPriority(Priority.High)]
	public class CompAbilityEffect_WordOfLove_ValidateTargetPatch
	{
		// Pretty sure this needs to be a prefix to prevent the unusable message from the original method.
		[HarmonyPrefix]
		private static bool GenderChecks(ref LocalTargetInfo target, LocalTargetInfo ___selectedTarget, ref bool __result)
		{
			Pawn pawn = ___selectedTarget.Pawn;
			Pawn otherPawn = target.Pawn;

			if (pawn != otherPawn && pawn != null && otherPawn != null)
			{
				PawnSexuality pawnSexuality = GenUtil.getOverridedSexuality(pawn);
				PawnSexuality otherPawnSexuality = GenUtil.getOverridedSexuality(otherPawn);
				if (pawn.gender == otherPawn.gender && GenUtil.gayOkay(pawnSexuality) && GenUtil.gayOkay(otherPawnSexuality))
				{
					__result = true;
				}
				else if (pawn.gender != otherPawn.gender && GenUtil.heteroOkay(pawnSexuality) && GenUtil.heteroOkay(otherPawnSexuality))
				{
					__result = true;
				}
				else
                {
					__result = false;
                }
				if (!__result)
				{
					Messages.Message("AbilityCantApplyWrongAttractionGender".Translate(pawn, otherPawn), pawn, MessageTypeDefOf.RejectInput, false);
				}
				return false;
				
			}
			return true;
		}
	}
}
