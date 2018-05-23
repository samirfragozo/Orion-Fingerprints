VB6 Sample Code Readme.
--------------------------

The sample source builds in Visual Basic 6.0.

The VB6Example.exe uses relative paths and will work only in it's installed folder.
If you whish to move the VB6Example.exe to another folder, you will need to change 
the paths defined in the lumiFunctions.bas and recompile the exe.

Description of App:
The VB6 Demo app is a simple GUI that has an image control to display images and 
six buttons.  It is meant to provide examples on how to use the LumiSDK for 
developers who are developing software in Visual Basic 6.0.  The Snap Shot" button 
just takes a picture from the sensor.  The "Capture Image" button takes a 
capture of a finger print and returns a composite image.  The "Capture Template"
button returns a composite image and a template.  The "Match" does two template 
captures and matches them and returns a match score.  The "Get Model Information" button 
returns model info on the sensor and SDK version.  When the app is closed, the
Lumi Sensors are closed.