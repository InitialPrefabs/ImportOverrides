using System;
using System.Reflection;

namespace InitialPrefabs.ImportOverrides {
    static class ReflectionUtils {
        public static object Invoke<T>(object target, string method, BindingFlags flags, params object[] args) {
            var type = typeof(T);
            return Invoke(type, target, method, flags, args);
        }

        public static object Invoke(Type type, object target, string method, BindingFlags flags, params object[] args) {
            var methodInfo = type.GetMethod(method, flags);
            if (methodInfo != null) {
                UnityEngine.Debug.Log("Invoked");
                return methodInfo.Invoke(target, args);
            }
            return null;
        }

        public static object Invoke(
            Type type, 
            object target, 
            string method, 
            BindingFlags flags, 
            Type[] argTypes, 
            object[] args, 
            ParameterModifier[] paramModifiers) {
            var methodInfo = type.GetMethod(method, argTypes.Length, flags, null, argTypes, paramModifiers);
            if (methodInfo != null) {
                return methodInfo.Invoke(target, args);
            }
            return null;
        }

        public static void SetField<T>(object target, object value, string name, BindingFlags flags) {
            var type = typeof(T);
            var fieldInfo = type.GetField(name, flags);
            fieldInfo.SetValue(target, value);
        }
    }
}

