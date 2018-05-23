
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
    public class LumiInOpAPIWrapper
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int LumiAcqStatusCallbackDelegate(LumiStatus status);

        ////////////////////////////////////////////////////////////////////////
        // LumiAPI functions (C API)
        ////////////////////////////////////////////////////////////////////////        
        public const string LUMI_IN_OP_API_DLL = "LumiInOpAPI.dll";

        // LumiAPI.dll is loaded in the Main() function of the Program.cs for the 
        // CSharpExample.exe
        [DllImport(LUMI_IN_OP_API_DLL)]
        public static extern LumiStatus LumiComputeNFIQFromImage(byte[] pImage,
                                                             uint nWidth,
                                                             uint nHeight,
                                                             uint nBPP,
                                                             uint nDPI,
                                                             ref uint nNFIQ);
        [DllImport(LUMI_IN_OP_API_DLL)]
        public static extern LumiStatus LumiMatchTemplate(byte[] pProbeTemplate,
                                                    ref uint nProbeTemplateLength,
                                                    byte[] pGalleryTemplate,
                                                    ref uint nGalleryTemplateLength,
                                                    ref uint nMatchScore);


        public enum LumiStatus
        {
            LUMI_STATUS_OK = 0,			/* Operation completed successfully */
            LUMI_STATUS_ERROR_DEVICE_OPEN = 0x0001,		/* Could not find or open requested
													                biometric reader */
            LUMI_STATUS_ERROR_DEVICE_CLOSE = 0x0002,		/* Could not close biometric reader
													                or release allocated resources */
            LUMI_STATUS_ERROR_CMD_NOT_SUPPORTED = 0x0004,		/* This command is not supported on
													                current platform */
            LUMI_STATUS_ERROR_COMM_LINK = 0x0008,		/* General internal communication problem
													                prevented execution of command */
            LUMI_STATUS_ERROR_PREPROCESSOR = 0x0010,		/* Minimum thresholds for processing
													                quality were not achieved */
            LUMI_STATUS_ERROR_CALIBRATION = 0x0020,		/* Calibration cycle failed due to
													                inoperable hardware state */
            LUMI_STATUS_ERROR_BUSY = 0x0040,		/* Device is busy processing previous
													                command */
            LUMI_STATUS_ERROR_INVALID_PARAMETER = 0x0080,		/* Parameter or input is out of range */
            LUMI_STATUS_ERROR_TIMEOUT = 0x0100,		/* Event did not occur within programmed
													                time interval */
            LUMI_STATUS_ERROR_INVALID_TEMPLATE = 0x0200,		/* Supplied input template is invalid */
            LUMI_STATUS_ERROR_MEMORY_ALLOCATION = 0x0400,		/* Unable to allocate memory */
            LUMI_STATUS_ERROR_INVALID_DEVICE_ID = 0x0800,		/* Invalid device ID */
            LUMI_STATUS_ERROR_INVALID_CONNECTION_ID = 0x1000,		/* Invalid instance ID */
            LUMI_STATUS_ERROR_CONFIG_UNSUPPORTED = 0x2000,		/* Current configuration or policy does not
																    support the function that was just called */
            LUMI_STATUS_UNSUPPORTED = 0x4000,		/* The function is not currently supported by the SDK*/
            LUMI_STATUS_INTERNAL_ERROR = 0x8000,		/* An internal error occurred */
            LUMI_STATUS_INVALID_PARAMETER = 0x8001,		/* Invalid parameter or non allocated buffer passed to function*/
            LUMI_STATUS_DEVICE_TIMEOUT = 0x8002,		/* Biometric sensor is accessed by another process */
            LUMI_STATUS_INVALID_OPTION = 0x8004,		/* Invalid option, invalid argument, or invalid
																    argument size passed to function LumiSetOption*/
            LUMI_STATUS_ERROR_MISSING_SPOOFTPL = 0x8008,       /* Missing Spoof template from Composite image or ANSI
																    378 Template */
            LUMI_STATUS_CANCELLED_BY_USER = 0x8010,		/* If the LumiPresenceDetectCallback returns -2, then
																    the SDK will return this status code during capture*/
            LUMI_STATUS_INVALID_FOLDER = 0x8020,		/* Function LumiSetDCOptions returns this code if the folder
																    doesn't exist */
            LUMI_STATUS_DC_OPTIONS_NOT_SET = 0x8040,		/* Function LumiSaveLastCapture returns this code if the
																    (Data Collection) option was not set OR set to a non
																    existing folder */
            LUMI_STATUS_INCOMPATIBLE_FIRMWARE = 0x8080,	    /* The firmware loaded on the sensor is not compatible with
															        this version of the SDK. */
            LUMI_STATUS_NO_DEVICE_FOUND = 0x8100,		/* LumiInOpAPI returns this code if no device was found*/
            LUMI_STATUS_ERROR_READ_FILE = 0x8200,		/* LumiInOpAPI returns this code if could not read from file*/
            LUMI_STATUS_ERROR_WRITE_FILE = 0x8400,		/* LumiInOpAPI returns this code if could not write to file*/
            LUMI_STATUS_INVALID_FILE_FORMAT = 0x8800,       /* Function LoadBitmapFromFile in LumiInOpAPI returns this 
															        code if invalid file format is passed. */
            LUMI_STATUS_ERROR_TIMEOUT_LATENT = 0x9000,		/* Latent detection on sensor returned time out due to possible latent
															        or finger not moving between capture calls or PD cycles*/
            LUMI_STATUS_QUALITY_MAP_NOT_GENERATED = 0x9010,		/* If LumiGetQualityMap is called before a LumiCapture, LumiCaptureEX or
															        LumiVerify is called, this code returned.  Also, this code is returned if 
															        a NULL buffer is passed into LumiGetQualityMap.*/
            LUMI_STATUS_THREAD_ERROR = 0x9011,		/* More than one thread from same process attempting to enter SDK */
            LUMI_STATUS_ERROR_SENSOR_COMM_TIMEOUT = 0x9012,		/* Serious communication error occured with the sensor.  The connection handle
															        to that sensor will be closed and no further functions can be called with
															        that connection handle.*/
            LUMI_STATUS_DEVICE_STATUS_ERROR = 0x9013,               /* LumiSetOption for the LUMI_OPTION_CHECK_SENSOR_HEALTH option will return
															        this error code if a sensor status error is present.*/
            LUMI_STATUS_ERROR_BAD_INSTALLATION = 0x9014            /* If Lumidigm SDK files and folders are not in their proper relative paths or missing,
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
