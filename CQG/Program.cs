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
                SelectioWord(wordContent);
            }
            foreach (var wordContent in Content)
            {
                if (wordContent.AssumCorrectList.Count == 1)
                {
                    Console.WriteLine(wordContent.AssumCorrectList[0]);
                }
                else
                {
                    string str = "{" + wordContent.AssumCorrectList[0] + " " + wordContent.AssumCorrectList[1] + "}";
                    Console.WriteLine(str);
                }
                if (wordContent.type == TypeOfError.notFound)
                {
                    string str = "{" + wordContent.value + "?" + "}";
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
            int len = 0;
            int difference = 0;
            // сравнение большего слова с меньшим (словарное больше)
          
            foreach (var L in errorW)
            {
                if (!DictW.Contains(L))
                {
                    difference++;
                    if (difference >= 2) return;
                }
            }
            foreach (var L in DictW)
            {
                if (!errorW.Contains(L))
                {
                    difference++;
                    if (difference > 2) return;
                }
            }

            word.AssumCorrectList.Add(Dict);
            if (DictW.Count() < errorW.Count())
            {
                word.type = TypeOfError.delete;
            }
            else if (DictW.Count() > errorW.Count())
            {
                word.type = TypeOfError.insert;
            }
            else 
            {
                word.type = TypeOfError.exactly;
            }
     
        }

        static void SelectioWord(WordElement word)
        {
            // если не найдено совпадений в словаре 
            if (word.AssumCorrectList.Count == 0 || word.AssumCorrectList == null) return;
            if (word.AssumCorrectList.Count > 0)
            {
                //если наше слово уже словарное , проверить на совпадения в найденых и просто завершиться 
                if (word.AssumCorrectList.Contains(word.value))
                {
                    return;
                }
                else
                {
                    //сравниваем с найденными словами и смотрим отличие от них 
                    for (var item = 0; item < word.AssumCorrectList.Count(); item++)
                    {
                        if (word.value.Length < word.AssumCorrectList[item].Length)
                        {
                            List<char> assumeWord = new List<char>(word.AssumCorrectList[item].ToCharArray());
                            List<char> wordValue = new List<char>(word.value.ToCharArray());
                            for (int letter = 0; letter < wordValue.Count(); letter++)
                            {
                                if (wordValue.Contains(assumeWord[letter]))
                                {
                                    wordValue.Remove(assumeWord[letter]);
                                    wordValue.Remove(assumeWord[letter]);
                                }
                            }
                            if (assumeWord.Count == 1)
                            {
                                word.CorrertWords.Add(word.AssumCorrectList[item]);
                            }
                            else
                            {
                                word.AssumCorrectList.Remove(word.AssumCorrectList[item]);
                                continue;
                            }

                        }
                        if (word.value.Length > word.AssumCorrectList[item].Length)
                        {
                            List<char> assumeWord = new List<char>(word.AssumCorrectList[item].ToCharArray());
                            List<char> wordValue = new List<char>(word.value.ToCharArray());
                            for (int letter = 0; letter < assumeWord.Count(); letter++)
                            {
                                if (wordValue.Contains(assumeWord[letter]))
                                {
                                    wordValue.Remove(assumeWord[letter]);
                                    wordValue.Remove(assumeWord[letter]);
                                }
                            }
                            if (wordValue.Count == 1)
                            {
                                word.CorrertWords.Add(word.AssumCorrectList[item]);
                                continue;
                            }
                            else
                            {
                                  word.AssumCorrectList.Remove(word.AssumCorrectList[item]);
                            }
                        }
                        if (word.value.Length == word.AssumCorrectList[item].Length)
                        {
                            List<char> assumeWord = new List<char>(word.AssumCorrectList[item].ToCharArray());
                            List<char> wordValue = new List<char>(word.value.ToCharArray());
                            for (int letter = assumeWord.Count()-1; letter >= 0; letter--)
                            {
                                if (wordValue.Contains(assumeWord[letter]))
                                {
                                    wordValue.Remove(assumeWord[letter]);
                                    assumeWord.Remove(assumeWord[letter]);
                                }
                            }
                            if (wordValue.Count == 0 || (word.AssumCorrectList.Count()==1 && wordValue.Count == 1))
                            {
                                word.CorrertWords.Add(word.AssumCorrectList[item]);
                                word.type = TypeOfError.bothEdits;
                            }
                            else
                            {
                                 word.AssumCorrectList.Remove(word.AssumCorrectList[item]);
                                continue;
                            }
                        }
                    }
                    if (word.CorrertWords.Count() > 1)
                    {
                        string finalString = "{";
                        foreach (var Word in word.CorrertWords)
                        {
                            finalString += (" " + word);
                        }
                    }
                    if (word.CorrertWords.Count() == 1)
                    {
                        word.value = word.CorrertWords[0];
                    }
                    if (word.CorrertWords.Count() == 0 || word.CorrertWords == null)
                    {
                        word.value = "{" + word.value + "?" +"}";
                    }
                }
            }
        }
    }
}
