#pragma once

/**********************************************/
/*  Simple Data Types
/**********************************************/
typedef unsigned char	uchar;
typedef unsigned int	uint;
typedef unsigned long   LUMI_HANDLE;
typedef unsigned long   LUMI_DEVICE_HANDLE;

/**********************************************/
/*  Error Definitons
/**********************************************/

typedef enum tagLUMI_ERROR
{
	LUMI_STATUS_OK							= 0,			/* Operation completed successfully */
	LUMI_STATUS_ERROR_DEVICE_OPEN			= 0x0001,		/* Could not find or open requested
																biometric reader */
	LUMI_STATUS_ERROR_DEVICE_CLOSE			= 0x0002,		/* Could not close biometric reader
																or release allocated resources */
	LUMI_STATUS_ERROR_CMD_NOT_SUPPORTED		= 0x0004,		/* This command is not supported on
																current platform */
	LUMI_STATUS_ERROR_COMM_LINK				= 0x0008,		/* General internal communication problem
																prevented execution of command */
	LUMI_STATUS_ERROR_PREPROCESSOR			= 0x0010,		/* Minimum thresholds for processing
																quality were not achieved */
	LUMI_STATUS_ERROR_CALIBRATION			= 0x0020,		/* Calibration cycle failed due to
																inoperable hardware state */
	LUMI_STATUS_ERROR_BUSY					= 0x0040,		/* Device is busy processing previous
																command */
	LUMI_STATUS_ERROR_INVALID_PARAMETER		= 0x0080,		/* Parameter or input is out of range */
	LUMI_STATUS_ERROR_TIMEOUT				= 0x0100,		/* Event did not occur within programmed
																time interval */
	LUMI_STATUS_ERROR_INVALID_TEMPLATE		= 0x0200,		/* Supplied input template is invalid */
	LUMI_STATUS_ERROR_MEMORY_ALLOCATION		= 0x0400,		/* Unable to allocate memory */
	LUMI_STATUS_ERROR_INVALID_DEVICE_ID		= 0x0800,		/* Invalid connection handle */
	LUMI_STATUS_ERROR_INVALID_CONNECTION_ID	= 0x1000,		/* Invalid instance ID */
	LUMI_STATUS_ERROR_CONFIG_UNSUPPORTED    = 0x2000,		/* Current configuration or policy does not
																support the function that was just called */
	LUMI_STATUS_UNSUPPORTED					= 0x4000,		/* The function is not currently supported*/
	LUMI_STATUS_INTERNAL_ERROR				= 0x8000,		/* An internal error occurred */
	LUMI_STATUS_INVALID_PARAMETER			= 0x8001,		/* Invalid parameter or non allocated buffer passed to function*/
	LUMI_STATUS_DEVICE_TIMEOUT				= 0x8002,		/* Biometric sensor is accessed by another process */
	LUMI_STATUS_INVALID_OPTION				= 0x8004,		/* Invalid option, invalid argument, or invalid
																argument size passed to function LumiSetOption*/
	LUMI_STATUS_ERROR_MISSING_SPOOFTPL      = 0x8008,       /* Missing Spoof template from Composite image or ANSI
																378 Template */
	LUMI_STATUS_CANCELLED_BY_USER			= 0x8010,		/* If the LumiPresenceDetectCallback returns -2, then
																the SDK will return this status code during capture*/
	LUMI_STATUS_INVALID_FOLDER				= 0x8020,		/* Function LumiSetDCOptions returns this code if the folder
																doesn't exist */
	LUMI_STATUS_DC_OPTIONS_NOT_SET			= 0x8040,		/* Function LumiSaveLastCapture returns this code if the
																(Data Collection) option was not set OR set to a non
																existing folder */
	LUMI_STATUS_INCOMPATIBLE_FIRMWARE		= 0x8080,		/* The firmware loaded on the sensor is not compatible with
															     this version of the SDK. */
    LUMI_STATUS_NO_DEVICE_FOUND				= 0x8100,		/* LumiInOpAPI returns this code if no device was found*/
	LUMI_STATUS_ERROR_READ_FILE				= 0x8200,		/* LumiInOpAPI returns this code if could not read from file*/
	LUMI_STATUS_ERROR_WRITE_FILE			= 0x8400,		/* LumiInOpAPI returns this code if could not write to file*/
	LUMI_STATUS_INVALID_FILE_FORMAT			= 0x8800,       /* LumiInOpAPI returns this code if invalid file format is passed. */
	LUMI_STATUS_ERROR_TIMEOUT_LATENT		= 0x9000,		/* Latent detection on sensor returned time out due to possible latent
															     or finger not moving between capture calls or PD cycles*/
	LUMI_STATUS_QUALITY_MAP_NOT_GENERATED	= 0x9010,		/* If LumiGetQualityMap is called before a LumiCapture, LumiCaptureEX or
															   LumiVerify is called, this code returned.  Also, this code is returned if 
															   a NULL buffer is passed into LumiGetQualityMap.*/
    LUMI_STATUS_THREAD_ERROR				= 0x9011,		/* More than one thread from same process attempting to enter SDK */
	LUMI_STATUS_ERROR_SENSOR_COMM_TIMEOUT	= 0x9012,		/* A serious communication error occurred with the sensor.  The connection handle
															   to that sensor will be closed and further function calls using that connection
															   handle will return LUMI_STATUS_ERROR_INVALID_DEVICE_ID.*/
	LUMI_STATUS_DEVICE_STATUS_ERROR			= 0x9013,		/* LumiSetOption for the LUMI_OPTION_CHECK_SENSOR_HEALTH option will return
															   this error code if a sensor status error is present.*/
    LUMI_STATUS_ERROR_BAD_INSTALLATION		= 0x9014,		/* If Lumidigm SDK files and folders are not in their proper relative paths or missing,
															   this error code will be returned.  If the plugin folder is missing, this error
															   code will be returned.  If the SPM folder is missing, this error is returned.
															   NOTE: This error code may be returned after the a call to the LumiQueryNumberDevices 
															   function or from other functions - depending on the situation.
															   NOTE: If the MercuryDvc.dll, SDvc.dll, or the VenusDvc.dll are removed from
															   the plugin folder, no error will be returned.  However, if you have a Lumidigm
															   sensor that requires one of these dlls, the LumiQueryNumberDevices function
															   will return 0 devices.
															   As of August 2011, the MercuryDvc.dll is required by the M300 and M301 sensors,
															   the SDvc.dll is required by the M31x and the V31x sensors, the VenusDvc.dll is
															   required by the V300 and V301 sensors.	
															   */
} LumiStatus;

/**********************************************/
/*  Hardware Configuration
/*	Defines transport layer
/**********************************************/

typedef enum _tagTransport
{
	CONF_TRANS_DEFAULT	 = 0x0000,
	CONF_TRANS_USB       = 0x0001,
	CONF_TRANS_ETHERNET  = 0x0002,
	CONF_TRANS_RS232	 = 0x0004,
	CONF_TRANS_WYGEN     = 0x0008,
	CONF_TRANS_END		 = 0xFFFF
} TransportInfo;


/**********************************************/
/*  Algorithmic Configuration
/*	Defines where processing executes
/**********************************************/

typedef enum _tagProcFlag
{
	CONF_PROCESS_DSP		= 0x0001,		/* Generate Template DSP */
	CONF_PROCESS_LOCAL		= 0x0002,		/* Generate Template Local */
} ProcFlag;

/**********************************************/
/*  Template Types
/**********************************************/
typedef enum _tagTplType
{
	CONF_TPL_DEFAULT			= 0x0000,		/* Template type unknown */
	CONF_TPL_NEC_3				= 0x0001,
	CONF_TPL_NEC_4				= 0x0002,
	CONF_TPL_ANSI378			= 0x0003,
	END_TEMPLATES
} TplInfo;


/**********************************************/
/*  Current Device State
/**********************************************/

typedef enum tagLUMI_DEVICE_FLAGS
{
	DEVICE_STATE_UNKNOWN = 0,
	DEVICE_STATE_OK,
	DEVICE_STATE_BUSY,
	DEVICE_STATE_ERROR
} LUMI_DEVICE_FLAG;

typedef struct tagLUMI_DEVICE_STATE
{
	LUMI_DEVICE_FLAG eFlag;
	uint Device_Temp;
	uint Trigger_Timeout;
} LUMI_DEVICE_STATE;

/**********************************************/
/*  Configuration structure (get/set)
/**********************************************/

typedef struct
{
	// Template Configuration
	TplInfo			eTemplateType;
	// Transport Configuration
	TransportInfo	eTransInfo;
	// Trigger Timeout
	uint			nTriggerTimeout;
} LUMI_CONFIG;


/********************************************************/
/*		Lumi Process Location				*/
/********************************************************/

typedef enum
{
	LUMI_PROCESS_NONE = 0,
	LUMI_PROCESS_SENSOR	= 1,
	LUMI_PROCESS_PC	= 2
} LUMI_PROCESS_LOCATION;


/**********************************************/
/*  Device capabilities
/**********************************************/

typedef struct
{
	bool					bCaptureImage;
	bool					bExtract;
	bool					bMatch;
	bool					bIdentify;
	bool					bSpoof;
	//
	TplInfo					eTemplate;			// Template Configuration
	TransportInfo			eTransInfo;			// Transport Configuration
	// Composite Image Size
	uint					m_nWidth;
	uint					m_nHeight;
	uint					m_nDPI;
	uint					m_nImageFormat;
	// Process Location
	LUMI_PROCESS_LOCATION	eProcessLocation;
} LUMI_DEVICE_CAPS;


/**********************************************/
/*  Version Information.
/**********************************************/

typedef struct
{
	char sdkVersion[256];
	char fwrVersion[256];
	char prcVersion[256];
	char tnsVersion[256];
} LUMI_VERSION;


/**********************************************/
/*		Lumidigm sensor types			      */
/**********************************************/
typedef enum
{
	VENUS	= 0,			/* Venus series sensors		*/
	M100	= 1,			/* Mercury M100 sensors		*/
	M300	= 2,			/* Mercury M300 sensors		*/
	M31X	= 3,			/* M31X sensors				*/
	V31X	= 4,			/* V31X sensors				*/
	V371	= 5,			/* V371 sensors				*/
	M32X	= 8				/* Mercury M32X sensors		*/
} LUMI_SENSOR_TYPE;

/**********************************************/
/*		LumiDevice Enumeration Structure      */
/**********************************************/

typedef struct
{
	LUMI_DEVICE_HANDLE hDevHandle;						/* Handle to Device.  Should only be used for calls to Init */
	LUMI_DEVICE_CAPS   dCaps;							/* Device Capabilities										*/
	char			   strIdentifier[256];				/* Device Identifier										*/
	LUMI_SENSOR_TYPE   SensorType;						/* Sensor Class												*/
} LUMI_DEVICE;


/********************************************************/
/*		Lumi Options (Used with LumiSetOption function)	*/
/********************************************************/

typedef enum
{
	LUMI_OPTION_SET_OVERRIDE_HEARTBEAT_DISPLAY = 0,	// Option to override Venus Sensor Heartbeat.  Pass 1 to turn off, 0 to turn on
	LUMI_OPTION_SET_PRESENCE_DET_MODE,				// Option to override Venus Sensor Presense Detect.
	LUMI_OPTION_SET_PRESENCE_DET_THRESH,			// Legacy - do not use
	LUMI_OPTION_SET_ACQ_STATUS_CALLBACK,			// Set the pointer to the LumiAcqStatusCallback function.  Pass the pointer to the LumiAcqStatusCallback function.
	LUMI_OPTION_SET_PRESENCE_DET_CALLBACK,			// Set the pointer to the LumiPresenceDetectCallback function.  Pass the pointer to the LumiPresenceDetectCallback function.
	LUMI_OPTION_SET_PRESENCE_DET_COLOR,				// Set the presence detection callback image color.  Pass 0 for grayscale, 1 for color.
	LUMI_OPTION_RESET_DEVICE,						// Reset device.  You MUST close and reopen the handle to this device after it has been reset.
	LUMI_OPTION_CHECK_SENSOR_HEALTH,				// Immediately checks for sensor status errors.
} LUMI_OPTIONS;

/********************************************************************/
/*		Lumi Presence detection settings(Leave on for 99% of cases	*/
/********************************************************************/
typedef enum
{
	LUMI_PD_ON = 0,
	LUMI_PD_OFF,
} LUMI_PRES_DET_MODE;


/****************************************************************/
/*		Lumi Presence detection thresholds (Legacy, do not use)	*/
/****************************************************************/
typedef enum
{
	LUMI_PD_SENSITIVITY_LOW = 0,
	LUMI_PD_SENSITIVITY_MEDIUM,
	LUMI_PD_SENSITIVITY_HIGH
} LUMI_PRES_DET_THRESH;


/********************************************************/
/*		Vid Stream	(used with LumiSetLiveMode 			*/
/********************************************************/

#define	LUMI_VID_OFF  0
#define LUMI_VID_ON   1

/**********************************************/
/*		Acquisition Status					  */
/**********************************************/

typedef enum
{
	LUMI_ACQ_DONE = 0,
	LUMI_ACQ_PROCESSING_DONE,
	LUMI_ACQ_BUSY,
	LUMI_ACQ_TIMEOUT,
	LUMI_ACQ_NO_FINGER_PRESENT,
	LUMI_ACQ_MOVE_FINGER_UP,
	LUMI_ACQ_MOVE_FINGER_DOWN,
	LUMI_ACQ_MOVE_FINGER_LEFT,
	LUMI_ACQ_MOVE_FINGER_RIGHT,
	LUMI_ACQ_FINGER_POSITION_OK,
	LUMI_ACQ_CANCELLED_BY_USER,
	LUMI_ACQ_TIMEOUT_LATENT,
	LUMI_ACQ_FINGER_PRESENT,
	LUMI_ACQ_SPOOF_DETECTED,
	LUMI_ACQ_NOOP = 99,	
} LUMI_ACQ_STATUS;



/**********************************************/
/*		Lumi LED Control					  */
/**********************************************/

typedef enum
{
	LUMI_JUPITER_LED_00 = 0,
	LUMI_JUPITER_LED_01,
	LUMI_JUPITER_LED_02,
	LUMI_JUPITER_LED_03,
	LUMI_JUPITER_LED_04,
	LUMI_JUPITER_LED_05,
	LUMI_JUPITER_LED_06,
	LUMI_JUPITER_LED_07,
	LUMI_JUPITER_LED_08,
	LUMI_JUPITER_LED_09,
	LUMI_JUPITER_LED_10,
	LUMI_JUPITER_LED_11,
	LUMI_VENUS_ALL_OFF = 1024,
	LUMI_VENUS_GREEN_ON,
	LUMI_VENUS_GREEN_OFF,
	LUMI_VENUS_RED_ON,
	LUMI_VENUS_RED_OFF,
	LUMI_VENUS_GREEN_CYCLE_ON,
	LUMI_VENUS_GREEN_CYCLE_OFF,
	LUMI_VENUS_RED_CYCLE_ON,
	LUMI_VENUS_RED_CYCLE_OFF
} LUMI_LED_CONTROL;


/**********************************************/
/*		LumiProcessingMode Structure          */
/**********************************************/
typedef struct
{
	bool bExtract;				/* Turn on or off minutia extraction (true = on, false = off)*/
	bool bSpoof;				/* Turn on or off spoof	(true = on, false = off)			*/
	bool bLatent;				/* Turn on or off latent detection (true = on, false = off)	*/
} LUMI_PROCESSING_MODE;


/**********************************************/
/*  Callback Types
/**********************************************/

typedef void (*LumiPreviewCallback)(unsigned char* pImage, int nWidth, int nHeight, int nNum);   // Pointer to function

typedef int (*LumiAcqStatusCallback)(uint nStatus);  // Pointer to function

typedef int (*LumiPresenceDetectCallback)(unsigned char* pImage, int nWidth, int nHeight, uint nStatus);   // Pointer to function
