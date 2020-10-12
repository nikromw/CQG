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
        doubleInsert,
        doubleDelete,
        notFound,
        bothEdits,
        ambiguity
    }

    public class WordElement
    {
        public string value { get; set; }
        public TypeOfError type { get; set; }
        public List<string> AssumCorrectList = new List<string>();
        public List<string> CorrectWordsOneEdit = new List<string>();
        public List<string> CorrectWordsTwoEdit = new List<string>();
        public WordElement(string _value)
        {
            value = _value;
            type = TypeOfError.notFound;
        }

    }
}
