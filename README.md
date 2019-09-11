# AutoMapper.DataAnnotations

## Create Mapping

```csharp
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

```csharp
public class DtoMappers : Profile
{
  public DtoMappers()
  {
    var (userMapFrom, userMapTo) = typeof(User).GetCustomMapper<User>(this, true);
      userMapFrom
        .ForMember(nameof(UserDataModel.IsActive), m => m.MapFrom(src => ((User)src).Active ? "Y" : "N"));
      userMapTo?
        .ForMember(nameof(User.Active), m => m.MapFrom(src => ((UserDataModel)src).IsActive == "Y"));
  }
}
```