from sdv_types import Point, PlayerStatus
import constants
import sdv_types
import server

async def get_debris() -> list[sdv_types.Debris]:
    return await server.request("GET_DEBRIS")

async def get_resource_clumps() -> list[sdv_types.ResourceClump]:
    return await server.request("GET_RESOURCE_CLUMPS")


async def get_trees():
    terrain_features: list[sdv_types.TerrainFeature] = await server.request("GET_TERRAIN_FEATURES")
    return [tf for tf in terrain_features if tf["type"] == "tree"]


async def get_fully_grown_trees_and_stumps():
    trees = await get_trees()
    return [t for t in trees if t["stump"] or (t["growthStage"] >= 5 and not t["tapped"])]

async def get_grass():
    terrain_features: list[sdv_types.TerrainFeature] = await server.request("GET_TERRAIN_FEATURES")
    return [tf for tf in terrain_features if tf["type"] == "grass"]

async def get_location_objects() -> list[sdv_types.LocationObject]:
    objects = await server.request(constants.GET_LOCATION_OBJECTS)
    return objects

async def get_player_status() -> sdv_types.PlayerStatus:
    req_builder = server.RequestBuilder("PLAYER_STATUS")
    status = await req_builder.request()
    return status