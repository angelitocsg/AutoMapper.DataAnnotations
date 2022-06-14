# AutoMapper.DataAnnotations

Nuget Package: https://www.nuget.org/packages/AC.AutoMapper.DataAnnotations/

## Register Mapping

```csharp
using AutoMapper.DataAnnotations;
using AutoMapper.DataAnnotations.Extensions;
...
public class DataModelMappers : Profile
{
  public DataModelMappers()
  {
    MapDataAnnotations.Init(AppDomain.CurrentDomain.GetAssemblies().Where(a => a.FullName.Contains("MyApp")););

    var (userMapFrom, userMapTo) = typeof(User).GetCustomMapper<User>(this, true);
        
    userMapFrom
      .ForMember(nameof(UserDataModel.IsActive), m => m.MapFrom(src => ((User)src).Active ? "Y" : "N"));
    userMapTo?
      .ForMember(nameof(User.Active), m => m.MapFrom(src => ((UserDataModel)src).IsActive == "Y"));
    ...
  }
}
```

## Create Mapping

```csharp
using AutoMapper.DataAnnotations.Attributes;
...
namespace Domain {
  public class User {
    public int Id;
    public int Name;
    public int Email;
    public bool Active;
  }
}
```

```csharp
using AutoMapper.DataAnnotations.Attributes;
...
namespace InfraData {
  [MapTargetFrom(typeof(User))]
  public class UserDTO {
    [MapFieldFrom(nameof(User.Id))]
    public int IdUser;
    [MapFieldFrom(nameof(User.Name))]
    public int UserName;
    public int Email;
    public string IsActive;
  }
}
```
