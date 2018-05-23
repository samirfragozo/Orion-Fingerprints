CSharpExample Code Readme
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

If you are building on Windows 7 64-bit:
1) If Lumidigm SDK is 64-bit, then the default Solution Platform of "Any CPU" should
   build this C# example for the 64-bit platform.
2) If the Lumidigm SDK is 32-bit, then use the configuration manager to set both C#
   projects in this example to the x86 platform.

If you are building on Windows 7 (32 or 64 bit), and you are building withing the
Lumidigm SDK default install location:
1) Open Visual Studio by right clicking and "Run as Administrator."  Then open the
   CSharpExample solution.  This is because when you build the solution, files will
   be generated within the Program Files folder and you need administrative rights
   to do this.

!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
!!! IMPORTANT !!!!    !!! IMPORTANT !!!!   !!! IMPORTANT !!!!

     Durring run time, the CSharpExample expects that the Lumidigm SDK was installed  
     in the default directory.  If this is not the case, you may need to change the 
     LoadLumiSDK method in the Program.cs source file.
     
!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

Be sure that "Do not reference mscorlib.dll" is NOT checked in the Advanced Build Settings of 
the Project properties of the CSharpExample project and the SDKHelper project.

Description of App:
The C# Demo app is a simple GUI that has an image control to display images,
an output window to display messages, three buttons, and a text box to display
and set the trigger timeout.  The description of the controls is below: 

  "Lumi Capture w/ Presence Detection Feedback" captures a composite image and displays 
  it in the image control.  During the capture, feedback from the sensors is displayed 
  in the image control.  A live "video stream" of about 10 frames per second is displayed 
  along with finger placement feedback.  In the code a threading model is implemented in 
  order to display the feedback and perform the capture.
	NOTE: Finger placement feedback is only available on Lumidigm Venus sensors.
  
  "Match" button performs two captures as described for the "Lumi Capture w/ Presence 
  Detection Feedback" button.  The extracted minutia templates are then matched and a 
  match score is displayed.  Note that this "Match" only works with sensors that are 
  enabled to extract templates.  Otherwise an error message will be returned.
  
  The "Trigger Timout" text box and "Change Timeout" button allow the user to change
  the timeout of captures.  The default timeout for Lumidigm sensors is 15 seconds.
	NOTE: The trigger timeout will not be set on the device until a 
	command that directly interacts with the sensor is called.  In the
	case of this CSharp example, either the capture or match commands
	will suffice.
  



  
  