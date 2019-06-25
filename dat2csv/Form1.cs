using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace dat2csv
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public class Course
        {
            public string name;
            public int count=0;
            public Dictionary<int, List<int>> answers = new Dictionary<int, List<int>>();
            public Dictionary<int, float> stdd = new Dictionary<int, float>();
            public Dictionary<int, float> avg = new Dictionary<int, float>();
            public Dictionary<int, int[]> answerdist = new Dictionary<int,int[]>();

            public Course(string Name)
            {
                name = Name;
                for (int i = 0; i < 39; i++)
                {
                    answers.Add(i, new List<int>());
                    stdd.Add(i,0);
                    avg.Add(i,0);
                    answerdist.Add(i, new int[] { 0, 0, 0, 0, 0, 0 });
                }
            }
        }
        public static float StdDev(IEnumerable<int> values)
        {
            float mean = values.Sum() / values.Count();
            var squares_query =
                from int value in values
                select (value - mean) * (value - mean);
            float sum_of_squares = squares_query.Sum();
            return (float)Math.Sqrt(sum_of_squares / values.Count());
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string anketfolder = @"C:\Users\alaet\Downloads\anket\";
            List<Course> courses = new List<Course>();
            foreach (var coursedat in Directory.GetFiles(anketfolder,"*.dat"))
            {
                Course course = new Course(coursedat.Split('\\')[5].Substring(0, 3)); 
                string outstr = "";
                Console.WriteLine(coursedat);
                string[] lines = File.ReadAllLines(coursedat);
                int[] ansdist = new int[]{ 0, 0, 0, 0, 0, 0 };
                for (int i = 0; i < lines.Length; i++)
                {
                    outstr += (i+1).ToString() + ";";
                    for (int j = 40; j < 79 && j< lines[i].Length; j++)
                    {
                        int answer = 6 - (((int)lines[i][j]) - 64);
                        if (answer > 5 || answer < 0)
                            answer = 0;
                        course.answers[j - 40].Add(answer);
                        course.answerdist[j - 40][answer]++;                      
                        outstr += answer + ";";
                    }
                    outstr += "\n";
                    course.count++;
                }
                for (int i = 0; i < 39; i++)
                {
                    course.avg[i] = course.answers[i].Sum() / (float)course.answers[i].Count;
                    course.stdd[i] = StdDev(course.answers[i]);
                }
                courses.Add(course);
                courseinfo(course, coursedat);
            File.WriteAllText(coursedat + "_numeric.csv", outstr);
            }
        }

        private void courseinfo(Course course, string file)
        {
            string outstr = "";
            outstr += "BBM" + course.name + "\n";
            outstr += "Student Count;" + course.count + "\n";
            outstr += "Question;0;1;2;3;4;5;Avg;Stdd\n";
            for (int i = 0; i < 39; i++)
            {
                outstr += (i+1).ToString() + ";";
                for (int j = 0; j < 6; j++)
                {
                    outstr += course.answerdist[i][j].ToString() + ";";
                }
                outstr += course.avg[i].ToString() + ";";
                outstr += course.stdd[i].ToString();
                outstr += "\n";
            }
            File.WriteAllText(file + "_info.csv", outstr);
        }
    }
}
