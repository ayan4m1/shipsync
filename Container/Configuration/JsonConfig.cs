using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ShipSync.Container.Entity;

namespace ShipSync.Container.Configuration
{
    public class JsonConfig
    {
        public DropboxConfig Dropbox;
        public List<SaveSource> Sources;
    }
}