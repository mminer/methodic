//
// Author: Matthew Miner
//         matthew@matthewminer.com
//         http://matthewminer.com
//
// Copyright (c) 2014
//

using System;
using System.Linq;
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
		/// <summary>
		/// Whether the editor window is currently open.
		/// </summary>
		internal static bool isOpen { get; private set; }

		const string noSelectionMessage = "Select a game object in the Hierarchy to list its methods.";
		const string noMethodsMessage = "Components attached to the selected game object contain no methods of the desired type.";
		static readonly GUIContent lockLabel = new GUIContent("Lock", "Lock Methodic to the currently selected game object.");
		static readonly GUIContent invokeLabel = new GUIContent("Invoke", "Execute this method.");

		MonoBehaviour[] components;
		MethodInfo[] methods;
		ParameterInfo[] parameters;
		object[] parameterValues;

		int componentIndex;
		int methodIndex;

		MonoBehaviour selectedComponent;
		MethodInfo selectedMethod;

		string[] componentLabels;
		string[] methodLabels;

		GameObject _activeGameObject;
		GameObject activeGameObject
		{
			get {
				if (!lockedToGameObject || _activeGameObject == null) {
					_activeGameObject = Selection.activeGameObject;
				}

				return _activeGameObject;
			}
		}

		bool lockedToGameObject;
		Vector2 scrollPosition;

		/// <summary>
		/// Resets the components and methods.
		/// </summary>
		internal void Refresh ()
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
			components = Reflector.GetComponents(activeGameObject);
			componentIndex = 0;

			if (components == null || components.Length == 0) {
				selectedComponent = null;
			} else {
				selectedComponent = components[componentIndex];
			}

			componentLabels = Reflector.GetComponentLabels(components);
		}

		/// <summary>
		/// Resets the methods to the selected component's.
		/// </summary>
		void RefreshMethods ()
		{
			methods = Reflector.GetMethods(selectedComponent);
			methodIndex = 0;

			if (methods == null || methods.Length == 0) {
				selectedMethod = null;
			} else {
				selectedMethod = methods[methodIndex];
			}

			methodLabels = Reflector.GetMethodLabels(methods);
		}

		/// <summary>
		/// Resets the parameters to the selected methods's.
		/// </summary>
		void RefreshParameters ()
		{
			parameters = Reflector.GetParameters(selectedMethod);
			parameterValues = Reflector.GetDefaultParameterValues(parameters);
		}

		void OnGUI ()
		{
			EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

				GUILayout.FlexibleSpace();
				lockedToGameObject = GUILayout.Toggle(lockedToGameObject, lockLabel, EditorStyles.toolbarButton);

				OnGUIChanged(delegate {
					if (!lockedToGameObject) {
						RefreshComponents();
						RefreshMethods();
						RefreshParameters();
					}
				});

			EditorGUILayout.EndHorizontal();

			scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

			if (components == null) {
				EditorGUILayout.HelpBox(noSelectionMessage, MessageType.Info);
			} else if (components.Length == 0) {
				EditorGUILayout.HelpBox(noMethodsMessage, MessageType.Warning);
			} else {
				componentIndex = EditorGUILayout.Popup("Component", componentIndex, componentLabels);

				// Update selected component if a different one has been chosen.
				OnGUIChanged(delegate {
					selectedComponent = components[componentIndex];
					RefreshMethods();
					RefreshParameters();
				});

				methodIndex = EditorGUILayout.Popup("Method", methodIndex, methodLabels);

				// Update selected method if a different one has been chosen.
				OnGUIChanged(delegate {
					selectedMethod = methods[methodIndex];
					RefreshParameters();
				});

				// Display list of parameters.
				if (parameters.Length > 0) {
					GUILayout.Label("Parameters");
					EditorGUI.indentLevel = 1;

					for (int i = 0; i < parameters.Length; i++) {
						parameterValues[i] = ParameterGUIField(parameters[i], parameterValues[i]);
					}
				}
			}

			EditorGUILayout.EndScrollView();

			GUI.enabled = components != null;

			if (GUILayout.Button(invokeLabel)) {
				var undoLabel = string.Format("{0} Call", selectedComponent.name);
				Undo.RecordObject(activeGameObject, undoLabel);
				Reflector.InvokeMethod(selectedComponent, selectedMethod, parameterValues);
			}
		}

		void OnSelectionChange ()
		{
			if (!lockedToGameObject) {
				Refresh();
			}
		}

		void OnEnable ()
		{
			isOpen = true;
			Refresh();
		}

		void OnDisable ()
		{
			isOpen = false;
		}

		/// <summary>
		/// Editor GUI field for a parameter.
		/// Specific EditorGUILayout function used depends on parameter type.
		/// </summary>
		/// <param name="parameter">Parameter info.</param>
		/// <param name="parameterValue">Current value of parameter field.</param>
		/// <returns>New value for parameter.</returns>
		static object ParameterGUIField (ParameterInfo parameter, object parameterValue)
		{
			var parameterType = parameter.ParameterType;
			var label = new GUIContent(parameter.Name, parameterType.Name);
			object newValue;

			// Primitives
			if (parameterType == typeof(int)) {
				newValue = EditorGUILayout.IntField(label, (int)parameterValue);
			} else if (parameterType == typeof(float)) {
				newValue = EditorGUILayout.FloatField(label, (float)parameterValue);
			} else if (parameterType == typeof(string)) {
				newValue = EditorGUILayout.TextField(label, (string)parameterValue);
			} else if (parameterType == typeof(bool)) {
				newValue = EditorGUILayout.Toggle(label, (bool)parameterValue);
			}

			// Unity Objects / Structs
			else if (parameterType == typeof(Color)) {
				newValue = EditorGUILayout.ColorField(label, (Color)parameterValue);
			} else if (parameterType == typeof(AnimationCurve)) {
				newValue = EditorGUILayout.CurveField(label, (AnimationCurve)parameterValue);
			} else if (parameterType == typeof(Rect)) {
				newValue = EditorGUILayout.RectField(label, (Rect)parameterValue);
			} else if (parameterType == typeof(Vector2)) {
				newValue = EditorGUILayout.Vector2Field(parameter.Name, (Vector2)parameterValue);
			} else if (parameterType == typeof(Vector3)) {
				newValue = EditorGUILayout.Vector3Field(parameter.Name, (Vector3)parameterValue);
			} else if (parameterType == typeof(Vector4)) {
				newValue = EditorGUILayout.Vector4Field(parameter.Name, (Vector4)parameterValue);
			} else if (parameterType.IsEnum) {
				var enumNames = Enum.GetNames(parameterType);
				var names = enumNames
					.Select(name => new GUIContent(name))
					.ToArray();

				var enumValues = Enum.GetValues(parameterType);
				var selected = EditorGUILayout.Popup(label, (int)parameterValue, names);
				newValue = enumValues.GetValue(selected);
			} else if (typeof(UnityEngine.Object).IsAssignableFrom(parameterType)) {
				// Transform, GameObject, etc.
				newValue = EditorGUILayout.ObjectField(label, (UnityEngine.Object)parameterValue, parameterType, true);
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

		static void OnGUIChanged (Action action)
		{
			if (GUI.changed) {
				action();
				GUI.changed = true;
			}
		}
	}
}
