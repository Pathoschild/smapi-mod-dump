
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace CustomEmojis.Framework.Utilities {

    public class ModUtilities {

        public static string GetParentFolder(string path) {
            string parentFolder = "";
            try {
                parentFolder = GetParentFolder(Path.GetDirectoryName(path), path);
            } catch(ArgumentException) {
            }
            return parentFolder;
        }

        private static string GetParentFolder(string path, string lastPath) {
            if(!String.IsNullOrWhiteSpace(path)) {
                lastPath = path;
                return GetParentFolder(Path.GetDirectoryName(path), lastPath);
            } else {
                return lastPath;
            }
        }

        /// <summary>
        ///  Return a <code>IEnumerable<string></code> of file path that matches at least one of the patters. The search is executed in parallel.
        /// </summary>
        public static IEnumerable<string> GetFiles(string path, SearchOption searchOption = SearchOption.TopDirectoryOnly, string[] searchPatterns = null) {
            searchPatterns = searchPatterns ?? new string[] { "*" };
            Directory.EnumerateFiles(path, "*.*", searchOption).Where(s => searchPatterns.Any(e => s.EndsWith(e, StringComparison.OrdinalIgnoreCase)));
            return searchPatterns.AsParallel().SelectMany(searchPattern => Directory.EnumerateFiles(path, "*.*", searchOption).Where(s => s.EndsWith(searchPattern)));
        }

        public static string GetFileHash(string filePath) {

            using(HashAlgorithm hashAlgorithm = SHA256.Create()) {

                using(FileStream stream = File.OpenRead(filePath)) {
                    byte[] hash = hashAlgorithm.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }

            }

        }

        public static string GetHash(Stream stream) {

            using(HashAlgorithm hashAlgorithm = SHA256.Create()) {
                byte[] hash = hashAlgorithm.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }

        }

        internal static string[] BreakLines(string str) {
            return str.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
        }

        public static Dictionary<string, string> GetFolderFilesHash(string path, SearchOption searchOption = SearchOption.TopDirectoryOnly, string[] searchPatterns = null) {

            searchPatterns = searchPatterns ?? new string[] { "*" };

            // Search files matching the pattern and return it ordered by name
            IEnumerable<string> foundFiles = searchPatterns.AsParallel().SelectMany(searchPattern => Directory.EnumerateFiles(path, "*.*", searchOption).Where(s => s.EndsWith(searchPattern))).OrderBy(x => x);

            Dictionary<string, string> fileHashDictionary = new Dictionary<string, string>();

            using(HashAlgorithm hashAlgorithm = SHA256.Create()) {
                foreach(string file in foundFiles.ToList()) {
                    byte[] fileHash = hashAlgorithm.ComputeHash(File.ReadAllBytes(file));
                    string hashValue = BitConverter.ToString(fileHash).Replace("-", "").ToLowerInvariant();
                    if(!fileHashDictionary.ContainsKey(hashValue)) {
                        fileHashDictionary[hashValue] = file;
                    }
                }
            }

            return fileHashDictionary;
        }

        public static string GetFolderHash(string path, SearchOption searchOption = SearchOption.TopDirectoryOnly, string[] searchPatterns = null) {

            searchPatterns = searchPatterns ?? new string[] { "*" };

            // Search files matching the pattern and return it ordered by name
            IEnumerable<string> foundFiles = searchPatterns.AsParallel().SelectMany(searchPattern => Directory.EnumerateFiles(path, "*.*", searchOption).Where(s => s.EndsWith(searchPattern))).OrderBy(x => x);

            var temp = GetFolderFilesHash(path, searchOption, searchPatterns);

            List<string> fileList = foundFiles.ToList();

            using(HashAlgorithm hashAlgorithm = SHA256.Create()) {

                for(int i = 0; i < fileList.Count; i++) {

                    string file = fileList[i];

                    // Hash path
                    string relativePath = file.Substring(path.Length + 1);
                    byte[] pathBytes = Encoding.UTF8.GetBytes(relativePath.ToLower());
                    hashAlgorithm.TransformBlock(pathBytes, 0, pathBytes.Length, pathBytes, 0);

                    // Hash contents
                    byte[] contentBytes = File.ReadAllBytes(file);
                    if(i == fileList.Count - 1) {
                        hashAlgorithm.TransformFinalBlock(contentBytes, 0, contentBytes.Length);
                    } else {
                        hashAlgorithm.TransformBlock(contentBytes, 0, contentBytes.Length, contentBytes, 0);
                    }
                }

                return BitConverter.ToString(hashAlgorithm.Hash).Replace("-", "").ToLower();
            }

        }

        /// <summary>
        /// Get a relative path from one file or folder to another.
        /// </summary>
        /// <param name="fromPath">Contains the directory that defines the start of the relative path.</param>
        /// <param name="toPath">Contains the path that defines the endpoint of the relative path.</param>
        /// <returns>The relative path from the start directory to the end path or <c>toPath</c> if the paths are not related.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="UriFormatException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static String GetRelativePath(String fromPath, String toPath) {

            if(String.IsNullOrEmpty(fromPath)) {
                throw new ArgumentNullException("fromPath");
            }

            if(String.IsNullOrEmpty(toPath)) {
                throw new ArgumentNullException("toPath");
            }

            fromPath += Path.DirectorySeparatorChar;

            Uri fromUri = new Uri(fromPath);
            Uri toUri = new Uri(toPath);

            if(fromUri.Scheme != toUri.Scheme) { // path can't be made relative.
                return toPath;
            }

            Uri relativeUri = fromUri.MakeRelativeUri(toUri);
            String relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            if(toUri.Scheme.Equals("file", StringComparison.InvariantCultureIgnoreCase)) {
                relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            }

            return relativePath;
        }

        /*
		/// <summary>
		/// Construct a derived class of from a base class
		/// </summary>
		/// <typeparam name="F">Type of base class</typeparam>
		/// <typeparam name="T">Type of class you want</typeparam>
		/// <param name="baseClass">the instance of the base class</param>
		/// <returns></returns>
		public static T Construct<T>(Type baseClassType, object baseClassInstance) where T : new() {

			// Create derived instance
			T derived = new T();

			if(baseClassInstance.GetType().IsSubclassOf(baseClassType)) {

				// Get all base class properties
				PropertyInfo[] properties = baseClassInstance.GetType().GetProperties();

				foreach(PropertyInfo basePropertyInfo in properties) {

					// Get derived matching property
					PropertyInfo derivedPropertyInfo = typeof(T).GetProperty(basePropertyInfo.Name, basePropertyInfo.PropertyType);

					// this property must not be index property
					if(derivedPropertyInfo != null && derivedPropertyInfo.GetSetMethod() != null && basePropertyInfo.GetIndexParameters().Length == 0 && derivedPropertyInfo.GetIndexParameters().Length == 0) {
						derivedPropertyInfo.SetValue(derived, derivedPropertyInfo.GetValue(baseClassInstance, null), null);
					}
				}

			}

			return derived;
		}
		*/

        /// <summary>
        /// Perform a deep Copy of the object, using Json as a serialisation method. NOTE: Private members are not cloned using this method.
        /// </summary>
        /// <typeparam name="T">The type of object being copied.</typeparam>
        /// <param name="source">The object instance to copy.</param>
        /// <returns>The copied object.</returns>
        public static T CloneJson<T>(T source) {

            // Don't serialize a null object, simply return the default for that object
            if(source == null) {
                return default(T);
            }

            // initialize inner objects individually
            // for example in default constructor some list property initialized with some values,
            // but in 'source' these items are cleaned -
            // without ObjectCreationHandling.Replace default constructor values will be added to result
            var deserializeSettings = new JsonSerializerSettings {
                ObjectCreationHandling = ObjectCreationHandling.Replace
            };

            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source), deserializeSettings);
        }

        // Source: https://www.codeproject.com/Articles/42221/Constructing-an-instance-class-from-its-base-class
        /// <summary>
        /// Construct a derived class of from a base class
        /// </summary>
        /// <typeparam name="F">Type of base class</typeparam>
        /// <typeparam name="T">Type of class you want</typeparam>
        /// <param name="baseClass">the instance of the base class</param>
        /// <returns></returns>
        public static T Construct<F, T>(F baseClass) where T : F, new() {

            // Create derived instance
            T derived = new T();

            // Get all base class properties
            PropertyInfo[] properties = typeof(F).GetProperties();

            foreach(PropertyInfo basePropertyInfo in properties) {

                // Get derived matching property
                PropertyInfo derivedPropertyInfo = typeof(T).GetProperty(basePropertyInfo.Name, basePropertyInfo.PropertyType);

                // this property must not be index property
                if(derivedPropertyInfo != null && derivedPropertyInfo.GetSetMethod() != null && basePropertyInfo.GetIndexParameters().Length == 0 && derivedPropertyInfo.GetIndexParameters().Length == 0) {
                    derivedPropertyInfo.SetValue(derived, derivedPropertyInfo.GetValue(baseClass, null), null);
                }
            }

            return derived;
        }

        // Based in the solution made by Routine.
        // Source: https://github.com/Platonymous/Stardew-Valley-Mods/blob/4b2d4bb933603ef81d9a03e431038b8ec0ebf420/PyTK/PyUtils.cs#L140
        /// <summary>
        /// Get the correct Stardew Valley object type for the different system assembly name.
        /// <param name="type">Name of type</param>
        /// <returns>The object type</returns>
        public static Type GetSDVType(string type) {
            const string prefix = "StardewValley.";
            return Type.GetType(prefix + type + ", Stardew Valley") ?? Type.GetType(prefix + type + ", StardewValley");
        }

    }

}
