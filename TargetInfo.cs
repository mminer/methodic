//
// Author: Matthew Miner
//         matthew@matthewminer.com
//         https://matthewminer.com
//
// Copyright (c) 2016
//

using System.Reflection;
using UnityEngine;

namespace Methodic
{
    /// <summary>
    /// Holds information about the target game object's components / methods.
    /// </summary>
    static class TargetInfo
    {
        internal static MonoBehaviour[] components { get; private set; }
        internal static string[] componentLabels { get; private set; }
        internal static MethodInfo[] methods { get; private set; }
        internal static string[] methodLabels { get; private set; }
        internal static ParameterInfo[] parameters { get; private set; }
        internal static object[] parameterValues { get; private set; }

        static TargetInfo ()
        {
            components = new MonoBehaviour[] {};
            componentLabels = new string[] {};
            methods = new MethodInfo[] {};
            methodLabels = new string[] {};
            parameters = new ParameterInfo[] {};
            parameterValues = new object[] {};
        }

        internal static void SetSelectedGameObject (GameObject selectedGameObject)
        {
            components = Reflector.GetComponents(selectedGameObject);
            componentLabels = Reflector.GetComponentLabels(components);
        }

        internal static void SetSelectedComponent (MonoBehaviour selectedComponent)
        {
            methods = Reflector.GetMethods(selectedComponent);
            methodLabels = Reflector.GetMethodLabels(methods);
        }

        internal static void SetSelectedMethod (MethodInfo selectedMethod)
        {
            parameters = Reflector.GetParameters(selectedMethod);
            parameterValues = Reflector.GetDefaultParameterValues(TargetInfo.parameters);
        }
    }
}
