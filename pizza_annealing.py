
import random
import numpy as np
from parse import *
from pizza_get_slices_constraints import *

random.seed(123456)

def get_score(picked_slices, slice_scores, cells_to_slices, overlap_penalty):

	score = 0
	for x in picked_slices:
		score += slice_scores[x]

	for cell in cells_to_slices:
		slices_to_check = cells_to_slices[cell]
		for x in slices_to_check:
			if x in picked_slices:
				for y in slices_to_check:
					if y > x and y in picked_slices:
						score -= overlap_penalty

	return score

def add_slice(picked_slices, possible_slices, slice_scores, slice_specs, cells_to_slices, overlap_penalty):
	if len(picked_slices) == len(possible_slices):
		return [], [], 0

	new_slice = random.sample(possible_slices, 1)[0]
	while new_slice in picked_slices:
		new_slice = random.sample(possible_slices, 1)[0]

	delta_score = slice_scores[new_slice]

	[x_min, y_min, x_max, y_max] = slice_specs[new_slice]
	for x in range(x_min, x_max + 1):
		for y in range(y_min, y_max + 1):
			cell = (x, y)
			for other_slice in cells_to_slices[cell]:
				if other_slice in picked_slices:
					delta_score -= overlap_penalty

	return [new_slice], [], delta_score

def drop_slice(picked_slices, possible_slices, slice_scores, slice_specs, cells_to_slices, overlap_penalty):
	if len(picked_slices) == 0:
		return [], [], 0

	dropped_slice = random.sample(picked_slices, 1)[0]

	delta_score = -slice_scores[dropped_slice]

	[x_min, y_min, x_max, y_max] = slice_specs[dropped_slice]

	for x in range(x_min, x_max + 1):
		for y in range(y_min, y_max + 1):
			cell = (x, y)
			for other_slice in cells_to_slices[cell]:
				if other_slice != dropped_slice:
					if other_slice in picked_slices:
						delta_score += overlap_penalty

	return [], [dropped_slice], delta_score	

def replace_slice(picked_slices, possible_slices, slice_scores, slice_specs, cells_to_slices, overlap_penalty):
	if len(picked_slices) == 0:
		return [], [], 0

	if len(picked_slices) == len(possible_slices):
		return [], [], 0

	new_slice = random.sample(possible_slices, 1)[0]
	while new_slice in picked_slices:
		new_slice = random.sample(possible_slices, 1)[0]

	dropped_slice = random.sample(picked_slices, 1)[0]

	delta_score = -slice_scores[dropped_slice]
	[x_min, y_min, x_max, y_max] = slice_specs[dropped_slice]
	for x in range(x_min, x_max + 1):
		for y in range(y_min, y_max + 1):
			cell = (x, y)
			for other_slice in cells_to_slices[cell]:
				if other_slice != dropped_slice:
					if other_slice in picked_slices:
						delta_score += overlap_penalty

	delta_score += slice_scores[new_slice]
	[x_min, y_min, x_max, y_max] = slice_specs[new_slice]
	for x in range(x_min, x_max + 1):
		for y in range(y_min, y_max + 1):
			cell = (x, y)
			for other_slice in cells_to_slices[cell]:
				if other_slice != dropped_slice:
					if other_slice in picked_slices:
						delta_score -= overlap_penalty

	return [new_slice], [dropped_slice], delta_score

def pizza_annealing(fname):

	overlap_penalty = 10
	start_temp = 100
	end_temp = 0.01
	cooldown = 0.99999

	[L, H, pizza] = parse(fname)
	print L, H, pizza.shape

	[slice_scores, slice_specs, cells_to_slices] = get_slices_and_constraints(L, H, pizza)

	possible_slices = set(slice_scores.keys())
	possible_moves = [add_slice, drop_slice, replace_slice]

	# current_slices = set(random.sample(possible_slices, int(0.25 * len(possible_slices))))
	current_slices = set()
	current_score = get_score(current_slices, slice_scores, cells_to_slices, overlap_penalty)

	current_temp = start_temp
	counter = 0
	while current_temp > end_temp:
		counter += 1
		if counter % 10 ** 3 == 0:
			print current_temp, current_score

		candidate_move = random.choice(possible_moves)
		[added_slices, dropped_slices, delta_score] = candidate_move(current_slices, possible_slices, slice_scores, slice_specs, cells_to_slices, overlap_penalty)

		move_approved = False
		if delta_score / current_temp > np.log(random.random()):
			move_approved = True

		if move_approved == True:
			for element in dropped_slices:
				current_slices.remove(element)
			for element in added_slices:
				current_slices.add(element)
			current_score += delta_score

		# if current_score != get_score(current_slices, slice_scores, cells_to_slices, overlap_penalty):
		# 	print candidate_move, added_slices, dropped_slices, delta_score
		# 	print current_slices
		# 	for x in current_slices:
		# 		print x, slice_specs[x]
		# 	print slice_specs[0]
		# 	break

		current_temp *= cooldown

	print current_score
	print get_score(current_slices, slice_scores, cells_to_slices, overlap_penalty)

	for cell in cells_to_slices:
		cell_used = False
		for element in cells_to_slices[cell]:
			if element in current_slices:
				if cell_used == True:
					current_slices.remove(element)
				if cell_used == False:
					cell_used = True
	final_score = get_score(current_slices, slice_scores, cells_to_slices, overlap_penalty)

	return final_score

print pizza_annealing("medium.in")