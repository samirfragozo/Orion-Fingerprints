
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

//********************************************************************************/
// LumiInOpAPIExample.cpp : Defines the entry point for the console application. The 
//                      application provides sample code for LumiInOpAPI functions 
// TestAPI:				Demonstrates how to Compute NFIQ of Image, 
//                      encode and decode images , load and save as bitmap images, 
//                      truncate templates.
// TestFIR:             Demonstrates Finger Image Record format functions

#include <conio.h>
#include <fstream>

#include "LumiInOpAPI.h"
#include "stdafx.h"
#include "LumiAPI.h"

// Path to save/load files
const char* DataPath = "C:\\temp";
char logFile[100] ;
FILE * logFileID;

// Funtion that test API
bool TestAPI();
// Function to demonstrate LumiFIRecord calls
bool TestFIR();
// Display error codes
void PrintReturnCode(LumiStatus rc);
// Get error message str
char* GetReturnCode(LumiStatus rc);

int _tmain(int argc, _TCHAR* argv[])
{
	int nRC = 0;
	sprintf(logFile,"%s\\LumiInOpAPIExample.txt",DataPath);
	logFileID = fopen(logFile, "a+");
	bool bStayInWhile = true;
	while(bStayInWhile)
	{
		fprintf(stdout, "\r\n\r\nChoose Which Function To Run:");
		fprintf(stdout, "\r\n---------------------------------------");
		fprintf(stdout, "\r\n1:   Test API");
		fprintf(stdout, "\r\n2:   Test FIR");
		fprintf(stdout, "\r\nQ:   Quit");
		fprintf(stdout, "\r\n");
		
		int ch = _getch();			
		switch(toupper(ch))
		{
			case '1':
				{
					bStayInWhile = false;
					if(!TestAPI())		// Funtion that test API
					{
						nRC = -1;
					}					

				} break;
			case '2':
				{
					bStayInWhile = false;
					if(!TestFIR())		//Function to demonstrate LumiFIRecord calls			
					{
						nRC = -2;
					}
					
				} break;
			case 'Q':
				{	
					bStayInWhile = false;
					nRC =  0;					
				} break;				
			default:				
				{	
					fprintf(stdout, "Invalid Option! Please try again.\n");
					continue;					
				} break;		
		}
		if(nRC!=0)
			fprintf(logFileID,"****** TEST FAILED ******\n");

	}
	fprintf(stdout,"\nPress Enter to continue....");
	fprintf(logFileID,"\n****** TEST Complete ******\n");
	fclose(logFileID);
	getchar();

	return nRC;

}

bool TestAPI()
{
	uint				nNumDevices = 0;
	LUMI_DEVICE			dev;
	LUMI_HANDLE			connHandle = 0;
	LumiStatus			rc = LUMI_STATUS_OK;
	
	fprintf(logFileID,"****** TESTAPI Started ******\n");
	// QUERY NUMBER DEVICES
	fprintf(stdout, "\nQuerying Number of Devices.  This may take several seconds...");
	fprintf(logFileID, "\nQuerying Number of Devices.  This may take several seconds...");
	rc = LumiQueryNumberDevices(nNumDevices, NULL);
	PrintReturnCode(rc);
	if(rc != LUMI_STATUS_OK) return false;

	//Check if only one Device is connected
	if(nNumDevices < 1)
	{
		fprintf(stdout, "\nNo Device found. Please connect a Device.");
		fprintf(logFileID, "\nNo Device found. Please connect a Device.");
		return false;
	}

	if(nNumDevices > 1)
	{
		fprintf(stdout, "\nFound %d Devices. Please connect only one Device",nNumDevices);
		fprintf(logFileID, "\nFound %d Devices. Please connect only one Device",nNumDevices);

		return false;
	}

	// QUERY DEVICE
	fprintf(stdout, "\nQuerying Device...");
	fprintf(logFileID, "\nQuerying Device...");
	rc = LumiQueryDevice(0, dev);
	PrintReturnCode(rc);
	if(rc != LUMI_STATUS_OK) return false;

	// INIT
	fprintf(stdout, "\nInitializing Device...");
	fprintf(logFileID, "\nInitializing Device...");
	rc = LumiInit(dev.hDevHandle, connHandle);
	PrintReturnCode(rc);
	if(rc != LUMI_STATUS_OK) return false;
	fprintf(stdout, "\nInit Returned Connection Handle %d.", connHandle);
	fprintf(logFileID, "\nInit Returned Connection Handle %d.", connHandle);

	uint nW, nH, nDPI, nBPP;
	int nSpoof;

	// Get Image Params
	fprintf(stdout, "\nCalling LumiGetImageParams...");
	fprintf(logFileID, "\nCalling LumiGetImageParams...");
	rc = LumiGetImageParams(connHandle, nW, nH, nBPP, nDPI);
	PrintReturnCode(rc);
	if(rc != LUMI_STATUS_OK) return false;

	uchar* pImage = new uchar[nW * nH];

	// Capture Image
	fprintf(stdout, "\nCalling LumiCapture...");
	fprintf(logFileID, "\nCalling LumiCapture...");
	rc = LumiCapture(connHandle, pImage, nSpoof, NULL);
	PrintReturnCode(rc);
	if(rc != LUMI_STATUS_OK) return false;
	fprintf(stdout, "Spoof %d\n",nSpoof);
	fprintf(logFileID, "Spoof %d\n",nSpoof);

	// Compute NIST Quality
	int nNFIQ;
	fprintf(stdout, "\nCalling LumiComputeNFIQFromImage...");
	fprintf(logFileID, "\nCalling LumiComputeNFIQFromImage...");
	rc = LumiComputeNFIQFromImage(pImage, nW, nH, nBPP, nDPI, nNFIQ);
	PrintReturnCode(rc);
	if(rc != LUMI_STATUS_OK) return false;
	fprintf(stdout, "NFIQ  %d\n",nNFIQ);

	// Encode Image
	uchar* pWSQImage = NULL;
	uint nWSQImageSize = 0;
	float nCompressionRatio = 13;
	fprintf(stdout, "\nCalling LumiWSQEncode...");
	fprintf(logFileID, "\nCalling LumiWSQEncode...");
	rc = LumiWSQEncode(pImage, nW, nH, nBPP, nDPI, nCompressionRatio, &pWSQImage, nWSQImageSize);
	PrintReturnCode(rc);
	if(rc != LUMI_STATUS_OK) return false;

	// Save WSQ Image
	fprintf(stdout, "\nSaving WSQImage...");
	fprintf(logFileID, "\nSaving WSQImage...");
	FILE* pFile = NULL;
	char* pFileName = "WSQImage.wsq";
	size_t nNameSize = strlen(DataPath) + strlen(pFileName) + 10;
    char* pFullFileName = new char[nNameSize];

	sprintf_s(pFullFileName,nNameSize,"%s\\%s",DataPath, pFileName);
	fopen_s(&pFile,pFullFileName,"w+b");
	if (pFile == NULL) 
	{
          printf("\nERROR: Cannot open %s\n",pFullFileName);
		  fprintf(logFileID, "\nERROR: Cannot open %s\n",pFullFileName);
          return false;
	}
	
	// Write data to the file
	int nWriteOK = (int)fwrite(pWSQImage, nWSQImageSize, 1, pFile); // expect 1
	fclose(pFile);
	pFile = NULL;

	// Release memory allocation
	fprintf(stdout, "\nCalling LumiRelease...");
	fprintf(logFileID, "\nCalling LumiRelease...");
	rc = LumiRelease(pWSQImage);
	pWSQImage = NULL;
	PrintReturnCode(rc);
	if(rc != LUMI_STATUS_OK) return false;

	//  Load WSQImage
	uint nWSQImageSize1 =0;
    fopen_s(&pFile,pFullFileName,"r+b");
    if (pFile == NULL) 
    {
		printf("\nERROR: Cannot open %s\n",pFullFileName);
		fprintf(logFileID,"\nERROR: Cannot open %s\n",pFullFileName);
        return false;
    }
    
    fseek(pFile,0L,SEEK_END);
    nWSQImageSize1 = ftell(pFile);
    fseek(pFile,0L,SEEK_SET); 
    uchar* pWSQImage1 = new uchar[nWSQImageSize1];
    
    // Read data from the file
    int nReadOK = (int)fread(pWSQImage1, nWSQImageSize1, 1, pFile);	// expect 1
    fclose(pFile);
    pFile = NULL;
	delete[] pFullFileName; pFullFileName = NULL;

	// Decode image
    uint nW1, nH1, nDPI1, nBPP1;
    int nlossyflag1;
    uchar* pImage1 = NULL;
    fprintf(stdout, "\nCalling LumiWSQDecode...");
	fprintf(logFileID, "\nCalling LumiWSQDecode...");
    rc = LumiWSQDecode(pWSQImage1, nWSQImageSize1, &pImage1, nW1, nH1, nBPP1, nDPI1, nlossyflag1);
    PrintReturnCode(rc);
    if(rc != LUMI_STATUS_OK) return false;

    // Save as bitmap
	pFileName = "Bitmap.bmp";
	nNameSize = strlen(DataPath) + strlen(pFileName) + 10;
	pFullFileName = new char[nNameSize];
	sprintf_s(pFullFileName,nNameSize,"%s\\%s",DataPath, pFileName);

    fprintf(stdout, "\nCalling LumiSaveAsBitmapToFile...");
	fprintf(logFileID, "\nCalling LumiSaveAsBitmapToFile...");
    rc = LumiSaveAsBitmapToFile(pImage1, nW1, nH1, nBPP1, pFullFileName);
    PrintReturnCode(rc);
    if(rc != LUMI_STATUS_OK) return false;

    // Release memory allocation
    fprintf(stdout, "\nCalling LumiRelease...");
	fprintf(logFileID, "\nCalling LumiRelease...");
    rc = LumiRelease(pImage1);
	pImage1 = NULL;
    PrintReturnCode(rc);
    if(rc != LUMI_STATUS_OK) return false;

    // Load Bitmap
    uint nW2, nH2, nBPP2;
	uchar* pImage2 = NULL;
    fprintf(stdout, "\nCalling LumiLoadBitmapFromFile...");
	fprintf(logFileID, "\nCalling LumiLoadBitmapFromFile...");
    rc = LumiLoadBitmapFromFile(pFullFileName, &pImage2, nW2, nH2, nBPP2);
    PrintReturnCode(rc);
    if(rc != LUMI_STATUS_OK) return false;
	delete[] pFullFileName; pFullFileName = NULL;

	// Extract templates from captured image and  decoded image
	// Extract template from captured image
	uchar* pTemplate = new uchar[5000];
	uint nTemplateLength;
	fprintf(stdout, "\nExtract template from captured image...");
	fprintf(logFileID, "\nExtract template from captured image...");
	fprintf(stdout, "\nCalling LumiExtractTemplate...");
	fprintf(logFileID, "\nCalling LumiExtractTemplate...");
	rc = LumiExtractTemplate(pImage, nW, nH, pTemplate, nTemplateLength);
	PrintReturnCode(rc);
	if(rc != LUMI_STATUS_OK) return false;
	
	uchar* pTemplate2 = new uchar[5000];
	uint nTemplateLength2, nScore;

	// Extract template from WSQ decoded image
	fprintf(stdout, "\nExtract template from WSQ decoded image...");
	fprintf(logFileID, "\nExtract template from WSQ decoded image...");
	fprintf(stdout, "\nCalling LumiExtractTemplate...");
	fprintf(logFileID, "\nCalling LumiExtractTemplate...");
	rc = LumiExtractTemplate(pImage2, nW2, nH2, pTemplate2, nTemplateLength2);
	PrintReturnCode(rc);
	if(rc != LUMI_STATUS_OK) return false;

	// Match 
	fprintf(stdout, "\nCalling LumiMatchTemplate...");
	fprintf(logFileID, "\nCalling LumiMatchTemplate...");
	rc = LumiMatchTemplate(pTemplate, nTemplateLength, pTemplate2, nTemplateLength2, nScore);
	PrintReturnCode(rc);
	if(rc != LUMI_STATUS_OK) return false;
	fprintf(stdout, "Match Complete.  Score %d.\n", nScore);

	// Truncate
	fprintf(stdout, "\nTruncate template extracted from captured image...");
	fprintf(logFileID, "\nCalling LumiTruncateANSI378Template...");
	uint nMaxTruncatedTemplateSize = 1000;
	uchar* pTruncatedTemplate = new uchar[nMaxTruncatedTemplateSize];
	rc = LumiTruncateANSI378Template(pTemplate, pTruncatedTemplate, nMaxTruncatedTemplateSize, REMOVE_ALL);
	PrintReturnCode(rc);
	if(rc != LUMI_STATUS_OK) return false;

	// Match with truncated template
	fprintf(stdout, "\nCalling LumiMatchTemplate...");
	fprintf(logFileID, "\nCalling LumiMatchTemplate...");
	rc = LumiMatchTemplate(pTemplate, nTemplateLength, pTruncatedTemplate, nMaxTruncatedTemplateSize, nScore);
	PrintReturnCode(rc);
	if(rc != LUMI_STATUS_OK) return false;
	fprintf(stdout, "Match Complete.  Score %d.\n", nScore);
	fprintf(logFileID, "Match Complete.  Score %d.\n", nScore);

	// ANSI378 to Propreitery and Propreitery to ANSI template calls
	uchar* pPropTemplate = NULL;//new uchar[5000];
	uint nPropTemplateSize = 0;
	fprintf(stdout, "\nCalling LumiANSI378ToProp...");
	fprintf(logFileID, "\nCalling LumiANSI378ToProp...");
	rc = LumiANSI378ToProp(pTemplate, nTemplateLength, &pPropTemplate, nPropTemplateSize);
	PrintReturnCode(rc);
	if(rc != LUMI_STATUS_OK) return false;

	//Prop to ANSI
	uchar* pPropToANSITemplate = NULL;
	uint nANSI378TemplateSize;
	fprintf(stdout, "\nCalling LumiPropToANSI378...");
	fprintf(logFileID, "\nCalling LumiPropToANSI378...");
	rc = LumiPropToANSI378(pPropTemplate, nPropTemplateSize, &pPropToANSITemplate, nANSI378TemplateSize);
	PrintReturnCode(rc);
	if(rc != LUMI_STATUS_OK) return false;
	rc = LumiRelease(pPropTemplate);
	PrintReturnCode(rc);

	//ANSI to Prop
	uchar* pPropTemplate2 = NULL;
	uint nPropTemplateSize2 = 0;
	fprintf(stdout, "\nCalling LumiANSI378ToProp...");
	fprintf(logFileID, "\nCalling LumiANSI378ToProp...");
	rc = LumiANSI378ToProp(pTemplate2, nTemplateLength2, &pPropTemplate2, nPropTemplateSize2);
	PrintReturnCode(rc);

	if(rc != LUMI_STATUS_OK) return false;

	//Prop to ANSI
	uchar* pPropToANSITemplate2 = NULL;
	uint nANSI378TemplateSize2;
	fprintf(stdout, "\nCalling LumiPropToANSI378...");
	fprintf(logFileID, "\nCalling LumiPropToANSI378...");
	rc = LumiPropToANSI378(pPropTemplate2, nPropTemplateSize2, &pPropToANSITemplate2, nANSI378TemplateSize2);
	PrintReturnCode(rc);
	rc = LumiRelease(pPropTemplate2);
	if(rc != LUMI_STATUS_OK) return false;


	fprintf(stdout, "\nCalling LumiMatchTemplate...");
	fprintf(logFileID, "\nCalling LumiMatchTemplate...");
	rc = LumiMatchTemplate(pPropToANSITemplate, nANSI378TemplateSize, pPropToANSITemplate2, nANSI378TemplateSize2, nScore);
fprintf(stdout, "\nMatch Complete.  Score %d.\n", nScore);
	rc = LumiMatchTemplate(pTemplate, nTemplateLength, pTemplate2, nTemplateLength2, nScore);
fprintf(stdout, "\nMatch Complete.  Score %d.\n", nScore);
	//rc = LumiMatchTemplate(pTemplate, nTemplateLength, pTemplate2, nTemplateLength2, nScore);
	PrintReturnCode(rc);
	if(rc != LUMI_STATUS_OK) return false;
	fprintf(stdout, "\nMatch Complete.  Score %d.\n", nScore);
	fprintf(logFileID, "Match Complete.  Score %d.\n", nScore);



	////////////////////////////////////////////
	//ANSI TO ISO
	uchar* pISOTemplate = NULL;
	uint nISOTemplateSize; 	
	rc = LumiANSI378ToISO(pPropToANSITemplate2, nANSI378TemplateSize2, &pISOTemplate, nISOTemplateSize);
	PrintReturnCode(rc);


	//ISO TO ANSI
	uchar* pANSI378Template = NULL;
	rc = LumiISOToANSI378(pISOTemplate, nISOTemplateSize, &pANSI378Template, nANSI378TemplateSize);
	PrintReturnCode(rc);
	rc = LumiRelease(pANSI378Template);
	PrintReturnCode(rc);

	rc = LumiRelease(pPropToANSITemplate);
	PrintReturnCode(rc);
    if(rc != LUMI_STATUS_OK) return false;
	rc = LumiRelease(pPropToANSITemplate2);
	PrintReturnCode(rc);
    if(rc != LUMI_STATUS_OK) return false;
	rc = LumiRelease(pISOTemplate);
	PrintReturnCode(rc);
    if(rc != LUMI_STATUS_OK) return false;

	/////////////////////////////////////////////

	// Release memory allocation
    fprintf(stdout, "\nCalling LumiRelease...");
	fprintf(logFileID, "\nCalling LumiRelease...");
    rc = LumiRelease(pImage2);
	pImage2 = NULL;
	PrintReturnCode(rc);
    if(rc != LUMI_STATUS_OK) return false;

	delete[] pImage;
	delete[] pTemplate;
	delete[] pWSQImage1;
	delete[] pTruncatedTemplate;
	delete[] pTemplate2;


	// EXIT
	fprintf(stdout, "\nCalling LumiExit...");
	fprintf(logFileID, "\nCalling LumiExit...");
	rc = LumiExit();
	PrintReturnCode(rc);
	if(rc != LUMI_STATUS_OK) return false;

	return true;
}

bool TestFIR()
{
	uint				nNumDevices = 0;
	LUMI_DEVICE			dev;
	LUMI_HANDLE			connHandle = 0;
	LumiStatus			rc = LUMI_STATUS_OK;

	// QUERY NUMBER DEVICES
	fprintf(stdout, "\nQuerying Number of Devices.  This may take several seconds...");
	rc = LumiQueryNumberDevices(nNumDevices, NULL);
	PrintReturnCode(rc);
	if(rc != LUMI_STATUS_OK) return false;

	//Check if only one Device is connected
	if(nNumDevices < 1)
	{
		fprintf(stdout, "\nNo Device found. Please connect a Device.");
		return false;
	}

	if(nNumDevices > 1)
	{
		fprintf(stdout, "\nFound %d Devices. Please connect only one Device",nNumDevices);
		return false;
	}

	// QUERY DEVICE
	fprintf(stdout, "\nQuerying Device...");
	rc = LumiQueryDevice(0, dev);
	PrintReturnCode(rc);
	if(rc != LUMI_STATUS_OK) return false;

	// INIT
	fprintf(stdout, "\nInitializing Device...");
	rc = LumiInit(dev.hDevHandle, connHandle);
	PrintReturnCode(rc);
	if(rc != LUMI_STATUS_OK) return false;
	fprintf(stdout, "\nInit Returned Connection Handle %d.", connHandle);

	uint nW, nH, nDPI, nBPP;
	int nSpoof;

	// Get Image Params
	fprintf(stdout, "\nCalling LumiGetImageParams...");
	rc = LumiGetImageParams(connHandle, nW, nH, nBPP, nDPI);
	PrintReturnCode(rc);
	if(rc != LUMI_STATUS_OK) return false;

	uchar* pImage = new uchar[nW * nH];
	uchar* pImage1 = new uchar[nW * nH];

	// Create FIRecord
	FIRECORD_HANDLE hRecord  = NULL;
	LUMI_COMPRESSION_TYPE CompressionType = COMPRESSED_WSQ;
	LUMI_FIR_STANDARD Standard = FIR_ISO;
	fprintf(stdout, "\nCalling LumiFIRecordCreate...");
	rc = LumiFIRecordCreate(nBPP, CompressionType, Standard, &hRecord);
	PrintReturnCode(rc);
	if(rc != LUMI_STATUS_OK) return false;

	// First capture with finger positon1
	fprintf(stdout, "\nFirst capture with right index finger... Place right index finger on sensor ");
	rc = LumiCapture(connHandle, pImage, nSpoof, NULL);
	PrintReturnCode(rc);
	if(rc != LUMI_STATUS_OK) return false;
	fprintf(stdout, "Spoof %d\n",nSpoof);	

	// Add first captured image with finger positon1
	LUMI_FINGER_PALM_POSITION FingerPosition1 = FINGER_POSITION_RIGHT_INDEX_FINGER;
	LUMI_IMPRESSION_TYPE ImpressionType = LIVE_SCAN_PLAIN;
	uint nViewNumberOfFinger;
	fprintf(stdout, "\nCalling LumiFIRecordAddView...");
	rc = LumiFIRecordAddView(hRecord, pImage, nW, nH, FingerPosition1, ImpressionType, nViewNumberOfFinger);
	PrintReturnCode(rc);
	if(rc != LUMI_STATUS_OK) return false;
	fprintf(stdout, "View number %d\n",nViewNumberOfFinger);

	// Second capture with finger positon1
	fprintf(stdout, "\nSecond capture with right index finger... Place right index finger on sensor ");
	rc = LumiCapture(connHandle, pImage, nSpoof, NULL);
	PrintReturnCode(rc);
	if(rc != LUMI_STATUS_OK) return false;
	fprintf(stdout, "Spoof %d\n",nSpoof);

	// Add second captured image with finger positon1
	fprintf(stdout, "\nCalling LumiFIRecordAddView...");
	rc = LumiFIRecordAddView(hRecord, pImage, nW, nH, FingerPosition1, ImpressionType, nViewNumberOfFinger);
	PrintReturnCode(rc);
	if(rc != LUMI_STATUS_OK) return false;
	fprintf(stdout, "View number %d\n",nViewNumberOfFinger);

	// First capture with finger positon2
	LUMI_FINGER_PALM_POSITION FingerPosition2 = FINGER_POSITION_RIGHT_MIDDLE_FINGER;
	fprintf(stdout, "\nFirst capture with right middle finger... Place right middle finger on sensor ");
	rc = LumiCapture(connHandle, pImage, nSpoof, NULL);
	PrintReturnCode(rc);
	if(rc != LUMI_STATUS_OK) return false;
	fprintf(stdout, "Spoof %d\n",nSpoof);	

	// Add first captured image with finger positon2
	fprintf(stdout, "\nCalling LumiFIRecordAddView...");
	rc = LumiFIRecordAddView(hRecord, pImage, nW, nH, FingerPosition2, ImpressionType, nViewNumberOfFinger);
	PrintReturnCode(rc);
	if(rc != LUMI_STATUS_OK) return false;
	fprintf(stdout, "View number %d\n",nViewNumberOfFinger);

	// Second capture with finger positon2
	fprintf(stdout, "\nSecond capture with right middle finger... Place right middle finger on sensor ");
	rc = LumiCapture(connHandle, pImage, nSpoof, NULL);
	PrintReturnCode(rc);
	if(rc != LUMI_STATUS_OK) return false;
	fprintf(stdout, "Spoof %d\n",nSpoof);

	// Add second captured image with finger positon2
	fprintf(stdout, "\nCalling LumiFIRecordAddView...");
	rc = LumiFIRecordAddView(hRecord, pImage, nW, nH, FingerPosition2, ImpressionType, nViewNumberOfFinger);
	PrintReturnCode(rc);
	if(rc != LUMI_STATUS_OK) return false;
	fprintf(stdout, "View number %d\n",nViewNumberOfFinger);

	// Get FIRecord parameters
	fprintf(stdout, "\nCalling LumiFIRecordGetParameters...");
	uint nDeviceID1, nImageAcqLevel1, nFingers1, nScaleUnit1, nScanXres1, nScanYres1, nImageXres1, nImageYres1, nBPP1;
	LUMI_COMPRESSION_TYPE CompressionType1;
	LUMI_FIR_STANDARD Standard1;
	rc = LumiFIRecordGetParameters(hRecord, nDeviceID1, nImageAcqLevel1, nFingers1, nScaleUnit1, nScanXres1, nScanYres1, nImageXres1, nImageYres1, nBPP1, CompressionType1, Standard1);
	PrintReturnCode(rc);
	if(rc != LUMI_STATUS_OK) return false;
	fprintf(stdout, "\nRecord parameters: \nDeviceID = %d, \nImageAcqLevel = %d, \nnFingers = %d, \nScaleUnit = %d, \nScanXres = %d, \
			\nScanYres = %d, \nImageXres = %d, \nImageYres = %d, \nBPP = %d, \nCompressionType = %d, \nStandard = %d\n", 
			nDeviceID1, nImageAcqLevel1, nFingers1, nScaleUnit1, nScanXres1, nScanYres1, nImageXres1, nImageYres1, nBPP1, CompressionType1, Standard1);

	// Get View parameters for finger position1
	fprintf(stdout, "\nCalling LumiFIRecordGetViewParameters...");
	uint nViewCount1, nImageQuality1, nW1, nH1;
	LUMI_IMPRESSION_TYPE ImpressionType1;
	rc = LumiFIRecordGetViewParameters(hRecord, FingerPosition1, 1, nViewCount1, nImageQuality1, ImpressionType1, nW1, nH1);
	PrintReturnCode(rc);
	if(rc != LUMI_STATUS_OK) return false;
	fprintf(stdout, "\nView parameters for finger position = %d and view 1: \nViewCount = %d, \nImageQuality = %d, \nImpressionType = %d, \
					\nWidth = %d, \nHeight = %d\n",  
					FingerPosition1, nViewCount1, nImageQuality1, ImpressionType1, nW1, nH1);

	// Remove second captured image for finger positon2
	fprintf(stdout, "\nCalling LumiFIRecordRemoveView...");
	rc = LumiFIRecordRemoveView(hRecord, FingerPosition2, 2);
	PrintReturnCode(rc);
	if(rc != LUMI_STATUS_OK) return false;
	
	// Get View count for finger positon2
	fprintf(stdout, "\nCalling LumiFIRecordGetViewCount...");
	uint nViewCount2;
	rc = LumiFIRecordGetViewCount(hRecord, FingerPosition2, nViewCount2);
	PrintReturnCode(rc);
	if(rc != LUMI_STATUS_OK) return false;
	fprintf(stdout, "View count for finger position %d = %d\n", FingerPosition2, nViewCount2);

	// Save FIRecord
	fprintf(stdout, "\nCalling LumiFIRecordSaveToFile...");
	char* pFileName = "FIRecord.bin";
	size_t nNameSize = strlen(DataPath) + strlen(pFileName) + 10;
    char* pFullFileName = new char[nNameSize];
	sprintf_s(pFullFileName,nNameSize,"%s\\%s",DataPath, pFileName);
	rc = LumiFIRecordSaveToFile(hRecord, pFullFileName);
	PrintReturnCode(rc);
	if(rc != LUMI_STATUS_OK) return false;

	// Release FIRecord
	fprintf(stdout, "\nCalling LumiFIRecordRelease...");
	rc = LumiFIRecordRelease(hRecord);
	PrintReturnCode(rc);
	if(rc != LUMI_STATUS_OK) return false;

	// Load FIRecord
	fprintf(stdout, "\nCalling LumiFIRecordLoadFromFile...");
	rc = LumiFIRecordLoadFromFile(pFullFileName, Standard, &hRecord);
	PrintReturnCode(rc);
	if(rc != LUMI_STATUS_OK) return false;
	delete[] pFullFileName; pFullFileName = NULL;

	// Get FIRecord parameters
	fprintf(stdout, "\nCalling LumiFIRecordGetParameters...");
	uint nDeviceID2, nImageAcqLevel2, nFingers2, nScaleUnit2, nScanXres2, nScanYres2, nImageXres2, nImageYres2, nBPP2;
	LUMI_COMPRESSION_TYPE CompressionType2;
	LUMI_FIR_STANDARD Standard2;
	rc = LumiFIRecordGetParameters(hRecord, nDeviceID2, nImageAcqLevel2, nFingers2, nScaleUnit2, nScanXres2, nScanYres2, nImageXres2, nImageYres2, nBPP2, CompressionType2, Standard2);
	PrintReturnCode(rc);
	if(rc != LUMI_STATUS_OK) return false;
	fprintf(stdout, "\nRecord parameters: \nDeviceID = %d, \nImageAcqLevel = %d, \nnFingers = %d, \nScaleUnit = %d, \nScanXres = %d, \
			\nScanYres = %d, \nImageXres = %d, \nImageYres = %d, \nBPP = %d, \nCompressionType = %d, \nStandard = %d\n", 
			nDeviceID2, nImageAcqLevel2, nFingers2, nScaleUnit2, nScanXres2, nScanYres2, nImageXres2, nImageYres2, nBPP2, CompressionType2, Standard2);

	// Get View parameters for finger positon2
	fprintf(stdout, "\nCalling LumiFIRecordGetViewParameters...");
	uint nImageQuality2, nW2, nH2;
	LUMI_IMPRESSION_TYPE ImpressionType2;
	rc = LumiFIRecordGetViewParameters(hRecord, FingerPosition2, 1, nViewCount2, nImageQuality2, ImpressionType2, nW2, nH2);
	PrintReturnCode(rc);
	if(rc != LUMI_STATUS_OK) return false;
	fprintf(stdout, "\nView parameters for finger position = %d and view 1: \nViewCount = %d, \nImageQuality = %d, \nImpressionType = %d, \
					\nWidth = %d, \nHeight = %d\n",  
					FingerPosition2, nViewCount2, nImageQuality2, ImpressionType2, nW2, nH2);

	// Get First captured image for finger positon1
	fprintf(stdout, "\nCalling LumiFIRecordGetImage...");
	rc = LumiFIRecordGetImage(hRecord, FingerPosition1, 1, pImage1, nW, nH);
	PrintReturnCode(rc);
	if(rc != LUMI_STATUS_OK) return false;

	// Save image as bitmap
	fprintf(stdout, "\nCalling LumiSaveAsBitmapToFile...");
	pFileName = "FIRecordImage1.bmp";
	nNameSize = strlen(DataPath) + strlen(pFileName) + 10;
    pFullFileName = new char[nNameSize];
	sprintf_s(pFullFileName,nNameSize,"%s\\%s",DataPath, pFileName);
    rc = LumiSaveAsBitmapToFile(pImage1, nW, nH, nBPP, pFullFileName);
    PrintReturnCode(rc);
    if(rc != LUMI_STATUS_OK) return false;
	delete[] pFullFileName; pFullFileName = NULL;

	// Release FIRecord
	fprintf(stdout, "\nCalling LumiFIRecordRelease...");
	rc = LumiFIRecordRelease(hRecord);
	PrintReturnCode(rc);
	if(rc != LUMI_STATUS_OK) return false;	

	delete[] pImage;
	delete[] pImage1;



	// EXIT
	fprintf(stdout, "\nCalling LumiExit...");
	rc = LumiExit();
	PrintReturnCode(rc);
	if(rc != LUMI_STATUS_OK) return false;

	return true;
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
	"LUMI_STATUS_NO_DEVICE_FOUND",
	"LUMI_STATUS_ERROR_READ_FILE",
	"LUMI_STATUS_ERROR_WRITE_FILE",
	"LUMI_STATUS_INVALID_FILE_FORMAT",
	"LUMI_STATUS_ERROR_TIMEOUT_LATENT",
	"LUMI_STATUS_QUALITY_MAP_NOT_GENERATED",
	"LUMI_STATUS_THREAD_ERROR",
	"LUMI_STATUS_ERROR_SENSOR_COMM_TIMEOUT",
	"LUMI_STATUS_DEVICE_STATUS_ERROR"};

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
		case LUMI_STATUS_NO_DEVICE_FOUND: { nReturnCodeIndex = 25; } break;				
		case LUMI_STATUS_ERROR_READ_FILE: { nReturnCodeIndex = 26; } break;				
		case LUMI_STATUS_ERROR_WRITE_FILE: { nReturnCodeIndex = 27; } break;			
		case LUMI_STATUS_INVALID_FILE_FORMAT: { nReturnCodeIndex = 28; } break;	
		case LUMI_STATUS_ERROR_TIMEOUT_LATENT: { nReturnCodeIndex = 29; } break;
		case LUMI_STATUS_QUALITY_MAP_NOT_GENERATED: { nReturnCodeIndex = 30; } break;
		case LUMI_STATUS_THREAD_ERROR: { nReturnCodeIndex = 31; } break;
		case LUMI_STATUS_ERROR_SENSOR_COMM_TIMEOUT: { nReturnCodeIndex = 32; } break;
		case LUMI_STATUS_DEVICE_STATUS_ERROR: { nReturnCodeIndex = 33; } break;
	}
	return (char*) pReturnCode[nReturnCodeIndex];
}