// Copyright (c) 2011 Matthew Miner

using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class Methodic : EditorWindow
{
	struct Method
	{
		public Component component;
		public MethodInfo method;
	}
	
	GameObject target;
	int selected;
	
	/// <summary>
	/// Adds Methodic to Window menu.
	/// </summary>
	[MenuItem ("Window/Methodic")]
	static void Init ()
	{
		// Get existing open window, or make new one if none
		EditorWindow.GetWindow<Methodic>().Show();
	}
	
	void OnGUI ()
	{
		if (target == null) {
			return;
		}
		
		var methods = new List<Method>();
		
		foreach (var component in target.GetComponents<MonoBehaviour>()) {
			var type = component.GetType();
			var publicMethods = type.GetMethods(BindingFlags.Public|BindingFlags.Instance|BindingFlags.DeclaredOnly);
			var privateMethods = type.GetMethods(BindingFlags.NonPublic|BindingFlags.Instance|BindingFlags.DeclaredOnly);
			
			foreach (var method in publicMethods) {
				methods.Add(new Method { component = component, method = method });
			}
			
			foreach (var method in privateMethods) {
				methods.Add(new Method { component = component, method = method });
			}
		}
		
		if (methods.Count > 0) {
			EditorGUILayout.BeginHorizontal();
				
				var methodNames = new string[methods.Count];
				
				for (int i = 0; i < methods.Count; i++) {
					methodNames[i] = methods[i].method.Name;
				}
			
				selected = EditorGUILayout.Popup("Method", selected, methodNames);
				
				if (GUILayout.Button("Invoke", EditorStyles.miniButton, GUILayout.ExpandWidth(false))) {
					methods[selected].method.Invoke(methods[selected].component, null); // null = parameters
				}
			
			EditorGUILayout.EndHorizontal();
		}
	}
	
	/// <summary>
	/// Refreshes the GUI when a new game object is selected.
	/// </summary>
	void OnSelectionChange ()
	{
		target = Selection.activeGameObject;
		Repaint();
	}
}