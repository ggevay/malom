/*
Malom, a Nine Men's Morris (and variants) player and solver program.
Copyright(C) 2007-2016  Gabor E. Gevay, Gabor Danner

See our webpage (and the paper linked from there):
http://compalg.inf.elte.hu/~ggevay/mills/index.php


This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/


#pragma once

template<class T>
class biglinked_queue {
	static const int link_size = 1024 * 1024;
	struct link {
		array<T, link_size> a;
		int start, end;
		link* next;
		link() : start{ 0 }, end{ 0 }, next{ nullptr }{}
	};
	link *first, *last;
public:
	using value_type = T;
	using size_type = size_t;
	using reference = T&;
	using const_reference = const T&;

	biglinked_queue() : first{ new link{} }, last{ first } {}

	void push_back(const T &x){
		last->a[last->end++] = x;
		if(last->end == link_size)
			last = last->next = new link{};
	}

	bool empty() const {
		return first == last && first->start == first->end;
	}

	void pop_front(){
		assert(!empty());
		if(++first->start == link_size){
			link *newfirst = first->next;
			delete first;
			first = newfirst;
		}
	}

	T& front(){
		assert(!empty());
		return first->a[first->start];
	}
};