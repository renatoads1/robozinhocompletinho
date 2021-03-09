using System;

namespace Importa_Questor
{
    class Program
    {
        static void Main(string[] args)
        {

            Tratamento_Informações TI = new Tratamento_Informações();
            Realiza_Importação RI = new Realiza_Importação();

            
            TI.UpdateStatus("1", "Importa_Questor", "");

            RI.Importacao();

            TI.UpdateStatus("0", "MONITOR", "");
        }
    }
}
