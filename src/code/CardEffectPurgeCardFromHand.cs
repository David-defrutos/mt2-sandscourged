using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ShinyShoe;

namespace mt2_sandscourged.Plugin
{
    public sealed class CardEffectPurgeCardFromHand : CardEffectBase
    {
        private CardState? sourceCardState;
        public override bool CanPlayAfterBossDead
        {
            get => false;
        }

        public override bool CanApplyInPreviewMode
        {
            get => false;
        }

        public override PropDescriptions CreateEditorInspectorDescriptions()
        {
            return new PropDescriptions
            {
                [CardEffectFieldNames.ParamCardData.GetFieldName()] = new PropDescription("True = whitelist the subtype. False = blacklist the subtype."),
                [CardEffectFieldNames.ParamSubtype.GetFieldName()] = new PropDescription("All subtypes to restrict by")
            };
        }

        public override void Setup(CardEffectState cardEffectState)
        {
            base.Setup(cardEffectState);
            this.sourceCardState = cardEffectState.GetParentCardState();
        }

        public override bool TestEffect(CardEffectState cardEffectState, CardEffectParams cardEffectParams, ICoreGameManagers coreGameManagers)
        {
            List<CardState> hand = coreGameManagers.GetCardManager().GetHand(false);
            if (hand.Count == 0)
            {
                return false;
            }
            bool result;
            using (GenericPools.GetList<CardState>(out List<CardState> list2))
            {
                foreach (CardState cardState in hand)
                {
                    if (cardState != cardEffectParams.playedCard && !this.CardFilterFunc(cardState))
                    {
                        list2.Add(cardState);
                    }
                }
                if (list2.Count == 0)
                {
                    result = false;
                }
                else
                {
                    result = true;
                }
            }
            return result;
        }

        public override IEnumerator ApplyEffect(CardEffectState cardEffectState, CardEffectParams cardEffectParams, ICoreGameManagers coreGameManagers, ISystemManagers sysManagers)
        {
            yield return coreGameManagers.GetCardManager().SelectCardFromHand(new HandSelectionUI.Params
            {
                descriptionKey = "PurgeCardFromHandKey",
                overrideActionKey = "PurgeCardFromHandActionKey",
                cardChosenCallback = new HandSelectionUI.CardStateChosenDelegate(this.HandleChooseCard),
                filterCallback = new HandSelectionUI.FilterCardStateDelegate(this.CardFilterFunc),
                instantApplyDelay = coreGameManagers.GetAllGameData().GetBalanceData().GetAnimationTimingData().cardDrawAnimationDuration,
                disableIneligibleCards = true,
                selectionHandState = new CardUI.CardUIState?(CardUI.CardUIState.HandDiscardCard)
            });
            yield break;
        }

        private IEnumerator HandleChooseCard(CardState chosenCard, CardManager cardManager)
        {
            yield return cardManager.PurgeCardFromHand(new CardManager.DiscardCardParams
            {
                overrideDiscardEffect = HandUI.DiscardEffect.Purged,
                discardCard = chosenCard,
                triggeredByCard = true,
                triggeredCard = this.sourceCardState,
                wasPlayed = false
            }, false);
            cardManager.RefreshHandCards(true, false);
            yield break;
        }

        private bool CardFilterFunc(CardState cardState)
        {
            return cardState.HasTrait<CardTraitUnpurgable>();
        }
    }
}