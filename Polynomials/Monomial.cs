using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polynomials
{
    class Monomial : IComparable
    {
        public int[] powers; // Consider uint instead of int.

        /// <summary>
        /// The ordering scheme the monomials are going to use. 
        /// WARNING - This is a weak point in the code. The user should
        /// set this only once when the polynomials/ basis are being
        /// constructed and should not change it in the middle of computations.
        /// </summary>
        public static string orderingScheme = "lex";

        /// <summary>
        /// Initializes an instance of a monomial of a given degree.
        /// </summary>
        /// <param name="numVariables"></param>
        public Monomial(int numVariables, double coefficient = 1)
        {
            this.powers = new int[numVariables];
        }

        /// <summary>
        /// Instantiates the Monomial class.
        /// </summary>
        /// <param name="powers">An array with the powers of each of the variables.</param>
        /// <param name="coefficient">The coefficient of the monomial.</param>
        public Monomial(int[] powers, double coefficient = 1)
        {
            this.powers = powers;
        }

        /// <summary>
        /// Instantiates an instance of the Monomial class. Creates a deep copy of an existing monomial.
        /// </summary>
        /// <param name="m">The existing monomial to be deep copied.</param>
        public Monomial(Monomial m)
        {
            int[] newPowers = new int[m.powers.Length];
            for (int i = 0; i < newPowers.Length; i++)
            {
                newPowers[i] = m.powers[i];
            }

            this.powers = newPowers;
        }

        /// <summary>
        /// Instantiates an instance of the <see cref="Monomial"/> class.
        /// </summary>
        /// <param name="powers">An int array of powers of the different variables.</param>
        public Monomial(params int[] powers)
        {
            this.powers = powers;
        }

        public int Compare(object x, object y)
        {
            Monomial m1 = (Monomial)x;
            Monomial m2 = (Monomial)y;

            // This is the lex ordering scheme.
            int numVariables = Math.Min(m1.powers.Length, m2.powers.Length);

            if (orderingScheme == "grlex")
            {
                if (m1.powers.Sum() > m2.powers.Sum())
                {
                    return 1;
                }
                else if (m1.powers.Sum() < m2.powers.Sum())
                {
                    return -1;
                }
            }

            // If grlex is set and total degree produces a tie, we can move on to lex ordering.
            for (int i = 0; i < numVariables; i++) 
            {
                if (m1.powers[i] > m2.powers[i])
                {
                    return 1;
                }
                else if (m1.powers[i] < m2.powers[i])
                {
                    return -1;
                }
            }

            return 0;
        }

        /// <summary>
        /// Compares this monomial to another monomial passed as argument. This method is called as the comparator when adding 
        /// monomials to the ordered dictionary behind the polynomial class.
        /// </summary>
        /// <param name="obj">The other monomial to compare to.</param>
        /// <returns>An integer which is 1 if this monomial is bigger, -1 if obj is bigger and 0 if they are the same.</returns>
        int IComparable.CompareTo(object obj)
        {
            Monomial m = (Monomial)obj;
            return Compare(this, m);
        }

        /// <summary>
        /// Determines if this monomial is equal to another monomial passed as argument.
        /// </summary>
        /// <param name="obj">The other monomial with which the comparison is to be made.</param>
        /// <returns>A boolean which is true if this monomial is equal to the one passed in the argument.</returns>
        public override bool Equals(object obj)
        {
            Monomial other = (Monomial)obj;
            int numVariables = Math.Min(this.powers.Length, other.powers.Length);
            for (int i = 0; i < numVariables; i++)
            {
                if (this.powers[i] != other.powers[i])
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            for (int i = 0; i < this.powers.Length; i++)
            {
                hash = hash * 31 + this.powers[i].GetHashCode();
            }

            return hash;
        }

        public static Boolean operator >(Monomial m1, Monomial m2)
        {
            if (m1.Compare(m1, m2) == 1)
            {
                return true;
            }

            return false;
        }

        public static Boolean operator <(Monomial m1, Monomial m2)
        {
            if (m1.Compare(m1, m2) == -1)
            {
                return true;
            }

            return false;
        }

        public static Boolean operator ==(Monomial m1, Monomial m2)
        {
            if (m1.Equals(m2))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// A wrapper for the <see cref="Equals(object)"/> method. 
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <returns></returns>
        public static Boolean operator !=(Monomial m1, Monomial m2)
        {
            if (m1.Equals(m2))
            {
                return false;
            }

            return true;
        }

        public static Monomial operator *(Monomial m1, Monomial m2)
        {
            return m1.Multiply(m2);
        }

        /// <summary>
        /// Checks to see if this polynomial divides the dividend polynomial.
        /// This polynomial plays the role of would be divisor.
        /// </summary>
        /// <param name="divisor">The dividend to be checked for divisibility.</param>
        /// <returns></returns>
        public bool Divides(Monomial dividend)
        {
            // TODO: Handle the case when the number of terms in the two polynomials is different.
            for (int i = 0; i < this.powers.Length; i++) 
            {
                if (dividend.powers[i] < this.powers[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Is this monomial divisible by the monomial in the argument. In other words can we write this = h.divisor?
        /// </summary>
        /// <param name="divisor">The dividing monomial</param>
        /// <returns>true if divisibility is possible</returns>
        public bool IsDividedBy(Monomial divisor)
        {
            // TODO: Handle the case when the number of variables in the two monomials is different.
            for (int i = 0; i < this.powers.Length; i++)
            {
                if (divisor.powers[i] > this.powers[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Divides this monomial with another.
        /// </summary>
        /// <param name="divisor">The divisor</param>
        /// <returns>The quotient from the division.</returns>
        public Monomial DivideBy(Monomial divisor)
        {
            int degree = Math.Min(divisor.powers.Length, this.powers.Length); // Ideally, the degrees of the two should be the same.
            int[] result = new int[degree];
            for (int i = 0; i < degree; i++)
            {
                result[i] = (this.powers[i] - divisor.powers[i]);
                if (result[i] < 0)
                {
                    throw new Exception("Monomial provided as dividend did not divide this monomial");
                }
            }

            return new Monomial(result);
        }

        /// <summary>
        /// Calculates the Least Common Multiple between two monomials.
        /// </summary>
        /// <param name="other">this monomial and other's LCM will be calculated.</param>
        /// <returns>The LCM monomial.</returns>
        public Monomial LCM(Monomial other)
        {
            if (this.powers.Length != other.powers.Length)
            {
                throw new Exception("The two monomials don't have the same variables.");
            }

            int[] result = new int[this.powers.Length];

            for (int i = 0; i < this.powers.Length; i++)
            {
                result[i] = Math.Max(this.powers[i], other.powers[i]); // Assuming here that the polynomials have the same number of variables.
            }

            return new Monomial(result);
        }

        /// <summary>
        /// Multiplies this monomial to the other monomial.
        /// </summary>
        /// <param name="other">The monomial to be multiplied.</param>
        /// <returns>A monomial which is the result of the multiplication of the two monomials.</returns>
        public Monomial Multiply(Monomial other)
        {
            if (this.powers.Length != other.powers.Length)
            {
                throw new Exception("The two monomials don't have the same variables.");
            }

            int[] result = new int[this.powers.Length];

            for (int i = 0; i < this.powers.Length; i++)
            {
                result[i] = this.powers[i] + other.powers[i];
            }

            return new Monomial(result);
        }
    }
}

//[1] Implementing comparators for classes - https://support.microsoft.com/en-us/kb/320727

