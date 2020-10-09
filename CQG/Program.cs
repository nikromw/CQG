using System;
using System.Collections.Generic;
using System.Linq;
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
            foreach (var wordContent in Content)
            {
                if (wordContent.CorrectWordList.Count == 1)
                {
                    Console.WriteLine(wordContent.CorrectWordList[0]);
                }
                else 
                {
                    string str = "{" + wordContent.CorrectWordList[0] + " " + wordContent.CorrectWordList[1] + "}";
                    Console.WriteLine(str);
                }
                if (wordContent.type == TypeOfError.notFound)
                {
                    string str = "{" + wordContent.value + "?" +"}";
                }
            }
        }

        //делим строки на словарь и контекст 
        //создаем листы со словами из словаря и неверных слов
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
                        WordElement tmp = new WordElement(word);
                        Content.Add(tmp);
                    }
                }
            }
        }
        //ищем совпадение по размеру слова +-1 буква , иначе не интересует
        static void FindWord(WordElement word)
        {
            foreach (var DictWord in Dict)
            {
                if (Math.Abs(DictWord.Length - word.value.Length) < 2)
                {
                    CompareWords(word, DictWord);
                }
            }
        }
        //итерируемся побуквенно по словарному слову и слову с ошибкой 
        // меньшее по длинне слово точно содержиться в большем
        static void CompareWords(WordElement word, string Dict)
        {
            char[] errorW = word.value.ToCharArray();
            char[] DictW = Dict.ToCharArray();
            int difference = 0;
            // сравнение большего слова с меньшим (словарное больше)
            if (Dict.Length >= word.value.Length)
            {
                foreach (var L in errorW)
                {
                    if (!DictW.Contains(L))
                    {
                        difference++;
                        if (difference >= 2) return;
                    }
                }
                //добавление словарного слова , на которое заменим позже
                word.CorrectWordList.Add(Dict);
                word.type = TypeOfError.insert;// словарное слово больше , значит вставка буквы
            }
            else
            {
                // сравнение большего слова с меньшим (словарное меньше)
                foreach (var L in DictW)
                {
                    if (!errorW.Contains(L))
                    {
                        difference++;
                        if (difference >= 2) return;
                    }
                }
                //добавление словарного слова , на которое заменим позже
                word.CorrectWordList.Add(Dict);
                word.type = TypeOfError.delete; // словарное слово меньше , значит удаление 
            }
        }

    }
}
