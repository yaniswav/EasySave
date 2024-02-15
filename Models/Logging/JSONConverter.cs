using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
namespace EasySave;

public class LoggerConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return (objectType == typeof(LoggingModel));
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        JObject jo = JObject.Load(reader);

        // Déterminez ici quel type de Logger créer
        // Par exemple, en fonction d'une propriété spécifique dans le JSON
        if (jo["Type"].Value<string>() == "JsonLogger")
        {
            return jo.ToObject<JsonLogger>(serializer);
        }
        else if (jo["Type"].Value<string>() == "XmlLogger")
        {
            return jo.ToObject<XmlLogger>(serializer);
        }
        return null;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}