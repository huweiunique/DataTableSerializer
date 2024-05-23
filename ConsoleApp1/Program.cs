using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Runtime;
using System.Runtime.Serialization.Json;
using System.Text;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // 创建一个DataTable
            DataTable dataTable = new DataTable("MyTable");
            dataTable.Columns.Add("ID", typeof(int));

            dataTable.Columns.Add("gData", typeof(byte[]));
            dataTable.Columns.Add("Name", typeof(string));

            DataRow row1 = dataTable.NewRow();
            row1["ID"] = DBNull.Value;
            row1["Name"] = DBNull.Value;
            row1["gData"] = DBNull.Value;
            dataTable.Rows.Add(row1);


            DataRow row2 = dataTable.NewRow();
            row1["ID"] = DBNull.Value;
            row1["Name"] = DBNull.Value;
            row1["gData"] = DBNull.Value;
            dataTable.Rows.Add(row2);



            // 使用DataContractJsonSerializer序列化DataTable
            var json = DataTableSerializer.Serialize(dataTable);
            Console.WriteLine(json);
            var table = DataTableSerializer.Deserialize(json);
            Console.WriteLine($"有{table.Rows.Count}行数据,{table.Columns.Count}列");

            dataTable.Rows.Clear();
            json = DataTableSerializer.Serialize(dataTable);
            Console.WriteLine(json);
            table = DataTableSerializer.Deserialize(json);
            Console.WriteLine($"有{table.Rows.Count}行数据,{table.Columns.Count}列");

            row1 = table.NewRow();
            row1["ID"] = 1;
            row1["Name"] = "John Doe";
            row1["gData"] = new byte[] { 10, 20, 30 };
            table.Rows.Add(row1);


             row2 = table.NewRow();
            row1["ID"] = 2;
            row1["Name"] = "huwei";
            row1["gData"] = new byte[] { 30, 20, 30 };
            table.Rows.Add(row2);
            json = DataTableSerializer.Serialize(table);
            Console.WriteLine(json);
            var newtable = DataTableSerializer.Deserialize(json);
            Console.WriteLine($"有{newtable.Rows.Count}行数据,{newtable.Columns.Count}列");
            Console.ReadKey();
        }


    }

    public class DataTableSerializer
    {
        static DataTableSerializer()
        {
            settings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All };
            settings.Converters.Insert(0, new ConsoleApp1.DataTableConverter());
        }
        public static JsonSerializerSettings settings;
        private class InnerTable
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
                    schemaTable.Merge(Data,false,MissingSchemaAction.Ignore);
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
        public static string Serialize(DataTable dataTable, bool includeSchemaAlways = true)
        {
            InnerTable innerTable = new InnerTable();
            if (includeSchemaAlways || dataTable.Rows.Count == 0)
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
            return JsonConvert.SerializeObject(innerTable, settings);
        }
        /// <summary>
        /// 反序列化到DataTable
        /// </summary>
        /// <param name="json">dataTable的序列化结果</param>
        /// <returns>DataTable</returns>
        public static DataTable Deserialize(string json)
        {
            InnerTable innerTable = JsonConvert.DeserializeObject<InnerTable>(json, settings);
            return innerTable.GetData();
        }
    }
}
