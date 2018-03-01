
import random
from common import read_file

def random_order(rides):
	random.shuffle(rides)
	return rides

# test = read_file("../dat/a_example.in")
# print test