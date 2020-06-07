namespace ClientProject
{
    partial class formStartGame
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtNickname = new System.Windows.Forms.TextBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.rdoImage4 = new System.Windows.Forms.RadioButton();
            this.rdoImage3 = new System.Windows.Forms.RadioButton();
            this.rdoImage2 = new System.Windows.Forms.RadioButton();
            this.rdoImage1 = new System.Windows.Forms.RadioButton();
            this.lblAlert = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Century Gothic", 48F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(80, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(492, 93);
            this.label1.TabIndex = 0;
            this.label1.Text = "Catch Mind";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.Location = new System.Drawing.Point(184, 187);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 20);
            this.label3.TabIndex = 2;
            this.label3.Text = "닉네임 ";
            // 
            // txtNickname
            // 
            this.txtNickname.Location = new System.Drawing.Point(296, 182);
            this.txtNickname.Name = "txtNickname";
            this.txtNickname.Size = new System.Drawing.Size(177, 25);
            this.txtNickname.TabIndex = 3;
            // 
            // btnStart
            // 
            this.btnStart.Font = new System.Drawing.Font("HY견고딕", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnStart.Location = new System.Drawing.Point(212, 470);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(199, 61);
            this.btnStart.TabIndex = 4;
            this.btnStart.Text = "시작하기";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.pictureBox4);
            this.groupBox1.Controls.Add(this.pictureBox3);
            this.groupBox1.Controls.Add(this.pictureBox2);
            this.groupBox1.Controls.Add(this.pictureBox1);
            this.groupBox1.Controls.Add(this.rdoImage4);
            this.groupBox1.Controls.Add(this.rdoImage3);
            this.groupBox1.Controls.Add(this.rdoImage2);
            this.groupBox1.Controls.Add(this.rdoImage1);
            this.groupBox1.Location = new System.Drawing.Point(67, 269);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(521, 181);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "프로필 선택";
            // 
            // pictureBox4
            // 
            this.pictureBox4.Image = global::ClientProject.Properties.Resources.라이언;
            this.pictureBox4.Location = new System.Drawing.Point(399, 58);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(100, 97);
            this.pictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox4.TabIndex = 7;
            this.pictureBox4.TabStop = false;
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = global::ClientProject.Properties.Resources.어피치;
            this.pictureBox3.Location = new System.Drawing.Point(282, 58);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(100, 97);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox3.TabIndex = 6;
            this.pictureBox3.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::ClientProject.Properties.Resources.네오;
            this.pictureBox2.Location = new System.Drawing.Point(149, 58);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(102, 97);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 5;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::ClientProject.Properties.Resources.무지;
            this.pictureBox1.Location = new System.Drawing.Point(29, 58);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(100, 97);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            // 
            // rdoImage4
            // 
            this.rdoImage4.AutoSize = true;
            this.rdoImage4.Location = new System.Drawing.Point(394, 33);
            this.rdoImage4.Name = "rdoImage4";
            this.rdoImage4.Size = new System.Drawing.Size(62, 19);
            this.rdoImage4.TabIndex = 3;
            this.rdoImage4.TabStop = true;
            this.rdoImage4.Text = "Ryan";
            this.rdoImage4.UseVisualStyleBackColor = true;
            // 
            // rdoImage3
            // 
            this.rdoImage3.AutoSize = true;
            this.rdoImage3.Location = new System.Drawing.Point(277, 33);
            this.rdoImage3.Name = "rdoImage3";
            this.rdoImage3.Size = new System.Drawing.Size(77, 19);
            this.rdoImage3.TabIndex = 2;
            this.rdoImage3.TabStop = true;
            this.rdoImage3.Text = "Apeach";
            this.rdoImage3.UseVisualStyleBackColor = true;
            // 
            // rdoImage2
            // 
            this.rdoImage2.AutoSize = true;
            this.rdoImage2.Location = new System.Drawing.Point(149, 33);
            this.rdoImage2.Name = "rdoImage2";
            this.rdoImage2.Size = new System.Drawing.Size(54, 19);
            this.rdoImage2.TabIndex = 1;
            this.rdoImage2.TabStop = true;
            this.rdoImage2.Text = "Neo";
            this.rdoImage2.UseVisualStyleBackColor = true;
            // 
            // rdoImage1
            // 
            this.rdoImage1.AutoSize = true;
            this.rdoImage1.Location = new System.Drawing.Point(29, 33);
            this.rdoImage1.Name = "rdoImage1";
            this.rdoImage1.Size = new System.Drawing.Size(59, 19);
            this.rdoImage1.TabIndex = 0;
            this.rdoImage1.TabStop = true;
            this.rdoImage1.Text = "Muzi";
            this.rdoImage1.UseVisualStyleBackColor = true;
            // 
            // lblAlert
            // 
            this.lblAlert.AutoSize = true;
            this.lblAlert.Location = new System.Drawing.Point(293, 242);
            this.lblAlert.Name = "lblAlert";
            this.lblAlert.Size = new System.Drawing.Size(0, 15);
            this.lblAlert.TabIndex = 7;
            // 
            // formStartGame
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(633, 584);
            this.Controls.Add(this.lblAlert);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.txtNickname);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Name = "formStartGame";
            this.Text = "StartGame";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.formStartGame_FormClosed);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtNickname;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.RadioButton rdoImage4;
        private System.Windows.Forms.RadioButton rdoImage3;
        private System.Windows.Forms.RadioButton rdoImage2;
        private System.Windows.Forms.RadioButton rdoImage1;
        private System.Windows.Forms.Label lblAlert;
    }
}

