namespace SimpleSoft.DependencyInjection.Tests.ServiceMocks.ForAttribute
{
    using Microsoft.Extensions.DependencyInjection;

    [Service(ServiceLifetime.Scoped)]
    public class ScopedMockService : MockService, IScopedMockService
    {

    }
}