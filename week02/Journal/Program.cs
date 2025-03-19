using System;
using System.Collections.Generic;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        Journal journal = new Journal();
        string[] prompts =
        {
            "Who was the most interesting person you interacted with today?",
            "What was the best part of your day?",
            "How did you see God's hand in your life today?",
            "What was the strongest emotion you felt today?",
            "If you could redo something today, what would it be?"
        };

        while (true)
        {
            Console.WriteLine("\nJournal Menu:");
            Console.WriteLine("1. Write a new entry");
            Console.WriteLine("2. Display the journal");
            Console.WriteLine("3. Save the journal to a file");
            Console.WriteLine("4. Load the journal from a file");
            Console.WriteLine("5. Delete an entry");
            Console.WriteLine("6. Edit an entry");
            Console.WriteLine("7. Exit");

            Console.Write("Choose an option: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    WriteNewEntry(journal, prompts);
                    break;

                case "2":
                    journal.DisplayEntries();
                    break;

                case "3":
                    SaveJournal(journal);
                    break;

                case "4":
                    LoadJournal(journal);
                    break;

                case "5":
                    DeleteEntry(journal);
                    break;

                case "6":
                    EditEntry(journal);
                    break;

                case "7":
                    return;

                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }

    static void WriteNewEntry(Journal journal, string[] prompts)
    {
        Random random = new Random();
        string prompt = prompts[random.Next(prompts.Length)];
        Console.WriteLine($"Prompt: {prompt}");
        Console.Write("Your response: ");
        string response = Console.ReadLine();
        journal.AddEntry(new JournalEntry(prompt, response));
    }

    static void SaveJournal(Journal journal)
    {
        Console.Write("Enter the filename to save: ");
        string filename = Console.ReadLine();
        journal.SaveToFile(filename);
    }

    static void LoadJournal(Journal journal)
    {
        Console.Write("Enter the filename to load: ");
        string filename = Console.ReadLine();
        journal.LoadFromFile(filename);
    }

    static void DeleteEntry(Journal journal)
    {
        Console.Write("Enter the index of the entry to delete: ");
        if (int.TryParse(Console.ReadLine(), out int index))
        {
            journal.DeleteEntry(index - 1); // Ajusta para índice base 0
        }
        else
        {
            Console.WriteLine("Invalid input. Please enter a valid number.");
        }
    }

    static void EditEntry(Journal journal)
    {
        Console.Write("Enter the index of the entry to edit: ");
        if (int.TryParse(Console.ReadLine(), out int index))
        {
            Console.Write("Enter the new response: ");
            string newResponse = Console.ReadLine();
            journal.EditEntry(index - 1, newResponse); // Ajusta para índice base 0
        }
        else
        {
            Console.WriteLine("Invalid input. Please enter a valid number.");
        }
    }
}

public class JournalEntry
{
    public string _textPrompt { get; set; }
    public string _textResponse { get; set; }
    public string _textDate { get; set; }

    public JournalEntry(string prompt, string response)
    {
        _textPrompt = prompt;
        _textResponse = response;
        _textDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }

    public override string ToString()
    {
        return $"Date: {_textDate}\nPrompt: {_textPrompt}\nResponse: {_textResponse}\n";
    }
}

public class Journal
{
    private List<JournalEntry> _entries = new List<JournalEntry>();

    public void AddEntry(JournalEntry entry)
    {
        _entries.Add(entry);
    }

    public void DisplayEntries()
    {
        if (_entries.Count == 0)
        {
            Console.WriteLine("The journal is empty.");
        }
        else
        {
            for (int i = 0; i < _entries.Count; i++)
            {
                Console.WriteLine($"Entry #{i + 1}:\n{_entries[i]}");
            }
        }
    }

    public void SaveToFile(string filename)
    {
        using (StreamWriter writer = new StreamWriter(filename))
        {
            foreach (var entry in _entries)
            {
                writer.WriteLine($"{entry._textDate}|{entry._textPrompt}|{entry._textResponse}");
            }
        }
        Console.WriteLine("Journal saved successfully.");
    }

    public void LoadFromFile(string filename)
    {
        if (File.Exists(filename))
        {
            _entries.Clear();
            using (StreamReader reader = new StreamReader(filename))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var parts = line.Split('|');
                    if (parts.Length == 3)
                    {
                        var entry = new JournalEntry(parts[1], parts[2]) { _textDate = parts[0] };
                        _entries.Add(entry);
                    }
                }
            }
            Console.WriteLine("Journal loaded successfully.");
        }
        else
        {
            Console.WriteLine("File not found.");
        }
    }

    public void DeleteEntry(int index)
    {
        if (index >= 0 && index < _entries.Count)
        {
            _entries.RemoveAt(index);
            Console.WriteLine("Entry deleted successfully.");
        }
        else
        {
            Console.WriteLine("Invalid entry index.");
        }
    }

    public void EditEntry(int index, string newResponse)
    {
        if (index >= 0 && index < _entries.Count)
        {
            _entries[index]._textResponse = newResponse;
            _entries[index]._textDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            Console.WriteLine("Entry edited successfully.");
        }
        else
        {
            Console.WriteLine("Invalid entry index.");
        }
    }
}
