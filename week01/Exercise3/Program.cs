using System;

class Program
{
    static void Main(string[] args)
    {
    {
        string playAgain = "yes";

        while (playAgain.ToLower() == "yes")
        {
            // Gera um número aleatório de 1 a 100
            Random randomGenerator = new Random();
            int magicNumber = randomGenerator.Next(1, 101);

            int guess = -1;
            int attempts = 0;

            Console.WriteLine("Bem-vindo ao jogo 'Adivinhe o Meu Número'!");

            // Loop para verificar se o usuário acertou o número mágico
            while (guess != magicNumber)
            {
                Console.Write("Qual é o seu palpite? ");
                guess = int.Parse(Console.ReadLine());
                attempts++;

                if (guess < magicNumber)
                {
                    Console.WriteLine("Mais alto");
                }
                else if (guess > magicNumber)
                {
                    Console.WriteLine("Mais baixo");
                }
                else
                {
                    Console.WriteLine($"Você acertou! Foram necessárias {attempts} tentativas.");
                }
            }

            // Pergunta se o usuário quer jogar novamente
            Console.Write("Você quer jogar novamente (sim/nao)? ");
            playAgain = Console.ReadLine();
        }

        Console.WriteLine("Obrigado por jogar! Até a próxima.");
    }
}

    }
