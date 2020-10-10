/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/GilarF/SVM
**
*************************************************/

using System.Collections.Generic;

namespace ModSettingsTab.Framework.Integration
{
    public class ModIntegrationSettings
    {
        public I18N Description { get; set; } = new I18N();
        public List<Param> Config { get; set; } = new List<Param>();
    }
}