// Copyright (c) 2011 Matthew Miner

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

/// <summary>
/// Displays a dropdown menu of functions available for the selected game object to access.
/// </summary>
public class Methodic : EditorWindow
{
	struct Method
	{
		public Component component;
		public MethodInfo method;
	}
	
	GameObject target;
	List<Method> methods = new List<Method>();
	List<string> methodNames = new List<string>();
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
		
		if (methods.Count == 0) {
			GUI.enabled = false;
		}
		
		EditorGUILayout.BeginHorizontal();
			
			selected = EditorGUILayout.Popup("Method", selected, methodNames.ToArray());
			
			if (GUILayout.Button("Invoke", EditorStyles.miniButton, GUILayout.ExpandWidth(false))) {
				methods[selected].method.Invoke(methods[selected].component, null); // null = parameters
			}
		
		EditorGUILayout.EndHorizontal();
	}
	
	/// <summary>
	/// Refreshes the GUI when a new game object is selected.
	/// </summary>
	void OnSelectionChange ()
	{
		target = Selection.activeGameObject;
		
		if (target != null) {
			methods = new List<Method>();
			
			foreach (var component in target.GetComponents<MonoBehaviour>()) {
				var type = component.GetType();
				var publicMethods = type.GetMethods(BindingFlags.Public|BindingFlags.Instance|BindingFlags.DeclaredOnly);
				var privateMethods = type.GetMethods(BindingFlags.NonPublic|BindingFlags.Instance|BindingFlags.DeclaredOnly);
				
				foreach (var method in publicMethods) {
					methods.Add(new Method { component = component, method = method });
					methodNames.Add(method.Name);
				}
				
				foreach (var method in privateMethods) {
					methods.Add(new Method { component = component, method = method });
					methodNames.Add(method.Name);
				}
			}
		}
		
		Repaint();
	}
}