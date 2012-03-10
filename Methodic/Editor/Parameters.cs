//
// Parameters.cs
//
// Author: Matthew Miner
//         matthew@matthewminer.com
//         http://www.matthewminer.com/
//
// Copyright (c) 2012
//

using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Methodic
{
	/// <summary>
	/// Panel for inputting parameters into a method for execution.
	/// </summary>
	class Parameters
	{
		readonly Parameter[] parameters;
		Vector2 scrollPos;

		internal object[] parametersArray {
			get {
				var arr = parameters.Select(param => param.val).ToArray();
				return arr;
			}
		}

		/// <summary>
		/// Sets up the parameters.
		/// </summary>
		/// <param name="method">The method to invoke.</param>
		internal Parameters (MethodInfo method)
		{
			var info = method.GetParameters();
			parameters = new Parameter[info.Length];

			for (int i = 0; i < parameters.Length; i++) {
				parameters[i] = new Parameter(info[i]);
			}
		}

		internal void OnGUI ()
		{
			scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

			// Display an appropriate input field for each parameter
			for (int i = 0; i < parameters.Length; i++) {
				parameters[i].OnGUI();
			}

			EditorGUILayout.EndScrollView();
		}
	}
}

