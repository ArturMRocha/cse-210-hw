using System;
using System.Collections.Generic;
using System.IO;

class Program
{
    // Classe para representar uma entrada do diário
    public class JournalEntry
    {
        public string Prompt { get; set; }
        public string Response { get; set; }
        public string Date { get; set; }

        public JournalEntry(string prompt, string response)
        {
            Prompt = prompt;
            Response = response;
            Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public override string ToString()
        {
            return $"Data: {Date}\nPergunta: {Prompt}\nResposta: {Response}\n";
        }
    }

    static List<JournalEntry> journal = new List<JournalEntry>();
    static string[] prompts = 
    {
        "Quem foi a pessoa mais interessante com quem você interagiu hoje?",
        "Qual foi a melhor parte do seu dia?",
        "Como você viu a mão de Deus na sua vida hoje?",
        "Qual foi a emoção mais forte que você sentiu hoje?",
        "Se você pudesse refazer algo hoje, o que seria?"
    };

    static void Main(string[] args)
    {
        while (true)
        {
            Console.WriteLine("\nMenu do Diário:");
            Console.WriteLine("1. Escrever uma nova entrada");
            Console.WriteLine("2. Exibir o diário");
            Console.WriteLine("3. Salvar o diário em um arquivo");
            Console.WriteLine("4. Carregar o diário de um arquivo");
            Console.WriteLine("5. Sair");

            Console.Write("Escolha uma opção: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    WriteNewEntry();
                    break;

                case "2":
                    DisplayJournal();
                    break;

                case "3":
                    SaveJournal();
                    break;

                case "4":
                    LoadJournal();
                    break;

                case "5":
                    return;

                default:
                    Console.WriteLine("Opção inválida. Tente novamente.");
                    break;
            }
        }
    }

    static void WriteNewEntry()
    {
        Random random = new Random();
        string prompt = prompts[random.Next(prompts.Length)];
        Console.WriteLine($"Pergunta: {prompt}");
        Console.Write("Sua resposta: ");
        string response = Console.ReadLine();
        journal.Add(new JournalEntry(prompt, response));
    }

    static void DisplayJournal()
    {
        if (journal.Count == 0)
        {
            Console.WriteLine("O diário está vazio.");
        }
        else
        {
            foreach (var entry in journal)
            {
                Console.WriteLine(entry);
            }
        }
    }

    static void SaveJournal()
    {
        Console.Write("Digite o nome do arquivo para salvar: ");
        string filename = Console.ReadLine();

        using (StreamWriter writer = new StreamWriter(filename))
        {
            foreach (var entry in journal)
            {
                writer.WriteLine($"{entry.Date}|{entry.Prompt}|{entry.Response}");
            }
        }
        Console.WriteLine("Diário salvo com sucesso.");
    }

    static void LoadJournal()
    {
        Console.Write("Digite o nome do arquivo para carregar: ");
        string filename = Console.ReadLine();

        if (File.Exists(filename))
        {
            journal.Clear();
            using (StreamReader reader = new StreamReader(filename))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var parts = line.Split('|');
                    if (parts.Length == 3)
                    {
                        var entry = new JournalEntry(parts[1], parts[2]) { Date = parts[0] };
                        journal.Add(entry);
                    }
                }
            }
            Console.WriteLine("Diário carregado com sucesso.");
        }
        else
        {
            Console.WriteLine("Arquivo não encontrado.");
        }
    }
}
