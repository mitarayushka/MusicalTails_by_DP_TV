using System;
using System.IO;
using System.Xml.Serialization;

namespace Model.Data
{
    public class XmlGameSerializer : SerializationBase<GameData>
    {
        public XmlGameSerializer() : base("highscores.xml") { }

        public override GameData Deserialize(string filePath)
        {
            using var reader = new StreamReader(filePath);
            return (GameData)new XmlSerializer(typeof(GameData)).Deserialize(reader);
        }

        public override void Serialize(GameData data, string filePath)
        {
            // 1. Создаём сериализатор для GameData
            var serializer = new XmlSerializer(typeof(GameData));

            // 2. Открываем файл для записи (или другой поток)
            using var writer = new StreamWriter(filePath);

            // 3. Преобразуем data в XML и записываем в файл
            serializer.Serialize(writer, data);
        }
    }
}