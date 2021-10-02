using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

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
            ParseData(inputFilePath, OutputFilePath);
        }

        public static void ParseData(string inputFilePath, string OutputFilePath)
        {     
            string date_file_name = inputFilePath.Substring(inputFilePath.Length - 13, 10);  //In this line, the date has been omitted from the file path;
            string Path = OutputFilePath + date_file_name + ".ur";      //In this line, specify the path to save the entire file with the date;
            if (File.Exists(inputFilePath))                // we want to make sure that the file exists in this path;
            {
                StreamReader sr = new StreamReader(inputFilePath, Encoding.Default);    
                var first_line = sr.ReadLine().Split(',').ToList();                    
                string date_first_line = first_line[0].ToString().Substring(5);        //the date in the first line;
                if (date_first_line != date_file_name)       //Make sure that the date in the file name matches the date on the first line inside the file;
                {
                    MessageBox.Show("invalid file!! : file name don’t match with date in first line");
                    return;
                }                               
                string Line;             
                int count = 0;
                List<string> file = new List<string>();         
                while ((Line = sr.ReadLine()) != null)         //Read all lines from the file and put them in a list ;
                {
                    file.Add(Line);
                    count++;
                }
                string count_first_line = first_line[1].ToString().Substring(6);
                if (Convert.ToInt32(count_first_line) != count)                    //Verify that the number of burglaries on the first line is correct;
                {
                    MessageBox.Show("invalid file!! : count on first line don’t match with lines count ");
                    return;
                }
                using (StreamWriter sw = new StreamWriter(Path))
                {
                    sw.WriteLine(first_line[0].ToString() + "," + first_line[1].ToString() + "\n" + "{");  //Write the first line in the processing file;
                    for (int i = 0; i < file.Count; i++)
                    {
                          //Separate all the lines into two halves, between [] in one part, and before them in one part;
                        string[] All = file[i].ToString().Split(new string[] { "[", "]" }, StringSplitOptions.None);
                        string[] first_half = All[0].Split(new char[] { ',' });
                        sw.WriteLine("    [");
                        sw.WriteLine("        age:" + first_half[0].ToString() + "," + "\n" + "        country:" + first_half[1].ToString() + "," + "\n" + "        Name:" + first_half[2].ToString()); //Writing the first half;
                        if (All[1].Contains(","))     //Find out if there is between [] more than one element;
                        {
                            string[] last_half = All[1].Split(new char[] { ',' });
                            sw.WriteLine("        Date:[");
                            for (int x = 0; x < last_half.Length; x++)
                            {
                                sw.WriteLine("             " + last_half[x] + ","); //Write more than one element;                      
                            }
                            sw.WriteLine("        ]");
                            sw.WriteLine("    ]");
                        }
                        else
                        {
                            sw.WriteLine("        Date:[");
                            sw.WriteLine("            "+All[1]);
                            sw.WriteLine("        ]");
                            sw.WriteLine("    ],");
                        }
                    }
                }
            }
            MessageBox.Show(" The data has been processed successfully ");
        }
    }
}
 