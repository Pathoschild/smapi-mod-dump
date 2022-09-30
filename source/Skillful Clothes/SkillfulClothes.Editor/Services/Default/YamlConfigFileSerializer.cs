/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LunaticShade/StardewValley.SkillfulClothes
**
*************************************************/

using SkillfulClothes.Configuration;
using SkillfulClothes.Editor.ViewModels;
using SkillfulClothes.Effects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillfulClothes.Editor.Services.Default
{
    class YamlConfigFileSerializer : IConfigFileSerializer
    {
        IDialogService DialogService { get; }

        public YamlConfigFileSerializer(IDialogService dialogService)
        {
            DialogService = dialogService;
        }

        public IList<ClothingItemViewModel<T>> Load<T>(Stream stream)
        {
            List<ClothingItemViewModel<T>> resultList = new List<ClothingItemViewModel<T>>();

            CustomEffectConfigurationParser parser = new CustomEffectConfigurationParser();
            var list = parser.Parse(stream);

            foreach(var item in list)
            {
                T itemId = (T)Enum.Parse(typeof(T), item.ItemIdentifier);

                ClothingItemViewModel<T> vm = new ClothingItemViewModel<T>(itemId, DialogService);

                if (item.Effect is EffectSet effectSet)
                {
                    foreach(var effect in effectSet.Effects)
                    {
                        vm.EffectRoot.AttachedEffects.Add(new EffectViewModel(effect));
                    }
                } else
                {
                    vm.EffectRoot.AttachedEffects.Add(new EffectViewModel(item.Effect));
                }                

                resultList.Add(vm);
            }

            return resultList;
        }

        public IList<ClothingItemViewModel<T>> LoadFromFile<T>(string filename)
        {
            using(FileStream fStream = new FileStream(filename, FileMode.Open))
            {
                return Load<T>(fStream);
            }
        }
    }
}
