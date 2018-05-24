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
using System.Text;

namespace SDKWrapper
{
    public static class LumiSdkWrapper
    {


        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LumiPreviewCallbackDelegate(IntPtr pOutputImage,
                                                            int width,
                                                            int height,
                                                            int imgNum);
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int LumiAcqStatusCallbackDelegate(LumiAcqStatus status);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int LumiPresenceDetectCallbackDelegate(IntPtr pImage,
                                                            int width,
                                                            int height,
                                                            uint status);

        ////////////////////////////////////////////////////////////////////////
        // LumiAPI functions (C API)
        ////////////////////////////////////////////////////////////////////////        
        public const string LumiApiDll = "LumiAPI.dll";

        // LumiAPI.dll is loaded in the Main() function of the Program.cs for the 
        // CSharpExample.exe
        [DllImport(LumiApiDll)]
        public static extern LumiStatus LumiQueryNumberDevices(ref uint nNumDevices,  
                                                                StringBuilder strIpList);
  
        [DllImport(LumiApiDll)]
        public static extern LumiStatus LumiQueryDevice(uint deviceToQuery, 
                                                            ref LumiDevice dev);
        
        [DllImport(LumiApiDll)]
        public static extern LumiStatus LumiInit(uint hDevHandle,
                                                    ref uint hHandle);

        
        [DllImport(LumiApiDll)]
        public static extern LumiStatus LumiGetImageParams(uint hHandle,
                                                    ref uint nWidth,
                                                    ref uint nHeight,
                                                    ref uint nBpp,
                                                    ref uint nDpi);

        [DllImport(LumiApiDll)]
        public static extern LumiStatus LumiCapture(uint hHandle, 
                                                    byte[] pOutputImage,
                                                    ref int nSpoof,
                                                    LumiPreviewCallbackDelegate pCallbackFunc);

        [DllImport(LumiApiDll)]
        public static extern LumiStatus LumiCaptureEx(uint hHandle,
                                                    byte[] pOutputImage,
                                                    byte[] pTemplate,
                                                    ref uint nTemplateLength,
                                                    ref int nSpoof,
                                                    LumiPreviewCallbackDelegate pCallbackFunc);

        [DllImport(LumiApiDll)]
        public static extern LumiStatus LumiSetOption(uint hHandle,
                                                    LumiOptions option,
                                                    IntPtr pArgument,
                                                    uint nArgumentSize);

        [DllImport(LumiApiDll)]//new
        public static extern LumiStatus LumiSetDCOptions(uint hHandle,
											   string pFolderToSaveTo,
											   byte bOverwriteExistingFiles);//byte to make sure its a C++ bool

        [DllImport(LumiApiDll)]//new
        public static extern LumiStatus LumiSaveLastCapture(uint hHandle,
												  string pUserIdentifier,
												  uint  nFinger,
												  uint  nInstance);

        [DllImport(LumiApiDll)]//new
        public static extern LumiStatus LumiGetQualityMap(uint hHandle, byte[] pQualityMap);

        [DllImport(LumiApiDll)]//new
        public static extern LumiStatus GetProcessingMode(uint hHandle, ref LumiProcessingMode processingMode);

        [DllImport(LumiApiDll)]//new
        public static extern LumiStatus SetProcessingMode(uint hHandle, LumiProcessingMode processingMode);
											
        [DllImport(LumiApiDll)]//new
        public static extern LumiStatus LumiDetectFinger(uint hHandle,
                                               ref LumiAcqStatus nStatus, 
											   LumiAcqStatusCallbackDelegate func);

        [DllImport(LumiApiDll)]//new
        public static extern LumiStatus LumiVerify(uint hHandle,
                                                    byte[] pInputTemplate,
                                                    uint nInputTemplateLength,
                                                    ref int nSpoof,
                                                    ref uint nScore);

        [DllImport(LumiApiDll)]
        public static extern LumiStatus LumiMatch(uint hHandle,
                                                    byte[] pProbeTemplate,
                                                    ref uint nProbeTemplateLength,
                                                    byte[] pGalleryTemplate,
                                                    ref uint nGalleryTemplateLength,
                                                    ref uint nMatchScore,
                                                    ref int nSpoofScore);

        [DllImport(LumiApiDll)]//new
        public static extern LumiStatus LumiExtract(uint hHandle,
                                                    byte[] pImageBuffer,
                                                    uint nWidth,
                                                    uint nHeight,
                                                    uint nDpi,
                                                    byte[] pTemplate,
                                                    ref uint nTemplateLength);

        // LumiSetOption declared for the Presence Detection callback funtion.
        // A more generic declaration for LumiSetOption is in LumiSDKSetOption.cs.

        [DllImport(LumiApiDll)]
        public static extern LumiStatus LumiGetDeviceCaps(uint hHandle, ref LumiDeviceCaps dCaps);

        [DllImport(LumiApiDll)]//new
        public static extern LumiStatus LumiGetDeviceState(uint hHandle, ref LumiDeviceState dState);

        [DllImport(LumiApiDll)]//new
        public static extern LumiStatus LumiSetLED(uint hHandle, LumiLedControl led);

        [DllImport(LumiApiDll)]//new
        public static extern LumiStatus LumiSnapShot(uint hHandle,
											         byte[] image,
											         int exposure,
											         int gain );
        [DllImport(LumiApiDll)]//new
        public static extern LumiStatus LumiSetLiveMode(uint hHandle, uint mode, LumiPreviewCallbackDelegate pCallbackFunc);

        [DllImport(LumiApiDll)]//new
        public static extern LumiStatus LumiGetVersionInfo (uint hHandle, ref LumiVersion hVer);

        [DllImport(LumiApiDll)]
        public static extern LumiStatus LumiGetConfig(uint hHandle, ref LumiConfig config);

        [DllImport(LumiApiDll)]
        public static extern LumiStatus LumiSetConfig(uint hHandle, LumiConfig config);
       
        [DllImport(LumiApiDll)]
        public static extern LumiStatus LumiClose(uint hHandle);

        [DllImport(LumiApiDll)]
        public static extern LumiStatus LumiExit();

        ///////////////////////////////////////////////////////////
        // Define needed LumiTypes (found in LumiTypes.h)
        ///////////////////////////////////////////////////////////
        /**********************************************/


        public enum LumiStatus
        {
            LumiStatusOk							= 0,			/* Operation completed successfully */
	        LumiStatusErrorDeviceOpen			= 0x0001,		/* Could not find or open requested
													                biometric reader */
	        LumiStatusErrorDeviceClose			= 0x0002,		/* Could not close biometric reader
													                or release allocated resources */
	        LumiStatusErrorCmdNotSupported		= 0x0004,		/* This command is not supported on
													                current platform */
	        LumiStatusErrorCommLink				= 0x0008,		/* General internal communication problem
													                prevented execution of command */
	        LumiStatusErrorPreprocessor			= 0x0010,		/* Minimum thresholds for processing
													                quality were not achieved */
	        LumiStatusErrorCalibration			= 0x0020,		/* Calibration cycle failed due to
													                inoperable hardware state */
	        LumiStatusErrorBusy					= 0x0040,		/* Device is busy processing previous
													                command */
	        LumiStatusErrorInvalidParameter		= 0x0080,		/* Parameter or input is out of range */
	        LumiStatusErrorTimeout				= 0x0100,		/* Event did not occur within programmed
													                time interval */
	        LumiStatusErrorInvalidTemplate		= 0x0200,		/* Supplied input template is invalid */
	        LumiStatusErrorMemoryAllocation		= 0x0400,		/* Unable to allocate memory */
	        LumiStatusErrorInvalidDeviceId		= 0x0800,		/* Invalid device ID */
	        LumiStatusErrorInvalidConnectionId	= 0x1000,		/* Invalid instance ID */
            LumiStatusErrorConfigUnsupported    = 0x2000,		/* Current configuration or policy does not
																    support the function that was just called */
            LumiStatusUnsupported                 = 0x4000,		/* The function is not currently supported by the SDK*/
            LumiStatusInternalError              = 0x8000,		/* An internal error occurred */
            LumiStatusInvalidParameter           = 0x8001,		/* Invalid parameter or non allocated buffer passed to function*/
            LumiStatusDeviceTimeout              = 0x8002,		/* Biometric sensor is accessed by another process */
            LumiStatusInvalidOption              = 0x8004,		/* Invalid option, invalid argument, or invalid
																    argument size passed to function LumiSetOption*/
            LumiStatusErrorMissingSpooftpl      = 0x8008,       /* Missing Spoof template from Composite image or ANSI
																    378 Template */
            LumiStatusCancelledByUser           = 0x8010,		/* If the LumiPresenceDetectCallback returns -2, then
																    the SDK will return this status code during capture*/
            LumiStatusInvalidFolder              = 0x8020,		/* Function LumiSetDCOptions returns this code if the folder
																    doesn't exist */
            LumiStatusDcOptionsNotSet          = 0x8040,		/* Function LumiSaveLastCapture returns this code if the
																    (Data Collection) option was not set OR set to a non
																    existing folder */            
            LumiStatusIncompatibleFirmware       = 0x8080,	    /* The firmware loaded on the sensor is not compatible with
															        this version of the SDK. */
            LumiStatusNoDeviceFound             = 0x8100,		/* LumiInOpAPI returns this code if no device was found*/
            LumiStatusErrorReadFile             = 0x8200,		/* LumiInOpAPI returns this code if could not read from file*/
            LumiStatusErrorWriteFile            = 0x8400,		/* LumiInOpAPI returns this code if could not write to file*/
            LumiStatusInvalidFileFormat         = 0x8800,       /* Function LoadBitmapFromFile in LumiInOpAPI returns this 
															        code if invalid file format is passed. */
            LumiStatusErrorTimeoutLatent        = 0x9000,		/* Latent detection on sensor returned time out due to possible latent
															        or finger not moving between capture calls or PD cycles*/
            LumiStatusQualityMapNotGenerated   = 0x9010,		/* If LumiGetQualityMap is called before a LumiCapture, LumiCaptureEX or
															        LumiVerify is called, this code returned.  Also, this code is returned if 
															        a NULL buffer is passed into LumiGetQualityMap.*/
            LumiStatusThreadError                = 0x9011,		/* More than one thread from same process attempting to enter SDK */
            LumiStatusErrorSensorCommTimeout   = 0x9012,		/* Serious communication error occured with the sensor.  The connection handle
															        to that sensor will be closed and no further functions can be called with
															        that connection handle.*/
            LumiStatusDeviceStatusError = 0x9013,               /* LumiSetOption for the LUMI_OPTION_CHECK_SENSOR_HEALTH option will return
															        this error code if a sensor status error is present.*/
            LumiStatusErrorBadInstallation = 0x9014            /* If Lumidigm SDK files and folders are not in their proper relative paths or missing,
														            this error code will be returned.  If the plugin folder is missing, this error
														            code will be returned.  If the SPM folder is missing, this error is returned.
														            NOTE: This error code may be returned after the a call to the LumiQueryNumberDevices 
														            function or from other functions - depending on the situation.
														            NOTE: If the MercuryDvc.dll, SDvc.dll, or the VenusDvc.dll are removed from
														            the plugin folder, no error will be returned.  However, if you have a Lumidigm
														            sensor that requires one of these dlls, the LumiQueryNumberDevices function
														            will return 0 devices.
														            As of August 2011, the MercuryDvc.dll is required by the M300 and M301 sensors,
														            the SDvc.dll is required by the M31x and the V31x sensors, the VenusDvc.dll is
														            required by the V300 and V301 sensors.	
														            */
        }

        public enum LumiProcessLocation
        {
	        LumiProcessNone = 0,
	        LumiProcessSensor	= 1,
	        LumiProcessPc	= 2
        }

        public enum TplInfo
        {
	        ConfTplDefault			= 0x0000,		/* Template type unknown */
	        ConfTplNec3				= 0x0001,
	        ConfTplNec4				= 0x0002,
	        ConfTplAnsi378			= 0x0003,
	        EndTemplates
        }
        /********************************************************/
        /*		Vid Stream	(used with LumiSetLiveMode 			*/
        /********************************************************/

        public const int LumiVidOff = 0;
        public const int LumiVidOn = 1;


        public enum TransportInfo
        {
	        ConfTransDefault	 = 0x0000,
	        ConfTransUsb       = 0x0001,
	        ConfTransEthernet  = 0x0002,
	        ConfTransRs232	 = 0x0004,
	        ConfTransWygen     = 0x0008,
	        ConfTransEnd		 = 0xFFFF
        }

        public enum LumiPresDetThresh
        {
	        LumiPdSensitivityLow = 0,
	        LumiPdSensitivityMedium,
	        LumiPdSensitivityHigh
        } 
        
        public enum LumiPresDetMode
        {
            LumiPdOn = 0,
            LumiPdOff = 1
        }   

        public enum LumiOptions
        {
            LumiOptionSetOverrideHeartbeatDisplay = 0,
            LumiOptionSetPresenceDetMode = 1,
            LumiOptionSetPresenceDetThresh = 2,
            LumiOptionSetAcqStatusCallback = 3,
            LumiOptionSetPresenceDetCallback = 4,
            LumiOptionSetPresenceDetColor = 5,
            LumiOptionResetDevice = 6,
            LumiOptionCheckSensorHealth = 7
        }
        
        [StructLayout(LayoutKind.Sequential)]
        public struct LumiDeviceCaps
        {
            public byte bCaptureImage;          // Represents c++ bool
            public byte bExtract;               // Represents c++ bool
            public byte bMatch;                 // Represents c++ bool
            public byte bIdentify;              // Represents c++ bool
            public byte bSpoof;                 // Represents c++ bool
            public TplInfo eTemplate;			// Template Configuration
            public TransportInfo eTransInfo;	// Transport Configuration
            // Composite Image Size
            public uint m_nWidth;
            public uint m_nHeight;
            public uint m_nDPI;
            public uint m_nImageFormat;
            // Process Location
            public LumiProcessLocation eProcessLocation;
        }
        
        public enum LumiDeviceFlag
        {
	        DeviceStateUnknown = 0,
	        DeviceStateOk,
	        DeviceStateBusy,
	        DeviceStateError
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct LumiDeviceState
        {
            public LumiDeviceFlag eFlag;
            public uint Device_Temp;
            public uint Trigger_Timeout;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct LumiVersion
        {
	        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string sdkVersion;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string fwrVersion;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string prcVersion;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string tnsVersion;
        }
        public enum LumiSensorType
        {
            Venus = 0,			/* Venus series sensors		*/
            M100 = 1,			/* Mercury M100 sensors		*/
            M300 = 2,			/* Mercury M300 sensors		*/
            M31X = 3,			/* M31X sensors		*/
            V31X = 4,			/* V31X sensors		*/
            V371 = 5,			/* V371 sensors		*/
            M32X = 8			/* Mercury M32X sensors		*/
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct LumiDevice
        {
            public uint hDevHandle;						            
            public LumiDeviceCaps dCaps;                                
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]   
            public string strIdentifier;
            public LumiSensorType SensorType;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct LumiConfig
        {
            public TplInfo eTemplateType;       
            public TransportInfo eTransInfo; 
            public uint nTriggerTimeout;
        }

        public enum LumiLedControl
        {
	        LumiJupiterLed00 = 0,
	        LumiJupiterLed01,
	        LumiJupiterLed02,
	        LumiJupiterLed03,
	        LumiJupiterLed04,
	        LumiJupiterLed05,
	        LumiJupiterLed06,
	        LumiJupiterLed07,
	        LumiJupiterLed08,
	        LumiJupiterLed09,
	        LumiJupiterLed10,
	        LumiJupiterLed11,
	        LumiVenusAllOff = 1024,
	        LumiVenusGreenOn,
	        LumiVenusGreenOff,
	        LumiVenusRedOn,
	        LumiVenusRedOff,
	        LumiVenusGreenCycleOn,
	        LumiVenusGreenCycleOff,
	        LumiVenusRedCycleOn,
	        LumiVenusRedCycleOff
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct LumiProcessingMode
        {
	      public byte bExtract;				/* Turn on or off minutia extraction (true = on, false = off)*/
          public byte bSpoof;				/* Turn on or off spoof	(true = on, false = off)			*/
          public byte bLatent;				/* Turn on or off latent detection (true = on, false = off)	*/
        }

        public enum LumiAcqStatus
        {
	        LumiAcqDone = 0,
	        LumiAcqProcessingDone,
	        LumiAcqBusy,
	        LumiAcqTimeout,
	        LumiAcqNoFingerPresent,
	        LumiAcqMoveFingerUp,
	        LumiAcqMoveFingerDown,
	        LumiAcqMoveFingerLeft,
	        LumiAcqMoveFingerRight,
	        LumiAcqFingerPositionOk,
	        LumiAcqCancelledByUser,
	        LumiAcqTimeoutLatent,
	        LumiAcqFingerPresent,
	        LumiAcqNoop = 99	
        }

       //typedef unsigned long LUMI_HANDLE;
    }
}
