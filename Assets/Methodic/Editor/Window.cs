//
// Author: Matthew Miner
//         matthew@matthewminer.com
//         http://matthewminer.com
//
// Copyright (c) 2013
//

using UnityEditor;
using UnityEngine;

namespace Methodic
{
	/// <summary>
	/// Entry point to the Methodic editor window.
	/// </summary>
	class Window : EditorWindow
	{
		internal static bool isOpen { get; private set; }

		const string noSelectionMessage = "Select a game object in the Hierarchy to list its methods.";
		const string noMethodsMessage = "Components attached to the selected game object contain no methods of the desired type.";
		static readonly GUIContent invokeLabel = new GUIContent("Invoke", "Execute this method.");

		Method[] methods;
		GUIContent[] methodLabels;
		int selectedIndex;

		/// <summary>
		/// Resets the methods and labels to the selected game object's.
		/// </summary>
		internal void Refresh ()
		{
			methods = MethodFinder.GetMethods(Selection.activeGameObject);
			methodLabels = MethodFinder.GetMethodLabels(methods);
			selectedIndex = 0;
			Repaint();
		}

		void OnGUI ()
		{
			if (methods == null) {
				EditorGUILayout.HelpBox(noSelectionMessage, MessageType.Info);
				return;
			}

			if (methods.Length == 0) {
				EditorGUILayout.HelpBox(noMethodsMessage, MessageType.Warning);
				return;
			}

			EditorGUILayout.BeginHorizontal();

			selectedIndex = EditorGUILayout.Popup(selectedIndex, methodLabels);
			var selectedMethod = methods[selectedIndex];

			if (GUILayout.Button(invokeLabel, EditorStyles.miniButton)) {
				var undoLabel = string.Format("{0} Call", selectedMethod.name);
				Undo.RecordObject(Selection.activeGameObject, undoLabel);
				selectedMethod.Invoke();
			}

			EditorGUILayout.EndHorizontal();

			selectedMethod.OnGUI();
		}

		void OnSelectionChange ()
		{
			Refresh();
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
	}
}
