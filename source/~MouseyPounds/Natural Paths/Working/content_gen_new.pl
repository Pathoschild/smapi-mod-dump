#!/usr/bin/perl
#
# content.json entry generator for Natural Paths mod
#
# Content Patcher 1.11 version (Stardew 1.4)

use strict;

my $include_floors = 1;
my $include_ck = 0;
if (scalar @ARGV) {
	# An argument of `ck` adds special keys for clefairykid, who is helping with testing
	if (lc($ARGV[0]) eq 'ck') {
		$include_ck = 1;
	}}

# ConfigSchema texture choices and mapping to in-game name
my %texture = (
	'None' => '',
	'LightGrass' => 'Light Grass',
	'DarkGrass' => 'Dark Grass',
	'LightDirt' => 'Light Dirt',
	'DarkDirt' => 'Dark Dirt',
	# Removing in favor of a general variable
	#'EemieLightGrass' => 'Light Grass',
	#'EemieDarkGrass' => 'Dark Grass',
	#'EemieLightDirt' => 'Light Dirt',
	#'EemieDarkDirt' => 'Dark Dirt',
	'Pipe' => 'Pipe',
	'Sand' => 'Sand',
	'Straw' => 'Straw',
	'Shadow' => 'Shadow',
	'Transparent' => 'Transparent',
	);
if ($include_ck) {
	$texture{'LightSoil'} = 'Light Soil';
	$texture{'DarkSoil'} = 'Dark Soil';
}
# ConfigSchema options for which texture to replace.
# These need to be mapped to in-game recipe name, object ID, and sprite coordinates
my %token = (
	'CobblestonePath_Replacement' => {
		'name' => 'Cobblestone Path',
		'obj_id' => 411,
		'floor_sprite' => {"X" => 0, "Y" => 128},
		'obj_sprite' => {"X" => 48, "Y" => 272},
		'variant' => 'CobblestonePath_Variant',
	},
	'CrystalPath_Replacement' => {
		'name' => 'Crystal Path',
		'obj_id' => 409,
		'floor_sprite' => {"X" => 192, "Y" => 64},
		'obj_sprite' => {"X" => 16, "Y" => 272},
		'variant' => 'CrystalPath_Variant',
	},
	'GravelPath_Replacement' => {
		'name' => 'Gravel Path',
		'obj_id' => 407,
		'floor_sprite' => {"X" => 64, "Y" => 64},
		'obj_sprite' => {"X" => 368, "Y" => 256},
		'variant' => 'GravelPath_Variant',
	},
	'WoodPath_Replacement' => {
		'name' => 'Wood Path',
		'obj_id' => 405,
		'floor_sprite' => {"X" => 128, "Y" => 64},
		'obj_sprite' => {"X" => 336, "Y" => 256},
		'variant' => 'WoodPath_Variant',
	},
);
if ($include_floors) {
	$token{'BrickFloor_Replacement'} = {
		'name' => 'Brick Floor',
		'obj_id' => 293,
		'floor_sprite' => {"X" => 128, "Y" => 128},
		'obj_sprite' => {"X" => 80, "Y" => 192},
		'variant' => 'BrickFloor_Variant',
	};
	$token{'CrystalFloor_Replacement'} = {
		'name' => 'Crystal Floor',
		'obj_id' => 333,
		'floor_sprite' => {"X" => 192, "Y" => 0},
		'obj_sprite' => {"X" => 336, "Y" => 208},
		'variant' => 'CrystalFloor_Variant',
	};
	$token{'StoneFloor_Replacement'} = {
		'name' => 'Stone Floor',
		'obj_id' => 329,
		'floor_sprite' => {"X" => 64, "Y" => 0},
		'obj_sprite' => {"X" => 272, "Y" => 208},
		'variant' => 'StoneFloor_Variant',
	};
	$token{'StrawFloor_Replacement'} = {
		'name' => 'Straw Floor',
		'obj_id' => 401,
		'floor_sprite' => {"X" => 0, "Y" => 64},
		'obj_sprite' => {"X" => 272, "Y" => 256},
		'variant' => 'StrawFloor_Variant',
	};
	$token{'WeatheredFloor_Replacement'} = {
		'name' => 'Weathered Floor',
		'obj_id' => 331,
		'floor_sprite' => {"X" => 128, "Y" => 0},
		'obj_sprite' => {"X" => 304, "Y" => 208},
		'variant' => 'WeatheredFloor_Variant',
	};
	$token{'WoodFloor_Replacement'} = {
		'name' => 'Wood Floor',
		'obj_id' => 328,
		'floor_sprite' => {"X" => 0, "Y" => 0},
		'obj_sprite' => {"X" => 256, "Y" => 208},
		'variant' => 'WoodFloor_Variant',
	};
}
# ConfigSchema options for Crafting_Amount
my @craft_amt = (1, 5, 10);
# ConfigSchema options for Crafting_Material and associated object ID. NoChange will need special handling.
my %craft_mat = (
	'NoChange' => -1,
	'Fiber' => 771,
	'Wood' => 388,
	'Stone' => 390,
	'Clay' => 330,
	'Hardwood' => 709,
	'Sap' => 92,
	);

# No output file, everything just prints to stdout and needs redirection because !lazy
select STDOUT;
print qq({\n\t"Format": "1.11",\n\t"ConfigSchema": {\n);
foreach my $t (sort keys %token) {
	# default is DarkDirt for GravelPath and LightGrass for WoodPath; None for all others
	my $d = "None";
	if ($t eq 'GravelPath_Replacement') {
		$d = 'DarkDirt';
	} elsif ($t eq 'WoodPath_Replacement') {
		$d = 'LightGrass';
	}
	print qq(\t\t"$t": {\n\t\t\t"AllowValues": ") . join(', ', (sort keys %texture)) . qq(",\n\t\t\t"Default": "$d"\n\t\t},\n);
}
print qq(\t\t"Crafting_Material": {\n\t\t\t"AllowValues":  ") . join(', ', (sort keys %craft_mat)) . qq(",\n\t\t\t"Default": "Fiber"\n\t\t},\n);
print qq(\t\t"Crafting_Amount": {\n\t\t\t"AllowValues":  ") . join(', ', (@craft_amt)) . qq(",\n\t\t\t"Default": "1"\n\t\t},\n);
print <<"END_PRINT";
		"Snow_Overrides_LightGrass": {
			"AllowValues": "true, false",
			"Default": "false"
		},
		"Ice_Overrides_DarkGrass": {
			"AllowValues": "true, false",
			"Default": "true"
		},
		"Recolor_Option": {
			"AllowValues": "auto, Eemie, Starblue",
			"Default": "auto"
		},
		"Eemie_Fall_Variant": {
			"AllowValues": "green, orange",
			"Default": "green"
		},
	},
    "DynamicTokens": [
        {
            "Name": "CraftObject",
            "Value": "771",
            "When": { "Crafting_Material": "Fiber" }
        },
        {
            "Name": "CraftObject",
            "Value": "388",
            "When": { "Crafting_Material": "Wood" }
        },
        {
            "Name": "CraftObject",
            "Value": "709",
            "When": { "Crafting_Material": "Hardwood" }
        },
        {
            "Name": "CraftObject",
            "Value": "390",
            "When": { "Crafting_Material": "Stone" }
        },
        {
            "Name": "CraftObject",
            "Value": "330",
            "When": { "Crafting_Material": "Clay" }
        },
        {
            "Name": "CraftObject",
            "Value": "92",
            "When": { "Crafting_Material": "Sap" }
        },
        {
            "Name": "CraftCount",
            "Value": " {{Crafting_Amount}}",
        },
        {
            "Name": "CraftCount",
            "Value": "",
            "When": { "Crafting_Amount": "1" }
        },
		// Tokens related to recolors
        {
            "Name": "Prefix",
            "Value": "",
        },
        {
            "Name": "Prefix",
            "Value": "Eemie_",
            "When": {
				"Recolor_Option": "auto",
				"HasMod": "minervamaga.CP.eemieMapRecolour",
			}
        },
        {
            "Name": "Prefix",
            "Value": "Starblue_",
            "When": {
				"Recolor_Option": "auto",
				"HasMod": "Lita.StarblueValley",
			}
        },
        {
            "Name": "Prefix",
            "Value": "Eemie_",
            "When": { "Recolor_Option": "Eemie" }
        },
        {
            "Name": "Prefix",
            "Value": "Starblue_",
            "When": { "Recolor_Option": "Starblue" }
        },
        {
            "Name": "Variant",
            "Value": "",
        },
        {
            "Name": "Variant",
            "Value": "_{{Eemie_Fall_Variant}}",
            "When": {
				// To catch both possibilities (explicit setting or auto-detected) we will check the Dynamic Token instead of the config
				"Prefix": "Eemie_",
				"Season": "Fall",
			}
        },
		// Area Tokens for each possible floor
END_PRINT

# THIS won't work. I can't override one token with multiple values...
foreach my $t (sort keys %token) {
	print <<"END_PRINT";
        {
            "Name": "FloorSpriteX",
            "Value": "$token{$t}{'floor_sprite'}{'X'}",
			"When": { "$t:None": "false" },
        },
        {
            "Name": "FloorSpriteY",
            "Value": "$token{$t}{'floor_sprite'}{'Y'}",
			"When": { "$t:None": "false" },
        },
        {
            "Name": "ObjectSpriteX",
            "Value": "$token{$t}{'obj_sprite'}{'X'}",
			"When": { "$t:None": "false" },
        },
        {
            "Name": "ObjectSpriteY",
            "Value": "$token{$t}{'obj_sprite'}{'Y'}",
			"When": { "$t:None": "false" },
        },
END_PRINT

print <<"END_PRINT";
	],
	"Changes": [
END_PRINT
# Some changes use all 'When' choices except 'None'. Hardcoding bit me in the ass.
#my $tex_condition = "LightGrass, DarkGrass, LightDirt, DarkDirt, Sand, Straw, Shadow, Transparent";
my $tex_condition = join(', ', (sort keys %texture));
$tex_condition =~ s/None,? ?//;
$tex_condition =~ s/, $//;

foreach my $t (sort keys %token) {
	print <<"END_PRINT";
		{
			"LogName": "Flooring ($token{$t}{'name'}) -- Default",
			"Action": "EditImage",
			"Target": "TerrainFeatures/Flooring,TerrainFeatures/Flooring_winter",
			"FromFile": "assets/{{$t}}_{{Season}}.png",
			"ToArea": { $token{$t}{'floor_sprite'}, "Width": 64, "Height": 64},
			"FromArea": { "X": 0, "Y": 0, "Width": 64, "Height": 64},
			"When": { "$t:None": "false" }
		},
		{
			"LogName": "Flooring ($token{$t}{'name'}) -- Recolor",
			"Action": "EditImage",
			"Target": "TerrainFeatures/Flooring,TerrainFeatures/Flooring_winter",
			"FromFile": "assets/{{Prefix}}{{$t}}_{{Season}}{{Variant}}.png",
			"ToArea": { $token{$t}{'floor_sprite'}, "Width": 64, "Height": 64},
			"FromArea": { "X": 0, "Y": 0, "Width": 64, "Height": 64},
			"When": { 
				"HasFile:assets/{{Prefix}}{{$t}}_{{Season}}{{Variant}}.png": "true",
				"$t:None": "false",
			}
		},
		{
			"LogName": "Springobject ($token{$t}{'name'}) -- Default",
			"Action": "EditImage",
			"Target": "Maps/springobjects",
			"FromFile": "assets/{{$t}}_{{Season}}.png",
			"ToArea": { $token{$t}{'obj_sprite'}, "Width": 16, "Height": 16},
			"FromArea": { "X": 0, "Y": 0, "Width": 16, "Height": 16},
			"When": { "$t:None": "false" }
		},
		{
			"LogName": "Springobject ($token{$t}{'name'}) -- Recolor",
			"Action": "EditImage",
			"Target": "Maps/springobjects",
			"FromFile": "assets/{{Prefix}}{{$t}}_{{Season}}{{Variant}}.png",
			"ToArea": { $token{$t}{'obj_sprite'}, "Width": 16, "Height": 16},
			"FromArea": { "X": 0, "Y": 0, "Width": 16, "Height": 16},
			"When": { 
				"HasFile:assets/{{Prefix}}{{$t}}_{{Season}}{{Variant}}.png": "true",
				"$t:None": "false",
			}
		},
		{
			"LogName": "CraftingRecipes ($token{$t}{'name'})",
			"Action": "EditData",
			"Target": "Data/CraftingRecipes",
			"Fields": { "$token{$t}{'name'}": { 0: "{{CraftObject}} 1", 2: "$token{$t}{'obj_id'}\{{CraftCount}}" } },
			"When": { 
				"$t:None": "false",
				"Crafting_Material:NoChange": "false",
			}
		},
		// Need to test if Display name has any effect on crafting recipes or not.
		{
			"LogName": "ObjectInformation Display Name Change ($token{$t}{'name'})",
			"Action": "EditData",
			"Target": "Data/ObjectInformation",
			"Fields": {	"$token{$t}{'obj_id'}": { 4: "{{$t}} Path" } },
			"When": { "$t:None": "false" }
		},
END_PRINT
}

# The Grass Overrides are processed here in a new loop to make sure they happen after the regular changes
foreach my $t (sort keys %token) {
	print <<"END_PRINT";
		{
			"LogName": "Flooring Snow Override ($token{$t}{'name'}) -- Default",
			"Action": "EditImage",
			"Target": "TerrainFeatures/Flooring_winter",
			"FromFile": "assets/Snow_Override.png",
			"ToArea": { $token{$t}{'floor_sprite'}, "Width": 64, "Height": 64},
			"FromArea": { "X": 0, "Y": 0, "Width": 64, "Height": 64},
			"When": { "$t": "LightGrass", "Season": "Winter", "Snow_Overrides_LightGrass": "true" }
		},
		{
			"LogName": "Flooring Snow Override ($token{$t}{'name'}) -- Recolor",
			"Action": "EditImage",
			"Target": "TerrainFeatures/Flooring_winter",
			"FromFile": "assets/{{Prefix}}Snow_Override.png",
			"ToArea": { $token{$t}{'floor_sprite'}, "Width": 64, "Height": 64},
			"FromArea": { "X": 0, "Y": 0, "Width": 64, "Height": 64},
			"When": { "$t": "LightGrass", "Season": "Winter", "Snow_Overrides_LightGrass": "true",
				"HasFile:assets/{{Prefix}}Snow_Override.png": "true" }
		},
		{
			"LogName": "SpringObjects Snow Override ($token{$t}{'name'}) -- Default",
			"Action": "EditImage",
			"Target": "Maps/springobjects",
			"FromFile": "assets/Snow_Override.png",
			"ToArea": { $token{$t}{'obj_sprite'}, "Width": 16, "Height": 16},
			"FromArea": { "X": 0, "Y": 0, "Width": 16, "Height": 16},
			"When": { "$t": "LightGrass", "Season": "Winter", "Snow_Overrides_LightGrass": "true" }
		},
		{
			"LogName": "SpringObjects Snow Override ($token{$t}{'name'}) -- Recolor",
			"Action": "EditImage",
			"Target": "Maps/springobjects",
			"FromFile": "assets/{{Prefix}}Snow_Override.png",
			"ToArea": { $token{$t}{'obj_sprite'}, "Width": 16, "Height": 16},
			"FromArea": { "X": 0, "Y": 0, "Width": 16, "Height": 16},
			"When": { "$t": "LightGrass", "Season": "Winter", "Snow_Overrides_LightGrass": "true",
				"HasFile:assets/{{Prefix}}Snow_Override.png": "true" }
		},
		{
			"LogName": "Flooring Ice Override ($token{$t}{'name'}) -- Default",
			"Action": "EditImage",
			"Target": "TerrainFeatures/Flooring_winter",
			"FromFile": "assets/Ice_Override.png",
			"ToArea": { $token{$t}{'floor_sprite'}, "Width": 64, "Height": 64},
			"FromArea": { "X": 0, "Y": 0, "Width": 64, "Height": 64},
			"When": { "$t": "DarkGrass", "Season": "Winter", "Ice_Overrides_DarkGrass": "true" }
		},
		{
			"LogName": "Flooring Ice Override ($token{$t}{'name'}) -- Recolor",
			"Action": "EditImage",
			"Target": "TerrainFeatures/Flooring_winter",
			"FromFile": "assets/{{Prefix}}Ice_Override.png",
			"ToArea": { $token{$t}{'floor_sprite'}, "Width": 64, "Height": 64},
			"FromArea": { "X": 0, "Y": 0, "Width": 64, "Height": 64},
			"When": { "$t": "DarkGrass", "Season": "Winter", "Ice_Overrides_DarkGrass": "true",
				"HasFile:assets/{{Prefix}}Ice_Override.png": "true" }
		},
		{
			"LogName": "SpringObjects Ice Override ($token{$t}{'name'}) -- Default",
			"Action": "EditImage",
			"Target": "Maps/springobjects",
			"FromFile": "assets/Ice_Override.png",
			"ToArea": { $token{$t}{'obj_sprite'}, "Width": 16, "Height": 16},
			"FromArea": { "X": 0, "Y": 0, "Width": 16, "Height": 16},
			"When": { "$t": "DarkGrass", "Season": "Winter", "Ice_Overrides_DarkGrass": "true" }
		},
		{
			"LogName": "SpringObjects Ice Override ($token{$t}{'name'}) -- Recolor",
			"Action": "EditImage",
			"Target": "Maps/springobjects",
			"FromFile": "assets/{{Prefix}}Ice_Override.png",
			"ToArea": { $token{$t}{'obj_sprite'}, "Width": 16, "Height": 16},
			"FromArea": { "X": 0, "Y": 0, "Width": 16, "Height": 16},
			"When": { "$t": "DarkGrass", "Season": "Winter", "Ice_Overrides_DarkGrass": "true",
				"HasFile:assets/{{Prefix}}Ice_Override.png": "true" }
		},
END_PRINT
}
print qq(\t]\n}\n);

__END__
## The old name change logic, removed because too much patch redundancy just to have a space in the name
	#Now we change the in-game name of any altered texture. These are texture-specific
	#Can we simplify this through dynamic tokens?
	$token{$t}{'name'} =~ / (\w+)$/;
	my $type = $1;
	foreach my $x (sort keys %texture) {
		next if ($x eq 'None');
		print <<"END_PRINT";
		{
			"LogName": "ObjectInformation ($token{$t}{'name'}) to ($texture{$x})",
			"Action": "EditData",
			"Target": "Data/ObjectInformation",
			"Fields": {	"$token{$t}{'obj_id'}": { 0: "$texture{$x} $type" } },
			"When": { "$t": "$x" }
		},
END_PRINT
	}
