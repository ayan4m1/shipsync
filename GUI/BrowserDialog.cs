using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using Dropbox.Api;
using Eto.Drawing;
using Eto.Forms;
using log4net;
using ShipSync.Container.Service;
using Form = Eto.Forms.Form;

namespace ShipSync.GUI
{
    internal class BrowserDialog : Form, IDisposable
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(BrowserDialog));
        private static readonly Uri RedirectUri = new Uri("https://localhost/authorize");

        private readonly TaskCompletionSource<bool> _browserInitialized = new TaskCompletionSource<bool>();
        private ChromiumWebBrowser _browser;
        private readonly string _authNonce;

        public IAuthService AuthService;

        public BrowserDialog()
        {
            if (!Cef.IsInitialized)
            {
                Cef.Initialize(new CefSettings());
                _authNonce = Guid.NewGuid().ToString("N");
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            if (Cef.IsInitialized)
            {
                Cef.Shutdown();
            }
        }

        protected override async void OnLoadComplete(EventArgs e)
        {
            base.OnLoadComplete(e);

            var authed = await AuthService.TestAccessToken();
            if (authed)
            {
                Eto.Forms.Application.Instance.AsyncInvoke(Close);
            }

            Size = Eto.Forms.Screen.PrimaryScreen.WorkingArea.Size.ToSize();
            Location = Point.Empty;

            _browser = new ChromiumWebBrowser(string.Empty)
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
            var loadEx = new InvalidOperationException($"{e.FailedUrl} ({e.ErrorText})");
            _browserInitialized.TrySetException(loadEx);
        }

        private void _browser_IsBrowserInitializedChanged(object sender, IsBrowserInitializedChangedEventArgs e)
        {
            Log.Info("Initialized: " + e.IsBrowserInitialized);
            _browserInitialized.TrySetResult(e.IsBrowserInitialized);
            Eto.Forms.Application.Instance.AsyncInvoke(() =>
            {
                var uri = DropboxOAuth2Helper.GetAuthorizeUri(OAuthResponseType.Token, AuthService.ClientIdentifier,
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
            if (string.IsNullOrWhiteSpace(e.Address))
            {
                return;
            }
            Log.Info("Address: " + e.Address);
            Log.Info("Validating response");

            var addressUri = new Uri(e.Address);
            if (addressUri.Host != "localhost")
            {
                return;
            }

            // the first character of the fragment is '#', discard it
            var queryParams = HttpUtility.ParseQueryString(addressUri.Fragment.Substring(1));
            var stateCheck = queryParams["state"];
            if (_authNonce.Equals(stateCheck))
            {
                Log.Info("State validation completed successfully, setting token");
                AuthService.UpdateToken(queryParams["access_token"]);
                Eto.Forms.Application.Instance.AsyncInvoke(Close);
            }
        }

        private void _browser_TitleChanged(object sender, TitleChangedEventArgs e)
        {
            Log.Debug("Title: " + e.Title);
        }

        private void _browser_ConsoleMessage(object sender, ConsoleMessageEventArgs e)
        {
            Log.Info(e.Source + "> " + e.Message);
        }

        private void _browser_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            Log.Debug("Loading is " + e.IsLoading);
        }
    }
}