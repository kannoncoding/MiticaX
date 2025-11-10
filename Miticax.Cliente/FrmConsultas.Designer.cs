//UNED
//Mitica X
//Jorge Arias Melendez
//tercer cuatrimestre 2025
//Designer de FrmConsultas.

namespace Miticax.Cliente
{
    partial class FrmConsultas
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing) { if (disposing && (components != null)) components.Dispose(); base.Dispose(disposing); }

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            this.btnTop10 = new System.Windows.Forms.Button();
            this.btnHistorial = new System.Windows.Forms.Button();
            this.btnRondas = new System.Windows.Forms.Button();
            this.grd = new System.Windows.Forms.DataGridView();
            this.lblEstado = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.grd)).BeginInit();
            this.SuspendLayout();
            // 
            // botones
            // 
            this.btnTop10.Location = new System.Drawing.Point(12, 12);
            this.btnTop10.Name = "btnTop10"; this.btnTop10.Size = new System.Drawing.Size(120, 27);
            this.btnTop10.Text = "Top 10"; this.btnTop10.UseVisualStyleBackColor = true;
            this.btnTop10.Click += new System.EventHandler(this.btnTop10_Click);

            this.btnHistorial.Location = new System.Drawing.Point(150, 12);
            this.btnHistorial.Name = "btnHistorial"; this.btnHistorial.Size = new System.Drawing.Size(120, 27);
            this.btnHistorial.Text = "Historial"; this.btnHistorial.UseVisualStyleBackColor = true;
            this.btnHistorial.Click += new System.EventHandler(this.btnHistorial_Click);

            this.btnRondas.Location = new System.Drawing.Point(288, 12);
            this.btnRondas.Name = "btnRondas"; this.btnRondas.Size = new System.Drawing.Size(120, 27);
            this.btnRondas.Text = "Rondas"; this.btnRondas.UseVisualStyleBackColor = true;
            this.btnRondas.Click += new System.EventHandler(this.btnRondas_Click);
            // 
            // grid
            // 
            this.grd.AllowUserToAddRows = false; this.grd.AllowUserToDeleteRows = false;
            this.grd.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grd.Location = new System.Drawing.Point(12, 50);
            this.grd.Name = "grd"; this.grd.ReadOnly = true; this.grd.RowHeadersVisible = false; this.grd.Size = new System.Drawing.Size(560, 300);
            // 
            // lblEstado
            // 
            this.lblEstado.AutoSize = true; this.lblEstado.Location = new System.Drawing.Point(430, 18);
            this.lblEstado.Text = "Esperando consulta";
            // 
            // FrmConsultas
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F); this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 361);
            this.Controls.Add(this.lblEstado); this.Controls.Add(this.grd);
            this.Controls.Add(this.btnRondas); this.Controls.Add(this.btnHistorial); this.Controls.Add(this.btnTop10);
            this.Name = "FrmConsultas"; this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent; this.Text = "Consultas";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmConsultas_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.grd)).EndInit();
            this.ResumeLayout(false); this.PerformLayout();
        }
        #endregion

        private System.Windows.Forms.Button btnTop10;
        private System.Windows.Forms.Button btnHistorial;
        private System.Windows.Forms.Button btnRondas;
        private System.Windows.Forms.DataGridView grd;
        private System.Windows.Forms.Label lblEstado;
    }
}
