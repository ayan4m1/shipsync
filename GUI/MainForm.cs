using System;
using System.IO;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;
using log4net;
using ShipSync.Container.Service;

namespace ShipSync.GUI
{
    internal class MainForm : Form
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(MainForm));

        public TableLayout Display = new TableLayout(1, 2);
        public TreeView CraftList = new TreeView();
        public Button Submit;
        public Button Cancel;

        public IPathService PathService;
        public IShipService ShipService;

        public MainForm()
        {
            Title = "ShipSync";
            ClientSize = new Size(800, 600);

            Submit = new Button(Submit_Click)
            {
                Text = "Sync Selected"
            };
            Cancel = new Button(Cancel_Click)
            {
                Text = "Cancel"
            };

            Display.Padding = new Padding(10, 5, 0, 5);
            Display.Spacing = new Size(5, 5);
            Display.Rows.Add(new TableRow(new TableCell(CraftList)));
            Display.Rows.Add(new TableRow(new TableCell(Submit), new TableCell(Cancel)));

            Content = Display;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            var steamPath = PathService.FindSaveSource();
            var saveList = ShipService.FindSavesInSource(steamPath);
            
        }

        protected override void OnLoadComplete(EventArgs e)
        {
            base.OnLoadComplete(e);
            Submit.Click += Submit_Click;
            Cancel.Click += Cancel_Click;
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Submit_Click(object sender, EventArgs e)
        {
            var steamPath = PathService.FindSaveSource();
            var baseDest = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Dropbox", "Apps", "ShipSync");
            /*foreach (var craft in CraftList.Items)
            {
                var relative = new Uri(steamPath.InstallPath).MakeRelativeUri(new Uri(craft.Text)).ToString();
                var saveName = relative.Split('/')[1].ToLower();
                var craftName = Uri.UnescapeDataString(Path.GetFileName(relative).ToLower().Replace('_', '-')).Replace(' ', '-');
                var destPath = Path.Combine(baseDest, saveName + "_" + craftName);

                if (File.Exists(destPath))
                {
                    var craftTime = File.GetLastWriteTime(craft.Text);
                    var dropboxTime = File.GetLastWriteTime(destPath);
                    if (craftTime.CompareTo(dropboxTime) > 0)
                    {
                        Log.Info("Syncing " + craft + " into " + destPath);
                        using (var writer = File.OpenWrite(destPath))
                        {
                            writer.Write(new byte[writer.Length], 0, (int)writer.Length);
                            writer.Position = 0;
                            using (var reader = File.OpenRead(craft.Text))
                            {
                                reader.CopyTo(writer);
                            }
                            writer.Flush();
                        }
                    }
                }
                else
                {
                    File.Copy(craft.Text, destPath);
                    Log.Info("Adding new craft " + craft + " to " + destPath);
                }
            }*/
        }
    }
}