using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ANDREICSLIB.Helpers;

//andrei gec

namespace ANDREICSLIB.ClassExtras
{
    /// <summary>
    ///     example usage: https://github.com/andreigec/Consultant-Plus
    /// </summary>
    public static class ListViewExtras
    {
        /// <summary>
        /// Subs the item collection to range.
        /// </summary>
        /// <param name="lvsic">The lvsic.</param>
        /// <returns></returns>
        private static ListViewItem.ListViewSubItem[] SubItemCollectionToRange(
            ListViewItem.ListViewSubItemCollection lvsic)
        {
            var result = new ListViewItem.ListViewSubItem[lvsic.Count];

            var count = -1;
            foreach (ListViewItem.ListViewSubItem LVSI in lvsic)
            {
                count++;
                if (count == 0)
                    continue;

                result[count] = LVSI;
            }

            return result;
        }

        /// <summary>
        /// swap two rows given by their index
        /// </summary>
        /// <param name="lv">The lv.</param>
        /// <param name="index1">First Index to Swap</param>
        /// <param name="index2">Second Index to Swap</param>
        public static void SwapIndicies(ListView lv, int index1, int index2)
        {
            lv.Hide();
            if (index1 < 0 || index2 < 0 || index1 >= lv.Items.Count || index2 >= lv.Items.Count)
                return;

            //make clones
            var LVI1 = (ListViewItem) lv.Items[index1].Clone();
            var LVI2 = (ListViewItem) lv.Items[index2].Clone();

            //swap the sub items
            lv.Items[index1].SubItems.Clear();
            lv.Items[index1].SubItems.AddRange(SubItemCollectionToRange(LVI2.SubItems));

            lv.Items[index2].SubItems.Clear();
            lv.Items[index2].SubItems.AddRange(SubItemCollectionToRange(LVI1.SubItems));

            //swap the name and text
            lv.Items[index1].Text = LVI1.Text;
            lv.Items[index2].Text = LVI2.Text;

            lv.Items[index1].Name = LVI1.Text;
            lv.Items[index2].Name = LVI2.Text;
            lv.Show();
        }

        /// <summary>
        /// Automatics the resize ListView column.
        /// </summary>
        /// <param name="lv">The lv.</param>
        /// <param name="ch">The ch.</param>
        private static void AutoResizeListViewColumn(ListView lv, ColumnHeader ch)
        {
            var headerWidth = ch.Text.Length;

            var changeHeader = true;

            foreach (ListViewItem LVI in lv.Items)
            {
                EnsureSubItemCount(lv, LVI);
                var temp = ch.Index == 0 ? LVI.Text.Length : LVI.SubItems[ch.Index].Text.Length;

                if (temp > headerWidth)
                {
                    changeHeader = false;
                    break;
                }
            }

            lv.AutoResizeColumn(ch.Index,
                changeHeader
                    ? ColumnHeaderAutoResizeStyle.HeaderSize
                    : ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        /// <summary>
        /// resize all columns to best fit the header and the contents
        /// </summary>
        /// <param name="lv">The lv.</param>
        public static void AutoResize(ListView lv)
        {
            lv.Hide();
            foreach (ColumnHeader CH in lv.Columns)
            {
                AutoResizeListViewColumn(lv, CH);
            }

            lv.Show();
        }

        /// <summary>
        /// Initialise the columns to be those in a list of strings
        /// </summary>
        /// <param name="lv">The lv.</param>
        /// <param name="columnList">The list of strings to be made columns of</param>
        public static void InitColumnHeaders(ListView lv, List<string> columnList)
        {
            if (columnList == null)
                return;

            lv.Columns.Clear();
            foreach (var s in columnList)
            {
                lv.Columns.Add(s, s);
            }
        }

        /// <summary>
        /// init the column headers from a class' public variables
        /// </summary>
        /// <param name="lv">The lv.</param>
        /// <param name="ty">The ty.</param>
        public static void InitColumnHeaders(ListView lv, Type ty)
        {
            //get session vars
            var sv = Reflection.GetFieldNames(ty);

            InitColumnHeaders(lv, sv);
        }

        /// <summary>
        /// Select all the items in the list view
        /// </summary>
        /// <param name="lv">The lv.</param>
        public static void SelectAllItems(ListView lv)
        {
            lv.SelectedItems.Clear();
            foreach (ListViewItem LVI in lv.Items)
            {
                LVI.Selected = true;
            }
        }

        /// <summary>
        /// get the index of a column name
        /// </summary>
        /// <param name="lv">The lv.</param>
        /// <param name="columnName">the column name</param>
        /// <param name="LVIField">The lvi field.</param>
        /// <returns>
        /// the index of the column, -1 if not found
        /// </returns>
        public static int GetColumnNumber(ListView lv, string columnName, string LVIField = "Text")
        {
            var count = -1;
            foreach (ColumnHeader CH in lv.Columns)
            {
                count++;

                var val = Reflection.GetFieldValue(CH, LVIField);
                if (val == null)
                    continue;

                if (val.ToString().Equals(columnName))
                    return count;
            }
            return -1;
        }

        /// <summary>
        /// add a line item comparing the header text with the object field names
        /// </summary>
        /// <param name="lv">The lv.</param>
        /// <param name="classInstance">The class instance.</param>
        /// <param name="overwrite">The overwrite.</param>
        /// <returns></returns>
        public static ListViewItem CopyClassToListView(ListView lv, object classInstance, ListViewItem overwrite = null)
        {
            var t = classInstance.GetType();
            var pi = t.GetProperties();
            var fi = t.GetFields();

            ListViewItem lvi = null;
            if (overwrite != null)
                lvi = overwrite;
            else
                lvi = new ListViewItem();

            var itemlist = new List<Tuple<string, object>>();

            //get all the properties and fields for the class
            foreach (var prop in pi)
            {
                //see if a column matches
                var o = prop.GetValue(classInstance, null);
                if (o != null)
                    itemlist.Add(new Tuple<string, object>(prop.Name, o));
            }

            foreach (var field in fi)
            {
                var o = field.GetValue(classInstance);
                if (o != null)
                    itemlist.Add(new Tuple<string, object>(field.Name, o));
            }

            var fn = Reflection.GetFieldName(() => lvi.Name);
            //match with the lvi columns
            foreach (var i in itemlist)
            {
                SetColumn(lv, lvi, i.Item1, i.Item2.ToString(), fn);
            }

            if (overwrite == null)
                lv.Items.Add(lvi);

            return lvi;
        }

        /// <summary>
        /// Sets the column.
        /// </summary>
        /// <param name="lv">The lv.</param>
        /// <param name="lvi">The lvi.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="columnValue">The column value.</param>
        /// <param name="LVIField">The lvi field.</param>
        /// <returns></returns>
        public static bool SetColumn(ListView lv, ListViewItem lvi, string columnName, string columnValue,
            string LVIField = "Text")
        {
            var col = GetColumnNumber(lv, columnName, LVIField);

            if (col == -1)
                return false;

            EnsureSubItemCount(lv, lvi);

            lvi.SubItems[col].Text = columnValue;
            lvi.SubItems[col].Name = lv.Columns[col].Name;
            return true;
        }

        /// <summary>
        /// Gets the column.
        /// </summary>
        /// <param name="lv">The lv.</param>
        /// <param name="lvi">The lvi.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="LVIField">The lvi field.</param>
        /// <returns></returns>
        public static string GetColumn(ListView lv, ListViewItem lvi, string columnName, string LVIField = "Text")
        {
            var col = GetColumnNumber(lv, columnName);

            if (col == -1)
                return null;

            if (lvi.SubItems.Count < col)
                return null;

            var si = lvi.SubItems[col];
            var val = Reflection.GetFieldValue(si, LVIField);
            return val.ToString();
        }

        /// <summary>
        /// make the LVI subitem count match the column count
        /// </summary>
        /// <param name="lv">The lv.</param>
        /// <param name="lvi">The lvi.</param>
        public static void EnsureSubItemCount(ListView lv, ListViewItem lvi)
        {
            while (lvi.SubItems.Count > lv.Columns.Count)
                lvi.SubItems.Remove(lvi.SubItems[lvi.SubItems.Count - 1]);

            while (lvi.SubItems.Count < lv.Columns.Count)
                lvi.SubItems.Add("").Name = lv.Columns[lvi.SubItems.Count - 1].Name;
            ;
        }

        /// <summary>
        /// Gets the objects from ListView items.
        /// </summary>
        /// <param name="lv">The lv.</param>
        /// <param name="ty">The ty.</param>
        /// <returns></returns>
        public static List<object> GetObjectsFromListViewItems(ListView lv, Type ty)
        {
            var ret = new List<object>();
            foreach (ListViewItem lvi in lv.Items)
            {
                ret.Add(GetObjectFromListViewItem(lv, lvi, ty));
            }
            return ret;
        }

        /// <summary>
        /// Gets the object from ListView item.
        /// </summary>
        /// <param name="lv">The lv.</param>
        /// <param name="LVI">The lvi.</param>
        /// <param name="ty">The ty.</param>
        /// <returns></returns>
        public static object GetObjectFromListViewItem(ListView lv, ListViewItem LVI, Type ty)
        {
            var ls = GetListViewItemRowValuesAndColumnName(lv, LVI);
            var o = Reflection.DeserialiseObject(ty, ls);
            return o;
        }

        /// <summary>
        /// returns a tuple of column header name, and the row value for this LVI
        /// </summary>
        /// <param name="lv">The lv.</param>
        /// <param name="LVI">The lvi.</param>
        /// <param name="LVIField">The lvi field.</param>
        /// <returns>
        /// column header name,row value
        /// </returns>
        public static List<Tuple<string, string>> GetListViewItemRowValuesAndColumnName(ListView lv, ListViewItem LVI,
            string LVIField = "Text")
        {
            var ret = new List<Tuple<string, string>>();
            for (var a = 0; a < LVI.SubItems.Count; a++)
            {
                var val = Reflection.GetFieldValue(LVI.SubItems[a], LVIField);
                if (val == null)
                    continue;

                ret.Add(new Tuple<string, string>(lv.Columns[a].Text, val.ToString()));
            }
            return ret;
        }
    }
}