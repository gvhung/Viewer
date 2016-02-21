// Copyright (c) 2005 Daniel Grunwald
// Licensed under the terms of the "BSD License", see doc/license.txt

using System;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;
using ICSharpCode.Core;
using Base;

namespace Startup
{
	public static class Start
	{
		[STAThread]
		public static void Main(string[] args)
		{
			LoggingService.Info("Application start");
			
			Assembly exe = typeof(Start).Assembly;
			
			FileUtility.ApplicationRootPath = Path.GetDirectoryName(exe.Location);
			
			CoreStartup coreStartup = new CoreStartup("Viewer");

			coreStartup.PropertiesName = "AppProperties";
			coreStartup.StartCoreServices();
			
			ResourceService.RegisterNeutralStrings(new ResourceManager("Startup.StringResources", exe));
			ResourceService.RegisterNeutralImages(new ResourceManager("Startup.ImageResources", exe));

            coreStartup.AddAddInsFromDirectory(FileUtility.ApplicationRootPath);
			
			coreStartup.ConfigureExternalAddIns(Path.Combine(PropertyService.ConfigDirectory, "AddIns.xml"));
			
			coreStartup.ConfigureUserAddIns(Path.Combine(PropertyService.ConfigDirectory, "AddInInstallTemp"),
			                                Path.Combine(PropertyService.ConfigDirectory, "AddIns"));
			
			coreStartup.RunInitialization();
			
			Workbench.InitializeWorkbench(args);
			
			try {
				LoggingService.Info("Running application...");
				
				Application.Run(Workbench.Instance);
			} finally {
				try {
					PropertyService.Save();
				} catch (Exception ex) {
					MessageService.ShowError(ex, "Error storing properties");
				}
			}
		}
	}
}