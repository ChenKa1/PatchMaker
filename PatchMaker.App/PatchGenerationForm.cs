﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml.Linq;

namespace PatchMaker.App
{
    public partial class PatchGenerationForm : Form
    {
        private IEnumerable<BasePatch> _patches;
        private XDocument _sourceXml;
        private XDocument _patch;
        private string _sourceFileName;

        public PatchGenerationForm(ListBox patchListView, XDocument sourceXml, string sourceFileName)
        {
            InitializeComponent();
            this.ConfigureDialog();

            if(!File.Exists("Sitecore.Kernel.dll"))
            {
                previewBtn.Enabled = false;
            }

            if(patchListView == null)
            {
                throw new ArgumentNullException(nameof(patchListView));
            }

            _sourceXml = sourceXml ?? throw new ArgumentNullException(nameof(sourceXml));

            _sourceFileName = sourceFileName;

            _patches = extractPatches(patchListView);

            var generator = new PatchGenerator(_sourceXml);
            _patch = generator.GeneratePatchFile(_patches);
            patchXmlEdit.Text = _patch.ToString();

            patchXmlEdit.Select(0, 0);
        }

        private IEnumerable<BasePatch> extractPatches(ListBox listView)
        {
            var patches = new List<BasePatch>();

            foreach(PatchItem patchListItem in listView.Items)
            {
                if(patchListItem != null)
                {
                    patches.Add(patchListItem.Patch);
                }
            }

            return patches;
        }

        private void previewBtn_Click(object sender, EventArgs e)
        {
            var preview = new PatchPreviewForm(_sourceXml.ToString(), patchXmlEdit.Text);
            var dr = preview.ShowDialog(this);
        }

        private string makeNewFilename(string fileName)
        {
            return $"{Path.GetFileNameWithoutExtension(fileName)}-patch";
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            var sfd = new SaveFileDialog() { 
                Filter = "Sitecore Config Patch (*.config)|*.config",
                OverwritePrompt = true,
                FileName = makeNewFilename(_sourceFileName)
            };
            var dr = sfd.ShowDialog(this);

            if(dr == DialogResult.OK)
            {
                _patch.Save(sfd.FileName);
            }
        }

        private void PatchGenerationForm_HelpButtonClicked(object sender, System.ComponentModel.CancelEventArgs e)
        {
            HelpSpawner.SpawnLocalFile("generate");
        }
    }
}