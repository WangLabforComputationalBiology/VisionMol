/*
  Based on UnityEngine.UI/UI/Core/SetPropertyUtility.cs
*/

namespace BinaryCharm.UI.TextColorButtons.Impl
{
    internal static class SetPropertyUtility
    {
        public static bool SetStruct<T>(ref T currentValue, T newValue) 
                where T : struct {
            if (currentValue.Equals(newValue))
                return false;

            currentValue = newValue;
            return true;
        }
    }
}