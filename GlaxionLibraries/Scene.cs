using Glaxion.Tools;
using Novacode;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glaxion.Libraries
{
    public class Scene
    {
        public string Name;
        public string ProjectName;
        public string projectPath;
        public int index = 0;
        public DocX doc;
        public  List<string> audioFiles = new List<string>();
        public List<string> textFiles = new List<string>();
        public List<string> imageFiles = new List<string>();
        public List<string> videoFiles = new List<string>();
        public GlaxFile glaxFile;
        public string extention = ".scene";
        public string directory;
        public string fullPath;

        public Scene()
        {

        }

        public Scene(string name)
        {
            Name = name;
        }

        public void AddFile(string s)
        {
            if (tool.IsAudioFile(s)||tool.IsPlaylistFile(s))
            {
                audioFiles.Add(s);
            }
            if (tool.IsImageFile(s))
            {
                imageFiles.Add(s);
            }
            if (tool.IsVideoFile(s))
            {
                videoFiles.Add(s);
            }
            if (tool.IsTextFile(s))
            {
                textFiles.Add(s);
            }
        }

        public void LoadFile(string FullPath)
        {
            if (!File.Exists(FullPath))
            {
                tool.debug("Scene: File ", fullPath, " Does not exist");
                return;
            }
            fullPath = FullPath;
            Name = Path.GetFileNameWithoutExtension(FullPath);
            extention = Path.GetExtension(FullPath);
            glaxFile = new GlaxFile();
            glaxFile.SetFromFile(FullPath);
          //  glaxFile.ReadExistingFile();
          //  Tools.tool.Show(FullPath);
            string line;
            string prevline = null;
            
            StreamReader sr = new StreamReader(FullPath);
            while((line = sr.ReadLine()) != null)
            {
                Tools.tool.debug(line);
                if(line == "::Project Name")
                {
                    ProjectName = prevline;
                }

                if (line == "::Project Path")
                {
                    projectPath = prevline;
                }

                if (line == "::Scene Name")
                {
                    Name = prevline;
                }

                if(line == "::Index")
                {
                    index = int.Parse(prevline);
                }

                if(line == "::A")
                {
                    audioFiles.Add(prevline);
                }

                if(line == "::T")
                {
                    textFiles.Add(prevline);
                }

                if (line == "::I")
                {
                    imageFiles.Add(prevline);
                }

                if(line == "::V")
                {
                    videoFiles.Add(prevline);
                }

                prevline = line;
            }
            sr.Close();
           // glaxFile.CloseStream();
        }

        public void RemoveFromList(List<string> list, string s)
        {
            if (list.Contains(s))
                list.Remove(s);
        }

        public void RemoveEntry(string s)
        {
            RemoveFromList(this.audioFiles, s);
            RemoveFromList(this.textFiles, s);
            RemoveFromList(this.videoFiles, s);
            RemoveFromList(this.imageFiles, s);
        }

        public void SaveToFile()
        {
            SaveToFile(fullPath);
        }

        public void SaveToDirectory(string dir)
        {
            fullPath = dir + "\\" + Name + extention;
            SaveToFile(fullPath);
        }

        public void SaveToFile(string fullPath)
        {

            string dir = Path.GetDirectoryName(fullPath);
            directory = dir;
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            tool.debug(fullPath);

            glaxFile = new GlaxFile();
            glaxFile.Create(dir, Name, extention);

            glaxFile.WriteEntry(null, ProjectName, "::Project Name");
            glaxFile.WriteEntry(null, projectPath, "::Project Path");
            glaxFile.WriteEntry(null, Name, "::Scene Name");
            glaxFile.WriteEntry(null, index.ToString(), "::Index");

            foreach(string s in audioFiles)
            {
                glaxFile.WriteEntry(null, s, "::A");
            }

            foreach (string s in textFiles)
            {
                glaxFile.WriteEntry(null, s, "::T");
            }

            foreach (string s in imageFiles)
            {
                glaxFile.WriteEntry(null, s, "::I");
            }

            foreach (string s in videoFiles)
            {
                glaxFile.WriteEntry(null, s, "::V");
            }

            glaxFile.CloseStream();
        }
    }
}
