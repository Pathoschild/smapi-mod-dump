/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/DialogueExtension
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Globalization;
using StardewValley;

namespace DialogueExtension.Tests.MockWrappers
{
  public class MockLocalizedContentManager : LocalizedContentManager
  {
    public MockLocalizedContentManager(IServiceProvider serviceProvider, string rootDirectory, CultureInfo currentCulture) : base(serviceProvider, rootDirectory, currentCulture)
    {
    }

    public MockLocalizedContentManager(IServiceProvider serviceProvider, string rootDirectory) : base(serviceProvider, rootDirectory)
    {
    }

    public override string LoadString(string path)
    {
      var file = path.Split('\\');
      var fileParts = file[1].Split(':');
      return MockData.LoadString(fileParts[0], fileParts[1]);
    }

    public override T Load<T>(string key)
    {
      if (typeof(T) == typeof(IDictionary<string,string>))
        return (T)MockData.LoadStrings(key.Split('\\')[1]);
      return default;
    }
  }

  public class MockServiceProvider : IServiceProvider
  {
    public object GetService(Type serviceType) => null;
  }
}
