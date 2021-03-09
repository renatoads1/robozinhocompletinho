using System;

namespace OrganizaXml
{
    class Program
    {
        static void Main(string[] args)
        {
            Tratamento_Informações TI = new Tratamento_Informações();
            Organiza Org = new Organiza();
            TI.UpdateStatus("1", "OrganizaXml", "");
            Org.VerificaItemServiço();
            TI.UpdateStatus("0", "MONITOR", "");

        }
    }
}
