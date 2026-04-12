using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSC10
{
    public class ComboItem
    {
        public string Texto { get; set; }
        public string Valor { get; set; }

        public override string ToString() => Texto; // Lo que se muestra
    }
}
