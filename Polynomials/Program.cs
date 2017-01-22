using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polynomials
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine("##Univariate polynomial division##");
            TestOneVariablePolynomialDivision();

            System.Console.WriteLine("##Multivariate polynomial division##");
            TestMultiVariatePolynomialDivision();

            System.Console.WriteLine("##S-polynomials##");
            TestSPolynomial();

            System.Console.WriteLine("##Groebner basis##");
            TestGroebnerBasisCLOExample1Secn2Pt7();

            System.Console.Read();
        }

        public static void TestOneVariablePolynomialDivision()
        {
            OneVariablePolynomial dividend = new OneVariablePolynomial(new double[] { 1, 1, 2, 1 });
            OneVariablePolynomial divisor = new OneVariablePolynomial(new double[] { 1, 2 });
            Dictionary<string, OneVariablePolynomial> result = dividend.Divide(divisor);
            System.Console.WriteLine(string.Join(",", result["quotient"].coefficients));
        }

        public static void TestMultiVariatePolynomialDivision()
        {
            Monomial m1 = new Monomial(new int[] { 3, 0, 0 }); Monomial m2 = new Monomial(new int[] { 2, 1, 0 }); Monomial m3 = new Monomial(new int[] { 2, 0, 1 }); Monomial m4 = new Monomial(new int[] { 1, 0, 0 });
            Polynomial dividend = new Polynomial(m1);
            dividend.AddMonomial(m2, -1);
            dividend.AddMonomial(m3, -1);
            dividend.AddMonomial(m4);

            dividend = new Polynomial(new Monomial(new int[] { 1, 0, 0 }));
            dividend.AddMonomial(new Monomial(new int[] { 0, 0, 1 }), -1);

            Polynomial divisor1 = new Polynomial(new Monomial(new int[] { 2, 1, 0 })); // (y^2 - 1)
            divisor1.AddMonomial(new Monomial(new int[] { 0, 0, 1 }), -1);

            Polynomial divisor2 = new Polynomial(new Monomial(new int[] { 1, 1, 0 })); // (xy - 1)
            divisor2.AddMonomial(new Monomial(new int[] { 0, 0, 0 }), -1);

            List<Polynomial> quotients = dividend.DivideBy(divisor1, divisor2);

            foreach (Monomial m in quotients.LastOrDefault().monomialData.Keys)
            {
                System.Console.WriteLine(string.Join(",", m.powers) + " coefficient: " + quotients.LastOrDefault().monomialData[m].ToString());
            }
        }

        public static void TestSPolynomial()
        {
            Polynomial f = new Polynomial(new Monomial(new int[] { 3, 2}), 1);
            f.AddMonomial(new Monomial(new int[] { 2, 3 }), -1);
            f.AddMonomial(new Monomial(new int[] { 1, 0 }), 1);

            Polynomial g = new Polynomial(new Monomial(new int[] { 4 , 1}), 3);
            g.AddMonomial(new Monomial(new int[] { 0, 2 }), 1);

            Polynomial s = f.GetSPolynomial(g);

            foreach (Monomial m in s.monomialData.Keys)
            {
                System.Console.WriteLine(string.Join(",", m.powers));
            }
        }

        /// <summary>
        /// Test method for Groebner basis.
        /// </summary>
        public static void TestGroebnerBasis()
        {
            Monomial.orderingScheme = "lex";

            //1st order
            var del_x = 1.0;
            var del_y = 2.0;

            //2nd order
            var del_x_x = 3.5;
            var del_x_y = 1.5;
            var del_y_y = 2.7;

            //3rd order
            var del_y_y_y = 1.1;
            var del_x_y_y = 1.9;
            var del_x_x_y = 2.3;
            var del_x_x_x = 1.4;

            Polynomial f = new Polynomial(new Monomial(new int[] { 2, 0 }), del_x_x_x);
            f.AddMonomial(new Monomial(new int[] { 1, 1 }), 2* del_x_x_y);
            f.AddMonomial(new Monomial(new int[] { 0, 2 }), del_x_y_y);
            f.AddMonomial(new Monomial(new int[] { 1, 0 }), del_x_y);
            f.AddMonomial(new Monomial(new int[] { 0, 1 }), del_y_y);
            f.AddMonomial(new Monomial(new int[] { 0, 0 }), -del_x);

            Polynomial g = new Polynomial(new Monomial(new int[] { 2, 0 }), del_x_x_y);
            g.AddMonomial(new Monomial(new int[] { 1, 1 }), 2 * del_x_y_y);
            g.AddMonomial(new Monomial(new int[] { 0, 2 }), del_y_y_y);
            g.AddMonomial(new Monomial(new int[] { 1, 0 }), del_x_x);
            g.AddMonomial(new Monomial(new int[] { 0, 1 }), del_x_y);
            g.AddMonomial(new Monomial(new int[] { 0, 0 }), -del_y);

            PolynomialBasis gb = PolynomialBasis.GroebnerBasis(f, g);

            PrintPolynomialBasis(gb);
        }

        public static void TestGroebnerBasisCLOExample1Secn2Pt7()
        {
            Monomial.orderingScheme = "grlex";
            Polynomial f = new Polynomial(new Monomial(new int[] { 3, 0 })); // x^3
            f.AddMonomial(new Monomial(new int[] { 1, 1 }), -2.0); // -2xy

            Polynomial g = new Polynomial(new Monomial(new int[] { 2, 1 })); // x^2y
            g.AddMonomial(new Monomial(new int[] { 0, 2 }), -2.0); // -2y^2
            g.AddMonomial(new Monomial(new int[] { 1, 0})); // x

            PolynomialBasis gb = PolynomialBasis.GroebnerBasis(f, g);

            PrintPolynomialBasis(gb);
        }

        public static void TestGroebnerBasisLinearSystem()
        {
            Monomial.orderingScheme = "lex";
            Polynomial f = new Polynomial(new Monomial(new int[] { 1, 0 }));
            f.AddMonomial(new Monomial(new int[] { 0, 1 }), 2);
            f.AddMonomial(new Monomial(new int[] { 0, 0 }), -3);

            Polynomial g = new Polynomial(new Monomial(new int[] { 1, 0 }));
            g.AddMonomial(new Monomial(new int[] { 0, 1 }), -2);
            g.AddMonomial(new Monomial(new int[] { 0, 0 }), -7);

            PolynomialBasis gb = PolynomialBasis.GroebnerBasis(f, g);
            PrintPolynomialBasis(gb);
        }

        public static void TestGroebnerBasisCLOSec2Pt7Exerc3()
        {
            Monomial.orderingScheme = "lex";
            Polynomial f = new Polynomial(new Monomial(new int[] { 2, 1 }));
            f.AddMonomial(new Monomial(new int[] { 0, 0 }), -1);

            Polynomial g = new Polynomial(new Monomial(new int[] { 1, 2 }));
            g.AddMonomial(new Monomial(new int[] { 1, 0 }), -1);

            PolynomialBasis gb = PolynomialBasis.GroebnerBasis(f, g);
            PrintPolynomialBasis(gb);
        }

        public static void PrintPolynomialBasis(PolynomialBasis gb)
        {
            int i = 0;
            foreach (Polynomial p in gb.polynomialData)
            {
                System.Console.WriteLine("Polynomial - " + i++.ToString());
                foreach (Monomial m in p.monomialData.Keys)
                {
                    System.Console.WriteLine(string.Join(",", m.powers) + " * " + p.monomialData[m].ToString() + " + ");
                }
            }
        }
    }
}
