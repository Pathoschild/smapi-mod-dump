/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Denifia/StardewMods
**
*************************************************/

using Microsoft.AspNetCore.Mvc;

namespace Denifia.Stardew.SendItemsApi.Controllers
{
    [Route("api/[controller]")]
    public class HealthController : Controller
    {
        // GET api/health
        [HttpGet()]
        public bool Get()
        {
            return true;
        }
    }
}
