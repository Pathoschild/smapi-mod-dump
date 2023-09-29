import clr
clr.AddReference("StardewModdingAPI")
from StardewModdingAPI import Mod, LogLevel


class ModEntry(Mod):
    def Entry(self, helper):
        self.Monitor.Log("Hello World from Python", LogLevel.Alert)

        helper.Events.GameLoop.GameLaunched += self.gameLaunchedHandler
    
    def gameLaunchedHandler(self, *args):
        self.Monitor.Log("Game Launched detected in python!", LogLevel.Alert)