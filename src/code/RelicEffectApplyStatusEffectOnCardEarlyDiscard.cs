using System;
using System.Collections;
using System.Runtime.CompilerServices;
using ShinyShoe;
namespace mt2_sandscourged.Plugin
{
	public sealed class RelicEffectApplyStatusEffectOnCardEarlyDiscard : RelicEffectBase, IOnDiscardRelicEffect, IStatusEffectRelicEffect, IRelicEffect
	{
		private Team.Type sourceTeam;
		private TargetMode targetMode;
		private StatusEffectStackData[] statusEffectsToApply = [];
		private readonly CharacterState.AddStatusEffectParams addStatusEffectParams = new();
		public override PropDescriptions CreateEditorInspectorDescriptions()
		{
			PropDescriptions propDescriptions = [];
			string fieldName = RelicEffectFieldNames.ParamSourceTeam.GetFieldName();
			propDescriptions[fieldName] = new PropDescription("Target Team", "", null, false);
			string fieldName1 = RelicEffectFieldNames.ParamTargetMode.GetFieldName();
			propDescriptions[fieldName1] = new PropDescription("Target Mode", "", null, false);
			string fieldName4 = RelicEffectFieldNames.ParamStatusEffects.GetFieldName();
			propDescriptions[fieldName4] = new PropDescription("Status Effects to apply", "", null, false);

			return propDescriptions;
		}


		public override void Initialize(RelicState relicState, RelicData relicData, RelicEffectData relicEffectData)
		{
			base.Initialize(relicState, relicData, relicEffectData);
			this.sourceTeam = relicEffectData.GetParamSourceTeam();
			this.targetMode = relicEffectData.GetTargetMode();
			this.statusEffectsToApply = relicEffectData.GetParamStatusEffects();
			this.addStatusEffectParams.sourceRelicState = base.SourceRelicState;
		}


		public bool TestEffectOnCardDiscarded(CardDiscardedRelicEffectParams relicEffectParams, ICoreGameManagers coreGameManagers)
		{
			return relicEffectParams.discardCardParams.triggeredByCard;
		}

		public IEnumerator ApplyEffectOnCardDiscarded(CardDiscardedRelicEffectParams relicEffectParams, ICoreGameManagers coreGameManagers)
		{
			base.NotifyRelicTriggered(relicEffectParams.relicManager, null, "", false, 0f);
			int roomIndex = coreGameManagers.GetRoomManager().CurrentSelectedRoom;
			TargetHelper.CollectTargetsData targetsData = new()
			{
				targetTeamType = this.sourceTeam,
				targetMode = this.targetMode,
				roomIndex = roomIndex,
				inCombat = false,
				isTesting = coreGameManagers.GetSaveManager().PreviewMode,
			};
			List<CharacterState> characterStates = [];
			using (GenericPools.GetList<CharacterState>(out characterStates))
			{
				TargetHelper.CollectTargets(targetsData, coreGameManagers, ref characterStates);
				foreach (CharacterState characterState in characterStates)
				{
					foreach (StatusEffectStackData statusEffectStackData in this.statusEffectsToApply)
					{
						characterState.AddStatusEffect(statusEffectStackData.statusId, statusEffectStackData.count, this.addStatusEffectParams, null, true, false, true);
					}
				}
			}

			targetsData.Reset(TargetMode.Room);
			yield break;
		}

		public StatusEffectStackData[]? GetStatusEffects()
		{
			return this.statusEffectsToApply;
		}
	}
}