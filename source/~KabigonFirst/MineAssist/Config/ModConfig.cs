using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using MineAssist.Framework;

namespace MineAssist.Config {
    public class ModConfig {
        public bool isEnable = true;
        public Dictionary<string, ModeCfg> modes { get; set; } = new Dictionary<string, ModeCfg> {
            { "Default", new ModeCfg(
                new HashSet<SButton> {
                    SButton.ControllerBack
                },
                new List<CmdCfg>{
                    new CmdCfg(SButton.ControllerBack, SButton.ControllerA, CommandSwitchMode.name, new Dictionary<string, string>{
                        { CommandSwitchMode.Paramter.ModeName.ToString(), "Mining" }
                    }),
                    new CmdCfg(SButton.None, SButton.ControllerStart, CommandOpenMenu.name, new Dictionary<string, string>()),
                    new CmdCfg(SButton.ControllerBack, SButton.ControllerStart, CommandPause.name, new Dictionary<string, string>())
                }
            )},
            { "Mining", new ModeCfg(
                new HashSet<SButton> {
                    SButton.ControllerBack
                },
                new List<CmdCfg>{
                    new CmdCfg(SButton.None, SButton.ControllerStart, CommandSwitchMode.name, new Dictionary<string, string>{
                        { CommandSwitchMode.Paramter.ModeName.ToString(), "Default" }
                    }),
                    new CmdCfg(SButton.ControllerBack, SButton.ControllerY, CommandCraft.name, new Dictionary<string, string>{
                        { CommandCraft.Paramter.ItemName.ToString(), "Staircase" },
                        { CommandCraft.Paramter.ToPosition.ToString(), "3" }
                    }),
                    new CmdCfg(SButton.None, SButton.LeftShoulder, ConnamdUseItem.name, new Dictionary<string, string>{
                        { ConnamdUseItem.Paramter.ItemName.ToString() , "PickAxe" },
                        { ConnamdUseItem.Paramter.IsContinuous.ToString() , "true" }
                    }),
                    new CmdCfg(SButton.None, SButton.RightShoulder, ConnamdUseItem.name, new Dictionary<string, string>{
                        { ConnamdUseItem.Paramter.ItemName.ToString() , "Weapon" },
                        { ConnamdUseItem.Paramter.IsContinuous.ToString() , "true" },
                        { ConnamdUseItem.Paramter.Order.ToString() , StardewWrap.UseOrder.QualityHighest.ToString()}
                    }),
                    new CmdCfg(SButton.None, SButton.RightStick, ConnamdUseItem.name, new Dictionary<string, string>{
                        { ConnamdUseItem.Paramter.ItemName.ToString() , "Staircase" }
                    }),
                    new CmdCfg(SButton.ControllerBack, SButton.LeftStick, ConnamdUseItem.name, new Dictionary<string, string>{
                        { ConnamdUseItem.Paramter.ItemName.ToString() , "Edible" },
                        { ConnamdUseItem.Paramter.Condition.ToString() , StardewWrap.UseCondition.HealthAtMost.ToString() + " 30" },
                        { ConnamdUseItem.Paramter.Order.ToString() , StardewWrap.UseOrder.PriceLowest.ToString()}
                    }),
                    new CmdCfg(SButton.None, SButton.LeftStick, ConnamdUseItem.name, new Dictionary<string, string>{
                        { ConnamdUseItem.Paramter.Position.ToString() , "12" }
                    })
                }
            )}
        };

        private Dictionary<string, Dictionary<string, CmdCfg>> modeDict = null;

        public void constructDict() {
            modeDict = new Dictionary<string, Dictionary<string, CmdCfg>>();
            foreach (KeyValuePair<string, ModeCfg> m in modes) {
                modeDict[m.Key] = m.Value.getCmdDict();
            }
        }

        public Dictionary<string, CmdCfg> getModeDict(string modeName) {
            if (modeDict==null) {
                constructDict();
            }
            if (modeDict.ContainsKey(modeName)) {
                return modeDict[modeName];
            }
            return null;
        }
    }
}
