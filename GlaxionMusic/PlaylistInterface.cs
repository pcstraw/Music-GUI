using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glaxion.Music
{
    public interface IPlaylistView
    {
        Playlist CurrentList { get; set; }
        void DisplayPlaylist();
    }
}
