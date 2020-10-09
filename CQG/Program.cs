using System;
using System.Collections.Generic;
using System.Threading;

namespace CQG
{
    class Program
    {
        static List<string> Dict = new List<string>();
        static List<WordElement> Content = new List<WordElement>();

        static void Main(string[] args)
        {
            int count = 0;
            List<string> inputData = new List<string>();
            while (count != 2)
            {
                var str = Console.ReadLine();
                if (str == "===")
                {
                    count++;
                }
                inputData.Add(str);
            }
            SplitRows(inputData);
            foreach (var wordContent in Content)
            {
                FindWord(wordContent);
            }
        }

        static void SplitRows(List<string> str)
        {
            bool content = false; ;
            foreach (var i in str)
            {
                if (i == "===")
                {
                    content = true;
                    continue;
                }
                if (!content)
                {
                    foreach (var word in i.Split(' '))
                    {
                        Dict.Add(word);
                    }
                }
                else
                {
                    foreach (var word in i.Split(' '))
                    {
                        WordElement tmp = new WordElement(i);
                        Content.Add(tmp);
                    }
                }
            }
        }

        static void FindWord(WordElement word)
        {
            char[] errorW = word.value.ToCharArray();
            foreach (var DictWord in Dict)
            {
                if (Math.Abs(DictWord.Length - word.value.Length) < 2)
                {
                    if (DictWord.Length > word.value.Length)
                    { 

                    }
                }
            }
        }

        static void CompareWords(WordElement word) 
        { 
        
        }

    }
}
