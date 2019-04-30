using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using INSM.Template.Framework.v1;
using LibVLCSharp.Shared;
using LibVLCSharp.WinForms;

namespace MediaStreamer
{

    public partial class MediaStreamer : ITemplateControl
    {
        private LibVLC _libVLC;
        private MediaPlayer _mediaPlayer;
        private Template _template;
        private const string KeyTvUrl = "TvUrl";

        private WindowsFormsHost _host = new WindowsFormsHost();
        private VideoView _videoView;

        private string _streamUrl = string.Empty;

        public MediaStreamer()
        {
            Core.Initialize();

            InitializeComponent();

            _template = new Template(new TemplateDataSet("Default template dataset", "",
                new TemplateDataSetItem(TemplateDataSetItemType.Text, KeyTvUrl, "", ""
                )));

            _template.TemplateDataSet.TemplateDataSetChanged += TemplateDataSet_TemplateDataSetChanged;
            _template.TemplatePlaying += _template_TemplatePlaying;
            _template.TemplateStopped += _template_TemplateStopped;
            _template.TemplatePaused += _template_TemplatePaused;
            _template.TemplateUnload += _template_TemplateUnload;

            _libVLC = new LibVLC();
            _mediaPlayer = new MediaPlayer(_libVLC);
            _videoView = new VideoView();
            _videoView.MediaPlayer = _mediaPlayer;
            _host.Child = _videoView;

            VlcGrid.Children.Add(_host);
        }

        private void TemplateDataSet_TemplateDataSetChanged(TemplateDataSetEventArgs e)
        {
            _streamUrl = _template.TemplateDataSet.GetTemplateDataSetItemAsText(KeyTvUrl, string.Empty);
        }

        private void _template_TemplatePlaying(object sender, PlayingEventArgs e)
        {
            if (!string.IsNullOrEmpty(_streamUrl) && !_mediaPlayer.IsPlaying)
            {
                Media media = new Media(_libVLC, _streamUrl, FromType.FromLocation);
                _mediaPlayer.Play(media);
            }
        }

        private void _template_TemplateStopped(object sender, StoppedEventArgs e)
        {
            if (_mediaPlayer.IsPlaying)
            {
                _mediaPlayer.Stop();
            }
        }

        private void _template_TemplatePaused(object sender, PausedEventArgs e)
        {
            if (_mediaPlayer.IsPlaying)
            {
                _mediaPlayer.Pause();
            }
        }

        private void _template_TemplateUnload(object sender, UnloadEventArgs e)
        {
            _template.TemplateDataSet.TemplateDataSetChanged -= TemplateDataSet_TemplateDataSetChanged;
            _template.TemplatePlaying -= _template_TemplatePlaying;
            _template.TemplateStopped -= _template_TemplateStopped;
            _template.TemplatePaused -= _template_TemplatePaused;
            _template.TemplateUnload -= _template_TemplateUnload;
        }

        public ITemplate TemplateInstance
        {
            get { return _template; }
        }
    }
}
