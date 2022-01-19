using System;
using System.Collections.Generic;
using System.IO;

namespace As1_Sudoku
{
    class Puzzel
    {
        // De puzzel wordt bepaald door een 3D array van 9 bij 9
        public int[,] vakjes;
        public bool[,] fixeerdeVakjes;

        /// <summary>
        /// De Constructor van Puzzel.
        /// Maak een lege puzzel aan (gevuld met 0en) als er geen arrays gepasseerd worden.
        /// Wordt dit wel gedaan, dan wordt hiermee de puzzel aangemaakt.
        /// </summary>
        public Puzzel(int[,] inputVakjes = null, bool[,] inputFixeerdeVakjes = null)
        {
            if (inputVakjes == null)
                vakjes = new int[9, 9];
            else
                vakjes = inputVakjes;
            if (inputFixeerdeVakjes == null)
                fixeerdeVakjes = new bool[9, 9];
            else
                fixeerdeVakjes = inputFixeerdeVakjes;
        }

        /// <summary>
        /// Neemt een grid nummer en laadt de puzzel in die hierbij past.
        /// Het voorbeeldbestand heeft 5 puzzels, waardoor de grid nummers lopen van 1 t/m 5.
        /// </summary>
        public void LaadSudoku(int gridNummer)
        {
            StreamReader reader = new StreamReader("Suduko_puzzels_5.txt");

            // Gooi de eerste lijn weg. Verder, sla per gridnummer nog 2 lijnen over.
            reader.ReadLine();
            for (int i = 1; i < gridNummer; i++)
            {
                reader.ReadLine();
                reader.ReadLine();
            }

            // Lees de lijn en vertaal het naar een array van integers.
            string line = reader.ReadLine();
            line = line.Trim();
            string[] inputStr = line.Split(' ');
            int[] inputInt = new int[inputStr.Length];
            for (int i = 0; i < inputStr.Length; i++)
                inputInt[i] = int.Parse(inputStr[i]);

            // Vul de puzzel met de verkregen getallen.
            int r = 0; int k = 0;
            for (int i = 0; i < inputInt.Length; i++)
            {
                vakjes[r, k] = inputInt[i];
                k++;
                if (k == 9)
                {
                    r++;
                    k = 0;
                }
            }
        }

        /// <summary>
        /// Vul de puzzel met random getallen. Houdt nog geen rekening met correctheden.
        /// </summary>
        public void VulPuzzel()
        {
            for (int vakIndex = 0; vakIndex < 9; vakIndex++)
                VulVak(vakIndex);
        }

        /// <summary>
        /// De indexen van de blokken zijn als volgt:
        /// [0][1][2]
        /// [3][4][5]
        /// [6][7][8]
        /// waarbij iedere blok 3x3 kleinere vakjes bevat.
        /// </summary>
        internal void VulVak(int vakIndex)
        {
            int blokRijIndex = (vakIndex / 3) * 3;
            int blokKolomIndex = (vakIndex % 3) * 3;

            // Check al ingevulde nummers op duplicaten te voorkomen.
            List<int> beschikbareNummers = new List<int>();
            for (int i = 1; i <= 9; i++)
                beschikbareNummers.Add(i);
            for (int r = blokRijIndex; r < blokRijIndex + 3; r++)
                for (int k = blokKolomIndex; k < blokKolomIndex + 3; k++)
                {
                    int huidigNummer = vakjes[r, k];
                    if (huidigNummer != 0 && beschikbareNummers.Contains(huidigNummer))
                    {
                        beschikbareNummers.Remove(huidigNummer);
                        fixeerdeVakjes[r, k] = true;
                    }
                        
                }

            // Vul de overige vakjes in.
            Random rand = new Random();
            for (int r = blokRijIndex; r < blokRijIndex + 3; r++)
                for (int k = blokKolomIndex; k < blokKolomIndex + 3; k++)
                {
                    int huidigNummer = vakjes[r, k];
                    if (huidigNummer == 0)
                    {
                        int nieuwNummerIndex = rand.Next(0, beschikbareNummers.Count);
                        int nieuwNummer = beschikbareNummers[nieuwNummerIndex];
                        beschikbareNummers.RemoveAt(nieuwNummerIndex);
                        vakjes[r, k] = nieuwNummer;
                    }
                }
        }

        /// <summary>
        /// Print een weergave van de puzzel naar console.
        /// </summary>
        public void PrintPuzzel()
        {
            for (int r = 0; r < 9; r++)
            {
                for (int k = 0; k < 9; k++)
                {
                    Console.Write("[" + vakjes[r, k] + "]");
                    if (k == 2 || k == 5)
                        Console.Write(":");
                }
                Console.WriteLine();
                if (r == 2 || r == 5)
                    Console.WriteLine("---------+---------+---------");
            }
        }

        /// <summary>
        /// Kies een willekeurig blok en test alle mogelijke wissels.
        /// Kiest alleen toestanden uit met een verbetering verbetering (i.e., een lagere heuristische waarde).
        /// </summary>
        public List<Puzzel> GenereerToestanden()
        {
            Random rand = new Random();
            int vakIndex = rand.Next(0, 9);

            int blokRijIndex = (vakIndex / 3) * 3;
            int blokKolomIndex = (vakIndex % 3) * 3;

            List<Puzzel> toestanden = new List<Puzzel>();

            // Ga de nummers langs in het gekozen vak.
            for (int huidigVakjeIndex = 0; huidigVakjeIndex < 9; huidigVakjeIndex++)
            {
                int huidigeRijIndex = (huidigVakjeIndex / 3) + blokRijIndex;
                int huidigeKolomIndex = (huidigVakjeIndex % 3) + blokKolomIndex;

                // Als het huidige vakje gefixeerd is, sla hem over.
                if (fixeerdeVakjes[huidigeRijIndex, huidigeKolomIndex])
                    continue;

                // Pak alle toekomstige vakjes een voor een, check of het gefixeerde vakjes zijn,
                // en genereer een nieuwe toestand wanneer dit niet zo is.
                for (int wisselVakjeIndex = huidigVakjeIndex; wisselVakjeIndex < 9; wisselVakjeIndex++)
                {
                    int wisselRijIndex = (wisselVakjeIndex / 3) + blokRijIndex;
                    int wisselKolomIndex = (wisselVakjeIndex % 3) + blokKolomIndex;

                    if (fixeerdeVakjes[wisselRijIndex, wisselKolomIndex])
                        continue;

                    // Zijn de vakjes niet gefixeerd, genereer een nieuwe toestand.
                    int[,] nVakjes = KopieerToestand();
                    nVakjes[huidigeRijIndex, huidigeKolomIndex] = vakjes[wisselRijIndex, wisselKolomIndex];
                    nVakjes[wisselRijIndex, wisselKolomIndex] = vakjes[huidigeRijIndex, huidigeKolomIndex];
                    Puzzel toestand = new Puzzel(nVakjes, fixeerdeVakjes);

                    if (toestand.BerekenHeuristischeWaarde() <= BerekenHeuristischeWaarde())
                        toestanden.Add(toestand);
                }
            }

            // Kopieer de huidige toestand in een nieuwe 3D int array.
            int[,] KopieerToestand()
            {
                int[,] toestand = new int[9, 9];
                for (int r = 0; r < 9; r++)
                    for (int k = 0; k < 9; k++)
                        toestand[r, k] = vakjes[r, k];

                return toestand;
            }

            return toestanden;
        }

        /// <summary>
        /// Verwissel twee willekeurige niet-gefixeerde cijfers in hetzelfde vak, ongeacht van heuristische waarde.
        /// </summary>
        public void RandomWalk()
        {
            Random rand = new Random();
            int vakIndex = rand.Next(0, 9);

            // Genereer indexen voor de twee vakjes die verwisselt gaan worden.
            // De vakindex zorgt ervoor dat de twee vakjes onderdeel zijn van hetzelfde 3x3 vak.
            int rijIndex1 = (vakIndex / 3) * 3 + rand.Next(0, 3);
            int kolomIndex1 = (vakIndex % 3) * 3 + rand.Next(0, 3);
            int rijIndex2 = (vakIndex / 3) * 3 + rand.Next(0, 3);
            int kolomIndex2 = (vakIndex % 3) * 3 + rand.Next(0, 3);

            if (fixeerdeVakjes[rijIndex1, kolomIndex1] || fixeerdeVakjes[rijIndex2, kolomIndex2])
                RandomWalk();
            else
            {
                int temp = vakjes[rijIndex1, kolomIndex1];
                vakjes[rijIndex1, kolomIndex1] = vakjes[rijIndex2, kolomIndex2];
                vakjes[rijIndex2, kolomIndex2] = temp;
            }
        }

        /// <summary>
        /// Bereken the heuristische waarde van de huidige probleemtoestand.
        /// </summary>
        /// <returns></returns>
        public int BerekenHeuristischeWaarde()
        {
            int heuristischeWaarde = 0;
            for (int r = 0; r < 9; r++)
                heuristischeWaarde += CheckRij(r);
            for (int k = 0; k < 9; k++)
                heuristischeWaarde += CheckKolom(k);

            return heuristischeWaarde;
        }

        /// <summary>
        /// Check een rij met de heuristische functie ingebouwd.
        /// Returnt het aantal cijfers dat niet op de juiste plek staat.
        /// </summary>
        public int CheckRij(int rijIndex)
        {
            List<int> gevondenNummers = new List<int>();

            // De loop. Checkt een nummer om te kijken of deze al eerder is gevonden.
            // Zo nee, voeg het nummer toe aan de lijst.
            for (int i = 0; i < 9; i++)
            {
                int huidigNummer = vakjes[rijIndex, i];
                if (! gevondenNummers.Contains(huidigNummer))
                    gevondenNummers.Add(huidigNummer);
            }

            return 9 - gevondenNummers.Count;
        }

        /// <summary>
        /// Check een kolom met de heuristische functie ingebouwd.
        /// Returnt het aantal cijfers dat niet op de juiste plek staat.
        /// </summary>
        public int CheckKolom(int kolomIndex)
        {
            List<int> gevondenNummers = new List<int>();

            // De loop. Checkt een nummer om te kijken of deze al eerder is gevonden.
            // Zo nee, voeg het nummer toe aan de lijst.
            for (int i = 0; i < 9; i++)
            {
                int huidigNummer = vakjes[i, kolomIndex];
                if (!gevondenNummers.Contains(huidigNummer))
                    gevondenNummers.Add(huidigNummer);
            }

            return 9 - gevondenNummers.Count;
        }
    }

}
