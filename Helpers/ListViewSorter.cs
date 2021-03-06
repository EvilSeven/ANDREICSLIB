using System;
using System.Collections;
using System.Windows.Forms;

namespace ANDREICSLIB.Helpers
{
    /// <summary>
    ///     example usage: https://github.com/andreigec/IP-Scanrar
    /// </summary>
    public class ListViewSorter : IComparer
    {
        /// <summary>
        /// The enabled
        /// </summary>
        public bool Enabled = true;
        /// <summary>
        /// Gets or sets the by column.
        /// </summary>
        /// <value>
        /// The by column.
        /// </value>
        public int ByColumn { get; set; }
        /// <summary>
        /// Gets or sets the last sort.
        /// </summary>
        /// <value>
        /// The last sort.
        /// </value>
        public int LastSort { get; set; }

        #region IComparer Members

        /// <summary>
        /// Compares the specified o1.
        /// </summary>
        /// <param name="o1">The o1.</param>
        /// <param name="o2">The o2.</param>
        /// <returns></returns>
        public int Compare(object o1, object o2)
        {
            if (Enabled == false)
                return 0;

            if (o1 == null || !(o1 is ListViewItem))
                return (0);
            if (o2 == null || !(o2 is ListViewItem))
                return (0);

            var lvi1 = (ListViewItem) o2;
            var str1 = lvi1.SubItems[ByColumn].Text;
            var lvi2 = (ListViewItem) o1;
            var str2 = lvi2.SubItems[ByColumn].Text;

            var r = CompareNatural(str1, str2);

            if (lvi1.ListView.Sorting == SortOrder.Descending)
                return r;

            return r == -1 ? 1 : -1;
        }

        #endregion

        /// <summary>
        /// call this from column click.
        /// </summary>
        /// <param name="lvs">an instance of listviewsorter</param>
        /// <param name="lv">The lv.</param>
        /// <param name="column">The column.</param>
        /// <param name="forceorder">if set to a value, will sort by that all the time, otherwise will sort as normal</param>
        public static void ColumnSort(ListViewSorter lvs, ListView lv, int column, SortOrder? forceorder = null)
        {
            try
            {
                lv.ListViewItemSorter = lvs;
                if (!(lv.ListViewItemSorter is ListViewSorter))
                    return;
                lvs = (ListViewSorter) lv.ListViewItemSorter;
            }
            catch (Exception)
            {
                return;
            }

            if (forceorder != null)
            {
                lv.Sorting = (SortOrder) forceorder;
            }
            else
            {
                //invert sorting
                lv.Sorting = lv.Sorting == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
            }
            lvs.ByColumn = column;

            lv.Sort();
        }

        /// <summary>
        /// Compares the natural.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        private static int CompareNatural(string x, string y)
        {
            if (x == null && y == null) return 0;
            if (x == null) return -1;
            if (y == null) return 1;

            int lx = x.Length, ly = y.Length;

            for (int mx = 0, my = 0; mx < lx && my < ly; mx++, my++)
            {
                if (char.IsDigit(x[mx]) && char.IsDigit(y[my]))
                {
                    long vx = 0, vy = 0;

                    for (; mx < lx && char.IsDigit(x[mx]); mx++)
                        vx = vx*10 + x[mx] - '0';

                    for (; my < ly && char.IsDigit(y[my]); my++)
                        vy = vy*10 + y[my] - '0';

                    if (vx != vy)
                        return vx > vy ? 1 : -1;
                }

                if (mx < lx && my < ly && x[mx] != y[my])
                    return x[mx] > y[my] ? 1 : -1;
            }

            return lx - ly;
        }
    }
}