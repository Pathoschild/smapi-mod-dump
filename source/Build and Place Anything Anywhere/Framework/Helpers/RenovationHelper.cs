/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Smoked-Fish/AnythingAnywhere
**
*************************************************/

using AnythingAnywhere.Framework.Utilities;
using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.HomeRenovations;
using StardewValley.Locations;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using static StardewValley.HouseRenovation;

namespace AnythingAnywhere.Framework.Helpers;
internal static class RenovationHelper
{
    public static void RenovateCabinsResponses()
    {
        var cabinPageNames = CabinUtility.GetCabinsToUpgrade(true);
        Game1.currentLocation.ShowPagedResponses("Renovate Cabin?", cabinPageNames, ShowRenovationMenuAtLocation, true);
    }

    private static void ShowRenovationMenuAtLocation(string cabin)
    {
        var cabinBuilding = Game1.getLocationFromName(cabin).GetContainingBuilding();
        var cabinInstance = (Cabin)cabinBuilding.indoors.Value;

        Game1.activeClickableMenu = new ShopMenu("HouseRenovations", GetAvailableRenovationsForFarmer(cabinInstance.owner), 0, null, OnPurchaseRenovation)
        {
            purchaseSound = null
        };
    }

    #nullable disable
    #pragma warning disable AvoidImplicitNetFieldCast, RCS1163
    public static List<ISalable> GetAvailableRenovationsForFarmer(Farmer owner)
    {
        FarmHouse farmhouse = Game1.RequireLocation<FarmHouse>(owner.homeLocation.Value);
        List<ISalable> available_renovations = [];
        Dictionary<string, HomeRenovation> data = DataLoader.HomeRenovations(Game1.content);
        foreach (string key in data.Keys)
        {
            HomeRenovation renovation_data = data[key];
            bool valid = true;
            foreach (RenovationValue requirement_data in renovation_data.Requirements)
            {
                if (requirement_data.Type == "Value")
                {
                    string requirement_value = requirement_data.Value;
                    bool match = true;
                    if (requirement_value.Length > 0 && requirement_value[0] == '!')
                    {
                        requirement_value = requirement_value[1..];
                        match = false;
                    }
                    int value = int.Parse(requirement_value);
                    try
                    {
                        NetInt field2 = (NetInt)farmhouse.GetType().GetField(requirement_data.Key).GetValue(farmhouse);
                        if (field2 == null)
                        {
                            valid = false;
                            break;
                        }
                        if (field2.Value == value != match)
                        {
                            valid = false;
                            break;
                        }
                    }
                    catch (Exception)
                    {
                        valid = false;
                        break;
                    }
                }
                else if (requirement_data.Type == "Mail" && owner.hasOrWillReceiveMail(requirement_data.Key) != (requirement_data.Value == "1"))
                {
                    valid = false;
                    break;
                }
            }
            if (!valid)
            {
                continue;
            }
            HouseRenovation renovation = new()
            {
                location = farmhouse,
            };
            IReflectedField<string> _nameField = ModEntry.ModHelper.Reflection.GetField<string>(renovation, "_name");
            IReflectedField<string> _displayNameField = ModEntry.ModHelper.Reflection.GetField<string>(renovation, "_displayName");
            IReflectedField<string> _descriptionField = ModEntry.ModHelper.Reflection.GetField<string>(renovation, "_description");
            _nameField.SetValue(key);
            string[] split = Game1.content.LoadString(renovation_data.TextStrings).Split('/');
            try
            {
                _displayNameField.SetValue(split[0]);
                _descriptionField.SetValue(split[1]);
                renovation.placementText = split[2];
            }
            catch (Exception)
            {
                _displayNameField.SetValue("?");
                _descriptionField.SetValue("?");
                renovation.placementText = "?";
            }
            if (renovation_data.CheckForObstructions)
            {
                renovation.validate = (Func<HouseRenovation, int, bool>)Delegate.Combine(renovation.validate, new Func<HouseRenovation, int, bool>(EnsureNoObstructions));
            }
            if (renovation_data.AnimationType == "destroy")
            {
                renovation.animationType = AnimationType.Destroy;
            }
            else
            {
                renovation.animationType = AnimationType.Build;
            }
            renovation.Price = renovation_data.Price;
            renovation.RoomId = ((!string.IsNullOrEmpty(renovation_data.RoomId)) ? renovation_data.RoomId : key);
            if (!string.IsNullOrEmpty(renovation_data.SpecialRect))
            {
                if (renovation_data.SpecialRect == "crib")
                {
                    Rectangle? crib_bounds = farmhouse.GetCribBounds();
                    if (!farmhouse.CanModifyCrib() || !crib_bounds.HasValue)
                    {
                        continue;
                    }
                    renovation.AddRenovationBound(crib_bounds.Value);
                }
            }
            else
            {
                foreach (RectGroup rectGroup in renovation_data.RectGroups)
                {
                    List<Rectangle> rectangles = [];
                    foreach (Rect rect in rectGroup.Rects)
                    {
                        Rectangle rectangle = default;
                        rectangle.X = rect.X;
                        rectangle.Y = rect.Y;
                        rectangle.Width = rect.Width;
                        rectangle.Height = rect.Height;
                        rectangles.Add(rectangle);
                    }
                    renovation.AddRenovationBound(rectangles);
                }
            }
            foreach (RenovationValue renovateAction in renovation_data.RenovateActions)
            {
                RenovationValue action_data = renovateAction;
                if (action_data.Type == "Value")
                {
                    try
                    {
                        NetInt field = (NetInt)farmhouse.GetType().GetField(action_data.Key).GetValue(farmhouse);
                        if (field == null)
                        {
                            valid = false;
                            break;
                        }
                        renovation.onRenovation = (Action<HouseRenovation, int>)Delegate.Combine(renovation.onRenovation, new Action<HouseRenovation, int>(OnRenovation1));

                        void OnRenovation1(HouseRenovation selectedRenovation, int index)
                        {
                            if (action_data.Value == "selected")
                            {
                                field.Value = index;
                            }
                            else
                            {
                                int value2 = int.Parse(action_data.Value);
                                field.Value = value2;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        valid = false;
                        break;
                    }
                }
                else if (action_data.Type == "Mail")
                {
                    renovation.onRenovation = (Action<HouseRenovation, int>)Delegate.Combine(renovation.onRenovation, new Action<HouseRenovation, int>(OnRenovation2));
                }
                void OnRenovation2(HouseRenovation selectedRenovation, int index)
                {
                    if (action_data.Value == "0")
                    {
                        owner.mailReceived.Remove(action_data.Key);
                    }
                    else
                    {
                        owner.mailReceived.Add(action_data.Key);
                    }
                }
            }
            if (valid)
            {
                renovation.onRenovation = (Action<HouseRenovation, int>)Delegate.Combine(renovation.onRenovation, (Action<HouseRenovation, int>)delegate
                {
                    farmhouse.UpdateForRenovation();
                });
                available_renovations.Add(renovation);
            }
        }
        return available_renovations;
    }
}