using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Netcode;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewValley.Quests
{

    public class SocializeQuest : Quest
    {
       
       
        public readonly NetList<string, NetString> whoToGreet = new NetList<string, NetString>();
        [XmlElement("total")]
        public readonly NetInt total = new NetInt();
        public readonly NetDescriptionElementList parts = new NetDescriptionElementList();
        [XmlElement("objective")]
        public readonly NetDescriptionElementRef objective = new NetDescriptionElementRef();

        public SocializeQuest()
        {
            this.questType.Value = 5;
        }

        protected override void initNetFields()
        {
            base.initNetFields();
            this.NetFields.AddFields((INetSerializable)this.whoToGreet, (INetSerializable)this.total, (INetSerializable)this.parts, (INetSerializable)this.objective);
        }

        public void loadQuestInfo()
        {
            if (this.whoToGreet.Count > 0)
                return;
            this.questTitle = Game1.content.LoadString("Strings\\StringsFromCSFiles:SocializeQuest.cs.13785");
            this.parts.Clear();
            this.parts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:SocializeQuest.cs.13786", this.random.NextDouble() < 0.3 ? (object)new DescriptionElement("Strings\\StringsFromCSFiles:SocializeQuest.cs.13787") : (this.random.NextDouble() < 0.5 ? (object)new DescriptionElement("Strings\\StringsFromCSFiles:SocializeQuest.cs.13788") : (object)new DescriptionElement("Strings\\StringsFromCSFiles:SocializeQuest.cs.13789"))));
            this.parts.Add((DescriptionElement)"Strings\\StringsFromCSFiles:SocializeQuest.cs.13791");
            foreach (string key in Game1.content.Load<Dictionary<string, string>>("Data\\NPCDispositions").Keys)
            {
                if (!(key == "Kent") && !(key == "Sandy") && (!(key == "Dwarf")) && (!(key == "Wizard") && !(key == "Krobus")))
                    this.whoToGreet.Add(key);
            }
            this.objective.Value = new DescriptionElement("Strings\\StringsFromCSFiles:SocializeQuest.cs.13802", (object)"2", (object)this.whoToGreet.Count);
            this.total.Value = this.whoToGreet.Count;
            this.whoToGreet.Remove("Lewis");
            this.whoToGreet.Remove("Robin");
        }

        public override void reloadDescription()
        {
            if (this._questDescription == "")
                this.loadQuestInfo();
            if (this.parts.Count == 0 || this.parts == null)
                return;
            string str = "";
            foreach (DescriptionElement part in (NetList<DescriptionElement, NetDescriptionElementRef>)this.parts)
                str += part.loadDescriptionElement();
            this.questDescription = str;
        }

        public override void reloadObjective()
        {
            this.loadQuestInfo();
            if (this.objective.Value == null && this.whoToGreet.Count > 0)
                this.objective.Value = new DescriptionElement("Strings\\StringsFromCSFiles:SocializeQuest.cs.13802", (object)((int)((NetFieldBase<int, NetInt>)this.total) - this.whoToGreet.Count), (object)this.total.Value);
            if (this.objective.Value == null)
                return;
            this.currentObjective = this.objective.Value.loadDescriptionElement();
        }

        public override bool checkIfComplete(NPC npc = null, int number1 = -1, int number2 = -1, Item item = null, string monsterName = null)
        {
            this.loadQuestInfo();
            if (npc != null && this.whoToGreet.Remove(npc.Name))
                Game1.dayTimeMoneyBox.moneyDial.animations.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(387, 497, 3, 8), 800f, 1, 0, Game1.dayTimeMoneyBox.position + new Vector2(228f, 244f), false, false, 1f, 0.01f, Color.White, 4f, 0.3f, 0.0f, 0.0f, false)
                {
                    scaleChangeChange = -0.012f
                });
            if (this.whoToGreet.Count == 0 && !(bool)((NetFieldBase<bool, NetBool>)this.completed))
            {
                foreach (string key in Game1.player.friendshipData.Keys)
                {
                    if (Game1.player.friendshipData[key].Points < 2729)
                        Game1.player.friendshipData[key].Points += 100;
                }
                this.questComplete();
                return true;
            }
            this.objective.Value = new DescriptionElement("Strings\\StringsFromCSFiles:SocializeQuest.cs.13802", (object)((int)((NetFieldBase<int, NetInt>)this.total) - this.whoToGreet.Count), (object)this.total.Value);
            return false;
        }
    }
}