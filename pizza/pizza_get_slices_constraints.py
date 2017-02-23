import numpy as np

def get_shapes_for_area(area):

	shapes = set()

	small_side = 1
	while small_side ** 2 <= area:
		if area % small_side == 0:
			shapes.add(tuple([small_side, area / small_side]))
			shapes.add(tuple([area / small_side, small_side]))
		small_side += 1
	return shapes

def get_slices_and_constraints(L, H, pizza):

	possible_areas = range(2 * L, H + 1)
	all_shapes = []
	for area in possible_areas:
		all_shapes += list(get_shapes_for_area(area))

	cells_to_slices = {tuple([x, y]) : [] for x in range(pizza.shape[0]) for y in range(pizza.shape[1])}
	slice_scores = {}
	slice_specs = {}

	slice_id = 0

	for shape in all_shapes:
		print shape, len(slice_scores)
		min_x_index = 0
		min_y_index = 0
		max_x_index = pizza.shape[0] - shape[0]
		max_y_index = pizza.shape[1] - shape[1]

		for x in range(min_x_index, max_x_index + 1):
			for y in range(min_y_index, max_y_index + 1):
				potential_slice = pizza[x:x+shape[0], y:y+shape[1]]
				slice_count = sum(sum(potential_slice))

				if slice_count >= L and slice_count <= shape[0] * shape[1] - L:
					slice_scores[slice_id] = shape[0] * shape[1]
					slice_specs[slice_id] = [x, y, x+shape[0]-1, y+shape[1]-1]
					for x_cell in range(x, x+shape[0]):
						for y_cell in range(y, y+shape[1]):
							cells_to_slices[tuple([x_cell, y_cell])].append(slice_id)

					# print slice_id
					# print potential_slice
					# print slice_count
					# print [x, y, x+shape[0]-1, y+shape[1]-1]
					# print ""
					slice_id += 1

				# print potential_slice, shape, slice_count, [x, y, x+shape[0]-1, y+shape[1]-1]

	# conflict_slices = {x : set() for x in range(slice_id)}
	# for cell in cells_to_slices:
	# 	conflicts = cells_to_slices[cell]
	# 	for x in range(0, len(conflicts) - 1):
	# 		for y in range(x + 1, len(conflicts)):
	# 			conflict_slices[conflicts[x]].add(conflicts[y])
	# 			conflict_slices[conflicts[y]].add(conflicts[x])


	return slice_scores, slice_specs, cells_to_slices

# print get_slices_and_constraints(20, 50, "pizza")
# print get_shapes_for_area(64)