
import random
from parse import *
from pizza_get_slices_constraints import *

def greedy_pizza(fname):

	[L, H, pizza] = parse(fname)
	print L, H, pizza.shape

	[slice_scores, slice_specs, cells_to_slices] = get_slices_and_constraints(L, H, pizza)
	# print cells_to_slices

	best_score = 0
	for i in range(10 ** 3):
		possible_slices = set(slice_scores.keys())
		burned_slices = set()

		current_score = 0
		current_slices = []
		while len(possible_slices) > 0:
			picked_slice = random.sample(possible_slices, 1)[0]
			if picked_slice in burned_slices:
				possible_slices = possible_slices.difference(burned_slices)
				if len(possible_slices) == 0:
					break
				print len(possible_slices)
				picked_slice = random.sample(possible_slices, 1)[0]
				burned_slices = set()

			# print slice_specs[picked_slice], slice_scores[picked_slice]

			[x_min, y_min, x_max, y_max] = slice_specs[picked_slice]
			for x in range(x_min, x_max + 1):
				for y in range(y_min, y_max + 1):
					# possible_slices = possible_slices.difference(set(cells_to_slices[tuple([x, y])]))
					burned_slices = burned_slices.union(set(cells_to_slices[(x, y)]))

			current_score += slice_scores[picked_slice]
			current_slices.append(picked_slice)

		if current_score > best_score:
			best_score = current_score
			# for picked_slice in current_slices:
			# 	print picked_slice, slice_scores[picked_slice], slice_specs[picked_slice]
			print best_score
			print ""

	return best_score


print greedy_pizza("big.in")