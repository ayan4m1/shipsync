using System;
using System.Threading;
using System.Windows.Forms;
using CefSharp;
using CefSharp.Internals;
using CefSharp.WinForms;
using Dropbox.Api;
using Eto.Drawing;
using Eto.Forms;
using Eto.WinForms;
using log4net;
using Form = Eto.Forms.Form;

namespace ShipSync.GUI
{
    internal class BrowserForm : Form
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(BrowserForm));

        private ChromiumWebBrowser _browser;
        private readonly string _clientId;

        static BrowserForm()
        {
            Cef.Initialize(new CefSettings());
        }

        public BrowserForm(string clientId)
        {
            _clientId = clientId;
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

            _browser = new ChromiumWebBrowser(null)
            {
                Dock = DockStyle.Fill
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
        }

        private void _browser_IsBrowserInitializedChanged(object sender, IsBrowserInitializedChangedEventArgs e)
        {
            Log.Info("Initialized: " + e.IsBrowserInitialized);

            if (e.IsBrowserInitialized)
            {
                var uri = DropboxOAuth2Helper.GetAuthorizeUri(_clientId).ToString();
                Log.Info("Navigating to auth page @ " + uri);
                _browser.Load(uri);
            }
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