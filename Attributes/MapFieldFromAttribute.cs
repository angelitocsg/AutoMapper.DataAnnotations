using System;

namespace AutoMapper.DataAnnotations.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class MapFieldFromAttribute : Attribute
    {
        public string FieldName { get; }

        public MapFieldFromAttribute(string fieldName)
        {
            FieldName = fieldName;
        }
    }
}
