using System;
using System.Collections.Generic;
using System.Reflection;

namespace AutoMapper.DataAnnotations.Extensions
{
    public static class TypeEx
    {
        public static (IMappingExpression mapFrom, IMappingExpression mapTo) GetCustomMapper<T>(
            this Type type,
            Profile profile,
            bool reverseMap,
            IEnumerable<Assembly> assemblies) where T : class
        {
            var builder = new AutoMapperAttributeBuilder<T>(assemblies, profile, true);
            return builder.BuildMapper();
        }
    }
}
