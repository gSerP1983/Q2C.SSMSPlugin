using System;
using System.Linq;

namespace Q2C.Core
{
    public static class TypeExt
    {
        public static bool IsImplementInterface(this Type type, Type interfaceType)
        {
            return type.GetInterfaces()
                .Any(x => (x.IsGenericType && x.GetGenericTypeDefinition() == interfaceType) || x == interfaceType);
        } 
    }
}