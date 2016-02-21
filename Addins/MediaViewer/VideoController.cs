using System;
using System.Windows.Controls;

namespace MediaViewer
{
    public partial class VideoControl : System.Windows.Forms.UserControl
    {
        public MediaElement player = new MediaElement();

        public VideoControl(string filename)
        {
            InitializeComponent();

            elementHost1.Child = player;

            player.Source = new Uri(filename);
            player.LoadedBehavior = MediaState.Manual;
        }

        bool playing = false;
        private void button1_Click(object sender, EventArgs e)
        {
            if (!playing)
            {
                player.Play();
                playing = true;
                button1.Text = "Pause";
            }
            else
            {
                player.Pause();
                playing = false;
                button1.Text = "Play";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            player.Stop();
        }
    }
}
