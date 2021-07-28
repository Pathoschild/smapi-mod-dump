/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/chawolbaka/HaltEventTime
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using HaltEventTime.Patches;
#if HARMONY_2
using HarmonyLib;
#else
using Harmony;
#endif

namespace HaltEventTime
{
    public class HaltEventTimeMod : Mod
    {
        public bool Active; //只在是多人并且有玩家进入后开启
        public bool IsEevntEntered;
        public bool DebugEventUp;
        public LogLevel LogLevel => Config.Debug ? LogLevel.Info : LogLevel.Trace;
        public Dictionary<TaskType, TaskPayload> TaskList = new Dictionary<TaskType, TaskPayload>();

        public static HaltEventTimeMod Instance => _instance;
        private static HaltEventTimeMod _instance;
        private static ModConfig Config => ModConfig.Instance;


        public override void Entry(IModHelper helper)
        {
#if DEBUG
            helper.ConsoleCommands.Add(nameof(HaltEventTime), "", ConsoleCommand);
            helper.ConsoleCommands.Add("het", "", ConsoleCommand);
#endif
            _instance = this;
            ModConfig.Instance = Utils.ReadConfig();
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            helper.Events.Multiplayer.ModMessageReceived += OnModMessageReceived;
            helper.Events.Multiplayer.PeerContextReceived += OnPeerContextReceived;
            helper.Events.GameLoop.DayStarted += (sender, e) => { if (!Context.IsMainPlayer) Active = true; }; //会在每天和进入服务器时触发
#if HARMONY_2
            var harmony = new Harmony(nameof(BuffUpdatePatch));
            harmony.PatchAll();
#else
            HarmonyInstance harmony = HarmonyInstance.Create("StardewValley");
            harmony.Patch(typeof(Buff).GetMethod("update"), new HarmonyMethod(typeof(BuffUpdatePatch).GetMethod(nameof(BuffUpdatePatch.Prefix))));
#endif

        }

        public void ConsoleCommand(string command, string[] args)
        {
            if (args.Length == 0)
                Monitor.Log($"缺少有效参数，具体的请查看源码，我懒的写说明", LogLevel.Error);

            switch (args[0])
            {
                case "active": Active = true; ; break;
                case "inactive": Active = false; break;
                case "run": DebugEventUp = false; break;
                case "stop": DebugEventUp = true; break;
                case "restore": Time.Run(); break;
                case "nrun": Helper.Multiplayer.SendMessage(new TaskPacket(TaskType.TimeControl, TaskOperation.Run), nameof(TaskPacket)); break;
                case "nstop": Helper.Multiplayer.SendMessage(new TaskPacket(TaskType.TimeControl, TaskOperation.Stop), nameof(TaskPacket)); break;
                case "ntask": Helper.Multiplayer.SendMessage(new TaskPacket((TaskType)Enum.Parse(typeof(TaskType), args[1]), (TaskOperation)Enum.Parse(typeof(TaskOperation), args[1])), nameof(TaskPacket)); break;
                case "unlock": Game1.player.canMove = true; Game1.player.canOnlyWalk = false; Game1.player.speed = Farmer.runningSpeed; break;
                case "reload": ModConfig.Instance = Utils.ReadConfig(); break;
                case "status":
                    Monitor.Log($"Active={Active}, Time.Async={Time.Async}, Time.State={Time.State}", LogLevel.Info);
                    Monitor.Log($"Name={Game1.player.Name}, CanMove={Game1.player.CanMove}, Speed={Game1.player.Speed}, CanOnlyWalk={Game1.player.canOnlyWalk}", LogLevel.Info);
                    break;
                default: Monitor.Log($"未知参数 {args[0]}", LogLevel.Error); break;
            }
        }

        public void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Active || !Context.IsWorldReady)
                return;
            else if (Game1.eventUp || DebugEventUp)
                OnEventUp();
            else if(IsEevntEntered)
                OnEventDown();
                
            
            //每tick执行任务，如果存在。
            if (TaskList.Count > 0)
            {
                foreach (var task in TaskList.ToArray())
                {
                    //这边如果是Run那么只需要执行一次，不需要根据Sender数量来执行，所以随便给是sender名就好（也暂时用不上）
                    //如果是Stop那么根据Utils.UpdateTaskOperate内的规则，Sender一定是只有一个了，那么还是只需要给[0]就好
                    if (task.Value.Operate == TaskOperation.Remove)
                        TaskList.Remove(task.Key);
                    else
                        ExecuteTask(task.Key, task.Value.Operate, task.Value.Sponsor, task.Value.Sender[0]);
                }
            }
        }

        public void OnEventUp()
        {
            if (!DebugEventUp && (Game1.currentLocation.currentEvent == null || Game1.currentLocation.currentEvent.isWedding || Game1.currentLocation.currentEvent.isFestival))
                return;

            //IsEevntEntered用于非主机的控制，因为只有主机会有TimeControlTask，非主机就算收到也是不会加入TaskList的
            if (!IsEevntEntered && !TaskList.ContainsKey(TaskType.TimeControl))
            {
                IsEevntEntered = true;

                //如果是主机触发就只需要自己加入任务就好，因为TimeControl是主机独享的
                //如果不是那么发送给主机然后主机就会加入该任务（在OnModMessageReceived）
                if (Context.IsMainPlayer)
                    Utils.UpdateTaskOperate(TaskType.TimeControl, TaskOperation.Run, Game1.player.Name);
                else
                    Helper.Multiplayer.SendMessage(new TaskPacket(TaskType.TimeControl, TaskOperation.Run), nameof(TaskPacket));
                
                //由事件发起人开启，让所有玩家的TaskList都有该任务
                Utils.UpdateTaskOperate(Config.Limit, TaskOperation.Run, Game1.player.Name);
                Helper.Multiplayer.SendMessage(new TaskPacket(Config.Limit, TaskOperation.Run), nameof(TaskPacket));
                if(Config.PauseBuff)
                {
                    Utils.UpdateTaskOperate(TaskType.BuffControl, TaskOperation.Run, Game1.player.Name);
                    Helper.Multiplayer.SendMessage(new TaskPacket(TaskType.BuffControl, TaskOperation.Run), nameof(TaskPacket));
                }
            }
        }

        public void OnEventDown()
        {
            IsEevntEntered = false;
            if (Context.IsMainPlayer && Time.State == Time.Status.Stop && TaskList.Count > 0 && TaskList.ContainsKey(TaskType.TimeControl) && TaskList[TaskType.TimeControl].Operate == TaskOperation.Run)
                Utils.UpdateTaskOperate(TaskType.TimeControl, TaskOperation.Stop, Game1.player.Name);
            else if (!Context.IsMainPlayer)
                Helper.Multiplayer.SendMessage(new TaskPacket(TaskType.TimeControl, TaskOperation.Stop), nameof(TaskPacket));

            //如果Sender有自己并且状态是运行中就变更为stop
            if (TaskList.Count > 0 && TaskList.ContainsKey(Config.Limit) && TaskList[Config.Limit].Operate == TaskOperation.Run && TaskList[Config.Limit].Sender.Any(x=> x == Game1.player.Name))
            {
                Helper.Multiplayer.SendMessage(new TaskPacket(Config.Limit, TaskOperation.Stop), nameof(TaskPacket));
                Utils.UpdateTaskOperate(Config.Limit, TaskOperation.Stop, Game1.player.Name);
            }
            if (TaskList.Count > 0 && Config.PauseBuff && TaskList.ContainsKey(TaskType.BuffControl) && TaskList[TaskType.BuffControl].Operate == TaskOperation.Run && TaskList[TaskType.BuffControl].Sender.Any(x => x == Game1.player.Name))
            {
                Helper.Multiplayer.SendMessage(new TaskPacket(TaskType.BuffControl, TaskOperation.Stop), nameof(TaskPacket));
                Utils.UpdateTaskOperate(TaskType.BuffControl, TaskOperation.Stop, Game1.player.Name);
            }
        }

        private void OnPeerContextReceived(object sender, PeerContextReceivedEventArgs e)
        {
            if (Context.IsMainPlayer)
            {
                  
                if (e.Peer.GetMod(ModManifest.UniqueID) == null)
                {
                    string playerName = Utils.GetFarmerNameFromID(e.Peer.PlayerID);
                    Utils.SendMessage(Helper.Translation.Get("message.less.mod", new { playerName = playerName, ModManifestName = ModManifest.Name }), Color.Red);
                }
                else
                {
                    Active = true;
                    Monitor.Log($"{ModManifest.Name} Is Active", LogLevel);
                }
            }
        }

        public async void OnModMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            Monitor.Log($"Packet received ID:{e.FromModID}, Type:{e.Type}", LogLevel);
            if (e.FromModID == ModManifest.UniqueID && e.Type == nameof(ChatMessagePacket))
            {
                ChatMessagePacket packet = e.ReadAs<ChatMessagePacket>();
                Monitor.Log(packet.ToString(), LogLevel);
                if (packet.Delay > 0)
                    await Task.Delay(packet.Delay);
                Game1.chatBox.addMessage(packet.Message, packet.Color);
            }
            else if (e.FromModID == ModManifest.UniqueID && e.Type == nameof(TaskPacket))
            {
                TaskPacket packet = e.ReadAs<TaskPacket>();
                Monitor.Log(packet.ToString(), LogLevel);
                if (packet.Type != TaskType.None)
                    Utils.UpdateTaskOperate(packet.Type, packet.Operate, Utils.GetFarmerNameFromID(e.FromPlayerID));
            }
        }

        public void ExecuteTask(TaskType type, TaskOperation operate, string sponsor, string sender)
        {
            if (operate == TaskOperation.Run)
            {
                //这些基本上都必须每tick运行，不然会变出来，虽然有时候又不会，总之竟然大部分情况会变那就只能每tick都去改了
                switch (type)
                {
                    case TaskType.TimeControl: Time.Stop(); break; //如果在这里发送行为控制任务的话可以做到全部由主机的配置文件来控制
                    case TaskType.BuffControl: BuffUpdatePatch.Disable = false; break;
                    case TaskType.ActionControl: Game1.player.CanMove = false; break;
                    case TaskType.SpeedControl: Game1.player.Speed = 0; break;
                    case TaskType.WalkOnly: Game1.player.canOnlyWalk = true; break;
                    case TaskType.None: return;
                    default: throw new InvalidOperationException($"Unknow Type {type}");
                }
            }
            else if (operate == TaskOperation.Stop)
            {
                switch (type)
                {
                    case TaskType.TimeControl: Time.Run(); break;
                    case TaskType.BuffControl: BuffUpdatePatch.Disable = true; break;
                    case TaskType.ActionControl: Game1.player.CanMove = true; break;
                    case TaskType.SpeedControl: Game1.player.Speed = Farmer.runningSpeed; break;
                    case TaskType.WalkOnly: Game1.player.canOnlyWalk = false; break;
                    case TaskType.None: return;
                    default: throw new InvalidOperationException($"Unknow Type {type}");
                }
                Monitor.Log($"Task {type} stop operate executed successfully", LogLevel);
                Utils.UpdateTaskOperate(type, TaskOperation.Remove, sender);
            }
        }

    }
}
