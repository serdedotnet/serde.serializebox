namespace Serde;

/// <summary>
/// This is a box that holds a value, and the ISerialize<T> object that's used to serialize it.
/// Using this box, you can hold onto a value and its serialize implementation in a single object.
///
/// This is an advanced scenario: the source generator will not generate code that uses <see
/// cref="ISerialize{T}"/>  objects directly.
/// </summary>
public sealed partial class SerializeBox : ISerialize<SerializeBox>
{
    private readonly ISerialize _box;

    private SerializeBox(ISerialize box) { _box = box; }

    /// <summary>
    /// We need a non-generic interface to serialize a value without exposing the generic type.
    /// </summary>
    private interface ISerialize : ISerdeInfoProvider
    {
        void Serialize(ISerializer serializer);
    }

    /// <summary>
    /// We need a generic type to hold the value and its serializer, but we don't want to expose
    /// the generic type to the outside world, since it would require every user to know the type
    /// and carry it around as a generic parameter.
    /// </summary>
    private sealed class Box<T>(T value, ISerialize<T> ser) : ISerialize
    {
        public ISerdeInfo SerdeInfo => ser.SerdeInfo;
        public void Serialize(ISerializer serializer) => ser.Serialize(value, serializer);
    }

    public static SerializeBox Create<T>(T value, ISerialize<T> ser) => new SerializeBox(new Box<T>(value, ser));
    public static SerializeBox Create<T>(T value) where T : ISerializeProvider<T> => new SerializeBox(new Box<T>(value, T.Instance));
    public static SerializeBox Create<T, TProvider>(T value)
        where TProvider : ISerializeProvider<T>
        => new SerializeBox(new Box<T>(value, TProvider.Instance));

    public ISerdeInfo SerdeInfo => _box.SerdeInfo;

    public void Serialize(SerializeBox value, ISerializer serializer)
    {
        value._box.Serialize(serializer);
    }
}

// Helper creation functions
public sealed partial class SerializeBox
{
    public static SerializeBox Create(List<string> list)
    {
        return Create(list,
            ListProxy.Ser<string, StringProxy>.Instance);
    }

    public static SerializeBox Create<T>(List<T> list)
        where T : ISerializeProvider<T>
    {
        return Create(list, ListProxy.Ser<T, T>.Instance);
    }
}