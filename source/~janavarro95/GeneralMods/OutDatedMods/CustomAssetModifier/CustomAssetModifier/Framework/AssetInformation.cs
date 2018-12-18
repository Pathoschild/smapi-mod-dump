using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomAssetModifier.Framework
{
    /// <summary>
    /// Used to easily store Dictionary key pair values of (string,string) that are commonly used in .xnb files.
    /// </summary>
    public class AssetInformation
    {
        public string id; //
        public string informationString;

        /// <summary>
        /// Blank constructor.
        /// </summary>
        public AssetInformation()
        {

        }

        /// <summary>
        /// Normal constructor.
        /// </summary>
        /// <param name="ID">The id key used in the asset file. Aslo will be the file's name.</param>
        /// <param name="DataString">The data string that is to be set after the edit.</param>
        public AssetInformation(string ID, string DataString)
        {
            this.id = ID;
            this.informationString = DataString;
        }

        /// <summary>
        /// Write the information to a .json file.
        /// </summary>
        /// <param name="path"></param>
        public void writeJson(string path)
        {
            CustomAssetModifier.ModHelper.WriteJsonFile(path, this);
        }

        /// <summary>
        /// Read the information from a .json file and return an instance of AssetInformation.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static AssetInformation readJson(string path)
        {
            return CustomAssetModifier.ModHelper.ReadJsonFile<AssetInformation>(path);
        }
    }

}
