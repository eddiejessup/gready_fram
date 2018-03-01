def dist(c1, c2):
    a, b = c1
    x, y = c2
    return abs(a - x) + abs(b - y)

def read_file(filename):
    with open(filename) as fin:
        header = next(fin)
        R, C, F, N, B, T = map(int, header.split())
        rides = []
        for i in range(N):
            a, b, x, y, s, f = map(int, next(fin).split())
            start = (a, b)
            end = (x, y)
            time = (s, f)
            ride_dist = dist(start, end)
            rides.append((start, end, time, ride_dist, i))
        return {
            'R': R,
            'C': C,
            'F': F,
            'N': N,
            'B': B,
            'T': T,
            'rides': rides
        }

# task has the format returned by read_file
# solution is array of F arrays
def calc_score(task, solution):
    bonus = task['B']
    rides = task['rides']
    score = 0
    for car_rides in solution:
        car = (0, 0)
        t = 0
        for ride in car_rides:
            start, end, time, ride_dist, ride_i = rides[ride]

            car_to_start = dist(car, start)

            if t + car_to_start > time[1] - ride_dist:
                continue
            if t + car_to_start <= time[0]:
                score += bonus

            score += ride_dist

            t = max(t + car_to_start, time[0]) + ride_dist
            car = end

    return score

if __name__ == '__main__':
    task = read_file('sample.input')
    print(task)

    solution = [[0], [2, 1]]
    print(calc_score(task, solution))
