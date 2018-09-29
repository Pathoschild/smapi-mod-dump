using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarpToFriends
{
	public class ModConfig
	{

		[OptionDisplay("Open Menu Key")]
		public string OpenMenuKey { get; set; } = Keys.J.ToString();

	}
}
