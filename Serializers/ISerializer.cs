using System;

namespace Photofy.Caching.Serializers
{
    public interface ISerializer<TType> where TType : class
    {
        TType Deserialize(byte[] value);

        byte[] Serialize(TType value);
    }
}
