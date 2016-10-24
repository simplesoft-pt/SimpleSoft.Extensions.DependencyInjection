namespace SimpleSoft.Extensions.DependencyInjection.Tests.ServiceMocks
{
    using System;

    public abstract class MockService : IMockService
    {
        public Guid Id { get; } = Guid.NewGuid();
    }
}