/***************************************************************************
 *
 * AI Training Export Form
 * 
 * Configuration UI for exporting UO art assets for Stable Diffusion training.
 *
 ***************************************************************************/

using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using Ultima;
using UoFiddler.Controls.Classes;
using UoFiddler.Controls.Helpers;

namespace UoFiddler.Controls.Forms
{
    public partial class AITrainingExportForm : Form
    {
        private BackgroundWorker _exportWorker;

        public AITrainingExportForm()
        {
            InitializeComponent();
            SetupForm();
        }

        private void SetupForm()
        {
            // Initialize asset type combo
            assetTypeComboBox.Items.Clear();
            assetTypeComboBox.Items.Add("Static Items");
            assetTypeComboBox.Items.Add("Land Tiles");
            assetTypeComboBox.Items.Add("Gumps");
            assetTypeComboBox.SelectedIndex = 0;

            // Initialize target size combo
            targetSizeComboBox.Items.Clear();
            targetSizeComboBox.Items.Add("512 x 512 (SD 1.5)");
            targetSizeComboBox.Items.Add("1024 x 1024 (SDXL)");
            targetSizeComboBox.SelectedIndex = 0;

            // Initialize background combo
            backgroundComboBox.Items.Clear();
            backgroundComboBox.Items.Add("Transparent");
            backgroundComboBox.Items.Add("White");
            backgroundComboBox.SelectedIndex = 0;

            // Set default values
            captionTemplateTextBox.Text = "uo sprite, pixelart, {name}, isometric, rpg game asset";
            startIndexNumeric.Minimum = 0;
            startIndexNumeric.Maximum = 0xFFFF;
            startIndexNumeric.Value = 0;
            endIndexNumeric.Minimum = 0;
            endIndexNumeric.Maximum = 0xFFFF;
            endIndexNumeric.Value = 1000;

            outputFolderTextBox.Text = Options.OutputPath;

            // Setup background worker
            _exportWorker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            _exportWorker.DoWork += ExportWorker_DoWork;
            _exportWorker.ProgressChanged += ExportWorker_ProgressChanged;
            _exportWorker.RunWorkerCompleted += ExportWorker_Completed;

            progressBar.Visible = false;
            cancelButton.Enabled = false;
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Select output folder for AI training dataset";
                dialog.ShowNewFolderButton = true;
                dialog.SelectedPath = outputFolderTextBox.Text;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    outputFolderTextBox.Text = dialog.SelectedPath;
                }
            }
        }

        private void exportButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(outputFolderTextBox.Text))
            {
                MessageBox.Show("Please select an output folder.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!Directory.Exists(outputFolderTextBox.Text))
            {
                try
                {
                    Directory.CreateDirectory(outputFolderTextBox.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to create output folder: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            if (startIndexNumeric.Value > endIndexNumeric.Value)
            {
                MessageBox.Show("Start index must be less than or equal to end index.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Prepare options
            var options = new AIExportOptions
            {
                TargetSize = targetSizeComboBox.SelectedIndex == 0 ? 512 : 1024,
                UseWhiteBackground = backgroundComboBox.SelectedIndex == 1,
                CenterSprite = true,
                CaptionTemplate = captionTemplateTextBox.Text,
                IncludeFlags = includeFlagsCheckBox.Checked
            };

            // Prepare export parameters
            var exportParams = new ExportParameters
            {
                AssetType = (AIExportAssetType)assetTypeComboBox.SelectedIndex,
                StartIndex = (int)startIndexNumeric.Value,
                EndIndex = (int)endIndexNumeric.Value,
                OutputFolder = outputFolderTextBox.Text,
                Options = options
            };

            // UI state for export
            exportButton.Enabled = false;
            cancelButton.Enabled = true;
            progressBar.Visible = true;
            progressBar.Value = 0;
            statusLabel.Text = "Starting export...";

            _exportWorker.RunWorkerAsync(exportParams);
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            if (_exportWorker.IsBusy)
            {
                _exportWorker.CancelAsync();
                statusLabel.Text = "Cancelling...";
            }
        }

        private void ExportWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = (BackgroundWorker)sender;
            var parameters = (ExportParameters)e.Argument;

            int exported = 0;
            Action<int, int> progressCallback = (current, total) =>
            {
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                int percent = (int)((current / (float)total) * 100);
                worker.ReportProgress(percent, $"Processing {current} of {total}...");
            };

            switch (parameters.AssetType)
            {
                case AIExportAssetType.StaticItem:
                    exported = AITrainingExportHelper.BatchExportStaticItems(
                        parameters.StartIndex, parameters.EndIndex,
                        parameters.OutputFolder, parameters.Options, progressCallback);
                    break;

                case AIExportAssetType.LandTile:
                    exported = AITrainingExportHelper.BatchExportLandTiles(
                        parameters.StartIndex, parameters.EndIndex,
                        parameters.OutputFolder, parameters.Options, progressCallback);
                    break;

                case AIExportAssetType.Gump:
                    exported = AITrainingExportHelper.BatchExportGumps(
                        parameters.StartIndex, parameters.EndIndex,
                        parameters.OutputFolder, parameters.Options, progressCallback);
                    break;
            }

            e.Result = exported;
        }

        private void ExportWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = Math.Min(e.ProgressPercentage, 100);
            if (e.UserState is string status)
            {
                statusLabel.Text = status;
            }
        }

        private void ExportWorker_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            exportButton.Enabled = true;
            cancelButton.Enabled = false;
            progressBar.Visible = false;

            if (e.Cancelled)
            {
                statusLabel.Text = "Export cancelled.";
                MessageBox.Show("Export was cancelled.", "Cancelled",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (e.Error != null)
            {
                statusLabel.Text = "Export failed.";
                MessageBox.Show($"Export failed: {e.Error.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                int exported = (int)e.Result;
                statusLabel.Text = $"Exported {exported} assets.";
                MessageBox.Show($"Successfully exported {exported} assets to:\n{outputFolderTextBox.Text}",
                    "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void assetTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Update max range based on asset type
            switch (assetTypeComboBox.SelectedIndex)
            {
                case 0: // Static Items
                    endIndexNumeric.Maximum = Art.GetMaxItemId();
                    break;
                case 1: // Land Tiles
                    endIndexNumeric.Maximum = 0x3FFF;
                    break;
                case 2: // Gumps
                    endIndexNumeric.Maximum = Gumps.GetCount() - 1;
                    break;
            }

            if (endIndexNumeric.Value > endIndexNumeric.Maximum)
            {
                endIndexNumeric.Value = endIndexNumeric.Maximum;
            }
        }

        private class ExportParameters
        {
            public AIExportAssetType AssetType { get; set; }
            public int StartIndex { get; set; }
            public int EndIndex { get; set; }
            public string OutputFolder { get; set; }
            public AIExportOptions Options { get; set; }
        }
    }
}
