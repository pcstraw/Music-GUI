using Glaxion.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glaxion.Music
{
    public class MusicInfo
    {
        private MusicInfo() //lazy singleton
        {
        }
        public static MusicInfo Instance { get { return Nested.instance; } }
        private class Nested
        {
            static Nested() { }
            internal static readonly MusicInfo instance = new MusicInfo();
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
                if (!trackInfos[path].loaded)
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
            if (info == null)
                return;
            if (trackInfos.ContainsKey(info.path))
                return;
            trackInfos.Add(info.path, info);
            AddAlbum(info.album);
            AddArtist(info.artist);
            AddYear(info.year);
            AddGenre(info.genre);
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
