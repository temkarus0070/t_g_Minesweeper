using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace t_g_Minesweeper.WebApi.services
{
    public class MatrixConverter : JsonConverter<string[,]>
    {
        public override string[,]? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
           
            var arr=JsonSerializer.Deserialize<string[][]>(JsonDocument.ParseValue(ref reader));
            string[,]res= new string[arr.Length, arr[0].Length];
            for (int i = 0; i < res.GetLength(0); i++)
            {
                for (int j = 0; j < res.GetLength(1); j++)
                {

                    res[i,j]= arr[i][j];
                }
            }
            return res;
        }

        public override void Write(Utf8JsonWriter writer, string[,] value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            for (int i = 0; i < value.GetLength(0); i++)
            {
                writer.WriteStartArray();
                for (int j = 0; j < value.GetLength(1); j++)
                {

                    writer.WriteStringValue(value[i, j]);
                }
                writer.WriteEndArray();
            }
            writer.WriteEndArray();
        }
    }
}
