// Copyright (c) 2005 Daniel Grunwald
// Licensed under the terms of the "BSD License", see doc/license.txt

using System;
using System.Windows.Forms;
using ICSharpCode.Core;

namespace Base
{	
	public class OpenFileCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			Workbench workbench = (Workbench)this.Owner;
			if (workbench.CloseCurrentContent()) {
				using (OpenFileDialog dlg = new OpenFileDialog()) {
					dlg.CheckFileExists = true;
					dlg.DefaultExt = ".txt";
					dlg.Filter = FileViewContent.GetFileFilter("/Workspace/FileFilter");
					if (dlg.ShowDialog() == DialogResult.OK) {
						StatusService.SetStatus("Loading "+ dlg.FileName + "... ");
						IViewContent content = DisplayBindingManager.CreateViewContent(dlg.FileName);
						if (content != null) {
							workbench.ShowContent(content);
						}
						StatusService.SetStatus("Fertig ");
					}
				}
			}
		}
	}
	
	
	public class ExitCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			Workbench workbench = (Workbench)this.Owner;
			if (workbench.CloseCurrentContent()) {
				workbench.Close();
			}
		}
	}
}