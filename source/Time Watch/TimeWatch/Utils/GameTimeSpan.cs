/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KyuubiRan/TimeWatch
**
*************************************************/

namespace TimeWatch.Utils;

internal struct GameTimeSpan
{
    public static GameTimeSpan WorldMax => new(2600);
    public static GameTimeSpan WorldMin => new(600);
    public static GameTimeSpan WorldNow => new(GameTimeUtils.TimeOfDay);

    public int Hour { get; private set; }

    public int Minute { get; private set; }

    public int Count => Hour * 100 + Minute;
    public int TotalMinutes => Hour * 60 + Minute;
    public bool ValidWorldTime => Count is >= 600 and <= 2600;

    public GameTimeSpan(int hour, int minute)
    {
        var totalMinutes = hour * 60 + minute;
        Hour = totalMinutes / 60;
        Minute = totalMinutes % 60;
    }

    public static GameTimeSpan FromTime(int hour, int min) => new(hour, min);

    public GameTimeSpan(int time)
    {
        Hour = time / 100;
        Minute = time % 100;
    }

    public static GameTimeSpan FromMinutes(int minutes) => new(0, minutes);
    public static GameTimeSpan FromHours(int hours) => new(hours, 0);

    public static GameTimeSpan FromTime(int time) => new(time);

    public void Add(GameTimeSpan time)
    {
        var ttl = TotalMinutes + time.TotalMinutes;
        SetTime(ttl);
    }

    public void Decrease(GameTimeSpan time)
    {
        var ttl = TotalMinutes - time.TotalMinutes;
        SetTime(ttl);
    }

    public void AddHours(int hours)
    {
        Hour += hours;
    }

    public void AddMinutes(int minutes)
    {
        var ttl = TotalMinutes + minutes;
        SetTime(ttl);
    }

    public void SetTime(int hour, int minute)
    {
        var totalMinutes = hour * 60 + minute;
        Hour = totalMinutes / 60;
        Minute = totalMinutes % 60;
    }

    public void SetTime(GameTimeSpan time) => SetTime(time.Hour, time.Minute);

    public void SetTime(int minutes) => SetTime(minutes / 60, minutes % 60);


    public override string ToString()
    {
        return $"{Hour:00}:{Minute:00}";
    }

    public static GameTimeSpan operator +(GameTimeSpan l, GameTimeSpan r)
    {
        var ttl = l.TotalMinutes + r.TotalMinutes;
        return FromMinutes(ttl);
    }

    public static bool operator <(GameTimeSpan l, GameTimeSpan r)
    {
        return l.TotalMinutes < r.TotalMinutes;
    }

    public static bool operator >(GameTimeSpan l, GameTimeSpan r)
    {
        return l.TotalMinutes > r.TotalMinutes;
    }

    public static bool operator <=(GameTimeSpan l, GameTimeSpan r)
    {
        return l.TotalMinutes <= r.TotalMinutes;
    }

    public static bool operator >=(GameTimeSpan l, GameTimeSpan r)
    {
        return l.TotalMinutes >= r.TotalMinutes;
    }

    public static bool operator ==(GameTimeSpan l, GameTimeSpan r)
    {
        return l.Hour == r.Hour && l.Minute == r.Minute;
    }
    
    public static bool operator !=(GameTimeSpan l, GameTimeSpan r)
    {
        return l.Hour != r.Hour || l.Minute != r.Minute;
    }

    public void ApplyToWorldTime()
    {
        GameTimeUtils.TimeOfDay = Count.CoerceIn(600, 2600);
    }

    public static GameTimeSpan operator -(GameTimeSpan l, GameTimeSpan r)
    {
        var ttl = l.TotalMinutes - r.TotalMinutes;
        return FromMinutes(ttl);
    }

    public static GameTimeSpan operator -(GameTimeSpan thiz)
    {
        return FromMinutes(-thiz.TotalMinutes);
    }
}