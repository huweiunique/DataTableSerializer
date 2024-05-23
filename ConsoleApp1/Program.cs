using HuanSi.Lib.Serializer;
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

            var settings = new JsonSerializerSettingsEx();

            // 使用DataContractJsonSerializer序列化DataTable
            var json = DataTableJsonSerializer.Serialize(dataTable, settings);
            Console.WriteLine(json);
            var table = DataTableJsonSerializer.Deserialize(json, settings);
            Console.WriteLine($"有{table.Rows.Count}行数据,{table.Columns.Count}列");

            dataTable.Rows.Clear();
            json = DataTableJsonSerializer.Serialize(dataTable, settings);
            Console.WriteLine(json);
            table = DataTableJsonSerializer.Deserialize(json, settings);
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
            json = DataTableJsonSerializer.Serialize(table, settings);
            Console.WriteLine(json);
            var newtable = DataTableJsonSerializer.Deserialize(json, settings);
            Console.WriteLine($"有{newtable.Rows.Count}行数据,{newtable.Columns.Count}列");
            Console.ReadKey();
        }


    }
}
