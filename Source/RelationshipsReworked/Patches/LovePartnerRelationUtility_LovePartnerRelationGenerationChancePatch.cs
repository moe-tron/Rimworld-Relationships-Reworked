using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using Verse;
using UnityEngine;
using RelationshipsReworked.Util;

namespace RelationshipsReworked.Patches
{

    [HarmonyPatch(typeof(LovePartnerRelationUtility), "LovePartnerRelationGenerationChance", null)]
    public class LovePartnerRelationUtility_LovePartnerRelationGenerationChancePatch
    {

		// Mostly keeps the same logic as vanilla, but factors in overrided sexualitites as well as using the race's min adult age instead of the hardcoded 14.
        [HarmonyPostfix]
        public static void LovePartnerRelationGenerationChance(ref Pawn generated, ref Pawn other, ref PawnGenerationRequest request, ref bool ex, ref float __result) {
			if (generated.ageTracker.AgeBiologicalYearsFloat < generated.ageTracker.AdultMinAge)
			{
				__result = 0f;
				return;
			}
			if (other.ageTracker.AgeBiologicalYearsFloat < other.ageTracker.AdultMinAge)
			{
				__result = 0f;
				return;
			}
			PawnSexuality generatedSexuality = GenUtil.getOverridedSexuality(generated);
			PawnSexuality otherSexuality = GenUtil.getOverridedSexuality(other);
			// We'll respect the allowGay setting on the generation request.
			if (generated.gender == other.gender && (!GenUtil.gayOkay(generatedSexuality) || !GenUtil.gayOkay(otherSexuality) || !request.AllowGay))
			{
				__result = 0f;
				return;
			}
			if (generated.gender != other.gender && (!GenUtil.heteroOkay(generatedSexuality) || !GenUtil.heteroOkay(otherSexuality)))
			{
				__result = 0f;
				return;
			}

			float num = 1f;
			if (ex)
			{
				int num2 = 0;
				List<DirectPawnRelation> directRelations = other.relations.DirectRelations;
				for (int i = 0; i < directRelations.Count; i++)
				{
					if (LovePartnerRelationUtility.IsExLovePartnerRelation(directRelations[i].def))
					{
						num2++;
					}
				}
				num = Mathf.Pow(0.2f, num2);
			}
			else if (LovePartnerRelationUtility.HasAnyLovePartner(other, true))
			{
                __result = 0f;
				return;
			}
			float num3 = (generated.gender == other.gender) ? 0.01f : 1f;
			float generationChanceAgeFactor = LovePartnerRelationUtility_LovePartnerRelationGenerationChancePatch.GetGenerationChanceAgeFactor(generated);
			float generationChanceAgeFactor2 = LovePartnerRelationUtility_LovePartnerRelationGenerationChancePatch.GetGenerationChanceAgeFactor(other);
			float generationChanceAgeGapFactor = LovePartnerRelationUtility_LovePartnerRelationGenerationChancePatch.GetGenerationChanceAgeGapFactor(generated, other, ex);
			float num4 = 1f;
			if (generated.GetRelations(other).Any((PawnRelationDef x) => x.familyByBloodRelation))
			{
				num4 = 0.01f;
			}
			float num5;
			// TODO when I'm not so lazy check and see if HAR does anything unique here when it factors in melanin.
			if (request.FixedMelanin != null)
			{
				num5 = ChildRelationUtility.GetMelaninSimilarityFactor(request.FixedMelanin.Value, other.story.melanin);
			}
			else
			{
				num5 = PawnSkinColors.GetMelaninCommonalityFactor(other.story.melanin);
			}
			__result = num * generationChanceAgeFactor * generationChanceAgeFactor2 * generationChanceAgeGapFactor * num3 * num5 * num4;
		}


		// Below methods just keep mostly the same logic as in vanilla rimworld.
		// There's some changes - instead of harcoding 14 as min age we set the min age as the adult min age for pawn.
		// This way races that have old adult lifestages won't get lovers generated when they haven't reached adulthood yet.
		private static float GetGenerationChanceAgeFactor(Pawn p)
		{
			return Mathf.Clamp(GenMath.LerpDouble(p.ageTracker.AdultMinAge, p.ageTracker.AdultMinAge + 20f, 0f, 1f, p.ageTracker.AgeBiologicalYearsFloat), 0f, 1f);
		}

		private static float GetGenerationChanceAgeGapFactor(Pawn p1, Pawn p2, bool ex)
		{
			float num = Mathf.Abs(p1.ageTracker.AgeBiologicalYearsFloat - p2.ageTracker.AgeBiologicalYearsFloat);
			if (ex)
			{
				float num2 = LovePartnerRelationUtility_LovePartnerRelationGenerationChancePatch.MinPossibleAgeGapAtMinAgeToGenerateAsLovers(p1, p2);
				if (num2 >= 0f)
				{
					num = Mathf.Min(num, num2);
				}
				float num3 = LovePartnerRelationUtility_LovePartnerRelationGenerationChancePatch.MinPossibleAgeGapAtMinAgeToGenerateAsLovers(p2, p1);
				if (num3 >= 0f)
				{
					num = Mathf.Min(num, num3);
				}
			}
			if (num > 40f)
			{
				return 0f;
			}
			return Mathf.Clamp(GenMath.LerpDouble(0f, 20f, 1f, 0.001f, num), 0.001f, 1f);
		}

		private static float MinPossibleAgeGapAtMinAgeToGenerateAsLovers(Pawn p1, Pawn p2)
		{
			float num = p1.ageTracker.AgeChronologicalYearsFloat - p1.ageTracker.AdultMinAge;
			if (num < 0f)
			{
				Log.Warning("at < 0");
				return 0f;
			}
			float num2 = PawnRelationUtility.MaxPossibleBioAgeAt(p2.ageTracker.AgeBiologicalYearsFloat, p2.ageTracker.AgeChronologicalYearsFloat, num);
			float num3 = PawnRelationUtility.MinPossibleBioAgeAt(p2.ageTracker.AgeBiologicalYearsFloat, num);
			if (num2 < 0f)
			{
				return -1f;
			}
			if (num2 < 14f)
			{
				return -1f;
			}
			if (num3 <= 14f)
			{
				return 0f;
			}
			return num3 - 14f;
		}
	}
}
