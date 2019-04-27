using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Streaming
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        LibVLC _libVLC;
        MediaPlayer _mediaPlayer;

        public MainWindow()
        {
            InitializeComponent();
            //var currentDirectory = Environment.CurrentDirectory;
            //videoView.Loaded += VideoView_Loaded;

            LoadVLCDotNet();
        }
        private void LoadVLCDotNet()
        {
            var currentDirectory = Environment.CurrentDirectory;
            var vlcPath = System.IO.Path.Combine(currentDirectory, "libvlc", IntPtr.Size == 4 ? "win-x86" : "win-x64");
            
            var options = new string[]
            {
                "--video-filter=adjust",
                "--brightness=1",
                "--saturation=1"
            };

            vlcControl.SourceProvider.CreatePlayer(new DirectoryInfo(vlcPath));

            vlcControl.SourceProvider.MediaPlayer.Play("rtsp://192.168.2.39/live.sdp");
            //vlcControl.SourceProvider.MediaPlayer.Play("rtsp://wowzaec2demo.streamlock.net/vod/mp4:BigBuckBunny_115k.mov");

        }

        /*
        private void VideoView_Loaded(object sender, RoutedEventArgs e)
        {
            Core.Initialize();


            var options = new string[]
            {
                "--video-filter=adjust",
                "--brightness=1",
                "--saturation=1"
            };
            _libVLC = new LibVLC(options);

            _mediaPlayer = new MediaPlayer(_libVLC);

            videoView.MediaPlayer = _mediaPlayer;
            Media media = new Media(_libVLC, "http://www.quirksmode.org/html5/videos/big_buck_bunny.mp4",
                FromType.FromLocation);
            //media.AddOptionFlag("grayscale", 1);
            //media.AddOption(":grayscale=1");
            _mediaPlayer.Play(media);
            //_mediaPlayer.Play(new Media(_libVLC, "rtsp://192.168.2.39/live.sdp", FromType.FromLocation));


        }
        */
    }
}
