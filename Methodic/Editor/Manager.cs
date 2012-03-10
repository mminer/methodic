//
// Manager.cs
//
// Author: Matthew Miner
//         matthew@matthewminer.com
//         http://www.matthewminer.com/
//
// Copyright (c) 2012
//

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Methodic
{
	/// <summary>
	/// The method manager; functions to display the GUI.
	/// </summary>
	public static class Manager
	{
		static readonly GUIContent popupLabel = new GUIContent("Method");
		static readonly GUIContent invokeLabel = new GUIContent("Invoke", "Execute this method.");

		static Method[] methods = {};
		static GUIContent[] methodLabels = {};
		static int selectedIndex;

		static Method selectedMethod {
			get {
				if (selectedIndex >= 0 && selectedIndex < methods.Length) {
					return methods[selectedIndex];
				} else {
					return null;
				}
			}
		}

		public static void OnGUI ()
		{
			GUI.enabled = selectedMethod != null;
			EditorGUILayout.BeginHorizontal();

				selectedIndex = EditorGUILayout.Popup(popupLabel, selectedIndex, methodLabels);

				if (GUILayout.Button(invokeLabel, EditorStyles.miniButton, GUILayout.ExpandWidth(false))) {
					Undo.RegisterSceneUndo(selectedMethod.name + " Call");
					selectedMethod.Invoke();
				}

			EditorGUILayout.EndHorizontal();

			if (selectedMethod != null) {
				selectedMethod.OnGUI();
			}

			GUI.enabled = true;
		}

		/// <summary>
		/// Discovers the selected game object's methods and refreshes the GUI.
		/// </summary>
		public static void DiscoverMethods (GameObject selected)
		{
			var newMethods = new List<Method>();
			var newMethodLabels = new List<GUIContent>();

			if (selected != null) {
				// Discover methods in attached components
				foreach (var component in selected.GetComponents<MonoBehaviour>()) {
					var type = component.GetType();
					var allMethods = type.GetMethods(Preferences.flags);

					foreach (var method in allMethods) {
						newMethods.Add(new Method(component, method));
						var label = new GUIContent("", method.ToString());

						if (Preferences.displayClass) {
							label.text = component.GetType() + ": ";
						}

						label.text += method.Name;
						newMethodLabels.Add(label);
					}
				}
			}

			methods = newMethods.ToArray();
			methodLabels = newMethodLabels.ToArray();
			selectedIndex = 0;
		}
	}
}

