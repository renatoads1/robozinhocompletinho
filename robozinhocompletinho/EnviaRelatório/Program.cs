using System;

namespace EnviaRelatório
{
    class Program
    {
        [Obsolete]
        static void Main(string[] args)
        {
            Tratamento_Informações TI = new Tratamento_Informações();


            TI.UpdateStatus("1", "EnviaRelatório", "");

            TI.GetLogDay();

            TI.UpdateStatus("0", "MONITOR", "");
        }
    }
}
