/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/RuiNtD/SVRichPresence
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using StardewModdingAPI;
using StardewValley;
using static SVRichPresence.IRichPresenceAPI;

namespace SVRichPresence
{
  public class RichPresenceAPI : IRichPresenceAPI
  {
    private readonly Dictionary<string, Tag> reg = new(StringComparer.InvariantCultureIgnoreCase);
    private readonly RichPresenceMod RPMod;

    public RichPresenceAPI(RichPresenceMod mod)
    {
      RPMod = mod;
#if DEBUG
      reg["test1"] = new("test1", () => "hello");
      reg["test2"] = new("test1", () => "world");
      reg["test3"] = new("test2", () => "test");
      reg["thrower"] = new("test2", () => throw new Exception("test"));
      reg["nuller"] = new("test2", () => null);
#endif
    }

    public bool SetTag(IManifest mod, string key, Func<string> func)
    {
      string modID = mod.UniqueID;
      if (TagExists(key) && GetTagOwner(key) != modID)
        return false;
      reg[key] = new(modID, func);
      return true;
    }

    public bool RemoveTag(IManifest mod, string key)
    {
      string modID = mod.UniqueID;
      if (!reg.ContainsKey(modID))
        return true;
      if (GetTagOwner(key) != modID)
        return false;
      reg.Remove(key);
      return true;
    }

    public IResolvedTag ResolveTag(string key)
    {
      Tag tag = reg[key];
      if (tag == null)
        return null;
      try
      {
        string val = tag.func();
        return new ResolvedTag(val);
      }
      catch (Exception e)
      {
        return new ResolvedTag(e);
      }
    }

    public string FormatTag(string key, string replaceError = null, string replaceNull = null)
    {
      IResolvedTag res = ResolveTag(key);
      if (!res.Success)
        return replaceError;
      return res.Value ?? replaceNull;
    }

    public bool TagExists(string key) => reg.ContainsKey(key);

    public string GetTagOwner(string key) => reg[key]?.owner ?? null;

    public IDictionary<string, IResolvedTag> ResolveAllTags()
    {
      return reg.ToDictionary(
        pair => pair.Key,
        pair => ResolveTag(pair.Key),
        StringComparer.InvariantCultureIgnoreCase
      );
    }

    public IDictionary<string, string> FormatAllTags(
      string replaceErrors = null,
      string replaceNulls = null
    )
    {
      var output = new Dictionary<string, string>();

      foreach (var tag in ResolveAllTags())
      {
        var val = FormatTag(tag.Key, replaceErrors, replaceNulls);
        if (val is null)
          continue;
        output[tag.Key] = val;
      }

      return output;
    }

    public class ResolvedTag : IRichPresenceAPI.IResolvedTag
    {
      public ResolvedTag(string val)
      {
        value = val;
      }

      public ResolvedTag(Exception e)
      {
        exception = e;
      }

      private readonly string value;
      private readonly Exception exception;

      public bool Success => exception == null;
      public string Value => value;
      public Exception Exception => exception;
    }

    public string None => Game1.content.LoadString("Strings\\UI:Character_none");

    public string GamePresence
    {
      get =>
        RPMod.Helper.Reflection.GetField<string>(typeof(Game1), "debugPresenceString").GetValue();
      set =>
        RPMod
          .Helper.Reflection.GetField<string>(typeof(Game1), "debugPresenceString")
          .SetValue(value);
    }

    public string FormatText(string text, string replaceError = null, string replaceNull = null)
    {
      // Code is copied and modified from SMAPI.
      IDictionary<string, IResolvedTag> tags = ResolveAllTags();
      return Regex.Replace(
        text,
        @"{{?([ \w\.\-]+)}}?",
        match =>
        {
          string orig = match.Value;
          string key = match.Groups[1].Value.Trim();
          string formatted = FormatTag(key, replaceError, replaceNull);
          if (formatted == null)
            return orig;
          return formatted;
        }
      );
    }

    private class Tag
    {
      public Tag(string owner, Func<string> func)
      {
        this.owner = owner;
        this.func = func;
      }

      public string owner;
      public Func<string> func;
    }
  }
}
