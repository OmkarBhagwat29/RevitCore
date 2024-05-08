using System;
using System.Collections.Generic;
using System.Text.Json;

namespace RevitCore.Utils
{
    public static class JsonUtils
    {
        public static JsonSerializerOptions GetDefaultOptions() => new JsonSerializerOptions()
        {
            IgnoreNullValues = true,
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            AllowTrailingCommas = true,
            WriteIndented = true
        };

        public static T FromJsonTo<T>(string json, JsonSerializerOptions? options = null)
        {
            if (options == null)
                options = GetDefaultOptions();
            try
            {
                return JsonSerializer.Deserialize<T>(json, options);
            }
            catch (Exception ex)
            {
                throw new Exception("Deserialization failed, inner exception: " + ex.Message);
            }
        }

        public static string ToJson<T>(T obj, JsonSerializerOptions? options = null)
        {
            if (options == null)
                options = GetDefaultOptions();
            try
            {
                return JsonSerializer.Serialize(obj, options);
            }
            catch (Exception ex)
            {
                throw new Exception("Serialization failed, inner exception: " + ex.Message);
            }
        }
    }
}