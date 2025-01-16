using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryAnalyser
{
    class Program
    {

        static void Main(string[] args)
        {
            ProjectDependencyAnalyzer.AnalyzeSolution("D:\\Personal\\WeatherWidget");
            Console.ReadKey();
        }

    }
}
