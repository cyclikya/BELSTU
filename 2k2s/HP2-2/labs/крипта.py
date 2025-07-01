from sympy import symbols, Poly
from sympy.polys.domains import GF

x = symbols('x')
modulus = 2
field = GF(modulus)

def reduce_poly(poly):
    while poly.degree() >= 5:
        degree = poly.degree()
        coeff = poly.coeff_monomial(x**degree)
        
        # Создаём полиномы в GF(2)
        term = Poly(x**degree, x, domain=field)
        replacement = Poly((x**2 + 1) * x**(degree - 5), x, domain=field)
        
        poly = poly - coeff * (term - replacement)  # Убрали .trunc()
    return poly

poly = Poly(x**2 + 1, x, domain=field)
step = 5
seen = set()

while True:
    print(f"Step {step}: {poly.as_expr()}")
    if poly.as_expr() == 1:
        print("Успех: Единица получена!")
        break
    
    poly = reduce_poly(poly * Poly(x, domain=field))
    step += 1

    key = tuple(poly.all_coeffs())
    if key in seen:
        print("Ошибка: Обнаружен цикл!")
        break
    seen.add(key)
