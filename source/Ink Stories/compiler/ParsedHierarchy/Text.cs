/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/inkle/ink
**
*************************************************/


namespace Ink.Parsed
{
	public class Text : Parsed.Object
	{
		public string text { get; set; }

		public Text (string str)
		{
			text = str;
		}

		public override Runtime.Object GenerateRuntimeObject ()
		{
			return new Runtime.StringValue(this.text);
		}

        public override string ToString ()
        {
            return this.text;
        }
	}
}

