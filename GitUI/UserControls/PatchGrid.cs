﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using GitCommands.Patches;
using ResourceManager;

namespace GitUI
{
    public partial class PatchGrid : GitModuleControl
    {
        private readonly TranslationString _unableToShowPatchDetails = new TranslationString("Unable to show details of patch file.");

        public PatchGrid()
        {
            InitializeComponent();
            Translate();

            FileName.Width = DpiUtil.Scale(50);
            authorDataGridViewTextBoxColumn.Width = DpiUtil.Scale(140);
            dateDataGridViewTextBoxColumn.Width = DpiUtil.Scale(160);
            Status.Width = DpiUtil.Scale(80);
        }

        protected override void OnRuntimeLoad(EventArgs e)
        {
            Initialize();
        }

        public void Initialize()
        {
            IReadOnlyList<PatchFile> patchFiles;

            if (Module.InTheMiddleOfInteractiveRebase())
            {
                Patches.DataSource = patchFiles = Module.GetInteractiveRebasePatchFiles();
            }
            else
            {
                Patches.DataSource = patchFiles = Module.GetRebasePatchFiles();
            }

            if (patchFiles.Any())
            {
                int rowsInView = Patches.DisplayedRowCount(false);
                Patches.FirstDisplayedScrollingRowIndex = Math.Max(0, patchFiles.TakeWhile(pf => !pf.IsNext).Count() - (rowsInView / 2));
            }
        }

        private void Patches_DoubleClick(object sender, EventArgs e)
        {
            if (Patches.SelectedRows.Count != 1)
            {
                return;
            }

            var patchFile = (PatchFile)Patches.SelectedRows[0].DataBoundItem;

            if (string.IsNullOrEmpty(patchFile.FullName))
            {
                MessageBox.Show(_unableToShowPatchDetails.Text);
                return;
            }

            UICommands.StartViewPatchDialog(patchFile.FullName);
        }
    }
}
