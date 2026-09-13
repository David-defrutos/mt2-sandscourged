using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[SearchWindowPath("Card Upgrade")]
public sealed class CardEffectAddTempUpgradeToCurrentCard : CardEffectBase
{
	public override bool CanPlayAfterBossDead
	{
		get
		{
			return false;
		}
	}

	public override bool CanApplyInPreviewMode
	{
		get
		{
			return false;
		}
	}

	public override PropDescriptions CreateEditorInspectorDescriptions()
	{
		PropDescriptions propDescriptions = new PropDescriptions();
		string fieldName = CardEffectFieldNames.ParamCardUpgradeData.GetFieldName();
		propDescriptions[fieldName] = new PropDescription("Card Upgrade", "", null, false);
		return propDescriptions;
	}

	// Token: 0x060004F6 RID: 1270 RVA: 0x000168AE File Offset: 0x00014AAE
	public override IEnumerator ApplyEffect(CardEffectState cardEffectState, CardEffectParams cardEffectParams, ICoreGameManagers coreGameManagers, ISystemManagers sysManagers)
	{
		CardState? playedCard = cardEffectParams.playedCard;
		if (playedCard == null)
		{
			yield break;
		}
		CardManager cardManager = coreGameManagers.GetCardManager();
		CardUpgradeState cardUpgradeState;
		if (CardUpgradeHelper.TryApplyUpgradeToCard(playedCard, playedCard, cardEffectState.GetParamCardUpgradeData(), true, cardEffectParams.isFromHiddenTrigger, coreGameManagers, out cardUpgradeState))
		{
			playedCard.UpdateCardBodyText(null);
			cardManager.RefreshCardInHand(playedCard, false, false, false);
			CardUI cardInHand = cardManager.GetCardInHand(playedCard);
			if (cardInHand != null && cardInHand != null)
			{
				cardInHand.ShowEnhanceFX();
			}
		}
		yield break;
	}

	// Token: 0x060004F7 RID: 1271 RVA: 0x000168CB File Offset: 0x00014ACB
	public override void GetTooltipsStatusList(CardEffectState cardEffectState, ref List<string> outStatusIdList)
	{
		CardEffectAddUpgradeToCurrentCard.GetTooltipsStatusList(cardEffectState.GetSourceCardEffectData(), ref outStatusIdList);
	}

	// Token: 0x060004F8 RID: 1272 RVA: 0x000168DC File Offset: 0x00014ADC
	public static void GetTooltipsStatusList(CardEffectData cardEffectData, ref List<string> outStatusIdList)
	{
		foreach (StatusEffectStackData statusEffectStackData in cardEffectData.GetParamCardUpgradeData().GetStatusEffectUpgrades())
		{
			outStatusIdList.Add(statusEffectStackData.statusId);
		}
	}
}
