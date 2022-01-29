using System;
using System.Collections.Generic;
using System.Text;

class Vakje
{
    // Een vakje bevat een waarde (0 als er nog geen waarde is) en een lijst van de domeinvariabelen.
    public int waarde;
    public List<int> domein;

    /// <summary>
    /// De constructor van Vakje.
    /// </summary>
    public Vakje(int waarde = 0, List<int> domein = null)
    {
        this.waarde = waarde;

        if (domein == null)
            this.domein = new List<int>();
        else
            this.domein = domein;
    }

    /// <summary>
    /// Maak een kopie van het vakje, zodat naar een ander punt in het geheugen verwezen wordt.
    /// </summary>
    public Vakje KopieerVakje()
    {
        List<int> domeinKopie = new List<int>();
        foreach (int i in domein)
            domeinKopie.Add(i);

        return new Vakje(waarde, domeinKopie);
    }
}
