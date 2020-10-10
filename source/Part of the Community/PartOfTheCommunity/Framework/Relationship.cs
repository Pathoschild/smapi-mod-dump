/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/bmarquismarkail/SV_PotC
**
*************************************************/

namespace PartOfTheCommunity.Framework
{
    /// <summary>A relationship type.</summary>
    internal enum Relationship
    {
        /****
        ** Siblings
        ****/
        /// <summary>A brother.</summary>
        Brother,

        /// <summary>A sister.</summary>
        Sister,

        /// <summary>An adoptive brother.</summary>
        HalfBrother,

        /// <summary>An adoptive sister.</summary>
        HalfSister,

        /****
        ** Descendents
        ****/
        /// <summary>A son.</summary>
        Son,

        /// <summary>A daughter.</summary>
        Daughter,

        /// <summary>An adopted son.</summary>
        StepSon,

        /// <summary>A grandson.</summary>
        Grandson,

        /// <summary>A grandson.</summary>
        Granddaughter,

        /// <summary>A great-grandson.</summary>
        GreatGrandson,

        /// <summary>A great-granddaughter.</summary>
        GreatGranddaughter,

        /****
        ** Ancestors
        ****/
        /// <summary>A father.</summary>
        Father,

        /// <summary>A mother.</summary>
        Mother,

        /// <summary>An adoptive father.</summary>
        StepFather,

        /// <summary>An adoptive mother.</summary>
        StepMother,

        /// <summary>A grandfather.</summary>
        Grandfather,

        /// <summary>A grandmother.</summary>
        Grandmother,

        /// <summary>A great-grandfather.</summary>
        GreatGrandfather,

        /// <summary>A great-grandmother.</summary>
        GreatGrandmother,


        /****
        ** Other family
        ****/
        /// <summary>A male spouse.</summary>
        Husband,

        /// <summary>A female spouse.</summary>
        Wife,

        /// <summary>An aunt.</summary>
        Aunt,

        /// <summary>An uncle.</summary>
        Uncle,

        /// <summary>A niece.</summary>
        Niece,

        /// <summary>A nephew.</summary>
        Nephew,

        /// <summary>A godfather.</summary>
        Godfather,

        /// <summary>A goddaughter.</summary>
        Goddaughter,

        /// <summary>A cousin.</summary>
        Cousin,

        /****
        ** Non-family
        ****/
        /// <summary>A non-family friend.</summary>
        Friend,

        /// <summary>An acquaintance separated by the gulf of a past war.</summary>
        WarTorn
    }
}
