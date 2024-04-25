/*
  Based on UnityEngine.UI/UI/Animation/CoroutineTween.cs
*/

namespace BinaryCharm.UI.TextColorButtons.Impl
{
    internal interface ITweenValue
    {
        void TweenValue(float floatPercentage);
        float duration { get; }
        bool ValidTarget();
    }
}