using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using RelationshipsReworked.Settings;

namespace RelationshipsReworked.Util
{
    // Generic Util methods not specific to Ideology or HAR
    public static class GenUtil
    {

        public static bool gayOkay(PawnSexuality sexuality)
        {
            return sexuality == PawnSexuality.BISEXUAL || sexuality == PawnSexuality.GAY;
        }

        public static bool heteroOkay(PawnSexuality sexuality)
        {
            return sexuality == PawnSexuality.BISEXUAL || sexuality == PawnSexuality.HETERO;
        }

        public static PawnSexuality getOverridedSexuality(Pawn pawn) {
            switch (getSexualityFromTrait(pawn))
            {
                case PawnSexualityTrait.GAY:
                    return PawnSexuality.GAY; 
                case PawnSexualityTrait.BISEXUAL:
                    return PawnSexuality.BISEXUAL;
                case PawnSexualityTrait.ASEXUAL:
                    return PawnSexuality.ASEXUAL;
                default:
                    return getDefaultSexuality();
            }
        }

        // TODO could probably handle this entirely from settings and reference an active default sexuality rather than doing this.
        public static PawnSexuality getDefaultSexuality() {
            if (RelationshipsReworkedSettings.default_bisexual) {
                return PawnSexuality.BISEXUAL;
            }
            if (RelationshipsReworkedSettings.default_asexual)
            {
                return PawnSexuality.ASEXUAL;
            }
            if (RelationshipsReworkedSettings.default_gay)
            {
                return PawnSexuality.GAY;
            }
            return PawnSexuality.HETERO;

        }

        public static PawnSexualityTrait getSexualityFromTrait(Pawn pawn)
        {
            if (pawn.story == null || pawn.story.traits == null) {
                return PawnSexualityTrait.DEFAULT;
            }

            if (pawn.story.traits.HasTrait(TraitDefOf.Bisexual)) {
                return PawnSexualityTrait.BISEXUAL;
            }

            else if (pawn.story.traits.HasTrait(TraitDefOf.Gay))
            {
                return PawnSexualityTrait.GAY;
            }

            else if (pawn.story.traits.HasTrait(TraitDefOf.Asexual))
            {
                return PawnSexualityTrait.ASEXUAL;
            }
            return PawnSexualityTrait.DEFAULT;
        }

        // Util used for SecondaryLovinChanceFactor and RomanceAttempt patches
        public static float getGenderPreferenceFactor(Pawn initiator, Pawn recipient)
        {
            float genderFactor = 1f;
            PawnSexuality initiatorSexuality = GenUtil.getOverridedSexuality(initiator);
            PawnSexuality recipientSexuality = GenUtil.getOverridedSexuality(recipient);

            if (initiatorSexuality == PawnSexuality.ASEXUAL)
            {
                genderFactor *= 0.05f; // Ace pawns will probably not try to hit on another pawn regardless of the gender.
            }
            else if (initiator.gender != recipient.gender)
            {
                if (initiatorSexuality == PawnSexuality.GAY)
                {
                    genderFactor *= 0.05f; // Gay pawns are very unlikely to try and romance the opposite sex.
                }
                if (recipientSexuality == PawnSexuality.GAY)
                {
                    genderFactor *= 0.15f; // Much less likely to try and hit on someone who's the opposite gender and gay
                }
            }
            else if (initiator.gender == recipient.gender)
            {
                if (initiatorSexuality == PawnSexuality.HETERO)
                {
                    genderFactor *= 0.05f; // Straight pawns aren't likely to hit on the same gender
                }
                if (recipientSexuality == PawnSexuality.HETERO)
                {
                    genderFactor *= 0.15f; // less likely to hit on another pawn of the same gender that's hetero.
                }
            }
            if (recipientSexuality == PawnSexuality.ASEXUAL)
            {
                genderFactor *= 0.15f; // Pawns are going to be less likely to hit on a pawn that's Asexual.
            }
            return genderFactor;
        }

    }
}
