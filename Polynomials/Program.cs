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
            System.Console.WriteLine("\n## Univariate polynomial division ##");
            TestOneVariablePolynomialDivision();

            System.Console.WriteLine("\n## Multivariate polynomial division ##");
            TestMultiVariatePolynomialDivision();

            System.Console.WriteLine("\n## S-polynomials ##");
            TestSPolynomial();

            System.Console.WriteLine("\n## LinearSystem ##");
            TestGroebnerBasisLinearSystem();
            
            System.Console.WriteLine("\n## CLO Section 2.7 example 1 ##");
            TestGroebnerBasisCLOSecn2Pt7Example1();

            System.Console.WriteLine("\n## CLO Section 2.7 exercise 9 ##");
            CLOSec2Pt7Exerc9();

            System.Console.WriteLine("\n## CLO Section 2.7 exercise 2 (a) ##");
            CLOSecn2Pt7Exerc2a();

            System.Console.WriteLine("\n## CLO Section 2.8 exercise 11 ##");
            CLOSectn2Pt8Exerc11();

            System.Console.WriteLine("\n## Parsing polynomials from a string ##");
            TestString2GroebnerBasis();

            System.Console.WriteLine("\n## Testing optimized buchberger ##");
            TestOptimizedBuchberger();

            System.Console.ReadKey();
        }

        public static void TestOptimizedBuchberger()
        {
            Monomial.orderingScheme = "grlex";
            PolynomialBasis pb = new PolynomialBasis
            (
                "x^3 - 2xy",
                "x^2y - 2y^2 + x"
            );

            PolynomialBasis gb = pb.OptimizedBuchberger(); // since gb doesn't have pb's indiceToVariable, we will get old fashioned printing.
            gb.PrettyPrint();
        }

        public static void TestString2GroebnerBasis()
        {
            Monomial.orderingScheme = "grlex";

            PolynomialBasis pb = new PolynomialBasis
            (
                "x^3 - 2xy",
                "x^2y - 2y^2 + x"
            );
                        
            pb.SimplifiedBuchberger();
            pb.PrettyPrint();
        }

        public static void CLOSectn2Pt8Exerc11()
        {
            Monomial.orderingScheme = "lex";
            Polynomial p1 = new Polynomial(new double[] { 1, 1, 1, -3 }, new Monomial(1, 0, 0), new Monomial(0, 1, 0), new Monomial(0, 0, 1), new Monomial(0, 0, 0));
            Polynomial p2 = new Polynomial(new double[] { 1, 1, 1, -5 }, new Monomial(2, 0, 0), new Monomial(0, 2, 0), new Monomial(0, 0, 2), new Monomial(0, 0, 0));
            Polynomial p3 = new Polynomial(new double[] { 1, 1, 1, -7 }, new Monomial(3, 0, 0), new Monomial(0, 3, 0), new Monomial(0, 0, 3), new Monomial(0, 0, 0));
            Polynomial p4 = new Polynomial(new double[] { 1, 1, 1, -9 }, new Monomial(4, 0, 0), new Monomial(0, 4, 0), new Monomial(0, 0, 4), new Monomial(0, 0, 0));
            PolynomialBasis gb = PolynomialBasis.GetGroebnerBasis(p1, p2, p3, p4);
            PrintPolynomialBasis(gb);
        }

        public static void CLOSecn2Pt7Exerc2b()
        {
            Monomial.orderingScheme = "lex";
            Polynomial p1 = new Polynomial(new double[] { 1, 1 }, new Monomial(2, 0), new Monomial(0, 1));
            Polynomial p2 = new Polynomial(new double[] { 1, 2 , 1, 3}, new Monomial(4, 0), new Monomial(2, 1), new Monomial(0, 2), new Monomial(0, 0));
            PolynomialBasis gb = PolynomialBasis.GetGroebnerBasis(p1, p2);
            PrintPolynomialBasis(gb);
        }

        public static void CLOSecn2Pt7Exerc2a()
        {
            Monomial.orderingScheme = "lex";
            Polynomial p1 = new Polynomial(new double[] { 1, -1 }, new Monomial(2, 1), new Monomial(0, 0));
            Polynomial p2 = new Polynomial(new double[] { 1, -1 }, new Monomial(1, 2), new Monomial(1, 0));
            PolynomialBasis gb = PolynomialBasis.GetGroebnerBasis(p1, p2);
            PrintPolynomialBasis(gb);
        }

        public static void CheckMonomialOperators()
        {
            Monomial m1 = new Monomial(3, 0, 0);
            Monomial m2 = new Monomial(1, 2, 3);
            Monomial.orderingScheme = "grlex";
            if (m1 > m2)
            {
                Console.Out.WriteLine("Monomial operators seem to be working!");
            }
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

            PolynomialBasis gb = PolynomialBasis.GetGroebnerBasis(f, g);
            
            PrintPolynomialBasis(gb);
        }

        public static void TestGroebnerBasisCLOSecn2Pt7Example1()
        {
            Monomial.orderingScheme = "grlex";
            Polynomial f = new Polynomial(new Monomial(new int[] { 3, 0 })); // x^3
            f.AddMonomial(new Monomial(new int[] { 1, 1 }), -2.0); // -2xy

            Polynomial g = new Polynomial(new Monomial(new int[] { 2, 1 })); // x^2y
            g.AddMonomial(new Monomial(new int[] { 0, 2 }), -2.0); // -2y^2
            g.AddMonomial(new Monomial(new int[] { 1, 0})); // x

            PolynomialBasis gb = PolynomialBasis.GetGroebnerBasis(f, g);

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

            PolynomialBasis gb = PolynomialBasis.GetGroebnerBasis(f, g);
            PrintPolynomialBasis(gb);
        }

        public static void TestGroebnerBasisCLOSec2Pt7Exerc3()
        {
            Monomial.orderingScheme = "lex";
            Polynomial f = new Polynomial(new Monomial(new int[] { 2, 1 }));
            f.AddMonomial(new Monomial(new int[] { 0, 0 }), -1);

            Polynomial g = new Polynomial(new Monomial(new int[] { 1, 2 }));
            g.AddMonomial(new Monomial(new int[] { 1, 0 }), -1);

            PolynomialBasis gb = PolynomialBasis.GetGroebnerBasis(f, g);
            PrintPolynomialBasis(gb);
        }


        public static void CLOSec2Pt7Exerc9()
        {
            Monomial.orderingScheme = "lex";
            Polynomial p1 = new Polynomial(new double[] { 3, -6, -2}, new Monomial(1, 0, 0, 0), new Monomial(0, 1, 0, 0), new Monomial(0, 0, 1, 0));
            Polynomial p2 = new Polynomial(new double[] { 2, -4, 4 }, new Monomial(1, 0, 0, 0), new Monomial(0, 1, 0, 0), new Monomial(0, 0, 0, 1));
            Polynomial p3 = new Polynomial(new double[] { 1, -2, -1, -1 }, new Monomial(1, 0, 0, 0), new Monomial(0, 1, 0, 0), new Monomial(0, 0, 1, 0), new Monomial(0, 0, 0, 1));

            PolynomialBasis gb = PolynomialBasis.GetGroebnerBasis(p1, p2, p3);
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
