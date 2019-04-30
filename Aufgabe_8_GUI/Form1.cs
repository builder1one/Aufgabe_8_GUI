using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// Using Anweisung für verwendete Klasse
using WindowsFormsApplication2;

// Hinzufügen von System.Threading, um Multithreading zu ermöglichen. Dies verhindert Unterbrechungen der Bedienbarkeit des Programmes, bei komplexen Rechnungen.
using System.Threading;

/// <summary>
/// Programm um den Kapitalanstieg über n Jahre zu berechnen, bei gegebenen Zinssatz und gegebenen Startkapital.
/// </summary>
namespace Aufgabe_8_GUI
{

    public partial class Form1 : Form
    {

        // Erstellen eines neuen Objekts der Klasse "_8_17470_Klasse"
        _8_17470_Klasse kapitalertrag = new _8_17470_Klasse();

        public Form1()
        {
            InitializeComponent();

            // Setzen der Einstellungen für das Fenster (Entfernen der Buttons für minimieren und maximieren, sowie Größe unveränderbar setzen)
            this.FormBorderStyle = FormBorderStyle.Fixed3D;
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.Size = new Size(700, 380);

            // Graph füllen mit Vorgabewerten, damit der Graph beim Start nicht leer ist.
            fillChart();

            Thread kapital = new Thread(new ThreadStart(this.KapitalBerechnen));
            kapital.Start();
        }

        /// <summary>
        /// Ausfüllen des Graphen mit fest definierten Anfangswerten.
        /// </summary>
        private void fillChart()
        {
            // Hinzufügen, von X und Y Werten um den Graph beim Start zu füllen.
            chart1.Series["Kapital"].Points.AddXY("0", "400");
            chart1.Series["Kapital"].Points.AddXY("1", "10000");
            chart1.Series["Kapital"].Points.AddXY("2", "8000");
            chart1.Series["Kapital"].Points.AddXY("3", "7000");
            chart1.Series["Kapital"].Points.AddXY("4", "10000");
            chart1.Series["Kapital"].Points.AddXY("5", "8500");

            // Ändern des Titel des chart1.
            chart1.Titles.Add("Kapitalertrag");

            // Hinzufügen der Achsenbezeichnungen 
            chart1.ChartAreas["ChartArea1"].AxisX.Title = "Jahre n";
            chart1.ChartAreas["ChartArea1"].AxisY.Title = "Kapital in Euro";

            // Entfernen des Abstands der X-Achse von der Y-Achse
            chart1.ChartAreas["ChartArea1"].AxisX.IsMarginVisible = false;

            // Item in Listbox auswählen damit Listbox Item nicht NULL.
            comboBox1.SelectedIndex = 0;

            // Beim Start sichtbare Textboxen und Labels anpassen.
            TextBoxSichtbarkeit();
        }

        /// <summary>
        /// Event beim druecken des Buttons "button1". Berechnung des Kapitalertrags und hinzufügen in Diagramm sowie Ausgabe des genauen Endwerts, der Rechnung.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            this.GraphErstellen();
        }

        /// <summary>
        /// Prüft ob Pfeiltaste gedrückt wurde, während die Textbox aktiv ist und ändern der aktiven Textbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            // Eingabe unterdrücken, wenn keine Zahl / Back Taste (Komma und Punkt erlaubt)
            if (!Char.IsNumber((char)e.KeyCode) & e.KeyCode != Keys.Back & e.KeyCode != Keys.Left & e.KeyCode != Keys.Right)
                e.SuppressKeyPress = true;

            if ((e.KeyCode == Keys.Enter || e.KeyCode == Keys.Down) && textBox2.Visible)
            {
                textBox2.Focus();
                textBox2.SelectAll();
            } else if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Down)
            {
                textBox3.Focus();
                textBox3.SelectAll();
            }
        }

        /// <summary>
        /// Prüft ob Pfeiltaste gedrückt wurde, während die Textbox aktiv ist und ändern der aktiven Textbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            // Eingabe unterdrücken, wenn keine Zahl / Back Taste (Komma und Punkt erlaubt)
            if (!Char.IsNumber((char)e.KeyCode) & e.KeyCode != Keys.Back & e.KeyCode != Keys.Left & e.KeyCode != Keys.Right)
                e.SuppressKeyPress = true;

            if ((e.KeyCode == Keys.Enter || e.KeyCode == Keys.Down) && textBox3.Visible)
            {
                textBox3.Focus();
                textBox3.SelectAll();
            }
            else if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Down)
            {
                textBox4.Focus();
                textBox4.SelectAll();
            }
            else if (e.KeyCode == Keys.Up && textBox1.Visible)
            {
                textBox1.Focus();
                textBox1.SelectAll();
            }
        }

        /// <summary>
        /// Prüft ob Pfeiltaste gedrückt wurde, während die Textbox aktiv ist und ändern der aktiven Textbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox3_KeyDown(object sender, KeyEventArgs e)
        {
            // Eingabe unterdrücken, wenn keine Zahl / Back Taste (Komma und Punkt erlaubt)
            if (!Char.IsNumber((char)e.KeyCode) & e.KeyCode != Keys.Back & e.KeyCode != Keys.Left & e.KeyCode != Keys.Right)
                e.SuppressKeyPress = true;

            if ((e.KeyCode == Keys.Enter || e.KeyCode == Keys.Down) && textBox4.Visible)
            {
                textBox4.Focus();
                textBox4.SelectAll();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                this.GraphErstellen();
            }
            else if (e.KeyCode == Keys.Up && textBox2.Visible)
            {
                textBox2.Focus();
                textBox2.SelectAll();
            }
            else if (e.KeyCode == Keys.Up)
            {
                textBox1.Focus();
                textBox1.SelectAll();
            }
        }

        /// <summary>
        /// Prüft ob Pfeiltaste gedrückt wurde, während die Textbox aktiv ist und ändern der aktiven Textbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox4_KeyDown(object sender, KeyEventArgs e)
        {
            // Eingabe unterdrücken, wenn keine Zahl / Back Taste (Komma und Punkt erlaubt)
            if(!Char.IsNumber((char)e.KeyCode) & e.KeyCode != Keys.Back & e.KeyCode != Keys.Left & e.KeyCode != Keys.Right)
                e.SuppressKeyPress = true;

            if (e.KeyCode == Keys.Enter)
            {
                this.GraphErstellen();
            }
            else if (e.KeyCode == Keys.Up && textBox3.Visible)
            {
                textBox3.Focus();
                textBox3.SelectAll();
            }
            else if (e.KeyCode == Keys.Up)
            {
                textBox2.Focus();
                textBox2.SelectAll();
            }
        }

        /// <summary>
        /// Erstellen des Graphen zur Dartellung des Ergebnisses sowie Ausgabe des genauen Endwwerts als Label.
        /// </summary>
        private void GraphErstellen()
        {

            // Löschen des Graphen vor der Berechnung. (Nötig, wenn neue Rechnung weniger Jahre als zuletzt ausgeführte.)
            GraphLoeschen();

            try
            {
                // Auswählen welche Berechnung ausgeführt wird, je nachdem was in der ComboBox ausgewählt ist.
                switch (comboBox1.SelectedItem.ToString())
                {
                    case ("Kapital berechnen"):
                        KapitalBerechnen();
                        break;
                    case ("Jahre berechnen"):
                        JahreBerechnen();
                        break;
                    case ("Zins berechnen"):
                        ZinsBerechnen();
                        break;
                    case ("Startkapital berechnen"):
                        StartKapitalBerechnen();
                        break;
                }
            } catch (NullReferenceException)
            {
                // Fehler, wenn Nutzer nicht vorhandenes Item in der Listbox auswählt.
                label4.Text = "Nicht vorhandene Berechnung";
            }
        }

        /// <summary>
        /// Ändern der Farbe, wenn sich der Inhalt der Textbox ändert.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            // Entfernt die Textfarbe, wenn eine Eingabe erfolgt ist.
            if(textBox1.Text != "")
                textBox1.BackColor = Color.Empty;
        }     

        /// <summary>
        /// Ändern der Farbe, wenn sich der Inhalt der Textbox ändert.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            try
            {
                // Entfernt die Textfarbe, wenn eine Eingabe erfolgt ist.
                if (textBox2.Text != "")
                    textBox2.BackColor = Color.Empty;

                textBox2.ForeColor = getTextBoxColor(double.Parse(textBox2.Text));
            } catch (Exception)
            {

            }     
        }

        /// <summary>
        /// Ändern der Farbe, wenn sich der Inhalt der Textbox ändert.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            try
            {
                // Entfernt die Textfarbe, wenn eine Eingabe erfolgt ist.
                if (textBox3.Text != "")
                    textBox3.BackColor = Color.Empty;

                textBox3.ForeColor = getTextBoxColor(double.Parse(textBox3.Text));
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// Ändern der Farbe, wenn sich der Inhalt der Textbox ändert.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            try
            {
                // Entfernt die Textfarbe, wenn eine Eingabe erfolgt ist.
                if (textBox4.Text != "")
                    textBox4.BackColor = Color.Empty;

                textBox4.ForeColor = getTextBoxColor(double.Parse(textBox4.Text));
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// Prüft ob Zahl in Textbox negativ oder positiv und ändern der Farbe der Textbox je nach Inhalt. 
        /// </summary>
        /// <param name="textBoxValue"></param>
        /// <returns></returns>
        private Color getTextBoxColor(double textBoxValue)
        {
            try
            {
                // Prüfen ob eingegebene Zahl positiv oder negativ ist.
                if (textBoxValue < 0)
                {
                    // Wenn die Zahl negativ ist darstellung in Rot.
                    return Color.Red;
                }
                else
                {
                    // Wenn die Zahl positiv ist darstellung in Grün.
                    return Color.Green;
                }
            }
            catch
            {
                // Wenn ein Fehler Feld färben in System Farben.
                return SystemColors.ControlText;
            }
        }

        /// <summary>
        /// Löscht den Graphen indem alle Punkte der Sammlung "Kapital" gelöscht werden.
        /// </summary>
        public void GraphLoeschen()
        {
            // Löschen des Graphen um die neuen Werte zum Graph hinzufügen zu können.
            chart1.Series["Kapital"].Points.Clear();
        }

        /// <summary>
        /// Berechnet wie viele Jahre nötig sind, um den gewünschten Kontostand zu erreichen.
        /// </summary>
        public void JahreBerechnen()
        {
            // Ruft die Methode zum ändern der Textbox Farbe auf.
            TextboxFarbe();

            try
            {
                // Variablen für die Berechnung.
                double kapital = Convert.ToDouble(textBox2.Text);
                double ziel = Convert.ToDouble(textBox4.Text);
                double zinssatz = Convert.ToDouble(textBox3.Text);

                // Abfangen von Fehlern, die zu einer "unendlichen" Rechnung führen würden.
                // Abfangen wenn das Kapital bereits größer als der Zielwert.
                if (kapital > ziel)
                {
                    label4.Text = "Das Kapital ist größer als der Zielwert!";
                    return;
                }

                // Abfangen negativer Zinssatz.
                if (zinssatz <= 0)
                {
                    label4.Text = "Nicht mathematisch Möglich";
                    return;
                }
                    

                chart1.Series["Kapital"].Points.AddXY(0, textBox2.Text);

                // Zählervariable i für die Anzahl an Jahren, die benötigt werden.
                int i = 0;

                // Solange das Kapital kleiner als das Ziel ist erhöhen der Jahre um den Zielwert zu erreichen.
                while (kapital < ziel)
                {
                    i++;
                    kapital = kapitalertrag.kapitalNeu(i, Convert.ToDouble(textBox2.Text), zinssatz);
                    chart1.Series["Kapital"].Points.AddXY(i, kapital);
                }
                if (kapital > ziel)
                    label4.Text = String.Format("Es werden {0} {1} benötigt, bis das gewünschte Kapital erreicht wurde." + Environment.NewLine +
                        "Es sind noch {1} Euro über.", i, (kapital - ziel).ToString("0.00"), label1.Text.Substring(0, label1.Text.Length - 1));
                else
                    label4.Text = String.Format("Es werden {0} {1} benötigt, bis das gewünschte Kapital erreicht wurde.", i, label1.Text.Substring(0, label1.Text.Length-1));
            }
            catch (Exception)
            {
                // Ausgabe aeiner Fehlermeldung in Label4
                label4.Text = "Alle Felder müssen richtig ausgefüllt sein!";
            }
        }

        /// <summary>
        /// Berechnet das Kapital, bei gegebenem Zinssatz, Startkapital und einer bestimmten Zahl an Jahren.
        /// </summary>
        public void KapitalBerechnen()
        {
            // Ruft die Methode zum ändern der Textbox Farbe auf.
            TextboxFarbe();

            double kapital = 0;

            // Try um leere Felder bzw. falsche Eingaben abzufangen.
            try
            {
                chart1.Series["Kapital"].Points.AddXY(0, textBox2.Text);
                //chart1.Series["Kapital"].Points.AddXY(0, textBox2.Text);
                for (int i = 1; i <= Convert.ToInt32(textBox1.Text); i++)
                {
                    kapital = kapitalertrag.kapitalNeu(i, (Convert.ToDouble(textBox2.Text)+(Convert.ToDouble(textBox4.Text)*i)), Convert.ToDouble(textBox3.Text));
                    chart1.Series["Kapital"].Points.AddXY(i, kapital);
                }

                // Ausgabe des genauen Werts für den Entkontostand gerundet auf Cent und Ausgabe des Wachstums Faktors gerundet auf 3 Nachkommastellen.
                double zuwachs = kapital / Convert.ToDouble(textBox2.Text);
                // Prüfen ob Kapital gestiegen oder gesunken bzw. gleich.
                if (zuwachs >= 1)
                    label4.Text = String.Format("Der Kontostand am Ende beträgt {0} Euro." + Environment.NewLine +
                        "Das Kapital ist um den Faktor {1} gestiegen.", kapital.ToString("0.00"), Math.Round(zuwachs, 3));
                else
                    label4.Text = String.Format("Der Kontostand am Ende beträgt {0} Euro. " + Environment.NewLine +
                        "Das Kapital ist um den Faktor {1} gesunken.", kapital.ToString("0.00"), Math.Round(zuwachs, 3));
            }
            catch (Exception) // Wird ausgeführt, wenn Fehler aufgetreten ist.
            {
                // Ausgabe aeiner Fehlermeldung in Label4
                label4.Text = "Alle Felder müssen richtig ausgefüllt sein!";
            }
        }

        /// <summary>
        /// Berechnet den benötigten Zinssatz, bei gegebenem Startwert, Zielwert und Anzahl an Jahren.
        /// </summary>
        public void ZinsBerechnen()
        {
            // Ruft die Methode zum ändern der Textbox Farbe auf.
            TextboxFarbe();

            double kapital = 0;

            // Variable für die gewünschte Rechengenauigkeit. (Zinssatz für die Berechnung)
            double genauigkeit = 0.01 / trackBar1.Value;

            // Try um leere Felder bzw. falsche Eingaben abzufangen.
            try
            {
                kapital = Convert.ToDouble(textBox2.Text);
                double ziel = Convert.ToDouble(textBox4.Text);

                // Verwendeter Zinssatz entspricht der Genauigkeit.
                double zinssatz = genauigkeit;

                // Iteratives berechnen des Zinssatzes mit einer genauigkeit von 0.001%. Durch erhöhen des Zinssatzes, bis dieser ausreichend.
                while (kapital <= ziel)
                {
                    // Kapitalberechnen für geschätzten Zinssatz.
                    kapital = kapitalertrag.kapitalNeu(Convert.ToInt32(textBox1.Text), Convert.ToDouble(textBox2.Text), zinssatz);
                    zinssatz = zinssatz + 0.001;
                }

                // Erstellen des Graphen für den berechneten Zinssatz 
                chart1.Series["Kapital"].Points.AddXY(0, textBox2.Text);

                for (int i = 1; i <= Convert.ToInt32(textBox1.Text); i++)
                {
                    kapital = kapitalertrag.kapitalNeu(i, Convert.ToDouble(textBox2.Text),zinssatz);
                    chart1.Series["Kapital"].Points.AddXY(i, kapital);
                }

                // Ausgabe des Zinssatzes, welcher benötigt wird und der genutzten Genauigkeit.
                label4.Text = String.Format("Der benötige Zinssatz beträgt {0} Prozent." + Environment.NewLine +
                    "(Genauigkeit {1}) pro {2}.", Math.Round(zinssatz,6), Math.Round(genauigkeit,5), label1.Text.Substring(0, label1.Text.Length - 2));
            }
            catch (Exception)
            {
                // Ausgabe aeiner Fehlermeldung in Label4
                label4.Text = "Alle Felder müssen richtig ausgefüllt sein!";
            }
        }

        /// <summary>
        /// Berechnet das Kapital das benötigt wird, um den Zielwert bei gegebner Zeit und gegebenem Zins zu erreichen.
        /// </summary>
        public void StartKapitalBerechnen()
        {
            // Ruft die Methode zum ändern der Textbox Farbe auf.
            TextboxFarbe();

            double kapital = 0;         

            // Try um leere Felder bzw. falsche Eingaben abzufangen.
            try
            {
                // Berechnung des Zins Faktors für die gegebene Zeit, den Startwert und des Zinssatzes, mit dem Startwert 1.
                for (int i = 1; i <= Convert.ToInt32(textBox1.Text); i++)
                {
                    kapital = kapitalertrag.kapitalNeu(i, 1, Convert.ToDouble(textBox3.Text));
                }

                // Berechnung des Startkapitals mit dem Wachstumsfaktor und dem gewünschten Ziel Kapital.
                double startkapital =  Convert.ToDouble(textBox4.Text)/kapital;

                // Erneute Berechnung um den korrekten Graphen mit dem zuvor berechneten Startwert anzuzeigen.
                for (int i = 1; i <= Convert.ToInt32(textBox1.Text); i++)
                {
                    kapital = kapitalertrag.kapitalNeu(i, startkapital, Convert.ToDouble(textBox3.Text));
                    chart1.Series["Kapital"].Points.AddXY(i, kapital);
                }

                // Ausgabe des Startkapitals das benötigt wird.
                label4.Text = String.Format("Das benötigte Startkapital beträgt {0} Euro.",Math.Round(startkapital,2));
            } catch (Exception)
            {
                // Ausgabe aeiner Fehlermeldung in Label4
                label4.Text = "Alle Felder müssen richtig ausgefüllt sein!";
            }
        }

        /// <summary>
        /// Ruft die Methode TextBoxSichtbarkeit() auf.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Zurücksetzen der Textbox Farben, wenn Berechnung geändert wird.
            TextboxFarbeZuruecksetzen();

            // Ändern der Sichtabrkeiten der Textfelder, Label und der Trackbar.
            TextBoxSichtbarkeit();
        }

        /// <summary>
        /// Setzt die Hintergrundfarbe für die Textbox, wenn diese leer ist.
        /// </summary>
        public void TextboxFarbe()
        {
            if (textBox1.Text == "")
            {
                textBox1.BackColor = Color.Crimson;
            } else
            {
                textBox1.BackColor = Color.Empty;
            }
            if (textBox2.Text == "")
            {
                textBox2.BackColor = Color.Crimson;
            } else
            {
                textBox2.BackColor = Color.Empty;
            }
            if (textBox3.Text == "")
            {
                textBox3.BackColor = Color.Crimson;
            } else
            {
                textBox3.BackColor = Color.Empty;
            }
            if (textBox4.Text == "")
            {
                textBox4.BackColor = Color.Crimson;
            } else
            {
                textBox4.BackColor = Color.Empty;
            }
        }

        /// <summary>
        /// Setzt die Hintergrundfarbe zurück, wenn z.b die zu berechnende Funktion gewechselt wird.
        /// </summary>
        public void TextboxFarbeZuruecksetzen()
        {          
            // Löschen der textbox Farben.
            textBox1.BackColor = Color.Empty;
            textBox2.BackColor = Color.Empty;
            textBox3.BackColor = Color.Empty;
            textBox4.BackColor = Color.Empty;
        }

        /// <summary>
        /// Setzt die Sichtbarkeiten der Eingaben für die möglichen Rechnungen.
        /// </summary>
        public void TextBoxSichtbarkeit()
        {
            try
            {
                switch (comboBox1.SelectedItem.ToString())
                {
                    case ("Kapital berechnen"):

                        label7.Visible = false;

                        // Trackbar ausblenden
                        trackBar1.Visible = false;

                        label3.Text = "Zins in [%]: ";

                        label1.Visible = true;
                        textBox1.Visible = true;

                        label2.Visible = true;
                        textBox2.Visible = true;

                        label3.Visible = true;
                        textBox3.Visible = true;

                        label5.Text = "Sparrate: ";
                        textBox4.Text = "0";
                        label5.Visible = true;
                        textBox4.Visible = true;
                        break;
                    case ("Jahre berechnen"):

                        label7.Visible = false;

                        // Trackbar ausblenden
                        trackBar1.Visible = false;

                        label3.Text = "Zins in [%]: ";

                        label1.Visible = false;                   
                        textBox1.Visible = false;

                        label2.Visible = true;
                        textBox2.Visible = true;

                        label3.Visible = true;
                        textBox3.Visible = true;

                        label5.Text = "Zielkapital: ";
                        textBox4.Clear();
                        label5.Visible = true;
                        textBox4.Visible = true;
                        break;
                    case ("Zins berechnen"):

                        label7.Visible = true;

                        // Trackbar für Zinssatz genauigkeit.
                        label3.Text = "Genauigkeit : " + Environment.NewLine
                            +"(Rechenintensiv!)";

                        trackBar1.Visible = true;


                        label1.Visible = true;
                        textBox1.Visible = true;

                        label2.Visible = true;
                        textBox2.Visible = true;

                        label3.Visible = true;
                        textBox3.Visible = false;

                        label5.Text = "Zielkapital: ";
                        textBox4.Clear();
                        label5.Visible = true;
                        textBox4.Visible = true;
                        break;
                    case ("Startkapital berechnen"):

                        label7.Visible = false;

                        // Trackbar ausblenden
                        trackBar1.Visible = false;

                        label3.Text = "Zins in [%]: ";

                        label1.Visible = true;
                        textBox1.Visible = true;

                        label2.Visible = false;
                        textBox2.Visible = false;

                        label3.Visible = true;
                        textBox3.Visible = true;

                        label5.Text = "Zielkapital: ";
                        textBox4.Clear();
                        label5.Visible = true;
                        textBox4.Visible = true;
                        break;
                }
            } catch (NullReferenceException)
            {
                // Fehler, wenn Nutzer nicht vorhandenes Item in der Listbox auswählt.
                label4.Text = "Nicht vorhandene Berechnung";
            }
        }

        /// <summary>
        /// Ausgabe der aktuellen Berechnungsgenauigkeit, wird angepasst, wenn sich der Wert der Trackbar ändert. (Zinssatz für Berechnung).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            // Ausgabe der aktuellen Berechnungsgenauigkeit. (Zinssatz für Berechnung).
            label7.Text = "" + Math.Round(0.01 / trackBar1.Value, 5);
        }

        private void label1_Click(object sender, EventArgs e)
        {
            switch (label1.Text)
            {
                case ("Jahre:"):
                    label1.Text = "Monate:";
                    break;
                case ("Monate:"):
                    label1.Text = "Jahre:";
                    break;
            }
        }

        private void label8_Click(object sender, EventArgs e)
        {
            label1_Click(sender, e);
        }
    }
}
