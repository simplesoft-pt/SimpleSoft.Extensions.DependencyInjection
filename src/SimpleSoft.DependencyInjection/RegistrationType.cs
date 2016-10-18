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
        /// The service will only be registered as itself
        /// </summary>
        Self = 1 << 1,

        /// <summary>
        /// Interfaces, derived and self will be used
        /// </summary>
        All = Interfaces | Self
    }
}