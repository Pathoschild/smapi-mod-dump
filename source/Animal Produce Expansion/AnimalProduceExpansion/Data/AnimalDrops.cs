/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/AnimalProduceExpansion
**
*************************************************/

using System.Collections.Generic;

namespace AnimalProduceExpansion.Data
{
  public class AnimalDrops
  {
    public string Animal { get; set; }
    public IList<Drop> Drops { get; set; }
    public double ChanceForExtraDrop { get; set; }
    public IList<Drop> ExtraDrops { get; set; }
    internal bool Cached { get; set; }
  }
}