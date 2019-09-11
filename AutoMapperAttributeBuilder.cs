using AutoMapper.DataAnnotations.Attributes;
using AutoMapper.DataAnnotations.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AutoMapper.DataAnnotations
{
    public class AutoMapperAttributeBuilder<T1> : IMapperFactory<T1> where T1 : class
    {
        private readonly IEnumerable<Assembly> _assemblies;
        private readonly IEnumerable<Type> _mapTargets;
        private readonly Profile _profile;
        private readonly bool _reverseMap;

        public AutoMapperAttributeBuilder(IEnumerable<Assembly> assemblies, Profile profile, bool reverseMap)
        {
            _assemblies = assemblies;
            _mapTargets = GetMapTargets();
            _profile = profile;
            _reverseMap = reverseMap;
        }

        private IEnumerable<Type> GetMapTargets()
        {
            return from a in _assemblies
                   from b in a.GetTypes()
                   where b.IsClass && b.GetCustomAttributes<MapTargetFromAttribute>().Count() > 0
                   select b;
        }

        private IEnumerable<Type> GetMapperBySource(Type typeSource)
        {
            return _mapTargets.Where(t => t.GetCustomAttribute<MapTargetFromAttribute>().Source == typeSource);
        }

        public (IMappingExpression mapFrom, IMappingExpression mapTo) BuildMapper(Type targetExplicit = null)
        {
            var types = GetMapperBySource(typeof(T1));

            if (targetExplicit != null)
                types = types.Where(e => e.GetType() == targetExplicit);

            if (types.Count() == 0)
                throw new InvalidOperationException("Target Type Not Found For given source");

            var target = types.FirstOrDefault();

            if (HasMapFieldNameAttribute(target))
            {
                return CreateMapWithForMembers(typeof(T1), target);
            }

            var mappingExpression = _profile.CreateMap(typeof(T1), target);
            if (_reverseMap) { mappingExpression.ReverseMap(); }

            return (mappingExpression, null);
        }

        private (IMappingExpression mapFrom, IMappingExpression mapTo) CreateMapWithForMembers(Type fromType, Type targetType)
        {
            IMappingExpression mappingExpression = _profile.CreateMap(fromType, targetType);
            IMappingExpression mappingExpressionReverse = null;

            foreach (var property in GetMapFieldNameProperties(targetType))
            {
                foreach (var mapField in GetMapFieldName(property))
                {
                    var sourceField = mapField.FieldName;
                    var targetField = property.Name;
                    mappingExpression.ForMember(targetField, m => m.MapFrom(sourceField));
                }
            }
            if (_reverseMap) { mappingExpressionReverse = CreateMapWithForMembersReverse(fromType, targetType); }

            return (mappingExpression, mappingExpressionReverse);
        }

        private IMappingExpression CreateMapWithForMembersReverse(Type fromType, Type targetType)
        {
            var mappingExpression = _profile.CreateMap(targetType, fromType);

            foreach (var property in GetMapFieldNameProperties(targetType))
            {
                foreach (var mapField in GetMapFieldName(property))
                {
                    var sourceField = property.Name;
                    var targetField = mapField.FieldName;
                    mappingExpression.ForMember(targetField, m => m.MapFrom(sourceField));
                }
            }

            return mappingExpression;
        }

        private IEnumerable<MapFieldFromAttribute> GetMapFieldName(PropertyInfo property)
        {
            return property.GetCustomAttributes<MapFieldFromAttribute>();
        }

        private bool HasMapFieldNameAttribute(Type targetType)
        {
            return targetType.GetProperties().Any(p => p.GetCustomAttributes<MapFieldFromAttribute>().Count() > 0);
        }

        private IEnumerable<PropertyInfo> GetMapFieldNameProperties(Type targetType)
        {
            return targetType.GetProperties().Where(p => p.GetCustomAttributes<MapFieldFromAttribute>().Count() > 0);
        }
    }
}
