/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using Namotion.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Ninject;
using Ninject.Activation;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using TehPers.Core.Api.Setup;
using TehPers.Core.Json;

namespace TehPers.Core.Setup
{
    [SuppressMessage(
        "ReSharper",
        "InconsistentNaming",
        Justification = "Harmony has a specific naming convention."
    )]
    internal class NewtonsoftPatcher : Patcher
    {
        private static NewtonsoftPatcher? Instance { get; set; }

        private static readonly ConditionalWeakTable<JsonProperty, MemberInfo> memberInfos = new();

        private NewtonsoftPatcher(Harmony harmony)
            : base(harmony)
        {
        }

        public static NewtonsoftPatcher Create(IContext context)
        {
            NewtonsoftPatcher.Instance ??= new(context.Kernel.Get<Harmony>());
            return NewtonsoftPatcher.Instance;
        }

        public override void Setup()
        {
            // Newtonsoft.Json.Serialization.DefaultContractResolver::CreateProperty
            this.Patch(
                AccessTools.Method(typeof(DefaultContractResolver), "CreateProperty"),
                postfix: new(
                    AccessTools.Method(
                        typeof(NewtonsoftPatcher),
                        nameof(NewtonsoftPatcher.DefaultContractResolver_CreateProperty_Postfix)
                    )
                )
            );

            // Newtonsoft.Json.Serialization.JsonSerializerInternalWriter::CalculatePropertyValues
            this.Patch(
                AccessTools.Method(
                    AccessTools.TypeByName(
                        "Newtonsoft.Json.Serialization.JsonSerializerInternalWriter"
                    ),
                    "CalculatePropertyValues"
                ),
                postfix: new(
                    AccessTools.Method(
                        typeof(NewtonsoftPatcher),
                        nameof(NewtonsoftPatcher
                            .JsonSerializerInternalWriter_CalculatePropertyValues_Postfix)
                    )
                )
            );
        }

        private static void DefaultContractResolver_CreateProperty_Postfix(
            JsonProperty __result,
            MemberInfo member
        )
        {
            NewtonsoftPatcher.memberInfos.AddOrUpdate(__result, member);
        }

        private static void JsonSerializerInternalWriter_CalculatePropertyValues_Postfix(
            bool __result,
            JsonWriter writer,
            object value,
            JsonProperty property
        )
        {
            if (!__result)
            {
                return;
            }

            if (writer is not DescriptiveJsonWriter descriptiveWriter)
            {
                return;
            }

            if (!NewtonsoftPatcher.memberInfos.TryGetValue(property, out var memberInfo))
            {
                return;
            }

            var type = value.GetType().ToContextualType();
            var description = memberInfo switch
            {
                PropertyInfo {Name: var name} when type.GetProperty(name) is { } p =>
                    NewtonsoftPatcher.GetDescription(p),
                FieldInfo {Name: var name} when type.GetField(name) is { } f => NewtonsoftPatcher
                    .GetDescription(f),
                _ => null,
            };
            if (description is not null)
            {
                descriptiveWriter.WritePropertyComment(description);
            }
        }

        private static string? GetDescription(ContextualMemberInfo contextualMemberInfo)
        {
            var description = contextualMemberInfo.MemberInfo
                .GetCustomAttribute<DescriptionAttribute>()
                ?.Description;
            description ??= contextualMemberInfo.GetXmlDocsSummary().Replace("\n", " ");
            return string.IsNullOrEmpty(description) ? null : description;
        }
    }
}
