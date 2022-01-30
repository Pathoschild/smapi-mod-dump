/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mus-candidus/ImagEd
**
*************************************************/

using System;
using System.Linq;
using System.Security.Cryptography;

using Microsoft.Xna.Framework;


namespace ImagEd.Framework {
    /// <summary>Handles arguments of <see cref="RecolorToken"/>.</summary>
    internal class RecolorTokenArguments {
        public string ContentPackName { get; private set; }
        public string AssetName { get; private set; }
        public string SourcePath { get; private set; }
        public Color BlendColor { get; private set; }
        public string MaskPath { get; private set; }
        public Desaturation.Mode DesaturationMode { get; private set; }
        public Flip.Mode FlipMode { get; private set; }
        public float Brightness { get; private set; }

        /// <inheritdoc />
        public override bool Equals(object obj) {
            if (!(obj is RecolorTokenArguments args)) {
                return false;
            }

            return this.ContentPackName.Equals(args.ContentPackName)
                && this.AssetName.Equals(args.AssetName)
                && this.SourcePath.Equals(args.SourcePath)
                && this.BlendColor.Equals(args.BlendColor)
                && this.MaskPath.Equals(args.MaskPath)
                && this.DesaturationMode.Equals(args.DesaturationMode)
                && this.FlipMode.Equals(args.FlipMode)
                && this.Brightness.Equals(args.Brightness);
        }

        /// <inheritdoc />
        public override int GetHashCode() {
            // Sufficient as long as we don't need performance.
            return Tuple.Create(this.ContentPackName,
                                this.AssetName,
                                this.SourcePath,
                                this.BlendColor,
                                this.MaskPath,
                                this.DesaturationMode,
                                this.FlipMode,
                                this.Brightness)
                        .GetHashCode();
        }

        ///<summary> Returns a unique suffix to identify the file in different runs of SDV.</summary>
        public string GetUniqueFileSuffix() {
            // ATTENTION: We can't persist the result of GetHashCode() because it's not consistent between several runs of a program.
            // This is by design and a security feature of since the .NET Core 1.0, see
            // https://docs.microsoft.com/en-us/dotnet/api/system.string.gethashcode?view=netcore-1.0
            // To get consistent results required to identify the file on next run we use MD5 over all token arguments.
            string argsInfo = $"{this.ContentPackName}"
                            + $"{this.AssetName}"
                            + $"{this.SourcePath}"
                            + $"{this.BlendColor}"
                            + $"{this.MaskPath}"
                            + $"{this.DesaturationMode}"
                            + $"{this.FlipMode}"
                            + $"{this.Brightness}";

            using (MD5 md5algorithm = MD5.Create()) {
                byte[] argsMd5 = md5algorithm.ComputeHash(System.Text.Encoding.UTF8.GetBytes(argsInfo));

                return string.Concat(argsMd5.Select(c => $"{c:x2}"));
            }
        }

        /// <summary>Parse JSON string and return token arguments.</summary>
        public static RecolorTokenArguments Parse(string input) {
            if (string.IsNullOrWhiteSpace(input)) {
                throw new ArgumentException("Argument list required");
            }

            string[] tempInput = input.Split(',');

            if (tempInput.Length < 5) {
                throw new ArgumentException($"Wrong number of items in argument list, at least 5 required, {tempInput.Length} found");
            }

            return new RecolorTokenArguments {
                ContentPackName = tempInput[0].Trim(),
                AssetName = tempInput[1].Trim(),
                SourcePath = tempInput[2].Trim(),
                MaskPath = tempInput[3].Trim(),
                BlendColor = Utility.ColorFromHtml(tempInput[4].Trim()),
                DesaturationMode = tempInput.Length > 5
                                 ? Desaturation.ParseEnum(tempInput[5].Trim())
                                 : Desaturation.Mode.None,
                FlipMode = tempInput.Length > 6
                         ? Flip.ParseEnum(tempInput[6].Trim())
                         : Flip.Mode.None,
                Brightness = tempInput.Length > 7
                           ? float.Parse(tempInput[7].Trim(), System.Globalization.CultureInfo.InvariantCulture)
                           : 1.0f
            };
        }
    }
}
