using UnityEngine;
using UnityEngine.Events;

namespace ScriptableObjectArchitecture {
    [CreateAssetMenu(
        fileName = "ShortVariable.asset",
        menuName = SOArchitecture_Utility.ADVANCED_VARIABLE_SUBMENU + "short",
        order = SOArchitecture_Utility.ASSET_MENU_ORDER_COLLECTIONS + 14)]
    public class ShortVariable : BaseVariable<short> {
        public override bool Clampable => true;

        protected override short ClampValue(short value) {
            if (value.CompareTo(MinClampValue) < 0) return MinClampValue;
            if (value.CompareTo(MaxClampValue) > 0) return MaxClampValue;
            return value;
        }
    }
}