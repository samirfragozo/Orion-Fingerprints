
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

#include "LumiInOpAPI.h"
#include "stdafx.h"
#include "LumiAPI.h"

#include "conio.h"
#include "Windows.h"
#include <fstream>

// Timeout
int g_Timeout = 0;
// Start Time of acquisition
DWORD g_dwStartTime = 0;
// Function to allow user to pick which device to use
bool PickDevice(uint& nDevicePicked);
// Function to demonstrate LumiCaptureEX and LumiMatch
bool RunMatchTest();
// Function that tests API
bool TestAPI();
// Display error codes
void PrintReturnCode(LumiStatus rc);
// Get error message str
char* GetReturnCode(LumiStatus rc);
// Display Device Capabilities
void PrintDeviceCaps(LUMI_DEVICE_CAPS sysCaps);
// Callback function for LumiSetOption - AcqStatusCallback
int AcqStatusCallback(unsigned int nStatus);
// Callback function for Live Mode
void VideoCallbackFunc(unsigned char* pImage, int nWidth, int nHeight, int nNum);
// Gets string for sensor type
const char* MarshalSensorType(LUMI_SENSOR_TYPE type);
//Get the Serial number and Product ID
void GetSerialNumberAndProdID(LUMI_DEVICE dev,int &nSerial,int &nProdID);
// g_bDetectFinger is used to determine if we have called LumiDetectFinger for the AcqStatusCallback
bool g_bDetectFinger;
// Detect fingers for M31X and V31X sensors
void DetectFinger(LUMI_DEVICE dev, LUMI_HANDLE connHandle);
// Example function of how to open re open the SDK after a connection is lost
bool OpenLumiSDK(LUMI_DEVICE& dev, LUMI_HANDLE& handle, bool bReOpenSDK);

//////////////////////////
void SaveImage(const char* filename, unsigned char* pImage, uint width, uint height);
uchar* Pack(uchar* pDst, void* pSrc, uint nSize);
uchar* Unpack(void* pDst, uchar* pSrc, uint nSize);

LUMI_SENSOR_TYPE g_sensorType;

int _tmain(int argc, _TCHAR* argv[])
{
	bool bStayInWhile = true;
	while(bStayInWhile)
	{
		fprintf(stdout, "\r\n\r\nChoose Which Function To Run (Q to quit):");
		fprintf(stdout, "\r\n---------------------------------------");
		fprintf(stdout, "\r\n1:   Run Match Test");
		fprintf(stdout, "\r\n2:   Test API");
		fprintf(stdout, "\r\nQ:   Quit");
		fprintf(stdout, "\r\n");
		
		int ch = _getch();			
		switch(toupper(ch))
		{
			case '1':
				{					
					if(!RunMatchTest())		// Captures two images and templates and then matches the templates.
					{
						return -1;
					}
				} break;
			case '2':
				{
					if(!TestAPI())			// Test SDK API
					{
						return -2;
					}
				} break;
			case 'Q':
				{			
					return -1;
				} break;				
			default:				
				{	
					continue;					
				} break;		
		}
		
	}
	return 0;
}

bool PickDevice(uint& nDevicePicked)
{
	LumiStatus			rc = LUMI_STATUS_OK;	
	LUMI_DEVICE			dev;
	uint				nNumDevices = 0;
	int					nDev = -1;

	// QUERY NUMBER DEVICES - Always the first call to make to the Lumidigm SDK API
	fprintf(stdout, "\nQuerying Devices.  This may take several seconds...");
	rc = LumiQueryNumberDevices(nNumDevices, NULL);
	
	if(rc != LUMI_STATUS_OK)
	{
		PrintReturnCode(rc);
		return false;
	}

	fprintf(stdout, "\nFound %d Devices.", nNumDevices);

	if(nNumDevices < 1) return false;

	// Get user input as to which device to use
	if(nDev == -1)
	{
		bool bStayInWhile = true;
		while(bStayInWhile)
		{
			fprintf(stdout, "\r\n\r\nChoose Which Device To Use (Q to quit):");
			fprintf(stdout, "\r\n---------------------------------------");
			if(nNumDevices > 10) nNumDevices = 10;  // Keep the number of devices no more than 10
			for(int ii = 0 ; ii < (int)nNumDevices ; ii++)
			{
				rc = LumiQueryDevice(ii, dev);
				if(rc == LUMI_STATUS_OK)
				{
					fprintf(stdout, "\r\n%d: SensorType %s\tIdString %s",dev.hDevHandle, MarshalSensorType(dev.SensorType), dev.strIdentifier);
				}
				else
				{
					char* strRC = GetReturnCode(rc);
					fprintf(stdout, "\r\n%d:  Device unavailable because of %s error.",ii,strRC);
				}
			}
			fprintf(stdout, "\r\nQ: Quit");
			fprintf(stdout, "\r\n");
			
			int ch = _getch();			
			switch(toupper(ch))
			{
				case 'Q':
					{			
						return false;
					} break;				
				default:				
					{						
						nDev = ch - 48;  // Subtract ASCII value for 0
						if(nDev < 0 || nDev > 9)
						{
							continue; 
						}
						else 
						{
							bStayInWhile = false;
						}
					} break;		
			}
			
		}
	}
	nDevicePicked = nDev;
	return true;
}

//The OpenLumiSDK() function demonstrates how to properly open a connection to the SDK after handling 
//a LUMI_STATUS_ERROR_SENSOR_COMM_TIMEOUT.  This function also will open the SDK without an error condition
//if the bReOpenSDK parameter is false.
bool OpenLumiSDK(LUMI_DEVICE& dev, LUMI_HANDLE& handle, bool bReOpenSDK)
{	
	LumiStatus rc = LUMI_STATUS_OK;
	if(bReOpenSDK)
	{
		rc = LumiClose(handle);	// Atempt to close handle.  If handle not valide, 
								// LUMI_STATUS_ERROR_INVALID_DEVICE_ID will be returned
								// which is OK.
		rc = LumiExit();		// LumiExit() Must be called to free memory.
		if(rc != LUMI_STATUS_OK) return false;
	}
	uint nDevices = 0;
	bool bStayInLoop = true;
	uint nIterations = 0;
	while(1)
	{
		rc = LumiQueryNumberDevices(nDevices, NULL);
		if(rc != LUMI_STATUS_OK) return false;

		if(nDevices > 0) break;
		rc = LumiExit();	// Must have this LumiExit(), or LumiQueryNumberDevices
							// will return incorrect number of devices.
		if(rc != LUMI_STATUS_OK) return false;
		nIterations++;
		if(nIterations == 80) return false; // If after 80 iterations (20 seconds) we still don't
											// have a device, we give up.
		Sleep(250);		// Sleep to alow time for sensor to come back up
	}

	rc = LumiQueryDevice(0, dev);	
	if(rc != LUMI_STATUS_OK) 
	{
		return false;
	}

	rc = LumiInit(dev.hDevHandle, handle);
	if(rc != LUMI_STATUS_OK)
	{
		return false;
	}
	// Perform any additional setup on the sensor here
	return true;
}


bool RunMatchTest()
{
	LumiStatus			rc = LUMI_STATUS_OK;
	LUMI_DEVICE			dev;
	LUMI_HANDLE			connHandle = 0;
	LUMI_DEVICE_CAPS	sysCaps;
	uint				nDev, nW, nH, nDPI, nBPP, nTemplateLength1, nTemplateLength2, nScore;
	int					nSpoof;

	if(!PickDevice(nDev)) return false;
	if(nDev == -1)
	{
		return false;
	}

	// QUERY DEVICE
	rc = LumiQueryDevice(nDev, dev);	
	if(rc != LUMI_STATUS_OK) 
	{
		PrintReturnCode(rc);
		return false;
	}

	// INIT	
	rc = LumiInit(dev.hDevHandle, connHandle);
	if(rc != LUMI_STATUS_OK)
	{
		PrintReturnCode(rc);
		return false;
	}

	// Get Device Capabilities
	rc = LumiGetDeviceCaps(connHandle, sysCaps);
	if(rc != LUMI_STATUS_OK)
	{
		PrintReturnCode(rc);
		return false;
	}
	if(sysCaps.bExtract == false)
	{
		fprintf(stdout, "The sensor configuration does not support template extract and match.\n");
		LumiClose(connHandle);
		LumiExit();
		return false;
	}


	// Set Option – Acq Status Callback
	rc = LumiSetOption(connHandle, LUMI_OPTION_SET_ACQ_STATUS_CALLBACK, AcqStatusCallback, sizeof(LumiAcqStatusCallback));
	if(rc != LUMI_STATUS_OK)
	{
		PrintReturnCode(rc);
		return false;
	}

	// Get Image Parameters
	rc = LumiGetImageParams(connHandle, nW, nH, nBPP, nDPI);	
	if(rc != LUMI_STATUS_OK)
	{
		PrintReturnCode(rc);
		return false;
	}

	// Allocate image buffer
	uchar* pImage	  = new uchar[nW * nH];
	// Allocate template buffers
	uchar* pTemplate1 = new uchar[5000];
	uchar* pTemplate2 = new uchar[5000];


	// Capture image and extract template
#define __MULT_CAPT 0
#if __MULT_CAPT
	for(int ii = 0; ii < 100; ii++)
	{
#endif
	fprintf(stdout, "\nPlace finger one on sensor.\n");	
	bool bCOMM_TIMOUT_OCCURED = false;

	//If you want to test the OpenLumiSDK function, unplug the sensor and plug it back in right after 
	//the following LumiCaptureEx is called.
	//The following check for LUMI_STATUS_ERROR_SENSOR_COMM_TIMEOUT demonstrates the correct way to
	//re-establish a connection with the sensor if a communication error occurs.
	rc = LumiCaptureEx(connHandle, pImage, pTemplate1, nTemplateLength1, nSpoof, NULL);
	if(rc != LUMI_STATUS_OK)
	{
		PrintReturnCode(rc);
		if(rc == LUMI_STATUS_ERROR_SENSOR_COMM_TIMEOUT)
		{
			bCOMM_TIMOUT_OCCURED = true;
			bool bError = false;
			DWORD dwStartTime = GetTickCount();
			fprintf(stdout, "\n\nRe-Opening SDK\n\n");
			bError = true;
			bool bOpenSDKRC = OpenLumiSDK(dev, connHandle, true);			
			DWORD dwDuration = GetTickCount() - dwStartTime;	
			if(bOpenSDKRC == true)
			{
				fprintf(stdout, "\n\nSDK re-opened.  Duration = %d\n\n", dwDuration);
			}
			else
			{
				fprintf(stdout, "\n\nFAILED to re-open SDK.  Duration = %d\n\n", dwDuration);
				return false;
			}
			
		}
		else
		{
			return false;
		}
	}	
	fprintf(stdout, "\n...Spoof %d\n",nSpoof);

	DetectFinger(dev, connHandle);  
	

	// Capture second image and extract second template
	fprintf(stdout, "\nPlace finger two on sensor.\n");
	rc = LumiCaptureEx(connHandle, pImage, pTemplate2, nTemplateLength2, nSpoof, NULL);
	if(rc != LUMI_STATUS_OK)
	{
		PrintReturnCode(rc);
		return false;
	}	
	fprintf(stdout, "\n...Spoof %d\n",nSpoof);

	DetectFinger(dev, connHandle);  

	// MATCH
	if(bCOMM_TIMOUT_OCCURED == false)
	{		
		rc = LumiMatch(connHandle, pTemplate1, nTemplateLength1, pTemplate2, nTemplateLength2, nScore, nSpoof);
		if(rc != LUMI_STATUS_OK)
		{
			PrintReturnCode(rc);
			return false;
		}	
		fprintf(stdout, "\n\nMatch Score = %d\n\n", nScore);
	}
	else
	{
		fprintf(stdout, "\n\nUnable to perform match because first capture failed dut to COMM TIMOUT.");
	}
#if __MULT_CAPT
	}
#endif

	// CLOSE
	rc = LumiClose(connHandle);
	if(rc != LUMI_STATUS_OK)
	{
		PrintReturnCode(rc);
		return false;
	}	

	// EXIT
	rc = LumiExit();
	if(rc != LUMI_STATUS_OK)
	{
		PrintReturnCode(rc);
		return false;
	}	

	// Clean up
	if(pImage) { delete[] pImage; pImage = NULL; }
	if(pTemplate1) { delete[] pTemplate1; pTemplate1 = NULL; }
	if(pTemplate2) { delete[] pTemplate2; pTemplate2 = NULL; }

	fprintf(stdout, "Match completed.\n");

	return true;
}

bool TestAPI()
{
	LUMI_DEVICE			dev;
	LUMI_HANDLE			connHandle = 0;
	LumiStatus			rc = LUMI_STATUS_OK;
	LUMI_VERSION		ver;
	LUMI_DEVICE_CAPS	sysCaps;
	LUMI_CONFIG			devConfig;
	LUMI_PROCESSING_MODE procMode;
	DWORD				dwStartTime;

	g_bDetectFinger = false;

	uint nDev = 0;
	if(!PickDevice(nDev)) return false;

	// QUERY DEVICE
	fprintf(stdout, "\nQuerying Device %d...", nDev);
	rc = LumiQueryDevice(nDev, dev);
	PrintReturnCode(rc);
	if(rc != LUMI_STATUS_OK) return false;

	// Determine sensor type
	g_sensorType = dev.SensorType;

	// INIT
	fprintf(stdout, "\nInitializing Device %d...", nDev);
	rc = LumiInit(dev.hDevHandle, connHandle);
	PrintReturnCode(rc);
	if(rc != LUMI_STATUS_OK) return false;
	fprintf(stdout, "\nInit Returned Connection Handle %d.", connHandle);

	// GET VERSION INFO
	fprintf(stdout, "\nCalling LumiGetVersionInfo...");
	rc = LumiGetVersionInfo(connHandle,ver);
	PrintReturnCode(rc);
	if(rc != LUMI_STATUS_OK) return false;
	fprintf(stdout, "\nVersion Info SDK(%s) FW(%s) PROC(%s) CONF(%s)\r\n", ver.sdkVersion, ver.fwrVersion, ver.prcVersion, ver.tnsVersion);

	// Get Serial Number
	int nSerial, nProdID;
	GetSerialNumberAndProdID(dev,nSerial,nProdID);
	fprintf(stdout, "\nSerial: %d ProductID: %d\r\n",nSerial, nProdID);

	// GET DEVICECAPS
	fprintf(stdout, "\nCalling LumiGetDeviceCaps...");
	rc = (LumiStatus)LumiGetDeviceCaps(connHandle, sysCaps);
	PrintReturnCode(rc);
	if(rc != LUMI_STATUS_OK) return false;
	PrintDeviceCaps(sysCaps);

	// GET PROCESSING MODE
	fprintf(stdout, "\nCalling GetProcessingMode...");
	rc = (LumiStatus)GetProcessingMode(connHandle, procMode);
	PrintReturnCode(rc);
	if((rc != LUMI_STATUS_OK) && (rc != LUMI_STATUS_ERROR_CMD_NOT_SUPPORTED)) return false;

	// SET DC OPTIONS WITH BAD FOLDER
	fprintf(stdout,"\nSetting Data Collection Options with bad folder...");
	rc = (LumiStatus)LumiSetDCOptions(connHandle, "C:\\BAD_FOLDER\\", true);
	PrintReturnCode(rc);
	if(rc != LUMI_STATUS_INVALID_FOLDER) return false;
	
	if(g_sensorType == VENUS || g_sensorType == V31X)
	{
		// SET OPTION - Turn off hearbeat
		fprintf(stdout, "\nCalling LumiSetOption to turn off heart beat...");
		int nHeartbeat = 1;
		rc = LumiSetOption(connHandle, LUMI_OPTION_SET_OVERRIDE_HEARTBEAT_DISPLAY, &nHeartbeat, sizeof(int));
		PrintReturnCode(rc);

		// Set LED -- Red light on
		fprintf(stdout, "\nCalling LumiSetLED... Red light on");
		rc = (LumiStatus)LumiSetLED(connHandle, LUMI_VENUS_RED_ON);
		PrintReturnCode(rc);
		dwStartTime = GetTickCount();
		while(1)
		{
			if(GetTickCount() - dwStartTime > 500) break;
		}

		// Set LED -- Red light off
		fprintf(stdout, "\nCalling LumiSetLED... Red light off");
		rc = LumiSetLED(connHandle, LUMI_VENUS_RED_OFF);
		PrintReturnCode(rc);

		// Set LED -- Green light on
		fprintf(stdout, "\nCalling LumiSetLED... Green light on");
		rc = (LumiStatus)LumiSetLED(connHandle, LUMI_VENUS_GREEN_ON);
		PrintReturnCode(rc);
		dwStartTime = GetTickCount();
		while(1)
		{
			if(GetTickCount() - dwStartTime > 500) break;
		}

		// Set LED -- Green light off
		fprintf(stdout, "\nCalling LumiSetLED... Green light off");
		rc = LumiSetLED(connHandle, LUMI_VENUS_GREEN_OFF);
		PrintReturnCode(rc);

		//Turn heartbeat back on
		fprintf(stdout, "\nCalling LumiSetOption to turn on heart beat...");
		nHeartbeat = 0;
		rc = LumiSetOption(connHandle, LUMI_OPTION_SET_OVERRIDE_HEARTBEAT_DISPLAY, &nHeartbeat, sizeof(int));
		PrintReturnCode(rc);
	}

	// Set Option – Acq Status Callback
	fprintf(stdout, "\nCalling LumiSetOption... Acq status callback");
	rc = LumiSetOption(connHandle, LUMI_OPTION_SET_ACQ_STATUS_CALLBACK, AcqStatusCallback, sizeof(LumiAcqStatusCallback));
	PrintReturnCode(rc);

	uint nW, nH, nDPI, nBPP, nTemplateLength1, nTemplateLength2, nScore;
	int nSpoof;

	// Turn Video mode on
	fprintf(stdout, "\nCalling LumiSetLiveMode (ON)...");
	rc = LumiSetLiveMode(connHandle, LUMI_VID_ON, VideoCallbackFunc);
	PrintReturnCode(rc);
	// Idle for a second while it captures frames
	dwStartTime = GetTickCount();
	while(1)
	{
		if(GetTickCount() - dwStartTime > 1000) break;
	}
	// Turn Video mode off
	fprintf(stdout, "\nCalling LumiSetLiveMode (OFF)...");
	rc = LumiSetLiveMode(connHandle, LUMI_VID_OFF, VideoCallbackFunc);
	PrintReturnCode(rc);

	fprintf(stdout, "\nCalling LumiGetImageParams...");
	rc = LumiGetImageParams(connHandle, nW, nH, nBPP, nDPI);
	PrintReturnCode(rc);
	if(rc != LUMI_STATUS_OK) return false;

	uchar* pQualityMap = new uchar[nW * nH];
	fprintf(stdout, "\nCalling LumiGetQualityMap...");
	if(g_sensorType == VENUS)
	{
		// Try to get Quality Map before capture or verify - should return LUMI_STATUS_QUALITY_MAP_NOT_GENERATED
		rc = LumiGetQualityMap(connHandle, pQualityMap);
		PrintReturnCode(rc);
		if(rc != LUMI_STATUS_QUALITY_MAP_NOT_GENERATED) return false;
	}
	else if(g_sensorType == M300 || g_sensorType == M31X || g_sensorType == V31X)
	{
		// Try to get Quality Map with Mercury - should return LUMI_STATUS_ERROR_CMD_NOT_SUPPORTED
		rc = LumiGetQualityMap(connHandle, pQualityMap);
		PrintReturnCode(rc);
	}

	if(g_sensorType == M31X || g_sensorType == V31X || g_sensorType == M300)
	{

// Uncomment the following to demonstrate detecting fingers with a callback function.
// You can place a finger on the sensor to get a finger LUMI_ACQ_FINGER_PRESENT status 
// code returned, otherwise you will get a LUMI_ACQ_NO_FINGER_PRESENT status code returned.
// NOTE: THIS IS THE PREFERED WAY OF DETECTING A FINGER AFTER A CAPTURE.
#if 0
		LUMI_ACQ_STATUS		nStatus;
		// Detect finger with callback function		
		fprintf(stdout, "\nCalling LumiDetectFinger with the AcqStatusCallback function in 4 seconds.");
		fprintf(stdout, "\nLumiDetectFinger is blocking, but the callback will print out status codes.");
		Sleep(4000);
		// Mark the time, so we can cancel the capture after 5 seconds
		g_dwStartTime = GetTickCount();	
		g_bDetectFinger = true;
		rc = LumiDetectFinger(connHandle, nStatus, AcqStatusCallback);
		PrintReturnCode(rc);
		if(rc == LUMI_STATUS_ERROR_CMD_NOT_SUPPORTED)
		{			
			fprintf(stdout, "\nDetect Finger is not supported by this version of Mercury firmware.");
		}
		g_bDetectFinger = false;
#endif
	}

	uchar* pImage	  = new uchar[nW * nH];
	uchar* pTemplate1 = new uchar[5000];
	uchar* pTemplate2 = new uchar[5000];

	// CAPTURE
	// Mark the time, so we can cancel the capture after 6 seconds
	g_dwStartTime = GetTickCount();	
	fprintf(stdout, "\nCalling LumiCapture...");
	
	// If the sensor is configured to do on board processing, then pass in NULL for the callback function
	if(sysCaps.eProcessLocation == LUMI_PROCESS_SENSOR)
	{
		rc = (LumiStatus)LumiCapture(connHandle, pImage, nSpoof, NULL);
	}
	else
	{
		// Passing in the VideoCallbackFunc to get one raw image from the LumiCapture call
		rc = (LumiStatus)LumiCapture(connHandle, pImage, nSpoof, VideoCallbackFunc);
	}
	PrintReturnCode(rc);
	if(rc != LUMI_STATUS_OK) return false;
	fprintf(stdout, "...Spoof %d\n",nSpoof);
	
	DetectFinger(dev, connHandle);  // This function is for M30X, M31X and V31X sensors only (V30X will not detect finger)

	// CHECK SENSOR HEALTH
	fprintf(stdout, "\nCalling LumiSetOption... Checking for sensor health");
	rc = LumiSetOption(connHandle, LUMI_OPTION_CHECK_SENSOR_HEALTH, NULL, 0);
	PrintReturnCode(rc);

	// SAVE LAST CAPTURE WITH BAD FOLDER
	fprintf(stdout,"\n Saving Last Capture with bad folder...");
	rc = (LumiStatus)LumiSaveLastCapture(connHandle, "User1",5,1);
	PrintReturnCode(rc); // Expecting LUMI_STATUS_DC_OPTIONS_NOT_SET to be returned

	// SET DC OPTIONS WITH GOOD FOLDER
	fprintf(stdout,"\nSetting Data Collection Options...");
	rc = (LumiStatus)LumiSetDCOptions(connHandle, "C:\\", true);
	PrintReturnCode(rc);
	if(rc != LUMI_STATUS_OK) return false;

	// CAPTURE EX
	// Mark the time, so we can cancel the capture after 6 seconds
	g_dwStartTime = GetTickCount();	
	fprintf(stdout, "\nCalling LumiCaptureEx...");
	rc = (LumiStatus)LumiCaptureEx(connHandle, pImage, pTemplate1, nTemplateLength1, nSpoof, NULL);
	PrintReturnCode(rc);
	if( (rc != LUMI_STATUS_OK) && (rc!=LUMI_STATUS_CANCELLED_BY_USER))return false;
	fprintf(stdout, "...Spoof %d\n",nSpoof);

	DetectFinger(dev, connHandle);  // This function is for M30X, M31X and V31X sensors only (V30X will not detect finger)

	if(g_sensorType == VENUS)
	{
		fprintf(stdout, "\nCalling LumiGetQualityMap...");
		// Get the QualityMap
		rc = LumiGetQualityMap(connHandle, pQualityMap);
		if(rc != LUMI_STATUS_OK) return false;
		// Save the quality map image
		SaveImage("C:\\QualityMap2.bmp", pQualityMap, nW, nH);
	}
	if(pQualityMap) { delete [] pQualityMap; pQualityMap = NULL; }
	
	// Save the composite image
	SaveImage("C:\\CompositeImg2.bmp", pImage, nW, nH);

	// SAVE LAST CAPTURE
	fprintf(stdout,"\nSaving Last Capture...");
	rc = (LumiStatus)LumiSaveLastCapture(connHandle, "User1",5,1);
	PrintReturnCode(rc);
	
	// CAPTURE EX
	// Mark the time, so we can cancel the capture after 6 seconds
	g_dwStartTime = GetTickCount();	
	fprintf(stdout, "\nCalling LumiCaptureEx...");
	rc = (LumiStatus)LumiCaptureEx(connHandle, pImage, pTemplate2, nTemplateLength2, nSpoof, NULL);
	PrintReturnCode(rc);
	if(rc != LUMI_STATUS_OK) return false;
	fprintf(stdout, "...Spoof %d\n",nSpoof);

	DetectFinger(dev, connHandle);  // This function is for M30X, M31X and V31X sensors only (V30X will not detect finger)

	// VERIFY
	// Mark the time, so we can cancel the capture after 6 seconds
	g_dwStartTime = GetTickCount();	
	fprintf(stdout, "\nCalling LumiVerify...");
	rc = (LumiStatus)LumiVerify(connHandle, pTemplate2, nTemplateLength2, nSpoof, nScore);
	PrintReturnCode(rc);
	if(rc != LUMI_STATUS_OK) return false;
	fprintf(stdout, "Verification Complete.  Score %d. Spoof %d.\n", nScore, nSpoof);

	// MATCH
	fprintf(stdout, "\nCalling LumiMatch...");
	rc = (LumiStatus)LumiMatch(connHandle, pTemplate1, nTemplateLength1, pTemplate2, nTemplateLength2, nScore, nSpoof);
	PrintReturnCode(rc);
	if(rc != LUMI_STATUS_OK) return false;
	fprintf(stdout, "Match Complete.  Score %d. Spoof %d (-1 expected for Match spoof score)\n.", nScore, nSpoof);

	// CAPTURE EX Attempt to Capture image, handing in NULL info for templates
	// Mark the time, so we can cancel the capture after 6 seconds
	g_dwStartTime = GetTickCount();	
	fprintf(stdout, "\nCalling LumiCaptureEx with NULL template, will return LUMI_STATUS_INVALID_PARAMETER...");
	rc = (LumiStatus)LumiCaptureEx(connHandle, pImage, NULL, nTemplateLength1, nSpoof, NULL);
	PrintReturnCode(rc);
	if( (rc != LUMI_STATUS_OK) && (rc!=LUMI_STATUS_CANCELLED_BY_USER) && (rc!=LUMI_STATUS_INVALID_PARAMETER)) return false;
	fprintf(stdout, "...Spoof %d\n",nSpoof);

	DetectFinger(dev, connHandle);  // This function is for M30X, M31X and V31X sensors only (V30X will not detect finger)

	// EXTRACT
	fprintf(stdout, "\nCalling LumiExtract...");
	rc = (LumiStatus)LumiExtract(connHandle, pImage, nW, nH, nDPI, pTemplate1, nTemplateLength1);
	PrintReturnCode(rc);
	if(rc != LUMI_STATUS_OK && rc!= LUMI_STATUS_ERROR_CONFIG_UNSUPPORTED) return false;

	if(rc == LUMI_STATUS_OK)
	{
		// VERIFY
		// Mark the time, so we can cancel the capture after 6 seconds
		g_dwStartTime = GetTickCount();	
		fprintf(stdout, "\nCalling LumiVerify after LumiExtract...");
		rc = (LumiStatus)LumiVerify(connHandle, pTemplate1, nTemplateLength1, nSpoof, nScore);
		PrintReturnCode(rc);
		if( (rc != LUMI_STATUS_OK) && ( rc != LUMI_STATUS_CANCELLED_BY_USER)) return false;
		fprintf(stdout, "Verification Complete.  Score %d. Spoof %d.\n", nScore, nSpoof);

		DetectFinger(dev, connHandle);  // This function is for M30X, M31X and V31X sensors only (V30X will not detect finger)

		// MATCH
		fprintf(stdout, "\nCalling LumiMatch...");
		rc = (LumiStatus)LumiMatch(connHandle, pTemplate1, nTemplateLength1, pTemplate1, nTemplateLength1, nScore, nSpoof);
		PrintReturnCode(rc);
		if(rc != LUMI_STATUS_OK) return false;
		fprintf(stdout, "Match Complete.  Score %d. Spoof %d (-1 expected for Match spoof score)\n.", nScore, nSpoof);
	}

	// GET CONFIG
	fprintf(stdout, "\nCalling LumiGetConfig...");
	rc = (LumiStatus)LumiGetConfig(connHandle, devConfig);
	PrintReturnCode(rc);
	if(rc != LUMI_STATUS_OK) return false;

	// SET TRIGGER TIME OUT to 3 seconds
	devConfig.nTriggerTimeout = 3;
	fprintf(stdout, "\nCalling LumiSetConfig...");
	rc = (LumiStatus)LumiSetConfig(connHandle, devConfig);
	PrintReturnCode(rc);
	if(rc != LUMI_STATUS_OK) return false;

	// CAPTURE - to demonstrate 3 second time out (User needs to place finger on sensor within 3 seconds
	//                                             or LumiCapture() will return with a time out.)
	fprintf(stdout, "\nCalling LumiCapture...");
	rc = (LumiStatus)LumiCapture(connHandle, pImage, nSpoof, NULL);
	PrintReturnCode(rc);
	fprintf(stdout, "...Spoof %d\n",nSpoof);

	DetectFinger(dev, connHandle); // This function is for M30X, M31X and V31X sensors only (V30X will not detect finger)

	// SET TRIGGER TIME OUT back to 15 seconds
	devConfig.nTriggerTimeout = 15;
	fprintf(stdout, "\nCalling LumiSetConfig...");
	rc = (LumiStatus)LumiSetConfig(connHandle, devConfig);
	PrintReturnCode(rc);
	if(rc != LUMI_STATUS_OK) return false;

	// SNAPSHOT
	fprintf(stdout, "\nCalling LumiSnapShot...");
	rc = (LumiStatus)LumiSnapShot(connHandle, pImage, 128, 10);
	PrintReturnCode(rc);

	// CLOSE
	fprintf(stdout, "\nCalling LumiClose...");
	rc = LumiClose(connHandle);
	PrintReturnCode(rc);
	
	// EXIT
	fprintf(stdout, "\nCalling LumiExit...");
	rc = LumiExit();
	PrintReturnCode(rc);
	if(rc != LUMI_STATUS_OK) return false;

	delete [] pImage;
	delete [] pTemplate1;
	delete [] pTemplate2;
	

	fprintf(stdout, "API Test completed.\n");

	return true;
}

void DetectFinger(LUMI_DEVICE dev, LUMI_HANDLE connHandle)
{
	// Check to see if finger is still present.  If so, ask user to lift finger from sensor.  
	// Stay in loop until they lift finger.    Only for M30X, M31X, and V31X Sensors.  V30X sensors with
	// firmware revisions greater than 20292 also support DetectFinger.
	// M31X and V31X sensor acquisitions are fast.  For enrollments, fingers should be lifted from  
	// the sensor between insertions.  After captures and verifications, fingers should also be 
	// lifted from the sensor between captures.

	fprintf(stdout, "\nCalling LumiDetectFinger()");
	bool bStayInLoop = true;
	LUMI_ACQ_STATUS nStatus = LUMI_ACQ_NOOP;
	while(bStayInLoop)
	{
		// NOTE: Polling for detecting the finger is expensive - this is what we are doing here.  
		//       It's better to use the LumiDetectFinger as a blocking call by passing a callback.  
		//       This is demonstrated in the TestAPI() function in a commented out section.
		LumiStatus rc = LumiDetectFinger(connHandle, nStatus, NULL);
		if(rc != LUMI_STATUS_OK)
		{
			if(rc == LUMI_STATUS_ERROR_CMD_NOT_SUPPORTED)
			{
				PrintReturnCode(rc);
				fprintf(stdout, "\nDetect Finger is not supported by this sensor.");
				break;
			}
			PrintReturnCode(rc);
			fprintf(stdout, "\nLumiStatis is not OK!");
			break;
		}
		if(nStatus == LUMI_ACQ_FINGER_PRESENT)
		{
			fprintf(stdout, "\nFinger present!  Please lift finger from sensor.");
		}
		else if(nStatus == LUMI_ACQ_NO_FINGER_PRESENT)
		{
			bStayInLoop = false;
		}
	}
}

// Return codes
const char* pReturnCode[50] = {
	"LUMI_STATUS_OK",
	"LUMI_STATUS_ERROR_DEVICE_OPEN",
	"LUMI_STATUS_ERROR_DEVICE_CLOSE",
	"LUMI_STATUS_ERROR_CMD_NOT_SUPPORTED",
	"LUMI_STATUS_ERROR_COMM_LINK",
	"LUMI_STATUS_ERROR_PREPROCESSOR",
	"LUMI_STATUS_ERROR_CALIBRATION",
	"LUMI_STATUS_ERROR_BUSY",
	"LUMI_STATUS_ERROR_INVALID_PARAMETER",
	"LUMI_STATUS_ERROR_TIMEOUT",
	"LUMI_STATUS_ERROR_INVALID_TEMPLATE",
	"LUMI_STATUS_ERROR_MEMORY_ALLOCATION",
	"LUMI_STATUS_ERROR_INVALID_DEVICE_ID",
	"LUMI_STATUS_ERROR_INVALID_CONNECTION_ID",
	"LUMI_STATUS_ERROR_CONFIG_UNSUPPORTED",
	"LUMI_STATUS_UNSUPPORTED",
	"LUMI_STATUS_INTERNAL_ERROR",
	"LUMI_STATUS_INVALID_PARAMETER",
	"LUMI_STATUS_DEVICE_TIMEOUT",
	"LUMI_STATUS_INVALID_OPTION",
	"LUMI_STATUS_ERROR_MISSING_SPOOFTPL",
	"LUMI_STATUS_CANCELLED_BY_USER",
	"LUMI_STATUS_INVALID_FOLDER",
	"LUMI_STATUS_DC_OPTIONS_NOT_SET",
	"LUMI_STATUS_INCOMPATIBLE_FIRMWARE",
	"LUMI_STATUS_ERROR_TIMEOUT_LATENT",
	"LUMI_STATUS_QUALITY_MAP_NOT_GENERATED",
	"LUMI_STATUS_THREAD_ERROR",
	"LUMI_STATUS_ERROR_SENSOR_COMM_TIMEOUT",
	"LUMI_STATUS_DEVICE_STATUS_ERROR",
	"LUMI_STATUS_ERROR_BAD_INSTALLATION"};

void PrintReturnCode(LumiStatus rc)
{
	char* strRC = GetReturnCode(rc);
	fprintf(stdout, "\nrc = %s \n", strRC);
}

char* GetReturnCode(LumiStatus rc)
{
	int nReturnCodeIndex = 0;
	switch(rc)
	{
		case LUMI_STATUS_OK: { nReturnCodeIndex = 0; } break;
		case LUMI_STATUS_ERROR_DEVICE_OPEN: { nReturnCodeIndex = 1; } break;
		case LUMI_STATUS_ERROR_DEVICE_CLOSE: { nReturnCodeIndex = 2; } break;
		case LUMI_STATUS_ERROR_CMD_NOT_SUPPORTED: { nReturnCodeIndex = 3; } break;
		case LUMI_STATUS_ERROR_COMM_LINK: { nReturnCodeIndex = 4; } break;
		case LUMI_STATUS_ERROR_PREPROCESSOR: { nReturnCodeIndex = 5; } break;
		case LUMI_STATUS_ERROR_CALIBRATION: { nReturnCodeIndex = 6; } break;
		case LUMI_STATUS_ERROR_BUSY: { nReturnCodeIndex = 7; } break;
		case LUMI_STATUS_ERROR_INVALID_PARAMETER: { nReturnCodeIndex = 8; } break;
		case LUMI_STATUS_ERROR_TIMEOUT: { nReturnCodeIndex = 9; } break;
		case LUMI_STATUS_ERROR_INVALID_TEMPLATE: { nReturnCodeIndex = 10; } break;
		case LUMI_STATUS_ERROR_MEMORY_ALLOCATION: { nReturnCodeIndex = 11; } break;
		case LUMI_STATUS_ERROR_INVALID_DEVICE_ID: { nReturnCodeIndex = 12; } break;
		case LUMI_STATUS_ERROR_INVALID_CONNECTION_ID: { nReturnCodeIndex = 13; } break;
		case LUMI_STATUS_ERROR_CONFIG_UNSUPPORTED: { nReturnCodeIndex = 14; } break;
		case LUMI_STATUS_UNSUPPORTED: { nReturnCodeIndex = 15; } break;
		case LUMI_STATUS_INTERNAL_ERROR: { nReturnCodeIndex = 16; } break;
		case LUMI_STATUS_INVALID_PARAMETER: { nReturnCodeIndex = 17; } break;
		case LUMI_STATUS_DEVICE_TIMEOUT: { nReturnCodeIndex = 18; } break;
		case LUMI_STATUS_INVALID_OPTION: { nReturnCodeIndex = 19; } break;
		case LUMI_STATUS_ERROR_MISSING_SPOOFTPL: { nReturnCodeIndex = 20; } break;
		case LUMI_STATUS_CANCELLED_BY_USER: { nReturnCodeIndex = 21; } break;
		case LUMI_STATUS_INVALID_FOLDER: { nReturnCodeIndex = 22; } break;
		case LUMI_STATUS_DC_OPTIONS_NOT_SET: { nReturnCodeIndex = 23; } break;
		case LUMI_STATUS_INCOMPATIBLE_FIRMWARE: { nReturnCodeIndex = 24; } break;
		case LUMI_STATUS_ERROR_TIMEOUT_LATENT: { nReturnCodeIndex = 25; } break;
		case LUMI_STATUS_QUALITY_MAP_NOT_GENERATED: { nReturnCodeIndex = 26; } break;
		case LUMI_STATUS_THREAD_ERROR: { nReturnCodeIndex = 27; } break;
		case LUMI_STATUS_ERROR_SENSOR_COMM_TIMEOUT: { nReturnCodeIndex = 28; } break;
		case LUMI_STATUS_DEVICE_STATUS_ERROR: { nReturnCodeIndex = 29; } break;
		case LUMI_STATUS_ERROR_BAD_INSTALLATION: { nReturnCodeIndex = 30; } break;
	}
	return (char*) pReturnCode[nReturnCodeIndex];
}

void PrintDeviceCaps(LUMI_DEVICE_CAPS sysCaps)
{
	fprintf(stdout, "\n*** Device Caps ***");
	fprintf(stdout, "\n-------------------");
	fprintf(stdout, "\nbImageCapture:\t\t%d",sysCaps.bCaptureImage);
	fprintf(stdout, "\nbExtract:\t\t%d",sysCaps.bExtract);
	fprintf(stdout, "\nbMatch:\t\t\t%d",sysCaps.bMatch);
	fprintf(stdout, "\nbIdentify:\t\t%d",sysCaps.bIdentify);
	fprintf(stdout, "\nbSpoof:\t\t\t%d", sysCaps.bSpoof);
	fprintf(stdout, "\neTemplate:\t\t%d",sysCaps.eTemplate);
	fprintf(stdout, "\neTransInfo:\t\t%d",sysCaps.eTransInfo);
	fprintf(stdout, "\nWidth:\t\t\t%d",sysCaps.m_nWidth);
	fprintf(stdout, "\nHeight:\t\t\t%d",sysCaps.m_nHeight);
	fprintf(stdout, "\nDPI:\t\t\t%d",sysCaps.m_nDPI);
	fprintf(stdout, "\nImage Format:\t\t%d\n",sysCaps.m_nImageFormat);
	fprintf(stdout, "\neProcessLocation:\t%d", sysCaps.eProcessLocation);
}

void GetSerialNumberAndProdID(LUMI_DEVICE dev,int &nSerial,int &nProdID)
{
	switch(dev.SensorType)
	{
	case VENUS:
		sscanf(dev.strIdentifier,"%5d%d",&nProdID,&nSerial);
		break;
	case M300:
		sscanf(dev.strIdentifier,"%4d%d",&nProdID,&nSerial);
		break;
	case M31X:
		sscanf(dev.strIdentifier,"%2d%d",&nProdID,&nSerial);
		break;
	case M32X:
		sscanf(dev.strIdentifier,"%2d%d",&nProdID,&nSerial);
		break;
	case V31X:
		sscanf(dev.strIdentifier,"%2d%d",&nProdID,&nSerial);
		break;
	default:
		printf("This device is not supported\n");
	}
}
const char* LumiSensorType[9] = {
	"V30X",
	"M10X",
	"M30X",
	"M31X",
	"V31X",
	"",
	"",
	"",
	"M32X"
	};

const char* MarshalSensorType(LUMI_SENSOR_TYPE type)
{
	return LumiSensorType[type];
}


int g_nCallBackCount = 0;

int AcqStatusCallback(unsigned int nStatus)
{
	// Illustrates that returning -2 from this callback cancels the capture.
#if 0
	static bool bCancelByRequest = true;
	if( (GetTickCount() - g_dwStartTime > 6000) && (bCancelByRequest==true))
	{
		bCancelByRequest = false;
		return -2;
	}
#endif

	// Cancel the LumiDetectFinger call by returning true.
	if(g_bDetectFinger == true) // check to see if this callback is being used for LumiDetectFinger
	{
		if( GetTickCount() - g_dwStartTime > 5000)
		{			
			return -2;
		}
	}
	

	switch(nStatus)
	{
	case LUMI_ACQ_DONE:
		{
			fprintf(stdout, "\nAcquisition Status = LUMI_ACQ_DONE");
			g_nCallBackCount = 0;
		} break;
	case LUMI_ACQ_PROCESSING_DONE:
		{
			if(g_sensorType == VENUS)
			{
				fprintf(stdout, "\nAcquisition Status = LUMI_ACQ_PROCESSING_DONE");
				g_nCallBackCount = 0;
			}
			else
			{
				g_nCallBackCount += 1;
				if(g_nCallBackCount == 10)
				{
					fprintf(stdout, "\nAcquisition Status = LUMI_ACQ_PROCESSING_DONE");
					g_nCallBackCount = 0;
				}
			}
		} break;
	case LUMI_ACQ_BUSY:
		{
			fprintf(stdout, "\nAcquisition Status = LUMI_ACQ_BUSY");
		} break;
	case LUMI_ACQ_TIMEOUT:
		{
			fprintf(stdout, "\nAcquisition Status = LUMI_ACQ_TIMEOUT");
			g_nCallBackCount = 0;
		} break;
	case LUMI_ACQ_NO_FINGER_PRESENT:
		{
			fprintf(stdout, "\nAcquisition Status = LUMI_ACQ_NO_FINGER_PRESENT");
		} break;
	case LUMI_ACQ_MOVE_FINGER_UP:
		{
			fprintf(stdout, "\nAcquisition Status = LUMI_ACQ_MOVE_FINGER_UP");
		} break;
	case LUMI_ACQ_MOVE_FINGER_DOWN:
		{
			fprintf(stdout, "\nAcquisition Status = LUMI_ACQ_MOVE_FINGER_DOWN");
		} break;
	case LUMI_ACQ_MOVE_FINGER_LEFT:
		{
			fprintf(stdout, "\nAcquisition Status = LUMI_ACQ_MOVE_FINGER_LEFT");
		} break;
	case LUMI_ACQ_MOVE_FINGER_RIGHT:
		{
			fprintf(stdout, "\nAcquisition Status = LUMI_ACQ_MOVE_FINGER_RIGHT");
		} break;
	case LUMI_ACQ_FINGER_POSITION_OK:
		{
			fprintf(stdout, "\nAcquisition Status = LUMI_ACQ_FINGER_POSITION_OK");
		} break;
	case LUMI_ACQ_CANCELLED_BY_USER:
		{
			fprintf(stdout, "\nAcquisition Status = LUMI_ACQ_CANCELLED_BY_USER");
		} break;
	case LUMI_ACQ_TIMEOUT_LATENT:
		{
			fprintf(stdout, "\nAcquisition Status = LUMI_ACQ_TIMEOUT_LATENT");
			g_nCallBackCount = 0;
		} break;
	case LUMI_ACQ_NOOP:
		{
			fprintf(stdout, "\nAcquisition Status = LUMI_ACQ_NOOP");
		}break;
	case LUMI_ACQ_FINGER_PRESENT:
		{
			fprintf(stdout, "\nAcquisition Status = LUMI_ACQ_FINGER_PRESENT");
		}break;
	case LUMI_ACQ_SPOOF_DETECTED:
		{
			fprintf(stdout, "\nAcquisition Status = LUMI_ACQ_SPOOF_DETECTED");
		}break;
	default:
		{
			fprintf(stdout, "\nAcquisition Status is undefined!!!!");
		}
	}
	return 0;
}
void VideoCallbackFunc(unsigned char* pImage, int nWidth, int nHeight, int nNum)
{
	fprintf(stdout, "\nReceived Live Frame (%d) - Width(%d), Height(%d)", nNum, nWidth, nHeight);
}

// Generic function to save an 8 bpp grayscale bitmap without using GDI+
void SaveImage(const char* filename, unsigned char* pImage, uint width, uint height)
{	
	uchar* pBitmapImage = NULL;
	BITMAPFILEHEADER bitmapFileHeader;
	BITMAPINFOHEADER bitmapInfoHeader;

    memset(&bitmapInfoHeader, 0, sizeof(BITMAPINFOHEADER));
    bitmapInfoHeader.biSize = sizeof(BITMAPINFOHEADER);
    bitmapInfoHeader.biHeight = -1 * height; // negative to indicate that the data is stored in forward order
    bitmapInfoHeader.biWidth = width;
    bitmapInfoHeader.biPlanes = 1;
    bitmapInfoHeader.biBitCount = 8;  // 8 bpp
    bitmapInfoHeader.biCompression= BI_RGB;
    bitmapInfoHeader.biSizeImage = 0;  // Can be set to 0
    bitmapInfoHeader.biXPelsPerMeter = 19685; //19685.05, Can be set to 0
    bitmapInfoHeader.biYPelsPerMeter = 19685; //19685.05, Can be set to 0
	bitmapInfoHeader.biClrUsed = 0;  // Can be set to 0
    bitmapInfoHeader.biClrImportant = 0;  // Can be set to 0    
  
	memset(&bitmapFileHeader, 0, sizeof(BITMAPFILEHEADER));
    bitmapFileHeader.bfOffBits = sizeof(BITMAPFILEHEADER) + sizeof(BITMAPINFOHEADER);
    bitmapFileHeader.bfSize = (bitmapInfoHeader.biHeight * bitmapInfoHeader.biWidth) + sizeof(BITMAPFILEHEADER) + sizeof(BITMAPINFOHEADER);
    bitmapFileHeader.bfType = 0x4d42; // for bitmap
    bitmapFileHeader.bfReserved1 = 0;
    bitmapFileHeader.bfReserved2 = 0;
 
	int nBitmapImageBufferSize = sizeof(BITMAPFILEHEADER) + sizeof(BITMAPINFOHEADER) + (sizeof(tagRGBQUAD) * 256) + (width * height);

	pBitmapImage = new uchar[nBitmapImageBufferSize];

	pBitmapImage = Pack(pBitmapImage, &bitmapFileHeader, sizeof(BITMAPFILEHEADER));

	pBitmapImage = Pack(pBitmapImage, &bitmapInfoHeader, sizeof(BITMAPINFOHEADER));

	// Create color table (required for 8 bpp bitmap)
	tagRGBQUAD rgbQuad;	
	for(int i = 0; i < 256; i++)
	{
		rgbQuad.rgbRed = i;
		rgbQuad.rgbGreen = i;
		rgbQuad.rgbBlue = i;
		rgbQuad.rgbReserved = 0;
		pBitmapImage = Pack(pBitmapImage, &rgbQuad, sizeof(tagRGBQUAD));
	}

	// Copy Raw Data into 8 bit format.
	int nPixelsToCopy = width * height;
	uchar* pDst = new uchar[width * height];
	while(nPixelsToCopy-- > 0)
	{
		*pDst++ = *pImage++;
	}
	//Get pDst back to begining
	pDst -= (width * height);

	pBitmapImage = Pack(pBitmapImage, pDst, (width * height));

	//Get pBitmapImage back to begining
	pBitmapImage -= nBitmapImageBufferSize;
	
	std::ofstream file (filename, std::ios::out|std::ios::binary);
	file.write((const char*)pBitmapImage, nBitmapImageBufferSize);
	file.close();

	if(pBitmapImage) { delete[] pBitmapImage; pBitmapImage = NULL; }
	if(pDst) { delete[] pDst; pDst = NULL; }

}

uchar* Pack(uchar* pDst, void* pSrc, uint nSize)
{
 	memcpy(pDst,(uchar*)pSrc,nSize);
 	return pDst+nSize;    
}

uchar* Unpack(void* pDst, uchar* pSrc, uint nSize)
{
	memcpy(pDst,(uchar*)pSrc,nSize);
 	return pSrc+nSize;    
}
