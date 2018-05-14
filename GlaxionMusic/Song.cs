using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Glaxion.Tools;
using System.Diagnostics;
using System.Drawing;
using TagLib;


//id3 info class example
//https://www.codeproject.com/Articles/17890/Do-Anything-With-ID

//ID3 lib
//https://sourceforge.net/projects/csid3lib/
//saving pictures to ID3
//https://stackoverflow.com/questions/25944311/write-picture-to-mp3-file-id3-net-windows-store-apps?utm_medium=organic&utm_source=google_rich_qa&utm_campaign=google_rich_qa


//https://stackoverflow.com/questions/68283/view-edit-id3-data-for-mp3-files?utm_medium=organic&utm_source=google_rich_qa&utm_campaign=google_rich_qa
//https://github.com/crowell/Linebeck/tree/master/ultraID3lib

//true peak analysis for regain
//https://github.com/audionuma/libtruepeak/tree/master/src/truepeak
//https://toneprints.com/media/1017421/lundt013011.pdf
//http://www.speech.kth.se/prod/publications/files/3319.pdf
//http://www.speech.kth.se/prod/publications/files/3319.pdf


//mp3 info
//https://github.com/moumar/ruby-mp3info
//https://stackoverflow.com/questions/13404957/loading-album-art-with-taglib-sharp-and-then-saving-it-to-same-different-file-in/13612644?utm_medium=organic&utm_source=google_rich_qa&utm_campaign=google_rich_qa



namespace Glaxion.Music
{
    public class Song
    {
        private void Construction()
        {
            title = "Something";
            album = "Untitled";
            artist = "Someone";
            year = "Unknown";
        }

        public Song(string filePath)
        {
            path = filePath;
            name = Path.GetFileNameWithoutExtension(filePath);
            loaded = false;
            Construction();
        }

        public void Reset()
        {
            loaded = false;
        }

        //taglib will fail if it attempts to read a corrupted mp3 file
        bool isMP3()
        {
            byte[] buf = System.IO.File.ReadAllBytes(path);
            if (buf[0] == 0xFF && (buf[1] & 0xF6) > 0xF0 && (buf[2] & 0xF0) != 0xF0)
            {
                return true;
            }
            return false;
        }

        private byte[] imageToByteArray(Image imageIn)
        {
            Image img;
            using (MemoryStream ms = new MemoryStream())
            {
                img = new Bitmap(imageIn);
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }

        public void SetPictureFromImage(Image i)
        {
            byte[] image_bytes = imageToByteArray(i);
            if (image_bytes == null)
                return;
            image = i;
            file.Tag.Pictures = new TagLib.IPicture[]
            {
                new TagLib.Picture(new TagLib.ByteVector(image_bytes))
            };
        }

        private void TestTagLibLoading()
        {
            TagLib.File file = null;
            
            if (!System.IO.File.Exists(path))
            {
                tool.show(3, "Invalid path", path);
                return;
            }
            try
            {
                
                //if(isMP3())
                    file = TagLib.File.Create(path);
            }
            catch(TagLib.CorruptFileException e)
            {
                tool.show(2,e.Message);
                return;
            }
            if (file == null)
                return;
            file.Mode = TagLib.File.AccessMode.Read;
            file.GetTag(TagTypes.AllTags);
        }

        public bool ReadID3Info()
        {
            if (loaded)
                return true;
            
            if(tool.IsAudioFile(path))
            {
               try
               {
                    //if(isMP3())
                    file = TagLib.File.Create(path);
                    file.GetTag(TagTypes.AllTags);
                    album = file.Tag.Album;
                    artist = file.Tag.FirstAlbumArtist;
                    track = file.Tag.Track;
                    lyrics = file.Tag.Lyrics;
                    pictures = file.Tag.Pictures;
                    title = file.Tag.Title;
                    if (string.IsNullOrWhiteSpace(title))
                        title = Path.GetFileNameWithoutExtension(path);
                    genres = file.Tag.Genres;
                    if(genres.Count() == 0)
                    {
                        genres = new string[] { "" };
                    }
                    year = file.Tag.Year.ToString();
                    failed = false;
                    return loaded = true;
                }
                catch (Exception e)
                {
                    try
                    {
                        //if(isMP3())
                        file = TagLib.File.Create(path);
                        file.GetTag(TagTypes.Id3v2);
                        album = file.Tag.Album;
                        artist = file.Tag.FirstAlbumArtist;
                        track = file.Tag.Track;
                        lyrics = file.Tag.Lyrics;
                        pictures = file.Tag.Pictures;
                        title = file.Tag.Title;
                        genres = file.Tag.Genres;
                        year = file.Tag.Year.ToString();
                        failed = false;
                        return loaded = true;
                    }
                    catch (Exception e2)
                    {
                        TagLoadingLog.Add(string.Concat("--> Failed to Get All Tags: \n", path, "\n", e.Message));
                   
                        TagLoadingLog.Add(string.Concat("--> Also Failed to Get ID3v2 Tag: \n", path, "\n", e2.Message));
                        failed = true;
                        return loaded = false;
                    }
                    //TagLoadingLog.Add(string.Concat("Failed to Get ID3 Tag: \n", path, "\n", e.Message));
                    //failed = true;
                   // return loaded = false;
                }
            }
            failed = true;
            return false;
        }

        public string GetFolderImage()
        {
            return folderImage = tool.GetFolderJPGFile(path);
        }

        public Task LoadAlbumArtAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                LoadAlbumArt();
            });
        }

        //using streamn method for getting image byte[]
        //https://stackoverflow.com/questions/3801275/how-to-convert-image-to-byte-array/16576471#16576471
        public void LoadAlbumArt()
        {
            if(!loaded)
                MusicPlayer.Player.fileLoader.trackInfoManager.GetInfo(path);
            if (failed)
            {
                //tool.show(1, "failed to load ID3 tag: ", failed);
                return;
            }

            image = null;
            if (pictures.Length > 0)
            {
                byte[] bytes = pictures[0].Data.Data;

                ImageConverter ic = new ImageConverter();
                image = (Image)ic.ConvertFrom(bytes);
            }
            if (image == null)
            {
                string pic = GetFolderImage();
                if (System.IO.File.Exists(pic))
                    image = Image.FromFile(pic);
                return;
            }
        }

        public void SaveInfo()
        {
            //try/catch?
            try
            {
                file.Save();
                Reset();
            }
            catch(Exception e)
            {
                tool.show(5, e.Message);
            }
            
            // tool.show(3, "ID Tag Saved: ",path);
        }

        public void SetTitle(string text)
        {
            file.Tag.Title = text;
        }
        public void SetAlbum(string text)
        {
            file.Tag.Album = text;
        }
        public void SetArtist(string text)
        {
            List<string> art = new List<string>();
            art.Add(text);
            file.Tag.AlbumArtists = art.ToArray();
            //throw new NotImplementedException(text);
        }
        public void SetGenre(string text)
        {
            genres = new string[] { text };
            //throw new NotImplementedException(text);
        }
        public void SetYear(uint num)
        {
            file.Tag.Year = num;   
        }
        public void SetYear(string text)
        {
            uint result = 0;
            if(uint.TryParse(text,out result))
            {
                if(result != 0)
                {
                    file.Tag.Year = result;
                }
            }
        }
        public void SetTrack(uint num)
        {
            file.Tag.Track = num;
        }

        public void SetLyrics(string text)
        {
            file.Tag.Lyrics = text;
        }

        public string path;
        public string[] genres;
        public string genre;
        public string album;
        public string artist;
        public string year;
        public string title;
       // public long length;
        //public int genreID;
        public int trackNo;
        public uint track;
        public string folderImage;
        public Image image;
        public string name;
        public string comment;
        public IPicture[] pictures;
        public string lyrics;
        //public Id3Tag tag;
        public TagLib.File file;
        public bool loaded;
        public bool failed;
        public static List<string> TagLoadingLog = new List<string>();
        
        /*
        public static Task<TrackInfo> GetInfoAsync(string path)
        {
            if (Tracks.ContainsKey(path))
                return Tracks[path];
            TrackInfo ti = new TrackInfo(path);
            ti.ReadID3Info();
            Tracks.Add(path, ti);
            return ti;
        }
        */
    }

    public class TrackInfoManager
    {
        public TrackInfoManager()
        {
        }

        public List<string> artists = new List<string>();
        public List<string> albums = new List<string>();
        public List<string> years = new List<string>();
        public List<int> genreIDs = new List<int>();
        public List<string> genres = new List<string>();
        public Dictionary<string, Song> trackInfos = new Dictionary<string, Song>();
        
        //TODO:  Handle path being null and read tag failer
        public Song GetInfo(string path)
        {
            if (!tool.IsAudioFile(path))
                return null;
            if (trackInfos.ContainsKey(path))
            {
                //if the id3 info has been modified then should be set to false, reload the id3 tag
                if(!trackInfos[path].loaded)
                    trackInfos[path].ReadID3Info();
                return trackInfos[path];
            }
            Song ti = new Song(path);
            ti.ReadID3Info();
            AddInfo(ti);
            return ti;
        }

        public void AddInfo(Song info)
        {
            AddAlbum(info.album);
            AddArtist(info.artist);
            AddYear(info.year);
            //AddGenre(info.genre);
            //AddGenreID(info.genreID);
            trackInfos.Add(info.path, info);
            Process p = new Process();
            
        }

        public string AddAlbum(string albumName)
        {
            if (!tool.StringCheck(albumName))
                return albumName;
            if (!albums.Contains(albumName))
                albums.Add(albumName);
                return albumName;
        }

        public string AddArtist(string artistName)
        {
            if (!tool.StringCheck(artistName))
                return artistName;
            if (!artists.Contains(artistName))
                artists.Add(artistName);
            return artistName;
        }

        public string AddYear(string year)
        {
            if (!tool.StringCheck(year))
                return year;
            if (!years.Contains(year))
               years.Add(year);
            return year;
        }

        public string AddGenre(string genre)
        {
            if (!tool.StringCheck(genre))
                return genre;
            if (!genres.Contains(genre))
                genres.Add(genre);
            return genre;
        }
        public int AddGenreID(int genreID)
        {
            if (genreID == 0)
                return 0;
            if (!genreIDs.Contains(genreID))
                genreIDs.Add(genreID);
            return genreID;
        }
    }
}
