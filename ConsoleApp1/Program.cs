using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Security;
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
            dataTable.Columns.Add("Name", typeof(string));

            DataRow row1 = dataTable.NewRow();
            row1["ID"] = 1;
            row1["Name"] = "John Doe";
            dataTable.Rows.Add(row1);





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

            Console.ReadKey();
        }


    }

    public class DataTableSerializer
    {
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
                    using (var reader =new System.Xml.XmlTextReader(stream))
                    {
                        schemaTable.ReadXmlSchema(reader);
                    }
                }
                //合并数据
                if(Data != null && Data.Rows.Count > 0)
                {
                    schemaTable.Merge(Data);
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
        public static string Serialize(DataTable dataTable,bool includeSchemaAlways=false)
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
            return JsonConvert.SerializeObject(innerTable);
        }
        /// <summary>
        /// 反序列化到DataTable
        /// </summary>
        /// <param name="json">dataTable的序列化结果</param>
        /// <returns>DataTable</returns>
        public static DataTable Deserialize(string json)
        {
            InnerTable innerTable = JsonConvert.DeserializeObject<InnerTable>(json);
            return innerTable.GetData();
        }
    }
}
