using System;
using System.Collections.Generic;
using System.IO;

    class Puzzel
    {
        // De puzzel wordt bepaald door een 3D array van 9 bij 9
        public Vakje[,] vakjes;
        //public bool[,] fixeerdeVakjes;

        /// <summary>
        /// De Constructor van Puzzel.
        /// </summary>
        public Puzzel()
        {
            vakjes = new Vakje[9, 9];
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
                vakjes[r, k] = new Vakje(inputInt[i]);
                k++;
                if (k == 9)
                {
                    r++;
                    k = 0;
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

    }

