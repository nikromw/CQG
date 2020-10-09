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
        ambiguity
    }

    public class WordElement
    {
        public string value { get; set; }
        public TypeOfError type { get; set; }
        public int countOfMatches = 0;
        public WordElement(string _value)
        {
            value = _value;
        }

    }
}
