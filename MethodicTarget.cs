// Copyright Matthew Miner <matthew@matthewminer.com>

using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Methodic
{
    /// <summary>
    /// Holds information about the target game object's components / methods.
    /// </summary>
    class MethodicTarget
    {
        public bool showPrivate { get; set; }
        public bool showStatic { get; set; }

        public MonoBehaviour[] components { get; private set; } = Array.Empty<MonoBehaviour>();
        public string[] componentLabels { get; private set; } = Array.Empty<string>();
        public MethodInfo[] methods { get; private set; } = Array.Empty<MethodInfo>();
        public string[] methodLabels { get; private set; } = Array.Empty<string>();
        public ParameterInfo[] parameters { get; private set; } = Array.Empty<ParameterInfo>();
        public object[] parameterValues { get; private set; } = Array.Empty<object>();

        BindingFlags bindingFlags
        {
            get
            {
                var flags = BindingFlags.DeclaredOnly |
                            BindingFlags.Instance |
                            BindingFlags.Public;

                if (showPrivate)
                {
                    flags |= BindingFlags.NonPublic;
                }

                if (showStatic)
                {
                    flags |= BindingFlags.Static;
                }

                return flags;
            }
        }

        public void SetSelectedGameObject(GameObject selectedGameObject)
        {
            components = GetComponents(selectedGameObject, bindingFlags);
            componentLabels = GetComponentLabels(components);
        }

        public void SetSelectedComponent(MonoBehaviour selectedComponent)
        {
            methods = GetMethods(selectedComponent, bindingFlags);
            methodLabels = GetMethodLabels(methods);
        }

        public void SetSelectedMethod(MethodInfo selectedMethod)
        {
            parameters = GetParameters(selectedMethod);
            parameterValues = GetParameterDefaultValues(parameters);
        }

        static MonoBehaviour[] GetComponents(GameObject gameObject, BindingFlags bindingFlags)
        {
            if (gameObject == null)
            {
                return Array.Empty<MonoBehaviour>();
            }

            return gameObject
                .GetComponents<MonoBehaviour>()
                .Where(component => component
                    .GetType()
                    .GetMethods(bindingFlags)
                    .Any())
                .ToArray();
        }

        static string[] GetComponentLabels(MonoBehaviour[] components)
        {
            if (components == null)
            {
                return Array.Empty<string>();
            }

            return components
                .Select(component => component.GetType().Name)
                .ToArray();
        }

        static MethodInfo[] GetMethods(MonoBehaviour component, BindingFlags bindingFlags)
        {
            if (component == null)
            {
                return Array.Empty<MethodInfo>();
            }

            return component
                .GetType()
                .GetMethods(bindingFlags)
                .ToArray();
        }

        static string[] GetMethodLabels(MethodInfo[] methods)
        {
            if (methods == null)
            {
                return Array.Empty<string>();
            }

            return methods
                .Select(method => method.Name)
                .ToArray();
        }

        static ParameterInfo[] GetParameters(MethodInfo method)
        {
            if (method == null)
            {
                return Array.Empty<ParameterInfo>();
            }

            return method
                .GetParameters()
                .ToArray();
        }

        static object[] GetParameterDefaultValues(ParameterInfo[] parameters)
        {
            if (parameters == null)
            {
                return Array.Empty<object>();
            }

            return parameters
                .Select(parameter =>
                {
                    if (parameter.ParameterType.IsValueType)
                    {
                        return Activator.CreateInstance(parameter.ParameterType);
                    }

                    if (parameter.ParameterType == typeof(AnimationCurve))
                    {
                        return new AnimationCurve();
                    }

                    if (parameter.ParameterType == typeof(string))
                    {
                        return "";
                    }

                    return null;
                })
                .ToArray();
        }
    }
}
