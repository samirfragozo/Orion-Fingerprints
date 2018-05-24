using System;
using System.Collections;

namespace SDKEnrollApp
{
    [Serializable]
    public class SubjectData
    {

        public ArrayList Fingers;
        public ArrayList Templates;
        public ArrayList TemplateLengths;

        public SubjectData()
        {
            Fingers = new ArrayList();
            Templates = new ArrayList();
            TemplateLengths = new ArrayList();
        }
        public void AddData(byte[] fingerTemplate, uint fingerTemplateLen, int fingerIndex)
        {
            Templates.Add(fingerTemplate);
            TemplateLengths.Add(fingerTemplateLen);
            Fingers.Add(fingerIndex);
        }
        public void DeleteData(int fingerIndex)
        {
            int index;
            index = Fingers.IndexOf(fingerIndex);
            if (index >= 0)
            {
                Fingers.RemoveAt(index);
                Templates.RemoveAt(index);
                TemplateLengths.RemoveAt(index);
            }
        }
        public void Clear()
        {
            Fingers.Clear();
            TemplateLengths.Clear();
            Templates.Clear();
        }

    }
}