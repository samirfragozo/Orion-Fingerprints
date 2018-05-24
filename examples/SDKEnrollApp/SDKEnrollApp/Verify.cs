
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
using System.Drawing;
using System.Runtime.InteropServices;
using SDKWrapper;

namespace SDKEnrollApp
{
    class Verify
    {
        MainForm _mForm;
        private LumiSdkWrapper.LumiStatus _rc;
        private string _statusMessage;
        private uint _hHandle;

        public Verify(MainForm form)
        {
            _mForm = form;
            Sensor sensorid = (Sensor)_mForm._sensorList[(int)_mForm._nSelectedSensorId];
            _hHandle = sensorid.Handle;
        }

        public void Run()
        {
            
            uint width = 0, height = 0;
            int nSpoof = 0;
            GetWidthAndHeight(ref width, ref height);
            byte[] snapShot24Bpp = new byte[width * height * 3]; // multiply by 3 to get 24bppRgb format
            byte[] template1 = new byte[5000]; //array to hold the template
            uint templateLen1 = 0;
            uint nMatchScore1 = 0;
            int nSpoofScore = 0;
            LumiSdkWrapper.LumiPresenceDetectCallbackDelegate del = _mForm.PresenceDetectionCallback;
            _mForm.Invoke(_mForm._delegateErrorMessage, "Coloque el dedo para realizar la verificación", Color.Blue);

            ////////////////////////////
            LumiSdkWrapper.LumiProcessingMode processingMode;
            processingMode.bExtract = 0;
            processingMode.bLatent = 0;
            processingMode.bSpoof = 0;
            LumiSdkWrapper.LumiStatus pStatus = LumiSdkWrapper.GetProcessingMode(_hHandle, ref processingMode);
            if (pStatus != LumiSdkWrapper.LumiStatus.LumiStatusOk)
            {
                _mForm.Invoke(_mForm._delegateErrorMessage, "Enrollment failed. Unable to Set the Spoof detection mode", Color.Red);
                return;
            }
            processingMode.bSpoof = _mForm._bSpoofEnabled ? (byte) 1 : (byte) 0;

            pStatus = LumiSdkWrapper.SetProcessingMode(_hHandle, processingMode);
            if (pStatus != LumiSdkWrapper.LumiStatus.LumiStatusOk)
            {
                _mForm.Invoke(_mForm._delegateErrorMessage, "Enrollment failed. Unable to Set the Spoof detection mode", Color.Red);
                return;
            }
            //////////////////////////////////

            if (EnrollAndDisplay(snapShot24Bpp, template1, ref templateLen1, width, height, ref nSpoof, del) == false)
            {
                _mForm.Invoke(_mForm._VerifyThreadFinished);
                return;
            }
            
            if (_mForm._bClosingApp)
            {
                CloseScanner();
                return;
            }

            /*int index;
            index = _form._currentSubject.fingers.IndexOf(_form._CurrentHotSpot);
            uint templatelength = (uint)_form._currentSubject.templateLengths[index];

            LumiSDKWrapper.LumiMatch(_hHandle, template1, ref templateLen1, (byte[])_form._currentSubject.templates[index], ref templatelength, ref nMatchScore1, ref nSpoofScore);
            if (_form._bSpoofEnabled&&(nSpoof != -1))
            {
                if (nMatchScore1 > _form._MatchThreshold && nSpoof <= _form._SpoofThreshold)
                {
                    _form.Invoke(_form._delegateErrorMessage, "Match Score = " + nMatchScore1 + "\nSpoof Score = " + nSpoof + "\nVerification Successful", _form.Blue);
                }

                else
                {
                    _form.Invoke(_form._delegateErrorMessage, "Match Score = " + nMatchScore1 + "\nSpoof Score = " + nSpoof + "\nVerification Failed", _form.Red);

                }
            }
            else
            {
                if (nMatchScore1 > _form._MatchThreshold)
                {
                    _form.Invoke(_form._delegateErrorMessage, "Match Score = " + nMatchScore1 + "\nVerification Successful", _form.Blue);
                }

                else
                {
                    _form.Invoke(_form._delegateErrorMessage, "Match Score = " + nMatchScore1 + "\nVerification Failed", _form.Red);

                }
            }*/


            _mForm.Invoke(_mForm._VerifyThreadFinished);
            
        }

        public void CloseScanner()
        {
            try
            {
                _rc = LumiSdkWrapper.LumiClose(_hHandle);
                _rc = LumiSdkWrapper.LumiExit();
            }
            catch (Exception err)
            {
                _statusMessage += "An Error occured: " + err + "  _rc = " + _rc + "\r\n";
                throw err;
            }
            finally { }
        }


        public void ConvertRawImageTo24Bpp(byte[] snapShot24BppPointer, byte[] snapshotPointer, int snapShotLength)
        {
            int innerOffset = 0;
            for (int offset = 0; offset < snapShotLength; offset++)
            {
                for (int counter = 1; counter <= 3; counter++)
                {
                    snapShot24BppPointer[innerOffset] = snapshotPointer[offset];
                    innerOffset += 1;
                }
            }
        }

        public void GetWidthAndHeight(ref uint width, ref uint height)
        {
            try
            {
                uint nBpp = 0, nDpi = 0;
                _rc = LumiSdkWrapper.LumiGetImageParams(_hHandle, ref width, ref height, ref nBpp, ref nDpi);
                if (_rc != LumiSdkWrapper.LumiStatus.LumiStatusOk)
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

        public LumiSdkWrapper.LumiStatus EnrollImage(byte[] snapShot, byte[] snapShot24BppPointer, byte[] template1Pointer, ref uint templateSize, uint width, uint height, ref int spoof, LumiSdkWrapper.LumiPresenceDetectCallbackDelegate callbackFunc, ref uint nNfiq)
        {
            try
            {
                //byte[] snapShot = new byte[width * height]; // Array to hold raw data 8 bpp format 

                // Make sure Presence Detection is on.
                // Because we need another definition for LumiSetOption, we call the SetPresenceDetectionMode
                // static method on the object LumiSDKLumiSetOption.  This is because the LumiSDKWrapper
                // has the LumiSetOption defined to take the LumiPresenceDetectCallbackDelegate argument while for
                // setting PD mode, we need it to take an integer pointer argument instead.
                if (_mForm._bSensorTriggerArmed)
                {
                    LumiSdkLumiSetOption.SetPresenceDetectionMode(_hHandle, LumiSdkWrapper.LumiPresDetMode.LumiPdOn);
                }
                else
                {
                    LumiSdkLumiSetOption.SetPresenceDetectionMode(_hHandle, LumiSdkWrapper.LumiPresDetMode.LumiPdOff);
                }

                // Set the address of the presence detection callback function   
                IntPtr prtDel = Marshal.GetFunctionPointerForDelegate(callbackFunc);
                _rc = LumiSdkWrapper.LumiSetOption(_hHandle, LumiSdkWrapper.LumiOptions.LumiOptionSetPresenceDetCallback, prtDel, (uint)IntPtr.Size);
                if (_rc != LumiSdkWrapper.LumiStatus.LumiStatusOk)
                {
                    return _rc;

                }
                _rc = LumiSdkWrapper.LumiSetDCOptions(_hHandle, _mForm._debugFolder, 0);
                if (_rc != LumiSdkWrapper.LumiStatus.LumiStatusOk)
                {
                    return _rc;

                }
                _rc = LumiSdkWrapper.LumiCaptureEx(_hHandle, snapShot, template1Pointer, ref templateSize, ref spoof, null);


                if (_rc != LumiSdkWrapper.LumiStatus.LumiStatusOk)
                {
                    return _rc;
                    
                }
                else
                {
                    _statusMessage += "PASS: lumiCaptureProcess rc = " + _rc + " spoof = " + spoof + "\r\n";
                }

                GetNistScore(snapShot, ref nNfiq);


                LumiSdkWrapper.LumiSetOption(_hHandle, LumiSdkWrapper.LumiOptions.LumiOptionSetPresenceDetCallback, IntPtr.Zero, (uint)IntPtr.Size); // size of int is 4

                ConvertRawImageTo24Bpp(snapShot24BppPointer, snapShot, snapShot.Length);

                return _rc;
            }
            catch (Exception err)
            {
                _statusMessage += "An Error occured: " + err + "\r\n";
                throw err;
            }
            finally { }
        }

        public void GetNistScore(byte[] pImage, ref uint nNfiq)
        {
            uint nBpp = 0, nDpi = 0;
            uint nWidth = 0;
            uint nHeight = 0;

            _rc = LumiSdkWrapper.LumiGetImageParams(_hHandle, ref nWidth, ref nHeight, ref nBpp, ref nDpi);
            LumiInOpAPIWrapper.LumiInOpApiWrapper.LumiStatus rc;
            rc = LumiInOpAPIWrapper.LumiInOpApiWrapper.LumiComputeNFIQFromImage(pImage, nWidth, nHeight, nBpp, nDpi, ref nNfiq);

        }
        public bool EnrollAndDisplay(byte[] snapShot24BppPointer, byte[] template1Pointer, ref uint templateSize, uint width, uint height, ref int nSpoof, LumiSdkWrapper.LumiPresenceDetectCallbackDelegate callbackFunc)
        {
            uint nNfiq = 0;
            byte[] snapShot = new byte[width * height];
            LumiSdkWrapper.LumiStatus status = EnrollImage(snapShot, snapShot24BppPointer, template1Pointer, ref templateSize, width, height, ref nSpoof, callbackFunc, ref nNfiq);

            if (_mForm._bClosingApp)
            {
                CloseScanner();
                return false;
            }

            
            
            if (_mForm._bNISTQuality)
            {
                _mForm.Invoke(_mForm._DelegateNISTStatus, "NIST Quality = " + nNfiq, _mForm._blue);
                _mForm.Invoke(_mForm._DelegateSetNISTImage, nNfiq);

            }
            else
            {
                _mForm.Invoke(_mForm._DelegateNISTStatus, "", _mForm._blue);
            }

            if (status == LumiSdkWrapper.LumiStatus.LumiStatusCancelledByUser)
            {
                _mForm.Invoke(_mForm._DelegateCaptureCancelled);
                _mForm.Invoke(_mForm._delegateErrorMessage, "Enroll Cancelled by User", Color.Red);
                //throw new Exception();
                return false;
            }

            if (status == LumiSdkWrapper.LumiStatus.LumiStatusErrorTimeout)
            {
                _mForm.Invoke(_mForm._DelegateCaptureTimeOut);
                _mForm.Invoke(_mForm._delegateErrorMessage, "Sensor Time Out.", Color.Red);

                return false;
            }
            if (status == LumiSdkWrapper.LumiStatus.LumiStatusErrorTimeoutLatent)
            {
                _mForm.Invoke(_mForm._DelegateCaptureTimeOut);
                _mForm.Invoke(_mForm._delegateErrorMessage, "Sensor Latent Time Out.", Color.Red);
                return false;
            }
            if (status == LumiSdkWrapper.LumiStatus.LumiStatusErrorSensorCommTimeout)
            {
                _mForm._bComTimeOut = true;
                _mForm.Invoke(_mForm._DelegateComTimeOut);
                _mForm.Invoke(_mForm._delegateErrorMessage, "Sensor Communication Time Out.\n Please re-connect the device and restart the application", Color.Red);
                return false;
            }

            _mForm.Invoke(_mForm._DelegateSetImage, snapShot, snapShot24BppPointer, width, height, template1Pointer, templateSize, -1);
            GC.KeepAlive(callbackFunc);

  
            //}
            return true;
        }


    }
}
