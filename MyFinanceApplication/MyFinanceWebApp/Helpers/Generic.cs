using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MyFinanceWebApp.Helpers
{
    public static class ObjectUtils
    {
        public static T GetCopy<T>(this T source) where T : new()
        {
            var destination = new T();
            CopyFields(source, destination);
            return destination;
        }

        public static void CopyFields<T>(T source, T destination)
        {
            var fields = GetAllFields(source.GetType());
            foreach (var field in fields)
            {
                field.SetValue(destination, field.GetValue(source));
            }
        }

        private static IEnumerable<FieldInfo> GetAllFields(Type t)
        {
            if (t == null)
                return Enumerable.Empty<FieldInfo>();

            return t.GetRuntimeFields().Concat(GetAllFields(t.GetTypeInfo().BaseType));
        }
    }
}