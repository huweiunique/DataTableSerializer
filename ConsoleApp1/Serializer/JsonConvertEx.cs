using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace HuanSi.Lib.Serializer
{
    public static class JsonConvertEx
    {
        public static string SerializeObject(object value, JsonSerializerSettingsEx settings)
        {
            if (value is DataTable t)
                return DataTableJsonSerializer.Serialize(t, settings);
            if (value is DataTable[] tArray)
                return DataTableJsonSerializer.Serialize(tArray, settings);
            return JsonConvert.SerializeObject(value, settings);
        }
        public static object DeserializeObject(string value, Type type, JsonSerializerSettingsEx settings)
        {
            if (type == typeof(DataTable))
                return DataTableJsonSerializer.Deserialize(value, settings);
            if (type == typeof(DataTable[]))
                return DataTableJsonSerializer.DeserializeDataTables(value, settings);
            return JsonConvert.DeserializeObject(value, type, settings);
        }

        public static T DeserializeObject<T>(string value, JsonSerializerSettingsEx settings)
        {
            return (T)DeserializeObject(value, typeof(T), settings);
        }
    }
}
