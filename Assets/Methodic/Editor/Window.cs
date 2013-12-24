//
// Author: Matthew Miner
//         matthew@matthewminer.com
//         http://matthewminer.com
//
// Copyright (c) 2013
//

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Methodic
{
	/// <summary>
	/// Entry point to the Methodic editor window.
	/// </summary>
	class Window : EditorWindow
	{
		static readonly GUIContent invokeLabel = new GUIContent("Invoke", "Execute this method.");

		Method[] methods = {};
		GUIContent[] methodLabels = {};
		int selectedIndex;

		Method selectedMethod
		{
			get {
				if (selectedIndex >= 0 && selectedIndex < methods.Length) {
					return methods[selectedIndex];
				} else {
					return null;
				}
			}
		}

		void OnGUI ()
		{
			GUI.enabled = selectedMethod != null;

			EditorGUILayout.BeginHorizontal();

			selectedIndex = EditorGUILayout.Popup(selectedIndex, methodLabels);

			if (GUILayout.Button(invokeLabel, EditorStyles.miniButton)) {
				var undoLabel = selectedMethod.name + " Call";
				Undo.RecordObject(Selection.activeGameObject, undoLabel);
				selectedMethod.Invoke();
			}

			EditorGUILayout.EndHorizontal();

			if (selectedMethod != null) {
				selectedMethod.OnGUI();
			}

			GUI.enabled = true;
		}

		internal void OnSelectionChange ()
		{
			methods = DiscoverMethods(Selection.activeGameObject);
			methodLabels = methods.Select(m => m.GetLabel()).ToArray();
			selectedIndex = 0;
			Repaint();
		}

		/// <summary>
		/// Discovers the specified game object's methods.
		/// </summary>
		static Method[] DiscoverMethods (GameObject target)
		{
			var methods = new List<Method>();

			if (target != null) {
				// Discover methods in attached components.
				var components = target.GetComponents<MonoBehaviour>();

				foreach (var component in components) {
					var componentMethods = component.GetType()
					                                .GetMethods(Preferences.reflectionFlags)
					                                .Select(info => new Method(component, info));
					methods.AddRange(componentMethods);
				}
			}

			return methods.ToArray();
		}
	}
}
