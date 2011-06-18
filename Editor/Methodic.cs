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
	
	static GUIContent popupLabel = new GUIContent("Method");
	static GUIContent invokeLabel = new GUIContent("Invoke", "Execute this method.");
	
	GameObject target;
	List<Method> methods = new List<Method>();
	List<GUIContent> methodLabels = new List<GUIContent>();
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
		if (methods.Count == 0) {
			GUI.enabled = false;
		}
		
		EditorGUILayout.BeginHorizontal();
			
			selected = EditorGUILayout.Popup(popupLabel, selected, methodLabels.ToArray());
			
			if (GUILayout.Button(invokeLabel, EditorStyles.miniButton, GUILayout.ExpandWidth(false))) {
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
		methods = new List<Method>();
		methodLabels = new List<GUIContent>();
		selected = 0;
		
		if (target != null) {
			// Discover methods in attached components
			foreach (var component in target.GetComponents<MonoBehaviour>()) {
				var type = component.GetType();
				var publicMethods = type.GetMethods(BindingFlags.Public|BindingFlags.Instance|BindingFlags.DeclaredOnly);
				var privateMethods = type.GetMethods(BindingFlags.NonPublic|BindingFlags.Instance|BindingFlags.DeclaredOnly);
				
				foreach (var method in publicMethods) {
					methods.Add(new Method { component = component, method = method });
					methodLabels.Add(new GUIContent(method.Name, method.ToString()));
				}
				
				foreach (var method in privateMethods) {
					methods.Add(new Method { component = component, method = method });
					methodLabels.Add(new GUIContent(method.Name, method.ToString()));
				}
			}
		}
		
		Repaint();
	}
}