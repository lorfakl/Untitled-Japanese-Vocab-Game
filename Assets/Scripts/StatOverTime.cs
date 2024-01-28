using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectSpecificGlobals;
using System.Collections;

[Serializable]
public class StatOverTime<T> : IEnumerable<OrderedJsonItem<T>>
{
    [JsonIgnore]
    private readonly int maxCount;

    [JsonProperty("OrderedStats")]
    private List<OrderedJsonItem<T>> OrderedStats { get; set; }

    public int Count {  get { return OrderedStats.Count; } }

    public StatOverTime()
    {
        maxCount = Globals.MaxStatOverTimeSize;
        OrderedStats = new List<OrderedJsonItem<T>>();  
    }

    public void AddStat(T value)
    {
        if(OrderedStats.Count == 0)
        {
            var item = new OrderedJsonItem<T>(value, 0, DateTime.UtcNow);
            OrderedStats.Add(item);
        }
        else
        {
            if(OrderedStats.Count < maxCount)
            {
                var item = new OrderedJsonItem<T>(value, OrderedStats.Count, DateTime.UtcNow);
                OrderedStats.Add(item);
            }
            else
            {
                List<OrderedJsonItem<T>> tempList = new List<OrderedJsonItem<T>>();
                for(int i = 1; i < OrderedStats.Count; i++)
                {
                    var oldListItem = OrderedStats[i];
                    tempList.Add(new OrderedJsonItem<T>(oldListItem.Value, i-1, DateTime.UtcNow));
                }
            }
        }    
    }

    public T MostRecent()
    {
        if(OrderedStats.Count > 0)
        {
            return OrderedStats[OrderedStats.Count - 1].Value;
        }
        else
        {
            return default(T);
        }
    }

    public T Oldest()
    {
        if (OrderedStats.Count > 0)
        {
            return OrderedStats[0].Value;
        }
        else
        {
            return default(T);
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return OrderedStats.GetEnumerator();
    }

    IEnumerator<OrderedJsonItem<T>> IEnumerable<OrderedJsonItem<T>>.GetEnumerator()
    {
        foreach(var item in OrderedStats)
        {
            yield return item;
        }
    }
}

