//UNED
//Mitica X
//Jorge Arias Melendez
//tercer cuatrimestre 2025
//Designer de FrmInventario.

namespace Miticax.Cliente
{
    partial class FrmInventario
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing) { if (disposing && (components != null)) components.Dispose(); base.Dispose(disposing); }

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            this.btnRefrescar = new System.Windows.Forms.Button();
            this.grd = new System.Windows.Forms.DataGridView();
            this.colId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNombre = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTipo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNivel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPoder = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblEstado = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.grd)).BeginInit();
            this.SuspendLayout();
            // 
            // btnRefrescar
            // 
            this.btnRefrescar.Location = new System.Drawing.Point(12, 12);
            this.btnRefrescar.Name = "btnRefrescar";
            this.btnRefrescar.Size = new System.Drawing.Size(120, 27);
            this.btnRefrescar.TabIndex = 0;
            this.btnRefrescar.Text = "Refrescar";
            this.btnRefrescar.UseVisualStyleBackColor = true;
            this.btnRefrescar.Click += new System.EventHandler(this.btnRefrescar_Click);
            // 
            // grd
            // 
            this.grd.AllowUserToAddRows = false;
            this.grd.AllowUserToDeleteRows = false;
            this.grd.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grd.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colId,this.colNombre,this.colTipo,this.colNivel,this.colPoder});
            this.grd.Location = new System.Drawing.Point(12, 50);
            this.grd.Name = "grd"; this.grd.ReadOnly = true;
            this.grd.RowHeadersVisible = false; this.grd.Size = new System.Drawing.Size(560, 300);
            this.grd.TabIndex = 1;
            // 
            // Columnas
            // 
            this.colId.HeaderText = "Id"; this.colId.Name = "colId"; this.colId.ReadOnly = true; this.colId.Width = 50;
            this.colNombre.HeaderText = "Nombre"; this.colNombre.Name = "colNombre"; this.colNombre.ReadOnly = true; this.colNombre.Width = 150;
            this.colTipo.HeaderText = "Tipo"; this.colTipo.Name = "colTipo"; this.colTipo.ReadOnly = true; this.colTipo.Width = 120;
            this.colNivel.HeaderText = "Nivel"; this.colNivel.Name = "colNivel"; this.colNivel.ReadOnly = true; this.colNivel.Width = 80;
            this.colPoder.HeaderText = "Poder"; this.colPoder.Name = "colPoder"; this.colPoder.ReadOnly = true; this.colPoder.Width = 80;
            // 
            // lblEstado
            // 
            this.lblEstado.AutoSize = true; this.lblEstado.Location = new System.Drawing.Point(150, 18);
            this.lblEstado.Name = "lblEstado"; this.lblEstado.Size = new System.Drawing.Size(109, 15);
            this.lblEstado.TabIndex = 2; this.lblEstado.Text = "Inventario pendiente";
            // 
            // FrmInventario
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F); this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 361);
            this.Controls.Add(this.lblEstado); this.Controls.Add(this.grd); this.Controls.Add(this.btnRefrescar);
            this.Name = "FrmInventario"; this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent; this.Text = "Inventario de criaturas";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmInventario_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.grd)).EndInit();
            this.ResumeLayout(false); this.PerformLayout();
        }
        #endregion

        private System.Windows.Forms.Button btnRefrescar;
        private System.Windows.Forms.DataGridView grd;
        private System.Windows.Forms.DataGridViewTextBoxColumn colId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNombre;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTipo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNivel;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPoder;
        private System.Windows.Forms.Label lblEstado;
    }
}
