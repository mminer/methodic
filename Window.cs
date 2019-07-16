//
// Author: Matthew Miner
//         matthew@matthewminer.com
//         https://matthewminer.com
//
// Copyright (c) 2016
//

using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Methodic
{
    /// <summary>
    /// The Methodic editor window.
    /// </summary>
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

        void OnEnable ()
        {
            #if UNITY_5_3_OR_NEWER
                titleContent = titleLabel;
            #endif

            Preferences.OnPreferencesChange += Refresh;
            Refresh();
        }

        void OnDisable ()
        {
            Preferences.OnPreferencesChange -= Refresh;
        }

        void OnGUI ()
        {
            DrawToolbar();
            DrawParametersForm();
            DrawInvokeButton();
        }

        void OnSelectionChange ()
        {
            if (lockedToGameObject) {
                return;
            }

            Refresh();
        }

        void DrawToolbar ()
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

        void DrawParametersForm ()
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

        void DrawInvokeButton ()
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

        /// <summary>
        /// Runs the specified action if the GUI changed.
        /// </summary>
        /// <param name="action">Action to run.</param>
        void OnGUIChanged (Action action)
        {
            if (!GUI.changed) {
                return;
            }

            action();
            GUI.changed = true;
        }

        /// <summary>
        /// Resets the components and methods.
        /// </summary>
        void Refresh ()
        {
            RefreshComponents();
            RefreshMethods();
            RefreshParameters();
            Repaint();
        }

        /// <summary>
        /// Resets the components to the selected game object's.
        /// </summary>
        void RefreshComponents ()
        {
            TargetInfo.SetSelectedGameObject(selectedGameObject);
            componentIndex = 0;
        }

        /// <summary>
        /// Resets the methods to the selected component's.
        /// </summary>
        void RefreshMethods ()
        {
            TargetInfo.SetSelectedComponent(selectedComponent);
            methodIndex = 0;
        }

        /// <summary>
        /// Resets the parameters to the selected methods's.
        /// </summary>
        void RefreshParameters ()
        {
            TargetInfo.SetSelectedMethod(selectedMethod);
        }
    }
}
