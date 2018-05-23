
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

#include "lumi_stdint.h"

typedef uint32_t u32;
typedef uint16_t u16;
typedef uint8_t u8;


#include "imagedisplayctrl.h"
//#include "lumidisplayctrl1.h"

typedef enum
{
	SC_CAPTURE_NOT_IN_PROGRESS,
	SC_CAPTURE_IN_PROGRESS,						
} StreamCaptureCheckState;

// CStreamCheckDlg dialog
class CStreamCheckDlg : public CDialog
{
// Construction
public:
	CStreamCheckDlg(CWnd* pParent = NULL);	// standard constructor

// Destructor
	~CStreamCheckDlg();

// Dialog Data
	enum { IDD = IDD_STREAMCHECK_DIALOG };

	protected:
	virtual void DoDataExchange(CDataExchange* pDX);	// DDX/DDV support


// Implementation
protected:
	HICON m_hIcon;
	bool LoadLumiDevice();
	// Generated message map functions
	virtual BOOL OnInitDialog();
	afx_msg void OnSysCommand(UINT nID, LPARAM lParam);
	afx_msg void OnPaint();
	afx_msg HCURSOR OnQueryDragIcon();
	DECLARE_MESSAGE_MAP()
public:
	afx_msg void OnBnClickedButton1();
public:
	afx_msg void OnBnClickedButton2();
public:
//	CLumidisplayctrl1 m_Display;
public:
	afx_msg void OnBnClickedButton3();
public:
	ImageDisplayCtrl m_ImageDisplay;
public:
	afx_msg void OnStnClickedImageDisplay();
public:
	afx_msg void OnBnClickedCapture();
public:
	afx_msg void OnBnClickedSaveBitmap();
public:
	afx_msg LRESULT OnHandleThreadMessage(WPARAM nL, LPARAM nM);
	unsigned char* m_pImage;
	unsigned char* m_pQualityMap;
	unsigned int m_nWidth, m_nHeight;
	
private:
	void UpdateUI(int nTurnOn);
	afx_msg void OnBnClickedCheckColorPd();
	BOOL m_bPDIsInColor;	
	BOOL m_bOverrideTrigger;
	afx_msg void OnBnClickedSwitchView();
	afx_msg void OnBnClickedCheckOverrideTrigger();
	bool m_bDisplayingQualityMap;
	HANDLE m_hLumiCaptureThreadHandle;	
};
