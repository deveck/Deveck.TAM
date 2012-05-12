/*
 * Created by SharpDevelop.
 * User: dev
 * Date: 11.05.2012
 * Time: 22:03
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace Deveck.TAM.Utils
{
	/// <summary>
	/// Description of Tokenizer.
	/// </summary>
	public class Tokenizer
	{
		private String[] _tokens;
		private int _currentIndex = -1;
		
		public Tokenizer(String text, char delimiter)
		{
			_tokens = text.Split(delimiter);
		}
		
		public bool TokenAvailable
		{
			get{ return _tokens.Length > (_currentIndex +1);}
		}
		
		/// <summary>
		/// Gets the next token or throws a ArgumentException if no more tokens are available
		/// </summary>
		/// <returns></returns>
		public String NextToken()
		{
			if(!TokenAvailable)
				throw new ArgumentException("No more tokens available");
			
			_currentIndex++;
			return _tokens[_currentIndex];
		}
	}
}
