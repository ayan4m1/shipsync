﻿using System.IO;

namespace ShipSync.Container.Entity
{
    public class Ship
    {
        // "friendly" name extracted from .craft
        public string Name;

        // actual filename
        public string FileName;

        // related save file
        public GameSave Save;
    }
}