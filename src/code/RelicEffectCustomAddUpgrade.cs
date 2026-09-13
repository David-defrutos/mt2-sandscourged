using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
namespace mt2_sandscourged.Plugin
{
	public sealed class RelicEffectCustomAddUpgrade : RelicEffectBase, ICardModifierRelicEffect, IRelicEffect, IStatusEffectRelicEffect
	{

		public override PropDescriptions CreateEditorInspectorDescriptions()
		{
			PropDescriptions propDescriptions = new PropDescriptions();
			string fieldName = RelicEffectFieldNames.ParamSourceTeam.GetFieldName();
			propDescriptions[fieldName] = new PropDescription("Source Team", "", null, false);
			string fieldName2 = RelicEffectFieldNames.ParamCardUpgradeData.GetFieldName();
			propDescriptions[fieldName2] = new PropDescription("Upgrade", "", null, false);
			string fieldName3 = RelicEffectFieldNames.ParamBool.GetFieldName();
			propDescriptions[fieldName3] = new PropDescription("Apply to Cardless Spawns", "", null, false);
			return propDescriptions;
		}


		public override void Initialize(RelicState relicState, RelicData relicData, RelicEffectData relicEffectData)
		{
			base.Initialize(relicState, relicData, relicEffectData);
			this._sourceTeam = relicEffectData.GetParamSourceTeam();
			this._cardUpgradeData = relicEffectData.GetParamCardUpgradeData();
			this._applyToCardlessSpawns = relicEffectData.GetParamBool();
		}

		// Token: 0x06001FB0 RID: 8112 RVA: 0x00081840 File Offset: 0x0007FA40
		public bool ApplyCardStateModifiers(CardState cardState, RelicManager relicManager, SaveManager saveManager, CardManager? cardManager)
		{
			bool result = false;
			if (this._cardUpgradeData != null && this._sourceTeam.HasFlag(Team.Type.Monsters))
			{
				using (List<CardUpgradeMaskData>.Enumerator enumerator = this._cardUpgradeData.GetFilters().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (!enumerator.Current.FilterCard<CardState>(cardState, relicManager))
						{
							return false;
						}
					}
				}
				//CardStateModifiers temporaryCardStateModifiers = cardState.GetTemporaryCardStateModifiers();
				CardUpgradeState cardUpgradeState = new();
				cardUpgradeState.Setup(this._cardUpgradeData, false, false);
				var currentUpgradeData = cardState.GetUpgradeData();
				if (currentUpgradeData == null)
				{
					return false;
				}
				if (cardUpgradeState.IsUnique() && cardState.GetCardStateModifiers().HasUpgrade(this._cardUpgradeData.GetID()))
				{
					return false;
				}

				cardState.ApplyPermanentUpgrade(cardUpgradeState, saveManager, true, null);
				cardState.UpdateCardBodyText(null);
				if (cardManager != null)
				{
					cardManager.RefreshCardInHand(cardState, true, false, false);
				}
				return true;
			}
			return result;
		}

		// Token: 0x06001FB1 RID: 8113 RVA: 0x00081904 File Offset: 0x0007FB04
		public override IEnumerator OnCharacterAdded(CharacterState characterState, CardState fromCard, ICoreGameManagers coreGameManagers)
		{
			RelicManager relicManager = coreGameManagers.GetRelicManager();
			if (this._cardUpgradeData != null && this._sourceTeam.HasFlag(characterState.GetTeamType()) && characterState.HasStatusEffect("cardless") && !characterState.GetIsClone())
			{
				if (!this._applyToCardlessSpawns)
				{
					yield break;
				}
				foreach (CardUpgradeMaskData cardUpgradeMaskData in this._cardUpgradeData.GetFilters())
				{
					if (!cardUpgradeMaskData.FilterCard<CardState>(fromCard, relicManager))
					{
						yield break;
					}
					if (!cardUpgradeMaskData.FilterCharacter(characterState, relicManager))
					{
						yield break;
					}
				}
				if (this._cardUpgradeData.IsUnique() && fromCard != null && (fromCard.GetCardStateModifiers().HasUpgrade(this._cardUpgradeData.GetID()) || fromCard.GetTemporaryCardStateModifiers().HasUpgrade(this._cardUpgradeData.GetID())))
				{
					yield break;
				}
				CardUpgradeState cardUpgradeState = new CardUpgradeState();
				cardUpgradeState.Setup(this._cardUpgradeData, false, false);
				yield return characterState.ApplyCardUpgrade(cardUpgradeState, false, "", true, false, null, null, null, false);
			}
			yield break;
		}

		// Token: 0x06001FB2 RID: 8114 RVA: 0x00081928 File Offset: 0x0007FB28
		public bool GetTooltip(out string title, out string body)
		{
			title = string.Empty;
			body = string.Empty;
			return false;
		}

		public StatusEffectStackData[]? GetStatusEffects()
		{
			if (this._cardUpgradeData != null)
			{
				return this._cardUpgradeData.GetStatusEffectUpgrades().ToArray();
			}
			return null;
		}

		// Token: 0x04000DF0 RID: 3568
		private Team.Type _sourceTeam;

		// Token: 0x04000DF1 RID: 3569
		private CardUpgradeData? _cardUpgradeData;

		// Token: 0x04000DF2 RID: 3570
		private bool _applyToCardlessSpawns;
	}
}