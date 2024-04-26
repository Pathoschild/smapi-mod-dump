/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Alphablackwolf/SkillPrestige
**
*************************************************/

using NUnit.Framework;
using SkillPrestige;

namespace UnitTests
{
    [TestFixture]
    public class ExtensionTests
    {
        [Test]
        public void SetInstanceFieldOfBase_SetsPrivateBaseClassProperty()
        {
            var item = new DerivedClass();
            Assert.That(item.GetCost == 2);
            item.SetProperty();
            Assert.That(item.GetCost == 5);
        }

        private class BaseClass
        {
            // ReSharper disable once InconsistentNaming
            private readonly int _cost;

            // ReSharper disable once ConvertToAutoProperty - test requires the 'cost' property be private.
            public int GetCost => this._cost;

            protected BaseClass()
            {
                this._cost = 2;
            }
        }

        private class DerivedClass : BaseClass
        {
            public void SetProperty()
            {
                this.SetInstanceFieldOfBase("_cost", 5);
            }
        }
    }
}
