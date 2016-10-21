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

		foreach (var service in provider.GetServices<IProvider>())
		{
			Console.WriteLine(service);
		}
	}
}

public interface IProvider { }

public abstract class Provider : IProvider
{
	private readonly ProviderSettings _settings;

	protected Provider(ProviderSettings settings)
	{
		_settings = settings;
	}

	public override string ToString() => string.Concat("{ Id : '", _settings.Id.ToString("D"), "' }");
}

public class ProviderSettings
{
	public Guid Id { get; set; } = Guid.NewGuid();
}

public interface IFacebookProvider : IProvider { }

public interface IGoogleProvider : IProvider { }

[Service(ServiceLifetime.Transient)]
public class GoogleProvider : Provider, IGoogleProvider
{
	public GoogleProvider(ProviderSettings settings) : base(settings)
	{

	}
}

[Service(TypesToRegister = new[] {typeof(IFacebookProvider)})]
public class FacebookProvider : Provider, IFacebookProvider
{
	public FacebookProvider(ProviderSettings settings) : base(settings)
	{

	}
}

public class ServiceConfigurator : IServiceConfigurator
{
	public void Configure(IServiceCollection services)
	{
		services.AddTransient(k => new ProviderSettings
		{
			Id = Guid.NewGuid()
		});
	}
}
```


