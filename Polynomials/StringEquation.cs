using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polynomials
{
    static class StringEquation
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="eqn">
        ///     A string that specifies the polynomial equation. For example: 2 x^2 y + 3 y^3 - 7
        ///     Since I don't want to get into the business of robust string parsnig, this input should conform to strict restrictions - 
        ///     1) eqn can't be null or empty.
        ///     2) Variable names should be single characters (x, y, z, a, b, c, ...).
        ///     3) Coefficient of first polynomial should be positive. If that is not the case, simply multiply the polynomial with -1.
        ///     4) The powers in the polynomials should be positive integers.
        /// </param>
        public static Polynomial PolynomialEqn(string eqn, Dictionary<char, int> variableIndicesOverride = null)
        {
            if (String.IsNullOrEmpty(eqn))
            {
                throw new Exception("You can't pass a null or empty string to the polynomial equation parser.");
            }

            int variableCount = 0;
            int numMonomials = 1;            
            Dictionary<char, int> variablesIndices = new Dictionary<char, int>();
            List<bool> monomialSigns = new List<bool>(); // true means positive and false means negative.
            monomialSigns.Add(true); // First sign should always be positive. If this is not so, multiply the monomial with -1.

            foreach (char c in eqn)
            {
                if (Char.IsLetter(c) && !variablesIndices.ContainsKey(c))
                {
                    variablesIndices[c] = variableCount++;
                }
                else if (c == '+')
                {
                    numMonomials++;
                    monomialSigns.Add(true);
                }
                else if (c == '-')
                {
                    numMonomials++;
                    monomialSigns.Add(false);
                }
            }

            if (variableIndicesOverride != null)
            {
                variablesIndices = variableIndicesOverride;
                variableCount = variableIndicesOverride.Count();
            }

            Monomial[] monomArray = new Monomial[numMonomials];
            double[] coeffs = new double[numMonomials];

            string[] terms = eqn.Split(new char[] { '+', '-' }, StringSplitOptions.None);
            int j = 0;

            foreach (string term in terms)
            {
                coeffs[j] = (monomialSigns[j] ? 1.0 : -1.0) * ParseMonomialCoefficient(term, variablesIndices.Keys.ToArray());
                monomArray[j] = ParseMonomial(term, variablesIndices);
                j++;
            }

            return new Polynomial(coeffs, monomArray);
            //return new Polynomial(new double[] { 1, 1, 1, -3 }, new Monomial[] { new Monomial(1, 0, 0), new Monomial(0, 1, 0), new Monomial(0, 0, 1), new Monomial(0, 0, 0) });
        }

        public static Dictionary<char, int> GetVariableInfo(string[] eqns)
        {
            int variableCount = 0;
            Dictionary<char, int> variablesIndices = new Dictionary<char, int>();
            
            foreach (string eqn in eqns)
            {
                foreach (char c in eqn)
                {
                    if (Char.IsLetter(c) && !variablesIndices.ContainsKey(c))
                    {
                        variablesIndices[c] = variableCount++;
                    }
                }
            }

            return variablesIndices;
        }

        private static double ParseMonomialCoefficient(string input, char[] variableNames)
        {
            try
            {
                return Double.Parse(input.Split(variableNames, StringSplitOptions.None)[0]);
            }
            catch (Exception e)
            {
                return 1.0;
            }   
        }

        public static Monomial ParseMonomial(string input, Dictionary<char, int> variableIndices)
        {
            int[] powers = new int[variableIndices.Count];
            int[] scrambledPowers = ParsePowers(input, variableIndices);
            int j = 0;
            foreach (char c in input)
            {
                if (variableIndices.ContainsKey(c))
                {
                    powers[variableIndices[c]] = scrambledPowers[j++];
                }
            }

            return new Monomial(powers);
        }

        public static int[] ParsePowers(string input, Dictionary<char, int> variableIndices)
        {
            int[] powers = new int[variableIndices.Count()];
            int j = 0;

            foreach (char c in input)
            {
                if (variableIndices.ContainsKey(c))
                {
                    powers[j++] = 1;
                }
            }

            j = 0;
            foreach (string term in input.Split(variableIndices.Keys.ToArray(), StringSplitOptions.None))
            {
                powers[j] = 1;
                if (!String.IsNullOrEmpty(term) && term[0] == '^')
                {
                    string numeric = term.Trim(new char[] { '^' });
                    powers[j] = Int32.Parse(numeric);
                    j++;
                }
            }

            return powers;
        }
    }
}
