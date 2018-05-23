using System;
using System.Collections;

namespace SDKEnrollApp
{
    [Serializable]
    public class SubjectData
    {

        public ArrayList fingers;
        public ArrayList templates;
        public ArrayList templateLengths;

        public SubjectData()
        {
            fingers = new ArrayList();
            templates = new ArrayList();
            templateLengths = new ArrayList();
        }
        public void addData(byte[] fingerTemplate, uint fingerTemplateLen, int fingerIndex)
        {
            templates.Add(fingerTemplate);
            templateLengths.Add(fingerTemplateLen);
            fingers.Add(fingerIndex);
        }
        public void deleteData(int fingerIndex)
        {
            int index;
            index = fingers.IndexOf(fingerIndex);
            if (index >= 0)
            {
                fingers.RemoveAt(index);
                templates.RemoveAt(index);
                templateLengths.RemoveAt(index);
            }
        }
        public void clear()
        {
            fingers.Clear();
            templateLengths.Clear();
            templates.Clear();
        }

    }
}