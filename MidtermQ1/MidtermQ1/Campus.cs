/*
* Name: [YOUR NAME HERE]
* South Hills Username: [YOUR SOUTH HILLS USERNAME HERE]
*/
#pragma warning disable CS8600, CS8603, CS8618
using System;
using System.Collections.Generic;
using System.Linq;
using static MidtermQ1.Program;

namespace MidtermQ1
{
    public class Campus
    {
        public List<Person> People { get; private set; } = new List<Person>();
        public List<Class> Classes { get; private set; } = new List<Class>();

        //Factory
        public Person PersonFactory(PersonType personType, String name, String email, float GPA_or_Salary)
        {
            //TODO: Create a factory method. Add the created person to the list People and return the newly created person.
            throw new NotImplementedException();
        }

        public Class AddClass(String courseID, String courseName, float creditsWorth, Teacher teacher, Student[] students)
        {
            //TODO: Construct a Class, add it to the Class list, and return the newly created Class.
            throw new NotImplementedException();
        }

        public float GetAvgStudentGPA()
        {
            //TODO: Calculate the average GPA for all students and return it.
            //Example on how to only iterate through students from the People list
            foreach(Student s in People.Where(x => x.GetType() == typeof(Student)))
            {

            }
            throw new NotImplementedException();
        }

        public float GetMedianStudentGPA()
        {
            //TODO: Calculate the median GPA for all students and return it.
            //Reminder: The median is the central number of a data set. If there are 2 numbers in the middle, the median is the average of those 2 numbers.
            //Ex: The median of this array [2, 2, 3, 5, 8] is 3. The median of this array [4, 6, 6, 8, 9, 10] is 7.
            throw new NotImplementedException();
        }

        public Student GetStudentWithHighestGPA()
        {
            //TODO: Return the student who has the highest GPA.
            //The following line will get the students for you:
            List<Student> students = People.Where(x => x.GetType() == typeof(Student)).Select(x => x as Student).ToList()!;
            throw new NotImplementedException();
        }

        public float GetAvgTeacherSalary()
        {
            //TODO: Return the average salary for all teachers and return it.
            //The following line will get the teachers for you:
            List<Teacher> teachers = People.Where(x => x.GetType() == typeof(Teacher)).Select(x => x as Teacher).ToList()!;
            throw new NotImplementedException();
        }

        public Teacher GetTeacherTeachingTheMost()
        {
            //TODO: Return the teacher who is teaching the most classes.
            throw new NotImplementedException();
        }
    }
}
