//UNED
//Mitica X
//Jorge Arias Melendez
//tercer cuatrimestre 2025
//Designer de FrmBatallas.

namespace Miticax.Cliente
{
    partial class FrmBatallas
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing) { if (disposing && (components != null)) components.Dispose(); base.Dispose(disposing); }

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            this.btnBuscar = new System.Windows.Forms.Button();
            this.btnRendirse = new System.Windows.Forms.Button();
            this.txtBitacora = new System.Windows.Forms.TextBox();
            this.lblEstado = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnBuscar
            // 
            this.btnBuscar.Location = new System.Drawing.Point(12, 12);
            this.btnBuscar.Name = "btnBuscar"; this.btnBuscar.Size = new System.Drawing.Size(140, 27);
            this.btnBuscar.Text = "Buscar batalla"; this.btnBuscar.UseVisualStyleBackColor = true;
            this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);
            // 
            // btnRendirse
            // 
            this.btnRendirse.Location = new System.Drawing.Point(170, 12);
            this.btnRendirse.Name = "btnRendirse"; this.btnRendirse.Size = new System.Drawing.Size(120, 27);
            this.btnRendirse.Text = "Rendirse"; this.btnRendirse.UseVisualStyleBackColor = true;
            this.btnRendirse.Click += new System.EventHandler(this.btnRendirse_Click);
            // 
            // txtBitacora
            // 
            this.txtBitacora.Location = new System.Drawing.Point(12, 50);
            this.txtBitacora.Multiline = true; this.txtBitacora.Name = "txtBitacora";
            this.txtBitacora.ReadOnly = true; this.txtBitacora.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtBitacora.Size = new System.Drawing.Size(560, 280);
            // 
            // lblEstado
            // 
            this.lblEstado.AutoSize = true; this.lblEstado.Location = new System.Drawing.Point(310, 18);
            this.lblEstado.Text = "Esperando accion";
            // 
            // FrmBatallas
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F); this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 341);
            this.Controls.Add(this.lblEstado); this.Controls.Add(this.txtBitacora);
            this.Controls.Add(this.btnRendirse); this.Controls.Add(this.btnBuscar);
            this.Name = "FrmBatallas"; this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent; this.Text = "Batallas";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmBatallas_FormClosed);
            this.ResumeLayout(false); this.PerformLayout();
        }
        #endregion

        private System.Windows.Forms.Button btnBuscar;
        private System.Windows.Forms.Button btnRendirse;
        private System.Windows.Forms.TextBox txtBitacora;
        private System.Windows.Forms.Label lblEstado;
    }
}
