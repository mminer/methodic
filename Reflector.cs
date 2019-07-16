//
// Author: Matthew Miner
//         matthew@matthewminer.com
//         https://matthewminer.com
//
// Copyright (c) 2019
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
        internal static void InvokeMethod(MonoBehaviour component, MethodInfo method, object[] parameterValues)
        {
            try {
                var result = method.Invoke(component, parameterValues);
                LogReturnValue(method, result);
            } catch (ArgumentException e) {
                Debug.LogError("[Methodic] Unable to invoke method: " + e.Message);
            }
        }

        internal static MonoBehaviour[] GetComponents(GameObject gameObject)
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

        internal static string[] GetComponentLabels(MonoBehaviour[] components)
        {
            if (components == null) {
                return new string[] {};
            }

            var labels = components
                .Select(component => component.GetType().Name)
                .ToArray();

            return labels;
        }

        internal static MethodInfo[] GetMethods(MonoBehaviour component)
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

        internal static string[] GetMethodLabels(MethodInfo[] methods)
        {
            if (methods == null) {
                return new string[] {};
            }

            var labels = methods
                .Select(method => method.Name)
                .ToArray();

            return labels;
        }

        internal static ParameterInfo[] GetParameters(MethodInfo method)
        {
            if (method == null) {
                return new ParameterInfo[] {};
            }

            var parameters = method
                .GetParameters()
                .ToArray();

            return parameters;
        }

        internal static object[] GetDefaultParameterValues(ParameterInfo[] parameters)
        {
            if (parameters == null) {
                return new object[] {};
            }

            var values = parameters
                .Select(parameter => GetDefaultParameterValue(parameter))
                .ToArray();

            return values;
        }

        static object GetDefaultParameterValue(ParameterInfo parameter)
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

        static void LogReturnValue(MethodInfo method, object result)
        {
            if (method.ReturnType == typeof(void)) {
                return;
            }

            Debug.Log("[Methodic] Result: " + result);
        }
    }
}
