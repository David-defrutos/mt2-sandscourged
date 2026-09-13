using System;
using System.Collections;

/// <summary>
/// Basically a copy of CardEffectDrawType but when it draws from the discard pile, it draws at random, instead of the most recently discarded.
/// </summary>
[SearchWindowPath("Cards")]
public sealed class CardEffectDrawTypeWithDiscardFix : CardEffectBase, ICardTargetingCardEffect
{
    public override bool CanPlayAfterBossDead => false;
    public override bool CanApplyInPreviewMode => false;

    public override PropDescriptions CreateEditorInspectorDescriptions()
    {
        PropDescriptions propDescriptions = [];
        string fieldName = CardEffectFieldNames.ParamInt.GetFieldName();
        propDescriptions[fieldName] = new PropDescription("Num Cards", "", null, false);
        string fieldName6 = CardEffectFieldNames.ParamCardFilter.GetFieldName();
        propDescriptions[fieldName6] = new PropDescription("Card Filter", "Optional. If one is defined it will supercede other card type checking", null, false);
        string fieldName7 = CardEffectFieldNames.ParamCardFilterSecondary.GetFieldName();
        propDescriptions[fieldName7] = new PropDescription("Secondary Card Filter", "Optional. Only used if there is a primary card filter and it failed to find a match", null, false);
        return propDescriptions;
    }

    public override IEnumerator ApplyEffect(CardEffectState cardEffectState, CardEffectParams cardEffectParams, ICoreGameManagers coreGameManagers, ISystemManagers sysManagers)
    {
        CardManager cardManager = coreGameManagers.GetCardManager();
        RelicManager relicManager = coreGameManagers.GetRelicManager();
        this.toProcessCards.Clear();
        // This bit here is the only change from the original CardEffectDrawType
        var randomisedDiscardPile = cardManager.GetDiscardPile(true);
        randomisedDiscardPile.Shuffle(RngId.CardDraw);
        this.toProcessCards.AddRange(randomisedDiscardPile);

        this.toProcessCards.AddRange(cardManager.GetDrawPile(false));

        int num = 0;
        int intInRange = cardEffectState.GetIntInRange();
        CardUpgradeMaskData? paramCardFilter = cardEffectState.GetParamCardFilter();
        CardUpgradeMaskData? paramCardFilterSecondary = cardEffectState.GetParamCardFilterSecondary();
        int num2 = this.toProcessCards.Count - 1;
        while (num2 >= 0 && num < intInRange)
        {
            CardState cardState = this.toProcessCards[num2];
            if (cardState != cardEffectParams.playedCard)
            {
                if (paramCardFilter != null)
                {
                    if (!paramCardFilter.FilterCard<CardState>(cardState, relicManager))
                    {
                        goto IL_104;
                    }
                }
                else if (cardState.GetCardType() != cardEffectState.GetTargetCardType())
                {
                    goto IL_104;
                }
                cardManager.DrawSpecificCard(cardState, false, HandUI.DrawSource.Deck, cardEffectState.GetParentCardState(), 1, 1);
                num++;
            }
        IL_104:
            num2--;
        }
        if (num == 0 && paramCardFilter != null && paramCardFilterSecondary != null)
        {
            int num3 = this.toProcessCards.Count - 1;
            while (num3 >= 0 && num < intInRange)
            {
                CardState cardState2 = this.toProcessCards[num3];
                if (cardState2 != cardEffectParams.playedCard && paramCardFilterSecondary.FilterCard<CardState>(cardState2, relicManager))
                {
                    cardManager.DrawSpecificCard(cardState2, false, HandUI.DrawSource.Deck, cardEffectState.GetParentCardState(), 1, 1);
                    num++;
                }
                num3--;
            }
        }
        yield break;
    }


    public int GetNumCardTargets(CardState cardState, CardEffectState cardEffectState, CardStatistics cardStatistics)
    {
        CardStatistics.StatValueData statValueData = new()
        {
            cardState = cardState,
            trackedValue = CardStatistics.TrackedValueType.TypeInDrawPile,
            entryDuration = CardStatistics.EntryDuration.ThisBattle,
            cardTypeTarget = cardStatistics.ConvertCardTypeToCardTargetType(cardEffectState.GetTargetCardType())
        };
        int statValue = cardStatistics.GetStatValue(statValueData);
        statValueData.trackedValue = CardStatistics.TrackedValueType.TypeInDiscardPile;
        int statValue2 = cardStatistics.GetStatValue(statValueData);
        return statValue + statValue2;
    }


    private readonly List<CardState> toProcessCards = [];
}
