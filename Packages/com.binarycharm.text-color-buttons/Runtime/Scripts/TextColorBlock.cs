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

/*
  Based on UnityEngine.UI/UI/Core/ColorBlock.cs
*/

using System;

using UnityEngine;

namespace BinaryCharm.UI.TextColorButtons {

    /// <summary>
    /// struct similar to @UnityEngine.UI.ColorBlock but with different default
    /// values and member names, for improved usability and readability.
    /// </summary>
    [Serializable]
    public struct TextColorBlock : IEquatable<TextColorBlock>
    {
        [SerializeField]
        internal Color m_TextNormalColor;

        [SerializeField]
        internal Color m_TextHighlightedColor;

        [SerializeField]
        internal Color m_TextPressedColor;

        [SerializeField]
        internal Color m_TextSelectedColor;

        [SerializeField]
        internal Color m_TextDisabledColor;

        [Range(1, 5)]
        [SerializeField]
        internal float m_TextColorMultiplier;

        [SerializeField]
        internal float m_TextFadeDuration;

        /// <summary>
        /// Color for label text in the "normal" button state.
        /// </summary>
        public Color textNormalColor {
            get { return m_TextNormalColor; }
            set { m_TextNormalColor = value; } 
        }

        /// <summary>
        /// Color for label text in the "highlighted" button state.
        /// </summary>
        public Color textHighlightedColor {
            get { return m_TextHighlightedColor; } 
            set { m_TextHighlightedColor = value; } 
        }

        /// <summary>
        /// Color for label text in the "pressed" button state.
        /// </summary>
        public Color textPressedColor { 
            get { return m_TextPressedColor; }
            set { m_TextPressedColor = value; } 
        }

#if UNITY_2019_1_OR_NEWER
        /// <summary>
        /// Color for label text in the "selected" button state.
        /// </summary>
        public Color textSelectedColor { 
            get { return m_TextSelectedColor; }
            set { m_TextSelectedColor = value; } 
        }
#endif

        /// <summary>
        /// Color for label text in the "disabled" button state.
        /// </summary>
        public Color textDisabledColor { 
            get { return m_TextDisabledColor; }
            set { m_TextDisabledColor = value; } 
        }

        /// <summary>
        /// Color multiplier for label text.
        /// </summary>
        public float textColorMultiplier { 
            get { return m_TextColorMultiplier; }
            set { m_TextColorMultiplier = value; } 
        }

        /// <summary>
        /// Fade duration for the label text, in seconds.
        /// </summary>
        public float textFadeDuration { 
            get { return m_TextFadeDuration; } 
            set { m_TextFadeDuration = value; } 
        }

        /// <summary>
        /// Default settings definition.
        /// </summary>
        public static TextColorBlock defaultColorBlock {
            get {
                var c = new TextColorBlock {
                    m_TextNormalColor = new Color32(0, 0, 0, 255),
                    m_TextHighlightedColor = new Color32(5, 5, 5, 255),
                    m_TextPressedColor = new Color32(55, 55, 55, 255),
                    m_TextSelectedColor = new Color32(10, 10, 10, 255),
                    m_TextDisabledColor = new Color32(55, 55, 55, 128),
                    m_TextColorMultiplier = 1.0f,
                    m_TextFadeDuration = 0.1f
                };
                return c;
            }
        }

        /// <inheritdoc/>
        public override bool Equals(object obj) {
            if (!(obj is TextColorBlock))
                return false;

            return Equals((TextColorBlock)obj);
        }

        public bool Equals(TextColorBlock other) {
            return textNormalColor == other.textNormalColor &&
                textHighlightedColor == other.textHighlightedColor &&
                textPressedColor == other.textPressedColor &&
                textSelectedColor == other.textSelectedColor &&
                textDisabledColor == other.textDisabledColor &&
                textColorMultiplier == other.textColorMultiplier &&
                textFadeDuration == other.textFadeDuration;
        }

        public static bool operator ==(TextColorBlock b1, TextColorBlock b2) {
            return b1.Equals(b2);
        }

        public static bool operator !=(TextColorBlock b1, TextColorBlock b2) {
            return !b1.Equals(b2);
        }

        /// <inheritdoc/>
        public override int GetHashCode() {
            return base.GetHashCode();
        }

    }

}
