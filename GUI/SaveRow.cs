using ShipSync.Container.Entity;

namespace ShipSync.GUI
{
    public class SaveRow
    {
        /// <summary>
        /// Whether or not the row is checked for export
        /// </summary>
        public bool Checked { get; set; }

        /// <summary>
        /// Name of the game save
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Reference to the GameSave object
        /// </summary>
        public GameSave Tag { get; set; }
    }
}
