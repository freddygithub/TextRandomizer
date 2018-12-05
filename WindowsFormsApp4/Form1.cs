using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp4
{
    public partial class Form1 : Form
    {
        string gridFile = null, barcodeFile = null;
        public Form1()
        {
            InitializeComponent();

            label1.Parent = pictureBox1;
            label2.Parent = pictureBox1;
            label3.Parent = pictureBox1;
            label1.BackColor = Color.Transparent;
            label2.BackColor = Color.Transparent;
            label3.BackColor = Color.Transparent;
        }

        private void gridButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog fbd = new OpenFileDialog();
            fbd.Title = "Choose your input file:";
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                StreamReader sr = new StreamReader(fbd.FileName);
                gridFile = fbd.FileName;
                sr.Close();
            }
        }

        private void barcodeButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog fbd = new OpenFileDialog();
            fbd.Title = "Choose your input file:";
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                StreamReader sr = new StreamReader(fbd.FileName);
                barcodeFile = fbd.FileName;
                sr.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            doWork();
        }

        public void doWork()
        {
            List<string> chutes = new List<string>();
            ArrayList barcodes = new ArrayList();
            Random rnd = new Random();
            char level = char.MinValue;

            //checks if barcodefile has been selected
            if (barcodeFile == null)
            {
                MessageBox.Show("Did not select barcode file",
                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {
                using (var reader = new StreamReader(barcodeFile))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        barcodes.Add(line);
                    }
                }
            }

            //checks if grid file has been selected
            if (gridFile == null)
            {
                MessageBox.Show("Did not select grid physical file",
                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {
                using (var reader = new StreamReader(gridFile))
                {
                    string line; int x = 0;
                    while ((line = reader.ReadLine()) != null)
                    {
                        //this skips the first 2 lines that are usually comments
                        if (x++ > 1)
                        {
                            for (int i = 0; i < line.Length; i++)
                            {
                                if (Char.IsWhiteSpace(line[i]))
                                {
                                    line = line.Substring(0, i);
                                    break;
                                }
                            }

                            level = line[0];
                            line = line.Substring(1, line.Length - 1);

                            chutes.Add(line);
                        }
                    }
                }
            }

            //Arbitrary number
            int howManyBarcodes = 500;

            //Sort by alpha
            chutes.Sort();

            //first group always starts with A
            char temp = chutes[0][0];

            //keep track of letters
            ArrayList placesList = new ArrayList();

            //Add A to placesList
            placesList.Add(0);

            //Find where other letters start
            for (int i = 0; i < chutes.Count; i++)
            {
                if (chutes[i][0] != temp)
                {
                    placesList.Add(i);
                    temp = chutes[i][0];
                }
            }

            //removing the Y chute locations because they are not same length. 
            //placesList.RemoveAt(placesList.Count - 1);

            //find out where to stop counting up
            int endPoint = int.Parse(placesList[1].ToString());

            try
            {
                //delete preexisting file content
                using (var tsw = new StreamWriter(@"C:\TompkinsRobotics\barcodeRandom.txt"))
                {
                    tsw.Write("");
                }

                int x = 0;
                //if we want 300 products scanned, 4 letters, 75 products per letter
                for (int i = 0; i < (howManyBarcodes / placesList.Count); i++)
                {
                    //for random barcode
                    int randomTemp = rnd.Next(0, barcodes.Count);

                    if (x >= endPoint)
                        x = 0;

                    //to make sure we at least hit one barcode every time we iterate through them before doing random
                    if (i >= barcodes.Count)
                    {
                        int index = i % placesList.Count;

                        //if we have gone through all letters at least once, then increase index
                        if (i > 0 && index == 0)
                            x++;

                        if (chutes[int.Parse(placesList[index].ToString())][0] == 'Y')
                        {
                            using (var tsw = new StreamWriter(@"C:\TompkinsRobotics\barcodeRandom.txt", true))
                            {
                                tsw.WriteLine(barcodes[randomTemp] + "\t" + level + chutes[int.Parse(placesList[index].ToString()) + rnd.Next(0, (chutes.Count - int.Parse(placesList[placesList.Count - 1].ToString()) - 1))] + "\t\t\t" + "default" + "\t\t\t" + "default");
                            }
                        }
                        else
                        {
                            using (var tsw = new StreamWriter(@"C:\TompkinsRobotics\barcodeRandom.txt", true))
                            {
                                tsw.WriteLine(barcodes[randomTemp] + "\t" + level + chutes[int.Parse(placesList[index].ToString()) + x] + "\t\t\t" + "default" + "\t\t\t" + "default");
                            }
                        }
                    }
                    else
                    {
                        int index = i % placesList.Count;

                        using (var tsw = new StreamWriter(@"C:\TompkinsRobotics\barcodeRandom.txt", true))
                        {
                            tsw.WriteLine(barcodes[i] + "\t" + level + chutes[int.Parse(placesList[index].ToString())] + "\t\t\t" + "default" + "\t\t\t" + "default");
                        }
                    } 
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                return;
            }

            label3.Text = "Done!";
        }

    }
}
