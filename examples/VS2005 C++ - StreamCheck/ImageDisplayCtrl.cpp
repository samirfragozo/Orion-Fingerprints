
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


#include "stdafx.h"
#include "StreamCheck.h"
#include "ImageDisplayCtrl.h"
#include "gdiplus.h"
#include "math.h"

// ImageDisplayCtrl

IMPLEMENT_DYNAMIC(ImageDisplayCtrl, CStatic)

#define ACQ_DONE					0
#define ACQ_PROCESSING				1
#define ACQ_BUSY					2
#define ACQ_TIMEOUT					3
#define ACQ_NO_FINGER_PRESENT		4
#define ACQ_MOVE_FINGER_UP			5
#define ACQ_MOVE_FINGER_DOWN		6
#define ACQ_MOVE_FINGER_LEFT		7
#define ACQ_MOVE_FINGER_RIGHT		8
#define ACQ_FINGER_POSITION_OK		9
#define ACQ_TIMEOUT_LATENT			11
#define ACQ_NONE					100

#define ARROW_LENGTH	10
#define	ARROW_CAP_SIZE	3


ImageDisplayCtrl::ImageDisplayCtrl()
{
	m_pImage = NULL;
	m_pBitmap = NULL;
	m_nWidth = 0;
	m_nHeight = 0;
	m_nStartTime = 0;
	GdiplusStartup(&gdiplusToken, &gdiplusStartupInput, NULL);
	m_nBlinkRate = BLINK_RATE;
	m_nFingerPosition = ACQ_NONE;
}

ImageDisplayCtrl::~ImageDisplayCtrl()
{
	if(m_pImage)
	{
		delete [] m_pImage;
		m_pImage = NULL;
	}
}


BEGIN_MESSAGE_MAP(ImageDisplayCtrl, CStatic)
	ON_WM_PAINT()
END_MESSAGE_MAP()



// ImageDisplayCtrl message handlers



void ImageDisplayCtrl::OnPaint()
{
	CPaintDC dc(this); // device context for painting
	static int ID = 0;
	// Grab the Image Rect
	CRect dimsOfThisWindow;
	// If an Image is set...
	if(m_pBitmap)
	{
		LONG nDisplayWidth;
		LONG nDisplayHeight;
		UINT nHeight = m_pBitmap->GetHeight();
		UINT nWidth = m_pBitmap->GetWidth();
		GetClientRect(&dimsOfThisWindow);
	
		// calc aspect ratio of bitmap
		float fRatioBitmap = (float) nWidth / (float) nHeight;

		// calc aspect ratio of window
		float fRatioWindow = (float) dimsOfThisWindow.right / (float) dimsOfThisWindow.bottom;

		// calc change in aspect ratio 
		float fChange = (fRatioBitmap - fRatioWindow) / fRatioBitmap;

		if(fabs(fChange) > .07) // if the difference in aspect ratio is greater than 7%, we'll fix it.
		{

			if(fRatioBitmap < 1)
			{
				nDisplayWidth = dimsOfThisWindow.right;
				nDisplayHeight = (LONG)(dimsOfThisWindow.right / fRatioBitmap);
			}
			else
			{
				// Currently don't have a sensor that falls into this category, so we'll do nothing.
			}
		}
		else
		{
			nDisplayWidth = dimsOfThisWindow.right;
			nDisplayHeight = dimsOfThisWindow.bottom;
		}

		CMemDC memDC(&dc, &dimsOfThisWindow);
		Gdiplus::Graphics graphics(memDC);
		graphics.DrawImage(m_pBitmap,0,0,nDisplayWidth,nDisplayHeight);
		DrawArrows(graphics);
	}
}
void ImageDisplayCtrl::SetImagePosition(unsigned int nStatus)
{
	m_nFingerPosition = nStatus;
}
void ImageDisplayCtrl::DrawArrows(Gdiplus::Graphics& graphics)
{
	if(m_nStartTime == 0)
	{
		m_nStartTime = GetTickCount();
	}
	if( (GetTickCount() - m_nStartTime) >= m_nBlinkRate)
	{
		m_nStartTime = 0;
		if(m_bShowArrows) 
		{
			m_bShowArrows = false;
			m_nBlinkRate  = BLINK_RATE/3;
		}
		else 
		{
			m_bShowArrows = true;
			m_nBlinkRate  = BLINK_RATE;
		}
	} 
	
	if(m_bShowArrows || (m_nFingerPosition == ACQ_FINGER_POSITION_OK) )
	{
		CustomLineCap *p = new AdjustableArrowCap(3,3,true);
		Pen pen(Color::Red, 10);
		Pen redCircle(Color::Red,11);
		Pen greenCircle(Color::Green,11);

		pen.SetCustomEndCap(p);
		// Draw Appropriate Graphic
		switch(m_nFingerPosition)
		{
			case ACQ_DONE:
			{
				graphics.DrawEllipse(&greenCircle, 20,20,50,50);
			} break;
			case ACQ_PROCESSING:				
			{
				graphics.DrawEllipse(&greenCircle, 20,20,50,50);
			} break;
			case ACQ_BUSY:					
			{
				graphics.DrawEllipse(&greenCircle, 20,20,50,50);
			} break;
			case ACQ_TIMEOUT_LATENT:
			case ACQ_TIMEOUT:				
			{
				
			} break;
			case ACQ_NO_FINGER_PRESENT:		
			{
				graphics.DrawEllipse(&redCircle, 20,20,50,50);
			} break;
			case ACQ_MOVE_FINGER_UP:			
			{
				graphics.DrawLine(&pen, 20,70,20,20);
			} break;
			case ACQ_MOVE_FINGER_DOWN:		
			{
				graphics.DrawLine(&pen, 20,20,20,70);
			} break;
			case ACQ_MOVE_FINGER_LEFT:		
			{
				graphics.DrawLine(&pen, 70,20,20,20);
			} break;
			case ACQ_MOVE_FINGER_RIGHT:		
			{
				graphics.DrawLine(&pen, 20,20,70,20);
			} break;
			case ACQ_FINGER_POSITION_OK:
			{
				graphics.DrawEllipse(&greenCircle, 20,20,50,50);
			} break;
		}
		delete p;
	}
}
bool ImageDisplayCtrl::SetImage(unsigned char* pImage, unsigned int nWidth, unsigned int bpp, unsigned int nHeight)
{
	unsigned char* pTmpSrc = NULL;
	unsigned char* pTmpDst = NULL;

	if( (m_nWidth > 1000) || (m_nWidth < 0) || (m_nHeight < 0) || (m_nHeight > 1000) )
	{
		return false;
	}

	m_nWidth = nWidth;
	m_nHeight = nHeight;

	if(m_pImage)
	{
		delete [] m_pImage;
		m_pImage = NULL;
	}	
	if(m_pBitmap)
	{
		delete m_pBitmap;
		m_pBitmap = NULL;
	}
	// Convert to RGB from I
	m_pImage = new unsigned char[nWidth*nHeight*3];
	if(m_pImage == NULL) return false;
	switch(bpp)
	{
	case 8:
		{
			
			pTmpSrc = pImage;
			pTmpDst = m_pImage;
			//
			for(unsigned int ii = 0 ; ii < nWidth*nHeight ; ii++)
			{
				*pTmpDst++ = *pTmpSrc;
				*pTmpDst++ = *pTmpSrc;
				*pTmpDst++ = *pTmpSrc++;
			}
		} break;
	case 24:
		{
			memcpy(m_pImage, pImage, nWidth*nHeight*3);
		} break;
	default:
		{

		} break;
	}


	m_pBitmap = new Bitmap(m_nWidth,m_nHeight,m_nWidth*3,PixelFormat24bppRGB,(BYTE*)m_pImage);
	Invalidate();
	return true;
}



bool ImageDisplayCtrl::SaveCurrentImage(const WCHAR* pFullSavePath)
{
	CLSID bmpClsid;
	GetEncoderClsid(L"image/bmp", &bmpClsid);
	m_pBitmap->Save(pFullSavePath, &bmpClsid);
	return true;
}

int ImageDisplayCtrl::GetEncoderClsid(const WCHAR* format, CLSID* pClsid)
{
   UINT  num = 0;          // number of image encoders
   UINT  size = 0;         // size of the image encoder array in bytes

   ImageCodecInfo* pImageCodecInfo = NULL;

   GetImageEncodersSize(&num, &size);
   if(size == 0)
      return -1;  // Failure

   pImageCodecInfo = (ImageCodecInfo*)(malloc(size));
   if(pImageCodecInfo == NULL)
      return -1;  // Failure

   GetImageEncoders(num, size, pImageCodecInfo);

   for(UINT j = 0; j < num; ++j)
   {
      if( wcscmp(pImageCodecInfo[j].MimeType, format) == 0 )
      {
         *pClsid = pImageCodecInfo[j].Clsid;
         free(pImageCodecInfo);
         return j;  // Success
      }    
   }

   free(pImageCodecInfo);
   return -1;  // Failure
}