using System;
using System.Collections;
using System.Collections.Generic;

using System.Text;

namespace Infosys.Lif.LegacyWorkbench
{
    internal static class Helper
    {
        internal static object CreateInstanceOf(string assemblyPath, string fullType)
        {
            System.Runtime.Remoting.ObjectHandle objHandle = Activator.CreateInstanceFrom( assemblyPath, fullType);
            object unwrappedObject = objHandle.Unwrap();

            return unwrappedObject;
        }
        //this method will append .txt
        internal static string apendtxt(string name)
        {
            return (name + ".txt");

        }
        internal static Hashtable RetrievedTemplates = new Hashtable();
    }
}
