from __future__ import annotations
from dataclasses import dataclass
from typing import TypedDict, Any, Literal, NotRequired
from enum import Enum

class PlayerStatus(TypedDict):
    location: str
    position: tuple[float, float]
    facingDirection: int
    isMoving: bool
    tileX: int
    tileY: int
    canMove: bool

type ToolStatus = Tool | None

class BaseGameItem(TypedDict):
    type: Literal[""]
    netName: str
    stack: int

class Tool(BaseGameItem):
    type: Literal["tool"]
    isTool: True
    power: int
    baseName: str
    upgradeLevel: int
    tileX: int
    tileY: int

class MeleeWeapon(BaseGameItem):
    type: Literal["meleeWeapon"]

class Scythe(BaseGameItem):
    type: Literal["scythe"]

type GameItem = Tool | MeleeWeapon | Scythe

class ClickableComponent(TypedDict):
    containsMouse: bool
    visible: bool
    center: Point
    focusTarget: NotRequired[Point]

type Point = tuple[int, int]

class Debris(TypedDict):
    chunkType: int
    debrisType: int
    tileX: int
    tileY: int
    isMoving: bool
    movingTowardsPlayer: bool

class ResourceClump(TypedDict):
    tileX: int
    tileY: int
    height: int 
    width: int
    objectIndex: int
    health: float
    name: str
    type: Literal["resource_clump"]

class Tree(TypedDict):
    type: Literal["tree"]
    treeType: int
    tileX: int 
    tileY: int 
    tapped: bool
    stump: bool
    growthStage: int

class Grass(TypedDict):
    type: Literal["grass"]
    grassType: int
    tileX: int 
    tileY: int 
    numberOfWeeds: int

type TerrainFeature = Tree | Grass

class LocationObject(TypedDict):
    name: str
    tileX: int 
    tileY: int
    type: str
    isForage: bool
    readyForHarvest: bool
    canBeGrabbed: bool
    isOnScreen: bool
    parentSheetIndex: int

@dataclass
class Rectangle:
    left: int
    top: int
    width: int
    height: int

    @property
    def right(self):
        return self.left + self.width
    
    @property
    def bottom(self):
        return self.top + self.height
    
    def contains_point(self, point: Point):
        x, y = point
        return (self.left <= x < self.right) and (self.top <= y < self.bottom)