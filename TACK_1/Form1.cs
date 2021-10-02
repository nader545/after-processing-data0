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
            //Validate file name format;
            string format = inputFilePath.Substring(inputFilePath.Length - 13);            
            Regex reg = new Regex("^(\\d{2}[-]\\d{2}[-]\\d{4}[\\.]pu)$");
            if (!reg.IsMatch(format))
            {
                MessageBox.Show("The file name is the incorrect format !");
                return;
            }
            //Validate that the file exists in the path entered by the user;
            if (File.Exists(inputFilePath))                
            {
                //In this line, the date has been omitted from the file path;
                string date_file_name = inputFilePath.Substring(inputFilePath.Length - 13, 10);
                StreamReader sr = new StreamReader(inputFilePath, Encoding.Default);    //Read the first line of the file;
                var first_line = sr.ReadLine().Split(',').ToList();
                string date_first_line = first_line[0].ToString().Substring(5);         //the date in the first line;  
                //Make sure that the date in the file name matches the date on the first line inside the file;
                if (date_first_line != date_file_name)       
                {
                    MessageBox.Show("invalid file : The date in the file name does not match the date on the first line of the file !");
                    return;
                }                               
                string Line;             
                int count = 0;
                List<string> file = new List<string>();
                //Read all lines from the file and put them in a list ;
                while ((Line = sr.ReadLine()) != null)         
                {
                    file.Add(Line);
                    count++;
                }
                string count_first_line = first_line[1].ToString().Substring(6);
                //Verify that the number of burglaries on the first line is correct;
                if (Convert.ToInt32(count_first_line) != count)                    
                {
                    MessageBox.Show("invalid file!! : count on first line don’t match with lines count ");
                    return;
                }
                //In this line, specify the path to save the entire file with the date;
                string Path = OutputFilePath + date_file_name + ".ur";
                using (StreamWriter sw = new StreamWriter(Path))
                {
                    //Write the first line in the processing file;
                    sw.WriteLine(first_line[0].ToString() + "," + first_line[1].ToString() + "\n" + "{");  
                    for (int i = 0; i < file.Count; i++)
                    {
                        //Separate all the lines into two halves, between [] in one part, and before them in one part;
                        string[] All = file[i].ToString().Split(new string[] { "[", "]" }, StringSplitOptions.None);
                        string[] first_half = All[0].Split(new char[] { ',' }); //put{Number , string}in array; 
                        if (first_half.Count() != 4)  //Check the number of columns in all lines;
                        {
                            MessageBox.Show("The number of columns per line is incorrect !");
                            return;
                        }
                        Regex reg2 = new Regex("^(\\d+)$");   //Validate that the first element in the line is a number
                        if (!reg2.IsMatch(first_half[0]))
                        {
                            MessageBox.Show("The first element is not a number !");
                            return;
                        }
                        sw.WriteLine("    [");
                        //Writing the first half {Number , string};
                        sw.WriteLine("        age:" + first_half[0].ToString() + "," + "\n" + "        country:" + first_half[1].ToString() + "," + "\n" + "        Name:" + first_half[2].ToString());
                        if (All[1].Contains(","))   //Find out if there is between [] more than one element {array of string};  
                        {   //Write more than one element;
                            string[] last_half = All[1].Split(new char[] { ',' }); //put{array of string}in array;
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
 