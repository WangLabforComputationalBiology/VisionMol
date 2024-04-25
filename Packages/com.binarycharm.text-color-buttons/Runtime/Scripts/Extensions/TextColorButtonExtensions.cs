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

namespace BinaryCharm.UI.TextColorButtons.Extensions {

    /// <summary>
    /// Extension methods to easily modify a single value of the `textColors` 
    /// @BinaryCharm.UI.TextColorButtons.TextColorBlock struct of a @BinaryCharm.UI.TextColorButtons.ITextColorButton
    /// (interface implemented by @BinaryCharm.UI.TextColorButtons.TextColorButton and 
    /// @BinaryCharm.UI.TextColorButtons.TMP_TextColorButton).
    /// </summary>
    public static class TextColorButtonExtensions {

        /// <summary>
        /// Sets the color fade duration of button <paramref name="rB"/> label to <paramref name="fFadeDurationSecs"/>
        /// seconds.
        /// </summary>
        /// <param name="rB">reference to button</param>
        /// <param name="fFadeDurationSecs">fade duration in seconds</param>
        public static void setTextColorFadeDuration(this ITextColorButton rB, float fFadeDurationSecs) {
            TextColorBlock cb = rB.textColors;
            cb.textFadeDuration = fFadeDurationSecs;
            rB.textColors = cb;
        }

        /// <summary>
        /// Sets the color multiplier of button <paramref name="rB"/> label to <paramref name="fColorMultiplier"/>.
        /// </summary>
        /// <param name="rB">reference to button</param>
        /// <param name="fColorMultiplier">desired `colorMultiplier` value</param>
        public static void setTextColorMultiplier(this ITextColorButton rB, float fColorMultiplier) {
            TextColorBlock cb = rB.textColors;
            cb.textColorMultiplier = fColorMultiplier;
            rB.textColors = cb;
        }

        /// <summary>
        /// Sets the color for the "normal" state of button <paramref name="rB"/> label.
        /// </summary>
        /// <param name="rB">reference to button</param>
        /// <param name="c">color value to set</param>
        public static void setNormalTextColor(this ITextColorButton rB, Color c) {
            TextColorBlock cb = rB.textColors;
            cb.textNormalColor = c;
            rB.textColors = cb;
        }

        /// <summary>
        /// Sets the color for the "highlighted" state of button <paramref name="rB"/> label.
        /// </summary>
        /// <param name="rB">reference to button</param>
        /// <param name="c">color value to set</param>
        public static void setHighlightedTextColor(this ITextColorButton rB, Color c) {
            TextColorBlock cb = rB.textColors;
            cb.textHighlightedColor = c;
            rB.textColors = cb;
        }

        /// <summary>
        /// Sets the color for the "pressed" state of button <paramref name="rB"/> label.
        /// </summary>
        /// <param name="rB">reference to button</param>
        /// <param name="c">color value to set</param>
        public static void setPressedTextColor(this ITextColorButton rB, Color c) {
            TextColorBlock cb = rB.textColors;
            cb.textPressedColor = c;
            rB.textColors = cb;
        }

        /// <summary>
        /// Sets the color for the "disabled" state of button <paramref name="rB"/> label.
        /// </summary>
        /// <param name="rB">reference to button</param>
        /// <param name="c">color value to set</param>
        public static void setDisabledTextColor(this ITextColorButton rB, Color c) {
            TextColorBlock cb = rB.textColors;
            cb.textDisabledColor = c;
            rB.textColors = cb;
        }

#if UNITY_2019_1_OR_NEWER
        /// <summary>
        /// Sets the color for the "selected" state of button <paramref name="rB"/> label.
        /// </summary>
        /// <param name="rB">reference to button</param>
        /// <param name="c">color value to set</param>
        public static void setSelectedTextColor(this ITextColorButton rB, Color c) {
            TextColorBlock cb = rB.textColors;
            cb.textSelectedColor = c;
            rB.textColors = cb;
        }
#endif
    }
}