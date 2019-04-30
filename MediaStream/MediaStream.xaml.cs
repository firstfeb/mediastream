using INSM.Template.Framework.v1;
using System;
using System.IO;
using System.Windows.Forms.Integration;
using Vlc.DotNet.Forms;

namespace MediaStream
{
    public partial class MediaStream : ITemplateControl
    {
        private Template _mTemplate;
        private const string KeyTvUrl = "TvUrl";
        private WindowsFormsHost windowsFormsHost;
        private VlcControl vlcControl;
        private string streamUrl;

        public MediaStream()
        {
            InitializeComponent();
            windowsFormsHost = new WindowsFormsHost();
            vlcControl = new VlcControl();

            windowsFormsHost.Child = vlcControl;
            VlcGrid.Children.Add(windowsFormsHost);

            _mTemplate = new Template(new TemplateDataSet("Default template dataset", "",
                new TemplateDataSetItem(TemplateDataSetItemType.Text, KeyTvUrl, "", ""
                )));

            _mTemplate.TemplateDataSet.TemplateDataSetChanged += TemplateDataSet_TemplateDataSetChanged;
            _mTemplate.TemplatePlaying += m_Template_TemplatePlaying;
            _mTemplate.TemplateStopped += m_Template_TemplateStopped;
            _mTemplate.TemplatePaused += m_Template_TemplatePaused;
            _mTemplate.TemplateUnload += m_Template_TemplateUnload;
        }

        private void TemplateDataSet_TemplateDataSetChanged(TemplateDataSetEventArgs e)
        {
            streamUrl = _mTemplate.TemplateDataSet.GetTemplateDataSetItemAsText(KeyTvUrl, string.Empty);
            var templatePath = _mTemplate.Attributes["TemplatePath"];
            var libPath = Path.Combine(templatePath, "libvlc", IntPtr.Size == 4 ? "win-x86" : "win-x64");
            var libDirectory = new DirectoryInfo(libPath);

            vlcControl.BeginInit();
            vlcControl.VlcLibDirectory = libDirectory;
            vlcControl.EndInit();
        }

        private void m_Template_TemplatePlaying(object sender, PlayingEventArgs e)
        {
            if (!string.IsNullOrEmpty(streamUrl) && !vlcControl.IsPlaying)
            {
                vlcControl.Play(new Uri(streamUrl));
            }
        }

        private void m_Template_TemplateStopped(object sender, StoppedEventArgs e)
        {
            if (vlcControl.IsPlaying)
            {
                vlcControl.Stop();
            }
        }

        private void m_Template_TemplatePaused(object sender, PausedEventArgs e)
        {
            if (vlcControl.IsPlaying)
            {
                vlcControl.Pause();
            }
        }

        private void m_Template_TemplateUnload(object sender, UnloadEventArgs e)
        {
            _mTemplate.TemplateDataSet.TemplateDataSetChanged -= TemplateDataSet_TemplateDataSetChanged;
            _mTemplate.TemplatePlaying -= m_Template_TemplatePlaying;
            _mTemplate.TemplateStopped -= m_Template_TemplateStopped;
            _mTemplate.TemplateUnload -= m_Template_TemplateUnload;
            _mTemplate.TemplatePaused -= m_Template_TemplatePaused;
        }

        public ITemplate TemplateInstance
        {
            get { return _mTemplate; }
        }
    }
}
