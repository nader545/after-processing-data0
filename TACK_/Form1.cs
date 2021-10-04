using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace TACK_1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string inputFilePath = txt_inPath.Text;
            string OutputFilePath = txt_OutPath.Text;  //Please type the path only without the file name
            Stopwatch sw = new Stopwatch();
            sw.Start();
            try
            {
                ParseData(inputFilePath, OutputFilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            sw.Stop();
            MessageBox.Show(sw.ElapsedTicks.ToString());
        }
        /// <summary>
        /// validate if file input is exist and have valid name;
        /// </summary>
        /// <param name="inputFilePath"></param>
        /// <returns></returns>
        public static bool ValidateInputFile(string inputFilePath)
        {

            FileInfo file = new FileInfo(inputFilePath); //get file info;         
            if (!file.Exists) //check if file exist;
                return false;
            if (file.Name != string.Format("{0}.pu", DateTime.Now.ToString("dd-MM-yyyy"))) //check if file name is valid;
                return false;
            return true;
        }
        public static void  writeinfilr(string OutputFilePath)
        {

        }
        /// <summary>
        /// Reading an input file and then writing it to an output file after processing the data;
        /// </summary>
        /// <param name="inputFilePath"></param>
        /// <param name="OutputFilePath"></param>
        /// 
        public static void ParseData(string inputFilePath, string OutputFilePath)
        {            
            if (!ValidateInputFile(inputFilePath))      //validate input file;
                throw new Exception("The file does not exist or the file name is not in the correct format !");
            var fileLines = File.ReadAllLines(inputFilePath);    //Read all lines of the file;
            var first_line = fileLines[0].Split(',').ToList();   //Read the first line of the file;
            if(first_line[0] != ("date:" + inputFilePath.Substring(inputFilePath.Length - 13, 10))) //Substring the date part from the first line and then match with The date in the file name;
                throw new Exception("Invalid file : Date does not match !");
            if(first_line[1] !="count:" + (fileLines.Count() -1).ToString()) //validate that the count of lines in the file is correct;
                throw new Exception("Invalid file : The count of lines in the file is incorrect !");
            using (StreamWriter sw = new StreamWriter(OutputFilePath + inputFilePath.Substring(inputFilePath.Length - 13, 10) + ".ur")) //Writing in the extracted file;
            {
                sw.WriteLine(fileLines[0] + "\n" + "{");
                for (int r = 1; r < fileLines.Length; r++)
                {   //Put a line by line in an array and divide it into two parts, the first is the number and the string, and the second is the array of string;
                    string[] All = fileLines[r].ToString().Split(new string[] { "[", "]" }, StringSplitOptions.None);
                    string[] first_half = All[0].Split(new char[] { ',' }); //put part One{Number , string}in array;
                    if (first_half.Count() != 4)  //Check the number of columns in all lines;
                        throw new Exception("Invalid file : The count of columns is incorrect !");
                    sw.WriteLine("    [");
                    //Writing the first half {Number , string};
                    sw.WriteLine("        age:" + first_half[0].ToString() + "," + "\n" + "        country:" + first_half[1].ToString() + "," + "\n" + "        Name:" + first_half[2].ToString());
                    if (All[1].Contains(","))   //Find out if there is between [] more than one element {array of string};  
                    {   //Write more than one element;
                        string[] last_half = All[1].Split(new char[] { ',' }); //put part second{array of string}in array;
                        sw.WriteLine("        Date:[");
                        for (int x = 0; x < last_half.Length; x++)
                        {
                            sw.WriteLine("             " + last_half[x] + ",");
                        }
                        sw.WriteLine("        ]");
                        sw.WriteLine("    ]");
                    }
                    else
                    {   //If it is one element;
                        sw.WriteLine("        Date:[");
                        sw.WriteLine("            " + All[1]);
                        sw.WriteLine("        ]");
                        sw.WriteLine("    ],");
                    }
                }
            }
            MessageBox.Show("The data from the file has been successfully processed");
        }
    }
}
 