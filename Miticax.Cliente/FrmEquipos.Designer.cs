//UNED
//Mitica X
//Jorge Arias Melendez
//tercer cuatrimestre 2025
//Designer de FrmEquipos.

namespace Miticax.Cliente
{
    partial class FrmEquipos
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing) { if (disposing && (components != null)) components.Dispose(); base.Dispose(disposing); }

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            this.btnListarEquipos = new System.Windows.Forms.Button();
            this.btnVerMiembros = new System.Windows.Forms.Button();
            this.grdEquipos = new System.Windows.Forms.DataGridView();
            this.colEqId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEqNombre = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.grdMiembros = new System.Windows.Forms.DataGridView();
            this.colMiId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMiNombre = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMiNivel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblEstadoEquipos = new System.Windows.Forms.Label();
            this.lblEstadoMiembros = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.grdEquipos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdMiembros)).BeginInit();
            this.SuspendLayout();
            // 
            // botones
            // 
            this.btnListarEquipos.Location = new System.Drawing.Point(12, 12);
            this.btnListarEquipos.Name = "btnListarEquipos"; this.btnListarEquipos.Size = new System.Drawing.Size(120, 27);
            this.btnListarEquipos.Text = "Listar equipos"; this.btnListarEquipos.UseVisualStyleBackColor = true;
            this.btnListarEquipos.Click += new System.EventHandler(this.btnListarEquipos_Click);
            this.btnVerMiembros.Location = new System.Drawing.Point(150, 12);
            this.btnVerMiembros.Name = "btnVerMiembros"; this.btnVerMiembros.Size = new System.Drawing.Size(140, 27);
            this.btnVerMiembros.Text = "Ver miembros";
            this.btnVerMiembros.UseVisualStyleBackColor = true; this.btnVerMiembros.Click += new System.EventHandler(this.btnVerMiembros_Click);
            // 
            // grdEquipos
            // 
            this.grdEquipos.AllowUserToAddRows = false; this.grdEquipos.AllowUserToDeleteRows = false;
            this.grdEquipos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdEquipos.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { this.colEqId, this.colEqNombre });
            this.grdEquipos.Location = new System.Drawing.Point(12, 50); this.grdEquipos.Name = "grdEquipos";
            this.grdEquipos.ReadOnly = true; this.grdEquipos.RowHeadersVisible = false; this.grdEquipos.Size = new System.Drawing.Size(320, 300);
            // 
            // columnas equipos
            // 
            this.colEqId.HeaderText = "Id"; this.colEqId.Name = "colEqId"; this.colEqId.ReadOnly = true; this.colEqId.Width = 60;
            this.colEqNombre.HeaderText = "Nombre"; this.colEqNombre.Name = "colEqNombre"; this.colEqNombre.ReadOnly = true; this.colEqNombre.Width = 240;
            // 
            // grdMiembros
            // 
            this.grdMiembros.AllowUserToAddRows = false; this.grdMiembros.AllowUserToDeleteRows = false;
            this.grdMiembros.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdMiembros.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { this.colMiId, this.colMiNombre, this.colMiNivel });
            this.grdMiembros.Location = new System.Drawing.Point(350, 50); this.grdMiembros.Name = "grdMiembros";
            this.grdMiembros.ReadOnly = true; this.grdMiembros.RowHeadersVisible = false; this.grdMiembros.Size = new System.Drawing.Size(420, 300);
            // 
            // columnas miembros
            // 
            this.colMiId.HeaderText = "Id"; this.colMiId.Name = "colMiId"; this.colMiId.ReadOnly = true; this.colMiId.Width = 60;
            this.colMiNombre.HeaderText = "Nombre"; this.colMiNombre.Name = "colMiNombre"; this.colMiNombre.ReadOnly = true; this.colMiNombre.Width = 240;
            this.colMiNivel.HeaderText = "Nivel"; this.colMiNivel.Name = "colMiNivel"; this.colMiNivel.ReadOnly = true; this.colMiNivel.Width = 80;
            // 
            // estados
            // 
            this.lblEstadoEquipos.AutoSize = true; this.lblEstadoEquipos.Location = new System.Drawing.Point(310, 18);
            this.lblEstadoEquipos.Text = "Equipos: 0";
            this.lblEstadoMiembros.AutoSize = true; this.lblEstadoMiembros.Location = new System.Drawing.Point(500, 18);
            this.lblEstadoMiembros.Text = "Miembros: 0";
            // 
            // FrmEquipos
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F); this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 361);
            this.Controls.Add(this.lblEstadoMiembros); this.Controls.Add(this.lblEstadoEquipos);
            this.Controls.Add(this.grdMiembros); this.Controls.Add(this.grdEquipos);
            this.Controls.Add(this.btnVerMiembros); this.Controls.Add(this.btnListarEquipos);
            this.Name = "FrmEquipos"; this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent; this.Text = "Equipos";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmEquipos_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.grdEquipos)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdMiembros)).EndInit();
            this.ResumeLayout(false); this.PerformLayout();
        }
        #endregion

        private System.Windows.Forms.Button btnListarEquipos;
        private System.Windows.Forms.Button btnVerMiembros;
        private System.Windows.Forms.DataGridView grdEquipos;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEqId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEqNombre;
        private System.Windows.Forms.DataGridView grdMiembros;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMiId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMiNombre;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMiNivel;
        private System.Windows.Forms.Label lblEstadoEquipos;
        private System.Windows.Forms.Label lblEstadoMiembros;
    }
}
