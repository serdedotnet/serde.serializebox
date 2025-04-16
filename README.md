Serde.SerializeBox is a box that holds a value, and its `ISerialize<T>` serialize object together. It offers a replacement for APIs in other serializers that store a simple `object` field and then serialize the runtime type of the object.

Usually types have both an `ISerializeProvider<T>` and an `ISerialize<T>` implementation. However, `ISerializeProvider<T>` has static methods, so it cannot be boxed. This makes `ISerialize<T>` alone an advanced scenario. To use it correctly, you'll have to pass it as both the value and the serialize object. For example, with JsonSerializer:

```C#
var list = new List<string> { "a", "b", "c" };
var box = SerializeBox.Create(list, ListProxy.Ser<string, StringProxy>.Instance);
var result = JsonSerializer.Serialize(box, box); // pass 'box' twice
```
