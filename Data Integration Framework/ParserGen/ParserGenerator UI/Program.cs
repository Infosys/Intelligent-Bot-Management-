using System;
using System.Collections.Generic;
using System.Windows.Forms;

/****************************************************************
 * This file is a part of the Legacy Parser utility.
 * This file provides the Program class which starts the GUI 
 * Copyright (c) 2003 - 2005 Infosys Technologies Ltd. All Rights Reserved.
 ***************************************************************/
namespace Infosys.Lif.LegacyParser.UI
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();            
			Application.Run(new LegacyParserFrontEnd());
		}
	}
}