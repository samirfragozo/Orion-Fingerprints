/******************************************************************************
**  You are free to use this example code to generate similar functionality  
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
**
**	
**	Author: RMcKee
**
**	Copyright 2016 HID Global Corporation/ASSA ABLOY AB. ALL RIGHTS RESERVED.
**
**  In order to capture with the preview callback image, we need to make sure that the 
**  main window's message queue is not blocked by a call to capture because we want to display
**  the preview image that comes back from the callback and handle the callback appropriately in the main window.
**  Therefore we make the calls to perform the two captures and match in it's own thread.
********************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using SDKWrapper;

namespace CSharpExample
{
    class MatchProcess
    {
        // Reference to main form used to make syncronous user interface calls
        ImageFrm m_form;

        public MatchProcess(ImageFrm form)
		{
			m_form = form;
		}

        public void Run()
        {
            try
            {
                uint width = 0, height = 0;

                SDKBiometrics.GetWidthAndHeight(ref width, ref height);

                byte[] snapShot24bpp = new byte[width * height * 3]; // multiply by 3 to get 24bppRgb format               
                byte[] template = new byte[5000]; //array to hold the template
                byte[] template2 = new byte[5000]; //array to hold the 2ed template
                uint templateLen = 0;
                uint templateLen2 = 0;
           
                // Assign the main form's PresenceDetectionCallback method to the LumiPresenceDetectCallbackDelegate
                LumiSDKWrapper.LumiPresenceDetectCallbackDelegate del = new LumiSDKWrapper.LumiPresenceDetectCallbackDelegate(m_form.PresenceDetectionCallback);

                m_form.Invoke(m_form.m_DelegateWriteResults, new object[] { ("Put finger one down...") });

                SDKBiometrics.LumiCaptureWithPresenceDetectionFeedback(snapShot24bpp, template, ref templateLen, width, height, del);

                m_form.Invoke(m_form.m_DelegateSetImage, new object[] { snapShot24bpp, (uint)width, (uint)height });

                m_form.Invoke(m_form.m_DelegateWriteResults, new object[] { ("Put finger two down...") });

                SDKBiometrics.LumiCaptureWithPresenceDetectionFeedback(snapShot24bpp, template2, ref templateLen2, width, height, del);

                m_form.Invoke(m_form.m_DelegateSetImage, new object[] { snapShot24bpp, (uint)width, (uint)height });

                uint score = 0;
                int spoof = 0;
                SDKBiometrics.Match(template, ref templateLen, template2, ref templateLen2, ref score, ref spoof);
                m_form.Invoke(m_form.m_DelegateWriteResults, new object[] { (SDKBiometrics.StatusMessage) });
                GC.KeepAlive(del);
                                
            }
            catch (Exception err)
            {
                m_form.Invoke(m_form.m_DelegateWriteResults, new object[] { (err.ToString()) });
            }

            m_form.Invoke(m_form.m_CaptureWithPDFinished, null);
            m_form.Invoke(m_form.m_MatchThreadFinished, null);
        } 
    }
}
