using System;
using System.Collections.Generic;
using System.Text;



class Vakje
{
    public int waarde;
    public List<int> domein;

    public Vakje(int waarde = 0, List<int> domein = null)
    {
        this.waarde = waarde;

        if (domein == null)
            this.domein = new List<int>();
        else
            this.domein = domein;
    }

    public Vakje KopieerVakje()
    {
        List<int> domeinKopie = new List<int>();
        foreach (int i in domein)
            domeinKopie.Add(i);

        return new Vakje(waarde, domeinKopie);
    }

    public void VerwijderUitDomein(int waarde)
    {
        domein.Remove(waarde);
    }

    public string PrintDomein()
    {
        string s = "{ ";
        foreach (int x in domein)
            s+= x + ", ";
        s += "}";
        return s;
    }
}

