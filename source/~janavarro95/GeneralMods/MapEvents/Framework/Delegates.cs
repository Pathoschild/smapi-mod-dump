using System.Collections.Generic;

namespace EventSystem.Framework
{
    public class Delegates
    {
        public delegate void voidDel();
        public delegate void strDel(string s);
        public delegate void paramFunction(List<object> parameters);
    }
}
