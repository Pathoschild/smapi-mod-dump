namespace CustomNPCFramework.Framework.Enums
{
    /// <summary>An enum used to signify the different asset types that can be used for NPCs.</summary>
    public enum PartType
    {
        /// <summary>Used to signify that the asset is of the body part category. Without this the npc is basically a ghost.</summary>
        body,

        /// <summary>Used to signify that the asset is of the eyes part category. The window to the soul.</summary>
        eyes,

        /// <summary>Used to signify that the asset is of the hair part category. Volume looks good in 2D.</summary>
        hair,

        /// <summary>Used to signify that the asset is of the shirt part category.No shirt = no service.</summary>
        shirt,

        /// <summary>Used to signify that the asset is of the pants/bottoms part category. Also known as bottoms, skirts, shorts, etc.</summary>
        pants,

        /// <summary>Used to signify that the asset is of the shoes part category. Lace up those kicks.</summary>
        shoes,

        /// <summary>Used to signify that the asset is of the accessort part category. Got to wear that bling.</summary>
        accessory,

        /// <summary>Used to signify that the asset is of the other part category. Who knows what this really is...</summary>
        other,

        /// <summary>Used to signify that the asset is of the swimsuit part category. Got to be decent when taking a dip.</summary>
        swimsuit,

        /// <summary>Used to signify that the asset is of the amrs part category. Arms need to be rendered above a shirt on npcs otherwise they get covered.</summary>
        arms
    }
}
