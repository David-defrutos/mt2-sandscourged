using System;
using System.Collections;
using System.Runtime.CompilerServices;
namespace mt2_sandscourged.Plugin
{
	public sealed class RelicEffectDrawOnCardEarlyDiscard : RelicEffectBase, IOnDiscardRelicEffect, IRelicEffect
	{
		private int _cardsToDraw = 0;
		public override PropDescriptions CreateEditorInspectorDescriptions()
		{
			PropDescriptions propDescriptions = [];
			string fieldName = RelicEffectFieldNames.ParamInt.GetFieldName();
			propDescriptions[fieldName] = new PropDescription("Num cards to draw", "", null, false);

			return propDescriptions;
		}


		public override void Initialize(RelicState relicState, RelicData relicData, RelicEffectData relicEffectData)
		{
			base.Initialize(relicState, relicData, relicEffectData);
			_cardsToDraw = relicEffectData.GetParamInt();
		}


		public bool TestEffectOnCardDiscarded(CardDiscardedRelicEffectParams relicEffectParams, ICoreGameManagers coreGameManagers)
		{
			return relicEffectParams.discardCardParams.triggeredByCard;
		}

		public IEnumerator ApplyEffectOnCardDiscarded(CardDiscardedRelicEffectParams relicEffectParams, ICoreGameManagers coreGameManagers)
		{
			base.NotifyRelicTriggered(relicEffectParams.relicManager, null, "", false, 0f);
			coreGameManagers.GetCardManager().DrawCards(this._cardsToDraw, null, CardType.Invalid);
			yield break;
		}
	}
}