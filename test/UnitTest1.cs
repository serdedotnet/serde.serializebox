using Serde.Json;

namespace Serde.Test;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        var list = new List<string> { "a", "b", "c" };
        var box = SerializeBox.Create(list);
        var result = JsonSerializer.Serialize(box, box);
        Assert.Equal("""
        ["a","b","c"]
        """, result);

    }
}
