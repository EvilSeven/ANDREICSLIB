﻿using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace ANDREICSLIB.Licensing
{
    public partial class aboutScreen : Form
    {
        public aboutScreen()
        {
            InitializeComponent();
        }

        private void aboutScreen_Load(object sender, EventArgs e)
        {
            closebutton.Select();
        }

        private void closebutton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void otherapptext_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            Process.Start(e.LinkText);
        }

        private void aboutScreen_FormClosing(object sender, FormClosingEventArgs e)
        {
            Licensing.ShowingAbout = false;
        }
    }
}