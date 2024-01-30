from sdv_types import Point, PlayerStatus
import constants
import sdv_types
import server

async def get_debris() -> list[sdv_types.Debris]:
    return await server.request("GET_DEBRIS")

async def get_resource_clumps() -> list[sdv_types.ResourceClump]:
    return await server.request("GET_RESOURCE_CLUMPS")