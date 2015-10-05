//
// Author: Matthew Miner
//         matthew@matthewminer.com
//         http://matthewminer.com
//
// Copyright (c) 2015
//

using System;
using System.Reflection;
using System.Linq;
using UnityEngine;

namespace Methodic
{
	/// <summary>
	/// Performs the heavy lifting of finding methods and parameters.
	/// </summary>
	static class Reflector
	{
		/// <summary>
		/// Executes a method.
		/// </summary>
		/// <param name="component">Component to execute the method on.</param>
		/// <param name="method">Method to execute.</param>
		/// <param name="parameterValues">Values to send to method.</param>
		internal static void InvokeMethod (MonoBehaviour component, MethodInfo method, object[] parameterValues)
		{
			try {
				var result = method.Invoke(component, parameterValues);

				// Display the return value, if one is expected.
				if (method.ReturnType != typeof(void)) {
					Debug.Log("[Methodic] Result: " + result);
				}
			} catch (ArgumentException e) {
				Debug.LogError("[Methodic] Unable to invoke method: " + e.Message);
			}
		}

		/// <summary>
		/// Finds scripts attached to the given game object.
		/// Only ones containing methods are returned.
		/// </summary>
		/// <param name="gameObject">Target game object.</param>
		/// <returns>Component array.</returns>
		internal static MonoBehaviour[] GetComponents (GameObject gameObject)
		{
			if (gameObject == null) {
				return new MonoBehaviour[] {};
			}

			var components = gameObject
				.GetComponents<MonoBehaviour>()
				.Where(component => component
					.GetType()
					.GetMethods(Preferences.reflectionOptions)
					.Any())
				.ToArray();

			return components;
		}

		/// <summary>
		/// Gets an array of component labels for display in a GUI dropdown.
		/// </summary>
		/// <param name="methods">Components to get labels for.</param>
		/// <returns>Labels array.</returns>
		internal static string[] GetComponentLabels (MonoBehaviour[] components)
		{
			if (components == null) {
				return new string[] {};
			}

			var labels = components
				.Select(component => component.GetType().Name)
				.ToArray();

			return labels;
		}

		/// <summary>
		/// Finds methods contained in the given component.
		/// </summary>
		/// <param name="component">Target component.</param>
		/// <returns>Method array.</returns>
		internal static MethodInfo[] GetMethods (MonoBehaviour component)
		{
			if (component == null) {
				return new MethodInfo[] {};
			}

			var methods = component
				.GetType()
				.GetMethods(Preferences.reflectionOptions)
				.ToArray();

			return methods;
		}

		/// <summary>
		/// Gets an array of method labels for display in a GUI dropdown.
		/// </summary>
		/// <param name="methods">Methods to get labels for.</param>
		/// <returns>Labels array.</returns>
		internal static string[] GetMethodLabels (MethodInfo[] methods)
		{
			if (methods == null) {
				return new string[] {};
			}

			var labels = methods
				.Select(method => method.Name)
				.ToArray();

			return labels;
		}

		/// <summary>
		/// Finds parameters that can be provided to the given method.
		/// </summary>
		/// <param name="method">Target method.</param>
		/// <returns>Parameter array.</returns>
		internal static ParameterInfo[] GetParameters (MethodInfo method)
		{
			if (method == null) {
				return new ParameterInfo[] {};
			}

			var parameters = method
				.GetParameters()
				.ToArray();

			return parameters;
		}

		/// <summary>
		/// Gets the default values for the given parameters.
		/// </summary>
		/// <param name="parameters">Parameter info array.</param>
		/// <returns>Default values array.</returns>
		internal static object[] GetDefaultParameterValues (ParameterInfo[] parameters)
		{
			if (parameters == null) {
				return new object[] {};
			}

			var values = parameters
				.Select(parameter => GetDefaultParameterValue(parameter))
				.ToArray();

			return values;
		}

		/// <summary>
		/// Gets the default value for the given parameter.
		/// This will vary depending on the parameter's type.
		/// </summary>
		/// <param name="parameter">Parameter info.</param>
		/// <returns>Default value.</returns>
		static object GetDefaultParameterValue (ParameterInfo parameter)
		{
			object defaultValue = null;

			if (parameter.ParameterType.IsValueType) {
				defaultValue = Activator.CreateInstance(parameter.ParameterType);
			} else if (parameter.ParameterType == typeof(string)) {
				defaultValue = "";
			} else if (parameter.ParameterType == typeof(AnimationCurve)) {
				defaultValue = new AnimationCurve();
			}

			return defaultValue;
		}
	}
}
