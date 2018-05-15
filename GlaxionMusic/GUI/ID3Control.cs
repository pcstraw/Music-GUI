using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Glaxion.Tools;

namespace Glaxion.Music
{
    public partial class ID3Control : UserControl
    {
        public ID3Control()
        {
            InitializeComponent();
            InitializeEntryBoxes();
        }

        public Song song;
        public Song SetSong(Song songInfo)
        {
            song = songInfo;
            ShowSongInfo();
            return song;
        }

        public void SetFont(Font f)
        {
            foreach(EntryBox entry in entries)
            {
                entry.SetFont(f);
            }
        }

        public Font GetFont()
        {
            if (entries.Count == 0)
                return this.Font;
            return entries[0].label1.Font;
        }

        void ResetAllEntries()
        {
            /*
            albumEntryBox.textChanged = false;
            artistEntryBox.textChanged = false;
            yearEntryBox.textChanged = false;
            genreEntryBox.textChanged = false;
            titleEntryBox.textChanged = false;
            */
        }
        public List<EntryBox> entries = new List<EntryBox>();
        void ShowSongInfo()
        {
            foreach (EntryBox entry in entries)
            {
                string s = entry.Tag as string;
                if(s == "artist")
                {
                    entry.SetTextbox(song.artist);
                }
                if (s == "album")
                {
                    entry.SetTextbox(song.album);
                }
                if (s == "title")
                {
                    entry.SetTextbox(song.title);
                }
                if (s == "year")
                {
                    entry.SetTextbox(song.year);
                }
                if (s == "genre")
                {
                    entry.SetTextbox(song.genres[0]);
                }
            }

            /*
            if (song != null)
            {
                albumEntryBox.SetTextbox(song.album);
                artistEntryBox.SetTextbox(song.artist);
                yearEntryBox.SetTextbox(song.year);
                genreEntryBox.SetTextbox(song.genre[0]);
                titleEntryBox.SetTextbox(song.title);
            }
            */
        }

        void Save()
        {
            bool modified = false;
            foreach (EntryBox entry in entries)
            {
                string s = entry.Tag as string;
                if (!entry.textChanged)
                    continue;
                if (s == "artist")
                    song.SetArtist(entry.GetEntry());
                if (s == "album")
                    song.SetAlbum(entry.GetEntry());
                if (s == "title")
                    song.SetTitle(entry.GetEntry());
                if (s == "year")
                    song.SetYear(entry.GetEntry());
                if (s == "genre")
                    song.SetGenre(entry.GetEntry());
                entry.textChanged = false;
                modified = true;
            }
            if (modified)
                song.SaveInfo();
        }
        
        public void InitializeEntryBoxes()
        {
            entries.Clear();
            foreach (Control c in this.Controls)
            {
                if (c is EntryBox)
                {
                    entries.Add(c as EntryBox);
                    c.BringToFront();
                }
            }
            entries.Reverse();
            for (int i = 0; i < entries.Count; i++)
            {
                switch (i)
                {
                    case 0:
                        AssignEntry(entries[i], "Artist");
                        break;
                    case 1:
                        AssignEntry(entries[i], "Album");
                        break;
                    case 2:
                        AssignEntry(entries[i], "Title");
                        break;
                    case 3:
                        AssignEntry(entries[i], "Year");
                        break;
                    case 4:
                        AssignEntry(entries[i], "Genre");
                        break;
                }
            }
        }

        private void AssignEntry(EntryBox entry, string identifier)
        {
            entry.label1.Text = identifier;
            entry.Tag = identifier.ToLower();
        }

        private void ID3Control_Load(object sender, EventArgs e)
        {
            //titleEntryBox.BringToFront();
            
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void entryBox1_FontChanged(object sender, EventArgs e)
        {
            SetFont(this.Font);
        }
    }
}
