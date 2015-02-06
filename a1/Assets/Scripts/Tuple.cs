using System;
using UnityEngine;

public class Tuple<T1,T2> {

	public T1 first{get; set;}
	public T2 second{get; set;}

	public Tuple(){}

	public Tuple(T1 first, T2 second){
		this.first = first;
		this.second = second;
	}

}