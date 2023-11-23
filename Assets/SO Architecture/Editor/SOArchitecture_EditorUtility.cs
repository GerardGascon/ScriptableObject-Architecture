using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using Type = System.Type;

namespace ScriptableObjectArchitecture.Editor {
    public static class SOArchitecture_EditorUtility {
        static SOArchitecture_EditorUtility() {
            CreateDebugStyle();
        }

        /// <summary>
        /// A debug <see cref="GUIStyle"/> that allows for identification of EditorGUI Rect issues.
        /// </summary>
        public static GUIStyle DebugStyle { get; private set; }

        private const float DebugStyleBackgroundAlpha = 0.33f;

        private static PropertyDrawerGraph _propertyDrawerGraph;
        private static BindingFlags _fieldBindingsFlag = BindingFlags.Instance | BindingFlags.NonPublic;

        private class AssemblyDefinitionSurrogate {
            public string name = "";
        }

        private static void CreatePropertyDrawerGraph() {
            _propertyDrawerGraph = new PropertyDrawerGraph();
            HashSet<string> assemblyNamesToCheck = new HashSet<string>() {
                "Assembly-CSharp-Editor",
            };

            GetAllAssetDefinitionNames(assemblyNamesToCheck);

            string dataPath = Application.dataPath;
            string libraryPath = dataPath.Substring(0, dataPath.LastIndexOf('/')) + "/Library/ScriptAssemblies";

            foreach (string file in Directory.GetFiles(libraryPath)) {
                if (!assemblyNamesToCheck.Contains(Path.GetFileNameWithoutExtension(file)) ||
                    Path.GetExtension(file) != ".dll") continue;

                Assembly assembly = Assembly.LoadFrom(file);
                _propertyDrawerGraph.CreateGraph(assembly);
            }
        }

        private static void GetAllAssetDefinitionNames(HashSet<string> targetList) {
            string[] assemblyDefinitionGUIDs = AssetDatabase.FindAssets("t:asmdef");

            foreach (string guid in assemblyDefinitionGUIDs) {
                string path = AssetDatabase.GUIDToAssetPath(guid);

                if (path.StartsWith("Assets/")) {
                    string fullPath = Application.dataPath + path.Remove(0, path.IndexOf('/'));

                    targetList.Add(GetNameValueFromAssemblyDefinition(fullPath));
                }
            }
        }

        private static string GetNameValueFromAssemblyDefinition(string fullpath) {
            string allText = File.ReadAllText(fullpath);
            AssemblyDefinitionSurrogate surrogate = JsonUtility.FromJson<AssemblyDefinitionSurrogate>(allText);

            return surrogate.name;
        }

        private static void CreateDebugStyle() {
            DebugStyle = new GUIStyle();

            Color debugColor = Color.magenta;
            debugColor.a = DebugStyleBackgroundAlpha;

            DebugStyle.normal.background = CreateTexture(2, 2, debugColor);
        }

        /// <summary>
        /// Converts the entire rect of a <see cref="UnityEditorInternal.ReorderableList"/> element into a rect used for displaying a field
        /// </summary>
        public static Rect GetReorderableListElementFieldRect(Rect elementRect) {
            elementRect.height = EditorGUIUtility.singleLineHeight;
            elementRect.y++;

            return elementRect;
        }

        public static bool HasPropertyDrawer(Type type) {
            if (HasBuiltinPropertyDrawer(type))
                return true;

            if (_propertyDrawerGraph == null)
                CreatePropertyDrawerGraph();

            return _propertyDrawerGraph.HasPropertyDrawer(type);
        }

        private static bool HasBuiltinPropertyDrawer(Type type) {
            return type.IsPrimitive || type == typeof(string) || IsFromUnityAssembly(type);
        }

        private static bool IsFromUnityAssembly(Type type) {
            return type.Assembly == typeof(GameObject).Assembly;
        }

        [DidReloadScripts]
        private static void OnProjectReloaded() {
            _propertyDrawerGraph = null;
        }

        private static Texture2D CreateTexture(int width, int height, Color col) {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; ++i) {
                pix[i] = col;
            }

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }

        /// <summary>
        /// Goes through the entirety of the project and collects data about custom property drawers
        /// </summary>
        private class PropertyDrawerGraph {
            private List<Type> _supportedTypes = new List<Type>();
            private List<Type> _supportedInheritedTypes = new List<Type>();
            private List<Assembly> _checkedAssemblies = new List<Assembly>();

            public bool HasPropertyDrawer(Type type) {
                return _supportedTypes.Any(supportedType => supportedType == type) || _supportedInheritedTypes.Any(type.IsSubclassOf);
            }

            public void CreateGraph(Assembly assembly) {
                if (_checkedAssemblies.Contains(assembly))
                    return;

                _checkedAssemblies.Add(assembly);

                foreach (Type type in assembly.GetTypes()) {
                    object[] attributes = type.GetCustomAttributes(typeof(CustomPropertyDrawer), false);

                    foreach (object attribute in attributes) {
                        if (attribute is not CustomPropertyDrawer drawerData) continue;

                        bool useForChildren = (bool)typeof(CustomPropertyDrawer)
                            .GetField("m_UseForChildren", _fieldBindingsFlag)!.GetValue(drawerData);
                        Type targetType = (Type)typeof(CustomPropertyDrawer) .GetField("m_Type", _fieldBindingsFlag)!
                            .GetValue(drawerData);

                        if (useForChildren) {
                            _supportedInheritedTypes.Add(targetType);
                        } else {
                            _supportedTypes.Add(targetType);
                        }
                    }
                }
            }
        }
    }
}