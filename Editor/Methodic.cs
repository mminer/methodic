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
	
	static BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
	static GUIContent popupLabel = new GUIContent("Method");
	static GUIContent invokeLabel = new GUIContent("Invoke", "Execute this method.");
	
	GameObject target;
	Method[] methods = {};
	GUIContent[] methodLabels = {};
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
		if (methods.Length == 0) {
			GUI.enabled = false;
		}
		
		EditorGUILayout.BeginHorizontal();
			
			selected = EditorGUILayout.Popup(popupLabel, selected, methodLabels);
			
			if (GUILayout.Button(invokeLabel, EditorStyles.miniButton, GUILayout.ExpandWidth(false))) {
				var toInvoke = methods[selected];
				var result = toInvoke.method.Invoke(toInvoke.component, null); // null = parameters
				
				if (toInvoke.method.ReturnType != typeof(void)) {
					Debug.Log("[Methodic] Result: " + result);
				}
			}
		
		EditorGUILayout.EndHorizontal();
	}
	
	/// <summary>
	/// Discovers the selected game object's methods and refreshes the GUI.
	/// </summary>
	void OnSelectionChange ()
	{
		target = Selection.activeGameObject;
		selected = 0;
		var methods = new List<Method>();
		var methodLabels = new List<GUIContent>();
		
		if (target != null) {
			// Discover methods in attached components
			foreach (var component in target.GetComponents<MonoBehaviour>()) {
				var type = component.GetType();
				var allMethods = type.GetMethods(flags);
				
				foreach (var method in allMethods) {
					methods.Add(new Method { component = component, method = method });
					methodLabels.Add(new GUIContent(method.Name, method.ToString()));
				}
			}
		}
		
		this.methods = methods.ToArray();
		this.methodLabels = methodLabels.ToArray();
		Repaint();
	}
}