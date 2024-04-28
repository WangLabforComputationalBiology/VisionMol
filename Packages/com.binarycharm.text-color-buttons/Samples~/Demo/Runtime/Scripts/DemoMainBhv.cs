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

using BinaryCharm.UI.TextColorButtons;
using BinaryCharm.UI.TextColorButtons.Extensions;

namespace BinaryCharm.Samples.UI.TextColorButtons.Demo
{
    public class DemoMainBhv : MonoBehaviour
    {
        #pragma warning disable 649
        [SerializeField]
        private Transform m_rRandomColorButtonsFather;

        [SerializeField]
        private Button m_rSwapButton;

        [SerializeField]
        private Transform m_rBasicButtonsFather;

        [SerializeField]
        private TextColorButton m_rBgFadeButton;

        [SerializeField]
        private TextColorButton m_rBgAndTextFadeButton;

        [SerializeField]
        private TextColorButton m_rTextFadeButton;

        [SerializeField]
        private VarDisplayBhv m_rFadeDurationVarDisplay;
        #pragma warning restore 649

        private void Start() {

            // left setup
            m_rSwapButton.onClick.AddListener(swapLeftBtnsTextAndGfxColors);

            // center setup
            foreach (TextColorButton rTCB in m_rRandomColorButtonsFather.
                    GetComponentsInChildren<TextColorButton>()) {
                rTCB.onClick.AddListener(()=> {
                    float fNewHue = Random.Range(0f, 1f);
                    setCenterBtnsColor(fNewHue);
                });
            }

            // right setup
            m_rFadeDurationVarDisplay.setup("secs: ",
                false, 0f, 1f, 0.5f, setRightBtnsFadeDuration);
        }

        private void swapLeftBtnsTextAndGfxColors() {

            // notice how using the ITextColorButton interface you can get both
            // TextColorButton and TMP_TextColorButton instances...

            foreach (ITextColorButton rTCB in m_rBasicButtonsFather.
                    GetComponentsInChildren<ITextColorButton>()) {
                
                ColorBlock cb = rTCB.colors;
                cb.normalColor = rTCB.textColors.textNormalColor;
                cb.highlightedColor = rTCB.textColors.textHighlightedColor;
                cb.pressedColor = rTCB.textColors.textPressedColor;
                cb.disabledColor = rTCB.textColors.textDisabledColor;
#if UNITY_2019_1_OR_NEWER
                cb.selectedColor = rTCB.textColors.textSelectedColor;
#endif

                TextColorBlock tcb = rTCB.textColors;
                tcb.textNormalColor = rTCB.colors.normalColor;
                tcb.textHighlightedColor = rTCB.colors.highlightedColor;
                tcb.textPressedColor = rTCB.colors.pressedColor;
                tcb.textDisabledColor = rTCB.colors.disabledColor;
#if UNITY_2019_1_OR_NEWER
                tcb.textSelectedColor = rTCB.colors.selectedColor;
#endif

                rTCB.colors = cb;
                rTCB.textColors = tcb;
            }
        }

        private void setCenterBtnsColor(float fNewHue) {
            System.Func<Color, float, Color> getColorWithChangedHue =
                (Color c, float fHueToSet) => {
                    float fHue, fSat, fVal;
                    Color.RGBToHSV(c, out fHue, out fSat, out fVal);
                    return Color.HSVToRGB(fHueToSet, fSat, fVal);
                };
            foreach (TextColorButton rTCB in m_rRandomColorButtonsFather.
                    GetComponentsInChildren<TextColorButton>()) {
                fNewHue = (fNewHue + (1f / 9f)) % 1f;
                TextColorBlock cb = rTCB.textColors;
                cb.textNormalColor = getColorWithChangedHue(
                    cb.textNormalColor, fNewHue);
                cb.textHighlightedColor = getColorWithChangedHue(
                    cb.textHighlightedColor, fNewHue);
                cb.textPressedColor = getColorWithChangedHue(
                    cb.textPressedColor, fNewHue);
                cb.textDisabledColor = getColorWithChangedHue(
                    cb.textDisabledColor, fNewHue);
                cb.textSelectedColor = getColorWithChangedHue(
                    cb.textSelectedColor, fNewHue);

                rTCB.textColors = cb;
            }
        }

        private void setRightBtnsFadeDuration(float fDurationSecs) {
            // this example code uses the extension methods provided by
            // BinaryCharm.UI.TextColorButtons.Extensions

            m_rBgFadeButton.setGfxColorFadeDuration(fDurationSecs);

            m_rBgAndTextFadeButton.setGfxColorFadeDuration(fDurationSecs);
            m_rBgAndTextFadeButton.setTextColorFadeDuration(fDurationSecs);

            m_rTextFadeButton.setTextColorFadeDuration(fDurationSecs);
        }
    }
}