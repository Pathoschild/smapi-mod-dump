using System;
using System.Collections.Generic;
using StardewModdingAPI;

namespace TehPers.CoreMod.Api.ContentLoading {
    public interface ICoreTranslation {
        /// <inheritdoc cref="Translation.Key"/>
        string Key { get; }

        /// <summary>Whether the translation was found.</summary>
        bool Found { get; }

        /// <inheritdoc cref="Translation.Assert"/>
        ICoreTranslation Assert();

        /// <summary>Replace format specifiers in the text like <c>{0}</c> with formatted values.</summary>
        /// <param name="values">The values to insert into the formatted string.</param>
        ICoreTranslation WithFormatValues(params object[] values);

        /// <inheritdoc cref="Translation.Tokens"/>
        ICoreTranslation WithTokens(object tokens);

        /// <inheritdoc cref="Translation.Tokens"/>
        /// <param name="tokens">The tokens to replace and their associated values.</param>
        ICoreTranslation WithTokens(IDictionary<string, string> tokens);

        /// <inheritdoc cref="Translation.Tokens"/>
        /// <param name="tokenConverter">A callback which converts a token into a value.</param>
        ICoreTranslation WithTokens(Func<string, string> tokenConverter);

        /// <inheritdoc cref="Translation.Default"/>
        ICoreTranslation WithDefault(string @default);

        /// <summary>Converts this translation into a string with all the replacements applied.</summary>
        /// <returns>The translated string with all the replacements.</returns>
        string ToString();
    }
}