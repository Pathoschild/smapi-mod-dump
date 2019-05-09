using ProtoBuf;
using System.Collections.Generic;
using System.Linq;

namespace StarStats.Common
{
    public static class Timestamp
    {
        public static uint DayStart(uint raw)
        {
            return raw / 121;
        }
    }

    [ProtoContract]
    public class Database
    {
        [ProtoMember(1)]
        public uint Max;

        [ProtoMember(2)]
        public IList<TimeSeries> Metrics;

        public Database()
        {
            Metrics = new List<TimeSeries>();
        }

        private TimeSeries get(string m, string t)
        {
            return Metrics.SingleOrDefault(x => x.Metric == m && x.Tags == t);
        }

        private TimeSeries getOrCreate(string m, string t)
        {
            var ts = get(m, t);
            if (ts == null)
            {
                ts = new TimeSeries
                {
                    Metric = m,
                    Tags = t,
                };
                Metrics.Add(ts);
            }
            return ts;
        }


        public void Add(uint t, double val, string metric, string tags = null)
        {
            var ts = getOrCreate(metric, tags);
            if(ts.Data.Any() && ts.Data.Last().Value == val)
            {
                return;
            }
            ts.Add(t, val);
        }

        // like add, but won;t add a zero value unless it is already in the dataset.
        public void AddSkipZero(uint t, double val, string metric, string tags = null)
        {
            var ts = get(metric, tags);
            if (ts == null && val == 0) return;
            ts = getOrCreate(metric, tags);
            ts.Add(t, val);
        }

        public void AddRaw(uint t, double val, string metric, string tags = null)
        {
            var ts = getOrCreate(metric, tags);
            ts.Add(t, val);
        }

        public IEnumerable<TimeSeries> Metric(string metric)
        {
            return Metrics.Where(x => x.Metric == metric);
        }
    }

    [ProtoContract]
    public class TimeSeries
    {
        [ProtoMember(1)]
        public string Metric { get; set; } = "";
        [ProtoMember(2)]
        public string Tags { get; set; } = "";
        [ProtoMember(3)]
        public IList<Point> Data;

        public TimeSeries()
        {
            Data = new List<Point>();
        }

        public void Add(uint t, double v)
        {
            Data.Add(new Point { Time = t, Value = v });
        }

        public IEnumerable<double> ToDaily(uint max)
        {
            var days = Data.GroupBy(x => Timestamp.DayStart(x.Time)).ToDictionary(x => x.Key, x=> x.Last().Value);

            double last = 0;
            for(uint dayStart = 0; dayStart <= max; dayStart++)
            {
                if (days.ContainsKey(dayStart))
                {
                    last = days[dayStart];
                    yield return last;
                }
                else
                {
                    yield return last;
                }
            }
        }
    }

    [ProtoContract]
    public struct Point
    {
        [ProtoMember(1)]
        public uint Time;
        [ProtoMember(2)]
        public double Value;
    }
}
