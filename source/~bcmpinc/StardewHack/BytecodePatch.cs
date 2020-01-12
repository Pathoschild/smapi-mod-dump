using Harmony;
using System;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Linq;

namespace StardewHack
{
    /// <summary>
    /// Indicates that this is a transpiler for the given method.
    /// Can be used multiple times to patch multiple methods.
    /// </summary>
    [Obsolete]
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
        /// </summary>
        public MethodBase GetMethod() 
        {
            return new MethodParser(sig).ParseMethod();
        }
        
        public string GetSignature() {
            return sig;
        }
    }
    
    /// <summary>
    /// Retrieves the method with the given signature.
    /// Methods are specified with the pattern "fully.qualified.type::function_name". 
    /// In case of overloading, argument types must be specified, for example: "fully.qualified.type::function_name(fully.qualified.argument.type1,fully.qualified.argument.type2)"
    /// Constructors are specified using the magic method name ".ctor".
    /// For inner types, specify the path seperated by '/'.
    /// Generic types are supported using the parameter types enclosed in &lt; and >.
    /// </summary>
    public class MethodParser {
        static Regex split = new Regex("([(,)/<>]|::)");
        static Assembly sdv = AppDomain.CurrentDomain.GetAssemblies().First(x => x.FullName == "StardewValley");
        
        readonly string sig;
        readonly string[] tokens;
        int position;
        
        public MethodParser(string sig) {
            this.sig = sig;
            position = 0;
            // Tokenize the signature.
            tokens = split.Split(sig).Where(s => s != string.Empty).ToArray();
        }
        
        public MethodBase ParseMethod() {
            // Parse & retrieve the base type.
            Type type = ParseType();
            expectToken("::");
            
            // Parse the method name
            string name = getToken("method name");
            
            // Parse parameters, if specified.
            Type[] parameters = null;
            if (!end()) {
                expectToken("(");
                parameters = ParseTypes();
                expectToken(")");
            }
            
            // Retrieve the method.
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
        
        // Parse a comma separated list of Types.
        internal Type[] ParseTypes() {
            System.Collections.Generic.List<Type> ret = new System.Collections.Generic.List<Type>();
            while (!end() && !split.IsMatch(tokens[position])) {
                ret.Add(ParseType());
                if (!end() && tokens[position] == ",") position++;
            }
            return ret.ToArray();
        }

        internal Type ParseType() {
            string name = getToken("type");
            System.Collections.Generic.List<Type> generic_parameters = new System.Collections.Generic.List<Type>();
            TryParseGenericParameters(ref name, ref generic_parameters);
            //Type res = AccessTools.TypeByName(name);
            Type res = Type.GetType(name, false);
            if (res == null) {
                res = sdv.GetType(name, false);
            }
            if (res == null) {
                throw new TypeAccessException($"Type \"{name}\" not found.");
            }
            // Recursively retrieve the inner type (if any)
            while (!end() && tokens[position] == "/") {
                position++;
                name = getToken("inner type");
                TryParseGenericParameters(ref name, ref generic_parameters);
                res = AccessTools.Inner(res, name);
                if (res == null) {
                    throw new TypeAccessException($"Inner-type \"{name}\" not found.");
                }
            }
            // Apply generic type arguments (if any)
            if (generic_parameters.Count > 0) {
                res = res.MakeGenericType(generic_parameters.ToArray());
            }
            return res;
        }
        
        // Parse the generic parameters of a type, if available, and update the name accordingly.
        internal void TryParseGenericParameters(ref string name, ref System.Collections.Generic.List<Type> generic_parameters) {
            if (end() || tokens[position] != "<") return;
            position++;
            var par = ParseTypes();
            expectToken(">");
            name += "`" + par.Length;
            generic_parameters.AddRange(par);
        }
        
        // Retrieve the next token.
        internal string getToken(string what) {
            if (end()) {
                throw new FormatException($"Unexpected end of signature, expected '{what}'.");
            }
            return tokens[position++];
        }

        // Parse a token and check whether it is the expected token.
        // Assumes position > 0.
        internal void expectToken(string what) {
            string tok = getToken(what);
            if (tok != what) {
                throw new FormatException($"Unexpected token '{tok}', expected '{what}' after '{tokens[position-2]}'.");
            }
        }
        
        internal bool end() {
            return position >= tokens.Length;
        }
    }
}

