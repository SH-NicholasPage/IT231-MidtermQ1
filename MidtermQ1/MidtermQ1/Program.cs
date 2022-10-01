#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8629 // Nullable value type may be null.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MidtermQ1
{
    public class Program
    {
        public enum PersonType
        {
            Student,
            Teacher
        }

        private static Dictionary<Int32, Tuple<Char, String, String, Single, Int32?, Int32[]?>> Dict { get; set; } = new Dictionary<Int32, Tuple<Char, String, String, Single, Int32?, Int32[]?>>();
        private static Dictionary<Int32, Object> Validation { get; set; } = new Dictionary<Int32, Object>();
        private static Campus Campus { get; } = new Campus();
        private const int MAX_SCORE = 80;//20 points reserved for manual code review

        public static void Main()
        {
            List<List<String>> lines = null;

            if (File.Exists("inputs.txt") == true)
            {
                //This is so nasty lol
                lines = File.ReadLines("inputs.txt").ToList().Select(x => x.Split(",").Select(y => y.Trim()).ToList()).ToList();
            }
            else
            {
                throw new FileNotFoundException("ERROR: Couldn't find the inputs file. Tell the instructor about this ASAP.");
            }

            Setup(lines);
            float score = PerformTests();
            Console.WriteLine("\nScore: " + Math.Round(score, 2) + "/" + MAX_SCORE);
        }

        private static void Setup(List<List<String>> lines)
        {
            foreach (List<String> line in lines)
            {
                Int32? elem5 = null;
                Int32[]? arr = null;

                try
                {
                    elem5 = (Int32.TryParse(line[5], out _)) ? Convert.ToInt32(line[5]) : null;
                    arr = Array.ConvertAll(line[6].Split().Select(x => x.Trim()).ToArray(), s => int.Parse(s));
                }
                catch { }//Exceptions will be thrown. No error handling required.

                Dict.Add(Convert.ToInt32(line[0]), new Tuple<Char, String, String, Single, Int32?, Int32[]?>(line[1][0], line[2], line[3], Convert.ToSingle(line[4]), elem5, arr));

                switch (line[1][0])
                {
                    case 's'://Student
                    case 'f'://Teacher
                        Validation.Add(Convert.ToInt32(line[0]), Campus.PersonFactory((line[1][0] == 's') ? PersonType.Student : PersonType.Teacher, line[2], line[3], Convert.ToSingle(line[4])));
                        break;
                    case 'c'://Class
                        Validation.Add(Convert.ToInt32(line[0]), Campus.AddClass(line[2], line[3], Convert.ToSingle(line[4]), ((Teacher)Validation[elem5.Value]), arr!.Select(x => ((Student)Validation[x])).ToArray()));
                        break;
                }
            }
        }

        private static float PerformTests()
        {
            float score = MAX_SCORE;
            Console.WriteLine("Checking campus validity...");
            score -= CheckCampusValidity(22);
            Console.WriteLine("Checking inheritance & class completion...");
            score -= TestInheritance(25);
            Console.WriteLine("Checking method correctness in Campus and Class...");
            score -= TestOtherMethods(33);
            return score;
        }

        private static float CheckCampusValidity(float score)
        {
            int people = 0;
            int classes = 0;

            foreach(char c in Dict.Values.Select(x => x.Item1))
            {
                switch (c)
                {
                    case 's'://Student
                    case 'f'://Teacher
                        people++;
                        break;
                    case 'c'://Class
                        classes++;
                        break;
                }
            }

            if(Campus.People.Count != people)
            {
                Console.Error.WriteLine("Not enough people were added to the campus!");
                score = (score / 2) * (people - Math.Abs(Campus.People.Count - people)) / people;
            }
            
            if(Campus.Classes.Count != classes)
            {
                Console.Error.WriteLine("Not enough classes were added to the campus!");
                score = (score / 2) * (classes - Math.Abs(Campus.Classes.Count - classes)) / classes;
            }

            return score;
        }

        private static float TestInheritance(float score)
        {
            int sprops = 3;
            int tprops = 3;
            int cprops = 5;

            if (typeof(Student).GetProperties().Length < sprops)
            {
                Console.Error.WriteLine("Not enough properties for the Students!");
                score = (score / 3) * (sprops - Math.Abs(typeof(Student).GetProperties().Length - sprops)) / sprops;
            }

            if (typeof(Teacher).GetProperties().Length < tprops)
            {
                Console.Error.WriteLine("Not enough properties for the Teachers!");
                score = (score / 3) * (tprops - Math.Abs(typeof(Teacher).GetProperties().Length - tprops)) / tprops;
            }

            if (typeof(Class).GetProperties().Length < cprops)
            {
                Console.Error.WriteLine("Not enough properties for the Classes!");
                score = (score / 3) * (cprops - Math.Abs(typeof(Teacher).GetProperties().Length - cprops)) / cprops;
            }

            return score;
        }

        private static float TestOtherMethods(float score)
        {
            int checks = 7;
            float decreasePerIncorrect = (float)score / checks;
            float[] GPAs = Dict.Values.Where(x => x.Item1 == 's').Select(x => x.Item4).OrderBy(x => x).ToArray();
            float[] salaries = Dict.Values.Where(x => x.Item1 == 't').Select(x => x.Item4).OrderBy(x => x).ToArray();
            Class[] classes = Validation.Values.Count(x => x == null) > 0 ? null : Validation.Values.Where(x => x.GetType() == typeof(Class)).Select(x => (Class)x).ToArray();

            float med = (float)Math.Round(((GPAs.Length % 2 == 0) ? (GPAs[GPAs.Length / 2] + GPAs[GPAs.Length / 2 - 1]) / 2 : GPAs[GPAs.Length - 1]), 3);

            if(NearlyEqual(Campus.GetAvgStudentGPA(), GPAs.Average()) == false)
            {
                Console.Error.WriteLine("GetAvgStudentGPA() returned incorrect value. Returned: " + Math.Round(Campus.GetAvgStudentGPA(), 3) + " | Expected: " + Math.Round(GPAs.Average(), 3));
                score -= decreasePerIncorrect;
            }

            if(NearlyEqual(Campus.GetMedianStudentGPA(), med) == false)
            {
                Console.Error.WriteLine("GetMedianStudentGPA() returned incorrect value. Returned: " + Math.Round(Campus.GetMedianStudentGPA(), 3) + " | Expected: " + Math.Round(med, 3));
                score -= decreasePerIncorrect;
            }

            int key = Dict.Where(x => x.Value.Item1 == 's').OrderBy(x => x.Value.Item4).LastOrDefault().Key;

            if(Campus.GetStudentWithHighestGPA() != Validation[key])
            {
                Console.Error.WriteLine("GetStudentWithHighestGPA() returned incorrect value. Expected " + Dict[key].Item2);
                score -= decreasePerIncorrect;
            }

            if(NearlyEqual(Campus.GetAvgTeacherSalary(), salaries.Average()) == false)
            {
                Console.Error.WriteLine("GetAvgTeacherSalary() returned incorrect value. Returned: " + Math.Round(Campus.GetAvgTeacherSalary(), 3) + " | Expected: " + Math.Round(salaries.Average(), 3));
                score -= decreasePerIncorrect;
            }

            key = Dict.Where(x => x.Value.Item1 == 'c').GroupBy(x => x.Value.Item5).OrderByDescending(x => x.Count()).Select(x => x.Key).FirstOrDefault().Value;

            if(Campus.GetTeacherTeachingTheMost() != Validation[key])
            {
                Console.Error.WriteLine("GetTeacherTeachingTheMost() returned incorrect value. Expected " + Dict[key].Item2);
                score -= decreasePerIncorrect;
            }

            if (classes != null)
            {
                foreach (Class c in classes)
                {
                    key = Validation.Where(x => x.Value == c).First().Key;
                    if(c.GetHowManyStudentsInClass() != Dict[key].Item6!.Length)
                    {
                        Console.Error.WriteLine("GetHowManyStudentsInClass() returned incorrect value. Returned: " + c.GetHowManyStudentsInClass(), 3
                            + " | Expected: " + Dict[key].Item6!.Length);
                        score -= decreasePerIncorrect;
                    }

                    float avg = Convert.ToSingle(Dict.Where(x => Dict[key].Item6!.Contains(x.Key)).Select(x => x.Value.Item5).Average().Value);

                    if(NearlyEqual(c.GetAvgGPAInClass(), avg) == false)
                    {
                        Console.Error.WriteLine("GetAvgGPAInClass() returned incorrect value. Returned: " + Math.Round(c.GetAvgGPAInClass(), 3) + " | Expected: " + Math.Round(avg, 3));
                        score -= decreasePerIncorrect;
                    }
                }
            }
            else
            {
                Console.Error.WriteLine("One or more null classes exist, so testing the methods in Class has been skipped.");
                score -= decreasePerIncorrect * 2;//Since two checks can't be completed
            }

            return score;
        }

        //Stupid floating point error fix
        private static bool NearlyEqual(float a, float b, float delta = 0.001f)
        {
            return Math.Abs(a - b) <= delta;
        }
    }
}
