using Newtonsoft.Json;

namespace HuanSi.Lib.Serializer
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
