/***************************************************************************
 *
 * AI Training Export Plugin
 * 
 * Exports UO art assets in a format optimized for training 
 * Stable Diffusion models.
 *
 ***************************************************************************/

using System;
using System.Windows.Forms;
using UoFiddler.Controls.Forms;
using UoFiddler.Controls.Plugin;
using UoFiddler.Controls.Plugin.Interfaces;
using Ultima;

namespace UoFiddler.Plugin.AITrainingExport
{
    public class AITrainingExportPluginBase : PluginBase
    {
        /// <summary>
        /// Name of the plugin
        /// </summary>
        public override string Name { get; } = "AI Training Export";

        /// <summary>
        /// Description of the Plugin's purpose
        /// </summary>
        public override string Description { get; } = "Export UO assets for Stable Diffusion training with background removal, nearest neighbor scaling, and captioning.";

        /// <summary>
        /// Author of the plugin
        /// </summary>
        public override string Author { get; } = "Moshu";

        /// <summary>
        /// Version of the plugin
        /// </summary>
        public override string Version { get; } = "1.0.0";

        /// <summary>
        /// Host of the plugin.
        /// </summary>
        public override IPluginHost Host { get; set; } = null;

        private AITrainingExportForm _exportForm;

        public override void Initialize()
        {
            // Ensure files are accessible
            _ = Files.RootDir;
        }

        public override void Unload()
        {
            _exportForm?.Close();
            _exportForm?.Dispose();
            _exportForm = null;
        }

        public override void ModifyTabPages(TabControl tabControl)
        {
            // This plugin doesn't add a tab
        }

        public override void ModifyPluginToolStrip(ToolStripDropDownButton toolStrip)
        {
            ToolStripMenuItem item = new ToolStripMenuItem
            {
                Text = "AI Training Export..."
            };
            item.Click += ToolStripClick;
            toolStrip.DropDownItems.Add(item);
        }

        private void ToolStripClick(object sender, EventArgs e)
        {
            if (_exportForm?.IsDisposed == false)
            {
                _exportForm.BringToFront();
                return;
            }

            _exportForm = new AITrainingExportForm
            {
                TopMost = true
            };
            _exportForm.Show();
        }
    }
}
