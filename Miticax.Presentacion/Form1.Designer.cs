//UNED
//Mitica X
//Jorge Arias Melendez
//Septiembre 2025
//Formulario principal: codigo generado por el diseñador (InitializeComponent y Dispose).

namespace Miticax.Presentacion
{
    // Parcial: esta parte la mantiene el diseñador. Contiene InitializeComponent y fields de controles.
    partial class Form1
    {
        /// <summary>
        /// Contenedor de componentes del formulario.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Liberar recursos usados.
        /// </summary>
        /// <param name="disposing">true si se deben desechar recursos administrados.</param>
        protected override void Dispose(bool disposing)
        {
            // Si el disposing es verdadero y hay componentes, liberarlos.
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            // Llamar a la clase base para completar la liberacion.
            base.Dispose(disposing);
        }

        /// <summary>
        /// Metodo donde el diseñador crea y configura los controles.
        /// Debe existir SOLO en el archivo Designer.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F); // escala de fuente base
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;   // modo de auto escalado
            this.ClientSize = new System.Drawing.Size(800, 450);            // tamano inicial de la ventana
            this.Name = "Form1";                                            // nombre del formulario
            this.Text = "Mitica X - Form1";                                 // titulo de la ventana
            this.ResumeLayout(false);                                       // fin de la suspension del layout
        }
    }
}
