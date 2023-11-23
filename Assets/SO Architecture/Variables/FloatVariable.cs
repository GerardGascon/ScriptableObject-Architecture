using UnityEngine;
using UnityEngine.Events;

namespace ScriptableObjectArchitecture {
    [CreateAssetMenu(
        fileName = "FloatVariable.asset",
        menuName = SOArchitecture_Utility.VARIABLE_SUBMENU + "float",
        order = SOArchitecture_Utility.ASSET_MENU_ORDER_COLLECTIONS + 3)]
    public class FloatVariable : BaseVariable<float> {
        public override bool Clampable => true;

        protected override float ClampValue(float value) {
            if (value.CompareTo(MinClampValue) < 0) return MinClampValue;
            if (value.CompareTo(MaxClampValue) > 0) return MaxClampValue;
            return value;
        }

        protected override bool AreValuesEqual(float a, float b) {
            return Mathf.Abs(a - b) < Mathf.Epsilon;
        }
    }
}