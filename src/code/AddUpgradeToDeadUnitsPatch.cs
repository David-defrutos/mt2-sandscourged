using HarmonyLib;
using System.Collections;
using static CardManager;
using System.Diagnostics;

namespace mt2_sandscourged.Plugin
{
    [HarmonyPatch(typeof(CombatManager), nameof(CombatManager.ShowKillCam))]
    class AddUpgradeToDeadUnitsPatch
    {
        public static void Prefix(CombatManager __instance, CardManager ___cardManager)
        {
            // This patch is used to copy the exhausted pile to a static list for later use.
            // We do this before ShowKillCam, because it clears cards before celebrate.
            // The second clear cards in OnScenarioComplete seems to be redundant.
            // This patch is necessary for the CardEffectAddUpgradeToDeadUnits to function correctly.
            CardEffectAddUpgradeToDeadUnits.consumedCards ??= ___cardManager.GetExhaustedPile(true);
        }
    }

    [HarmonyPatch(typeof(CombatManager), nameof(CombatManager.StartCombat))]
    class ClearListOnStartOfBattlePatch
    {
        public static void Postfix(CombatManager __instance)
        {
            CardEffectAddUpgradeToDeadUnits.consumedCards = null;
        }
    }
}
