//
// ParametersPanel.cs
//
// Author: Matthew Miner
//         matthew@matthewminer.com
//         http://www.matthewminer.com/
//
// Copyright (c) 2012
//

using UnityEditor;
using UnityEngine;
using System.Reflection;

namespace Methodic
{
	/// <summary>
	/// Panel for inputting parameters into a method for execution.
	/// </summary>
	public class ParametersPanel
	{
		public object[] parameters { get; private set; }
		ParameterInfo[] info;
		Vector2 scrollPos;

		/// <summary>
		/// Sets up the parameters.
		/// </summary>
		/// <param name="method">The method to invoke.</param>
		public ParametersPanel (MethodInfo method)
		{
			info = method.GetParameters();
			parameters = new object[info.Length];

			// Set the parameters to default values
			for (int i = 0; i < parameters.Length; i++) {
				var type = info[i].ParameterType;

				if (type.IsValueType) {
					parameters[i] = System.Activator.CreateInstance(type);
				} else if (type == typeof(string)) {
					parameters[i] = "";
				} else if (type == typeof(AnimationCurve)) {
					parameters[i] = new AnimationCurve();
				}
			}
		}

		public void OnGUI ()
		{
			scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

			// Display an appropriate input field for each parameter
			for (int i = 0; i < parameters.Length; i++) {
				var paramInfo = info[i];
				var label = new GUIContent(paramInfo.Name, paramInfo.ParameterType.Name);

				// Primitives
				if (paramInfo.ParameterType == typeof(int)) {
					var val = (int)parameters[i];
					parameters[i] = EditorGUILayout.IntField(label, val);
				} else if (paramInfo.ParameterType == typeof(float)) {
					var val = (float)parameters[i];
					parameters[i] = EditorGUILayout.FloatField(label, val);
				} else if (paramInfo.ParameterType == typeof(string)) {
					var val = (string)parameters[i];
					parameters[i] = EditorGUILayout.TextField(label, val);
				} else if (paramInfo.ParameterType == typeof(bool)) {
					var val = (bool)parameters[i];
					parameters[i] = EditorGUILayout.Toggle(label, val);
				}
				// Unity objects / structs
				else if (paramInfo.ParameterType == typeof(Color)) {
					var val = (Color)parameters[i];
					parameters[i] = EditorGUILayout.ColorField(label, val);
				} else if (paramInfo.ParameterType == typeof(AnimationCurve)) {
					var val = (AnimationCurve)parameters[i];
					parameters[i] = EditorGUILayout.CurveField(label, val);
				} else if (paramInfo.ParameterType == typeof(Rect)) {
					var val = (Rect)parameters[i];
					parameters[i] = EditorGUILayout.RectField(label, val);
				} else if (paramInfo.ParameterType == typeof(Vector2)) {
					var val = (Vector2)parameters[i];
					parameters[i] = EditorGUILayout.Vector2Field(paramInfo.Name, val);
				} else if (paramInfo.ParameterType == typeof(Vector3)) {
					var val = (Vector3)parameters[i];
					parameters[i] = EditorGUILayout.Vector3Field(paramInfo.Name, val);
				} else if (paramInfo.ParameterType == typeof(Vector4)) {
					var val = (Vector4)parameters[i];
					parameters[i] = EditorGUILayout.Vector4Field(paramInfo.Name, val);
				} else if (paramInfo.ParameterType.IsEnum) {
					// Place the enum names into GUIContent labels
					var enumNames = System.Enum.GetNames(paramInfo.ParameterType);
					var names = new GUIContent[enumNames.Length];

					for (int j = 0; j < names.Length; j++) {
						names[j] = new GUIContent(enumNames[j]);
					}

					var enumValues = System.Enum.GetValues(paramInfo.ParameterType);
					var selected = EditorGUILayout.Popup(label, (int)parameters[i], names);
					parameters[i] = enumValues.GetValue(selected);
				} else if (typeof(Object).IsAssignableFrom(paramInfo.ParameterType)) { // Transform, GameObject, etc.
					parameters[i] = EditorGUILayout.ObjectField(label, (Object)parameters[i], paramInfo.ParameterType, true);
				}
				// Unknown / unsupported
				else if (paramInfo.ParameterType.IsArray) {
					EditorGUILayout.LabelField(paramInfo.Name, "Array parameter type unsupported (sends null)");
				} else {
					EditorGUILayout.LabelField(paramInfo.Name, label.tooltip + " parameter type unsupported (sends null)");
				}
			}

			EditorGUILayout.EndScrollView();
		}
	}
}

