namespace SimpleSoft.DependencyInjection.Tests
{
    using System;

    public static class TestUtils
    {
        public static void UsingConfiguredServiceProvider(Action<IServiceProvider> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            
        }
    }
}
