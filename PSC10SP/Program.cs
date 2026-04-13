using PSC10;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace PSC10SP
{
    internal static class Program
    {
      
        /// Punto de entrada principal para la aplicacion
        
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmStarts());
        }
    }
}
