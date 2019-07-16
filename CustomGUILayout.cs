//
// Author: Matthew Miner
//         matthew@matthewminer.com
//         https://matthewminer.com
//
// Copyright (c) 2016
//

using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Methodic
{
    /// <summary>
    /// Custom GUI components.
    /// </summary>
    static class CustomGUILayout
    {
        /// <summary>
        /// A help box indicating that no methods are available for selection.
        /// </summary>
        internal static void NoMethodsBox ()
        {
            EditorGUILayout.HelpBox(
                "Components attached to the selected game object contain no methods of the desired type.",
                MessageType.Warning
            );
        }

        /// <summary>
        /// A help box indicating that no game object is selected.
        /// </summary>
        internal static void NoSelectionBox ()
        {
            EditorGUILayout.HelpBox(
                "Select a game object in the Hierarchy to list its methods.",
                MessageType.Info
            );
        }

        /// <summary>
        /// Editor GUI field for a parameter.
        /// Specific EditorGUILayout function used depends on parameter type.
        /// </summary>
        /// <param name="parameter">Parameter info.</param>
        /// <param name="currentValue">Current value.</param>
        /// <returns>New value for parameter.</returns>
        internal static object ParameterField (ParameterInfo parameter, object currentValue)
        {
            var parameterType = parameter.ParameterType;
            var label = new GUIContent(parameter.Name, parameterType.Name);
            object newValue;

            // Primitives
            if (parameterType == typeof(int)) {
                newValue = EditorGUILayout.IntField(label, (int)currentValue);
            } else if (parameterType == typeof(float)) {
                newValue = EditorGUILayout.FloatField(label, (float)currentValue);
            } else if (parameterType == typeof(string)) {
                newValue = EditorGUILayout.TextField(label, (string)currentValue);
            } else if (parameterType == typeof(bool)) {
                newValue = EditorGUILayout.Toggle(label, (bool)currentValue);
            }

            // Unity Objects / Structs
            else if (parameterType == typeof(Color)) {
                newValue = EditorGUILayout.ColorField(label, (Color)currentValue);
            } else if (parameterType == typeof(AnimationCurve)) {
                newValue = EditorGUILayout.CurveField(label, (AnimationCurve)currentValue);
            } else if (parameterType == typeof(Rect)) {
                newValue = EditorGUILayout.RectField(label, (Rect)currentValue);
            } else if (parameterType == typeof(Vector2)) {
                newValue = EditorGUILayout.Vector2Field(parameter.Name, (Vector2)currentValue);
            } else if (parameterType == typeof(Vector3)) {
                newValue = EditorGUILayout.Vector3Field(parameter.Name, (Vector3)currentValue);
            } else if (parameterType == typeof(Vector4)) {
                newValue = EditorGUILayout.Vector4Field(parameter.Name, (Vector4)currentValue);
            } else if (parameterType.IsEnum) {
                var enumNames = Enum.GetNames(parameterType);
                var names = enumNames
                    .Select(name => new GUIContent(name))
                    .ToArray();

                var enumValues = Enum.GetValues(parameterType);
                var selected = EditorGUILayout.Popup(label, (int)currentValue, names);
                newValue = enumValues.GetValue(selected);
            } else if (typeof(UnityEngine.Object).IsAssignableFrom(parameterType)) {
                // Transform, GameObject, etc.
                newValue = EditorGUILayout.ObjectField(label, (UnityEngine.Object)currentValue, parameterType, true);
            }

            // Unknown / Unsupported
            else if (parameterType.IsArray) {
                var message = "Array parameter type unsupported (sends null)";
                EditorGUILayout.LabelField(parameter.Name, message);
                newValue = null;
            } else {
                var message = label.tooltip + " parameter type unsupported (sends null)";
                EditorGUILayout.LabelField(parameter.Name, message);
                newValue = null;
            }

            return newValue;
        }
    }
}
