//
// Author: Matthew Miner
//         matthew@matthewminer.com
//         https://matthewminer.com
//
// Copyright (c) 2019
//

using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Methodic
{
    class Window : EditorWindow
    {
        static readonly GUIContent invokeLabel = new GUIContent(
            "Invoke",
            "Execute this method."
        );

        static readonly GUIContent lockLabel = new GUIContent(
            "Lock",
            "Lock Methodic to the currently selected game object."
        );

        static GUIContent titleLabel
        {
            get {
                var icon = AssetDatabase.LoadAssetAtPath<Texture2D>(
                    "Assets/Methodic/Editor/Icon.png"
                );

                var label = new GUIContent("Methodic", icon);
                return label;
            }
        }

        int componentIndex;
        int methodIndex;

        GameObject _selectedGameObject;
        GameObject selectedGameObject
        {
            get {
                if (!lockedToGameObject || _selectedGameObject == null) {
                    _selectedGameObject = Selection.activeGameObject;
                }

                return _selectedGameObject;
            }
        }

        MonoBehaviour selectedComponent
        {
            get {
                if (TargetInfo.components.Length == 0) {
                    return null;
                }

                return TargetInfo.components[componentIndex];
            }
        }

        MethodInfo selectedMethod
        {
            get {
                if (TargetInfo.methods.Length == 0) {
                    return null;
                }

                return TargetInfo.methods[methodIndex];
            }
        }

        bool lockedToGameObject;
        Vector2 scrollPosition;

        [MenuItem("Window/Methodic %#m")]
        static void OpenMethodic()
        {
            EditorWindow.GetWindow<Window>("Methodic");
        }

        void OnEnable()
        {
            titleContent = titleLabel;
            Preferences.OnPreferencesChange += Refresh;
            Refresh();
        }

        void OnDisable()
        {
            Preferences.OnPreferencesChange -= Refresh;
        }

        void OnGUI()
        {
            DrawToolbar();
            DrawParametersForm();
            DrawInvokeButton();
        }

        void OnSelectionChange()
        {
            if (lockedToGameObject) {
                return;
            }

            Refresh();
        }

        void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

                GUILayout.FlexibleSpace();

                lockedToGameObject = GUILayout.Toggle(
                    lockedToGameObject,
                    lockLabel,
                    EditorStyles.toolbarButton
                );

                OnGUIChanged(() => {
                    if (lockedToGameObject) {
                        return;
                    }

                    RefreshComponents();
                    RefreshMethods();
                    RefreshParameters();
                });

            EditorGUILayout.EndHorizontal();
        }

        void DrawParametersForm()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            if (selectedGameObject == null) {
                CustomGUILayout.NoSelectionBox();
            } else if (TargetInfo.components.Length == 0) {
                CustomGUILayout.NoMethodsBox();
            } else {
                componentIndex = EditorGUILayout.Popup(
                    "Component",
                    componentIndex,
                    TargetInfo.componentLabels
                );

                // Update selected component if a different one has been chosen.
                OnGUIChanged(() => {
                    RefreshMethods();
                    RefreshParameters();
                });

                methodIndex = EditorGUILayout.Popup(
                    "Method",
                    methodIndex,
                    TargetInfo.methodLabels
                );

                // Update selected method if a different one has been chosen.
                OnGUIChanged(() => {
                    RefreshParameters();
                });

                // Display list of parameters.
                if (TargetInfo.parameters.Length > 0) {
                    GUILayout.Label("Parameters");
                    EditorGUI.indentLevel = 1;

                    for (int i = 0; i < TargetInfo.parameters.Length; i++) {
                        TargetInfo.parameterValues[i] = CustomGUILayout.ParameterField(
                            TargetInfo.parameters[i],
                            TargetInfo.parameterValues[i]
                        );
                    }
                }
            }

            EditorGUILayout.EndScrollView();
        }

        void DrawInvokeButton()
        {
            GUI.enabled = selectedGameObject != null;

            if (GUILayout.Button(invokeLabel)) {
                var undoLabel = string.Format("{0} Call", selectedComponent.name);
                Undo.RecordObject(selectedGameObject, undoLabel);

                Reflector.InvokeMethod(
                    selectedComponent,
                    selectedMethod,
                    TargetInfo.parameterValues
                );
            }
        }

        void OnGUIChanged(Action action)
        {
            if (!GUI.changed) {
                return;
            }

            action();
            GUI.changed = true;
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
            TargetInfo.SetSelectedGameObject(selectedGameObject);
            componentIndex = 0;
        }

        void RefreshMethods()
        {
            TargetInfo.SetSelectedComponent(selectedComponent);
            methodIndex = 0;
        }

        void RefreshParameters()
        {
            TargetInfo.SetSelectedMethod(selectedMethod);
        }
    }
}
