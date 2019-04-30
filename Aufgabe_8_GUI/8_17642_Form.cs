using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// Using Anweisung für Namespace, der verwendeten Klasse ("class _8_17470_Klasse").
using WindowsFormsApplication2;

/// <summary>
/// Programm um den Kapitalanstieg über n Jahre zu berechnen, bei gegebenen Zinssatz und gegebenen Startkapital. 
/// Desweiteren ist eine Berechung aller anderen Werte mit jeweils 3 der Größen Zinssatz, Startkapital, Jahre und Zielkapital möglich.
/// Der Zinssatz wird dabei in einem extra Thread berechnet, da diese Berechnung größere Zeit in Anspruch nehmen kann.
/// 
/// Aufgabe: "8. Berechnen Sie den Kapitalertrag in Abhängigkeit vom Startkapital, Zinswert und Laufzeit."
/// 
/// By Eric Dornieden, Matrikelnummer: 17642
/// Copyright © 2019 by Eric Dornieden
/// </summary>
namespace Aufgabe_8_GUI
{

    public partial class Form1 : Form
    {
        #region Variablendeklaration

        // Erstellen eines neuen Objekts der Klasse "_8_17470_Klasse"
        _8_17470_Klasse kapitalertrag = new _8_17470_Klasse();

        // Variablen für die Multithreading (asynchrone) Berechnung des Zinssatzes und die anderen Berechnungen.
        private double Kapital { get; set; }
        private double StartKapital { get; set; }
        private double Ziel { get; set; }
        private double Zinssatz { get; set; }
        private int Jahre { get; set; }
        private double Genauigkeit { get; set; }

        // Ermöglicht das stoppen, der für die Rechnung benötigten Zeit.
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

        // Variablen für die vergangene Zeit.
        private Int64 RechenZeitMilli;
        private Int64 RechenZeitTicks;

        #endregion

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

            // Einstellungen für den Backgroundworker
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = true;
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

        #region Form1 Methoden

        /// <summary>
        /// Event beim druecken des Buttons "button1". Berechnung des Kapitalertrags und hinzufügen in Diagramm sowie Ausgabe des genauen Endwerts, der Rechnung.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            GraphErstellen();
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
                GraphErstellen();
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
                GraphErstellen();
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
        /// Ausgabe der aktuellen Berechnungsgenauigkeit, wird angepasst, wenn sich der Wert der Trackbar ändert. (Zinssatz für Berechnung).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            // Ausgabe der aktuellen Berechnungsgenauigkeit. (Zinssatz für Berechnung).
            label7.Text = "" + Math.Round(0.01 / trackBar1.Value, 5);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void label8_Click(object sender, EventArgs e)
        {
            label1_Click(sender, e);
        }

        /// <summary>
        /// Event Handler in der der Zinssatz berechnet wird.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            Jahre = Convert.ToInt32(textBox1.Text);
            StartKapital = Convert.ToDouble(textBox2.Text);

            Kapital = 0;
            Zinssatz = 0;

            // Bei Beginn der Rechnung aktuelle Zeit speichern.
            sw.Start();

            // Iteratives berechnen des Zinssatzes mit einer vorgegeben Genauigkeit. Durch erhöhen des Zinssatzes, bis dieser ausreichend.
            while (Kapital <= Ziel)
            {
                // Kapitalberechnen für geschätzten Zinssatz.
                try
                {
                    Zinssatz = Zinssatz + Genauigkeit;
                    Kapital = kapitalertrag.kapitalNeu(Jahre, StartKapital, Zinssatz);
                }
                catch (Exception)
                {
                    e.Cancel = true;
                    break;
                }

                // Wenn beenden ausstehend beenden.
                if (worker.CancellationPending == true)
                {
                    e.Cancel = true;
                    break;
                }
            }

            // Stoppen der Stoppuhr.
            sw.Stop();

            // Berechnen der vergangen Rechen Zeit.
            RechenZeitMilli = sw.ElapsedMilliseconds;
            RechenZeitTicks = sw.ElapsedTicks;

            // Zurücksetzen der Stoppuhr.
            sw.Reset();

        }

        /// <summary>
        /// Background Worker um Berechnung des Zinssates asynchron zu ermöglichen. (Durch einen zweiten Thread)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled == true)
            {
                // Ausgabe das Berechnung abgebrochen wurde.
                label14.Text = "Die Berechnung wurde abgebrochen!";
            }
            else if (e.Error != null)
            {
                label14.Text = "Bei der Berechnung ist ein Fehler aufgetreten";
            }
            // Wenn Berechnung erfolgreich ausgeführt, den Graphen erstellen Werte ausgeben.
            else
            {
                // Erstellen des Graphen für den berechneten Zinssatz 
                chart1.Series["Kapital"].Points.AddXY(0, textBox2.Text);

                for (int i = 1; i <= Convert.ToInt32(textBox1.Text); i++)
                {
                    Kapital = kapitalertrag.kapitalNeu(i, Convert.ToDouble(textBox2.Text), Zinssatz);
                    chart1.Series["Kapital"].Points.AddXY(i, Kapital);
                }

                // Ausgabe des Zinssatzes, welcher benötigt wird und der genutzten Genauigkeit.
                label14.Text = String.Format("Der benötige Zinssatz beträgt {0} Prozent, bei {2} {3}n." + Environment.NewLine +
                    "(Genauigkeit {1}) Es wurden {4} Millisekunden benötigt. ({5} Ticks)", Math.Round(Zinssatz, 6), Genauigkeit, textBox1.Text, label1.Text.Substring(0, label1.Text.Length - 1), RechenZeitMilli, RechenZeitTicks);

                backgroundWorker1.CancelAsync();
            }
        }

        /// <summary>
        /// Tritt ein, wenn die Form geschlossen wird. Wird genutzt um Rechnungs Thread zu beenden.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (backgroundWorker1.WorkerSupportsCancellation == true)
            {
                // Unterbrechen der asynchronen Berechnung des Zinssatzes.
                backgroundWorker1.CancelAsync();
            }
        }
        #endregion

        #region nicht vom Form abhängige Methoden
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
            }
            catch (NullReferenceException)
            {
                // Fehler, wenn Nutzer nicht vorhandenes Item in der Listbox auswählt.
                label14.Text = "Nicht vorhandene Berechnung";
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
                // Abfangen wenn das Kapital bereits größer oder gleich wie der Zielwert.
                if (kapital >= ziel)
                {
                    label14.Text = "Das Kapital ist größer oder gleich dem Zielwert!";
                    return;
                }

                // Abfangen negativer Zinssatz.
                if (zinssatz <= 0)
                {
                    label14.Text = "Nicht mathematisch Möglich";
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
                    label14.Text = String.Format("Es werden {0} {1} benötigt, bis das gewünschte Kapital erreicht wurde." + Environment.NewLine +
                        "Es sind noch {2} Euro über.", i, label1.Text.Substring(0, label1.Text.Length - 1), (kapital - ziel).ToString("0.00"), label1.Text.Substring(0, label1.Text.Length - 1));
                else
                    label14.Text = String.Format("Es werden {0} {1} benötigt, bis das gewünschte Kapital erreicht wurde.", i, label1.Text.Substring(0, label1.Text.Length-1));
            }
            catch (Exception)
            {
                // Ausgabe aeiner Fehlermeldung in Label14
                label14.Text = "Alle Felder müssen richtig ausgefüllt sein!";
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
                    label14.Text = String.Format("Der Kontostand beträgt nach {0} {1}n {2} Euro." + Environment.NewLine +
                        "Das Kapital ist um den Faktor {3} gestiegen.", textBox1.Text, label1.Text.Substring(0, label1.Text.Length - 1), kapital.ToString("0.00"), Math.Round(zuwachs, 3));
                else
                    label14.Text = String.Format("Der Kontostand beträgt nach {0} {1}n {2} Euro." + Environment.NewLine +
                        "Das Kapital ist um den Faktor {3} gesunken.", textBox1.Text, label1.Text.Substring(0, label1.Text.Length - 1), kapital.ToString("0.00"), Math.Round(zuwachs, 3));
            }
            catch (Exception) // Wird ausgeführt, wenn Fehler aufgetreten ist.
            {
                // Ausgabe aeiner Fehlermeldung in Label14
                label14.Text = "Alle Felder müssen richtig ausgefüllt sein!";
            }
        }

        /// <summary>
        /// Berechnet den benötigten Zinssatz, bei gegebenem Startwert, Zielwert und Anzahl an Jahren.
        /// </summary>
        public void ZinsBerechnen()
        {
            // Ruft die Methode zum ändern der Textbox Farbe auf.
            TextboxFarbe();

            // Variable für die gewünschte Rechengenauigkeit. (Zinssatz für die Berechnung)
            Genauigkeit = 0.01 / trackBar1.Value;         

            // Try um leere Felder bzw. falsche Eingaben abzufangen.
            try
            {
                // Setzt das Kapital und das Ziel
                Kapital = Convert.ToDouble(textBox2.Text);
                Ziel = Convert.ToDouble(textBox4.Text);

                // Abbruch, wenn Kapital größer Ziel.
                if (Kapital > Ziel)
                {
                    label14.Text = "Das Kapital ist gleich oder größer dem Startwert.";
                    return;
                }

                // Prüft, ob Backgroundworker, bereits beschäftigt ist.
                if (backgroundWorker1.IsBusy != true)
                {
                    // Beginn der Rechnung durch Label kennzeichnen.
                    label14.Text = "Asynchrone Berechnung gestartet!";
                    // Start the asynchronous operation.
                    backgroundWorker1.RunWorkerAsync();
                } else
                {
                    string RechnungAktiv = "Rechnung aktiv! Nochmal klicken zum beenden!";
                        
                    if (label14.Text != RechnungAktiv)
                    {
                        label14.Text = "Rechnung aktiv! Nochmal klicken zum beenden!";
                    }
                    else if (backgroundWorker1.WorkerSupportsCancellation == true)
                    {
                        // Unterbrechen der asynchronen Berechnung des Zinssatzes.
                        backgroundWorker1.CancelAsync();
                    }

                }            
            }
            catch (Exception)
            {
                // Ausgabe aeiner Fehlermeldung in Label14
                label14.Text = "Alle Felder müssen richtig ausgefüllt sein!";
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
                label14.Text = String.Format("Das benötigte Startkapital beträgt {0} Euro, bei {1} {2}n.",Math.Round(startkapital,2), textBox1.Text, label1.Text.Substring(0, label1.Text.Length - 1));
            } catch (Exception)
            {
                // Ausgabe aeiner Fehlermeldung in Label14
                label14.Text = "Alle Felder müssen richtig ausgefüllt sein!";
            }
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
                        /*
                         * Kapital Berechnen.
                         */
                    case ("Kapital berechnen"):

                        //Label für Fn Tasten
                        label14.Visible = true;
                        label11.Visible = true;
                        label12.Visible = true;
                        label13.Visible = true;

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

                        /*
                        * Jahre Berechnen.
                        */ 
                    case ("Jahre berechnen"):

                        //Label für Fn Tasten
                        label14.Visible = false;
                        label11.Visible = true;
                        label12.Visible = true;
                        label13.Visible = true;

                        label7.Visible = false;

                        // Trackbar ausblenden
                        trackBar1.Visible = false;

                        label3.Text = "Zins in [%]: ";

                        label1.Visible = true;                   
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

                        /*
                         * Zins berechnen.
                         */
                    case ("Zins berechnen"):

                        //Label für Fn Tasten
                        label14.Visible = true;
                        label11.Visible = true;
                        label12.Visible = false;
                        label13.Visible = true;

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

                        /*
                         * Startkapital Berechnen.
                         */
                    case ("Startkapital berechnen"):

                        //Label für Fn Tasten
                        label14.Visible = true;
                        label11.Visible = false;
                        label12.Visible = true;
                        label13.Visible = true;

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
                label14.Text = "Nicht vorhandene Berechnung";
            }
        }
        #endregion

        /// <summary>
        /// Überprüfen ob Taste F1-F4 gedrückt und dann Textbox anwählen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F1:
                    if (textBox1.Visible)
                    {
                        textBox1.Focus();
                        textBox1.SelectAll();
                    }                       
                    break;

                case Keys.F2:
                    if (textBox2.Visible)
                    {
                        textBox2.Focus();
                        textBox2.SelectAll();
                    }
                    break;

                case Keys.F3:
                    if (textBox3.Visible)
                    {
                        textBox3.Focus();
                        textBox3.SelectAll();
                    }
                    break;

                case Keys.F4:
                    if (textBox4.Visible)
                    {
                        textBox4.Focus();
                        textBox4.SelectAll();
                    }
                    break;              
            }
            
        }
    }
}