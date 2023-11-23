using UnityEngine;
using UnityEngine.Events;

namespace ScriptableObjectArchitecture {
    [CreateAssetMenu(
        fileName = "SByteVariable.asset",
        menuName = SOArchitecture_Utility.ADVANCED_VARIABLE_SUBMENU + "sbyte",
        order = SOArchitecture_Utility.ASSET_MENU_ORDER_COLLECTIONS + 15)]
    public class SByteVariable : BaseVariable<sbyte> {
        public override bool Clampable => true;

        protected override sbyte ClampValue(sbyte value) {
            if (value.CompareTo(MinClampValue) < 0) return MinClampValue;
            if (value.CompareTo(MaxClampValue) > 0) return MaxClampValue;
            return value;
        }
    }
}