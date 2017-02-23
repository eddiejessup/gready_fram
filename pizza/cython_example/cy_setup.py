from distutils.core import setup

from Cython.Build import cythonize

import numpy as np

setup(
    ext_modules=cythonize("cy_example_cy.pyx"),
    include_dirs=[np.get_include()]
)