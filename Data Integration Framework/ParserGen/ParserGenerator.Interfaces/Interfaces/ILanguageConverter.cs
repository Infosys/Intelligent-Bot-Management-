using System;
using System.Collections.Generic;
using System.Text;
/****************************************************************
 * This file is a part of the Legacy Parser utility.
 * This interface defines how a language converter should interface 
 * with the Legacy Parser tool. This defines the converter used 
 * to convert the mainframe data entities definition to .NET entities.
 * Copyright (c) 2003 - 2005 Infosys Technologies Ltd. All Rights Reserved.
 ***************************************************************/

namespace Infosys.Lif.LegacyParser.Interfaces
{
    /// <summary>
    /// This interface defines the manner in which the language conversion 
    /// will take place. The language conversion refers to conversion 
    /// of the mainframe copy books to the legacy parser specific .NET entity.
    /// </summary>
	public interface ILanguageConverter
	{
		/// <summary>
		/// This will be used to translate an object to the Parser Generater 
		/// Input Entity.
		/// </summary>
        /// <param name="pathOfCobolCopyBook">
		/// This is the path to the copy book which has to be translated.
		/// Usually this is a file path, The implementor will have to decide 
		/// as to what has to be passed.
		/// </param>
		/// <returns>The Legacy parser specific entity 
        /// type generated from the input copy books.</returns>
		Entity Translate(string pathOfCopyBook);
	}
}
