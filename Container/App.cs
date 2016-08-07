using System;
using System.IO;
using System.Reflection;
using Autofac;
using log4net;
using ShipSync.Container.Modules;

namespace ShipSync.Container
{
    public class App
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(App));

        public static IContainer Container;

        /// <summary>
        ///     Static initializer for the application container.
        ///     If this fails, it will throw a TypeInitializationException.
        ///     This is desired because the application should not run without a container.
        /// </summary>
        static App()
        {
            var builder = new ContainerBuilder();

            Log.Debug("Getting current assembly");
            var containerAssy = Assembly.GetAssembly(typeof(App));

            Log.Debug("Registering Services and Repositories");
            builder.RegisterAssemblyTypes(containerAssy)
                .Where(t => t.Name.EndsWith("Repository") || t.Name.EndsWith("Service"))
                .AsImplementedInterfaces()
                .PropertiesAutowired()
                .SingleInstance();

            Log.Debug("Reading configuration");
            var configPath = Path.Combine(Environment.CurrentDirectory, "app.json");
            builder.RegisterModule(new JsonConfigModule(configPath));

            Log.Debug("Creating application container");
            Container = builder.Build();
        }
    }
}