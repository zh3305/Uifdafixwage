namespace Ec_Changjie
{
    partial class Ec_GZ_from
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.Bt_About = new Glass.GlassButton();
            this.tb_NewMend = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.glassButton1 = new Glass.GlassButton();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Bt_About
            // 
            this.Bt_About.Location = new System.Drawing.Point(363, 33);
            this.Bt_About.Name = "Bt_About";
            this.Bt_About.Size = new System.Drawing.Size(75, 23);
            this.Bt_About.TabIndex = 0;
            this.Bt_About.Text = "关于";
            this.Bt_About.Click += new System.EventHandler(this.Bt_About_Click);
            // 
            // tb_NewMend
            // 
            this.tb_NewMend.Location = new System.Drawing.Point(12, 35);
            this.tb_NewMend.Name = "tb_NewMend";
            this.tb_NewMend.ReadOnly = true;
            this.tb_NewMend.Size = new System.Drawing.Size(171, 21);
            this.tb_NewMend.TabIndex = 1;
            this.tb_NewMend.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tb_do_code_KeyPress);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "最新可操作月份";
            // 
            // glassButton1
            // 
            this.glassButton1.BackColor = System.Drawing.Color.LightGray;
            this.glassButton1.ForeColor = System.Drawing.Color.Red;
            this.glassButton1.Location = new System.Drawing.Point(236, 33);
            this.glassButton1.Name = "glassButton1";
            this.glassButton1.Size = new System.Drawing.Size(75, 23);
            this.glassButton1.TabIndex = 7;
            this.glassButton1.Text = "确定";
            this.glassButton1.Click += new System.EventHandler(this.glassButton1_Click_1);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 73);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(473, 36);
            this.label2.TabIndex = 8;
            this.label2.Text = "    本月工资管理模块工资分摊已制单，当月生成的凭证已被删除，要求重新制单重新点\r\n击工资分摊时，提示“无可制单数据”无法制本月单据\r\n     在执行操作前请" +
    "确认已经删除已经当月工资系统生成的凭证";
            // 
            // Ec_GZ_from
            // 
            this.AllowDrag = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(508, 121);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.glassButton1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tb_NewMend);
            this.Controls.Add(this.Bt_About);
            this.Name = "Ec_GZ_from";
            this.Text = "T3工资分摊不能制单";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Glass.GlassButton Bt_About;
        private System.Windows.Forms.TextBox tb_NewMend;
        private System.Windows.Forms.Label label1;
        private Glass.GlassButton glassButton1;
        private System.Windows.Forms.Label label2;

    }
}

