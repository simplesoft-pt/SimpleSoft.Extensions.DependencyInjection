#region License
// The MIT License (MIT)
// 
// Copyright (c) 2016 João Simões
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
#endregion
namespace SimpleSoft.DependencyInjection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    /// <summary>
    /// Extension methods used to scan assemblies
    /// </summary>
    public static class ServiceCollectionExtensions
    {
#if NET45
        private static readonly Type ServiceConfiguratorType = typeof(IServiceConfigurator);
#else
        private static readonly TypeInfo ServiceConfiguratorType = typeof(IServiceConfigurator).GetTypeInfo();
        private static readonly TypeInfo ServiceAttributeType = typeof(ServiceAttribute).GetTypeInfo();
#endif

        /// <summary>
        /// Scans the given assembly for classes with <see cref="ServiceAttribute"/>
        /// or implementing <see cref="IServiceConfigurator"/> and registers everything
        /// into the given service collection.
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="assembly">The assembly to scan</param>
        /// <returns>The service collection after changes</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IServiceCollection AddServicesFrom(this IServiceCollection services, Assembly assembly)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));

            foreach (var exportedType in GetExportedTypes(assembly).Where(e => e.IsClass && !e.IsAbstract))
            {
                IServiceConfigurator serviceConfigurator;
                ServiceAttribute serviceAttribute;
                if (TryCastAsServiceConfigurator(exportedType, out serviceConfigurator))
                {
                    serviceConfigurator.Configure(services);
                }
                else if(TryGetServiceAttribute(exportedType, out serviceAttribute))
                {
                    IEnumerable<Type> typesToRegister;
                    if (serviceAttribute.TypesToRegister == null || serviceAttribute.TypesToRegister.Length == 0)
                    {
                        typesToRegister = 
                            GetServicesToRegisterBasedOnRegistration(exportedType, serviceAttribute.Registration);
                    }
                    else
                    {
                        typesToRegister = serviceAttribute.TypesToRegister;
                    }

                    if (serviceAttribute.TryAdd)
                    {
                        foreach (var type in typesToRegister)
                        {
                            TryAdd(services, type, exportedType, serviceAttribute.ServiceLifetime);
                        }
                    }
                    else
                    {
                        foreach (var type in typesToRegister)
                        {
                            Add(services, type, exportedType, serviceAttribute.ServiceLifetime);
                        }
                    }
                }
            }

            return services;
        }

        /// <summary>
        /// Scans the assembly of the given type for classes with <see cref="ServiceAttribute"/>
        /// or implementing <see cref="IServiceConfigurator"/> and registers everything
        /// into the given service collection.
        /// </summary>
        /// <typeparam name="T">The type to scan the assembly</typeparam>
        /// <param name="services">The service collection</param>
        /// <returns>The service collection after changes</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IServiceCollection AddServicesFrom<T>(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            return services.AddServicesFrom(GetAssembly(typeof(T)));
        }

        /// <summary>
        /// Scans the given assemblies for classes with <see cref="ServiceAttribute"/>
        /// or implementing <see cref="IServiceConfigurator"/> and registers everything
        /// into the given service collection.
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="assemblies">The assemblies to scan</param>
        /// <returns>The service collection after changes</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IServiceCollection AddServicesFrom(this IServiceCollection services, params Assembly[] assemblies)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (assemblies == null) throw new ArgumentNullException(nameof(assemblies));

            return assemblies.Length == 0
                ? services
                : services.AddServicesFrom((IEnumerable<Assembly>) assemblies);
        }

        /// <summary>
        /// Scans the given assemblies for classes with <see cref="ServiceAttribute"/>
        /// or implementing <see cref="IServiceConfigurator"/> and registers everything
        /// into the given service collection.
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="assemblies">The assemblies to scan</param>
        /// <returns>The service collection after changes</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IServiceCollection AddServicesFrom(this IServiceCollection services, IEnumerable<Assembly> assemblies)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (assemblies == null) throw new ArgumentNullException(nameof(assemblies));

            foreach (var assembly in assemblies)
            {
                services.AddServicesFrom(assembly);
            }

            return services;
        }

        /// <summary>
        /// Configures the service collection using the given <see cref="IServiceConfigurator"/>.
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="configurator">The service configurator</param>
        /// <returns>The service collection after changes</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IServiceCollection ConfigureUsing(this IServiceCollection services, IServiceConfigurator configurator)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configurator == null) throw new ArgumentNullException(nameof(configurator));

            configurator.Configure(services);

            return services;
        }

        /// <summary>
        /// Configures the service collection using the given <see cref="IServiceConfigurator"/> collection.
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="configurators">The service configurators</param>
        /// <returns>The service collection after changes</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IServiceCollection ConfigureUsing(
            this IServiceCollection services, params IServiceConfigurator[] configurators)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configurators == null) throw new ArgumentNullException(nameof(configurators));

            return configurators.Length == 0
                ? services
                : services.ConfigureUsing((IEnumerable<IServiceConfigurator>) configurators);
        }

        /// <summary>
        /// Configures the service collection using the given <see cref="IServiceConfigurator"/> collection.
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="configurators">The service configurators</param>
        /// <returns>The service collection after changes</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IServiceCollection ConfigureUsing(
            this IServiceCollection services, IEnumerable<IServiceConfigurator> configurators)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configurators == null) throw new ArgumentNullException(nameof(configurators));

            foreach (var configurator in configurators)
            {
                services.ConfigureUsing(configurator);
            }

            return services;
        }

        #region Private Helpers

#if NET45

        [System.Runtime.CompilerServices.MethodImpl(
            System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private static Assembly GetAssembly(Type type) {
            return type.Assembly;
        }

        [System.Runtime.CompilerServices.MethodImpl(
            System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private static IEnumerable<Type> GetExportedTypes(Assembly assembly)
        {
            return assembly.GetExportedTypes();
        }

        [System.Runtime.CompilerServices.MethodImpl(
            System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private static bool TryCastAsServiceConfigurator(Type type, out IServiceConfigurator serviceConfigurator)
        {
            if (ServiceConfiguratorType.IsAssignableFrom(type))
            {
                serviceConfigurator = (IServiceConfigurator) Activator.CreateInstance(type);
                return true;
            }

            serviceConfigurator = null;
            return false;
        }

        [System.Runtime.CompilerServices.MethodImpl(
            System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private static bool TryGetServiceAttribute(Type type, out ServiceAttribute serviceAttribute)
        {
            serviceAttribute = type.GetCustomAttribute<ServiceAttribute>(true);
            return serviceAttribute != null;
        }

        [System.Runtime.CompilerServices.MethodImpl(
            System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private static IEnumerable<Type> GetServicesToRegisterBasedOnRegistration(
            Type implementationType, RegistrationType registration)
        {
            var result = new List<Type>();

            if ((registration & RegistrationType.Interfaces) == RegistrationType.Interfaces)
            {
                result.AddRange(implementationType.GetInterfaces());
            }

            if (result.Count == 0 ||
                ((registration & RegistrationType.Self) == RegistrationType.Self))
            {
                result.Add(implementationType);
            }

            return result;
        }

        [System.Runtime.CompilerServices.MethodImpl(
            System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private static void TryAdd(IServiceCollection services, Type serviceType, Type implementationType, ServiceLifetime lifetime)
        {
            services.TryAdd(
                new ServiceDescriptor(serviceType, implementationType, lifetime));
        }

        [System.Runtime.CompilerServices.MethodImpl(
            System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private static void Add(IServiceCollection services, Type serviceType, Type implementationType, ServiceLifetime lifetime)
        {
            services.Add(
                new ServiceDescriptor(serviceType, implementationType, lifetime));
        }

#else

        [System.Runtime.CompilerServices.MethodImpl(
            System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private static Assembly GetAssembly(Type type)
        {
            return type.GetTypeInfo().Assembly;
        }

        [System.Runtime.CompilerServices.MethodImpl(
            System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private static IEnumerable<TypeInfo> GetExportedTypes(Assembly assembly)
        {
            return assembly.ExportedTypes.Select(e => e.GetTypeInfo());
        }

        [System.Runtime.CompilerServices.MethodImpl(
            System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private static bool TryCastAsServiceConfigurator(TypeInfo type, out IServiceConfigurator serviceConfigurator)
        {
            if (ServiceConfiguratorType.IsAssignableFrom(type))
            {
                serviceConfigurator = (IServiceConfigurator) Activator.CreateInstance(type.AsType());
                return true;
            }

            serviceConfigurator = null;
            return false;
        }

        [System.Runtime.CompilerServices.MethodImpl(
            System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private static bool TryGetServiceAttribute(TypeInfo type, out ServiceAttribute serviceAttribute)
        {
            var customAttributeData =
                type.CustomAttributes.SingleOrDefault(
                    e => ServiceAttributeType.IsAssignableFrom(e.AttributeType.GetTypeInfo()));
            if (customAttributeData == null)
            {
                serviceAttribute = null;
                return false;
            }

            serviceAttribute = (ServiceAttribute)
                ServiceAttributeType.DeclaredConstructors.Single().Invoke(
                    new[] {customAttributeData.ConstructorArguments[0].Value});
            foreach (var namedArgument in customAttributeData.NamedArguments)
            {
                switch (namedArgument.MemberName)
                {
                    case nameof(ServiceAttribute.Registration):
                    {
                        serviceAttribute.Registration = (RegistrationType) namedArgument.TypedValue.Value;
                        break;
                    }
                    case nameof(ServiceAttribute.TryAdd):
                    {
                        serviceAttribute.TryAdd = (bool) namedArgument.TypedValue.Value;
                        break;
                    }
                    case nameof(ServiceAttribute.TypesToRegister):
                    {
                        serviceAttribute.TypesToRegister = ((IEnumerable<CustomAttributeTypedArgument>)
                            namedArgument.TypedValue.Value).Select(e => (Type) e.Value).ToArray();
                        break;
                    }
                    default:
                        throw new InvalidOperationException(
                            $"The member '{namedArgument.MemberName}' has no assign code");
                }
            }

            return true;
        }

        [System.Runtime.CompilerServices.MethodImpl(
            System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private static IEnumerable<Type> GetServicesToRegisterBasedOnRegistration(
            TypeInfo implementationType, RegistrationType registration)
        {
            var result = new List<Type>();

            if ((registration & RegistrationType.Interfaces) == RegistrationType.Interfaces)
            {
                result.AddRange(implementationType.ImplementedInterfaces);
            }

            if (result.Count == 0 ||
                ((registration & RegistrationType.Self) == RegistrationType.Self))
            {
                result.Add(implementationType.AsType());
            }

            return result;
        }

        [System.Runtime.CompilerServices.MethodImpl(
            System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private static void TryAdd(IServiceCollection services, Type serviceType, TypeInfo implementationType, ServiceLifetime lifetime)
        {
            services.TryAdd(
                new ServiceDescriptor(serviceType, implementationType.AsType(), lifetime));
        }
        
        [System.Runtime.CompilerServices.MethodImpl(
            System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private static void Add(IServiceCollection services, Type serviceType, TypeInfo implementationType, ServiceLifetime lifetime)
        {
            services.Add(
                new ServiceDescriptor(serviceType, implementationType.AsType(), lifetime));
        }

#endif

        #endregion
    }
}
