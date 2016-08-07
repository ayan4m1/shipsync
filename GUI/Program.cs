using System;
using Autofac;
using Eto.Forms;
using log4net;
using ShipSync.Container;
using ShipSync.Container.Configuration;

namespace ShipSync.GUI
{
    public sealed class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Program));

        [STAThread]
        private static void Main(string[] args)
        {
            using (var appInstance = new Application())
            {
                var appConf = App.Container.Resolve<JsonConfig>();
                var form = new BrowserForm(appConf.Dropbox["key"]);
                appInstance.Run(form);
            }
        }
    }
}