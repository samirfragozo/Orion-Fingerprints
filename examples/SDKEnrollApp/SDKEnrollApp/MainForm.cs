using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;
using SDKEnrollApp.Properties;
using SDKWrapper;

namespace SDKEnrollApp
{
    public delegate void SetTextCallback(string text, Color txtColor);
    public delegate void SetImageDelegate(ref byte[] snapShotRaw, ref byte[] snapShot, uint width, uint height, byte[] pTemplate, uint nTempSz, int nEnrollmentCaptureIndex);
    public delegate void DelegateCaptureThreadFinished();
    public delegate void DelegateVerifyThreadFinished();
    public delegate void DelegateCaptureWithPdCancelled();
    public delegate void DelegateCaptureTimeOut();
    public delegate void DelegateComTimeOut();
    public delegate void DelegateLatentDetected();
    public delegate void WriteResultsDelegate(string text, Color txtColor);
    public delegate void SetNistImageDelegate(uint nistQauality);
    public delegate void PreviewLiveMode(IntPtr pImage, int width, int height, int imgnum);

    //typedef int (*LumiAcqStatusCallback)(uint nStatus);
    //public delegate
    
    public enum SensorType
    {
        M30X,
        V30X,
        V30XId,
        M31X,
        V31X,
        Unknown
    }

    public partial class MainForm : Form
    {
        //Own variables
        private readonly Connection _conn;
        private readonly bool[] _fingersEnrolled;
        private int _currentFinger;
        private int _currentId;
        private readonly string _fingerprintsPath;
        private int _numberDpi;
        private int _numberUmbral;

        private Config _config;
        private readonly XmlSerializer _serializer;
        public uint _nSelectedSensorId;
        public readonly ArrayList _sensorList;
        private bool _bClosingApp;
        private string _sFolderPath;
        private string _debugFolder;
        private readonly Color _red = Color.FromArgb(222, 56, 49);
        private readonly Color _blue = Color.FromArgb(0, 63, 114);
        private Thread _captureWithPdThread;
        private Thread _enrollThread;
        private Thread _verifyThread;
        private DelegateCaptureThreadFinished _captureThreadFinished;
        private DelegateVerifyThreadFinished _verifyThreadFinished;
        private DelegateCaptureThreadFinished _enrollThreadFinished;
        public WriteResultsDelegate _delegateErrorMessage;
        private SetNistImageDelegate _delegateSetNistImage;
        private WriteResultsDelegate _delegateNistStatus;
        private DelegateCaptureWithPdCancelled _delegateCaptureCancelled;
        private DelegateCaptureTimeOut _delegateCaptureTimeOut;
        private DelegateComTimeOut _delegateComTimeOut;
        private DelegateLatentDetected _delegateLatentDetected;
        private PreviewLiveMode _delegatePreviewLiveMode;
        private bool _bPdCaptureInProcess;
        private bool _bCancelCapture;
        private bool _bSensorTriggerArmed;
        public bool _bSpoofEnabled;
        private uint _matchThreshold;
        private uint _spoofThreshold;
        private bool _spoofThresholdHighlySecured;
        private bool _spoofThresholdSecured;
        private bool _matchThresholdConvenient;
        private bool _matchThresholdHighlySecured;
        private bool _matchThresholdSecured;
        private bool _spoofThresholdConvenient;
        private bool _bDevicePresent;
        private bool _bNistQuality;
        private bool _bCancelLiveMode;
        private bool _bLiveModeinProcess;
        private string _sEnrollSubjId;
        private bool _newSubjId;
        public bool _bComTimeOut;
        private bool _bLatentDetected;
        private SubjectData _currentSubject;
        private int _currentHotSpot;
        private string _sVerifySubjId;
        private readonly string[] _fingersNames = {
            "Meñique Izquierdo",
            "Anular Izquierdo",
            "Corazón Izquierdo",
            "Índice Izquierdo",
            "Pulgar Izquierdo",
            "Pulgar Derecho",
            "Índice Derecho",
            "Corazón Derecho",
            "Anular Derecho",
            "Meñique Derecho"
        } ;
        Image _defaultImage, _gImage;

        LumiSdkWrapper.LumiPreviewCallbackDelegate _delLiveMode;

        private byte[] _bRawImageBuffer;
        private uint _nWidth;
        private uint _nHeight;
        private byte[] _bTemplateBuffer;

        private int _nEnrollmentCaptureIndex = -1;

        int _prevclosePointIndex;
        bool _draw = true;

        public MainForm()
        {
            InitializeComponent();
            _conn = new Connection();
            _fingersEnrolled = new bool[10];
            _fingerprintsPath = "Temp\\";
            _numberDpi = 500;
            _numberUmbral = 28;

            _sensorList = new ArrayList();
            _bClosingApp = false;
            _matchThreshold = 0;
            _spoofThreshold = 0;
            _spoofThresholdHighlySecured = false;
            _spoofThresholdSecured = false;
            _spoofThresholdConvenient = false;
            _matchThresholdHighlySecured = false;
            _matchThresholdSecured = false;
            _matchThresholdConvenient = false;
            labelStatus.Text = "";
            labelStatus2.Text = "";
            labelStatus2.Text = "";
            NISTScoreLabel.Text = "";
            _umbral.Text = _numberUmbral.ToString();
            _bPdCaptureInProcess = false;
            _bCancelCapture = false;
            _bCancelLiveMode = false;
            _bLiveModeinProcess = false;
            _bDevicePresent = false;
            _captureThreadFinished = CaptureFinished;
            _verifyThreadFinished = VerifyFinished;
            _enrollThreadFinished = EnrollFinished;
            _currentSubject = new SubjectData();
            _newSubjId = true;
            _sVerifySubjId = "";
            _bComTimeOut = false;
            _bLatentDetected = false;
            _config = new Config();
            _serializer = new XmlSerializer(typeof(Config));
            
            _delegateErrorMessage = SetText;
            _delegateCaptureCancelled = CaptureCancelled;
            _delegateCaptureTimeOut = CaptureTimeOut;
            _delegateComTimeOut = ComTimeOut;
            _delegateLatentDetected = LatentDetected;
            _delegateNistStatus = SetNistStatusText;
            _delegatePreviewLiveMode = PreviewCallback;
            _delegateSetNistImage = SetNistStatusImage;
            OperatingSystem os = Environment.OSVersion;

            CreateAppDataFolder(os);
            _defaultImage = pictureBox1.Image;
    
            if (GetSensorList() == false)
            {
                DisableControls();
            }
            else
            {
                _bDevicePresent = true;
                PopulateSensorComboBox();
                SetMatchAndSpoofThresholds();
            }

            _delLiveMode = PreviewCallback;
        }

        ~MainForm()
        {
            GC.KeepAlive(_delLiveMode);
        }

        private void EnableControls()
        {

            CaptureBtnClick.Enabled = true;
            if (_bPdCaptureInProcess)
            {
                CaptureBtnClick.Text = "Capture";
                _bPdCaptureInProcess = false;

            }
            LiveBtn.Enabled = true;
            btnSaveImage.Enabled = true;
            SelectSensorComboBox.Enabled = true;
            tabControl.Enabled = true;
        }

        private void EnableControlsForLiveMode()
        {
            LiveBtn.Text = "Live Mode";
            LumiPictureBox1.Image = null;
            SelectSensorComboBox.Enabled = true;
            tabControl.Enabled = true;
            CaptureBtnClick.Enabled = true;
            btnSaveImage.Enabled = true;
        }

        private void CaptureFinished()
        {
            EnableControls();
        }

        private void VerifyFinished()
        {
            if(_bComTimeOut==false)
                EnableVerifyControls();
        }

        private void CaptureCancelled()
        {
            _bCancelCapture = true;
        }

        private void CaptureTimeOut()
        {
            LumiPictureBox1.Image = null;
            EnableControls();
        }

        private void ComTimeOut()
        {
            LumiPictureBox1.Image = null;
            DisableControls();
        }

        private void LatentDetected()
        {
            LumiPictureBox1.Image = null;
            DisableControls();
        }

        private void DisableControls()
        {
            DisableControls(false);
        }

        private void DisableControls(bool bCaptureWithPd)
        {
            if (_bDevicePresent == false)
            {
                LiveBtn.Enabled = false;
                SelectSensorComboBox.Enabled = false;
                CaptureBtnClick.Enabled = false;
                SensorTriggerArmedChkBox.Enabled = false;
                EnableSpoofDetChkBox.Enabled = false;
                tabControl.Enabled = false;
                LumiPictureBox1.Enabled = false;
                btnSaveImage.Enabled = false;
            }
            else
            {
                LumiPictureBox1.Image = null;
                if (bCaptureWithPd)
                {
                    _bPdCaptureInProcess = true;
                    CaptureBtnClick.Text = "Cancel";
                }
                else
                {
                    CaptureBtnClick.Enabled = false;
                }
                LiveBtn.Enabled = false;
                btnSaveImage.Enabled = false;
                SelectSensorComboBox.Enabled = false;
                tabControl.Enabled = false;
            }
        }

        private void DisableControlsForLiveMode()
        {
            if (_bDevicePresent == false)
            {
                LiveBtn.Enabled = false;
                SelectSensorComboBox.Enabled = false;
                CaptureBtnClick.Enabled = false;
                SensorTriggerArmedChkBox.Enabled = false;
                EnableSpoofDetChkBox.Enabled = false;
                tabControl.Enabled = false;
                LumiPictureBox1.Enabled = false;
            }
            else
            {
                LiveBtn.Text = "Stop Live Mode";
                CaptureBtnClick.Enabled = false;
                btnSaveImage.Enabled = false;
                SelectSensorComboBox.Enabled = false;
                tabControl.Enabled = false;
            }
        }

        private bool GetSensorList()
        {
            LumiSdkWrapper.LumiStatus rc = LumiSdkWrapper.LumiStatus.LumiStatusOk;

            LumiSdkWrapper.LumiDevice dev = new LumiSdkWrapper.LumiDevice();

            uint nNumDevices = 0;
            uint handle = 0;
            StringBuilder devList = null;

            rc = LumiSdkWrapper.LumiQueryNumberDevices(ref nNumDevices, devList);

            if (rc != LumiSdkWrapper.LumiStatus.LumiStatusOk)
            {
                SetText("Installation error please reinstall\n", _red);
                return false;
            }

            if (nNumDevices < 1)
            {
                SetText("No compatible Lumidigm sensors found. \nPlease connect a sensor and \nrestart the SDK Enroll App", _red);
                return false;
            }
            uint nFailedDevices = 0;

            for (uint ii = 0; ii < nNumDevices; ii++)
            {
                rc = LumiSdkWrapper.LumiQueryDevice(ii, ref dev);
                if (rc != LumiSdkWrapper.LumiStatus.LumiStatusOk) return false;

                rc = LumiSdkWrapper.LumiInit(dev.hDevHandle, ref handle);
                if (rc != LumiSdkWrapper.LumiStatus.LumiStatusOk)
                {
                    SetText("A device could not be initialized, it may be busy in another application. \n",_red);
                    nFailedDevices++;
                }
                else
                {
                    Sensor sensorid = new Sensor();
                    sensorid.Handle = handle;
                    sensorid.SensorType = dev.SensorType;
                    sensorid.StrIdentifier = dev.strIdentifier;
                    _sensorList.Add(sensorid);
                }

            }
            if (nFailedDevices == nNumDevices)
            {
                SetText("All devices could not be initialized, they may be busy in\n another application. Please close all other\n applications and restart the Enroll App", _red);
                return false;
            }
            return true;
        }

        private void SetNistStatusText(string text, Color txtColor)
        {
            if (_bClosingApp) return;
            if (NISTScoreLabel.InvokeRequired)
            {
                SetTextCallback d = SetNistStatusText;
                Invoke(d, text, txtColor);
            }
            else
            {
                NISTScoreLabel.Text = text;
                NISTScoreLabel.ForeColor = txtColor;
            }
        }

        private void SetNistStatusImage(uint nistScore)
        {
            nistPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            if(nistScore == 1)
            {
                nistPictureBox.Image = Resources.NIST_quality_1;
            }
            else if(nistScore == 2)
            {
                nistPictureBox.Image = Resources.NIST_quality_2;
            }
            else if(nistScore == 3)
            {
                nistPictureBox.Image = Resources.NIST_quality_3;
            }
            else if (nistScore == 4)
            {
                nistPictureBox.Image = Resources.NIST_quality_4;
            }
            else
            {
                nistPictureBox.Image = Resources.NIST_quality_5;
            }
        }

        private void SetText(string text, Color txtColor)
        {
            if (_bClosingApp) return;
            if (labelStatus.InvokeRequired)
            {
                SetTextCallback d = SetText;
                Invoke(d, text, txtColor);
            }
            else
            {
                labelStatus.Text = text;
                labelStatus2.Text = text;
                labelStatus.ForeColor = txtColor;
                labelStatus2.ForeColor = txtColor;
            }
        }

        // Callback function for Preview
        private void PreviewCallback(IntPtr pOutputImage, int width, int height, int imgNum)
        {
            if (_nEnrollmentCaptureIndex != -1) return;
            try
            {
                int nSize = width * height * 3;//actually a color image
                byte[] image;
                image = new byte[nSize];
                byte[] tmpImg = new byte[1]; // temp image to get to build for now
                Marshal.Copy(pOutputImage, image, 0, nSize);
                SetImage(ref tmpImg, ref image, (uint)width, (uint)height, null, 0, -1);

                image = null;
            }
            catch (Exception errorPrev)
            {
                MessageBox.Show(errorPrev.Message);
            }
        }

        private bool CreateAppDataFolder(OperatingSystem os)
        {
            if (os == null)
                return false;

            Version vs = os.Version;
            bool isExists;

            if (os.Platform == PlatformID.Win32NT)
            {
                switch (vs.Major)
                {
                
                    case 5:
                    {
                        // if XP
                        _sFolderPath = "..\\bin\\AppData";
                        string curDir = Directory.GetCurrentDirectory();
                        _debugFolder = curDir + "\\AppData\\Debug";

                        isExists = Directory.Exists(_sFolderPath);
                        if (!isExists)
                        {
                            Directory.CreateDirectory(_sFolderPath);
                            Directory.CreateDirectory(_sFolderPath + "\\Database");
                            Directory.CreateDirectory(_sFolderPath + "\\Debug");
                        }
                        else
                        {                            
                            if(!Directory.Exists(_debugFolder))
                                Directory.CreateDirectory(_debugFolder);

                        }

                        if (!File.Exists(@"..\\bin\\AppData\\Settings.xml"))
                        {
                            _config.SpoofEnabled = false;
                            EnableSpoofDetChkBox.Checked = false;
                            _bSpoofEnabled = false;

                            _config.SensorTriggerArmed = true;
                            SensorTriggerArmedChkBox.Checked = true;
                            _bSensorTriggerArmed = true;

                            _config.NistQuality = true;
                            NISTQualityChkBox.Checked = true;
                            _bNistQuality = true;

                            _config.SpoofSecure = true;
                            SpoofSecure.Checked = true;
                            _spoofThresholdSecured = true;

                            _config.SpoofHighlySecured = false;
                            SpoofHighlySecure.Checked = false;
                            _spoofThresholdHighlySecured = false;

                            _config.SpoofConvenient = false;
                            SpoofConvenient.Checked = false;
                            _spoofThresholdConvenient = false;

                            _config.MatchSecure = true;
                            MatchSecure.Checked = true;
                            _matchThresholdSecured = true;

                            _config.MatchHighlySecure = false;
                            MatchHighlySecured.Checked = false;
                            _matchThresholdHighlySecured = false;

                            _config.MatchConvenient = false;
                            MatchConvenient.Checked = false;
                            _matchThresholdConvenient = false;

                            UpdateConfigXml();

                        }
                        else
                        {

                            ReadConfigXml();

                            EnableSpoofDetChkBox.Checked = _config.SpoofEnabled;
                            _bSpoofEnabled = _config.SpoofEnabled;

                            SensorTriggerArmedChkBox.Checked = _config.SensorTriggerArmed;
                            _bSensorTriggerArmed = _config.SensorTriggerArmed;

                            NISTQualityChkBox.Checked = _config.NistQuality;
                            _bNistQuality = _config.NistQuality;

                            SpoofSecure.Checked = _config.SpoofSecure;
                            _spoofThresholdSecured = _config.SpoofSecure;

                            SpoofHighlySecure.Checked = _config.SpoofHighlySecured;
                            _spoofThresholdHighlySecured = _config.SpoofHighlySecured;

                            SpoofConvenient.Checked = _config.SpoofConvenient;
                            _spoofThresholdConvenient = _config.SpoofConvenient;

                            MatchSecure.Checked = _config.MatchSecure;
                            _matchThresholdSecured = _config.MatchSecure;

                            MatchHighlySecured.Checked = _config.MatchHighlySecure;
                            _matchThresholdHighlySecured = _config.MatchHighlySecure;

                            MatchConvenient.Checked = _config.MatchConvenient;
                            _matchThresholdConvenient = _config.MatchConvenient;

                        }
                    }break;
                case 6:  // if Win7
                    {
                        _sFolderPath = "C:\\ProgramData\\Lumidigm\\SDKEnrollExample\\AppData";
                        _debugFolder = _sFolderPath + "\\Debug";                     
                        isExists = Directory.Exists(_sFolderPath);
                        if (vs.Minor != 0)
                        {
                            if (!isExists)
                            {
                                Directory.CreateDirectory(_sFolderPath);
                                Directory.CreateDirectory(_sFolderPath + "\\Database");
                                Directory.CreateDirectory(_sFolderPath + "\\Debug");
                            }
                            else
                            {
                                
                                if (!Directory.Exists(_debugFolder))
                                    Directory.CreateDirectory(_sFolderPath + "\\Debug");

                            }
                            if (!File.Exists(@"C:\\ProgramData\\Lumidigm\\SDKEnrollExample\\AppData\\Settings.xml"))
                            {
                                _config.SpoofEnabled = false;
                                EnableSpoofDetChkBox.Checked = false;
                                _bSpoofEnabled = false;

                                _config.SensorTriggerArmed = true;
                                SensorTriggerArmedChkBox.Checked = true;
                                _bSensorTriggerArmed = true;

                                _config.NistQuality = true;
                                NISTQualityChkBox.Checked = true;
                                _bNistQuality = true;

                                _config.SpoofSecure = true;
                                SpoofSecure.Checked = true;
                                _spoofThresholdSecured = true;

                                _config.SpoofHighlySecured = false;
                                SpoofHighlySecure.Checked = false;
                                _spoofThresholdHighlySecured = false;

                                _config.SpoofConvenient = false;
                                SpoofConvenient.Checked = false;
                                _spoofThresholdConvenient = false;

                                _config.MatchSecure = true;
                                MatchSecure.Checked = true;
                                _matchThresholdSecured = true;

                                _config.MatchHighlySecure = false;
                                MatchHighlySecured.Checked = false;
                                _matchThresholdHighlySecured = false;

                                _config.MatchConvenient = false;
                                MatchConvenient.Checked = false;
                                _matchThresholdConvenient = false;

                                UpdateConfigXml();
                            }
                            else
                            {

                                ReadConfigXml();

                                EnableSpoofDetChkBox.Checked = _config.SpoofEnabled;
                                _bSpoofEnabled = _config.SpoofEnabled;

                                SensorTriggerArmedChkBox.Checked = _config.SensorTriggerArmed;
                                _bSensorTriggerArmed = _config.SensorTriggerArmed;

                                NISTQualityChkBox.Checked = _config.NistQuality;
                                _bNistQuality = _config.NistQuality;

                                SpoofSecure.Checked = _config.SpoofSecure;
                                _spoofThresholdSecured = _config.SpoofSecure;

                                SpoofHighlySecure.Checked = _config.SpoofHighlySecured;
                                _spoofThresholdHighlySecured = _config.SpoofHighlySecured;

                                SpoofConvenient.Checked = _config.SpoofConvenient;
                                _spoofThresholdConvenient = _config.SpoofConvenient;

                                MatchSecure.Checked = _config.MatchSecure;
                                _matchThresholdSecured = _config.MatchSecure;

                                MatchHighlySecured.Checked = _config.MatchHighlySecure;
                                _matchThresholdHighlySecured = _config.MatchHighlySecure;

                                MatchConvenient.Checked = _config.MatchConvenient;
                                _matchThresholdConvenient = _config.MatchConvenient;
                            }
                        }
                    } break;
                    default:
                        MessageBox.Show(@"This Operating System isn't Supported");
                        return false;
                }
            }

            _debugFolder = _debugFolder.Replace("\\", "/");
            _debugFolder = _debugFolder + "/";

            return true;
        }

        private bool UpdateConfigXml()
        {
            string xmlFileLoc;
            if (Environment.OSVersion.Version.Major == 5) // XP
                xmlFileLoc = "..\\bin\\AppData\\Settings.xml";
            else
                xmlFileLoc = "C:\\ProgramData\\Lumidigm\\SDKEnrollExample\\AppData\\Settings.xml";

            TextWriter tw = new StreamWriter(xmlFileLoc);

            _serializer.Serialize(tw, _config);
            tw.Close();
            return true;
        }

        private bool ReadConfigXml()
        {
            string xmlFileLoc;
            if (Environment.OSVersion.Version.Major == 5) // XP
                xmlFileLoc = "..\\bin\\AppData\\Settings.xml";
            else
                xmlFileLoc = "C:\\ProgramData\\Lumidigm\\SDKEnrollExample\\AppData\\Settings.xml";

            TextReader tr = new StreamReader(xmlFileLoc);
            _config = (Config)_serializer.Deserialize(tr);
            tr.Close();
            return true;
        }

        public int AcquStatusCallback(LumiSdkWrapper.LumiAcqStatus status)
        {
            if (status == LumiSdkWrapper.LumiAcqStatus.LumiAcqFingerPresent) return 0;

            SetText("", Color.Blue);

            return -2;
        }

        public int PresenceDetectionCallback(IntPtr pImage, int width, int height, uint status)
        {
 
            if (_nEnrollmentCaptureIndex != -1) return 0;
            if (_bClosingApp)
            {
                return -2;
            }
            int nSize = width * height * 3; // 24 bpp format is returned from SDK
            byte[] pOutputImage = new byte[nSize];
            Sensor sensorid = (Sensor)_sensorList[(int)_nSelectedSensorId];
            LumiSdkWrapper.LumiConfig deviceConfig;
            deviceConfig.eTemplateType = 0;
            deviceConfig.eTransInfo = 0;
            deviceConfig.nTriggerTimeout = 0;
            LumiSdkWrapper.LumiGetConfig(sensorid.Handle, ref deviceConfig);

            if (sensorid.SensorType == LumiSdkWrapper.LumiSensorType.M32X)
            {
                Bitmap bitmap1 = Resources.CaptureInProgress_M320_resized;
                BitmapData bmpData = bitmap1.LockBits(new Rectangle(0, 0, bitmap1.Width, bitmap1.Height), ImageLockMode.WriteOnly, bitmap1.PixelFormat);
                Marshal.Copy(bmpData.Scan0, pOutputImage, 0, nSize);
            }
            else
            {
                Marshal.Copy(pImage, pOutputImage, 0, nSize);
            }

            SetImage(ref pOutputImage, (uint)width, (uint)height, (int)status, null, 0);

            if (_bCancelCapture)
            {
                _bCancelCapture = false;
                LumiPictureBox1.Image = null;
                return -2;   // Return -2 to cancel the capture             
            }

            return 0;
        }

        // Overloaded Method to set the image buffer into the Picture box control
        private void SetImage(ref byte[] snapShotRaw, ref byte[] image, uint width, uint height, byte[] pTemplate, uint nTempSz, int nEnrollmentCaptureIndex)
        {
            //if (width == 280)
            //{
            //    int breakme = 2;
            //}

            //// If M320, display composite image between 2ed and 3ed enrollment capture - do not show white PD image.
            //if (nEnrollmentCaptureIndex > 1 && width != 280)
            //{
            //    return;
            //}

            _nEnrollmentCaptureIndex = nEnrollmentCaptureIndex;

            SetImage(ref image, width, height, -1, pTemplate, nTempSz);
            SetImage(ref image, width, height, -1, pTemplate, nTempSz, nEnrollmentCaptureIndex);
            _bRawImageBuffer = new byte[snapShotRaw.Length];
            snapShotRaw.CopyTo(_bRawImageBuffer, 0);

            if (pTemplate != null)
            {
                _bTemplateBuffer = new byte[pTemplate.Length];
                pTemplate.CopyTo(_bTemplateBuffer, 0);
            }
            _nWidth = width;
            _nHeight = height;
            //Marshal.Copy(snapShotRaw, 0, _bRawImageBuffer[0], snapShotRaw.Length);
        }

        // Overloaded Method to set the image buffer into the Picture box control
        private void SetImage(ref byte[] image, uint width, uint height, int pdStatus, byte[] pTemplate, uint nTempSz)
        {
            Bitmap bmp = new Bitmap((int)width, (int)height, PixelFormat.Format24bppRgb);

            BitmapData bmpData = bmp.LockBits(
                                 new Rectangle(0, 0, bmp.Width, bmp.Height),
                                 ImageLockMode.WriteOnly, bmp.PixelFormat);           

            Marshal.Copy(image, 0, bmpData.Scan0, image.Length);
            
            if (pTemplate != null)
            {
                bmp.UnlockBits(bmpData);
                DrawMinutiae(ref bmp, ref pTemplate, nTempSz);
                bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                                 ImageLockMode.WriteOnly, bmp.PixelFormat);
            }
             
            // Get aspect ratios
            float bmpAspectRatio = width / (float)height;
            float imgControlAspectRatio = LumiPictureBox1.Width / (float)LumiPictureBox1.Height;

            // Correct for aspect ration differnece between img control and bmp
            int widthDisplay = 0, heightDisplay = 0;
            if (Math.Abs((bmpAspectRatio - imgControlAspectRatio) / bmpAspectRatio) > .07)
            {
                if (bmpAspectRatio < 1)
                {
                    widthDisplay = LumiPictureBox1.Width;
                    heightDisplay = (int)(LumiPictureBox1.Width / bmpAspectRatio);
                }
            }
            else
            {
                widthDisplay = LumiPictureBox1.Width;
                heightDisplay = LumiPictureBox1.Height;
            }

            bmp.UnlockBits(bmpData);
            // Resize to composite image size and draw arrows 
            LumiPictureBox1.Image = ResizeBitmapAndDrawArrows(bmp, widthDisplay, heightDisplay, pdStatus);            
        }
        
        private void SetImage(ref byte[] image, uint width, uint height, int pdStatus, byte[] pTemplate, uint nTempSz, int index)
        {
            const int printHeight = 150;
            const int printWidth = 102;

            var bmp = new Bitmap((int)width, (int)height, PixelFormat.Format24bppRgb);

            var bmpData = bmp.LockBits(
                                 new Rectangle(0, 0, bmp.Width, bmp.Height),
                                 ImageLockMode.WriteOnly, bmp.PixelFormat);

            Marshal.Copy(image, 0, bmpData.Scan0, image.Length);
            
            if (pTemplate != null)
            {
                bmp.UnlockBits(bmpData);
                bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                                 ImageLockMode.WriteOnly, bmp.PixelFormat);
            }
             
            bmp.UnlockBits(bmpData);
            bmp.SetResolution(500, 500);

            // Resize to composite image size and draw arrows
            switch (index)
            {
                case 0:
                    _print1.Image = ResizeBitmap(bmp, printWidth, printHeight, pdStatus);
                    
                    bmp.Save(Path.Combine(_fingerprintsPath, _currentFinger + "_1.bmp"), ImageFormat.Bmp);
                    break;
                case 1:
                    _print2.Image = ResizeBitmap(bmp, printWidth, printHeight, pdStatus);
                    bmp.Save(Path.Combine(_fingerprintsPath, _currentFinger + "_2.bmp"), ImageFormat.Bmp);
                    break;
                case 2:
                    _print3.Image = ResizeBitmap(bmp, printWidth, printHeight, pdStatus);
                    bmp.Save(Path.Combine(_fingerprintsPath, _currentFinger + "_3.bmp"), ImageFormat.Bmp);
                    break;
                case -1:
                    bmp.Save("CurrentMatch.bmp", ImageFormat.Bmp);
                    ValidateMatch();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void CalculateNistScore(byte[] pImage, uint nWidth, uint nHeight, uint nBpp, uint nDpi, ref uint nNfiq)
        {
            //uint width = 0, height = 0;
            //uint BPP = 0, DPI = 0, NFIQ = 0;
            byte[] nistImage = new byte[5000]; //array to hold the template

        }

        private Bitmap ResizeBitmapAndDrawArrows(Bitmap image, int nWidth, int nHeight, int pdStatus)
        {
            Bitmap result = new Bitmap(nWidth, nHeight);
            using (Graphics graphics = Graphics.FromImage(result))
            {
                graphics.DrawImage(image, 0, 0, nWidth, nHeight);
                //DrawArrow(graphics, pdStatus);
            }
            return result;
        }
        
        // Resize bitmap image
        private Bitmap ResizeBitmap(Bitmap image, int nWidth, int nHeight, int pdStatus)
        {
            Bitmap result = new Bitmap(nWidth, nHeight);
            using (Graphics graphics = Graphics.FromImage(result))
            {
                graphics.DrawImage(image, 0, 0, nWidth, nHeight);
                //DrawArrow(graphics, pdStatus);
            }
            return result;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _bClosingApp = true;
            if (_captureWithPdThread != null)
            {
                _captureWithPdThread.Join(1000);
            }
            if (_enrollThread != null)
            {
                _enrollThread.Join(1000);
            }
            else
            {
                if (_bDevicePresent)
                    DeviceClose();
            }
        }

        private void DeviceClose()
        {
            Sensor currentSensor = (Sensor)_sensorList[(int)_nSelectedSensorId];
            LumiSdkWrapper.LumiStatus rc;

            try
            {
                rc = LumiSdkWrapper.LumiClose(currentSensor.Handle);
                rc = LumiSdkWrapper.LumiExit();
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        private void SetMatchAndSpoofThresholds()
        {
            if (_sensorList.Count == 0)
                return;
            Sensor currentSensor = (Sensor)_sensorList[(int)_nSelectedSensorId];
            LumiSdkWrapper.LumiStatus rc;
            LumiSdkWrapper.LumiVersion versInfo = new LumiSdkWrapper.LumiVersion();

            switch (currentSensor.SensorType)
            {
                case LumiSdkWrapper.LumiSensorType.Venus:
                    {
                        uint handle = 0;
                        handle = currentSensor.Handle;
                        rc = LumiSdkWrapper.LumiGetVersionInfo(handle, ref versInfo);
                        if (rc != LumiSdkWrapper.LumiStatus.LumiStatusOk)
                            return;
                        uint version = Convert.ToUInt32(versInfo.fwrVersion);
                        uint deviceType = Convert.ToUInt32(versInfo.tnsVersion);

                        if (version > 21304)
                        {
                            if (_matchThresholdHighlySecured)
                                _matchThreshold = 27532;
                            else if (_matchThresholdSecured)
                                _matchThreshold = 23688;
                            else
                                _matchThreshold = 20646;

                            if (_spoofThresholdHighlySecured)
                                _spoofThreshold = 5;
                            else if (_spoofThresholdSecured)
                                _spoofThreshold = 150;
                            else
                                _spoofThreshold = 1050;                            
                        }
                        else if (version == 21304)
                        {
                            if (_matchThresholdHighlySecured)
                                _matchThreshold = 24298;
                            else if (_matchThresholdSecured)
                                _matchThreshold = 22418;
                            else
                                _matchThreshold = 21548;

                            if (_spoofThresholdHighlySecured)
                                _spoofThreshold = 5;
                            else if (_spoofThresholdSecured)
                                _spoofThreshold = 150;
                            else
                                _spoofThreshold = 1050;    
                        }
                        else if (version <= 9538)
                        {
                            if (deviceType == 61)
                            {
                                if (_matchThresholdHighlySecured)
                                    _matchThreshold = 24298;
                                else if (_matchThresholdSecured)
                                    _matchThreshold = 22418;
                                else
                                    _matchThreshold = 21548;

                                if (_spoofThresholdHighlySecured)
                                    _spoofThreshold = 100;
                                else if (_spoofThresholdSecured)
                                    _spoofThreshold = 200;
                                else
                                    _spoofThreshold = 1000;   

                            }
                            else
                            {
                                if (_matchThresholdHighlySecured)
                                    _matchThreshold = 24298;
                                else if (_matchThresholdSecured)
                                    _matchThreshold = 22418;
                                else
                                    _matchThreshold = 21548;

                                if (_spoofThresholdHighlySecured)
                                    _spoofThreshold = 100;
                                else if (_spoofThresholdSecured)
                                    _spoofThreshold = 200;
                                else
                                    _spoofThreshold = 1000;   
                            }

                        }

                    } break;
                case LumiSdkWrapper.LumiSensorType.V31X:
                case LumiSdkWrapper.LumiSensorType.V371:
                    {
                        if (_matchThresholdHighlySecured)
                            _matchThreshold = 24739;
                        else if (_matchThresholdSecured)
                            _matchThreshold = 21493;
                        else
                            _matchThreshold = 16873;

                        if (_spoofThresholdHighlySecured)
                            _spoofThreshold = 5;
                        else if (_spoofThresholdSecured)
                            _spoofThreshold = 150;
                        else
                            _spoofThreshold = 1050;   

                    } break;
                case LumiSdkWrapper.LumiSensorType.M300:
                    {
                        if (_matchThresholdHighlySecured)
                            _matchThreshold = 29990;
                        else if (_matchThresholdSecured)
                            _matchThreshold = 27520;
                        else
                            _matchThreshold = 26350;

                        if (_spoofThresholdHighlySecured)
                            _spoofThreshold = 100;
                        else if (_spoofThresholdSecured)
                            _spoofThreshold = 200;
                        else
                            _spoofThreshold = 1000;   

                    } break;
                case LumiSdkWrapper.LumiSensorType.M100:
                    {
                        if (_matchThresholdHighlySecured)
                            _matchThreshold = 22418;
                        else if (_matchThresholdSecured)
                            _matchThreshold = 22000;
                        else
                            _matchThreshold = 15000;

                        if (_spoofThresholdHighlySecured)
                            _spoofThreshold = 100;
                        else if (_spoofThresholdSecured)
                            _spoofThreshold = 200;
                        else
                            _spoofThreshold = 1000;   

                    } break;
                case LumiSdkWrapper.LumiSensorType.M32X:
                    {
                        if (_matchThresholdHighlySecured)
                            _matchThreshold = 30762;
                        else if (_matchThresholdSecured)
                            _matchThreshold = 26368;
                        else
                            _matchThreshold = 25331;

                        if (_spoofThresholdHighlySecured)
                            _spoofThreshold = 100;
                        else if (_spoofThresholdSecured)
                            _spoofThreshold = 200;
                        else
                            _spoofThreshold = 1000;   

                    } break;
                case LumiSdkWrapper.LumiSensorType.M31X:
                    {
                        if (_matchThresholdHighlySecured)
                            _matchThreshold = 31448;
                        else if (_matchThresholdSecured)
                            _matchThreshold = 26728;
                        else
                            _matchThreshold = 25668;

                        if (_spoofThresholdHighlySecured)
                            _spoofThreshold = 100;
                        else if (_spoofThresholdSecured)
                            _spoofThreshold = 200;
                        else
                            _spoofThreshold = 1000;   

                    } break;
                default:
                    {
                    } break;
            }
        }

        private void PopulateSensorComboBox()
        {
            foreach (Sensor sensor in _sensorList)
            {
                SelectSensorComboBox.Items.Add(sensor.SensorType + " " + sensor.StrIdentifier);
            }
            SelectSensorComboBox.SelectedIndex = 0;
            _nSelectedSensorId = 0;

            Sensor currentSensor = (Sensor)_sensorList[(int)_nSelectedSensorId];
            string serialNum = "";
            GetSensorSerialNumber(ref serialNum);
            SetText("You have selected Sensor: " + currentSensor.SensorType + " SN " + serialNum, _blue);

            LumiSdkWrapper.LumiStatus rc = LumiSdkWrapper.LumiSetDCOptions(currentSensor.Handle, _debugFolder, 0);
        }

        private void GetSensorSerialNumber(ref string serialNum)
        {
            Sensor currentSensor = (Sensor)_sensorList[(int)_nSelectedSensorId];
            switch (currentSensor.SensorType)
            {
                case LumiSdkWrapper.LumiSensorType.Venus:
                    {
                        serialNum = currentSensor.StrIdentifier.Substring(5);

                    } break;
                case LumiSdkWrapper.LumiSensorType.V31X:
                    {
                        serialNum = currentSensor.StrIdentifier.Substring(2);

                    } break;
                case LumiSdkWrapper.LumiSensorType.M300:
                    {
                        serialNum = currentSensor.StrIdentifier.Substring(4);

                    } break;
                case LumiSdkWrapper.LumiSensorType.M100:
                    {
                        serialNum = currentSensor.StrIdentifier.Substring(4);

                    } break;
                case LumiSdkWrapper.LumiSensorType.M31X:
                    {
                        serialNum = currentSensor.StrIdentifier.Substring(2);

                    } break;
                case LumiSdkWrapper.LumiSensorType.M32X:
                    {
                        serialNum = currentSensor.StrIdentifier.Substring(2);

                    } break;
                default:
                    {
                    } break;
            }
        }

        private void SelectSensorComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            _nSelectedSensorId = (uint)(SelectSensorComboBox.SelectedIndex);
            Sensor currentSensor = (Sensor)_sensorList[(int)_nSelectedSensorId];
            string serialNum = "";
            GetSensorSerialNumber(ref serialNum);
            SetText("You have selected Sensor: " + currentSensor.SensorType + " SN " + serialNum, _blue);
            SetMatchAndSpoofThresholds();

            LumiSdkWrapper.LumiStatus rc = LumiSdkWrapper.LumiSetDCOptions(currentSensor.Handle, _debugFolder, 0);
        }

        private void CaptureWithPdThreadFuction()
        {
            _nEnrollmentCaptureIndex = -1;
            CaptureWithPd captureWithPresDetect = new CaptureWithPd(this);
            captureWithPresDetect.Run();
        }

        private void EnrollThreadFuction()
        {
            _nEnrollmentCaptureIndex = -1;
            Enroll enrollWithPresDetect = new Enroll(this);
            enrollWithPresDetect.Run();
        }

        private void VerifyThreadFuction()
        {
            _nEnrollmentCaptureIndex = -1;
            Verify verifyWithPresDetect = new Verify(this);
            verifyWithPresDetect.Run();
        }

        private void CaptureBtnClick_Click(object sender, EventArgs e)
        {           
            _bCancelCapture = false;
            if (_bPdCaptureInProcess)
            {
                _bCancelCapture = true;
                EnableControls();
            }
            else
            {
                _bPdCaptureInProcess = true;
                SetText("Capture in Process", _blue);
                DisableControls(true);

                _captureWithPdThread = new Thread(CaptureWithPdThreadFuction);
                _captureWithPdThread.Name = "CaptureWithPDThread";
                _captureWithPdThread.Start();
            }
        }

        private void EnableSpoofDetChkBox_CheckedChanged(object sender, EventArgs e)
        {

            if (EnableSpoofDetChkBox.Checked == false)
            {
                _config.SpoofEnabled = false;
                _bSpoofEnabled = false;
                UpdateConfigXml();
            }
            else
            {
                _config.SpoofEnabled = true;
                _bSpoofEnabled = true;
                UpdateConfigXml();
            }
        }

        private void SensorTriggerArmedChkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (SensorTriggerArmedChkBox.Checked == false)
            {
                _bSensorTriggerArmed = false;
                _config.SensorTriggerArmed = false;
                UpdateConfigXml();
            }
            else
            {
                _bSensorTriggerArmed = true;
                _config.SensorTriggerArmed = true;
                UpdateConfigXml();
            }

        }

        private void NISTQualityChkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (NISTQualityChkBox.Checked == false)
            {
                _bNistQuality = false;
                _config.NistQuality = false;
                nistPictureBox.Image = null;
                UpdateConfigXml();

            }
            else
            {
                _bNistQuality = true;
                _config.NistQuality = true;
                UpdateConfigXml();
            }
        }

        private void LiveBtn_Click(object sender, EventArgs e)
        {
            NISTScoreLabel.Text = "";
            Sensor currentSensor = (Sensor)_sensorList[(int)_nSelectedSensorId];

            if (_bCancelLiveMode == false)
            {
                LumiSdkWrapper.LumiSetLiveMode(currentSensor.Handle, 1, _delLiveMode);

                _bCancelLiveMode = true;
                SetText("Live Mode in Process", _blue);
                DisableControlsForLiveMode();
            }
            else
            {
                LumiSdkWrapper.LumiSetLiveMode(currentSensor.Handle, 0, _delLiveMode);
                _bCancelLiveMode = false;
                SetText("Live Mode Stopped", _blue);
                EnableControlsForLiveMode();
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tabControl.SelectedIndex)
            {
                case 0: // Capture Tab
                {
                    SetText("", _blue);
                    LumiPictureBox1.Image = null;
                    CaptureBtnClick.Enabled = true;
                    LiveBtn.Enabled = true;
                    nistPictureBox.Image = null;

                } break;
                case 1: // ID Enroll Tab
                {
                    _document_type.SelectedItem = null;
                    _document.Text = string.Empty;
                    _name.Text = string.Empty;
                    _last_name.Text = string.Empty;
                    _birthdate.Value = DateTime.Now;
                    CalculateAndSetAgeEnroll();
                    CleanOrCreateDirectory(_fingerprintsPath);
                    SetText("Digite los datos para enrolamiento", _blue);
                    _fingers.Clear();
                    LumiPictureBox1.Image = null;
                    _currentSubject.Clear();
                    pictureBox1.Image = _defaultImage;
                    nistPictureBox.Image = null;
                    _bCancelCapture = false;
                    //DisableEnrollControls();
                } break;
                case 2:
                {
                    _document_type2.Text = string.Empty;
                    _document2.Text = string.Empty;
                    _name2.Text = string.Empty;
                    _last_name2.Text = string.Empty;
                    _birthdate2.Text = string.Empty;
                    _age2.Text = string.Empty;
                    CleanOrCreateDirectory(_fingerprintsPath);
                    SetText("Seleccione un usuario para verificación", _blue);
                    LumiPictureBox1.Image = null;
                    nistPictureBox.Image = null;
                    _bCancelCapture = false;
                    // DisableVerifyControls();   

                } break;
            }
        }

        private void DisableEnrollControls()
        {
            if (_bDevicePresent == false)
            {
                LiveBtn.Enabled = false;
                SelectSensorComboBox.Enabled = false;
                CaptureBtnClick.Enabled = false;
                tabControl.Enabled = false;
                LumiPictureBox1.Enabled = false;
                SetText("Device not found. Please connect device and restart the application", _red);
            }
            else
            {
                NISTScoreLabel.Text = "";
                LumiPictureBox1.Image = null;
                CaptureBtnClick.Enabled = false;
                LiveBtn.Enabled = false;
                btnSaveImage.Enabled = false;
                SelectSensorComboBox.Enabled = false;
                tabControl.Enabled = false;
            }
        }

        private void EnableEnrollControls()
        {
            //CaptureBtnClick.Text = "Capture";
            CaptureBtnClick.Enabled = true;
            LiveBtn.Enabled = true;
            btnSaveImage.Enabled = true;
            SelectSensorComboBox.Enabled = true;
            tabControl.Enabled = true;
        }

        private void EnableVerifyControls()
        {
           // CaptureBtnClick.Text = "Capture";
            CaptureBtnClick.Enabled = true;
            LiveBtn.Enabled = true;
            btnSaveImage.Enabled = true;
            SelectSensorComboBox.Enabled = true;
            tabControl.Enabled = true;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {            
            int closePointIndex = ClosesetHotSpot(e);
            int radius = 14;
            int offset = 4;
            Graphics g = pictureBox1.CreateGraphics();
            
            Pen pen = new Pen(Color.Tomato, 4);
            SolidBrush brush = new SolidBrush(Color.LawnGreen);            
            Font drawFont = new Font("Arial", 10, FontStyle.Bold);
            
            SolidBrush brush2 = new SolidBrush(Color .Red);       
            
            if (_prevclosePointIndex == closePointIndex)
            {
               if (_draw)
                {
                    _draw = false;
                    FingerPos cc = new FingerPos();
                    _fingers.Text = _fingersNames[closePointIndex];

                    g.DrawEllipse(pen, cc.Pos[closePointIndex].X - (radius - offset), cc.Pos[closePointIndex].Y - (radius - offset), radius, radius);
                    g.FillEllipse(brush, cc.Pos[closePointIndex].X - (radius - offset), cc.Pos[closePointIndex].Y - (radius - offset), radius, radius);

                    if (FingerExists(closePointIndex))
                        g.DrawString("X", drawFont, brush2, cc.Pos[closePointIndex].X - (radius - offset), cc.Pos[closePointIndex].Y - (radius - offset));
                }
            }
            else
            {
                pictureBox1.Invalidate();                
                _prevclosePointIndex = closePointIndex;
                _draw = true;
            }
        }

        private bool FingerExists(int closePointIndex)
        {
            foreach (int finger in _currentSubject.Fingers)
            {
                if (finger == closePointIndex)
                    return true;
            }
            return false;
        }

        private static int ClosesetHotSpot(MouseEventArgs e)
        {
            FingerPos cc = new FingerPos();
            double min = 20000;
            int closestPointIndex = 0;
            for (int i = 0; i < 10; i++)
            {
                double dist = (Math.Pow(e.X - cc.Pos[i].X, 2) + Math.Pow(e.Y - cc.Pos[i].Y, 2));
                if (dist < min)
                {
                    min = dist;
                    closestPointIndex = i;

                }
            }
            return closestPointIndex;
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            CleanFingersPictureBox();
            SetText("Enroll in Process", _blue);
            _currentFinger = Array.IndexOf(_fingersNames, _fingers.Text);
            DisableEnrollControls();
            _enrollThread = new Thread(EnrollThreadFuction) {Name = "enrollPDThread"};
            _enrollThread.Start();
        }

        private void CleanFingersPictureBox()
        {
            _print1.Image = null;
            _print2.Image = null;
            _print3.Image = null;
        }

        private void UpdatePictureBox1()
        {            
            _gImage = (Image)_defaultImage.Clone();
            var g = Graphics.FromImage(_gImage);
            var pen = new Pen(Color.Red);
            var brush = new SolidBrush(Color.FromArgb(170,255,0,0));

            var drawFont = new Font("Arial", 10, FontStyle.Bold);
            var brush3 = new SolidBrush(Color.White);

            var cc = new FingerPos();
            const int radius = 15;
            const int offset = 4;

            for (var i = 0; i < _fingersEnrolled.Length; i++)
            {
                if (!_fingersEnrolled[i]) continue;
                g.DrawEllipse(pen, cc.Pos[i].X - (radius - offset), cc.Pos[i].Y - (radius - offset), radius, radius);
                g.FillEllipse(brush, cc.Pos[i].X - (radius - offset), cc.Pos[i].Y - (radius - offset), radius, radius);
                g.DrawString("E", drawFont, brush3, cc.Pos[i].X - (radius - offset), cc.Pos[i].Y - (radius - offset));

            }
            pictureBox1.Image = _gImage;
        }
        
        private void EnrollFinished()
        {
            if (_bComTimeOut) return;
            EnableEnrollControls();
            _fingersEnrolled[_currentFinger] = true;
            //UpdateExistingUserComboBox();
            UpdatePictureBox1();
        }

        // Draw Minutae
        private static void DrawMinutiae(ref Bitmap bmp, ref byte[] template, uint templateSize)
        {
            try
            {
                ////// Get the minutia list
                Ansi378TemplateHelper templateHelper = new Ansi378TemplateHelper(template, (int)templateSize);
                Minutiae[] minutiaeList = templateHelper.GetMinutiaeList();
                ////// Draw minutia on capturedImage.Image
                Graphics g = Graphics.FromImage(bmp);// this.LumiPictureBox1.CreateGraphics();


                Pen pen2;
                SolidBrush brush;

                foreach (var minutiae in minutiaeList)
                {
                    if (minutiae.NType == 1)
                    {
                        // Line ending minutiae
                        pen2 = new Pen(Color.Red);
                        brush = new SolidBrush(Color.Red);
                    }
                    else
                    {
                        // Bifurcation minutiae
                        pen2 = new Pen(Color.Green);
                        brush = new SolidBrush(Color.Green);
                    }

                    int nX = minutiae.NX;
                    int nY = minutiae.NY;

                    g.DrawEllipse(pen2, nX - 3, nY - 3, 6, 6);
                    g.FillEllipse(brush, nX - 3, nY - 3, 6, 6);

                    double nR = minutiae.NRotAngle;
                    int nX1;
                    int nY1;

                    // Draw the quiver                
                    nX1 = (int)(nX + (15.0 * Math.Cos(nR)));
                    nY1 = (int)(nY - (15.0 * Math.Sin(nR)));
                    g.DrawLine(pen2, nX, nY, nX1, nY1);
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void btnSaveImage_Click_1(object sender, EventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "BMPfiles|*.bmp";
            {
                 LiveBtn.Enabled = false;
                 CaptureBtnClick.Enabled = false;
                 SelectSensorComboBox.Enabled = false;
                if (LumiPictureBox1.Image == null)
                    MessageBox.Show("Image box is empty.");
                else
                {
                    if (save.ShowDialog() == DialogResult.OK)
                    {
                        // Save image directly from the picture box, includig minutiae
                        Bitmap bm = new Bitmap(LumiPictureBox1.Image);
                        bm.Save(save.FileName, ImageFormat.Bmp);

                        // Save image full size without minutiae
                        Byte[] tmpImageBuffer = new Byte[_nWidth * _nHeight * 3];
                        int j = 0;
                        for (int i = 0; i < (_nWidth * _nHeight); i++)
                        {
                            tmpImageBuffer[j++] = _bRawImageBuffer[i];
                            tmpImageBuffer[j++] = _bRawImageBuffer[i];
                            tmpImageBuffer[j++] = _bRawImageBuffer[i];
                        }
                        String rawFileName = save.FileName.Replace(".bmp", "_NoMinutiae.bmp");
                        
                        Bitmap bmp = new Bitmap((int)_nWidth, (int)_nHeight, PixelFormat.Format24bppRgb);
                        BitmapData bmpData = bmp.LockBits(
                                             new Rectangle(0, 0, (int)_nWidth, (int)_nHeight),
                                             ImageLockMode.WriteOnly, bmp.PixelFormat);
                        Marshal.Copy(tmpImageBuffer, 0, bmpData.Scan0, tmpImageBuffer.Length);
                        bmp.Save(rawFileName, ImageFormat.Bmp);
                        
                        // Save raw byte data from image
                        String rawByteFileName = save.FileName.Replace(".bmp", ".bin");
                        BinaryWriter binaryWriter = new BinaryWriter(File.OpenWrite(rawByteFileName));
                        binaryWriter.Write(_bRawImageBuffer);
                        binaryWriter.Flush();
                        binaryWriter.Close();

                        // Save raw byte data from fingerprint template
                        String rawByteTmpName = save.FileName.Replace(".bmp", "_tmp.bin");
                        BinaryWriter binaryWriter2 = new BinaryWriter(File.OpenWrite(rawByteTmpName));
                        binaryWriter2.Write(_bTemplateBuffer);
                        binaryWriter2.Flush();
                        binaryWriter2.Close();
                    }

                }
            }
            LiveBtn.Enabled = true;
            CaptureBtnClick.Enabled = true;
            SelectSensorComboBox.Enabled = true;
        }

        private void SetConvenientSpoofThresholds()
        {

            Sensor currentSensor = (Sensor)_sensorList[(int)_nSelectedSensorId];
            LumiSdkWrapper.LumiStatus rc;
            LumiSdkWrapper.LumiVersion versInfo = new LumiSdkWrapper.LumiVersion();

            switch (currentSensor.SensorType)
            {
                case LumiSdkWrapper.LumiSensorType.Venus:
                    {
                        uint handle = 0;
                        handle = currentSensor.Handle;
                        rc = LumiSdkWrapper.LumiGetVersionInfo(handle, ref versInfo);
                        uint version = Convert.ToUInt32(versInfo.fwrVersion);
                        uint deviceType = Convert.ToUInt32(versInfo.tnsVersion);

                        if (version >= 21304)
                        {
                            _spoofThreshold = 1050;
                        }
                        else if (version <= 9538)
                        {
                            if (deviceType == 61)
                            {
                                _spoofThreshold = 1050;

                            }
                            else
                            {
                                _spoofThreshold = 1000;

                            }
                        }

                    } break;
                case LumiSdkWrapper.LumiSensorType.V31X:
                    {
                        _spoofThreshold = 1050;
                    } break;
                case LumiSdkWrapper.LumiSensorType.M300:
                    {
                        _spoofThreshold = 1000;
                    } break;
                case LumiSdkWrapper.LumiSensorType.M100:
                    {
                        _spoofThreshold = 1000;
                    } break;
                case LumiSdkWrapper.LumiSensorType.M31X:
                    {
                        _spoofThreshold = 1000;
                    } break;
                case LumiSdkWrapper.LumiSensorType.M32X:
                    {
                        _spoofThreshold = 1000;
                    } break;
                default:
                    {
                    } break;
            }
        }

        private void SetSecureMatchThresholds()
        {
            Sensor currentSensor = (Sensor)_sensorList[(int)_nSelectedSensorId];
            LumiSdkWrapper.LumiStatus rc;
            LumiSdkWrapper.LumiVersion versInfo = new LumiSdkWrapper.LumiVersion();

            switch (currentSensor.SensorType)
            {
                case LumiSdkWrapper.LumiSensorType.Venus:
                    {
                        uint handle = 0;
                        handle = currentSensor.Handle;
                        rc = LumiSdkWrapper.LumiGetVersionInfo(handle, ref versInfo);
                        uint version = Convert.ToUInt32(versInfo.fwrVersion);
                        uint deviceType = Convert.ToUInt32(versInfo.tnsVersion);

                        if (version >= 21304)
                        {
                            _matchThreshold = 22418;
                        }
                        else if (version <= 9538)
                        {
                            _matchThreshold = deviceType == 61 ? (uint) 27520 : (uint) 22418;
                        }

                    } break;
                case LumiSdkWrapper.LumiSensorType.V31X:
                    {
                        _matchThreshold = 22418;
                    } break;
                case LumiSdkWrapper.LumiSensorType.M300:
                    {
                        _matchThreshold = 27520;
                    } break;
                case LumiSdkWrapper.LumiSensorType.M100:
                    {
                        _matchThreshold = 27520;
                    } break;
                case LumiSdkWrapper.LumiSensorType.M31X:
                    {
                        _matchThreshold = 27520;
                    } break;
                case LumiSdkWrapper.LumiSensorType.M32X:
                    {
                        _matchThreshold = 27520;
                    } break;
                default:
                    {
                    } break;
            }
        }

        private void SpoofHighlySecure_CheckedChanged_1(object sender, EventArgs e)
        {           
           if (SpoofHighlySecure.Checked == false)
           {        
               _spoofThresholdHighlySecured = false;
               _config.SpoofHighlySecured = false;
               UpdateConfigXml();
           }
           else
           {            
               _spoofThresholdHighlySecured = true;
               _config.SpoofHighlySecured = true;
               SetMatchAndSpoofThresholds();
               UpdateConfigXml();
           }
        }

        private void SpoofSecure_CheckedChanged_1(object sender, EventArgs e)
        {            
            if (SpoofSecure.Checked == false)
            {         
                _spoofThresholdSecured = false;
                _config.SpoofSecure = false;
                UpdateConfigXml();
            }
            else
            {             
                _spoofThresholdSecured = true;
                _config.SpoofSecure = true;
                SetMatchAndSpoofThresholds();
                UpdateConfigXml();
            }
        }

        private void SpoofConvenient_CheckedChanged_1(object sender, EventArgs e)
        {           
            if (SpoofConvenient.Checked == false)
            {         
                _spoofThresholdConvenient = false;
                _config.SpoofConvenient = false;
                UpdateConfigXml();
            }
            else
            {             
                _spoofThresholdConvenient = true;
                _config.SpoofConvenient = true;
                SetMatchAndSpoofThresholds();
                UpdateConfigXml();
            }
        }

        private void MatchHighlySecure_CheckedChanged_1(object sender, EventArgs e)
        {
            if (MatchHighlySecured.Checked == false)
            {
                _matchThresholdHighlySecured = false;
                _config.MatchHighlySecure = false;
                UpdateConfigXml();
            }
            else
            {
                _matchThresholdHighlySecured = true;
                _config.MatchHighlySecure = true;
                SetMatchAndSpoofThresholds();
                UpdateConfigXml();
            }
        }

        private void MatchSecure_CheckedChanged_1(object sender, EventArgs e)
        {            
            if (MatchSecure.Checked == false)
            {         
                _matchThresholdSecured = false;
                _config.MatchSecure = false;
                UpdateConfigXml();
            }
            else
            {             
                _matchThresholdSecured = true;
                _config.MatchSecure = true;
                SetMatchAndSpoofThresholds();
                UpdateConfigXml();
            }
        }

        private void MatchConvenient_CheckedChanged_1(object sender, EventArgs e)
        {            
            if (MatchConvenient.Checked == false)
            {         
                _matchThresholdConvenient = false;
                _config.MatchConvenient = false;
                UpdateConfigXml();
            }
            else
            {             
                _matchThresholdConvenient = true;
                _config.MatchConvenient = true;
                SetMatchAndSpoofThresholds();
                UpdateConfigXml();
            }
        }

        private void _guardar_Click(object sender, EventArgs e)
        {
            if (_guardar.Text == @"Actualizar")
            {
                if (!ValidateEnrollTab(true)) return;
                UpdateUser();
            }
            else
            {
                if (!ValidateEnrollTab(false)) return;
                SaveUser();
            }
        }

        private void _delete_Click(object sender, EventArgs e)
        {
            DeleteUser();
        }

        private void _cancelar_Click(object sender, EventArgs e)
        {
            CleanEnrollTab();
        }

        private void _users_table_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (_users_table.CurrentRow == null) return;
            _currentId = (int)_users_table.CurrentRow.Cells[0].Value;
            _document_type.Text = _users_table.CurrentRow.Cells[1].Value.ToString();
            _document.Text = _users_table.CurrentRow.Cells[2].Value.ToString();
            _name.Text = _users_table.CurrentRow.Cells[3].Value.ToString();
            _last_name.Text = _users_table.CurrentRow.Cells[4].Value.ToString();
            _birthdate.Value = DateTime.Parse(_users_table.CurrentRow.Cells[5].Value.ToString());
            _guardar.Text = @"Actualizar";
            CopyFingerprintsImages(_document.Text);
            VerifyFingersEnrolled();
            UpdatePictureBox1();
        }

        private void VerifyFingersEnrolled()
        {
            var filesFind = new int[10];
            var files = Directory.GetFiles(_fingerprintsPath);
            foreach (var file in files)
            {
                int index;
                if (!int.TryParse(Path.GetFileName(file).Substring(0, 1), out index)) continue;
                if (index >= 0 && index <= 9)
                    filesFind[index]++;
            }

            for (var i = 0; i < 10; i++)
            {
                _fingersEnrolled[i] = false;
            }

            for (var i = 0; i < 10; i++)
            {
                if (filesFind[i] >= 3)
                {
                    _fingersEnrolled[i] = true;
                }
            }
        }
        
        private void FingersVefication()
        {
            var filesFind = new int[10];
            var files = Directory.GetFiles(_fingerprintsPath);
            foreach (var file in files)
            {
                int index;
                if (!int.TryParse(Path.GetFileName(file).Substring(0, 1), out index)) continue;
                if (index >= 0 && index <= 9)
                    filesFind[index]++;
            }

            for (var i = 0; i < 10; i++)
                _fingersEnrolled[i] = false;

            for (var i = 0; i < 10; i++)
            {
                if (filesFind[i] <= 0) continue;
                _fingersEnrolled[i] = true;
                _finger2.Items.Add(_fingersNames[i]);
                var match = Path.Combine(_fingerprintsPath, "Match_" + i + ".txt");
                if (!File.Exists(match)) continue;
                try
                {
                    var iterations = new string[] { "1-2", "2-3", "1-3" };
                    var sr = new StreamReader(match);
                    _log.AppendText("//////////////////////////////////////////////////////////");
                    _log.AppendText("\r\n"+ _fingersNames[i]);
                    _log.AppendText("\r\nIDKit:\r\n");
                    for (var j = 0; j < 3; j++)
                        _log.AppendText(iterations[j] + ": " + sr.ReadLine() + ", ");
                    _log.AppendText("\r\nIso:\r\n");
                    for (var j = 0; j < 3; j++)
                        _log.AppendText(iterations[j] + ": " + sr.ReadLine() + ", ");
                    _log.AppendText("\r\nAnsi:\r\n");
                    for (var j = 0; j < 3; j++)
                        _log.AppendText(iterations[j] + ": " + sr.ReadLine() + ", ");
                    _log.AppendText("\r\nScore:");
                    for (var j = 1; j <= 3; j++)
                        _log.AppendText("\r\nHuella " + j + ": " + sr.ReadLine() + ", ");
                    var line = sr.ReadLine();
                    _log.AppendText("\r\nDPI: " + line + "\r\n");
                    _numberDpi = Convert.ToInt32(line);
                    sr.Close();
                }
                catch (Exception e)
                {
                    MessageBox.Show(@"Exception: " + e.Message, @"Archivo", MessageBoxButtons.OK);
                }
            }
        }

        private void CopyFingerprintsImages(string directory)
        {
            directory = directory.Trim();
            CleanOrCreateDirectory(_fingerprintsPath);
            if (!Directory.Exists(directory)) return;
            var files = Directory.GetFiles(directory);
            foreach (var file in files)
                File.Copy(file, Path.Combine(_fingerprintsPath, Path.GetFileName(file)), true);
        }

        private void SaveUser()
        {
            if (!_conn.SaveUser(_document_type.Text, _document.Text, _name.Text, _last_name.Text, _birthdate.Text))
                MessageBox.Show(@"Error al intentar guardar", @"Guardar", MessageBoxButtons.OK);
            else
            {
                SaveFingerprintsImages(_document.Text);
                CalculateMatch(_document.Text);
                CleanEnrollTab();
                UpdateGridUsers();
            }
        }

        private void CalculateMatch(string directory)
        {
            var ofp = new OrionFingerprints();
            ofp.SetAnsiLibLicense(@"C:\Licencias\ISO&ANSI\isoansi.lic");
            ofp.SetIdkitLibLicense(@"C:\Licencias\IDKitPC\iengine.lic");
            ofp.SetIsegLibLicense(@"C:\Licencias\Segmentation\segmentation.lic");
            ofp.SetDpi(_numberDpi);
            ofp.Initialize();
            directory = directory.Trim();

            var filesFind = new int[10];
            var files = Directory.GetFiles(directory);
            var results = new double[4, 3];
            foreach (var file in files)
            {
                int index;
                if (!int.TryParse(Path.GetFileName(file).Substring(0, 1), out index)) continue;
                if (index >= 0 && index <= 9)
                    filesFind[index]++;
            }

            for (var i = 0; i < 10; i++)
            {
                if (filesFind[i] < 3) continue;
                var file1 = Path.Combine(directory, i + "_1.bmp");
                var file2 = Path.Combine(directory, i + "_2.bmp");
                var file3 = Path.Combine(directory, i + "_3.bmp");
                if (File.Exists(file1) && File.Exists(file2) && File.Exists(file3))
                {
                    ofp.SetImages(file1, file2);
                    results[0, 0] = ofp.MatchIdkit(file1, file1, file2, @"F:\iengine.db");
                    results[1, 0] = ofp.SingleSegmentateWithMatch();
                    results[2, 0] = ofp.SingleSegmentateWithMatch("ansi");
                    ofp.SetImages(file2, file3);
                    results[0, 1] = ofp.MatchIdkit(file2, file2, file3, @"F:\iengine.db");
                    results[1, 1] = ofp.SingleSegmentateWithMatch();
                    results[2, 1] = ofp.SingleSegmentateWithMatch("ansi");
                    ofp.SetImages(file1, file3);
                    results[0, 2] = ofp.MatchIdkit(file1, file1, file3, @"F:\iengine.db");
                    results[1, 2] = ofp.SingleSegmentateWithMatch();
                    results[2, 2] = ofp.SingleSegmentateWithMatch("ansi");

                    var higher =  new double[3];
                    for (var j = 0; j < 3; j++)
                    {
                        for (var k = 0; k < 3; k++)
                        {
                            if (results[k, j] > higher[j])
                                higher[j] = results[k, j];
                        }
                    }

                    results[3, 0] = (higher[0] + higher[2]) / 2;
                    results[3, 1] = (higher[1] + higher[2]) / 2;
                    results[3, 2] = (higher[1] + higher[2]) / 2;
                    
                    try
                    {
                        //Pass the filepath and filename to the StreamWriter Constructor
                        var sw = new StreamWriter(Path.Combine(directory, "Match_" + i + ".txt"));

                        for (var j = 0; j < 4; j++)
                        {
                            for (var k = 0; k < 3; k++)
                            {
                                sw.WriteLine(results[j,k]);
                            }
                        }
                        sw.WriteLine(_numberDpi);

                        sw.Close();
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(@"Exception: " + e.Message, @"Match", MessageBoxButtons.OK);
                    }
                }
                else
                {
                    MessageBox.Show(@"Error al intentar hacer match", @"Match", MessageBoxButtons.OK);
                }
            }
        }

        private void SaveFingerprintsImages(string directory)
        {
            directory = directory.Trim();
            CleanOrCreateDirectory(directory);
            var files = Directory.GetFiles(_fingerprintsPath);
            foreach (var file in files)
                File.Copy(file, Path.Combine(directory, Path.GetFileName(file)), true);
            CleanOrCreateDirectory(_fingerprintsPath);
        }

        private void UpdateUser()
        {
            if (!_conn.UpdateUser(_document_type.Text, _document.Text, _name.Text, _last_name.Text, _birthdate.Text,
                _currentId))
                MessageBox.Show(@"Error al intentar actualizar", @"Actualizar", MessageBoxButtons.OK);
            else
            {
                SaveFingerprintsImages(_document.Text);
                CalculateMatch(_document.Text);
                CleanEnrollTab();
                UpdateGridUsers();
            }
        }

        private void DeleteUser()
        {
            if (_users_table.CurrentRow == null)
                MessageBox.Show(@"Seleccione una fila", @"Eliminar", MessageBoxButtons.OK);
            else
            {
                var id = Convert.ToInt32(_users_table.CurrentRow.Cells[0].Value.ToString());
                var directory = _users_table.CurrentRow.Cells[2].Value.ToString();
                directory = directory.Trim();
                if (!_conn.DeleteUser(id)) return;
                UpdateGridUsers();
                CleanEnrollTab();
                if (!Directory.Exists(directory))
                    Directory.Delete(directory, true);
            }
        }

        private void UpdateGridUsers()
        {
            _conn.UpdateTableUser(_users_table);
        }

        private bool ValidateEnrollTab(bool id)
        {
            if (id && _currentId <= 0) return false;
            if (!string.IsNullOrEmpty(_document_type.Text) && !string.IsNullOrEmpty(_document.Text) &&
                !string.IsNullOrEmpty(_name.Text) && !string.IsNullOrEmpty(_last_name.Text) &&
                !string.IsNullOrEmpty(_birthdate.Text))
            {
                return true;
            }

            MessageBox.Show(@"Diligencie todos los campos", @"Error", MessageBoxButtons.OK);
            return false;
        }

        private void CleanEnrollTab()
        {
            _currentId = 0;
            _document_type.Text = null;
            _document.Text = string.Empty;
            _name.Text = string.Empty;
            _last_name.Text = string.Empty;
            _birthdate.Value = DateTime.Now;
            _guardar.Text = @"Guardar";
            _dpi.Text = (_numberDpi).ToString();
            UpdateGridUsers();
            CleanFingersPictureBox();
            CleanOrCreateDirectory(_fingerprintsPath);
            UpdatePictureBox1();
            for (var i = 0; i < 10; i++)
                _fingersEnrolled[i] = false;
        }

        private static void CleanOrCreateDirectory(string directory)
        {
            directory = directory.Trim();
            if (Directory.Exists(directory))
            {
                var files = Directory.GetFiles(directory);
                foreach (var file in files)
                    File.Delete(file);
            }
            else
                Directory.CreateDirectory(directory);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            CleanOrCreateDirectory(_fingerprintsPath);
            CleanEnrollTab();
        }

        private void CalculateAndSetAgeEnroll()
        {
            var age = DateTime.Now.Year - _birthdate.Value.Year;

            if (DateTime.Now.Month < _birthdate.Value.Month || (DateTime.Now.Month == _birthdate.Value.Month && DateTime.Now.Day < _birthdate.Value.Day))
                age--;

            _age.Text = (age).ToString();
        }

        private void CalculateAndSetAgeValidate()
        {
            var birthdate = DateTime.Parse(_birthdate2.Text);
            var age = DateTime.Now.Year - birthdate.Year;

            if (DateTime.Now.Month < birthdate.Month || (DateTime.Now.Month == birthdate.Month && DateTime.Now.Day < birthdate.Day))
                age--;

            _age2.Text = (age).ToString();
        }

        private void _birthdate_ValueChanged(object sender, EventArgs e)
        {
            CalculateAndSetAgeEnroll();
        }

        private void _setDpi_Click(object sender, EventArgs e)
        {
            _numberDpi = Convert.ToInt32(_dpi.Text);
        }

        private void _buscar_Click(object sender, EventArgs e)
        {
            _log.Text = string.Empty;
            if (string.IsNullOrEmpty(_document2.Text))
            {
                SetText("Digite el número de documento", Color.Red);
            }
            else
            {
                var data = _conn.FindByDocument(_document2.Text.Trim());
                if (data[0] != null)
                {
                    _document_type2.Text = data[1];
                    _name2.Text = data[3];
                    _last_name2.Text = data[4];
                    _birthdate2.Text = data[5];
                    if (data[5] != null)
                        CalculateAndSetAgeValidate();
                    CopyFingerprintsImages(_document2.Text);
                    FingersVefication();

                    SetText("Seleccione un dedo para verificar", Color.Blue);
                }
                else
                {
                    SetText("Usuario no encontrado", Color.Red);
                }
            }
        }

        private void _finger2_SelectedIndexChanged(object sender, EventArgs e)
        {
            VerifyFingerprints();
        }

        private void VerifyFingerprints()
        {
            var value = Array.IndexOf(_fingersNames, _finger2.Text);
            if (value < 0) return;
            var files = Directory.GetFiles(_fingerprintsPath);
            var count = 0;
            foreach (var file in files)
            {
                int index;
                if (!int.TryParse(Path.GetFileName(file).Substring(0, 1), out index)) continue;
                if (index == value)
                    count++;
            }

            for (var i = 1; i <= count; i++)
                _fingerprint.Items.Add("Huella " + i);
        }

        private void _verificar_Click(object sender, EventArgs e)
        {
            SetText("Verificación en proceso", _blue);
            _currentFinger = Array.IndexOf(_fingersNames, _fingers.Text);
            DisableEnrollControls();
            _verifyThread = new Thread(VerifyThreadFuction) { Name = "verifyPDThread" };
            _verifyThread.Start();
        }

        private void ValidateMatch()
        {
            int finger = Array.IndexOf(_fingersNames, _finger2.Text);
            int fingerprint = Convert.ToInt32(_fingerprint.Text.Substring(7));
            var ofp = new OrionFingerprints();
            ofp.SetAnsiLibLicense(@"C:\Licencias\ISO&ANSI\isoansi.lic");
            ofp.SetIdkitLibLicense(@"C:\Licencias\IDKitPC\iengine.lic");
            ofp.SetIsegLibLicense(@"C:\Licencias\Segmentation\segmentation.lic");
            ofp.SetDpi(_numberDpi);
            ofp.Initialize();

            var results = new double[3];
            var file1 = (Path.Combine(_fingerprintsPath, finger + "_" + fingerprint + ".bmp"));
            var file2 = "CurrentMatch.bmp";
            if (File.Exists(file1) && File.Exists(file2))
            {
                ofp.SetImages(file1, file2);
                results[0] = ofp.MatchIdkit(file1, file1, file2, @"F:\iengine.db");
                results[1] = ofp.SingleSegmentateWithMatch();
                results[2] = ofp.SingleSegmentateWithMatch("ansi");

                var higher = 0.0;
                for (var i = 0; i < 3; i++)
                {
                    if (results[i] > higher)
                        higher = results[i];
                }
                _resultsMatch.Text = string.Empty;
                _resultsMatch.AppendText("Scores");
                _resultsMatch.AppendText("\r\nIDKit: " + results[0]);
                _resultsMatch.AppendText("\r\nIso: " + results[1]);
                _resultsMatch.AppendText("\r\nAnsi: " + results[2]);
                _resultsMatch.AppendText("\r\nUmbral: " + _numberUmbral);

                MessageBox.Show(higher > _numberUmbral ? @"Verificación satisfactoria" : @"Verificación no satisfactoria",
                    @"Verificación", MessageBoxButtons.OK);
            }
            else
            {
                MessageBox.Show(@"Error al intentar hacer match", @"Match", MessageBoxButtons.OK);
            }
        }

        private void _setUmbral_Click(object sender, EventArgs e)
        {
            _numberUmbral = Convert.ToInt32(_umbral.Text);
        }
    }
}
