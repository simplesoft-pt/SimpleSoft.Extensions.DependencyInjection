# SimpleSoft - DependencyInjection
Library that helps to register services into the `IServiceCollection`, Microsoft's dependency injection facade, by enabling the developer to scan assemblies for service attributes or configurators.

## Reasons
Some of the reasons to use this little library:

1. Enables assembly scan;
2. Use `IServiceConfigurator` to aggregate related registrations;

## Installation 
This library can be installed via [NuGet](https://www.nuget.org/packages/SimpleSoft.DependencyInjection/) package. Just run the following command:

```powershell
Install-Package SimpleSoft.DependencyInjection -Pre
```

## Compatibility

This library is compatible with the folowing frameworks:

* .NET Framework 4.5
* .NET Standard 1.0

## Tipical usage
```csharp
using System;
using Microsoft.Extensions.DependencyInjection;
using SimpleSoft.DependencyInjection;

public class Program
{
	public static void Main(string[] args)
	{
		var provider =
			new ServiceCollection()
				.AddServicesFrom<Program>()
				.BuildServiceProvider();

		using (var scope = provider.GetRequiredService<IServiceScopeFactory>().CreateScope())
		{
			var users = scope.ServiceProvider.GetRequiredService<IUserRepository>();
			Console.WriteLine(users);

			var roles = scope.ServiceProvider.GetRequiredService<IRoleRepository>();
			Console.WriteLine(roles);
		}
	}
}

public interface IRepository { }

public interface IUserRepository : IRepository { }

public interface IRoleRepository : IRepository { }

public abstract class Repository : IRepository
{
	protected RepositoryOptions Options { get; }

	protected Repository(RepositoryOptions options)
	{
		Options = options;
	}
}

[Service(ServiceLifetime.Scoped, TypesToRegister = new[] {typeof(IUserRepository)})]
public class UserRepository : Repository, IUserRepository
{
	public UserRepository(RepositoryOptions options) : base(options)
	{

	}

	public override string ToString()
	{
		return string.Concat("Repository -> ", nameof(UserRepository));
	}
}

[Service(ServiceLifetime.Scoped, TypesToRegister = new[] { typeof(IRoleRepository) })]
public class RoleRepository : Repository, IRoleRepository
{
	public RoleRepository(RepositoryOptions options) : base(options)
	{

	}

	public override string ToString()
	{
		return string.Concat("Repository -> ", nameof(RoleRepository));
	}
}

public class RepositoryOptions
{
	public string ConnectionString { get; set; }
}

public class ServiceConfigurator : IServiceConfigurator
{
	public void Configure(IServiceCollection services)
	{
		services.AddSingleton(k => new RepositoryOptions
		{
			ConnectionString = "<some connection string>"
		});
	}
}
```


