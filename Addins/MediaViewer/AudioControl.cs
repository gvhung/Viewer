using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Controls;
using System.IO;

namespace MediaViewer
{
    public partial class AudioControl : System.Windows.Forms.UserControl
    {
        public MediaElement player = new MediaElement();

        public AudioControl(string filename)
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
