using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using EnvDTE;

namespace Infosys.Lif.LegacyWorkbench.ReportManager
{
    public class VSEnvironmentDteHandler
    {
         #region Interop imports

        [DllImport("ole32.dll")]
        public static extern int GetRunningObjectTable(int reserved, out UCOMIRunningObjectTable prot);

        [DllImport("ole32.dll")]
        public static extern int CreateBindCtx(int reserved, out UCOMIBindCtx ppbc);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private const int SW_RESTORE = 9;
        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern bool IsIconic(IntPtr hWnd);

        #endregion

        /// <summary>
        /// Get the DTE object for the instance of Visual Studio IDE that has 
        /// the specified solution open.
        /// </summary>
        /// <param name="solutionFile">The absolute filename of the solution</param>
        /// <returns>Corresponding DTE object or null if no such IDE is running</returns>
        public static EnvDTE.DTE GetIDEInstance(string solutionFile)
        {
            Hashtable runningInstances = GetIDEInstances(true);
            IDictionaryEnumerator enumerator = runningInstances.GetEnumerator();

            while (enumerator.MoveNext())
            {
                try
                {
                    _DTE ide = (_DTE)enumerator.Value;
                    if (ide != null)
                    {
                        if (ide.Solution.FullName == solutionFile)
                        {
                            return (EnvDTE.DTE)ide;
                        }
                    }
                }
                catch { }
            }

            return null;
        }

        /// <summary>
        /// Get a table of the currently running instances of the Visual Studio .NET IDE.
        /// </summary>
        /// <param name="openSolutionsOnly">Only return instances that have opened a solution</param>
        /// <returns>A hashtable mapping the name of the IDE in the running object table to the corresponding DTE object</returns>
        public static Hashtable GetIDEInstances(bool openSolutionsOnly)
        {
            Hashtable runningIDEInstances = new Hashtable();
            Hashtable runningObjects = GetRunningObjectTable();

            IDictionaryEnumerator rotEnumerator = runningObjects.GetEnumerator();
            while (rotEnumerator.MoveNext())
            {
                string candidateName = (string)rotEnumerator.Key;
                if (!candidateName.StartsWith("!VisualStudio.DTE"))
                    continue;

                _DTE ide = rotEnumerator.Value as _DTE;

                if (ide == null)
                    continue;

                if (openSolutionsOnly)
                {
                    try
                    {
                        string solutionFile = ide.Solution.FullName;
                        if (solutionFile != String.Empty)
                        {
                            runningIDEInstances[candidateName] = ide;
                        }
                    }
                    catch { }
                }
                else
                {
                    runningIDEInstances[candidateName] = ide;
                }
            }
            return runningIDEInstances;
        }

        /// <summary>
        /// Get a snapshot of the running object table (ROT).
        /// </summary>
        /// <returns>A hashtable mapping the name of the object in the ROT to the corresponding object</returns>
        [STAThread]
        public static Hashtable GetRunningObjectTable()
        {
            Hashtable result = new Hashtable();

            int numFetched;
            UCOMIRunningObjectTable runningObjectTable;
            UCOMIEnumMoniker monikerEnumerator;
            UCOMIMoniker[] monikers = new UCOMIMoniker[1];

            GetRunningObjectTable(0, out runningObjectTable);
            runningObjectTable.EnumRunning(out monikerEnumerator);
            monikerEnumerator.Reset();

            while (monikerEnumerator.Next(1, monikers, out numFetched) == 0)
            {
                UCOMIBindCtx ctx;
                CreateBindCtx(0, out ctx);

                string runningObjectName;
                monikers[0].GetDisplayName(ctx, null, out runningObjectName);

                object runningObjectVal;
                runningObjectTable.GetObject(monikers[0], out runningObjectVal);

                result[runningObjectName] = runningObjectVal;
            }

            return result;
        }


    }



    }

