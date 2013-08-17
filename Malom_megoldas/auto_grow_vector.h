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


#include "stdafx.h"

template<class T>
class auto_grow_vector : public vector<T> {
	function<T(int)> f;
public:
	auto_grow_vector(const function<T(int)> &f) : f(f) {}
	auto_grow_vector() : f([](int){return T(); }) {}

	T& operator[](int i) {
		while(i >= (int)size())
			push_back(f((int)size()));
		return vector<T>::operator[](i);
	}
};


template<class T>
class pn_vector {
	auto_grow_vector<T> pos, neg;
public:
	pn_vector(const function<T(int)> &f) : pos(f), neg([f](int i){return f(-i - 1); }) {}
	pn_vector() : pn_vector([](int){ return T(); }) {}

	T& operator[](int i) {
		if(i >= 0)
			return pos[i];
		else
			return neg[-i - 1];
	}

	//This is just a forward_iterator, but writing a random_access_iterator should also be straightforward.
	class iterator : public std::iterator<forward_iterator_tag, T> {
		pn_vector<T> *cont;
		int i;
	public:
		iterator(pn_vector<T> &cont, int i) : cont(&cont), i(i) {}
		T& operator*(){
			return (*cont)[i];
		}
		T* operator->(){
			return &(*cont)[i];
		}
		bool operator==(const iterator &o) const { return cont == o.cont && i == o.i; }
		bool operator!=(const iterator &o) const { return !((*this) == o); }
		iterator operator++(int){
			iterator tmp = *this;
			i++;
			return tmp;
		}
		iterator& operator++() {
			this->i++;
			return *this;
		}

		int ind() {
			return i;
		}
	};

	iterator begin() {
		return iterator(*this, -(int)neg.size());
	}
	iterator end() {
		return iterator(*this, (int)pos.size());
	}

	size_t size(){
		return neg.size() + pos.size();
	}

	vector<T> to_vector(){
		return vector<T>(this->begin(), this->end());
	}
};


//Olyan, mint a pn_vector, de
//-mast csinal a default konstruktor
//-szinkronban tartja a ket veget 
//-a konstruktoraban rogton letrehozza a 0 indexu elemet
//-mas sorrendben sorol fel (ez a lenyeg)
//-a vege a 0
template<class T>
class onion_vector {
	auto_grow_vector<T> pos, neg;
public:
	T& operator[](int i) {
		if(i >= 0) {
			if(i != 0)
				neg[i - 1]; //to keep the two ends in sync
			return pos[i];
		} else {
			pos[-i]; //to keep the two ends in sync
			return neg[-i - 1];
		}
	}

	onion_vector(const function<T(int)> &f) : pos(f), neg([f](int i){return f(-i - 1); }) {
		operator[](0);
	}
	onion_vector() : onion_vector([](int i){ return T(i); }) {}

	/*bool has(int i){
	return i < pos.size() && i >= -(int)neg.size();
	}*/

	class iterator : public std::iterator<forward_iterator_tag, T> {
		onion_vector<T> *cont;
		int i;
	public:
		iterator(onion_vector<T> &cont, int i) : cont{ &cont }, i{ i } {}
		T& operator*(){
			//assert(!at_end());
			//No problem if we are at the end: we have a dummy elem there (so bbr ctor won't have any problem), and brm.pop() won't read from it, because it checks for at_end.
			return (*cont)[i];
		}
		T* operator->(){
			assert(!at_end());
			return &(*cont)[i];
		}
		bool operator==(const iterator &o) const { return cont == o.cont && i == o.i; }
		bool operator!=(const iterator &o) const { return !((*this) == o); }
		iterator& operator++() {
			assert(!at_end());
			if(i < 0)
				this->i = -this->i - 1;
			else
				this->i = -this->i;
			return *this;
		}
		bool at_end() { return i == 0; }
	};

	iterator begin() {
		return iterator{ *this, (int)pos.size() - 1 };
	}
	iterator end() {
		return iterator{ *this, 0 };
	}
};
