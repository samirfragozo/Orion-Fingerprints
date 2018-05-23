
/***************************************************************************************/
// ©Copyright 2016 HID Global Corporation/ASSA ABLOY AB. ALL RIGHTS RESERVED.
//
// For a list of applicable patents and patents pending, visit www.hidglobal.com/patents/
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS 
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER 
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN 
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
//
/***************************************************************************************/

using System;
using System.Runtime.InteropServices;
using SDKWrapper;

namespace SDKEnrollApp
{
    class Verify
    {
        MainForm m_form;
        private LumiSDKWrapper.LumiStatus _rc;
        private string _statusMessage;
        private uint _hHandle;

        public Verify(MainForm form)
        {
            m_form = form;
            Sensor sensorid = (Sensor)m_form.SensorList[(int)m_form.m_nSelectedSensorID];
            _hHandle = sensorid.handle;
        }

        public void Run()
        {
            
            uint width = 0, height = 0;
            int nSpoof = 0;
            GetWidthAndHeight(ref width, ref height);
            byte[] snapShot24bpp = new byte[width * height * 3]; // multiply by 3 to get 24bppRgb format
            byte[] template1 = new byte[5000]; //array to hold the template
            uint templateLen1 = 0;
            uint nMatchScore1 = 0;
            int nSpoofScore = 0;
            LumiSDKWrapper.LumiPresenceDetectCallbackDelegate del = m_form.PresenceDetectionCallback;
            m_form.Invoke(m_form.m_DelegateErrorMessage, "Place finger down for verification", m_form.Blue);

            ////////////////////////////
            LumiSDKWrapper.LUMI_PROCESSING_MODE processingMode;
            processingMode.bExtract = 0;
            processingMode.bLatent = 0;
            processingMode.bSpoof = 0;
            LumiSDKWrapper.LumiStatus pStatus = LumiSDKWrapper.GetProcessingMode(_hHandle, ref processingMode);
            if (pStatus != LumiSDKWrapper.LumiStatus.LUMI_STATUS_OK)
            {
                m_form.Invoke(m_form.m_DelegateErrorMessage, "Enrollment failed. Unable to Set the Spoof detection mode", m_form.Red);
                return;
            }
            processingMode.bSpoof = m_form.m_bSpoofEnabled ? (byte) 1 : (byte) 0;

            pStatus = LumiSDKWrapper.SetProcessingMode(_hHandle, processingMode);
            if (pStatus != LumiSDKWrapper.LumiStatus.LUMI_STATUS_OK)
            {
                m_form.Invoke(m_form.m_DelegateErrorMessage, "Enrollment failed. Unable to Set the Spoof detection mode", m_form.Red);
                return;
            }
            //////////////////////////////////

            if (EnrollAndDisplay(snapShot24bpp, template1, ref templateLen1, width, height, ref nSpoof, del) == false)
            {
                m_form.Invoke(m_form.m_VerifyThreadFinished);
                return;
            }
            
            if (m_form.m_bClosingApp)
            {
                CloseScanner();
                return;
            }

            /*int index;
            index = m_form.m_currentSubject.fingers.IndexOf(m_form.m_CurrentHotSpot);
            uint templatelength = (uint)m_form.m_currentSubject.templateLengths[index];

            LumiSDKWrapper.LumiMatch(_hHandle, template1, ref templateLen1, (byte[])m_form.m_currentSubject.templates[index], ref templatelength, ref nMatchScore1, ref nSpoofScore);
            if (m_form.m_bSpoofEnabled&&(nSpoof != -1))
            {
                if (nMatchScore1 > m_form.m_MatchThreshold && nSpoof <= m_form.m_SpoofThreshold)
                {
                    m_form.Invoke(m_form.m_DelegateErrorMessage, "Match Score = " + nMatchScore1 + "\nSpoof Score = " + nSpoof + "\nVerification Successful", m_form.Blue);
                }

                else
                {
                    m_form.Invoke(m_form.m_DelegateErrorMessage, "Match Score = " + nMatchScore1 + "\nSpoof Score = " + nSpoof + "\nVerification Failed", m_form.Red);

                }
            }
            else
            {
                if (nMatchScore1 > m_form.m_MatchThreshold)
                {
                    m_form.Invoke(m_form.m_DelegateErrorMessage, "Match Score = " + nMatchScore1 + "\nVerification Successful", m_form.Blue);
                }

                else
                {
                    m_form.Invoke(m_form.m_DelegateErrorMessage, "Match Score = " + nMatchScore1 + "\nVerification Failed", m_form.Red);

                }
            }*/


            m_form.Invoke(m_form.m_VerifyThreadFinished);
            
        }

        public void CloseScanner()
        {
            try
            {
                _rc = LumiSDKWrapper.LumiClose(_hHandle);
                _rc = LumiSDKWrapper.LumiExit();
            }
            catch (Exception err)
            {
                _statusMessage += "An Error occured: " + err + "  _rc = " + _rc + "\r\n";
                throw err;
            }
            finally { }
        }


        public void ConvertRawImageTo24bpp(byte[] snapShot24bppPointer, byte[] snapshotPointer, int snapShotLength)
        {
            int innerOffset = 0;
            for (int offset = 0; offset < snapShotLength; offset++)
            {
                for (int counter = 1; counter <= 3; counter++)
                {
                    snapShot24bppPointer[innerOffset] = snapshotPointer[offset];
                    innerOffset += 1;
                }
            }
        }

        public void GetWidthAndHeight(ref uint width, ref uint height)
        {
            try
            {
                uint nBPP = 0, nDPI = 0;
                _rc = LumiSDKWrapper.LumiGetImageParams(_hHandle, ref width, ref height, ref nBPP, ref nDPI);
                if (_rc != LumiSDKWrapper.LumiStatus.LUMI_STATUS_OK)
                {
                    throw new Exception("FAIL: lumiGetImageSize");
                }

                _statusMessage += "PASS: Image Size = " + width + " X " + height + "\r\n";

            }
            catch (Exception err)
            {
                _statusMessage += "An Error occured: " + err + "\r\n";
                throw err;
            }
            finally { }
        }

        public LumiSDKWrapper.LumiStatus EnrollImage(byte[] snapShot, byte[] snapShot24bppPointer, byte[] template1Pointer, ref uint templateSize, uint width, uint height, ref int spoof, LumiSDKWrapper.LumiPresenceDetectCallbackDelegate callbackFunc, ref uint nNFIQ)
        {
            try
            {
                //byte[] snapShot = new byte[width * height]; // Array to hold raw data 8 bpp format 

                // Make sure Presence Detection is on.
                // Because we need another definition for LumiSetOption, we call the SetPresenceDetectionMode
                // static method on the object LumiSDKLumiSetOption.  This is because the LumiSDKWrapper
                // has the LumiSetOption defined to take the LumiPresenceDetectCallbackDelegate argument while for
                // setting PD mode, we need it to take an integer pointer argument instead.
                if (m_form.m_bSensorTriggerArmed)
                {
                    LumiSDKLumiSetOption.SetPresenceDetectionMode(_hHandle, LumiSDKWrapper.LUMI_PRES_DET_MODE.LUMI_PD_ON);
                }
                else
                {
                    LumiSDKLumiSetOption.SetPresenceDetectionMode(_hHandle, LumiSDKWrapper.LUMI_PRES_DET_MODE.LUMI_PD_OFF);
                }

                // Set the address of the presence detection callback function   
                IntPtr prtDel = Marshal.GetFunctionPointerForDelegate(callbackFunc);
                _rc = LumiSDKWrapper.LumiSetOption(_hHandle, LumiSDKWrapper.LUMI_OPTIONS.LUMI_OPTION_SET_PRESENCE_DET_CALLBACK, prtDel, (uint)IntPtr.Size);
                if (_rc != LumiSDKWrapper.LumiStatus.LUMI_STATUS_OK)
                {
                    return _rc;

                }
                _rc = LumiSDKWrapper.LumiSetDCOptions(_hHandle, m_form.m_debugFolder, 0);
                if (_rc != LumiSDKWrapper.LumiStatus.LUMI_STATUS_OK)
                {
                    return _rc;

                }
                _rc = LumiSDKWrapper.LumiCaptureEx(_hHandle, snapShot, template1Pointer, ref templateSize, ref spoof, null);


                if (_rc != LumiSDKWrapper.LumiStatus.LUMI_STATUS_OK)
                {
                    return _rc;
                    
                }
                else
                {
                    _statusMessage += "PASS: lumiCaptureProcess rc = " + _rc + " spoof = " + spoof + "\r\n";
                }

                GetNISTScore(snapShot, ref nNFIQ);


                LumiSDKWrapper.LumiSetOption(_hHandle, LumiSDKWrapper.LUMI_OPTIONS.LUMI_OPTION_SET_PRESENCE_DET_CALLBACK, IntPtr.Zero, (uint)IntPtr.Size); // size of int is 4

                ConvertRawImageTo24bpp(snapShot24bppPointer, snapShot, snapShot.Length);

                return _rc;
            }
            catch (Exception err)
            {
                _statusMessage += "An Error occured: " + err + "\r\n";
                throw err;
            }
            finally { }
        }

        public void GetNISTScore(byte[] pImage, ref uint nNFIQ)
        {
            uint nBPP = 0, nDPI = 0;
            uint nWidth = 0;
            uint nHeight = 0;

            _rc = LumiSDKWrapper.LumiGetImageParams(_hHandle, ref nWidth, ref nHeight, ref nBPP, ref nDPI);
            LumiInOpAPIWrapper.LumiInOpAPIWrapper.LumiStatus rc;
            rc = LumiInOpAPIWrapper.LumiInOpAPIWrapper.LumiComputeNFIQFromImage(pImage, nWidth, nHeight, nBPP, nDPI, ref nNFIQ);

        }
        public bool EnrollAndDisplay(byte[] snapShot24bppPointer, byte[] template1Pointer, ref uint templateSize, uint width, uint height, ref int nSpoof, LumiSDKWrapper.LumiPresenceDetectCallbackDelegate callbackFunc)
        {
            uint nNFIQ = 0;
            byte[] snapShot = new byte[width * height];
            LumiSDKWrapper.LumiStatus Status = EnrollImage(snapShot, snapShot24bppPointer, template1Pointer, ref templateSize, width, height, ref nSpoof, callbackFunc, ref nNFIQ);

            if (m_form.m_bClosingApp)
            {
                CloseScanner();
                return false;
            }

            
            
            if (m_form.m_bNISTQuality)
            {
                m_form.Invoke(m_form.m_DelegateNISTStatus, "NIST Quality = " + nNFIQ, m_form.Blue);
                m_form.Invoke(m_form.m_DelegateSetNISTImage, nNFIQ);

            }
            else
            {
                m_form.Invoke(m_form.m_DelegateNISTStatus, "", m_form.Blue);
            }

            if (Status == LumiSDKWrapper.LumiStatus.LUMI_STATUS_CANCELLED_BY_USER)
            {
                m_form.Invoke(m_form.m_DelegateCaptureCancelled);
                m_form.Invoke(m_form.m_DelegateErrorMessage, "Enroll Cancelled by User", m_form.Red);
                //throw new Exception();
                return false;
            }

            if (Status == LumiSDKWrapper.LumiStatus.LUMI_STATUS_ERROR_TIMEOUT)
            {
                m_form.Invoke(m_form.m_DelegateCaptureTimeOut);
                m_form.Invoke(m_form.m_DelegateErrorMessage, "Sensor Time Out.", m_form.Red);

                return false;
            }
            if (Status == LumiSDKWrapper.LumiStatus.LUMI_STATUS_ERROR_TIMEOUT_LATENT)
            {
                m_form.Invoke(m_form.m_DelegateCaptureTimeOut);
                m_form.Invoke(m_form.m_DelegateErrorMessage, "Sensor Latent Time Out.", m_form.Red);
                return false;
            }
            if (Status == LumiSDKWrapper.LumiStatus.LUMI_STATUS_ERROR_SENSOR_COMM_TIMEOUT)
            {
                m_form.m_bComTimeOut = true;
                m_form.Invoke(m_form.m_DelegateComTimeOut);
                m_form.Invoke(m_form.m_DelegateErrorMessage, "Sensor Communication Time Out.\n Please re-connect the device and restart the application", m_form.Red);
                return false;
            }

            m_form.Invoke(m_form.m_DelegateSetImage, snapShot, snapShot24bppPointer, width, height, template1Pointer, templateSize, -1);
            GC.KeepAlive(callbackFunc);

  
            //}
            return true;
        }


    }
}
