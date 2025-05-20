using System;

namespace Calculator.Models
{
    public interface IOperation
    {
        double Execute(double a, double b);
    }

    public class Addition : IOperation
    {
        public double Execute(double a, double b) => a + b;
    }

    public class Subtraction : IOperation
    {
        public double Execute(double a, double b) => a - b;
    }

    public class Multiplication : IOperation
    {
        public double Execute(double a, double b) => a * b;
    }

    public class Division : IOperation
    {
        public double Execute(double a, double b)
        {
            if (b == 0) throw new DivideByZeroException();
            return a / b;
        }
    }

    public class Power : IOperation
    {
        public double Execute(double a, double b) => Math.Pow(a, b);
    }
}
