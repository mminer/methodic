//
// Parameter.cs
//
// Author: Matthew Miner
//         matthew@matthewminer.com
//         http://www.matthewminer.com/
//
// Copyright (c) 2012
//

using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Methodic
{
	public static class Parameter
	{
		/// <summary>
		/// Displays a field for modifying the parameter.
		/// </summary>
		public static object DisplayGUI (object param, ParameterInfo info)
		{
			var label = new GUIContent(info.Name, info.ParameterType.Name);

			// Primitives
			if (info.ParameterType == typeof(int)) {
				param = EditorGUILayout.IntField(label, (int)param);
			} else if (info.ParameterType == typeof(float)) {
				param = EditorGUILayout.FloatField(label, (float)param);
			} else if (info.ParameterType == typeof(string)) {
				param = EditorGUILayout.TextField(label, (string)param);
			} else if (info.ParameterType == typeof(bool)) {
				param = EditorGUILayout.Toggle(label, (bool)param);
			}

			// Unity objects / structs
			else if (info.ParameterType == typeof(Color)) {
				param = EditorGUILayout.ColorField(label, (Color)param);
			} else if (info.ParameterType == typeof(AnimationCurve)) {
				param = EditorGUILayout.CurveField(label,
				                                   (AnimationCurve)param);
			} else if (info.ParameterType == typeof(Rect)) {
				param = EditorGUILayout.RectField(label, (Rect)param);
			} else if (info.ParameterType == typeof(Vector2)) {
				param = EditorGUILayout.Vector2Field(info.Name,
				                                     (Vector2)param);
			} else if (info.ParameterType == typeof(Vector3)) {
				param = EditorGUILayout.Vector3Field(info.Name,
				                                    (Vector3)param);
			} else if (info.ParameterType == typeof(Vector4)) {
				param = EditorGUILayout.Vector4Field(info.Name,
				                                     (Vector4)param);
			} else if (info.ParameterType.IsEnum) {
				// Place the enum names into GUIContent labels
				var enumNames = System.Enum.GetNames(info.ParameterType);
				var names = new GUIContent[enumNames.Length];

				for (int i = 0; i < names.Length; i++) {
					names[i] = new GUIContent(enumNames[i]);
				}

				var enumValues = System.Enum.GetValues(info.ParameterType);
				var selected = EditorGUILayout.Popup(label, (int)param, names);
				param = enumValues.GetValue(selected);
			} else if (typeof(Object).IsAssignableFrom(info.ParameterType)) {
				// Transform, GameObject, etc.
				param = EditorGUILayout.ObjectField(label, (Object)param,
				                                    info.ParameterType, true);
			}

			// Unknown / unsupported
			else if (info.ParameterType.IsArray) {
				var message = "Array parameter type unsupported (sends null)";
				EditorGUILayout.LabelField(info.Name, message);
			} else {
				var message = label.tooltip +
				              " parameter type unsupported (sends null)";
				EditorGUILayout.LabelField(info.Name, message);
			}

			return param;
		}
	}
}

