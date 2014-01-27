//
// Author: Matthew Miner
//         matthew@matthewminer.com
//         http://matthewminer.com
//
// Copyright (c) 2013
//

using System.Linq;
using UnityEngine;

namespace Methodic
{
	/// <summary>
	/// Discovers methods in components attached to the active game object.
	/// </summary>
	static class MethodFinder
	{
		/// <summary>
		/// Finds the specified game object's methods.
		/// </summary>
		/// <param name="target">Target game object.</param>
		/// <returns>Methods array.</returns>
		internal static Method[] GetMethods (GameObject target)
		{
			if (target == null) {
				return null;
			}

			var methods = target
				.GetComponents<MonoBehaviour>()
				.SelectMany(component => component
					.GetType()
					.GetMethods(Preferences.reflectionOptions)
					.Select(info => new Method(component, info)))
				.ToArray();

			return methods;
		}

		/// <summary>
		/// Gets an array of method labels for display in a GUI dropdown.
		/// </summary>
		/// <param name="methods">Methods to get labels for.</param>
		/// <returns>Labels.</returns>
		internal static GUIContent[] GetMethodLabels (Method[] methods)
		{
			if (methods == null) {
				return null;
			}

			var labels = methods
				.Select(method => method.GetLabel())
				.ToArray();

			return labels;
		}
	}
}
