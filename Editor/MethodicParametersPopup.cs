//
// MethodicParametersPopup.cs
//
// Author: Matthew Miner (matthew@matthewminer.com)
// Copyright (c) 2011
//

using UnityEditor;
using UnityEngine;
using System.Reflection;

/// <summary>
/// Popup window for inputting parameters into a method for execution.
/// </summary>
public class MethodicParametersPopup : EditorWindow
{
	Methodic.Method method;
	ParameterInfo[] paramInfo;
	object[] parameters;
	Vector2 scrollPos;
	
	/// <summary>
	/// Shows the parameters popup window.
	/// </summary>
	/// <param name="method">The method to invoke.</param>
	public static void ShowPopup (Methodic.Method method)
	{
		var window = CreateInstance<MethodicParametersPopup>();
		window.title = "Parameters";
		window.Init(method);
		window.ShowUtility();
	}
	
	/// <summary>
	/// Sets up the parameters.
	/// </summary>
	/// <param name="method">The method to invoke.</param>
	void Init (Methodic.Method method)
	{
		this.method = method;
		paramInfo = method.method.GetParameters();
		parameters = new object[paramInfo.Length];
		
		// Set the parameters to default values
		for (int i = 0; i < parameters.Length; i++) {
			var type = paramInfo[i].ParameterType;
			
			if (type.IsValueType) {
				parameters[i] = System.Activator.CreateInstance(type);
			} else if (type == typeof(string)) {
				parameters[i] = "";
			} else if (type == typeof(AnimationCurve)) {
				parameters[i] = new AnimationCurve();
			}
		}
	}
	
	void OnGUI ()
	{
		scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.ExpandHeight(true));
		// Display an appropriate input field for each parameter
		for (int i = 0; i < parameters.Length; i++) {
			var info = paramInfo[i];
			var label = new GUIContent(info.Name, info.ParameterType.Name);
			
			// Primitives
			if (info.ParameterType == typeof(int)) {
				parameters[i] = EditorGUILayout.IntField(label, (int)parameters[i]);
			} else if (info.ParameterType == typeof(float)) {
				parameters[i] = EditorGUILayout.FloatField(label, (float)parameters[i]);
			} else if (info.ParameterType == typeof(string)) {
				parameters[i] = EditorGUILayout.TextField(label, (string)parameters[i]);
			}
			// Unity objects / structs
			else if (info.ParameterType == typeof(Color)) {
				parameters[i] = EditorGUILayout.ColorField(label, (Color)parameters[i]);
			} else if (info.ParameterType == typeof(AnimationCurve)) {
				parameters[i] = EditorGUILayout.CurveField(label, (AnimationCurve)parameters[i]);
			} else if (info.ParameterType == typeof(Rect)) {
				parameters[i] = EditorGUILayout.RectField(label, (Rect)parameters[i]);
			} else if (info.ParameterType == typeof(Vector2)) {
				parameters[i] = EditorGUILayout.Vector2Field(info.Name, (Vector2)parameters[i]);
			} else if (info.ParameterType == typeof(Vector3)) {
				parameters[i] = EditorGUILayout.Vector3Field(info.Name, (Vector3)parameters[i]);
			} else if (info.ParameterType == typeof(Vector4)) {
				parameters[i] = EditorGUILayout.Vector4Field(info.Name, (Vector4)parameters[i]);
			} else if (info.ParameterType.IsEnum) {
				// Place the enum names into GUIContent labels
				var enumNames = System.Enum.GetNames(info.ParameterType);
				var names = new GUIContent[enumNames.Length];
				
				for (int j = 0; j < names.Length; j++) {
					names[j] = new GUIContent(enumNames[j]);
				}
				
				var enumValues = System.Enum.GetValues(info.ParameterType);
				var selected = EditorGUILayout.Popup(label, (int)parameters[i], names);
				parameters[i] = enumValues.GetValue(selected);
			} else if (typeof(Object).IsAssignableFrom(info.ParameterType)) { // Transform, GameObject, etc.
				parameters[i] = EditorGUILayout.ObjectField(label, (Object)parameters[i], info.ParameterType);
			}
			// Unknown / unsupported
			else if (info.ParameterType.IsArray) {
				EditorGUILayout.LabelField(info.Name, "Array parameter type unsupported (sends null)");
			} else {
				EditorGUILayout.LabelField(info.Name, label.tooltip + " parameter type unsupported (sends null)");
			}
		}
		EditorGUILayout.EndScrollView();
		
		EditorGUILayout.BeginHorizontal();
			
			GUILayout.FlexibleSpace();
		
			if (GUILayout.Button("Cancel")) {
				Close();
			}
		
			if (GUILayout.Button("Invoke")) {
				Methodic.InvokeMethod(method, parameters);
				Close();
			}
		
		EditorGUILayout.EndHorizontal();
	}
}