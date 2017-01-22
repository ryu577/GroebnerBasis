using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polynomials
{
    class OneVariableMonomial
    {
        public int degree;
        public double coefficient;

        public OneVariableMonomial(int degree, double coefficient = 1)
        {
            this.coefficient = coefficient;
            this.degree = degree;
        }

        public OneVariableMonomial Divide(OneVariableMonomial otherMonomial)
        {
            if (this.degree < otherMonomial.degree)
            {
                throw new Exception("Division is not closed under the field of monomials. To yield another monomial, degree of numerator must be greater!");
            }
            else
            {
                return new OneVariableMonomial(this.degree - otherMonomial.degree, this.coefficient / otherMonomial.coefficient);
            }
        }
    }
}
