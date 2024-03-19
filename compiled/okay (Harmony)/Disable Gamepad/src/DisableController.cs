using Microsoft.Xna.Framework.Utilities;
using System.Reflection;
using Harmony;
namespace Microsoft.Xna.Framework.Input
{
    static class DisableGamePad {
        public static void DisableGamePads() {
            var gp = typeof(GamePad);
            var method = gp.GetMethod("CloseDevices", BindingFlags.Static | BindingFlags.NonPublic);
            if (method!=null) {
                method.Invoke(obj: null, parameters: new object[]{});
            }
            
            var cf = gp.GetField("_connected", BindingFlags.NonPublic |BindingFlags.GetField | BindingFlags.Static);
            var t = gp.GetField("_timeout", BindingFlags.NonPublic |BindingFlags.GetField | BindingFlags.Static);
            if (cf==null || t==null) {
                return;
            }
            
            var cfo = cf.GetValue(obj: null);
            var to = t.GetValue(obj: null);
            if (cfo==null || to==null) {
                return;
            }
            bool[] _connected = (bool[]) cfo;
            long[] _timeout = (long[]) to;
            
            for (int i=0; i<4; i++) {
                
                _connected[i] = false;
                
                _timeout[i] = long.MaxValue - 2;
            }
            
            patchWindows();
        }
        
        private static void patchWindows() {
            
            var harmonyInstance = typeof(HarmonyInstance);
            var harmonyCreate = harmonyInstance.GetMethod("Create");
            var patch = harmonyInstance.GetMethod("Patch");
            var harmony = harmonyCreate.Invoke(null, new object[]{"com.lp-programming.stardewvalley"});
            var disableGPWin = new HarmonyMethod(typeof(DisableGamePad).GetMethod("DisableGPWin"));
            var gps = typeof(GamePadState);
            var isConnected = new HarmonyMethod(gps.GetProperty("IsConnected").GetGetMethod());
            
            
            patch.Invoke(harmony, new object[]{isConnected, disableGPWin, null, null});
            
        }
        
        private static void DisableGPWin(ref bool __result) {
            __result = false;
        }
    }
}
