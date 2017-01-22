using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polynomials
{
    class OneVariablePolynomial
    {
        public List<double> coefficients;

        public OneVariablePolynomial(double[] coefficients)
        {
            this.coefficients = coefficients.OfType<double>().ToList();
            CullFromEnd();            
        }

        /// <summary>
        ///  Instantiates the single degree polynomial with array of coefficients. Assumption is that the leading term is not zero.
        /// </summary>
        /// <param name="coefficients"></param>
        public OneVariablePolynomial(List<double> coefficients)
        {
            this.coefficients = coefficients;
            CullFromEnd();
        }

        public OneVariablePolynomial()
        {
            this.coefficients = null;
        }

        public OneVariablePolynomial(OneVariableMonomial monomial)
        {
            this.coefficients = new List<double>(new double[monomial.degree + 1]);
            this.coefficients[monomial.degree] = monomial.coefficient;
        }

        /// <summary>
        /// The Leading term of the polynomial.
        /// </summary>
        /// <returns>A tuple with the degree and coefficient of the leading term.</returns>
        public OneVariableMonomial LeadingTerm()
        {
            if (this.IsZero())
            {
                return new OneVariableMonomial(0, 0);
            }

            double leadingCoefficient = this.coefficients[this.coefficients.Count - 1];
            int degree = this.coefficients.Count - 1;
            return new OneVariableMonomial(degree, leadingCoefficient);
        }

        /// <summary>
        /// Is this a "zero polynomial". This can happen if it has no coefficients or if all the coefficients are zero.
        /// </summary>
        /// <returns>A boolean - true for zero polynomials, false otherwise.</returns>
        public bool IsZero()
        {
            if(this.coefficients == null)
            {
                return true;
            }

            bool isZero = true;
            foreach(double d in this.coefficients)
            {
                if (d != 0)
                {
                    isZero = false;
                }
            }

            return isZero;
        }

        /// <summary>
        /// Adds the polynomial to another polynomial
        /// </summary>
        /// <returns></returns>
        public OneVariablePolynomial Add(OneVariablePolynomial otherPolynomial, bool addOrSubtract = true)
        {
            if (this.IsZero())
            {
                return otherPolynomial;
            }

            // First, initialize a zero list.
            List<double> result = new List<double>(new double[Math.Max(this.coefficients.Count, otherPolynomial.coefficients.Count)]);
            int i = 0;
            foreach (double d in this.coefficients)
            {
                result[i++] += d;
            }

            i = 0;
            foreach (double d in otherPolynomial.coefficients)
            {
                if (addOrSubtract)
                {
                    result[i++] += d;
                }
                else
                {
                    result[i++] -= d;
                }
            }

            return new OneVariablePolynomial(result);
        }

        public OneVariablePolynomial Multiply(OneVariablePolynomial otherPolynomial)
        {
            throw new Exception("Not implemented yet");
        }

        public OneVariablePolynomial Multiply(OneVariableMonomial monomial)
        {
            if (monomial.coefficient == 0)
            {
                return new OneVariablePolynomial();
            }

            List<double> result = new List<double>(new double[this.coefficients.Count + monomial.degree]);
            for(int i = 0; i < this.coefficients.Count; i++)
            {
                result[i + monomial.degree] = this.coefficients[i] * monomial.coefficient;
            }

            return new OneVariablePolynomial(result);
        }

        public OneVariablePolynomial Multiply(double factor)
        {
            List<double> result = new List<double>(new double[this.coefficients.Count]);
            int i = 0;
            foreach (double d in this.coefficients)
            {
                result[i++] = factor * d;
            }

            return new OneVariablePolynomial(result);
        }

        /// <summary>
        /// Divides the polynomial with the divisor.
        /// </summary>
        /// <param name="divisor">The divisor. Denoted by g in the CLO book.</param>
        /// <returns>The quotient and remainder in the form of a tuple.</returns>
        public Dictionary<string, OneVariablePolynomial> Divide(OneVariablePolynomial divisor)
        {
            OneVariablePolynomial quotient = new OneVariablePolynomial();
            OneVariablePolynomial remainder = this;
            while (!remainder.IsZero() && divisor.LeadingTerm().degree <= remainder.LeadingTerm().degree)
            {
                OneVariableMonomial update = remainder.LeadingTerm().Divide(divisor.LeadingTerm());
                quotient = quotient.Add(new OneVariablePolynomial(update));
                remainder = remainder.Add(divisor.Multiply(update), false); // A false addition is basically a subtraction.
            }

            return new Dictionary<string, OneVariablePolynomial>()
            {
                { "quotient", quotient},
                { "remainder", remainder}
            };
        }

        /// <summary>
        /// Finds the Greatest Common Divisor between this and another polynomial
        /// </summary>
        /// <param name="otherPolynomial">The other polynomial with which the GCD is to be calculated</param>
        /// <returns>The Greatest Common Divisor</returns>
        public OneVariablePolynomial GCD(OneVariablePolynomial otherPolynomial)
        {
            OneVariablePolynomial h = this;
            OneVariablePolynomial s = otherPolynomial;
            OneVariablePolynomial rem = new OneVariablePolynomial();
            while (!s.IsZero())
            {
                rem = h.Divide(s)["remainder"];
                h = s;
                s = rem;
            }

            return rem;
        }

        private void CullFromEnd()
        {
            int size = this.coefficients.Count - 1;
            for (int i = size; i >= 0; i--) // Remove the leading zeros from list of coefficients.
            {
                if (this.coefficients[i] == 0)
                {
                    this.coefficients.RemoveAt(i);
                }
                else
                {
                    break;
                }
            }
        }
    }
}
