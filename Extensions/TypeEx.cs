using System;

namespace AutoMapper.DataAnnotations.Extensions
{
    public static class TypeEx
    {
        public static (IMappingExpression mapFrom, IMappingExpression mapTo) GetCustomMapper<T>(
            this Type type,
            Profile profile,
            bool reverseMap) where T : class
        {
            var builder = new MapDataAnnotations(typeof(T), profile, reverseMap);
            return builder.BuildMapper();
        }
    }
}
