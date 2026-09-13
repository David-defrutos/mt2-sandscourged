

using System.Collections;
using ShinyShoe;

namespace mt2_sandscourged.Plugin
{
    class StatusEffectScourgeState : StatusEffectState, IDamageStatusEffect
    {
        public override bool TestTrigger(InputTriggerParams inputTriggerParams, OutputTriggerParams outputTriggerParams, ICoreGameManagers coreGameManagers)
        {
            if (inputTriggerParams.associatedCharacter != null && inputTriggerParams.associatedCharacter.IsAlive)
            {
                associatedCharacter = inputTriggerParams.associatedCharacter;
            }
            else
            {
                associatedCharacter = null;
            }
            CharacterState? characterState = associatedCharacter;
            stacks = (characterState != null) ? characterState.GetStatusEffectStacks(base.GetStatusId()) : 0;
            return stacks > 0 && associatedCharacter != null;
        }

        protected override IEnumerator OnTriggered(InputTriggerParams inputTriggerParams, OutputTriggerParams outputTriggerParams, ICoreGameManagers coreGameManagers)
        {
            CoreSignals.DamageAppliedPlaySound.Dispatch(Damage.Type.Corruption);
            CombatManager combatManager = coreGameManagers.GetCombatManager();
            int damageAmount = GetDamageAmount(stacks);
            CharacterState? target = associatedCharacter;
            CombatManager.ApplyDamageToTargetParameters parameters = default;
            parameters.damageType = Damage.Type.Default;
            StatusEffectData sourceStatusEffectData = GetSourceStatusEffectData();
            parameters.affectedVfx = sourceStatusEffectData?.GetOnAffectedVFX();
            parameters.relicState = inputTriggerParams.suppressingRelic;
            yield return combatManager.ApplyDamageToTarget(damageAmount, target, parameters);
            yield break;
        }

        public override int GetEffectMagnitude(int stacks = 1)
        {
            return (base.GetParamInt() + relicManager.GetModifiedStatusMagnitudePerStack("mt2_sandscourged.plugin_scourge", base.GetAssociatedCharacter().GetTeamType())) * stacks;
        }

        private int GetDamageAmount(int stacks)
        {
            int numDistinctDebuffs = 0;
            List<CharacterState.StatusEffectStack> statusEffects = [];
            using (GenericPools.GetList<CharacterState.StatusEffectStack>(out statusEffects))
            {
                GetAssociatedCharacter().GetStatusEffects(ref statusEffects);
                numDistinctDebuffs = statusEffects.Where(s => s.State.GetDisplayCategory() == StatusEffectData.DisplayCategory.Negative).Count();
            }
            // Yes, we have to fully qualify the status name
            return (base.GetParamInt() + relicManager.GetModifiedStatusMagnitudePerStack("mt2_sandscourged.plugin_scourge", base.GetAssociatedCharacter().GetTeamType())) * numDistinctDebuffs * stacks;
        }

        public const string StatusId = "scourge";

        private CharacterState? associatedCharacter;
        private int stacks;
    }
}