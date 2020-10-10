using System;
using System.Collections.Generic;
using System.Text;

namespace CQG
{
    public enum TypeOfError
    {
        exactly,
        delete,
        insert,
        notFound,
        bothEdits,
        ambiguity
    }

    public class WordElement
    {
        public string value { get; set; }
        public TypeOfError type { get; set; }
        public int countOfMatches = 0;
        public List<string> AssumCorrectList = new List<string>();
        public List<string> CorrertWords = new List<string>();
        public WordElement(string _value)
        {
            value = _value;
            type = TypeOfError.notFound;
        }

    }
}
