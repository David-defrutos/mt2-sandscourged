using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using I2.Loc;
using Microsoft.Extensions.Configuration;
using ShinyShoe.Logging;
using SimpleInjector;
using TrainworksReloaded.Base;
using TrainworksReloaded.Base.Card;
using TrainworksReloaded.Base.CardUpgrade;
using TrainworksReloaded.Base.Character;
using TrainworksReloaded.Base.Class;
using TrainworksReloaded.Base.Effect;
using TrainworksReloaded.Base.Localization;
using TrainworksReloaded.Base.Prefab;
using TrainworksReloaded.Base.Trait;
using TrainworksReloaded.Base.Trigger;
using TrainworksReloaded.Core;
using TrainworksReloaded.Core.Impl;
using TrainworksReloaded.Core.Interfaces;
using TrainworksReloaded.Core.Extensions;
using UnityEngine;
using UnityEngine.AddressableAssets;
using TrainworksReloaded.Base.Extensions;
using Conductor.Extensions;
using static BalanceData;
using System.Reflection;

namespace mt2_sandscourged.Plugin
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger = new(MyPluginInfo.PLUGIN_GUID);
        public void Awake()
        {
            // Plugin startup logic
            Logger = base.Logger;
            //StreamWriter writer = new("localisation.csv", false);

            var builder = Railhead.GetBuilder();
            builder.Configure(
                MyPluginInfo.PLUGIN_GUID,
                c =>
                {
                    // Be sure to include all of your json files if you add more.
                    // Be sure to update the project configuration if you include more folders
                    //   the project only copies json files in the json folder and not in subdirectories.
                    c.AddMergedJsonFile(
                        "json/plugin.json",

                        // Class stuff
                        "json/class.json",

                        // Champions
                        "json/champions/champion_Pharaoh.json",
                        "json/champions/champion_Vizier.json",

                        // Units
                        "json/units/unit_OverseerEternal.json",
                        "json/units/unit_ReveredScarab.json",
                        "json/units/unit_GildedScarab.json",
                        "json/units/unit_CursedScarab.json",
                        "json/units/unit_ScarabSwarm.json",
                        "json/units/unit_Wretch.json",
                        "json/units/unit_PearlHost.json",
                        "json/units/unit_SphinxHierarch.json",
                        "json/units/unit_TwistedHusk.json",
                        "json/units/unit_ForgottenHulk.json",
                        "json/units/unit_PriestOfLostThoughts.json",
                        "json/units/unit_Revenant.json",
                        "json/units/unit_SuperiorEgo.json",
                        "json/units/unit_Evermaw.json",

                        // Spells
                        "json/spells/card_Pitfall.json",
                        "json/spells/card_OblivionSands.json",
                        "json/spells/card_Sandstorm.json",
                        "json/spells/card_Rancor.json",
                        "json/spells/card_InMemoriam.json",
                        "json/spells/card_TormentA.json",
                        "json/spells/card_TormentB.json",
                        "json/spells/card_TormentC.json",
                        "json/spells/card_TormentD.json",
                        "json/spells/card_TormentE.json",
                        "json/spells/card_Desiccate.json",
                        "json/spells/card_Efface.json",
                        "json/spells/card_Beseech.json",
                        "json/spells/card_Jinx.json",
                        "json/spells/card_Malediction.json",
                        "json/spells/card_SpikeOfTheScourged.json",
                        "json/spells/card_Idolize.json",
                        "json/spells/card_Expose.json",
                        "json/spells/card_PartTheStorm.json",
                        "json/spells/card_GravenRite.json",
                        "json/spells/card_ForgottenTome.json",
                        "json/spells/card_SelectiveMemory.json",
                        "json/spells/card_PainfulMemories.json",

                        // Equipment
                        "json/equipments/equipment_ScryingEye.json",
                        "json/equipments/equipment_PearlTap.json",
                        "json/equipments/equipment_SpearOfTheDeserted.json",
                        "json/equipments/equipment_CorruptedSceptre.json",

                        // Rooms
                        "json/rooms/room_OblivionChamber.json",
                        "json/rooms/room_ScarabPit.json",

                        //Blights
                        "json/blights/card_PlagueOfScarabs.json",
                        "json/blights/card_PlagueOfBlood.json",
                        "json/blights/card_PlagueOfGnarling.json",

                        // Scourges
                        "json/scourges/card_Vexation.json",

                        // Relics
                        "json/relics/relic_WornHourglass.json",
                        "json/relics/relic_CarnelianMask.json",
                        "json/relics/relic_CursedTablet.json",
                        "json/relics/relic_BrokenSeal.json",
                        "json/relics/relic_EvokingFlute.json",
                        "json/relics/relic_OblivionCrook.json",
                        "json/relics/relic_ShiningPearl.json",
                        "json/relics/relic_TwistedReed.json",
                        "json/relics/relic_CanopicJar.json",
                        "json/relics/relic_PolishedIdol.json",
                        "json/relics/relic_SandBowl.json",

                        // Enhancers
                        "json/enhancers/enhancer_Scarabstone.json",

                        // Status Effects
                        "json/status_effects/hex.json",
                        "json/status_effects/scourge.json",

                        // Card Effects
                        "json/card_effects/PurgeCardFromHand.json",

                        // Card Traits
                        "json/card_traits/ScalingByHandCount.json",

                        // Tracked Values
                        "json/tracked_values/tracked_values.json"
                    );
                }
            );

            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

            // This is here just to make Spike of the Scourged work.
            Railend.ConfigurePostAction(
  c =>
  {
      var cardPoolManager = c.GetInstance<IRegister<CardPool>>();
      var id = MyPluginInfo.PLUGIN_GUID.GetId(TemplateConstants.CardPool, "TormentPool");
      CardTraitScalingAddBattleCardToHandFromPool.paramCardPool = cardPoolManager.GetValueOrDefault(id);

      // This is all just how we make the Scourges/Blights play their animations at the same time.
      // GameDataClient has all of the relevant Provider Objects, We get the instance from the SimpleInjector Container object.
      var client = c.GetInstance<GameDataClient>();

      // Get a provider (SaveManager) from GameDataClient.
      if (!client.TryGetProvider<SaveManager>(out var saveManager))
      {
          Logger.LogError("Failed to get SaveManager instance please report this https://github.com/Monster-Train-2-Modding-Group/Balance-Configurator/issues");
          return;
      }
      var allGameData = saveManager.GetAllGameData();
      List<CardData?> cardsToAdd = [
        allGameData.FindCardDataByName("mt2_sandscourged.Plugin-Card-Vexation"),
        allGameData.FindCardDataByName("mt2_sandscourged.Plugin-Card-PlagueOfBlood"),
        allGameData.FindCardDataByName("mt2_sandscourged.Plugin-Card-PlagueOfScarabs"),
        allGameData.FindCardDataByName("mt2_sandscourged.Plugin-Card-PlagueOfGnarling")
      ];
      var balanceData = allGameData.GetBalanceData();

      if (SafeGetField(balanceData, "cardsThatResolveSimultaneouslyOnUnplayed") is not List<CardData> simultaneousReserveCards)
      {
          Logger.LogError("Couldn't find simultaneous reserve cards");
          return;
      }
      cardsToAdd.ForEach(c => simultaneousReserveCards.AddIfNotNull(c));
      SafeSetField(balanceData, "cardsThatResolveSimultaneouslyOnUnplayed", simultaneousReserveCards);

      // TrackedValue implementation wiring.
      var trackedValueRegister = c.GetInstance<IRegister<CardStatistics.TrackedValueType>>();
      CardStatistics.TrackedValueType GetTrackedValueType(string id)
      {
          return trackedValueRegister.GetValueOrDefault(MyPluginInfo.PLUGIN_GUID.GetId(TemplateConstants.TrackedValueTypeEnum, id));
      }
      GetTrackedValueType("CardsInHand").SetTrackedValueGetter(TrackedValueFunctions.CountCardsInHand);
      GetTrackedValueType("HalfCardsInHand").SetTrackedValueGetter(TrackedValueFunctions.HalfCountCardsInHand);

      //var localisation = c.GetInstance<CustomLocalizationTermRegistry>()!;
      //foreach (var l in localisation)
      //{
      //    Logger.LogInfo($"{l.Key}: {l.Value.English}");
      //    var escapedValue = l.Value.English.Replace("\"", "\"\""); // Double up any quotes
      //    writer.WriteLine($"{l.Key},\"{escapedValue}\"");
      //}
  }
);
            Conductor.Utilities.SetupTraitTooltips(this.GetType().Assembly);
            Conductor.Utilities.SetupCardEffectTooltips(this.GetType().Assembly);
            //SetupTraitTooltips(this.GetType().Assembly);
            //SetupCardEffectTooltips(this.GetType().Assembly);

            // Uncomment if you need harmony patches, if you are writing your own custom effects.
            var harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
            harmony.PatchAll();
        }


        /// <summary>
        /// Function to use reflection to set a field on BalanceData without throwing an exception if it fails.
        /// Taken from Balance Configurator: https://github.com/Monster-Train-2-Modding-Group/Balance-Configurator/blob/main/BalanceConfigurator.Plugin/Plugin.cs
        /// </summary>
        /// <param name="balanceData"></param>
        /// <param name="field"></param>
        /// <param name="obj"></param>
        private void SafeSetField<T>(T? data, string field, object? obj)
        {
            if (data == null)
            {
                Logger.LogError($"Internal Error data is null");
                throw new ArgumentNullException(nameof(data));
            }
            else if (obj == null)
            {
                Logger.LogWarning($"Not setting {typeof(T).Name} field {field} because the value to set is null (value specified may be invalid or field not present)");
                return;
            }
            try
            {
                Logger.LogDebug($"Setting {typeof(T).Name} field {field} to {obj}");
                AccessTools.Field(data.GetType(), field).SetValue(data, obj);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Could not set {typeof(T).Name} field {field} because of an exception {ex.Message}");
            }
        }

        /// Function to use reflection to set a field on BalanceData without throwing an exception if it fails.
        /// Taken from Balance Configurator: https://github.com/Monster-Train-2-Modding-Group/Balance-Configurator/blob/main/BalanceConfigurator.Plugin/Plugin.cs
        /// </summary>
        /// <param name="balanceData"></param>
        /// <param name="field"></param>
        /// <param name="obj"></param>
        private object? SafeGetField(BalanceData balanceData, string field)
        {
            try
            {
                Logger.LogDebug($"Getting BalanceData field {field}");
                return AccessTools.Field(balanceData.GetType(), field).GetValue(balanceData);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Could not get BalanceData field {field} because of an exception {ex.Message}");
            }
            return null;
        }
    }
}
