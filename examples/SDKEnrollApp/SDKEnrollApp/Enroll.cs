
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
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using SDKWrapper;

namespace SDKEnrollApp
{
    internal class Enroll
    {
        // Reference to main form used to make syncronous user interface calls
        private readonly MainForm _form;
        private LumiSDKWrapper.LumiStatus _rc;
        private string _statusMessage;
        private uint _hHandle;

        public Enroll(MainForm form)
        {
            _form = form;
            Sensor sensorid = (Sensor)_form.SensorList[(int)_form.m_nSelectedSensorID];
            _hHandle = sensorid.handle;
        }

        public void Run()
        {
            uint width = 0, height = 0;
            int nSpoof = 0;
            GetWidthAndHeight(ref width, ref height);
            byte[] snapShot24bpp = new byte[width * height * 3]; // multiply by 3 to get 24bppRgb format
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

            LumiSDKWrapper.LumiPresenceDetectCallbackDelegate del = _form.PresenceDetectionCallback;
            ////////////////////////////
            LumiSDKWrapper.LUMI_PROCESSING_MODE processingMode;
            processingMode.bExtract = 0;
            processingMode.bLatent = 0;
            processingMode.bSpoof = 0;
            LumiSDKWrapper.LumiStatus pStatus = LumiSDKWrapper.GetProcessingMode(_hHandle, ref processingMode);
            if (pStatus != LumiSDKWrapper.LumiStatus.LUMI_STATUS_OK)
            {
                _form.Invoke(_form.m_DelegateErrorMessage, "Enrollment failed. Unable to Set the Spoof detection mode", _form.Red);
                return;
            }
            
            processingMode.bSpoof = _form.m_bSpoofEnabled ? (byte) 1 : (byte) 0;

            pStatus = LumiSDKWrapper.SetProcessingMode(_hHandle, processingMode);
            
            if (pStatus != LumiSDKWrapper.LumiStatus.LUMI_STATUS_OK)
            {
                _form.Invoke(_form.m_DelegateErrorMessage, "Enrollment failed. Unable to Set the Spoof detection mode", _form.Red);
                return;
            }

            //////////////////////////////////
            _form.Invoke(_form.m_DelegateErrorMessage, "Place finger down for first insertion", _form.Blue);
            if (EnrollAndDisplay(0, snapShot24bpp, template1, ref templateLen1, width, height, ref nSpoof, del) == false)
            {
                _form.Invoke(_form.m_EnrollThreadFinished);
                return;
            }

            if (_form.m_bClosingApp)
            {
                CloseScanner();
                return;
            }

            /////////////
            if (_form.m_bSpoofEnabled)
            {
                if (nSpoof > 1050)
                {
                    _form.Invoke(_form.m_DelegateErrorMessage, "Enrollment failed. Please enroll with real finger", _form.Red);
                    _form.Invoke(_form.m_EnrollThreadFinished);
                    return;
                }
            }

            // Force Finger Lift between enrollment insertions
            _form.Invoke(_form.m_DelegateErrorMessage, "Please lift finger and place again", _form.Red);
            ForceFingerLift();

            _form.Invoke(_form.m_DelegateErrorMessage, "Place finger down for second insertion", _form.Blue);
            if (EnrollAndDisplay(1, snapShot24bpp, template2, ref templateLen2, width, height, ref nSpoof, del) == false)
            {
                _form.Invoke(_form.m_EnrollThreadFinished);
                return;
            }

            if (_form.m_bClosingApp)
            {
                CloseScanner();
                return;
            }

            if (_form.m_bSpoofEnabled)
            {
                if (nSpoof > 1050)
                {
                    _form.Invoke(_form.m_DelegateErrorMessage, "Enrollment failed. Please enroll with real finger", _form.Red);
                    _form.Invoke(_form.m_EnrollThreadFinished);
                    return;
                }
            }

            // Force Finger Lift between enrollment insertions
            _form.Invoke(_form.m_DelegateErrorMessage, "Please lift finger and place again", _form.Red);            
            ForceFingerLift();

            _form.Invoke(_form.m_DelegateErrorMessage, "Place finger down for third insertion", _form.Blue);
            if (EnrollAndDisplay(2, snapShot24bpp, template3, ref templateLen3, width, height, ref nSpoof, del) == false)
            {
                _form.Invoke(_form.m_EnrollThreadFinished);
                return;
            }

            if (_form.m_bClosingApp)
            {
                CloseScanner();
                return;
            }

            if (_form.m_bSpoofEnabled)
            {
                if (nSpoof > 1050)
                {
                    _form.Invoke(_form.m_DelegateErrorMessage, "Enrollment failed. Please enroll with real finger", _form.Red);
                    _form.Invoke(_form.m_EnrollThreadFinished);
                    return;
                }

            }

            ///////////////
            /*
            LumiSDKWrapper.LumiMatch(_hHandle, template2, ref templateLen2, template3, ref templateLen3, ref nMatchScore2, ref nSpoofScore);

            if (nMatchScore2 < _form.m_MatchThreshold)
            {
                _form.Invoke(_form.m_DelegateErrorMessage, "Enrollment failed. Please use the same finger for enrollment", _form.Red);
                _form.Invoke(_form.m_EnrollThreadFinished);
                return;
            }
            LumiSDKWrapper.LumiMatch(_hHandle, template1, ref templateLen1, template3, ref templateLen3, ref nMatchScore3, ref nSpoofScore);
          
            FileStream stream;
            BinaryFormatter bformatter = new BinaryFormatter();

            if (_form.m_newSubjID)
            {
                stream = File.Create(_form.m_sFolderPath + "\\Database\\" + _form.m_sEnrollSubjID + ".bin");              
            }
            else
            {
                stream = File.Open(_form.m_sFolderPath + "\\Database\\" + _form.m_sEnrollSubjID + ".bin", FileMode.Open);                
            }


            _form.m_currentSubject.deleteData(_form.m_CurrentHotSpot);
            
            if(nMatchScore1>nMatchScore2)
            {
                if (nMatchScore1 > nMatchScore3)
                {
                    if (nMatchScore2 > nMatchScore3)
                    {
                        _form.m_currentSubject.addData(template2, templateLen2, _form.m_CurrentHotSpot);
                      
                    }
                    else
                    {
                        _form.m_currentSubject.addData(template1, templateLen1, _form.m_CurrentHotSpot);

                    }

                }
                else
                {
                    _form.m_currentSubject.addData(template1, templateLen1, _form.m_CurrentHotSpot);
                }   
            		
            }
            else
            {
                if (nMatchScore2 > nMatchScore3)
                {
                    if (nMatchScore1 > nMatchScore3)
                    {
                        _form.m_currentSubject.addData(template2, templateLen2, _form.m_CurrentHotSpot);
                    }
                    else
                    {
                        _form.m_currentSubject.addData(template3, templateLen3, _form.m_CurrentHotSpot);
                    }
                }
                else
                {
                    _form.m_currentSubject.addData(template3, templateLen3, _form.m_CurrentHotSpot);
                }   

            }

            bformatter.Serialize(stream, _form.m_currentSubject);
            stream.Close();*/

            _form.Invoke(_form.m_DelegateErrorMessage, "Enrollment successful", _form.Blue);

            _form.Invoke(_form.m_EnrollThreadFinished);

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
                if (_form.m_bSensorTriggerArmed)
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
                _rc = LumiSDKWrapper.LumiSetDCOptions(_hHandle, _form.m_debugFolder, 0);
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
        public void GetNISTScore(byte[] pImage, ref uint nNFIQ)
        {
            uint nBPP = 0, nDPI = 0;
            uint nWidth = 0;
            uint nHeight = 0;

            _rc = LumiSDKWrapper.LumiGetImageParams(_hHandle, ref nWidth, ref nHeight, ref nBPP, ref nDPI);
            LumiInOpAPIWrapper.LumiInOpAPIWrapper.LumiStatus rc;
            rc = LumiInOpAPIWrapper.LumiInOpAPIWrapper.LumiComputeNFIQFromImage(pImage, nWidth, nHeight, nBPP, nDPI, ref nNFIQ);

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
        public bool EnrollAndDisplay(int nEnrollmentCaptureIndex, byte[] snapShot24bppPointer, byte[] template1Pointer, ref uint templateSize, uint width, uint height, ref int nSpoof, LumiSDKWrapper.LumiPresenceDetectCallbackDelegate callbackFunc)
        {
            uint nNFIQ = 0;
            byte[] snapShot = new byte[width * height];
            LumiSDKWrapper.LumiStatus Status = EnrollImage(snapShot, snapShot24bppPointer, template1Pointer, ref templateSize, width, height, ref nSpoof, callbackFunc, ref nNFIQ);
 
            if (_form.m_bClosingApp)
            {
                CloseScanner();
                return false;
            }
            
            if (_form.m_bNISTQuality)
            {
                _form.Invoke(_form.m_DelegateNISTStatus, "NIST Quality = " + nNFIQ, _form.Blue);
                _form.Invoke(_form.m_DelegateSetNISTImage, nNFIQ);

            }
            else
            {
                _form.Invoke(_form.m_DelegateNISTStatus, "", _form.Blue);
            }

            if (Status == LumiSDKWrapper.LumiStatus.LUMI_STATUS_CANCELLED_BY_USER)
            {
                _form.Invoke(_form.m_DelegateCaptureCancelled);
                _form.Invoke(_form.m_DelegateErrorMessage, "Enroll Cancelled by User", _form.Red);
                return false;
            }

            if (Status == LumiSDKWrapper.LumiStatus.LUMI_STATUS_ERROR_TIMEOUT)
            {
                _form.Invoke(_form.m_DelegateCaptureTimeOut);
                _form.Invoke(_form.m_DelegateErrorMessage, "Sensor Time Out. ", _form.Red);

                return false;
            }
            if (Status == LumiSDKWrapper.LumiStatus.LUMI_STATUS_ERROR_TIMEOUT_LATENT)
            {
                _form.Invoke(_form.m_DelegateCaptureTimeOut);
                _form.Invoke(_form.m_DelegateErrorMessage, "Sensor Latent Time Out.\nPlease lift the finger and then place again", _form.Red);
                return false;
            }
            if (Status == LumiSDKWrapper.LumiStatus.LUMI_STATUS_ERROR_SENSOR_COMM_TIMEOUT)
            {
                _form.m_bComTimeOut = true;
                _form.Invoke(_form.m_DelegateComTimeOut);
                _form.Invoke(_form.m_DelegateErrorMessage, "Sensor Communication Time Out.\n Please re-connect the device and restart the application", _form.Red);
                return false;
            }

            _form.Invoke(_form.m_DelegateSetImage, snapShot, snapShot24bppPointer, width, height, template1Pointer, templateSize, nEnrollmentCaptureIndex);
            GC.KeepAlive(callbackFunc);

            return true;
        }

        public void ForceFingerLift()
        {
            LumiSDKWrapper.LumiAcqStatusCallbackDelegate del = _form.AcquStatusCallback;

            LumiSDKWrapper.LUMI_ACQ_STATUS nStatus = LumiSDKWrapper.LUMI_ACQ_STATUS.LUMI_ACQ_BUSY;

            LumiSDKWrapper.LumiStatus rc = LumiSDKWrapper.LumiDetectFinger(_hHandle, ref nStatus, del);

            if (rc != LumiSDKWrapper.LumiStatus.LUMI_STATUS_OK)
            {
            }
        }
    }
}
