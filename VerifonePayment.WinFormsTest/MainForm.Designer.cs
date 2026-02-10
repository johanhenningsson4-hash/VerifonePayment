namespace VerifonePayment.WinFormsTest
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        
        #region Windows Form Designer generated code
        
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._tlpMain = new System.Windows.Forms.TableLayoutPanel();
            this._grpOperations = new System.Windows.Forms.GroupBox();
            this._pnlOperations = new System.Windows.Forms.TableLayoutPanel();
            this._btnCommunicate = new System.Windows.Forms.Button();
            this._btnLogin = new System.Windows.Forms.Button();
            this._btnStartSession = new System.Windows.Forms.Button();
            this._btnAddMerchandise = new System.Windows.Forms.Button();
            this._btnPayment = new System.Windows.Forms.Button();
            this._btnRemoveMerchandise = new System.Windows.Forms.Button();
            this._btnEndSession = new System.Windows.Forms.Button();
            this._btnTearDown = new System.Windows.Forms.Button();
            this._grpConfiguration = new System.Windows.Forms.GroupBox();
            this._tlpConfig = new System.Windows.Forms.TableLayoutPanel();
            this._lblInvoiceId = new System.Windows.Forms.Label();
            this._txtInvoiceId = new System.Windows.Forms.TextBox();
            this._btnGenerateInvoice = new System.Windows.Forms.Button();
            this._lblAmount = new System.Windows.Forms.Label();
            this._numAmount = new System.Windows.Forms.NumericUpDown();
            this._lblCurrency = new System.Windows.Forms.Label();
            this._cmbCurrency = new System.Windows.Forms.ComboBox();
            this._btnValidateConfig = new System.Windows.Forms.Button();
            this._grpLog = new System.Windows.Forms.GroupBox();
            this._tlpLog = new System.Windows.Forms.TableLayoutPanel();
            this._txtLog = new System.Windows.Forms.TextBox();
            this._btnClearLog = new System.Windows.Forms.Button();
            this._tlpMain.SuspendLayout();
            this._grpOperations.SuspendLayout();
            this._pnlOperations.SuspendLayout();
            this._grpConfiguration.SuspendLayout();
            this._tlpConfig.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._numAmount)).BeginInit();
            this._grpLog.SuspendLayout();
            this._tlpLog.SuspendLayout();
            this.SuspendLayout();
            // 
            // _tlpMain
            // 
            this._tlpMain.ColumnCount = 2;
            this._tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this._tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this._tlpMain.Controls.Add(this._grpOperations, 0, 0);
            this._tlpMain.Controls.Add(this._grpConfiguration, 1, 0);
            this._tlpMain.Controls.Add(this._grpLog, 0, 1);
            this._tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this._tlpMain.Location = new System.Drawing.Point(0, 0);
            this._tlpMain.Margin = new System.Windows.Forms.Padding(4);
            this._tlpMain.Name = "_tlpMain";
            this._tlpMain.RowCount = 2;
            this._tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 350F));
            this._tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._tlpMain.Size = new System.Drawing.Size(1000, 700);
            this._tlpMain.TabIndex = 0;
            // 
            // _grpOperations
            // 
            this._grpOperations.AutoSize = true;
            this._grpOperations.Controls.Add(this._pnlOperations);
            this._grpOperations.Dock = System.Windows.Forms.DockStyle.Fill;
            this._grpOperations.Location = new System.Drawing.Point(4, 4);
            this._grpOperations.Margin = new System.Windows.Forms.Padding(4);
            this._grpOperations.Name = "_grpOperations";
            this._grpOperations.Padding = new System.Windows.Forms.Padding(4);
            this._grpOperations.Size = new System.Drawing.Size(492, 342);
            this._grpOperations.TabIndex = 0;
            this._grpOperations.TabStop = false;
            this._grpOperations.Text = "Payment Operations";
            // 
            // _pnlOperations
            // 
            this._pnlOperations.AutoSize = true;
            this._pnlOperations.ColumnCount = 2;
            this._pnlOperations.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this._pnlOperations.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this._pnlOperations.Controls.Add(this._btnCommunicate, 0, 0);
            this._pnlOperations.Controls.Add(this._btnLogin, 1, 0);
            this._pnlOperations.Controls.Add(this._btnStartSession, 0, 1);
            this._pnlOperations.Controls.Add(this._btnAddMerchandise, 1, 1);
            this._pnlOperations.Controls.Add(this._btnPayment, 0, 2);
            this._pnlOperations.Controls.Add(this._btnRemoveMerchandise, 1, 2);
            this._pnlOperations.Controls.Add(this._btnEndSession, 0, 3);
            this._pnlOperations.Controls.Add(this._btnTearDown, 1, 3);
            this._pnlOperations.Dock = System.Windows.Forms.DockStyle.Fill;
            this._pnlOperations.Location = new System.Drawing.Point(4, 19);
            this._pnlOperations.Margin = new System.Windows.Forms.Padding(4);
            this._pnlOperations.Name = "_pnlOperations";
            this._pnlOperations.RowCount = 4;
            this._pnlOperations.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._pnlOperations.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._pnlOperations.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._pnlOperations.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._pnlOperations.Size = new System.Drawing.Size(484, 319);
            this._pnlOperations.TabIndex = 0;
            // 
            // _btnCommunicate
            // 
            this._btnCommunicate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._btnCommunicate.Enabled = false;
            this._btnCommunicate.Location = new System.Drawing.Point(4, 4);
            this._btnCommunicate.Margin = new System.Windows.Forms.Padding(4);
            this._btnCommunicate.Name = "_btnCommunicate";
            this._btnCommunicate.Size = new System.Drawing.Size(234, 40);
            this._btnCommunicate.TabIndex = 0;
            this._btnCommunicate.Text = "1. Communicate with SDK";
            this._btnCommunicate.UseVisualStyleBackColor = true;
            this._btnCommunicate.Click += new System.EventHandler(this.BtnCommunicate_Click);
            // 
            // _btnLogin
            // 
            this._btnLogin.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._btnLogin.Enabled = false;
            this._btnLogin.Location = new System.Drawing.Point(246, 4);
            this._btnLogin.Margin = new System.Windows.Forms.Padding(4);
            this._btnLogin.Name = "_btnLogin";
            this._btnLogin.Size = new System.Drawing.Size(234, 40);
            this._btnLogin.TabIndex = 1;
            this._btnLogin.Text = "2. Login with Credentials";
            this._btnLogin.UseVisualStyleBackColor = true;
            this._btnLogin.Click += new System.EventHandler(this.BtnLogin_Click);
            // 
            // _btnStartSession
            // 
            this._btnStartSession.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._btnStartSession.Enabled = false;
            this._btnStartSession.Location = new System.Drawing.Point(4, 52);
            this._btnStartSession.Margin = new System.Windows.Forms.Padding(4);
            this._btnStartSession.Name = "_btnStartSession";
            this._btnStartSession.Size = new System.Drawing.Size(234, 40);
            this._btnStartSession.TabIndex = 2;
            this._btnStartSession.Text = "3. Start Session";
            this._btnStartSession.UseVisualStyleBackColor = true;
            this._btnStartSession.Click += new System.EventHandler(this.BtnStartSession_Click);
            // 
            // _btnAddMerchandise
            // 
            this._btnAddMerchandise.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._btnAddMerchandise.Enabled = false;
            this._btnAddMerchandise.Location = new System.Drawing.Point(246, 52);
            this._btnAddMerchandise.Margin = new System.Windows.Forms.Padding(4);
            this._btnAddMerchandise.Name = "_btnAddMerchandise";
            this._btnAddMerchandise.Size = new System.Drawing.Size(234, 40);
            this._btnAddMerchandise.TabIndex = 3;
            this._btnAddMerchandise.Text = "4. Add Merchandise";
            this._btnAddMerchandise.UseVisualStyleBackColor = true;
            this._btnAddMerchandise.Click += new System.EventHandler(this.BtnAddMerchandise_Click);
            // 
            // _btnPayment
            // 
            this._btnPayment.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._btnPayment.Enabled = false;
            this._btnPayment.Location = new System.Drawing.Point(4, 100);
            this._btnPayment.Margin = new System.Windows.Forms.Padding(4);
            this._btnPayment.Name = "_btnPayment";
            this._btnPayment.Size = new System.Drawing.Size(234, 40);
            this._btnPayment.TabIndex = 4;
            this._btnPayment.Text = "5. Process Payment";
            this._btnPayment.UseVisualStyleBackColor = true;
            this._btnPayment.Click += new System.EventHandler(this.BtnPayment_Click);
            // 
            // _btnRemoveMerchandise
            // 
            this._btnRemoveMerchandise.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._btnRemoveMerchandise.Enabled = false;
            this._btnRemoveMerchandise.Location = new System.Drawing.Point(246, 100);
            this._btnRemoveMerchandise.Margin = new System.Windows.Forms.Padding(4);
            this._btnRemoveMerchandise.Name = "_btnRemoveMerchandise";
            this._btnRemoveMerchandise.Size = new System.Drawing.Size(234, 40);
            this._btnRemoveMerchandise.TabIndex = 5;
            this._btnRemoveMerchandise.Text = "Remove Merchandise";
            this._btnRemoveMerchandise.UseVisualStyleBackColor = true;
            this._btnRemoveMerchandise.Click += new System.EventHandler(this.BtnRemoveMerchandise_Click);
            // 
            // _btnEndSession
            // 
            this._btnEndSession.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._btnEndSession.Enabled = false;
            this._btnEndSession.Location = new System.Drawing.Point(4, 148);
            this._btnEndSession.Margin = new System.Windows.Forms.Padding(4);
            this._btnEndSession.Name = "_btnEndSession";
            this._btnEndSession.Size = new System.Drawing.Size(234, 40);
            this._btnEndSession.TabIndex = 6;
            this._btnEndSession.Text = "6. End Session";
            this._btnEndSession.UseVisualStyleBackColor = true;
            this._btnEndSession.Click += new System.EventHandler(this.BtnEndSession_Click);
            // 
            // _btnTearDown
            // 
            this._btnTearDown.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._btnTearDown.Enabled = false;
            this._btnTearDown.Location = new System.Drawing.Point(246, 148);
            this._btnTearDown.Margin = new System.Windows.Forms.Padding(4);
            this._btnTearDown.Name = "_btnTearDown";
            this._btnTearDown.Size = new System.Drawing.Size(234, 40);
            this._btnTearDown.TabIndex = 7;
            this._btnTearDown.Text = "7. Tear Down";
            this._btnTearDown.UseVisualStyleBackColor = true;
            this._btnTearDown.Click += new System.EventHandler(this.BtnTearDown_Click);
            // 
            // _grpConfiguration
            // 
            this._grpConfiguration.AutoSize = true;
            this._grpConfiguration.Controls.Add(this._tlpConfig);
            this._grpConfiguration.Dock = System.Windows.Forms.DockStyle.Fill;
            this._grpConfiguration.Location = new System.Drawing.Point(504, 4);
            this._grpConfiguration.Margin = new System.Windows.Forms.Padding(4);
            this._grpConfiguration.Name = "_grpConfiguration";
            this._grpConfiguration.Padding = new System.Windows.Forms.Padding(4);
            this._grpConfiguration.Size = new System.Drawing.Size(492, 342);
            this._grpConfiguration.TabIndex = 1;
            this._grpConfiguration.TabStop = false;
            this._grpConfiguration.Text = "Configuration";
            // 
            // _tlpConfig
            // 
            this._tlpConfig.AutoSize = true;
            this._tlpConfig.ColumnCount = 3;
            this._tlpConfig.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._tlpConfig.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._tlpConfig.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._tlpConfig.Controls.Add(this._lblInvoiceId, 0, 0);
            this._tlpConfig.Controls.Add(this._txtInvoiceId, 1, 0);
            this._tlpConfig.Controls.Add(this._btnGenerateInvoice, 2, 0);
            this._tlpConfig.Controls.Add(this._lblAmount, 0, 1);
            this._tlpConfig.Controls.Add(this._numAmount, 1, 1);
            this._tlpConfig.Controls.Add(this._lblCurrency, 0, 2);
            this._tlpConfig.Controls.Add(this._cmbCurrency, 1, 2);
            this._tlpConfig.Controls.Add(this._btnValidateConfig, 0, 3);
            this._tlpConfig.Dock = System.Windows.Forms.DockStyle.Fill;
            this._tlpConfig.Location = new System.Drawing.Point(4, 19);
            this._tlpConfig.Margin = new System.Windows.Forms.Padding(4);
            this._tlpConfig.Name = "_tlpConfig";
            this._tlpConfig.RowCount = 4;
            this._tlpConfig.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._tlpConfig.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._tlpConfig.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._tlpConfig.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._tlpConfig.Size = new System.Drawing.Size(484, 319);
            this._tlpConfig.TabIndex = 0;
            // 
            // _lblInvoiceId
            // 
            this._lblInvoiceId.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._lblInvoiceId.AutoSize = true;
            this._lblInvoiceId.Location = new System.Drawing.Point(4, 7);
            this._lblInvoiceId.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lblInvoiceId.Name = "_lblInvoiceId";
            this._lblInvoiceId.Size = new System.Drawing.Size(73, 16);
            this._lblInvoiceId.TabIndex = 0;
            this._lblInvoiceId.Text = "Invoice ID:";
            // 
            // _txtInvoiceId
            // 
            this._txtInvoiceId.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._txtInvoiceId.Enabled = false;
            this._txtInvoiceId.Location = new System.Drawing.Point(85, 4);
            this._txtInvoiceId.Margin = new System.Windows.Forms.Padding(4);
            this._txtInvoiceId.Name = "_txtInvoiceId";
            this._txtInvoiceId.Size = new System.Drawing.Size(270, 22);
            this._txtInvoiceId.TabIndex = 1;
            // 
            // _btnGenerateInvoice
            // 
            this._btnGenerateInvoice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._btnGenerateInvoice.Location = new System.Drawing.Point(363, 4);
            this._btnGenerateInvoice.Margin = new System.Windows.Forms.Padding(4);
            this._btnGenerateInvoice.Name = "_btnGenerateInvoice";
            this._btnGenerateInvoice.Size = new System.Drawing.Size(117, 23);
            this._btnGenerateInvoice.TabIndex = 2;
            this._btnGenerateInvoice.Text = "Generate";
            this._btnGenerateInvoice.UseVisualStyleBackColor = true;
            this._btnGenerateInvoice.Click += new System.EventHandler(this.BtnGenerateInvoice_Click);
            // 
            // _lblAmount
            // 
            this._lblAmount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._lblAmount.AutoSize = true;
            this._lblAmount.Location = new System.Drawing.Point(4, 74);
            this._lblAmount.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lblAmount.Name = "_lblAmount";
            this._lblAmount.Size = new System.Drawing.Size(73, 16);
            this._lblAmount.TabIndex = 3;
            this._lblAmount.Text = "Amount:";
            // 
            // _numAmount
            // 
            this._numAmount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._numAmount.DecimalPlaces = 2;
            this._numAmount.Enabled = false;
            this._numAmount.Location = new System.Drawing.Point(85, 71);
            this._numAmount.Margin = new System.Windows.Forms.Padding(4);
            this._numAmount.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this._numAmount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this._numAmount.Name = "_numAmount";
            this._numAmount.Size = new System.Drawing.Size(270, 22);
            this._numAmount.TabIndex = 4;
            this._numAmount.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // _lblCurrency
            // 
            this._lblCurrency.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._lblCurrency.AutoSize = true;
            this._lblCurrency.Location = new System.Drawing.Point(4, 108);
            this._lblCurrency.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lblCurrency.Name = "_lblCurrency";
            this._lblCurrency.Size = new System.Drawing.Size(73, 16);
            this._lblCurrency.TabIndex = 6;
            this._lblCurrency.Text = "Currency:";
            // 
            // _cmbCurrency
            // 
            this._cmbCurrency.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._cmbCurrency.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cmbCurrency.FormattingEnabled = true;
            this._cmbCurrency.Items.AddRange(new object[] {
            "USD",
            "EUR",
            "SEK"});
            this._cmbCurrency.Location = new System.Drawing.Point(85, 105);
            this._cmbCurrency.Margin = new System.Windows.Forms.Padding(4);
            this._cmbCurrency.Name = "_cmbCurrency";
            this._cmbCurrency.Size = new System.Drawing.Size(270, 24);
            this._cmbCurrency.TabIndex = 7;
            this._cmbCurrency.SelectedIndexChanged += new System.EventHandler(this.CmbCurrency_SelectedIndexChanged);
            // 
            // _btnValidateConfig
            // 
            this._tlpConfig.SetColumnSpan(this._btnValidateConfig, 3);
            this._btnValidateConfig.Enabled = false;
            this._btnValidateConfig.Location = new System.Drawing.Point(4, 137);
            this._btnValidateConfig.Margin = new System.Windows.Forms.Padding(4);
            this._btnValidateConfig.Name = "_btnValidateConfig";
            this._btnValidateConfig.Size = new System.Drawing.Size(160, 30);
            this._btnValidateConfig.TabIndex = 5;
            this._btnValidateConfig.Text = "Validate Configuration";
            this._btnValidateConfig.UseVisualStyleBackColor = true;
            this._btnValidateConfig.Click += new System.EventHandler(this.BtnValidateConfig_Click);
            // 
            // _grpLog
            // 
            this._tlpMain.SetColumnSpan(this._grpLog, 2);
            this._grpLog.Controls.Add(this._tlpLog);
            this._grpLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this._grpLog.Location = new System.Drawing.Point(4, 354);
            this._grpLog.Margin = new System.Windows.Forms.Padding(4);
            this._grpLog.Name = "_grpLog";
            this._grpLog.Padding = new System.Windows.Forms.Padding(4);
            this._grpLog.Size = new System.Drawing.Size(992, 342);
            this._grpLog.TabIndex = 2;
            this._grpLog.TabStop = false;
            this._grpLog.Text = "Event Log";
            // 
            // _tlpLog
            // 
            this._tlpLog.ColumnCount = 2;
            this._tlpLog.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._tlpLog.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._tlpLog.Controls.Add(this._txtLog, 0, 0);
            this._tlpLog.Controls.Add(this._btnClearLog, 1, 0);
            this._tlpLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this._tlpLog.Location = new System.Drawing.Point(4, 19);
            this._tlpLog.Margin = new System.Windows.Forms.Padding(4);
            this._tlpLog.Name = "_tlpLog";
            this._tlpLog.RowCount = 1;
            this._tlpLog.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._tlpLog.Size = new System.Drawing.Size(984, 319);
            this._tlpLog.TabIndex = 0;
            // 
            // _txtLog
            // 
            this._txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this._txtLog.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._txtLog.Location = new System.Drawing.Point(4, 4);
            this._txtLog.Margin = new System.Windows.Forms.Padding(4);
            this._txtLog.Multiline = true;
            this._txtLog.Name = "_txtLog";
            this._txtLog.ReadOnly = true;
            this._txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this._txtLog.Size = new System.Drawing.Size(887, 311);
            this._txtLog.TabIndex = 0;
            // 
            // _btnClearLog
            // 
            this._btnClearLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._btnClearLog.Location = new System.Drawing.Point(899, 4);
            this._btnClearLog.Margin = new System.Windows.Forms.Padding(4);
            this._btnClearLog.Name = "_btnClearLog";
            this._btnClearLog.Size = new System.Drawing.Size(81, 30);
            this._btnClearLog.TabIndex = 1;
            this._btnClearLog.Text = "Clear";
            this._btnClearLog.UseVisualStyleBackColor = true;
            this._btnClearLog.Click += new System.EventHandler(this.BtnClearLog_Click);
            // 
            // lstCurrency - REMOVED (replaced with ComboBox)
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 700);
            this.Controls.Add(this._tlpMain);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Verifone Payment Test - Windows Forms";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this._tlpMain.ResumeLayout(false);
            this._tlpMain.PerformLayout();
            this._grpOperations.ResumeLayout(false);
            this._grpOperations.PerformLayout();
            this._pnlOperations.ResumeLayout(false);
            this._grpConfiguration.ResumeLayout(false);
            this._grpConfiguration.PerformLayout();
            this._tlpConfig.ResumeLayout(false);
            this._tlpConfig.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._numAmount)).EndInit();
            this._grpLog.ResumeLayout(false);
            this._tlpLog.ResumeLayout(false);
            this._tlpLog.PerformLayout();
            this.ResumeLayout(false);

        }
        
        #endregion
        
        private System.Windows.Forms.TableLayoutPanel _tlpMain;
        private System.Windows.Forms.GroupBox _grpOperations;
        private System.Windows.Forms.TableLayoutPanel _pnlOperations;
        private System.Windows.Forms.Button _btnCommunicate;
        private System.Windows.Forms.Button _btnLogin;
        private System.Windows.Forms.Button _btnStartSession;
        private System.Windows.Forms.Button _btnAddMerchandise;
        private System.Windows.Forms.Button _btnPayment;
        private System.Windows.Forms.Button _btnRemoveMerchandise;
        private System.Windows.Forms.Button _btnEndSession;
        private System.Windows.Forms.Button _btnTearDown;
        private System.Windows.Forms.GroupBox _grpConfiguration;
        private System.Windows.Forms.TableLayoutPanel _tlpConfig;
        private System.Windows.Forms.Label _lblInvoiceId;
        private System.Windows.Forms.TextBox _txtInvoiceId;
        private System.Windows.Forms.Button _btnGenerateInvoice;
        private System.Windows.Forms.Label _lblAmount;
        private System.Windows.Forms.NumericUpDown _numAmount;
        private System.Windows.Forms.Label _lblCurrency;
        private System.Windows.Forms.ComboBox _cmbCurrency;
        private System.Windows.Forms.Button _btnValidateConfig;
        private System.Windows.Forms.GroupBox _grpLog;
        private System.Windows.Forms.TableLayoutPanel _tlpLog;
        private System.Windows.Forms.TextBox _txtLog;
        private System.Windows.Forms.Button _btnClearLog;
    }
}