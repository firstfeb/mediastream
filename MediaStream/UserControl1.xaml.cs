using INSM.Template.Framework.v1;
using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using LibVLCSharp.WPF;

namespace MediaStream
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : ITemplateControl
    {
        LibVLC _libVLC;
        MediaPlayer _mediaPlayer;
        private Template _mTemplate;
        private const string KeyTvUrl = "TvUrl";

        public UserControl1()
        {
            InitializeComponent();
            _mTemplate = new Template(new TemplateDataSet("Default template dataset", "",
                new TemplateDataSetItem(TemplateDataSetItemType.Text, KeyTvUrl, "", ""
                )));

            _mTemplate.TemplateDataSet.TemplateDataSetChanged += TemplateDataSet_TemplateDataSetChanged;
            _mTemplate.TemplatePlaying += m_Template_TemplatePlaying;
            _mTemplate.TemplateStopped += m_Template_TemplateStopped;
            _mTemplate.TemplateUnload += m_Template_TemplateUnload;
            _mTemplate.TemplatePaused += m_Template_TemplatePaused;
        }

        private void m_Template_TemplatePaused(object sender, PausedEventArgs e)
        {
            _mTemplate.Debug(DebugLevel.High, DebugCategory.Error, "MediaStream: Paused");
        }

        private void m_Template_TemplateUnload(object sender, UnloadEventArgs e)
        {
            _mTemplate.Debug(DebugLevel.High, DebugCategory.Error, "MediaStream: Unload");
            _mTemplate.TemplateDataSet.TemplateDataSetChanged -= TemplateDataSet_TemplateDataSetChanged;
            _mTemplate.TemplatePlaying -= m_Template_TemplatePlaying;
            _mTemplate.TemplateStopped -= m_Template_TemplateStopped;
            _mTemplate.TemplateUnload -= m_Template_TemplateUnload;
            _mTemplate.TemplatePaused -= m_Template_TemplatePaused;
        }

        private void m_Template_TemplateStopped(object sender, StoppedEventArgs e)
        {
            _mTemplate.Debug(DebugLevel.High, DebugCategory.Error, "MediaStream: Stopped");
        }

        private void m_Template_TemplatePlaying(object sender, PlayingEventArgs e)
        {
            _mTemplate.Debug(DebugLevel.High, DebugCategory.Error, "MediaStream: Playing");
        }

        private void TemplateDataSet_TemplateDataSetChanged(TemplateDataSetEventArgs e)
        {
            _mTemplate.Debug(DebugLevel.High, DebugCategory.Error, "MediaStream: DatasetChanged");
            this.LoadVideo();
        }

        private void LoadVideo()
        {
            try
            {
                Core.Initialize();

                var options = new string[]
                {
                    "--video-filter=adjust",
                    "--brightness=1",
                    "--saturation=1",
                    "--verbose=2"
                };
                _libVLC = new LibVLC(options);
                
                _mediaPlayer = new MediaPlayer(_libVLC);
                _mediaPlayer.SetVideoTitleDisplay(Position.Disable, 0);
                _mediaPlayer.EncounteredError += this.OnMediaError;
                _mediaPlayer.EndReached += this.OnMediaEnded;
                _mediaPlayer.Stopped += this.OnMediaStopped;
                
                videoView.MediaPlayer = _mediaPlayer;

                String url = "rtsp://wowzaec2demo.streamlock.net/vod/mp4:BigBuckBunny_115k.mov";
                Media media = new Media(_libVLC, url, FromType.FromLocation);

                media.StateChanged += this.MediaStateChanged;
                _mTemplate.Debug(DebugLevel.High, DebugCategory.Error, "Mediastream: Play URL:" + url);
                _mediaPlayer.Play(media);
            }
            catch (Exception ex)
            {
                _mTemplate.Debug(DebugLevel.High, DebugCategory.Error, "Mediastream: Exception caught when loading/running VLC");
                _mTemplate.Debug(DebugLevel.High, DebugCategory.Error, "Mediastream: " + ex.Message);
            }
        }

        private void OnMediaStopped(object sender, EventArgs e)
        {
            _mTemplate.Debug(DebugLevel.High, DebugCategory.Error, "Mediastream: VLC Media stopped.");
            _mTemplate.Debug(DebugLevel.High, DebugCategory.Error, "Mediastream: " + _mediaPlayer.State);
        }

        private void MediaStateChanged(object sender, MediaStateChangedEventArgs e)
        {
            _mTemplate.Debug(DebugLevel.High, DebugCategory.Error, "Mediastream: VLC Media state changed.");
            _mTemplate.Debug(DebugLevel.High, DebugCategory.Error, "Mediastream: " + _mediaPlayer.Media.State);
        }

        private void OnMediaEnded(object sender, EventArgs e)
        {
            _mTemplate.Debug(DebugLevel.High, DebugCategory.Error, "Mediastream: VLC Media ended");
            _mTemplate.Debug(DebugLevel.High, DebugCategory.Error, "Mediastream: " + _mediaPlayer.State);
        }

        private void OnMediaError(object sender, EventArgs e)
        {
            _mTemplate.Debug(DebugLevel.High, DebugCategory.Error, "Mediastream: VLC Exception");
            _mTemplate.Debug(DebugLevel.High, DebugCategory.Error, "Mediastream: " + e.ToString());
            _mTemplate.Debug(DebugLevel.High, DebugCategory.Error, "Mediastream: Last VLC lib error: " + _libVLC.LastLibVLCError);
            _mTemplate.Debug(DebugLevel.High, DebugCategory.Error, "Mediastream: _mediaPlayer.State: " + _mediaPlayer.State);
        }

        public ITemplate TemplateInstance
        {
            get { return _mTemplate; }
        }
    }
}
