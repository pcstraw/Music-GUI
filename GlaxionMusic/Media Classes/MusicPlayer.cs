using System;
using System.Collections.Generic;
using System.Linq;
using WMPLib;
using Glaxion.Tools;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace Glaxion.Music
{
    public class MusicPlayer
    {
        private void Construction()
        {
            windowsMediaPlayer = new WindowsMediaPlayer();
            PlayEvent += On_Play;
            PauseEvent += On_Pause;
            ResumeEvent += On_Resume;
            NextEvent += On_Next;
            PrevEvent += On_Prev;
            StopEvent += On_Stop;
            TickEvent += On_Tick;
            DirectoriesLoadedEvent += On_DirectoriesLoaded;
            TrackChangeEvent += On_TrackChange;
            PlaybackFailedEvent += On_PlaybackFailed;
            BeforePlayEvent += MusicPlayer_BeforePlayEvent;
            MusicUpdatedEvent += MusicPlayer_MusicUpdated;
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 100;
            timer.Tick += t_Tick;
            CreateTempPlaybackFiles = true;
            _isRunning = false;
            //initialise to -1.  If we start at 0 then loading trackmanager
            //will highlight the  first track of the playlist before we start playing anything
            currentTrack = -1; 
        }
        //used to call updatePlayliststate in trackmanager
        private void MusicPlayer_BeforePlayEvent(object sender, EventArgs args)
        {
        }

        
        public static void Create(string[] args)
        {
            Player = new MusicPlayer();
           // Player._startPlayer = true;
            Player.startArguments = args;
        }

        public MusicPlayer()
        {
            Construction();
        }

        
        public static MusicPlayer Player;
        public static MusicControlGUI WinFormApp;
        public static Image MusicGUILogo = Glaxion.Music.Properties.Resources.music_gui_logo;
        public bool CreateTempPlaybackFiles { get; private set; }
        public WindowsMediaPlayer windowsMediaPlayer;
        public double positionIndex;
        public double trackDuration;
        public bool Stopped;
        public bool IsPlaying;
        public bool IsPaused;
        public bool mediaLoaded;
        public bool mediaLoading;
        public bool ProgramInitialized;
        public bool NodeDrop;
        public bool Loop;
        public bool Muted;
        //bool _startPlayer;
        public bool InitializeDirectories;
        public bool Initialized;
        bool _isRunning;
        public string prevTrack;
        public string currentTrackString;
        public string[] startArguments;
        public int currentTrack;
        public int trackbarValue;
        private int prevVolume;
        public int Volume;
        public Playlist playlist;
        public FileLoader fileLoader;
        public System.Windows.Forms.Timer timer;
        public static Color PlayColor = Color.Aquamarine;
        public static Color PreviousPlayColor = Color.DarkCyan;
        public static Color OldPlayColor = Color.MediumTurquoise;
        public static Color MissingColor = Color.OrangeRed;
        public static Color RepeatColor = Color.DarkSlateBlue;
        public static Color ConflictColor = Color.MediumVioletRed;

        public delegate void MusicUpdatedEventHandler(object sender, EventArgs args);
        public event MusicUpdatedEventHandler MusicUpdatedEvent;
        protected void MusicPlayer_MusicUpdated(object sender, EventArgs e)
        {
        }

        //  public static MusicGUI musicGUI;
        //pause Event handler
        public delegate void PlaybackFailedEventHandler(object sender, EventArgs args);
        public event PlaybackFailedEventHandler PlaybackFailedEvent;
        protected void On_PlaybackFailed(object sender, EventArgs e)
        {
            tool.debugError("Playback Failed event is being called twice");
            return;
        }
        //Play event handler
        public delegate void PlayEventHandler(object sender, EventArgs args);
        public event PlayEventHandler PlayEvent;
        protected void On_Play(object sender, EventArgs e)
        {
        }

        //pause Event handler
        public delegate void PauseEventHandler(object sender, EventArgs args);
        public event PauseEventHandler PauseEvent;
        protected void On_Pause(object sender, EventArgs e)
        {
        }

        //Next Event handler
        public delegate void NextEventHandler(object sender, EventArgs args);
        public event NextEventHandler NextEvent;
        protected void On_Next(object sender, EventArgs e)
        {
        }

        //Prev Event handler
        public delegate void PrevEventHandler(object sender, EventArgs args);
        public event PrevEventHandler PrevEvent;
        protected void On_Prev(object sender, EventArgs e)
        {
        }

        //Stop Event handler
        public delegate void StopEventHandler(object sender, EventArgs args);
        public event StopEventHandler StopEvent;
        protected void On_Stop(object sender, EventArgs e)
        {
        }

        //Resume Event handler
        public delegate void ResumeEventHandler(object sender, EventArgs args);
        public event ResumeEventHandler ResumeEvent;
        protected void On_Resume(object sender, EventArgs e)
        {
        }

        public delegate void TickEventHandler(object sender, EventArgs args);
        public event TickEventHandler TickEvent;
        protected void On_Tick(object sender, EventArgs e)
        {
           // tool.show(1, "tick");
        }

        public delegate void TrackChangeEventHandler(object sender, EventArgs args);
        public event TrackChangeEventHandler TrackChangeEvent;
        protected void On_TrackChange(object sender, EventArgs e)
        {
        }

        public delegate void MediaLoadedEventHandler(object sender, EventArgs args);
        public event MediaLoadedEventHandler MediaLoadedEvent;
        protected void On_MediaLoaded(object sender, EventArgs e)
        {
        }

        public delegate void DirectoriesLoadedEventHandler(object sender, EventArgs args);
        public event DirectoriesLoadedEventHandler DirectoriesLoadedEvent;
        protected void On_DirectoriesLoaded(object sender, EventArgs e)
        {
        }

        public delegate void DirectoriesAddedEventHandler(object sender, EventArgs args);
        public event DirectoriesAddedEventHandler DirectoriesAddedEvent;
        protected void On_DirectoriesAdded(object sender, EventArgs e)
        {
        }

        //Upodate Music Player Event handler
        public delegate void BeforePlayEventHandler(object sender, EventArgs args);
        public event BeforePlayEventHandler BeforePlayEvent;
        protected void On_BeforePlay(object sender, EventArgs e)
        {
        }

        // Define the event handlers.
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            // Specify what is done when a file is changed, created, or deleted.
            tool.debug("File: " + e.FullPath + " " + e.ChangeType);
        }

        private void OnRenamed(object source, RenamedEventArgs e)
        {
            // Specify what is done when a file is renamed.
            tool.debug("File: {0} renamed to {1}", e.OldFullPath, e.FullPath);
        }

        public bool Start()
        {
            //needs to be called from music file manager
            //GetSavedDirectories();
            fileLoader = new FileLoader();
            fileLoader.Load();
            timer.Start();
            return true;
        }

        public void UseMediaKeys(Keys KeyCode)
        {
           // Tool.Debug(k.KeyCode.ToString());
            if (KeyCode == Keys.MediaPlayPause)
            {
                if (IsPlaying)
                {
                    Pause();
                    return;
                }
                if (IsPaused)
                {
                    Resume(currentTrackString, positionIndex);
                    return;
                }
                if (!IsPlaying && !IsPaused)
                {
                    Play(currentTrack);
                    return;
                }
            }

            if (KeyCode == Keys.MediaNextTrack)
            {
                NextTrack();
                return;
            }
            if (KeyCode == Keys.MediaPreviousTrack)
            {
                PrevTrack();
                return;
            }
            if (KeyCode == Keys.MediaStop)
            {
                Stop();
                return;
            }
            if (KeyCode == Keys.VolumeMute)
            {
                Mute();
                return;
            }
        }

        //save the last played playlist??
        public void SetLastPlaylistTrack()
        {
            string lp = Properties.Settings.Default.LastPlaylist;
            Playlist p = fileLoader.GetPlaylist(lp, true);
            p.UpdatePaths();
            if (p == null)
                tool.show();
            string t = Properties.Settings.Default.LastTrack;
            int index = Properties.Settings.Default.LastTrackIndex;
            UpdateMusicPlayer(p, index);
        }

        public void UpdateMusicPlayer(Playlist p,int index)
        {
            if (p == null)
            {
                tool.Show(5,"error: playlist is null", p.name);
                return;
            }
            playlist = p;
            if (index > -1 && index < p.tracks.Count())
            {
                currentTrack = index;
                p.trackIndex = currentTrack;
            }
            MusicUpdatedEvent(p, EventArgs.Empty);
        }
        
        public void OnKeydown(object o, System.Windows.Forms.KeyEventArgs k)
        {
            tool.debug(k.KeyCode.ToString());
            if (k.KeyCode == Keys.MediaPlayPause)
            {
                if (IsPlaying)
                {
                    Pause();
                    return;
                }
                if (IsPaused)
                {
                    Resume(currentTrackString, positionIndex);
                    return;
                }
                if (!IsPlaying && !IsPaused)
                {
                    Play(currentTrack);
                    return;
                }
            }

            if (k.KeyCode == Keys.MediaNextTrack)
            {
                NextTrack();
                return;
            }
            if (k.KeyCode == Keys.MediaPreviousTrack)
            {
                PrevTrack();
                return;
            }
            if (k.KeyCode == Keys.MediaStop)
            {
                Stop();
                return;
            }
            if (k.KeyCode == Keys.VolumeMute)
            {
                Mute();
                return;
            }
        }

        public bool PlayPlaylist(Playlist p,int index)
        {
            if (p == null)
                return false;
            
            p.trackIndex = index;
            
            if (index < 0)
                index = 0;
            
            if (p != playlist)
            {
                playlist = p;
                MusicUpdatedEvent(p, EventArgs.Empty);
            }
            return Play(index);
        }
        
        public bool Mute()
        {
            Muted = !Muted;

            if (Muted)
            {
                prevVolume = Volume;
                SetVolume(0);
                return Muted;
            }
            else
            {
                SetVolume(prevVolume);
                return Muted;
            }
        }

        
        void t_Tick(object sender, EventArgs e)
        {
            if(!_isRunning)
            {
                SetVolume(Volume); //default volume may have been set through the playbackVolumeConbtrol.  Update it here 
                _isRunning = true;
            }

            TickEvent(sender, e);
            if (windowsMediaPlayer.playState == WMPPlayState.wmppsStopped)
            {
                if (IsPlaying)
                    NextTrack();
            }
            
            if (windowsMediaPlayer != null && windowsMediaPlayer.currentMedia != null)
            {
                if (windowsMediaPlayer.openState == WMPLib.WMPOpenState.wmposMediaOpen)
                {
                    mediaLoading = false;
                    if (!mediaLoaded)
                    {
                        MediaLoadedEvent(sender, e);
                        mediaLoaded = true;
                    }
                    trackbarValue = (int)windowsMediaPlayer.controls.currentPosition;/// trackBar.Maximum;
                }
                else
                {
                    mediaLoaded = false;
                }
            }
        }
        
        
        public bool IsPlayingTrack(string path)
        {
            if (path == currentTrackString)
                return true;
            return false;
        }

        //used to update file paths based on file name
        public List<string> SearchMusicFiles(string fileName)
        {
            List<string> ls = new List<string>();
            tool.debugWarning("Warning:  Lock fileLoader.MusicFiles before using it for searching");
            foreach (KeyValuePair<string, string> kv in fileLoader.MusicFiles)
            {
                if (kv.Value == fileName)
                    ls.Add(kv.Key);
            }
            return ls;
        }

        public bool HasStopped()
        {
            if (windowsMediaPlayer.playState == WMPPlayState.wmppsStopped)
                return true;
            else
                return false;
        }
        
        public void NextTrack()
        {
            if (playlist == null)
                return;

            int nextindex = currentTrack + 1;
            if (nextindex >= playlist.tracks.Count)
                nextindex = 0;
            else
            {
                if (File.Exists(playlist.tracks[nextindex]))
                {
                    Play(nextindex);
                    NextEvent(null, EventArgs.Empty);
                    return;
                }
                else
                {
                    currentTrack = nextindex;
                    NextTrack();
                }
            }
        }

        public void PrevTrack()
        {
            if (playlist == null)
                return;

            int nextindex = currentTrack- 1;
            if (nextindex < 0)
                nextindex = playlist.tracks.Count-1;
            if (IsPlaying)
            {
                if (File.Exists(playlist.tracks[nextindex]))
                {
                    Play(nextindex);
                    PrevEvent(null, EventArgs.Empty);
                    return;
                }else
                {
                    currentTrack = nextindex;
                    PrevTrack();
                }
            }
        }
        public bool Play()
        {
            return Play(currentTrack);
        }

        public bool Play(int index)
        {
            if(playlist == null)
            {
                PlaybackFailedEvent("Playlist is Missing", EventArgs.Empty);
                Stop();
                tool.show(2, "NO PLAYLIST","","Playback Failed");
                return false;
            }

            string file = playlist.tracks[index];

            if (!File.Exists(file))
            {
                PlaybackFailedEvent(file, EventArgs.Empty);
                Stop();
                tool.show(2, "INVALID FILE", "","Playback Failed");
                return false;
            }

            BeforePlayEvent(file, EventArgs.Empty);

            string f = file;
            if (CreateTempPlaybackFiles)
                f = CreateTempPlayFile(file);  //quickly create a temp file for playback so when can edit the tags

            windowsMediaPlayer.URL = f;
            windowsMediaPlayer.controls.play();
            
            mediaLoading = true;
            IsPaused = false;
            IsPlaying = true;
            Stopped = false;
            if (index != currentTrack)
                TrackChangeEvent(null, EventArgs.Empty);

            prevTrack = currentTrackString;
            currentTrack = index;
            currentTrackString = file;
            playlist.trackIndex = index;
            PlayEvent(null, EventArgs.Empty);
            return true;
        }

        private string CreateTempPlayFile(string file)
        {
            string temp_dir = tool.GetTempFolder();
            string file_name = Path.GetFileName(file);
            string ext = Path.GetExtension(file);
            string new_file = string.Concat(temp_dir, file_name);
            if(File.Exists(new_file))
                return new_file;

            File.Copy(file, new_file, true);
            return new_file;
        }

        public bool PlayFile(string file)
        {
            if (!File.Exists(file))
            {
                PlaybackFailedEvent("Music Player: Cannot Find file: " + file, EventArgs.Empty);
                return false;
            }

            BeforePlayEvent(file,EventArgs.Empty);
            string f = file;

            if (CreateTempPlaybackFiles)
                f = CreateTempPlayFile(file);  //quickly create a temp file for playback so when can edit the tags

            windowsMediaPlayer.URL = f;
            windowsMediaPlayer.controls.play();
            mediaLoading = true;
            IsPaused = false;
            IsPlaying = true;
            Stopped = false;

            if (file != currentTrackString)
            {
                EventArgs e = new EventArgs();
                TrackChangeEvent(currentTrackString, EventArgs.Empty);
            }

            currentTrackString = file;
            PlayEvent(null, EventArgs.Empty);
            return true;
        }
        
        public void Replay()
        {
            Stop();
            Play(currentTrack);
        }

        public void Stop()
        {
            positionIndex = windowsMediaPlayer.controls.currentPosition;
            windowsMediaPlayer.controls.stop();
            IsPlaying = false;
            IsPaused = false;
            Stopped = true;
            StopEvent(null, EventArgs.Empty);
        }

        public void Pause()
        {
            windowsMediaPlayer.controls.pause();
            IsPlaying = false;
            IsPaused = true;
            positionIndex = windowsMediaPlayer.controls.currentPosition;
            PauseEvent(null, EventArgs.Empty);
        }

        public void Resume(string s, double position)
        {
            windowsMediaPlayer.controls.play();
            windowsMediaPlayer.controls.currentPosition = position;
            IsPaused = false;
            IsPlaying = true;
            Stopped = false;
            positionIndex = position;
            ResumeEvent(null, EventArgs.Empty);
        }

        public void Resume()
        {
            Resume(currentTrackString, positionIndex);
        }

        public void SetVolume(int value)
        {
            Volume = value;
            if (Muted)
                windowsMediaPlayer.settings.volume = 0;
            else
                windowsMediaPlayer.settings.volume = Volume;
        }
    }
}
