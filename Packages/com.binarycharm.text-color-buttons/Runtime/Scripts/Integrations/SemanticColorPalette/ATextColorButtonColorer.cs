/*
             @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
             Copyright (C) 2022 Binary Charm - All Rights Reserved
             @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
             @@@@@                  @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
             @@@@@@                        @@@@@@@@@@@@@@@@@@@@@@@
             @@@@@@@@                           @@@@@@@@@@@@@@@@@@
             @@@@@@@@@   @@@@@@@@@@@  @@@@@        @@@@@@@@@@@@@@@
             @@@@@@@@@@@  @@@@@@@@@  @@@@@@@@@@       (@@@@@@@@@@@
             @@@@@@@@@@@@  @@@@@@@@& @@@@@@@@@@ @@@@     @@@@@@@@@
             @@@@@@@@@@@@@ @@@@@@@@@@ *@@@@@@@ @@@@@@@@@*   @@@@@@
             @@@@@@@@@@@@@@@@@@@@@@@@@@      @@@@@@@@@@@@@@@@@@@@@
             @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
             @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
*/

#if SEMANTICCOLORPALETTE_PRESENT

using System;

using UnityEngine.UI;

using BinaryCharm.SemanticColorPalette;
using BinaryCharm.SemanticColorPalette.Colorers;
using BinaryCharm.SemanticColorPalette.Colorers.UI;

namespace BinaryCharm.UI.TextColorButtons.Integrations.SemanticColorPalette {

    [Serializable]
    public struct TextColorButtonColorsDefs {
        public SCP_SelectableColorsDef selectable;
        public SCP_ColorId targetGraphicMulColor;
        public SCP_SelectableColorsDef label;
    }

    public abstract class ATextColorButtonColorer<T> : SCP_AColorer<TextColorButtonColorsDefs> {

        protected override void applyPalette(SCP_IPaletteCore rPalette) {
            ATextColorButton<T> rBtn = GetComponent<ATextColorButton<T>>();
            applyPalette(rPalette, rBtn, GetColorIds());
        }

        protected static void applyPalette(SCP_IPaletteCore rPalette, ATextColorButton<T> rButton, TextColorButtonColorsDefs def) {
            Selectable rSelectable = rButton.GetComponent<Selectable>();
            SCP_SelectableColorer.applyPalette(rPalette, rSelectable, def.selectable);

            SCP_AGraphicColorer.applyPalette(rPalette, rButton.targetGraphic, def.targetGraphicMulColor);

            T rLabel = rButton.GetComponentInChildren<T>();
            if (rLabel != null) {
                TextColorBlock tcb = rButton.textColors;
                if (def.label.normal != SCP_ColorId.DO_NOT_APPLY) {
                    tcb.textNormalColor = rPalette.GetColor(def.label.normal);
                }
                if (def.label.highlighted != SCP_ColorId.DO_NOT_APPLY) {
                    tcb.textHighlightedColor = rPalette.GetColor(def.label.highlighted);
                }
                if (def.label.pressed != SCP_ColorId.DO_NOT_APPLY) {
                    tcb.textPressedColor = rPalette.GetColor(def.label.pressed);
                }
#if UNITY_2019_1_OR_NEWER
                if (def.label.selected != SCP_ColorId.DO_NOT_APPLY) {
                    tcb.textSelectedColor = rPalette.GetColor(def.label.selected);
                }
#endif
                if (def.label.disabled != SCP_ColorId.DO_NOT_APPLY) {
                    tcb.textDisabledColor = rPalette.GetColor(def.label.disabled);
                }
                rButton.textColors = tcb;
            }
        }
    }
}

#endif
