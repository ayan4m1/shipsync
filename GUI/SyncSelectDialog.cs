﻿using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using Eto.Drawing;
using Eto.Forms;
using log4net;
using ShipSync.Container.Service;

namespace ShipSync.GUI
{
    internal class SyncSelectDialog : Form
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SyncSelectDialog));

        public DynamicLayout Display;
        public GridView CraftList;

        public Button Submit;
        public Button Cancel;

        public Button CheckAll;
        public Button CheckNone;

        public IPathService PathService;
        public IShipService ShipService;

        private ObservableCollection<SaveRow> _viewModel;

        public SyncSelectDialog()
        {
            Icon = Icon.FromResource("ShipSync.GUI.app.ico", Assembly.GetExecutingAssembly());
            Title = "ShipSync";
            MinimumSize = new Size(600, 600);
            Resizable = false;

            CraftList = new GridView
            {
                ShowHeader = true,
                AllowColumnReordering = false,
                GridLines = GridLines.Horizontal,
                RowHeight = 32
            };

            CraftList.Columns.Add(new GridColumn
            {
                HeaderText = "Sync",
                AutoSize = false,
                Width = 120,
                DataCell = new CheckBoxCell
                {
                    Binding = Binding.Property<SaveRow, bool?>(item => item.Checked)
                }
            });
            CraftList.Columns.Add(new GridColumn
            {
                HeaderText = "Save Name",
                AutoSize = false,
                Width = 480,
                DataCell = new TextBoxCell
                {
                    Binding = Binding.Property<SaveRow, string>(item => item.Name)
                }
            });

            Submit = new Button(Submit_Click) { Text = "Sync Selected" };
            Cancel = new Button(Cancel_Click) { Text = "Cancel" };
            CheckAll = new Button { Text = "All" };
            CheckNone = new Button { Text = "None" };

            CheckAll.Click += CheckAll_Click;
            CheckNone.Click += CheckNone_Click;

            Display = new DynamicLayout()
            {
                Padding = new Padding(10),
                Spacing = new Size(10, 20)
                /*Rows =
                {
                    new TableRow(new Label { Text = "Which saves would you like to upload?" }, null),
                    new TableRow()
                    {
                        ScaleHeight = true,
                        Cells = { null, CraftList }
                    },
                    new TableRow(CheckAll, CheckNone),
                    new TableRow(Submit, Cancel)
                }*/
            };

            CraftList.Size = new Size(-1, 600);

            Display.BeginHorizontal();
            Display.Add(new Label { Text = "Select saves to sync:" } );
            Display.EndHorizontal();
            Display.EndBeginVertical();
            Display.BeginHorizontal();
            Display.Add(CraftList);
            Display.EndHorizontal();
            Display.EndBeginVertical();
            Display.BeginHorizontal();
            Display.Add(CheckAll);
            Display.Add(CheckNone);
            Display.EndHorizontal();
            Display.EndBeginVertical();
            Display.BeginHorizontal();
            Display.Add(Submit);
            Display.Add(Cancel);
            Display.EndHorizontal();
            Display.EndVertical();
            Display.AddRow();

            Content = Display;
        }

        private void CheckNone_Click(object sender, EventArgs e)
        {
            ChangeCheckState(false);
        }

        private void CheckAll_Click(object sender, EventArgs e)
        {
            ChangeCheckState(true);
        }

        private void ChangeCheckState(bool state)
        {
            _viewModel.ToList().ForEach(row => row.Checked = state);
            CraftList.ReloadData(-1);
        }

        protected override void OnLoadComplete(EventArgs e)
        {
            base.OnLoadComplete(e);

            Submit.Click += Submit_Click;
            Cancel.Click += Cancel_Click;

            var steamPath = PathService.FindSaveSource();
            var saveList = ShipService.FindSavesInSource(steamPath);
            _viewModel = new ObservableCollection<SaveRow>();

            saveList.ForEach(save =>
            {
                _viewModel.Add(new SaveRow()
                {
                    Checked = false,
                    Name = save.Name,
                    Tag = save
                });
            });

            CraftList.DataStore = _viewModel;
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Submit_Click(object sender, EventArgs e)
        {
            var added = 0;
            var updated = 0;
            var scanned = 0;

            var steamPath = PathService.FindSaveSource();
            var baseDest = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Dropbox", "Apps", "ShipSync");
            foreach (var directory in _viewModel)
            {
                if (!directory.Checked)
                {
                    continue;
                }

                var gameSave = directory.Tag;
                var pathFiles = new DirectoryInfo(gameSave.Path).GetFiles("*.craft");
                foreach (var craft in pathFiles)
                {
                    scanned++;
                    var relative = new Uri(steamPath.InstallPath).MakeRelativeUri(new Uri(craft.FullName)).ToString();
                    var saveName = relative.Split('/')[2].ToLower();
                    var craftName =
                        Uri.UnescapeDataString(Path.GetFileName(relative).ToLower().Replace('_', '-')).Replace(' ', '-');
                    var destPath = Path.Combine(baseDest, saveName + "_" + craftName);

                    if (File.Exists(destPath))
                    {
                        var craftTime = File.GetLastWriteTime(relative);
                        var dropboxTime = File.GetLastWriteTime(destPath);
                        if (craftTime.CompareTo(dropboxTime) <= 0)
                        {
                            continue;
                        }

                        updated++;
                        Log.Info("Syncing " + craft + " into " + destPath);
                        using (var writer = File.OpenWrite(destPath))
                        {
                            writer.Write(new byte[writer.Length], 0, (int) writer.Length);
                            writer.Position = 0;
                            using (var reader = File.OpenRead(relative))
                            {
                                reader.CopyTo(writer);
                            }
                            writer.Flush();
                        }
                    }
                    else
                    {
                        added++;
                        File.Copy(craft.FullName, destPath);
                        Log.Info("Adding new craft " + craft + " to " + destPath);
                    }
                }
            }

            MessageBox.Show("Sync completed, added " + added + " and updated " + updated + " out of " + scanned + " craft files.", MessageBoxButtons.OK);
            Eto.Forms.Application.Instance.AsyncInvoke(Close);
        }
    }
}