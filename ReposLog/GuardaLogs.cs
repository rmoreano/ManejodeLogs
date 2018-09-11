using System;
using System.Data.SqlClient;
using System.Configuration;

namespace ReposLog
{
    public class GuardaLogs
    {
        private static bool cArchivo;
        private static bool cConsola;
        private static bool cMensaje;
        private static bool cAdvertencia;
        private static bool cError;
        private static bool cBasededatos;
        private static bool cInicializar;
                          
        public GuardaLogs() : this(true, false, true, false, true, true)
        {
        }

        public GuardaLogs(bool _Archivo, bool _Consola, bool _Mensaje,bool _Advertencia, bool _Error, bool _Basededatos)
        {
            cArchivo  = _Archivo ;
            cConsola  = _Consola;
            cMensaje  = _Mensaje;
            cAdvertencia  = _Basededatos ;
            cError  = _Advertencia;
            cBasededatos  = _Error;
        }
        public static void LogMessage(string Mensaje, bool ActivarMensaje, bool ActivarAdvertencia , bool ActivarError)
        {
            Mensaje.Trim();

            if (Mensaje == null || Mensaje.Length == 0)
            {
                return;
            }
            if (!cConsola  && !cArchivo && !cBasededatos )
            {
                throw new Exception("Configuracion invalida");
            }
            if ((!cError  && !ActivarMensaje && !cAdvertencia  ) || (!ActivarMensaje && !ActivarAdvertencia && !ActivarError))
            {
                throw new Exception("Debe especificar el nivel de error");
            }

            int tipo = -10;
            string texto = "inicializar string";
            if (ActivarMensaje && cMensaje )
            {
                tipo = 1;
                texto = texto + DateTime.Now.ToShortDateString() + Mensaje;
                Console.ForegroundColor = ConsoleColor.White;
            }
            if (ActivarError && cError )
            {
                tipo = 2;
                texto = texto + DateTime.Now.ToShortDateString() + Mensaje;
                Console.ForegroundColor = ConsoleColor.Red;
            }
            if (ActivarAdvertencia && cAdvertencia)
            {
                tipo = 3;
                texto = texto + DateTime.Now.ToShortDateString() + Mensaje;
                Console.ForegroundColor = ConsoleColor.Yellow;
            }   


            RegistrarenArchivoTxt(texto);
            InsertarBasedeDatos(Mensaje, tipo);
            Console.WriteLine(DateTime.Now.ToShortDateString() + Mensaje);

        }

        
        public static void InsertarBasedeDatos(String Mensaje,int Tipo)
        {
            SqlConnection connection = new SqlConnection(ConfigurationManager.AppSettings["CadenaDeConexion"]);
            connection.Open();
            SqlCommand command = new SqlCommand("Insert into Logger Values('" + Mensaje + "', " + Tipo.ToString() + ")");
            command.ExecuteNonQuery();

        }

        public static void RegistrarenArchivoTxt(String Texto)
        {
            String Archivo = ConfigurationManager.AppSettings["CarpetaLogs"] + "DocLog" + DateTime.Now.ToShortDateString() + ".txt";
            if (!System.IO.File.Exists(Archivo))
            {
                Texto = System.IO.File.ReadAllText(Archivo);
            }
            System.IO.File.WriteAllText(Archivo, Texto) ;
        }



    }
}
