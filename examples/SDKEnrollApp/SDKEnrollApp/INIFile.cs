using System.Runtime.InteropServices;
using System.Text;

class IniFile
{

    private string _filePath;

    [DllImport("kernel32")]
    private static extern long WritePrivateProfileString(string section,
    string key,
    string val,
    string filePath);

    [DllImport("kernel32")]
    private static extern int GetPrivateProfileString(string section,
    string key,
    string def,
    StringBuilder retVal,
    int size,
    string filePath);

    public IniFile(string filePath)
    {
        this._filePath = filePath;
    }

    public void Write(string section, string key, string value)
    {
        WritePrivateProfileString(section, key, value.ToLower(), _filePath);
    }

    public string Read(string section, string key)
    {
        StringBuilder sb = new StringBuilder(255);
        int i = GetPrivateProfileString(section, key, "", sb, 255, _filePath);
        return sb.ToString();
    }

    public string FilePath
    {
        get { return _filePath; }
        set { _filePath = value; }
    }
}
  