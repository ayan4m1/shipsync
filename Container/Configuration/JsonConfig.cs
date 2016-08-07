using System.Collections.Generic;
using Newtonsoft.Json;

namespace ShipSync.Container.Configuration
{
    public class JsonConfig
    {
        public int Version;
        public Dictionary<string, string> Dropbox;
        public List<Installation> Installations;
    }
}