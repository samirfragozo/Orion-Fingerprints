
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
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using SDKEnrollApp.Properties;
using SDKWrapper;

namespace SDKEnrollApp
{
    class CaptureWithPd
    {
        public delegate int CaptureCallbackDelegate(byte[] pImage,
                                            int width,
                                            int height,
                                            int nBytesPerPixel,
                                            int nSpoof,
                                            LumiSdkWrapper.LumiAcqStatus status);
                                            
        // Reference to main form used to make syncronous user interface calls
        MainForm _mForm;
        private LumiSdkWrapper.LumiStatus _rc;
        private string _statusMessage;
        private uint _hHandle;

        public CaptureWithPd(MainForm form)
        {
            _mForm = form;
        }

        public void Run()
        {
            try
            {             
                var sensorid = (Sensor)_mForm._sensorList[(int)_mForm._nSelectedSensorId];
                _hHandle = sensorid.Handle;
                uint width = 0, height = 0;
                int nSpoof = 0;
                GetWidthAndHeight(ref width, ref height);
                byte[] snapShot24Bpp = new byte[width * height * 3]; // multiply by 3 to get 24bppRgb format
                byte[] template = new byte[5000]; //array to hold the template
                uint templateLen = 0;
                uint nNfiq = 0;
                // Assign the main form's PresenceDetectionCallback method to the LumiPresenceDetectCallbackDelegate               
                LumiSdkWrapper.LumiPresenceDetectCallbackDelegate del = _mForm.PresenceDetectionCallback;

                byte[] snapShot = new byte[width * height]; // raw 8 bpp image buffer

                ////////////////////

                if (sensorid.SensorType == LumiSdkWrapper.LumiSensorType.M32X)
                {

                    Bitmap bitmap1 = Resources.CaptureInProgress_M320_resized;
                    BitmapData bmpData = bitmap1.LockBits(new Rectangle(0, 0, bitmap1.Width, bitmap1.Height), ImageLockMode.WriteOnly, bitmap1.PixelFormat);
                    byte[] snapShot24 = new byte[bitmap1.Width * bitmap1.Height * 3]; // multiply by 3 to get 24bppRgb format
                    Marshal.Copy(bmpData.Scan0, snapShot24, 0, bitmap1.Width * bitmap1.Height * 3);
                    _mForm.Invoke(_mForm.m_DelegateSetImage, snapShot, snapShot24, (uint)bitmap1.Width, (uint)bitmap1.Height, template, templateLen, -1);
                }

                /////////////////////

                /////////////////

                LumiSdkWrapper.LumiProcessingMode processingMode;
                processingMode.bExtract = 0;
                processingMode.bLatent = 0;
                processingMode.bSpoof = 0;
                LumiSdkWrapper.LumiStatus pStatus = LumiSdkWrapper.GetProcessingMode(_hHandle, ref processingMode);
                if (pStatus != LumiSdkWrapper.LumiStatus.LumiStatusOk)
                {
                    _mForm.Invoke(_mForm.m_DelegateErrorMessage, "Capture failed. Unable to Set the Spoof detection mode", _mForm._red);
                    return;
                }
                if (_mForm.m_bSpoofEnabled)
                    processingMode.bSpoof = 1;
                else
                    processingMode.bSpoof = 0;

                pStatus = LumiSdkWrapper.SetProcessingMode(_hHandle, processingMode);
                if (pStatus != LumiSdkWrapper.LumiStatus.LumiStatusOk)
                {
                    _mForm.Invoke(_mForm.m_DelegateErrorMessage, "Capture failed. Unable to Set the Spoof detection mode", _mForm._red);
                    return;
                }

                ////////////////
                LumiSdkWrapper.LumiStatus status = CaptureImage(snapShot, snapShot24Bpp, template, ref templateLen, width, height, ref nSpoof, del, ref nNfiq);

                if (_mForm.m_bClosingApp)
                {
                    CloseScanner();
                    return;
                }

                
                
                if (_mForm.m_bNISTQuality)
                {
                    _mForm.Invoke(_mForm.m_DelegateNISTStatus, "NIST Quality = " + nNfiq, _mForm._blue);
                    _mForm.Invoke(_mForm.m_DelegateSetNISTImage, nNfiq);

                }
                else
                {
                    _mForm.Invoke(_mForm.m_DelegateNISTStatus, "", _mForm._blue);
                }
                if (status == LumiSdkWrapper.LumiStatus.LumiStatusCancelledByUser)
                {
                    _mForm.Invoke(_mForm.m_DelegateCaptureCancelled);
                    _mForm.Invoke(_mForm.m_DelegateErrorMessage, "Capture Cancelled by User", _mForm._red);
                    //throw new Exception();
                    return;
                }
                if (status == LumiSdkWrapper.LumiStatus.LumiStatusErrorTimeout)
                {                
                    _mForm.Invoke(_mForm.m_DelegateCaptureTimeOut);
                    _mForm.Invoke(_mForm.m_DelegateErrorMessage, "Sensor Time Out.", _mForm._red);
                                   
                    return;
                }
                if (status == LumiSdkWrapper.LumiStatus.LumiStatusErrorTimeoutLatent)
                {
                    _mForm.Invoke(_mForm.m_DelegateCaptureTimeOut);
                    _mForm.Invoke(_mForm.m_DelegateErrorMessage, "Sensor Time Out.", _mForm._red);                    
                    return;
                }
                if (status == LumiSdkWrapper.LumiStatus.LumiStatusErrorSensorCommTimeout)
                {
                    _mForm.Invoke(_mForm.m_DelegateComTimeOut);
                    _mForm.Invoke(_mForm.m_DelegateErrorMessage, "Sensor Communication Time Out.\n Please re-connect the device and restart the application", _mForm._red);
                    return;

                }

                _mForm.Invoke(_mForm.m_DelegateSetImage, snapShot, snapShot24Bpp, width, height, template, templateLen, -1);
                GC.KeepAlive(del);
                if (status == LumiSdkWrapper.LumiStatus.LumiStatusOk)
                {
                    if (_mForm.m_bSpoofEnabled&&(nSpoof!=-1))
                    {
                        if (nSpoof <= _mForm.m_SpoofThreshold)
                        {
                            _mForm.Invoke(_mForm.m_DelegateErrorMessage, "Capture Complete\r\nSpoofScore = " + nSpoof + ": It is a Real Finger", _mForm._blue);
                        }
                        else
                        {
                            _mForm.Invoke(_mForm.m_DelegateErrorMessage, "Capture Complete\r\nSpoofScore = " + nSpoof + ": It is a Spoof", _mForm._red);
                        }
                    }
                    else
                    {
                        _mForm.Invoke(_mForm.m_DelegateErrorMessage, "Capture Complete", _mForm._blue);
                    }
                } 
            }
            catch (Exception err)
            {
                string str = err.ToString();
                _mForm.Invoke(_mForm.m_DelegateErrorMessage, ("Error occured. Restart your application"), _mForm._red);
            }    
            _mForm.Invoke(_mForm.m_CaptureThreadFinished);
        
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
        public void GetNistScore(byte[] pImage, ref uint nNfiq)
        {
            uint nBpp = 0, nDpi = 0;
            uint nWidth = 0;
            uint nHeight = 0;

            _rc = LumiSdkWrapper.LumiGetImageParams(_hHandle, ref nWidth, ref nHeight, ref nBpp, ref nDpi);
            LumiInOpAPIWrapper.LumiInOpApiWrapper.LumiStatus rc;
            rc = LumiInOpAPIWrapper.LumiInOpApiWrapper.LumiComputeNFIQFromImage(pImage, nWidth, nHeight, nBpp, nDpi, ref nNfiq);

        }

        public LumiSdkWrapper.LumiStatus CaptureImage(byte[] snapShot, byte[] snapShot24BppPointer, byte[] template1Pointer, ref uint templateSize1, uint width, uint height, ref int spoof, LumiSdkWrapper.LumiPresenceDetectCallbackDelegate callbackFunc, ref uint nNfiq)
        {
            try
            {
                //byte[] snapShot = new byte[width * height]; // Array to hold raw data 8 bpp format 

                // Make sure Presence Detection is on.
                // Because we need another definition for LumiSetOption, we call the SetPresenceDetectionMode
                // static method on the object LumiSDKLumiSetOption.  This is because the LumiSDKWrapper
                // has the LumiSetOption defined to take the LumiPresenceDetectCallbackDelegate argument while for
                // setting PD mode, we need it to take an integer pointer argument instead.
                if (_mForm.m_bSensorTriggerArmed)
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

                _rc = LumiSdkWrapper.LumiSetDCOptions(_hHandle, _mForm.m_debugFolder, 0);

                //LumiSDKWrapper.LUMI_PROCESSING_MODE procMode;
                //procMode.bExtract = 1;
                //procMode.bLatent = 0;
                //procMode.bSpoof = 1;
                //_rc = LumiSDKWrapper.GetProcessingMode(_hHandle, ref procMode);

                _rc = LumiSdkWrapper.LumiCaptureEx(_hHandle, snapShot, template1Pointer, ref templateSize1, ref spoof, null);

                if (_rc != LumiSdkWrapper.LumiStatus.LumiStatusOk)
                {            
                    return _rc;
                    //throw new Exception("ERROR: lumiCaptureProcess rc = " + _rc);
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
    }
}
