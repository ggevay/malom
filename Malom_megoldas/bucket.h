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

#include "sector.h"
#include "auto_grow_vector.h"


#define SZ(a) int((a).size())


//A nagy bucket fajlok kozott lehetnek lyukak (amit a big_bucket_reader elabsztrahal olvasaskor), a kicsik kozott nem.

#pragma pack(push)
#pragma pack(1)
struct small_bucket_elem{
	short_id sid;
	int hash;

	small_bucket_elem() {}
	small_bucket_elem(short_id sid, int hash) :sid{ sid }, hash{ hash } {}

	static const small_bucket_elem spec; //ez jelzi, ha kiurult a bucket

	bool operator==(const small_bucket_elem &o) const { return sid == o.sid && hash == o.hash; }
	bool operator!=(const small_bucket_elem &o) const { return !((*this) == o); }
};

struct big_bucket_elem : small_bucket_elem {
	int key2;

	big_bucket_elem();
	big_bucket_elem(short_id id, int hash, int key2);

	static const big_bucket_elem spec; //ez jelzi, ha kiurult a bucket

	bool operator==(const big_bucket_elem &o) const { return sid == o.sid && hash == o.hash && key2 == o.key2; }
	bool operator!=(const big_bucket_elem &o) const { return !((*this) == o); }
};
#pragma pack(pop)


class big_bucket_writer{
	const static int bufsize = 256 * 1024;
	char *buf;
public:
	FILE *f;
	sec_val key1;
private:
	void init(); //ez azert van kulon, mert csak akkor akarjuk letrehozni a fajlt, ha tenyleg irunk bele
public:
	//big_bucket_writer(); //sets f to null
	big_bucket_writer(sec_val key1);
	void push(big_bucket_elem e);
	void close();
};


class small_bucket_writer{
	const static int bufsize = 256 * 1024;
	char *buf;
	FILE *f;
public:
	int key2;

	small_bucket_writer(int key2);
	void push(small_bucket_elem e);
	void close();
};

class small_bucket_reader{
	const static int bufsize = 1024*1024;
	char *buf;
	string bucket_name;
	FILE *f;

public:
	bool exists;
	small_bucket_reader(); //(leaves it undefined)
	small_bucket_reader(int key2);
	small_bucket_elem pop();
	void close();
};



class big_bucket_reader {
	int key2;
	small_bucket_reader sbr;
	bool empty;

public:
	big_bucket_reader(big_bucket_writer &bbw);
	big_bucket_elem pop();
	//(nem kell close)
};



struct queue_elem{
	short_id sid;
	int hash;
	val val;

	static const queue_elem max; //ezt akkor kapjuk, ha mar ures a queue (strazsa elem)
	bool operator==(const queue_elem &o) const { return sid == o.sid && hash == o.hash && val == o.val; }
};


//writes big buckets
class bucket_writer_mgr{
public:
	onion_vector<big_bucket_writer> buckets;

	bucket_writer_mgr();

	void push(queue_elem &e);
	void close();
};

//reads big buckets
class bucket_reader_mgr{ //(az adattagok sorrendje fontos)
	onion_vector<big_bucket_writer>::iterator bbw_it;
	big_bucket_reader bbr;
public:
	bucket_reader_mgr();
	queue_elem pop();
	~bucket_reader_mgr();
};