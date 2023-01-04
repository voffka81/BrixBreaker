using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.NetworkInformation;
using System.Media;
using System.IO;
using System.Threading;
using System.ComponentModel;
using System.Windows.Threading;
using System.Windows;

namespace WPF.Sound
{
    
    /// <summary>
    /// Works with sound files
    /// </summary>
    public class SoundManager
    {
        static Dictionary<string, MediaFile> SoundBank = new Dictionary<string, MediaFile>();
        static SoundPlayer _player;
        
        private static AudionEndsDelegate _AudionEnds;
        public static event AudionEndsDelegate AudionEnds
        {
            add
            {
                if (_AudionEnds != null || value.GetInvocationList().Length > 1)
                {
                    _AudionEnds = (AudionEndsDelegate)Delegate.Remove(_AudionEnds,_AudionEnds);
                }
                _AudionEnds = (AudionEndsDelegate)Delegate.Combine(_AudionEnds, value);
            }
            remove
            {
                _AudionEnds = (AudionEndsDelegate)Delegate.Remove(_AudionEnds, value);
            }
        }

        public SoundManager()
        {
            _player = new SoundPlayer();
        }

        public void LoadSounds()
        {
            // Load all sound files to memory
            string[] filePaths = Directory.GetFiles(Directory.GetCurrentDirectory()+@"/Media/");
            //string[] filePaths = Directory.GetFiles(@"C:\Windows\Media\");
            foreach (string fileName in filePaths)
            {
                if (fileName.Contains(".wav"))
                {
                    MediaFile Media = new MediaFile(fileName);
                    SoundBank.Add(Media.ActualName, Media);
                }
            }
           
        }
        static BackgroundWorker bg;
        static Thread t;
        static DispatcherTimer timer;

        public void Play(string file)
        {
            //bg = new BackgroundWorker();

            
            _player.Tag = file;
            _player.Stream = new MemoryStream();
            _player.Stream.Write(SoundBank[file].AudioData, 0, SoundBank[file].AudioData.Length);
            _player.Stream.Seek(0, SeekOrigin.Begin);
           
            
            timer = null;
            timer = new DispatcherTimer();
            timer.Tick += timer_Tick;
            timer.Interval = SoundBank[file].Duration;
            _player.Play();
            timer.Start();
        }

        public void PlaySync(string file)
        {
            //bg = new BackgroundWorker();


            _player.Tag = file;
            _player.Stream = new MemoryStream();
            _player.Stream.Write(SoundBank[file].AudioData, 0, SoundBank[file].AudioData.Length);
            _player.Stream.Seek(0, SeekOrigin.Begin);

            _player.PlaySync();
        }

        static void timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            if (_AudionEnds != null && _player.Tag != "Wait")
            {
                Application.Current.Dispatcher.Invoke(new Action(()=>{
                    _AudionEnds();
                }));
            }
        }
        
        public static void Stop()
        {
            timer.Stop();
            _player.Stop();
        }
    }
}
