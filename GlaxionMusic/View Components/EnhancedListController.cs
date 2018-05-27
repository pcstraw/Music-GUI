using Glaxion.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Glaxion.Music
{
    public class EnhancedListController : EnhancedListView
    {
        public VListView manager;
        
        public IEnumerable<int> GetSelected()
        {
            foreach (int i in SelectedIndices)
                yield return i;
        }
        //move to enhance viewbox?
        private ListViewItem.ListViewSubItem MakeSubItem(ListViewItem i, object rowObject, VColumn column)
        {
            object cellValue = column.GetValue(rowObject);
            ListViewItem.ListViewSubItem subItem = new ListViewItem.ListViewSubItem
                (i, column.ValueToString(cellValue), ForeColor, BackColor, this.Font);
            return subItem;
        }

        public void GetColumnInfo(ListViewItem item, object o)
        {
            item.SubItems.Clear();
            //item.SubItems[0] = this.MakeSubItem(item, o, Columns[0] as OLVColumn);
            //it seems we start with an empty column we can't fill
            //hack to put the info in the right columns
            for (int i = -1; i < Columns.Count; i++)
            {
                if (i > -1)
                {
                    VColumn col = new VColumn(Columns[i].Text.ToLower());
                    if (i == 0)
                        item.SubItems[0] = this.MakeSubItem(item, o, col);
                    else
                        item.SubItems.Add(this.MakeSubItem(item, o, col));
                }
                //if (subItem == null)
            }
        }
    }
}
