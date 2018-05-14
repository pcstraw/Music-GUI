using Glaxion.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glaxion.Music
{
    public class Tests
    {
        public static void RunTests()
        {
            CheckForBlankString();
        }

        public static void CheckForBlankString()
        {
            bool pass = false;
            string s = "             ";

            if (string.IsNullOrWhiteSpace(s))
                pass = true;

            if(pass)
                tool.show(5, "TEST success: found bank string","","Length of string: "+s.Length);
            else
                tool.show(5, "TEST failed: unable find bank string", "", "Length of string: " + s.Length);
        }
    }
}
