using System;
using System.Threading.Tasks;
using Autofac;
using CefSharp;
using CefSharp.WinForms;
using Dropbox.Api;
using Eto.Drawing;
using Eto.Forms;
using log4net;
using ShipSync.Container.Configuration;

namespace ShipSync.GUI
{
    internal class BrowserDialog : Dialog
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(BrowserDialog));
        private static readonly Uri RedirectUri = new Uri("https://localhost/authorize");

        private readonly TaskCompletionSource<bool> _browserInitialized = new TaskCompletionSource<bool>();
        private readonly IContainer _container;
        private ChromiumWebBrowser _browser;
        private readonly string _authNonce;

        public BrowserDialog(IContainer container)
        {
            Cef.Initialize(new CefSettings());
            _container = container;
            _authNonce = Guid.NewGuid().ToString("N");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && Cef.IsInitialized)
            {
                Cef.Shutdown();
            }

            base.Dispose(disposing);
        }

        protected override void OnLoadComplete(EventArgs e)
        {
            base.OnLoadComplete(e);

            _browser = new ChromiumWebBrowser(string.Empty)
            {
                Dock = System.Windows.Forms.DockStyle.Fill
            };

            _browser.LoadingStateChanged += _browser_LoadingStateChanged;
            _browser.ConsoleMessage += _browser_ConsoleMessage;
            _browser.TitleChanged += _browser_TitleChanged;
            _browser.AddressChanged += _browser_AddressChanged;
            _browser.StatusMessage += _browser_StatusMessage;
            _browser.IsBrowserInitializedChanged += _browser_IsBrowserInitializedChanged;
            _browser.LoadError += _browser_LoadError;
            _browser.HandleCreated += _browser_HandleCreated;
            _browser.Resize += _browser_Resize;
            Content = _browser.ToEto();
        }

        private void _browser_Resize(object sender, EventArgs e)
        {
            Log.Info("Resized");
            MinimumSize = Size = Size.Round(Screen.WorkingArea.Size);
        }

        private void _browser_HandleCreated(object sender, EventArgs e)
        {
            Log.Info("Handle created");
        }

        private void _browser_LoadError(object sender, LoadErrorEventArgs e)
        {
            Log.Info("Load error");
            var loadEx = new InvalidOperationException($"{e.FailedUrl} ({e.ErrorText})");
            _browserInitialized.TrySetException(loadEx);
        }

        private void _browser_IsBrowserInitializedChanged(object sender, IsBrowserInitializedChangedEventArgs e)
        {
            Log.Info("Initialized: " + e.IsBrowserInitialized);
            _browserInitialized.TrySetResult(e.IsBrowserInitialized);
            Application.Instance.AsyncInvoke(() =>
            {
                var config = _container.Resolve<JsonConfig>();
                var uri = DropboxOAuth2Helper.GetAuthorizeUri(OAuthResponseType.Token, config.Dropbox.Client,
                    RedirectUri, _authNonce);
                Log.Info("Navigating to auth page @ " + uri);
                _browser.Load(uri.ToString());
            });
        }

        private void _browser_StatusMessage(object sender, StatusMessageEventArgs e)
        {
            Log.Info("Status: " + e.Value);
        }

        private void _browser_AddressChanged(object sender, AddressChangedEventArgs e)
        {
            Log.Info("Address: " + e.Address);
        }

        private void _browser_TitleChanged(object sender, TitleChangedEventArgs e)
        {
            Log.Info("Title: " + e.Title);
        }

        private void _browser_ConsoleMessage(object sender, ConsoleMessageEventArgs e)
        {
            Log.Info(e.Source + ": " + e.Message);
        }

        private void _browser_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            Log.Info("Loading is " + e.IsLoading);
        }
    }
}