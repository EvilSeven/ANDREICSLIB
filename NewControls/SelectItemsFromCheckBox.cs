﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ANDREICSLIB.NewControls
{
    /// <summary>
    /// example usage: https://github.com/andreigec/BeadSprite-Pro
    /// </summary>
    public partial class SelectItemsFromCheckBox : Form
    {
        private int mustSelectCount;
        private List<string> returnvalues = new List<string>();

        private SelectItemsFromCheckBox()
        {
            InitializeComponent();
        }

        private void SelectItemsFromCheckBox_Load(object sender, EventArgs e)
        {
        }

        public static List<string> ShowDialog(string labelText, string title, List<SelectItemFromListBox.SelectItem> listBoxItems,
                                              int mustSelectCountIN = -1)
        {
            var s = new SelectItemsFromCheckBox();
            s.mustSelectCount = mustSelectCountIN;

            s.Text = title;
            s.label1.Text = labelText;

            s.checkedListBox1.Items.Clear();
            int a = 0;
            foreach (SelectItemFromListBox.SelectItem v in listBoxItems)
            {
                s.checkedListBox1.Items.Add(v.Text);
                s.checkedListBox1.SetSelected(a, v.preselected);
                a++;
            }

            s.ShowDialog();
            return s.returnvalues;
        }

        private void okbutton_Click(object sender, EventArgs e)
        {
            if (checkedListBox1.SelectedItems.Count < mustSelectCount && mustSelectCount != -1)
            {
                MessageBox.Show("You must select at least " + mustSelectCount.ToString() + " items");
                return;
            }
            foreach (object v in checkedListBox1.SelectedItems)
            {
                returnvalues.Add(v.ToString());
            }
            Close();
        }

        private void cancelbutton_Click(object sender, EventArgs e)
        {
            returnvalues = null;
            Close();
        }
    }
}