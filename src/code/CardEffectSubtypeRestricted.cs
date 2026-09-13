using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace mt2_sandscourged.Plugin
{
 public sealed class CardEffectSubtypeRestricted : CardEffectBase
    {

        private SubtypeData? subtypeToRestrict;
        private bool isWhitelist;

        public override PropDescriptions CreateEditorInspectorDescriptions()
        {
            return new PropDescriptions
            {
                [CardEffectFieldNames.ParamBool.GetFieldName()] = new PropDescription("True = whitelist the subtype. False = blacklist the subtype."),
                [CardEffectFieldNames.ParamSubtype.GetFieldName()] = new PropDescription("All subtypes to restrict by")
            };
        }

        public override void Setup(CardEffectState cardEffectState)
        {
            base.Setup(cardEffectState);
            subtypeToRestrict = cardEffectState.GetParamSubtype();
            isWhitelist = cardEffectState.GetParamBool();
        }

        public override bool TestEffect(CardEffectState cardEffectState, CardEffectParams cardEffectParams, ICoreGameManagers coreGameManagers)
        {
            if (subtypeToRestrict == null)
                return false;

            bool atLeastOneMet = false;
            foreach (CharacterState target in cardEffectParams.targets)
            {
                if (target.GetHasSubtype(subtypeToRestrict))
                    atLeastOneMet = true;
            }
            return atLeastOneMet == isWhitelist;              
        }

        public override IEnumerator ApplyEffect(CardEffectState cardEffectState, CardEffectParams cardEffectParams, ICoreGameManagers coreGameManagers, ISystemManagers sysManagers)
        {
            yield break;
        }
    }
}