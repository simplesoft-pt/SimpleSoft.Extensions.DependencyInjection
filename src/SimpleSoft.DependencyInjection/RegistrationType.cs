namespace SimpleSoft.DependencyInjection
{
    using System;

    /// <summary>
    /// The registration type
    /// </summary>
    [Flags]
    public enum RegistrationType
    {
        /// <summary>
        /// Only implemented interfaces will be used
        /// </summary>
        Interfaces = 1 << 0,

        /// <summary>
        /// Only derived classes will be used
        /// </summary>
        Derived = 1 << 1,

        /// <summary>
        /// The service will only be registered as itself
        /// </summary>
        Self = 1 << 2,

        /// <summary>
        /// Both implemented interfaces and derived classes will be used
        /// </summary>
        InterfacesAndDerived = Interfaces | Derived,

        /// <summary>
        /// Interfaces, derived and self will be used
        /// </summary>
        All = Interfaces | Derived | Self
    }
}