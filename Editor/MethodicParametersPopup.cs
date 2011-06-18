// Copyright (c) 2011 Matthew Miner.

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
	
	/// <summary>
	/// Shows the parameters popup window.
	/// </summary>
	/// <param name="method">The method to invoke.</param>
	public static void ShowPopup (Methodic.Method method)
	{
		var window = CreateInstance<MethodicParametersPopup>();
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
				Debug.Log("Creating value for " + type + "; " + parameters[i]);
			} else if (type == typeof(string)) {
				parameters[i] = "";
			} else if (type == typeof(AnimationCurve)) {
				parameters[i] = new AnimationCurve();
			}
		}
	}
	
	void OnGUI ()
	{
		// Display an appropriate input field for each parameter
		for (int i = 0; i < parameters.Length; i++) {
			var info = paramInfo[i];
			
			// Primitives
			if (info.ParameterType == typeof(int)) {
				parameters[i] = EditorGUILayout.IntField(info.Name, (int)parameters[i]);
			} else if (info.ParameterType == typeof(float)) {
				parameters[i] = EditorGUILayout.FloatField(info.Name, (float)parameters[i]);
			} else if (info.ParameterType == typeof(string)) {
				parameters[i] = EditorGUILayout.TextField(info.Name, (string)parameters[i]);
			}
			// Unity objects / structs
			else if (info.ParameterType == typeof(Color)) {
				parameters[i] = EditorGUILayout.ColorField(info.Name, (Color)parameters[i]);
			} else if (info.ParameterType == typeof(AnimationCurve)) {
				parameters[i] = EditorGUILayout.CurveField(info.Name, (AnimationCurve)parameters[i]);
			} else if (info.ParameterType == typeof(Rect)) {
				parameters[i] = EditorGUILayout.RectField(info.Name, (Rect)parameters[i]);
			} else if (info.ParameterType == typeof(Vector2)) {
				parameters[i] = EditorGUILayout.Vector2Field(info.Name, (Vector2)parameters[i]);
			} else if (info.ParameterType == typeof(Vector3)) {
				parameters[i] = EditorGUILayout.Vector3Field(info.Name, (Vector3)parameters[i]);
			} else if (info.ParameterType == typeof(Vector4)) {
				parameters[i] = EditorGUILayout.Vector4Field(info.Name, (Vector4)parameters[i]);
			} else if (info.ParameterType.IsEnum) {				
				var enumNames = System.Enum.GetNames(info.ParameterType);
				var enumValues = System.Enum.GetValues(info.ParameterType);
				var selected = EditorGUILayout.Popup(info.Name, (int)parameters[i], enumNames);
				parameters[i] = enumValues.GetValue(selected);
			} else if (typeof(Object).IsAssignableFrom(info.ParameterType)) { // Transform, GameObject, etc.
				parameters[i] = EditorGUILayout.ObjectField(info.Name, (Object)parameters[i], info.ParameterType);
			}
			// Unknown / unsupported
			else if (info.ParameterType.IsArray) {
				EditorGUILayout.LabelField(info.Name, "Array parameter type unsupported (sends null)");
			} else {
				EditorGUILayout.LabelField(info.Name, "Unsupported parameter type (sends null)");
			}
		}
		
		GUILayout.FlexibleSpace();
		
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