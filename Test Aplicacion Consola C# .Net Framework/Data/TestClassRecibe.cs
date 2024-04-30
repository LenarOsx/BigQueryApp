using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Test_Aplicacion_Consola_C_.Net_Framework.Data.ExtensionMethod;

namespace Test_Aplicacion_Consola_C_.Net_Framework.Data
{
    public class TestClassRecibe
    {
        public int Id { get; set; }
        public int ValueXsp { get; set; }
        [DbReaderMap("This is a custom property")]
        public string Name { get; set; } = string.Empty;
        public List<int> ListInt { get; set; }
        public SubTestClassRecibe SubClass { get; set; }
    }
    public class TestClassOrigen
    {
        public int Id { get; set; }
        public int ValueXsp { get; set; }
       [DbReaderMap("This is a custom property")]
        public string Name { get; set; } = string.Empty;
        public List<int> ListInt { get; set; }
        public SubTestClassOrigen SubClass { get; set; }

    }

    public class SubTestClassRecibe
    {
        public  string SubName { get; set; }   
    }
    public class SubTestClassOrigen
    {
        public string SubName { get; set; }

    }

}
