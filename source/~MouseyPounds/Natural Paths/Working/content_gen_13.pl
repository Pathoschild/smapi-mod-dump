#!/usr/bin/perl
#
# content.json entry generator for Natural Paths mod

use strict;

my $include_floors = 1;
my $include_ck = 1;
if (scalar @ARGV) {
	# An argument of `ck` adds special keys for clefairykid, who is helping with testing
	# Any other argument sets $include_floors, used for the "alternate" version.
	if (lc($ARGV[0]) eq 'ck') {
		$include_ck = 1;
	} else {
		$include_floors = 1;
	}
}

# ConfigSchema texture choices and mapping to in-game name
my %texture = (
	'None' => '',
	'LightGrass' => 'Light Grass',
	'DarkGrass' => 'Dark Grass',
	'LightDirt' => 'Light Dirt',
	'DarkDirt' => 'Dark Dirt',
	'EemieLightGrass' => 'Light Grass',
	'EemieDarkGrass' => 'Dark Grass',
	'EemieLightDirt' => 'Light Dirt',
	'EemieDarkDirt' => 'Dark Dirt',
	'Sand' => 'Sand',
	'Straw' => 'Straw',
	'Shadow' => 'Shadow',
	'Transparent' => 'Transparent',
	);
if ($include_ck) {
	$texture{'LightSoil'} = 'Light Soil';
	$texture{'DarkSoil'} = 'Dark Soil';
	$texture{'EemieLightSoil'} = 'Light Soil';
	$texture{'EemieDarkSoil'} = 'Dark Soil';
}
# ConfigSchema options for which texture to replace.
# These need to be mapped to in-game recipe name, object ID, and sprite coordinates
my %token = (
	'CobblestonePath_Replacement' => {
		'name' => 'Cobblestone Path',
		'obj_id' => 411,
		'floor_sprite' => '"X": 0, "Y": 128',
		'obj_sprite' => '"X": 48, "Y": 272',
		},
	'CrystalPath_Replacement' => {
		'name' => 'Crystal Path',
		'obj_id' => 409,
		'floor_sprite' => '"X": 192, "Y": 64',
		'obj_sprite' => '"X": 16, "Y": 272',
		},
	'GravelPath_Replacement' => {
		'name' => 'Gravel Path',
		'obj_id' => 407,
		'floor_sprite' => '"X": 64, "Y": 64',
		'obj_sprite' => '"X": 368, "Y": 256',
		},
	'WoodPath_Replacement' => {
		'name' => 'Wood Path',
		'obj_id' => 405,
		'floor_sprite' => '"X": 128, "Y": 64',
		'obj_sprite' => '"X": 336, "Y": 256',
		},
	);
if ($include_floors) {
	$token{'CrystalFloor_Replacement'} = {
		'name' => 'Crystal Floor',
		'obj_id' => 333,
		'floor_sprite' => '"X": 192, "Y": 0',
		'obj_sprite' => '"X": 336, "Y": 208',
	};
	$token{'StoneFloor_Replacement'} = {
		'name' => 'Stone Floor',
		'obj_id' => 329,
		'floor_sprite' => '"X": 64, "Y": 0',
		'obj_sprite' => '"X": 272, "Y": 208',
	};
	$token{'StrawFloor_Replacement'} = {
		'name' => 'Straw Floor',
		'obj_id' => 401,
		'floor_sprite' => '"X": 0, "Y": 64',
		'obj_sprite' => '"X": 272, "Y": 256',
	};
	$token{'WeatheredFloor_Replacement'} = {
		'name' => 'Weathered Floor',
		'obj_id' => 331,
		'floor_sprite' => '"X": 128, "Y": 0',
		'obj_sprite' => '"X": 304, "Y": 208',
	};
	$token{'WoodFloor_Replacement'} = {
		'name' => 'Wood Floor',
		'obj_id' => 328,
		'floor_sprite' => '"X": 0, "Y": 0',
		'obj_sprite' => '"X": 256, "Y": 208',
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
print qq({\n\t"Format": "1.3",\n\t"ConfigSchema": {\n);
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
			"Default": "false"
		},
	},
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
			"Action": "EditImage",
			"Target": "TerrainFeatures/Flooring",
			"FromFile": "assets/{{$t}}_{{Season}}.png",
			"ToArea": { $token{$t}{'floor_sprite'}, "Width": 64, "Height": 64},
			"FromArea": { "X": 0, "Y": 0, "Width": 64, "Height": 64},
			"When": { "$t": "$tex_condition" }
		},
		{
			"Action": "EditImage",
			"Target": "Maps/springobjects",
			"FromFile": "assets/{{$t}}_{{Season}}.png",
			"ToArea": { $token{$t}{'obj_sprite'}, "Width": 16, "Height": 16},
			"FromArea": { "X": 0, "Y": 0, "Width": 16, "Height": 16},
			"When": { "$t": "$tex_condition" }
		},
END_PRINT
	# Crafting recipe still triggers on any non-'None' texture condition but needs to account for all possible material/amount combos
	foreach my $m (sort keys %craft_mat) {
		next if ($m eq 'NoChange');
		foreach my $a (@craft_amt) {
			# Vanilla files only list a crafting amt on the product if it's more than 1 so we do the same.
			my $suffix = '';
			$suffix = " $a" if ($a > 1);
			print <<"END_PRINT";
		{
			"Action": "EditData",
			"Target": "Data/CraftingRecipes",
			"Fields": { "$token{$t}{'name'}": { 0: "$craft_mat{$m} 1", 2: "$token{$t}{'obj_id'}$suffix" } },
			"When": { 
				"$t": "$tex_condition",
				"Crafting_Material": "$m",
				"Crafting_Amount": $a
			}
		},
END_PRINT
		}
	}
	# Now we change the in-game name of any altered texture. These are texture-specific
	$token{$t}{'name'} =~ / (\w+)$/;
	my $type = $1;
	foreach my $x (sort keys %texture) {
		next if ($x eq 'None');
		print <<"END_PRINT";
		{
			"Action": "EditData",
			"Target": "Data/ObjectInformation",
			"Fields": {	"$token{$t}{'obj_id'}": { 0: "$texture{$x} $type" } },
			"When": { "$t": "$x" }
		},
END_PRINT
	}
}

# The Grass Overrides are processed here in a new loop to make sure they happen after the regular changes
foreach my $t (sort keys %token) {
	print <<"END_PRINT";
		{
			"Action": "EditImage",
			"Target": "TerrainFeatures/Flooring",
			"FromFile": "assets/Snow_Override.png",
			"ToArea": { $token{$t}{'floor_sprite'}, "Width": 64, "Height": 64},
			"FromArea": { "X": 0, "Y": 0, "Width": 64, "Height": 64},
			"Enabled": "{{Snow_Overrides_LightGrass}}",
			"When": { "$t": "LightGrass, EemieLightGrass", "Season": "Winter" }
		},
		{
			"Action": "EditImage",
			"Target": "Maps/springobjects",
			"FromFile": "assets/Snow_Override.png",
			"ToArea": { $token{$t}{'obj_sprite'}, "Width": 16, "Height": 16},
			"FromArea": { "X": 0, "Y": 0, "Width": 16, "Height": 16},
			"Enabled": "{{Snow_Overrides_LightGrass}}",
			"When": { "$t": "LightGrass, EemieLightGrass", "Season": "Winter" }
		},
		{
			"Action": "EditImage",
			"Target": "TerrainFeatures/Flooring",
			"FromFile": "assets/Ice_Override.png",
			"ToArea": { $token{$t}{'floor_sprite'}, "Width": 64, "Height": 64},
			"FromArea": { "X": 0, "Y": 0, "Width": 64, "Height": 64},
			"Enabled": "{{Ice_Overrides_DarkGrass}}",
			"When": { "$t": "DarkGrass, EemieDarkGrass", "Season": "Winter" }
		},
		{
			"Action": "EditImage",
			"Target": "Maps/springobjects",
			"FromFile": "assets/Ice_Override.png",
			"ToArea": { $token{$t}{'obj_sprite'}, "Width": 16, "Height": 16},
			"FromArea": { "X": 0, "Y": 0, "Width": 16, "Height": 16},
			"Enabled": "{{Ice_Overrides_DarkGrass}}",
			"When": { "$t": "DarkGrass, EemieDarkGrass", "Season": "Winter" }
		},
END_PRINT
}
print qq(\t]\n}\n);

__END__
