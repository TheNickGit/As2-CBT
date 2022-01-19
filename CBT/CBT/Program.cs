using System;
using System.Collections.Generic;

namespace As1_Sudoku
{
    class Program
    {
        static void Main()
        {
            // Config:
            // Bepaalt het aantal iteraties dat het algoritme op dezelfde heuristische waarde
            // kan vastzitten voordat een aantal (niet gefixeerde) vakjes willekeurig gewisseld worden
            // ongeacht van verbetering. 12 en 2 zijn hier door testen als goede waardes uit gekomen.
            // 20 en 4 leveren vergelijkbare resultaten op, maar andere waarden dan dit zorgt voor aanzienlijke vertragingen.
            int maximaleStagnatie = 12;
            int aantalRandomVakjes = 2;

            // Geeft groep informatie weer.
            Console.WriteLine("Welkom bij de implementatie van Iterated Local Search van groep 47.");

            // Voer het programma uit dat de gebruiker in staat stelt een van de puzzels in te laden
            // en deze te laten oplossen met het ingebouwde ILS algoritme.
            SudokuProgramma();
            void SudokuProgramma() {
                // Vraag de gebruiker welke puzzel ingeladen dient te worden,
                // Maak deze puzzel aan en print hem in de console.
                Puzzel p = new Puzzel();
                int puzzelID = VraagPuzzelID();
                p.LaadSudoku(puzzelID);
                Console.WriteLine("Initiële puzzel:");
                p.PrintPuzzel();
                Console.WriteLine();

                // Vul de puzzel met willekeurige cijfers, zodanig dat ieder 3x3 blok de cijfers 1-9 eenmaal bevat.
                p.VulPuzzel();

                // De loop van het algoritme.
                Console.WriteLine("Het algoritme is bezig...\n");
                int iteratie = 0;
                int huidigeStagnatie = 0;
                while (p.BerekenHeuristischeWaarde() != 0)
                {
                    int oudeHeuristischeWaarde = p.BerekenHeuristischeWaarde();
                    p = HillClimbing(p);

                    /*
                    Console.WriteLine("Puzzel #" + iteratie + " - Heuristische waarde: " + p.BerekenHeuristischeWaarde());
                    p.PrintPuzzel();
                    */ // Verwijder comment om alle tussentijdse iteraties te zien.

                    // Als er geen verbetering is ontstaan na 1 iteratie van Hill Climbing, incrementeer de counter.
                    if (oudeHeuristischeWaarde == p.BerekenHeuristischeWaarde())
                        huidigeStagnatie++;

                    // Als te lang op dezelfde waarde wordt rondgedwaald, doe random walk een x aantal keer.
                    if (huidigeStagnatie == maximaleStagnatie)
                    {
                        for (int i = 0; i < aantalRandomVakjes; i++)
                            p.RandomWalk();
                        huidigeStagnatie = 0;
                    }
                    iteratie++;
                }

                // Oplossing gevonden! Geef alle informatie weer op het scherm.
                Console.WriteLine("Gevonden oplossing in " + iteratie + " iteraties:");
                p.PrintPuzzel();
                Console.WriteLine("Heuristische waarde check: " + p.BerekenHeuristischeWaarde()
                    + "\n\nEen nieuwe sudoku kan nu ingeladen worden.");

                // Voer het programma opnieuw uit.
                SudokuProgramma();
            }
        }

        /// <summary>
        /// Pas Hill Climbing toe:
        /// Genereer alle toestanden uit swaps in een 3x3 blok die een verbetering zijn, en pak de
        /// (eerste) beste toestand hieruit. Dit kan potentieel een verbetering zijn, maar ook een stagnatie.
        /// </summary>
        public static Puzzel HillClimbing(Puzzel p)
        {
            List<Puzzel> toestanden = p.GenereerToestanden();

            // Verkrijg de beste toestand.
            int minHeuristischeWaarde = p.BerekenHeuristischeWaarde();
            int minIndex = -1;
            for (int i = 0; i < toestanden.Count; i++)
            {
                // Verkrijg de index van de beste toestand.
                if (toestanden[i].BerekenHeuristischeWaarde() <= minHeuristischeWaarde)
                {
                    minHeuristischeWaarde = toestanden[i].BerekenHeuristischeWaarde();
                    minIndex = i;
                }
            }

            // Zet de huidige toestand naar de best gevonden toestand.
            if (minIndex != -1)
                p = toestanden[minIndex];

            return p;
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
                if(puzzelID >= 1 && puzzelID <=5)
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
}
