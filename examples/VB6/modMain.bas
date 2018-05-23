Attribute VB_Name = "modMain"
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
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

Option Explicit


'---------------------------------------------------------------------------
'GDI Plus API
'    The following Enums, Types, and Functions are needed to interface with
'    GDI+.  The VB6Example uses GDI+ to create and scale images on the
'    Image form.
'---------------------------------------------------------------------------

''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
'Return status for GDI+ function calls
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
Public Enum GpStatus
   Ok = 0
   GenericError = 1
   InvalidParameter = 2
   OutOfMemory = 3
   ObjectBusy = 4
   InsufficientBuffer = 5
   NotImplemented = 6
   Win32Error = 7
   WrongState = 8
   Aborted = 9
   FileNotFound = 10
   ValueOverflow = 11
   AccessDenied = 12
   UnknownImageFormat = 13
   FontFamilyNotFound = 14
   FontStyleNotFound = 15
   NotTrueTypeFont = 16
   UnsupportedGdiplusVersion = 17
   GdiplusNotInitialized = 18
   PropertyNotFound = 19
   PropertyNotSupported = 20
End Enum

''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
'GDI+ Startup input object
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
Public Type GdiplusStartupInput
   GdiplusVersion As Long              ' Must be 1 for GDI+ v1.0, the current version as of this writing.
   DebugEventCallback As Long          ' Ignored on free builds
   SuppressBackgroundThread As Long    ' FALSE unless you're prepared to call
                                       ' the hook/unhook functions properly
   SuppressExternalCodecs As Long      ' FALSE unless you want GDI+ only to use
                                       ' its internal image codecs.
End Type

''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
'Quality Modes
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
Public Enum QualityMode
   QualityModeInvalid = -1
   QualityModeDefault = 0
   QualityModeLow = 1                   ' Best performance
   QualityModeHigh = 2                  ' Best rendering quality
End Enum

''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
'Interpolation Modes
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
Public Enum InterpolationMode
   InterpolationModeInvalid = QualityModeInvalid
   InterpolationModeDefault = QualityModeDefault
   InterpolationModeLowQuality = QualityModeLow
   InterpolationModeHighQuality = QualityModeHigh
   InterpolationModeBilinear
   InterpolationModeBicubic
   InterpolationModeNearestNeighbor
   InterpolationModeHighQualityBilinear
   InterpolationModeHighQualityBicubic
End Enum

''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
'Pixel formats
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
Public Const PixelFormatIndexed = &H10000           ' Indexes into a palette
Public Const PixelFormatGDI = &H20000               ' Is a GDI-supported format
Public Const PixelFormatAlpha = &H40000             ' Has an alpha component
Public Const PixelFormatPAlpha = &H80000            ' Pre-multiplied alpha
Public Const PixelFormatExtended = &H100000         ' Extended color 16 bits/channel
Public Const PixelFormatCanonical = &H200000

Public Const PixelFormatUndefined = 0
Public Const PixelFormatDontCare = 0

Public Const PixelFormat1bppIndexed = &H30101
Public Const PixelFormat4bppIndexed = &H30402
Public Const PixelFormat8bppIndexed = &H30803
Public Const PixelFormat16bppGreyScale = &H101004
Public Const PixelFormat16bppRGB555 = &H21005
Public Const PixelFormat16bppRGB565 = &H21006
Public Const PixelFormat16bppARGB1555 = &H61007
Public Const PixelFormat24bppRGB = &H21808
Public Const PixelFormat32bppRGB = &H22009
Public Const PixelFormat32bppARGB = &H26200A
Public Const PixelFormat32bppPARGB = &HE200B
Public Const PixelFormat48bppRGB = &H10300C
Public Const PixelFormat64bppARGB = &H34400D
Public Const PixelFormat64bppPARGB = &H1C400E
Public Const PixelFormatMax = 15 '&HF

''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
'Units
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
Public Enum GpUnit  ' aka Unit
   UnitWorld      ' 0 -- World coordinate (non-physical unit)
   UnitDisplay    ' 1 -- Variable -- for PageTransform only
   UnitPixel      ' 2 -- Each unit is one device pixel.
   UnitPoint      ' 3 -- Each unit is a printer's point, or 1/72 inch.
   UnitInch       ' 4 -- Each unit is 1 inch.
   UnitDocument   ' 5 -- Each unit is 1/300 inch.
   UnitMillimeter ' 6 -- Each unit is 1 millimeter.
End Enum


''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
'GDI+ flat API functions used in the VB6Example
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
Public Declare Function GdiplusStartup Lib "gdiplus" (token As Long, inputbuf As GdiplusStartupInput, Optional ByVal outputbuf As Long = 0) As GpStatus
Public Declare Sub GdiplusShutdown Lib "gdiplus" (ByVal token As Long)
Public Declare Function GdipGetImageWidth Lib "gdiplus" (ByVal image As Long, Width As Long) As GpStatus
Public Declare Function GdipGetImageHeight Lib "gdiplus" (ByVal image As Long, Height As Long) As GpStatus
Public Declare Function GdipGetImagePixelFormat Lib "gdiplus" (ByVal image As Long, PixelFormat As Long) As GpStatus
Public Declare Function GdipCreateFromHDC Lib "gdiplus" (ByVal hdc As Long, graphics As Long) As GpStatus
Public Declare Function GdipCreateBitmapFromScan0 Lib "gdiplus" (ByVal Width As Long, ByVal Height As Long, ByVal stride As Long, ByVal PixelFormat As Long, scan0 As Any, bitmap As Long) As GpStatus
Public Declare Function GdipSetInterpolationMode Lib "gdiplus" (ByVal graphics As Long, ByVal interpolation As InterpolationMode) As GpStatus
Public Declare Function GdipBitmapGetPixel Lib "gdiplus" (ByVal bitmap As Long, ByVal x As Long, ByVal y As Long, color As Long) As GpStatus
Public Declare Function GdipBitmapSetPixel Lib "gdiplus" (ByVal bitmap As Long, ByVal x As Long, ByVal y As Long, ByVal color As Long) As GpStatus
Public Declare Function GdipDrawImageRectRectI Lib "gdiplus" (ByVal graphics As Long, ByVal image As Long, ByVal dstx As Long, _
                     ByVal dsty As Long, ByVal dstwidth As Long, ByVal dstheight As Long, _
                     ByVal srcx As Long, ByVal srcy As Long, ByVal srcwidth As Long, ByVal srcheight As Long, _
                     ByVal srcUnit As GpUnit, Optional ByVal imageAttributes As Long = 0, _
                     Optional ByVal callback As Long = 0, Optional ByVal callbackData As Long = 0) As GpStatus
Public Declare Function GdipDisposeImage Lib "gdiplus" (ByVal image As Long) As GpStatus
Public Declare Function GdipDeleteGraphics Lib "gdiplus" (ByVal graphics As Long) As GpStatus
                   
Public Function ToARGB(ByVal lRGB As Long) As Long
'This functions converts an RGB value to a ARGB value, assuming the alpha (transperancy) channel is 100% to give fully opaque pixels
    ToARGB = &HFF000000 Or (((lRGB And &HFF) * 65536) Or (lRGB And &HFF00&) Or (lRGB \ 65536))
End Function
 
