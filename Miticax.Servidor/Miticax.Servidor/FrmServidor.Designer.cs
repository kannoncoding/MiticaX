//UNED
//Mitica X
//Jorge Arias Melendez
//Tercer cuatrimestre 2025
//Diseño de controles del formulario del servidor TCP.

using System.Windows.Forms;

namespace Miticax.Presentacion
{
    partial class FrmServidor
    {
        private System.ComponentModel.IContainer components = null;
        private Button btnIniciar;
        private Button btnDetener;
        private Button btnLimpiar;
        private RichTextBox rtbBitacora;
        private Label lblPuerto;
        private TextBox txtPuerto;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.btnIniciar = new Button();
            this.btnDetener = new Button();
            this.btnLimpiar = new Button();
            this.rtbBitacora = new RichTextBox();
            this.lblPuerto = new Label();
            this.txtPuerto = new TextBox();
            this.SuspendLayout();
            // 
            // btnIniciar
            // 
            this.btnIniciar.Location = new System.Drawing.Point(20, 55);
            this.btnIniciar.Name = "btnIniciar";
            this.btnIniciar.Size = new System.Drawing.Size(90, 30);
            this.btnIniciar.TabIndex = 0;
            this.btnIniciar.Text = "Iniciar";
            this.btnIniciar.UseVisualStyleBackColor = true;
            this.btnIniciar.Click += new System.EventHandler(this.btnIniciar_Click);
            // 
            // btnDetener
            // 
            this.btnDetener.Enabled = false;
            this.btnDetener.Location = new System.Drawing.Point(120, 55);
            this.btnDetener.Name = "btnDetener";
            this.btnDetener.Size = new System.Drawing.Size(90, 30);
            this.btnDetener.TabIndex = 1;
            this.btnDetener.Text = "Detener";
            this.btnDetener.UseVisualStyleBackColor = true;
            this.btnDetener.Click += new System.EventHandler(this.btnDetener_Click);
            // 
            // btnLimpiar
            // 
            this.btnLimpiar.Location = new System.Drawing.Point(220, 55);
            this.btnLimpiar.Name = "btnLimpiar";
            this.btnLimpiar.Size = new System.Drawing.Size(90, 30);
            this.btnLimpiar.TabIndex = 2;
            this.btnLimpiar.Text = "Limpiar";
            this.btnLimpiar.UseVisualStyleBackColor = true;
            this.btnLimpiar.Click += new System.EventHandler(this.btnLimpiar_Click);
            // 
            // rtbBitacora
            // 
            this.rtbBitacora.Location = new System.Drawing.Point(20, 100);
            this.rtbBitacora.Name = "rtbBitacora";
            this.rtbBitacora.ReadOnly = true;
            this.rtbBitacora.Size = new System.Drawing.Size(560, 300);
            this.rtbBitacora.TabIndex = 3;
            this.rtbBitacora.Text = "";
            this.rtbBitacora.WordWrap = false;
            // 
            // lblPuerto
            // 
            this.lblPuerto.AutoSize = true;
            this.lblPuerto.Location = new System.Drawing.Point(20, 20);
            this.lblPuerto.Name = "lblPuerto";
            this.lblPuerto.Size = new System.Drawing.Size(45, 15);
            this.lblPuerto.TabIndex = 4;
            this.lblPuerto.Text = "Puerto:";
            // 
            // txtPuerto
            // 
            this.txtPuerto.Location = new System.Drawing.Point(75, 17);
            this.txtPuerto.Name = "txtPuerto";
            this.txtPuerto.Size = new System.Drawing.Size(85, 23);
            this.txtPuerto.TabIndex = 5;
            this.txtPuerto.Text = "14100";
            // 
            // FrmServidor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 420);
            this.Controls.Add(this.txtPuerto);
            this.Controls.Add(this.lblPuerto);
            this.Controls.Add(this.rtbBitacora);
            this.Controls.Add(this.btnLimpiar);
            this.Controls.Add(this.btnDetener);
            this.Controls.Add(this.btnIniciar);
            this.Name = "FrmServidor";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "MITICAX - Servidor TCP (Phase 3)";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
