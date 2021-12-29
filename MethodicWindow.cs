//
// Author: Matthew Miner
//         matthew@matthewminer.com
//         https://matthewminer.com
//
// Copyright (c) 2019
//

using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Methodic
{
    class MethodicWindow : EditorWindow
    {
        static readonly MethodicTarget target = new MethodicTarget();

        static readonly GUIContent invokeLabel = new GUIContent(
            "Invoke",
            "Execute this method.");

        static readonly GUIContent lockLabel = new GUIContent(
            "Lock",
            "Lock Methodic to the currently selected game object.");

        int componentIndex;
        int methodIndex;

        GameObject selectedGameObject
        {
            get
            {
                if (!lockedToGameObject || _selectedGameObject == null)
                {
                    _selectedGameObject = Selection.activeGameObject;
                }

                return _selectedGameObject;
            }
        }

        GameObject _selectedGameObject;

        MonoBehaviour selectedComponent => target.components.Length > 0 ? target.components[componentIndex] : null;
        MethodInfo selectedMethod => target.methods.Length > 0 ? target.methods[methodIndex] : null;

        bool lockedToGameObject;
        Vector2 scrollPosition;

        [MenuItem("Window/Methodic %#m")]
        static void OpenMethodic()
        {
            GetWindow<MethodicWindow>("Methodic");
        }

        void OnEnable()
        {
            var icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Methodic/Editor/Icon.png");
            titleContent = new GUIContent("Methodic", icon);
            MethodicPreferences.OnPreferencesChange += Refresh;
            Refresh();
        }

        void OnDisable()
        {
            MethodicPreferences.OnPreferencesChange -= Refresh;
        }

        void OnGUI()
        {
            // Toolbar:

            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                GUILayout.FlexibleSpace();

                lockedToGameObject = GUILayout.Toggle(
                    lockedToGameObject,
                    lockLabel,
                    EditorStyles.toolbarButton
                );

                RunIfGUIChanged(() =>
                {
                    if (lockedToGameObject)
                    {
                        return;
                    }

                    RefreshComponents();
                    RefreshMethods();
                    RefreshParameters();
                });
            }

            // Parameters:

            using (var scrollView =  new EditorGUILayout.ScrollViewScope(scrollPosition))
            {
                scrollPosition = scrollView.scrollPosition;

                if (selectedGameObject == null)
                {
                    EditorGUILayout.HelpBox(
                        "Select a game object in the Hierarchy to list its methods.",
                        MessageType.Info);
                }
                else if (target.components.Length == 0)
                {
                    EditorGUILayout.HelpBox(
                        "Components attached to the selected game object contain no methods of the desired type.",
                        MessageType.Warning);
                }
                else
                {
                    componentIndex = EditorGUILayout.Popup( "Component", componentIndex, target.componentLabels);

                    // Update selected component if a different one has been chosen.
                    RunIfGUIChanged(() =>
                    {
                        RefreshMethods();
                        RefreshParameters();
                    });

                    methodIndex = EditorGUILayout.Popup("Method", methodIndex, target.methodLabels);

                    // Update selected method if a different one has been chosen.
                    RunIfGUIChanged(RefreshParameters);

                    // Display list of parameters.
                    if (target.parameters.Length > 0)
                    {
                        GUILayout.Label("Parameters");
                        EditorGUI.indentLevel = 1;

                        for (var i = 0; i < target.parameters.Length; i++)
                        {
                            target.parameterValues[i] = ParameterField(target.parameters[i], target.parameterValues[i]);
                        }
                    }
                }
            }

            // Invoke Button:

            using (new EditorGUI.DisabledScope(selectedGameObject == null))
            {
                if (GUILayout.Button(invokeLabel))
                {
                    Undo.RecordObject(selectedGameObject, $"{selectedComponent.name} Call");
                    InvokeMethod(selectedComponent, selectedMethod, target.parameterValues);
                }
            }
        }

        void Refresh()
        {
            RefreshComponents();
            RefreshMethods();
            RefreshParameters();
            Repaint();
        }

        void RefreshComponents()
        {
            target.SetSelectedGameObject(selectedGameObject);
            componentIndex = 0;
        }

        void RefreshMethods()
        {
            target.SetSelectedComponent(selectedComponent);
            methodIndex = 0;
        }

        void RefreshParameters()
        {
            target.SetSelectedMethod(selectedMethod);
        }

        static void InvokeMethod(MonoBehaviour component, MethodInfo method, object[] parameterValues)
        {
            try
            {
                var result = method.Invoke(component, parameterValues);

                if (method.ReturnType != typeof(void))
                {
                    Debug.Log($"[Methodic] Result: {result}");
                }
            }
            catch (ArgumentException e)
            {
                Debug.LogError($"[Methodic] Unable to invoke method: {e.Message}");
            }
        }

        static object ParameterField(ParameterInfo parameter, object currentValue)
        {
            var parameterType = parameter.ParameterType;
            var label = new GUIContent(parameter.Name, parameterType.Name);
            object newValue;

            // Primitives
            if (parameterType == typeof(int))
            {
                newValue = EditorGUILayout.IntField(label, (int)currentValue);
            }
            else if (parameterType == typeof(float))
            {
                newValue = EditorGUILayout.FloatField(label, (float)currentValue);
            }
            else if (parameterType == typeof(string))
            {
                newValue = EditorGUILayout.TextField(label, (string)currentValue);
            }
            else if (parameterType == typeof(bool))
            {
                newValue = EditorGUILayout.Toggle(label, (bool)currentValue);
            }
            // Unity Objects / Structs
            else if (parameterType == typeof(AnimationCurve))
            {
                newValue = EditorGUILayout.CurveField(label, (AnimationCurve)currentValue);
            }
            else if (parameterType == typeof(Color))
            {
                newValue = EditorGUILayout.ColorField(label, (Color)currentValue);
            }
            else if (parameterType == typeof(Rect))
            {
                newValue = EditorGUILayout.RectField(label, (Rect)currentValue);
            }
            else if (parameterType == typeof(Vector2))
            {
                newValue = EditorGUILayout.Vector2Field(parameter.Name, (Vector2)currentValue);
            }
            else if (parameterType == typeof(Vector3))
            {
                newValue = EditorGUILayout.Vector3Field(parameter.Name, (Vector3)currentValue);
            }
            else if (parameterType == typeof(Vector4))
            {
                newValue = EditorGUILayout.Vector4Field(parameter.Name, (Vector4)currentValue);
            }
            // Transform, GameObject, etc.
            else if (typeof(UnityEngine.Object).IsAssignableFrom(parameterType))
            {
                newValue = EditorGUILayout.ObjectField(label, (UnityEngine.Object)currentValue, parameterType, true);
            }
            // Other
            else if (parameterType.IsEnum)
            {
                var enumNames = Enum.GetNames(parameterType);
                var names = enumNames.Select(name => new GUIContent(name)).ToArray();
                var enumValues = Enum.GetValues(parameterType);
                var selected = EditorGUILayout.Popup(label, (int)currentValue, names);
                newValue = enumValues.GetValue(selected);
            }
            // Unknown / Unsupported
            else if (parameterType.IsArray)
            {
                EditorGUILayout.LabelField(parameter.Name, "Array parameter type unsupported (sends null)");
                newValue = null;
            }
            else
            {
                EditorGUILayout.LabelField(parameter.Name, $"{label.tooltip} parameter type unsupported (sends null)");
                newValue = null;
            }

            return newValue;
        }

        static void RunIfGUIChanged(Action action)
        {
            if (!GUI.changed)
            {
                return;
            }

            action();
            GUI.changed = true;
        }
    }
}
