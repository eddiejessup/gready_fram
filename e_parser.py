from __future__ import (print_function, division, unicode_literals, absolute_import)

import numpy as np


def parser(file_name):
    with open(file_name) as f:
        nr_vids, nr_ends, nr_reqs, nr_caches, cache_size = [int(s) for s in f.readline().split()]
        print('nr_vids: ', nr_vids)
        print('nr_ends: ', nr_ends)
        print('nr_reqs: ', nr_reqs)
        print('nr_caches: ', nr_caches)
        print('cache_size: ', cache_size)
        vid_sizes = [int(s) for s in f.readline().split()]
        print('mean video size: ', sum(vid_sizes) / float(len(vid_sizes)))

        ends = []
        for i_end in range(nr_ends):
            ds_delay, nr_conns = [int(s) for s in f.readline().split()]
            conns = []
            for i_conn in range(nr_conns):
                cache_id, conn_delay = [int(s) for s in f.readline().split()]
                conns.append({'cache_id': cache_id, 'delay': conn_delay})
            conns.sort(key=lambda c: c['delay'])
            end = {'ds_delay': ds_delay, 'conns': conns}
            ends.append(end)

        reqs = []
        for i_req in range(nr_reqs):
            vid_id, end_id, weight = [int(s) for s in f.readline().split()]
            reqs.append({'vid_id': vid_id, 'end_id': end_id, 'weight': weight})
        vid_ids = set(r['vid_id'] for r in reqs)
        assert len(ends) == nr_ends
        assert len(reqs) == nr_reqs
        assert max(vid_ids) < nr_vids

    return nr_vids, nr_caches, cache_size, vid_sizes, ends, reqs


def cache_used(cache):
    return sum(v[1] for v in cache)


def cache_vid_ids(cache):
    return [v[0] for v in cache]



def solve(nr_vids, nr_caches, cache_size, vid_sizes, ends, reqs,
          randomize=False, seed=1):
    reqs_sort = reversed(sorted(reqs, key=lambda r: r['weight']))

    rng = np.random.RandomState(seed=seed)

    mean_vid_size = sum(vid_sizes) / float(len(vid_sizes))

    total_weight = sum(req['weight'] for req in reqs)
    nr_fails = 0
    nr_fails_max = 200

    caches = [[] for _ in range(nr_caches)]
    for i_req, req in enumerate(reqs_sort):
        # print('on request ', i_req, ' of ', len(reqs))
        end_id = req['end_id']
        vid_id = req['vid_id']
        vid_size = vid_sizes[vid_id]
        end = ends[end_id]
        conns = end['conns']
        if randomize:
            rng.shuffle(conns)
        for conn in conns:
            cache_id = conn['cache_id']
            cache = caches[cache_id]
            cache_free = cache_size - cache_used(cache)
            if vid_id not in cache_vid_ids(cache) and cache_free > vid_size:
                cache.append((vid_id, vid_size))
                break
                # if vid_size < mean_vid_size:
                #     cache.append((vid_id, vid_size))
                #     break
                # elif rng.rand() < 0.8:
                #     cache.append((vid_id, vid_size))
                #     break                    
        else:
            nr_fails += 1
            # print('couldnt cache requests vid nooo')
            if nr_fails > nr_fails_max:
                break
    return caches


def write_soln(caches, file_name):
    with open(file_name, 'w') as f:
        f.write(str(len(caches)) + '\n')
        for i, cache in enumerate(caches):
            # f.write(i)
            ints = [i] + [v[0] for v in cache]
            s = ' '.join([str(v) for v in ints])
            # print()
            f.write(s + '\n')


def score_soln(caches):
    saved_delays = []
    end_hits = {}
    end_reqs = {}
    total_weight = 0
    for req in reqs:
        end_id = req['end_id']
        end = ends[end_id]
        vid_id = req['vid_id']
        ds_delay = end['ds_delay']
        min_delay = ds_delay
        weight = req['weight']

        total_weight += weight

        conns = end['conns']
        hit = False
        for conn in conns:
            cache_id = conn['cache_id']
            cache = caches[cache_id]
            if vid_id in cache_vid_ids(cache):
                hit = True
                cache_delay = conn['delay']
                min_delay = min(cache_delay, min_delay)

        if hit:
            if end_id not in end_hits:
                end_hits[end_id] = 0
            end_hits[end_id] += 1
        if end_id not in end_reqs:
            end_reqs[end_id] = 0
        end_reqs[end_id] += 1

        saved_delay = weight * (ds_delay - min_delay)
        saved_delays.append(saved_delay)
    mean_saved_delay = (1000 * sum(saved_delays)) / total_weight
    return mean_saved_delay, end_hits, end_reqs


fname = 'dat/me_at_the_zoo.in'
fname = 'dat/kittens.in'
nr_vids, nr_caches, cache_size, vid_sizes, ends, reqs = parser(fname)

caches = solve(nr_vids, nr_caches, cache_size, vid_sizes, ends, reqs, randomize=False)

# score_max = 0
# seed_max = -1
# caches_best = None
# for seed in range(1):
#     caches = solve(nr_vids, nr_caches, cache_size, vid_sizes, ends, reqs,
#                    randomize=True, seed=seed)
#     score = score_soln(caches)
#     if score > score_max:
#         score_max = score
#         seed_max = seed
#         caches_best = caches
#     # print(seed, score)

# print(seed_max, score_max)
# write_soln(caches_best, fname[:-3] + '.out')

# print()
# print(seed_max, score_max)
# for c in caches:
#     print('size: ', cache_used(c))
# s, h, r = score_soln(caches)


# print(s)
# for end_id in h:
#     hit_rate = h[end_id] / r[end_id]
#     print(end_id, hit_rate)
# print(h)
# print(r)
