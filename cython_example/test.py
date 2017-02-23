from __future__ import (print_function, division, unicode_literals,
                        absolute_import)

import numpy as np

import cy_example_cy
import cy_example_py

a_cy = cy_example_cy.naive_convolve(np.array([[1, 1, 1]], dtype=np.int), np.array([[1],[2],[1]], dtype=np.int))
a_py = cy_example_py.naive_convolve(np.array([[1, 1, 1]], dtype=np.int), np.array([[1],[2],[1]], dtype=np.int))

print('Cython function returns:')
print(a_cy)

print('Python function returns:')
print(a_py)
