//
// Author: Matthew Miner
//         matthew@matthewminer.com
//         http://matthewminer.com
//
// Copyright (c) 2013
//

using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Methodic
{
	/// <summary>
	/// Holds method information and displays a GUI to invoke it.
	/// </summary>
	class Method
	{
		readonly Component component;
		readonly MethodInfo info;
		readonly Parameter[] parameters;

		Vector2 scrollPos;

		/// <summary>
		/// The name of the method.
		/// </summary>
		internal string name
		{
			get { return info.Name; }
		}

		/// <summary>
		/// Creates a new Method instance.
		/// </summary>
		/// <param name="component">The component the scripts are attached to.</param>
		/// <param name="method">The method.</param>
		internal Method (Component component, MethodInfo info)
		{
			this.component = component;
			this.info = info;
			this.parameters = info.GetParameters().Select(p => new Parameter(p)).ToArray();
		}

		/// <summary>
		/// If method has parameters, displays fields for them to be modified.
		/// </summary>
		internal void OnGUI ()
		{
			// Skip out early if there are no parameters to display.
			if (parameters.Length == 0) {
				return;
			}

			scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

			foreach (var parameter in parameters) {
				parameter.OnGUI();
			}

			EditorGUILayout.EndScrollView();
		}

		/// <summary>
		/// Executes the method.
		/// </summary>
		internal void Invoke ()
		{
			try {
				var input = parameters.Select(p => p.val).ToArray();
				var result = info.Invoke(component, input);

				// Display the return value, if one is expected.
				if (info.ReturnType != typeof(void)) {
					Debug.Log("[Methodic] Result: " + result);
				}
			} catch (ArgumentException e) {
				Debug.LogError("[Methodic] Unable to invoke method: " + e.Message);
			}
		}

		/// <summary>
		/// Construct a GUI label representing the method.
		/// </summary>
		internal GUIContent GetLabel ()
		{
			var label = new GUIContent(info.Name, info.ToString());

			if (Preferences.displayClass) {
				label.text = component.GetType() + ": " + info.Name;
			}

			return label;
		}
	}
}
