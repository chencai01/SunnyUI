﻿/******************************************************************************
 * SunnyUI 开源控件库、工具类库、扩展类库、多页面开发框架。
 * CopyRight (C) 2012-2020 ShenYongHua(沈永华).
 * QQ群：56829229 QQ：17612584 EMail：SunnyUI@qq.com
 *
 * Blog:   https://www.cnblogs.com/yhuse
 * Gitee:  https://gitee.com/yhuse/SunnyUI
 * GitHub: https://github.com/yhuse/SunnyUI
 *
 * SunnyUI.dll can be used for free under the GPL-3.0 license.
 * If you use this code, please keep this note.
 * 如果您使用此代码，请保留此说明。
 ******************************************************************************
 * 文件名称: UIImageListBox.cs
 * 文件说明: 图片列表框
 * 当前版本: V2.2
 * 创建日期: 2020-01-01
 *
 * 2020-01-01: V2.2.0 增加文件说明
 * 2020-04-25: V2.2.4 更新主题配置类
 * 2020-05-21: V2.2.5 增加鼠标滑过高亮
******************************************************************************/

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace Sunny.UI
{
    [DefaultEvent("ItemClick")]
    public sealed partial class UIImageListBox : UIPanel
    {
        private readonly ImageListBox listbox = new ImageListBox();
        private readonly UIScrollBar bar = new UIScrollBar();

        public UIImageListBox()
        {
            InitializeComponent();
            ShowText = false;

            Padding = new Padding(2);

            bar.ValueChanged += Bar_ValueChanged;
            bar.Width = SystemInformation.VerticalScrollBarWidth + 2;
            bar.Parent = this;
            bar.Dock = DockStyle.None;
            bar.Style = UIStyle.Custom;
            bar.Visible = false;

            listbox.Parent = this;
            listbox.Dock = DockStyle.Fill;
            listbox.Show();
            listbox.Bar = bar;

            listbox.SelectedIndexChanged += Listbox_SelectedIndexChanged;
            listbox.SelectedValueChanged += Listbox_SelectedValueChanged;
            listbox.Click += Listbox_Click;
            listbox.DoubleClick += Listbox_DoubleClick;
            listbox.BeforeDrawItem += Listbox_BeforeDrawItem;
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            bar.Top = 2;
            bar.Height = Height - 4;
            bar.Left = Width - bar.Width - 2;
        }

        private void Listbox_BeforeDrawItem(object sender, ListBox.ObjectCollection items, DrawItemEventArgs e)
        {
            if (Items.Count != LastCount)
            {
                listbox.SetScrollInfo();
                LastCount = Items.Count;
                ItemsCountChange?.Invoke(this, null);
            }
        }

        private void Listbox_DoubleClick(object sender, EventArgs e)
        {
            if (SelectedItem != null)
                ItemDoubleClick?.Invoke(sender, e);
        }

        private void Listbox_Click(object sender, EventArgs e)
        {
            if (SelectedItem != null)
                ItemClick?.Invoke(sender, e);
        }

        [Browsable(false)]
        public int Count => Items.Count;

        public event EventHandler ItemClick;

        public event EventHandler ItemDoubleClick;

        public event EventHandler ItemsCountChange;

        public event EventHandler SelectedIndexChanged;

        public event EventHandler SelectedValueChanged;

        private void Listbox_SelectedValueChanged(object sender, EventArgs e)
        {
            SelectedValueChanged?.Invoke(sender, e);
            Text = listbox.SelectedItem?.ToString();
        }

        private void Listbox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedIndexChanged?.Invoke(sender, e);
        }

        [DefaultValue(100)]
        public int ItemHeight
        {
            get => listbox.ItemHeight;
            set => listbox.ItemHeight = value;
        }

        [DefaultValue(4)]
        public int ImageInterval
        {
            get => listbox.ImageInterval;
            set => listbox.ImageInterval = value;
        }

        [DefaultValue(true)]
        public bool ShowDescription
        {
            get => listbox.ShowDescription;
            set => listbox.ShowDescription = value;
        }

        public override void SetStyleColor(UIBaseStyle uiColor)
        {
            base.SetStyleColor(uiColor);
            if (uiColor.IsCustom()) return;

            if (bar != null)
            {
                bar.ForeColor = uiColor.PrimaryColor;
                bar.HoverColor = uiColor.ButtonFillHoverColor;
                bar.PressColor = uiColor.ButtonFillPressColor;
                bar.FillColor = Color.White;
            }

            hoverColor = uiColor.TreeViewHoverColor;
            if (listbox != null)
            {
                listbox.HoverColor = hoverColor;
                listbox.SetStyleColor(uiColor);
            }
        }

        private int LastCount;

        private int lastBarValue = -1;
        private void Bar_ValueChanged(object sender, EventArgs e)
        {
            if (listbox != null)
            {
                if (bar.Value != lastBarValue)
                {
                    ScrollBarInfo.SetScrollValue(listbox.Handle, bar.Value);
                    lastBarValue = bar.Value;
                }
            }
        }

        protected override void OnRadiusChanged(int value)
        {
            base.OnRadiusChanged(value);
            Padding = new Padding(Math.Max(2, value / 2));
        }

        protected override void OnPaintFill(Graphics g, GraphicsPath path)
        {
            g.Clear(Color.White);
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Localizable(true)]
        [Editor("System.Windows.Forms.Design.ListControlStringCollectionEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [MergableProperty(false)]
        [Browsable(false)]
        public ListBox.ObjectCollection Items => listbox.Items;

        /// <summary>
        /// 增加图片
        /// </summary>
        /// <param name="record">图片对象</param>
        public void AddImage(ImageListItem record)
        {
            Items.Add(record);
        }

        /// <summary>
        /// 增加图片
        /// </summary>
        /// <param name="imagePath">图片路径</param>
        /// <param name="description">图片描述</param>
        public void AddImage(string imagePath, string description = "")
        {
            AddImage(new ImageListItem(imagePath, description));
        }

        public void SelectedFirst()
        {
            listbox.SelectedFirst();
        }

        [DefaultValue(typeof(Color), "80, 160, 255")]
        public Color ItemSelectBackColor
        {
            get => listbox.ItemSelectBackColor;
            set => listbox.ItemSelectBackColor = value;
        }

        [DefaultValue(typeof(Color), "White")]
        public Color ItemSelectForeColor
        {
            get => listbox.ItemSelectForeColor;
            set => listbox.ItemSelectForeColor = value;
        }

        [Browsable(false)]
        [DefaultValue(-1)]
        public int SelectedIndex
        {
            get => listbox.SelectedIndex;
            set => listbox.SelectedIndex = value;
        }

        [Browsable(false)]
        [DefaultValue(null)]
        public ImageListItem SelectedItem
        {
            get => listbox.SelectedItem as ImageListItem;
            set => listbox.SelectedItem = value;
        }

        [Browsable(false)]
        [DefaultValue(null)]
        public object SelectedValue
        {
            get => listbox.SelectedValue;
            set => listbox.SelectedValue = value;
        }

        private Color hoverColor = Color.FromArgb(155, 200, 255);

        [DefaultValue(typeof(Color), "155, 200, 255")]
        public Color HoverColor
        {
            get => hoverColor;
            set
            {
                hoverColor = value;
                listbox.HoverColor = hoverColor;
                _style = UIStyle.Custom;
            }
        }

        private sealed class ImageListBox : ListBox, IStyleInterface
        {
            private UIScrollBar bar;

            [DefaultValue(null)]
            public string TagString { get; set; }

            public UIScrollBar Bar
            {
                get => bar;
                set
                {
                    bar = value;
                    SetScrollInfo();
                }
            }

            public ImageListBox()
            {
                SetStyle(ControlStyles.AllPaintingInWmPaint, true);
                SetStyle(ControlStyles.DoubleBuffer, true);
                UpdateStyles();

                BorderStyle = BorderStyle.None;
                ForeColor = UIFontColor.Primary;
                IntegralHeight = false;
                ItemHeight = 100;
                DrawMode = DrawMode.OwnerDrawFixed;
                Version = UIGlobal.Version;
                SetScrollInfo();
            }

            protected override void OnSizeChanged(EventArgs e)
            {
                SetScrollInfo();
            }

            public void SetScrollInfo()
            {
                if (Bar == null)
                {
                    return;
                }

                var si = ScrollBarInfo.GetInfo(Handle);
                if (si.ScrollMax > 0)
                {
                    Bar.Maximum = si.ScrollMax;
                    Bar.Visible = si.ScrollMax > 0 && si.nMax > 0 && si.nPage > 0;
                    Bar.Value = si.nPos;
                }
                else
                {
                    Bar.Visible = false;
                }
            }

            protected override void OnMeasureItem(MeasureItemEventArgs e)
            {
                e.ItemHeight = e.ItemHeight + ItemHeight;
            }

            [DefaultValue(false)]
            public bool StyleCustomMode { get; set; }

            public string Version { get; }

            private UIStyle _style = UIStyle.Blue;
            private Color _itemSelectBackColor = UIColor.Blue;
            private Color _itemSelectForeColor = Color.White;
            private int imageInterval = 4;
            private bool showDescription = true;

            [Browsable(false)]
            public int Count => Items.Count;

            public void SelectedFirst()
            {
                if (Count > 0 && SelectedIndex < 0)
                {
                    SelectedIndex = 0;
                }
            }

            protected override void OnMouseWheel(MouseEventArgs e)
            {
                base.OnMouseWheel(e);

                if (Bar.Visible)
                {
                    var si = ScrollBarInfo.GetInfo(Handle);
                    if (e.Delta > 10)
                    {
                        if (si.nPos > 0)
                        {
                            ScrollBarInfo.ScrollUp(Handle);
                        }
                    }
                    else if (e.Delta < -10)
                    {
                        if (si.nPos < si.ScrollMax)
                        {
                            ScrollBarInfo.ScrollDown(Handle);
                        }
                    }
                }

                SetScrollInfo();
            }

            [DefaultValue(4)]
            public int ImageInterval
            {
                get => imageInterval;
                set
                {
                    if (imageInterval != value)
                    {
                        imageInterval = value;
                        Invalidate();
                    }
                }
            }

            [DefaultValue(true)]
            public bool ShowDescription
            {
                get => showDescription;
                set
                {
                    if (showDescription != value)
                    {
                        showDescription = value;
                        Invalidate();
                    }
                }
            }

            [DefaultValue(UIStyle.Blue)]
            public UIStyle Style
            {
                get => _style;
                set => SetStyle(value);
            }

            public void SetStyle(UIStyle style)
            {
                SetStyleColor(UIStyles.GetStyleColor(style));
                _style = style;
            }

            public void SetStyleColor(UIBaseStyle uiColor)
            {
                if (uiColor.IsCustom()) return;

                ItemSelectBackColor = uiColor.ListItemSelectBackColor;
                ItemSelectForeColor = uiColor.ListItemSelectForeColor;

                Invalidate();
            }

            [Category("Appearance"), Description("The border color used to paint the control.")]
            public Color ItemSelectBackColor
            {
                get => _itemSelectBackColor;
                set
                {
                    if (_itemSelectBackColor != value)
                    {
                        _itemSelectBackColor = value;
                        _style = UIStyle.Custom;
                        if (DesignMode)
                            Invalidate();
                    }
                }
            }

            [Category("Appearance"), Description("The border color used to paint the control.")]
            public Color ItemSelectForeColor
            {
                get => _itemSelectForeColor;
                set
                {
                    if (_itemSelectForeColor != value)
                    {
                        _itemSelectForeColor = value;
                        _style = UIStyle.Custom;
                        if (DesignMode)
                            Invalidate();
                    }
                }
            }

            /// <summary>
            /// 增加图片
            /// </summary>
            /// <param name="record">图片对象</param>
            public void AddImage(ImageListItem record)
            {
                Items.Add(record);
            }

            /// <summary>
            /// 增加图片
            /// </summary>
            /// <param name="imagePath">图片路径</param>
            /// <param name="description">图片描述</param>
            public void AddImage(string imagePath, string description = "")
            {
                AddImage(new ImageListItem(imagePath, description));
            }

            public delegate void OnBeforeDrawItem(object sender, ObjectCollection items, DrawItemEventArgs e);

            public event OnBeforeDrawItem BeforeDrawItem;

            protected override void OnDrawItem(DrawItemEventArgs e)
            {
                base.OnDrawItem(e);
                BeforeDrawItem?.Invoke(this, Items, e);
                if (Items.Count == 0)
                {
                    return;
                }

                bool otherState = e.State == DrawItemState.Grayed || e.State == DrawItemState.HotLight;
                if (!otherState)
                {
                    e.DrawBackground();
                }

                if (e.Index < 0 || e.Index >= Items.Count)
                {
                    return;
                }

                bool isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
                Color backColor = isSelected ? ItemSelectBackColor : BackColor;
                Color foreColor = isSelected ? ItemSelectForeColor : ForeColor;
                Rectangle rect = new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width - 1, e.Bounds.Height - 1);

                if (!otherState)
                {
                    e.Graphics.FillRectangle(BackColor, e.Bounds);
                    e.Graphics.FillRoundRectangle(backColor, rect, 5);
                }
                else
                {
                    if (e.State == DrawItemState.Grayed)
                    {
                        backColor = BackColor;
                        foreColor = ForeColor;
                    }

                    if (e.State == DrawItemState.HotLight)
                    {
                        backColor = HoverColor;
                        foreColor = ForeColor;
                    }

                    e.Graphics.FillRectangle(BackColor, e.Bounds);
                    e.Graphics.FillRoundRectangle(backColor, rect, 5);
                }

                Graphics g = e.Graphics;
                Matrix oldTransform = g.Transform;
                Matrix newTransform = oldTransform.Clone();
                newTransform.Translate(e.Bounds.X, e.Bounds.Y);
                g.Transform = newTransform;
                ImageListItem item = (ImageListItem)Items[e.Index];
                SizeF sf = g.MeasureString("ImageListBox", Font);
                int thumbnailSize = ShowDescription ? ((int)(ItemHeight - ImageInterval - sf.Height)) : (ItemHeight - ImageInterval * 2);

                if (File.Exists(item.ImagePath))
                {
                    Image image = new Bitmap(item.ImagePath);

                    if (image.Width <= thumbnailSize && image.Height <= thumbnailSize)
                    {
                        g.DrawImage(image, new Rectangle(ImageInterval, ImageInterval, image.Width, image.Height));
                    }
                    else
                    {
                        float scale = thumbnailSize * 1.0f / image.Height;
                        g.DrawImage(image, new Rectangle(ImageInterval, ImageInterval, (int)(image.Width * scale), (int)(image.Height * scale)));
                    }

                    image.Dispose();
                }

                if (ShowDescription && !string.IsNullOrEmpty(item.Description))
                {
                    g.DrawString(item.Description, e.Font, foreColor, new Point(ImageInterval, thumbnailSize + ImageInterval));
                }

                g.Transform = oldTransform;
            }

            private Color hoverColor = Color.FromArgb(155, 200, 255);

            [DefaultValue(typeof(Color), "155, 200, 255")]
            public Color HoverColor
            {
                get => hoverColor;
                set
                {
                    hoverColor = value;
                    _style = UIStyle.Custom;
                }
            }

            private int lastIndex = -1;
            private int mouseIndex = -1;

            [Browsable(false)]
            public int MouseIndex
            {
                get => mouseIndex;
                set
                {
                    if (mouseIndex != value)
                    {
                        if (lastIndex >= 0 && lastIndex != SelectedIndex)
                        {
                            OnDrawItem(new DrawItemEventArgs(this.CreateGraphics(), Font, GetItemRectangle(lastIndex), lastIndex, DrawItemState.Grayed));
                        }

                        mouseIndex = value;
                        if (mouseIndex >= 0 && mouseIndex != SelectedIndex)
                        {
                            OnDrawItem(new DrawItemEventArgs(this.CreateGraphics(), Font, GetItemRectangle(value), value, DrawItemState.HotLight));
                        }

                        lastIndex = mouseIndex;
                    }
                }
            }

            protected override void OnMouseMove(MouseEventArgs e)
            {
                base.OnMouseMove(e);
                MouseIndex = IndexFromPoint(e.Location);
            }

            protected override void OnMouseLeave(EventArgs e)
            {
                base.OnMouseLeave(e);
                MouseIndex = -1;
            }
        }

        public class ImageListItem
        {
            public string ImagePath { get; set; }

            public string Description { get; set; }

            public ImageListItem(string imagePath, string description = "")
            {
                ImagePath = imagePath;
                Description = description;
            }

            public override string ToString()
            {
                return ImagePath;
            }
        }
    }
}