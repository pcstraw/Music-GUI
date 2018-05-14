using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
//using Colour = GlaxionCore.Colour;

//normal calculations
//https://www.youtube.com/watch?v=unL0tN5tnC0

namespace Glaxion.GUI
{
    //make these structs, not normals
    public class Face
    {
        public Face()
        {

        }

        public Face(uint a, uint b, uint c)
        {
            A = a;
            B = b;
            C = c;
           // Console.WriteLine("Faces added through constructor: " + a + " " + b + " " + c);
        }
        public uint A;
        public uint B;
        public uint C;
    }

    public class TextureUV
    {
        public TextureUV()
        {
            U = 0;
            V = 0;
        }

        public TextureUV(float u, float v)
        {
            U = u;
            V = v;
        }

        public float U;
        public float V;
    }

    public class Normal
    {
        public Normal()
        {
            x = 0;
            y = 0;
            z = 0;
        }

        public Normal(float X, float Y, float Z)
        {
            x = X;
            y = Y;
            z = Z;
            // Console.WriteLine("Faces added through constructor: " + a + " " + b + " " + c);
        }
        public float x;
        public float y;
        public float z;
    }

    public class XFile
    {
        public List<float> Data = new List<float>();
        public uint DataCount;
        public List<uint> IndexList = new List<uint>();
        public List<Normal> Normals = new List<Normal>();
        public List<TextureUV> UVs = new List<TextureUV>();
        public List<Normal> FaceNormals = new List<Normal>();
        public List<float> ColourData = new List<float>();
        public List<Face> Faces = new List<Face>();
        public bool Loaded;
        public int bufferPosID;
        public uint bufferID;
        public string Name;
        public string FilePath;
        public bool Debug;
        public float Frame;
   
        public uint DataSize
        {
            get
            {
                return GetDataSize();
            }
            set
            {
                DataSize = value;
            }
        }

        public XFile()
        {

        }

        public XFile(string filePath)
        {
            GetMesh(filePath, false);
        }

        public XFile(string filePath, bool debug)
        {
            GetMesh(filePath, debug);
        }

        public void GetMesh(string filePath)
        {
            GetMesh(filePath, Debug);
        }

        public uint GetDataSize()
        {
            uint byteSize = (uint)Data.Count() * sizeof(float);
            return byteSize;
        }

        /*
        public void ParseFile(string filePath,bool debug)
        {
            XFile xf = XParser.ReadMeshData(filePath,debug);
            Data = xf.Data;
            IndexList = xf.IndexList;
            Normals = xf.Normals;
            FaceNormals = xf.FaceNormals;
            ColourData = xf.ColourData;
            UVs = xf.UVs;
            Faces = xf.Faces;
            FilePath = filePath;
            xf = null;
            CreateVertexNormals();
        }
         * */

        public void CreateVertexNormals()
        {
            if (Debug)
            {
                Console.WriteLine("Face count: " + Faces.Count());
                Console.WriteLine("Face Normal count: " + FaceNormals.Count());
            }

            for (int i = 0; i < Data.Count(); i++)
            {
                float x=0, y=0, z = 0;
                int num = 0;
                for (int j = 0; j < Faces.Count(); j++)
                {
                    if (Faces[j].A == i || Faces[j].B == i || Faces[j].C == i)
                    {
                        x += FaceNormals[j].x;
                        y += FaceNormals[j].y;
                        z += FaceNormals[j].z;
                        num++;
                    }
                }

                if (num != 0)
                {
                    x /= num;
                    y /= num;
                    z /= num;
                }
                float d = (float)Math.Sqrt(x*x+y*y+z*z);
                if (d != 0)
                {
                    x /= d;
                    y /= d;
                    z /= d;
                }
                Normals.Add(new Normal(x, y, z));
            }
        }

        public float[] BuildInterleaveData()
        {
            if (Data.Count() == 0)
            {
                MessageBox.Show("error, no vertex data");
                return null;
            }

            if (Normals.Count() == 0)
            {
                MessageBox.Show("error, no normal data");
                return null;
            }

            if (UVs.Count() == 0)
            {
                Console.WriteLine(" no UV data");
            }
            else
            {
                Console.WriteLine(" UVs "+UVs.Count());
            }

            List<float> dat = new List<float>();
            int vertIndex = 0;

            if (Debug)
            {
                Console.WriteLine(" interleave vertex count: " + Data.Count());
                Console.WriteLine(" interleave normal count: " + Normals.Count());
            }

            for (int i = 0; i < Data.Count() / 3;i++ )
            {
                dat.Add(Data[vertIndex]);
                dat.Add(Data[vertIndex+1]);
                dat.Add(Data[vertIndex+2]);
                dat.Add(Normals[i].x);
                dat.Add(Normals[i].y);
                dat.Add(Normals[i].z);
                
                
                if (UVs.Count() > 0)
                {
                    dat.Add(UVs[i].U);
                    dat.Add(UVs[i].V);
                    //Console.WriteLine("XFile: adding uvs");
                   // dat.Add(1);
                    //dat.Add(1);
                }
                else
                {
                    dat.Add(0.5f);
                    dat.Add(0.5f);
                }
                 
                 

                vertIndex = vertIndex + 3;
            }

            if(Data.Count() != Normals.Count())
                Console.WriteLine(FilePath + "\n"+"error, vertex count and normal count do not match" + "\n\nData count: "+Data.Count() + "\nNormal count: "+IndexList.Count());
            return dat.ToArray();
        }

        public void TestInterleave()
        {
            if (Normals.Count() == 0)
            {
                CreateVertexNormals();
            }

            float[] dat = BuildInterleaveData();

            if(Debug)
            Console.WriteLine("test interleave data: " + Normals.Count());

            int verti = 0;
            int num = 0;

          //  for (int i = 0; i < Normals.Count(); i++)
           //     Console.WriteLine(Normals[i].x + " : " + Normals[i].y + " : " + Normals[i].z);

            for (int i = 0; i < dat.Count()/6; i++)
            {
                if (Debug)
                {
                    Console.Write("\nVertices: " + dat[verti] + "," + dat[verti + 1] + "," + dat[verti + 2]);
                    Console.Write("\nnormal: " + dat[verti + 3] + "," + dat[verti + 4] + "," + dat[verti + 5] + "\n");
                }

                verti += 6;
                num+=6;

            }
            if(Debug)
            Console.WriteLine("\n verti: "+num);
        }

        public void GetMesh(string filePath, bool debug)
        {

            Stream stream = File.OpenRead(filePath);
            StreamReader sr = new StreamReader(stream);
            string line;
            Regex parseMesh = new Regex("Mesh {");
            Regex parseNormals = new Regex("MeshNormals {");
            Regex parseUVs = new Regex("MeshTextureCoords {");
            bool beginNormalRead = false;
            Regex end = new Regex(";;");
            List<string> vertices = new List<string>();
            List<uint> indices = new List<uint>();
            List<uint> uvs = new List<uint>();
            bool beginVertRead = false;
            bool beginIndexRead = false;
            bool beginUVRead = false;
            int index = 0;
            

            while ((line = sr.ReadLine()) != null)
            {
                if (parseMesh.IsMatch(line))
                {
                    beginVertRead = true;
                    if (debug)
                        Console.WriteLine(line);
                }

                if (parseNormals.IsMatch(line))
                {
                    beginNormalRead = true;
                    if (debug)
                        Console.WriteLine(line);
                }

                if (parseUVs.IsMatch(line))
                {
                    beginUVRead = true;
                    if (debug)
                        Console.WriteLine(line);
                }

                if (beginVertRead)
                {

                    if (index == 1)
                    {
                        string resultString = Regex.Match(line, @"\d+").Value;
                        int result = Int32.Parse(resultString);
                        if (debug)
                            Console.WriteLine("Vertex Count: " + result);
                    }

                    //read position data
                    if (index > 1)
                    {
                        string findFloat = @"[-+]?[0-9]*\.?[0-9]+.";
                        string[] parts = line.Split(';');
                        string position1 = Regex.Match(parts[0], findFloat).Value;
                        string position2 = Regex.Match(parts[1], findFloat).Value;
                        string position3 = Regex.Match(parts[2], findFloat).Value;

                        float pos1 = float.Parse(position1, CultureInfo.InvariantCulture);
                        float pos2 = float.Parse(position2, CultureInfo.InvariantCulture);
                        float pos3 = float.Parse(position3, CultureInfo.InvariantCulture);
                        Data.Add(pos1);
                        Data.Add(pos2);
                        Data.Add(pos3);
                    }

                    index++;

                    if (index > 1 & end.IsMatch(line))
                    {
                        beginIndexRead = true;
                        if (debug)
                            Console.WriteLine("Begin Index read");
                        beginVertRead = false;
                        index = 0;
                    }
                }

                if (beginIndexRead)
                {
                    if (index == 1)
                    {
                        string resultString = Regex.Match(line, @"\d+").Value;
                        int result = Int32.Parse(resultString);
                        if (debug)
                            Console.WriteLine("Index Count: " + result);
                    }

                    if (index > 1)
                    {
                        string findInt = @"(?<![-.])\b[0-9]+\b(?!\.[0-9])";
                        string[] parts = line.Split(';');
                        string[] ints = parts[1].Split(',');
                        ints[2].Replace(" ", string.Empty);
                        uint ind1 = uint.Parse(Regex.Match(ints[0], findInt).Value);
                        uint ind2 = uint.Parse(Regex.Match(ints[1], findInt).Value);
                        uint ind3 = uint.Parse(Regex.Match(ints[2], findInt).Value);
                        if (debug)
                            Console.WriteLine("Xparser: Debug Indices: " + ind1 + " " + ind2 + " " + ind3);

                        IndexList.Add(ind1);
                        IndexList.Add(ind2);
                        IndexList.Add(ind3);
                        Faces.Add(new Face(ind1, ind2, ind3));
                    }

                    //End index read
                    if (index > 1 & end.IsMatch(line))
                    {
                        IndexList.Add(IndexList[0]);
                        if (debug)
                            Console.WriteLine("End index read");
                        beginIndexRead = false;
                        index = 0;
                    }
                    index++;
                }

                if (beginNormalRead)
                {

                    if (index == 2)
                    {
                        //get the header data count
                        if (debug)
                        {
                            string resultString = Regex.Match(line, @"\d+").Value;
                            if (resultString != null)
                            {
                                int result = Int32.Parse(resultString);

                                Console.WriteLine("Normal Count: " + result);
                            }
                        }
                    }

                    //read position data
                    if (index > 2)
                    {
                        string findFloat = @"[-+]?[0-9]*\.?[0-9]+.";
                        string[] parts = line.Split(';');
                        string position1 = Regex.Match(parts[0], findFloat).Value;
                        string position2 = Regex.Match(parts[1], findFloat).Value;
                        string position3 = Regex.Match(parts[2], findFloat).Value;

                        if (debug)
                            Console.WriteLine(position1 + " " + position2 + " " + position3);

                        float pos1 = float.Parse(position1, CultureInfo.InvariantCulture);
                        float pos2 = float.Parse(position2, CultureInfo.InvariantCulture);
                        float pos3 = float.Parse(position3, CultureInfo.InvariantCulture);

                        if (debug)
                            Console.WriteLine(pos1 + " " + pos2 + " " + pos3);

                        FaceNormals.Add(new Normal(pos1, pos2, pos3));
                    }

                    index++;

                    if (index > 1 & end.IsMatch(line))
                    {

                        if (debug)
                            Console.WriteLine("End Normal read");

                        beginNormalRead = false;
                        index = 0;
                        line = null;
                    }
                }

                if (beginUVRead)
                {

                    if (index == 1)
                    {
                        //get the header data count
                        if (debug)
                            Console.WriteLine("begin UV read");

                        if (debug)
                        {
                            string resultString = Regex.Match(line, @"\d+").Value;
                            if (resultString != null)
                            {
                                int result = Int32.Parse(resultString);

                                Console.WriteLine("UV Count: " + result);
                            }
                        }
                    }

                    if (index > 1)
                    {
                        string findFloat = @"[-+]?[0-9]*\.?[0-9]+.";
                        string[] parts = line.Split(';');
                        string position1 = Regex.Match(parts[0], findFloat).Value;
                        string position2 = Regex.Match(parts[1], findFloat).Value;

                        float pos1 = float.Parse(position1, CultureInfo.InvariantCulture);
                        float pos2 = float.Parse(position2, CultureInfo.InvariantCulture);

                        if (debug)
                            Console.WriteLine(string.Concat("U: ", pos1, " V: ", pos2));
                        UVs.Add(new TextureUV(pos1, pos2));
                    }

                    index++;

                    if (index > 1 & end.IsMatch(line))
                    {

                        if (debug)
                            Console.WriteLine("End UV read");

                        beginUVRead = false;
                        index = 0;
                        line = null;
                    }
                }
            }

            sr.Close();
            stream.Close();

            CreateVertexNormals();
            Name = Path.GetFileNameWithoutExtension(filePath);
        }
    }
}
