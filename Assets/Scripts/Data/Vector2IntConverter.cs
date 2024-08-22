using System;
using Newtonsoft.Json;
using UnityEngine;

public class Vector2IntConverter : JsonConverter<Vector2Int>
{
    public override void WriteJson(JsonWriter writer, Vector2Int value, JsonSerializer serializer)
    {
        writer.WriteValue($"{value.x},{value.y}");
    }

    public override Vector2Int ReadJson(JsonReader reader, Type objectType, Vector2Int existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        string[] values = reader.Value.ToString().Split(',');
        return new Vector2Int(int.Parse(values[0]), int.Parse(values[1]));
    }
}
