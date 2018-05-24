using System;

namespace SDKEnrollApp
{
    public class Ansi378TemplateHelper
    {
        private int _maxLengthPos = 8;
        private int _minutiaeItemSize = 6;
        private byte[] _template;
        private int _templateSize;
        private Minutiae[] _minutiaeList;

        public Ansi378TemplateHelper(byte[] template, int templateSize)
        {
            _template = new byte[templateSize];
            _templateSize = templateSize;
            _template = template;
        }

        public Minutiae[] GetMinutiaeList()
        {
            ExtractAnsi378Template();
            return _minutiaeList;
        }

        private void ExtractAnsi378Template()
        {
            int numOfMinutiae = UnsignedByteToInt(_template[GetDataStartPosition()]);
            _minutiaeList = new Minutiae[numOfMinutiae];

            for (int i = 0; i < numOfMinutiae; i++)
            {
                _minutiaeList[i] = UnpackMinutiaeDataItem(i);
            }
        }
        public static int UnsignedByteToInt(byte b)
        {
            return b & 0xFF;
        }

        private int GetDataStartPosition()
        {
            int pos = _maxLengthPos;
            int templateSize = _template[pos++] << 8;
            templateSize += _template[pos++];
            if (templateSize == 0)
            {
                templateSize = (_template[pos] << 24)
                           + (_template[pos + 1] << 16)
                           + (_template[pos + 2] << 8)
                           + _template[pos + 3];
                pos += 4;
            }
            pos += 19;

            return pos;
        }

        Minutiae UnpackMinutiaeDataItem(int n)
        {
            Minutiae minutiae = new Minutiae();
            int pos = GetDataStartPosition() + n * _minutiaeItemSize + 1;
            minutiae.NType = (UnsignedByteToInt(_template[pos]) >> 6);
            minutiae.NX = ((UnsignedByteToInt(_template[pos]) & 0x03f) << 8) + UnsignedByteToInt(_template[pos + 1]);
            pos += 2;
            minutiae.NY = ((UnsignedByteToInt(_template[pos]) & 0x03f) << 8) + UnsignedByteToInt(_template[pos + 1]);
            pos += 2;
            int angle = UnsignedByteToInt(_template[pos++]) << 1;
            minutiae.NRotAngle = (angle * Math.PI) / 180;
            return minutiae;
        }
    }
}
