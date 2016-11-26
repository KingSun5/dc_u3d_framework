using UnityEngine;
using System.Collections;

public class SFConstants : MonoBehaviour {

	public static string[] FontSetsArray = new string[] {
		"Full Character Set",
		"Uppercase Alphabet",
		"Lowercase Alphabet",
		"Digits Only",
		"Custom Value" //<-- Custom Value needs to be the last value in this array
	};
	public static string[] fsArray = new string[] {
		"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789.,;:?!-_~#" + "\"" + "'&()[]|`" + "\\" + "/@°+=*$£€<>%",
		"ABCDEFGHIJKLMNOPQRSTUVWXYZ",
		"abcdefghijklmnopqrstuvwxyz",
		"0123456789"
	};

}
