using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZadanieRekrutacyjne
{
    public class Towar
    {
        public string Nazwa { get; set; }
        public decimal Cena { get; set; }
        public Opisy Opis { get; set; }
    }
    public class Opisy
    {
        public string A { get; set; }
        public string B { get; set; }
    }   
}
