using StardewModdingAPI;

namespace TillableGround {
    class ModConfig {
        public bool AllowTillingAnywhere { get; set; } = false;
        public SButton AllowTillingKeybind { get; set; } = SButton.H;
        public SButton PreventTillingKeybind { get; set; } = SButton.U;
    }
}
