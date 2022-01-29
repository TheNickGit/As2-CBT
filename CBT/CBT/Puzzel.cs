using System;
using System.Collections.Generic;
using System.IO;

class Puzzel
{
    // De puzzel wordt bepaald door een 3D array van 9 bij 9.
    public Vakje[,] vakjes;

    // Deze variabel geeft de index weer van het huidige vakje (van 0 t/m 80).
    public int indexCounter = 0;

    /// <summary>
    /// De Constructor van Puzzel.
    /// </summary>
    public Puzzel(Vakje[,] vakjes = null, int indexCounter = 0)
    {
        if (vakjes == null)
            this.vakjes = new Vakje[9, 9];
        else
            this.vakjes = vakjes;

        this.indexCounter = indexCounter;
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
                Console.Write("[" + vakjes[r, k].waarde + "]");
                if (k == 2 || k == 5)
                    Console.Write(":");
            }
            Console.WriteLine();
            if (r == 2 || r == 5)
                Console.WriteLine("---------+---------+---------");
        }
    }

    /// <summary>
    /// Een loop van het chronologische backtracking algoritme.
    /// </summary>
    public void ChronologicalBackTracking()
    {
        // Als er al een oplossing gevonden is, return meteen
        if (Program.oplossingGevonden)
            return;

        // Er is een oplossing gevonden als het laatste vakje bereikt wordt en deze een domeingrootte van 1 heeft.
        if (indexCounter == 80 && vakjes[8, 8].domein.Count == 1)
        {
            vakjes[8, 8].waarde = vakjes[8, 8].domein[0];
            Program.oplossingGevonden = true;
            Console.WriteLine("Oplossing gevonden!");
            PrintPuzzel();
            return;
        }

        // Verkrijg het volgende lege vakje.
        int rijIndex = indexCounter / 9;
        int kolomIndex = indexCounter % 9;
        while (indexCounter < 81 && vakjes[rijIndex, kolomIndex].waarde != 0)
        {
            indexCounter++;
            rijIndex = indexCounter / 9;
            kolomIndex = indexCounter % 9;
        }

        // Als het laatste vakje bereikt wordt en deze al ingevuld is (waardoor indexCounter 81 bereikt wordt), is ook een oplossing gevonden.
        if (indexCounter >= 81)
        {
            Program.oplossingGevonden = true;
            Console.WriteLine("Oplossing gevonden!");
            PrintPuzzel();
            return;
        }
            
        // Voor het lege vakje dat hoort bij de verkregen indexen, ga het hele domein langs vanaf het begin.
        foreach (int waarde in vakjes[rijIndex, kolomIndex].domein)
        {
            Vakje[,] vakjesKopie = KopieerVakjes();
            Puzzel forwardPuzzel = new Puzzel(vakjesKopie, indexCounter);
            forwardPuzzel.vakjes[rijIndex, kolomIndex].waarde = waarde;
            forwardPuzzel.UpdateDomeinen(rijIndex, kolomIndex);

            // Pas forward checking toe.
            // Als de huidige toestand een partiële toestand is, ga verder met de nieuwe puzzel.
            if(forwardPuzzel.CheckDomeinen(rijIndex, kolomIndex))
            {
                forwardPuzzel.indexCounter++;
                forwardPuzzel.ChronologicalBackTracking();
            }
        }
    }

    /// <summary>
    /// Update de domeinen van ieder vakje zodat de puzzel knoopconsistent wordt.
    /// </summary>
    public void MaakKnoopConsistent()
    {
        for (int r = 0; r < 9; r++)
            for (int k = 0; k < 9; k++)
                if (vakjes[r, k].waarde == 0)
                    InitialiseerDomein(r, k);

        // Verkrijg het domein van een vakje en voeg deze toe.
        void InitialiseerDomein(int rijIndex, int kolomIndex)
        {
            List<int> mogelijkDomein = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            // Check de kolom waarin dit vak zich bevindt voor al ingevulde waarden.
            for (int r = 0; r < 9; r++)
                mogelijkDomein.Remove(vakjes[r, kolomIndex].waarde);

            // Doe hetzelfde voor de rij.
            for (int k = 0; k < 9; k++)
                mogelijkDomein.Remove(vakjes[rijIndex, k].waarde);

            // En voor het blok waarin het vakje zich bevindt.
            int blokRijIndex = rijIndex / 3;
            int blokKolomIndex = kolomIndex / 3;

            for (int r = blokRijIndex * 3; r < blokRijIndex * 3 + 3; r++)
                for (int k = blokKolomIndex * 3; k < blokKolomIndex * 3 + 3; k++)
                    mogelijkDomein.Remove(vakjes[r, k].waarde);

            // Vervang de (lege) domeinlijst met de zojuist verkregene.
            vakjes[rijIndex, kolomIndex].domein = mogelijkDomein;
        }
    }

    /// <summary>
    /// Neemt de rij en de kolom index van een vakje
    /// en update de domeinen van alle vakjes in dezelfde rij en kolom.
    /// </summary>
    public void UpdateDomeinen(int rijIndex, int kolomIndex)
    {
        Vakje huidigvakje = vakjes[rijIndex, kolomIndex];
        huidigvakje.domein.Clear();

        // Update de domeinen uit dezelfde kolom.
        for (int r = 0; r < 9; r++)
            vakjes[r, kolomIndex].domein.Remove(huidigvakje.waarde);

        // Update de domeinen uit dezelfde rij.
        for (int k = 0; k < 9; k++)
            vakjes[rijIndex, k].domein.Remove(huidigvakje.waarde);

        // Update de domeinen uit hetzelfde blok.
        int blokRijIndex = rijIndex / 3;
        int blokKolomIndex = kolomIndex / 3;
        for (int r = blokRijIndex * 3; r < blokRijIndex * 3 + 3; r++)
            for (int k = blokKolomIndex * 3; k < blokKolomIndex * 3 + 3; k++)
                vakjes[r, k].domein.Remove(huidigvakje.waarde);
    }

    /// <summary>
    /// Neemt de rij en kolom index van een vakje en bekijkt of de rij waarin
    /// deze voorkomt consistent is (i.e. geen lege domeinen in vakjes zonder waarde).
    /// true = alles in orde
    /// false = discard deze iteratie, want het is geen partiële oplossing.
    /// </summary>
    public bool CheckDomeinen(int rijIndex, int kolomIndex)
    {
        // Check de kolom op consistentie.
        for (int r = 0; r < 9; r++)
            if (vakjes[r, kolomIndex].domein.Count == 0 && vakjes[r, kolomIndex].waarde == 0)
                return false;
        
        // Check de rij op consistentie.
        for (int k = 0; k < 9; k++)
            if (vakjes[rijIndex, k].domein.Count == 0 && vakjes[rijIndex, k].waarde == 0)
                return false;

        // Check het blok op consistentie.
        int blokRijIndex = rijIndex / 3;
        int blokKolomIndex = kolomIndex / 3;
        for (int r = blokRijIndex * 3; r < blokRijIndex * 3 + 3; r++)
            for (int k = blokKolomIndex * 3; k < blokKolomIndex * 3 + 3; k++)
                if (vakjes[r, k].domein.Count == 0 && vakjes[r, k].waarde == 0)
                    return false;

        return true;
    }

    /// <summary>
    /// Maak een kopie van de vakjes array zodat het algoritme
    /// geen data aanpast die momenteel gebruikt wordt.
    /// </summary>
    private Vakje[,] KopieerVakjes()
    {
        Vakje[,] vakjesKopie = new Vakje[9, 9];
        for (int r = 0; r < 9; r++)
            for (int k = 0; k < 9; k++)
                vakjesKopie[r, k] = vakjes[r, k].KopieerVakje();

        return vakjesKopie;
    }
}

