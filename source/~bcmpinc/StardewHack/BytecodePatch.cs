using Harmony;
using System;
using System.Reflection;

namespace StardewHack
{
    /// <summary>
    /// Indicates that this is a transpiler for the given method.
    /// Can be used multiple times to patch multiple methods.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = true)]  
    public class BytecodePatch : System.Attribute  
    {
        readonly string sig;
        readonly string enabled;

        public bool IsEnabled(HackBase hack)
        {
            if (enabled == null) return true;
            var method = AccessTools.Method(hack.GetType(), enabled);
            return (bool)method.Invoke(hack, null);
        }

        public BytecodePatch(string sig, string enabled=null)
        {
            this.sig = sig;
            this.enabled = enabled;
        }

        /// <summary>
        /// Returns a reference to the method or constructor specified by this BytecodePatch Attribute.
        /// Methods are specified with the pattern "fully.qualified.type::function_name". 
        /// In case of overloading, argument types must be specified, for example: "fully.qualified.type::function_name(fully.qualified.argument.type1,fully.qualified.argument.type2)"
        /// Constructors are specified using the magic method name ".ctor".
        /// </summary>
        public MethodBase GetMethod() 
        {
            string[] a = sig.Split(':','(',',',')');
            // Hack.Log("Signature: " + String.Join(";", a));

            // Get type
            Type type = GetType(a[0]);
            if (a.Length < 3 || a[1].Length > 0) throw new Exception($"Expected \"::\" between class and method name, but got \"{sig}\".");

            // Get name
            string name = a[2];

            // Check for arguments.
            Type[] parameters = null;
            if (a.Length > 3) {
                if (a[3].Trim().Length == 0) {
                    // Empty argument list
                    if (a.Length != 5) throw new Exception($"Expected \"()\" for method without arguments, but got \"{sig}\".");
                    parameters = new Type[0];
                } else {
                    if (a[a.Length-1].Length > 0) throw new Exception($"Expected parentheses around method arguments, but got \"{sig}\".");
                    parameters = new Type[a.Length-4];
                    for (int i=0; i<parameters.Length; i++) {
                        parameters[i] = GetType(a[i+3]);
                    }
                }
            }
            MethodBase method;
            if (name == ".ctor") {
                if (parameters == null) {
                    // If no parameters are specified, assume there is a unique constructor.
                    var array = type.GetConstructors();
                    if (array.Length != 1) {
                        throw new Exception($"Found {array.Length} matching constructors for \"{sig}\".");
                    }
                    method = array[0];
                } else {
                    method = AccessTools.DeclaredConstructor(type, parameters);
                }
                if (method == null) {
                    throw new Exception($"Failed to find constructor \"{sig}\".");
                }
            } else {
                method = AccessTools.DeclaredMethod(type, name, parameters);
                if (method == null) {
                    throw new Exception($"Failed to find method \"{sig}\".");
                }
            }
            return method;
        }

        /// <summary>
        /// Retrieves the type definition with the specified name.
        /// For inner types, specify the path seperated by '/'.
        /// </summary>
        internal static Type GetType(string type) {
            string[] a = type.Trim().Split('/');
            
            // Retrieve the base type.
            var res = AccessTools.TypeByName(a[0]);
            if (res == null) {
                throw new TypeAccessException($"ERROR: type \"{a[0]}\" not found.");
            }
            
            // Recursively retrieve the inner type (if any)
            for (int i=1; i<a.Length; i++) {
                res = AccessTools.Inner(res, a[i]);
                if (res == null) {
                    throw new TypeAccessException($"ERROR: sub-type \"{a[i]}\" not found.");
                }
            }
            return res;
        }
    }
}

