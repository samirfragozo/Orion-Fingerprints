
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


using System.Runtime.InteropServices;

namespace LumiInOpAPIWrapper
{
    public class LumiInOpApiWrapper
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int LumiAcqStatusCallbackDelegate(LumiStatus status);

        ////////////////////////////////////////////////////////////////////////
        // LumiAPI functions (C API)
        ////////////////////////////////////////////////////////////////////////        
        public const string LumiInOpApiDll = "LumiInOpAPI.dll";

        // LumiAPI.dll is loaded in the Main() function of the Program.cs for the 
        // CSharpExample.exe
        [DllImport(LumiInOpApiDll)]
        public static extern LumiStatus LumiComputeNFIQFromImage(byte[] pImage,
                                                             uint nWidth,
                                                             uint nHeight,
                                                             uint nBpp,
                                                             uint nDpi,
                                                             ref uint nNfiq);
        [DllImport(LumiInOpApiDll)]
        public static extern LumiStatus LumiMatchTemplate(byte[] pProbeTemplate,
                                                    ref uint nProbeTemplateLength,
                                                    byte[] pGalleryTemplate,
                                                    ref uint nGalleryTemplateLength,
                                                    ref uint nMatchScore);


        public enum LumiStatus
        {
            LumiStatusOk = 0,			/* Operation completed successfully */
            LumiStatusErrorDeviceOpen = 0x0001,		/* Could not find or open requested
													                biometric reader */
            LumiStatusErrorDeviceClose = 0x0002,		/* Could not close biometric reader
													                or release allocated resources */
            LumiStatusErrorCmdNotSupported = 0x0004,		/* This command is not supported on
													                current platform */
            LumiStatusErrorCommLink = 0x0008,		/* General internal communication problem
													                prevented execution of command */
            LumiStatusErrorPreprocessor = 0x0010,		/* Minimum thresholds for processing
													                quality were not achieved */
            LumiStatusErrorCalibration = 0x0020,		/* Calibration cycle failed due to
													                inoperable hardware state */
            LumiStatusErrorBusy = 0x0040,		/* Device is busy processing previous
													                command */
            LumiStatusErrorInvalidParameter = 0x0080,		/* Parameter or input is out of range */
            LumiStatusErrorTimeout = 0x0100,		/* Event did not occur within programmed
													                time interval */
            LumiStatusErrorInvalidTemplate = 0x0200,		/* Supplied input template is invalid */
            LumiStatusErrorMemoryAllocation = 0x0400,		/* Unable to allocate memory */
            LumiStatusErrorInvalidDeviceId = 0x0800,		/* Invalid device ID */
            LumiStatusErrorInvalidConnectionId = 0x1000,		/* Invalid instance ID */
            LumiStatusErrorConfigUnsupported = 0x2000,		/* Current configuration or policy does not
																    support the function that was just called */
            LumiStatusUnsupported = 0x4000,		/* The function is not currently supported by the SDK*/
            LumiStatusInternalError = 0x8000,		/* An internal error occurred */
            LumiStatusInvalidParameter = 0x8001,		/* Invalid parameter or non allocated buffer passed to function*/
            LumiStatusDeviceTimeout = 0x8002,		/* Biometric sensor is accessed by another process */
            LumiStatusInvalidOption = 0x8004,		/* Invalid option, invalid argument, or invalid
																    argument size passed to function LumiSetOption*/
            LumiStatusErrorMissingSpooftpl = 0x8008,       /* Missing Spoof template from Composite image or ANSI
																    378 Template */
            LumiStatusCancelledByUser = 0x8010,		/* If the LumiPresenceDetectCallback returns -2, then
																    the SDK will return this status code during capture*/
            LumiStatusInvalidFolder = 0x8020,		/* Function LumiSetDCOptions returns this code if the folder
																    doesn't exist */
            LumiStatusDcOptionsNotSet = 0x8040,		/* Function LumiSaveLastCapture returns this code if the
																    (Data Collection) option was not set OR set to a non
																    existing folder */
            LumiStatusIncompatibleFirmware = 0x8080,	    /* The firmware loaded on the sensor is not compatible with
															        this version of the SDK. */
            LumiStatusNoDeviceFound = 0x8100,		/* LumiInOpAPI returns this code if no device was found*/
            LumiStatusErrorReadFile = 0x8200,		/* LumiInOpAPI returns this code if could not read from file*/
            LumiStatusErrorWriteFile = 0x8400,		/* LumiInOpAPI returns this code if could not write to file*/
            LumiStatusInvalidFileFormat = 0x8800,       /* Function LoadBitmapFromFile in LumiInOpAPI returns this 
															        code if invalid file format is passed. */
            LumiStatusErrorTimeoutLatent = 0x9000,		/* Latent detection on sensor returned time out due to possible latent
															        or finger not moving between capture calls or PD cycles*/
            LumiStatusQualityMapNotGenerated = 0x9010,		/* If LumiGetQualityMap is called before a LumiCapture, LumiCaptureEX or
															        LumiVerify is called, this code returned.  Also, this code is returned if 
															        a NULL buffer is passed into LumiGetQualityMap.*/
            LumiStatusThreadError = 0x9011,		/* More than one thread from same process attempting to enter SDK */
            LumiStatusErrorSensorCommTimeout = 0x9012,		/* Serious communication error occured with the sensor.  The connection handle
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
    }

       
}
