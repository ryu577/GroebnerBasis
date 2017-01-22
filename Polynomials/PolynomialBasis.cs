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
        /// <param name="inputPolynomials"></param>
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
        public PolynomialBasis(Polynomial[] inputPolynomials)
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
        /// <param name="obj"></param>
        /// <returns>A boolean value specifying if this polynomial is equal to the other or not.</returns>
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
        /// <returns></returns>
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
        /// Computes the Groebner basis for a polynomial ideal using Buchbergers algorithm.
        /// </summary>
        /// <param name="basis">The basis that defines the ideal.</param>
        /// <returns>The Groebner basis.</returns>
        public static PolynomialBasis GroebnerBasis(params Polynomial[] basis)
        {
            PolynomialBasis groebner = new PolynomialBasis(basis);
            return groebner.SimplifiedGroebnerBasis();
        }

        /// <summary>
        /// Computes Groebner basis for the polynomial basis. Based on Theorem 2 of sectin 2.7, CLO. Not very efficient.
        /// </summary>
        /// <returns>A polynomial basis that is the reduced Groebner basis.</returns>
        public PolynomialBasis SimplifiedGroebnerBasis()
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

            groebner.MakeMinimal();
            return groebner;
        }

        /// <summary>
        /// Implementation incomplete.
        /// </summary>
        /// <param name="basis"></param>
        /// <returns></returns>
        public static PolynomialBasis GrobnerBasisOptimized(params Polynomial[] basis)
        {
            int t = basis.Length;
            List<Tuple<int, int>> b = new List<Tuple<int, int>>();

            for (int i = 0; i < t; i++)
            {
                for (int j = i; j < t; j++)
                {
                    b.Add(Tuple.Create(i, j));
                }
            }

            PolynomialBasis groebner = new PolynomialBasis(basis);

            while (b.Count > 0)
            {
                int i = b[0].Item1;
                int j = b[0].Item2;

                /*
                if (basis[i].GetLeadingTerm().LCM(basis[j].GetLeadingTerm()).Equals(basis[i].GetLeadingTerm()) && !criterion()) // TODO: Implement multiply monomials.
                {

                }*/
            }

            return groebner;
        }

        /// <summary>
        /// Converts the basis into a minimal one. Makes leading coefficients 1
        /// and utilizes lemma 3 of chapter 2 (CLO) to eliminate polynomials that are not
        /// essential to the basis.
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
                    if (p.GetLeadingTerm() != p1.GetLeadingTerm() && p.GetLeadingTerm().IsDividedBy(p1.GetLeadingTerm()))
                    { // If the leading term is divisible by the leading term of any other polynomial in the basis, we can get rid of it.
                        reducedPolynomials.Remove(p);
                    }
                }
            }

            this.polynomialData = reducedPolynomials;
        }
    }
}
