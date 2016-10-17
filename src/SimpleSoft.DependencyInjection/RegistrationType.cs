namespace SimpleSoft.DependencyInjection
{
    /// <summary>
    /// The registration type
    /// </summary>
    public enum RegistrationType
    {
        /// <summary>
        /// Both interfaces and derived classes will be used
        /// </summary>
        All,
        /// <summary>
        /// Only implemented interfaces will be used
        /// </summary>
        InterfacesOnly,
        /// <summary>
        /// Only derived classes will be used
        /// </summary>
        DerivedClassesOnly
    }
}