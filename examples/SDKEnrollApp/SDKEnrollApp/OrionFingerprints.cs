using System;
using System.Drawing;
using System.IO;
using Innovatrics.IDKit;
using Innovatrics.IDKit.Enums;
using Innovatrics.ISegLib;
using Innovatrics.ISegLib.Enums;
using Innovatrics.Sdk.Commons;
using Innovatrics.Sdk.Commons.Enums;
using isoansi = Innovatrics.AnsiIso;
using isoansie = Innovatrics.AnsiIso.Enums;


namespace SDKEnrollApp
{
    public class OrionFingerprints
    {
        //Vars
        private byte[] _isegLibLicense;
        private byte[] _ansiLibLicense;
        private byte[] _idkitLibLicense;

        private isoansi.IEngine _iEngine;
        private ISegLib _iSegLib;

        private isoansi.Ansi _ansi;

        private RawImage _image1;
        private RawImage _image2;

        //Image vars
        private int _cfgDpi = 350;
        private int _minimumFingersCount = 1;
        private int _maximumFingersCount = 1;
        private int _expectedFingersCount = 1;
        private int _maxRotation = 40;
        private int _outWidth = 400;
        private int _outHeight = 500;
        private byte _bcgValue = 255;

        //Constructor

        //Setters
        public void SetIsegLibLicense(string licenseFile)
        {
            _isegLibLicense = File.ReadAllBytes(licenseFile);
        }

        public void SetAnsiLibLicense(string licenseFile)
        {
            _ansiLibLicense = File.ReadAllBytes(licenseFile);
        }
        public void SetIdkitLibLicense(string licenseFile)
        {
            _idkitLibLicense = File.ReadAllBytes(licenseFile);
        }

        public void SetBasicParameters(int minFingersCount, int maxFingersCount, int expectedFingers, int iMaxRotation, int outW, int outH, byte bcg)
        {
            _minimumFingersCount = minFingersCount;
            _maximumFingersCount = maxFingersCount;
            _expectedFingersCount = expectedFingers;
            _maxRotation = iMaxRotation;
            _outWidth = outW;
            _outHeight = outH;
            _bcgValue = bcg;
        }

        public void SetDpi(int dpi)
        {
            _cfgDpi = dpi;
        }

        //Getters
        public byte[] GetIsegLibLicense()
        {
            return _isegLibLicense;
        }

        public byte[] GetAnsiLibLicense()
        {
            return _ansiLibLicense;
        }
        public byte[] GetIdkitLibLicense()
        {
            return _idkitLibLicense;
        }

        //Methods
        public void Initialize()
        {
            //Instance Innovatrics libs
            _iEngine = isoansi.IEngine.Instance;
            _iEngine.SetLicenseContent(_ansiLibLicense);

            _iSegLib = ISegLib.Instance;
            _iSegLib.SetLicenseContent(_isegLibLicense);

            //Init objects
            _iEngine.Init();
            _iSegLib.Init();

            _ansi = isoansi.Ansi.Instance;
        }

        public void SetImages(string route1, string route2)
        {
            Bitmap img1 = (Bitmap)Image.FromFile(route1, false);
            Bitmap img2 = (Bitmap)Image.FromFile(route2, false);
            _image1 = RawImageExtension.ConvertImageToRaw(img1);
            _image2 = RawImageExtension.ConvertImageToRaw(img2);
        }

        private void Segmentate(RawImage image, string slapOut, string prefix, string extension = ".bmp", bool single = false)
        {
            using (SegmentationResult result = _iSegLib.SegmentFingerprints(image, _expectedFingersCount, _minimumFingersCount, _maximumFingersCount, _maxRotation, SegmentationOptions.None, _outWidth, _outHeight, _bcgValue))
            {
                Image boxedImage = result.BoxedImage;
                boxedImage.Save(slapOut);

                var size = single ? 1 : result.Fingerprints.Length;

                for (int i = 0; i < size; i++)
                {
                    var fingerprintseg = result.Fingerprints[i];
                    string fileName = prefix + (i + 1) + extension;
                    using (Image fpImage = fingerprintseg.RawImage.ConvertRawToImage())
                    {
                        fpImage.Save(fileName);
                    }
                }
            }
        }

        public void Segmentate(string slapOut, string slapOut2, string prefix, string prefix2, string extension = ".bmp")
        {
            Segmentate(_image1, slapOut, prefix, extension);
            Segmentate(_image2, slapOut2, prefix2, extension);
        }


        public int MatchImages(string route1, string route2, string method = "iso", string source = "original", string path = "")
        {

            //iso create template1
            isoansi.Ansi.Instance.CreateTemplate(_image1, path + "skth1.bmp", path + "bnh1.bmp", path + "mnth1.bmp");
            isoansi.Ansi.Instance.CreateTemplate(_image2, path + "skth2.bmp", path + "bnh2.bmp", path + "mnth2.bmp");


            switch (source)
            {
                case "original":
                    _image1 = isoansi.RawImageExtension.LoadBmp(route1);
                    _image2 = isoansi.RawImageExtension.LoadBmp(route2);
                    break;
                case "skeleton":
                    _image1 = isoansi.RawImageExtension.LoadBmp(path + "skth1.bmp");
                    _image2 = isoansi.RawImageExtension.LoadBmp(path + "skth2.bmp");
                    break;
                case "binary":
                    _image1 = isoansi.RawImageExtension.LoadBmp(path + "bnh1.bmp");
                    _image2 = isoansi.RawImageExtension.LoadBmp(path + "bnh2.bmp");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var ansiTemplate1 = _ansi.CreateTemplate(_image1, _cfgDpi);
            var ansiTemplate2 = _ansi.CreateTemplate(_image2, _cfgDpi);

            if (method != "iso") return _ansi.VerifyMatch(ansiTemplate1, ansiTemplate2, 180);
            var isoTemplate1 = _ansi.ConvertToISO(ansiTemplate1);
            byte[] isoTemplate2 = _ansi.ConvertToISO(ansiTemplate2);
            return isoansi.Iso.Instance.VerifyMatch(isoTemplate1, isoTemplate2, 180);

        }

        public int MatchIdkit(string randomRoute, string route1, string route2, string database, string path = "")
        {
            IDKit.InitWithLicense(_idkitLibLicense);
            var connection = new Innovatrics.IDKit.Connection();

            IDKit.SetParameter(ConfigParameter.CfgResolutionDpi, _cfgDpi);
            IDKit.SetParameter(ConfigParameter.CfgSimilarityThreshold, 1);
            IDKit.GetUserLimit();
            connection.Connect(database);
            var user = IDKit.InitUser();

            IDKit.AddFingerprintFromFile(user, FingerPosition.LeftIndex, randomRoute);
            IDKit.AddFingerprintFromFile(user, FingerPosition.RightIndex, route1);
            var userId = connection.RegisterUser(user);
            IDKit.ClearUser(user);
            IDKit.AddFingerprintFromFile(user, FingerPosition.Unknown, route2);

            var matchResult = connection.MatchFingerprint(user, 0, userId);
            var result = matchResult.Score;
            user = connection.GetUser(userId);
            IDKit.GetFingerPosition(user, matchResult.Index);
            IDKit.SaveFingerprintImage(user, matchResult.Index, ImageFormat.Bmp, path + "found.bpm");
            IDKit.FreeUser(user);
            connection.Close();
            IDKit.TerminateModule();
            return result;
        }

        public int SingleSegmentateWithMatch(string method = "iso", string source = "original")
        {
            Segmentate(_image1, "slapOut1", "finger1", ".bmp", true);
            Segmentate(_image2, "slapOut2", "finger2", ".bmp", true);

            return MatchImages("finger11.bmp", "finger21.bmp", method, source);
        }

    }
}
