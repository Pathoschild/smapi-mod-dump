/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NeroYuki/StardewSurvivalProject
**
*************************************************/

using StardewSurvivalProject.source.effects;

namespace StardewSurvivalProject.source.commands
{
    public class Commands
    {
        private Manager instance = null;
        public Commands(Manager instance)
        {
            this.instance = instance;
        }
        public void SetHungerCmd(string cmd, string[] args)
        {
            if (cmd == "player_sethunger")
            {
                if (args.Length != 1)
                    LogHelper.Info("Usage: player_sethunger <amt>");
                else {
                    instance.setPlayerHunger(double.Parse(args[0]));
                    LogHelper.Info($"Ok, set player hunger level to {args[0]}");
                }
                    
            }

        }
        public void SetThirstCmd(string cmd, string[] args)
        {
            if (cmd == "player_setthirst")
            {
                if (args.Length != 1)
                    LogHelper.Info("Usage: player_setthirst <amt>");
                else
                {
                    instance.setPlayerThirst(double.Parse(args[0]));
                    LogHelper.Info($"Ok, set player thirst level to {args[0]}");
                }
            }
        }

        public void SetBodyTemp(string cmd, string[] args)
        {
            if (cmd == "player_settemp")
            {
                if (args.Length < 1)
                    LogHelper.Info("Usage: player_settemp <temp>");
                else
                {
                    instance.setPlayerBodyTemp(double.Parse(args[0]));
                    LogHelper.Info($"Ok, set player body temperature to {args[0]}C");
                }
            }
        }
        
        public void SetEffect(string cmd, string[] args)
        {
            if (cmd == "player_testeffect")
            {
                if (args.Length < 1)
                {
                    EffectManager.applyEffect(EffectManager.burnEffectIndex);
                    EffectManager.applyEffect(EffectManager.dehydrationEffectIndex);
                    EffectManager.applyEffect(EffectManager.feverEffectIndex);
                    EffectManager.applyEffect(EffectManager.frostbiteEffectIndex);
                    EffectManager.applyEffect(EffectManager.heatstrokeEffectIndex);
                    EffectManager.applyEffect(EffectManager.hypothermiaEffectIndex);
                    EffectManager.applyEffect(EffectManager.starvationEffectIndex);
                    EffectManager.applyEffect(EffectManager.stomachacheEffectIndex);
                    LogHelper.Info($"Effect index not found, adding all custom effect at once");
                }
                else
                {
                    foreach (string i in args)
                    {
                        EffectManager.applyEffect(int.Parse(i));
                        LogHelper.Info($"Ok, applied effect at index {i}");
                    }
                }
            }
        }
    }
}
