/******************************************************************************
**  You are free to use this example code to generate similar functionality  
**  tailored to your own specific needs.  
**
**  This example code is provided by Lumidigm Inc. for illustrative purposes only.  
**  This example has not been thoroughly tested under all conditions.  Therefore 
**  Lumidigm Inc. cannot guarantee or imply reliability, serviceability, or 
**  functionality of this program.
**
**  This example code is provided by Lumidigm Inc. "AS IS" and without any express 
**  or implied warranties, including, but not limited to the implied warranties of 
**  merchantability and fitness for a particular purpose.
**
**	
**	Author: RMcKee
**
** 
**	Copyright 2012 Lumidigm
**
********************************************************************************/

using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Threading;
using SDKWrapper;

namespace CSharpExample
{
    // Delegate meathods for threading
    public delegate void SetImageDelegate(ref byte[] snapShot, uint width, uint height);
    public delegate void WriteResultsDelegate(string strResults);
    public delegate void DelegateCaptureThreadFinished();
    public delegate void DelegateMatchThreadFinished();
    public delegate void DelegateCaptureWithPDFinished();    

    public partial class ImageFrm : Form
    {
        // Thread definitions
        Thread m_MatchThread;
        Thread m_CaptureWithPDThread;

        // Delegate methods that are called from threads
        public SetImageDelegate m_DelegateSetImage;
        public WriteResultsDelegate m_DelegateWriteResults;
        public DelegateCaptureThreadFinished m_CaptureThreadFinished;
        public DelegateMatchThreadFinished m_MatchThreadFinished;
        public DelegateCaptureWithPDFinished m_CaptureWithPDFinished;
        public LumiSDKWrapper.LumiAcqStatusCallbackDelegate del;

        // Variables used for displaying presence detection feedback 
        private long m_startTime = 0;
        private uint BLINK_RATE = 500;
        private uint m_nBlinkRate = 0;
        private bool m_bShowArrows;

        // PD capture flag
        private bool m_bPDCaptureInProcess;

        private bool m_bCancelCapture;
        
        public ImageFrm()
        {
            InitializeComponent();
            m_DelegateSetImage = new SetImageDelegate(this.SetImage);
            m_DelegateWriteResults = new WriteResultsDelegate(this.WriteResults);
            m_CaptureThreadFinished = new DelegateCaptureThreadFinished(this.CaptureFinished);
            m_MatchThreadFinished = new DelegateMatchThreadFinished(this.MatchFinished);
            m_CaptureWithPDFinished = new DelegateCaptureWithPDFinished(this.CaptureWithPDFinished);
            del = new LumiSDKWrapper.LumiAcqStatusCallbackDelegate(this.AcqStatusCallback);

            // Set PD Capture flag
            m_bPDCaptureInProcess = false;    
            // Set Cancel Capture state
            m_bCancelCapture = false;     
            
            try
            {
                // Open the scanner
                SDKBiometrics.OpenScanner();
                // Get Current Timeout
                LumiSDKWrapper.LUMI_CONFIG config = new LumiSDKWrapper.LUMI_CONFIG();
                SDKBiometrics.GetConfig(ref config);
                this.txtTriggerTimeout.Text = config.nTriggerTimeout.ToString();
            }
            catch(Exception err)
            {
                MessageBox.Show(err.Message, "Error");
                DisableControls();
                return;
            }           
            
        }

        private void Image_Activated(object sender, System.EventArgs e)
        {
            statusTextBox.Text = string.Empty;
        }
      
        // Capture with presence detection feedback
        private void CaptureWithPD_Click(object sender, EventArgs e)
        {
            if (m_bPDCaptureInProcess)
            {
                m_bCancelCapture = true;
                EnableControls();
            }
            else
            {
                DisableControls(true);
                m_CaptureWithPDThread = new Thread(new ThreadStart(this.CaptureWithPDThreadFuction));
                m_CaptureWithPDThread.Name = "CaptureWithPDThread";
                m_CaptureWithPDThread.Start();
            }
        }

        // Match with preview image
        private void Match_Click(object sender, EventArgs e)
        {
            // Check to see if we can do a template extract and match
            try
            {                
                if (!SDKBiometrics.ExtractTemplateAvailable())
                {
                    WriteResults("ERROR: The sensor is not configured to extract or match templates.\r\n");
                    return;
                }             
            }
            catch (Exception err)
            {
                WriteResults(err.ToString());
                return;
            }

            DisableControls();
            m_MatchThread = new Thread(new ThreadStart(this.MatchThreadFuction));
            m_MatchThread.Name = "MatchThread";
            m_MatchThread.Start();  
        }

        // Threading methods
        private void CaptureFinished() 
        {
            EnableControls();
        }
        private void MatchFinished() 
        {
            EnableControls();
        }
        private void CaptureWithPDFinished()
        {
            EnableControls();
        }
        private void CaptureWithPDThreadFuction()
        {
            CaptureWithPresDetFeedback captureWithPD = new CaptureWithPresDetFeedback(this);
            captureWithPD.Run();
        }
        private void MatchThreadFuction()
        {
            MatchProcess matchProcess = new MatchProcess(this);
            matchProcess.Run();
        }

        public void EnableControls()
        {            
            CaptureWithPD.Enabled = true;
            if (m_bPDCaptureInProcess)
            {
                CaptureWithPD.Text = "Lumi Capture w/ Presence Detection Feedback";
                m_bPDCaptureInProcess = false;                
            }
            Match.Enabled = true;
            ChangeTimeout.Enabled = true;
            btnTestAPI.Enabled = true;
        }

        private void DisableControls(bool bCaptureWithPD)
        {
            lumiPictureBox.Image = null;
            if (bCaptureWithPD)
            {
                m_bPDCaptureInProcess = true;
                CaptureWithPD.Text = "Cancel";
            }
            else
            {
                CaptureWithPD.Enabled = false;
            }            
            Match.Enabled = false;
            ChangeTimeout.Enabled = false;
            btnTestAPI.Enabled = false;
        }

        public void DisableControls()
        {
            DisableControls(false);
        }

        // Overloaded Method to set the image buffer into the Picture box control
        private void SetImage(ref byte[] image, uint width, uint height)
        {
            SetImage(ref image, width, height, -1);
        }

        // Overloaded Method to set the image buffer into the Picture box control
        private void SetImage(ref byte[] image, uint width, uint height, int pdStatus)
        {
            Bitmap bmp = new Bitmap((int)width, (int)height, PixelFormat.Format24bppRgb);

            BitmapData bmpData = bmp.LockBits(
                                 new Rectangle(0, 0, bmp.Width, bmp.Height),
                                 ImageLockMode.WriteOnly, bmp.PixelFormat);

            Marshal.Copy(image, 0, bmpData.Scan0, image.Length);

            // Get aspect ratios
            float bmpAspectRatio = (float)width / (float)height;
            float imgControlAspectRatio = (float)lumiPictureBox.Width / (float)lumiPictureBox.Height;

            // Correct for aspect ration differnece between img control and bmp
            int widthDisplay = 0, heightDisplay = 0;
            if (Math.Abs((bmpAspectRatio - imgControlAspectRatio) / bmpAspectRatio) > .07)
            {
                if (bmpAspectRatio < 1)
                {
                    widthDisplay = lumiPictureBox.Width;
                    heightDisplay = (int)(lumiPictureBox.Width / bmpAspectRatio);
                }
                else
                {
                    // Currently don't have a sensor that falls into this category, so we'll do nothing.
                }
            }
            else
            {
                widthDisplay = lumiPictureBox.Width;
                heightDisplay = lumiPictureBox.Height;
            }      

            bmp.UnlockBits(bmpData);
            // Resize to composite image size and draw arrows 
            lumiPictureBox.Image = ResizeBitmapAndDrawArrows(bmp, widthDisplay, heightDisplay, pdStatus);
        
        }

        // Resize bitmap image
        public Bitmap ResizeBitmapAndDrawArrows(Bitmap image, int nWidth, int nHeight, int pdStatus) 
        { 
            Bitmap result = new Bitmap(nWidth, nHeight);
            using (Graphics graphics = Graphics.FromImage((Image)result))
            {
                graphics.DrawImage(image, 0, 0, nWidth, nHeight);
                DrawArrow(graphics, pdStatus);
            }            
            return result; 
        }
        
        // Method to write results to text box
        private void WriteResults(string results)
        {
            statusTextBox.Text = results;
        }
        public void AddResults(string results)
        {
            statusTextBox.AppendText(results);
        }
        // Callback function for presence detection image display
        public int PresenceDetectionCallback(IntPtr pImage, int width, int height, uint status)
        {
            int nSize = width * height * 3; // 24 bpp format is returned from SDK
            byte[] pOutputImage = new byte[nSize];    
            Marshal.Copy(pImage, pOutputImage, 0, nSize);   

            SetImage(ref pOutputImage, (uint)width, (uint)height, (int)status);

            if (m_bCancelCapture)
            {
                m_bCancelCapture = false;
                lumiPictureBox.Image = null;
                return -2;   // Return -2 to cancel the capture             
            }
            else
            {
                return 0;
            }
        }
        // Callback function for Aquisitition Status
        public int AcqStatusCallback(LumiSDKWrapper.LUMI_ACQ_STATUS nStatus)
        {
            switch (nStatus)
            {
                case LumiSDKWrapper.LUMI_ACQ_STATUS.LUMI_ACQ_DONE:
                    {
                        AddResults("Acquisition Status = LUMI_ACQ_DONE\n");
                    } break;
                case LumiSDKWrapper.LUMI_ACQ_STATUS.LUMI_ACQ_PROCESSING_DONE:
                    {
                        //AddResults("Acquisition Status = LUMI_ACQ_PROCESSING_DONE\n");
                    } break;
                case LumiSDKWrapper.LUMI_ACQ_STATUS.LUMI_ACQ_BUSY:
                    {

                    } break;
                case LumiSDKWrapper.LUMI_ACQ_STATUS.LUMI_ACQ_TIMEOUT:
                    {
                        AddResults("Acquisition Status = LUMI_ACQ_TIMEOUT\n");
                    } break;
                case LumiSDKWrapper.LUMI_ACQ_STATUS.LUMI_ACQ_NO_FINGER_PRESENT:
                    {
                    } break;
                case LumiSDKWrapper.LUMI_ACQ_STATUS.LUMI_ACQ_MOVE_FINGER_UP:
                    {
                    } break;
                case LumiSDKWrapper.LUMI_ACQ_STATUS.LUMI_ACQ_MOVE_FINGER_DOWN:
                    {
                    } break;
                case LumiSDKWrapper.LUMI_ACQ_STATUS.LUMI_ACQ_MOVE_FINGER_LEFT:
                    {
                    } break;
                case LumiSDKWrapper.LUMI_ACQ_STATUS.LUMI_ACQ_MOVE_FINGER_RIGHT:
                    {
                    } break;
                case LumiSDKWrapper.LUMI_ACQ_STATUS.LUMI_ACQ_FINGER_POSITION_OK:
                    {
                    } break;
                case LumiSDKWrapper.LUMI_ACQ_STATUS.LUMI_ACQ_CANCELLED_BY_USER:
                    {

                    } break;
                case LumiSDKWrapper.LUMI_ACQ_STATUS.LUMI_ACQ_TIMEOUT_LATENT:
                    {
                        AddResults("Acquisition Status = LUMI_ACQ_TIMEOUT_LATENT\n");
                    } break;
                case LumiSDKWrapper.LUMI_ACQ_STATUS.LUMI_ACQ_NOOP:
                    {
                    } break;
                default:
                    {
                        AddResults("Acquisition Status is undefined!!!!\n");
                    } break;
            }
            return 0;
        }
        // Callback function for Preview
        public void PreviewCallback(IntPtr pOutputImage, int width, int height, int imgNum)
        {
            int n = width * height * 3;//actually a color image
            byte[] image = new byte[n];
            Marshal.Copy(pOutputImage, image,0,n);
            SetImage(ref image, (uint)width, (uint)height);
        }
        // Draws arrows onto Graphics object based upon the status returned in the PresenceDetectionCallback
        private void DrawArrow(Graphics graphics, int status)
        {
            if (m_startTime == 0)
            {
                m_startTime = DateTime.Now.Ticks;
            }

            if ((DateTime.Now.Ticks - m_startTime) >= m_nBlinkRate)
            {
                m_startTime = 0;
                if (m_bShowArrows)
                {
                    m_bShowArrows = false;
                    m_nBlinkRate = BLINK_RATE / 3;
                }
                else
                {
                    m_bShowArrows = true;
                    m_nBlinkRate = BLINK_RATE;
                }
            }

            if (m_bShowArrows || ((LumiSDKWrapper.LUMI_ACQ_STATUS)status == LumiSDKWrapper.LUMI_ACQ_STATUS.LUMI_ACQ_FINGER_POSITION_OK) ||
                ((LumiSDKWrapper.LUMI_ACQ_STATUS)status == LumiSDKWrapper.LUMI_ACQ_STATUS.LUMI_ACQ_DONE) ||
                ((LumiSDKWrapper.LUMI_ACQ_STATUS)status == LumiSDKWrapper.LUMI_ACQ_STATUS.LUMI_ACQ_PROCESSING_DONE))
            {
                Pen arrowPen = new Pen(Color.Red, 10);
                Pen redCircle = new Pen(Color.Red, 11);
		        Pen greenCircle = new Pen(Color.Green, 11);
                Pen blueCircle = new Pen(Color.Blue, 11);
                arrowPen.CustomEndCap = new AdjustableArrowCap(3, 3, true);
                switch ((LumiSDKWrapper.LUMI_ACQ_STATUS)status)
                {
                    case LumiSDKWrapper.LUMI_ACQ_STATUS.LUMI_ACQ_DONE:
                    case LumiSDKWrapper.LUMI_ACQ_STATUS.LUMI_ACQ_PROCESSING_DONE:
                        {
                            graphics.DrawEllipse(blueCircle, 151, 237, 50, 50);
                        } break;
                    case LumiSDKWrapper.LUMI_ACQ_STATUS.LUMI_ACQ_FINGER_POSITION_OK:
                        {
                            graphics.DrawEllipse(greenCircle, 151, 237, 50, 50);
                        } break;
                    case LumiSDKWrapper.LUMI_ACQ_STATUS.LUMI_ACQ_MOVE_FINGER_UP:
                        {
                            graphics.DrawLine(arrowPen, new Point(176, 250), new Point(176, 200));
                        } break;
                    case LumiSDKWrapper.LUMI_ACQ_STATUS.LUMI_ACQ_MOVE_FINGER_DOWN:
                        {
                            graphics.DrawLine(arrowPen, new Point(176, 270), new Point(176, 320));
                        } break;
                    case LumiSDKWrapper.LUMI_ACQ_STATUS.LUMI_ACQ_MOVE_FINGER_LEFT:
                        {
                            graphics.DrawLine(arrowPen, new Point(166, 260), new Point(106, 260));
                        } break;
                    case LumiSDKWrapper.LUMI_ACQ_STATUS.LUMI_ACQ_MOVE_FINGER_RIGHT:
                        {
                            graphics.DrawLine(arrowPen, new Point(186, 260), new Point(236, 260));
                        } break;
                    case LumiSDKWrapper.LUMI_ACQ_STATUS.LUMI_ACQ_NO_FINGER_PRESENT:
                        {
                            graphics.DrawEllipse(redCircle, 151, 237, 50, 50);
                        } break;
                }
            }
        }

        private void ImageFrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Close the scanner
            SDKBiometrics.CloseScanner();
        }

        private void ChangeTimeout_Click(object sender, EventArgs e)
        {
            // NOTE: The trigger timeout will not be set on the device until a 
            // command that directly interacts with the sensor is called.  In the
            // case of this CSharp example, either the capture or match commands
            // will suffice.
            LumiSDKWrapper.LUMI_CONFIG config = new LumiSDKWrapper.LUMI_CONFIG();
            SDKBiometrics.GetConfig(ref config);
            config.nTriggerTimeout = uint.Parse(this.txtTriggerTimeout.Text);
            SDKBiometrics.SetConfig(config);
            SDKBiometrics.GetConfig(ref config);
            this.txtTriggerTimeout.Text = config.nTriggerTimeout.ToString();
        }
        public void PrintVersion(LumiSDKWrapper.LUMI_VERSION ver)
        {
            AddResults("Version Info SDK(");
            AddResults(ver.sdkVersion);
            AddResults(") FW(");
            AddResults(ver.fwrVersion);
            AddResults(") PROC(");
            AddResults(ver.prcVersion);
            AddResults(") CONF(");
            AddResults(ver.tnsVersion);
            AddResults(")\n");
        }
        public void PrintDeviceCaps(LumiSDKWrapper.LUMI_DEVICE_CAPS caps)
        {
            AddResults("*** Device Caps ***");
            AddResults("\r\nbImageCapture:\t\t");
            AddResults(caps.bCaptureImage.ToString());
            AddResults("\r\nbExtract:\t\t\t");
            AddResults(caps.bExtract.ToString());
            AddResults("\r\nbMatch:\t\t\t");
            AddResults(caps.bMatch.ToString());
            AddResults("\r\nbIdentify:\t\t\t");
            AddResults(caps.bIdentify.ToString());
            AddResults("\r\nbSpoof:\t\t\t");
            AddResults(caps.bSpoof.ToString());
            AddResults("\r\neTemplate:\t\t");
            AddResults(caps.eTemplate.ToString());
            AddResults("\r\neTransInfo:\t\t");
            AddResults(caps.eTransInfo.ToString());
            AddResults("\r\nWidth:\t\t\t");
            AddResults(caps.m_nWidth.ToString());
            AddResults("\r\nHeight:\t\t\t");
            AddResults(caps.m_nHeight.ToString());
            AddResults("\r\nDPI:\t\t\t");
            AddResults(caps.m_nDPI.ToString());
            AddResults("\r\nImage Format:\t\t");
            AddResults(caps.m_nImageFormat.ToString());
            AddResults("\r\neProcessLocation:\t");
            AddResults(caps.eProcessLocation.ToString());
            AddResults("\r\n");
        }
        private void btnTestAPI_Click(object sender, EventArgs e)
        {

            SDKBiometrics.TestAPI(this);
            lumiPictureBox.Image = null;

        }
    }
}