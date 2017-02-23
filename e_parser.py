from __future__ import (print_function, division, unicode_literals, absolute_import)


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
            end = {'ds_delay': ds_delay, 'conns': conns}
            ends.append(end)

        reqs = []
        for i_req in range(nr_reqs):
            vid_id, end_id, nr_reqs = [int(s) for s in f.readline().split()]
            reqs.append({'vid_id': vid_id, 'end_id': end_id, 'weight': nr_reqs})

    return nr_vids, nr_caches, cache_size, vid_sizes, ends, reqs


fname = 'dat/me_at_the_zoo.in'
nr_vids, nr_caches, cache_size, vid_sizes, ends, reqs = parser(fname)

reqs_sort = reversed(sorted(reqs, key=lambda r: r['weight']))


def cache_used(cache):
    return sum(v[1] for v in cache)


def cache_vid_ids(cache):
    return [v[0] for v in cache]


total_weight = sum(req['weight'] for req in reqs)
nr_fails = 0
nr_fails_max = 100

caches = [[] for _ in range(nr_caches)]
for i_req, req in enumerate(reqs_sort):
    print('on request ', i_req, ' of ', len(reqs))
    end_id = req['end_id']
    vid_id = req['vid_id']
    vid_size = vid_sizes[vid_id]
    end = ends[end_id]
    conns = end['conns']
    conns_short = sorted(conns, key=lambda c: c['delay'])
    for conn in conns_short:
        cache_id = conn['cache_id']
        cache = caches[cache_id]
        cache_free = cache_size - cache_used(cache)
        if vid_id not in cache_vid_ids(cache) and cache_free > vid_size:
            cache.append((vid_id, vid_size))
            break
    else:
        nr_fails += 1
        print('couldnt cache requests vid nooo')
        if nr_fails > nr_fails_max:
            break

for c in caches:
    print('size: ', cache_used(c))

def write_soln(caches, file_name):
    with open(file_name, 'w') as f:
        f.write(str(len(caches)) + '\n')
        for i, cache in enumerate(caches):
            # f.write(i)
            ints = [i] + [v[0] for v in cache]
            s = ' '.join([str(v) for v in ints])
            # print()
            f.write(s + '\n')


write_soln(caches, fname[:-3] + '.out')

