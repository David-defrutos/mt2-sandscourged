using System;
using System.Collections.Generic;
using System.Text;

namespace mt2_sandscourged.Plugin
{
    public static class TrackedValueFunctions
    {
        internal static int CountCardsInHand(CardStatistics.StatValueData _, IReadOnlyDictionary<CardState, CardStatsEntry> deckStats, ICoreGameManagers coreGameManagers)
        {
            var cardManager = coreGameManagers.GetCardManager();
            int numCardsInHand = cardManager.GetHand().Count(c => c != _.cardState);
            return numCardsInHand;
        }
        internal static int HalfCountCardsInHand(CardStatistics.StatValueData _, IReadOnlyDictionary<CardState, CardStatsEntry> deckStats, ICoreGameManagers coreGameManagers)
        {
            return CountCardsInHand(_, deckStats, coreGameManagers) / 2;
        }
    }
}