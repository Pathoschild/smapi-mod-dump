/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omegasis.SaveAnywhere.Framework
{
    public class SchedulePathInfo
    {
        public int timeToGoTo;
        public int endX;
        public int endY;
        public string endMap;
        public int endDirection;
        public string endBehavior;
        public string endMessage;

        public SchedulePathInfo()
        {

        }

        public SchedulePathInfo(string RawData)
        {
            string[] fields = RawData.Split(' ');
            try
            {
                this.timeToGoTo = Convert.ToInt32(fields[0]);
            }
            catch(Exception err)
            {


                return;
            }
            this.endMap = fields[1];
            this.endX =Convert.ToInt32(fields[2]);
            this.endY =Convert.ToInt32(fields[3]);
            this.endDirection =Convert.ToInt32(fields[4]);
            if (fields.Length >= 6)
            {
                if (fields[5][0] == '"')
                {
                    this.endMessage = fields[5].Substring(fields[5].IndexOf('"'));
                }
            }
            if (fields.Length >= 7)
            {

                if (fields[5][0] == '"')
                {
                    this.endBehavior = fields[5];
                }
                else
                {
                    this.endMessage = fields[6].Substring(fields[6].IndexOf('"'));
                }

            }
        }

    }
}
