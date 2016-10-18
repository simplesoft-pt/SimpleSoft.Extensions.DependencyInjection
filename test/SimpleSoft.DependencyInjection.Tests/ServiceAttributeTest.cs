namespace SimpleSoft.DependencyInjection.Tests
{
    using Microsoft.Extensions.DependencyInjection;
    using ServiceMocks.ForAttribute;
    using Xunit;

    public class ServiceAttributeTest
    {
        #region ServiceLifetime.Singleton

        [Fact]
        public void GivenAServiceScopeWhenResolvingSingletonServiceAddedByAttributeThenNoExceptionIsThrown()
        {
            TestUtils.UsingConfiguredServiceProviderScope(k =>
            {
                k.ServiceProvider.GetRequiredService<ISingletonMockService>();
            });
        }

        [Fact]
        public void GivenAServiceScopeWhenResolvingSingletonServiceAddedByAttributeThenSameInstanceIsObtained()
        {
            TestUtils.UsingConfiguredServiceProviderScope(scope =>
            {
                var instance01 = scope.ServiceProvider.GetRequiredService<ISingletonMockService>();
                var instance02 = scope.ServiceProvider.GetRequiredService<ISingletonMockService>();

                Assert.Same(instance01, instance02);
                Assert.Equal(instance01.Id, instance02.Id);
            });
        }

        [Fact]
        public void GivenMultipleServiceScopesWhenResolvingSingletonServiceAddedByAttributeThenTheSameInstanceIsObtained()
        {
            var instance01 =
                TestUtils.UsingConfiguredServiceProviderScope(
                    scope => scope.ServiceProvider.GetRequiredService<ISingletonMockService>());

            var instance02 =
                TestUtils.UsingConfiguredServiceProviderScope(
                    scope => scope.ServiceProvider.GetRequiredService<ISingletonMockService>());

            Assert.Same(instance01, instance02);
            Assert.Equal(instance01.Id, instance02.Id);
        }

        #endregion

        #region ServiceLifetime.Scoped

        [Fact]
        public void GivenAServiceScopeWhenResolvingScopedServiceAddedByAttributeThenNoExceptionIsThrown()
        {
            TestUtils.UsingConfiguredServiceProviderScope(k =>
            {
                k.ServiceProvider.GetRequiredService<IScopedMockService>();
            });
        }

        [Fact]
        public void GivenAServiceScopeWhenResolvingScopedServiceAddedByAttributeThenSameInstanceIsObtained()
        {
            TestUtils.UsingConfiguredServiceProviderScope(scope =>
            {
                var instance01 = scope.ServiceProvider.GetRequiredService<IScopedMockService>();
                var instance02 = scope.ServiceProvider.GetRequiredService<IScopedMockService>();

                Assert.Same(instance01, instance02);
                Assert.Equal(instance01.Id, instance02.Id);
            });
        }

        [Fact]
        public void GivenMultipleServiceScopesWhenResolvingScopedServiceAddedByAttributeThenDifferentInstancesAreObtained()
        {
            var instance01 =
                TestUtils.UsingConfiguredServiceProviderScope(
                    scope => scope.ServiceProvider.GetRequiredService<IScopedMockService>());

            var instance02 =
                TestUtils.UsingConfiguredServiceProviderScope(
                    scope => scope.ServiceProvider.GetRequiredService<IScopedMockService>());

            Assert.NotSame(instance01, instance02);
            Assert.NotEqual(instance01.Id, instance02.Id);
        }

        #endregion

        #region ServiceLifetime.Transient

        [Fact]
        public void GivenAServiceScopeWhenResolvingTransientServiceAddedByAttributeThenNoExceptionIsThrown()
        {
            TestUtils.UsingConfiguredServiceProviderScope(k =>
            {
                k.ServiceProvider.GetRequiredService<ITransientMockService>();
            });
        }

        [Fact]
        public void GivenAServiceScopeWhenResolvingTransientServiceAddedByAttributeThenDifferentInstancesAreObtained()
        {
            TestUtils.UsingConfiguredServiceProviderScope(scope =>
            {
                var instance01 = scope.ServiceProvider.GetRequiredService<ITransientMockService>();
                var instance02 = scope.ServiceProvider.GetRequiredService<ITransientMockService>();

                Assert.NotSame(instance01, instance02);
                Assert.NotEqual(instance01.Id, instance02.Id);
            });
        }

        [Fact]
        public void GivenMultipleServiceScopesWhenResolvingTransientServiceAddedByAttributeThenDifferentInstancesAreObtained()
        {
            var instance01 =
                TestUtils.UsingConfiguredServiceProviderScope(
                    scope => scope.ServiceProvider.GetRequiredService<ITransientMockService>());

            var instance02 =
                TestUtils.UsingConfiguredServiceProviderScope(
                    scope => scope.ServiceProvider.GetRequiredService<ITransientMockService>());

            Assert.NotSame(instance01, instance02);
            Assert.NotEqual(instance01.Id, instance02.Id);
        }

        #endregion
    }
}
