/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

namespace Comics
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public partial class IssuesRequest
    {
        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("limit")]
        public long Limit { get; set; }

        [JsonProperty("offset")]
        public long Offset { get; set; }

        [JsonProperty("number_of_page_results")]
        public long NumberOfPageResults { get; set; }

        [JsonProperty("number_of_total_results")]
        public long NumberOfTotalResults { get; set; }

        [JsonProperty("status_code")]
        public long StatusCode { get; set; }

        [JsonProperty("results")]
        public List<Issue> Results { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }
    }

    public partial class Issue
    {
        
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("image")]
        public Image Image { get; set; }

        [JsonProperty("issue_number")]
        public string IssueNumber { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("volume")]
        public Volume Volume { get; set; }
    }

    public partial class Image
    {

        [JsonProperty("medium_url")]
        public Uri MediumUrl { get; set; }

        [JsonProperty("small_url")]
        public Uri SmallUrl { get; set; }
    }

    public partial class Volume
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
