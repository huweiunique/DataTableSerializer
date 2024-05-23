using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace HuanSi.Lib.Serializer
{
    public class DataTableJsonSerializer
    {
        protected class InnerTable
        {
            public string Schema { get; set; }
            public DataTable Data { get; set; }
            public DataTable GetData()
            {

                if (string.IsNullOrWhiteSpace(Schema))
                    return Data;
                var schemaTable = new DataTable();
                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(Schema)))
                {
                    using (var reader = new System.Xml.XmlTextReader(stream))
                    {
                        schemaTable.ReadXmlSchema(reader);
                    }
                }
                //合并数据
                if (Data != null && Data.Rows.Count > 0)
                {
                    try
                    {
                        schemaTable.BeginLoadData();
                        schemaTable.Merge(Data, false, MissingSchemaAction.Ignore);
                    }
                    finally
                    {
                        schemaTable.EndLoadData();
                    }
                }
                return schemaTable;
            }
        }
        /// <summary>
        /// 序列化dataTable
        /// </summary>
        /// <param name="dataTable">要序列化的dataTable</param>
        /// <param name="includeSchemaAlways">是否总是带上schema,默认情况下有数据时，是不会带上schema的，但是当一行数据都没有时，会带上</param>
        /// <returns></returns>
        public static string Serialize(DataTable dataTable, JsonSerializerSettingsEx settings)
        {
            return JsonConvert.SerializeObject(CreateInnerTable(dataTable, settings?.AlwaysIncludeDataTableSchema == true), settings);
        }
        private static InnerTable CreateInnerTable(DataTable dataTable, bool bAlwaysIncludeTableSchema)
        {
            if (dataTable == null)
                return null;
            InnerTable innerTable = new InnerTable();
            if (bAlwaysIncludeTableSchema || dataTable.Rows.Count == 0)
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    //删除约束
                    dataTable.Constraints.Clear();

                    dataTable.WriteXmlSchema(stream);

                    innerTable.Schema = Encoding.UTF8.GetString(stream.ToArray());
                }
            }
            innerTable.Data = dataTable;
            return innerTable;
        }
        /// <summary>
        /// 序列化dataTable
        /// </summary>
        /// <param name="dataTable">要序列化的dataTable</param>
        /// <param name="includeSchemaAlways">是否总是带上schema,默认情况下有数据时，是不会带上schema的，但是当一行数据都没有时，会带上</param>
        /// <returns></returns>
        public static string Serialize(DataTable[] dataTable, JsonSerializerSettingsEx settings)
        {
            if (dataTable == null)
                return null;

            return JsonConvert.SerializeObject(dataTable.Select(t => CreateInnerTable(t, settings?.AlwaysIncludeDataTableSchema == true)).ToArray(), settings);
        }
        /// <summary>
        /// 反序列化到DataTable
        /// </summary>
        /// <param name="json">dataTable的序列化结果</param>
        /// <returns>DataTable</returns>
        public static DataTable Deserialize(string json, JsonSerializerSettingsEx settings)
        {
            InnerTable innerTable = JsonConvert.DeserializeObject<InnerTable>(json, settings);
            return innerTable.GetData();
        }
        /// <summary>
        /// 反序列化到DataTable
        /// </summary>
        /// <param name="json">dataTable的序列化结果</param>
        /// <returns>DataTable[]</returns>
        public static DataTable[] DeserializeDataTables(string json, JsonSerializerSettingsEx settings)
        {
            var data = JsonConvert.DeserializeObject<InnerTable[]>(json, settings);
            return data.Select(x => x?.GetData()).ToArray();
        }
    }
}
