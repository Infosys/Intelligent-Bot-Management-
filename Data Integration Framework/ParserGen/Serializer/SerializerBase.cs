using System;
using System.Collections.Generic;
using System.Text;

using Infosys.Lif.LegacyParameters;
/****************************************************************
 * This file is a part of the Legacy Parser utility.
 * This file provides the base class for the serializers.
 * Copyright (c) 2003 - 2005 Infosys Technologies Ltd. All Rights Reserved.
 ***************************************************************/

namespace Infosys.Lif.LegacyParser.Framework
{
    public abstract class SerializerBase
    {
        public abstract string Serialize(object dataEntity);
        public abstract object Deserialize(string parseString);


        protected string Pad(int integerToBePadded, int padLength,
            string padString, bool padOnRight)
        {
            // Integer type can never be null, so no null checks placed here.
            string stringToBePadded = integerToBePadded.ToString();
            return Pad(stringToBePadded, padLength, padString, padOnRight);
        }


        protected string Pad(string stringToBePadded, int padLength,
            string padString, bool padOnRight)
        {
            if (stringToBePadded == null)
            {
                stringToBePadded = string.Empty;
            }
            if (stringToBePadded.Length < padLength)
            {
                int lengthOfPad = padLength - stringToBePadded.Length;
                lengthOfPad = lengthOfPad / padString.Length;
                string pad = padString;
                for (int padCounter = 1; padCounter < lengthOfPad; padCounter++)
                {
                    pad += padString;
                }
                if (padOnRight)
                {
                    stringToBePadded = stringToBePadded + pad;
                }
                else
                {
                    stringToBePadded = pad + stringToBePadded;
                }
            }
            return stringToBePadded;
        }
        public virtual RequestParameters Parameters
        {
            get
            {
                return parameters;
            }
            set
            {
                parameters = value;
            }
        }
        protected RequestParameters parameters = null;
    }
}

