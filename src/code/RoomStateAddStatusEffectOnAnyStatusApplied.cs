using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public sealed class RoomStateAddStatusEffectOnAnyStatusApplied : RoomStateModifierBase, IRoomStateOnStatusEffectAppliedModifier, IRoomStateModifier, ILocalizationParamInt, ILocalizationParameterContext, IRoomStateOnCharacterTriggerModifier
{
    public override void Initialize(RoomModifierData roomModifierData, SaveManager saveManager)
    {
        base.Initialize(roomModifierData, saveManager);
        this.thisInstTrackingId = ++trackingIdCounter;
        foreach (StatusEffectStackData statusEffectStackData in roomModifierData.GetParamStatusEffects())
        {
            StatusEffectStackData statusEffectStackData2 = new()
            {
                statusId = statusEffectStackData.statusId,
                count = statusEffectStackData.count
            };
            this.statusEffects?.Add(statusEffectStackData2);
        }
        // Binary encoding, 1 for enabled, 0 for disabled. Order = Ability, Persistent, Debuff, Buff
        // E,g, 0010 = 2, triggers only when a debuff is applied
        int allowedCategories = roomModifierData.GetParamInt();
        this.buffsAllowed = (allowedCategories & 1) > 0;
        this.debuffsAllowed = (allowedCategories & 2) > 0;
        this.persistentAllowed = (allowedCategories & 4) > 0;
        this.abilitiesAllowed = (allowedCategories & 8) > 0;

        int teamsCode = roomModifierData.GetParamInt2();
        if ((teamsCode & 1) > 0)
        {
            teamsAllowed.Add(Team.Type.Monsters);
        }
        if ((teamsCode & 2) > 0)
        {
            teamsAllowed.Add(Team.Type.Heroes);
        }
    }

    public void OnStatusEffectAppliedModification(CharacterState affectedCharacter, string statusId, CharacterState.AddStatusEffectParams addStatusEffectParams, ICoreGameManagers coreGameManagers)
    {
        if (addStatusEffectParams.spawnEffect)
        {
            return;
        }
        if (addStatusEffectParams.fromRoomModifier)
        {
            return;
        }
        if (!teamsAllowed.Contains(affectedCharacter.GetTeamType()))
        {
            return;
        }
        var statusEffectData = StatusEffectManager.Instance.GetStatusEffectDataById(statusId);
        var displayCategory = statusEffectData?.GetDisplayCategory();
        if (displayCategory != null)
        {
            if (displayCategory == StatusEffectData.DisplayCategory.Positive && !buffsAllowed)
            {
                return;
            }
            else if (displayCategory == StatusEffectData.DisplayCategory.Negative && !debuffsAllowed)
            {
                return;
            }
            else if (displayCategory == StatusEffectData.DisplayCategory.Persistent && !persistentAllowed)
            {
                return;
            }
            else if (displayCategory == StatusEffectData.DisplayCategory.Ability && !abilitiesAllowed)
            {
                return;
            }
        }
        CombatManager combatManager = coreGameManagers.GetCombatManager();
        using List<StatusEffectStackData>.Enumerator enumerator = this.statusEffects!.GetEnumerator();
        while (enumerator.MoveNext())
        {
            combatManager.QueueTrigger(affectedCharacter, CharacterTriggerData.Trigger.OnQueuedStatusEffectToAdd, null, true, true, new CharacterState.FireTriggersData
            {
                paramString = enumerator.Current.statusId,
                paramInt = enumerator.Current.count,
                paramInt2 = this.thisInstTrackingId
            }, 1, null);
        }

    }

    public IEnumerator ApplyCharacterTriggerModification(CharacterTriggerData.Trigger trigger, CharacterState affectedCharacter, CharacterState.FireTriggersData? fireTriggersData, ICoreGameManagers coreGameManagers)
    {
        if (trigger != CharacterTriggerData.Trigger.OnQueuedStatusEffectToAdd)
        {
            yield break;
        }
        if (fireTriggersData == null)
        {
            yield break;
        }
        if (fireTriggersData.paramInt2 != this.thisInstTrackingId)
        {
            yield break;
        }
        this.addStatusEffectParams!.fromRoomModifier = true;

        affectedCharacter.AddStatusEffect(fireTriggersData.paramString, fireTriggersData.paramInt, this.addStatusEffectParams, null, true, false, true, false);
        yield break;
    }
    private static int trackingIdCounter;
    private int thisInstTrackingId;

    private bool buffsAllowed = false;
    private bool debuffsAllowed = false;
    private bool persistentAllowed = false;
    private bool abilitiesAllowed = false;

    private readonly List<Team.Type> teamsAllowed = [];

    private readonly List<StatusEffectStackData>? statusEffects = [];

    private readonly CharacterState.AddStatusEffectParams? addStatusEffectParams = new();
}
