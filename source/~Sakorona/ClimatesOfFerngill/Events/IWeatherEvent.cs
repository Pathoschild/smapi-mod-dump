using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClimatesOfFerngillRebuild
{
    interface IWeatherEvent
    {
        event EventHandler<WeatherNotificationArgs> OnUpdateStatus;
        void UpdateStatus(string weather, bool status);
    }
}
