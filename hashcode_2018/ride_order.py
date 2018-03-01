
import random
from common import read_file
from common import dist

def random_order(rides):
	random.shuffle(rides)
	return rides

def ontime_adjacency_check(first_ride, second_ride):
	return first_ride[2][0] + first_ride[3] + dist(first_ride[1], second_ride[0]) <= second_ride[2][0]

def lasttime_adjacency_check(first_ride, second_ride):
	return first_ride[2][1] + first_ride[3] + dist(first_ride[1], second_ride[0]) <= second_ride[2][1]

def optimal_ontime_rides(rides, B):
	# Rides is an array of ride tuples
	# A single ride looks like 
	# ((0, 0), (1, 3), (2, 9), 4)
	# (start, end, (min_start_time, max_end_time), dist)
	rides.sort(key=lambda x: x[2][0])

	rides = [((0, 0), (0, 0), (0, 0), 0, -1)] + rides

	parent_rides = {}
	for i in range(1, len(rides)):
		parent_rides[rides[i]] = []
		for j in range(0, i):
			if ontime_adjacency_check(rides[j], rides[i]):
				parent_rides[rides[i]].append(rides[j])

	# Showing optimal scores and the associated parents
	optimal_scores = {rides[0] : (0, None)}
	for ride in rides[1:]:
		optimal_ride_score = None
		optimal_parent = None
		for parent in parent_rides[ride]:
			if optimal_scores[parent][0] is not None:
				candidate_ride_score = optimal_scores[parent][0] + B + ride[3]
				if candidate_ride_score > optimal_ride_score:
					optimal_ride_score = candidate_ride_score
					optimal_parent = parent
		optimal_scores[ride] = (optimal_ride_score, optimal_parent)

	# Reconstructing optimal path
	optimal_end = max(optimal_scores, key=lambda x: optimal_scores[x][0])
	final_score = optimal_scores[optimal_end][0]

	optimal_path = [optimal_end]
	next_step = optimal_scores[optimal_path[-1]][1]
	while next_step is not None:
		optimal_path.append(next_step)
		next_step = optimal_scores[optimal_path[-1]][1]

	# Getting rid of zero entry point and reversing the path
	optimal_path = optimal_path[:-1]
	optimal_path.reverse()
	for ride in rides[1:]:
		if ride not in optimal_path:
			optimal_path.append(ride)
	return optimal_path, final_score

def optimal_lasttime_rides(rides, B):
	# Rides is an array of ride tuples
	# A single ride looks like 
	# ((0, 0), (1, 3), (2, 9), 4)
	# (start, end, (min_start_time, max_end_time), dist)
	rides.sort(key=lambda x: x[2][0])

	rides = [((0, 0), (0, 0), (0, 0), 0)] + rides

	parent_rides = {}
	for i in range(1, len(rides)):
		parent_rides[rides[i]] = []
		for j in range(0, i):
			if lasttime_adjacency_check(rides[j], rides[i]):
				parent_rides[rides[i]].append(rides[j])

	# Showing optimal scores and the associated parents
	optimal_scores = {rides[0] : (0, None)}
	for ride in rides[1:]:
		optimal_ride_score = None
		optimal_parent = None
		for parent in parent_rides[ride]:
			if optimal_scores[parent][0] is not None:
				candidate_ride_score = optimal_scores[parent][0] + ride[3]
				if candidate_ride_score > optimal_ride_score:
					optimal_ride_score = candidate_ride_score
					optimal_parent = parent
		optimal_scores[ride] = (optimal_ride_score, optimal_parent)

	# Reconstructing optimal path
	optimal_end = max(optimal_scores, key=lambda x: optimal_scores[x][0])
	final_score = optimal_scores[optimal_end][0]

	optimal_path = [optimal_end]
	next_step = optimal_scores[optimal_path[-1]][1]
	while next_step is not None:
		optimal_path.append(next_step)
		next_step = optimal_scores[optimal_path[-1]][1]

	# Getting rid of zero entry point and reversing the path
	optimal_path = optimal_path[:-1]
	optimal_path.reverse()
	for ride in rides[1:]:
		if ride not in optimal_path:
			optimal_path.append(ride)
	return optimal_path, final_score

if __name__ == '__main__':
    task = read_file('../dat/b_should_be_easy.in')
    rides = task["rides"]
    B = task["B"]
    print(task)
    print optimal_ontime_rides(rides, B)

    # print 5 + None > None
