using NUnit.Framework;
using SkillPrestige;


namespace UnitTest
{
    [TestFixture]
    public class ExtensionTests
    {

        [Test]
        public void SetInstanceFieldOfBase_SetsPrivateBaseClassProperty()
        {
            var item = new DerivedClass();
            Assert.IsTrue(item.GetCost == 2);
            item.SetProperty();
            Assert.IsTrue(item.GetCost == 5);
        }

        private class BaseClass
        {
            private readonly int _cost;

            // ReSharper disable once ConvertToAutoProperty - test requires the 'cost' property be private.
            public int GetCost => _cost;

            protected BaseClass()
            {
                _cost = 2;
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
