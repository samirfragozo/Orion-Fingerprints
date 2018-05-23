VERSION 5.00
Begin VB.Form Image 
   Caption         =   "VB 6.0 Demo"
   ClientHeight    =   8625
   ClientLeft      =   60
   ClientTop       =   450
   ClientWidth     =   9735
   LinkTopic       =   "Form1"
   ScaleHeight     =   575
   ScaleMode       =   3  'Pixel
   ScaleWidth      =   649
   StartUpPosition =   3  'Windows Default
   Begin VB.CommandButton cmdCaptureTemplate 
      Caption         =   "Capture Template"
      Enabled         =   0   'False
      Height          =   735
      Left            =   6120
      TabIndex        =   6
      Top             =   840
      Width           =   975
   End
   Begin VB.CommandButton cmdInitSensors 
      Caption         =   "Init Sensors"
      Height          =   615
      Left            =   3840
      TabIndex        =   5
      Top             =   120
      Width           =   5535
   End
   Begin VB.TextBox txtMessage 
      Height          =   6615
      Left            =   3840
      MultiLine       =   -1  'True
      ScrollBars      =   2  'Vertical
      TabIndex        =   4
      Top             =   1800
      Width           =   5655
   End
   Begin VB.CommandButton cmdModelInfo 
      Caption         =   "Get Model Info"
      Enabled         =   0   'False
      Height          =   735
      Left            =   7200
      TabIndex        =   3
      Top             =   840
      Width           =   975
   End
   Begin VB.CommandButton cmdMatch 
      Caption         =   "Match"
      Enabled         =   0   'False
      Height          =   735
      Left            =   8280
      TabIndex        =   2
      Top             =   840
      Width           =   975
   End
   Begin VB.CommandButton cmdLumiCapture 
      Caption         =   "Capture Image"
      Enabled         =   0   'False
      Height          =   735
      Left            =   5040
      TabIndex        =   1
      Top             =   840
      Width           =   975
   End
   Begin VB.CommandButton cmdSnapShot 
      Caption         =   "Snap Shot"
      Enabled         =   0   'False
      Height          =   735
      Left            =   3960
      TabIndex        =   0
      Top             =   840
      Width           =   975
   End
End
Attribute VB_Name = "Image"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
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
 
' Date: Wed, 19 Aug 2009
' Author: RMcKee
' This example demonstrates how to interface with the
' LumiAPI.dll using VB 6.0
'
' Make the VB6Example.exe in the bin folder where LumiAPI.dll resides.
'
' Copyright 2011 Lumidigm
'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
Option Explicit
Option Base 0

Dim retval As LUMI_STATUS
Dim mConnHandle As Long
Dim mbytImageBuffer() As Byte
Dim mbytTemplateBuffer() As Byte
Dim mbytImageBufferTwo() As Byte
Dim mbytTemplateBufferTwo() As Byte
Dim mDevice As LUMI_DEVICE
Dim mWidth As Long
Dim mHeight As Long
Dim mBPP As Long
Dim mDPI As Long
Dim mDevConfig As LUMI_CONFIG
Dim mStrConf As String
Dim mStart
Dim mStop
Dim token As Long ' Needed to close GDI+

Private Sub Form_Load()
   ' Load the GDI+ Dll
   Dim GpInput As GdiplusStartupInput
   GpInput.GdiplusVersion = 1
   If GdiplusStartup(token, GpInput) <> Ok Then
      MsgBox "Error loading GDI+!", vbCritical
      Unload Me
   End If
End Sub
Private Sub Form_Unload(Cancel As Integer)
   ' Unload the GDI+ Dll
   Call GdiplusShutdown(token)
End Sub

Private Sub cmdInitSensors_Click()
    Const strIPList As String = "192.168.0.65"
        
    On Error GoTo errorHandler

    retval = -1
    Dim nNumDevices As Long
    retval = queryNumberDevices(nNumDevices, strIPList)
        
    If retval = LUMI_STATUS_OK Then
        txtMessage.Text = txtMessage.Text & "PASS: Found " & nNumDevices & " Device(s)" & Chr(13) & Chr(10)
    Else
        txtMessage.Text = txtMessage.Text & "FAIL: queryNumberDevices retval = " & GetLumiStatusString(retval) & " nNumDevices = " & nNumDevices
        Exit Sub
    End If
    
    'Passing 0 as the device to query since there is only one device in this example.  If there
    'were more, we would have to iterate through the number of devices.  The list of devices is 0 based.
    retval = queryDevice(0, mDevice)
    If retval = LUMI_STATUS_OK Then
        Dim strIdentifier As String
        strIdentifier = Replace(Left(mDevice.strIdentifier, InStr(1, mDevice.strIdentifier, vbNullChar) - 1), Chr(10), "")
        txtMessage.Text = txtMessage.Text & "PASS: queryDevice.  hDevHandle = " & mDevice.hDevHandle & " strIdentifier = " & strIdentifier & Chr(13) & Chr(10)
        txtMessage.Text = txtMessage.Text & "                    bCaptureImages = " & mDevice.dCaps.bCaptureImages & Chr(13) & Chr(10)
        txtMessage.Text = txtMessage.Text & "                    bExtract = " & mDevice.dCaps.bExtract & Chr(13) & Chr(10)
        txtMessage.Text = txtMessage.Text & "                    bIdentify = " & mDevice.dCaps.bIdentify & Chr(13) & Chr(10)
        txtMessage.Text = txtMessage.Text & "                    bSpoof = " & mDevice.dCaps.bSpoof & Chr(13) & Chr(10)
        txtMessage.Text = txtMessage.Text & "                    bMatch = " & mDevice.dCaps.bMatch & Chr(13) & Chr(10)
        txtMessage.Text = txtMessage.Text & "                    eTemplate = " & mDevice.dCaps.eTemplate & Chr(13) & Chr(10)
        txtMessage.Text = txtMessage.Text & "                    eTransInfo = " & mDevice.dCaps.eTransInfo & Chr(13) & Chr(10)
        txtMessage.Text = txtMessage.Text & "                    nDPI = " & mDevice.dCaps.nDPI & Chr(13) & Chr(10)
        txtMessage.Text = txtMessage.Text & "                    nHeight = " & mDevice.dCaps.nHeight & Chr(13) & Chr(10)
        txtMessage.Text = txtMessage.Text & "                    nWidth = " & mDevice.dCaps.nWidth & Chr(13) & Chr(10)
        txtMessage.Text = txtMessage.Text & "                    nImageFormat = " & mDevice.dCaps.nImageFormat & Chr(13) & Chr(10)
        txtMessage.Text = txtMessage.Text & "                    strIdentifier = " & mDevice.strIdentifier & Chr(13) & Chr(10)
    Else
        txtMessage.Text = txtMessage.Text & "FAIL: queryDevice.  hDevHandle = " & mDevice.hDevHandle & " retval = " & GetLumiStatusString(retval) & Chr(13) & Chr(10)
        Exit Sub
    End If
    
    retval = init(mDevice.hDevHandle, mConnHandle)
    If retval = LUMI_STATUS_OK Then
        txtMessage.Text = txtMessage.Text & Chr(13) & Chr(10) & "PASS: init.  mConnHandle = " & mConnHandle & Chr(13) & Chr(10)
    Else
        txtMessage.Text = txtMessage.Text & Chr(13) & Chr(10) & "FAIL: init.  hDevHandle = " & mDevice.hDevHandle & " retval = " & GetLumiStatusString(retval) & Chr(13) & Chr(10)
        Exit Sub
    End If
    
    retval = getImageParams(mConnHandle, mWidth, mHeight, mBPP, mDPI)
    If retval = LUMI_STATUS_OK Then
        txtMessage.Text = txtMessage.Text & "PASS: getImageParams.  mWidth = " & mWidth & " mHeight = " & mHeight & " mBPP = " & mBPP & " mDPI = " & mDPI & Chr(13) & Chr(10)
    Else
        txtMessage.Text = txtMessage.Text & "FAIL: getImageParams.  mConnHandle = " & mConnHandle & " retval = " & GetLumiStatusString(retval) & Chr(13) & Chr(10)
        Exit Sub
    End If
    
    Dim hVersion As LUMI_VERSION
    
    retval = getVersionInfo(mConnHandle, hVersion)
    
    If (retval = LUMI_STATUS_OK) Then
        Dim strSdk As String
        Dim strFw As String
        Dim strProc As String
        Dim strConf As String
        strSdk = Replace(Left(hVersion.sdkVersion, InStr(1, hVersion.sdkVersion, vbNullChar) - 1), Chr(10), "")
        strFw = Replace(Left(hVersion.fwrVersion, InStr(1, hVersion.fwrVersion, vbNullChar) - 1), Chr(10), "")
        strProc = Replace(Left(hVersion.prcVersion, InStr(1, hVersion.prcVersion, vbNullChar) - 1), Chr(10), "")
        strConf = Replace(Left(hVersion.tnsVersion, InStr(1, hVersion.tnsVersion, vbNullChar) - 1), Chr(10), "")
        mStrConf = strConf
        txtMessage.Text = txtMessage.Text & "PASS: Get version information  SDK = " & strSdk & " FW = " & strFw & " PROC = " & strProc & " CONF = " & strConf & Chr(13) & Chr(10)

    Else
        txtMessage.Text = txtMessage.Text & "FAILED: Get version information" & GetLumiStatusString(retval) & Chr(13) & Chr(10)
        Exit Sub
    End If
    
    'Set trigger timeout to 15 seconds
    Dim lumiConfig As LUMI_CONFIG
   
    retval = getConfig(mConnHandle, lumiConfig)
    If (retval = LUMI_STATUS_OK) Then
        txtMessage.Text = txtMessage.Text & "PASS: Get Config" & Chr(13) & Chr(10)
    Else
        txtMessage.Text = txtMessage.Text & "FAIL: Get Config retval = " & GetLumiStatusString(retval) & Chr(13) & Chr(10)
        Exit Sub
    End If
    
    lumiConfig.nTriggerTimeout = 15

    retval = setConfig(mConnHandle, lumiConfig.eTplInfoTemplateType, lumiConfig.eTransInfo, lumiConfig.nTriggerTimeout)
    If (retval = LUMI_STATUS_OK) Then
        txtMessage.Text = txtMessage.Text & "PASS: Set Config" & Chr(13) & Chr(10)
    Else
        txtMessage.Text = txtMessage.Text & "FAIL: Set Config retval = " & GetLumiStatusString(retval) & Chr(13) & Chr(10)
        Exit Sub
    End If
     
    cmdLumiCapture.Enabled = True
    cmdSnapShot.Enabled = True
    cmdCaptureTemplate.Enabled = True
    cmdModelInfo.Enabled = True
    cmdMatch.Enabled = True
    cmdInitSensors.Enabled = False
            
    Exit Sub
errorHandler:
    
    MsgBox (Err.Description)
    txtMessage.Text = txtMessage.Text & "Fail: InitSensors" & Chr(13) & Chr(10)
End Sub

Private Sub cmdSnapShot_Click()
    
    On Error GoTo errorHandler
        
    retval = snapShot()
    
    If (retval = LUMI_STATUS_OK) Then
        txtMessage.Text = txtMessage.Text & "PASS: lumiSnapShot." & Chr(13) & Chr(10)
        txtMessage.Text = txtMessage.Text & "      Elapsed time in seconds = " & ((mStop - mStart) / 1000) & Chr(13) & Chr(10)

    Else
        txtMessage.Text = txtMessage.Text & "FAILED: lumiSnapShot." & GetLumiStatusString(retval) & Chr(13) & Chr(10)
        txtMessage.Text = txtMessage.Text & "        Elapsed time in seconds = " & ((mStop - mStart) / 1000) & Chr(13) & Chr(10)

    End If
        
    Exit Sub

errorHandler:
    
    Call CloseScanner
    Exit Sub
End Sub

Private Sub cmdLumiCapture_Click()
    On Error GoTo errorHandler
    Dim x As Long
        
    retval = capture()
        
    If (retval = LUMI_STATUS_OK) Then
        txtMessage.Text = txtMessage.Text & "PASS: Capture Composite Image." & Chr(13) & Chr(10)
    Else
        txtMessage.Text = txtMessage.Text & "FAILED: Capture Composite Image." & Chr(13) & Chr(10)
    End If
    
    Exit Sub

errorHandler:
    Call CloseScanner
  
    Exit Sub
End Sub

Private Sub cmdCaptureTemplate_Click()
    On Error GoTo errorHandler
        
    retval = captureAndExtract()
    
    If (retval = LUMI_STATUS_OK) Then
        txtMessage.Text = txtMessage.Text & "PASS: captureAndExtract." & Chr(13) & Chr(10)
    Else
        txtMessage.Text = txtMessage.Text & "FAILED: captureAndExtract." & Chr(13) & Chr(10)
    End If
          
    Exit Sub

errorHandler:
    
    
    Call CloseScanner
  
    Exit Sub
End Sub
Private Sub cmdMatch_Click()
    On Error GoTo errorHandler
        
    retval = match()
    
    If (retval = LUMI_STATUS_OK) Then
        txtMessage.Text = txtMessage.Text & "PASS: lumiMatch." & Chr(13) & Chr(10)
    Else
        txtMessage.Text = txtMessage.Text & "FAILED: lumiMatch." & GetLumiStatusString(retval) & Chr(13) & Chr(10)
    End If
    
    Exit Sub

errorHandler:
    
    Call CloseScanner

    Exit Sub
End Sub

Private Sub cmdModelInfo_Click()
    On Error GoTo errorHandler
    
    retval = GetModelInfo()
        
    Exit Sub

errorHandler:

    Call CloseScanner

    Exit Sub
End Sub



Private Function CloseScanner()

    On Error GoTo errorHandler
    
    retval = closeDevice(mDevice.hDevHandle)

    retval = -1
    txtMessage.Text = txtMessage.Text & "Closed Scanner" & Chr(13) & Chr(10)
    
    cmdLumiCapture.Enabled = False
    cmdSnapShot.Enabled = False
    cmdCaptureTemplate.Enabled = False
    cmdModelInfo.Enabled = False
    cmdMatch.Enabled = False
    cmdInitSensors.Enabled = True
    
    Exit Function

errorHandler:
    

    retval = -1
    Exit Function

End Function

Private Function GetModelInfo()
    Dim hVersion As LUMI_VERSION
    
    On Error GoTo errorHandler
    
    mStart = GetTickCount
    
    retval = getVersionInfo(mConnHandle, hVersion)
    
    mStop = GetTickCount
    
    If (retval = LUMI_STATUS_OK) Then
        Dim strSdk As String
        Dim strFw As String
        Dim strProc As String
        Dim strConf As String
        strSdk = Replace(Left(hVersion.sdkVersion, InStr(1, hVersion.sdkVersion, vbNullChar) - 1), Chr(10), "")
        strFw = Replace(Left(hVersion.fwrVersion, InStr(1, hVersion.fwrVersion, vbNullChar) - 1), Chr(10), "")
        strProc = Replace(Left(hVersion.prcVersion, InStr(1, hVersion.prcVersion, vbNullChar) - 1), Chr(10), "")
        strConf = Replace(Left(hVersion.tnsVersion, InStr(1, hVersion.tnsVersion, vbNullChar) - 1), Chr(10), "")
        txtMessage.Text = txtMessage.Text & "PASS: Get version information  SDK = " & strSdk & " FW = " & strFw & " PROC = " & strProc & " CONF = " & strConf & Chr(13) & Chr(10)
        txtMessage.Text = txtMessage.Text & "      Elapsed time in seconds = " & ((mStop - mStart) / 1000) & Chr(13) & Chr(10)

    Else
        txtMessage.Text = txtMessage.Text & "FAILED: Get version information" & GetLumiStatusString(retval) & Chr(13) & Chr(10)
        txtMessage.Text = txtMessage.Text & "      Elapsed time in seconds = " & ((mStop - mStart) / 1000) & Chr(13) & Chr(10)

    End If
    
    Exit Function

errorHandler:
    Call CloseScanner
    Exit Function
        
End Function

Private Function snapShot()
    Dim lngSpoofFlag As Long
    Dim lngTemplateSize As Long
    Dim lngHeight As Long
    Dim lngWidth As Long

    On Error GoTo errorHandler
           
    MsgBox ("Place finger on Scanner.")
    Me.Refresh
    
    mStart = GetTickCount
    
    ReDim mbytImageBuffer(mWidth * mHeight)
    
    retval = takeSnapShot(mConnHandle, VarPtr(mbytImageBuffer(0)), 128, 10)
    
    mStop = GetTickCount

    ' Set up image
    If (retval = LUMI_STATUS_OK) Then
        Call SetUpImage(mbytImageBuffer())
    End If
        
    snapShot = retval
    
    Exit Function

errorHandler:

    Call CloseScanner
    Exit Function

End Function

Private Function capture()
    Dim i As Integer
    Dim j As Integer
    Dim lngColor  As Long
    Dim lngGrayscale As Long
    Dim lngSpoofFlag As Long
    Dim lngTemplateSize As Long
    Dim lngHeight As Long
    Dim lngWidth As Long
    Dim lngTmpLength As Long
        
    On Error GoTo errorHandler
            
    Me.Refresh
    
    mStart = GetTickCount
    
    ReDim mbytImageBuffer(mWidth * mHeight)
    
    ReDim mbytTemplateBuffer(5000)
    
    retval = captureImage(mConnHandle, VarPtr(mbytImageBuffer(0)), lngSpoofFlag, 0&)
    
    mStop = GetTickCount
       
    ' Set up image
    If (retval = LUMI_STATUS_OK) Then
        If (mStrConf <> "20") Then
            Call SetUpImage(mbytImageBuffer())
        End If
        txtMessage.Text = txtMessage.Text & "PASS: captureImage lngTmpLength = " & lngTmpLength & " lngSpoofFlag = " & lngSpoofFlag & Chr(13) & Chr(10)
        txtMessage.Text = txtMessage.Text & "      Elapsed time in seconds = " & ((mStop - mStart) / 1000) & Chr(13) & Chr(10)
    Else
        txtMessage.Text = txtMessage.Text & "FAIL: captureImage lngTmpLength = " & lngTmpLength & " lngSpoofFlag = " & lngSpoofFlag & " retval = " & GetLumiStatusString(retval) & Chr(13) & Chr(10)
        txtMessage.Text = txtMessage.Text & "      Elapsed time in seconds = " & ((mStop - mStart) / 1000) & Chr(13) & Chr(10)

    End If
        
    capture = retval
    Exit Function

errorHandler:
    txtMessage.Text = txtMessage.Text & Err.Description
    Call CloseScanner
    Exit Function

End Function

Private Function captureAndExtract()
    Dim i As Integer
    Dim j As Integer
    Dim lngColor  As Long
    Dim lngGrayscale As Long
    Dim lngSpoofFlag As Long
    Dim lngTemplateSize As Long
    Dim lngHeight As Long
    Dim lngWidth As Long
    Dim lngTmpLength As Long
        
    On Error GoTo errorHandler
            
    Me.Refresh
    
    mStart = GetTickCount
    
    ReDim mbytImageBuffer(mWidth * mHeight)
    
    ReDim mbytTemplateBuffer(5000)
  
    retval = captureImageEx(mConnHandle, VarPtr(mbytImageBuffer(0)), VarPtr(mbytTemplateBuffer(0)), lngTmpLength, lngSpoofFlag, 0&)
         
    mStop = GetTickCount
    
    ' Set up image
    If (retval = LUMI_STATUS_OK) Then
        If (mStrConf <> "20") Then
            Call SetUpImage(mbytImageBuffer())
        End If
        txtMessage.Text = txtMessage.Text & "PASS: captureImage and Extract Template lngTmpLength = " & lngTmpLength & " lngSpoofFlag = " & lngSpoofFlag & Chr(13) & Chr(10)
        txtMessage.Text = txtMessage.Text & "      Elapsed time in seconds = " & ((mStop - mStart) / 1000) & Chr(13) & Chr(10)

    Else
        txtMessage.Text = txtMessage.Text & "FAIL: captureImage and Extract Template lngTmpLength = " & lngTmpLength & " lngSpoofFlag = " & lngSpoofFlag & " retval = " & GetLumiStatusString(retval) & Chr(13) & Chr(10)
        txtMessage.Text = txtMessage.Text & "      Elapsed time in seconds = " & ((mStop - mStart) / 1000) & Chr(13) & Chr(10)

    End If
    
    captureAndExtract = retval
    Exit Function

errorHandler:
    txtMessage.Text = txtMessage.Text & Err.Description
    Call CloseScanner
    Exit Function
End Function

Private Function match()
    Dim lngTemplateSize As Long
    Dim lngHeight As Long
    Dim lngWidth As Long
    Dim lngScore As Long
    Dim lngSpoof As Long
    Dim lngTmpLength As Long
    Dim lngTmpLengthTwo As Long
    Dim lngSpoofFlag As Long
    Dim lngSpoofFlagTwo As Long
    Dim start
    Dim stopp
    
        
    On Error GoTo errorHandler

    Me.Refresh
    
    ReDim mbytImageBuffer(mWidth * mHeight)
    
    ReDim mbytImageBufferTwo(mWidth * mHeight)
    
    ReDim mbytTemplateBuffer(5000)
    
    ReDim mbytTemplateBufferTwo(5000)
    
    mStart = GetTickCount
    start = mStart
    retval = captureImageEx(mConnHandle, VarPtr(mbytImageBuffer(0)), VarPtr(mbytTemplateBuffer(0)), lngTmpLength, lngSpoofFlag, 0&)
    stopp = GetTickCount
    
    ' Set up image
    If (retval = LUMI_STATUS_OK) Then
        If (mStrConf <> "20") Then
            Call SetUpImage(mbytImageBuffer())
        End If
        txtMessage.Text = txtMessage.Text & "PASS: captureImage and Extract Template  lngTmpLength = " & lngTmpLength & " lngSpoofFlag = " & lngSpoofFlag & Chr(13) & Chr(10)
        txtMessage.Text = txtMessage.Text & "      Elapsed time in seconds = " & ((stopp - start) / 1000) & Chr(13) & Chr(10)

    Else
        txtMessage.Text = txtMessage.Text & "FAIL: captureImage and Extract Template retval = " & GetLumiStatusString(retval) & Chr(13) & Chr(10)
        txtMessage.Text = txtMessage.Text & "      Elapsed time in seconds = " & ((stopp - start) / 1000) & Chr(13) & Chr(10)

    End If

    Me.Refresh
    
    start = GetTickCount
    retval = captureImageEx(mConnHandle, VarPtr(mbytImageBufferTwo(0)), VarPtr(mbytTemplateBufferTwo(0)), lngTmpLengthTwo, lngSpoofFlagTwo, 0&)
    stopp = GetTickCount
    
    ' Set up image
    If (retval = LUMI_STATUS_OK) Then
        If (mStrConf <> "20") Then
            Call SetUpImage(mbytImageBufferTwo())
        End If
        txtMessage.Text = txtMessage.Text & "PASS: captureImage and Extract Template  lngTmpLengthTwo = " & lngTmpLengthTwo & " lngSpoofFlagTwo = " & lngSpoofFlagTwo & Chr(13) & Chr(10)
        txtMessage.Text = txtMessage.Text & "      Elapsed time in seconds = " & ((stopp - start) / 1000) & Chr(13) & Chr(10)

    Else
        txtMessage.Text = txtMessage.Text & "FAIL: captureImage and Extract Template retval = " & GetLumiStatusString(retval) & Chr(13) & Chr(10)
        txtMessage.Text = txtMessage.Text & "      Elapsed time in seconds = " & ((stopp - start) / 1000) & Chr(13) & Chr(10)

    End If
    
    retval = matchTemplate(mConnHandle, VarPtr(mbytTemplateBufferTwo(0)), lngTmpLengthTwo, VarPtr(mbytTemplateBuffer(0)), lngTmpLength, lngScore, lngSpoof)
           
    mStop = GetTickCount
           
    If (retval = LUMI_STATUS_OK) Then
        txtMessage.Text = txtMessage.Text & "PASS: Match  Score = " & lngScore & " Spoof Score = " & lngSpoof & Chr(13) & Chr(10)
        txtMessage.Text = txtMessage.Text & "      Elapsed time in seconds = " & ((mStop - mStart) / 1000) & Chr(13) & Chr(10)

    Else
        txtMessage.Text = txtMessage.Text & "FAIL: Match  retval = " & GetLumiStatusString(retval) & Chr(13) & Chr(10)
        txtMessage.Text = txtMessage.Text & "      Elapsed time in seconds = " & ((mStop - mStart) / 1000) & Chr(13) & Chr(10)

    End If
    
    match = retval
    
    Exit Function

errorHandler:
    
    Call CloseScanner
    Exit Function

End Function
Private Function SetUpImage(imageBuffer() As Byte)
    Dim i As Long
    Dim j As Long
    Dim lngColor  As Long
    Dim lngGrayscale As Long
    Dim bitmap As Long          'Pointer to bitmap
    Dim graphics As Long        'Pointer to graphics object
    Dim lngHeight As Long
    Dim lngWidth As Long
    Dim rc As GpStatus          'return code for GDI+ function calls
    Dim PixelFormat As Long
    
    'Initialize the graphics class - required for all drawing
    rc = GdipCreateFromHDC(Me.hdc, graphics)
   
    'Create an empty bitmap
    rc = GdipCreateBitmapFromScan0(mWidth, mHeight, 0, PixelFormat24bppRGB, ByVal 0, bitmap)
        
    'Get properties from the bitmap
    rc = GdipGetImageHeight(bitmap, lngHeight)
    rc = GdipGetImageWidth(bitmap, lngWidth)
    rc = GdipGetImagePixelFormat(bitmap, PixelFormat)

    'Set the pixels of the bitmap to values from the imageBuffer
    For i = 0 To mHeight - 1
        For j = 0 To mWidth - 1
            lngGrayscale = imageBuffer(j + i * mWidth) ' get the pixel value at the current point
            lngColor = RGB(lngGrayscale, lngGrayscale, lngGrayscale)
            rc = GdipBitmapSetPixel(bitmap, j, i, ToARGB(lngColor)) ' change RGB to ARGB
        Next
    Next
    
    ' Shrink the image using high-quality interpolation.
    rc = GdipSetInterpolationMode(graphics, InterpolationModeHighQualityBicubic)
    ' Draw the image to the form
    rc = GdipDrawImageRectRectI(graphics, bitmap, 10, 10, 0.6 * lngWidth, 0.6 * lngHeight, 0, 0, lngWidth, lngHeight, UnitPixel)
    
   ' Cleanup
   Call GdipDisposeImage(bitmap)
   Call GdipDeleteGraphics(graphics)
End Function

Private Function GetLumiStatusString(lumiStatus As LUMI_STATUS) As String
    Dim strReturn As String
    
    Select Case lumiStatus
        Case LUMI_STATUS_OK
            strReturn = "LUMI_STATUS_OK"
        Case LUMI_STATUS_ERROR_DEVICE_OPEN
            strReturn = "LUMI_STATUS_ERROR_DEVICE_OPEN"
        Case LUMI_STATUS_ERROR_DEVICE_CLOSE
            strReturn = "LUMI_STATUS_ERROR_DEVICE_CLOSE"
        Case LUMI_STATUS_ERROR_CMD_NOT_SUPPORTED
            strReturn = "LUMI_STATUS_ERROR_CMD_NOT_SUPPORTED"
        Case LUMI_STATUS_ERROR_COMM_LINK
            strReturn = "LUMI_STATUS_ERROR_COMM_LINK"
        Case LUMI_STATUS_ERROR_PREPROCESSOR
            strReturn = "LUMI_STATUS_ERROR_PREPROCESSOR"
        Case LUMI_STATUS_ERROR_CALIBRATION
            strReturn = "LUMI_STATUS_ERROR_CALIBRATION"
        Case LUMI_STATUS_ERROR_BUSY
            strReturn = "LUMI_STATUS_ERROR_BUSY"
        Case LUMI_STATUS_ERROR_INVALID_PARAMETER
            strReturn = "LUMI_STATUS_ERROR_INVALID_PARAMETER"
        Case LUMI_STATUS_ERROR_TIMEOUT
            strReturn = "LUMI_STATUS_ERROR_TIMEOUT"
        Case LUMI_STATUS_ERROR_INVALID_TEMPLATE
            strReturn = "LUMI_STATUS_ERROR_INVALID_TEMPLATE"
        Case LUMI_STATUS_ERROR_MEMORY_ALLOCATION
            strReturn = "LUMI_STATUS_ERROR_MEMORY_ALLOCATION"
        Case LUMI_STATUS_ERROR_INVALID_DEVICE_ID
            strReturn = "LUMI_STATUS_ERROR_INVALID_DEVICE_ID"
        Case LUMI_STATUS_ERROR_INVALID_CONNECTION_ID
            strReturn = "LUMI_STATUS_ERROR_INVALID_CONNECTION_ID"
        Case LUMI_STATUS_ERROR_CONFIG_UNSUPPORTED
            strReturn = "LUMI_STATUS_ERROR_CONFIG_UNSUPPORTED"
        Case LUMI_STATUS_UNSUPPORTED
            strReturn = "LUMI_STATUS_UNSUPPORTED"
        Case LUMI_STATUS_INTERNAL_ERROR
            strReturn = "LUMI_STATUS_INTERNAL_ERROR"
        Case LUMI_STATUS_INVALID_PARAMETER
            strReturn = "LUMI_STATUS_INVALID_PARAMETER"
        Case LUMI_STATUS_DEVICE_TIMEOUT
            strReturn = "LUMI_STATUS_DEVICE_TIMEOUT"
        Case LUMI_STATUS_NO_DEVICE_FOUND
            strReturn = "LUMI_STATUS_DEVICE_TIMEOUT"
        Case LUMI_STATUS_ERROR_READ_FILE
             strReturn = "LUMI_STATUS_ERROR_READ_FILE"
        Case LUMI_STATUS_ERROR_WRITE_FILE
             strReturn = "LUMI_STATUS_ERROR_WRITE_FILE"
        Case LUMI_STATUS_INVALID_FILE_FORMAT
             strReturn = "LUMI_STATUS_INVALID_FILE_FORMAT"
        Case LUMI_STATUS_INCOMPATIBLE_FIRMWARE
             strReturn = "LUMI_STATUS_INCOMPATIBLE_FIRMWARE"
        Case LUMI_STATUS_ERROR_TIMEOUT_LATENT
             strReturn = "LUMI_STATUS_ERROR_TIMEOUT_LATENT"
        Case LUMI_STATUS_QUALITY_MAP_NOT_GENERATED
            strReturn = "LUMI_STATUS_QUALITY_MAP_NOT_GENERATED"
        Case LUMI_STATUS_THREAD_ERROR
            strReturn = "LUMI_STATUS_THREAD_ERROR"
        Case LUMI_STATUS_ERROR_SENSOR_COMM_TIMEOUT
            strReturn = "LUMI_STATUS_ERROR_SENSOR_COMM_TIMEOUT"
        Case LUMI_STATUS_DEVICE_STATUS_ERROR
            strReturn = "LUMI_STATUS_DEVICE_STATUS_ERROR"
        Case Else
            strReturn = "Unknown Return Code"
    End Select
    GetLumiStatusString = strReturn
    
End Function

Private Sub Form_Terminate()
    Call CloseScanner
End Sub

