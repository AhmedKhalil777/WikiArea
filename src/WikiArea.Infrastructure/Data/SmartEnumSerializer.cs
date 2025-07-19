using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson;
using Ardalis.SmartEnum;

namespace WikiArea.Infrastructure.Data;

public class SmartEnumSerializer<TEnum> : SerializerBase<TEnum>
    where TEnum : SmartEnum<TEnum>
{
    public override TEnum Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var bsonReader = context.Reader;
        var bsonType = bsonReader.GetCurrentBsonType();

        return bsonType switch
        {
            BsonType.String => SmartEnum<TEnum>.FromName(bsonReader.ReadString()),
            BsonType.Int32 => SmartEnum<TEnum>.FromValue(bsonReader.ReadInt32()),
            BsonType.Int64 => SmartEnum<TEnum>.FromValue((int)bsonReader.ReadInt64()),
            BsonType.Null => null!,
            _ => throw new FormatException($"Cannot convert a {bsonType} to a SmartEnum.")
        };
    }

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, TEnum value)
    {
        var bsonWriter = context.Writer;
        
        if (value == null)
        {
            bsonWriter.WriteNull();
        }
        else
        {
            // Serialize as string (Name) for better readability in MongoDB
            bsonWriter.WriteString(value.Name);
        }
    }
} 