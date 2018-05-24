/***************************************************************************************/
// ©Copyright 2016 HID Global Corporation/ASSA ABLOY AB. ALL RIGHTS RESERVED.
//
// For a list of applicable patents and patents pending, visit www.hidglobal.com/patents/
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS 
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER 
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN 
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
//
/***************************************************************************************/

using System.Runtime.InteropServices;

namespace SDKWrapper
{
    public static class LumiSdkLumiSetOption
    {
        // LumiAPI.dll is loaded in the Main() function of the Program.cs for the 
        // CSharpExample.exe
        // LumiSetOption declared for the Override trigger int pointer
        [DllImport(LumiSdkWrapper.LumiApiDll)]
        static extern LumiSdkWrapper.LumiStatus LumiSetOption(uint hHandle,
                                                    LumiSdkWrapper.LumiOptions option,
                                                    ref int pArgument,
                                                    uint nArgumentSize);

        public static LumiSdkWrapper.LumiStatus OverrideTrigger(uint handle, bool overrideTrigger)
        {
            int argument = 0;
            if (overrideTrigger) argument = 1;
            return LumiSetOption(handle, LumiSdkWrapper.LumiOptions.LumiOptionSetOverrideHeartbeatDisplay, ref argument, sizeof(int));  
        }

        public static LumiSdkWrapper.LumiStatus SetPresenceDetectionMode(uint handle, LumiSdkWrapper.LumiPresDetMode pdMode)
        {
            int argument = (int)pdMode;
            return LumiSetOption(handle, LumiSdkWrapper.LumiOptions.LumiOptionSetPresenceDetMode, ref argument, sizeof(int));  
        }

        public static LumiSdkWrapper.LumiStatus SetPresenceDetectionThreshold(uint handle, LumiSdkWrapper.LumiPresDetThresh thresh)
        {
            int argument = (int)thresh;
            return LumiSetOption(handle, LumiSdkWrapper.LumiOptions.LumiOptionSetPresenceDetThresh, ref argument, sizeof(int)); 
        }
    }
}
