using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polynomials
{
    class PolynomialBasis
    {
        public HashSet<Polynomial> polynomialData;

        private Dictionary<char, int> variableIndices;

        private char[] indiceToVariable;

        /// <summary>
        /// Initializes a new instance of class PolynomialBasis
        /// </summary>
        /// <param name="inputPolynomials">List of polynomials that will form the basis of the ideal.</param>
        public PolynomialBasis(List<Polynomial> inputPolynomials)
        {
            this.polynomialData = new HashSet<Polynomial>();

            foreach (Polynomial p in inputPolynomials)
            {
                Polynomial toAdd = new Polynomial(p);
                this.polynomialData.Add(toAdd);
            }
        }

        /// <summary>
        /// Initializes a new instance of class PolynomialBasis
        /// </summary>
        /// <param name="inputPolynomials">An array of input polynomials</param>
        public PolynomialBasis(params Polynomial[] inputPolynomials)
        {
            this.polynomialData = new HashSet<Polynomial>();

            foreach (Polynomial p in inputPolynomials)
            {
                Polynomial toAdd = new Polynomial(p);
                this.polynomialData.Add(toAdd);
            }
        }

        /// <summary>
        /// Initializes a new instance of class <see cref="PolynomialBasis"/>. Creates a deep copy of the input basis in a new instance.
        /// </summary>
        /// <param name="inputBasis">The input basis which is to be deep copied.</param>
        public PolynomialBasis(PolynomialBasis inputBasis)
        {
            this.polynomialData = new HashSet<Polynomial>();

            foreach (Polynomial p in inputBasis.polynomialData)
            {
                Polynomial toAdd = new Polynomial(p);
                this.polynomialData.Add(toAdd);
            }
        }

        /// <summary>
        /// Instantiates an instance of <see cref="PolynomialBasis"/> given some equations in the form of strings.
        /// </summary>
        /// <param name="equations">An array of equations in the form of strings.</param>
        public PolynomialBasis(params string[] equations)
        {
            this.polynomialData = new HashSet<Polynomial>();
            this.variableIndices = StringEquation.GetVariableInfo(equations);
            this.indiceToVariable = new char[this.variableIndices.Count()];
            int j = 0;

            foreach (char c in this.variableIndices.Keys)
            {
                this.indiceToVariable[j++] = c;
            }

            foreach (string eqn in equations)
            {
                this.polynomialData.Add(StringEquation.PolynomialEqn(eqn, this.variableIndices));
            }
        }

        /// <summary>
        /// Checks to see if the two polynomial bases are equal.
        /// </summary>
        /// <param name="obj">An object that is parsed to another polynomial with which equality is to be tested.</param>
        /// <returns>A boolean value specifying if this polynomial basis is equal to the other or not.</returns>
        public override bool Equals(object obj)
        {
            PolynomialBasis other = (PolynomialBasis)obj;

            if (this.polynomialData.Count != other.polynomialData.Count)
            {
                return false;
            }

            foreach (Polynomial p in other.polynomialData)
            {
                if (!this.polynomialData.Contains(p))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Generates hash code for the polynomial object.
        /// </summary>
        /// <returns>An integer hash code for this polynomial basis.</returns>
        public override int GetHashCode()
        {
            int hash = 17;
            foreach (Polynomial p in this.polynomialData)
            {
                hash = hash * 31 + p.GetHashCode();
            }

            return hash;
        }

        /// <summary>
        /// Checks if the basis is zero.
        /// </summary>
        /// <returns>A boolean indicating if the basis is zero.</returns>
        public bool IsZero()
        {
            if (this.polynomialData == null || this.polynomialData.Count == 0)
            {
                return true;
            }

            foreach (Polynomial p in this.polynomialData)
            {
                if (!p.IsZero)
                {
                    return true;
                }
            }

            return true;
        }

        /// <summary>
        /// Determines if the current polynomial basis is a Groebner basis. 
        /// Based on Buchbergers criterion from theorem 6 of section 6, CLO.
        /// The one with the very intimidating proof.
        /// </summary>
        /// <returns>A boolean indicating Groebner or not.</returns>
        public bool IsGroebnerBasis()
        {
            List<Polynomial> groebnerTempList = this.polynomialData.ToList();
            for (int i = 0; i < this.polynomialData.Count; i++)
            {
                for (int j = (i + 1); j < this.polynomialData.Count; j++)
                {
                    Polynomial s = groebnerTempList[i].GetSPolynomial(groebnerTempList[j]).GetRemainder(this);
                    if (!s.IsZero)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Adds the other polynomial to the current one.
        /// </summary>
        /// <param name="p">The polynomial that is to be added to this one.</param>
        public void AddPolynomial(Polynomial p)
        {
            this.polynomialData.Add(p);
        }

        /// <summary>
        /// Computes the Groebner basis for a polynomial ideal using Buchbergers algorithm.
        /// </summary>
        /// <param name="basis">The basis that defines the ideal.</param>
        /// <returns>The Groebner basis.</returns>
        public static PolynomialBasis GetGroebnerBasis(params Polynomial[] basis)
        {
            PolynomialBasis groebner = new PolynomialBasis(basis);
            return groebner.SimplifiedBuchberger();
            //return groebner.OptimizedBuchberger(basis); //TODO: Change to optimized Buchberger.
        }

        /// <summary>
        /// Computes Groebner basis for the polynomial basis. Based on Theorem 2 of sectin 2.7, CLO. Not very efficient.
        /// </summary>
        /// <returns>A polynomial basis that is the reduced Groebner basis.</returns>
        public PolynomialBasis SimplifiedBuchberger()
        {
            PolynomialBasis groebner = this;
            PolynomialBasis groebnerTemp = this;
            do
            {
                groebnerTemp = new PolynomialBasis(groebner); // G' := G

                List<Polynomial> groebnerTempList = groebnerTemp.polynomialData.ToList();
                for (int i = 0; i < groebnerTemp.polynomialData.Count; i++)
                {
                    for (int j = (i + 1); j < groebnerTemp.polynomialData.Count; j++)
                    {
                        Polynomial s = groebnerTempList[i].GetSPolynomial(groebnerTempList[j]).GetRemainder(groebnerTemp);
                        if (!s.IsZero)
                        {
                            groebner.polynomialData.Add(s); // G := G ∪ {S}
                        }
                    }
                }
            }
            while (!groebner.Equals(groebnerTemp));

            groebner.MakeMinimal(); // First minimal and then reduced.
            groebner.MakeReduced();
            return groebner;
        }

        /// <summary>
        /// Optimized Buchberger implementation based on Theorem 11 of section 2.9.
        /// </summary>
        /// <param name="basis">An array of polynomials that are going to form the basis.</param>
        /// <returns>A polynomial basis that wraps the polynomials that make up the Groebner basis.</returns>
        public PolynomialBasis OptimizedBuchberger(params Polynomial[] basis)
        {
            int t = basis.Length - 1;
            HashSet<Tuple<int, int>> b = new HashSet<Tuple<int, int>>();

            for (int i = 0; i <= t; i++)
            {
                for (int j = i+1; j <= t; j++)
                {
                    b.Add(Tuple.Create(i, j)); // i will always be less than j.
                }
            }

            PolynomialBasis groebner = new PolynomialBasis(basis);
            List<Polynomial> listBasis = basis.ToList();

            while (b.Count > 0)
            {
                List<Tuple<int, int>> bList = b.ToList();
                foreach (Tuple<int, int> ij in bList)
                {
                    int i = ij.Item1;
                    int j = ij.Item2;

                    if (listBasis[i].GetLeadingTerm().LCM(listBasis[j].GetLeadingTerm()) != listBasis[i].GetLeadingTerm() * (listBasis[j].GetLeadingTerm())
                        && !this.Criterion(i, j, b, listBasis))
                    {
                        Polynomial s = listBasis[i].GetSPolynomial(listBasis[j]).GetRemainder(new PolynomialBasis(listBasis)); // TODO: Implement remainder method that takes list. It will be more efficient.
                        if (!s.IsZero)
                        {
                            t++;
                            listBasis.Add(s);

                            for (int l = 0; l < t; l++)
                            {
                                b.Add(Tuple.Create(l, t)); // l will always be smaller than t.
                            }
                        }
                    }

                    b.Remove(ij);
                }
            }

            PolynomialBasis grb = new PolynomialBasis(listBasis);
            grb.MakeMinimal(); // First minimal and then reduced.
            grb.MakeReduced();
            return grb;
        }

        /// <summary>
        /// A wrapper for optimized buchberger that can be called through the object of this class
        /// </summary>
        /// <returns>The minimal Gorebner basis</returns>
        public PolynomialBasis OptimizedBuchberger()
        {
            return this.OptimizedBuchberger(this.polynomialData.ToArray());
        }

        /// <summary>
        /// The criterion from section 2.9 of CLO. 
        /// If this is satisfied, S_ij can be written as a combination of S_ik and S_jk
        /// </summary>
        /// <param name="i">ith index in the basis</param>
        /// <param name="j">jth index in the basis</param>
        /// <param name="b">The set of all tuples of integers</param>
        /// <param name="basis">The polynomial basis</param>
        /// <returns>A boolean which is true if S_ij can be expressed in terms of S_ik and S_jk</returns>
        private bool Criterion(int i, int j, HashSet<Tuple<int, int>> b, List<Polynomial> listBasis)
        {
            for (int k = 0; k < listBasis.Count; k++)
            {
                if (!b.Contains(Tuple.Create(Math.Min(i,k), Math.Max(i,k))) && !b.Contains(Tuple.Create(Math.Min(j,k), Math.Max(j,k))) 
                    && listBasis[k].GetLeadingTerm().Divides(listBasis[i].GetLeadingTerm().LCM(listBasis[j].GetLeadingTerm())) 
                    && k != i && k != j)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Assumes current basis is already a Groebner basis and converts the basis into a minimal one. 
        /// Makes leading coefficients 1 and utilizes lemma 3 of chapter 2 (CLO) to eliminate polynomials 
        /// that are not essential to the basis.
        /// (i) LC(p) = 1
        /// (ii) For all p \in G, no monomial of p lies in <LT(G-{p})>
        /// To be run after the Buchberger algorithm to remove excess terms.
        /// </summary>
        public void MakeMinimal()
        {
            foreach (Polynomial p in this.polynomialData)
            {
                p.MakeLeadingTermCoefficientOne();
            }

            HashSet<Polynomial> result = new HashSet<Polynomial>();
            List<Polynomial> tempList = this.polynomialData.ToList();
            bool[] excludeIndex = new bool[tempList.Count]; // Don't consider polynomials that have already been removed.

            for (int i = 0; i < tempList.Count; i++)
            {
                bool uniqueLeadingTerm = true;
                for (int j = 0; j < tempList.Count; j++)
                {
                    if (j == i || excludeIndex[j])
                    {
                        continue;
                    }

                    if (tempList[i].GetLeadingTerm().IsDividedBy(tempList[j].GetLeadingTerm()))
                    {
                        uniqueLeadingTerm = false;
                        excludeIndex[i] = true;
                        break;
                    }
                }

                if (uniqueLeadingTerm)
                {
                    result.Add(tempList[i]);
                }
            }

            this.polynomialData = result;
        }

        /// <summary>
        /// Assumes that this current basis is already minimal and converts it to reduced (from Definition 5 of section 2.7, CLO).
        /// CAUTION - if basis is not already minimal, a non- descriptive exception will be thrown from somewhere in the monomial code.
        /// Based on algorithm 2 from - https://www.kent.ac.uk/smsas/personal/gdb/MA574/week6.pdf
        /// </summary>
        public void MakeReduced()
        {
            var tempPolynomials = this.polynomialData.ToList();
            Polynomial hi = polynomialData.ElementAt(0);
            Polynomial gi;
            tempPolynomials.RemoveAt(0);

            for (int i = 0; i < tempPolynomials.Count; i++)
            {
                gi = hi.GetRemainder(tempPolynomials);
                hi = tempPolynomials[i];
                tempPolynomials[i] = gi;
            }

            gi = hi.GetRemainder(tempPolynomials);
            tempPolynomials.Add(gi);
            this.polynomialData = new HashSet<Polynomial>(tempPolynomials);
        }

        /// <summary>
        /// Prints the Groebner basis to console. If the mapping from indices to variable names is present (see private field), 
        /// it will print in human readable format. Otherwise, it will print monomials as integer arrays.
        /// </summary>
        public void PrettyPrint()
        {
            int i = 0, j = 0;
            foreach (Polynomial p in this.polynomialData)
            {
                System.Console.WriteLine("Polynomial - " + i++.ToString());
                foreach (Monomial m in p.monomialData.Keys)
                {
                    if (this.indiceToVariable == null)
                    {
                        System.Console.WriteLine(string.Join(",", m.powers) + " * " + p.monomialData[m].ToString() + " + ");
                    }
                    else
                    {
                        System.Console.Write
                        (
                            (
                                p.monomialData[m] > 0 ? " + " +
                                    (p.monomialData[m] == 1 ? " " : p.monomialData[m].ToString())    // Don't print +1, just +
                                :   (p.monomialData[m] == -1 ? " - " : p.monomialData[m].ToString()) // Don't print -1, just -
                            )
                        );

                        for (j = 0; j < m.powers.Length; j++)
                        {
                            if (m.powers[j] > 1)
                            {
                                System.Console.Write(this.indiceToVariable[j] + "^" + m.powers[j]);
                            }
                            else if (m.powers[j] == 1)
                            {
                                System.Console.Write(this.indiceToVariable[j]);
                            }
                        }
                    }
                }

                System.Console.WriteLine();
            }
        }
    }
}
