/*
 *  Interfaz para electrocardiograma (ECG) v3.5.11 (estable)
 *
 *  Docente: Varela Lara Luis Julián
 *  Equipo: Teletobbies
 *
 *  Interfaz realizada para trabajo final de materia Tópicos avanzados de INEL II
 *  realizada por alumnos de la Universidad Veracruzana (UV)
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Electrocardiografo
{
    public partial class Form1 : Form
    {
        Bitmap Papel;                               // Inicializar grafica
        Pen Lapiz, Borrador;                        // Inicializar lapices
        Graphics Dibujante;                         // Inicializar dibujante

        Double ECG = 0, ECGAnterior = 0;            // Inicializar variables para guardar la señal

        int Tiempo = 0;                             // Inicializar iterador del eje tiempo
        int Segundos = 0;                           // Inicializar iterador de tiempo en segundos
        int picos = 1;                              // Inicializar contador de latidos
        int band = 0;                               // Inicializar bandera para estado de la imagen de corazon 

        public Form1()
        {
            InitializeComponent();
        }

        //////////////////////////////////<-	Boton de conectar   ->/////////////////////////////////////
        private void Conectar_Click(object sender, EventArgs e)
        {
            s.BaudRate = Convert.ToInt32(comboBoxBaudRate.Text);        // Convertir datos de comboBox y asignarlo
            s.PortName = comboBoxPuerto.Text;                           // Asignar puerto disponible
            s.Open();                                                   // Activar comunicacion serial

            circularProgressBar1.Style= ProgressBarStyle.Continuous;    // Cambiar de modo a grafica circular

            timer2.Enabled = true;                                      // Activar Timers
            timer1.Enabled = true;
            
        }

        //////////////////////////<-	Impresion de datos en grafica   ->////////////////////////////////
        private void timer1_Tick(object sender, EventArgs e)
        {
            int Dato;                                                   // Variable auxiliar

            while (s.BytesToRead > 0)                                   // Mientras haya datos por leer
            {
                Dato = s.ReadByte();                                    // Guardar valor en variable
                ECG = Dato;                                             
                Dibujante.DrawLine(Borrador, Tiempo, 0, Tiempo, 600);                                   // Parametros para grafica
                Dibujante.DrawLine(Lapiz, Tiempo - 1, (int)ECGAnterior + 20, Tiempo, (int)ECG + 20);
                Tiempo++;                                               // Incrementar valor de tiempo en 1

                if (ECG == 0 && ECGAnterior != 0)                       // Detectar caida de latido
                {
                    picos = picos + 1;                                  // Incrementar valor de los latidos registrados
                }

                if (Segundos >= 10)                                     // ¿Pasaron 10 segundos?
                {
                    picos = picos*6;                                    // Calcular latidos por minuto
                    label3.Text = picos.ToString();                     // Imprimir latidos por minuto

                    Tiempo = 0;                                         // Refrescar datos
                    Segundos = 0;
                    picos = 1;
                }

                Plantilla.Image = Papel;                                // Refrescar plantilla

                ECGAnterior = ECG;                                      // Guardar dato anterior
            }
        }

        //////////////////////////////////<-	Cronometro   ->/////////////////////////////////////
        private void timer2_Tick(object sender, EventArgs e)
        {
            Segundos = Segundos + 1;                                    // Incrementar valor de tiempo en 1
            circularProgressBar1.Value = Convert.ToInt32(Segundos);     // Enviar datos a grafica circular

            if (band == 0)                              // Alternar la aparicion del corazon 
            {
                pictureBox2.Visible = true;
                band= 1;
            }
            else
            {
                pictureBox2.Visible = false; 
                band= 0;
            }
        }

        //////////////////////////////////<-	Parametros iniciales   ->/////////////////////////////////////
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {       
                Lapiz = new Pen(Color.LightSkyBlue, 2);                                 // Inicializar parametros para graficar
                Borrador = new Pen(Color.Black, 1);
                Papel = new Bitmap(2500, 200);
                Dibujante = Graphics.FromImage(Papel);

                Plantilla.Image = Papel;                                                // Refrescar plantilla

                string[] portList = SerialPort.GetPortNames();                          // Buscar y guardar puertos disponibles
                comboBoxPuerto.Items.AddRange(portList);                                // Añadir puertos encontrados

                string[] rates = {"9600", "38400", "57600", "115200", "250000"};        // Guardar velocidades permitidas
                comboBoxBaudRate.DataSource = rates;                                    // Añadir velocidades
            }

            catch (Exception error)                                                     // ¿Ocurrio algun error?
            {
                MessageBox.Show(error.Message);
            }
        }

    }
}
