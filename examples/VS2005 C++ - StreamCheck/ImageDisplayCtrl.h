
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

#include <gdiplus.h>
using namespace Gdiplus;
#pragma comment(lib, "gdiplus.lib")

// ImageDisplayCtrl
#define BLINK_RATE 500

class ImageDisplayCtrl : public CStatic
{
	DECLARE_DYNAMIC(ImageDisplayCtrl)

public:
	ImageDisplayCtrl();
	virtual ~ImageDisplayCtrl();
	// Setting Image Functions
	bool SetImage(unsigned char* pImage, unsigned int nWidth, unsigned int bpp, unsigned int nHeight);
	// Saves current image as a bitmap.
	bool SaveCurrentImage(const WCHAR* pFullSavePath);
	void SetImagePosition(unsigned int nStatus);
private:
	void			DrawArrows(Gdiplus::Graphics& graphics);
	Bitmap*			m_pBitmap;
	unsigned char*  m_pImage;		// Ptr to Image
	unsigned int	m_nWidth;
	unsigned int	m_nHeight;
	unsigned int	m_nStartTime;
	bool			m_bShowArrows;
	unsigned int    m_nBlinkRate;
	unsigned int	m_nFingerPosition;
	// GDIPLUS
	GdiplusStartupInput gdiplusStartupInput;
	ULONG_PTR gdiplusToken;
	// Helper functions
	// Gets BMP encoder for saving
	int GetEncoderClsid(const WCHAR* format, CLSID* pClsid);
protected:
	DECLARE_MESSAGE_MAP()
public:
	afx_msg void OnPaint();
};


class CMemDC : public CDC 
{
private:	
	CBitmap		m_bitmap;		// Offscreen bitmap
	CBitmap*	m_oldBitmap;	// bitmap originally found in CMemDC
	CDC*		m_pDC;			// Saves CDC passed in constructor
	CRect		m_rect;			// Rectangle of drawing area.
	BOOL		m_bMemDC;		// TRUE if CDC really is a Memory DC.
public:
	
	CMemDC(CDC* pDC, const CRect* pRect = NULL) : CDC()
	{
		ASSERT(pDC != NULL); 

		// Some initialization
		m_pDC = pDC;
		m_oldBitmap = NULL;
		m_bMemDC = !pDC->IsPrinting();

		// Get the rectangle to draw
		if (pRect == NULL) {
			pDC->GetClipBox(&m_rect);
		} else {
			m_rect = *pRect;
		}

		if (m_bMemDC) {
			// Create a Memory DC
			CreateCompatibleDC(pDC);
			pDC->LPtoDP(&m_rect);

			m_bitmap.CreateCompatibleBitmap(pDC, m_rect.Width(), m_rect.Height());
			m_oldBitmap = SelectObject(&m_bitmap);

			SetMapMode(pDC->GetMapMode());

			SetWindowExt(pDC->GetWindowExt());
			SetViewportExt(pDC->GetViewportExt());

			pDC->DPtoLP(&m_rect);
			SetWindowOrg(m_rect.left, m_rect.top);
		} else {
			// Make a copy of the relevent parts of the current DC for printing
			m_bPrinting = pDC->m_bPrinting;
			m_hDC       = pDC->m_hDC;
			m_hAttribDC = pDC->m_hAttribDC;
		}

		// Fill background 
		FillSolidRect(m_rect, pDC->GetBkColor());
	}
	
	~CMemDC()	
	{		
		if (m_bMemDC) {
			// Copy the offscreen bitmap onto the screen.
			m_pDC->BitBlt(m_rect.left, m_rect.top, m_rect.Width(), m_rect.Height(),
				this, m_rect.left, m_rect.top, SRCCOPY);			
			
			//Swap back the original bitmap.
			SelectObject(m_oldBitmap);		
		} else {
			// All we need to do is replace the DC with an illegal value,
			// this keeps us from accidently deleting the handles associated with
			// the CDC that was passed to the constructor.			
			m_hDC = m_hAttribDC = NULL;
		}	
	}
	
	// Allow usage as a pointer	
	CMemDC* operator->() 
	{
		return this;
	}	

	// Allow usage as a pointer	
	operator CMemDC*() 
	{
		return this;
	}
};

