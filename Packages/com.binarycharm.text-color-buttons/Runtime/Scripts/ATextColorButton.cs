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

using BinaryCharm.UI.TextColorButtons.Impl;

namespace BinaryCharm.UI.TextColorButtons {

    /// <summary>
    /// Abstract class extending @UnityEngine.UI.Button so that it also updates the text label of the button when the
    /// button state changes. The concrete extensions of this class take care of actually applying the color to the
    /// label text component, depending on its actual type (@UnityEngine.UI.Text or @TMPro.TMP_Text).
    /// </summary>
    /// <typeparam name="T">type of the label text component</typeparam>
    public abstract class ATextColorButton<T> : Button, ITextColorButton {

        [SerializeField]
        internal TextColorBlock m_TextColors = TextColorBlock.defaultColorBlock;

        /// <summary>
        /// Property to access the button label colors, similarly to how one accesses a @UnityEngine.UI.Button
        /// colors through the `colors` property.
        /// </summary>
        public TextColorBlock textColors { 
            get {
                return m_TextColors; 
            } 
            set { 
                if (SetPropertyUtility.SetStruct(ref m_TextColors, value))
                    OnSetProperty();
            }
        }

        [System.NonSerialized]
        private readonly TweenRunner<ColorTween> m_ColorTweenRunner;

        /// <summary>
        /// Reference to the label text component.
        /// </summary>
        private T m_rText;

        protected T getTextComponent() {
            if (m_rText == null)
                m_rText = gameObject.GetComponentInChildren<T>();
            return m_rText;
            //return m_rText ??= gameObject.GetComponentInChildren<T>();
        }

        protected ATextColorButton() {
            if (m_ColorTweenRunner == null)
                m_ColorTweenRunner = new TweenRunner<ColorTween>();
            m_ColorTweenRunner.Init(this);
        }

        private void OnSetProperty() {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                InternalEvaluateAndTransitionToSelectionState(true);
            else
#endif
                InternalEvaluateAndTransitionToSelectionState(false);
        }

        private void InternalEvaluateAndTransitionToSelectionState(bool instant) {
            var transitionState = currentSelectionState;
            if (IsActive() && !IsInteractable())
                transitionState = SelectionState.Disabled;
            DoStateTransition(transitionState, instant);
        }

        protected override void DoStateTransition(SelectionState state, bool instant) {
            //Debug.Log("SelectionState = " + state);
            base.DoStateTransition(state, instant);
            Color tintColor;
            switch (state) {
                case SelectionState.Normal:
                    tintColor = m_TextColors.textNormalColor;
                    break;
                case SelectionState.Highlighted:
                    tintColor = m_TextColors.textHighlightedColor;
                    break;
                case SelectionState.Pressed:
                    tintColor = m_TextColors.textPressedColor;
                    break;
                case SelectionState.Disabled:
                    tintColor = m_TextColors.textDisabledColor;
                    break;
#if UNITY_2019_1_OR_NEWER
                case SelectionState.Selected:
                    tintColor = m_TextColors.textSelectedColor;
                    break;
#endif
                default:
                    tintColor = Color.black;
                    break;
            }

            StartColorTween(tintColor * textColors.textColorMultiplier, instant);
        }

        void StartColorTween(Color targetColor, bool instant) {
            float duration = instant ? 0f : m_TextColors.textFadeDuration;
            Color currentColor = getTextColor();
            if (currentColor.Equals(targetColor))
                return;

            var colorTween = new ColorTween {
                duration = duration, 
                startColor = getTextColor(),
                targetColor = targetColor 
            };
            colorTween.AddOnChangedCallback(setTextColor);
            m_ColorTweenRunner.StartTween(colorTween);
        }

        protected abstract void setTextColor(Color c);
        protected abstract Color getTextColor();
    
    }

}
