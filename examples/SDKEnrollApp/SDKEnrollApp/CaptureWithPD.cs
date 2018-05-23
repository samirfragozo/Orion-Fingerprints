
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
    class CaptureWithPD
    {
        public delegate int CaptureCallbackDelegate(byte[] pImage,
                                            int width,
                                            int height,
                                            int nBytesPerPixel,
                                            int nSpoof,
                                            LumiSDKWrapper.LUMI_ACQ_STATUS status);
                                            
        // Reference to main form used to make syncronous user interface calls
        MainForm m_form;
        private LumiSDKWrapper.LumiStatus _rc;
        private string _statusMessage;
        private uint _hHandle;

        public CaptureWithPD(MainForm form)
        {
            m_form = form;
        }

        public void Run()
        {
            try
            {             
                var sensorid = (Sensor)m_form.SensorList[(int)m_form.m_nSelectedSensorID];
                _hHandle = sensorid.handle;
                uint width = 0, height = 0;
                int nSpoof = 0;
                GetWidthAndHeight(ref width, ref height);
                byte[] snapShot24bpp = new byte[width * height * 3]; // multiply by 3 to get 24bppRgb format
                byte[] template = new byte[5000]; //array to hold the template
                uint templateLen = 0;
                uint nNFIQ = 0;
                // Assign the main form's PresenceDetectionCallback method to the LumiPresenceDetectCallbackDelegate               
                LumiSDKWrapper.LumiPresenceDetectCallbackDelegate del = m_form.PresenceDetectionCallback;

                byte[] snapShot = new byte[width * height]; // raw 8 bpp image buffer

                ////////////////////

                if (sensorid.SensorType == LumiSDKWrapper.LUMI_SENSOR_TYPE.M32X)
                {

                    Bitmap bitmap1 = Resources.CaptureInProgress_M320_resized;
                    BitmapData bmpData = bitmap1.LockBits(new Rectangle(0, 0, bitmap1.Width, bitmap1.Height), ImageLockMode.WriteOnly, bitmap1.PixelFormat);
                    byte[] snapShot24 = new byte[bitmap1.Width * bitmap1.Height * 3]; // multiply by 3 to get 24bppRgb format
                    Marshal.Copy(bmpData.Scan0, snapShot24, 0, bitmap1.Width * bitmap1.Height * 3);
                    m_form.Invoke(m_form.m_DelegateSetImage, snapShot, snapShot24, (uint)bitmap1.Width, (uint)bitmap1.Height, template, templateLen, -1);
                }

                /////////////////////

                /////////////////

                LumiSDKWrapper.LUMI_PROCESSING_MODE processingMode;
                processingMode.bExtract = 0;
                processingMode.bLatent = 0;
                processingMode.bSpoof = 0;
                LumiSDKWrapper.LumiStatus pStatus = LumiSDKWrapper.GetProcessingMode(_hHandle, ref processingMode);
                if (pStatus != LumiSDKWrapper.LumiStatus.LUMI_STATUS_OK)
                {
                    m_form.Invoke(m_form.m_DelegateErrorMessage, "Capture failed. Unable to Set the Spoof detection mode", m_form.Red);
                    return;
                }
                if (m_form.m_bSpoofEnabled)
                    processingMode.bSpoof = 1;
                else
                    processingMode.bSpoof = 0;

                pStatus = LumiSDKWrapper.SetProcessingMode(_hHandle, processingMode);
                if (pStatus != LumiSDKWrapper.LumiStatus.LUMI_STATUS_OK)
                {
                    m_form.Invoke(m_form.m_DelegateErrorMessage, "Capture failed. Unable to Set the Spoof detection mode", m_form.Red);
                    return;
                }

                ////////////////
                LumiSDKWrapper.LumiStatus Status = CaptureImage(snapShot, snapShot24bpp, template, ref templateLen, width, height, ref nSpoof, del, ref nNFIQ);

                if (m_form.m_bClosingApp)
                {
                    CloseScanner();
                    return;
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
                    m_form.Invoke(m_form.m_DelegateErrorMessage, "Capture Cancelled by User", m_form.Red);
                    //throw new Exception();
                    return;
                }
                if (Status == LumiSDKWrapper.LumiStatus.LUMI_STATUS_ERROR_TIMEOUT)
                {                
                    m_form.Invoke(m_form.m_DelegateCaptureTimeOut);
                    m_form.Invoke(m_form.m_DelegateErrorMessage, "Sensor Time Out.", m_form.Red);
                                   
                    return;
                }
                if (Status == LumiSDKWrapper.LumiStatus.LUMI_STATUS_ERROR_TIMEOUT_LATENT)
                {
                    m_form.Invoke(m_form.m_DelegateCaptureTimeOut);
                    m_form.Invoke(m_form.m_DelegateErrorMessage, "Sensor Time Out.", m_form.Red);                    
                    return;
                }
                if (Status == LumiSDKWrapper.LumiStatus.LUMI_STATUS_ERROR_SENSOR_COMM_TIMEOUT)
                {
                    m_form.Invoke(m_form.m_DelegateComTimeOut);
                    m_form.Invoke(m_form.m_DelegateErrorMessage, "Sensor Communication Time Out.\n Please re-connect the device and restart the application", m_form.Red);
                    return;

                }

                m_form.Invoke(m_form.m_DelegateSetImage, snapShot, snapShot24bpp, width, height, template, templateLen, -1);
                GC.KeepAlive(del);
                if (Status == LumiSDKWrapper.LumiStatus.LUMI_STATUS_OK)
                {
                    if (m_form.m_bSpoofEnabled&&(nSpoof!=-1))
                    {
                        if (nSpoof <= m_form.m_SpoofThreshold)
                        {
                            m_form.Invoke(m_form.m_DelegateErrorMessage, "Capture Complete\r\nSpoofScore = " + nSpoof + ": It is a Real Finger", m_form.Blue);
                        }
                        else
                        {
                            m_form.Invoke(m_form.m_DelegateErrorMessage, "Capture Complete\r\nSpoofScore = " + nSpoof + ": It is a Spoof", m_form.Red);
                        }
                    }
                    else
                    {
                        m_form.Invoke(m_form.m_DelegateErrorMessage, "Capture Complete", m_form.Blue);
                    }
                } 
            }
            catch (Exception err)
            {
                string str = err.ToString();
                m_form.Invoke(m_form.m_DelegateErrorMessage, ("Error occured. Restart your application"), m_form.Red);
            }    
            m_form.Invoke(m_form.m_CaptureThreadFinished);
        
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
        public void GetNISTScore(byte[] pImage, ref uint nNFIQ)
        {
            uint nBPP = 0, nDPI = 0;
            uint nWidth = 0;
            uint nHeight = 0;

            _rc = LumiSDKWrapper.LumiGetImageParams(_hHandle, ref nWidth, ref nHeight, ref nBPP, ref nDPI);
            LumiInOpAPIWrapper.LumiInOpAPIWrapper.LumiStatus rc;
            rc = LumiInOpAPIWrapper.LumiInOpAPIWrapper.LumiComputeNFIQFromImage(pImage, nWidth, nHeight, nBPP, nDPI, ref nNFIQ);

        }

        public LumiSDKWrapper.LumiStatus CaptureImage(byte[] snapShot, byte[] snapShot24bppPointer, byte[] template1Pointer, ref uint templateSize1, uint width, uint height, ref int spoof, LumiSDKWrapper.LumiPresenceDetectCallbackDelegate callbackFunc, ref uint nNFIQ)
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

                _rc = LumiSDKWrapper.LumiSetDCOptions(_hHandle, m_form.m_debugFolder, 0);

                //LumiSDKWrapper.LUMI_PROCESSING_MODE procMode;
                //procMode.bExtract = 1;
                //procMode.bLatent = 0;
                //procMode.bSpoof = 1;
                //_rc = LumiSDKWrapper.GetProcessingMode(_hHandle, ref procMode);

                _rc = LumiSDKWrapper.LumiCaptureEx(_hHandle, snapShot, template1Pointer, ref templateSize1, ref spoof, null);

                if (_rc != LumiSDKWrapper.LumiStatus.LUMI_STATUS_OK)
                {            
                    return _rc;
                    //throw new Exception("ERROR: lumiCaptureProcess rc = " + _rc);
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
    }
}
