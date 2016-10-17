#region License
// The MIT License (MIT)
// 
// Copyright (c) 2016 João Simões
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
#endregion
namespace SimpleSoft.DependencyInjection
{
    using System;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Classes with this attribute will be loaded as services
    /// into the container
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ServiceAttribute : Attribute
    {
        /// <summary>
        /// The service lifetime
        /// </summary>
        public ServiceLifetime ServiceLifetime { get; }

        /// <summary>
        /// Try to add this service? Defaults to false.
        /// </summary>
        public bool TryAdd { get; set; }

        /// <summary>
        /// The registration type. Defaults to <see cref="RegistrationType.InterfacesOnly"/>.
        /// </summary>
        public RegistrationType Registration { get; set; } = RegistrationType.InterfacesOnly;

        /// <summary>
        /// Types to register this service. If none specified, the property
        /// <see cref="Registration"/> will be used.
        /// </summary>
        public Type[] TypesToRegister { get; set; }

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="serviceLifetime">The service lifetime</param>
        public ServiceAttribute(ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
        {
            ServiceLifetime = serviceLifetime;
        }
    }
}
