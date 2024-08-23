using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using PlatformCrafterModularSystem;
using NaughtyAttributes;

namespace PlatformCrafterModularSystem
{
    [CustomEditor(typeof(ModularBrain))]
    public class ModularBrainEditor : Editor
    {
        private SerializedProperty horizontalMovementModuleProperty;
        private SerializedProperty verticalMovementModuleProperty;
        private SerializedProperty actionModulesProperty;
        private SerializedProperty interactionModulesProperty;
        private SerializedProperty resourceModulesProperty;
        private SerializedProperty inventoryModulesProperty;
        private SerializedProperty customModulesProperty;
        private SerializedProperty animationModuleProperty;
        private SerializedProperty soundEffectModuleProperty;

        private bool showPhysicsModules;
        private bool showActionInteractionModules;
        private bool showContainerModules;
        private bool showVisualsAudioModules;
        private bool showCustomModules;

        private bool showHorizontalMovementModule;
        private bool showVerticalMovementModule;
        private bool showAnimationModule;
        private bool showSoundEffectModule;


        private List<bool> actionFoldouts = new List<bool>();
        private List<bool> interactionFoldouts = new List<bool>();
        private List<bool> resourceFoldouts = new List<bool>();
        private List<bool> inventoryFoldouts = new List<bool>();
        private List<bool> customFoldouts = new List<bool>();

        private Dictionary<string, Texture2D> icons;

        private SerializedProperty disableEditorFeaturesProperty;

        private void OnEnable()
        {
            horizontalMovementModuleProperty = serializedObject.FindProperty("horizontalMovementModule");
            verticalMovementModuleProperty = serializedObject.FindProperty("verticalMovementModule");
            actionModulesProperty = serializedObject.FindProperty("actionModules");
            interactionModulesProperty = serializedObject.FindProperty("interactionModules");
            resourceModulesProperty = serializedObject.FindProperty("resourceModules");
            inventoryModulesProperty = serializedObject.FindProperty("inventoryModules");
            animationModuleProperty = serializedObject.FindProperty("animationModule");
            soundEffectModuleProperty = serializedObject.FindProperty("soundEffectModule");
            customModulesProperty = serializedObject.FindProperty("customModules");

            disableEditorFeaturesProperty = serializedObject.FindProperty("disableEditorFeatures");

            InitializeFoldouts();
            LoadFoldoutStates();
            LoadIcons();
        }
        private void OnDisable()
        {
            SaveFoldoutStates();
        }

        private void LoadFoldoutStates()
        {
            showPhysicsModules = EditorPrefs.GetBool("ModularBrain_ShowPhysicsModules", true);
            showActionInteractionModules = EditorPrefs.GetBool("ModularBrain_ShowActionInteractionModules", true);
            showContainerModules = EditorPrefs.GetBool("ModularBrain_ShowContainerModules", true);
            showVisualsAudioModules = EditorPrefs.GetBool("ModularBrain_ShowAudioVisualsModules", true);
            showCustomModules = EditorPrefs.GetBool("ModularBrain_CustomFoldouts", true);

            showHorizontalMovementModule = EditorPrefs.GetBool("ModularBrain_ShowHorizontalMovementModule", true);
            showVerticalMovementModule = EditorPrefs.GetBool("ModularBrain_ShowVerticalMovementModule", true);
            showAnimationModule = EditorPrefs.GetBool("ModularBrain_ShowAnimationModule", true);
            showSoundEffectModule = EditorPrefs.GetBool("ModularBrain_ShowSFXModule", true);

            LoadFoldoutList(actionFoldouts, actionModulesProperty, "ModularBrain_ActionFoldouts");
            LoadFoldoutList(interactionFoldouts, interactionModulesProperty, "ModularBrain_InteractionFoldouts");
            LoadFoldoutList(resourceFoldouts, resourceModulesProperty, "ModularBrain_ResourceFoldouts");
            LoadFoldoutList(inventoryFoldouts, inventoryModulesProperty, "ModularBrain_InventoryFoldouts");
            LoadFoldoutList(customFoldouts, customModulesProperty, "ModularBrain_CustomFoldouts");
        }

        private void SaveFoldoutStates()
        {
            EditorPrefs.SetBool("ModularBrain_ShowPhysicsModules", showPhysicsModules);
            EditorPrefs.SetBool("ModularBrain_ShowActionInteractionModules", showActionInteractionModules);
            EditorPrefs.SetBool("ModularBrain_ShowContainerModules", showContainerModules);
            EditorPrefs.SetBool("ModularBrain_CustomFoldouts", showCustomModules);

            EditorPrefs.SetBool("ModularBrain_ShowHorizontalMovementModule", showHorizontalMovementModule);
            EditorPrefs.SetBool("ModularBrain_ShowVerticalMovementModule", showVerticalMovementModule);
            EditorPrefs.SetBool("ModularBrain_ShowAnimationModule", showAnimationModule);
            EditorPrefs.SetBool("ModularBrain_ShowSFXModule", showSoundEffectModule);

            SaveFoldoutList(actionFoldouts, actionModulesProperty, "ModularBrain_ActionFoldouts");
            SaveFoldoutList(interactionFoldouts, interactionModulesProperty, "ModularBrain_InteractionFoldouts");
            SaveFoldoutList(resourceFoldouts, resourceModulesProperty, "ModularBrain_ResourceFoldouts");
            SaveFoldoutList(inventoryFoldouts, inventoryModulesProperty, "ModularBrain_InventoryFoldouts");
            SaveFoldoutList(customFoldouts, customModulesProperty, "ModularBrain_CustomFoldouts");
        }

        private void LoadFoldoutList(List<bool> foldoutList, SerializedProperty property, string key)
        {
            int arraySize = property.arraySize;
            foldoutList.Clear();

            for (int i = 0; i < arraySize; i++)
            {
                foldoutList.Add(EditorPrefs.GetBool($"{key}_{i}", false));
            }
        }

        private void SaveFoldoutList(List<bool> foldoutList, SerializedProperty property, string key)
        {
            int arraySize = property.arraySize;

            for (int i = 0; i < arraySize; i++)
            {
                EditorPrefs.SetBool($"{key}_{i}", foldoutList[i]);
            }
        }

        private void LoadIcons()
        {
            if (icons == null)
            {
                icons = new Dictionary<string, Texture2D>
            {
                { "HorizontalMovementModule", AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Script/Modular System/Editor/hmmoduleicon.png") },
                { "VerticalMovementModule", AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Script/Modular System/Editor/vmicon.png") },
                { "ActionModule", AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Script/Modular System/Editor/iconaction.png") },
                { "InteractionModule", AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Script/Modular System/Editor/interactionicon.png") },
                { "ResourceModule", AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Script/Modular System/Editor/resourceicon.png") },
                { "InventoryModule", AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Script/Modular System/Editor/inventorymodule.png") },
                { "AnimationModule", AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Script/Modular System/Editor/animationicon.png") },
                { "SoundEffectModule", AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Script/Modular System/Editor/sfxicon.png") },
                { "CustomModule", AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Script/Modular System/Editor/customicon.png") },
            };
            }
        }

        public override void OnInspectorGUI()
        {
            if (disableEditorFeaturesProperty.boolValue)
            {
                GUILayout.Label("Editor features disabled for performance.");
                return;
            }

            serializedObject.Update();

            DrawModuleSummary();

            DrawCategoryFoldout(ref showPhysicsModules, "Physics Modules", DrawPhysicsModules);
            DrawCategoryFoldout(ref showActionInteractionModules, "Action/Interaction Modules", DrawActionInteractionModules);
            DrawCategoryFoldout(ref showContainerModules, "Container Modules", DrawContainerModules);
            DrawCategoryFoldout(ref showVisualsAudioModules, "Visuals & Audio Modules", DrawVisualsAudioModules);
            DrawCategoryFoldout(ref showCustomModules, "Custom Modules", DrawCustomModules);

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawModuleSummary()
        {
            EditorGUILayout.BeginHorizontal();

            DrawModuleSummaryItem("HorizontalMovementModule", "", horizontalMovementModuleProperty.objectReferenceValue != null ? 1 : 0);
            DrawModuleSummaryItem("VerticalMovementModule", "", verticalMovementModuleProperty.objectReferenceValue != null ? 1 : 0);
            DrawModuleSummaryItem("ActionModule", "", actionModulesProperty.arraySize);
            DrawModuleSummaryItem("InteractionModule", "", interactionModulesProperty.arraySize);
            DrawModuleSummaryItem("ResourceModule", "", resourceModulesProperty.arraySize);
            DrawModuleSummaryItem("InventoryModule", "", inventoryModulesProperty.arraySize);
            DrawModuleSummaryItem("AnimationModule", "", animationModuleProperty.objectReferenceValue != null ? 1 : 0);
            DrawModuleSummaryItem("SoundEffectModule", "", soundEffectModuleProperty.objectReferenceValue != null ? 1 : 0);
            DrawModuleSummaryItem("CustomModule", "", customModulesProperty.arraySize);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }


        private void DrawModuleSummaryItem(string iconKey, string label, int count)
        {
            Texture2D icon = icons.ContainsKey(iconKey) ? icons[iconKey] : null;

            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();

            if (icon != null)
            {
                GUILayout.Label(new GUIContent(icon), GUILayout.Width(30), GUILayout.Height(30));
            }

            EditorGUILayout.BeginVertical();

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label($"    {count}", new GUIStyle(EditorStyles.boldLabel) { fontSize = 14, alignment = TextAnchor.MiddleLeft });
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();
        }

        private void DrawCategoryFoldout(ref bool foldout, string title, System.Action drawAction)
        {
            EditorGUILayout.BeginVertical();

            Rect rect = GUILayoutUtility.GetRect(20, EditorGUIUtility.singleLineHeight);
            foldout = EditorGUI.Foldout(rect, foldout, new GUIContent(title), true, EditorStyles.foldout);

            if (EditorGUILayout.BeginFadeGroup(foldout ? 1 : 0))
            {
                EditorGUI.indentLevel++;
                drawAction();
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFadeGroup();
            EditorGUILayout.EndVertical();
        }

        private void DrawPhysicsModules()
        {
            DrawSingleModule(horizontalMovementModuleProperty, ref showHorizontalMovementModule, "Horizontal Movement Module", "#CBE5FE");
            DrawSingleModule(verticalMovementModuleProperty, ref showVerticalMovementModule, "Vertical Movement Module", "#FFFE8A");
        }

        private void DrawActionInteractionModules()
        {
            DrawModuleList(actionModulesProperty, actionFoldouts, "Action Modules", "#FDCCCB");
            DrawModuleList(interactionModulesProperty, interactionFoldouts, "Interaction Modules", "#CDEB8B");
        }

        private void DrawContainerModules()
        {
            DrawModuleList(resourceModulesProperty, resourceFoldouts, "Resource Modules", "#FFCA99");
            DrawModuleList(inventoryModulesProperty, inventoryFoldouts, "Inventory Modules", "#DBB2FF");
        }

        private void DrawVisualsAudioModules()
        {
            DrawSingleModule(animationModuleProperty, ref showAnimationModule, "Animation Module", "#FFAFF4");
            DrawSingleModule(soundEffectModuleProperty, ref showSoundEffectModule, "Sound Effect Module", "#32DDA2");
        }

        private void DrawCustomModules()
        {
            DrawModuleList(customModulesProperty, customFoldouts, "Custom Modules", "#EEEEEE");
        }


        private void DrawSingleModule(SerializedProperty moduleProperty, ref bool foldout, string label, string colorHex)
        {
            DrawPropertyBackground(label, colorHex);

            EditorGUILayout.PropertyField(moduleProperty, new GUIContent(label), true);

            if (moduleProperty.objectReferenceValue != null)
            {
                Module module = moduleProperty.objectReferenceValue as Module;

                if (module != null)
                {
                    EditorGUILayout.Space();
                    DrawModuleBackground(module);

                    string moduleName = TypeName(module);
                    foldout = EditorGUILayout.Foldout(foldout, moduleName, true);
                    if (foldout)
                    {
                        Editor moduleEditor = CreateEditor(module);
                        moduleEditor.OnInspectorGUI();
                    }

                    EditorGUILayout.Space();
                }
            }
        }


        private void DrawModuleList(SerializedProperty modulesProperty, List<bool> foldouts, string label, string colorHex)
        {
            DrawPropertyBackground(label, colorHex);

            EditorGUILayout.PropertyField(modulesProperty, new GUIContent(label), true);


            if (modulesProperty.isExpanded)
            {
                int maxDisplayCount = 10;
                int totalModules = Mathf.Min(modulesProperty.arraySize, maxDisplayCount);

                if (modulesProperty.arraySize != foldouts.Count)
                {
                    InitializeFoldouts(foldouts, modulesProperty.arraySize);
                }

                for (int i = 0; i < totalModules; i++)
                {
                    SerializedProperty moduleProperty = modulesProperty.GetArrayElementAtIndex(i);
                    Module module = moduleProperty.objectReferenceValue as Module;

                    if (module != null)
                    {
                        EditorGUILayout.Space();
                        DrawModuleBackground(module);

                        string moduleName = TypeName(module);
                        foldouts[i] = EditorGUILayout.Foldout(foldouts[i], moduleName, true);
                        if (foldouts[i])
                        {
                            Editor moduleEditor = CreateEditor(module);
                            moduleEditor.OnInspectorGUI();
                        }

                        EditorGUILayout.Space();
                    }
                }

                if (modulesProperty.arraySize > maxDisplayCount)
                {
                    EditorGUILayout.HelpBox("More modules exist but are not displayed to maintain performance.", MessageType.Info);
                }
            }
        }

        private void DrawPropertyBackground(string label, string colorHex)
        {
            Rect rect = GUILayoutUtility.GetRect(20, EditorGUIUtility.singleLineHeight);
            rect.x -= 5;
            rect.y += 20;
            rect.width = 5;
            Color backgroundColor = HexToColor(colorHex);
            EditorGUI.DrawRect(rect, backgroundColor);
        }

        private string TypeName(Module module)
        {
            if (module is HorizontalMovementTypeModule)
            {
                return $"{module.name} : Type-HMovement";
            }
            else if (module is VerticalMovementTypeModule)
            {
                return $"{module.name} : Type-VMovement";
            }
            else if (module is ActionTypeModule)
            {
                return $"{module.name} : Type-Action";
            }
            else if (module is InteractionTypeModule)
            {
                return $"{module.name} : Type-Interaction";
            }
            else if (module is ResourceTypeModule)
            {
                return $"{module.name} : Type-Resource";
            }
            else if (module is InventoryTypeModule)
            {
                return $"{module.name} : Type-Inventory";
            }
            else if (module is AnimationTypeModule)
            {
                return $"{module.name} : Type-Animation";
            }
            else if (module is SoundEffectTypeModule)
            {
                return $"{module.name} : Type-SFX";
            }
            return $"{module.name} : Custom";
        }

        private void DrawModuleBackground(Module module)
        {
            _ = Color.white;
            Color backgroundColor;
            if (module is HorizontalMovementTypeModule)
            {
                backgroundColor = HexToColor("#CBE5FE");
            }
            else if (module is VerticalMovementTypeModule)
            {
                backgroundColor = HexToColor("#FFFE8A");
            }
            else if (module is ActionTypeModule)
            {
                backgroundColor = HexToColor("#FDCCCB");
            }
            else if (module is InteractionTypeModule)
            {
                backgroundColor = HexToColor("#CDEB8B");
            }
            else if (module is ResourceTypeModule)
            {
                backgroundColor = HexToColor("#FFCA99");
            }
            else if (module is InventoryTypeModule)
            {
                backgroundColor = HexToColor("#DBB2FF");
            }
            else if (module is AnimationTypeModule)
            {
                backgroundColor = HexToColor("#FFAFF4");
            }
            else if (module is SoundEffectTypeModule)
            {
                backgroundColor = HexToColor("#32DDA2");
            }
            else
            {
                backgroundColor = HexToColor("#EEEEEE");
            }

            EditorGUILayout.BeginHorizontal();

            Rect rect = GUILayoutUtility.GetRect(0, 0, GUILayout.ExpandWidth(true), GUILayout.Height(0));

            rect.x -= 0; rect.y += EditorGUIUtility.singleLineHeight;
            rect.width += 30;
            rect.height += EditorGUIUtility.singleLineHeight / 5;

            EditorGUI.DrawRect(rect, backgroundColor);

            Color buttonColor = module.IsActive ? Color.white : Color.grey;
            Color outlineColor = Color.black;

            Rect toggleRect = new Rect(rect.x + rect.width - 50, rect.y - 20, 15, EditorGUIUtility.singleLineHeight);
            if (GUI.Button(toggleRect, ""))
            {
                if (module.IsActive)
                {
                    module.SetModuleState(false);
                }
                else
                {
                    module.SetModuleState(true);
                }

                EditorUtility.SetDirty(module);
            }

            EditorGUI.DrawRect(toggleRect, buttonColor);

            DrawOutline(toggleRect, outlineColor, 1f);

            EditorGUILayout.EndHorizontal();
        }

        private void DrawOutline(Rect rect, Color color, float thickness)
        {
            // Top
            EditorGUI.DrawRect(new Rect(rect.x, rect.y, rect.width, thickness), color);
            // Bottom
            EditorGUI.DrawRect(new Rect(rect.x, rect.yMax - thickness, rect.width, thickness), color);
            // Left
            EditorGUI.DrawRect(new Rect(rect.x, rect.y, thickness, rect.height), color);
            // Right
            EditorGUI.DrawRect(new Rect(rect.xMax - thickness, rect.y, thickness, rect.height), color);
        }

        private void InitializeFoldouts()
        {
            InitializeFoldouts(actionFoldouts, actionModulesProperty.arraySize);
            InitializeFoldouts(interactionFoldouts, interactionModulesProperty.arraySize);
            InitializeFoldouts(resourceFoldouts, resourceModulesProperty.arraySize);
            InitializeFoldouts(inventoryFoldouts, inventoryModulesProperty.arraySize);
            InitializeFoldouts(customFoldouts, customModulesProperty.arraySize);
        }

        private void InitializeFoldouts(List<bool> foldouts, int size)
        {
            foldouts.Clear();
            for (int i = 0; i < size; i++)
            {
                foldouts.Add(false);
            }
        }

        private Color HexToColor(string hex)
        {
            if (ColorUtility.TryParseHtmlString(hex, out Color color))
            {
                return color;
            }
            return Color.white;
        }
    }
}