using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
namespace TentaDatabas
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.Unicode; //Behöver ändra typsnitt i konsolenfönstret för att tyda ovanliga tecken.
            Console.InputEncoding = Encoding.Unicode;
            // Connectionstringen ligger högst upp i klassen SQLMethods
            MenuClass.MainMenu();
        }
        
    }
}
