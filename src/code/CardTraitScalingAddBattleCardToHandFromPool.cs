using System.Collections;
using ShinyShoe;
namespace mt2_sandscourged.Plugin
{
    public sealed class CardTraitScalingAddBattleCardToHandFromPool : CardTraitState
    {
        public static CardPool? paramCardPool;

        public override PropDescriptions CreateEditorInspectorDescriptions()
        {
            return [];
        }

        public override IEnumerator OnCardPlayed(CardState thisCard, ICoreGameManagers coreGameManagers)
        {
            int maxCount = GetAdditionalCards(coreGameManagers.GetCardStatistics());
            CardManager cardManager = coreGameManagers.GetCardManager();

            int count = maxCount;
            int maxAdd = cardManager.GetMaxHandSize() - cardManager.GetNumCardsInHand();
            if (maxAdd > 0)
            {
                count = UnityEngine.Mathf.Min(count, maxAdd);
            }
            CardManager.AddCardUpgradingInfo? addCardUpgradingInfo = null;
            var cardUpgradeData = GetCardUpgradeDataParam();
            if (cardUpgradeData != null)
            {
                addCardUpgradingInfo = new();
                addCardUpgradingInfo.upgradeDatas.Add(cardUpgradeData);
                addCardUpgradingInfo.tempCardUpgrade = true;
                addCardUpgradingInfo.upgradingCardSource = GetCard();
                addCardUpgradingInfo.ignoreTempUpgrades = false;
                addCardUpgradingInfo.copyModifiersFromCard = GetCard();
            }
            using (GenericPools.GetList(out List<CardData> toProcessCards))
            {
                for (int i = 0; i < paramCardPool?.GetNumCards(); i++)
                {
                    CardData cardAtIndex = paramCardPool.GetCardAtIndex(i);
                    toProcessCards.Add(cardAtIndex);
                }

                for (int i = 0; i < count; i++)
                {
                    int index = RandomManager.Range(0, toProcessCards.Count, RngId.Battle);
                    CardData cardData = toProcessCards[index];
                    cardManager.AddNewCard(cardData, CardPile.HandPile, fromRelic: false, permanent: false, addCardUpgradingInfo: addCardUpgradingInfo);
                }

            }

            yield break;
        }

        private int GetAdditionalCards(CardStatistics cardStatistics)
        {
            if (!GetUseScalingParams())
            {
                return GetParamInt();
            }
            CardStatistics.StatValueData statValueData = new()
            {
                cardState = GetCard(),
                trackedValue = GetParamTrackedValue(),
                entryDuration = GetParamEntryDuration(),
                cardTypeTarget = GetParamCardType(),
                paramSubtype = GetParamSubtype(),
                forPreviewText = false
            };
            int statValue = cardStatistics.GetStatValue(statValueData);
            return GetParamInt() * statValue;
        }
    }
}