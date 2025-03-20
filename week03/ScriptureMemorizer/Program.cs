using System;
using System.Collections.Generic;
using System.Linq;

namespace ScriptureMemorizer
{
   
    public class ScriptureReference
    {
        public string Book { get; private set; }
        public int Chapter { get; private set; }
        public int VerseStart { get; private set; }
        public int? VerseEnd { get; private set; }

       
        public ScriptureReference(string book, int chapter, int verseStart)
        {
            Book = book;
            Chapter = chapter;
            VerseStart = verseStart;
            VerseEnd = null;
        }

        
        public ScriptureReference(string book, int chapter, int verseStart, int verseEnd)
        {
            Book = book;
            Chapter = chapter;
            VerseStart = verseStart;
            VerseEnd = verseEnd;
        }

        
        public override string ToString()
        {
            return VerseEnd.HasValue
                ? $"{Book} {Chapter}:{VerseStart}-{VerseEnd}"
                : $"{Book} {Chapter}:{VerseStart}";
        }
    }

  
    public class ScriptureWord
    {
        public string Text { get; private set; }
        public bool IsHidden { get; private set; }

        public ScriptureWord(string text)
        {
            Text = text;
            IsHidden = false;
        }

        // Hides the word
        public void Hide()
        {
            IsHidden = true;
        }

      
        public override string ToString()
        {
            return IsHidden ? new string('_', Text.Length) : Text;
        }
    }

       public class Scripture
    {
        public ScriptureReference Reference { get; private set; }
        public List<ScriptureWord> Words { get; private set; }

        public Scripture(ScriptureReference reference, string text)
        {
            Reference = reference;
            Words = text.Split(' ').Select(word => new ScriptureWord(word)).ToList();
        }

        
        public void Display()
        {
            Console.Clear();
            Console.WriteLine(Reference);
            Console.WriteLine(string.Join(" ", Words));
        }

       
        public void HideRandomWords(int count = 1)
        {
            var visibleWords = Words.Where(word => !word.IsHidden).ToList();
            if (visibleWords.Count == 0) return;

            Random random = new Random();
            for (int i = 0; i < count && visibleWords.Count > 0; i++)
            {
                int index = random.Next(visibleWords.Count);
                visibleWords[index].Hide();
                visibleWords.RemoveAt(index); 
            }
        }

      
        public bool AllWordsHidden()
        {
            return Words.All(word => word.IsHidden);
        }
    }

    
    class Program
    {
        static void Main(string[] args)
        {
            
            List<Scripture> scriptures = new List<Scripture>
            {
                new Scripture(
                    new ScriptureReference("John", 3, 16),
                    "For God so loved the world that he gave his one and only Son, that whoever believes in him shall not perish but have eternal life."
                ),
                new Scripture(
                    new ScriptureReference("Psalm", 23, 1),
                    "The Lord is my shepherd; I shall not want."
                ),
                new Scripture(
                    new ScriptureReference("Philippians", 4, 13),
                    "I can do all things through Christ who strengthens me."
                )
            };

      
            Random random = new Random();
            Scripture scripture = scriptures[random.Next(scriptures.Count)];

           
            while (true)
            {
                scripture.Display();
                Console.WriteLine("\nPress Enter to hide more words or type 'quit' to exit.");
                string input = Console.ReadLine();

                if (input.ToLower() == "quit")
                {
                    break;
                }

                scripture.HideRandomWords(2); 
                
                if (scripture.AllWordsHidden())
                {
                    scripture.Display();
                    Console.WriteLine("\nAll words are hidden. Congratulations!");
                    break;
                }
            }
        }
    }
}