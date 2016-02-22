using Base;
using System.IO;
using System.Windows.Forms;
using Telerik.WinControls;
using System.Linq;
using System.Collections.ObjectModel;

namespace TsspViewer
{
    public class DisplayBinding : IDisplayBinding
    {
        public IViewContent OpenFile(string fileName)
        {
            string extension = Path.GetExtension(fileName).ToLowerInvariant();
            switch (extension)
            {
                case ".tssp": return new XmlViewContent(fileName);
            }

            return null;
        }
    }

    public class XmlViewContent : FileViewContent
    {
        Control textBox = new Form();

        public XmlViewContent()
        {

        }

        public XmlViewContent(string fileName) : this()
        {
            ThemeResolutionService.LoadPackageFile(fileName);

            ThemeResolutionService.ApplicationThemeName = ThemeRepository.AvailableThemeNames.ToList()[0];

            var frm = new MainWindow();
            frm.TopLevel = false;

            textBox = frm;

            frm.Show();
        }

        public override Control Control => textBox;
    }
}