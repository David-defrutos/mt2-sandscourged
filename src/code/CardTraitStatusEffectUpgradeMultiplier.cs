using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace mt2_sandscourged.Plugin
{
    public class CardTraitStatusEffectUpgradeMultiplier : CardTraitState
    {
        public override PropDescriptions CreateEditorInspectorDescriptions()
        {
            return new PropDescriptions
            {
                [CardTraitFieldNames.ParamStatusEffects.GetFieldName()] = new PropDescription("Status to multiply by.")
            };
        }


        /**
            * This method is called when a card upgrade is applied to a unit.
            * It retrieves the status effects from the card and applies the upgrade based on the number of stacks of the status effect.
            * 
            * @param thisCard The card being upgraded.
            * @param targetUnit The unit receiving the upgrade.
            * @param upgradeState The state of the upgrade being applied.
            * @param coreGameManagers The core game managers instance.
            */
        public override void OnApplyingCardUpgradeToUnit(CardState thisCard, CharacterState targetUnit, CharacterTriggerState? characterTriggerState, CardUpgradeState upgradeState, ICoreGameManagers coreGameManagers)
        {
            base.OnApplyingCardUpgradeToUnit(thisCard, targetUnit, characterTriggerState, upgradeState, coreGameManagers);
            string statusId = GetParamStatusEffects().First().statusId;
            int stackCount = targetUnit.GetStatusEffectStacks(statusId);
            upgradeState.SetAttackDamage(upgradeState.GetAttackDamage() * stackCount);
            upgradeState.SetAdditionalHP(upgradeState.GetAdditionalHP() * stackCount);
        }
    }
}
