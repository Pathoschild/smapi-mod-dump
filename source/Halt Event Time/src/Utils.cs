/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/chawolbaka/HaltEventTime
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaltEventTime
{
    public static class Utils
    {

        private static Dictionary<TaskType, TaskPayload> TaskList => HaltEventTimeMod.Instance.TaskList;
        private static LogLevel LogLevel => HaltEventTimeMod.Instance.LogLevel;
        private static IMonitor Monitor => HaltEventTimeMod.Instance.Monitor;
        private static ITranslationHelper Translation => HaltEventTimeMod.Instance.Helper.Translation;

        public static void SendMessage(object message, int delay = 0) => SendMessage(message.ToString(), Color.White, delay);
        public static void SendMessage(string message, int delay = 0) => SendMessage(message, Color.White, delay);
        public static void SendMessage(object message, Color color, int delay = 0) => SendMessage(message.ToString(), color, delay);
        public static async void SendMessage(string message, Color color, int delay = 0)
        {
            if (HaltEventTimeMod.Instance.Active && Context.IsWorldReady)
            {
                if (Game1.multiplayerMode != Game1.singlePlayer)
                    HaltEventTimeMod.Instance.Helper.Multiplayer.SendMessage(new ChatMessagePacket(message, color, delay), nameof(ChatMessagePacket));

                if (delay > 0)
                    await Task.Delay(delay);
                Game1.chatBox.addMessage(message, color);
            }
        }
        public static string GetFarmerNameFromID(long id)
        {
            string name = Game1.getOnlineFarmers()?.FirstOrDefault(p => p.UniqueMultiplayerID == id).Name;
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(id), $"无法找到id为{id}的玩家");
            return name;
                
        }
        public static void UpdateTaskOperate(TaskType type, TaskOperation operate, string sender)
        {
            Monitor.Log($"Task updating, type={type}, operate={operate}, sender={sender}", LogLevel);
            if (type == TaskType.None || string.IsNullOrEmpty(sender) || (type == TaskType.TimeControl && !Context.IsMainPlayer))
                return;

            //在tick里面lock会不会导致游戏卡顿一下...
            lock (TaskList)
            {
                bool ShowEventMessageOnly = type == TaskType.TimeControl && TaskList.ContainsKey(type) && TaskList[type].Sender != null && TaskList[type].Sender.Count > 1;
                if (type == TaskType.TimeControl && operate == TaskOperation.Run)
                    SendMessage($"{Translation.Get("message.event.start", new { playerName = sender })}{(ShowEventMessageOnly ? "." : Translation.Get("message.time.stop"))}", Color.Yellow);
                if (type == TaskType.TimeControl && operate == TaskOperation.Stop)
                    SendMessage($"{Translation.Get("message.event.stop", new { playerName = sender })}{(ShowEventMessageOnly ? "." : Translation.Get("message.time.start"))}", Color.Green);

                //如果任务不存在就创建任务，但任务必须以Run开始（好像不一定必须，但这样子更不容易出现奇怪的bug，除非哪天有需求不然还是限制着吧）
                //如果任务存在就更新任务的状态，但如果要停止或移除就必须等待所有Sender都把Operate更新成Stop或者Remove
                if (TaskList.ContainsKey(type))
                {
                    Monitor.Log($"{type} try to {operate}", LogLevel);
                    switch (operate)
                    {
                        case TaskOperation.Run:
                            if (!TaskList[type].Sender.Any(x => x == sender)) //这里如果直接加入就是允许一个人同时触发多个事件
                                TaskList[type].Sender.Add(sender);
                            TaskList[type].Operate = operate;
                            break;
                        case TaskOperation.Stop:
                            //必须等到最后一个人看完事件才可以把operate更新成Stop或者Remove
                            if (TaskList[type].Sender.Count > 1)
                                TaskList[type].Sender.Remove(sender);
                            else if (TaskList[type].Sender[0] == sender)
                                TaskList[type].Operate = operate;
                            break;
                        case TaskOperation.Remove:
                            TaskList[type].Operate = operate;
                            break;
                        default: throw new InvalidOperationException($"Unknow operate {operate}");
                    }
                    Monitor.Log($"{type} is {TaskList[type].Operate} now", LogLevel);
                    Monitor.Log($"{type} has {TaskList[type].Sender.Count} Sender", LogLevel);
                }
                else if (operate == TaskOperation.Run)
                {
                    TaskList.Add(type, new TaskPayload(operate, sender, sender));
                }
            }
        }
        public static ModConfig ReadConfig()
        {
            ModConfig config = HaltEventTimeMod.Instance.Helper.ReadConfig<ModConfig>();
            if (config.Limit != TaskType.ActionControl && config.Limit != TaskType.SpeedControl && config.Limit != TaskType.WalkOnly && config.Limit != TaskType.None)
            {
                config.Limit = ModConfig.Default.Limit;
                HaltEventTimeMod.Instance.Helper.WriteConfig(config);
                throw new InvalidOperationException($"非法的Limit {config.Limit}");
            }
            return config;
        }
    }
}
