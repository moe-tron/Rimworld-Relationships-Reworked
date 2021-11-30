using UnityEngine;
using RimWorld;
using Verse;
using RelationshipsReworked.Util;
using RelationshipsReworked.Settings;
using HarmonyLib;

namespace RelationshipsReworked.Patches
{
    // Not sure if I should have just subclassed instead. Regardless, this will probably overwrite anything done by other mods beforehand, but that's intended.
    // HAR patches this, but we're doing the same stuff in regards to checking the species so it's probably fine.
    [HarmonyPatch(typeof(InteractionWorker_RomanceAttempt), "RandomSelectionWeight", null)]
	[HarmonyPriority(Priority.Last)]
	public class InteractionWorker_RomanceAttemptPatch
    {
		[HarmonyPostfix]
		public static void RandomSelectionWeight(ref Pawn initiator, ref Pawn recipient, ref float __result)
		{
			if (RelationshipsReworkedSettings.debug_logging) {
				Log.Message("RW: Generating romance attempt factor between " + initiator.Name + " and " + recipient.Name);
			}
			if (TutorSystem.TutorialMode)
			{
				__result = 0f; // IDK why anyone would be using this mod and doing the tutorial but you never know I guess...
				return;
			}
			if (LovePartnerRelationUtility.LovePartnerRelationExists(initiator, recipient))
			{
				__result = 0f;  // Can't romance attempt someone who's already your partner...
				return;
			}

			// If factor is low enough a pawn won't even attempt 
			float romanceChance = initiator.relations.SecondaryRomanceChanceFactor(recipient);
			if (romanceChance < 0.15f)
			{
				__result = 0f;
				return;
			}

			int opinionOfRecipient = initiator.relations.OpinionOf(recipient);
			if (opinionOfRecipient < 5)
			{
				__result = 0f;
				return;
			}

			// I'm assuming this is done so a pawn won't continuously keep hitting on the other to the point where they hate them..
			if (recipient.relations.OpinionOf(initiator) < 5)
			{
				__result = 0f;
				return;
			}

			float cheatingFactor = 1f;
			if (!new HistoryEvent(initiator.GetHistoryEventForLoveRelationCountPlusOne(), initiator.Named(HistoryEventArgsNames.Doer)).DoerWillingToDo())
			{
				Pawn pawn = LovePartnerRelationUtility.ExistingMostLikedLovePartner(initiator, false);
				if (pawn != null)
				{
					float value = initiator.relations.OpinionOf(pawn);
					cheatingFactor = Mathf.InverseLerp(30f, -50f, value);
				}
			}
			// Gender preference checks
			float genderFactor = GenUtil.getGenderPreferenceFactor(initiator, recipient);

			// Interspecies factor, utilizes HAR
			float speciesFactor = HARUtil.getInterspeciesRelationsFactor(initiator, recipient);

			float romanceFactor = Mathf.InverseLerp(0.15f, 1f, romanceChance);
			float opinionFactor = Mathf.InverseLerp(5f, 100f, opinionOfRecipient);
			__result = 1.15f * romanceFactor * opinionFactor * cheatingFactor * genderFactor * speciesFactor;
			if (RelationshipsReworkedSettings.debug_logging)
			{
				Log.Message("RW: Romance attempt factor between " + initiator.Name + " and " + recipient.Name + " is: " + __result.ToString("0.00"));
			}
			return;
		}



	

	}
}
