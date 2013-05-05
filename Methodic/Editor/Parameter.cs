//
// Author: Matthew Miner
//         matthew@matthewminer.com
//         http://matthewminer.com
//
// Copyright (c) 2013
//

using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Methodic
{
	/// <summary>
	/// Holds parameter information and displays a GUI to change its input
	/// value.
	/// </summary>
	class Parameter
	{
		internal object val { get; private set; }
		readonly ParameterInfo info;
		readonly GUIContent label;

		internal Parameter (ParameterInfo info)
		{
			this.info = info;
			this.label = new GUIContent(info.Name, info.ParameterType.Name);

			// Set the parameter to a default value.
			if (info.ParameterType.IsValueType) {
				this.val = System.Activator.CreateInstance(info.ParameterType);
			} else if (info.ParameterType == typeof(string)) {
				this.val = "";
			} else if (info.ParameterType == typeof(AnimationCurve)) {
				this.val = new AnimationCurve();
			}
		}

		/// <summary>
		/// Displays a field for modifying the parameter.
		/// </summary>
		internal void OnGUI ()
		{
			// Primitives
			if (info.ParameterType == typeof(int)) {
				val = EditorGUILayout.IntField(label, (int)val);
			} else if (info.ParameterType == typeof(float)) {
				val = EditorGUILayout.FloatField(label, (float)val);
			} else if (info.ParameterType == typeof(string)) {
				val = EditorGUILayout.TextField(label, (string)val);
			} else if (info.ParameterType == typeof(bool)) {
				val = EditorGUILayout.Toggle(label, (bool)val);
			}

			// Unity Objects / Structs
			else if (info.ParameterType == typeof(Color)) {
				val = EditorGUILayout.ColorField(label, (Color)val);
			} else if (info.ParameterType == typeof(AnimationCurve)) {
				val = EditorGUILayout.CurveField(label, (AnimationCurve)val);
			} else if (info.ParameterType == typeof(Rect)) {
				val = EditorGUILayout.RectField(label, (Rect)val);
			} else if (info.ParameterType == typeof(Vector2)) {
				val = EditorGUILayout.Vector2Field(info.Name, (Vector2)val);
			} else if (info.ParameterType == typeof(Vector3)) {
				val = EditorGUILayout.Vector3Field(info.Name, (Vector3)val);
			} else if (info.ParameterType == typeof(Vector4)) {
				val = EditorGUILayout.Vector4Field(info.Name, (Vector4)val);
			} else if (info.ParameterType.IsEnum) {
				var enumNames = System.Enum.GetNames(info.ParameterType);
				var names = enumNames.Select(n => new GUIContent(n)).ToArray();
				var enumValues = System.Enum.GetValues(info.ParameterType);
				var selected = EditorGUILayout.Popup(label, (int)val, names);
				val = enumValues.GetValue(selected);
			} else if (typeof(Object).IsAssignableFrom(info.ParameterType)) {
				// Transform, GameObject, etc.
				val = EditorGUILayout.ObjectField(label, (Object)val, info.ParameterType, true);
			}

			// Unknown / Unsupported
			else if (info.ParameterType.IsArray) {
				var message = "Array parameter type unsupported (sends null)";
				EditorGUILayout.LabelField(info.Name, message);
			} else {
				var message = label.tooltip + " parameter type unsupported (sends null)";
				EditorGUILayout.LabelField(info.Name, message);
			}
		}
	}
}
