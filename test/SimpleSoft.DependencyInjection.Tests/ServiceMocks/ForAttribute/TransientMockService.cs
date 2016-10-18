namespace SimpleSoft.DependencyInjection.Tests.ServiceMocks.ForAttribute
{
    using Microsoft.Extensions.DependencyInjection;

    [Service(ServiceLifetime.Transient)]
    public class TransientMockService : MockService, ITransientMockService
    {

    }
}