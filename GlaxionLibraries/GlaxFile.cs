using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using Glaxion.Tools;
using System.Windows.Forms;

namespace Glaxion.Libraries
{
    public class GlaxFile
    {
        public GlaxFile()
        {

        }

        public GlaxFile(string path)
        {
            FullPath = path;
        }

        public string Name;
        public string FilePath;
        public string Extension;
        public string FullPath;
        public System.IO.StreamWriter writer;
        public StreamReader reader;

        public void Create(string path, string name,string extension)
        {
            Name = name;
            FilePath = path+"\\";
            Extension = extension;
            FullPath = FilePath + Name + Extension;
            writer = new System.IO.StreamWriter(FullPath);
        }

        private static string IndexHeader(string text)
        {
            return text;
        }

        public void CloseStream()
        {
            if(writer != null)
            writer.Close();

            if (reader != null)
                reader.Close();
        }

        public void WriteEntry(string start,string entry, string end)
        {
            if (start != null)
                writer.WriteLine(start);
            writer.WriteLine(entry);
            if (end != null)
                writer.WriteLine(end);
        }

        public void WriteList(string start,List<string> list,string end)
        {
            foreach(string s in list)
            {
                WriteEntry(start, s, end);
            }
        }


        public void SetFromFile(string path)
        {
            Name = Path.GetFileNameWithoutExtension(path);
            FilePath = Path.GetDirectoryName(path) + Name;
            FullPath = path;
        }

        public StreamReader ReadExistingFile()
        {

            if (!File.Exists(FullPath))
            {
                Console.WriteLine("Glaxwriter-> writer does not exist: " + FullPath);
                return null;
            }
            return new StreamReader(FullPath);
        }

        public StreamWriter WriteExistingFile()
        {
            if (!File.Exists(FullPath))
            {
                Console.WriteLine("Glaxwriter-> writer does not exist: " + FullPath);
                return null;
            }

            return new System.IO.StreamWriter(FullPath, true);
        }

        public void OpenGlax()
        {
            Process.Start(FullPath);
        }
    }
}
