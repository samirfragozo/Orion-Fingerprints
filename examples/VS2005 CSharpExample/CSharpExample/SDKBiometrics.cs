/******************************************************************************
**  You are free to use this example code to generate similar functionality  
**  tailored to your own specific needs.  
**
**  This example code is provided by HID GLobal Inc. for illustrative purposes only.  
**  This example has not been thoroughly tested under all conditions.  Therefore 
**  Lumidigm Inc. cannot guarantee or imply reliability, serviceability, or 
**  functionality of this program.
**
**  This example code is provided by Lumidigm Inc. "AS IS" and without any express 
**  or implied warranties, including, but not limited to the implied warranties of 
**  merchantability and fitness for a particular purpose.
**
**	
**	Author: BCramer
**
** 
**	Copyright 2016 HID Global Corporation/ASSA ABLOY AB. ALL RIGHTS RESERVED.
**
********************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using SDKWrapper;

namespace CSharpExample
{
    class SDKBiometrics
    {
        public static uint TETHERED_PREVIEW_WIDTH = 352;
        public static uint TETHERED_PREVIEW_HEIGHT = 544;
        private static StringBuilder _ipAddress = new StringBuilder("");
        private static LumiSDKWrapper.LumiStatus _rc;
        private static string _statusMessage;
        private static uint _hHandle = 0;
        private static LumiSDKWrapper.LUMI_DEVICE dev;

        ////////////////////////////////////////////////////////////////////////
        // SDKBiometrics properties
        ////////////////////////////////////////////////////////////////////////
        public static string StatusMessage
        {
            get { return _statusMessage; }
        }
        ////////////////////////////////////////////////////////////////////////
        // SDKBiometrics functions
        ////////////////////////////////////////////////////////////////////////
        public static void OpenScanner()
        {
            try
            {
                uint nNumDevices = 0;
                _rc = LumiSDKWrapper.LumiQueryNumberDevices(ref nNumDevices, _ipAddress);
                if (_rc != LumiSDKWrapper.LumiStatus.LUMI_STATUS_OK)
                {
                    throw new Exception("Failed to query number of devices.  _rc = " + _rc);
                }
                if (nNumDevices == 0)
                {
                    throw new Exception("There are no Lumidigm 1s connected to the PC.\r\nPlease close the application and connect a Lumidigm Sensor.");
                }

                dev = new LumiSDKWrapper.LUMI_DEVICE();

                _rc = LumiSDKWrapper.LumiQueryDevice(0, ref dev);  // Query the first device

                if (_rc != LumiSDKWrapper.LumiStatus.LUMI_STATUS_OK)
                {
                    throw new Exception("Failed to query scanner.  _rc = " + _rc + "\r\n");
                }
                uint tmpHandle = 0;
                _rc = LumiSDKWrapper.LumiInit(dev.hDevHandle, ref tmpHandle);
                if (_rc != LumiSDKWrapper.LumiStatus.LUMI_STATUS_OK)
                {
                    throw new Exception("Failed to init scanner.  _rc = " + _rc + "\r\n");
                }
                _hHandle = tmpHandle;

            }
            catch (Exception err)
            {
                throw err;
            }
            finally { }
        }

        public static void CloseScanner()
        {
            try
            {
                _rc = LumiSDKWrapper.LumiClose(_hHandle);
                _rc = LumiSDKWrapper.LumiExit();
            }
            catch (Exception err)
            {
                _statusMessage += "An Error occured: " + err.ToString() + "  _rc = " + _rc + "\r\n";
                throw err;
            }
            finally { }
        }

        public static void GetWidthAndHeight(ref uint width, ref uint height)
        {
            try
            {
                uint nBPP = 0, nDPI = 0;

                _rc = LumiSDKWrapper.LumiGetImageParams(_hHandle, ref width, ref height, ref nBPP, ref nDPI);
                if (_rc != LumiSDKWrapper.LumiStatus.LUMI_STATUS_OK)
                {
                    throw new Exception("FAIL: lumiGetImageSize");
                }
                else
                {
                    _statusMessage += "PASS: Image Size = " + width.ToString() + " X " + height.ToString() + "\r\n";
                }

            }
            catch (Exception err)
            {
                _statusMessage += "An Error occured: " + err.ToString() + "\r\n";
                //CloseScanner();
                throw err;
            }
            finally { }
        }

        public static void LumiOverrideHeartbeat(bool overrideTrigger)
        {
            // Because we need another definition for LumiSetOption, we call the OverrideTrigger
            // static method on the object LumiSDKLumiSetOption.  This is because the LumiSDKWrapper
            // has the LumiSetOption defined to take the LumiPresenceDetectCallbackDelegate argument while for
            // overridding the trigger, we need it to accept an integer pointer argument instead.
            LumiSDKLumiSetOption.OverrideTrigger(_hHandle, overrideTrigger);
        }

        public static bool ExtractTemplateAvailable()
        {
            try
            {
                LumiSDKWrapper.LUMI_DEVICE_CAPS dCaps = new LumiSDKWrapper.LUMI_DEVICE_CAPS();
                _rc = LumiSDKWrapper.LumiGetDeviceCaps(_hHandle, ref dCaps);
                if (_rc != LumiSDKWrapper.LumiStatus.LUMI_STATUS_OK)
                {
                    throw new Exception("FAIL: LumiGetDeviceCaps");
                }

                if ((int)dCaps.bExtract == 0)
                {
                    return false;
                }

                return true;
            }
            catch (Exception err)
            {
                _statusMessage += "An Error occured: " + err.ToString() + "\r\n";
                //CloseScanner();
                throw err;
            }
            finally { }
        }

        public static void LumiSetPresenceDetectionThreshold(LumiSDKWrapper.LUMI_PRES_DET_THRESH thresh)
        {
            // Because we need another definition for LumiSetOption, we call the SetPresenceDetectionThreshold
            // static method on the object LumiSDKLumiSetOption.  This is because the LumiSDKWrapper
            // has the LumiSetOption defined to take the LumiPresenceDetectCallbackDelegate argument while for
            // overridding the trigger, we need it to accept an integer pointer argument instead.
            LumiSDKLumiSetOption.SetPresenceDetectionThreshold(_hHandle, thresh);
        }

        public static void LumiCaptureProcess(byte[] snapShot24bppPointer, uint width, uint height, LumiSDKWrapper.LumiPreviewCallbackDelegate callbackFunc)
        {
            try
            {
                byte[] snapShot = new byte[width * height]; // Array to hold raw data 8 bpp format 

                int spoof = 0;

                _rc = LumiSDKWrapper.LumiCapture(_hHandle, snapShot, ref spoof, callbackFunc);

                if (_rc != LumiSDKWrapper.LumiStatus.LUMI_STATUS_OK)
                {
                    throw new Exception("ERROR: lumiCaptureProcess rc = " + _rc);
                }
                else
                {
                    _statusMessage += "PASS: lumiCaptureProcess rc = " + _rc + " spoof = " + spoof + "\r\n";
                }

                ConvertRawImageTo24bpp(snapShot24bppPointer, snapShot, snapShot.Length);

            }
            catch (Exception err)
            {
                _statusMessage += "An Error occured: " + err.ToString() + "\r\n";
                //CloseScanner();
                throw err;
            }
            finally { }
        }

        public static void LumiCaptureProcess(byte[] snapShot24bppPointer, byte[] template1Pointer, ref uint templateSize1, uint width, uint height, LumiSDKWrapper.LumiPreviewCallbackDelegate callbackFunc)
        {
            try
            {
                byte[] snapShot = new byte[width * height]; // Array to hold raw data 8 bpp format 

                int spoof = 0;

                _rc = LumiSDKWrapper.LumiCaptureEx(_hHandle, snapShot, template1Pointer, ref templateSize1, ref spoof, callbackFunc);

                if (_rc != LumiSDKWrapper.LumiStatus.LUMI_STATUS_OK)
                {
                    throw new Exception("ERROR: lumiCaptureProcess rc = " + _rc);
                }
                else
                {
                    _statusMessage += "PASS: lumiCaptureProcess rc = " + _rc + " spoof = " + spoof + "\r\n";
                }

                ConvertRawImageTo24bpp(snapShot24bppPointer, snapShot, snapShot.Length);

            }
            catch (Exception err)
            {
                _statusMessage += "An Error occured: " + err.ToString() + "\r\n";
                //CloseScanner();
                throw err;
            }
            finally { }
        }

        public static void TestAPI(ImageFrm form)
        {
            try
            {
                LumiSDKWrapper.LumiStatus _rc;
                StringBuilder _ipAddress = new StringBuilder("");
                //uint _hHandle = 0;

                form.DisableControls();
                
                //get sensor type
                switch (dev.SensorType)
                {
                    case LumiSDKWrapper.LUMI_SENSOR_TYPE.VENUS:
                        form.AddResults("V30X Initialized\n");
                        break;
                    case LumiSDKWrapper.LUMI_SENSOR_TYPE.V31X:
                        form.AddResults("V31X Initialized\n");
                        break;
                    case LumiSDKWrapper.LUMI_SENSOR_TYPE.M300:
                        form.AddResults("M30X Initialized\n");
                        break;
                    case LumiSDKWrapper.LUMI_SENSOR_TYPE.M31X:
                        form.AddResults("M31X Initialized\n");
                        break;
                    case LumiSDKWrapper.LUMI_SENSOR_TYPE.M32X:
                        form.AddResults("M32X Initialized\n");
                        break;
                    default:
                        throw new Exception("Unrecognized Sensor Type");
                }

                //get version info
                LumiSDKWrapper.LUMI_VERSION ver = new LumiSDKWrapper.LUMI_VERSION();
                _rc = LumiSDKWrapper.LumiGetVersionInfo(_hHandle, ref ver);
                if (_rc != LumiSDKWrapper.LumiStatus.LUMI_STATUS_OK)
                {
                    throw new Exception("Failed to get version info.  _rc = " + _rc + "\r\n");
                }
               
                form.PrintVersion(ver);

                // GET DEVICECAPS
                LumiSDKWrapper.LUMI_DEVICE_CAPS sysCaps = new LumiSDKWrapper.LUMI_DEVICE_CAPS();
                _rc = LumiSDKWrapper.LumiGetDeviceCaps(_hHandle, ref sysCaps);
                if (_rc != LumiSDKWrapper.LumiStatus.LUMI_STATUS_OK)
                {
                    throw new Exception("Failed to get device capabilities.  _rc = " + _rc + "\r\n");
                }
                form.PrintDeviceCaps(sysCaps);
                //set PD callback off
                LumiSDKWrapper.LumiPresenceDetectCallbackDelegate del = new LumiSDKWrapper.LumiPresenceDetectCallbackDelegate(form.PresenceDetectionCallback);
                IntPtr prtDel = Marshal.GetFunctionPointerForDelegate(del);
                _rc = LumiSDKWrapper.LumiSetOption(_hHandle, LumiSDKWrapper.LUMI_OPTIONS.LUMI_OPTION_SET_PRESENCE_DET_CALLBACK, prtDel, GetIntPtrSz());
                //get processing mode
                LumiSDKWrapper.LUMI_PROCESSING_MODE procMode = new LumiSDKWrapper.LUMI_PROCESSING_MODE();
                _rc = LumiSDKWrapper.GetProcessingMode(_hHandle, ref procMode);
                if (_rc != LumiSDKWrapper.LumiStatus.LUMI_STATUS_OK && (_rc != LumiSDKWrapper.LumiStatus.LUMI_STATUS_ERROR_CMD_NOT_SUPPORTED))
                {
                    throw new Exception("Failed to get processing mode.  _rc = " + _rc + "\r\n");
                }
                //set DC options folder
                _rc = LumiSDKWrapper.LumiSetDCOptions(_hHandle, "C:\\BAD_FOLDER\\", 1);
                if (_rc != LumiSDKWrapper.LumiStatus.LUMI_STATUS_INVALID_FOLDER)
                    form.AddResults("Didn't recognize bad folder\n");
                //set DC options folder
                _rc = LumiSDKWrapper.LumiSetDCOptions(_hHandle, "C:\\", 1);
                if (_rc != LumiSDKWrapper.LumiStatus.LUMI_STATUS_OK)
                    form.AddResults("Didn't set good folder\n");

                //try out V series options
                if (dev.SensorType == LumiSDKWrapper.LUMI_SENSOR_TYPE.VENUS)
                {
                    // SET OPTION - Turn off hearbeat

                    IntPtr pHeartbeat = Marshal.AllocHGlobal(sizeof(int));// int *HeartBeat = new int;
                    Marshal.WriteInt32(pHeartbeat, 1);

                    _rc = LumiSDKWrapper.LumiSetOption(_hHandle, LumiSDKWrapper.LUMI_OPTIONS.LUMI_OPTION_SET_OVERRIDE_HEARTBEAT_DISPLAY, pHeartbeat, sizeof(int));
                    if (_rc != LumiSDKWrapper.LumiStatus.LUMI_STATUS_OK)
                    {
                        throw new Exception("Failed to override heartbeat display.  _rc = " + _rc + "\r\n");
                    }


                    form.AddResults("Flashing LEDs\n");
                    //Turn Red LED ON
                    _rc = LumiSDKWrapper.LumiSetLED(_hHandle, LumiSDKWrapper.LUMI_LED_CONTROL.LUMI_VENUS_RED_ON);
                    if (_rc != LumiSDKWrapper.LumiStatus.LUMI_STATUS_OK)
                        form.AddResults("Could not set Venus red LED to on\n");
                    System.Threading.Thread.Sleep(500);

                    //Turn Red LED OFF
                    _rc = LumiSDKWrapper.LumiSetLED(_hHandle, LumiSDKWrapper.LUMI_LED_CONTROL.LUMI_VENUS_RED_OFF);
                    if (_rc != LumiSDKWrapper.LumiStatus.LUMI_STATUS_OK)
                        form.AddResults("Could not set Venus red LED to off\n");
                    System.Threading.Thread.Sleep(500);

                    //Turn Green LED ON
                    _rc = LumiSDKWrapper.LumiSetLED(_hHandle, LumiSDKWrapper.LUMI_LED_CONTROL.LUMI_VENUS_GREEN_ON);
                    if (_rc != LumiSDKWrapper.LumiStatus.LUMI_STATUS_OK)
                        form.AddResults("Could not set Venus green LED to on\n");
                    System.Threading.Thread.Sleep(500);

                    //Turn GREEN LED OFF
                    _rc = LumiSDKWrapper.LumiSetLED(_hHandle, LumiSDKWrapper.LUMI_LED_CONTROL.LUMI_VENUS_GREEN_OFF);
                    if (_rc != LumiSDKWrapper.LumiStatus.LUMI_STATUS_OK)
                        form.AddResults("Could not set Venus green LED to off\n");
                    System.Threading.Thread.Sleep(500);

                    //Turn back on the hearbeat
                    Marshal.WriteInt32(pHeartbeat, 0);
                    _rc = LumiSDKWrapper.LumiSetOption(_hHandle, LumiSDKWrapper.LUMI_OPTIONS.LUMI_OPTION_SET_OVERRIDE_HEARTBEAT_DISPLAY, pHeartbeat, sizeof(int));
                    if (_rc != LumiSDKWrapper.LumiStatus.LUMI_STATUS_OK)
                    {
                        throw new Exception("Failed to override heartbeat display.  _rc = " + _rc + "\r\n");
                    }
                }


                // Set Option – Acq Status Callback
                form.AddResults("Calling LumiSetOption... Acq status callback\n");
                if (_rc != LumiSDKWrapper.LumiStatus.LUMI_STATUS_OK)
                    form.AddResults("Could not set ACQ status callback\n");

                uint nW = 0, nH = 0, nDPI = 0, nBPP = 0, nTemplateLength1 = 0, nTemplateLength2 = 0, nScore = 0;
                int nSpoof = 0;

                ////Turn off video Mode
                _rc = LumiSDKWrapper.LumiGetImageParams(_hHandle, ref nW, ref nH, ref nBPP, ref nDPI);
                if (_rc != LumiSDKWrapper.LumiStatus.LUMI_STATUS_OK)
                    form.AddResults("Could not get image params\n");
                else
                {
                    form.AddResults("Width: " + nW + " Height: " + nH);
                    form.AddResults("\r\nBPP: " + nBPP + " DPI: " + nDPI + "\r\n");
                }

                //Quality Map
                byte[] QualityMap = new byte[nW * nH];
                _rc = LumiSDKWrapper.LumiGetQualityMap(_hHandle, QualityMap);
                if (dev.SensorType == LumiSDKWrapper.LUMI_SENSOR_TYPE.VENUS)
                {
                    // Try to get Quality Map before capture or verify - should return LUMI_STATUS_QUALITY_MAP_NOT_GENERATED
                    if (_rc == LumiSDKWrapper.LumiStatus.LUMI_STATUS_QUALITY_MAP_NOT_GENERATED)
                        form.AddResults("Could not get quality map. This is expected.\n");
                }
                else
                {
                    // Try to get Quality Map with Mercury - should return LUMI_STATUS_ERROR_CMD_NOT_SUPPORTED
                    form.AddResults("Non V30x device. Return Code: " + _rc + "\r\n");
                }

                //Capture
                form.AddResults("Place Your Finger\n");
                byte[] pImage = new byte[nW * nH];
                byte[] pTemplate1 = new byte[5000];
                byte[] pTemplate2 = new byte[5000];

                _rc = LumiSDKWrapper.LumiCapture(_hHandle, pImage, ref nSpoof, null);

                if (_rc != LumiSDKWrapper.LumiStatus.LUMI_STATUS_OK)
                {
                    form.AddResults("Could not capture\n");
                }
                form.AddResults("Spoof: " + nSpoof + "\n");

                //check sensor health
                _rc = LumiSDKWrapper.LumiSetOption(_hHandle, LumiSDKWrapper.LUMI_OPTIONS.LUMI_OPTION_CHECK_SENSOR_HEALTH, IntPtr.Zero, 0);
                if (_rc != LumiSDKWrapper.LumiStatus.LUMI_STATUS_OK)
                {
                    form.AddResults("Could not set option\n");
                }
                // SAVE LAST CAPTURE WITH BAD FOLDER
                form.AddResults("Saving Last Capture with bad folder...\n");
                _rc = LumiSDKWrapper.LumiSaveLastCapture(_hHandle, "User1", 5, 1);
                form.AddResults("Results: " + _rc + "\n"); // Expecting LUMI_STATUS_DC_OPTIONS_NOT_SET to be returned

                // SET DC OPTIONS WITH GOOD FOLDER
                form.AddResults("Saving Last Capture with good folder...\n");
                _rc = LumiSDKWrapper.LumiSetDCOptions(_hHandle, "C:\\", 1);// 1 for true, 0 for false
                form.AddResults("Results: " + _rc + "\n");

                //CaptureEX test for invalid parameter
                form.AddResults("CaptureEX with null parameter...\n");
                _rc = LumiSDKWrapper.LumiCaptureEx(_hHandle, pImage, null, ref nTemplateLength1, ref nSpoof, null);
                if (_rc != LumiSDKWrapper.LumiStatus.LUMI_STATUS_INVALID_PARAMETER)
                {
                    form.AddResults("CaptureEX didn't return invalid parameter with a null template\n");
                }

                //And were done!
                form.EnableControls();

                // Capture Ex
                form.AddResults("CaptureEX...\n");
                _rc = LumiSDKWrapper.LumiCaptureEx(_hHandle, pImage, pTemplate1, ref nTemplateLength1, ref nSpoof, null);
                form.AddResults("Results: " + _rc + "\n");
                if ((_rc != LumiSDKWrapper.LumiStatus.LUMI_STATUS_OK) && (_rc != LumiSDKWrapper.LumiStatus.LUMI_STATUS_CANCELLED_BY_USER))
                {
                    form.AddResults("Capture failed\n");
                }
                form.AddResults("...Spoof " + nSpoof + "\n");

                //Quality Map
                _rc = LumiSDKWrapper.LumiGetQualityMap(_hHandle, QualityMap);

                if (dev.SensorType == LumiSDKWrapper.LUMI_SENSOR_TYPE.VENUS)
                {
                    // Try to get Quality Map before capture or verify - should return LUMI_STATUS_QUALITY_MAP_NOT_GENERATED
                    if (_rc != LumiSDKWrapper.LumiStatus.LUMI_STATUS_OK)
                        form.AddResults("Could not get quality map\n");
                    else
                    {
                        //save Quality map as bitmap
                        int newSize = QualityMap.Length * 3;
                        byte[] rgb = new byte[newSize];//24bpp
                        for (int i = 0; i < QualityMap.Length; i++)
                        {
                            rgb[i * 3] = QualityMap[i];
                            rgb[i * 3 + 1] = QualityMap[i];
                            rgb[i * 3 + 2] = QualityMap[i];
                        }
                        Bitmap bmp = new Bitmap((int)nW, (int)nH, PixelFormat.Format24bppRgb);
                        BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, (int)nW, (int)nH), ImageLockMode.WriteOnly, bmp.PixelFormat);
                        IntPtr ptr = bmpData.Scan0;
                        Marshal.Copy(rgb, 0, bmpData.Scan0, newSize);
                        bmp.UnlockBits(bmpData);
                        bmp.Save("QualityMap2.bmp");
                    }
                }
                else
                {
                    //should return LUMI_STATUS_ERROR_CMD_NOT_SUPPORTED
                    form.AddResults("Non V30x device. Return Code: " + _rc + "\r\n");
                }

                // save last capture
                form.AddResults("Saving Last Capture with good folder...\n");
                _rc = LumiSDKWrapper.LumiSaveLastCapture(_hHandle, "User1", 5, 1);
                form.AddResults("Results: " + _rc + "\n");

                //capture again
                form.AddResults("CaptureEX...\n");
                _rc = LumiSDKWrapper.LumiCaptureEx(_hHandle, pImage, pTemplate2, ref nTemplateLength2, ref nSpoof, null);
                form.AddResults("Results: " + _rc + "\n");
                if ((_rc != LumiSDKWrapper.LumiStatus.LUMI_STATUS_OK) && (_rc != LumiSDKWrapper.LumiStatus.LUMI_STATUS_CANCELLED_BY_USER))
                    form.AddResults("Capture failed\n");
                form.AddResults("...Spoof " + nSpoof + "\n");

                //verify
                form.AddResults("Verify...\n");
                _rc = LumiSDKWrapper.LumiVerify(_hHandle, pTemplate2, nTemplateLength2, ref nSpoof, ref nScore);
                if (_rc != LumiSDKWrapper.LumiStatus.LUMI_STATUS_OK)
                    form.AddResults("Verify failed\n");
                else
                {
                    form.AddResults("Verify Complete.  Score " + nScore + ". Spoof " + nSpoof + "\n");
                }

                //match
                form.AddResults("Match...\n");
                _rc = LumiSDKWrapper.LumiMatch(_hHandle, pTemplate1, ref nTemplateLength1, pTemplate2, ref nTemplateLength2, ref nScore, ref nSpoof);
                if (_rc != LumiSDKWrapper.LumiStatus.LUMI_STATUS_OK)
                    form.AddResults("Match failed\n");
                else
                {
                    form.AddResults("Match Complete.  Score " + nScore + "\n");
                }
                form.AddResults("API Test Complete \n");
            }
            catch (Exception err)
            {
                throw err;
            }
            finally { }

        }

        public static void LumiCaptureWithPresenceDetectionFeedback(byte[] snapShot24bppPointer, byte[] template1Pointer, ref uint templateSize1, uint width, uint height, LumiSDKWrapper.LumiPresenceDetectCallbackDelegate callbackFunc)
        {
            try
            {
                byte[] snapShot = new byte[width * height]; // Array to hold raw data 8 bpp format 

                int spoof = 0;
                // Make sure Presence Detection is on.
                // Because we need another definition for LumiSetOption, we call the SetPresenceDetectionMode
                // static method on the object LumiSDKLumiSetOption.  This is because the LumiSDKWrapper
                // has the LumiSetOption defined to take the LumiPresenceDetectCallbackDelegate argument while for
                // setting PD mode, we need it to take an integer pointer argument instead.
                LumiSDKLumiSetOption.SetPresenceDetectionMode(_hHandle, LumiSDKWrapper.LUMI_PRES_DET_MODE.LUMI_PD_ON);

                // Set the address of the presence detection callback function   
                IntPtr prtDel = Marshal.GetFunctionPointerForDelegate(callbackFunc);
                _rc = LumiSDKWrapper.LumiSetOption(_hHandle, LumiSDKWrapper.LUMI_OPTIONS.LUMI_OPTION_SET_PRESENCE_DET_CALLBACK, prtDel, GetIntPtrSz());
                _rc = LumiSDKWrapper.LumiSetDCOptions(_hHandle, "C:/Temp/", 0);
                LumiSDKWrapper.LUMI_PROCESSING_MODE procMode;
                procMode.bExtract = 1;
                procMode.bLatent = 0;
                procMode.bSpoof = 1;
                _rc = LumiSDKWrapper.SetProcessingMode(_hHandle, procMode);
                procMode.bExtract = 0;
                _rc = LumiSDKWrapper.GetProcessingMode(_hHandle, ref procMode);
                //_rc = LumiCapture(_hHandle, snapshotPointer, &spoof, null);
                _rc = LumiSDKWrapper.LumiCaptureEx(_hHandle, snapShot, template1Pointer, ref templateSize1, ref spoof, null);

                if (_rc != LumiSDKWrapper.LumiStatus.LUMI_STATUS_OK)
                {
                    throw new Exception("ERROR: lumiCaptureProcess rc = " + _rc);
                }
                else
                {
                    _statusMessage += "PASS: lumiCaptureProcess rc = " + _rc + " spoof = " + spoof + "\r\n";
                }
                LumiSDKWrapper.LumiSetOption(_hHandle, LumiSDKWrapper.LUMI_OPTIONS.LUMI_OPTION_SET_PRESENCE_DET_CALLBACK, IntPtr.Zero, GetIntPtrSz()); // size of int is 4

                ConvertRawImageTo24bpp(snapShot24bppPointer, snapShot, snapShot.Length);

            }
            catch (Exception err)
            {
                _statusMessage += "An Error occured: " + err.ToString() + "\r\n";
                //CloseScanner();
                throw err;
            }
            finally { }
        }
        public static void Match(byte[] template1Pointer, ref uint nTemp1Len, byte[] template2Pointer, ref uint nTemp2Len, ref uint score, ref int spoof)
        {
            try
            {
                List<byte[]> templates = new List<byte[]>();
                templates.Add(template1Pointer);
                templates.Add(template2Pointer);
                _rc = LumiSDKWrapper.LumiMatch(_hHandle, (byte [])templates[0], ref nTemp1Len, (byte[])templates[1], ref nTemp2Len, ref score, ref spoof);

                if (_rc != LumiSDKWrapper.LumiStatus.LUMI_STATUS_OK)
                {
                    throw new Exception("ERROR: lumiMatch rc = " + _rc);
                }
                else
                {
                    _statusMessage += "PASS: lumiMatch rc = " + _rc + "\r\n";
                    _statusMessage += "PASS: Match Score = " + score + "\r\n";
                    _statusMessage += "PASS: Match Spoof Score = " + spoof + " (should be  -1) \r\n";
                }
            }
            catch (Exception err)
            {
                _statusMessage += "An Error occured: " + err.ToString() + "\r\n";
                //CloseScanner();
                throw err;
            }
            finally { }
        }

        public static void GetConfig(ref LumiSDKWrapper.LUMI_CONFIG config)
        {
            try
            {
                _rc = LumiSDKWrapper.LumiGetConfig(_hHandle, ref config);
            }
            catch (Exception err)
            {
                _statusMessage += "An Error occured: " + err.ToString() + "  _rc = " + _rc + "\r\n";
                throw err;
            }
            finally { }
        }

        public static void SetConfig(LumiSDKWrapper.LUMI_CONFIG config)
        {
            try
            {
                _rc = LumiSDKWrapper.LumiSetConfig(_hHandle, config);
            }
            catch (Exception err)
            {
                _statusMessage += "An Error occured: " + err.ToString() + "  _rc = " + _rc + "\r\n";
                throw err;
            }
            finally { }
        }

        public static void ConvertRawImageTo24bpp(byte[] snapShot24bppPointer, byte[] snapshotPointer, int snapShotLength)
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

        ////////////////////////////////////////////////////////////////////////
        // 32 bit platform is 4, 64 bit platform is 8
        ////////////////////////////////////////////////////////////////////////
        public static uint GetIntPtrSz()
        {
            return (uint)IntPtr.Size;
        }
    }
}
