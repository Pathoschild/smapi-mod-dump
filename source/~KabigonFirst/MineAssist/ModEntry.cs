/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KabigonFirst/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using MineAssist.Framework;
using MineAssist.Config;
using System.Collections;

namespace MineAssist {
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod {
        public static ModEntry m_instance;

        private ModConfig m_config;
        private string m_curMode = "Default";

        private SButton m_modify = SButton.None;
        private SButton m_lastActionButtion = SButton.None;

        private Dictionary<string, CmdCfg> m_keyMap = null;
        private Command m_curCmd  = null;
        private CmdCfg m_cmdcfg = null;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper) {
            m_instance = this;

            m_config = helper.ReadConfig<ModConfig>();
            //check if m_config is correct
            if (m_config == null || m_config.modes == null) {
                this.Monitor.Log("Read configure file error.", LogLevel.Error);
                return;
            }
            //check if default mode exists. If not, try to get one
            if(!m_config.modes.ContainsKey(m_curMode)) {
                if (m_config.modes.Count<=0) {
                    this.Monitor.Log("No modes found in configure file.", LogLevel.Info);
                    return;
                }
                IEnumerator enumerator = m_config.modes.Keys.GetEnumerator();
                enumerator.MoveNext();
                m_curMode = (string)enumerator.Current;
                this.Monitor.Log($"Default mode does not exists, use {m_curMode} Mode instead.", LogLevel.Error);
            }
            m_keyMap = m_config.getModeDict(m_curMode);
            if (!m_config.isEnable) {
                return;
            }

            helper.Events.Input.ButtonPressed += onButtonPressed;
            helper.Events.Input.ButtonReleased += onButtonReleased;
            helper.Events.GameLoop.UpdateTicked += onUpdateTick;
            helper.Events.Display.Rendered += onRendered;
        }

        public void switchMode(string modeName) {
            if (!m_config.modes.ContainsKey(modeName)) {
                StardewWrap.inGameMessage($"No {modeName} mode!!");
                return;
            }
            m_curMode = modeName;
            m_keyMap = m_config.getModeDict(m_curMode);
            StardewWrap.inGameMessage($"{modeName} mode ON!!");
        }

        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void onButtonPressed(object sender, ButtonPressedEventArgs e) {
#if DEBUG
            this.Monitor.Log($"Pressed {e.Button}.");
#endif
            //only when player is ready shall we start process button event
            if (!StardewWrap.isPlayerReady()) {
                return;
            }

            //if current key is modify key, mark the key
            if (isModifyKey(e.Button)) {
                m_modify = e.Button;
                this.Helper.Input.Suppress(e.Button);
                return;
            }
            //not modify key. Now check if the key combination is in the config
            CmdCfg cfg = findCmdCfg(e.Button);
            if (cfg == null) {
                return;
            }
            //old command must finished before new command could be created, but record the key to see if we can invoke it later
            if ((m_curCmd != null && !m_curCmd.isFinish) || StardewWrap.isPlayerBusy()) {
                m_lastActionButtion = e.Button;
                return;
            }
            //now create the command
            m_cmdcfg = cfg;
            m_curCmd = Command.create(m_cmdcfg.cmd);
            if (m_curCmd == null) {
                m_cmdcfg = null;
                return;
            }
            //command is there so just overide and execute it
            m_curCmd.exec(m_cmdcfg.par);
            this.Helper.Input.Suppress(e.Button);
        }

        /// <summary>Raised after the player releases a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void onButtonReleased(object sender, ButtonReleasedEventArgs e) {
#if DEBUG
            this.Monitor.Log($"Released {e.Button}.");
#endif
            //only when player is ready shall we start process button event
            if(!StardewWrap.isPlayerReady()) {
                return;
            }
            //if modify key released, mark m_modify as none and stop current command if necessary
            if (isModifyKey(e.Button)) {
                if (m_modify == e.Button) {
                    m_modify = SButton.None;
                }
                if (m_cmdcfg!= null && e.Button == m_cmdcfg.modifyKey) {
                    stopCmd();
                }
                this.Helper.Input.Suppress(e.Button);
                return;
            }
            //it's action key, mark released
            if (m_lastActionButtion == e.Button) {
                m_lastActionButtion = SButton.None;
            }
            //try to end active command
            if (m_cmdcfg != null && e.Button == m_cmdcfg.key) {
                stopCmd();
            }
            this.Helper.Input.Suppress(e.Button);
        }

        /// <summary>Raised after the game draws to the sprite patch in a draw tick, just before the final sprite batch is rendered to the screen.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void onRendered(object sender, RenderedEventArgs e) {
            if (m_curCmd != null && !m_curCmd.isFinish) {
                m_curCmd.updateGraphic();
                return;
            }
        }
        
        /// <summary>Raised after the game state is updated (≈60 times per second).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void onUpdateTick(object sender, UpdateTickedEventArgs e) {
            //if last command is not finished, update it
            if (m_curCmd != null && !m_curCmd.isFinish) {
                m_curCmd.update();
                return;
            }

            //try to see if we can invoke new command by last pressed but not released button
            if(StardewWrap.isPlayerBusy() || m_lastActionButtion == SButton.None) {
                return;
            }
            CmdCfg cfg = findCmdCfg(m_lastActionButtion);
            if(cfg == null) {
                return;
            }
            m_lastActionButtion = SButton.None;
            m_cmdcfg = cfg;
            m_curCmd = Command.create(m_cmdcfg.cmd);
            if(m_curCmd == null) {
                return;
            }
            m_curCmd.exec(m_cmdcfg.par);
        }

        /// <summary>Try to get command config from combined keys.</summary>
        /// <param name="key">The action key.</param>
        /// <returns>Is new command created</returns>
        private CmdCfg findCmdCfg(SButton key) {
            string combinedKey = ModeCfg.constructCmdKey(m_modify, key);
            if (m_keyMap == null || !m_keyMap.ContainsKey(combinedKey)) {
                return null;
            }
            return m_keyMap[combinedKey];
        }

        private bool isModifyKey(SButton key) {
            if (m_config == null || m_config.modes == null || !m_config.modes.ContainsKey(m_curMode)) {
                return false;
            }
            return (m_config.modes[m_curMode].modifyKeys.Contains(key));
        }

        private void stopCmd() {
            if (m_curCmd != null && !m_curCmd.isFinish) {
                m_curCmd.end();
            }
            m_curCmd = null;
            m_cmdcfg = null;
        }
    }
}