using System;
using System.IO;
using System.Text.Json;

namespace Model.Data
{
    public class JsonGameSerializer : SerializationBase<HighScore[]>
    {
        public JsonGameSerializer(string fileName) : base(fileName) { }

        public override HighScore[] Deserialize(string filePath)
        {
                var json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<HighScore[]>(json);
           
        }

        public override void Serialize(HighScore[] data, string filePath)
        {
            var json = JsonSerializer.Serialize(data);
            File.WriteAllText(filePath, json);
        }
    }
}