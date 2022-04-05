using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Mnk.Library.Common.Log;

namespace Mnk.Library.Common.SaveLoad
{
    sealed class UnixEpochDateTimeConverter : JsonConverter<DateTime>
    {
        static readonly DateTime epoch = new DateTime(1970, 1, 1);
        static readonly Regex regex = new Regex("^/Date\\(([+-]*\\d+)([+-])(\\d{2})(\\d{2})\\)/$", RegexOptions.CultureInvariant);

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            var match = regex.Match(value);

            if (!match.Success)
            {
                throw new JsonException($"Invalid date time format: '{value}'");
            }
            if (!long.TryParse(match.Groups[1].Value, System.Globalization.NumberStyles.Integer, CultureInfo.InvariantCulture, out long unixTime))
            {
                throw new JsonException($"Can't parse milliseconds: '{match.Groups[1].Value}'");
            }
            if (!int.TryParse(match.Groups[3].Value, System.Globalization.NumberStyles.Integer, CultureInfo.InvariantCulture, out int hours))
            {
                throw new JsonException($"Can't parse hours: '{match.Groups[3].Value}'");
            }
            if (!int.TryParse(match.Groups[4].Value, System.Globalization.NumberStyles.Integer, CultureInfo.InvariantCulture, out int minutes))
            {
                throw new JsonException($"Can't parse minutes: '{match.Groups[4].Value}'");
            }

            int sign = match.Groups[2].Value[0] == '-' ? -1 : 1;

            return epoch + TimeSpan.FromMilliseconds(unixTime) + new TimeSpan(hours * sign, minutes * sign, 0);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            long unixTime = Convert.ToInt64((value - epoch).TotalMilliseconds);
            var utcOffset = new TimeSpan(value.Ticks);

            string formatted = FormattableString.Invariant($"/Date({unixTime}{(utcOffset >= TimeSpan.Zero ? "+" : "-")}{utcOffset:hhmm})/");
            writer.WriteStringValue(formatted);
        }
    }

    public sealed class AnySerializer
    {
        private static readonly ILog Log = LogManager.GetLogger<AnySerializer>();
        private readonly string configPath;
        private readonly Type type;
        private readonly JsonSerializerOptions options;

        public AnySerializer(string configPath, Type type)
        {
            this.configPath = Path.GetFullPath(configPath);
            this.type = type;
            this.options = new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter(), new UnixEpochDateTimeConverter() },

            };
        }

        public object Load(object defValue = null)
        {
            if (File.Exists(configPath))
            {
                try
                {
                    using (var s = File.Open(configPath, FileMode.Open))
                    {
                        return JsonSerializer.Deserialize(s, type, options) ?? defValue;
                    }
                }
                catch (Exception ex)
                {
                    Log.Write(ex, "Can't load configuration: " + configPath);
                }
            }
            else
            {
                if (defValue != null) Save(defValue);
            }
            return defValue;
        }

        public bool Save(object data)
        {
            try
            {
                using (var s = File.Open(configPath, FileMode.Create))
                {
                    JsonSerializer.Serialize(s, data, type, options);
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Write(ex, "Can't save configuration: " + configPath);
                return false;
            }
        }
    }
}
