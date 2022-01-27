using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        // Geeft groep informatie weer.
        Console.WriteLine("Welkom bij de implementatie van Chronological Backtracking van groep 47.");

        // Voer het programma uit dat de gebruiker in staat stelt een van de puzzels in te laden
        // en deze te laten oplossen met het ingebouwde CBT algoritme.
        SudokuProgramma();

        void SudokuProgramma()
        {
            // Vraag de gebruiker welke puzzel ingeladen dient te worden,
            // Maak deze puzzel aan en print hem in de console.
            Puzzel p = new Puzzel();
            int puzzelID = VraagPuzzelID();
            p.LaadSudoku(puzzelID);
            Console.WriteLine("Initiële puzzel:");
            p.PrintPuzzel();
            Console.WriteLine();

            // De loop van het algoritme.
            Console.WriteLine("Het algoritme is bezig...\n");
            p.MaakKnoopConsistent();
            p.ChronologicalBackTracking();

            // Voer het programma opnieuw uit.
            Console.WriteLine("\n\nEen nieuwe sudoku kan nu ingeladen worden.");
            SudokuProgramma();
        }
    }

    /// <summary>
    /// Vraag de gebruiker om het ID (index) van de puzzel.
    /// Vraag opnieuw als een verkeerd cijfer of andere input gegeven wordt.
    /// </summary>
    protected static int VraagPuzzelID()
    {
        Console.WriteLine("Kies een getal tussen de 1 en 5 om een sudoku puzzel in te laden.");
        string input = Console.ReadLine();
        try
        {
            int puzzelID = int.Parse(input);
            if (puzzelID >= 1 && puzzelID <= 5)
                return puzzelID;
            else
            {
                Console.WriteLine("Verkeerde input.");
                return VraagPuzzelID();
            }
        }
        catch
        {
            Console.WriteLine("Verkeerde input.");
            return VraagPuzzelID();
        }
    }
}
