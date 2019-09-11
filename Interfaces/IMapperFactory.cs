using System;

namespace AutoMapper.DataAnnotations.Interfaces
{
    public interface IMapperFactory<T1>
    {
        (IMappingExpression mapFrom, IMappingExpression mapTo) BuildMapper(Type targetExplicit = null);
    }
}
