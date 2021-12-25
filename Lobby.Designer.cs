
namespace DaVinci_Code
{
    partial class Lobby
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
            this.InputAddress = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.but_JoinRoom = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.but_CreateRoom = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.InputPlayerCount = new System.Windows.Forms.TextBox();
            this.but_MyIPShow = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // InputAddress
            // 
            this.InputAddress.Font = new System.Drawing.Font("맑은 고딕", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.InputAddress.Location = new System.Drawing.Point(407, 171);
            this.InputAddress.Name = "InputAddress";
            this.InputAddress.Size = new System.Drawing.Size(303, 38);
            this.InputAddress.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.label1.Font = new System.Drawing.Font("궁서", 34.8F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.DarkRed;
            this.label1.Location = new System.Drawing.Point(299, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(315, 58);
            this.label1.TabIndex = 1;
            this.label1.Text = "다빈치코드";
            // 
            // but_JoinRoom
            // 
            this.but_JoinRoom.Font = new System.Drawing.Font("맑은 고딕", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.but_JoinRoom.Location = new System.Drawing.Point(489, 344);
            this.but_JoinRoom.Name = "but_JoinRoom";
            this.but_JoinRoom.Size = new System.Drawing.Size(153, 51);
            this.but_JoinRoom.TabIndex = 2;
            this.but_JoinRoom.Text = "입장";
            this.but_JoinRoom.UseVisualStyleBackColor = true;
            this.but_JoinRoom.Click += new System.EventHandler(this.but_JoinRoom_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.label2.Font = new System.Drawing.Font("궁서", 19.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.ForeColor = System.Drawing.SystemColors.MenuText;
            this.label2.Location = new System.Drawing.Point(238, 176);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(152, 33);
            this.label2.TabIndex = 3;
            this.label2.Text = "아이피 -";
            // 
            // but_CreateRoom
            // 
            this.but_CreateRoom.Font = new System.Drawing.Font("맑은 고딕", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.but_CreateRoom.Location = new System.Drawing.Point(261, 346);
            this.but_CreateRoom.Name = "but_CreateRoom";
            this.but_CreateRoom.Size = new System.Drawing.Size(153, 51);
            this.but_CreateRoom.TabIndex = 4;
            this.but_CreateRoom.Text = "방만들기";
            this.but_CreateRoom.UseVisualStyleBackColor = true;
            this.but_CreateRoom.Click += new System.EventHandler(this.but_CreateRoom_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.label3.Font = new System.Drawing.Font("궁서", 19.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.ForeColor = System.Drawing.SystemColors.MenuText;
            this.label3.Location = new System.Drawing.Point(238, 237);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(163, 33);
            this.label3.TabIndex = 6;
            this.label3.Text = "방 인원 -";
            // 
            // InputPlayerCount
            // 
            this.InputPlayerCount.Font = new System.Drawing.Font("맑은 고딕", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.InputPlayerCount.Location = new System.Drawing.Point(407, 237);
            this.InputPlayerCount.Name = "InputPlayerCount";
            this.InputPlayerCount.Size = new System.Drawing.Size(73, 38);
            this.InputPlayerCount.TabIndex = 5;
            // 
            // but_MyIPShow
            // 
            this.but_MyIPShow.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold);
            this.but_MyIPShow.Location = new System.Drawing.Point(778, 12);
            this.but_MyIPShow.Name = "but_MyIPShow";
            this.but_MyIPShow.Size = new System.Drawing.Size(136, 48);
            this.but_MyIPShow.TabIndex = 7;
            this.but_MyIPShow.Text = "내 아이피";
            this.but_MyIPShow.UseVisualStyleBackColor = true;
            this.but_MyIPShow.Click += new System.EventHandler(this.but_MyIPShow_Click);
            // 
            // Lobby
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(926, 503);
            this.Controls.Add(this.but_MyIPShow);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.InputPlayerCount);
            this.Controls.Add(this.but_CreateRoom);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.but_JoinRoom);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.InputAddress);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Lobby";
            this.Text = "Lobby";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox InputAddress;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button but_JoinRoom;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button but_CreateRoom;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox InputPlayerCount;
        private System.Windows.Forms.Button but_MyIPShow;
    }
}