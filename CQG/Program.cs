using System;
using System.Threading;

namespace CQG
{
    class Program
    {
        static void Main(string[] args)
        {
            int count = 0;
            while (count != 2)
            {
                string AllWords = Console.ReadLine();
                if (AllWords == "===")
                {
                    count++;
                }
            }
        }

        static void SplitRows(string str)
        {

        }
    }
}
