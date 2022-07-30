/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

/************************************************
 * The following file was copied from: https://github.com/Shockah/Stardew-Valley-Mods/blob/master/FlexibleSprinklers/IFlexibleSprinklersApi.cs
 *
 * The original license is as follows:
 *
 *                                  Apache License
                           Version 2.0, January 2004
                        http://www.apache.org/licenses/

   TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION

   1. Definitions.

      "License" shall mean the terms and conditions for use, reproduction,
      and distribution as defined by Sections 1 through 9 of this document.

      "Licensor" shall mean the copyright owner or entity authorized by
      the copyright owner that is granting the License.

      "Legal Entity" shall mean the union of the acting entity and all
      other entities that control, are controlled by, or are under common
      control with that entity. For the purposes of this definition,
      "control" means (i) the power, direct or indirect, to cause the
      direction or management of such entity, whether by contract or
      otherwise, or (ii) ownership of fifty percent (50%) or more of the
      outstanding shares, or (iii) beneficial ownership of such entity.

      "You" (or "Your") shall mean an individual or Legal Entity
      exercising permissions granted by this License.

      "Source" form shall mean the preferred form for making modifications,
      including but not limited to software source code, documentation
      source, and configuration files.

      "Object" form shall mean any form resulting from mechanical
      transformation or translation of a Source form, including but
      not limited to compiled object code, generated documentation,
      and conversions to other media types.

      "Work" shall mean the work of authorship, whether in Source or
      Object form, made available under the License, as indicated by a
      copyright notice that is included in or attached to the work
      (an example is provided in the Appendix below).

      "Derivative Works" shall mean any work, whether in Source or Object
      form, that is based on (or derived from) the Work and for which the
      editorial revisions, annotations, elaborations, or other modifications
      represent, as a whole, an original work of authorship. For the purposes
      of this License, Derivative Works shall not include works that remain
      separable from, or merely link (or bind by name) to the interfaces of,
      the Work and Derivative Works thereof.

      "Contribution" shall mean any work of authorship, including
      the original version of the Work and any modifications or additions
      to that Work or Derivative Works thereof, that is intentionally
      submitted to Licensor for inclusion in the Work by the copyright owner
      or by an individual or Legal Entity authorized to submit on behalf of
      the copyright owner. For the purposes of this definition, "submitted"
      means any form of electronic, verbal, or written communication sent
      to the Licensor or its representatives, including but not limited to
      communication on electronic mailing lists, source code control systems,
      and issue tracking systems that are managed by, or on behalf of, the
      Licensor for the purpose of discussing and improving the Work, but
      excluding communication that is conspicuously marked or otherwise
      designated in writing by the copyright owner as "Not a Contribution."

      "Contributor" shall mean Licensor and any individual or Legal Entity
      on behalf of whom a Contribution has been received by Licensor and
      subsequently incorporated within the Work.

   2. Grant of Copyright License. Subject to the terms and conditions of
      this License, each Contributor hereby grants to You a perpetual,
      worldwide, non-exclusive, no-charge, royalty-free, irrevocable
      copyright license to reproduce, prepare Derivative Works of,
      publicly display, publicly perform, sublicense, and distribute the
      Work and such Derivative Works in Source or Object form.

   3. Grant of Patent License. Subject to the terms and conditions of
      this License, each Contributor hereby grants to You a perpetual,
      worldwide, non-exclusive, no-charge, royalty-free, irrevocable
      (except as stated in this section) patent license to make, have made,
      use, offer to sell, sell, import, and otherwise transfer the Work,
      where such license applies only to those patent claims licensable
      by such Contributor that are necessarily infringed by their
      Contribution(s) alone or by combination of their Contribution(s)
      with the Work to which such Contribution(s) was submitted. If You
      institute patent litigation against any entity (including a
      cross-claim or counterclaim in a lawsuit) alleging that the Work
      or a Contribution incorporated within the Work constitutes direct
      or contributory patent infringement, then any patent licenses
      granted to You under this License for that Work shall terminate
      as of the date such litigation is filed.

   4. Redistribution. You may reproduce and distribute copies of the
      Work or Derivative Works thereof in any medium, with or without
      modifications, and in Source or Object form, provided that You
      meet the following conditions:

      (a) You must give any other recipients of the Work or
          Derivative Works a copy of this License; and

      (b) You must cause any modified files to carry prominent notices
          stating that You changed the files; and

      (c) You must retain, in the Source form of any Derivative Works
          that You distribute, all copyright, patent, trademark, and
          attribution notices from the Source form of the Work,
          excluding those notices that do not pertain to any part of
          the Derivative Works; and

      (d) If the Work includes a "NOTICE" text file as part of its
          distribution, then any Derivative Works that You distribute must
          include a readable copy of the attribution notices contained
          within such NOTICE file, excluding those notices that do not
          pertain to any part of the Derivative Works, in at least one
          of the following places: within a NOTICE text file distributed
          as part of the Derivative Works; within the Source form or
          documentation, if provided along with the Derivative Works; or,
          within a display generated by the Derivative Works, if and
          wherever such third-party notices normally appear. The contents
          of the NOTICE file are for informational purposes only and
          do not modify the License. You may add Your own attribution
          notices within Derivative Works that You distribute, alongside
          or as an addendum to the NOTICE text from the Work, provided
          that such additional attribution notices cannot be construed
          as modifying the License.

      You may add Your own copyright statement to Your modifications and
      may provide additional or different license terms and conditions
      for use, reproduction, or distribution of Your modifications, or
      for any such Derivative Works as a whole, provided Your use,
      reproduction, and distribution of the Work otherwise complies with
      the conditions stated in this License.

   5. Submission of Contributions. Unless You explicitly state otherwise,
      any Contribution intentionally submitted for inclusion in the Work
      by You to the Licensor shall be under the terms and conditions of
      this License, without any additional terms or conditions.
      Notwithstanding the above, nothing herein shall supersede or modify
      the terms of any separate license agreement you may have executed
      with Licensor regarding such Contributions.

   6. Trademarks. This License does not grant permission to use the trade
      names, trademarks, service marks, or product names of the Licensor,
      except as required for reasonable and customary use in describing the
      origin of the Work and reproducing the content of the NOTICE file.

   7. Disclaimer of Warranty. Unless required by applicable law or
      agreed to in writing, Licensor provides the Work (and each
      Contributor provides its Contributions) on an "AS IS" BASIS,
      WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or
      implied, including, without limitation, any warranties or conditions
      of TITLE, NON-INFRINGEMENT, MERCHANTABILITY, or FITNESS FOR A
      PARTICULAR PURPOSE. You are solely responsible for determining the
      appropriateness of using or redistributing the Work and assume any
      risks associated with Your exercise of permissions under this License.

   8. Limitation of Liability. In no event and under no legal theory,
      whether in tort (including negligence), contract, or otherwise,
      unless required by applicable law (such as deliberate and grossly
      negligent acts) or agreed to in writing, shall any Contributor be
      liable to You for damages, including any direct, indirect, special,
      incidental, or consequential damages of any character arising as a
      result of this License or out of the use or inability to use the
      Work (including but not limited to damages for loss of goodwill,
      work stoppage, computer failure or malfunction, or any and all
      other commercial damages or losses), even if such Contributor
      has been advised of the possibility of such damages.

   9. Accepting Warranty or Additional Liability. While redistributing
      the Work or Derivative Works thereof, You may choose to offer,
      and charge a fee for, acceptance of support, warranty, indemnity,
      or other liability obligations and/or rights consistent with this
      License. However, in accepting such obligations, You may act only
      on Your own behalf and on Your sole responsibility, not on behalf
      of any other Contributor, and only if You agree to indemnify,
      defend, and hold each Contributor harmless for any liability
      incurred by, or claims asserted against, such Contributor by reason
      of your accepting any such warranty or additional liability.
 * **********************************************/

using Microsoft.Xna.Framework;

namespace AtraShared.Integrations.Interfaces;

#pragma warning disable SA1623 // Property summary documentation should match accessors - preserving original comments.
#pragma warning disable SA1611 // Element parameters should be documented
#pragma warning disable SA1615 // Element return value should be documented

/// <summary>The API which provides access to Flexible Sprinklers for other mods.</summary>
/// <remarks>Copied from: https://github.com/Shockah/Stardew-Valley-Mods/blob/master/FlexibleSprinklers/IFlexibleSprinklersApi.cs. </remarks>
public interface IFlexibleSprinklersApi
{
    /// <summary>Returns whether the current configuration allows independent sprinkler activation.</summary>
    bool IsSprinklerBehaviorIndependent { get; }

    /// <summary>
    /// Register a new sprinkler tier provider, to add support for Flexible Sprinklers for your custom tiered sprinklers in your mod or override existing ones.<br/>
    /// This is only used for tiered sprinkler power config overrides (how many tiles they water).<br/>
    /// Return `null` if you don't want to modify this specific tier.
    /// </summary>
    void RegisterSprinklerTierProvider(Func<SObject, int?> provider);

    /// <summary>
    /// Register a new sprinkler coverage provider, to add support for Flexible Sprinklers for your custom sprinklers in your mod or override existing ones.<br/>
    /// Returned tile coverage should be relative.<br />
    /// Return `null` if you don't want to modify this specific coverage.
    /// </summary>
    void RegisterSprinklerCoverageProvider(Func<SObject, Vector2[]> provider);

    /// <summary>
    /// Registers a new custom waterable tile provider, to make some tiles count as waterable or not.<br/>
    /// Return `true` if the tile should be waterable no matter what; return `false` if the tile should not be waterable no matter what; return `null` if you don't want to modify this specific tile.
    /// </summary>
    void RegisterCustomWaterableTileProvider(Func<GameLocation, Vector2, bool?> provider);

    /// <summary>Activates all sprinklers in a collective way, taking into account the Flexible Sprinklers mod behavior.</summary>
    void ActivateAllSprinklers();

    /// <summary>Activates sprinklers in specified location in a collective way, taking into account the Flexible Sprinklers mod behavior.</summary>
    void ActivateSprinklersInLocation(GameLocation location);

    /// <summary>Activates a sprinkler, taking into account the Flexible Sprinklers mod behavior.</summary>
    /// <exception cref="InvalidOperationException">Thrown when the current sprinkler behavior does not allow independent sprinkler activation.</exception>
    void ActivateSprinkler(SObject sprinkler, GameLocation location);

    /// <summary>Returns the sprinkler's power after config modifications (that is, the number of tiles it will water).</summary>
    int GetSprinklerPower(SObject sprinkler);

    /// <summary>Returns a sprinkler's flood fill range (that is, how many tiles away will it look for tiles to water) for a given sprinkler power.</summary>
    // [Obsolete("Sprinkler range now also depends on its unmodified coverage shape. Use `GetSprinklerSpreadRange` instead to achieve the same result as before. This method will be removed in a future update.")]
    // int GetFloodFillSprinklerRange(int power);

    /// <summary>Returns a sprinkler's spread range (that is, how many tiles away will it look for tiles to water) for a given sprinkler power (if evenly spread around it).</summary>
    int GetSprinklerSpreadRange(int power);

    /// <summary>Returns a sprinkler's focused range (that is, how many tiles away will it look for tiles to water) for a given unmodified coverage (if focused in one direction).</summary>
    int GetSprinklerFocusedRange(IReadOnlyCollection<Vector2> coverage);

    /// <summary>Returns a sprinkler's max range (that is, how many tiles away will it look for tiles to water) for a given sprinkler (the highest of the spread and focused ranges).</summary>
    int GetSprinklerMaxRange(SObject sprinkler);

    /// <summary>Get the relative tile coverage by supported sprinkler ID. This API is location/position-agnostic. Note that sprinkler IDs may change after a save is loaded due to Json Assets reallocating IDs.</summary>
    Vector2[] GetUnmodifiedSprinklerCoverage(SObject sprinkler);

    /// <summary>Get the relative tile coverage by supported sprinkler ID, modified by the Flexible Sprinklers mod. This API takes into consideration the location and position. Note that sprinkler IDs may change after a save is loaded due to Json Assets reallocating IDs.</summary>
    /// <exception cref="InvalidOperationException">Thrown when the current sprinkler behavior does not allow independent sprinkler activation.</exception>
    Vector2[] GetModifiedSprinklerCoverage(SObject sprinkler, GameLocation location);

    /// <summary>Returns whether a given tile is in range of any sprinkler in the location.</summary>
    bool IsTileInRangeOfAnySprinkler(GameLocation location, Vector2 tileLocation);

    /// <summary>Returns whether a given tile is in range of the specified sprinkler.</summary>
    /// <exception cref="InvalidOperationException">Thrown when the current sprinkler behavior does not allow independent sprinkler activation.</exception>
    bool IsTileInRangeOfSprinkler(SObject sprinkler, GameLocation location, Vector2 tileLocation);

    /// <summary>Returns whether a given tile is in range of specified sprinklers.</summary>
    /// <exception cref="InvalidOperationException">Thrown when the current sprinkler behavior does not allow independent sprinkler activation.</exception>
    bool IsTileInRangeOfSprinklers(IEnumerable<SObject> sprinklers, GameLocation location, Vector2 tileLocation);

    /// <summary>Returns all tiles that are currently in range of any sprinkler in the location.</summary>
    IReadOnlySet<Vector2> GetAllTilesInRangeOfSprinklers(GameLocation location);

    /// <summary>Displays the sprinkler coverage for the specified time.</summary>
    /// <param name="seconds">The amount of seconds to display the coverage for. Pass `null` to use the value configured by the user.</param>
    void DisplaySprinklerCoverage(float? seconds = null);

#pragma warning restore SA1615 // Element return value should be documented
#pragma warning restore SA1611 // Element parameters should be documented
#pragma warning restore SA1623 // Property summary documentation should match accessors
}