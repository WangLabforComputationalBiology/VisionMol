/*
  Based on UnityEngine.UI/UI/Animation/CoroutineTween.cs
*/

using UnityEngine;
using UnityEngine.Events;

namespace BinaryCharm.UI.TextColorButtons.Impl
{
    internal struct ColorTween : ITweenValue
    {
        public class ColorTweenCallback : UnityEvent<Color> { }

        private ColorTweenCallback m_Target;
        private Color m_StartColor;
        private Color m_TargetColor;

        private float m_Duration;
        private bool m_IgnoreTimeScale;

        public Color startColor {
            get { return m_StartColor; }
            set { m_StartColor = value; }
        }

        public Color targetColor {
            get { return m_TargetColor; }
            set { m_TargetColor = value; }
        }

        public float duration {
            get { return m_Duration; }
            set { m_Duration = value; }
        }

        public void TweenValue(float floatPercentage) {
            if (!ValidTarget())
                return;

            var newColor = Color.Lerp(
                m_StartColor, m_TargetColor, floatPercentage);
            m_Target.Invoke(newColor);
        }

        public void AddOnChangedCallback(UnityAction<Color> callback) {
            if (m_Target == null)
                m_Target = new ColorTweenCallback();

            m_Target.AddListener(callback);
        }

        public float GetDuration() {
            return m_Duration;
        }

        public bool ValidTarget() {
            return m_Target != null;
        }
    }
}