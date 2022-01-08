﻿using Nethereum.Hex.HexTypes;
using Newtonsoft.Json;
using System;
using System.Numerics;

namespace AtomicCore.BlockChain.BscscanAPI
{
    /// <summary>
    /// Hex Long Json Converter
    /// </summary>
    public class BscHexLongJsonConverter : JsonConverter
    {
        /// <summary>
        /// CanConvert
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public override bool CanConvert(Type objectType) => true;

        /// <summary>
        /// ReadJson
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="objectType"></param>
        /// <param name="existingValue"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null)
                return null;

            string hex = (string)reader.Value;
            if (hex.StartsWith("0x", StringComparison.Ordinal))
                hex = hex.Substring(2);

            long val;
            try
            {
                val = Convert.ToInt64(hex, 16);
            }
            catch (Exception ex)
            {
                throw new Exception($"BscHexLongJsonConverter --> {hex} can not convert to long! error msg --> {ex.Message}");
            }

            if (long.TryParse((string)reader.Value, System.Globalization.NumberStyles.HexNumber, null, out long value))
                return value;

            return val;
        }

        /// <summary>
        /// WriteJson
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        /// <exception cref="TypeAccessException"></exception>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is long long_val)
                writer.WriteValue(new HexBigInteger(new BigInteger(long_val)).HexValue);

            throw new TypeAccessException(nameof(value));
        }
    }
}
