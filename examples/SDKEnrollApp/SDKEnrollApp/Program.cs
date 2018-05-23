using System;
using System.Threading;
using System.Windows.Forms;

namespace SDKEnrollApp
{


    static class Program
    {

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            Mutex mutex = new Mutex(false, "MyUniqueMutexName");
            try
            {
                if (mutex.WaitOne(0, false))
                {


                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new MainForm());
                }
                else
                {
                    MessageBox.Show("An instance of SDKEnrollApp is already running.");
                } 
            }
            finally
            {
                if (mutex != null)
                {
                    mutex.Close();
                    mutex = null;
                }
            }
        }
    }
}


