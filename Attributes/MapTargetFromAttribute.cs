using System;

namespace AutoMapper.DataAnnotations.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MapTargetFromAttribute : Attribute
    {
        public Type Source;
        public MapTargetFromAttribute(Type source)
        {
            Source = source;
        }
    }
}
