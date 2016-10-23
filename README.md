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
using System.Linq;
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
			var repositories = scope.ServiceProvider.GetServices<IRepository>();
			Console.WriteLine("Total repositories: '{0}'", repositories.Count());

			var users = scope.ServiceProvider.GetRequiredService<IUserRepository>();
			Console.WriteLine(users);

			var roles = scope.ServiceProvider.GetRequiredService<IRoleRepository>();
			Console.WriteLine(roles);
		}
	}
}

public interface ILogger
{
	void Log(string message);
}

public interface IRepository { }

public interface IUserRepository : IRepository { }

public interface IRoleRepository : IRepository { }

//  Equivalent code:
//  services.TryAddSingleton<ILogger, Logger>();
[Service(TryAdd = true)]
public class Logger : ILogger
{
	public void Log(string message)
	{
		Console.WriteLine(message);
	}
}

public abstract class Repository : IRepository
{
	protected RepositoryOptions Options { get; }

	protected Repository(RepositoryOptions options)
	{
		Options = options;
	}
}

//  Equivalent code:
//  services.AddScoped<IRepository, UserRepository>();
//  services.AddScoped<IUserRepository, UserRepository>();
//  services.AddScoped<UserRepository>();
[Service(ServiceLifetime.Scoped, Registration = RegistrationType.All)]
public class UserRepository : Repository, IUserRepository
{
	public UserRepository(RepositoryOptions options, ILogger logger) : base(options)
	{
		logger.Log(string.Concat("Created new ", nameof(UserRepository)));
	}
}

//  Equivalent code:
//  services.AddScoped<IRepository, RoleRepository>();
//  services.AddScoped<IRoleRepository, RoleRepository>();
//  services.AddScoped<RoleRepository>();
[Service(ServiceLifetime.Scoped, Registration = RegistrationType.All)]
public class RoleRepository : Repository, IRoleRepository
{
	public RoleRepository(RepositoryOptions options, ILogger logger) : base(options)
	{
		logger.Log(string.Concat("Created new ", nameof(RoleRepository)));
	}
}

public class RepositoryOptions
{
	public string ConnectionString { get; set; }
}

//  Class will also be scaned Configure will be invoked
public class ServiceConfigurator : IServiceConfigurator
{
	public void Configure(IServiceCollection services)
	{
		services.AddSingleton(k => new RepositoryOptions
		{
			ConnectionString = "some connection string"
		});
	}
}
```


