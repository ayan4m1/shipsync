using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using log4net;
using log4net.Repository.Hierarchy;
using ShipSync.Container.Entity;

namespace ShipSync.Container.Service
{
    public sealed class ShipService : IShipService
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ShipService));

        public List<GameSave> FindSavesInSource(SaveSource source)
        {
            return Directory.GetDirectories(source.InstallPath)
                .SelectMany(dir =>
                {
                    if (dir.EndsWith("scenarios") || dir.EndsWith("training"))
                    {
                        return new string[] {};
                    }

                    var baseDir = Path.Combine(dir, "Ships");
                    return new[]
                    {
                        Path.Combine(baseDir, "VAB"),
                        Path.Combine(baseDir, "SPH")
                    };
                })
                .Where(dir => !string.IsNullOrWhiteSpace(dir))
                .Select(dir => new GameSave()
                {
                    Name = new DirectoryInfo(dir).Name,
                    SaveSource = source
                }).Distinct().ToList();
        }

        public IEnumerable<Ship> FindShipsInSave(GameSave save)
        {
            var dir = save.SaveSource.InstallPath;
            try
            {
                return Directory.GetFiles(dir, "*.craft").Select(file => new Ship()
                {
                    FileName = file,
                    // todo: parse name from .craft file
                    Save = save
                });
            }
            catch (Exception ex)
            {
                Log.Warn("Failed to enumerate craft in " + dir, ex);
            }

            return new List<Ship>();
        }
    }
}