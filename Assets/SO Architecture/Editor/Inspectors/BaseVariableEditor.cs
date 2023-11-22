using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;

namespace ScriptableObjectArchitecture.Editor {
    [CustomEditor(typeof(BaseVariable<>), true)]
    public class BaseVariableEditor : UnityEditor.Editor {
        private BaseVariable Target => (BaseVariable)target;

        protected bool IsClampable => Target.Clampable;

        private SerializedProperty _valueProperty;
        private SerializedProperty _readOnly;
        private SerializedProperty _raiseWarning;
        private SerializedProperty _isClamped;
        private SerializedProperty _minValueProperty;
        private SerializedProperty _maxValueProperty;
        private SerializedProperty _revertProperty;

        private AnimBool _raiseWarningAnimation;
        private AnimBool _isClampedVariableAnimation;

        private StackTrace _stackTrace;

        private const string READONLY_TOOLTIP =
            "Should this value be changable during runtime? Will still be editable in the inspector regardless";

        protected virtual void OnEnable() {
            _valueProperty = serializedObject.FindProperty("_value");
            _readOnly = serializedObject.FindProperty("_readOnly");
            _raiseWarning = serializedObject.FindProperty("_raiseWarning");
            _isClamped = serializedObject.FindProperty("_isClamped");
            _minValueProperty = serializedObject.FindProperty("_minClampedValue");
            _maxValueProperty = serializedObject.FindProperty("_maxClampedValue");
            _revertProperty = serializedObject.FindProperty("_revert");

            _raiseWarningAnimation = new AnimBool(_readOnly.boolValue);
            _raiseWarningAnimation.valueChanged.AddListener(Repaint);

            _isClampedVariableAnimation = new AnimBool(_isClamped.boolValue);
            _isClampedVariableAnimation.valueChanged.AddListener(Repaint);

            _stackTrace = new StackTrace((IStackTraceObject)target);
            _stackTrace.OnRepaint.AddListener(Repaint);
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            DrawValue();

            EditorGUILayout.Space();

            DrawClampedFields();
            DrawReadonlyField();
            EditorGUILayout.PropertyField(_revertProperty);

            serializedObject.ApplyModifiedProperties();

            if (!SOArchitecturePreferences.IsDebugEnabled)
                EditorGUILayout.HelpBox("Debug mode disabled\nStack traces will not be filed on raise!", MessageType.Warning);

            _stackTrace.Draw();
        }

        void DrawValue() {
            GenericPropertyDrawer.DrawPropertyDrawerLayout(_valueProperty, Target.Type);
        }

        void DrawClampedFields() {
            if (!IsClampable)
                return;

            EditorGUILayout.PropertyField(_isClamped);
            _isClampedVariableAnimation.target = _isClamped.boolValue;

            using EditorGUILayout.FadeGroupScope anim = new(_isClampedVariableAnimation.faded);
            if (!anim.visible) return;
            using (new EditorGUI.IndentLevelScope()) {
                EditorGUILayout.PropertyField(_minValueProperty);
                EditorGUILayout.PropertyField(_maxValueProperty);
            }
        }

        protected void DrawReadonlyField() {
            EditorGUILayout.PropertyField(_readOnly, new GUIContent("Read Only", READONLY_TOOLTIP));

            _raiseWarningAnimation.target = _readOnly.boolValue;

            using EditorGUILayout.FadeGroupScope fadeGroup = new(_raiseWarningAnimation.faded);
            if (!fadeGroup.visible) return;
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_raiseWarning);
            EditorGUI.indentLevel--;
        }
    }
}