using System.Windows.Forms;

namespace Base
{
    public class StatusService
    {
        private static ToolStripStatusLabel label;

        internal static void SetLabel(ToolStripStatusLabel toolStripStatusLabel1)
        {
            label = toolStripStatusLabel1;
        }

        public static void SetStatus(string v)
        {
            label.Text = v;
        }
    }
}