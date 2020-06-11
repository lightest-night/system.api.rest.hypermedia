using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;

namespace LightestNight.System.Api.Rest.Hypermedia
{
    public static class ExtendsServiceCollection
    {
        public static IServiceCollection AddHypermedia(this IServiceCollection services, string vendorName)
        {
            services.AddControllers(options => options.Filters.Add(typeof(ValidateMediaTypeAttribute)))
                .AddXmlSerializerFormatters()
                .AddNewtonsoftJson(options =>
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver());

            return services.AddOutputFormatters(vendorName);
        }

        public static IServiceCollection AddOutputFormatters(this IServiceCollection services, string vendorName)
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