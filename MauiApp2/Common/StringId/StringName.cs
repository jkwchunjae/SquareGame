using Newtonsoft.Json;

namespace Common;

public class StringName : StringId
{
    [JsonIgnore]
    public string Name
    {
        get => Id;
        set => Id = value;
    }

    public StringName() { }

    public StringName(string name)
        : base(name)
    {
    }
}
