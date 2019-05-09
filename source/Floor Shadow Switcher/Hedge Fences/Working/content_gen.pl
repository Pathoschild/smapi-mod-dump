#!/usr/bin/perl
#
# content.json entry generator for Hedge Fences mod

use strict;

# ConfigSchema texture choices and mapping to in-game name
my @flowers = qw(mixed pink blue yellow);
my @hedges = qw(dark light);
my @snow = qw(all half none);

# ConfigSchema options for which texture to replace.
# These need to be mapped to in-game recipe name, filename. object ID, and sprite coordinates
# Most of this is now superceded by Dynamic Tokens
my %replace = (
	'Wood' => {
		'name' => 'Wood Fence',
		'obj_id' => 322,
		'obj_sprite' => '"X": 160, "Y": 208',
		},
	'Stone' => {
		'name' => 'Stone Fence',
		'obj_id' => 323,
		'obj_sprite' => '"X": 176, "Y": 208',
		},
	'Iron' => {
		'name' => 'Iron Fence',
		'obj_id' => 324,
		'obj_sprite' => '"X": 192, "Y": 208',
		},
	'Hardwood' => {
		'name' => 'Hardwood Fence',
		'obj_id' => 298,
		'obj_sprite' => '"X": 160, "Y": 192',
		},
	);
# ConfigSchema options for CraftingMaterial and associated object ID.
my %craft_mat = (
	'Fiber' => 771,
	'Wood' => 388,
	'Hardwood' => 709,
	);

# No output file, everything just prints to stdout and needs redirection because !lazy
select STDOUT;
print qq({\n\t"Format": "1.5",\n\t"ConfigSchema": {\n);
print qq(\t\t"ReplaceFence": {\n\t\t\t"AllowValues": ") . join(', ', (sort keys %replace)) . qq(",\n\t\t\t"Default": "Hardwood"\n\t\t},\n);
print qq(\t\t"CraftingMaterial": {\n\t\t\t"AllowValues":  ") . join(', ', (sort keys %craft_mat)) . qq(",\n\t\t\t"Default": "Hardwood"\n\t\t},\n);
print qq(\t\t"AddFlowers": {\n\t\t\t"AllowValues": "true, false",\n\t\t\t"Default": "true"\n\t\t},\n);
print qq(\t\t"FlowerType": {\n\t\t\t"AllowValues": ") . join(', ', (@flowers)) . qq(",\n\t\t\t"Default": "mixed"\n\t\t},\n);
print qq(\t\t"HedgeShade": {\n\t\t\t"AllowValues": ") . join(', ', (@hedges)) . qq(",\n\t\t\t"Default": "dark"\n\t\t},\n);
print qq(\t\t"SnowInWinter": {\n\t\t\t"AllowValues": ") . join(', ', (@snow)) . qq(",\n\t\t\t"Default": "all"\n\t\t},\n);
print qq(\t\t"FlowersInWinter": {\n\t\t\t"AllowValues": "true, false",\n\t\t\t"Default": "false"\n\t\t},\n);

print <<"END_PRINT";
	},
    "DynamicTokens": [
        {
            "Name": "FenceTarget",
            "Value": "LooseSprites/Fence1",
            "When": { "ReplaceFence": "Wood" }
        },
        {
            "Name": "FenceTarget",
            "Value": "LooseSprites/Fence2",
            "When": { "ReplaceFence": "Stone" }
        },
        {
            "Name": "FenceTarget",
            "Value": "LooseSprites/Fence3",
            "When": { "ReplaceFence": "Iron" }
        },
        {
            "Name": "FenceTarget",
            "Value": "LooseSprites/Fence5",
            "When": { "ReplaceFence": "Hardwood" }
        },
        {
            "Name": "FenceObject",
            "Value": "322",
            "When": { "ReplaceFence": "Wood" }
        },
        {
            "Name": "FenceObject",
            "Value": "323",
            "When": { "ReplaceFence": "Stone" }
        },
        {
            "Name": "FenceObject",
            "Value": "324",
            "When": { "ReplaceFence": "Iron" }
        },
        {
            "Name": "FenceObject",
            "Value": "298",
            "When": { "ReplaceFence": "Hardwood" }
        },
        {
            "Name": "CraftObject",
            "Value": "771",
            "When": { "CraftingMaterial": "Fiber" }
        },
        {
            "Name": "CraftObject",
            "Value": "388",
            "When": { "CraftingMaterial": "Wood" }
        },
        {
            "Name": "CraftObject",
            "Value": "709",
            "When": { "CraftingMaterial": "Hardwood" }
        },
    ],
	"Changes": [
		{
			"LogName": "Main Fence Replace",
			"Action": "EditImage",
			"Target": "{{FenceTarget}}",
			"FromFile": "assets/hedge_{{HedgeShade}}.png",
			"ToArea": { "X": 0, "Y": 0, "Width": 48, "Height": 128},
			"FromArea": { "X": 0, "Y": 0, "Width": 48, "Height": 128},
		},
		{
			"LogName": "Lower Right Fence Replace",
			"Action": "EditImage",
			"Target": "{{FenceTarget}}",
			"FromFile": "assets/hedge_{{HedgeShade}}.png",
			"PatchMode": "Overlay",
			"ToArea": { "X": 32, "Y": 160, "Width": 16, "Height": 32 },
			"FromArea": { "X": 32, "Y": 160, "Width": 16, "Height": 32 },
		},
		{
			"LogName": "Flower Overlay SSF",
			"Action": "EditImage",
			"Target": "{{FenceTarget}}",
			"FromFile": "assets/flowers_{{FlowerType}}.png",
			"PatchMode": "Overlay",
			"When": { "Season": "Spring, Summer, Fall", "AddFlowers": "true" }
		},
		{
			"LogName": "Flower Overlay Winter",
			"Action": "EditImage",
			"Target": "{{FenceTarget}}",
			"FromFile": "assets/flowers_{{FlowerType}}.png",
			"PatchMode": "Overlay",
			"When": { "Season": "Winter" , "FlowersInWinter": "true", "AddFlowers": "true" }
		},
		{
			"LogName": "Snow Overlay All",
			"Action": "EditImage",
			"Target": "{{FenceTarget}}",
			"FromFile": "assets/snow.png",
			"PatchMode": "Overlay",
			"When": { "season": "winter", "SnowInWinter": "all" }
		},
		{
			"LogName": "Snow Overlay Half",
			"Action": "EditImage",
			"Target": "{{FenceTarget}}",
			"FromFile": "assets/snow_half.png",
			"PatchMode": "Overlay",
			"When": { "season": "winter", "SnowInWinter": "half" }
		},
		{
			"LogName": "Names in ObjectInformation",
			"Action": "EditData",
			"Target": "Data/ObjectInformation",
			"Fields": {
				"{{FenceObject}}": { "0": "Hedge Fence", "1": "1", "4": "Hedge Fence" }
				},
		},
		{
			"LogName": "CraftingRecipe",
			"Action": "EditData",
			"Target": "Data/CraftingRecipes",
			"Fields": { "{{ReplaceFence}} Fence": { 0: "{{CraftObject}} 1", 2: "{{FenceObject}}" } },
		},
END_PRINT

foreach my $r (sort keys %replace) {
	print <<"END_PRINT";
		{
			"LogName": "Inventory Sprite ($r)",
			"Action": "EditImage",
			"Target": "Maps/springobjects",
			"FromFile": "assets/hedge_{{HedgeShade}}.png",
			"ToArea": { $replace{$r}{'obj_sprite'}, "Width": 16, "Height": 16},
			"FromArea": { "X": 16, "Y": 64, "Width": 16, "Height": 16},
			"When": { "ReplaceFence": "$r" }
		},
		{
			"LogName": "Inventory Sprite ($r) with Flowers SSF",
			"Action": "EditImage",
			"Target": "Maps/springobjects",
			"FromFile": "assets/flowers_{{FlowerType}}.png",
			"PatchMode": "Overlay",
			"ToArea": { $replace{$r}{'obj_sprite'}, "Width": 16, "Height": 16 },
			"FromArea": { "X": 16, "Y": 64, "Width": 16, "Height": 16 },
			"When": { "ReplaceFence": "$r", "Season": "Spring, Summer, Fall", "AddFlowers": "true" }
		},
		{
			"LogName": "Inventory Sprite ($r) with Flowers Winter",
			"Action": "EditImage",
			"Target": "Maps/springobjects",
			"FromFile": "assets/flowers_{{FlowerType}}.png",
			"PatchMode": "Overlay",
			"ToArea": { $replace{$r}{'obj_sprite'}, "Width": 16, "Height": 16 },
			"FromArea": { "X": 16, "Y": 64, "Width": 16, "Height": 16 },
			"When": { "ReplaceFence": "$r", "Season": "Winter" , "FlowersInWinter": "true", "AddFlowers": "true" }
		},
		{
			"LogName": "Inventory Sprite ($r) with Snow All",
			"Action": "EditImage",
			"Target": "Maps/springobjects",
			"FromFile": "assets/snow.png",
			"PatchMode": "Overlay",
			"ToArea": { $replace{$r}{'obj_sprite'}, "Width": 16, "Height": 16 },
			"FromArea": { "X": 16, "Y": 64, "Width": 16, "Height": 16 },
			"When": { "season": "winter", "ReplaceFence": "$r", "SnowInWinter": "all"  }
		},
		{
			"LogName": "Inventory Sprite ($r) with Snow Half",
			"Action": "EditImage",
			"Target": "Maps/springobjects",
			"FromFile": "assets/snow_half.png",
			"PatchMode": "Overlay",
			"ToArea": { $replace{$r}{'obj_sprite'}, "Width": 16, "Height": 16 },
			"FromArea": { "X": 16, "Y": 64, "Width": 16, "Height": 16 },
			"When": { "season": "winter", "ReplaceFence": "$r", "SnowInWinter": "half"  }
		},
END_PRINT
}

print qq(\t]\n}\n);

__END__
THese aren't supported yet:
// Last patch before the loops
		{
			"LogName": "Names in ObjectInformation ($r)",
			"Action": "EditData",
			"Target": "Data/ObjectInformation",
			"Fields": {
				"$replace{$r}{'obj_id'}": { "0": "Hedge Fence", "1": "1", "4": "Hedge Fence" }
				},
			"When": { "ReplaceFence": "$r" }
		},

// Crafting Loop
	foreach my $m (sort keys %craft_mat) {
		# next if ($m eq 'NoChange');
		print <<"END_PRINT";
			{
				"LogName": "CraftingRecipe ($r) with ($m)",
				"Action": "EditData",
				"Target": "Data/CraftingRecipes",
				"Fields": { "$replace{$r}{'name'}": { 0: "$craft_mat{$m} 1", 2: "$replace{$r}{'obj_id'}" } },
				"When": { "CraftingMaterial": "$m", "ReplaceFence": "$r" }
			},
END_PRINT
	}
