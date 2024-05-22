using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public static class JsonReaderExtend
    {
        public static void ReadAndAssert(this JsonReader jsonReader)
        {
            if (!jsonReader.Read())
            {
                throw new JsonSerializationException( "Unexpected end when reading JSON.");
            }
        }
    }
}
