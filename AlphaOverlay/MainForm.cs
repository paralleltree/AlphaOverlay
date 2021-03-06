﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;

namespace AlphaOverlay
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            this.TopMost = true;
            this.AllowDrop = true;
            this.DragEnter += OnDragEnter;
            this.DragDrop += OnDragDrop;
        }

        private void OnDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.All;
        }

        private void OnDragDrop(object sender, DragEventArgs e)
        {
            try
            {
                string path = ((string[])e.Data.GetData(DataFormats.FileDrop)).First();
                using (var src = new Bitmap(path))
                {
                    mainPictureBox.Image = new Bitmap(src);
                }
            }
            catch
            {
            }
        }


        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        public static extern int GetWindowLong(IntPtr hWnd, GWL nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        public static extern int SetWindowLong(IntPtr hWnd, GWL nIndex, int dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetLayeredWindowAttributes")]
        public static extern bool SetLayeredWindowAttributes(IntPtr hWnd, int crKey, byte alpha, LWA dwFlags);

        public enum GWL
        {
            ExStyle = -20
        }

        public enum WS_EX
        {
            Transparent = 0x20,
            Layered = 0x80000
        }

        public enum LWA
        {
            ColorKey = 0x1,
            Alpha = 0x2
        }

        int initWl = 0;
        bool isTransparent = false;
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            initWl = GetWindowLong(this.Handle, GWL.ExStyle);
            SetLayerStyle(isTransparent);
            SetLayeredWindowAttributes(this.Handle, 0, 128, LWA.Alpha);
        }

        private void SetLayerStyle(bool isTransparent)
        {
            SetWindowLong(this.Handle, GWL.ExStyle, initWl | 0x80000 | (isTransparent ? 0x20 : 0));
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            isTransparent = !isTransparent;
            SetLayerStyle(isTransparent);
        }
    }
}
