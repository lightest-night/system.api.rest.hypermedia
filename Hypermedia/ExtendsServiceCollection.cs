using System;
using System.Linq;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;

namespace LightestNight.System.Api.Rest.Hypermedia
{
    /// <summary>
    /// Adds the necessary services to the standard IoC container to include Hypermedia in your Web Api
    /// </summary>
    public static class ExtendsServiceCollection
    {
        /// <summary>
        /// Adds the necessary services to the DI container to add Hypermedia links to resources
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to register into</param>
        /// <param name="vendorName">The custom vendor name to accept</param>
        /// <param name="includeFluentValidation">Whether or not to add Fluent Validation to the Mvc Pipeline</param>
        /// <param name="optionsAccessor">An action to generate an <see cref="MvcOptions" /> object to configure Mvc with</param>
        /// <returns>A populated <see cref="IServiceCollection" /></returns>
        public static IServiceCollection AddHypermedia(this IServiceCollection services, string vendorName,
            bool includeFluentValidation = true, Action<MvcOptions>? optionsAccessor = null)
        {
            services.AddControllers(options =>
                {
                    optionsAccessor?.Invoke(options);
                    options.Filters.Add(typeof(ValidateMediaTypeAttribute));
                }).AddNewtonsoftJson(options =>
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver())
                .AddXmlSerializerFormatters()
                .AddFluentValidation();

            return services.AddOutputFormatters(vendorName);
        }

        private static IServiceCollection AddOutputFormatters(this IServiceCollection services, string vendorName)
            => services.Configure<MvcOptions>(options =>
            {
                var newtonsoftJsonOutputFormatter =
                    options.OutputFormatters.OfType<NewtonsoftJsonOutputFormatter>().FirstOrDefault();
                newtonsoftJsonOutputFormatter?.SupportedMediaTypes.Add(
                    $"application/vnd.{vendorName.ToLowerInvariant()}.{Constants.HateoasIdentifier}+json");

                var xmlOutputFormatter = options.OutputFormatters.OfType<XmlDataContractSerializerOutputFormatter>()
                    .FirstOrDefault();
                xmlOutputFormatter?.SupportedMediaTypes.Add(
                    $"application/vnd.{vendorName.ToLowerInvariant()}.{Constants.HateoasIdentifier}+xml");
            });
    }
}