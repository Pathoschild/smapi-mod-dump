/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using System;

namespace Shockah.ProjectFluent
{
	internal class MappingFluent<Input, Output>: IFluent<Input>
	{
		private IFluent<Output> Wrapped { get; set; }
		private Func<Input, Output> Mapper { get; set; }

		public MappingFluent(IFluent<Output> wrapped, Func<Input, Output> mapper)
		{
			this.Wrapped = wrapped;
			this.Mapper = mapper;
		}

		public bool ContainsKey(Input key)
			=> Wrapped.ContainsKey(Mapper(key));

		public string Get(Input key, object? tokens)
			=> Wrapped.Get(Mapper(key), tokens);
	}
}