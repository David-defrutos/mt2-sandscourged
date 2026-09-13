

using Conductor.Interfaces;
using System.Runtime.CompilerServices;

namespace mt2_sandscourged.Plugin
{
    // Taken from Conductor
    // https://github.com/Monster-Train-2-Modding-Group/Conductor/blob/main/Conductor/StatusEffects/StatusEffectHexState.cs
    class StatusEffectHexState : StatusEffectState, IOnOtherStatusEffectAdded
    {
        public const string StatusId = "hex";

        public int OnOtherStatusEffectBeingAdded(int myStacks, string statusId, int numStacks)
        {
            if (statusId == StatusId)
            {
                return numStacks;
            }

            var statusEffectData = StatusEffectManager.Instance.GetStatusEffectDataById(statusId);
            if (statusEffectData == null)
                return numStacks;

            if (statusEffectData.IsPropagatable() && statusEffectData.GetDisplayCategory() == StatusEffectData.DisplayCategory.Negative)
            {
                return numStacks + myStacks;
            }

            return numStacks;
        }
    }
}