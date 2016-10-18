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

            foreach (var exportedType in assembly.GetExportedTypes().Where(e => e.IsClass && !e.IsAbstract))
            {
                IServiceConfigurator serviceConfigurator;
                ServiceAttribute serviceAttribute;
                if (exportedType.TryCastAsServiceConfigurator(out serviceConfigurator))
                {
                    serviceConfigurator.Configure(services);
                }
                else if(exportedType.TryGetServiceAttribute(out serviceAttribute))
                {

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

            return services.AddServicesFrom(typeof(T).GetAssembly());
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

            return services.AddServicesFrom((IEnumerable<Assembly>) assemblies);
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

#if NET45

        [System.Runtime.CompilerServices.MethodImpl(
            System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private static Assembly GetAssembly(this Type type) {
            return type.Assembly;
        }
        
        [System.Runtime.CompilerServices.MethodImpl(
            System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private static bool TryCastAsServiceConfigurator(this Type type, out IServiceConfigurator serviceConfigurator)
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
        private static bool TryGetServiceAttribute(this Type type, out ServiceAttribute serviceAttribute)
        {
            serviceAttribute = type.GetCustomAttribute<ServiceAttribute>(true);
            return serviceAttribute != null;
        }

#else

        [System.Runtime.CompilerServices.MethodImpl(
            System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private static Assembly GetAssembly(this Type type)
        {
            return type.GetTypeInfo().Assembly;
        }

        [System.Runtime.CompilerServices.MethodImpl(
            System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private static IEnumerable<TypeInfo> GetExportedTypes(this Assembly assembly)
        {
            return assembly.ExportedTypes.Select(e => e.GetTypeInfo());
        }

        [System.Runtime.CompilerServices.MethodImpl(
            System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private static bool TryCastAsServiceConfigurator(this TypeInfo type, out IServiceConfigurator serviceConfigurator)
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
        private static bool TryGetServiceAttribute(this TypeInfo type, out ServiceAttribute serviceAttribute)
        {
            var attrData =
                type.CustomAttributes.SingleOrDefault(
                    e => ServiceAttributeType.IsAssignableFrom(e.AttributeType.GetTypeInfo()));

            serviceAttribute = null;
            return serviceAttribute != null;
        }

#endif
    }
}
