Buchbergers algorithm for computing Groebner Basis

Polynomial bases are a generalization of systems of linear equations. In <a href="http://mathworld.wolfram.com/GaussianElimination.html">Gaussian elimination</a>, we take a system of linear equations and convert them to a basis where the solution becomes obvious.

A framework is provided for defining polynomial ideals. The various objects that are a part of this solution form a hierarchical structure:

PolynomialBasis -> Polynomial -> Monomial

The code starts with the building blocks like LCM, polynomial division, S-polynomials and builds up to Groebner basis. Some applications 
of Groebner basis are also provided. 

You can execute the code via Program.cs. 

All algorithms are based on the book - <a href="http://www.dm.unipi.it/~caboara/Misc/Cox,%20Little,%20O'Shea%20-%20Ideals,%20varieties%20and%20algorithms.pdf">Ideals, Varieties and Algorithms by Cox, Little and O'Shea</a> 

In comments and documentation, "CLO" referes to this book (Cox, Little and O'Shea). Here is a code snippet for the simplified Buchbergers algorithm -  

<script src="https://gist.github.com/ryu577/f6969ce0dfe43515e2849df1b881cd71.js"></script>

