using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication2
{
    class _8_17470_Klasse

    {
        public double kapitalNeu (int jahre, double kapital, double zinssatz)
        {
            if (jahre == 0) //wenn jahre = 0, dann Rückgabe des Kapitals
                return kapital;

            return kapitalNeu(jahre - 1, kapital * (zinssatz / 100) + kapital, zinssatz); // den Kapitalertrag berechnen, rekursive Abhängigkeit der Jahre          
        }            
    }
}
