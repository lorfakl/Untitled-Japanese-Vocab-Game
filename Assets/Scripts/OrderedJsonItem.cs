using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public class OrderedJsonItem<T>
{
    [JsonProperty(PropertyName ="value")]
    public readonly T Value;

    [JsonProperty(PropertyName = "index")]
    public readonly int Index;

    [JsonProperty(PropertyName = "entryDate")]
    public readonly DateTime EntryDate;

    [JsonConstructor]
    public OrderedJsonItem(T value, int index, DateTime entryDate)
    {
        Value = value;
        Index = index;
        EntryDate = entryDate;
    }
}

