namespace Nfse_Pbh
{
    class Program
    {

        static void Main(string[] args)
        {
            Realiza_Download RD = new Realiza_Download();
            Tratamento_Informações TI = new Tratamento_Informações();

            TI.UpdatelStatus("1", "NfsePbh", "");
            RD.Download();
            TI.UpdatelStatus("0", "MONITOR", "");

        }
    }
}
