using System;

namespace GravaBackUp
{
    class Program
    {
        static void Main(string[] args)
        {
            Tratamento_Informações TI = new Tratamento_Informações();
            BackUp BK = new BackUp();

            TI.UpdateStatus("1", "GravaBackUp", "");
            BK.GravaBackup();
            TI.UpdateStatus("0", "MONITOR", "");
        }
    }
}
