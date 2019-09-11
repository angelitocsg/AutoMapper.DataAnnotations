using System;

namespace AutoMapper.DataAnnotations.Interfaces
{
    public interface IMapperFactory
    {
        (IMappingExpression mapFrom, IMappingExpression mapTo) BuildMapper(Type targetExplicit = null);
    }
}
