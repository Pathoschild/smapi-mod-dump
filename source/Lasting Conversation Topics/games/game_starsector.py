# -*- encoding: utf-8 -*-
from pathlib import Path
from typing import List

from PyQt5.QtCore import QDir

import mobase

from ..basic_game import BasicGame, BasicGameSaveGame


class Starsector(BasicGame):
    Name = "Starsector Support Plugin"
    Author = "ddbb07"
    Version = "1.0.1"

    GameName = "Starsector"
    GameShortName = "starsector"
    GameNexusName = "starsector"
    GameBinary = "starsector.exe"
    GameDataPath = "mods"
    GameSavesDirectory = "%GAME_PATH%/saves"

    def listSaves(self, folder: QDir) -> List[mobase.ISaveGame]:
        return [
            BasicGameSaveGame(path)
            for path in Path(folder.absolutePath()).glob("save_*")
        ]
