/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/greyivy/OrnithologistsGuild
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using DynamicGameAssets.Game;
using DynamicGameAssets.PackData;
using OrnithologistsGuild.Content;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace OrnithologistsGuild.Game.Items
{
    [XmlType("Mods_Ivy_OrnithologistsGuild_LifeList")]
    public class LifeList : CustomObject
    {
        private const int PAGE_SIZE = 5;
        private const string ACTION_NEXT = "ACTION_NEXT";
        private const string ACTION_CLOSE = "ACTION_CLOSE";

        public LifeList() : base((ObjectPackData)ModEntry.DGAContentPack.Find("LifeList"))
        {
            var dataPack = ModEntry.DGAContentPack.Find("LifeList");

            this.name = $"{dataPack.ID}_Subclass";
        }

        private void drawBirdieList(Models.LifeList lifeList, List<Response> choices, int page = 1)
        {
            var totalPages = Math.Ceiling((decimal)choices.Count / PAGE_SIZE);

            var title = I18n.Items_LifeList_Title(playerName: Game1.player.Name, identified: lifeList.IdentifiedCount, total: ContentPackManager.BirdieDefs.Count);
            var pagination = I18n.Items_LifeList_Pagination(page: page, total: totalPages);

            var action = new GameLocation.afterQuestionBehavior((_, choice) => {
                if (choice.Equals(ACTION_NEXT)) drawBirdieList(lifeList, choices, page + 1);
                else if (choice.Equals(ACTION_CLOSE)) { }
                else drawBirdieDialogue(ContentPackManager.BirdieDefs[choice], lifeList[choice]);
            });

            var pageChoices = choices.Skip((page - 1) * PAGE_SIZE).Take(PAGE_SIZE).ToList();
            if (page < totalPages) pageChoices.Add(new Response(ACTION_NEXT, I18n.Items_LifeList_ActionNext()));
            pageChoices.Add(new Response(ACTION_CLOSE, I18n.Items_LifeList_ActionClose()));

            Game1.currentLocation.createQuestionDialogue($"{title}^{pagination}", pageChoices.ToArray(), action);
        }

        private void drawBirdieDialogue(BirdieDef birdieDef, Models.LifeListEntry lifeListEntry)
        {
            var id = birdieDef.ID;

            var contentPack = birdieDef.ContentPackDef.ContentPack;

            // Translations
            var commonNameString = contentPack.Translation.Get($"birdie.{id}.commonName");
            var scientificNameString = contentPack.Translation.Get($"birdie.{id}.scientificName");
            var funFactString = contentPack.Translation.Get($"birdie.{id}.funFact");
            var attributeStrings = Enumerable.Range(1, birdieDef.Attributes).ToDictionary(i => i, i => contentPack.Translation.Get($"birdie.{id}.attribute.{i}"));

            var lines = new List<string>();

            lines.Add(Utilities.LocaleToUpper(commonNameString.ToString()));
            if (scientificNameString.HasValue()) lines.Add(scientificNameString.ToString());

            lines.Add(string.Empty);
            foreach (var sighting in lifeListEntry.Sightings)
            {
                var date = SDate.FromDaysSinceStart(sighting.DaysSinceStart);
                var location = Game1.getLocationFromName(sighting.LocationName).Name;
                var attribute = attributeStrings[sighting.Attribute];

                if (lifeListEntry.Sightings.IndexOf(sighting) == lifeListEntry.Sightings.Count - 1)
                {
                    lines.Add(I18n.Items_LifeList_Identified(date, location, attribute));
                } else
                {
                    lines.Add(I18n.Items_LifeList_Sighted(date, location, attribute));
                }
            }

            if (funFactString.HasValue())
            {
                lines.Add(string.Empty);
                lines.Add(funFactString);
            }

            Game1.drawObjectDialogue(string.Join("^", lines));
        }

        public override bool performUseAction(GameLocation location)
        {
            var lifeList = SaveDataManager.SaveData.LifeList;

            if (lifeList.IdentifiedCount > 0)
            {
                var identified = ContentPackManager.BirdieDefs.Values.Where(birdieDef => lifeList.ContainsKey(birdieDef.UniqueID) && lifeList[birdieDef.UniqueID].Identified).ToList();

                List<Response> choices = identified
                    .OrderBy(birdieDef =>
                    {
                        var id = birdieDef.ID;
                        var contentPack = birdieDef.ContentPackDef.ContentPack;

                        return contentPack.Translation.Get($"birdie.{id}.commonName").ToString();
                    }).Select(birdieDef =>
                    {
                        var id = birdieDef.ID;
                        var contentPack = birdieDef.ContentPackDef.ContentPack;

                        var commonNameString = contentPack.Translation.Get($"birdie.{id}.commonName");
                        var scientificNameString = contentPack.Translation.Get($"birdie.{id}.scientificName");

                        var commonName = Utilities.LocaleToUpper(commonNameString.ToString());

                        if (scientificNameString.HasValue()) return new Response(birdieDef.UniqueID, $"{commonName} ({scientificNameString})");
                        else return new Response(birdieDef.UniqueID, commonName);
                    }).ToList();

                drawBirdieList(lifeList, choices, 1);
            }
            else
            {
                var title = I18n.Items_LifeList_Title(playerName: Game1.player.Name, identified: 0, total: ContentPackManager.BirdieDefs.Count);
                Game1.drawObjectDialogue($"{title}^{I18n.Items_LifeList_Empty()}^^{I18n.Items_LifeList_EmptyTip()}");
            }

            return false;
        }

        public override Item getOne()
        {
            var ret = new LifeList();
            ret.Quality = this.Quality;
            ret.Stack = 1;
            ret.Price = this.Price;
            ret.ObjectColor = this.ObjectColor;
            ret._GetOneFrom(this);
            return ret;
        }

        public override bool canStackWith(ISalable other)
        {
            return false;
        }
    }
}
