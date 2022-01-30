/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/DialogueExtension
**
*************************************************/

using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace DialogueExtension.Tests
{
  public static partial class MockData
  {
    private static readonly IDictionary<string, IDictionary<string, string>> _characterDialogue =
      new Dictionary<string, IDictionary<string, string>>();

    private static readonly IDictionary<string, IDictionary<string, string>> _characterMarriageDialogue =
      new Dictionary<string, IDictionary<string, string>>();

    private static readonly IDictionary<string, IDictionary<string, string>> _generalDialogue =
      new Dictionary<string, IDictionary<string, string>>();

    public static IDictionary<string, string> CharacterDialogue(string name)
    {
      if (!_characterDialogue.ContainsKey(name))
        _characterDialogue.Add(name, DeserializeObject<IDictionary<string, string>>(Path.Combine("..", "..", "..", "Dialogue", name + ".json")));
      return _characterDialogue[name];
    }

    public static IDictionary<string, string> CharacterMarriageDialogue(string name)
    {
      if (!_characterMarriageDialogue.ContainsKey(name))
        _characterMarriageDialogue.Add(name, DeserializeObject<IDictionary<string, string>>(Path.Combine("..", "..", "..", "Dialogue", name + ".json")));
      return _characterMarriageDialogue[name];
    }

    public static string LoadString(string file, string key)
    {
      if (!_generalDialogue.ContainsKey(file))
        LoadStrings(file);
      return _generalDialogue[file][key];
    }

    public static IDictionary<string, string> LoadStrings(string file)
    {
      if (!_generalDialogue.ContainsKey(file))
        _generalDialogue.Add(file, DeserializeObject<IDictionary<string, string>>(Path.Combine("..", "..", "..", "Dialogue", file + ".json")));
      return _generalDialogue[file];
    }

    private static T DeserializeObject<T>(string filePath) where T : class
    {
      var jsonString = File.ReadAllText(filePath);
      try
      {
        return JsonConvert.DeserializeObject<T>(jsonString);
      }
      catch (JsonReaderException)
      {
        if (!jsonString.Contains("“") && !jsonString.Contains("”")) throw;
        try
        {
          return JsonConvert.DeserializeObject<T>(jsonString.Replace('“', '"').Replace('”', '"'));
        }
        catch
        {
          // ignored
        }

        throw;
      }
    }
  }
}