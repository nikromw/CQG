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
        static List<int> format = new List<int>();
        static public List<Thread> threads = new List<Thread>();
        public delegate void WordsHandler(object obj);
        static Semaphore sem = new Semaphore(8, 8);
        static bool done = false;
        static void Main(string[] args)
        {
            int count = 0, formatting = 0;
            List<string> inputData = new List<string>();
            WordsHandler handler = new WordsHandler(FindWord);
            handler += SelectioWord;
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
            Format(inputData);
            if (Content.Count() != 0 && Content != null)
            {
                if (Dict.Count() > 200)
                {
                    foreach (var wordContent in Content)
                    {
                        Thread wordSearchThread = new Thread(new ParameterizedThreadStart(handler));
                        threads.Add(wordSearchThread);
                        wordSearchThread.Start(wordContent);
                    }
                Thread checkThread = new Thread(CheckThreads);
                }
                else
                {
                    done = true;
                    foreach (var wordContent in Content)
                    {
                        FindWord(wordContent);
                    }
                    foreach (var wordContent in Content)
                    {
                        SelectioWord(wordContent);
                    }
                }
                while (true)
                {
                    if (done)
                    {
                        foreach (var wordContent in Content)
                        {
                            Console.Write(wordContent.value + " ");
                            formatting++;
                            if (format.Contains(formatting)) Console.WriteLine();
                        }
                        return;
                    }
                }
            }
        }

        public static void CheckThreads()
        {
            while (true)
            {
                foreach (var thread in threads)
                {
                    if (!thread.IsAlive)
                    {
                        break;
                    }
                }
                done = true;
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
                        Dict.Add(word.ToLower());
                    }
                }
                else
                {
                    foreach (var word in i.Split(' '))
                    {
                        if (word != "")
                        {
                            WordElement tmp = new WordElement(word);
                            Content.Add(tmp);
                        }
                    }
                }
            }
        }
        //ищем совпадение по размеру слова +-1 буква , иначе не интересует
        public static void FindWord(object word)
        {
            sem.WaitOne();
            foreach (var DictWord in Dict)
            {
                if (Math.Abs(DictWord.Length - ((WordElement)word).value.Length) < 2)
                {
                    CompareWords((WordElement)word, DictWord);
                }
            }
            sem.Release();
        }
        //итерируемся побуквенно по словарному слову и слову с ошибкой 
        // меньшее по длинне слово точно содержиться в большем
        static void CompareWords(WordElement word, string Dict)
        {
            char[] errorW = word.value.ToCharArray();
            char[] DictW = Dict.ToCharArray();
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

        static void SelectioWord(object TmpWord)
        {
            WordElement word;
            word = (WordElement)TmpWord;
            // если не найдено совпадений в словаре 
            if (word.AssumCorrectList.Count == 0 || word.AssumCorrectList == null)
            {
                word.value = "{" + word.value + "?" + "}";
                return;
            }
            if (word.AssumCorrectList.Count > 0)
            {
                //если наше слово уже словарное , проверить на совпадения в найденых и просто завершиться 
                if (word.AssumCorrectList.Contains(word.value.ToLower()))
                {
                    return;
                }
                else
                {
                    //сравниваем с найденными словами и смотрим отличие от них 
                    for (var item = word.AssumCorrectList.Count() - 1; item >= 0; item--)
                    {
                        if (word.value.Length < word.AssumCorrectList[item].Length && word.type != TypeOfError.bothEdits)
                        {
                            List<char> assumeWord = new List<char>(word.AssumCorrectList[item].ToCharArray());
                            List<char> wordValue = new List<char>(word.value.ToCharArray());
                            for (int letter = assumeWord.Count() - 1; letter >= 0; letter--)
                            {
                                if (wordValue.Contains(assumeWord[letter]))
                                {
                                    wordValue.Remove(assumeWord[letter]);
                                    assumeWord.Remove(assumeWord[letter]);
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
                        if (word.value.Length > word.AssumCorrectList[item].Length && word.type != TypeOfError.bothEdits)
                        {
                            List<char> assumeWord = new List<char>(word.AssumCorrectList[item].ToCharArray());
                            List<char> wordValue = new List<char>(word.value.ToCharArray());
                            for (int letter = assumeWord.Count() - 1; letter >= 0; letter--)
                            {
                                if (wordValue.Contains(assumeWord[letter]))
                                {
                                    wordValue.Remove(assumeWord[letter]);
                                    assumeWord.Remove(assumeWord[letter]);
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
                            for (int letter = assumeWord.Count() - 1; letter >= 0; letter--)
                            {
                                if (wordValue.Contains(assumeWord[letter]))
                                {
                                    wordValue.Remove(assumeWord[letter]);
                                    assumeWord.Remove(assumeWord[letter]);
                                }
                            }
                            if (wordValue.Count == 0 || (word.AssumCorrectList.Count() == 1 && wordValue.Count == 1))
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
                        for (int i = 0; i < word.CorrertWords.Count(); i++)
                        {
                            if (i == word.CorrertWords.Count() - 1)
                            {
                                finalString += word.CorrertWords[i];
                            }
                            else
                            {
                                finalString += word.CorrertWords[i] + " ";
                            }
                        }
                        word.value = finalString + "}";
                    }
                    if (word.CorrertWords.Count() == 1)
                    {
                        word.value = word.CorrertWords[0];
                    }
                }
            }

        }

        static void Format(List<string> str)
        {
            bool content = false;
            int counter = 0;
            foreach (var i in str)
            {
                if (i == "===")
                {
                    content = true;
                    continue;
                }
                if (!content) continue;
                format.Add(i.Split(" ").Count() + counter);
                counter += i.Split(" ").Count();
            }
        }
    }
}
