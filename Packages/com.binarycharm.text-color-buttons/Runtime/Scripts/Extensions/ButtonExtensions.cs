/*
             @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
             Copyright (C) 2021 Binary Charm - All Rights Reserved
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

using UnityEngine;
using UnityEngine.UI;

namespace BinaryCharm.UI.TextColorButtons.Extensions {

    /// <summary>
    /// Extension methods to easily modify a single value of the `colors`
    /// @UnityEngine.UI.ColorBlock struct of a @UnityEngine.UI.Button
    /// </summary>
    public static class ButtonExtensions {

        /// <summary>
        /// Sets the color fade duration of button <paramref name="rB"/> to <paramref name="fFadeDurationSecs"/> seconds.
        /// </summary>
        /// <param name="rB">reference to button</param>
        /// <param name="fFadeDurationSecs">fade duration in seconds</param>
        public static void setGfxColorFadeDuration(this Button rB, float fFadeDurationSecs) {
            ColorBlock cb = rB.colors;
            cb.fadeDuration = fFadeDurationSecs;
            rB.colors = cb;
        }

        /// <summary>
        /// Sets the color multiplier of button <paramref name="rB"/> to <paramref name="fColorMultiplier"/>.
        /// </summary>
        /// <param name="rB">reference to button</param>
        /// <param name="fColorMultiplier">desired `colorMultiplier` value</param>
        public static void setGfxColorMultiplier(this Button rB, float fColorMultiplier) {
            ColorBlock cb = rB.colors;
            cb.colorMultiplier = fColorMultiplier;
            rB.colors = cb;
        }

        /// <summary>
        /// Sets the color for the "normal" state of button <paramref name="rB"/>.
        /// </summary>
        /// <param name="rB">reference to button</param>
        /// <param name="c">color value to set</param>
        public static void setNormalGfxColor(this Button rB, Color c) {
            ColorBlock cb = rB.colors;
            cb.normalColor = c;
            rB.colors = cb;
        }

        /// <summary>
        /// Sets the color for the "highlighted" state of button <paramref name="rB"/>.
        /// </summary>
        /// <param name="rB">reference to button</param>
        /// <param name="c">color value to set</param>
        public static void setHighlightedGfxColor(this Button rB, Color c) {
            ColorBlock cb = rB.colors;
            cb.highlightedColor = c;
            rB.colors = cb;
        }

        /// <summary>
        /// Sets the color for the "pressed" state of button <paramref name="rB"/>.
        /// </summary>
        /// <param name="rB">reference to button</param>
        /// <param name="c">color value to set</param>
        public static void setPressedGfxColor(this Button rB, Color c) {
            ColorBlock cb = rB.colors;
            cb.pressedColor = c;
            rB.colors = cb;
        }

        /// <summary>
        /// Sets the color for the "disabled" state of button <paramref name="rB"/>.
        /// </summary>
        /// <param name="rB">reference to button</param>
        /// <param name="c">color value to set</param>
        public static void setDisabledGfxColor(this Button rB, Color c) {
            ColorBlock cb = rB.colors;
            cb.disabledColor = c;
            rB.colors = cb;
        }

#if UNITY_2019_1_OR_NEWER
        /// <summary>
        /// Sets the color for the "selected" state of button <paramref name="rB"/>.
        /// </summary>
        /// <param name="rB">reference to button</param>
        /// <param name="c">color value to set</param>
        public static void setSelectedGfxColor(this Button rB, Color c) {
            ColorBlock cb = rB.colors;
            cb.selectedColor = c;
            rB.colors = cb;
        }
#endif
    }
}