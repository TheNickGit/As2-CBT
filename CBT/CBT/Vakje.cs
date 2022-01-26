using System;
using System.Collections.Generic;
using System.Text;



class Vakje
{
    public int waarde;
    public int[] domein;

    public Vakje(int waarde = 0, int[] domein = null)
    {
        this.waarde = waarde;

        if (domein == null)
            this.domein = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        else
            this.domein = domein;
    }
}

