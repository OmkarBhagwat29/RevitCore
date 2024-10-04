using System;
using System.Collections.Generic;
using System.Reflection;


#if !NET48
using System.Text.Json;
#else
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
#endif

namespace RevitCore.Utils
{
    public static class JsonUtils
    {
#if !NET48
        public static JsonSerializerOptions GetDefaultOptions() => new JsonSerializerOptions()
        {
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            AllowTrailingCommas = true,
            WriteIndented = true
        };

        public static T FromJsonTo<T>(string json, JsonSerializerOptions options = null)
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

        public static string ToJson<T>(T obj, JsonSerializerOptions options = null)
        {
            options ??= GetDefaultOptions();
            try
            {
                return JsonSerializer.Serialize(obj, options);
            }
            catch (Exception ex)
            {
                throw new Exception("Serialization failed, inner exception: " + ex.Message);
            }
        }

#else
        public static JsonSerializerSettings GetDefaultSettings() => new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            CheckAdditionalContent = true,
            Formatting = Formatting.Indented,
            ContractResolver = (IContractResolver)new PrivateSetterContractResolver(),
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            Converters = (IList<JsonConverter>)new List<JsonConverter>()
            {
                (JsonConverter) new StringEnumConverter()
            }
        };

        public static T FromJsonTo<T>(string json, JsonSerializerSettings? settings = null) => (T)FromJsonTo(json, typeof(T), settings);

        public static object FromJsonTo(string json, Type type, JsonSerializerSettings? settings = null)
        {
            if (settings == null)
                settings = GetDefaultSettings();
            try
            {
                return JsonConvert.DeserializeObject(json, type, settings);
            }
            catch (Exception ex)
            {
                throw new Exception("Deserialisation failed, inner exception : " + ex.Message);
            }
        }

        public static string ToJson(object obj, JsonSerializerSettings? settings = null)
        {
            if (settings == null)
                settings = GetDefaultSettings();
            try
            {
                return JsonConvert.SerializeObject(obj, settings);
            }
            catch (Exception ex)
            {
                throw new Exception("Serialisation failed, inner exception : " + ex.Message);
            }
        }

        class PrivateSetterContractResolver : DefaultContractResolver
        {
            public PrivateSetterContractResolver(NamingStrategy namingStrategy) => this.NamingStrategy = namingStrategy != null ? namingStrategy : throw new ArgumentNullException(nameof(namingStrategy));

            public PrivateSetterContractResolver() => this.NamingStrategy = (NamingStrategy)new CamelCaseNamingStrategy();

            protected override JsonProperty CreateProperty(
                MemberInfo member,
                MemberSerialization memberSerialization)
            {
                JsonProperty property = base.CreateProperty(member, memberSerialization);
                if (property.Writable)
                    return property;
                property.Writable = this.IsPropertyWithSetter(member);
                return property;
            }

            private bool IsPropertyWithSetter(MemberInfo member)
            {
                PropertyInfo propertyInfo = member as PropertyInfo;
                return (object)propertyInfo != null && propertyInfo.SetMethod != (MethodInfo)null;
            }
        }
#endif
    }
}