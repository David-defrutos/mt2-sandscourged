using System;
using System.Collections;
using HarmonyLib;
using ShinyShoe;
public sealed class RoomStateAttackPerDebuffOnTeam : RoomStateModifierBase, IRoomStateDamageModifier, IRoomStateModifier, ILocalizationParamInt, ILocalizationParameterContext
{
    public override void Initialize(RoomModifierData roomModifierData, SaveManager saveManager)
    {
        base.Initialize(roomModifierData, saveManager   );
        this.additionalDamagePerStack = roomModifierData.GetParamInt();
        int teamsCode = roomModifierData.GetParamInt2();
        if ((teamsCode & 1) > 0)
        {
            this.teamsAllowed.Add(Team.Type.Monsters);
        }
        if ((teamsCode & 2) > 0)
        {
            this.teamsAllowed.Add(Team.Type.Heroes);
        }
    }

    public override string GetTitleKey()
    {
        return GetType().Name;
    }

    public int GetModifiedMagicPowerDamage(ICoreGameManagers coreGameManagers)
    {
        return 0;
    }

    public int GetModifiedAttackDamage(Damage.Type damageType, CharacterState attackerState, bool requestingForCharacterStats, ICoreGameManagers coreGameManagers)
    {
        if (requestingForCharacterStats)
        {
            return this.GetDynamicInt(attackerState);
        }
        return 0;
    }

    public override int GetDynamicInt(CharacterState characterContext)
    {
        if (characterContext.GetRoomStateModifiers().Contains(this) && characterContext.GetSpawnPoint(false) != null && characterContext.GetCurrentRoom(false) != null)
        {
            var room = characterContext.GetCurrentRoom(false);
            List<string> debuffIds = [];
            using (GenericPools.GetList(out List<CharacterState> list2))
            {
                foreach (Team.Type team in teamsAllowed)
                {
                    room.AddCharactersToList(list2, team);
                }
                foreach (CharacterState character in list2)
                {
                    using (GenericPools.GetList(out List<CharacterState.StatusEffectStack> charStatusEffects))
                    {
                        character.GetStatusEffects(ref charStatusEffects);
                        debuffIds.AddRange(
                            charStatusEffects.Where(status => status.State.GetDisplayCategory() == StatusEffectData.DisplayCategory.Negative)
                            .Select(debuff => debuff.State.GetStatusId()));
                    }
                }
            }
            int numDistinctDebuffs = debuffIds.Distinct().Count();
            return numDistinctDebuffs * this.additionalDamagePerStack;
        }
        return 0;
    }

    private int additionalDamagePerStack;

    private readonly List<Team.Type> teamsAllowed = [];
}
