using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
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
    public delegate void DelegateCaptureWithPDCancelled();
    public delegate void DelegateCaptureTimeOut();
    public delegate void DelegateComTimeOut();
    public delegate void DelegateLatentDetected();
    public delegate void WriteResultsDelegate(string text, Color txtColor);
    public delegate void SetNISTImageDelegate(uint nistQauality);
    public delegate void PreviewLiveMode(IntPtr pImage, int width, int height, int imgnum);

    //typedef int (*LumiAcqStatusCallback)(uint nStatus);
    //public delegate
    
    public enum SensorType
    {
        M30X,
        V30X,
        V30X_ID,
        M31X,
        V31X,
        UNKNOWN
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

        public Config m_config;
        public XmlSerializer serializer;
        public uint m_nSelectedSensorID;
        public ArrayList SensorList = new ArrayList();
        public bool m_bClosingApp;
        public bool m_bStayInCapture;
        public string m_sFolderPath;
        public string m_debugFolder;
        public Color Red = Color.FromArgb(222, 56, 49);
        public Color Blue = Color.FromArgb(0, 63, 114);
        Thread m_CaptureWithPDThread;
        Thread m_EnrollThread;
        Thread m_VerifyThread;
        public SetImageDelegate m_DelegateSetImage;
        public DelegateCaptureThreadFinished m_CaptureThreadFinished;
        public DelegateVerifyThreadFinished m_VerifyThreadFinished;
        public DelegateCaptureThreadFinished m_EnrollThreadFinished;
        public WriteResultsDelegate m_DelegateErrorMessage;
        public SetNISTImageDelegate m_DelegateSetNISTImage;
        public WriteResultsDelegate m_DelegateNISTStatus;
        public DelegateCaptureWithPDCancelled m_DelegateCaptureCancelled;
        public DelegateCaptureTimeOut m_DelegateCaptureTimeOut;
        public DelegateComTimeOut m_DelegateComTimeOut;
        public DelegateLatentDetected m_DelegateLatentDetected;
        public PreviewLiveMode m_DelegatePreviewLiveMode;
        private bool m_bPDCaptureInProcess;
        private bool m_bCancelCapture;
        public bool m_bSensorTriggerArmed;
        public bool m_bSpoofEnabled;
        public uint m_MatchThreshold;
        public uint m_SpoofThreshold;
        public bool m_SpoofThreshold_HighlySecured;
        public bool m_SpoofThreshold_Secured;
        public bool m_MatchThreshold_Convenient;
        public bool m_MatchThreshold_HighlySecured;
        public bool m_MatchThreshold_Secured;
        public bool m_SpoofThreshold_Convenient;
        public bool m_bDevicePresent;
        public bool m_bNISTQuality;
        public bool m_bCancelLiveMode;
        public bool m_bLiveModeinProcess;
        public string m_sEnrollSubjID;
        public bool m_newSubjID;
        public bool m_bComTimeOut;
        public bool m_bLatentDetected;
        public SubjectData m_currentSubject;
        public int m_CurrentHotSpot;
        public string m_sVerifySubjID;
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
        Image defaultImage, gImage;

        LumiSDKWrapper.LumiPreviewCallbackDelegate m_delLiveMode;

        public byte[] m_bRawImageBuffer;
        public uint m_nWidth;
        public uint m_nHeight;
        public byte[] m_bTemplateBuffer;

        public int m_nEnrollmentCaptureIndex = -1;

        int prevclosePointIndex;
        bool draw = true;

        public MainForm()
        {
            InitializeComponent();
            _conn = new Connection();
            _fingersEnrolled = new bool[10];
            _fingerprintsPath = "Temp\\";
            _numberDpi = 500;
            _numberUmbral = 28;

            m_bClosingApp = false;
            m_bStayInCapture = false;
            m_MatchThreshold = 0;
            m_SpoofThreshold = 0;
            m_SpoofThreshold_HighlySecured = false;
            m_SpoofThreshold_Secured = false;
            m_SpoofThreshold_Convenient = false;
            m_MatchThreshold_HighlySecured = false;
            m_MatchThreshold_Secured = false;
            m_MatchThreshold_Convenient = false;
            labelStatus.Text = "";
            labelStatus2.Text = "";
            labelStatus2.Text = "";
            NISTScoreLabel.Text = "";
            _umbral.Text = _numberUmbral.ToString();
            m_bPDCaptureInProcess = false;
            m_bCancelCapture = false;
            m_bCancelLiveMode = false;
            m_bLiveModeinProcess = false;
            m_bDevicePresent = false;
            m_DelegateSetImage = SetImage;
            m_CaptureThreadFinished = CaptureFinished;
            m_VerifyThreadFinished = VerifyFinished;
            m_EnrollThreadFinished = EnrollFinished;
            m_currentSubject = new SubjectData();
            m_newSubjID = true;
            m_sVerifySubjID = "";
            m_bComTimeOut = false;
            m_bLatentDetected = false;
            m_config = new Config();
            serializer = new XmlSerializer(typeof(Config));
            
            m_DelegateErrorMessage = SetText;
            m_DelegateCaptureCancelled = CaptureCancelled;
            m_DelegateCaptureTimeOut = CaptureTimeOut;
            m_DelegateComTimeOut = ComTimeOut;
            m_DelegateLatentDetected = LatentDetected;
            m_DelegateNISTStatus = SetNISTStatusText;
            m_DelegatePreviewLiveMode = PreviewCallback;
            m_DelegateSetNISTImage = SetNISTStatusImage;
            OperatingSystem os = Environment.OSVersion;

            CreateAppDataFolder(os);
            defaultImage = pictureBox1.Image;
    
            if (GetSensorList() == false)
            {
                DisableControls();
            }
            else
            {
                m_bDevicePresent = true;
                PopulateSensorComboBox();
                SetMatchAndSpoofThresholds();
            }

            m_delLiveMode = PreviewCallback;
        }

        ~MainForm()
        {
            GC.KeepAlive(m_delLiveMode);
        }

        public void EnableControls()
        {

            CaptureBtnClick.Enabled = true;
            if (m_bPDCaptureInProcess)
            {
                CaptureBtnClick.Text = "Capture";
                m_bPDCaptureInProcess = false;

            }
            LiveBtn.Enabled = true;
            btnSaveImage.Enabled = true;
            SelectSensorComboBox.Enabled = true;
            tabControl.Enabled = true;
        }

        public void EnableControlsForLiveMode()
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
            if(m_bComTimeOut==false)
                EnableVerifyControls();
        }

        private void CaptureCancelled()
        {
            m_bCancelCapture = true;
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

        public void DisableControls()
        {
            DisableControls(false);
        }

        private void DisableControls(bool bCaptureWithPD)
        {
            if (m_bDevicePresent == false)
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
                if (bCaptureWithPD)
                {
                    m_bPDCaptureInProcess = true;
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
            if (m_bDevicePresent == false)
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

        public bool GetSensorList()
        {
            LumiSDKWrapper.LumiStatus rc = LumiSDKWrapper.LumiStatus.LUMI_STATUS_OK;

            LumiSDKWrapper.LUMI_DEVICE dev = new LumiSDKWrapper.LUMI_DEVICE();

            uint nNumDevices = 0;
            uint handle = 0;
            StringBuilder DevList = null;

            rc = LumiSDKWrapper.LumiQueryNumberDevices(ref nNumDevices, DevList);

            if (rc != LumiSDKWrapper.LumiStatus.LUMI_STATUS_OK)
            {
                SetText("Installation error please reinstall\n", Red);
                return false;
            }

            if (nNumDevices < 1)
            {
                SetText("No compatible Lumidigm sensors found. \nPlease connect a sensor and \nrestart the SDK Enroll App", Red);
                return false;
            }
            uint nFailedDevices = 0;

            for (uint ii = 0; ii < nNumDevices; ii++)
            {
                rc = LumiSDKWrapper.LumiQueryDevice(ii, ref dev);
                if (rc != LumiSDKWrapper.LumiStatus.LUMI_STATUS_OK) return false;

                rc = LumiSDKWrapper.LumiInit(dev.hDevHandle, ref handle);
                if (rc != LumiSDKWrapper.LumiStatus.LUMI_STATUS_OK)
                {
                    SetText("A device could not be initialized, it may be busy in another application. \n",Red);
                    nFailedDevices++;
                }
                else
                {
                    Sensor sensorid = new Sensor();
                    sensorid.handle = handle;
                    sensorid.SensorType = dev.SensorType;
                    sensorid.strIdentifier = dev.strIdentifier;
                    SensorList.Add(sensorid);
                }

            }
            if (nFailedDevices == nNumDevices)
            {
                SetText("All devices could not be initialized, they may be busy in\n another application. Please close all other\n applications and restart the Enroll App", Red);
                return false;
            }
            return true;
        }

        private void SetNISTStatusText(string text, Color txtColor)
        {
            if (m_bClosingApp) return;
            if (NISTScoreLabel.InvokeRequired)
            {
                SetTextCallback d = SetNISTStatusText;
                Invoke(d, text, txtColor);
            }
            else
            {
                NISTScoreLabel.Text = text;
                NISTScoreLabel.ForeColor = txtColor;
            }
        }

        private void SetNISTStatusImage(uint nistScore)
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
            if (m_bClosingApp) return;
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
        public void PreviewCallback(IntPtr pOutputImage, int width, int height, int imgNum)
        {
            if (m_nEnrollmentCaptureIndex != -1) return;
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
            catch (Exception error_prev)
            {
                MessageBox.Show(error_prev.Message);
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
                        m_sFolderPath = "..\\bin\\AppData";
                        string curDir = Directory.GetCurrentDirectory();
                        m_debugFolder = curDir + "\\AppData\\Debug";

                        isExists = Directory.Exists(m_sFolderPath);
                        if (!isExists)
                        {
                            Directory.CreateDirectory(m_sFolderPath);
                            Directory.CreateDirectory(m_sFolderPath + "\\Database");
                            Directory.CreateDirectory(m_sFolderPath + "\\Debug");
                        }
                        else
                        {                            
                            if(!Directory.Exists(m_debugFolder))
                                Directory.CreateDirectory(m_debugFolder);

                        }

                        if (!File.Exists(@"..\\bin\\AppData\\Settings.xml"))
                        {
                            m_config.SpoofEnabled = false;
                            EnableSpoofDetChkBox.Checked = false;
                            m_bSpoofEnabled = false;

                            m_config.SensorTriggerArmed = true;
                            SensorTriggerArmedChkBox.Checked = true;
                            m_bSensorTriggerArmed = true;

                            m_config.NISTQuality = true;
                            NISTQualityChkBox.Checked = true;
                            m_bNISTQuality = true;

                            m_config.SpoofSecure = true;
                            SpoofSecure.Checked = true;
                            m_SpoofThreshold_Secured = true;

                            m_config.SpoofHighlySecured = false;
                            SpoofHighlySecure.Checked = false;
                            m_SpoofThreshold_HighlySecured = false;

                            m_config.SpoofConvenient = false;
                            SpoofConvenient.Checked = false;
                            m_SpoofThreshold_Convenient = false;

                            m_config.MatchSecure = true;
                            MatchSecure.Checked = true;
                            m_MatchThreshold_Secured = true;

                            m_config.MatchHighlySecure = false;
                            MatchHighlySecured.Checked = false;
                            m_MatchThreshold_HighlySecured = false;

                            m_config.MatchConvenient = false;
                            MatchConvenient.Checked = false;
                            m_MatchThreshold_Convenient = false;

                            UpdateConfigXML();

                        }
                        else
                        {

                            ReadConfigXML();

                            EnableSpoofDetChkBox.Checked = m_config.SpoofEnabled;
                            m_bSpoofEnabled = m_config.SpoofEnabled;

                            SensorTriggerArmedChkBox.Checked = m_config.SensorTriggerArmed;
                            m_bSensorTriggerArmed = m_config.SensorTriggerArmed;

                            NISTQualityChkBox.Checked = m_config.NISTQuality;
                            m_bNISTQuality = m_config.NISTQuality;

                            SpoofSecure.Checked = m_config.SpoofSecure;
                            m_SpoofThreshold_Secured = m_config.SpoofSecure;

                            SpoofHighlySecure.Checked = m_config.SpoofHighlySecured;
                            m_SpoofThreshold_HighlySecured = m_config.SpoofHighlySecured;

                            SpoofConvenient.Checked = m_config.SpoofConvenient;
                            m_SpoofThreshold_Convenient = m_config.SpoofConvenient;

                            MatchSecure.Checked = m_config.MatchSecure;
                            m_MatchThreshold_Secured = m_config.MatchSecure;

                            MatchHighlySecured.Checked = m_config.MatchHighlySecure;
                            m_MatchThreshold_HighlySecured = m_config.MatchHighlySecure;

                            MatchConvenient.Checked = m_config.MatchConvenient;
                            m_MatchThreshold_Convenient = m_config.MatchConvenient;

                        }
                    }break;
                case 6:  // if Win7
                    {
                        m_sFolderPath = "C:\\ProgramData\\Lumidigm\\SDKEnrollExample\\AppData";
                        m_debugFolder = m_sFolderPath + "\\Debug";                     
                        isExists = Directory.Exists(m_sFolderPath);
                        if (vs.Minor != 0)
                        {
                            if (!isExists)
                            {
                                Directory.CreateDirectory(m_sFolderPath);
                                Directory.CreateDirectory(m_sFolderPath + "\\Database");
                                Directory.CreateDirectory(m_sFolderPath + "\\Debug");
                            }
                            else
                            {
                                
                                if (!Directory.Exists(m_debugFolder))
                                    Directory.CreateDirectory(m_sFolderPath + "\\Debug");

                            }
                            if (!File.Exists(@"C:\\ProgramData\\Lumidigm\\SDKEnrollExample\\AppData\\Settings.xml"))
                            {
                                m_config.SpoofEnabled = false;
                                EnableSpoofDetChkBox.Checked = false;
                                m_bSpoofEnabled = false;

                                m_config.SensorTriggerArmed = true;
                                SensorTriggerArmedChkBox.Checked = true;
                                m_bSensorTriggerArmed = true;

                                m_config.NISTQuality = true;
                                NISTQualityChkBox.Checked = true;
                                m_bNISTQuality = true;

                                m_config.SpoofSecure = true;
                                SpoofSecure.Checked = true;
                                m_SpoofThreshold_Secured = true;

                                m_config.SpoofHighlySecured = false;
                                SpoofHighlySecure.Checked = false;
                                m_SpoofThreshold_HighlySecured = false;

                                m_config.SpoofConvenient = false;
                                SpoofConvenient.Checked = false;
                                m_SpoofThreshold_Convenient = false;

                                m_config.MatchSecure = true;
                                MatchSecure.Checked = true;
                                m_MatchThreshold_Secured = true;

                                m_config.MatchHighlySecure = false;
                                MatchHighlySecured.Checked = false;
                                m_MatchThreshold_HighlySecured = false;

                                m_config.MatchConvenient = false;
                                MatchConvenient.Checked = false;
                                m_MatchThreshold_Convenient = false;

                                UpdateConfigXML();

                            }
                            else
                            {

                                ReadConfigXML();

                                EnableSpoofDetChkBox.Checked = m_config.SpoofEnabled;
                                m_bSpoofEnabled = m_config.SpoofEnabled;

                                SensorTriggerArmedChkBox.Checked = m_config.SensorTriggerArmed;
                                m_bSensorTriggerArmed = m_config.SensorTriggerArmed;

                                NISTQualityChkBox.Checked = m_config.NISTQuality;
                                m_bNISTQuality = m_config.NISTQuality;

                                SpoofSecure.Checked = m_config.SpoofSecure;
                                m_SpoofThreshold_Secured = m_config.SpoofSecure;

                                SpoofHighlySecure.Checked = m_config.SpoofHighlySecured;
                                m_SpoofThreshold_HighlySecured = m_config.SpoofHighlySecured;

                                SpoofConvenient.Checked = m_config.SpoofConvenient;
                                m_SpoofThreshold_Convenient = m_config.SpoofConvenient;

                                MatchSecure.Checked = m_config.MatchSecure;
                                m_MatchThreshold_Secured = m_config.MatchSecure;

                                MatchHighlySecured.Checked = m_config.MatchHighlySecure;
                                m_MatchThreshold_HighlySecured = m_config.MatchHighlySecure;

                                MatchConvenient.Checked = m_config.MatchConvenient;
                                m_MatchThreshold_Convenient = m_config.MatchConvenient;

                            }

                        }
                    } break;

                    default:
                        MessageBox.Show("This Operating System isn't Supported");
                        return false;
                }
            }

            m_debugFolder = m_debugFolder.Replace("\\", "/");
            m_debugFolder = m_debugFolder + "/";

            return true;
        }

        public bool UpdateConfigXML()
        {
            string xmlFileLoc;
            if (Environment.OSVersion.Version.Major == 5) // XP
                xmlFileLoc = "..\\bin\\AppData\\Settings.xml";
            else
                xmlFileLoc = "C:\\ProgramData\\Lumidigm\\SDKEnrollExample\\AppData\\Settings.xml";

            TextWriter tw = new StreamWriter(xmlFileLoc);

            serializer.Serialize(tw, m_config);
            tw.Close();
            return true;
        }

        public bool ReadConfigXML()
        {
            string xmlFileLoc;
            if (Environment.OSVersion.Version.Major == 5) // XP
                xmlFileLoc = "..\\bin\\AppData\\Settings.xml";
            else
                xmlFileLoc = "C:\\ProgramData\\Lumidigm\\SDKEnrollExample\\AppData\\Settings.xml";

            TextReader tr = new StreamReader(xmlFileLoc);
            m_config = (Config)serializer.Deserialize(tr);
            tr.Close();
            return true;
        }

        public int AcquStatusCallback(LumiSDKWrapper.LUMI_ACQ_STATUS status)
        {

            if (status == LumiSDKWrapper.LUMI_ACQ_STATUS.LUMI_ACQ_FINGER_PRESENT) return 0;

            SetText("", Color.Blue);

            return -2;
        }

        public int PresenceDetectionCallback(IntPtr pImage, int width, int height, uint status)
        {
 
            if (m_nEnrollmentCaptureIndex != -1) return 0;
            if (m_bClosingApp)
            {
                return -2;
            }
            int nSize = width * height * 3; // 24 bpp format is returned from SDK
            byte[] pOutputImage = new byte[nSize];
            Sensor sensorid = (Sensor)SensorList[(int)m_nSelectedSensorID];
            LumiSDKWrapper.LUMI_CONFIG deviceConfig;
            deviceConfig.eTemplateType = 0;
            deviceConfig.eTransInfo = 0;
            deviceConfig.nTriggerTimeout = 0;
            LumiSDKWrapper.LumiGetConfig(sensorid.handle, ref deviceConfig);

            if (sensorid.SensorType == LumiSDKWrapper.LUMI_SENSOR_TYPE.M32X)
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

            if (m_bCancelCapture)
            {
                m_bCancelCapture = false;
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

            m_nEnrollmentCaptureIndex = nEnrollmentCaptureIndex;

            SetImage(ref image, width, height, -1, pTemplate, nTempSz);
            SetImage(ref image, width, height, -1, pTemplate, nTempSz, nEnrollmentCaptureIndex);
            m_bRawImageBuffer = new byte[snapShotRaw.Length];
            snapShotRaw.CopyTo(m_bRawImageBuffer, 0);

            if (pTemplate != null)
            {
                m_bTemplateBuffer = new byte[pTemplate.Length];
                pTemplate.CopyTo(m_bTemplateBuffer, 0);
            }
            m_nWidth = width;
            m_nHeight = height;
            //Marshal.Copy(snapShotRaw, 0, m_bRawImageBuffer[0], snapShotRaw.Length);
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

        public void CalculateNISTScore(byte[] pImage, uint nWidth, uint nHeight, uint nBPP, uint nDPI, ref uint nNFIQ)
        {
            //uint width = 0, height = 0;
            //uint BPP = 0, DPI = 0, NFIQ = 0;
            byte[] NISTImage = new byte[5000]; //array to hold the template

        }

        public Bitmap ResizeBitmapAndDrawArrows(Bitmap image, int nWidth, int nHeight, int pdStatus)
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
        public Bitmap ResizeBitmap(Bitmap image, int nWidth, int nHeight, int pdStatus)
        {
            Bitmap result = new Bitmap(nWidth, nHeight);
            using (Graphics graphics = Graphics.FromImage(result))
            {
                graphics.DrawImage(image, 0, 0, nWidth, nHeight);
                //DrawArrow(graphics, pdStatus);
            }
            return result;
        }

        void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_bClosingApp = true;
            if (m_CaptureWithPDThread != null)
            {
                m_CaptureWithPDThread.Join(1000);
            }
            if (m_EnrollThread != null)
            {
                m_EnrollThread.Join(1000);
            }
            else
            {
                if (m_bDevicePresent)
                    DeviceClose();
            }
        }

        public void DeviceClose()
        {
            Sensor currentSensor = (Sensor)SensorList[(int)m_nSelectedSensorID];
            LumiSDKWrapper.LumiStatus _rc;

            try
            {
                _rc = LumiSDKWrapper.LumiClose(currentSensor.handle);
                _rc = LumiSDKWrapper.LumiExit();
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        void SetMatchAndSpoofThresholds()
        {
            if (SensorList.Count == 0)
                return;
            Sensor currentSensor = (Sensor)SensorList[(int)m_nSelectedSensorID];
            LumiSDKWrapper.LumiStatus rc;
            LumiSDKWrapper.LUMI_VERSION versInfo = new LumiSDKWrapper.LUMI_VERSION();

            switch (currentSensor.SensorType)
            {
                case LumiSDKWrapper.LUMI_SENSOR_TYPE.VENUS:
                    {
                        uint Handle = 0;
                        Handle = currentSensor.handle;
                        rc = LumiSDKWrapper.LumiGetVersionInfo(Handle, ref versInfo);
                        if (rc != LumiSDKWrapper.LumiStatus.LUMI_STATUS_OK)
                            return;
                        uint version = Convert.ToUInt32(versInfo.fwrVersion);
                        uint deviceType = Convert.ToUInt32(versInfo.tnsVersion);

                        if (version > 21304)
                        {
                            if (m_MatchThreshold_HighlySecured)
                                m_MatchThreshold = 27532;
                            else if (m_MatchThreshold_Secured)
                                m_MatchThreshold = 23688;
                            else
                                m_MatchThreshold = 20646;

                            if (m_SpoofThreshold_HighlySecured)
                                m_SpoofThreshold = 5;
                            else if (m_SpoofThreshold_Secured)
                                m_SpoofThreshold = 150;
                            else
                                m_SpoofThreshold = 1050;                            
                        }
                        else if (version == 21304)
                        {
                            if (m_MatchThreshold_HighlySecured)
                                m_MatchThreshold = 24298;
                            else if (m_MatchThreshold_Secured)
                                m_MatchThreshold = 22418;
                            else
                                m_MatchThreshold = 21548;

                            if (m_SpoofThreshold_HighlySecured)
                                m_SpoofThreshold = 5;
                            else if (m_SpoofThreshold_Secured)
                                m_SpoofThreshold = 150;
                            else
                                m_SpoofThreshold = 1050;    
                        }
                        else if (version <= 9538)
                        {
                            if (deviceType == 61)
                            {
                                if (m_MatchThreshold_HighlySecured)
                                    m_MatchThreshold = 24298;
                                else if (m_MatchThreshold_Secured)
                                    m_MatchThreshold = 22418;
                                else
                                    m_MatchThreshold = 21548;

                                if (m_SpoofThreshold_HighlySecured)
                                    m_SpoofThreshold = 100;
                                else if (m_SpoofThreshold_Secured)
                                    m_SpoofThreshold = 200;
                                else
                                    m_SpoofThreshold = 1000;   

                            }
                            else
                            {
                                if (m_MatchThreshold_HighlySecured)
                                    m_MatchThreshold = 24298;
                                else if (m_MatchThreshold_Secured)
                                    m_MatchThreshold = 22418;
                                else
                                    m_MatchThreshold = 21548;

                                if (m_SpoofThreshold_HighlySecured)
                                    m_SpoofThreshold = 100;
                                else if (m_SpoofThreshold_Secured)
                                    m_SpoofThreshold = 200;
                                else
                                    m_SpoofThreshold = 1000;   
                            }

                        }

                    } break;
                case LumiSDKWrapper.LUMI_SENSOR_TYPE.V31X:
                case LumiSDKWrapper.LUMI_SENSOR_TYPE.V371:
                    {
                        if (m_MatchThreshold_HighlySecured)
                            m_MatchThreshold = 24739;
                        else if (m_MatchThreshold_Secured)
                            m_MatchThreshold = 21493;
                        else
                            m_MatchThreshold = 16873;

                        if (m_SpoofThreshold_HighlySecured)
                            m_SpoofThreshold = 5;
                        else if (m_SpoofThreshold_Secured)
                            m_SpoofThreshold = 150;
                        else
                            m_SpoofThreshold = 1050;   

                    } break;
                case LumiSDKWrapper.LUMI_SENSOR_TYPE.M300:
                    {
                        if (m_MatchThreshold_HighlySecured)
                            m_MatchThreshold = 29990;
                        else if (m_MatchThreshold_Secured)
                            m_MatchThreshold = 27520;
                        else
                            m_MatchThreshold = 26350;

                        if (m_SpoofThreshold_HighlySecured)
                            m_SpoofThreshold = 100;
                        else if (m_SpoofThreshold_Secured)
                            m_SpoofThreshold = 200;
                        else
                            m_SpoofThreshold = 1000;   

                    } break;
                case LumiSDKWrapper.LUMI_SENSOR_TYPE.M100:
                    {
                        if (m_MatchThreshold_HighlySecured)
                            m_MatchThreshold = 22418;
                        else if (m_MatchThreshold_Secured)
                            m_MatchThreshold = 22000;
                        else
                            m_MatchThreshold = 15000;

                        if (m_SpoofThreshold_HighlySecured)
                            m_SpoofThreshold = 100;
                        else if (m_SpoofThreshold_Secured)
                            m_SpoofThreshold = 200;
                        else
                            m_SpoofThreshold = 1000;   

                    } break;
                case LumiSDKWrapper.LUMI_SENSOR_TYPE.M32X:
                    {
                        if (m_MatchThreshold_HighlySecured)
                            m_MatchThreshold = 30762;
                        else if (m_MatchThreshold_Secured)
                            m_MatchThreshold = 26368;
                        else
                            m_MatchThreshold = 25331;

                        if (m_SpoofThreshold_HighlySecured)
                            m_SpoofThreshold = 100;
                        else if (m_SpoofThreshold_Secured)
                            m_SpoofThreshold = 200;
                        else
                            m_SpoofThreshold = 1000;   

                    } break;
                case LumiSDKWrapper.LUMI_SENSOR_TYPE.M31X:
                    {
                        if (m_MatchThreshold_HighlySecured)
                            m_MatchThreshold = 31448;
                        else if (m_MatchThreshold_Secured)
                            m_MatchThreshold = 26728;
                        else
                            m_MatchThreshold = 25668;

                        if (m_SpoofThreshold_HighlySecured)
                            m_SpoofThreshold = 100;
                        else if (m_SpoofThreshold_Secured)
                            m_SpoofThreshold = 200;
                        else
                            m_SpoofThreshold = 1000;   

                    } break;
                default:
                    {
                    } break;
            }
        }

        void PopulateSensorComboBox()
        {
            foreach (Sensor sensor in SensorList)
            {
                SelectSensorComboBox.Items.Add(sensor.SensorType + " " + sensor.strIdentifier);
            }
            SelectSensorComboBox.SelectedIndex = 0;
            m_nSelectedSensorID = 0;

            Sensor currentSensor = (Sensor)SensorList[(int)m_nSelectedSensorID];
            string serialNum = "";
            GetSensorSerialNumber(ref serialNum);
            SetText("You have selected Sensor: " + currentSensor.SensorType + " SN " + serialNum, Blue);

            LumiSDKWrapper.LumiStatus rc = LumiSDKWrapper.LumiSetDCOptions(currentSensor.handle, m_debugFolder, 0);
        }

        void GetSensorSerialNumber(ref string serialNum)
        {
            Sensor currentSensor = (Sensor)SensorList[(int)m_nSelectedSensorID];
            switch (currentSensor.SensorType)
            {
                case LumiSDKWrapper.LUMI_SENSOR_TYPE.VENUS:
                    {
                        serialNum = currentSensor.strIdentifier.Substring(5);

                    } break;
                case LumiSDKWrapper.LUMI_SENSOR_TYPE.V31X:
                    {
                        serialNum = currentSensor.strIdentifier.Substring(2);

                    } break;
                case LumiSDKWrapper.LUMI_SENSOR_TYPE.M300:
                    {
                        serialNum = currentSensor.strIdentifier.Substring(4);

                    } break;
                case LumiSDKWrapper.LUMI_SENSOR_TYPE.M100:
                    {
                        serialNum = currentSensor.strIdentifier.Substring(4);

                    } break;
                case LumiSDKWrapper.LUMI_SENSOR_TYPE.M31X:
                    {
                        serialNum = currentSensor.strIdentifier.Substring(2);

                    } break;
                case LumiSDKWrapper.LUMI_SENSOR_TYPE.M32X:
                    {
                        serialNum = currentSensor.strIdentifier.Substring(2);

                    } break;
                default:
                    {
                    } break;
            }
        }

        private void SelectSensorComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_nSelectedSensorID = (uint)(SelectSensorComboBox.SelectedIndex);
            Sensor currentSensor = (Sensor)SensorList[(int)m_nSelectedSensorID];
            string serialNum = "";
            GetSensorSerialNumber(ref serialNum);
            SetText("You have selected Sensor: " + currentSensor.SensorType + " SN " + serialNum, Blue);
            SetMatchAndSpoofThresholds();

            LumiSDKWrapper.LumiStatus rc = LumiSDKWrapper.LumiSetDCOptions(currentSensor.handle, m_debugFolder, 0);
        }

        private void CaptureWithPDThreadFuction()
        {
            m_nEnrollmentCaptureIndex = -1;
            CaptureWithPD captureWithPresDetect = new CaptureWithPD(this);
            captureWithPresDetect.Run();
        }

        private void EnrollThreadFuction()
        {
            m_nEnrollmentCaptureIndex = -1;
            Enroll enrollWithPresDetect = new Enroll(this);
            enrollWithPresDetect.Run();
        }

        private void VerifyThreadFuction()
        {
            m_nEnrollmentCaptureIndex = -1;
            Verify verifyWithPresDetect = new Verify(this);
            verifyWithPresDetect.Run();
        }

        private void CaptureBtnClick_Click(object sender, EventArgs e)
        {           
            m_bCancelCapture = false;
            if (m_bPDCaptureInProcess)
            {
                m_bCancelCapture = true;
                EnableControls();
            }
            else
            {
                m_bPDCaptureInProcess = true;
                SetText("Capture in Process", Blue);
                DisableControls(true);

                m_CaptureWithPDThread = new Thread(CaptureWithPDThreadFuction);
                m_CaptureWithPDThread.Name = "CaptureWithPDThread";
                m_CaptureWithPDThread.Start();
            }
        }

        private void EnableSpoofDetChkBox_CheckedChanged(object sender, EventArgs e)
        {

            if (EnableSpoofDetChkBox.Checked == false)
            {
                m_config.SpoofEnabled = false;
                m_bSpoofEnabled = false;
                UpdateConfigXML();
            }
            else
            {
                m_config.SpoofEnabled = true;
                m_bSpoofEnabled = true;
                UpdateConfigXML();
            }
        }

        private void SensorTriggerArmedChkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (SensorTriggerArmedChkBox.Checked == false)
            {
                m_bSensorTriggerArmed = false;
                m_config.SensorTriggerArmed = false;
                UpdateConfigXML();
            }
            else
            {
                m_bSensorTriggerArmed = true;
                m_config.SensorTriggerArmed = true;
                UpdateConfigXML();
            }

        }

        private void NISTQualityChkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (NISTQualityChkBox.Checked == false)
            {
                m_bNISTQuality = false;
                m_config.NISTQuality = false;
                nistPictureBox.Image = null;
                UpdateConfigXML();

            }
            else
            {
                m_bNISTQuality = true;
                m_config.NISTQuality = true;
                UpdateConfigXML();
            }
        }

        private void LiveBtn_Click(object sender, EventArgs e)
        {
            NISTScoreLabel.Text = "";
            Sensor currentSensor = (Sensor)SensorList[(int)m_nSelectedSensorID];

            if (m_bCancelLiveMode == false)
            {
                LumiSDKWrapper.LumiSetLiveMode(currentSensor.handle, 1, m_delLiveMode);

                m_bCancelLiveMode = true;
                SetText("Live Mode in Process", Blue);
                DisableControlsForLiveMode();
            }
            else
            {
                LumiSDKWrapper.LumiSetLiveMode(currentSensor.handle, 0, m_delLiveMode);
                m_bCancelLiveMode = false;
                SetText("Live Mode Stopped", Blue);
                EnableControlsForLiveMode();
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tabControl.SelectedIndex)
            {
                case 0: // Capture Tab
                {
                    SetText("", Blue);
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
                    SetText("Digite los datos para enrolamiento", Blue);
                    _fingers.Clear();
                    LumiPictureBox1.Image = null;
                    m_currentSubject.clear();
                    pictureBox1.Image = defaultImage;
                    nistPictureBox.Image = null;
                    m_bCancelCapture = false;
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
                    SetText("Seleccione un usuario para verificación", Blue);
                    LumiPictureBox1.Image = null;
                    nistPictureBox.Image = null;
                    m_bCancelCapture = false;
                    // DisableVerifyControls();   

                } break;
            }
        }

        private void DisableEnrollControls()
        {
            if (m_bDevicePresent == false)
            {
                LiveBtn.Enabled = false;
                SelectSensorComboBox.Enabled = false;
                CaptureBtnClick.Enabled = false;
                tabControl.Enabled = false;
                LumiPictureBox1.Enabled = false;
                SetText("Device not found. Please connect device and restart the application", Red);
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
            int closePointIndex = closesetHotSpot(e);
            int radius = 14;
            int offset = 4;
            Graphics g = pictureBox1.CreateGraphics();
            
            Pen pen = new Pen(Color.Tomato, 4);
            SolidBrush brush = new SolidBrush(Color.LawnGreen);            
            Font drawFont = new Font("Arial", 10, FontStyle.Bold);
            
            SolidBrush Brush2 = new SolidBrush(Color .Red);       
            
            if (prevclosePointIndex == closePointIndex)
            {
               if (draw)
                {
                    draw = false;
                    FingerPos cc = new FingerPos();
                    _fingers.Text = _fingersNames[closePointIndex];

                    g.DrawEllipse(pen, cc.Pos[closePointIndex].x - (radius - offset), cc.Pos[closePointIndex].y - (radius - offset), radius, radius);
                    g.FillEllipse(brush, cc.Pos[closePointIndex].x - (radius - offset), cc.Pos[closePointIndex].y - (radius - offset), radius, radius);

                    if (fingerExists(closePointIndex))
                        g.DrawString("X", drawFont, Brush2, cc.Pos[closePointIndex].x - (radius - offset), cc.Pos[closePointIndex].y - (radius - offset));
                }
            }
            else
            {
                pictureBox1.Invalidate();                
                prevclosePointIndex = closePointIndex;
                draw = true;
            }
        }

        private bool fingerExists(int closePointIndex)
        {
            foreach (int finger in m_currentSubject.fingers)
            {
                if (finger == closePointIndex)
                    return true;
            }
            return false;
        }

        private static int closesetHotSpot(MouseEventArgs e)
        {
            FingerPos cc = new FingerPos();
            double min = 20000;
            int closestPointIndex = 0;
            for (int i = 0; i < 10; i++)
            {
                double dist = (Math.Pow(e.X - cc.Pos[i].x, 2) + Math.Pow(e.Y - cc.Pos[i].y, 2));
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
            SetText("Enroll in Process", Blue);
            _currentFinger = Array.IndexOf(_fingersNames, _fingers.Text);
            DisableEnrollControls();
            m_EnrollThread = new Thread(EnrollThreadFuction) {Name = "enrollPDThread"};
            m_EnrollThread.Start();
        }

        private void CleanFingersPictureBox()
        {
            _print1.Image = null;
            _print2.Image = null;
            _print3.Image = null;
        }

        private void UpdatePictureBox1()
        {            
            gImage = (Image)defaultImage.Clone();
            var g = Graphics.FromImage(gImage);
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
                g.DrawEllipse(pen, cc.Pos[i].x - (radius - offset), cc.Pos[i].y - (radius - offset), radius, radius);
                g.FillEllipse(brush, cc.Pos[i].x - (radius - offset), cc.Pos[i].y - (radius - offset), radius, radius);
                g.DrawString("E", drawFont, brush3, cc.Pos[i].x - (radius - offset), cc.Pos[i].y - (radius - offset));

            }
            pictureBox1.Image = gImage;
        }
        
        private void EnrollFinished()
        {
            if (m_bComTimeOut) return;
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
                ANSI378TemplateHelper templateHelper = new ANSI378TemplateHelper(template, (int)templateSize);
                Minutiae[] minutiaeList = templateHelper.GetMinutiaeList();
                ////// Draw minutia on capturedImage.Image
                Graphics g = Graphics.FromImage(bmp);// this.LumiPictureBox1.CreateGraphics();


                Pen pen2;
                SolidBrush brush;

                foreach (var minutiae in minutiaeList)
                {
                    if (minutiae.nType == 1)
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

                    int nX = minutiae.nX;
                    int nY = minutiae.nY;

                    g.DrawEllipse(pen2, nX - 3, nY - 3, 6, 6);
                    g.FillEllipse(brush, nX - 3, nY - 3, 6, 6);

                    double nR = minutiae.nRotAngle;
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
                        Byte[] tmpImageBuffer = new Byte[m_nWidth * m_nHeight * 3];
                        int j = 0;
                        for (int i = 0; i < (m_nWidth * m_nHeight); i++)
                        {
                            tmpImageBuffer[j++] = m_bRawImageBuffer[i];
                            tmpImageBuffer[j++] = m_bRawImageBuffer[i];
                            tmpImageBuffer[j++] = m_bRawImageBuffer[i];
                        }
                        String rawFileName = save.FileName.Replace(".bmp", "_NoMinutiae.bmp");
                        
                        Bitmap bmp = new Bitmap((int)m_nWidth, (int)m_nHeight, PixelFormat.Format24bppRgb);
                        BitmapData bmpData = bmp.LockBits(
                                             new Rectangle(0, 0, (int)m_nWidth, (int)m_nHeight),
                                             ImageLockMode.WriteOnly, bmp.PixelFormat);
                        Marshal.Copy(tmpImageBuffer, 0, bmpData.Scan0, tmpImageBuffer.Length);
                        bmp.Save(rawFileName, ImageFormat.Bmp);
                        
                        // Save raw byte data from image
                        String rawByteFileName = save.FileName.Replace(".bmp", ".bin");
                        BinaryWriter binaryWriter = new BinaryWriter(File.OpenWrite(rawByteFileName));
                        binaryWriter.Write(m_bRawImageBuffer);
                        binaryWriter.Flush();
                        binaryWriter.Close();

                        // Save raw byte data from fingerprint template
                        String rawByteTmpName = save.FileName.Replace(".bmp", "_tmp.bin");
                        BinaryWriter binaryWriter2 = new BinaryWriter(File.OpenWrite(rawByteTmpName));
                        binaryWriter2.Write(m_bTemplateBuffer);
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

            Sensor currentSensor = (Sensor)SensorList[(int)m_nSelectedSensorID];
            LumiSDKWrapper.LumiStatus rc;
            LumiSDKWrapper.LUMI_VERSION versInfo = new LumiSDKWrapper.LUMI_VERSION();

            switch (currentSensor.SensorType)
            {
                case LumiSDKWrapper.LUMI_SENSOR_TYPE.VENUS:
                    {
                        uint Handle = 0;
                        Handle = currentSensor.handle;
                        rc = LumiSDKWrapper.LumiGetVersionInfo(Handle, ref versInfo);
                        uint version = Convert.ToUInt32(versInfo.fwrVersion);
                        uint deviceType = Convert.ToUInt32(versInfo.tnsVersion);

                        if (version >= 21304)
                        {
                            m_SpoofThreshold = 1050;
                        }
                        else if (version <= 9538)
                        {
                            if (deviceType == 61)
                            {
                                m_SpoofThreshold = 1050;

                            }
                            else
                            {
                                m_SpoofThreshold = 1000;

                            }
                        }

                    } break;
                case LumiSDKWrapper.LUMI_SENSOR_TYPE.V31X:
                    {
                        m_SpoofThreshold = 1050;
                    } break;
                case LumiSDKWrapper.LUMI_SENSOR_TYPE.M300:
                    {
                        m_SpoofThreshold = 1000;
                    } break;
                case LumiSDKWrapper.LUMI_SENSOR_TYPE.M100:
                    {
                        m_SpoofThreshold = 1000;
                    } break;
                case LumiSDKWrapper.LUMI_SENSOR_TYPE.M31X:
                    {
                        m_SpoofThreshold = 1000;
                    } break;
                case LumiSDKWrapper.LUMI_SENSOR_TYPE.M32X:
                    {
                        m_SpoofThreshold = 1000;
                    } break;
                default:
                    {
                    } break;
            }
        }

        private void SetSecureMatchThresholds()
        {
            Sensor currentSensor = (Sensor)SensorList[(int)m_nSelectedSensorID];
            LumiSDKWrapper.LumiStatus rc;
            LumiSDKWrapper.LUMI_VERSION versInfo = new LumiSDKWrapper.LUMI_VERSION();

            switch (currentSensor.SensorType)
            {
                case LumiSDKWrapper.LUMI_SENSOR_TYPE.VENUS:
                    {
                        uint Handle = 0;
                        Handle = currentSensor.handle;
                        rc = LumiSDKWrapper.LumiGetVersionInfo(Handle, ref versInfo);
                        uint version = Convert.ToUInt32(versInfo.fwrVersion);
                        uint deviceType = Convert.ToUInt32(versInfo.tnsVersion);

                        if (version >= 21304)
                        {
                            m_MatchThreshold = 22418;
                        }
                        else if (version <= 9538)
                        {
                            m_MatchThreshold = deviceType == 61 ? (uint) 27520 : (uint) 22418;
                        }

                    } break;
                case LumiSDKWrapper.LUMI_SENSOR_TYPE.V31X:
                    {
                        m_MatchThreshold = 22418;
                    } break;
                case LumiSDKWrapper.LUMI_SENSOR_TYPE.M300:
                    {
                        m_MatchThreshold = 27520;
                    } break;
                case LumiSDKWrapper.LUMI_SENSOR_TYPE.M100:
                    {
                        m_MatchThreshold = 27520;
                    } break;
                case LumiSDKWrapper.LUMI_SENSOR_TYPE.M31X:
                    {
                        m_MatchThreshold = 27520;
                    } break;
                case LumiSDKWrapper.LUMI_SENSOR_TYPE.M32X:
                    {
                        m_MatchThreshold = 27520;
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
               m_SpoofThreshold_HighlySecured = false;
               m_config.SpoofHighlySecured = false;
               UpdateConfigXML();
           }
           else
           {            
               m_SpoofThreshold_HighlySecured = true;
               m_config.SpoofHighlySecured = true;
               SetMatchAndSpoofThresholds();
               UpdateConfigXML();
           }
        }

        private void SpoofSecure_CheckedChanged_1(object sender, EventArgs e)
        {            
            if (SpoofSecure.Checked == false)
            {         
                m_SpoofThreshold_Secured = false;
                m_config.SpoofSecure = false;
                UpdateConfigXML();
            }
            else
            {             
                m_SpoofThreshold_Secured = true;
                m_config.SpoofSecure = true;
                SetMatchAndSpoofThresholds();
                UpdateConfigXML();
            }
        }

        private void SpoofConvenient_CheckedChanged_1(object sender, EventArgs e)
        {           
            if (SpoofConvenient.Checked == false)
            {         
                m_SpoofThreshold_Convenient = false;
                m_config.SpoofConvenient = false;
                UpdateConfigXML();
            }
            else
            {             
                m_SpoofThreshold_Convenient = true;
                m_config.SpoofConvenient = true;
                SetMatchAndSpoofThresholds();
                UpdateConfigXML();
            }
        }

        private void MatchHighlySecure_CheckedChanged_1(object sender, EventArgs e)
        {
            if (MatchHighlySecured.Checked == false)
            {
                m_MatchThreshold_HighlySecured = false;
                m_config.MatchHighlySecure = false;
                UpdateConfigXML();
            }
            else
            {
                m_MatchThreshold_HighlySecured = true;
                m_config.MatchHighlySecure = true;
                SetMatchAndSpoofThresholds();
                UpdateConfigXML();
            }
        }

        private void MatchSecure_CheckedChanged_1(object sender, EventArgs e)
        {            
            if (MatchSecure.Checked == false)
            {         
                m_MatchThreshold_Secured = false;
                m_config.MatchSecure = false;
                UpdateConfigXML();
            }
            else
            {             
                m_MatchThreshold_Secured = true;
                m_config.MatchSecure = true;
                SetMatchAndSpoofThresholds();
                UpdateConfigXML();
            }
        }

        private void MatchConvenient_CheckedChanged_1(object sender, EventArgs e)
        {            
            if (MatchConvenient.Checked == false)
            {         
                m_MatchThreshold_Convenient = false;
                m_config.MatchConvenient = false;
                UpdateConfigXML();
            }
            else
            {             
                m_MatchThreshold_Convenient = true;
                m_config.MatchConvenient = true;
                SetMatchAndSpoofThresholds();
                UpdateConfigXML();
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
            SetText("Verificación en proceso", Blue);
            _currentFinger = Array.IndexOf(_fingersNames, _fingers.Text);
            DisableEnrollControls();
            m_VerifyThread = new Thread(VerifyThreadFuction) { Name = "verifyPDThread" };
            m_VerifyThread.Start();
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

        private void CleanVarifyTab()
        {

        }
    }
}
