using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuanSi.Lib.Serializer
{
    public class JsonSerializerSettingsEx : JsonSerializerSettings
    {
        public JsonSerializerSettingsEx()
        {
            this.TypeNameHandling = TypeNameHandling.All;
            this.NullValueHandling = NullValueHandling.Include;
            this.Converters.Insert(0, DataTableConverter.Instance);
        }
        /// <summary>
        /// 对于DataTable类型,总是包含schema
        /// </summary>
        public bool AlwaysIncludeDataTableSchema { get; set; } = true;
    }
}
