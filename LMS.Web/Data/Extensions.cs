using System;
using System.Globalization;
using System.Net.Http;
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
    }
}
