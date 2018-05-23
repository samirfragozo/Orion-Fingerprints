CmdLnExample Code Readme.
--------------------------
 * You are free to use this example code to generate similar functionality  
 * tailored to your own specific needs.  
 
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


The sample source builds in Visual Studio .NET 2005.

To build for the x64 Lumidigm SDK binaries, create a new active solution platform
for x64.

The sample source tests the LumiAPI.

Be sure to set your working directory to ../../bin

Be sure there is a C:\temp folder on the PC.  If C:\temp
does not exist, the calls to LumiSaveLastCapture will be 
unable to save encrypted data.  If you want to use a 
different folder to save last captured data, change the 
call to LumiSetDCOptions() to use a different folder.


