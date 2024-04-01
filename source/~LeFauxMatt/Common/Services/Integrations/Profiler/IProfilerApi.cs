/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.Common.Services.Integrations.Profiler;

using System.Reflection;

public interface IProfilerApi
{
    public IDisposable RecordSection(string ModId, string EventType, string Details);

    public MethodBase AddGenericDurationPatch(string type, string method, string detailsType = null!);
}