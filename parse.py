import random
import numpy as np


def parse(fname):
    with open(fname) as f:
        R, C, L, H = [int(c) for c in f.readline().split()]
        board = np.empty([R, C], dtype=np.int)
        for c, line in enumerate(f):
            for r, char in enumerate(line):
                if char not in ('M', 'T'):
                    continue
                board[c, r] = (char == 'T')
    return L, H, board



def generate(seed, maxRows, maxCols, maxL, maxH):
    random.seed(seed)

    rows = random.randint(1, maxRows)
    cols = random.randint(1, maxCols)
    L = random.randint(1, maxL)
    H = random.randint(1, maxH)

    mushrooms = random.randint(1, rows * cols)

    board = np.zeros((rows, cols), np.int)
    for m in range(mushrooms):
        while True:
            r, c = random.randint(0, rows - 1), random.randint(0, cols - 1)
            if board[r, c] == 0:
                board[r, c] = 1
                break

    return L, H, board


def get_valids(L, H, board):
    R, C = board.shape
    for c_1 in range(C):
        for c_2 in range(c_1 + 1, C):
            for r_1 in range(R):
                for r_2 in range(r_1, R):
                    slc = board[c_1:c_2 + 1, r_1:r_2 + 1]
                    nr_T = (slc == 1).sum()
                    nr_M = (slc == 0).sum()
                    if slc.size < H and min(nr_T, nr_M) > L:
                        valids.append([c_1, c_2, r_1, r_2])
    return valids
# print(generate(123, 5, 5, 3, 6));
# print(parse('test.txt'))
