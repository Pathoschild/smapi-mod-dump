using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;

namespace CustomMonsters.Framework
{
    public class MonsterData : Monster
    {
        private IModHelper _helper;
        private IMonitor _monitor;
        public string mName { get; set; }
        public AnimatedSprite mSprite { get; set; }
        public string mSpriteStr { get; set; }

        public MonsterData(IModHelper helper, IMonitor monitor)
        {
            _helper = helper;
            _monitor = monitor;
        }

        public override void reloadSprite()
        {
            var key = _helper.Content.GetActualAssetKey(mSpriteStr);
            this.Sprite = new AnimatedSprite(key + this.Name, 0, 16, 16);
        }
    }
}
