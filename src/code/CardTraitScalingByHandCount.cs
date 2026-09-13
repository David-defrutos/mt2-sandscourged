using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace mt2_sandscourged.Plugin
{
    public sealed class CardTraitScalingByHandCount : CardTraitState
    {
        // This trait is no longer used, but kept for reference.
        public override PropDescriptions CreateEditorInspectorDescriptions()
        {
            return new PropDescriptions
            {
                [CardTraitFieldNames.ParamInt.GetFieldName()] = new PropDescription("Multiplier per card in hand"),
                [CardTraitFieldNames.ParamInt2.GetFieldName()] = new PropDescription("Divider per card in hand"),
            };
        }

        public override string GetCardTooltipTitle()
        {
            var combatManager = AllGameManagers.Instance?.GetCombatManager();
            var cardManager = AllGameManagers.Instance?.GetCardManager();
            if (combatManager != null && combatManager.GetIsRunningCombat() && cardManager != null)
            {
                return string.Format("CardTraitScalingByHandCount_TooltipTitleInBattle".Localize(null), cardManager.GetHand().Count);
            }
            else
            {
                return string.Format("CardTraitScalingByHandCount_TooltipTitleOutOfBattle".Localize(null));
            }
        }

        public override string GetCardTooltipText()
        {
            var combatManager = AllGameManagers.Instance?.GetCombatManager();
            var cardManager = AllGameManagers.Instance?.GetCardManager();
            if (combatManager != null && combatManager.GetIsRunningCombat() && cardManager != null)
            {
                return string.Format("CardTraitScalingByHandCount_TooltipTextInBattle".Localize(null), cardManager.GetHand().Count);
            }
            else
            {
                return string.Format("CardTraitScalingByHandCount_TooltipTextOutOfBattle".Localize(null));
            }
        }

        // Not used anymore
        public override void OnApplyingCardUpgradeToUnit(CardState thisCard, CharacterState targetUnit, CharacterTriggerState? characterTriggerState, CardUpgradeState upgradeState, ICoreGameManagers coreGameManagers)
        {
            base.OnApplyingCardUpgradeToUnit(thisCard, targetUnit, characterTriggerState, upgradeState, coreGameManagers);
            var cardManager = coreGameManagers.GetCardManager();
            int numCardsInHand = cardManager.GetHand().Count(c => c != thisCard);

            upgradeState.SetAttackDamage(GetParamInt() * numCardsInHand);
            upgradeState.SetAdditionalHP(GetParamInt2() * numCardsInHand);
        }

        // For Vizier: StormSplitter
        public override int OnApplyingDamage(ApplyingDamageParameters damageParams, ICoreGameManagers coreGameManagers)
        {
            var cardManager = coreGameManagers.GetCardManager();
            int numCardsInHand = cardManager.GetHand().Count();
            return numCardsInHand * GetParamInt() / GetParamInt2();
        }

        // For Priest Of Lost Thoughts and Rancor
        public override int OnStatusEffectApplied(CharacterState affectedCharacter, CardState thisCard, ICoreGameManagers coreGameManagers, string statusId, int sourceStacks = 0)
        {
            var cardManager = coreGameManagers.GetCardManager();
            int numCardsInHand = cardManager.GetHand().Count(c => c != thisCard);
            // This is not a great way to do this, but it works for now.
            return sourceStacks * (numCardsInHand * GetParamInt() / GetParamInt2()) - sourceStacks;
        }

    }
}
