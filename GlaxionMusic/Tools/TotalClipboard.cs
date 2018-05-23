using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ListViewItem = System.Windows.Forms.ListViewItem;
//using Glaxion.Libraries;
using Glaxion.Tools;
using System.Collections.Specialized;
using System.IO;

namespace Glaxion.Music
{
    public class TotalClipboard
    {
        public static List<string> Files = new List<string>();
        public static List<Playlist> Playlists = new List<Playlist>();
        public static List<ListViewItem> Items = new List<ListViewItem>();
        //public static ViewBox SourceListBox;

        public static void CopyTreeNodeToItem(TreeNode node)
        {
            ListViewItem i = new ListViewItem(node.Text);
            i.SubItems.Add(new System.Windows.Forms.ListViewItem.ListViewSubItem());
            i.SubItems[1].Text = node.Tag as string;
            Items.Add(i);
        }

        public static void CopySelectedItems(System.Windows.Forms.ListView.SelectedListViewItemCollection collection)
        {
            Items.Clear();
            Files.Clear();
            foreach (ListViewItem item in collection)
            {
                Items.Add(item);
                Files.Add(item.Tag as string);
            }
            
        }

        public static void CopySelectedItems(List<ListViewItem> collection)
        {
            Items.Clear();
            foreach (ListViewItem item in collection)
            {
                Items.Add(item);
            }
        }

        public static bool IsPlaylist
        {
            get
            {
                return IsPlaylistClipboard();
            }
        }

        public static void CopyFile(List<string> ListFilePaths)
        {
            System.Collections.Specialized.StringCollection FileCollection = new System.Collections.Specialized.StringCollection();

            foreach (string FileToCopy in ListFilePaths)
            {
                FileCollection.Add(FileToCopy);
            }

            Clipboard.SetFileDropList(FileCollection);
        }

        public static void CopyToSystemClipboard(List<ListViewItem> items)
        {
            System.Windows.Clipboard.Clear();
            foreach (ListViewItem item in items)
            {
                System.Windows.Clipboard.SetText(item.SubItems[1].Text);
            }
        }

        public static void CopyToSystemClipboard(List<string> items)
        {
            StringCollection paths = new StringCollection();
            
            foreach (string item in items)
            {
                paths.Add(item);
            }
            Clipboard.SetFileDropList(paths);

        }

        public static void Copy(ListView listview)
        {
            Files.Clear();
            foreach(ListViewItem i in listview.SelectedItems)
            {
                if(i.Tag is string)
                Files.Add(i.Tag as string);
                if (i.Tag is Playlist)
                    Files.Add((i.Tag as Playlist).path);
            }
        }

        public static void CopyItems(List<ListViewItem> items)
        {
            TotalClipboard.Clear();
            Clipboard.Clear();
            foreach (ListViewItem item in items)
            {
                TotalClipboard.Add(item.SubItems[1].Text);
                Clipboard.SetText(item.SubItems[1].Text);
            }
        }

        public static bool IsPlaylistClipboard()
        {
            if (Playlists.Count > 0)
                return true;
            else
                return false;
        }

        public static void AddNodes(TreeNode node)
        {
            // tool.Show(t.Tag as string);
            string s = node.Tag as string;
            if (string.IsNullOrEmpty(s))
                return;

            if (Path.HasExtension(s))
            {
                if (File.Exists(s))
                {
                    TotalClipboard.Add(s);
                    return;
                }
            }
            else
            {
                foreach (TreeNode t in node.Nodes)
                {
                    // tool.Show(t.Tag as string);
                    string p = node.Tag as string;
                    if (string.IsNullOrEmpty(p))
                        continue;
                    Console.WriteLine(p);
                    if (File.Exists(p) && Path.HasExtension(p))
                        TotalClipboard.Add(t.Tag as string);
                    if (t.Nodes.Count > 0)
                    {
                        foreach (TreeNode tn in node.Nodes)
                            AddNodes(tn);
                    }
                    
                }
            }
        }

        public static void CopyTree(TreeViewMS tree)
        {
            TotalClipboard.Clear();
            List<string> list = new List<string>();
            foreach (TreeNode t in tree.SelectedNodes)
            {
                // tool.Show(t.Tag as string);
                string s = t.Name;
                if (string.IsNullOrEmpty(s))
                    continue;
                //List<string> ls = tool.GetAllAudioFiles(t);
                //foreach (string text in ls)
                Files.Add(s);
            }
        }

        public static void CopyToSystemClipboard()
        {
            StringCollection paths = new StringCollection();
            foreach(string s in Files)
            {
                paths.Add(s);
            }
            DataObject data = new DataObject(DataFormats.FileDrop,Files);
            //also add the selection as textdata
            data.SetData(DataFormats.StringFormat, Files[0]);
            //do the dragdrop
           // DoDragDrop(data, DragDropEffects.Copy);
            Clipboard.SetFileDropList(paths);
        }
        /*
        public static void CopyNodeToClipboard(TreeNode node)
        {
            TotalClipboard.Clear();
            TotalClipboard.Add(node.Tag as string);
        }
        */

        public static void Add(string text)
        {
            Files.Add(text);
        }

        public static void Clear()
        {
            Files.Clear();
        }

        public static int Count
        {
            get
            {
                return Files.Count();
            }
        }

        public static bool IsEmpty
        {
            get
            {
                return Empty();
            }
        }

        public static bool Empty()
        {
            if (Files.Count == 0)
                return true;
            else
                return false;
        }

        public static bool IsPlaylistEmpty()
        {
            if (Playlists.Count == 0)
                return true;
            else
                return false;
        }
    }

    public static class MouseHook

    {
        public static event EventHandler MouseAction = delegate { };

        public static void Start()
        {
            _hookID = SetHook(_proc);


        }
        public static void stop()
        {
            UnhookWindowsHookEx(_hookID);
        }

        private static LowLevelMouseProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;

        private static IntPtr SetHook(LowLevelMouseProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_MOUSE_LL, proc,
                  GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr HookCallback(
          int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && MouseMessages.WM_LBUTTONDOWN == (MouseMessages)wParam)
            {
                MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                MouseAction(null, new EventArgs());
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        private const int WH_MOUSE_LL = 14;

        private enum MouseMessages
        {
            WM_LBUTTONDOWN = 0x0201,
            WM_LBUTTONUP = 0x0202,
            WM_MOUSEMOVE = 0x0200,
            WM_MOUSEWHEEL = 0x020A,
            WM_RBUTTONDOWN = 0x0204,
            WM_RBUTTONUP = 0x0205
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
          LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
          IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);


    }
}
