using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using LMS.Shared;

namespace LMS.Web.Data
{
    public enum CultureConvertDirection
    {
        /// <summary>
        /// zh-CN => zh-Hans-CN
        /// </summary>
        ToInternal,

        /// <summary>
        /// zh-Hans-CN => zh-CN
        /// </summary>
        ToExternal
    }

    public static class Extensions
    {
        public static HttpClient CreateAnonymousClient(this IHttpClientFactory httpClientFactory) =>
            httpClientFactory.CreateClient($"{Config.API.Name}.Anonymous");

        public static HttpClient CreateAuthorizeClient(this IHttpClientFactory httpClientFactory) =>
            httpClientFactory.CreateClient($"{Config.API.Name}.Authorize");

        public static CultureInfo ConvertChineseCulture(this CultureInfo culture, CultureConvertDirection direction)
        {
            var name = culture.Name.ToLowerInvariant();
            return direction switch
            {
                CultureConvertDirection.ToInternal => name switch
                {
                    "zh-cn" => CultureInfo.GetCultureInfo("zh-Hans-CN"),
                    "zh-tw" => CultureInfo.GetCultureInfo("zh-Hant-TW"),
                    _ => culture
                },

                CultureConvertDirection.ToExternal => name switch
                {
                    "zh-hans-cn" => CultureInfo.GetCultureInfo("zh-CN"),
                    "zh-hant-tw" => CultureInfo.GetCultureInfo("zh-TW"),
                    _ => culture
                },

                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
            };
        }

        public static string DisplayNameFor<TModel>(this TModel _, string propertyName) =>
            typeof(TModel).GetProperty(propertyName)?.GetCustomAttribute<DisplayAttribute>()?.Name ?? "";
        
        public static string DisplayName(this Enum value) =>
            value.GetType().GetMember(value.ToString()).First().GetCustomAttribute<DisplayAttribute>()?.Name ?? "";

        public static T JsonClone<T>(this T obj) => JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(obj));
    }
}
