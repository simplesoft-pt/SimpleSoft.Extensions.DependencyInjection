namespace SimpleSoft.Extensions.DependencyInjection.Tests
{
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using ServiceMocks;

    public static class TestUtils
    {
        private static readonly IServiceProvider ServiceProvider;

        static TestUtils()
        {
            var services = new ServiceCollection().AddServicesFrom<IMockService>();
            ServiceProvider = services.BuildServiceProvider();
        }

        public static void UsingConfiguredServiceProvider(Action<IServiceProvider> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            
            action(ServiceProvider);
        }

        public static TResult UsingConfiguredServiceProvider<TResult>(Func<IServiceProvider, TResult> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            
            return action(ServiceProvider);
        }

        public static void UsingConfiguredServiceProviderScope(Action<IServiceScope> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            
            using (var scope = ServiceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                action(scope);
            }
        }

        public static TResult UsingConfiguredServiceProviderScope<TResult>(Func<IServiceScope, TResult> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            
            using (var scope = ServiceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                return action(scope);
            }
        }
    }
}
