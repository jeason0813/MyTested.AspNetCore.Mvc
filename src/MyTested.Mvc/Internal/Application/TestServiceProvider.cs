﻿namespace MyTested.Mvc.Internal.Application
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.DependencyInjection;
    using Utilities.Validators;
    using Microsoft.AspNetCore.Mvc.Internal;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Features;
    using Http;/// <summary>
               /// Provides global application services.
               /// </summary>
    public class TestServiceProvider
    {
        /// <summary>
        /// Gets the global service provider.
        /// </summary>
        /// <value>Type of IServiceProvider.</value>
        public static IServiceProvider Global => TestApplication.Services;

        /// <summary>
        /// Gets required service. Throws exception if such is not found or there are no registered services.
        /// </summary>
        /// <typeparam name="TInstance">Type of requested service.</typeparam>
        /// <returns>Instance of TInstance type.</returns>
        public static TInstance GetRequiredService<TInstance>()
        {
            ServiceValidator.ValidateServices();
            var service = Global.GetService<TInstance>();
            ServiceValidator.ValidateServiceExists(service);
            return service;
        }

        /// <summary>
        /// Gets service. Returns null if such is not found. Throws exception if there are no registered services.
        /// </summary>
        /// <typeparam name="TInstance">Type of requested service.</typeparam>
        /// <returns>Instance of TInstance type.</returns>
        public static TInstance GetService<TInstance>()
        {
            ServiceValidator.ValidateServices();
            return Global.GetService<TInstance>();
        }

        /// <summary>
        /// Gets collection of services. Returns null if no service of this type is not found. Throws exception if there are no registered services.
        /// </summary>
        /// <typeparam name="TInstance">Type of requested service.</typeparam>
        /// <returns>Instance of TInstance type.</returns>
        public static IEnumerable<TInstance> GetServices<TInstance>()
        {
            ServiceValidator.ValidateServices();
            return Global.GetServices<TInstance>();
        }

        /// <summary>
        /// Tries to get service. Returns null if such is not found or no services are registered.
        /// </summary>
        /// <typeparam name="TInstance">Type of requested service.</typeparam>
        /// <returns>Instance of TInstance type.</returns>
        public static TInstance TryGetService<TInstance>()
            where TInstance : class
        {
            return Global.GetService<TInstance>();
        }

        /// <summary>
        /// Tries to create instance of the provided type. Returns null if not successful.
        /// </summary>
        /// <typeparam name="TInstance">Type to create.</typeparam>
        /// <returns>Instance of TInstance type.</returns>
        public static TInstance TryCreateInstance<TInstance>()
            where TInstance : class
        {
            try
            {
                var typeActivatorCache = GetRequiredService<ITypeActivatorCache>();
                return typeActivatorCache.CreateInstance<TInstance>(Global, typeof(TInstance));
            }
            catch
            {
                return null;
            }
        }

        public static MockedHttpContext GetMockedHttpContext()
        {
            var httpContextFactory = GetService<IHttpContextFactory>();
            var httpContext = httpContextFactory != null
                ? MockedHttpContext.From(httpContextFactory.Create(new FeatureCollection()))
                : new MockedHttpContext();

            var httpContextAccessor = GetService<IHttpContextAccessor>();
            if (httpContextAccessor != null)
            {
                httpContextAccessor.HttpContext = httpContext;
            }

            return httpContext;
        }
    }
}
