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
using System.Text;
using System.Threading.Tasks;

namespace HaltEventTime
{
    public class TaskPayload
    {
        /// <summary>
        /// 现在需要对任务执行的操作
        /// </summary>
        public TaskOperation Operate;
        
        /// <summary>
        /// 第一个创建任务的玩家
        /// </summary>
        public string Sponsor;

        /// <summary>
        /// 从任务被创建到删除期间发起过Run请求的玩家
        /// </summary>
        public List<string> Sender;

        public TaskPayload(TaskOperation operate, string sponsor, string sender)
        {
            Operate = operate;
            Sponsor = sponsor ?? throw new ArgumentNullException(nameof(sponsor));
            Sender = new List<string>() { sender ?? throw new ArgumentNullException(nameof(sender)) };
        }
    }
}
