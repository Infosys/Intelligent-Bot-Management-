using System;
using System.Collections.Generic;
using System.Text;
/****************************************************************
 * This file is a part of the Legacy Parser utility.
 * This class is the excetpion type raised when an exception is encountered.
 * Copyright (c) 2003 - 2005 Infosys Technologies Ltd. All Rights Reserved.
 ***************************************************************/

namespace Infosys.Lif.LegacyParser.UI
{
    class LegacyParserException:ApplicationException {
        public LegacyParserException(string message) : base(message) { }
    }
}
