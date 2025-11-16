using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace Синхронизация
{
    internal class Properties
    {
        private static JsonSerializerOptions jsonDeserializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        private static JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        public List<Property> propertiesList { get; set; }
        public string notCopiedKeys { get; set; }
        private FileHandler fileHandler;

        public Properties()
        {
            propertiesList = new List<Property>();
            notCopiedKeys = ".$";
        }
        public Properties(string pathToConfig)
        {
            fileHandler = new FileHandler(pathToConfig);

            if (fileHandler.Exist())
            {
                Properties temp = GetFromJSON(fileHandler.Read());

                propertiesList = temp.propertiesList;
                notCopiedKeys = temp.notCopiedKeys;
            } else
            {
                propertiesList = new List<Property>();
                notCopiedKeys = ".$";

                fileHandler.Update(GetJSON());
            }
        }
        private static Properties GetFromJSON(string json)
        {
            Properties? temp = JsonSerializer.Deserialize<Properties>(json, jsonDeserializerOptions);

            if (temp == null)
                throw new Exception("Ошибка десериализации");
            else
                return temp;
        }

        public void AddProperty(Property property)
        {
            propertiesList.Add(property);
            fileHandler.Update(GetJSON());
        }
        public void DeleteProperty(int index)
        {
            propertiesList.RemoveAt(index);
            fileHandler.Update(GetJSON());
        }

        private string GetJSON()
        {
            return JsonSerializer.Serialize<Properties>(this, jsonSerializerOptions);
        }
    }

    internal class Property
    {
        public string name { get; set; }
        public string description { get; set; }
        public string pathSourse { get; set; }
        public string pathCopy { get; set; }
        public string fleshName { get; set; }

        public Property()
        {
            
        }
        public Property(string name, string description, string pathSourse, string pathCopy, string fleshName)
        {
            this.name = name;
            this.description = description;
            this.pathSourse = pathSourse;
            this.pathCopy = pathCopy;
            this.fleshName = fleshName;
        }
    }
}
