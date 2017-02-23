from __future__ import (print_function, division, unicode_literals,
                        absolute_import)
import random
import numpy as np


# L: minimum number of ingredients in a slice.
# H: maximum number of cells in a slice.


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


def to_mask(board, c_1, c_2, r_1, r_2):
    return board[c_1:c_2 + 1, r_1:r_2 + 1]

def get_valids(L, H, board):
    R, C = board.shape
    valids = []
    for c_1 in range(C):
        print(c_1)
        for c_2 in range(c_1, C):
            min_size = 1 + c_2 - c_1
            if min_size > H:
                # print('bad size already')
                break
            for r_1 in range(R):
                for r_2 in range(r_1, R):
                    slice_size = get_slice_size(c_1, c_2, r_1, r_2)
                    # If slice is too big, it will only get bigger, so give up.
                    if slice_size > H:
                        # print('bad size')
                        break
                    # If slice is too small to hold both ingredients, give up
                    # for now.
                    if slice_size < 2 * L:
                        continue
                    # print('good size')
                    slc = to_mask(board, c_1, c_2, r_1, r_2)
                    nr_T = (slc == 1).sum()
                    nr_M = slice_size - nr_T
                    if min(nr_T, nr_M) > L:
                        # print('good ingredients')
                        valids.append([c_1, c_2, r_1, r_2])
                    else:
                        pass
                        # print('bad ingredients')
    return valids


def get_slice_size(c_1, c_2, r_1, r_2):
    return (1 + c_2 - c_1) * (1 + r_2 - r_1)


def get_slice_size_lst(coords):
    return get_slice_size(*coords)


def overlaps(coords_A, coords_B):
    c_1_A, c_2_A, r_1_A, r_2_A = coords_A
    c_1_B, c_2_B, r_1_B, r_2_B = coords_B
    return (c_1_A < c_2_B and c_2_A > c_2_B and
            r_2_A < r_1_B and r_1_A > r_2_B) 


def overlaps_set(coords_lst, coords):
    return any(overlaps(coords_, coords) for coords_ in coords_lst)


def get_greedy_solution(valids):
    soln_coords_lst = [valids.pop()]
    while True:
        for i, candidate_coords in enumerate(valids):
            if not overlaps_set(soln_coords_lst, candidate_coords):
                soln_coords_lst.append(valids.pop(i))
                break
        else:
            break
    return soln_coords_lst


def print_board(board):
    for r, row in enumerate(board):
        for c, val in enumerate(row):
            print(val, end='    ')
        print()


# def is_valid(soln_coords_lst, board):



L, H, board = parse('medium.in')
valids = get_valids(L, H, board)
print(len(valids))
print(get_greedy_solution(valids))
# print_board(board)
