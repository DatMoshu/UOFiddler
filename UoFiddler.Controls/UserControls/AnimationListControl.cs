/***************************************************************************
 *
 * $Author: Turley
 * 
 * "THE BEER-WARE LICENSE"
 * As long as you retain this notice you can do whatever you want with 
 * this stuff. If we meet some day, and you think this stuff is worth it,
 * you can buy me a beer in return.
 *
 ***************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Text.Json;
using System.Text.Json.Serialization;
using Ultima;
using UoFiddler.Controls.Classes;
using UoFiddler.Controls.Forms;
using UoFiddler.Controls.Helpers;

namespace UoFiddler.Controls.UserControls
{
    public partial class AnimationListControl : UserControl
    {
        public AnimationListControl()
        {
            InitializeComponent();

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
            // TODO can this be moved into the control itself?
            listView1.Height += SystemInformation.HorizontalScrollBarHeight;
            // Add handlers for new context menu items
            packFramesToolStripMenuItem.Click += OnPackFramesClick;
            unpackFramesToolStripMenuItem.Click += OnUnpackFramesClick;
        }


        public string[][] GetActionNames { get; } = {
            // Monster
            new[]
            {
                "Walk",
                "Idle",
                "Die1",
                "Die2",
                "Attack1",
                "Attack2",
                "Attack3",
                "AttackBow",
                "AttackCrossBow",
                "AttackThrow",
                "GetHit",
                "Pillage",
                "Stomp",
                "Cast2",
                "Cast3",
                "BlockRight",
                "BlockLeft",
                "Idle",
                "Fidget",
                "Fly",
                "TakeOff",
                "GetHitInAir"
            },
            // Sea
            new[]
            {
                "Walk",
                "Run",
                "Idle",
                "Idle",
                "Fidget",
                "Attack1",
                "Attack2",
                "GetHit",
                "Die1"
            },
            // Animal
            new[]
            {
                "Walk",
                "Run",
                "Idle",
                "Eat",
                "Alert",
                "Attack1",
                "Attack2",
                "GetHit",
                "Die1",
                "Idle",
                "Fidget",
                "LieDown",
                "Die2"
            },
            // Human
            new[]
            {
                "Walk_01",
                "WalkStaff_01",
                "Run_01",
                "RunStaff_01",
                "Idle_01",
                "Idle_01",
                "Fidget_Yawn_Stretch_01",
                "CombatIdle1H_01",
                "CombatIdle1H_01",
                "AttackSlash1H_01",
                "AttackPierce1H_01",
                "AttackBash1H_01",
                "AttackBash2H_01",
                "AttackSlash2H_01",
                "AttackPierce2H_01",
                "CombatAdvance_1H_01",
                "Spell1",
                "Spell2",
                "AttackBow_01",
                "AttackCrossbow_01",
                "GetHit_Fr_Hi_01",
                "Die_Hard_Fwd_01",
                "Die_Hard_Back_01",
                "Horse_Walk_01",
                "Horse_Run_01",
                "Horse_Idle_01",
                "Horse_Attack1H_SlashRight_01",
                "Horse_AttackBow_01",
                "Horse_AttackCrossbow_01",
                "Horse_Attack2H_SlashRight_01",
                "Block_Shield_Hard_01",
                "Punch_Punch_Jab_01",
                "Bow_Lesser_01",
                "Salute_Armed1h_01",
                "Ingest_Eat_01"
            }
        };

        private int _currentSelect;
        private int _currentSelectAction;
        private int _customHue;
        private int _defHue;
        private int _facing = 1;
        private bool _sortAlpha;
        private int _displayType;
        private bool _loaded;

        /// <summary>
        /// ReLoads if loaded
        /// </summary>
        private void Reload()
        {
            if (!_loaded)
            {
                return;
            }

            _currentSelect = 0;
            _currentSelectAction = 0;
            _customHue = 0;
            _defHue = 0;
            _facing = 1;
            _sortAlpha = false;
            _displayType = 0;
            MainPictureBox.Reset();
            AnimateCheckBox.Checked = false;
            ShowFrameBoundsCheckBox.Checked = false;

            OnLoad(this, EventArgs.Empty);
        }

        private void OnLoad(object sender, EventArgs e)
        {
            if (IsAncestorSiteInDesignMode || FormsDesignerHelper.IsInDesignMode())
            {
                return;
            }

            Cursor.Current = Cursors.WaitCursor;
            Options.LoadedUltimaClass["Animations"] = true;
            Options.LoadedUltimaClass["Hues"] = true;
            TreeViewMobs.TreeViewNodeSorter = new GraphicSorter();
            if (!LoadXml())
            {
                Cursor.Current = Cursors.Default;
                return;
            }

            LoadListView();

            _currentSelect = 0;
            _currentSelectAction = 0;
            if (TreeViewMobs.Nodes[0].Nodes.Count > 0)
            {
                TreeViewMobs.SelectedNode = TreeViewMobs.Nodes[0].Nodes[0];
            }

            FacingBar.Value = (_facing + 3) & 7;
            if (!_loaded)
            {
                ControlEvents.FilePathChangeEvent += OnFilePathChangeEvent;
            }

            _loaded = true;
            Cursor.Current = Cursors.Default;
        }

        private void OnFilePathChangeEvent()
        {
            Reload();
        }

        /// <summary>
        /// Changes Hue of current Mob
        /// </summary>
        /// <param name="select"></param>
        public void ChangeHue(int select)
        {
            _customHue = select + 1;
            CurrentSelect = CurrentSelect;
        }

        /// <summary>
        /// Is Graphic already in TreeView
        /// </summary>
        /// <param name="graphic"></param>
        /// <returns></returns>
        public bool IsAlreadyDefined(int graphic)
        {
            return TreeViewMobs.Nodes[0].Nodes.Cast<TreeNode>().Any(node => ((int[])node.Tag)[0] == graphic) ||
                   TreeViewMobs.Nodes[1].Nodes.Cast<TreeNode>().Any(node => ((int[])node.Tag)[0] == graphic);
        }

        /// <summary>
        /// Adds Graphic with type and name to List
        /// </summary>
        /// <param name="graphic"></param>
        /// <param name="type"></param>
        /// <param name="name"></param>
        public void AddGraphic(int graphic, int type, string name)
        {
            TreeViewMobs.BeginUpdate();
            TreeViewMobs.TreeViewNodeSorter = null;
            TreeNode nodeParent = new TreeNode(name)
            {
                Tag = new[] { graphic, type },
                ToolTipText = Animations.GetFileName(graphic)
            };

            if (type == 4)
            {
                TreeViewMobs.Nodes[1].Nodes.Add(nodeParent);
                type = 3;
            }
            else
            {
                TreeViewMobs.Nodes[0].Nodes.Add(nodeParent);
            }

            for (int i = 0; i < GetActionNames[type].GetLength(0); ++i)
            {
                if (!Animations.IsActionDefined(graphic, i, 0))
                {
                    continue;
                }

                TreeNode node = new TreeNode($"{i} {GetActionNames[type][i]}")
                {
                    Tag = i
                };

                nodeParent.Nodes.Add(node);
            }

            TreeViewMobs.TreeViewNodeSorter = !_sortAlpha
                ? new GraphicSorter()
                : (IComparer)new AlphaSorter();

            TreeViewMobs.Sort();
            TreeViewMobs.EndUpdate();
            LoadListView();
            TreeViewMobs.SelectedNode = nodeParent;
            nodeParent.EnsureVisible();
        }

        private bool Animate
        {
            get => MainPictureBox.Animate;
            set => MainPictureBox.Animate = value;
        }

        private int CurrentSelect
        {
            get => _currentSelect;
            set
            {
                _currentSelect = value;
                SetPicture();
            }
        }

        private void SetPicture()
        {
            if (_currentSelect == 0)
            {
                return;
            }

            int body = _currentSelect;
            Animations.Translate(ref body);
            int hue = _customHue;
            bool preserveHue = hue != 0;

            MainPictureBox.Frames = Animations.GetAnimation(_currentSelect, _currentSelectAction, _facing, ref hue, preserveHue, false)
                ?.Select(animation => new AnimatedFrame(animation.Bitmap, animation.Center)).ToList();

            if (!preserveHue)
            {
                _defHue = hue;
            }

            if (MainPictureBox.FirstFrame == null)
            {
                return;
            }

            BaseGraphicLabel.Text = $"BaseGraphic: {body} (0x{body:X})";
            GraphicLabel.Text = $"Graphic: {_currentSelect} (0x{_currentSelect:X})";
            HueLabel.Text = $"Hue: {hue + 1} (0x{hue + 1:X})";

            LoadListViewFrames();
        }

        private void TreeViewMobs_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Parent != null)
            {
                if (e.Node.Parent.Name == "Mobs" || e.Node.Parent.Name == "Equipment")
                {
                    _currentSelectAction = 0;
                    CurrentSelect = ((int[])e.Node.Tag)[0];
                    if (e.Node.Parent.Name == "Mobs" && _displayType == 1)
                    {
                        _displayType = 0;
                        LoadListView();
                    }
                    else if (e.Node.Parent.Name == "Equipment" && _displayType == 0)
                    {
                        _displayType = 1;
                        LoadListView();
                    }
                }
                else
                {
                    _currentSelectAction = (int)e.Node.Tag;
                    CurrentSelect = ((int[])e.Node.Parent.Tag)[0];
                    if (e.Node.Parent.Parent.Name == "Mobs" && _displayType == 1)
                    {
                        _displayType = 0;
                        LoadListView();
                    }
                    else if (e.Node.Parent.Parent.Name == "Equipment" && _displayType == 0)
                    {
                        _displayType = 1;
                        LoadListView();
                    }
                }
            }
            else
            {
                if (e.Node.Name == "Mobs" && _displayType == 1)
                {
                    _displayType = 0;
                    LoadListView();
                }
                else if (e.Node.Name == "Equipment" && _displayType == 0)
                {
                    _displayType = 1;
                    LoadListView();
                }
                TreeViewMobs.SelectedNode = e.Node.Nodes[0];
            }
        }

        private bool LoadXml()
        {
            string fileName = Path.Combine(Options.AppDataPath, "Animationlist.xml");
            if (!File.Exists(fileName))
            {
                return false;
            }

            TreeViewMobs.BeginUpdate();
            try
            {
                TreeViewMobs.Nodes.Clear();

                XmlDocument dom = new XmlDocument();
                dom.Load(fileName);

                XmlElement xMobs = dom["Graphics"];
                List<TreeNode> nodes = new List<TreeNode>();
                TreeNode node;
                TreeNode typeNode;

                TreeNode rootNode = new TreeNode("Mobs")
                {
                    Name = "Mobs",
                    Tag = -1
                };
                nodes.Add(rootNode);

                foreach (XmlElement xMob in xMobs.SelectNodes("Mob"))
                {
                    string name = xMob.GetAttribute("name");
                    int value = int.Parse(xMob.GetAttribute("body"));
                    int type = int.Parse(xMob.GetAttribute("type"));
                    node = new TreeNode($"{name} (0x{value:X})")
                    {
                        Tag = new[] { value, type },
                        ToolTipText = Animations.GetFileName(value)
                    };
                    rootNode.Nodes.Add(node);

                    for (int i = 0; i < GetActionNames[type].GetLength(0); ++i)
                    {
                        if (!Animations.IsActionDefined(value, i, 0))
                        {
                            continue;
                        }

                        typeNode = new TreeNode($"{i} {GetActionNames[type][i]}")
                        {
                            Tag = i
                        };
                        node.Nodes.Add(typeNode);
                    }
                }

                rootNode = new TreeNode("Equipment")
                {
                    Name = "Equipment",
                    Tag = -2
                };
                nodes.Add(rootNode);

                foreach (XmlElement xMob in xMobs.SelectNodes("Equip"))
                {
                    string name = xMob.GetAttribute("name");
                    int value = int.Parse(xMob.GetAttribute("body"));
                    int type = int.Parse(xMob.GetAttribute("type"));
                    node = new TreeNode(name)
                    {
                        Tag = new[] { value, type },
                        ToolTipText = Animations.GetFileName(value)
                    };
                    rootNode.Nodes.Add(node);

                    for (int i = 0; i < GetActionNames[type].GetLength(0); ++i)
                    {
                        if (!Animations.IsActionDefined(value, i, 0))
                        {
                            continue;
                        }

                        typeNode = new TreeNode($"{i} {GetActionNames[type][i]}")
                        {
                            Tag = i
                        };
                        node.Nodes.Add(typeNode);
                    }
                }
                TreeViewMobs.Nodes.AddRange(nodes.ToArray());
                nodes.Clear();
            }
            finally
            {
                TreeViewMobs.EndUpdate();
            }

            return true;
        }

        private void LoadListView()
        {
            listView.BeginUpdate();
            try
            {
                listView.Clear();
                foreach (TreeNode node in TreeViewMobs.Nodes[_displayType].Nodes)
                {
                    ListViewItem item = new ListViewItem($"({((int[])node.Tag)[0]})", 0)
                    {
                        Tag = ((int[])node.Tag)[0]
                    };
                    listView.Items.Add(item);
                }
            }
            finally
            {
                listView.EndUpdate();
            }
        }

        private void SelectChanged_listView(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count > 0)
            {
                TreeViewMobs.SelectedNode = TreeViewMobs.Nodes[_displayType].Nodes[listView.SelectedItems[0].Index];
            }
        }

        private void ListView_DoubleClick(object sender, MouseEventArgs e)
        {
            tabControl1.SelectTab(tabPage1);
        }

        private void ListViewDrawItem(object sender, DrawListViewItemEventArgs e)
        {
            int graphic = (int)e.Item.Tag;
            int hue = 0;
            Bitmap bmp = Animations.GetAnimation(graphic, 0, 1, ref hue, false, true)?[0].Bitmap;

            if (bmp == null)
            {
                return;
            }

            int width = bmp.Width;
            int height = bmp.Height;

            if (width > e.Bounds.Width)
            {
                width = e.Bounds.Width;
            }

            if (height > e.Bounds.Height)
            {
                height = e.Bounds.Height;
            }

            e.Graphics.DrawImage(bmp, e.Bounds.X, e.Bounds.Y, width, height);
            e.DrawText(TextFormatFlags.Bottom | TextFormatFlags.HorizontalCenter);
            if (listView.SelectedItems.Contains(e.Item))
            {
                e.DrawFocusRectangle();
            }
            else
            {
                using (var pen = new Pen(Color.Gray))
                {
                    e.Graphics.DrawRectangle(pen, e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height);
                }
            }
        }

        private HuePopUpForm _showForm;

        private void OnClick_Hue(object sender, EventArgs e)
        {
            if (_showForm?.IsDisposed == false)
            {
                return;
            }

            _showForm = _customHue == 0
                ? new HuePopUpForm(ChangeHue, _defHue + 1)
                : new HuePopUpForm(ChangeHue, _customHue - 1);

            _showForm.TopMost = true;
            _showForm.Show();
        }

        private void LoadListViewFrames()
        {
            listView1.BeginUpdate();
            try
            {
                listView1.Clear();
                for (int frame = 0; frame < MainPictureBox.Frames?.Count; ++frame)
                {
                    ListViewItem item = new ListViewItem(frame.ToString(), 0)
                    {
                        Tag = frame
                    };
                    listView1.Items.Add(item);
                }
            }
            finally
            {
                listView1.EndUpdate();
            }
        }

        private void Frames_ListView_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            if (MainPictureBox.Frames == null)
            {
                return;
            }

            Bitmap bmp = MainPictureBox.Frames[(int)e.Item.Tag].Bitmap;
            int width = bmp.Width;
            int height = bmp.Height;

            if (width > e.Bounds.Width)
            {
                width = e.Bounds.Width;
            }

            if (height > e.Bounds.Height)
            {
                height = e.Bounds.Height;
            }

            if (listView1.SelectedItems.Contains(e.Item))
            {
                e.Graphics.FillRectangle(new SolidBrush(SystemColors.Highlight), e.Bounds);
            }

            e.Graphics.DrawImage(bmp, e.Bounds.X, e.Bounds.Y, width, height);
            e.DrawText(TextFormatFlags.Bottom | TextFormatFlags.HorizontalCenter);

            using (var pen = new Pen(Color.Gray))
            {
                e.Graphics.DrawRectangle(pen, e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height);
            }
        }

        private void OnScrollFacing(object sender, EventArgs e)
        {
            _facing = (FacingBar.Value - 3) & 7;
            CurrentSelect = CurrentSelect;
        }

        private void OnClick_Sort(object sender, EventArgs e)
        {
            _sortAlpha = !_sortAlpha;

            TreeViewMobs.BeginUpdate();
            try
            {
                TreeViewMobs.TreeViewNodeSorter = !_sortAlpha ? new GraphicSorter() : (IComparer)new AlphaSorter();
                TreeViewMobs.Sort();
            }
            finally
            {
                TreeViewMobs.EndUpdate();
            }

            LoadListView();
        }

        private void OnClickRemove(object sender, EventArgs e)
        {
            TreeNode node = TreeViewMobs.SelectedNode;
            if (node?.Parent == null)
            {
                return;
            }

            if (node.Parent.Name != "Mobs" && node.Parent.Name != "Equipment")
            {
                node = node.Parent;
            }

            node.Remove();
            LoadListView();
        }

        private AnimationEditForm _animEditFormEntry;

        private void OnClickAnimationEdit(object sender, EventArgs e)
        {
            if (_animEditFormEntry?.IsDisposed == false)
            {
                return;
            }

            _animEditFormEntry = new AnimationEditForm();
            //animEditEntry.TopMost = true; // TODO: should it be topMost?
            _animEditFormEntry.Show();
        }

        private AnimationListNewEntriesForm _animNewEntryForm;

        private void OnClickFindNewEntries(object sender, EventArgs e)
        {
            if (_animNewEntryForm?.IsDisposed == false)
            {
                return;
            }

            _animNewEntryForm = new AnimationListNewEntriesForm(IsAlreadyDefined, AddGraphic, GetActionNames)
            {
                TopMost = true
            };
            _animNewEntryForm.Show();
        }

        private void RewriteXml(object sender, EventArgs e)
        {
            TreeViewMobs.BeginUpdate();
            try
            {
                TreeViewMobs.TreeViewNodeSorter = new GraphicSorter();
                TreeViewMobs.Sort();
            }
            finally
            {
                TreeViewMobs.EndUpdate();
            }

            string fileName = Path.Combine(Options.AppDataPath, "Animationlist.xml");

            XmlDocument dom = new XmlDocument();
            XmlDeclaration decl = dom.CreateXmlDeclaration("1.0", "utf-8", null);
            dom.AppendChild(decl);
            XmlElement sr = dom.CreateElement("Graphics");
            XmlComment comment = dom.CreateComment("Entries in Mob tab");
            sr.AppendChild(comment);
            comment = dom.CreateComment("Name=Displayed name");
            sr.AppendChild(comment);
            comment = dom.CreateComment("body=Graphic");
            sr.AppendChild(comment);
            comment = dom.CreateComment("type=0:Monster, 1:Sea, 2:Animal, 3:Human/Equipment");
            sr.AppendChild(comment);

            XmlElement elem;
            foreach (TreeNode node in TreeViewMobs.Nodes[0].Nodes)
            {
                elem = dom.CreateElement("Mob");
                elem.SetAttribute("name", node.Text);
                elem.SetAttribute("body", ((int[])node.Tag)[0].ToString());
                elem.SetAttribute("type", ((int[])node.Tag)[1].ToString());

                sr.AppendChild(elem);
            }

            foreach (TreeNode node in TreeViewMobs.Nodes[1].Nodes)
            {
                elem = dom.CreateElement("Equip");
                elem.SetAttribute("name", node.Text);
                elem.SetAttribute("body", ((int[])node.Tag)[0].ToString());
                elem.SetAttribute("type", ((int[])node.Tag)[1].ToString());
                sr.AppendChild(elem);
            }
            dom.AppendChild(sr);
            dom.Save(fileName);

            MessageBox.Show("XML saved", "Rewrite", MessageBoxButtons.OK, MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1);
        }

        private void Extract_Image_ClickBmp(object sender, EventArgs e)
        {
            ExtractImage(ImageFormat.Bmp);
        }

        private void Extract_Image_ClickTiff(object sender, EventArgs e)
        {
            ExtractImage(ImageFormat.Tiff);
        }

        private void Extract_Image_ClickJpg(object sender, EventArgs e)
        {
            ExtractImage(ImageFormat.Jpeg);
        }

        private void Extract_Image_ClickPng(object sender, EventArgs e)
        {
            ExtractImage(ImageFormat.Png);
        }

        private void ExtractImage(ImageFormat imageFormat)
        {
            string what = "Mob";
            if (_displayType == 1)
            {
                what = "Equipment";
            }

            string fileExtension = Utils.GetFileExtensionFor(imageFormat);
            string fileName = Path.Combine(Options.OutputPath, $"{what} {_currentSelect}.{fileExtension}");

            Bitmap sourceBitmap = MainPictureBox.CurrentFrame?.Bitmap;

            if (sourceBitmap == null)
            {
                return;
            }

            using (Bitmap newBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height))
            {
                using (Graphics newGraph = Graphics.FromImage(newBitmap))
                {
                    newGraph.FillRectangle(Brushes.White, 0, 0, newBitmap.Width, newBitmap.Height);
                    newGraph.DrawImage(sourceBitmap, new Point(0, 0));
                    newGraph.Save();
                }

                newBitmap.Save(fileName, imageFormat);
            }

            MessageBox.Show($"{what} saved to {fileName}", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1);
        }

        private void OnClickExtractAnimBmp(object sender, EventArgs e)
        {
            ExportAnimationFrames(ImageFormat.Bmp);
        }

        private void OnClickExtractAnimTiff(object sender, EventArgs e)
        {
            ExportAnimationFrames(ImageFormat.Tiff);
        }

        private void OnClickExtractAnimJpg(object sender, EventArgs e)
        {
            ExportAnimationFrames(ImageFormat.Jpeg);
        }

        private void OnClickExtractAnimPng(object sender, EventArgs e)
        {
            ExportAnimationFrames(ImageFormat.Png);
        }

        private void ExportAnimationFrames(ImageFormat imageFormat)
        {
            string what = "Mob";
            if (_displayType == 1)
            {
                what = "Equipment";
            }

            string fileExtension = Utils.GetFileExtensionFor(imageFormat);
            string fileName = Path.Combine(Options.OutputPath, $"{what} {_currentSelect}");

            for (int i = 0; i < MainPictureBox.Frames?.Count; ++i)
            {
                var frameBitmap = MainPictureBox.Frames[i].Bitmap;
                using (Bitmap newBitmap = new Bitmap(frameBitmap.Width, frameBitmap.Height))
                {
                    using (Graphics newGraph = Graphics.FromImage(newBitmap))
                    {
                        newGraph.FillRectangle(Brushes.White, 0, 0, newBitmap.Width, newBitmap.Height);
                        newGraph.DrawImage(frameBitmap, new Point(0, 0));
                        newGraph.Save();
                    }

                    newBitmap.Save($"{fileName}-{i}.{fileExtension}", imageFormat);
                }
            }

            MessageBox.Show($"{what} saved to '{fileName}-X.{fileExtension}'", "Saved", MessageBoxButtons.OK,
                MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
        }

        private void OnClickExportFrameBmp(object sender, EventArgs e)
        {
            ExportSingleFrame(ImageFormat.Bmp);
        }

        private void OnClickExportFrameTiff(object sender, EventArgs e)
        {
            ExportSingleFrame(ImageFormat.Tiff);
        }

        private void OnClickExportFrameJpg(object sender, EventArgs e)
        {
            ExportSingleFrame(ImageFormat.Jpeg);
        }

        private void OnClickExportFramePng(object sender, EventArgs e)
        {
            ExportSingleFrame(ImageFormat.Png);
        }

        private void ExportSingleFrame(ImageFormat imageFormat)
        {
            if (listView1.SelectedItems.Count < 1)
            {
                return;
            }

            string what = "Mob";
            if (_displayType == 1)
            {
                what = "Equipment";
            }

            string fileExtension = Utils.GetFileExtensionFor(imageFormat);
            string fileName = Path.Combine(Options.OutputPath, $"{what} {_currentSelect}");

            Bitmap bit = MainPictureBox.Frames[(int)listView1.SelectedItems[0].Tag].Bitmap;
            using (Bitmap newBitmap = new Bitmap(bit.Width, bit.Height))
            {
                using (Graphics newGraph = Graphics.FromImage(newBitmap))
                {
                    newGraph.FillRectangle(Brushes.White, 0, 0, newBitmap.Width, newBitmap.Height);
                    newGraph.DrawImage(bit, new Point(0, 0));
                    newGraph.Save();
                }

                newBitmap.Save($"{fileName}-{(int)listView1.SelectedItems[0].Tag}.{fileExtension}", imageFormat);
            }
        }

        private void ExportAnimatedGif(bool looping)
        {
            if (MainPictureBox.Frames == null)
            {
                return;
            }

            var outputFile = Path.Combine(Options.OutputPath, $"{(_displayType == 1 ? "Equipment" : "Mob")} {_currentSelect}.gif");
            MainPictureBox.Frames.ToGif(outputFile, looping: looping, delay: 150, showFrameBounds: MainPictureBox.ShowFrameBounds);
            MessageBox.Show($"InGame Anim saved to {outputFile}", "Saved", MessageBoxButtons.OK,
                MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
        }

        private void OnClickExtractAnimGifLooping(object sender, EventArgs e)
        {
            ExportAnimatedGif(true);
        }

        private void OnClickExtractAnimGifNoLooping(object sender, EventArgs e)
        {
            ExportAnimatedGif(false);
        }

        private void Frames_ListView_Click(object sender, EventArgs e)
        {
            var index = listView1.SelectedIndices.Count > 0 ? listView1.SelectedIndices[0] : 0;
            MainPictureBox.FrameIndex = index;
        }

        private void AnimateCheckBox_Click(object sender, EventArgs e)
        {
            MainPictureBox.Animate = !MainPictureBox.Animate;
            AnimateCheckBox.Checked = MainPictureBox.Animate;
        }

        private void ShowFrameBoundsCheckBox_Click(object sender, EventArgs e)
        {
            MainPictureBox.ShowFrameBounds = !MainPictureBox.ShowFrameBounds;
            ShowFrameBoundsCheckBox.Checked = MainPictureBox.ShowFrameBounds;
        }

        private async void OnPackFramesClick(object? sender, EventArgs e)
        {
            if (_currentSelect == 0)
            {
                MessageBox.Show("No graphic selected.", "Pack Frames", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // show pack options dialog
            using var optionsForm = new PackOptionsForm();
            if (optionsForm.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            var selectedDirections = optionsForm.SelectedDirections; // list<int>
            int maxWidth = optionsForm.MaxWidth;

             // Ask for output base name/location
             using (var dlg = new FolderBrowserDialog())
             {
                 dlg.Description = "Select folder to save packed sprite and JSON";
                 dlg.ShowNewFolderButton = true;
                 if (dlg.ShowDialog() != DialogResult.OK)
                 {
                     return;
                 }

                 string outDir = dlg.SelectedPath;

                 try
                 {
                     Cursor.Current = Cursors.WaitCursor;

                     // Collect frames for directions 0..4 (common editable directions)
                     var packedFrames = new List<PackedFrameEntry>();

                     int body = _currentSelect;
                     Animations.Translate(ref body);
                     int hue = 0; // do not preserve hue here

                     var images = new List<Bitmap>();

                     int currentX = 0, currentY = 0, rowHeight = 0, canvasWidth = 0, canvasHeight = 0;

                     foreach (int dir in selectedDirections)
                     {
                         int localHue = 0;
                         var frames = Animations.GetAnimation(_currentSelect, _currentSelectAction, dir, ref localHue, false, false);
                         if (frames == null || frames.Length == 0)
                         {
                             continue;
                         }

                         for (int fi = 0; fi < frames.Length; fi++)
                         {
                             var anim = frames[fi];
                             if (anim?.Bitmap == null)
                             {
                                 continue;
                             }

                             // determine size
                             int w = anim.Bitmap.Width;
                             int h = anim.Bitmap.Height;

                             if (currentX + w > maxWidth)
                             {
                                 currentY += rowHeight;
                                 currentX = 0;
                                 rowHeight = 0;
                             }

                             if (currentX == 0)
                                 rowHeight = h;

                             var entry = new PackedFrameEntry
                             {
                                 Direction = dir,
                                 Index = fi,
                                 Frame = new Rect { X = currentX, Y = currentY, W = w, H = h },
                                 Center = new PointStruct { X = anim.Center.X, Y = anim.Center.Y }
                             };

                             packedFrames.Add(entry);

                             // store image copy
                             images.Add(new Bitmap(anim.Bitmap));

                             currentX += w;
                             canvasWidth = Math.Max(canvasWidth, currentX);
                             canvasHeight = Math.Max(canvasHeight, currentY + rowHeight);
                         }
                     }

                     if (images.Count == 0)
                     {
                         MessageBox.Show("No frames found to pack.", "Pack Frames", MessageBoxButtons.OK, MessageBoxIcon.Information);
                         return;
                     }

                     // Create sprite sheet and paste images
                     using (var sprite = new Bitmap(Math.Max(1, canvasWidth), Math.Max(1, canvasHeight)))
                     using (var g = Graphics.FromImage(sprite))
                     {
                         g.Clear(Color.Transparent);

                         for (int i = 0; i < images.Count; i++)
                         {
                             var img = images[i];
                             var rect = packedFrames[i].Frame;
                             g.DrawImage(img, rect.X, rect.Y, rect.W, rect.H);
                             img.Dispose();
                         }

                         string baseName = $"anim_{_currentSelect}_{_currentSelectAction}";
                         string imageFile = Path.Combine(outDir, baseName + ".png");
                         sprite.Save(imageFile, ImageFormat.Png);

                         // prepare JSON
                         var outObj = new PackedOutput
                         {
                             Meta = new PackedMeta { Image = Path.GetFileName(imageFile), Size = new SizeStruct { W = sprite.Width, H = sprite.Height }, Format = "RGBA8888" },
                             Frames = packedFrames
                         };

                         string jsonFile = Path.Combine(outDir, baseName + ".json");
                         var jsOptions = new JsonSerializerOptions { WriteIndented = true };
                         string json = JsonSerializer.Serialize(outObj, jsOptions);
                         File.WriteAllText(jsonFile, json);

                         MessageBox.Show($"Saved sprite: {imageFile}\nSaved JSON: {jsonFile}", "Pack Frames", MessageBoxButtons.OK, MessageBoxIcon.Information);
                     }
                 }
                 catch (Exception ex)
                 {
                     MessageBox.Show($"Failed to pack frames: {ex.Message}", "Pack Frames", MessageBoxButtons.OK, MessageBoxIcon.Error);
                 }
                 finally
                 {
                     Cursor.Current = Cursors.Default;
                 }
             }
         }

        private void OnUnpackFramesClick(object? sender, EventArgs e)
        {
            if (_currentSelect == 0)
            {
                MessageBox.Show("No graphic selected.", "Unpack Frames", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
                ofd.Title = "Select packing JSON file";
                if (ofd.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                string jsonFile = ofd.FileName;
                try
                {
                    string json = File.ReadAllText(jsonFile);
                    var doc = JsonSerializer.Deserialize<PackedOutput>(json);
                    if (doc == null)
                    {
                        MessageBox.Show("Invalid JSON file.", "Unpack Frames", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    string spritePath = Path.Combine(Path.GetDirectoryName(jsonFile) ?? string.Empty, doc.Meta.Image);
                    if (!File.Exists(spritePath))
                    {
                        MessageBox.Show($"Sprite sheet not found: {spritePath}", "Unpack Frames", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    using (var sprite = new Bitmap(spritePath))
                    {
                        // determine body/fileType for import
                        int bodyTrans = _currentSelect;
                        Animations.Translate(ref bodyTrans);
                        int fileType = BodyConverter.Convert(ref bodyTrans);

                        // Group frames by direction
                        var groups = doc.Frames.GroupBy(f => f.Direction).ToDictionary(g => g.Key, g => g.OrderBy(f => f.Index).ToList());

                        foreach (var kv in groups)
                        {
                            int dir = kv.Key;
                            var framesList = kv.Value;

                            AnimIdx animIdx = AnimationEdit.GetAnimation(fileType, bodyTrans, _currentSelectAction, dir);
                            if (animIdx == null)
                            {
                                MessageBox.Show($"Failed to get AnimIdx for direction {dir}", "Unpack Frames", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                continue;
                            }

                            // Ask whether to overwrite frames for this direction
                            var res = MessageBox.Show($"Overwrite existing frames for direction {dir}? (Yes = overwrite, No = append)", "Unpack Frames", MessageBoxButtons.YesNoCancel);
                            if (res == DialogResult.Cancel)
                            {
                                return;
                            }

                            if (res == DialogResult.Yes)
                            {
                                animIdx.ClearFrames();
                            }

                            foreach (var frameEntry in framesList)
                            {
                                var r = frameEntry.Frame;
                                var crop = sprite.Clone(new Rectangle(r.X, r.Y, r.W, r.H), System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                                // convert to 16bpp used by AnimIdx methods - AnimIdx.AddFrame expects Bitmap in proper pixel format; conversion happens in FrameEdit
                                Bitmap bmp16 = new Bitmap(crop);
                                crop.Dispose();

                                animIdx.AddFrame(bmp16, frameEntry.Center.X, frameEntry.Center.Y);
                            }
                        }

                        Options.ChangedUltimaClass["Animations"] = true;

                        MessageBox.Show("Import finished. Remember to save animations via AnimationEdit.Save if needed.", "Unpack Frames", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to unpack/import frames: {ex.Message}", "Unpack Frames", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Helper types for JSON
        private class PackedOutput
        {
            [JsonPropertyName("meta")] public PackedMeta Meta { get; set; }
            [JsonPropertyName("frames")] public List<PackedFrameEntry> Frames { get; set; }
        }

        private class PackedMeta
        {
            [JsonPropertyName("image")] public string Image { get; set; }
            [JsonPropertyName("size")] public SizeStruct Size { get; set; }
            [JsonPropertyName("format")] public string Format { get; set; }
        }

        private class PackedFrameEntry
        {
            [JsonPropertyName("direction")] public int Direction { get; set; }
            [JsonPropertyName("index")] public int Index { get; set; }
            [JsonPropertyName("frame")] public Rect Frame { get; set; }
            [JsonPropertyName("center")] public PointStruct Center { get; set; }
        }

        private class Rect
        {
            [JsonPropertyName("x")] public int X { get; set; }
            [JsonPropertyName("y")] public int Y { get; set; }
            [JsonPropertyName("w")] public int W { get; set; }
            [JsonPropertyName("h")] public int H { get; set; }
        }

        private class PointStruct
        {
            [JsonPropertyName("x")] public int X { get; set; }
            [JsonPropertyName("y")] public int Y { get; set; }
        }

        private class SizeStruct
        {
            [JsonPropertyName("w")] public int W { get; set; }
            [JsonPropertyName("h")] public int H { get; set; }
        }

        private class AlphaSorter : IComparer
        {
            public int Compare(object x, object y)
            {
                TreeNode tx = x as TreeNode;
                TreeNode ty = y as TreeNode;
                if (tx.Parent == null) // don't change Mob and Equipment
                {
                    return (int)tx.Tag == -1 ? -1 : 1;
                }
                if (tx.Parent.Parent != null)
                {
                    return (int)tx.Tag - (int)ty.Tag;
                }

                return string.CompareOrdinal(tx.Text, ty.Text);
            }
        }

        public class GraphicSorter : IComparer
        {
            public int Compare(object x, object y)
            {
                TreeNode tx = x as TreeNode;
                TreeNode ty = y as TreeNode;
                if (tx.Parent == null)
                {
                    return (int)tx.Tag == -1 ? -1 : 1;
                }

                if (tx.Parent.Parent != null)
                {
                    return (int)tx.Tag - (int)ty.Tag;
                }

                int[] ix = (int[])tx.Tag;
                int[] iy = (int[])ty.Tag;

                if (ix[0] == iy[0])
                {
                    return 0;
                }

                if (ix[0] < iy[0])
                {
                    return -1;
                }

                return 1;
            }
        }

        // Pack options dialog (moved here to ensure compilation visibility)
        public class PackOptionsForm : Form
        {
            private CheckedListBox _directionsBox;
            private NumericUpDown _maxWidthUpDown;
            private Button _ok;
            private Button _cancel;

            public List<int> SelectedDirections { get; private set; } = new List<int> { 0, 1, 2, 3, 4 };
            public int MaxWidth { get; private set; } = 2048;

            public PackOptionsForm()
            {
                Text = "Pack Options";
                FormBorderStyle = FormBorderStyle.FixedDialog;
                MaximizeBox = false;
                MinimizeBox = false;
                StartPosition = FormStartPosition.CenterParent;
                // increased size to accommodate taller direction list and wider right side
                ClientSize = new Size(520, 360);
                Padding = new Padding(10);

                Label lbl = new Label { Text = "Directions:", Location = new Point(12, 12), AutoSize = true };
                Controls.Add(lbl);

                _directionsBox = new CheckedListBox
                {
                    Location = new Point(12, 32),
                    // make dropdown / list taller
                    Size = new Size(160, 260),
                    CheckOnClick = true,
                    ScrollAlwaysVisible = true,
                    IntegralHeight = false
                };
                for (int i = 0; i < 8; i++)
                {
                    _directionsBox.Items.Add(i.ToString(), i <= 4);
                }
                Controls.Add(_directionsBox);

                // move the max sprite width controls further to the right
                Label lbl2 = new Label { Text = "Max sprite width:", Location = new Point(190, 32), AutoSize = true };
                Controls.Add(lbl2);

                _maxWidthUpDown = new NumericUpDown
                {
                    Location = new Point(190, 56),
                    Size = new Size(260, 30),
                    Minimum = 256,
                    Maximum = 8192,
                    Increment = 64,
                    Value = 2048,
                    ThousandsSeparator = true
                };
                Controls.Add(_maxWidthUpDown);

                // Optional quick presets (moved right and made slightly taller)
                var presetsLabel = new Label { Text = "Presets:", Location = new Point(190, 95), AutoSize = true };
                Controls.Add(presetsLabel);
                var presetSmall = new Button { Text = "1024", Location = new Point(190, 115), Size = new Size(70, 34) };
                var presetMedium = new Button { Text = "2048", Location = new Point(266, 115), Size = new Size(70, 34) };
                var presetLarge = new Button { Text = "4096", Location = new Point(342, 115), Size = new Size(70, 34) };
                presetSmall.Click += (s, e) => _maxWidthUpDown.Value = 1024;
                presetMedium.Click += (s, e) => _maxWidthUpDown.Value = 2048;
                presetLarge.Click += (s, e) => _maxWidthUpDown.Value = 4096;
                Controls.Add(presetSmall);
                Controls.Add(presetMedium);
                Controls.Add(presetLarge);

                // make OK/Cancel taller and move to the right (anchored)
                _ok = new Button { Text = "OK", DialogResult = DialogResult.OK, Location = new Point(330, 300), Size = new Size(100, 40), Anchor = AnchorStyles.Bottom | AnchorStyles.Right };
                _ok.Click += Ok_Click;
                Controls.Add(_ok);

                _cancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Location = new Point(440, 300), Size = new Size(100, 40), Anchor = AnchorStyles.Bottom | AnchorStyles.Right };
                Controls.Add(_cancel);

                AcceptButton = _ok;
                CancelButton = _cancel;
            }

            private void Ok_Click(object sender, EventArgs e)
            {
                // Collect checked directions as integers
                SelectedDirections = _directionsBox.CheckedItems.Cast<object>().Select(o => int.Parse(o.ToString())).ToList();
                if (SelectedDirections.Count == 0)
                {
                    MessageBox.Show("Select at least one direction.", "Pack Options", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    DialogResult = DialogResult.None;
                    return;
                }

                MaxWidth = (int)_maxWidthUpDown.Value;
            }
        }
    }
}
