using System;
using Autofac;
using Eto.Forms;
using log4net;
using ShipSync.Container;
using ShipSync.Container.Entity;

namespace ShipSync.GUI
{
    public sealed class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Program));

        [STAThread]
        private static void Main(string[] args)
        {
            var appInstance = new Application();
            var form = new MainForm();

            var appConf = App.Container.Resolve<Config>();
            Log.Debug(appConf.Dropbox["secret"]);

            appInstance.Run(form);
        }
    }
}