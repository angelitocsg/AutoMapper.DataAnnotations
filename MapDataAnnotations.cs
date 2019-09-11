using AutoMapper.DataAnnotations.Attributes;
using AutoMapper.DataAnnotations.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AutoMapper.DataAnnotations
{
    public class MapDataAnnotations : IMapperFactory
    {
        private static IEnumerable<Assembly> _assemblies;
        private readonly IEnumerable<Type> _mapTargets;
        private readonly Profile _profile;
        private readonly bool _reverseMap;
        private readonly Type _type;

        public static void Init(IEnumerable<Assembly> assemblies)
        {
            _assemblies = assemblies;
        }

        public MapDataAnnotations(Type type, Profile profile, bool reverseMap)
        {
            if (_assemblies == null) { throw new InvalidOperationException($"Please execute Init() on 'MapDataAnnotations' before start"); }
            _type = type;
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
            var types = GetMapperBySource(_type);

            if (targetExplicit != null)
                types = types.Where(e => e.GetType() == targetExplicit);

            if (types.Count() == 0)
                throw new InvalidOperationException("Target Type Not Found For given source");

            var target = types.FirstOrDefault();

            if (HasMapFieldNameAttribute(target))
            {
                return CreateMapWithForMembers(_type, target);
            }

            var mappingExpression = _profile.CreateMap(_type, target);
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
