
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
    internal class Enroll
    {
        // Reference to main form used to make syncronous user interface calls
        private readonly MainForm _form;
        private LumiSdkWrapper.LumiStatus _rc;
        private string _statusMessage;
        private uint _hHandle;

        public Enroll(MainForm form)
        {
            _form = form;
            Sensor sensorid = (Sensor)_form._sensorList[(int)_form._nSelectedSensorId];
            _hHandle = sensorid.Handle;
        }

        public void Run()
        {
            uint width = 0, height = 0;
            int nSpoof = 0;
            GetWidthAndHeight(ref width, ref height);
            byte[] snapShot24Bpp = new byte[width * height * 3]; // multiply by 3 to get 24bppRgb format
            byte[] template1 = new byte[5000]; //array to hold the template
            byte[] template2 = new byte[5000]; //array to hold the template
            byte[] template3 = new byte[5000]; //array to hold the template

            
            uint templateLen1 = 0;
            uint templateLen2 =  0;
            uint templateLen3 = 0;

            uint nMatchScore1 = 0;
            uint nMatchScore2 = 0;
            uint nMatchScore3 = 0;

            int nSpoofScore = 0;

            LumiSdkWrapper.LumiPresenceDetectCallbackDelegate del = _form.PresenceDetectionCallback;
            ////////////////////////////
            LumiSdkWrapper.LumiProcessingMode processingMode;
            processingMode.bExtract = 0;
            processingMode.bLatent = 0;
            processingMode.bSpoof = 0;
            LumiSdkWrapper.LumiStatus pStatus = LumiSdkWrapper.GetProcessingMode(_hHandle, ref processingMode);
            if (pStatus != LumiSdkWrapper.LumiStatus.LumiStatusOk)
            {
                _form.Invoke(_form._delegateErrorMessage, "Enrollment failed. Unable to Set the Spoof detection mode", Color.Red);
                return;
            }
            
            processingMode.bSpoof = _form._bSpoofEnabled ? (byte) 1 : (byte) 0;

            pStatus = LumiSdkWrapper.SetProcessingMode(_hHandle, processingMode);
            
            if (pStatus != LumiSdkWrapper.LumiStatus.LumiStatusOk)
            {
                _form.Invoke(_form._DelegateErrorMessage, "Enrollment failed. Unable to Set the Spoof detection mode", Color.Red);
                return;
            }

            //////////////////////////////////
            _form.Invoke(_form._DelegateErrorMessage, "Place finger down for first insertion", _form._blue);
            if (EnrollAndDisplay(0, snapShot24Bpp, template1, ref templateLen1, width, height, ref nSpoof, del) == false)
            {
                _form.Invoke(_form._EnrollThreadFinished);
                return;
            }

            if (_form._bClosingApp)
            {
                CloseScanner();
                return;
            }

            /////////////
            if (_form._bSpoofEnabled)
            {
                if (nSpoof > 1050)
                {
                    _form.Invoke(_form._DelegateErrorMessage, "Enrollment failed. Please enroll with real finger", Color.Red);
                    _form.Invoke(_form._EnrollThreadFinished);
                    return;
                }
            }

            // Force Finger Lift between enrollment insertions
            _form.Invoke(_form._DelegateErrorMessage, "Please lift finger and place again", Color.Red);
            ForceFingerLift();

            _form.Invoke(_form._DelegateErrorMessage, "Place finger down for second insertion", _form._blue);
            if (EnrollAndDisplay(1, snapShot24Bpp, template2, ref templateLen2, width, height, ref nSpoof, del) == false)
            {
                _form.Invoke(_form._EnrollThreadFinished);
                return;
            }

            if (_form._bClosingApp)
            {
                CloseScanner();
                return;
            }

            if (_form._bSpoofEnabled)
            {
                if (nSpoof > 1050)
                {
                    _form.Invoke(_form._DelegateErrorMessage, "Enrollment failed. Please enroll with real finger", Color.Red);
                    _form.Invoke(_form._EnrollThreadFinished);
                    return;
                }
            }

            // Force Finger Lift between enrollment insertions
            _form.Invoke(_form._DelegateErrorMessage, "Please lift finger and place again", Color.Red);            
            ForceFingerLift();

            _form.Invoke(_form._DelegateErrorMessage, "Place finger down for third insertion", _form._blue);
            if (EnrollAndDisplay(2, snapShot24Bpp, template3, ref templateLen3, width, height, ref nSpoof, del) == false)
            {
                _form.Invoke(_form._EnrollThreadFinished);
                return;
            }

            if (_form._bClosingApp)
            {
                CloseScanner();
                return;
            }

            if (_form._bSpoofEnabled)
            {
                if (nSpoof > 1050)
                {
                    _form.Invoke(_form._DelegateErrorMessage, "Enrollment failed. Please enroll with real finger", Color.Red);
                    _form.Invoke(_form._EnrollThreadFinished);
                    return;
                }

            }

            ///////////////
            /*
            LumiSDKWrapper.LumiMatch(_hHandle, template2, ref templateLen2, template3, ref templateLen3, ref nMatchScore2, ref nSpoofScore);

            if (nMatchScore2 < _form._MatchThreshold)
            {
                _form.Invoke(_form._DelegateErrorMessage, "Enrollment failed. Please use the same finger for enrollment", _form.Red);
                _form.Invoke(_form._EnrollThreadFinished);
                return;
            }
            LumiSDKWrapper.LumiMatch(_hHandle, template1, ref templateLen1, template3, ref templateLen3, ref nMatchScore3, ref nSpoofScore);
          
            FileStream stream;
            BinaryFormatter bformatter = new BinaryFormatter();

            if (_form._newSubjID)
            {
                stream = File.Create(_form._sFolderPath + "\\Database\\" + _form._sEnrollSubjID + ".bin");              
            }
            else
            {
                stream = File.Open(_form._sFolderPath + "\\Database\\" + _form._sEnrollSubjID + ".bin", FileMode.Open);                
            }


            _form._currentSubject.deleteData(_form._CurrentHotSpot);
            
            if(nMatchScore1>nMatchScore2)
            {
                if (nMatchScore1 > nMatchScore3)
                {
                    if (nMatchScore2 > nMatchScore3)
                    {
                        _form._currentSubject.addData(template2, templateLen2, _form._CurrentHotSpot);
                      
                    }
                    else
                    {
                        _form._currentSubject.addData(template1, templateLen1, _form._CurrentHotSpot);

                    }

                }
                else
                {
                    _form._currentSubject.addData(template1, templateLen1, _form._CurrentHotSpot);
                }   
            		
            }
            else
            {
                if (nMatchScore2 > nMatchScore3)
                {
                    if (nMatchScore1 > nMatchScore3)
                    {
                        _form._currentSubject.addData(template2, templateLen2, _form._CurrentHotSpot);
                    }
                    else
                    {
                        _form._currentSubject.addData(template3, templateLen3, _form._CurrentHotSpot);
                    }
                }
                else
                {
                    _form._currentSubject.addData(template3, templateLen3, _form._CurrentHotSpot);
                }   

            }

            bformatter.Serialize(stream, _form._currentSubject);
            stream.Close();*/

            _form.Invoke(_form._DelegateErrorMessage, "Enrollment successful", _form._blue);

            _form.Invoke(_form._EnrollThreadFinished);

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
                if (_form._bSensorTriggerArmed)
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
                _rc = LumiSdkWrapper.LumiSetDCOptions(_hHandle, _form._debugFolder, 0);
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
        public void GetNistScore(byte[] pImage, ref uint nNfiq)
        {
            uint nBpp = 0, nDpi = 0;
            uint nWidth = 0;
            uint nHeight = 0;

            _rc = LumiSdkWrapper.LumiGetImageParams(_hHandle, ref nWidth, ref nHeight, ref nBpp, ref nDpi);
            LumiInOpAPIWrapper.LumiInOpApiWrapper.LumiStatus rc;
            rc = LumiInOpAPIWrapper.LumiInOpApiWrapper.LumiComputeNFIQFromImage(pImage, nWidth, nHeight, nBpp, nDpi, ref nNfiq);

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
        public bool EnrollAndDisplay(int nEnrollmentCaptureIndex, byte[] snapShot24BppPointer, byte[] template1Pointer, ref uint templateSize, uint width, uint height, ref int nSpoof, LumiSdkWrapper.LumiPresenceDetectCallbackDelegate callbackFunc)
        {
            uint nNfiq = 0;
            byte[] snapShot = new byte[width * height];
            LumiSdkWrapper.LumiStatus status = EnrollImage(snapShot, snapShot24BppPointer, template1Pointer, ref templateSize, width, height, ref nSpoof, callbackFunc, ref nNfiq);
 
            if (_form._bClosingApp)
            {
                CloseScanner();
                return false;
            }
            
            if (_form._bNISTQuality)
            {
                _form.Invoke(_form._DelegateNISTStatus, "NIST Quality = " + nNfiq, _form._blue);
                _form.Invoke(_form._DelegateSetNISTImage, nNfiq);

            }
            else
            {
                _form.Invoke(_form._DelegateNISTStatus, "", _form._blue);
            }

            if (status == LumiSdkWrapper.LumiStatus.LumiStatusCancelledByUser)
            {
                _form.Invoke(_form._DelegateCaptureCancelled);
                _form.Invoke(_form._DelegateErrorMessage, "Enroll Cancelled by User", Color.Red);
                return false;
            }

            if (status == LumiSdkWrapper.LumiStatus.LumiStatusErrorTimeout)
            {
                _form.Invoke(_form._DelegateCaptureTimeOut);
                _form.Invoke(_form._DelegateErrorMessage, "Sensor Time Out. ", Color.Red);

                return false;
            }
            if (status == LumiSdkWrapper.LumiStatus.LumiStatusErrorTimeoutLatent)
            {
                _form.Invoke(_form._DelegateCaptureTimeOut);
                _form.Invoke(_form._DelegateErrorMessage, "Sensor Latent Time Out.\nPlease lift the finger and then place again", Color.Red);
                return false;
            }
            if (status == LumiSdkWrapper.LumiStatus.LumiStatusErrorSensorCommTimeout)
            {
                _form._bComTimeOut = true;
                _form.Invoke(_form._DelegateComTimeOut);
                _form.Invoke(_form._DelegateErrorMessage, "Sensor Communication Time Out.\n Please re-connect the device and restart the application", Color.Red);
                return false;
            }

            _form.Invoke(_form._DelegateSetImage, snapShot, snapShot24BppPointer, width, height, template1Pointer, templateSize, nEnrollmentCaptureIndex);
            GC.KeepAlive(callbackFunc);

            return true;
        }

        public void ForceFingerLift()
        {
            LumiSdkWrapper.LumiAcqStatusCallbackDelegate del = _form.AcquStatusCallback;

            LumiSdkWrapper.LumiAcqStatus nStatus = LumiSdkWrapper.LumiAcqStatus.LumiAcqBusy;

            LumiSdkWrapper.LumiStatus rc = LumiSdkWrapper.LumiDetectFinger(_hHandle, ref nStatus, del);

            if (rc != LumiSdkWrapper.LumiStatus.LumiStatusOk)
            {
            }
        }
    }
}
