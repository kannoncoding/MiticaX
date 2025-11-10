//UNED
//Mitica X
//Jorge Arias Melendez
//tercer cuatrimestre 2025
//Archivo Designer para FrmClienteTcp: define y configura los controles del formulario cliente TCP (incluye Login y Modulos).

namespace Miticax.Cliente
{
    partial class FrmClienteTcp
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        private void InitializeComponent()
        {
            this.lblHost = new System.Windows.Forms.Label();
            this.lblPuerto = new System.Windows.Forms.Label();
            this.txtHost = new System.Windows.Forms.TextBox();
            this.txtPuerto = new System.Windows.Forms.TextBox();
            this.btnConectar = new System.Windows.Forms.Button();
            this.btnDesconectar = new System.Windows.Forms.Button();
            this.txtEnviar = new System.Windows.Forms.TextBox();
            this.btnEnviar = new System.Windows.Forms.Button();
            this.btnPing = new System.Windows.Forms.Button();
            this.grpLogin = new System.Windows.Forms.GroupBox();
            this.lblLoginEstado = new System.Windows.Forms.Label();
            this.btnLogin = new System.Windows.Forms.Button();
            this.txtClave = new System.Windows.Forms.TextBox();
            this.lblClave = new System.Windows.Forms.Label();
            this.txtUsuario = new System.Windows.Forms.TextBox();
            this.lblUsuario = new System.Windows.Forms.Label();
            this.grpModulos = new System.Windows.Forms.GroupBox();
            this.btnConsultas = new System.Windows.Forms.Button();
            this.btnBatallas = new System.Windows.Forms.Button();
            this.btnEquipos = new System.Windows.Forms.Button();
            this.btnInventario = new System.Windows.Forms.Button();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.grpLogin.SuspendLayout();
            this.grpModulos.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblHost
            // 
            this.lblHost.AutoSize = true;
            this.lblHost.Location = new System.Drawing.Point(10, 15);
            this.lblHost.Name = "lblHost";
            this.lblHost.Size = new System.Drawing.Size(35, 15);
            this.lblHost.TabIndex = 0;
            this.lblHost.Text = "Host:";
            // 
            // lblPuerto
            // 
            this.lblPuerto.AutoSize = true;
            this.lblPuerto.Location = new System.Drawing.Point(270, 15);
            this.lblPuerto.Name = "lblPuerto";
            this.lblPuerto.Size = new System.Drawing.Size(47, 15);
            this.lblPuerto.TabIndex = 1;
            this.lblPuerto.Text = "Puerto:";
            // 
            // txtHost
            // 
            this.txtHost.Location = new System.Drawing.Point(70, 12);
            this.txtHost.Name = "txtHost";
            this.txtHost.Size = new System.Drawing.Size(180, 23);
            this.txtHost.TabIndex = 2;
            // 
            // txtPuerto
            // 
            this.txtPuerto.Location = new System.Drawing.Point(330, 12);
            this.txtPuerto.Name = "txtPuerto";
            this.txtPuerto.Size = new System.Drawing.Size(80, 23);
            this.txtPuerto.TabIndex = 3;
            // 
            // btnConectar
            // 
            this.btnConectar.Location = new System.Drawing.Point(430, 10);
            this.btnConectar.Name = "btnConectar";
            this.btnConectar.Size = new System.Drawing.Size(100, 27);
            this.btnConectar.TabIndex = 4;
            this.btnConectar.Text = "Conectar";
            this.btnConectar.UseVisualStyleBackColor = true;
            this.btnConectar.Click += new System.EventHandler(this.BtnConectar_Click);
            // 
            // btnDesconectar
            // 
            this.btnDesconectar.Location = new System.Drawing.Point(540, 10);
            this.btnDesconectar.Name = "btnDesconectar";
            this.btnDesconectar.Size = new System.Drawing.Size(110, 27);
            this.btnDesconectar.TabIndex = 5;
            this.btnDesconectar.Text = "Desconectar";
            this.btnDesconectar.UseVisualStyleBackColor = true;
            this.btnDesconectar.Click += new System.EventHandler(this.BtnDesconectar_Click);
            // 
            // txtEnviar
            // 
            this.txtEnviar.Location = new System.Drawing.Point(10, 50);
            this.txtEnviar.Name = "txtEnviar";
            this.txtEnviar.Size = new System.Drawing.Size(520, 23);
            this.txtEnviar.TabIndex = 6;
            // 
            // btnEnviar
            // 
            this.btnEnviar.Location = new System.Drawing.Point(540, 48);
            this.btnEnviar.Name = "btnEnviar";
            this.btnEnviar.Size = new System.Drawing.Size(110, 27);
            this.btnEnviar.TabIndex = 7;
            this.btnEnviar.Text = "Enviar linea";
            this.btnEnviar.UseVisualStyleBackColor = true;
            this.btnEnviar.Click += new System.EventHandler(this.BtnEnviar_Click);
            // 
            // btnPing
            // 
            this.btnPing.Location = new System.Drawing.Point(660, 48);
            this.btnPing.Name = "btnPing";
            this.btnPing.Size = new System.Drawing.Size(120, 27);
            this.btnPing.TabIndex = 8;
            this.btnPing.Text = "PING";
            this.btnPing.UseVisualStyleBackColor = true;
            this.btnPing.Click += new System.EventHandler(this.BtnPing_Click);
            // 
            // grpLogin
            // 
            this.grpLogin.Controls.Add(this.lblLoginEstado);
            this.grpLogin.Controls.Add(this.btnLogin);
            this.grpLogin.Controls.Add(this.txtClave);
            this.grpLogin.Controls.Add(this.lblClave);
            this.grpLogin.Controls.Add(this.txtUsuario);
            this.grpLogin.Controls.Add(this.lblUsuario);
            this.grpLogin.Location = new System.Drawing.Point(10, 90);
            this.grpLogin.Name = "grpLogin";
            this.grpLogin.Size = new System.Drawing.Size(770, 80);
            this.grpLogin.TabIndex = 10;
            this.grpLogin.TabStop = false;
            this.grpLogin.Text = "Login";
            // 
            // lblLoginEstado
            // 
            this.lblLoginEstado.AutoSize = true;
            this.lblLoginEstado.Location = new System.Drawing.Point(560, 33);
            this.lblLoginEstado.Name = "lblLoginEstado";
            this.lblLoginEstado.Size = new System.Drawing.Size(86, 15);
            this.lblLoginEstado.TabIndex = 5;
            this.lblLoginEstado.Text = "No autenticado";
            // 
            // btnLogin
            // 
            this.btnLogin.Location = new System.Drawing.Point(465, 28);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(80, 27);
            this.btnLogin.TabIndex = 4;
            this.btnLogin.Text = "Login";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new System.EventHandler(this.BtnLogin_Click);
            // 
            // txtClave
            // 
            this.txtClave.Location = new System.Drawing.Point(300, 30);
            this.txtClave.Name = "txtClave";
            this.txtClave.PasswordChar = '*';
            this.txtClave.Size = new System.Drawing.Size(150, 23);
            this.txtClave.TabIndex = 3;
            // 
            // lblClave
            // 
            this.lblClave.AutoSize = true;
            this.lblClave.Location = new System.Drawing.Point(250, 33);
            this.lblClave.Name = "lblClave";
            this.lblClave.Size = new System.Drawing.Size(41, 15);
            this.lblClave.TabIndex = 2;
            this.lblClave.Text = "Clave:";
            // 
            // txtUsuario
            // 
            this.txtUsuario.Location = new System.Drawing.Point(70, 30);
            this.txtUsuario.Name = "txtUsuario";
            this.txtUsuario.Size = new System.Drawing.Size(150, 23);
            this.txtUsuario.TabIndex = 1;
            // 
            // lblUsuario
            // 
            this.lblUsuario.AutoSize = true;
            this.lblUsuario.Location = new System.Drawing.Point(10, 33);
            this.lblUsuario.Name = "lblUsuario";
            this.lblUsuario.Size = new System.Drawing.Size(52, 15);
            this.lblUsuario.TabIndex = 0;
            this.lblUsuario.Text = "Usuario:";
            // 
            // grpModulos
            // 
            this.grpModulos.Controls.Add(this.btnConsultas);
            this.grpModulos.Controls.Add(this.btnBatallas);
            this.grpModulos.Controls.Add(this.btnEquipos);
            this.grpModulos.Controls.Add(this.btnInventario);
            this.grpModulos.Location = new System.Drawing.Point(10, 180);
            this.grpModulos.Name = "grpModulos";
            this.grpModulos.Size = new System.Drawing.Size(770, 60);
            this.grpModulos.TabIndex = 12;
            this.grpModulos.TabStop = false;
            this.grpModulos.Text = "Modulos";
            this.grpModulos.Enabled = false; // deshabilitado hasta que haya LOGIN_OK
            // 
            // btnConsultas
            // 
            this.btnConsultas.Location = new System.Drawing.Point(475, 22);
            this.btnConsultas.Name = "btnConsultas";
            this.btnConsultas.Size = new System.Drawing.Size(120, 27);
            this.btnConsultas.TabIndex = 3;
            this.btnConsultas.Text = "Consultas";
            this.btnConsultas.UseVisualStyleBackColor = true;
            this.btnConsultas.Click += new System.EventHandler(this.BtnConsultas_Click);
            // 
            // btnBatallas
            // 
            this.btnBatallas.Location = new System.Drawing.Point(335, 22);
            this.btnBatallas.Name = "btnBatallas";
            this.btnBatallas.Size = new System.Drawing.Size(120, 27);
            this.btnBatallas.TabIndex = 2;
            this.btnBatallas.Text = "Batallas";
            this.btnBatallas.UseVisualStyleBackColor = true;
            this.btnBatallas.Click += new System.EventHandler(this.BtnBatallas_Click);
            // 
            // btnEquipos
            // 
            this.btnEquipos.Location = new System.Drawing.Point(195, 22);
            this.btnEquipos.Name = "btnEquipos";
            this.btnEquipos.Size = new System.Drawing.Size(120, 27);
            this.btnEquipos.TabIndex = 1;
            this.btnEquipos.Text = "Equipos";
            this.btnEquipos.UseVisualStyleBackColor = true;
            this.btnEquipos.Click += new System.EventHandler(this.BtnEquipos_Click);
            // 
            // btnInventario
            // 
            this.btnInventario.Location = new System.Drawing.Point(15, 22);
            this.btnInventario.Name = "btnInventario";
            this.btnInventario.Size = new System.Drawing.Size(150, 27);
            this.btnInventario.TabIndex = 0;
            this.btnInventario.Text = "Inventario de criaturas";
            this.btnInventario.UseVisualStyleBackColor = true;
            this.btnInventario.Click += new System.EventHandler(this.BtnInventario_Click);
            // 
            // txtLog
            // 
            this.txtLog.Location = new System.Drawing.Point(10, 250);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(770, 220);
            this.txtLog.TabIndex = 13;
            // 
            // FrmClienteTcp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 480);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.grpModulos);
            this.Controls.Add(this.grpLogin);
            this.Controls.Add(this.btnPing);
            this.Controls.Add(this.btnEnviar);
            this.Controls.Add(this.txtEnviar);
            this.Controls.Add(this.btnDesconectar);
            this.Controls.Add(this.btnConectar);
            this.Controls.Add(this.txtPuerto);
            this.Controls.Add(this.txtHost);
            this.Controls.Add(this.lblPuerto);
            this.Controls.Add(this.lblHost);
            this.Name = "FrmClienteTcp";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Cliente TCP - MiticaX";
            this.grpLogin.ResumeLayout(false);
            this.grpLogin.PerformLayout();
            this.grpModulos.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblHost;
        private System.Windows.Forms.Label lblPuerto;
        private System.Windows.Forms.TextBox txtHost;
        private System.Windows.Forms.TextBox txtPuerto;
        private System.Windows.Forms.Button btnConectar;
        private System.Windows.Forms.Button btnDesconectar;
        private System.Windows.Forms.TextBox txtEnviar;
        private System.Windows.Forms.Button btnEnviar;
        private System.Windows.Forms.Button btnPing;

        // Login
        private System.Windows.Forms.GroupBox grpLogin;
        private System.Windows.Forms.Label lblLoginEstado;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.TextBox txtClave;
        private System.Windows.Forms.Label lblClave;
        private System.Windows.Forms.TextBox txtUsuario;
        private System.Windows.Forms.Label lblUsuario;

        // Modulos
        private System.Windows.Forms.GroupBox grpModulos;
        private System.Windows.Forms.Button btnInventario;
        private System.Windows.Forms.Button btnEquipos;
        private System.Windows.Forms.Button btnBatallas;
        private System.Windows.Forms.Button btnConsultas;

        private System.Windows.Forms.TextBox txtLog;
    }
}
