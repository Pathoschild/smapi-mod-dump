/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/DialogueExtension
**
*************************************************/

using Netcode;
using StardewValley;

namespace SDV.Shared.Abstractions
{
  public class ProposalWrapper : IProposalWrapper
  {
    public ProposalWrapper(Proposal item) => GetBaseType = item;
    public Proposal GetBaseType { get; }
    public NetFields NetFields { get; }
  }
}
