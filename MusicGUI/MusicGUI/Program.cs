using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glaxion.Music;

using Glaxion.Music;
using System.Windows.Forms;

namespace Glaxion.Music
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            /*
            if(args.Length > 0)
            {
                foreach(string s in args)
                {
                    MessageBox.Show(s);
                }
            }
            */
            Application.EnableVisualStyles();
            //Application.VisualStyleState = System.Windows.Forms.VisualStyles.VisualStyleState.NonClientAreaEnabled;
            Application.SetCompatibleTextRenderingDefault(false);
            MusicPlayer.Create(args);
            Application.Run(new MusicControlGUI());
        }
    }
}
