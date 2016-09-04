using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ShipSync.Container.Configuration
{
    public class JsonConfig
    {
        public int Version;

        [JsonProperty("dropbox.token")]
        public string DropboxToken;

        public List<Installation> Installations;
    }
}