﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ANDREICSLIB
{
    public class ColorListBox:ListBox 
    {
        public List<Color> colours;
        [Description("The colours for each item of text")]
        public List<Color> Colours
        {
            get { return colours; }
            set
            {
                colours = value;
            }
        }

        public ColorListBox()
        {
            this.DrawItem += DrawItemHandler;

        }

        private void DrawItemHandler(object sender, DrawItemEventArgs e)
		{
			e.DrawBackground();
			e.DrawFocusRectangle();
            SolidBrush sb;
            if (colours.Count < e.Index)
                sb = new SolidBrush(Color.Blue);
            else
                sb = new SolidBrush(colours[e.Index]);

            Font f = new Font(FontFamily.GenericSansSerif,
                              8, FontStyle.Bold);
			e.Graphics.DrawString(Items[e.Index].ToString() ,f,sb,e.Bounds);
		}
    }
}
