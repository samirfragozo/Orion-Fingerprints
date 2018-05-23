/**  You are free to use this example code to generate similar functionality  
**  tailored to your own specific needs.  
**
**  This example code is provided by HID Global Corporation for illustrative purposes only.  
**  This example has not been thoroughly tested under all conditions.  Therefore 
**  Lumidigm Inc. cannot guarantee or imply reliability, serviceability, or 
**  functionality of this program.
**
**  This example code is provided by HID Global Corporation "AS IS" and without any express 
**  or implied warranties, including, but not limited to the implied warranties of 
**  merchantability and fitness for a particular purpose.
 
 * */

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Reflection;

namespace CSharpExample
{

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 

        // Use kernel32.dll to load the LumiAPI.dll manually.
        // This gets around the issue of the DllImport search
        // path rule that prevents relative paths.
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr LoadLibrary(string lpFileName);

        [STAThread]
        static void Main()
        {
            LoadLumiSDK();
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ImageFrm());
        }

        // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        //
        // IMPORTANT - READ THIS
        //
        // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        // This method Loads the Lumidigm SDK.  It assumes that this CSharpExample solution is being built and executed 
        // within the default Lumidigm SDK install directory structure.  It also assumes the Lumidgim SDK dll's and
        // default installed folder names and locations HAVE NOT changed.  If the CSharpExample solution is not executed 
        // in the default install directory OR if you have modified the default install location or folder names of the 
        // Lumidigm SDK, you MUST modify this method to load the LumiAPI.dll using a different string. 
        static void LoadLumiSDK()
        {           
            //Environment.CurrentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
       
            //string strCurDir = Environment.CurrentDirectory;

            //int nStartIndex = strCurDir.IndexOf("SDK");

            //string strSubStr = strCurDir.Substring(nStartIndex);

            //int nFirstBackslashIndex = strSubStr.IndexOf("\\");

            //string strSDKForderName = strCurDir.Substring(nStartIndex, nFirstBackslashIndex);

            //string strLumiAPIDLL = strCurDir.Substring(0, nStartIndex) + strSDKForderName + "\\bin\\LumiAPI.dll";

            //IntPtr pDll = IntPtr.Zero;

            //pDll = LoadLibrary(strLumiAPIDLL);


            // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            // Simple change to run this example if you are not running this example from within the Lumidigm SDK
            // default install directory OR if you have changed the folder names of the default install directory.
            // Comment out all of the LoadLumiSDK function code above. Then uncomment the two lines below.  Modify
            // the [Your full path to Lumi SDK] to the full path of the folder where you put the Lumidigm SDK.
            
            // IntPtr pDll = IntPtr.Zero;
            // pDll = LoadLibrary("Your full path to Lumi SDK\\LumiAPI.dll");
           
        }

    }
}

