/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://sourceforge.net/p/sdvmods-artifact-fix-redux/
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Harmony;

using PatcherHelper.Audit;

namespace PatcherHelper
    {
    public static class SearchFor
        {
        /// <summary>
        /// Search for a method returning <c>IEnumerator</c> within a given Type
        /// </summary>
        /// <param name="type">The type to search in</param>
        /// <param name="methodName">The name of the method to patch</param>
        /// <returns>The MethodInfo ready to pass through Harmony's .Patch</returns>
        public static MethodInfo IEnumerator(Type type, string methodName) {
            var containing_member =
                type
                .GetMembers(AccessTools.all)
                .Where(memb => memb.Name.Contains($"<{methodName}>"))
                .First()
                ;
            var method_info =
                (containing_member as Type)
                .GetMembers(AccessTools.all)
                .Where(memb => memb.MemberType == MemberTypes.Method && memb.Name == "MoveNext")
                .First()
                ;
            return (method_info as MethodInfo);
            }

        }

    public static class Dumper
        {
        private static int _DumpStart = 0;
        private static int _DumpLength = 50;
        private static List<string> _Dumped;

        public static HarmonyMethod GetTranspiler(ref List<string> dumpTo, int startAt = 0, int length = 50) {
            _DumpStart = startAt;
            _DumpLength = length;
            _Dumped = dumpTo;
            return new HarmonyMethod(typeof(Dumper), nameof(_Dumper_transp));
            }

        public static IEnumerable<CodeInstruction> _Dumper_transp(IEnumerable<CodeInstruction> oldInstructions) {
            var walker = new InstructionsWalker(oldInstructions);
            walker
                .GoForward(_DumpStart)
                .GoDumpAddTo(ref _Dumped, N: _DumpLength)
                ;
            return walker;
            }

        }

    public static class TypeExtensions
        {
        /// <summary>
        /// Produce a <c>MethodInfo</c> for a method declared as <c>IEnumerator</c> within a given type
        /// </summary>
        /// <param name="type">The <c>Type</c> to search within</param>
        /// <param name="methodName">The name of the method being sought</param>
        /// <returns>A <c>MethodInfo</c> that points to the logic of the IEnumerator</returns>
        public static MethodInfo Grab_IEnumerator(this Type type, string methodName) {
            return SearchFor.IEnumerator(type, methodName);
            }
        }

    public static class Auditor
        {
        public static SortedDictionary<MethodBase, List<PatchDatum>> MethodPatches =
            new SortedDictionary<MethodBase, List<PatchDatum>>(new MethodBaseComparer());

        /// <summary>
        /// Extension method to HarmonyInstance, used to record the state of patching when invoked
        /// </summary>
        /// <param name="harmonyInstance">An instance of HarmonyInstance</param>
        /// <param name="method">The method to audit</param>
        public static void AuditPatches(this HarmonyInstance harmonyInstance, MethodBase method) {
            if (!MethodPatches.TryGetValue(method, out var patchlist)) {
                patchlist = new List<PatchDatum>();
                }
            patchlist.Add(new PatchDatum(harmonyInstance.GetPatchInfo(method)));
            MethodPatches[method] = patchlist;
            }
        }
    }

namespace PatcherHelper.Audit
    {
    public class PatchDatum
        {
        public readonly DateTime DateTime = DateTime.UtcNow;
        public readonly Patches Patches;
        public PatchDatum(Patches patches) {
            Patches = patches;
            }
        }
    internal class MethodBaseComparer : IComparer<MethodBase>
        {
        public int Compare(MethodBase x, MethodBase y) {
            return x.Name.CompareTo(y.Name);
            }
        }
    }
