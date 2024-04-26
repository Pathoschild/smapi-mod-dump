/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/scayze/multiprairie
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerPrairieKing
{
	public enum GameKeys
	{
		MoveLeft,
		MoveRight,
		MoveUp,
		MoveDown,
		ShootLeft,
		ShootRight,
		ShootUp,
		ShootDown,
		UsePowerup,
		SelectOption,
		Exit,
		MAX
	}

	public enum MAP_TILE
	{
		BARRIER1 = 0,
		BARRIER2 = 1,
		GRAVEL = 2,
		SAND = 3,
		GRASS = 4,
		CACTUS = 5,
		FENCE = 7,
		TRENCH1 = 8,
		TRENCH2 = 9,
		BRIDGE = 10
	}

	public enum MONSTER_TYPE
	{
		orc = 0,
		ghost = 1,
		ogre = 2,
		mummy = 3,
		devil = 4,
		mushroom = 5,
		spikey = 6,
		dracula = -2,
		outlaw = -1
	}

	public enum DIFFICULTY
	{
		EASY = 0,
		NORMAL = 1,
		HARD = 2
	}

	public enum MAP_TYPE
	{
		desert = 0,
		woods = 2,
		graveyard = 1
	}

	public enum POWERUP_TYPE
	{
		LOG = -1,
		SKULL = -2,
		COIN = 0,
		NICKEL = 1,
		SPREAD = 2,
		RAPIDFIRE = 3,
		NUKE = 4,
		ZOMBIE = 5,
		SPEED = 6,
		SHOTGUN = 7,
		LIFE = 8,
		TELEPORT = 9,
		SHERRIFF = 10,
		HEART = -3
	}

	public enum ITEM_TYPE
	{
		NONE = -1,
		FIRESPEED1 = 0,
		FIRESPEED2 = 1,
		FIRESPEED3 = 2,
		RUNSPEED1 = 3,
		RUNSPEED2 = 4,
		LIFE = 5,
		AMMO1 = 6,
		AMMO2 = 7,
		AMMO3 = 8,
		SPREADPISTOL = 9,
		STAR = 10,
		SKULL = 11,
		LOG = 12,
		FINISHED_GAME = 13
	}

	public enum OPTION_TYPE
	{
		RETRY = 0,
		QUIT = 1
	}


	public enum SYNC_SCOPE
	{
		SINGLE = 0,
		PLAYERS = 1,
		GLOBAL = 2
	}

	public enum ERROR
	{
		UNDEFINED = -1,
		MATCH_FULL = 0,
		MATCH_STARTED = 1,
		NOT_IN_LIST = 2
	}
}
