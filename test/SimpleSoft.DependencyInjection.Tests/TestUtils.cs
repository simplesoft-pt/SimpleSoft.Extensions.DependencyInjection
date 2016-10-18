namespace SimpleSoft.DependencyInjection.Tests
{
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using ServiceMocks;

    public static class TestUtils
    {
        public static void UsingConfiguredServiceProvider(Action<IServiceProvider> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            var services = new ServiceCollection().AddServicesFrom<IMockService>();
            action(services.BuildServiceProvider());
        }
    }
}
