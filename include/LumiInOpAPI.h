
/******************************<%BEGIN LICENSE%>******************************/
// (c) Copyright 2011 Lumidigm, Inc. (Unpublished Copyright) ALL RIGHTS RESERVED.
//
// For a list of applicable patents and patents pending, visit www.lumidigm.com/patents/
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS 
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER 
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN 
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
//
/******************************<%END LICENSE%>******************************/

#ifndef DOXYGEN_SHOULD_SKIP_THIS	// Skip this for auto-docs.
#pragma once

#include "LumiTypes.h"
#include "FIRTypes.h"

/*** Initialization  ***/
#if __cplusplus
#define _C_ "C"
#else
#define _C_ 
#endif

#ifdef LUMIINOP_EXPORTS
#define LUMI_INOP_EXPORT extern _C_ __declspec(dllexport)
#else
#define LUMI_INOP_EXPORT extern _C_
#endif

#define STDCALL __stdcall

#define	KEEP_ALL				0
#define	REMOVE_SPOOF_TEMPLATE	1
#define	REMOVE_ALL				2

typedef void* FIRECORD_HANDLE;


#endif
/*********************** LumiGetSpoofTemplateSize ***********************/
///////////////////////////////////////////////////////////////////////////////
///  global public  LumiGetTemplateVersion
///  Returns the Spoof template size
///
///  @param [in]       pSpoofTemplate uchar const*	 pointer to Spoof template
///  @param [in, out]  nSpoofTemplateSize int&		 Spoof template size
///
///  @return LumiStatus Refer to Error code documentation for detailed description of 
///						possible return values. LUMI_STATUS_OK indicates operation was 
///					    successful
///
///  @remarks Currently not supported
///
///
///  @author www.lumidigm.com @date 8/04/2008
///////////////////////////////////////////////////////////////////////////////

LUMI_INOP_EXPORT LumiStatus STDCALL LumiGetSpoofTemplateSize(uchar* pSpoofTemplate, 
															int& nSpoofTemplateSize);

/*********************** LumiGetSpoofTemplateVersion ***********************/
///////////////////////////////////////////////////////////////////////////////
///  global public  LumiGetSpoofTemplateVersion
///  Returns the Spoof template version
///
///  @param [in]       pSpoofTemplate uchar const*	 pointer to Spoof template
///  @param [in]	   nSpoofTemplateSize uint		 Spoof templates size in bytes	
///  @param [in, out]  nSpoofTemplateVer int&		 Spoof template version
///
///  @return LumiStatus Refer to Error code documentation for detailed description of 
///						possible return values. LUMI_STATUS_OK indicates operation was 
///					    successful
///
///  @remarks Currently not supported
///
///
///  @author www.lumidigm.com @date 8/04/2008
///////////////////////////////////////////////////////////////////////////////

LUMI_INOP_EXPORT LumiStatus STDCALL LumiGetSpoofTemplateVersion(uchar const* pSpoofTemplate, 
																const uint nSpoofTemplateSize, 
																int& nSpoofTemplateVer);

/*********************** LumiGetSpoofTemplateFromANSI378 ***********************/
///////////////////////////////////////////////////////////////////////////////
///  global public  LumiGetSpoofTemplateFromANSI378
///  Returns the Spoof template from an ANSI378 template which has Spoof template embedded in it
///
///  @param [in]       pANSI378Template uchar const*    pointer to ANSI378 Template with embedded Spoof template
///  @param [in, out]  pSpoofTemplate uchar *			Preallocated buffer which will contain output Spoof template
///
///  @return LumiStatus Refer to Error code documentation for detailed description of 
///						possible return values. LUMI_STATUS_OK indicates operation was 
///					    successful
///
///  @remarks Currently not supported
///
///  @see LumiGetSpoofTemplateFromImage
///
///  @author www.lumidigm.com @date 8/04/2008
///////////////////////////////////////////////////////////////////////////////

LUMI_INOP_EXPORT LumiStatus STDCALL LumiGetSpoofTemplateFromANSI378(uchar const* pANSI378Template, 
																	uchar* pSpoofTemplate);

/*********************** LumiGetSpoofTemplateFromImage ***********************/
///////////////////////////////////////////////////////////////////////////////
///  global public  LumiGetSpoofTemplateFromImage
///  Returns the Spoof template from an image which has Spoof template embedded in it
///
///  @param [in]       pImage uchar *			pointer to image with embedded Spoof template	
///  @param [in]	   nWidth uint				Input image width (number of pixels per row)	
///  @param [in]       nHeight uint				Input image height (number of pixels per column)
///  @param [in, out]  pSpoofTemplate uchar *	Preallocated buffer which will contain output Spoof template
///
///  @return LumiStatus Refer to Error code documentation for detailed description of 
///						possible return values. LUMI_STATUS_OK indicates operation was 
///					    successful
///
///  @remarks Currently not supported
///
///  @see LumiGetSpoofTemplateFromANSI378
///
///  @author www.lumidigm.com @date 8/04/2008
///////////////////////////////////////////////////////////////////////////////

LUMI_INOP_EXPORT LumiStatus STDCALL LumiGetSpoofTemplateFromImage(uchar const* pImage, 
																  uint nWidth, 
																  uint nHeight, 
																  uchar* pSpoofTemplate);

/*********************** LumiMatchSpoof ***********************/
///////////////////////////////////////////////////////////////////////////////
///  global public  LumiMatchSpoof
///  Returns Match score between two Spoof templates
///
///  @param [in]	   pSpoofTemplate1 uchar *	 pointer to Spoof template 1 
///  @param [in]	   pSpoofTemplate2 uchar *	 pointer to Spoof template 2
///  @param [in]	   nSpoofTemplateSize uint	 Spoof templates size in bytes	
///  @param [in, out]  nScore int &				 Spoof match score.  Value greater than -1
///												 if spoof supported and operation successful 		
///
///  @return LumiStatus Refer to Error code documentation for detailed description of 
///						possible return values. LUMI_STATUS_OK indicates operation was 
///					    successful
///
///  @remarks Currently not supported
///
///  @see LumiDefensiveMatchSpoof
///
///  @author www.lumidigm.com @date 8/04/2008
///////////////////////////////////////////////////////////////////////////////

LUMI_INOP_EXPORT LumiStatus STDCALL LumiMatchSpoof(uchar const* pSpoofTemplate1, 
												   uchar const* pSpoofTemplate2, 
												   const uint nSpoofTemplateSize, 
												   int& nScore);

/*********************** LumiDefensiveMatchSpoof ***********************/
///////////////////////////////////////////////////////////////////////////////
///  global public  LumiDefensiveMatchSpoof
///  Returns Spoof score of input Spoof template
///
///  @param [in]	   pSpoofTemplate uchar *	 pointer to Spoof template
///  @param [in]	   nSpoofTemplateSize uint	 Spoof template size in bytes	
///  @param [in, out]  nScore int &				 Spoof score . Value greater than -1
///												 if spoof supported and operation successful 
///
///  @return LumiStatus Refer to Error code documentation for detailed description of 
///						possible return values. LUMI_STATUS_OK indicates operation was 
///					    successful
///
///  @remarks Currently not supported
///
///  @see LumiMatchSpoof
///
///  @author www.lumidigm.com @date 8/04/2008
///////////////////////////////////////////////////////////////////////////////

LUMI_INOP_EXPORT LumiStatus STDCALL LumiDefensiveMatchSpoof(uchar const* pSpoofTemplate, 
															const uint nSpoofTemplateSize, 
															int& nScore);

/*********************** LumiWSQEncode ***********************/
///////////////////////////////////////////////////////////////////////////////
///  global public  LumiWSQEncode
///  Encodes input image using WSQ compression algorithm
///
///  @param [in]		 pImage uchar *						pointer to input image
///  @param [in]		 nWidth uint						Input image width (number of pixels per row)	
///  @param [in]		 nHeight uint						Input image height (number of pixels per column)
///  @param [in]		 nBPP uint							Input image pixel depth (number of bits per pixel; only 8BPP supported)
///  @param [in]		 nDPI uint							Input image DPI. If DPI is unknown (pass in 0), 
///															then default density of 500 DPI is used.
///  @param [in]		 nCompressionRatio float			Requested compression ratio. Compression ratio should be > 1 and < 20
///  @param [in, out]	 pWSQImage uchar **					Pointer to a pointer that will hold output WSQ image
///  @param [in, out]	 nWSQImageSize uint &				Size of the output WSQ image in bytes
///
///  @return LumiStatus Refer to Error code documentation for detailed description of 
///						possible return values. LUMI_STATUS_OK indicates operation was 
///					    successful
///
///  @remarks Client must call LumiRelease to release pWSQImage when it is no longer needed.
///
///  @see LumiWSQDecode
///
///  @author www.lumidigm.com @date 8/04/2008
///////////////////////////////////////////////////////////////////////////////

LUMI_INOP_EXPORT LumiStatus STDCALL LumiWSQEncode(uchar const* pImage, 
												  uint nWidth,
												  uint nHeight, 
												  uint nBPP,
												  uint nDPI, 
												  float nCompressionRatio, 
												  uchar** pWSQImage, 
												  uint& nWSQImageSize);

/*********************** LumiWSQDecode ***********************/
///////////////////////////////////////////////////////////////////////////////
///  global public  LumiWSQDecode
///  Decodes input WSQ encoded image and returns image information
///
///  @param [in]			 pWSQImage uchar *					pointer to input WSQ image
///  @param [in]			 nWSQImageSize uint 				Size of the WSQ image in bytes
///  @param [in, out]		 pImage uchar **					Pointer to a pointer that will hold output decompressed image
///  @param [in, out]		 nWidth uint &						Width of decompressed image (number of pixels per row)
///  @param [in, out]		 nHeight uint &						Height of decompressed image (number of pixels per column)
///  @param [in, out]		 nBPP uint &						Pixel depth of decompressed  image (number of bits per pixel)
///  @param [in, out]		 nDPI uint &						DPI of decompressed image
///  @param [in, out]		 nlossyflag int &					Image lossiness info of decompressed image

///
///  @return LumiStatus Refer to Error code documentation for detailed description of 
///						possible return values. LUMI_STATUS_OK indicates operation was 
///					    successful
///
///  @remarks Client must call LumiRelease to release pImage when it is no longer needed.
///
///  @see LumiWSQEncode
///
///  @author www.lumidigm.com @date 8/04/2008
///////////////////////////////////////////////////////////////////////////////

LUMI_INOP_EXPORT LumiStatus STDCALL LumiWSQDecode(uchar const* pWSQImage,
												  uint nWSQImageSize,
												  uchar** pImage,
												  uint& nWidth,
												  uint& nHeight,
												  uint& nBPP,
												  uint& nDPI,
												  int& nlossyflag);

/*********************** LumiComputeNFIQFromImage ***********************/
///////////////////////////////////////////////////////////////////////////////
///  global public  LumiComputeNFIQFromImage
///  Computes NFIQ given a finger print image.
///
///  @param [in]		 pImage uchar *			pointer to Input image
///  @param [in]		 nWidth uint			Input image width (number of pixels per row)	
///  @param [in]		 nHeight uint			Input image height (number of pixels per column)
///  @param [in]		 nBPP uint				Input image pixel depth (number of bits per pixel; only 8BPP supported)
///  @param [in]		 nDPI uint				Input image DPI. If DPI is unknown (pass in 0),
///												then default density of 500 DPI is used
///  @param [in, out]	 nNFIQ int &			Computed NFIQ value
///
///  @return LumiStatus Refer to Error code documentation for detailed description of 
///						possible return values. LUMI_STATUS_OK indicates operation was 
///					    successful
///
///  @remarks 
///
///  @see LumiComputeNFIQFromWSQImage
///
///  @author www.lumidigm.com @date 8/04/2008
///////////////////////////////////////////////////////////////////////////////

LUMI_INOP_EXPORT LumiStatus STDCALL LumiComputeNFIQFromImage(uchar const* pImage,
															 uint nWidth,
															 uint nHeight,
															 uint nBPP,
															 uint nDPI,
															 int& nNFIQ);

/*********************** LumiComputeNFIQFromWSQImage ***********************/
///////////////////////////////////////////////////////////////////////////////
///  global public  LumiComputeNFIQFromWSQImage
///  Computes NFIQ given a WSQ encoded image.
///
///  @param [in]		 pWSQImage uchar *			pointer to WSQ image
///  @param [in]		 nWSQImageSize uint			Size of the WSQ image in bytes
///  @param [in, out]	 nNFIQ int &				Computed NFIQ value
///
///  @return LumiStatus Refer to Error code documentation for detailed description of 
///						possible return values. LUMI_STATUS_OK indicates operation was 
///					    successful
///
///  @remarks 
///
///  @see LumiComputeNFIQFromImage, LumiWSQEncode, LumiWSQDecode
///
///  @author www.lumidigm.com @date 8/04/2008
///////////////////////////////////////////////////////////////////////////////

LUMI_INOP_EXPORT LumiStatus STDCALL LumiComputeNFIQFromWSQImage(uchar const* pWSQImage,
																uint nWSQImageSize,
																int& nNFIQ);

/*********************** LumiANSI378ToISO ***********************/
///////////////////////////////////////////////////////////////////////////////
///  global public  LumiANSI378ToISO
/// Converts ANSI378 template to ISO 19794-2 record format template
/// 
///  @param [in]		pANSI378Template	uchar *		pointer to input ANSI378 template
///  @param [in]		nANSI378TemplateSize uint		Size of ANSI378 template in bytes
///  @param [in, out]	pISOTemplate uchar **			Pointer to a pointer that will hold output ISO template
///  @param [in, out]	nISOTemplateSize uint &			Size of the output ISO template in bytes
///
///  @return LumiStatus Refer to Error code documentation for detailed description of 
///						possible return values. LUMI_STATUS_OK indicates operation was 
///					    successful
///
///  @remarks Client must call LumiRelease to release pISOTemplate when it is no longer needed.
///
///  @see LumiANSI378ToISONC LumiANSI378ToISOCC
///
///  @author www.lumidigm.com @date 8/04/2008
///////////////////////////////////////////////////////////////////////////////

LUMI_INOP_EXPORT LumiStatus STDCALL LumiANSI378ToISO(uchar const* pANSI378Template,
													 uint nANSI378TemplateSize,
													 uchar** pISOTemplate,
													 uint& nISOTemplateSize);

/*********************** LumiANSI378ToISONC ***********************/
///////////////////////////////////////////////////////////////////////////////
///  global public  LumiANSI378ToISONC
/// Converts ANSI378 template to Normal size ISO 19794-2 card format template
/// 
///  @param [in]		pANSI378Template	uchar *			pointer to input ANSI378 template
///  @param [in]		nANSI378TemplateSize uint			Size of input ANSI378 template in bytes
///  @param [in, out]	pISONCTemplate uchar **				Pointer to a pointer that will hold output ISONC template
///  @param [in, out]	nISONCTemplateSize uint &			Size of the output ISONC template in bytes
///
///  @return LumiStatus Refer to Error code documentation for detailed description of 
///						possible return values. LUMI_STATUS_OK indicates operation was 
///					    successful
///
///  @remarks Client must call LumiRelease to release pISONCTemplate when it is no longer needed. 
///
///  @see LumiANSI378ToISO LumiANSI378ToISOCC
///
///  @author www.lumidigm.com @date 8/04/2008
///////////////////////////////////////////////////////////////////////////////

LUMI_INOP_EXPORT LumiStatus STDCALL LumiANSI378ToISONC(uchar const* pANSI378Template,
													   uint nANSI378TemplateSize,
													   uchar** pISONCTemplate,
													   uint& nISONCTemplateSize);

/*********************** LumiANSI378ToISOCC ***********************/
///////////////////////////////////////////////////////////////////////////////
///  global public  LumiANSI378ToISOCC
/// Converts ANSI378 template to Compact size ISO 19794-2 card format template
/// 
///  @param [in]		pANSI378Template	uchar *			pointer to input ANSI378 template
///  @param [in]		nANSI378TemplateSize uint			Size of input ANSI378 template
///  @param [in, out]	pISOCCTemplate uchar **				Pointer to a pointer that will hold output ISOCC template
///  @param [in, out]	nISOCCTemplateSize uint &			Size of the Output ISOCC template in bytes
///
///  @return LumiStatus Refer to Error code documentation for detailed description of 
///						possible return values. LUMI_STATUS_OK indicates operation was 
///					    successful
///
///  @remarks Client must call LumiRelease to release pISOCCTemplate when it is no longer needed.
///
///  @see LumiANSI378ToISO LumiANSI378ToISONC
///
///  @author www.lumidigm.com @date 8/04/2008
///////////////////////////////////////////////////////////////////////////////

LUMI_INOP_EXPORT LumiStatus STDCALL LumiANSI378ToISOCC(uchar const* pANSI378Template,
													   uint nANSI378TemplateSize,
													   uchar** pISOCCTemplate,
													   uint& nISOCCTemplateSize);

///////////////////////////////////////////////////////////////////////////////
///  global public  LumiANSI378ToProp
/// Converts ANSI378 template to Proprietary template
///
///  @param [in]       pANSI378Template const uchar *    pointer to input ANSI378 template
///  @param [in]       nANSI378TemplateSize uint		 Size of input ANSI378 template
///  @param [in, out]  pPropTemplate uchar **			 Pointer to a pointer that will hold output Proprietary template
///  @param [in, out]  nPropTemplateSize uint &			 Size of the Output Proprietary template in bytes
///
///  @return LumiStatus Refer to Error code documentation for detailed description of 
///						possible return values. LUMI_STATUS_OK indicates operation was 
///					    successful
///
///  @remarks Client must call LumiRelease to release pPropTemplate when it is no longer needed.
///			  Input ANSI378 template should be created by Lumidigm SDK 2.40 or later
///
///  @see 
///
///  @author www.lumidigm.com @date 8/4/2008
///////////////////////////////////////////////////////////////////////////////

LUMI_INOP_EXPORT LumiStatus STDCALL LumiANSI378ToProp(uchar const* pANSI378Template,
													  uint nANSI378TemplateSize,
													  uchar** pPropTemplate,
													  uint& nPropTemplateSize);

/*********************** LumiISOToANSI378 ***********************/
///////////////////////////////////////////////////////////////////////////////
///  global public  LumiISOToANSI378
///  Converts ISO 19794-2 record format template to ANSI378 template
/// 
///  @param [in]		pISOTemplate	uchar *			pointer to input ISO template
///  @param [in]		nISOTemplateSize uint			Size of input ISO template in bytes
///  @param [in, out]	pANSI378Template uchar **		Pointer to a pointer that will hold output ANSI378 template
///  @param [in, out]	nANSI378TemplateSize uint &		Size of the output ANSI378 template in bytes
///
///  @return LumiStatus Refer to Error code documentation for detailed description of 
///						possible return values. LUMI_STATUS_OK indicates operation was 
///					    successful
///
///  @remarks Client must call LumiRelease to release pANSI378Template when it is no longer needed.
///
///  @see LumiISONCToANSI378 LumiISOCCToANSI378
///
///  @author www.lumidigm.com @date 8/04/2008
///////////////////////////////////////////////////////////////////////////////

LUMI_INOP_EXPORT LumiStatus STDCALL LumiISOToANSI378(uchar const* pISOTemplate,
													 uint nISOTemplateSize,
													 uchar** pANSI378Template,
													 uint& nANSI378TemplateSize);

/*********************** LumiISONCToANSI378 ***********************/
///////////////////////////////////////////////////////////////////////////////
///  global public  LumiISONCToANSI378
///  Converts Normal size ISO 19794-2 card format template to ANSI378 template
/// 
///  @param [in]		pISONCTemplate	uchar *			pointer to input ISONC template
///  @param [in]		nISONCTemplateSize uint			Size of input ISONC Template
///  @param [in]		nISONCXres unsigned short		X resolution of input ISONC template in pixels/cm.
///														if unknown (pass in 0), then default resolution of 197 
///														pixels/cm(500 DPI) is used.
///  @param [in]		nISONCYres unsigned short		Y resolution of input ISONC template in pixels/cm
///														if unknown (pass in 0), then default resolution of 197 
///														pixels/cm(500 DPI) is used.
///  @param [in, out]	pANSI378Template uchar **		Pointer to a pointer that will hold output ANSI378 template
///  @param [in, out]	nANSI378TemplateSize uint &		Size of the output ANSI378 template in bytes
///
///  @return LumiStatus Refer to Error code documentation for detailed description of 
///						possible return values. LUMI_STATUS_OK indicates operation was 
///					    successful
///
///  @remarks Client must call LumiRelease to release pANSI378Template when it is no longer needed.
///
///  @see LumiISOToANSI378 LumiISOCCToANSI378
///
///  @author www.lumidigm.com @date 8/04/2008
///////////////////////////////////////////////////////////////////////////////

LUMI_INOP_EXPORT LumiStatus STDCALL LumiISONCToANSI378(uchar const* pISONCTemplate,
													   uint nISONCTemplateSize,
													   uint nISONCXres,
													   uint nISONCYres,
													   uchar** pANSI378Template,
													   uint& nANSI378TemplateSize);

/*********************** LumiISOCCToANSI378 ***********************/
///////////////////////////////////////////////////////////////////////////////
///  global public  LumiISOCCToANSI378
///  Converts Compact size ISO 19794-2 card format template to ANSI378 template
/// 
///  @param [in]		pISOCCTemplate	uchar *			pointer to input ISOCC template
///  @param [in]		nISOCCTemplateSize uint			Size of input ISOCC Template in bytes
///  @param [in]		nISOCCXres unsigned short		X resolution of input ISOCC template in pixels/cm.
///														if unknown (pass in 0), then default resolution of 197 
///														pixels/cm (500 DPI) is used.
///  @param [in]		nISOCCYres unsigned short		Y resolution of input ISOCC template in pixels/cm
///														if unknown (pass in 0), then default resolution of 197 
///														pixels/cm (500 DPI) is used.
///  @param [in, out]	pANSI378Template uchar **		Pointer to a pointer that will hold output ANSI378 template
///  @param [in, out]	nANSI378TemplateSize uint &		Size of the output ANSI378 template in bytes
///
///  @return LumiStatus Refer to Error code documentation for detailed description of 
///						possible return values. LUMI_STATUS_OK indicates operation was 
///					    successful
///
///  @remarks Client must call LumiRelease to release pANSI378Template when it is no longer needed.
///
///  @see LumiISOToANSI378 LumiISONCToANSI378
///
///  @author www.lumidigm.com @date 8/04/2008
///////////////////////////////////////////////////////////////////////////////

LUMI_INOP_EXPORT LumiStatus STDCALL LumiISOCCToANSI378(uchar const* pISOCCTemplate,
													   uint nISOCCTemplateSize,
													   uint nISOCCXres,
													   uint nISOCCYres,
													   uchar** pANSI378Template,
													   uint& nANSI378TemplateSize);

///////////////////////////////////////////////////////////////////////////////
///  global public  LumiPropToANSI378
///  Converts Proprietary template to ANSI378 template
///
///  @param [in]       pPropTemplate const uchar *    pointer to input Proprietary template
///  @param [in]       nPropTemplateSize uint		  Size of input Proprietary Template in bytes
///  @param [in, out]  pANSI378Template uchar **      Pointer to a pointer that will hold output ANSI378 template
///  @param [in, out]  nANSI378TemplateSize uint &    Size of the output ANSI378 template in bytes
///
///  @return LumiStatus Refer to Error code documentation for detailed description of 
///						possible return values. LUMI_STATUS_OK indicates operation was 
///					    successful
///
///  @remarks Client must call LumiRelease to release pANSI378Template when it is no longer needed.
///  @see 
///
///  @author www.lumidigm.com @date 8/4/2008
///////////////////////////////////////////////////////////////////////////////

LUMI_INOP_EXPORT LumiStatus STDCALL LumiPropToANSI378(uchar const* pPropTemplate,
													  uint nPropTemplateSize,
													  uchar** pANSI378Template,
													  uint& nANSI378TemplateSize);
/*********************** LumiLoadBitmapFromFile ***********************/
///////////////////////////////////////////////////////////////////////////////
///  global public  LumiLoadBitmapFromFile
///  Loads image from 8/24-Bit Bitmap file. Supports only bitmaps containing grayscale images with no compression.
/// 
///  @param [in]		pFileName char *	pointer to file name 
///  @param [in, out]	pImage  char**		pointer to pointer that will hold output image
///  @param [in, out]	nWidth  uint &		Width of output image (number of pixels per row)
///  @param [in, out]	nHeight  uint &		Height of output image (number of pixels per column)
///	 @param [in, out]	nBPP  uint &		Pixel depth of output image (number of bits per pixel)
///											Always returns output image in 8BPP format.
///
///  @return LumiStatus Refer to Error code documentation for detailed description of 
///						possible return values. LUMI_STATUS_OK indicates operation was 
///					    successful
///
///  @remarks Client must call LumiRelease to release pImage when it is no longer needed.
///
///  @see LumiSaveAsBitmapToFile
///
///  @author www.lumidigm.com @date 8/04/2008
///////////////////////////////////////////////////////////////////////////////

LUMI_INOP_EXPORT LumiStatus STDCALL LumiLoadBitmapFromFile(char const* pFileName,
														   uchar** pImage, 
														   uint& nWidth, 
														   uint& nHeight,
														   uint& nBPP);

/*********************** LumiSaveAsBitmapToFile ***********************/
///////////////////////////////////////////////////////////////////////////////
///  global public  LumiSaveAsBitmapToFile
///  Saves image to file in 8-Bit Bitmap format
/// 
///  @param [in, out]	pImage uchar *		pointer to input image
///  @param [in]		nWidth uint 		Input image width (number of pixels per row)
///  @param [in]		nHeight uint 		Input image height (number of pixels per column)
///  @param [in]		nBPP uint 			Input image pixel depth (number of bits per pixel; only 8BPP supported)
///  @param [in]		pFileName char *	pointer to the file name
///
///  @return LumiStatus Refer to Error code documentation for detailed description of 
///						possible return values. LUMI_STATUS_OK indicates operation was 
///					    successful
///
///  @remarks 
///
///  @see LumiLoadBitmapFromFile
///
///  @author www.lumidigm.com @date 8/04/2008
///////////////////////////////////////////////////////////////////////////////

LUMI_INOP_EXPORT LumiStatus STDCALL LumiSaveAsBitmapToFile(uchar const* pImage,
														   uint nWidth, 
														   uint nHeight,
														   uint nBPP,
														   char const* pFileName);

/*********************** LumiTruncateANSI378Template ***********************/
///////////////////////////////////////////////////////////////////////////////
///  global public  LumiTruncateANSI378Template
///  Truncates an ANSI378 template to a requested size 
/// 
///  @param [in, out]	pANSI378Template uchar *				pointer to input ANSI378 template
///  @param [in, out]	pTruncatedTemplate uchar *				Preallocated buffer of nMaxTruncatedTemplateSize bytes  
///																which will contain output Truncated template.
///  @param [in, out]	nMaxTruncatedTemplateSize uint &		Requested maximum size of the truncated template as input. 
///																Actual size of the Truncated template (can be smaller than  
///																the requested maximum size) is returned as output.
///  @param [in]		nTruncateflag int						KEEP_ALL removes only minutiae
///																REMOVE_SPOOF_TEMPLATE removes spoof template and minutiae
///																REMOVE_ALL removes extended data, spoof template and minutiae
///
///  @return LumiStatus Refer to Error code documentation for detailed description of 
///						possible return values. LUMI_STATUS_OK indicates operation was 
///					    successful
///
///  @remarks Client needs to preallocate buffer of size nMaxTruncatedTemplateSize bytes for the Truncated template.
///			  Actual size of the Truncated template can be smaller than the requested size. 
///			  Below are the series of steps this function goes through:
///				1. If nTruncateflag is KEEP_ALL then input template size is calculated. If the input template size is less than or
///				   equal to the requested size then input template is copied to truncated template and returns as output  otherwise 
///				   goes to step 4. 
///				2. If nTruncateflag is REMOVE_SPOOF_TEMPLATE then spoof template is removed. Truncated template size is calculated.
///				   If calculated size is less than or equal to requested size then truncated template is returned. Otherwise goes to step 4.
///				3. If nTruncateflag is REMOVE_ALL then spoof template and extended data is removed.  Truncated template size is calculated.
///				   If calculated size is less than or equal to requested size then truncated template is returned. Otherwise goes to step 4.
///				4. Determines number of minutiae to be retained for requested size. If its less than required minimum number of minutiae (61) then
///				   LUMI_STATUS_INVALID_PARAMETER error code is returned. otherwise goes to step 5.
///				5. Removes the required number of minutiae and returns truncated template as output.
///
///  @see 
///
///  @author www.lumidigm.com @date 8/04/2008
///////////////////////////////////////////////////////////////////////////////

LUMI_INOP_EXPORT LumiStatus STDCALL LumiTruncateANSI378Template(uchar const* pANSI378Template, 
																uchar* pTruncatedTemplate, 
																uint& nMaxTruncatedTemplateSize, 
																int nTruncateflag);

/*********************** LumiRelease ***********************/
///////////////////////////////////////////////////////////////////////////////
///  global public  LumiRelease
///  Releases memory.
///
///  @param [in]		 pMemToRelease void *			pointer to memory allocation
///
///  @return LumiStatus Refer to Error code documentation for detailed description of 
///						possible return values. LUMI_STATUS_OK indicates operation was 
///					    successful
///
///  @remarks Client must call LumiRelease to deallocate memory buffer allocated by LumiInOpAPI when it is no longer needed.
///
///  @see 
///
///  @author www.lumidigm.com @date 8/04/2008
///////////////////////////////////////////////////////////////////////////////

LUMI_INOP_EXPORT LumiStatus STDCALL LumiRelease(void* pMemToRelease);

/*********************** LumiFIRecordCreate ***********************/
///////////////////////////////////////////////////////////////////////////////
///  global public  LumiFIRecordCreate
///  Creates an empty FIRecord
///
///	 @param [in]	   nBPP  uint								Pixel depth of the images in FIRecord (number of bits per pixel; only 8BPP supported)
///  @param [in]       CompressionType LUMI_COMPRESSION_TYPE    Compression type to be used for storing images
///  @param [in]       Standard LUMI_FIR_STANDARD				Standard to be used for FIRecord.
///  @param [in, out]  pHRecord FIRECORD_HANDLE *				pointer to FIRECORD_HANDLE that receives handle to created FIRecord
///
///  @return LumiStatus Refer to Error code documentation for detailed description of 
///						possible return values. LUMI_STATUS_OK indicates operation was 
///					    successful
///
///  @remarks See LUMI_COMPRESSION_TYPE for information on types of compressions.
///			  Currently following compression types are supported:
///				UNCOMPRESSED_NO_BIT_PACKING		
///				COMPRESSED_WSQ - Compression ratio of 5:1 is used	
///			  See LUMI_FIR_STANDARD for information on standards supported for FIRecord.
///
///  @see LumiFIRecordRelease
///
///  @author www.lumidigm.com @date 9/9/2008
///////////////////////////////////////////////////////////////////////////////

LUMI_INOP_EXPORT LumiStatus STDCALL LumiFIRecordCreate(uint nBPP,
													   LUMI_COMPRESSION_TYPE CompressionType,
													   LUMI_FIR_STANDARD Standard,
													   FIRECORD_HANDLE* pHRecord);

/*********************** LumiFIRecordRelease ***********************/
///////////////////////////////////////////////////////////////////////////////
///  global public  LumiFIRecordRelease
///  Deletes the FIRecord.
///
///  @param [in]       hRecord FIRECORD_HANDLE    Handle to FIRecord
///
///  @return LumiStatus Refer to Error code documentation for detailed description of 
///						possible return values. LUMI_STATUS_OK indicates operation was 
///					    successful
///
///  @remarks After the FIRecord is deleted specified handle is no longer valid
///
///  @see 
///
///  @author www.lumidigm.com @date 9/9/2008
///////////////////////////////////////////////////////////////////////////////

LUMI_INOP_EXPORT LumiStatus STDCALL	LumiFIRecordRelease(FIRECORD_HANDLE hRecord);

/*********************** LumiFIRecordAddView ***********************/
///////////////////////////////////////////////////////////////////////////////
///  global public  LumiFIRecordAddView
///  Adds a view to FIRecord for input image with specified finger/palm position
///
///  @param [in]       hRecord FIRECORD_HANDLE						Handle to FIRecord
///  @param [in, out]  pImage uchar *								pointer to Raw Input image
///  @param [in]       nWidth uint									Input image width (number of pixels per row)
///  @param [in]       nHeight uint									Input image height (number of pixels per column)
///  @param [in]       FingerPosition LUMI_FINGER_PALM_POSITION		Input image finger/palm position 
///  @param [in]       ImpressionType LUMI_IMPRESSION_TYPE			Input image Impression type 
///  @param [in, out]  nViewNumberOfFinger uint&					View number of the Finger position for input image in FIRecord returned as ouput
///
///  @return LumiStatus Refer to Error code documentation for detailed description of 
///						possible return values. LUMI_STATUS_OK indicates operation was 
///					    successful
///
///  @remarks See LUMI_FINGER_PALM_POSITION, LUMI_IMPRESSION_TYPE for information on Finger/palm positions and 
///			  impression types.
///
///  @see LumiFIRecordRemoveView
///
///  @author www.lumidigm.com @date 9/9/2008
///////////////////////////////////////////////////////////////////////////////

LUMI_INOP_EXPORT LumiStatus STDCALL LumiFIRecordAddView(FIRECORD_HANDLE hRecord,
													    uchar* pImage,
													    uint nWidth,
													    uint nHeight,
													    LUMI_FINGER_PALM_POSITION FingerPosition, 
													    LUMI_IMPRESSION_TYPE ImpressionType,
													    uint& nViewNumberOfFinger);

/*********************** LumiFIRecordRemoveView ***********************/
///////////////////////////////////////////////////////////////////////////////
///  global public  LumiFIRecordRemoveView
///  Removes a view with specified finger position and view number from FIRecord
///
///  @param [in]       hRecord FIRECORD_HANDLE						Handle to FIRecord
///  @param [in]       FingerPosition LUMI_FINGER_PALM_POSITION		Finger/palm position of the view to remove
///  @param [in]	   nViewNumberOfFinger uint						View number of the Finger position to remove
///
///  @return LumiStatus Refer to Error code documentation for detailed description of 
///						possible return values. LUMI_STATUS_OK indicates operation was 
///					    successful
///
///  @remarks See LUMI_FINGER_PALM_POSITION for information on Finger/palm positions. After  the
///			  view is removed view number of finger/palm position for existing views might change.
///
///  @see LumiFIRecordAddView
///
///  @author www.lumidigm.com @date 9/9/2008
///////////////////////////////////////////////////////////////////////////////

LUMI_INOP_EXPORT LumiStatus STDCALL LumiFIRecordRemoveView(FIRECORD_HANDLE hRecord,
														   LUMI_FINGER_PALM_POSITION FingerPosition,
														   uint nViewNumberOfFinger);

/*********************** LumiFIRecordSaveToFile ***********************/
///////////////////////////////////////////////////////////////////////////////
///  global public  LumiFIRecordSaveToFile
///  Saves FIRecord to file
///
///  @param [in]       hRecord FIRECORD_HANDLE    Handle to FIRecord
///  @param [in]       pFileName const char *     pointer to the file name 
///
///  @return LumiStatus Refer to Error code documentation for detailed description of 
///						possible return values. LUMI_STATUS_OK indicates operation was 
///					    successful
///
///  @remarks Images in FIRecord are saved with Compression type specified when issuing LumiFIRecordCreate call. 
///			  Compression type of the FIRecord can be found by issuing call to LumiFMRecordGetParameters.
///
///  @see LumiFIRecordLoadFromFile LumiFIRecordGetParameters
///
///  @author www.lumidigm.com @date 9/12/2008
///////////////////////////////////////////////////////////////////////////////

LUMI_INOP_EXPORT LumiStatus STDCALL LumiFIRecordSaveToFile(FIRECORD_HANDLE hRecord,
														   const char* pFileName);

/*********************** LumiFIRecordLoadFromFile ***********************/
///////////////////////////////////////////////////////////////////////////////
///  global public  LumiFIRecordLoadFromFile
///  Loads FIRecord from file
///
///  @param [in]       pFileName const char *		 pointer to the file name
///  @param [in]       Standard LUMI_FIR_STANDARD    Standard of the FIRecord to load
///  @param [in, out]  pHRecord FIRECORD_HANDLE *    pointer to FIRECORD_HANDLE that recieves handle to loaded FIRecord
///
///  @return LumiStatus Refer to Error code documentation for detailed description of 
///						possible return values. LUMI_STATUS_OK indicates operation was 
///					    successful
///
///  @remarks See LUMI_FIR_STANDARD for information on standards supported for FIRecord.
///
///  @see LumiFIRecordSaveToFile
///
///  @author www.lumidigm.com @date 9/12/2008
///////////////////////////////////////////////////////////////////////////////

LUMI_INOP_EXPORT LumiStatus STDCALL LumiFIRecordLoadFromFile(const char* pFileName,
															 LUMI_FIR_STANDARD Standard,
															 FIRECORD_HANDLE* pHRecord);

/*********************** LumiFIRecordGetParameters ***********************/
///////////////////////////////////////////////////////////////////////////////
///  global public  LumiFIRecordGetParameters
///  Gets FIRecord parameters
///
///  @param [in]       hRecord FIRECORD_HANDLE						Handle to FIRecord
///  @param [in, out]  nDeviceID uint &								Capture device ID
///  @param [in, out]  nImageAcqLevel uint &						Image acquisition level
///  @param [in, out]  nFingers uint &								Number of fingers/palms in FIRecord
///  @param [in, out]  nScaleUnit uint &							Unit used for Scan and Image resolution. 1 indicates pixels/inch. 
///																	2 indicates pixels/cm.
///  @param [in, out]  nScanXres uint &								Scan resolution (horizontal)
///  @param [in, out]  nScanYres uint &								Scan resolution (vertical)
///  @param [in, out]  nImageXres uint &							Image resolution (horizontal)
///  @param [in, out]  nImageYres uint &							Image resolution (vertical)
///  @param [in, out]  nBPP uint &									Pixel depth of images (number of bits per pixel)
///  @param [in, out]  CompressionType LUMI_COMPRESSION_TYPE &		Compression type used for storing images
///  @param [in, out]  Standard LUMI_FIR_STANDARD &					Standard used for the FIRecord
///
///  @return LumiStatus Refer to Error code documentation for detailed description of 
///						possible return values. LUMI_STATUS_OK indicates operation was 
///					    successful
///
///  @remarks See LUMI_COMPRESSION_TYPE for information on types of compressions.
///			  Currently following compression types are supported:
///				UNCOMPRESSED_NO_BIT_PACKING		
///				COMPRESSED_WSQ - Compression ratio of 5:1 is used	
///			  See LUMI_FIR_STANDARD for information on standards supported for FIRecord.
///
///  @see LumiFIRecordGetViewParameters
///
///  @author www.lumidigm.com @date 9/12/2008
///////////////////////////////////////////////////////////////////////////////

LUMI_INOP_EXPORT LumiStatus STDCALL LumiFIRecordGetParameters(FIRECORD_HANDLE hRecord,
															  uint& nDeviceID,
															  uint& nImageAcqLevel,
															  uint& nFingers,
															  uint& nScaleUnit,
															  uint& nScanXres,
															  uint& nScanYres,
															  uint& nImageXres,
															  uint& nImageYres,
															  uint& nBPP,
															  LUMI_COMPRESSION_TYPE& CompressionType,
															  LUMI_FIR_STANDARD& Standard);

/*********************** LumiFIRecordGetViewParameters ***********************/
///////////////////////////////////////////////////////////////////////////////
///  global public  LumiFIRecordGetViewParameters
///  Gets View parameters for view with specified finger/palm position and view number from FIRecord
///
///  @param [in]       hRecord FIRECORD_HANDLE						Handle to FIRecord
///  @param [in]       FingerPosition LUMI_FINGER_PALM_POSITION		Finger/palm position of the view to retrieve
///  @param [in]       nViewNumberOfFinger uint						View number of the Finger position to retrieve 
///  @param [in, out]  nViewCount uint &							Total number of views available for the finger position
///  @param [in, out]  nImageQuality uint &							Image quality
///  @param [in, out]  ImpressionType LUMI_IMPRESSION_TYPE &		Impression type of the image 
///  @param [in, out]  nWidth uint &								Width of the image
///  @param [in, out]  nHeight uint &								Height of the image
///
///  @return LumiStatus Refer to Error code documentation for detailed description of 
///						possible return values. LUMI_STATUS_OK indicates operation was 
///					    successful
///
///  @remarks See LUMI_FINGER_PALM_POSITION, LUMI_IMPRESSION_TYPE for information on Finger/palm positions and 
///			  impression types.
///
///  @see LumiFIRecordGetParameters LumiFIRecordGetViewCount
///
///  @author www.lumidigm.com @date 9/12/2008
///////////////////////////////////////////////////////////////////////////////

LUMI_INOP_EXPORT LumiStatus STDCALL LumiFIRecordGetViewParameters(FIRECORD_HANDLE hRecord,
																  LUMI_FINGER_PALM_POSITION FingerPosition,
																  uint nViewNumberOfFinger,
																  uint& nViewCount,
																  uint& nImageQuality,
																  LUMI_IMPRESSION_TYPE& ImpressionType,																  
																  uint& nWidth,
																  uint& nHeight);

/*********************** LumiFIRecordGetViewCount ***********************/
///////////////////////////////////////////////////////////////////////////////
///  global public  LumiFIRecordGetViewCount
///  Gets view count for the specified finger position from FIRecord
///
///  @param [in]       hRecord FIRECORD_HANDLE						Handle to FIRecord
///  @param [in]       FingerPosition LUMI_FINGER_PALM_POSITION		Finger/palm position
///  @param [in, out]  nViewCount uint &							Total number of views available for the finger position
///
///  @return LumiStatus Refer to Error code documentation for detailed description of 
///						possible return values. LUMI_STATUS_OK indicates operation was 
///					    successful
///
///  @remarks See LUMI_FINGER_PALM_POSITION for information on Finger/palm positions.
///
///  @see LumiFIRecordGetViewParameters
///
///  @author www.lumidigm.com @date 9/12/2008
///////////////////////////////////////////////////////////////////////////////

LUMI_INOP_EXPORT LumiStatus STDCALL LumiFIRecordGetViewCount(FIRECORD_HANDLE hRecord,
															 LUMI_FINGER_PALM_POSITION FingerPosition,
															 uint& nViewCount);

/*********************** LumiFIRecordGetImage ***********************/
///////////////////////////////////////////////////////////////////////////////
///  global public  LumiFIRecordGetImage
///  Gets an Image with specified finger/palm position and view number from the FIRecord
///
///  @param [in]       hRecord FIRECORD_HANDLE					   Handle to FIRecord
///  @param [in]       FingerPosition LUMI_FINGER_PALM_POSITION    Finger/palm position of the image to retrieve
///  @param [in]       nViewNumberOfFinger uint					   View number of the Finger position to retrieve 
///  @param [in, out]  pImage uchar *							   Preallocated buffer which will contain output image
///  @param [in, out]  nWidth uint &							   Width of output image
///  @param [in, out]  nHeight uint &							   Heigth of output image
///
///  @return LumiStatus Refer to Error code documentation for detailed description of 
///						possible return values. LUMI_STATUS_OK indicates operation was 
///					    successful
///
///  @remarks Client needs to preallocate buffer for output image. Output Image buffer size can be calculated by issuing call to 
///			  LumiFIRecordGetViewParameters.
///
///  @see LumiFIRecordGetViewParameters
///
///  @author www.lumidigm.com @date 9/12/2008
///////////////////////////////////////////////////////////////////////////////

LUMI_INOP_EXPORT LumiStatus STDCALL LumiFIRecordGetImage(FIRECORD_HANDLE hRecord,
														 LUMI_FINGER_PALM_POSITION FingerPosition,
														 uint nViewNumberOfFinger,
														 uchar* pImage,
														 uint& nWidth,
														 uint& nHeight);

/*********************** LumiExtractTemplate ***********************/
///////////////////////////////////////////////////////////////////////////////
///  global public  LumiExtractTemplate
///  Extracts an ANSI378 template from an image
///
///  @param [in, out]  pImage uchar *						pointer to input Image
///  @param [in]       nWidth uint							Width of the Image
///  @param [in]       nHeight uint							Height of the Image
///  @param [in, out]  pTemplate uchar *					Preallocated buffer of 5000 bytes for output ANSI378 template
///  @param [in, out]  nTemplateSize uint &					Size of output ANSI378 template returned
///
///  @return LumiStatus Refer to Error code documentation for detailed description of 
///						possible return values. LUMI_STATUS_OK indicates operation was 
///					    successful
///
///  @remarks Client needs to preallocate buffer of 5000 bytes for output ANSI378 template. 
///
///  @see 
///
///  @author www.lumidigm.com @date 10/19/2010
///////////////////////////////////////////////////////////////////////////////

LUMI_INOP_EXPORT LumiStatus STDCALL LumiExtractTemplate(uchar* pImage,
														uint nWidth,
														uint nHeight,
														uchar* pTemplate,
														uint& nTemplateSize);

/*********************** LumiMatchTemplate ***********************/
///////////////////////////////////////////////////////////////////////////////
///  global public  LumiMatchTemplate
///  Matches two templates and returns score. Input templates must be ANSI378 templates
///
///  @param [in, out]  pTemplate1 uchar *		pointer to input Template1 for match
///  @param [in]	   nTemplate1Size uint	    Size of Template1
///  @param [in, out]  pTemplate2 uchar *		pointer to input Template2 for match
///  @param [in]	   nTemplate2Size uint	    Size of Template2
///  @param [in, out]  nScore uint &			Match score 
///
///  @return LumiStatus Refer to Error code documentation for detailed description of 
///						possible return values. LUMI_STATUS_OK indicates operation was 
///					    successful
///
///  @remarks Input templates must be ANSI378 templates
///
///  @see 
///
///  @author www.lumidigm.com @date 10/25/2010
///////////////////////////////////////////////////////////////////////////////

LUMI_INOP_EXPORT LumiStatus STDCALL LumiMatchTemplate(uchar* pTemplate1,
													  uint nTemplate1Size,
													  uchar* pTemplate2,
													  uint nTemplate2Size,
													  uint& nScore);