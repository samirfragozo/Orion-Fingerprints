using System.ComponentModel;
using System.Windows.Forms;

namespace SDKEnrollApp
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// CleanEnrollTab up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.LumiPictureBox1 = new System.Windows.Forms.PictureBox();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.OptionsTabPage = new System.Windows.Forms.TabPage();
            this._setUmbral = new System.Windows.Forms.Button();
            this.label16 = new System.Windows.Forms.Label();
            this._umbral = new System.Windows.Forms.TextBox();
            this.SpoofThresholdGroupBox = new System.Windows.Forms.GroupBox();
            this.SpoofConvenient = new System.Windows.Forms.RadioButton();
            this.SpoofSecure = new System.Windows.Forms.RadioButton();
            this.SpoofHighlySecure = new System.Windows.Forms.RadioButton();
            this.MatchThresholdGroupBox = new System.Windows.Forms.GroupBox();
            this.MatchConvenient = new System.Windows.Forms.RadioButton();
            this.MatchSecure = new System.Windows.Forms.RadioButton();
            this.MatchHighlySecured = new System.Windows.Forms.RadioButton();
            this.SensorSettingsGroupBox = new System.Windows.Forms.GroupBox();
            this.NISTQualityChkBox = new System.Windows.Forms.CheckBox();
            this.SensorTriggerArmedChkBox = new System.Windows.Forms.CheckBox();
            this.EnableSpoofDetChkBox = new System.Windows.Forms.CheckBox();
            this.EnrollmentTabPage = new System.Windows.Forms.TabPage();
            this._setDpi = new System.Windows.Forms.Button();
            this._dpi = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this._users_table = new System.Windows.Forms.DataGridView();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this._document = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this._age = new System.Windows.Forms.TextBox();
            this._birthdate = new System.Windows.Forms.DateTimePicker();
            this.label8 = new System.Windows.Forms.Label();
            this._document_type = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this._name = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this._last_name = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this._delete = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this._cancelar = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this._print3 = new System.Windows.Forms.PictureBox();
            this._print2 = new System.Windows.Forms.PictureBox();
            this._print1 = new System.Windows.Forms.PictureBox();
            this.label4 = new System.Windows.Forms.Label();
            this._fingers = new System.Windows.Forms.TextBox();
            this._guardar = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.NISTScoreLabel = new System.Windows.Forms.Label();
            this.labelStatus = new System.Windows.Forms.Label();
            this.VerificationTabPage = new System.Windows.Forms.TabPage();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.labelStatus2 = new System.Windows.Forms.Label();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this._resultsMatch = new System.Windows.Forms.TextBox();
            this._log = new System.Windows.Forms.TextBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.label17 = new System.Windows.Forms.Label();
            this._verificar = new System.Windows.Forms.Button();
            this._cancelar2 = new System.Windows.Forms.Button();
            this._fingerprint = new System.Windows.Forms.ComboBox();
            this._finger2 = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label15 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this._buscar = new System.Windows.Forms.Button();
            this._birthdate2 = new System.Windows.Forms.TextBox();
            this._document_type2 = new System.Windows.Forms.TextBox();
            this._document2 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this._age2 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this._name2 = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this._last_name2 = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.CaptureBtnClick = new System.Windows.Forms.Button();
            this.SelectSensorComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.LiveBtn = new System.Windows.Forms.Button();
            this.btnSaveImage = new System.Windows.Forms.Button();
            this.nistPictureBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.LumiPictureBox1)).BeginInit();
            this.tabControl.SuspendLayout();
            this.OptionsTabPage.SuspendLayout();
            this.SpoofThresholdGroupBox.SuspendLayout();
            this.MatchThresholdGroupBox.SuspendLayout();
            this.SensorSettingsGroupBox.SuspendLayout();
            this.EnrollmentTabPage.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._users_table)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._print3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._print2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._print1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.VerificationTabPage.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nistPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // LumiPictureBox1
            // 
            this.LumiPictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.LumiPictureBox1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("LumiPictureBox1.BackgroundImage")));
            this.LumiPictureBox1.Location = new System.Drawing.Point(1133, 93);
            this.LumiPictureBox1.Margin = new System.Windows.Forms.Padding(4);
            this.LumiPictureBox1.Name = "LumiPictureBox1";
            this.LumiPictureBox1.Size = new System.Drawing.Size(353, 544);
            this.LumiPictureBox1.TabIndex = 0;
            this.LumiPictureBox1.TabStop = false;
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.OptionsTabPage);
            this.tabControl.Controls.Add(this.EnrollmentTabPage);
            this.tabControl.Controls.Add(this.VerificationTabPage);
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Margin = new System.Windows.Forms.Padding(4);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(1125, 787);
            this.tabControl.TabIndex = 1;
            this.tabControl.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // OptionsTabPage
            // 
            this.OptionsTabPage.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("OptionsTabPage.BackgroundImage")));
            this.OptionsTabPage.Controls.Add(this._setUmbral);
            this.OptionsTabPage.Controls.Add(this.label16);
            this.OptionsTabPage.Controls.Add(this._umbral);
            this.OptionsTabPage.Controls.Add(this.SpoofThresholdGroupBox);
            this.OptionsTabPage.Controls.Add(this.MatchThresholdGroupBox);
            this.OptionsTabPage.Controls.Add(this.SensorSettingsGroupBox);
            this.OptionsTabPage.Location = new System.Drawing.Point(4, 25);
            this.OptionsTabPage.Margin = new System.Windows.Forms.Padding(4);
            this.OptionsTabPage.Name = "OptionsTabPage";
            this.OptionsTabPage.Padding = new System.Windows.Forms.Padding(4);
            this.OptionsTabPage.Size = new System.Drawing.Size(1117, 758);
            this.OptionsTabPage.TabIndex = 0;
            this.OptionsTabPage.Text = "Options";
            this.OptionsTabPage.UseVisualStyleBackColor = true;
            // 
            // _setUmbral
            // 
            this._setUmbral.Location = new System.Drawing.Point(147, 322);
            this._setUmbral.Name = "_setUmbral";
            this._setUmbral.Size = new System.Drawing.Size(266, 33);
            this._setUmbral.TabIndex = 8;
            this._setUmbral.Text = "Umbral";
            this._setUmbral.UseVisualStyleBackColor = true;
            this._setUmbral.Click += new System.EventHandler(this._setUmbral_Click);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(13, 307);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(53, 17);
            this.label16.TabIndex = 7;
            this.label16.Text = "Umbral";
            // 
            // _umbral
            // 
            this._umbral.Location = new System.Drawing.Point(16, 327);
            this._umbral.Name = "_umbral";
            this._umbral.Size = new System.Drawing.Size(100, 22);
            this._umbral.TabIndex = 6;
            // 
            // SpoofThresholdGroupBox
            // 
            this.SpoofThresholdGroupBox.Controls.Add(this.SpoofConvenient);
            this.SpoofThresholdGroupBox.Controls.Add(this.SpoofSecure);
            this.SpoofThresholdGroupBox.Controls.Add(this.SpoofHighlySecure);
            this.SpoofThresholdGroupBox.Location = new System.Drawing.Point(9, 9);
            this.SpoofThresholdGroupBox.Margin = new System.Windows.Forms.Padding(4);
            this.SpoofThresholdGroupBox.Name = "SpoofThresholdGroupBox";
            this.SpoofThresholdGroupBox.Padding = new System.Windows.Forms.Padding(4);
            this.SpoofThresholdGroupBox.Size = new System.Drawing.Size(405, 59);
            this.SpoofThresholdGroupBox.TabIndex = 5;
            this.SpoofThresholdGroupBox.TabStop = false;
            this.SpoofThresholdGroupBox.Text = "Spoof Threshold";
            // 
            // SpoofConvenient
            // 
            this.SpoofConvenient.AutoSize = true;
            this.SpoofConvenient.Location = new System.Drawing.Point(213, 23);
            this.SpoofConvenient.Margin = new System.Windows.Forms.Padding(4);
            this.SpoofConvenient.Name = "SpoofConvenient";
            this.SpoofConvenient.Size = new System.Drawing.Size(100, 21);
            this.SpoofConvenient.TabIndex = 2;
            this.SpoofConvenient.TabStop = true;
            this.SpoofConvenient.Text = "Convenient";
            this.SpoofConvenient.UseVisualStyleBackColor = true;
            this.SpoofConvenient.CheckedChanged += new System.EventHandler(this.SpoofConvenient_CheckedChanged_1);
            // 
            // SpoofSecure
            // 
            this.SpoofSecure.AutoSize = true;
            this.SpoofSecure.Location = new System.Drawing.Point(127, 22);
            this.SpoofSecure.Margin = new System.Windows.Forms.Padding(4);
            this.SpoofSecure.Name = "SpoofSecure";
            this.SpoofSecure.Size = new System.Drawing.Size(74, 21);
            this.SpoofSecure.TabIndex = 1;
            this.SpoofSecure.TabStop = true;
            this.SpoofSecure.Text = "Secure";
            this.SpoofSecure.UseVisualStyleBackColor = true;
            this.SpoofSecure.CheckedChanged += new System.EventHandler(this.SpoofSecure_CheckedChanged_1);
            // 
            // SpoofHighlySecure
            // 
            this.SpoofHighlySecure.AutoSize = true;
            this.SpoofHighlySecure.Location = new System.Drawing.Point(7, 22);
            this.SpoofHighlySecure.Margin = new System.Windows.Forms.Padding(4);
            this.SpoofHighlySecure.Name = "SpoofHighlySecure";
            this.SpoofHighlySecure.Size = new System.Drawing.Size(117, 21);
            this.SpoofHighlySecure.TabIndex = 0;
            this.SpoofHighlySecure.TabStop = true;
            this.SpoofHighlySecure.Text = "Highly Secure";
            this.SpoofHighlySecure.UseVisualStyleBackColor = true;
            this.SpoofHighlySecure.CheckedChanged += new System.EventHandler(this.SpoofHighlySecure_CheckedChanged_1);
            // 
            // MatchThresholdGroupBox
            // 
            this.MatchThresholdGroupBox.Controls.Add(this.MatchConvenient);
            this.MatchThresholdGroupBox.Controls.Add(this.MatchSecure);
            this.MatchThresholdGroupBox.Controls.Add(this.MatchHighlySecured);
            this.MatchThresholdGroupBox.Location = new System.Drawing.Point(9, 73);
            this.MatchThresholdGroupBox.Margin = new System.Windows.Forms.Padding(4);
            this.MatchThresholdGroupBox.Name = "MatchThresholdGroupBox";
            this.MatchThresholdGroupBox.Padding = new System.Windows.Forms.Padding(4);
            this.MatchThresholdGroupBox.Size = new System.Drawing.Size(405, 59);
            this.MatchThresholdGroupBox.TabIndex = 4;
            this.MatchThresholdGroupBox.TabStop = false;
            this.MatchThresholdGroupBox.Text = "Matching Threshold";
            // 
            // MatchConvenient
            // 
            this.MatchConvenient.AutoSize = true;
            this.MatchConvenient.Location = new System.Drawing.Point(213, 23);
            this.MatchConvenient.Margin = new System.Windows.Forms.Padding(4);
            this.MatchConvenient.Name = "MatchConvenient";
            this.MatchConvenient.Size = new System.Drawing.Size(100, 21);
            this.MatchConvenient.TabIndex = 2;
            this.MatchConvenient.TabStop = true;
            this.MatchConvenient.Text = "Convenient";
            this.MatchConvenient.UseVisualStyleBackColor = true;
            this.MatchConvenient.CheckedChanged += new System.EventHandler(this.MatchConvenient_CheckedChanged_1);
            // 
            // MatchSecure
            // 
            this.MatchSecure.AutoSize = true;
            this.MatchSecure.Location = new System.Drawing.Point(127, 22);
            this.MatchSecure.Margin = new System.Windows.Forms.Padding(4);
            this.MatchSecure.Name = "MatchSecure";
            this.MatchSecure.Size = new System.Drawing.Size(74, 21);
            this.MatchSecure.TabIndex = 1;
            this.MatchSecure.TabStop = true;
            this.MatchSecure.Text = "Secure";
            this.MatchSecure.UseVisualStyleBackColor = true;
            this.MatchSecure.CheckedChanged += new System.EventHandler(this.MatchSecure_CheckedChanged_1);
            // 
            // MatchHighlySecured
            // 
            this.MatchHighlySecured.AccessibleName = "";
            this.MatchHighlySecured.AutoSize = true;
            this.MatchHighlySecured.Location = new System.Drawing.Point(7, 22);
            this.MatchHighlySecured.Margin = new System.Windows.Forms.Padding(4);
            this.MatchHighlySecured.Name = "MatchHighlySecured";
            this.MatchHighlySecured.Size = new System.Drawing.Size(117, 21);
            this.MatchHighlySecured.TabIndex = 0;
            this.MatchHighlySecured.TabStop = true;
            this.MatchHighlySecured.Text = "Highly Secure";
            this.MatchHighlySecured.UseVisualStyleBackColor = true;
            this.MatchHighlySecured.CheckedChanged += new System.EventHandler(this.MatchHighlySecure_CheckedChanged_1);
            // 
            // SensorSettingsGroupBox
            // 
            this.SensorSettingsGroupBox.Controls.Add(this.NISTQualityChkBox);
            this.SensorSettingsGroupBox.Controls.Add(this.SensorTriggerArmedChkBox);
            this.SensorSettingsGroupBox.Controls.Add(this.EnableSpoofDetChkBox);
            this.SensorSettingsGroupBox.Location = new System.Drawing.Point(8, 140);
            this.SensorSettingsGroupBox.Margin = new System.Windows.Forms.Padding(4);
            this.SensorSettingsGroupBox.Name = "SensorSettingsGroupBox";
            this.SensorSettingsGroupBox.Padding = new System.Windows.Forms.Padding(4);
            this.SensorSettingsGroupBox.Size = new System.Drawing.Size(405, 149);
            this.SensorSettingsGroupBox.TabIndex = 0;
            this.SensorSettingsGroupBox.TabStop = false;
            this.SensorSettingsGroupBox.Text = "Sensor Settings";
            // 
            // NISTQualityChkBox
            // 
            this.NISTQualityChkBox.AutoSize = true;
            this.NISTQualityChkBox.Location = new System.Drawing.Point(24, 121);
            this.NISTQualityChkBox.Margin = new System.Windows.Forms.Padding(4);
            this.NISTQualityChkBox.Name = "NISTQualityChkBox";
            this.NISTQualityChkBox.Size = new System.Drawing.Size(109, 21);
            this.NISTQualityChkBox.TabIndex = 2;
            this.NISTQualityChkBox.Text = "NIST Quality";
            this.NISTQualityChkBox.UseVisualStyleBackColor = true;
            this.NISTQualityChkBox.CheckedChanged += new System.EventHandler(this.NISTQualityChkBox_CheckedChanged);
            // 
            // SensorTriggerArmedChkBox
            // 
            this.SensorTriggerArmedChkBox.AutoSize = true;
            this.SensorTriggerArmedChkBox.Location = new System.Drawing.Point(24, 80);
            this.SensorTriggerArmedChkBox.Margin = new System.Windows.Forms.Padding(4);
            this.SensorTriggerArmedChkBox.Name = "SensorTriggerArmedChkBox";
            this.SensorTriggerArmedChkBox.Size = new System.Drawing.Size(170, 21);
            this.SensorTriggerArmedChkBox.TabIndex = 1;
            this.SensorTriggerArmedChkBox.Text = "Sensor Trigger Armed";
            this.SensorTriggerArmedChkBox.UseVisualStyleBackColor = true;
            this.SensorTriggerArmedChkBox.CheckedChanged += new System.EventHandler(this.SensorTriggerArmedChkBox_CheckedChanged);
            // 
            // EnableSpoofDetChkBox
            // 
            this.EnableSpoofDetChkBox.AutoSize = true;
            this.EnableSpoofDetChkBox.Location = new System.Drawing.Point(24, 36);
            this.EnableSpoofDetChkBox.Margin = new System.Windows.Forms.Padding(4);
            this.EnableSpoofDetChkBox.Name = "EnableSpoofDetChkBox";
            this.EnableSpoofDetChkBox.Size = new System.Drawing.Size(179, 21);
            this.EnableSpoofDetChkBox.TabIndex = 0;
            this.EnableSpoofDetChkBox.Text = "Enable Spoof Detection";
            this.EnableSpoofDetChkBox.UseVisualStyleBackColor = true;
            this.EnableSpoofDetChkBox.CheckedChanged += new System.EventHandler(this.EnableSpoofDetChkBox_CheckedChanged);
            // 
            // EnrollmentTabPage
            // 
            this.EnrollmentTabPage.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("EnrollmentTabPage.BackgroundImage")));
            this.EnrollmentTabPage.Controls.Add(this._setDpi);
            this.EnrollmentTabPage.Controls.Add(this._dpi);
            this.EnrollmentTabPage.Controls.Add(this.groupBox4);
            this.EnrollmentTabPage.Controls.Add(this.groupBox3);
            this.EnrollmentTabPage.Controls.Add(this._delete);
            this.EnrollmentTabPage.Controls.Add(this.groupBox2);
            this.EnrollmentTabPage.Controls.Add(this.groupBox1);
            this.EnrollmentTabPage.Location = new System.Drawing.Point(4, 25);
            this.EnrollmentTabPage.Margin = new System.Windows.Forms.Padding(4);
            this.EnrollmentTabPage.Name = "EnrollmentTabPage";
            this.EnrollmentTabPage.Padding = new System.Windows.Forms.Padding(4);
            this.EnrollmentTabPage.Size = new System.Drawing.Size(1117, 758);
            this.EnrollmentTabPage.TabIndex = 1;
            this.EnrollmentTabPage.Text = "Enrollment";
            this.EnrollmentTabPage.UseVisualStyleBackColor = true;
            // 
            // _setDpi
            // 
            this._setDpi.Location = new System.Drawing.Point(675, 581);
            this._setDpi.Margin = new System.Windows.Forms.Padding(4);
            this._setDpi.Name = "_setDpi";
            this._setDpi.Size = new System.Drawing.Size(191, 31);
            this._setDpi.TabIndex = 31;
            this._setDpi.Text = "Res";
            this._setDpi.UseVisualStyleBackColor = true;
            this._setDpi.Click += new System.EventHandler(this._setDpi_Click);
            // 
            // _dpi
            // 
            this._dpi.Location = new System.Drawing.Point(528, 586);
            this._dpi.Margin = new System.Windows.Forms.Padding(4);
            this._dpi.Name = "_dpi";
            this._dpi.Size = new System.Drawing.Size(139, 22);
            this._dpi.TabIndex = 30;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this._users_table);
            this.groupBox4.Location = new System.Drawing.Point(525, 8);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox4.Size = new System.Drawing.Size(580, 567);
            this.groupBox4.TabIndex = 19;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Lista";
            // 
            // _users_table
            // 
            this._users_table.AllowUserToAddRows = false;
            this._users_table.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._users_table.Dock = System.Windows.Forms.DockStyle.Fill;
            this._users_table.Location = new System.Drawing.Point(3, 17);
            this._users_table.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._users_table.Name = "_users_table";
            this._users_table.ReadOnly = true;
            this._users_table.RowTemplate.Height = 24;
            this._users_table.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._users_table.Size = new System.Drawing.Size(574, 548);
            this._users_table.TabIndex = 12;
            this._users_table.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this._users_table_CellClick);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this._document);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this._age);
            this.groupBox3.Controls.Add(this._birthdate);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this._document_type);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this._name);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this._last_name);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Location = new System.Drawing.Point(8, 8);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox3.Size = new System.Drawing.Size(500, 181);
            this.groupBox3.TabIndex = 18;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Datos";
            // 
            // _document
            // 
            this._document.Location = new System.Drawing.Point(270, 38);
            this._document.Margin = new System.Windows.Forms.Padding(4);
            this._document.Name = "_document";
            this._document.Size = new System.Drawing.Size(200, 22);
            this._document.TabIndex = 1;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(27, 118);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(78, 17);
            this.label6.TabIndex = 27;
            this.label6.Text = "Nacimiento";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(267, 118);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(41, 17);
            this.label7.TabIndex = 26;
            this.label7.Text = "Edad";
            // 
            // _age
            // 
            this._age.Enabled = false;
            this._age.Location = new System.Drawing.Point(269, 139);
            this._age.Margin = new System.Windows.Forms.Padding(4);
            this._age.Name = "_age";
            this._age.Size = new System.Drawing.Size(200, 22);
            this._age.TabIndex = 5;
            // 
            // _birthdate
            // 
            this._birthdate.CustomFormat = "MM/dd/yyyy";
            this._birthdate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this._birthdate.Location = new System.Drawing.Point(29, 139);
            this._birthdate.Margin = new System.Windows.Forms.Padding(4);
            this._birthdate.Name = "_birthdate";
            this._birthdate.Size = new System.Drawing.Size(200, 22);
            this._birthdate.TabIndex = 4;
            this._birthdate.Value = new System.DateTime(2018, 5, 18, 0, 0, 0, 0);
            this._birthdate.ValueChanged += new System.EventHandler(this._birthdate_ValueChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(27, 18);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(90, 17);
            this.label8.TabIndex = 23;
            this.label8.Text = "Identificación";
            // 
            // _document_type
            // 
            this._document_type.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._document_type.FormattingEnabled = true;
            this._document_type.Items.AddRange(new object[] {
            "Cédula de Ciudadanía",
            "Cédula de Extranjería",
            "Registro Civil de Nacimiento",
            "Tarjeta de Identidad"});
            this._document_type.Location = new System.Drawing.Point(29, 38);
            this._document_type.Margin = new System.Windows.Forms.Padding(4);
            this._document_type.Name = "_document_type";
            this._document_type.Size = new System.Drawing.Size(200, 24);
            this._document_type.TabIndex = 0;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(27, 67);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(65, 17);
            this.label9.TabIndex = 21;
            this.label9.Text = "Nombres";
            // 
            // _name
            // 
            this._name.Location = new System.Drawing.Point(29, 88);
            this._name.Margin = new System.Windows.Forms.Padding(4);
            this._name.Name = "_name";
            this._name.Size = new System.Drawing.Size(200, 22);
            this._name.TabIndex = 2;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(267, 67);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(65, 17);
            this.label10.TabIndex = 19;
            this.label10.Text = "Apellidos";
            // 
            // _last_name
            // 
            this._last_name.Location = new System.Drawing.Point(269, 88);
            this._last_name.Margin = new System.Windows.Forms.Padding(4);
            this._last_name.Name = "_last_name";
            this._last_name.Size = new System.Drawing.Size(200, 22);
            this._last_name.TabIndex = 3;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(267, 18);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(80, 17);
            this.label11.TabIndex = 16;
            this.label11.Text = "Documento";
            // 
            // _delete
            // 
            this._delete.Location = new System.Drawing.Point(874, 581);
            this._delete.Margin = new System.Windows.Forms.Padding(4);
            this._delete.Name = "_delete";
            this._delete.Size = new System.Drawing.Size(228, 31);
            this._delete.TabIndex = 17;
            this._delete.Text = "Eliminar";
            this._delete.UseVisualStyleBackColor = true;
            this._delete.Click += new System.EventHandler(this._delete_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this._cancelar);
            this.groupBox2.Controls.Add(this.pictureBox1);
            this.groupBox2.Controls.Add(this._print3);
            this.groupBox2.Controls.Add(this._print2);
            this.groupBox2.Controls.Add(this._print1);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this._fingers);
            this.groupBox2.Controls.Add(this._guardar);
            this.groupBox2.Location = new System.Drawing.Point(8, 199);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox2.Size = new System.Drawing.Size(500, 547);
            this.groupBox2.TabIndex = 15;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Huellas";
            // 
            // _cancelar
            // 
            this._cancelar.Location = new System.Drawing.Point(11, 504);
            this._cancelar.Margin = new System.Windows.Forms.Padding(4);
            this._cancelar.Name = "_cancelar";
            this._cancelar.Size = new System.Drawing.Size(229, 30);
            this._cancelar.TabIndex = 12;
            this._cancelar.Text = "Cancelar";
            this._cancelar.UseVisualStyleBackColor = true;
            this._cancelar.Click += new System.EventHandler(this._cancelar_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::SDKEnrollApp.Properties.Resources.Hands;
            this.pictureBox1.Location = new System.Drawing.Point(11, 274);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(477, 217);
            this.pictureBox1.TabIndex = 11;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseClick);
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
            // 
            // _print3
            // 
            this._print3.Location = new System.Drawing.Point(351, 78);
            this._print3.Margin = new System.Windows.Forms.Padding(4);
            this._print3.Name = "_print3";
            this._print3.Size = new System.Drawing.Size(136, 185);
            this._print3.TabIndex = 4;
            this._print3.TabStop = false;
            // 
            // _print2
            // 
            this._print2.Location = new System.Drawing.Point(181, 78);
            this._print2.Margin = new System.Windows.Forms.Padding(4);
            this._print2.Name = "_print2";
            this._print2.Size = new System.Drawing.Size(136, 185);
            this._print2.TabIndex = 3;
            this._print2.TabStop = false;
            // 
            // _print1
            // 
            this._print1.Location = new System.Drawing.Point(11, 78);
            this._print1.Margin = new System.Windows.Forms.Padding(4);
            this._print1.Name = "_print1";
            this._print1.Size = new System.Drawing.Size(136, 185);
            this._print1.TabIndex = 1;
            this._print1.TabStop = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 24);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(42, 17);
            this.label4.TabIndex = 3;
            this.label4.Text = "Dedo";
            // 
            // _fingers
            // 
            this._fingers.Enabled = false;
            this._fingers.Location = new System.Drawing.Point(11, 43);
            this._fingers.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._fingers.Name = "_fingers";
            this._fingers.Size = new System.Drawing.Size(476, 22);
            this._fingers.TabIndex = 2;
            // 
            // _guardar
            // 
            this._guardar.Location = new System.Drawing.Point(269, 504);
            this._guardar.Margin = new System.Windows.Forms.Padding(4);
            this._guardar.Name = "_guardar";
            this._guardar.Size = new System.Drawing.Size(218, 30);
            this._guardar.TabIndex = 0;
            this._guardar.Text = "Guardar";
            this._guardar.UseVisualStyleBackColor = true;
            this._guardar.Click += new System.EventHandler(this._guardar_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.NISTScoreLabel);
            this.groupBox1.Controls.Add(this.labelStatus);
            this.groupBox1.Location = new System.Drawing.Point(528, 633);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(574, 113);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Status";
            // 
            // NISTScoreLabel
            // 
            this.NISTScoreLabel.AutoSize = true;
            this.NISTScoreLabel.Font = new System.Drawing.Font("Calibri", 10.5F, System.Drawing.FontStyle.Bold);
            this.NISTScoreLabel.Location = new System.Drawing.Point(15, 70);
            this.NISTScoreLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.NISTScoreLabel.Name = "NISTScoreLabel";
            this.NISTScoreLabel.Size = new System.Drawing.Size(55, 22);
            this.NISTScoreLabel.TabIndex = 8;
            this.NISTScoreLabel.Text = "label2";
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.BackColor = System.Drawing.Color.Transparent;
            this.labelStatus.Font = new System.Drawing.Font("Calibri", 10.5F, System.Drawing.FontStyle.Bold);
            this.labelStatus.Location = new System.Drawing.Point(15, 27);
            this.labelStatus.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(55, 22);
            this.labelStatus.TabIndex = 0;
            this.labelStatus.Text = "label2";
            // 
            // VerificationTabPage
            // 
            this.VerificationTabPage.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("VerificationTabPage.BackgroundImage")));
            this.VerificationTabPage.Controls.Add(this.groupBox8);
            this.VerificationTabPage.Controls.Add(this.groupBox7);
            this.VerificationTabPage.Controls.Add(this.groupBox6);
            this.VerificationTabPage.Controls.Add(this.groupBox5);
            this.VerificationTabPage.Location = new System.Drawing.Point(4, 25);
            this.VerificationTabPage.Margin = new System.Windows.Forms.Padding(4);
            this.VerificationTabPage.Name = "VerificationTabPage";
            this.VerificationTabPage.Padding = new System.Windows.Forms.Padding(4);
            this.VerificationTabPage.Size = new System.Drawing.Size(1117, 758);
            this.VerificationTabPage.TabIndex = 2;
            this.VerificationTabPage.Text = "Verification";
            this.VerificationTabPage.UseVisualStyleBackColor = true;
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.labelStatus2);
            this.groupBox8.Location = new System.Drawing.Point(8, 571);
            this.groupBox8.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox8.Size = new System.Drawing.Size(500, 178);
            this.groupBox8.TabIndex = 22;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Status";
            // 
            // labelStatus2
            // 
            this.labelStatus2.AutoSize = true;
            this.labelStatus2.BackColor = System.Drawing.Color.Transparent;
            this.labelStatus2.Font = new System.Drawing.Font("Calibri", 10.5F, System.Drawing.FontStyle.Bold);
            this.labelStatus2.Location = new System.Drawing.Point(15, 27);
            this.labelStatus2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelStatus2.Name = "labelStatus2";
            this.labelStatus2.Size = new System.Drawing.Size(55, 22);
            this.labelStatus2.TabIndex = 0;
            this.labelStatus2.Text = "label2";
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this._resultsMatch);
            this.groupBox7.Controls.Add(this._log);
            this.groupBox7.Location = new System.Drawing.Point(521, 8);
            this.groupBox7.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox7.Size = new System.Drawing.Size(580, 741);
            this.groupBox7.TabIndex = 21;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Lista";
            // 
            // _resultsMatch
            // 
            this._resultsMatch.Location = new System.Drawing.Point(6, 523);
            this._resultsMatch.Multiline = true;
            this._resultsMatch.Name = "_resultsMatch";
            this._resultsMatch.ReadOnly = true;
            this._resultsMatch.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this._resultsMatch.Size = new System.Drawing.Size(568, 213);
            this._resultsMatch.TabIndex = 1;
            // 
            // _log
            // 
            this._log.Location = new System.Drawing.Point(6, 20);
            this._log.Multiline = true;
            this._log.Name = "_log";
            this._log.ReadOnly = true;
            this._log.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this._log.Size = new System.Drawing.Size(568, 496);
            this._log.TabIndex = 0;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.label17);
            this.groupBox6.Controls.Add(this._verificar);
            this.groupBox6.Controls.Add(this._cancelar2);
            this.groupBox6.Controls.Add(this._fingerprint);
            this.groupBox6.Controls.Add(this._finger2);
            this.groupBox6.Controls.Add(this.button1);
            this.groupBox6.Controls.Add(this.label15);
            this.groupBox6.Controls.Add(this.button2);
            this.groupBox6.Location = new System.Drawing.Point(8, 253);
            this.groupBox6.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox6.Size = new System.Drawing.Size(500, 310);
            this.groupBox6.TabIndex = 20;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Huellas";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(8, 77);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(48, 17);
            this.label17.TabIndex = 35;
            this.label17.Text = "Huella";
            // 
            // _verificar
            // 
            this._verificar.Location = new System.Drawing.Point(11, 142);
            this._verificar.Margin = new System.Windows.Forms.Padding(4);
            this._verificar.Name = "_verificar";
            this._verificar.Size = new System.Drawing.Size(459, 30);
            this._verificar.TabIndex = 34;
            this._verificar.Text = "Verificar";
            this._verificar.UseVisualStyleBackColor = true;
            this._verificar.Click += new System.EventHandler(this._verificar_Click);
            // 
            // _cancelar2
            // 
            this._cancelar2.Location = new System.Drawing.Point(11, 189);
            this._cancelar2.Margin = new System.Windows.Forms.Padding(4);
            this._cancelar2.Name = "_cancelar2";
            this._cancelar2.Size = new System.Drawing.Size(458, 30);
            this._cancelar2.TabIndex = 33;
            this._cancelar2.Text = "Cancelar";
            this._cancelar2.UseVisualStyleBackColor = true;
            // 
            // _fingerprint
            // 
            this._fingerprint.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._fingerprint.FormattingEnabled = true;
            this._fingerprint.Location = new System.Drawing.Point(11, 97);
            this._fingerprint.Name = "_fingerprint";
            this._fingerprint.Size = new System.Drawing.Size(458, 24);
            this._fingerprint.TabIndex = 14;
            // 
            // _finger2
            // 
            this._finger2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._finger2.FormattingEnabled = true;
            this._finger2.Location = new System.Drawing.Point(11, 44);
            this._finger2.Name = "_finger2";
            this._finger2.Size = new System.Drawing.Size(458, 24);
            this._finger2.TabIndex = 13;
            this._finger2.SelectedIndexChanged += new System.EventHandler(this._finger2_SelectedIndexChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(11, 504);
            this.button1.Margin = new System.Windows.Forms.Padding(4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(229, 30);
            this.button1.TabIndex = 12;
            this.button1.Text = "Cancelar";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(8, 24);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(42, 17);
            this.label15.TabIndex = 3;
            this.label15.Text = "Dedo";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(269, 504);
            this.button2.Margin = new System.Windows.Forms.Padding(4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(218, 30);
            this.button2.TabIndex = 0;
            this.button2.Text = "Guardar";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this._buscar);
            this.groupBox5.Controls.Add(this._birthdate2);
            this.groupBox5.Controls.Add(this._document_type2);
            this.groupBox5.Controls.Add(this._document2);
            this.groupBox5.Controls.Add(this.label2);
            this.groupBox5.Controls.Add(this.label3);
            this.groupBox5.Controls.Add(this._age2);
            this.groupBox5.Controls.Add(this.label5);
            this.groupBox5.Controls.Add(this.label12);
            this.groupBox5.Controls.Add(this._name2);
            this.groupBox5.Controls.Add(this.label13);
            this.groupBox5.Controls.Add(this._last_name2);
            this.groupBox5.Controls.Add(this.label14);
            this.groupBox5.Location = new System.Drawing.Point(8, 8);
            this.groupBox5.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox5.Size = new System.Drawing.Size(500, 237);
            this.groupBox5.TabIndex = 19;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Datos";
            // 
            // _buscar
            // 
            this._buscar.Location = new System.Drawing.Point(30, 181);
            this._buscar.Margin = new System.Windows.Forms.Padding(4);
            this._buscar.Name = "_buscar";
            this._buscar.Size = new System.Drawing.Size(439, 30);
            this._buscar.TabIndex = 32;
            this._buscar.Text = "Buscar";
            this._buscar.UseVisualStyleBackColor = true;
            this._buscar.Click += new System.EventHandler(this._buscar_Click);
            // 
            // _birthdate2
            // 
            this._birthdate2.Enabled = false;
            this._birthdate2.Location = new System.Drawing.Point(29, 139);
            this._birthdate2.Margin = new System.Windows.Forms.Padding(4);
            this._birthdate2.Name = "_birthdate2";
            this._birthdate2.Size = new System.Drawing.Size(200, 22);
            this._birthdate2.TabIndex = 31;
            // 
            // _document_type2
            // 
            this._document_type2.Enabled = false;
            this._document_type2.Location = new System.Drawing.Point(30, 38);
            this._document_type2.Margin = new System.Windows.Forms.Padding(4);
            this._document_type2.Name = "_document_type2";
            this._document_type2.Size = new System.Drawing.Size(200, 22);
            this._document_type2.TabIndex = 30;
            // 
            // _document2
            // 
            this._document2.Location = new System.Drawing.Point(270, 38);
            this._document2.Margin = new System.Windows.Forms.Padding(4);
            this._document2.Name = "_document2";
            this._document2.Size = new System.Drawing.Size(200, 22);
            this._document2.TabIndex = 29;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(27, 118);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 17);
            this.label2.TabIndex = 27;
            this.label2.Text = "Nacimiento";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(267, 118);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 17);
            this.label3.TabIndex = 26;
            this.label3.Text = "Edad";
            // 
            // _age2
            // 
            this._age2.Enabled = false;
            this._age2.Location = new System.Drawing.Point(269, 139);
            this._age2.Margin = new System.Windows.Forms.Padding(4);
            this._age2.Name = "_age2";
            this._age2.Size = new System.Drawing.Size(200, 22);
            this._age2.TabIndex = 5;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(27, 18);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(90, 17);
            this.label5.TabIndex = 23;
            this.label5.Text = "Identificación";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(27, 67);
            this.label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(65, 17);
            this.label12.TabIndex = 21;
            this.label12.Text = "Nombres";
            // 
            // _name2
            // 
            this._name2.Enabled = false;
            this._name2.Location = new System.Drawing.Point(29, 88);
            this._name2.Margin = new System.Windows.Forms.Padding(4);
            this._name2.Name = "_name2";
            this._name2.Size = new System.Drawing.Size(200, 22);
            this._name2.TabIndex = 2;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(267, 67);
            this.label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(65, 17);
            this.label13.TabIndex = 19;
            this.label13.Text = "Apellidos";
            // 
            // _last_name2
            // 
            this._last_name2.Enabled = false;
            this._last_name2.Location = new System.Drawing.Point(269, 88);
            this._last_name2.Margin = new System.Windows.Forms.Padding(4);
            this._last_name2.Name = "_last_name2";
            this._last_name2.Size = new System.Drawing.Size(200, 22);
            this._last_name2.TabIndex = 3;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(267, 18);
            this.label14.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(80, 17);
            this.label14.TabIndex = 16;
            this.label14.Text = "Documento";
            // 
            // CaptureBtnClick
            // 
            this.CaptureBtnClick.Location = new System.Drawing.Point(1137, 693);
            this.CaptureBtnClick.Margin = new System.Windows.Forms.Padding(4);
            this.CaptureBtnClick.Name = "CaptureBtnClick";
            this.CaptureBtnClick.Size = new System.Drawing.Size(352, 27);
            this.CaptureBtnClick.TabIndex = 3;
            this.CaptureBtnClick.Text = "Capture";
            this.CaptureBtnClick.UseVisualStyleBackColor = true;
            this.CaptureBtnClick.Click += new System.EventHandler(this.CaptureBtnClick_Click);
            // 
            // SelectSensorComboBox
            // 
            this.SelectSensorComboBox.FormattingEnabled = true;
            this.SelectSensorComboBox.Location = new System.Drawing.Point(1136, 660);
            this.SelectSensorComboBox.Margin = new System.Windows.Forms.Padding(4);
            this.SelectSensorComboBox.Name = "SelectSensorComboBox";
            this.SelectSensorComboBox.Size = new System.Drawing.Size(352, 24);
            this.SelectSensorComboBox.TabIndex = 4;
            this.SelectSensorComboBox.SelectedIndexChanged += new System.EventHandler(this.SelectSensorComboBox_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1133, 640);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 17);
            this.label1.TabIndex = 5;
            this.label1.Text = "Sensor";
            // 
            // LiveBtn
            // 
            this.LiveBtn.Location = new System.Drawing.Point(1137, 727);
            this.LiveBtn.Margin = new System.Windows.Forms.Padding(4);
            this.LiveBtn.Name = "LiveBtn";
            this.LiveBtn.Size = new System.Drawing.Size(352, 27);
            this.LiveBtn.TabIndex = 6;
            this.LiveBtn.Text = "Live Mode";
            this.LiveBtn.UseVisualStyleBackColor = true;
            this.LiveBtn.Click += new System.EventHandler(this.LiveBtn_Click);
            // 
            // btnSaveImage
            // 
            this.btnSaveImage.Location = new System.Drawing.Point(1137, 757);
            this.btnSaveImage.Margin = new System.Windows.Forms.Padding(4);
            this.btnSaveImage.Name = "btnSaveImage";
            this.btnSaveImage.Size = new System.Drawing.Size(352, 28);
            this.btnSaveImage.TabIndex = 9;
            this.btnSaveImage.Text = "Save Image";
            this.btnSaveImage.UseVisualStyleBackColor = true;
            this.btnSaveImage.Click += new System.EventHandler(this.btnSaveImage_Click_1);
            // 
            // nistPictureBox
            // 
            this.nistPictureBox.Location = new System.Drawing.Point(1133, 24);
            this.nistPictureBox.Margin = new System.Windows.Forms.Padding(4);
            this.nistPictureBox.Name = "nistPictureBox";
            this.nistPictureBox.Size = new System.Drawing.Size(352, 69);
            this.nistPictureBox.TabIndex = 10;
            this.nistPictureBox.TabStop = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(1497, 793);
            this.Controls.Add(this.nistPictureBox);
            this.Controls.Add(this.btnSaveImage);
            this.Controls.Add(this.LiveBtn);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.SelectSensorComboBox);
            this.Controls.Add(this.CaptureBtnClick);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.LumiPictureBox1);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Orion Fingerprints";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.LumiPictureBox1)).EndInit();
            this.tabControl.ResumeLayout(false);
            this.OptionsTabPage.ResumeLayout(false);
            this.OptionsTabPage.PerformLayout();
            this.SpoofThresholdGroupBox.ResumeLayout(false);
            this.SpoofThresholdGroupBox.PerformLayout();
            this.MatchThresholdGroupBox.ResumeLayout(false);
            this.MatchThresholdGroupBox.PerformLayout();
            this.SensorSettingsGroupBox.ResumeLayout(false);
            this.SensorSettingsGroupBox.PerformLayout();
            this.EnrollmentTabPage.ResumeLayout(false);
            this.EnrollmentTabPage.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._users_table)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._print3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._print2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._print1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.VerificationTabPage.ResumeLayout(false);
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nistPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TabControl tabControl;
        private TabPage OptionsTabPage;
        private TabPage EnrollmentTabPage;
        private TabPage VerificationTabPage;
        private GroupBox groupBox1;
        public Button CaptureBtnClick;
        public ComboBox SelectSensorComboBox;
        private Label label1;
        private Label labelStatus;
        private GroupBox SensorSettingsGroupBox;
        private CheckBox SensorTriggerArmedChkBox;
        public CheckBox EnableSpoofDetChkBox;
        public Button LiveBtn;
        private CheckBox NISTQualityChkBox;
        private TextBox _fingers;
        private Label label4;
        private GroupBox MatchThresholdGroupBox;
        private RadioButton MatchConvenient;
        private RadioButton MatchSecure;
        private RadioButton MatchHighlySecured;
        private GroupBox SpoofThresholdGroupBox;
        private RadioButton SpoofConvenient;
        private RadioButton SpoofSecure;
        private RadioButton SpoofHighlySecure;
        public Button btnSaveImage;
        public PictureBox LumiPictureBox1;
        private GroupBox groupBox4;
        private DataGridView _users_table;
        private GroupBox groupBox3;
        private Label label6;
        private Label label7;
        private TextBox _age;
        private DateTimePicker _birthdate;
        private Label label8;
        private ComboBox _document_type;
        private Label label9;
        private TextBox _name;
        private Label label10;
        private TextBox _last_name;
        private Label label11;
        private Button _delete;
        private GroupBox groupBox2;
        private PictureBox pictureBox1;
        private PictureBox _print3;
        private PictureBox _print2;
        private PictureBox _print1;
        private Button _guardar;
        private Button _cancelar;
        private TextBox _document;
        private Label NISTScoreLabel;
        private PictureBox nistPictureBox;
        private Button _setDpi;
        private TextBox _dpi;
        private GroupBox groupBox5;
        private TextBox _birthdate2;
        private TextBox _document_type2;
        private TextBox _document2;
        private Label label2;
        private Label label3;
        private TextBox _age2;
        private Label label5;
        private Label label12;
        private TextBox _name2;
        private Label label13;
        private TextBox _last_name2;
        private Label label14;
        private GroupBox groupBox8;
        private Label labelStatus2;
        private GroupBox groupBox7;
        private GroupBox groupBox6;
        private Button button1;
        private Label label15;
        private Button button2;
        private Button _cancelar2;
        private Button _buscar;
        private Label label17;
        private Button _verificar;
        private ComboBox _fingerprint;
        private ComboBox _finger2;
        private TextBox _log;
        private TextBox _resultsMatch;
        private Button _setUmbral;
        private Label label16;
        private TextBox _umbral;
    }
}

