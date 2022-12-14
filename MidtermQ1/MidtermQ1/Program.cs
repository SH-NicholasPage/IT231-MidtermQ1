#pragma warning disable CS8600, CS8629
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

            float score = 0f;

            if (Setup(lines) == true)
            {
                score = PerformTests();
            }
            Console.WriteLine("\nScore: " + Math.Round(score, 2) + "/" + MAX_SCORE);
        }

        private static bool Setup(List<List<String>> lines)
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
                        try
                        {
                            Validation.Add(Convert.ToInt32(line[0]), Campus.AddClass(line[2], line[3], Convert.ToSingle(line[4]), ((Teacher)Validation[elem5.Value]), arr!.Select(x => ((Student)Validation[x])).ToArray()));
                        }
                        catch (InvalidCastException)
                        {
                            Console.Error.WriteLine("FATAL ERROR: A person was not added correctly. Aborting.");
                            return false;
                        }
                        break;
                }
            }

            return true;
        }

        private static float PerformTests()
        {
            float score = MAX_SCORE;
            Console.WriteLine("Checking campus validity...");
            score -= Math.Abs(CheckCampusValidity(22) - 22);
            Console.WriteLine("Checking inheritance & class completion...");
            score -= Math.Abs(TestInheritance(25) - 25);
            Console.WriteLine("Checking method correctness in Campus and Class...");
            score -= Math.Abs(TestOtherMethods(33) - 33);
            return score;
        }

        private static float CheckCampusValidity(float score)
        {
            int people = 0;
            int classes = 0;

            foreach (char c in Dict.Values.Select(x => x.Item1))
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

            if (Campus.People.Count != people)
            {
                Console.Error.WriteLine("Not enough people were added to the campus!");
                score = (score / 2) * (people - Math.Abs(Campus.People.Count - people)) / people;
            }

            if (Campus.Classes.Count != classes)
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
            float[] salaries = Dict.Values.Where(x => x.Item1 == 'f').Select(x => x.Item4).OrderBy(x => x).ToArray();
            Class[] classes = Validation.Values.Count(x => x == null) > 0 ? null : Validation.Values.Where(x => x.GetType() == typeof(Class)).Select(x => (Class)x).ToArray();

            float med = (float)Math.Round(((GPAs.Length % 2 == 0) ? (GPAs[GPAs.Length / 2] + GPAs[GPAs.Length / 2 - 1]) / 2 : GPAs[GPAs.Length - 1]), 3);

            try
            {
                if (NearlyEqual(Campus.GetAvgStudentGPA(), GPAs.Average()) == false)
                {
                    Console.Error.WriteLine("GetAvgStudentGPA() returned incorrect value. Returned: " + Math.Round(Campus.GetAvgStudentGPA(), 3) + " | Expected: " + Math.Round(GPAs.Average(), 3));
                    score -= decreasePerIncorrect;
                }
            }
            catch (Exception ex) when (ex is NotImplementedException == false)
            {
                Console.Error.WriteLine("ERROR: GetAvgStudentGPA() threw an exception.");
                score -= decreasePerIncorrect;
            }

            try
            {
                if (NearlyEqual(Campus.GetMedianStudentGPA(), med) == false)
                {
                    Console.Error.WriteLine("GetMedianStudentGPA() returned incorrect value. Returned: " + Math.Round(Campus.GetMedianStudentGPA(), 3) + " | Expected: " + Math.Round(med, 3));
                    score -= decreasePerIncorrect;
                }
            }
            catch (Exception ex) when (ex is NotImplementedException == false)
            {
                Console.Error.WriteLine("ERROR: GetMedianStudentGPA() threw an exception.");
                score -= decreasePerIncorrect;
            }

            int key = Dict.Where(x => x.Value.Item1 == 's').OrderBy(x => x.Value.Item4).LastOrDefault().Key;

            try
            {
                if (Campus.GetStudentWithHighestGPA() != Validation[key])
                {
                    Console.Error.WriteLine("GetStudentWithHighestGPA() returned incorrect value. Expected " + Dict[key].Item2);
                    score -= decreasePerIncorrect;
                }
            }
            catch (Exception ex) when (ex is NotImplementedException == false)
            {
                Console.Error.WriteLine("ERROR: GetStudentWithHighestGPA() threw an exception.");
                score -= decreasePerIncorrect;
            }

            try
            {
                if (NearlyEqual(Campus.GetAvgTeacherSalary(), salaries.Average()) == false)
                {
                    Console.Error.WriteLine("GetAvgTeacherSalary() returned incorrect value. Returned: " + Math.Round(Campus.GetAvgTeacherSalary(), 3) + " | Expected: " + Math.Round(salaries.Average(), 3));
                    score -= decreasePerIncorrect;
                }
            }
            catch (Exception ex) when (ex is NotImplementedException == false)
            {
                Console.Error.WriteLine("ERROR: GetAvgTeacherSalary() threw an exception.");
                score -= decreasePerIncorrect;
            }

            if (classes != null)
            {
                key = Dict.Where(x => x.Value.Item1 == 'c').GroupBy(x => x.Value.Item5).OrderByDescending(x => x.Count()).Select(x => x.Key).FirstOrDefault().Value;

                try
                {
                    if (Campus.GetTeacherTeachingTheMost() != Validation[key])
                    {
                        Console.Error.WriteLine("GetTeacherTeachingTheMost() returned incorrect value. Expected " + Dict[key].Item2);
                        score -= decreasePerIncorrect;
                    }
                }
                catch (Exception ex) when (ex is NotImplementedException == false)
                {
                    Console.Error.WriteLine("ERROR: GetTeacherTeachingTheMost() threw an exception.");
                    score -= decreasePerIncorrect;
                }

                foreach (Class c in classes)
                {
                    key = Validation.Where(x => x.Value == c).First().Key;
                    try
                    {
                        if (c.GetHowManyStudentsInClass() != Dict[key].Item6!.Length)
                        {
                            Console.Error.WriteLine("GetHowManyStudentsInClass() returned incorrect value. Returned: " + c.GetHowManyStudentsInClass(), 3
                                + " | Expected: " + Dict[key].Item6!.Length);
                            score -= decreasePerIncorrect;
                        }
                    }
                    catch (Exception ex) when (ex is NotImplementedException == false)
                    {
                        Console.Error.WriteLine("ERROR: GetHowManyStudentsInClass() threw an exception.");
                        score -= decreasePerIncorrect;
                    }

                    List<KeyValuePair<Int32, Tuple<Char, String, String, Single, Int32?, Int32[]?>>> test2 = new List<KeyValuePair<int, Tuple<char, string, string, float, int?, int[]?>>>();

                    foreach (KeyValuePair<Int32, Tuple<Char, String, String, Single, Int32?, Int32[]?>> kv in Dict)
                    {
                        if (Dict[key].Item6!.Contains(kv.Key))
                        {
                            test2.Add(kv);
                        }
                    }

                    float avg = Convert.ToSingle(Dict.Where(x => Dict[key].Item6!.Contains(x.Key)).Select(x => x.Value.Item4).Average());

                    try
                    {
                        if (NearlyEqual(c.GetAvgGPAInClass(), avg) == false)
                        {
                            Console.Error.WriteLine("GetAvgGPAInClass() returned incorrect value. Returned: " + Math.Round(c.GetAvgGPAInClass(), 3) + " | Expected: " + Math.Round(avg, 3));
                            score -= decreasePerIncorrect;
                        }
                    }
                    catch (Exception ex) when (ex is NotImplementedException == false)
                    {
                        Console.Error.WriteLine("ERROR: GetAvgGPAInClass() threw an exception.");
                        score -= decreasePerIncorrect;
                    }
                }
            }
            else
            {
                Console.Error.WriteLine("One or more null classes exist, so testing anything related to classes will be skipped.");
                score -= decreasePerIncorrect * 3;//Since two checks can't be completed
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
