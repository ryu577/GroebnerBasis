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

        /// <summary>
        /// Initializes a new instance of class PolynomialBasis
        /// </summary>
        /// <param name="inputPolynomials">List of polynomials that will form the basis of the ideal.</param>
        public PolynomialBasis(List<Polynomial> inputPolynomials)
        {
            polynomialData = new HashSet<Polynomial>();

            foreach (Polynomial p in inputPolynomials)
            {
                Polynomial toAdd = new Polynomial(p);
                polynomialData.Add(toAdd);
            }
        }

        /// <summary>
        /// Initializes a new instance of class PolynomialBasis
        /// </summary>
        /// <param name="inputPolynomials">An array of input polynomials</param>
        public PolynomialBasis(params Polynomial[] inputPolynomials)
        {
            polynomialData = new HashSet<Polynomial>();

            foreach (Polynomial p in inputPolynomials)
            {
                Polynomial toAdd = new Polynomial(p);
                polynomialData.Add(toAdd);
            }
        }

        /// <summary>
        /// Initializes a new instance of class PolynomialBasis. Creates a deep copy of the input basis in a new instance.
        /// </summary>
        /// <param name="inputBasis">The input basis which is to be deep copied.</param>
        public PolynomialBasis(PolynomialBasis inputBasis)
        {
            polynomialData = new HashSet<Polynomial>();

            foreach (Polynomial p in inputBasis.polynomialData)
            {
                Polynomial toAdd = new Polynomial(p);
                polynomialData.Add(toAdd);
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
        /// Determinees if the current polynomial basis is a Groebner basis. 
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
        /// Implementation incomplete.
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
                Tuple<int, int> ij = b.First();
                int i = ij.Item1;
                int j = ij.Item2;

                if (listBasis[i].GetLeadingTerm().LCM(listBasis[j].GetLeadingTerm()) != listBasis[i].GetLeadingTerm().Multiply(listBasis[j].GetLeadingTerm())
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

            return new PolynomialBasis(listBasis);
        }

        /// <summary>
        /// The criterion from section 2.9 of CLO. 
        /// TODO: Details pending a reading of the section.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="b"></param>
        /// <param name="basis"></param>
        /// <returns>A boolean which is true if the condition is satisfied.</returns>
        private bool Criterion(int i, int j, HashSet<Tuple<int, int>> b, List<Polynomial> listBasis)
        {
            for (int k = 0; k < listBasis.Count; k++)
            {                
                if (!b.Contains(Tuple.Create(i,k)) && !b.Contains(Tuple.Create(j,k)) 
                    && listBasis[i].GetLeadingTerm().LCM(listBasis[j].GetLeadingTerm()).Divides(listBasis[k].GetLeadingTerm()))
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
            var reducedPolynomials = this.polynomialData;
            // Now go through and make coefficients of all leading terms 1.
            foreach (Polynomial p in this.polynomialData)
            {
                p.MakeLeadingTermCoefficientOne();
            }

            foreach (Polynomial p in this.polynomialData.ToList())
            {
                foreach (Polynomial p1 in this.polynomialData.ToList())
                {
                    if (p != p1 && p.GetLeadingTerm().IsDividedBy(p1.GetLeadingTerm()))
                    { // If the leading term is divisible by the leading term of any other polynomial in the basis, we can get rid of it.
                        reducedPolynomials.Remove(p);
                    }
                }
            }

            this.polynomialData = reducedPolynomials;
        }

        /// <summary>
        /// Assumes that this current basis is already minimal and converts it to reduced.
        /// based on algorithm 2 from - https://www.kent.ac.uk/smsas/personal/gdb/MA574/week6.pdf
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
    }
}
