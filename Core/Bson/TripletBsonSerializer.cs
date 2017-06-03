using System;
using Core.Model;
using MongoDB.Bson.Serialization;

namespace Core.Bson
{
    public class TripletBsonSerializer : IBsonSerializer<Triplet>
    {
        object IBsonSerializer.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            return Deserialize(context, args);
        }

        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Triplet value)
        {
            BsonSerializer.Serialize(context.Writer, value);
        }

        public Triplet Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            return BsonSerializer.Deserialize<Triplet>(context.Reader.ReadBytes());
        }

        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
        {
            BsonSerializer.Serialize(context.Writer, typeof (Triplet), value);
        }

        public Type ValueType { get { return typeof (Triplet); } }
    }
}
