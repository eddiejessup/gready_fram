import collections
import math
import numpy as np
import heapq
import time
from common import *
from ride_order import *

MOVE_CARS_COUNT = 2
RESORT_CARS_COUNT = 2

class PriorityQueue:
    def __init__(self):
        self.elements = []

    def empty(self):
        return len(self.elements) == 0

    def put(self, item, priority):
        heapq.heappush(self.elements, (priority, item))

    def get(self):
        return heapq.heappop(self.elements)

    def squeeze(self, n_remaining):
        self.elements[ : n_remaining]

def get_score_and_sort_roman(task, candidate):
    B = task['B']
    task_rides = task['rides']

    candidate_cost = 0.0
    candidate_sorted = []
    for rides_i in candidate:
        rides = [task_rides[i] for i in rides_i]
        rides_sorted, c = optimal_ontime_rides(rides, B)
        candidate_cost += c
        candidate_sorted.append([arr[4] for arr in rides_sorted])
    return candidate_sorted, candidate_cost

def get_score_and_sort(task, candidate):
    B = task['B']
    task_rides = task['rides']

    candidate_sorted = []
    for rides_i in candidate:
        rides = [task_rides[i] for i in rides_i]
        rides_sorted = rides
        # rides_sorted = sorted(rides, key=lambda x: -math.pow(x[3], 0.1) + math.pow(x[2][0] + 1, 0.8))
        # rides_sorted, c = optimal_ontime_rides(rides, B)
        candidate_sorted.append([arr[4] for arr in rides_sorted])
    candidate_cost = calc_score(task, candidate_sorted)
    return candidate_sorted, candidate_cost

def astar(task, start, n_iterations, branch_factor, n_frontier):
    start_sorted, start_score = get_score_and_sort(task, start)

    frontier = PriorityQueue()
    frontier.put(start_sorted, -start_score)

    for iter in xrange(n_iterations):
        current_score, current = frontier.get()

        cnt = 0
        for candidate in get_neighbors(task, current):
            candidate_sorted, candidate_cost = get_score_and_sort(task, candidate)
            frontier.put(candidate_sorted, -candidate_cost)

            cnt += 1
            if cnt == branch_factor:
                break

        frontier.squeeze(n_frontier)

        if ((iter + 1) % (n_iterations / 10)) == 0:
            print(iter + 1, frontier.get()[0])

    return frontier.get()

def get_neighbors(task, solution):
    F = task['F']
    N = task['N']

    indexes = np.arange(F)

    while True:
        candidate = solution[:]

        for car_i in range(MOVE_CARS_COUNT):
            from_i = np.random.choice(indexes)
            to_i = np.random.choice(indexes)

            if len(candidate[from_i]) == 0:
                break

            car = np.random.choice(candidate[from_i])
            candidate[from_i].remove(car)
            candidate[to_i].append(car)

        for car_i in range(RESORT_CARS_COUNT):
            arr_i = np.random.choice(indexes)
            arr = candidate[arr_i]
            if len(arr) == 0:
                continue
            i = np.random.randint(0, len(arr))
            j = np.random.randint(0, len(arr))
            arr[i], arr[j] = arr[j], arr[i]

        yield candidate

def get_neighbors_old(task, solution):
    F = task['F']
    N = task['N']

    indexes = np.arange(F)

    while True:
        probs_from = np.zeros(F)
        for i, rides in enumerate(solution):
            probs_from[i] = len(rides)
        probs_from = probs_from / probs_from.sum()

        probs_to = np.zeros(F)
        for i, rides in enumerate(solution):
            probs_to[i] = N - len(rides)
        probs_to = probs_to / probs_to.sum()

        candidate = solution[:]

        for car_i in range(MOVE_CARS_COUNT):
            from_i = np.random.choice(indexes, p=probs_from)
            to_i = np.random.choice(indexes, p=probs_to)

            if len(candidate[from_i]) == 0:
                break

            car = np.random.choice(candidate[from_i])
            candidate[from_i].remove(car)
            candidate[to_i].append(car)
        yield candidate

def get_random_start(task):
    F = task['F']
    N = task['N']

    rides = task['rides'][:]

    cars = [(0, 0) for _ in range(F)]

    result = [[] for i in range(F)]
    while rides
        best_car_i = 0
        best_car_dist = 10000
        best_ride = None

        for cari, car in enumerate(cars):
            for ride in rides:
                start, end, time, ride_dist, i = ride
                if dist(car, start) < best_car_dist:
                    best_car_i = cari
                    best_car_dist = dist(car, start)
                    best_ride = ride

        car_dist[best_car_i] = cars[]


    return result

def get_random_start_old(task):
    F = task['F']
    N = task['N']
    distr = np.zeros(F, dtype=np.int8)
    for i in range(F + 1):
        distr[np.random.randint(F)] += 1

    result = []
    perm = range(N)
    np.random.shuffle(perm)

    i = 0
    for j in distr:
        result.append(perm[i: (i + j)])
        i += j
    result.append

    return result

if __name__ == '__main__':

    # filename = 'a_example'
    # filename = 'b_should_be_easy'
    # filename = 'c_no_hurry'
    filename = 'd_metropolis'
    # filename = 'e_high_bonus'

    # np.random.seed(42)

    task = read_file('../dat/%s.in' % filename)
    # print('Task:', task)

    start = get_random_start(task)
    # print('Start:', start)

    score, sol = astar(
        task,
        start,
        n_iterations = 5000,
        branch_factor = 2,
        n_frontier = 100)
    score = -score

    print('Score:', score)

    with open('../dat/%s.%i.out' % (filename, score), 'w') as outfile:
        for i in range(task['F']):
            rides = sol[i]
            line = [len(rides)]
            line += rides
            outfile.write(' '.join(map(str, line)) + '\n')