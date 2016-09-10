using System.Collections.Generic;
using ShipSync.Container.Entity;

namespace ShipSync.Container.Configuration
{
    public class JsonConfig
    {
        public DropboxConfig Dropbox;
        public List<SaveSource> Sources;
    }
}