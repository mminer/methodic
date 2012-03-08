//
// MethodicWindow.cs
//
// Author: Matthew Miner
//         matthew@matthewminer.com
//         http://www.matthewminer.com/
//
// Copyright (c) 2012
//

using Methodic;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Displays a dropdown menu of functions available for the selected game object to access.
/// </summary>
public class MethodicWindow : EditorWindow
{
	enum Panel { Main, Preferences }

	static readonly GUIContent optionsLabel = new GUIContent("Preferences", "Customize which methods are shown.");
	static readonly GUIContent popupLabel = new GUIContent("Method");
	static readonly GUIContent invokeLabel = new GUIContent("Invoke", "Execute this method.");

	Panel selectedPanel;
	Method[] methods = {};
	GUIContent[] methodLabels = {};
	int methodIndex;

	Method selectedMethod {
		get {
			if (methodIndex >= 0 && methodIndex < methods.Length) {
				return methods[methodIndex];
			} else {
				return null;
			}
		}
	}

	void OnGUI ()
	{
		// Toolbar
		EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

			GUILayout.FlexibleSpace();

			var optionsToggle = GUILayout.Toggle(selectedPanel == Panel.Preferences, optionsLabel, EditorStyles.toolbarButton);
			selectedPanel = optionsToggle ? Panel.Preferences : Panel.Main;

		EditorGUILayout.EndHorizontal();

		// Panel
		switch (selectedPanel) {
			case Panel.Main:
				GUI.enabled = selectedMethod != null;
				EditorGUILayout.BeginHorizontal();

					methodIndex = EditorGUILayout.Popup(popupLabel, methodIndex, methodLabels);

					if (GUILayout.Button(invokeLabel, EditorStyles.miniButton, GUILayout.ExpandWidth(false))) {
						Undo.RegisterSceneUndo(selectedMethod.name + " Call");
						selectedMethod.Invoke();
					}

				EditorGUILayout.EndHorizontal();

				if (selectedMethod != null && selectedMethod.hasParameters) {
					Util.DrawDivider();
					selectedMethod.DisplayParameters();
				}

				GUI.enabled = true;
				break;

			case Panel.Preferences:
				Preferences.OnGUI();
				break;
		}
	}

	void OnSelectionChange ()
	{
		DiscoverMethods();
	}

	/// <summary>
	/// Discovers the selected game object's methods and refreshes the GUI.
	/// </summary>
	public void DiscoverMethods ()
	{
		var target = Selection.activeGameObject;
		var methods = new List<Method>();
		var methodLabels = new List<GUIContent>();

		if (target != null) {
			// Discover methods in attached components
			foreach (var component in target.GetComponents<MonoBehaviour>()) {
				var type = component.GetType();
				var allMethods = type.GetMethods(Preferences.flags);

				foreach (var method in allMethods) {
					methods.Add(new Method(component, method));
					var label = new GUIContent("", method.ToString());

					if (Preferences.displayClass) {
						label.text = component.GetType() + ": ";
					}

					label.text += method.Name;
					methodLabels.Add(label);
				}
			}
		}

		this.methods = methods.ToArray();
		this.methodLabels = methodLabels.ToArray();
		this.methodIndex = 0;
		Repaint();
	}
}

