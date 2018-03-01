import collections
import numpy as np
import heapq
from common import *
from ride_order import *

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

def get_score_and_sort(task, candidate):
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

    return frontier.get()

def get_neighbors(task, solution):
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

        from_i = np.random.choice(indexes, p=probs_from)
        to_i = np.random.choice(indexes, p=probs_to)

        candidate = solution[:]
        car = np.random.choice(candidate[from_i])
        candidate[from_i].remove(car)
        candidate[to_i].append(car)
        yield candidate

def get_random_start(task):
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

    filename = 'a_example'
    # filename = 'b_should_be_easy'

    # np.random.seed(42)

    task = read_file('../dat/%s.in' % filename)
    # print('Task:', task)

    start = get_random_start(task)
    # print('Start:', start)

    score, sol = astar(
        task,
        start,
        n_iterations = 1000,
        branch_factor = 3,
        n_frontier = 10)

    print('Score:', score)

    with open('../dat/%s.out' % filename, 'w') as outfile:
        for i in range(task['F']):
            rides = sol[i]
            line = [len(rides)]
            line += rides
            outfile.write(' '.join(map(str, line)) + '\n')