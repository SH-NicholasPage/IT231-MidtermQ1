/*
* Name: [YOUR NAME HERE]
* South Hills Username: [YOUR SOUTH HILLS USERNAME HERE]
*/
#pragma warning disable CS8603
#pragma warning disable CS8618
using System;
using System.Collections.Generic;
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

            return null;
            throw new NotImplementedException();
        }

        public Class AddClass(String courseID, String courseName, float creditsWorth, Teacher teacher, Student[] students)
        {
            return null;
            throw new NotImplementedException();
        }

        public float GetAvgStudentGPA()
        {
            throw new NotImplementedException();
        }

        public float GetMedianStudentGPA()
        {
            throw new NotImplementedException();
        }

        public Student GetStudentWithHighestGPA()
        {
            throw new NotImplementedException();
        }

        public float GetAvgTeacherSalary()
        {
            throw new NotImplementedException();
        }

        public Teacher GetTeacherTeachingTheMost()
        {
            throw new NotImplementedException();
        }
    }
}
