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

#include "stdafx.h"

#include "common.h"
#include "hash.h"
#include "symmetries.h"
#include "debug.h"
#include "sector.h"
#include "sector_graph.h"


using namespace std;
using namespace System;
using namespace System::Collections::Generic;
using namespace System::Numerics;

namespace Wrappers {

	unordered_map<id, int> sector_sizes;
	static int f_inv_count[] { 1, 4, 30, 158, 757, 2830, 8774, 22188, 46879, 82880, 124124, 157668, 170854 };

	public value struct id{
		int W,B,WF,BF;
		id(int W, int B, int WF, int BF):W(W),B(B),WF(WF),BF(BF){}
		//id(id^ id):W(id->W),B(id->B),WF(id->WF),BF(id->BF){}
		id(::id id):W(id.W),B(id.B),WF(id.WF),BF(id.BF){}
		::id tonat(){return ::id(W,B,WF,BF);}
		void negate();
		static id operator-(id s);
		//virtual String^ ToString() override {return W.ToString()+"_"+B.ToString()+"_"+WF.ToString()+"_"+BF.ToString();}
		virtual String^ ToString() override {string str=this->tonat().to_string(); return gcnew String(str.c_str());}
		//String^ FileName(){return gcnew String(::id(W,B,WF,BF).file_name().c_str());}
		virtual int GetHashCode() override {return (W<<0)|(B<<4)|(WF<<8)|(BF<<12);}

	private:
		static BigInteger factorial(int n){
			if(n == 0)
				return 1;
			else
				return n * factorial(n - 1);
		}
		static BigInteger nCr(int n, int r) {
			return factorial(n)/factorial(r)/factorial(n - r);
		}
	public:
		int size(){
			auto tn = tonat();
			if(sector_sizes.count(tn) == 0){
				sector_sizes[tn] = (int)nCr(24 - W, B) * f_inv_count[W];
			}
			return sector_sizes[tn];
		}
	};


	public value struct eval_elem{
		enum struct cas {val, count, sym};
		cas c;
		int x;
		eval_elem(cas c,int x):c(c),x(x){}
		eval_elem(::eval_elem e):c((cas)(e.c)),x(e.x){}
	};


	value struct gui_eval_elem2;

	public ref class Sector {
	public:
		::Sector *s;
		Sector(id id) : s(new ::Sector(id.tonat())) {}

		Tuple<int, gui_eval_elem2>^ hash(board a);

		sec_val sval(){ return s->sval; }
	};


	public value struct gui_eval_elem2 : IComparable<gui_eval_elem2> {
	private:
		//azert nem lehet val, mert az nem tarolhat countot (a ctoranak az assertje szerint)
		sec_val key1;
		int key2;
		::Sector *s; //ez akkor null, ha virtualis nyeres/vesztes vagy KLE

		enum struct Cas { Val, Count };

		eval_elem2 to_eval_elem2(){
			return eval_elem2{ key1, key2 };
		}

	public:
		//A key1 nezopontja az s. Viszont ha az s null, akkor meg a virt_unique_sec_val.
		gui_eval_elem2(sec_val key1, int key2, ::Sector *s) : key1{ key1 }, key2{ key2 }, s{ s } {}
		gui_eval_elem2(::eval_elem2 e, ::Sector *s) : gui_eval_elem2{ e.key1, e.key2, s } {}


		gui_eval_elem2 undo_negate(Sector^ s){
			auto a = this->to_eval_elem2().corr((s ? s->sval() : virt_unique_sec_val()) + (this->s ? this->s->sval : virt_unique_sec_val()));
			a.key1 *= -1;
			if(s) //ha s null, akkor KLE-be negalunk
				a.key2++;
			return gui_eval_elem2(a, s ? s->s : nullptr);
		}

		static bool ignore_DD = false;

	private:
		static sec_val abs_min_value(){
			assert(::virt_loss_val != 0);
			return ::virt_loss_val - 2;
		}
		static void drop_DD(eval_elem2 &e){
			//absolute viewpoint
			assert(e.key1 >= abs_min_value());  assert(e.key1 <= ::virt_win_val);
			assert(e.key1 != ::virt_loss_val - 1); //kiszedheto
			if(e.key1 != virt_win_val && e.key1 != ::virt_loss_val && e.key1 != abs_min_value())
				e.key1 = 0;
		}
	public:

		virtual int CompareTo(gui_eval_elem2 o) {
			assert(s == o.s);
			if(!ignore_DD){
				if(key1 != o.key1)
					return key1.CompareTo(o.key1);
				else if(key1 < 0)
					return key2.CompareTo(o.key2);
				else if(key1 > 0)
					return o.key2.CompareTo(key2);
				else
					return 0;
			} else {
				auto a1 = to_eval_elem2().corr(s ? s->sval : virt_unique_sec_val());
				auto a2 = o.to_eval_elem2().corr(o.s ? o.s->sval : virt_unique_sec_val());
				drop_DD(a1);
				drop_DD(a2);
				if(a1.key1 != a2.key1)
					return a1.key1.CompareTo(a2.key1);
				else if(a1.key1 < 0)
					return a1.key2.CompareTo(a2.key2);
				else if(a1.key1 > 0)
					return a2.key2.CompareTo(a1.key2);
				else
					return 0;
			}
		}

		static bool operator<(gui_eval_elem2 a, gui_eval_elem2 b){ return a.CompareTo(b) < 0; }
		static bool operator>(gui_eval_elem2 a, gui_eval_elem2 b){ return a.CompareTo(b) > 0; }
		static bool operator==(gui_eval_elem2 a, gui_eval_elem2 b){ return a.CompareTo(b) == 0; }


		static gui_eval_elem2 min_value(Sector ^s){
			return gui_eval_elem2{ abs_min_value() - (s ? s->sval() : virt_unique_sec_val()), 0, s ? s->s : nullptr };
		}

		static gui_eval_elem2 virt_loss_val(){ //vigyazat: csak KLE-ben mukodik jol, mert ugye ahhoz, hogy jol mukodjon, valami ertelmeset kene kivonni, de mi mindig virt_unique_sec_val-t vonunk ki
			assert(::virt_loss_val);
			return gui_eval_elem2{ ::virt_loss_val - virt_unique_sec_val(), 0, nullptr };
		}

		static sec_val virt_unique_sec_val(){ //azert kell, hogy a KLE-s allasokban ne resetelodjon a tavolsag
			assert(::virt_loss_val);
#ifdef DD
			return ::virt_loss_val - 1;
#else
			return 0;
#endif
		}

		sec_val akey1(){
			return key1 + (s ? s->sval : virt_unique_sec_val());
		}

		virtual String^ ToString() override {
			assert(::virt_loss_val);  assert(::virt_win_val);
			/*if(!s)
				return String::Format("!s ({0}, {1})", key1, key2);*/
			//assert(s);
			String ^s1, ^s2;

			sec_val akey1 = this->akey1();
			//int akey2 = key2 * sign(key1) * sign(akey1);
			s1 = gcnew String(sec_val_to_sec_name(akey1).c_str());

			if(key1 == 0)
#ifdef DD
				s2 = "C"; //az akey2 itt mindig 0
#else
				s2 = "";
#endif
			else
				s2 = String::Format("{0}", key2);

#ifdef DD
			return String::Format("{0}, ({1}, {2})", s1, key1, s2);
#else
			return String::Format("{0}{1}", s1, s2);
#endif
		}

		/*bool is_ntreks(){
			return akey1() == 0;
		}
		bool is_ntreks_or_89_98(){
			sec_val akey1 = this->akey1();
			assert(sec_vals.count(::id{ 8, 9, 0, 0 }));
			assert(sec_vals.count(::id{ 9, 8, 0, 0 }));
			return akey1 == 0 || akey1 == sec_vals[::id{ 8, 9, 0, 0 }] || akey1 == sec_vals[::id{ 9, 8, 0, 0 }];
		}
		bool is_loss(){
			return akey1() == ::virt_loss_val;
		}*/
	};


	public ref class Nwu abstract sealed {
	public:
		static List<id>^ WuIds;
		static void InitWuGraph(){
			init_sector_graph();
			WuIds=gcnew List<id>();
			for(auto it=wu_ids.begin(); it!=wu_ids.end(); ++it)
				WuIds->Add(id(*it));
		}
		static List<id>^ WuGraphT(id u){
			auto r=gcnew List<id>();
			wu *w=wus[u.tonat()];
			for(auto it=w->parents.begin(); it!=w->parents.end(); ++it)
				r->Add(id((*it)->id));
			return r;
		}
		static bool Twine(id w){
			return wus[w.tonat()]->twine;
		}
	};





	public ref class Init abstract sealed {
	public:
		static void init_sym_lookuptables(){
			::init_sym_lookuptables();
		}
		static void init_sec_vals(){
			::init_sec_vals();
		}
	};

	public ref class Constants abstract sealed {
	public:
		static const int Variant=VARIANT;
		static const String^ Fname_suffix = gcnew String(FNAME_SUFFIX);
		static const String^ MovegenFname = gcnew String(movegen_file.c_str());
		enum class Variants{
			std=STANDARD,
			mora=MORABARABA,
			lask=LASKER
		};

#ifdef DD
		static const bool dd = true;
#else
		static const bool dd = false;
#endif

		static const bool FBD = FULL_BOARD_IS_DRAW;

#ifdef FULL_SECTOR_GRAPH
		static const bool Extended = true;
#else
		static const bool Extended = false;
#endif
	};

	public ref class Helpers abstract sealed {
	public:
		static String^ toclp(board a){
			return gcnew String(::toclp(a));
		}
	};
}
