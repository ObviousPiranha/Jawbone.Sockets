using System;
using System.Diagnostics.CodeAnalysis;
using Xunit.Sdk;

namespace Piranha.Sockets.Test;

public class Serializable<T> : IXunitSerializable where T : unmanaged, IParsable<T>
{
    public T Value { get; set; }

    public Serializable()
    {
    }

    public Serializable(T value) => Value = value;

    public void Deserialize(IXunitSerializationInfo info)
    {
        var text = info.GetValue<string>(nameof(Value)) ?? "";
        Value = T.Parse(text, null);
    }

    public void Serialize(IXunitSerializationInfo info)
    {
        info.AddValue(nameof(Value), Value.ToString());
    }

    public override bool Equals([NotNullWhen(true)] object? obj) => Value.Equals(obj);
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value.ToString() ?? "";

    public static implicit operator Serializable<T>(T value) => new(value);
    public static implicit operator T(Serializable<T> serializable) => serializable.Value;
}
