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

#include "common.h"
#include "bucket.h"


extern id run_params;
string bucket_prefix(){ //a processzhez tartozo bucketek mappajanak neve
	return run_params.to_string();
}


const small_bucket_elem small_bucket_elem::spec = small_bucket_elem{ -1, 0 };
const big_bucket_elem big_bucket_elem::spec = big_bucket_elem{ -1, 0, 0 };

big_bucket_elem::big_bucket_elem() {}
big_bucket_elem::big_bucket_elem(short_id sid, int hash, int key2) : small_bucket_elem{ sid, hash }, key2{ key2 } {}


void big_bucket_writer::init(){
	fopen_s(&f, ("buckets\\" + bucket_prefix() + "\\" + tostring(key1) + ".bigbucket").c_str(), "wb");
	buf = new char[bufsize];
	setvbuf(f, buf, _IOFBF, bufsize);
}

big_bucket_writer::big_bucket_writer(sec_val key1) : key1{ key1 }, f{ 0 } {}

//big_bucket_writer::big_bucket_writer() : f{ nullptr } {}



small_bucket_writer::small_bucket_writer(int key2) : key2{ key2 } {
	errno_t err = fopen_s(&f, ("buckets\\" + bucket_prefix() + "\\small_buckets\\" + tostring(key2) + ".smallbucket").c_str(), "wb");
	if(err){
		LOG("!!! error: a %d small_bucket letrehozasa nem sikerult. (%s)", key2, ("buckets\\" + bucket_prefix() + "\\small_buckets\\" + tostring(key2) + ".smallbucket").c_str());
		perror("Hiba: ");
	}
	buf = new char[bufsize];
	setvbuf(f, buf, _IOFBF, bufsize);
}

void small_bucket_writer::push(small_bucket_elem e){
	assert(e.hash >= 0 && e.sid >= (short_id)(char)0);
	//fwrite(&e, sizeof(e),1,f); //igy a paddeles miatt 8 byte-ot irna ki
	
	/*_fwrite_nolock(&e.sid, sizeof(e.sid), 1, f);
	_fwrite_nolock(&e.hash, sizeof(e.hash), 1, f);*/
	_fwrite_nolock(&e, sizeof(e), 1, f);
}

void big_bucket_writer::push(big_bucket_elem e){
	assert(e.hash >= 0 && e.sid >= (short_id)(char)0);

	if(!f)
		init();

	//fwrite(&e, sizeof(e),1,f); //igy a paddeles miatt esetleg tobb byte-ot irna ki

	/*_fwrite_nolock(&e.sid, sizeof(e.sid), 1, f);
	_fwrite_nolock(&e.hash, sizeof(e.hash), 1, f);
	_fwrite_nolock(&e.key2, sizeof(e.key2), 1, f);*/

	//ez igy undefined lenne (http://stackoverflow.com/questions/98650/what-is-the-strict-aliasing-rule)
	//inkabb pragma pack lett
	/*const int bufsiz = sizeof(e.sid) + sizeof(e.hash) + sizeof(e.key2);
	char wbuf[bufsiz];
	*((short_id*)(wbuf + 0)) = e.sid;
	*((int*)(wbuf + sizeof(e.sid))) = e.hash;
	*((int*)(wbuf + sizeof(e.sid) + sizeof(e.key2))) = e.key2;
	_fwrite_nolock(wbuf, bufsiz, 1, f);*/

	_fwrite_nolock(&e, sizeof(e), 1, f);
}

void small_bucket_writer::close(){
	fclose(f);
	delete[] buf;
}

void big_bucket_writer::close(){
	if(f){
		fclose(f);
		delete[] buf;
	}
}



small_bucket_reader::small_bucket_reader() {}

small_bucket_reader::small_bucket_reader(int key2){
	bucket_name = "buckets\\" + bucket_prefix() + "\\small_buckets\\" + tostring(key2) + ".smallbucket";
	fopen_s(&f, bucket_name.c_str(), "rb");
	exists=!!f;
	if(!exists) return;
	buf=new char[bufsize];
	setvbuf(f,buf,_IOFBF,bufsize);
}

small_bucket_elem small_bucket_reader::pop(){
	/*short_id sid;
	if(!_fread_nolock(&sid, sizeof(sid), 1, f))
		return small_bucket_elem::spec;
	int hash;
	size_t not_eof = _fread_nolock(&hash, sizeof(hash), 1, f);  assert(not_eof);*/

	small_bucket_elem e;
	if(!_fread_nolock(&e, sizeof(e), 1, f))
		return small_bucket_elem::spec;
	return e;
}

void small_bucket_reader::close(){
	fclose(f);
	remove(bucket_name.c_str());
	delete[] buf;
}


//const queue_elem queue_elem::max = queue_elem{ (char)0, 0, ::val{ -1, numeric_limits<int>::max() } }; //(see val::operator<)
const queue_elem queue_elem::max = queue_elem{ (char)0, 0, ::val{ -1, numeric_limits<int>::max() } }; //(see val::operator<)

extern bucket_writer_mgr *bwm;

bucket_reader_mgr::bucket_reader_mgr() :
	bbw_it{ bwm->buckets.begin() },
	bbr{ *bbw_it }
	{}

queue_elem bucket_reader_mgr::pop(){
	if(bbw_it.at_end())
		return queue_elem::max;

	big_bucket_elem b;
	while(1){
		b = bbr.pop();
		assert(b.hash>=0);
		if(b != big_bucket_elem::spec){
			sec_val tkey1 = bbw_it->key1;
			int tkey2 = b.key2;
			sec_val ikey1 = tkey2 % 2 == 0 ? -tkey1 : tkey1;
			int ikey2 = tkey2 / 2;
			return queue_elem{ b.sid, b.hash, val{ ikey1, ikey2 } };
		}
		if((++bbw_it).at_end())
			return queue_elem::max;
		bbr = big_bucket_reader{ *bbw_it };
	}
}



bucket_writer_mgr::bucket_writer_mgr(){
	CreateDirectory(str2wstr("buckets").c_str(), NULL);
	system(("rmdir /s /q buckets\\" + bucket_prefix() + " 2>NUL").c_str());
	CreateDirectory(str2wstr("buckets\\" + bucket_prefix()).c_str(), NULL);
	CreateDirectory(str2wstr("buckets\\" + bucket_prefix() + "\\small_buckets").c_str(), NULL);
	_setmaxstdio(2048);
}

void bucket_writer_mgr::push(queue_elem &e){
	sec_val key1 = e.val.key1;
	int key2 = e.val.key2;
	sec_val nkey1 = abs(key1);
	int nkey2 = key2 * 2 + (key1>0 ? (key2>0 ? 1 : -1) : 0);
	//buckets[e.val.key1].push(big_bucket_elem{ e.sid, e.hash, e.val.key2 });
	buckets[nkey1].push(big_bucket_elem{ e.sid, e.hash, nkey2 });
}

void bucket_writer_mgr::close(){
	for(auto &b : buckets)
		b.close();
}


big_bucket_reader::big_bucket_reader(big_bucket_writer &bbw){
	if(!bbw.f){
		empty = true;
		return;
	}

	//szetvodrozunk a kis bucketekbe
	string big_bucket_name = "buckets\\" + bucket_prefix() + "\\" + tostring(bbw.key1) + ".bigbucket";
	FILE *f;
	fopen_s(&f, big_bucket_name.c_str(), "rb");
	const int bufsize = 1024 * 1024;
	char *buf = new char[bufsize];
	setvbuf(f, buf, _IOFBF, bufsize);
	pn_vector<small_bucket_writer> sbws([](int i){return small_bucket_writer(i); });
	while(1){
		big_bucket_elem e;

		/*if(!_fread_nolock(&e.sid, sizeof(e.sid), 1, f))
			break;
		_fread_nolock(&e.hash, sizeof(e.hash), 1, f);
		_fread_nolock(&e.key2, sizeof(e.key2), 1, f);*/

		//ez igy nem jo, ld. big_bucket_writer::push
		/*const int rbufsiz = sizeof(e.sid) + sizeof(e.hash) + sizeof(e.key2);
		char rbuf[rbufsiz];
		if(!_fread_nolock(rbuf, rbufsiz, 1, f))
			break;
		e.sid = *((short_id*)(rbuf + 0));
		e.hash = *((int*)(rbuf + sizeof(e.sid)));
		e.key2 = *((int*)(rbuf + sizeof(e.sid) + sizeof(e.hash)));*/

		if(!_fread_nolock(&e, sizeof(e), 1, f))
			break;


		sbws[e.key2].push(small_bucket_elem{ e.sid, e.hash });
	}
	fclose(f);
	delete[] buf;
	remove(big_bucket_name.c_str());
	for(auto sbw : sbws)
		sbw.close();

	assert(sbws.size()); //ez azert teljesul mindig, mert ha volt nagy bucket fajl, akkor lesz kis bucket is
	key2 = sbws.begin()->key2;
	sbr = small_bucket_reader(key2);
	empty = !sbr.exists;
}

//ebben van a nem letezo small_bucket_reader_mgr funkcionalitasa
big_bucket_elem big_bucket_reader::pop(){
	if(empty)
		return big_bucket_elem::spec;

	small_bucket_elem b;
	while(1){
		b = sbr.pop();
		assert(b.hash >= 0);
		if(b != small_bucket_elem::spec)
			return big_bucket_elem{ b.sid, b.hash, key2 };
		sbr.close();
		key2++;
		sbr = small_bucket_reader{ key2 };
		if(!sbr.exists){
			empty = true;
			return big_bucket_elem::spec;
		}
	}
}

bucket_reader_mgr::~bucket_reader_mgr(){
	system(("rmdir /s /q buckets\\" + bucket_prefix() + " 2>NUL").c_str());
}