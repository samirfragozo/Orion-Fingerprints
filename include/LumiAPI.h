
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


#ifndef DOXYGEN_SHOULD_SKIP_THIS	// Skip this for auto-docs.
#pragma once

#include "LumiTypes.h"

/*** Initialization  ***/

#ifdef LUMI_EXPORTS
#define DRV_EXPORT __declspec(dllexport)
#else
#define DRV_EXPORT __declspec(dllimport)
#endif

#define STDCALL __stdcall

#ifdef __cplusplus
#define __C__ "C"
#else
#define __C__
#endif

extern __C__
{
#endif

//define first plane
#define FIRST_PLANE	0x0001

///////////////////////////////////////////////////////////////////////////////
///  function  LumiQueryNumberDevices
///  Enumerates devices currently connected to system.  Use zero-based qualifier
///  returned from nNumDevices to call LumiQueryDevice in order to get a device handle.
///
///  @param [out]	   nNumDevices  Returns number of devices found.
///  @param [in]       strIPList 	IP Addresses to query for Jupiter device.  NULL Termination between devices.
///									Double NULL Terminated at end.  This SDK does not support Jupiter devices at this time.
///
///  @return LumiStatus See error code description.
///
///  @remarks After getting number of devices, you can call LumiQueryDevice with 0-based index in order to get a device handle
///	 as well as device capabilities.
///
///  @see LumiQueryDevice
///
///  @author Lumidigm Inc. @date 11/27/2006
///////////////////////////////////////////////////////////////////////////////

DRV_EXPORT LumiStatus STDCALL LumiQueryNumberDevices(uint &nNumDevices,  char* const strIPList);

///////////////////////////////////////////////////////////////////////////////
///  function  LumiQueryDevice
///  Retrieves LUMI_DEVICE information for a given 0-based nDevice index.  The LUMI_DEVICE_HANDLE
///  returned in dev can be used to get a connection Handle to the device using the LumiInit call.
///
///  @param [in]    deviceToQuery     0-based index of device to query retrieved from LumiQueryNumberDevices.
///  @param [out]	dev				  LUMI_DEVICE structure.
///
///  @return LumiStatus See Error Code description.
///
///  @remarks Pass the dev parameter into LumiInit to get a device connection handle
///
///  @see LumiQueryNumberDevices
///
///  @author Lumidigm Inc. @date 11/27/2006
///////////////////////////////////////////////////////////////////////////////

DRV_EXPORT LumiStatus STDCALL LumiQueryDevice(uint deviceToQuery, LUMI_DEVICE& dev);

///////////////////////////////////////////////////////////////////////////////
///  function  LumiInit
///  Attempts to retrieve a connection handle using a device ID.
///
///  @param [in]       hDevHandle     Returned by LumiQueryDevice
///  @param [in, out]  hHandle		  Upon success, a valid connection handle is returned here
///
///  @return LumiStatus  See Error Code description.
///
///  @remarks None.
///
///  @see LumiQueryDevice
///
///  @author Lumidigm Inc. @date 11/27/2006
///////////////////////////////////////////////////////////////////////////////

DRV_EXPORT LumiStatus STDCALL LumiInit(LUMI_DEVICE_HANDLE hDevHandle,
									   LUMI_HANDLE& hHandle);


///////////////////////////////////////////////////////////////////////////////
///  function  LumiClose
///  Closes a connection handle.
///
///  @param [in]       hHandle     Connection handle received by LumiInit
///
///  @return LumiStatus See Error Code description.
///
///  @remarks Every call to LumiInit must be followed at some point by a call to LumiClose
///
///  @see LumiInit
///
///  @author Lumidigm Inc. @date 11/27/2006
///////////////////////////////////////////////////////////////////////////////

DRV_EXPORT LumiStatus STDCALL LumiClose(const LUMI_HANDLE hHandle);



///////////////////////////////////////////////////////////////////////////////
///  function  LumiExit
///  Called to shut down device interface to all Lumidigm Devices.  To re-establish
///  the interface, a call to LumiQueryNumDevices must be made.
///
///  @return LumiStatus See Error Code description.
///
///  @remarks Every call to LumiQueryNumDevices must be followed by a call to LumiExit at some point.
///
///  @see LumiQueryNumDevices
///
///  @author Lumidigm Inc. @date 11/27/2006
///////////////////////////////////////////////////////////////////////////////
DRV_EXPORT LumiStatus STDCALL LumiExit();

///////////////////////////////////////////////////////////////////////////////
///  function  LumiLoadConfig
///  Load a Configuration.
///
///  @param [in]       hHandle		Connection Handle received from LumiInit
///  @param [in]       szConfigID   Configuration Identifier
///  @param [out]	   config		LUMI_CONFIG structure.
///
///  @return LumiStatus See Error Code description.
///
///  @remarks Loads a preset configuration.
///
///  @see LumiSetConfiguration
///
///  @author Lumidigm Inc. @date 11/27/2006
///////////////////////////////////////////////////////////////////////////////

DRV_EXPORT LumiStatus STDCALL LumiLoadConfig(const LUMI_HANDLE hHandle,
											 const char* szConfigID,
											 LUMI_CONFIG& config);

// Get a LUMI_CONFIG struct.
///////////////////////////////////////////////////////////////////////////////
///  function  LumiGetConfig
///  Get a Configuration.
///
///  @param [in]       hHandle const LUMI_HANDLE    Connection Handle received from LumiInit
///  @param [in, out]  config LUMI_CONFIG &    LUMI_CONFIG structure.
///
///  @return LumiStatus See Error Code description.
///
///  @remarks returns a configuration.
///
///  @see LumiSetConfig
///
///////////////////////////////////////////////////////////////////////////////
DRV_EXPORT LumiStatus STDCALL LumiGetConfig( const LUMI_HANDLE hHandle, LUMI_CONFIG& config );

///////////////////////////////////////////////////////////////////////////////
///  function LumiSetConfig
///  The user may modify any of the configuration parameters and send to the
///  device for immediate configuration state change.  This is a volatile
///  configuration change and will be lost upon power reset.
///
///  @param[in] hHandle Handle to an open biometric device
///  @param[in] config  Configuration
///  @return LumiStatus See Error Code description.
///
///  @remarks None.
///
///
///  @author Lumidigm Inc. @date 11/27/2006
///////////////////////////////////////////////////////////////////////////////
DRV_EXPORT LumiStatus STDCALL LumiSetConfig( const LUMI_HANDLE hHandle,
											 LUMI_CONFIG const config );


///////////////////////////////////////////////////////////////////////////////
///  function  LumiCapture
///  Acquires image, returns spoof info if supported.
///
///  @param [in]   hHandle		Connection Handle received from LumiInit
///  @param [out]  pOutputImage Preallocated buffer which will contain fingerprint image.  Allocate nWidth * nHeight from LumiGetImageParams.
///  @param [out]  nSpoof		Spoof value. -1 if spoof not supported.
///  @param [in]   func			If you would like a Raw image preview, you can pass in a callback function.  NULL if not required.
///
///  @return LumiStatus See Error Code description.
///
///  @remarks None.
///
///
///  @author Lumidigm Inc. @date 11/27/2006
///////////////////////////////////////////////////////////////////////////////
DRV_EXPORT LumiStatus STDCALL LumiCapture(	const LUMI_HANDLE hHandle,
											uchar* pOutputImage,
											int& nSpoof,
											LumiPreviewCallback func = 0x00);

///////////////////////////////////////////////////////////////////////////////
///  function	LumiCaptureEx
///  Acquires image, extracts template, returns spoof info if supported.
///
///  @param [in]	hHandle				Connection Handle received from LumiInit.
///  @param [out]	pOutputImage		Preallocated buffer which will contain fingerprint image.  Allocate nWidth * nHeight from LumiGetImageParams.
///  @param [in]	pTemplate			Extracted Template.
//   @param [out]	nTemplateLength		Length of extracted template, in Bytes.
///  @param [out]	nSpoof				Spoof value. -1 if spoof not supported.
///  @param [in]	func				If you would like a Raw image preview, you can pass in a callback function.  NULL if not required.
///
///  @return LumiStatus See Error Code description.
///
///  @remarks None.
///
///
///  @author Lumidigm Inc. @date 11/27/2006
///////////////////////////////////////////////////////////////////////////////
DRV_EXPORT LumiStatus STDCALL LumiCaptureEx(const LUMI_HANDLE hHandle,
											uchar* pOutputImage,
											uchar* pTemplate,
											uint& nTemplateLength,
											int& nSpoof,
											LumiPreviewCallback func = 0x00);


///////////////////////////////////////////////////////////////////////////////
///  function  LumiVerify
///  Captures fingerprint image, extracts template, verifies against input template,
///  returns score.
///
///  @param [in]	hHandle			Connection Handle received from LumiInit
///  @param [in]	pInputTemplate	Input Template.
///  @param [in]	nInputTemplateLength	Input Template Length, in Bytes.
///  @param [out]	nSpoof			Spoof value. -1 if spoof not supported.
///  @param [out]	nScore			Score obtained by matching acquired fingerprint vs passed-in template.
///
///  @return LumiStatus See Error Code description.
///
///  @remarks LumiVerify depends on whether the appropriate template extractor and matcher is available.
///
///  @see LumiCapture, LumiExtract
///
///  @author Lumidigm Inc. @date 11/27/2006
///////////////////////////////////////////////////////////////////////////////
DRV_EXPORT LumiStatus STDCALL LumiVerify(	const LUMI_HANDLE hHandle,
											uchar* const pInputTemplate,
											uint  nInputTemplateLength,
											int&  nSpoof,
											uint& nScore);
///////////////////////////////////////////////////////////////////////////////
///  global public  LumiMatch
///  Matches two templates.
///
///  @param [in]  hHandle					Connection Handle received from LumiInit
///  @param [in]  pProbeTemplate			Probe Template
///  @param [in]  nProbeTemplateLength		Length of Probe Template, in Bytes.
///  @param [in]  pGalleryTemplate			Gallery Template
///  @param [in]  nGalleryTemplateLength	Length of Gallery Template, in Bytes.
///  @param [out] nScore					Score of matched templates.
///  @param [out] nSpoof					Spoof is not supported for this call.
///
///  @return LumiStatus See Error Code description.
///
///  @remarks Appropriate matcher needs to be installed for this functionality.
///
///  @see LumiExtract
///
///  @author Lumidigm Inc. @date 11/27/2006
///////////////////////////////////////////////////////////////////////////////
DRV_EXPORT LumiStatus STDCALL LumiMatch(	const LUMI_HANDLE hHandle,
											uchar* const pProbeTemplate,
											uint& nProbeTemplateLength,
											uchar* const pGalleryTemplate,
											uint& nGalleryTemplateLength,
											uint& nScore,
											int& nSpoof);

///////////////////////////////////////////////////////////////////////////////
///  function  LumiExtract
///  Extracts a biometric template from a fingerprint image.
///
///  @param [in]       hHandle const    Connection Handle received from LumiInit
///  @param [in]       pImageBuffer     8-BPP image to pass in.
///  @param [in]       nWidth			Width of input image.
///  @param [in]       nHeight			Height of input image.
///  @param [in]       nDPI				DPI of input image.
///  @param [out]	   pTemplate		Template buffer.
///  @param [out]	   nTemplateLength  Size of buffer used, in Bytes.
///  @param [in]       eTemplateType    Template type to extract.
///
///  @return LumiStatus See Error Code description.
///
///  @remarks Successful completion of this function relies on appropriate extractor installed.
///
///  @see None.
///
///  @author Lumidigm Inc. @date 11/27/2006
///////////////////////////////////////////////////////////////////////////////
DRV_EXPORT LumiStatus STDCALL LumiExtract(	const LUMI_HANDLE hHandle,
											uchar* const pImageBuffer,
											const uint nWidth,
											const uint nHeight,
											const uint nDPI,
											uchar* pTemplate,
											uint& nTemplateLength);

///////////////////////////////////////////////////////////////////////////////
///  function  LumiGetImageParams
///  Retrieves image parameters of images returned by driver.
///
///  @param [in]   hHandle Connection Handle received from LumiInit
///  @param [out]  nWidth  Width of image.
///  @param [out]  nHeight Height of image.
///  @param [out]  nBPP    Bits per Pixel of image (RGB format)
///  @param [out]  nDPI    DPI of image.
///
///  @return LumiStatus See Error Code description.
///
///  @remarks None.
///
///  @see None.
///
///  @author Lumidigm Inc. @date 11/27/2006
///////////////////////////////////////////////////////////////////////////////
DRV_EXPORT LumiStatus STDCALL LumiGetImageParams(const LUMI_HANDLE hHandle,
												uint& nWidth,
												uint& nHeight,
												uint& nBPP,
												uint& nDPI);

///////////////////////////////////////////////////////////////////////////////
///  function  LumiGetDeviceCaps
///  Retrieves device capabilities of device associated with this connection.
///
///  @param [in]       hHandle    Connection Handle received from LumiInit>
///  @param [out]	   dCaps      Device Capabilities structure.
///
///  @return LumiStatus See Error Code description.
///
///  @remarks None.
///
///  @see None.
///
///  @author Lumidigm Inc. @date 11/27/2006
///////////////////////////////////////////////////////////////////////////////
DRV_EXPORT LumiStatus STDCALL LumiGetDeviceCaps(const LUMI_HANDLE hHandle,
												LUMI_DEVICE_CAPS& dCaps);

///////////////////////////////////////////////////////////////////////////////
///  function  LumiGetDeviceState
///  Get current state of device.
///
///  @param [in]		hHandle     Connection Handle received from LumiInit
///  @param [out]		dState      Returned device state.
///
///  @return LumiStatus See Error Code description.
///
///  @remarks None.
///
///  @see None.
///
///  @author Lumidigm Inc. @date 11/27/2006
///////////////////////////////////////////////////////////////////////////////
DRV_EXPORT LumiStatus STDCALL LumiGetDeviceState(const LUMI_HANDLE hHandle,
												LUMI_DEVICE_STATE& dState);


///////////////////////////////////////////////////////////////////////////////
///  function  LumiSetLED
///  Flash an LED.  Does not capture an image
///
///  @param [in]       hHandle		Connection Handle received from LumiInit
///  @param [in]       LED			LED to fire.
///
///  @return LumiStatus See Error Code description.
///
///  @remarks None.
///
///  @see None.
///
///  @author Lumidigm Inc. @date 11/27/2006
///////////////////////////////////////////////////////////////////////////////
DRV_EXPORT LumiStatus STDCALL LumiSetLED(const LUMI_HANDLE hHandle, LUMI_LED_CONTROL LED);

///////////////////////////////////////////////////////////////////////////////
///  function  LumiSnapShot
///  Captures a live image.
///
///  @param [in]       hHandle     Connection Handle received from LumiInit
///  @param [in]       image      Image buffer which will hold the captured preview image.
///  @param [in]       exposure     exposure Valid programmable gain amplifier (0-255)
///  @param [in]       gain     gain Valid exposure settings 20 micro-seconds to 2 seconds in
///					   ~20 micro-second increments
///
///  @return LumiStatus See Error Code description.
///
///  @remarks NOT to be used with ExtractTemplate.  Image to be used for preview only.
///
///  @see LumiCapture.
///
///  @author Lumidigm Inc. @date 11/27/2006
///////////////////////////////////////////////////////////////////////////////
DRV_EXPORT LumiStatus STDCALL LumiSnapShot( const LUMI_HANDLE hHandle,
											uchar * const image,
											int exposure,
											int gain );

///////////////////////////////////////////////////////////////////////////////
///  function  LumiSetLiveMode
///  Captures a live image.
///
///  @param [in]       hHandle LUMI_HANDLE    Connection Handle received from LumiInit
///  @param [in]       mode uint			  1 = Turn on.  0 = Turn off.
///  @param [in]	   func					  Function pointer that receives live images. Ignored if Mode = 0.
///
///  @return LumiStatus See Error Code description.
///
///  @remarks
///
///  @see
///
///  @author Lumidigm Inc. @date 11/27/2006
///////////////////////////////////////////////////////////////////////////////
DRV_EXPORT LumiStatus STDCALL LumiSetLiveMode(const LUMI_HANDLE hHandle,
											  uint mode,
											  LumiPreviewCallback func = 0x00);

///////////////////////////////////////////////////////////////////////////////
///  function  LumiGetVersionInfo
///  Retrieves version information of processing component, SDK, Device firmware
///
///  @param [in]       hHandle      Connection Handle received from LumiInit
///  @param [in, out]  hVer      	Version information struct.
///
///  @return LumiStatus See Error Code description.
///
///  @remarks None.
///
///  @see None.
///
///  @author Lumidigm Inc. @date 11/27/2006
///////////////////////////////////////////////////////////////////////////////
DRV_EXPORT LumiStatus STDCALL LumiGetVersionInfo (	const LUMI_HANDLE hHandle,
													LUMI_VERSION& hVer);

///////////////////////////////////////////////////////////////////////////////
///  function  LumiSetOption
///  Sets an option using a predetermined list of LUMI_OPTIONS
///
///  @param [in]       hHandle			Connection Handle received from LumiInit
///  @param [in]       option			Option that is being set.
///  @param [in]       pArgument		Pointer to the value the option is being set to.
///  @param [in]       nArgumentSize	Size of pArgument.
///
///  @return LumiStatus See Error Code description.
///
///  @remarks
///
///  @author Lumidigm Inc. @date 11/27/2006
///////////////////////////////////////////////////////////////////////////////
DRV_EXPORT LumiStatus STDCALL LumiSetOption(const LUMI_HANDLE hHandle,
											LUMI_OPTIONS option,
											void* pArgument,
											uint nArgumentSize);

///////////////////////////////////////////////////////////////////////////////
///  function  LumiSetDCOptions
///  Turns on data collection mode, enabling the ability to save and encrypt data
///  from the last capture made by the SDK.
///
///  @param [in]       hHandle					Connection Handle received from LumiInit
///  @param [in]       pFolderToSaveTo			Folder which will contain saved files.
///  @param [in]       bOverwriteExistingFiles	If the file exists, overwrite?
///
///  @return LumiStatus See Error Code description.
///
///  @remarks
///
///  @author Lumidigm Inc. @date 11/27/2006
///////////////////////////////////////////////////////////////////////////////
DRV_EXPORT LumiStatus STDCALL LumiSetDCOptions(const LUMI_HANDLE hHandle,
											   const char* pFolderToSaveTo,
											   bool bOverwriteExistingFiles);

///////////////////////////////////////////////////////////////////////////////
///  function  LumiSaveLastCapture
///  Ecrypts data collected from the last capture and saves it to a file.
///
///  @param [in]       hHandle			Connection Handle received from LumiInit
///  @param [in]       pUserIdentifier  Unique identifier for a specific user
///  @param [in]       nFinger		    Finger identifier, 0-9
///  @param [in]       nInstance		Impression number (same userId/finger).
///
///  @return LumiStatus See Error Code description.
///
///  @remarks
///
///  @author Lumidigm Inc. @date 11/27/2006
///////////////////////////////////////////////////////////////////////////////
DRV_EXPORT LumiStatus STDCALL LumiSaveLastCapture(const LUMI_HANDLE hHandle,
												  const char* pUserIdentifier,
												  uint  nFinger,
												  uint  nInstance);

///////////////////////////////////////////////////////////////////////////////
///  function  LumiGetQualityMap
///  Returns the quality map generated from the last capture
///
///  @param [in]       hHandle			Connection Handle received from LumiInit
///  @param [out]      pQualityMap		Preallocated buffer which will contain the quality map. Allocate nWidth * nHeight from LumiGetImageParams.

///
///  @return LumiStatus See Error Code description.
///
///  @remarks
///
///  @author Lumidigm Inc. @date 11/19/2008
///////////////////////////////////////////////////////////////////////////////
DRV_EXPORT LumiStatus STDCALL LumiGetQualityMap(const LUMI_HANDLE hHandle,
											uchar* pQualityMap);


///////////////////////////////////////////////////////////////////////////////
///  function  GetProcessingMode
///  Gets the processing mode of sensor
///
///  @param [in]       hHandle			Connection Handle received from LumiInit
///  @param [out]      processingMode	LUMI_PROCESSING_MODE structure

///
///  @return LumiStatus See Error Code description.
///
///  @remarks
///
///  @author Lumidigm Inc. @date 5/15/2009
///////////////////////////////////////////////////////////////////////////////
DRV_EXPORT LumiStatus STDCALL GetProcessingMode(const LUMI_HANDLE hHandle,
											LUMI_PROCESSING_MODE& processingMode);

///////////////////////////////////////////////////////////////////////////////
///  function  SetProcessingMode
///  Sets the processing mode of sensor
///
///  @param [in]       hHandle			Connection Handle received from LumiInit
///  @param [in]       processingMode	LUMI_PROCESSING_MODE structure

///
///  @return LumiStatus See Error Code description.
///
///  @remarks
///
///  @author Lumidigm Inc. @date 5/15/2009
///////////////////////////////////////////////////////////////////////////////
DRV_EXPORT LumiStatus STDCALL SetProcessingMode(const LUMI_HANDLE hHandle,
											LUMI_PROCESSING_MODE processingMode);


///////////////////////////////////////////////////////////////////////////////
///  function  LumiDetectFinger
///  Sets the processing mode of sensor
///
///  @param [in]       hHandle			Connection Handle received from LumiInit
///  @param [out]      nStatus			LUMI_ACQ_STATUS that is returned
///  @param [in]       func				LumiAcqStatusCallback func. Pass NULL if you want
///										LumiDetectFinger to return right away.  Pass in
///										a callback function if you want to "poll" for the
///										status.  If the callback function is passed in, the
///										LumiDetectFinger becomes a blocking call.  You can 
///										cancel by returning -2 from the callback function.

///
///  @return LumiStatus See Error Code description.
///
///  @remarks	
///
///  @author Lumidigm Inc. @date 2/24/2011
///////////////////////////////////////////////////////////////////////////////
DRV_EXPORT LumiStatus STDCALL LumiDetectFinger(const LUMI_HANDLE hHandle, 
											   LUMI_ACQ_STATUS &nStatus, 
											   LumiAcqStatusCallback func = 0x00);



// Other calls

DRV_EXPORT LumiStatus STDCALL LumiGetSecureInterface( char* pGUID, void** pInterface);
DRV_EXPORT LumiStatus STDCALL LumiReleaseInterface(void* pInterface);

#ifndef DOXYGEN_SHOULD_SKIP_THIS
} /* End of extern */
#endif