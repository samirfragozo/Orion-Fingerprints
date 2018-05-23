
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

#pragma once
#include "stdafx.h"
#include "StreamCheck.h"
#include "StreamCheckDlg.h"
#include "LumiAPI.h"

// To be cheap and easy, just define some globals for easy access
HWND					g_Hwnd = 0;
bool					g_bCancel = false;
StreamCaptureCheckState g_StreamCheckCaptureState;

#ifdef _DEBUG
#define new DEBUG_NEW
#endif

// Declarations and definitions
#define ACQ_NONE			100
#define PD_IMAGE			0
#define COMPOSITE_IMAGE		1
#define PD_PREVIEW_CALLBACK	2

LUMI_DEVICE			dev;
LUMI_HANDLE			connHandle = 0;
LUMI_SENSOR_TYPE	g_sensorType;

typedef struct tagImageContainer
{
	uint				nWidth;
	uint				nHeight;
	uint				nBPP;
	uint				nDPI;
	uchar*				pImg;
	uint				nStatus;
} ImageContainer;

// CAboutDlg dialog used for App About

class CAboutDlg : public CDialog
{
public:
	CAboutDlg();

// Dialog Data
	enum { IDD = IDD_ABOUTBOX };

	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support

// Implementation
protected:
	DECLARE_MESSAGE_MAP()
};

CAboutDlg::CAboutDlg() : CDialog(CAboutDlg::IDD)
{
}

void CAboutDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
}

BEGIN_MESSAGE_MAP(CAboutDlg, CDialog)
END_MESSAGE_MAP()


// CStreamCheckDlg dialog

void PreviewCallback(unsigned char* pImage, int nWidth, int nHeight, int nNum);   
int  PDCallback(unsigned char* pImage, int nWidth, int nHeight, uint nStatus); 

DWORD WINAPI CaptureWithPreviewThread(void* pObjPtr);

#define WM_THREAD_MESSAGE WM_USER+1



CStreamCheckDlg::CStreamCheckDlg(CWnd* pParent /*=NULL*/)
	: CDialog(CStreamCheckDlg::IDD, pParent)
	, m_bPDIsInColor(FALSE)
	, m_bOverrideTrigger(false)
{
	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);
	m_hLumiCaptureThreadHandle = NULL;
}

CStreamCheckDlg::~CStreamCheckDlg()
{
	//turn off the stream to save the sensor from rebooting
	LumiStatus rc = LUMI_STATUS_OK;
	rc = LumiSetLiveMode(connHandle, LUMI_VID_OFF,PreviewCallback);
	LUMI_PRES_DET_MODE Argument2 = LUMI_PD_ON;
	rc = LumiSetOption(connHandle, LUMI_OPTION_SET_PRESENCE_DET_MODE, &Argument2, sizeof(Argument2));
	if(m_pImage) {delete[] m_pImage; m_pImage = NULL;}
	if(m_pQualityMap) {delete[] m_pQualityMap; m_pQualityMap = NULL;}
}

void CStreamCheckDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	DDX_Check(pDX, IDC_CHECK_COLOR_PD, m_bPDIsInColor);
	DDX_Check(pDX, IDC_CHECK_OVERRIDE_TRIGGER, m_bOverrideTrigger);
	DDX_Control(pDX, IDC_IMAGE_DISPLAY, m_ImageDisplay);
}

BEGIN_MESSAGE_MAP(CStreamCheckDlg, CDialog)
	ON_WM_SYSCOMMAND()
	ON_WM_PAINT()
	ON_WM_QUERYDRAGICON()
	ON_BN_CLICKED(IDC_BUTTON2, &CStreamCheckDlg::OnBnClickedButton2)
	ON_BN_CLICKED(IDC_BUTTON3, &CStreamCheckDlg::OnBnClickedButton3)
	ON_STN_CLICKED(IDC_IMAGE_DISPLAY, &CStreamCheckDlg::OnStnClickedImageDisplay)
	ON_BN_CLICKED(IDC_CAPTURE, &CStreamCheckDlg::OnBnClickedCapture)
	ON_BN_CLICKED(IDC_SAVE_BITMAP, &CStreamCheckDlg::OnBnClickedSaveBitmap)
	ON_MESSAGE(WM_THREAD_MESSAGE, OnHandleThreadMessage)
	ON_BN_CLICKED(IDC_CHECK_COLOR_PD, &CStreamCheckDlg::OnBnClickedCheckColorPd)
	ON_BN_CLICKED(IDC_SWITCH_VIEW, &CStreamCheckDlg::OnBnClickedSwitchView)
	ON_BN_CLICKED(IDC_CHECK_OVERRIDE_TRIGGER, &CStreamCheckDlg::OnBnClickedCheckOverrideTrigger)
END_MESSAGE_MAP()


// CStreamCheckDlg message handlers

BOOL CStreamCheckDlg::OnInitDialog()
{
	CDialog::OnInitDialog();

	// Add "About..." menu item to system menu.

	// IDM_ABOUTBOX must be in the system command range.
	ASSERT((IDM_ABOUTBOX & 0xFFF0) == IDM_ABOUTBOX);
	ASSERT(IDM_ABOUTBOX < 0xF000);

	CMenu* pSysMenu = GetSystemMenu(FALSE);
	if (pSysMenu != NULL)
	{
		CString strAboutMenu;
		strAboutMenu.LoadString(IDS_ABOUTBOX);
		if (!strAboutMenu.IsEmpty())
		{
			pSysMenu->AppendMenu(MF_SEPARATOR);
			pSysMenu->AppendMenu(MF_STRING, IDM_ABOUTBOX, strAboutMenu);
		}
	}

	// Set the icon for this dialog.  The framework does this automatically
	//  when the application's main window is not a dialog
	SetIcon(m_hIcon, TRUE);			// Set big icon
	SetIcon(m_hIcon, FALSE);		// Set small icon

	/* Initialize the sensor */
	if(false == LoadLumiDevice())
	{
		GetDlgItem(IDC_SWITCH_VIEW)->EnableWindow(FALSE);
		GetDlgItem(IDC_CAPTURE)->EnableWindow(FALSE);
		GetDlgItem(IDC_SAVE_BITMAP)->EnableWindow(FALSE);
		GetDlgItem(IDC_BUTTON2)->EnableWindow(FALSE);
		GetDlgItem(IDC_BUTTON3)->EnableWindow(FALSE);
		GetDlgItem(IDC_CHECK_COLOR_PD)->EnableWindow(FALSE);
		GetDlgItem(IDC_CHECK_OVERRIDE_TRIGGER)->EnableWindow(FALSE);
	}
	else
	{
		/* Update dlg controls */
		UpdateUI(TRUE);
	}
	/* Set global hwnd */
	g_Hwnd = m_hWnd;
	/* Set global capture state */
	g_StreamCheckCaptureState = SC_CAPTURE_NOT_IN_PROGRESS;

	m_bDisplayingQualityMap = false;
	m_pImage = NULL;
	m_pQualityMap = NULL;
	m_nWidth = 0;
	m_nHeight = 0;
	return TRUE;  // return TRUE  unless you set the focus to a control
}

void CStreamCheckDlg::OnSysCommand(UINT nID, LPARAM lParam)
{
	if ((nID & 0xFFF0) == IDM_ABOUTBOX)
	{
		CAboutDlg dlgAbout;
		dlgAbout.DoModal();
	}
	else
	{
		CDialog::OnSysCommand(nID, lParam);
	}
}

// If you add a minimize button to your dialog, you will need the code below
//  to draw the icon.  For MFC applications using the document/view model,
//  this is automatically done for you by the framework.

void CStreamCheckDlg::OnPaint()
{
	if (IsIconic())
	{
		CPaintDC dc(this); // device context for painting

		SendMessage(WM_ICONERASEBKGND, reinterpret_cast<WPARAM>(dc.GetSafeHdc()), 0);

		// Center icon in client rectangle
		int cxIcon = GetSystemMetrics(SM_CXICON);
		int cyIcon = GetSystemMetrics(SM_CYICON);
		CRect rect;
		GetClientRect(&rect);
		int x = (rect.Width() - cxIcon + 1) / 2;
		int y = (rect.Height() - cyIcon + 1) / 2;

		// Draw the icon
		dc.DrawIcon(x, y, m_hIcon);
	}
	else
	{
		CDialog::OnPaint();
	}
}

// The system calls this function to obtain the cursor to display while the user drags
//  the minimized window.
HCURSOR CStreamCheckDlg::OnQueryDragIcon()
{
	return static_cast<HCURSOR>(m_hIcon);
}
/*
** Initializes the sensor
** If more than one sensor is connected, opens the first one.
** Alter this code if you would like to have control over which
** sensor is opened.
*/

bool CStreamCheckDlg::LoadLumiDevice()
{
	uint				nNumDevices = 0;
	
	LumiStatus			rc = LUMI_STATUS_OK;
	int					nDev = -1;
	
	// QUERY NUMBER DEVICES
	rc = LumiQueryNumberDevices(nNumDevices, NULL);
	if(nNumDevices == 0)
	{
		AfxMessageBox(L"No Devices connected!\nPlease connect a V30x or V31x sensor and try again.", MB_OK|MB_ICONINFORMATION);
		return false;
	}

	rc = LumiQueryDevice(0, dev);

	g_sensorType = dev.SensorType;

	if(VENUS != dev.SensorType && V31X != dev.SensorType)
	{
		AfxMessageBox(L"The connected device is not supported in this tool!\nPlease connect a V30x or V31x sensor and try again.", MB_OK|MB_ICONINFORMATION);
		return false;
	}

	rc = LumiInit(dev.hDevHandle, connHandle);
	LUMI_PRES_DET_MODE Argument2 = LUMI_PD_ON;
	rc = LumiSetOption(connHandle, LUMI_OPTION_SET_PRESENCE_DET_MODE, &Argument2, sizeof(Argument2));
	return true;
}

void CStreamCheckDlg::OnBnClickedButton1()
{
	
}	

/*
** Turn Live Mode on
*/

void CStreamCheckDlg::OnBnClickedButton2()
{
	LumiSetLiveMode(connHandle, LUMI_VID_ON,PreviewCallback);
	GetDlgItem(IDC_CHECK_COLOR_PD)->EnableWindow(FALSE);
}

/* 
** Callback for non Image-Preview status
*/

void PreviewCallback(unsigned char* pImage, int nWidth, int nHeight, int nNum)   
{
	ImageContainer* cnt = new ImageContainer;
	memset(cnt, 0, sizeof(ImageContainer));
		
	cnt->nWidth = nWidth;
	cnt->nHeight = nHeight;
	if(nNum == -1)
	{
		cnt->nBPP = 24;
		cnt->pImg = new uchar[nWidth * nHeight * 3];
		memcpy(cnt->pImg, pImage, nWidth * nHeight * 3);
	}
	else
	{
		cnt->nBPP = 8;
		cnt->pImg = new uchar[nWidth * nHeight];
		memcpy(cnt->pImg, pImage, nWidth * nHeight);
	}	
	::PostMessage(g_Hwnd, WM_THREAD_MESSAGE, PD_PREVIEW_CALLBACK, (LPARAM) cnt);
}
/*
** Turn Live-Mode without PD on.
*/
void CStreamCheckDlg::OnBnClickedButton3()
{
	LumiSetLiveMode(connHandle, LUMI_VID_OFF,PreviewCallback);
	GetDlgItem(IDC_CHECK_COLOR_PD)->EnableWindow(TRUE);
}

void CStreamCheckDlg::OnStnClickedImageDisplay()
{
	// TODO: Add your control notification handler code here
}
/* 
**  Capture
**  Creates capture thread, which begins capture process.
*/
void CStreamCheckDlg::OnBnClickedCapture()
{	
	if(g_StreamCheckCaptureState == SC_CAPTURE_NOT_IN_PROGRESS)
	{
		// Turn Live Mode off, if its on
		LumiSetLiveMode(connHandle, LUMI_VID_OFF, PreviewCallback);

		GetDlgItem(IDC_SWITCH_VIEW)->SetWindowTextW(L"Show Quality Map");
		m_bDisplayingQualityMap = false;

		g_StreamCheckCaptureState = SC_CAPTURE_IN_PROGRESS;
		g_bCancel = false;		
		// Start Thread
		m_hLumiCaptureThreadHandle = CreateThread(NULL,0,CaptureWithPreviewThread,this, NULL, NULL);
		// Turn off controls
		UpdateUI(0);		
	}
	else if(g_StreamCheckCaptureState == SC_CAPTURE_IN_PROGRESS)
	{
		g_StreamCheckCaptureState = SC_CAPTURE_NOT_IN_PROGRESS;
		g_bCancel = true;	
		// Wait for thread to exit
		if(m_hLumiCaptureThreadHandle)
		{
			if(WAIT_TIMEOUT == WaitForSingleObject(m_hLumiCaptureThreadHandle, 5000))
			{
				AfxMessageBox(L"Could not exit capture with preview thread");
			}
			m_hLumiCaptureThreadHandle = NULL;
		}
	}
}
/* 
** Mechanism for GUI/Worker thread interaction
*/

LRESULT CStreamCheckDlg::OnHandleThreadMessage(WPARAM nL, LPARAM nM)
{
	ImageContainer* pIM = (ImageContainer*)(nM);
	switch(nL)
	{
		// Update the UI
		case PD_IMAGE:
		{			
			if(m_ImageDisplay)
			{
				m_ImageDisplay.Invalidate();
				m_ImageDisplay.SetImagePosition(pIM->nStatus);
				m_ImageDisplay.SetImage(pIM->pImg, pIM->nWidth,pIM->nBPP,pIM->nHeight);
			}
		} break;
		// Done Capturing.
		case COMPOSITE_IMAGE:
		{
			if(m_ImageDisplay)
			{
				m_ImageDisplay.Invalidate();
				m_ImageDisplay.SetImagePosition(ACQ_NONE);
				m_ImageDisplay.SetImage(pIM->pImg, pIM->nWidth,pIM->nBPP,pIM->nHeight);

				m_nWidth = pIM->nWidth;
				m_nHeight = pIM->nHeight;

				// Allocate the composite image buffer used for switching views with the quality map
				if(m_pImage == NULL)
				{
					m_pImage = new uchar[pIM->nWidth * pIM->nHeight];
				}
				// Save the composite image to the member image variable
				memcpy(m_pImage, pIM->pImg, pIM->nWidth * pIM->nHeight);
				
				// Allocate the quality map
				if(m_pQualityMap == NULL)
				{
					m_pQualityMap = new uchar[pIM->nWidth * pIM->nHeight];
				}				
				// Make the quality map all white
				memset(m_pQualityMap, 255, m_nWidth * m_nHeight);
			}
			UpdateUI(1);
			g_StreamCheckCaptureState = SC_CAPTURE_NOT_IN_PROGRESS;

		} break;
		case PD_PREVIEW_CALLBACK:
		{
			if(m_ImageDisplay)
			{
				m_ImageDisplay.Invalidate();
				m_ImageDisplay.SetImage(pIM->pImg, pIM->nWidth,pIM->nBPP,pIM->nHeight);
			}
		} break;
	}
	
	// Clean up the image container
	delete[] pIM->pImg;
	delete pIM;

	return 0;
}
/*
** Its Always nice to be able to save images on demand
*/

void CStreamCheckDlg::OnBnClickedSaveBitmap()
{
	// Turn off Live Mode
	OnBnClickedButton3();
	// Pick Directory
	CFileDialog dlg(FALSE,L"bmp",NULL, OFN_OVERWRITEPROMPT, L"Bitmaps (*.bmp)|*.bmp||");
	int ret = (int)dlg.DoModal();

	switch(ret)
	{
	case IDOK:
		{
			CString path = dlg.GetPathName();
			m_ImageDisplay.SaveCurrentImage(path);
			return;
		} break;
	}
	return;
}
/* 
** Lets disable he UI buttons while the capture thread is
** active
*/ 
void CStreamCheckDlg::UpdateUI(int nTurnOn)
{
	if(nTurnOn == FALSE)
	{
		// Change Capture button to Cancel button
		GetDlgItem(IDC_CAPTURE)->SetWindowTextW(L"Cancel");
	}
	else
	{
		// Restore Capture button
		GetDlgItem(IDC_CAPTURE)->SetWindowTextW(L"Capture");
	}
	GetDlgItem(IDC_BUTTON2)->EnableWindow(nTurnOn);
	GetDlgItem(IDC_BUTTON3)->EnableWindow(nTurnOn);
	GetDlgItem(IDC_SAVE_BITMAP)->EnableWindow(nTurnOn);
	GetDlgItem(IDC_CHECK_COLOR_PD)->EnableWindow(nTurnOn);
	if(g_sensorType == M300 || g_sensorType == M100)
	{
		GetDlgItem(IDC_SWITCH_VIEW)->EnableWindow(FALSE);
	}
	else
	{
		GetDlgItem(IDC_SWITCH_VIEW)->EnableWindow(nTurnOn);
	}

	GetDlgItem(IDC_CHECK_OVERRIDE_TRIGGER)->EnableWindow(nTurnOn);
}
/*
** In order to capture with preview, I need to make sure that this Window's
** Message queue is not blocked by a call to capture, as I want to display
** the images that come back from the callback, and handle the call appropriately.
*/

DWORD WINAPI CaptureWithPreviewThread(void* pObjPtr)
{
	CStreamCheckDlg* pDlg = reinterpret_cast<CStreamCheckDlg*>(pObjPtr);
	LUMI_CONFIG config;
	int	 Spoof;
	uint rc;
	// Get Config
	if(LUMI_STATUS_OK != LumiGetConfig(connHandle, config)) return -1;
	// Increase Presence Detection Time-Out
	config.nTriggerTimeout = 0;
	// Set Configuration
	if(LUMI_STATUS_OK != LumiSetConfig(connHandle, config)) return -1;
	
	ImageContainer* cnt = new ImageContainer;
	memset(cnt, 0, sizeof(ImageContainer));
		
	if(LUMI_STATUS_OK != LumiGetImageParams(connHandle,cnt->nWidth,cnt->nHeight,cnt->nBPP,cnt->nDPI)) return -1;
		
	// Allocate buffer
	cnt->pImg = new uchar[cnt->nWidth * cnt->nHeight];

	// Set the Presence detection preview mode
	rc = LumiSetOption(connHandle, LUMI_OPTION_SET_PRESENCE_DET_CALLBACK, PDCallback, sizeof(LumiPresenceDetectCallback));

	// Issue call to capture (blocking)	
	LumiStatus rcCapture = LumiCapture(connHandle, cnt->pImg, Spoof, NULL);	

	

	if(rcCapture != LUMI_STATUS_OK) // Display white image if error or user cancels
	{
		memset(cnt->pImg, 255, cnt->nWidth * cnt->nHeight);
	}
		
	// Post a thread message passing the composite image to the gui
	::PostMessage(g_Hwnd, WM_THREAD_MESSAGE, COMPOSITE_IMAGE, (LPARAM) cnt);
	return 0;
};


/* This is the callback for Presence Detection Images and Status
** For a fluid user experience, this type of callback should return
** as fast as possible, though failure to return in a timely manner
** will not impact the timing of acquisition, or the return to your
** call to capture
*/ 
int PDCallback(unsigned char* pImage, int nWidth, int nHeight, uint nStatus)  
{
	ImageContainer* cnt = new ImageContainer;
	memset(cnt, 0, sizeof(ImageContainer));
	cnt->pImg = new uchar[nWidth * nHeight * 3];	
	cnt->nWidth = nWidth;
	cnt->nHeight = nHeight;
	cnt->nBPP = 24;
	memcpy(cnt->pImg, pImage, nWidth * nHeight * 3);
	if(g_sensorType == M100 || g_sensorType == M300)
	{
		cnt->nStatus = ACQ_NONE;
	}
	else
	{
		cnt->nStatus = nStatus;
	}
	::PostMessage(g_Hwnd, WM_THREAD_MESSAGE, PD_IMAGE, (LPARAM) cnt);

	// If the cancel button has been clicked, stop the capture by returning -2
	if(g_bCancel) return -2;
	return 0;
}
void CStreamCheckDlg::OnBnClickedCheckColorPd()
{
	UpdateData(TRUE);
	uint rc = LumiSetOption(connHandle, LUMI_OPTION_SET_PRESENCE_DET_COLOR, &m_bPDIsInColor, sizeof(int));
}

void CStreamCheckDlg::OnBnClickedSwitchView()
{	
	if(!m_bDisplayingQualityMap && !g_bCancel)
	{		
		LUMI_VERSION version;
		memset(&version, 0, sizeof(version));
		uint rc = LumiGetVersionInfo(connHandle, version);
		
		int nFW = atoi(version.fwrVersion);
		if(dev.SensorType == VENUS && nFW >= 21304)
		{
			MessageBox(L"The connected sensor does not support quality map generation.", L"StreamCheck", 0);
			
		}
		else
		{
			rc = LumiGetQualityMap(connHandle, m_pQualityMap);	

			if(rc == LUMI_STATUS_ERROR_CMD_NOT_SUPPORTED)
			{
				MessageBox(L"The connected sensor does not support quality map generation.", L"StreamCheck", 0);
			}
		}
		
		

		// display the quality map
		m_ImageDisplay.SetImage(m_pQualityMap,m_nWidth,8,m_nHeight);		
		GetDlgItem(IDC_SWITCH_VIEW)->SetWindowTextW(L"Show Image");
		m_bDisplayingQualityMap = true;
	}
	else
	{
		// display the composite image
		m_ImageDisplay.SetImage(m_pImage,m_nWidth,8,m_nHeight);
		GetDlgItem(IDC_SWITCH_VIEW)->SetWindowTextW(L"Show Quality Map");
		m_bDisplayingQualityMap = false;
	}
}

void CStreamCheckDlg::OnBnClickedCheckOverrideTrigger()
{
	UpdateData(TRUE);
	LUMI_PRES_DET_MODE Argument2;
	if(m_bOverrideTrigger == TRUE)
	{
		Argument2 = LUMI_PD_OFF;
	}
	else
	{
		Argument2 = LUMI_PD_ON;
	}
	LumiSetOption(connHandle, LUMI_OPTION_SET_PRESENCE_DET_MODE, &Argument2, sizeof(Argument2));
	
}
