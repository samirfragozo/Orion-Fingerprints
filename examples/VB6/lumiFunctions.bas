Attribute VB_Name = "lumiFunctions"
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
' You are free to use this example code to generate similar functionality
' tailored to your own specific needs.
'
' This example code is provided by Lumidigm Inc. for illustrative purposes only.
' This example has not been thoroughly tested under all conditions.  Therefore
' Lumidigm Inc. cannot guarantee or imply reliability, serviceability, or
' functionality of this program.
'
' This example code is provided by Lumidigm Inc. “AS IS” and without any express
' or implied warranties, including, but not limited to the implied warranties of
' merchantability and fitness for a particular purpose.
'
'
' Lumi Function Declarations - LumiAPI.h
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

Declare Function queryNumberDevices Lib "..\..\bin\LumiAPI.dll" Alias "_LumiQueryNumberDevices@8" (ByRef nNumDevices As Long _
                                                                , ByVal strIPList As String) _
                                                                As Long
                                                                                            
Declare Function queryDevice Lib "..\..\bin\LumiAPI.dll" Alias "_LumiQueryDevice@8" (ByVal deviceToQuery As Long _
                                                                , ByRef device As LUMI_DEVICE) _
                                                                As Long

Declare Function init Lib "..\..\bin\LumiAPI.dll" Alias "_LumiInit@8" (ByVal hDevHandle As Long _
                                                                , ByRef hHandle As Long) _
                                                                As Long
                                                                
Declare Function getVersionInfo Lib "..\..\bin\LumiAPI.dll" Alias "_LumiGetVersionInfo@8" (ByVal hHandle As Long _
                                                                , ByRef hVersion As LUMI_VERSION) _
                                                                As Long
                                                                                    
Declare Function getDeviceCaps Lib "..\..\bin\LumiAPI.dll" Alias "_LumiGetDeviceCaps@8" (ByVal hHandle As Long _
                                                                , ByVal deviceCaps As Long) _
                                                                As Long

Declare Function getConfig Lib "..\..\bin\LumiAPI.dll" Alias "_LumiGetConfig@8" (ByVal hHandle As Long _
                                                                , ByRef configuration As LUMI_CONFIG) _
                                                                As Long

Declare Function setConfig Lib "..\..\bin\LumiAPI.dll" Alias "_LumiSetConfig@16" (ByVal hHandle As Long _
                                                                , ByVal tmplateType As Long _
                                                                , ByVal transInfo As Long _
                                                                , ByVal triggerTimeOut As Long) _
                                                                As Long
                                                                
Declare Function setLED Lib "..\..\bin\LumiAPI.dll" Alias "_LumiSetLED@8" (ByVal hHandle As Long _
                                                                , ByVal nLED As Long) _
                                                                As Long

Declare Function getImageParams Lib "..\..\bin\LumiAPI.dll" Alias "_LumiGetImageParams@20" (ByVal hHandle As Long _
                                                                , ByRef nWidth As Long _
                                                                , ByRef nHeight As Long _
                                                                , ByRef nBPP As Long _
                                                                , ByRef nDPI As Long) _
                                                                As Long
                                                                
Declare Function captureImage Lib "..\..\bin\LumiAPI.dll" Alias "_LumiCapture@16" (ByVal hHandle As Long _
                                                                , ByVal outputImage As Long _
                                                                , ByRef nSpoof As Long _
                                                                , ByVal callback As Long) _
                                                                As Long
                                                                
Declare Function captureImageEx Lib "..\..\bin\LumiAPI.dll" Alias "_LumiCaptureEx@24" (ByVal hHandle As Long _
                                                                , ByVal outputImage As Long _
                                                                , ByVal template As Long _
                                                                , ByRef templateLength As Long _
                                                                , ByRef spoof As Long _
                                                                , ByVal callback As Long) _
                                                                As Long

Declare Function matchTemplate Lib "..\..\bin\LumiAPI.dll" Alias "_LumiMatch@28" (ByVal hHandle As Long _
                                                                , ByVal probeTemplate As Long _
                                                                , ByRef nProbeTemplateLength As Long _
                                                                , ByVal galleryTemplate As Long _
                                                                , ByRef nGalleryTemplateLength As Long _
                                                                , ByRef nScore As Long _
                                                                , ByRef nSpoof As Long) _
                                                                As Long
                                                                
Declare Function takeSnapShot Lib "..\..\bin\LumiAPI.dll" Alias "_LumiSnapShot@16" (ByVal hHandle As Long _
                                                                , ByVal image As Long _
                                                                , ByVal nExposure As Long _
                                                                , ByVal nGain As Long) _
                                                                As Long

Declare Function closeDevice Lib "..\..\bin\LumiAPI.dll" Alias "_LumiClose@4" (ByVal hHandle As Long) As Long
 
'Function for GetTickCount
Declare Function GetTickCount Lib "kernel32.dll" () As Long
 
'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
' LUMI SDK Enums and Structs.  These need to match the enums and structs in the SDK as defined in the LumiTypes.h
' Converted hex values from the lumitype.h file to longs
'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
Enum LUMI_STATUS
    LUMI_STATUS_OK = 0                                  '/* Operation completed successfully */
    LUMI_STATUS_ERROR_DEVICE_OPEN = 1                 '/* Could not find or open requested biometric reader */
    LUMI_STATUS_ERROR_DEVICE_CLOSE = 2                '/* Could not close biometric reader or release allocated resources */
    LUMI_STATUS_ERROR_CMD_NOT_SUPPORTED = 4           '/* This command is not supported on current platform */
    LUMI_STATUS_ERROR_COMM_LINK = 8                   '/* General internal communication problem prevented execution of command */
    LUMI_STATUS_ERROR_PREPROCESSOR = 16               '/* Minimum thresholds for processing quality were not achieved */
    LUMI_STATUS_ERROR_CALIBRATION = 32                '/* Calibration cycle failed due to inoperable hardware state */
    LUMI_STATUS_ERROR_BUSY = 64                       '/* Device is busy processing previous command */
    LUMI_STATUS_ERROR_INVALID_PARAMETER = 128          '/* Parameter or input is out of range */
    LUMI_STATUS_ERROR_TIMEOUT = 256                   '/* Event did not occur within programmed time interval */
    LUMI_STATUS_ERROR_INVALID_TEMPLATE = 512          '/* Supplied input template is invalid */
    LUMI_STATUS_ERROR_MEMORY_ALLOCATION = 1024         '/* Unable to allocate memory */
    LUMI_STATUS_ERROR_INVALID_DEVICE_ID = 2048         '/* Invalid device ID */
    LUMI_STATUS_ERROR_INVALID_CONNECTION_ID = 4096    '/* Invalid instance ID */
    LUMI_STATUS_ERROR_CONFIG_UNSUPPORTED = 8192       '/* Current configuration or policy does not
                                                            'support the function that was just called */
    LUMI_STATUS_UNSUPPORTED = 16384                    '/* The function is not currently supported by the SDK*/
    LUMI_STATUS_INTERNAL_ERROR = 32768                 '/* An internal error occurred */
    LUMI_STATUS_INVALID_PARAMETER = 32769              '/* Invalid parameter or non allocated buffer passed to function*/
    LUMI_STATUS_DEVICE_TIMEOUT = 32770                 '/* Biometric sensor is accessed by another process */
    LUMI_STATUS_INVALID_OPTION = 32772                 '/* Invalid option, invalid argument, or invalid
                                                                'argument size passed to function LumiSetOption*/
    LUMI_STATUS_ERROR_MISSING_SPOOFTPL = 32776         '/* Missing Spoof template from Composite image or ANSI
                                                                '378 Template */
    LUMI_STATUS_CANCELLED_BY_USER = 32784              '/* If the LumiPresenceDetectCallback returns -2, then
                                                                'the SDK will return this status code during capture*/
    LUMI_STATUS_INVALID_FOLDER = 32800                 '/* Function LumiSetDCOptions returns this code if the folder
                                                                'doesn 't exist */
    LUMI_STATUS_DC_OPTIONS_NOT_SET = 32832             '/* Function LumiSaveLastCapture returns this code if the
                                                                '(Data Collection) option was not set OR set to a non
                                                                'existing folder */
    LUMI_STATUS_INCOMPATIBLE_FIRMWARE = 32896          '/* The firmware loaded on the sensor is not compatible with
                                                                 'this version of the SDK. */
    LUMI_STATUS_NO_DEVICE_FOUND = 33024                   '/* LumiInOpAPI returns this code if no device was found*/
    LUMI_STATUS_ERROR_READ_FILE = 33280                   '/* LumiInOpAPI returns this code if could not read from file*/
    LUMI_STATUS_ERROR_WRITE_FILE = 33792                  '/* LumiInOpAPI returns this code if could not write to file*/
    LUMI_STATUS_INVALID_FILE_FORMAT = 34816               '/* Function LoadBitmapFromFile in LumiInOpAPI returns this
                                                               'code if invalid file format is passed. */
    LUMI_STATUS_ERROR_TIMEOUT_LATENT = 36864              '/* Latent detection on sensor returned time out due to possible latent
                                                                 'or finger not moving between capture calls or PD cycles*/
    LUMI_STATUS_QUALITY_MAP_NOT_GENERATED = 36880         '/* If LumiGetQualityMap is called before a LumiCapture, LumiCaptureEX or
                                                               'LumiVerify is called, this code returned.  Also, this code is returned if
                                                               'a NULL buffer is passed into LumiGetQualityMap.*/
    LUMI_STATUS_THREAD_ERROR = 36881                      '/* More than one thread from same process attempting to enter SDK */
    LUMI_STATUS_ERROR_SENSOR_COMM_TIMEOUT = 36882         '/* Serious communication error occured with the sensor.  The connection handle
                                                               'to that sensor will be closed and no further functions can be called with
                                                               'that connection handle.*/
    LUMI_STATUS_DEVICE_STATUS_ERROR = 36883

End Enum


Enum ProcFlag
   CONF_PROCESS_DSP = &H1           '/* Generate Template DSP */
   CONF_PROCESS_LOCAL = &H2         '/* Generate Template Local */
End Enum

Enum TransportInfo
    CONF_TRANS_DEFAULT = 0
    CONF_TRANS_ETHERNET = &H1
    CONF_TRANS_USB = &H2
    CONF_TRANS_RS232 = &H4
    CONF_TRANS_WYGEN = &H8
    CONF_TRANS_END = &HFFFF
End Enum

Type LUMI_DEVICE_CAPS
    bCaptureImages As Byte
    bExtract As Byte
    bMatch As Byte
    bIdentify As Byte
    bSpoof As Byte
    eTemplate As Long
    eTransInfo As Long
    nWidth As Long
    nHeight As Long
    nDPI As Long
    nImageFormat As Long
    eProcessLocation As Long
End Type

Type LUMI_DEVICE
    hDevHandle As Long
    dCaps As LUMI_DEVICE_CAPS
    strIdentifier As String * 256
End Type

Enum TplInfo
    CONF_TPL_DEFAULT = 0
    CONF_TPL_NEC_3 = &H1
    CONF_TPL_NEC_4 = &H2
    CONF_TPL_ANSI387 = &H4
    END_TEMPLATES = &H8
End Enum

Enum LUMI_PROCESS_LOCATION
    LUMI_PROCESS_NONE = 0
    LUMI_PROCESS_SENSOR = 1
    LUMI_PROCESS_PC = 2
End Enum



Type LUMI_CONFIG
    'template configuration
    eTplInfoTemplateType As Long
    'Transport Configuration
    eTransInfo As Long
    'Timeout
    nTriggerTimeout As Long
End Type

Type LUMI_VERSION
    sdkVersion As String * 256
    fwrVersion As String * 256
    prcVersion As String * 256
    tnsVersion As String * 256
End Type





