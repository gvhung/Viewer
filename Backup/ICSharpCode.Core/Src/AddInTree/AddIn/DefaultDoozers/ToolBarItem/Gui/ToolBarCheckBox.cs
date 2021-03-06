﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 915 $</version>
// </file>

using System;
using System.Drawing;
using System.Diagnostics;
using System.Drawing.Text;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace ICSharpCode.Core
{
	public class ToolBarCheckBox : ToolStripButton , IStatusUpdate
	{
		object caller;
		Codon  codon;
		string description   = String.Empty;
		ICheckableMenuCommand menuCommand = null;
		
		public ICheckableMenuCommand MenuCommand {
			get {
				return menuCommand;
			}
		}
		
		public string Description {
			get {
				return description;
			}
			set {
				description = value;
			}
		}
		
		public ToolBarCheckBox(string text)
		{
			this.RightToLeft = RightToLeft.Inherit;
			Text = text;
		}
		
		public ToolBarCheckBox(Codon codon, object caller)
		{
			this.RightToLeft = RightToLeft.Inherit;
			this.caller = caller;
			this.codon  = codon;
			try {
				menuCommand = (ICheckableMenuCommand)codon.AddIn.CreateObject(codon.Properties["class"]);
			} catch (Exception) {
			}
			if (menuCommand == null) {
				MessageService.ShowError("Can't create toolbar checkbox : " + codon.Id);
			}
			
			if (Image == null && codon.Properties.Contains("icon")) {
				Image = ResourceService.GetBitmap(codon.Properties["icon"]);
			}
			UpdateText();
			UpdateStatus();
		}
		
		protected override void OnClick(System.EventArgs e)
		{
			base.OnClick(e);
			if (menuCommand != null) {
				menuCommand.Run();
				Checked = menuCommand.IsChecked;
			}
		}
		
		public override bool Enabled {
			get {
				if (codon == null) {
					return base.Enabled;
				}
				ConditionFailedAction failedAction = codon.GetFailedAction(caller);
				return failedAction != ConditionFailedAction.Disable;
			}
		}
		
		public virtual void UpdateStatus()
		{
			if (codon != null) {
				ConditionFailedAction failedAction = codon.GetFailedAction(caller);
				bool isVisible = failedAction != ConditionFailedAction.Exclude;
				if (isVisible != Visible)
					Visible = isVisible;
				if (menuCommand != null) {
					bool isChecked = menuCommand.IsChecked;
					if (isChecked != Checked)
						Checked = isChecked;
				}
			}
		}
		
		public virtual void UpdateText()
		{
			Text = StringParser.Parse(codon.Properties["label"]);
			if (codon.Properties.Contains("tooltip")) {
				ToolTipText = StringParser.Parse(codon.Properties["tooltip"]);
			}
		}
	}
}
