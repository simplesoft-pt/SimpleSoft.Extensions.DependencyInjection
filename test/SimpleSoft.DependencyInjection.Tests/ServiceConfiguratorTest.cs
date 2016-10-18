namespace SimpleSoft.DependencyInjection.Tests
{
    using System.Linq;
    using Microsoft.Extensions.DependencyInjection;
    using ServiceMocks.ForConfigurator;
    using Xunit;

    public class ServiceConfiguratorTest
    {
        [Fact]
        public void GivenAConfiguredServiceProviderWhenResolvingServiceAddedByConfiguratorsNoExceptionIsThrown()
        {
            TestUtils.UsingConfiguredServiceProvider(k =>
            {
                k.GetRequiredService<IMockServiceConfiguratorService>();
            });
        }

        [Fact]
        public void GivenAConfiguredServiceProviderWhenResolvingServicesAddedByConfiguratorsAllMustBeReturned()
        {
            TestUtils.UsingConfiguredServiceProvider(k =>
            {
                var services = k.GetServices<IMockServiceConfiguratorService>().ToArray();
                Assert.Equal(2, services.Length);
            });
        }
    }
}
