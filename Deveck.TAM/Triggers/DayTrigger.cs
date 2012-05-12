/*
 * Created by SharpDevelop.
 * User: dev
 * Date: 11.05.2012
 * Time: 21:57
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Deveck.TAM.Core;
using Deveck.TAM.Utils;

namespace Deveck.TAM.Triggers
{
	/// <summary>
	/// Description of DayTrigger.
	/// </summary>
	public class DayTrigger : ITrigger
	{
		private string _name;
		private DayOfWeek _day;
		private DateTime? _fromTime;
		private DateTime? _toTime;
		
		public DayTrigger(String triggerText, String name)
		{
			_name = name;
			Tokenizer tokenizer = new Tokenizer(triggerText, ' ');
			
			_day = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), tokenizer.NextToken(), true);
			
			if(!tokenizer.TokenAvailable)
				return;
			
			String token = tokenizer.NextToken();
			
			if(token.Equals("from"))
			{
				token = tokenizer.NextToken();
				_fromTime = ParseTime(token, triggerText);
			}
			else
				throw new ArgumentException(String.Format("Token '{0}' in line '{1}'", token, triggerText));
			
			if(!tokenizer.TokenAvailable)
				return;

			token = tokenizer.NextToken();
			if(token.Equals("to"))
			{
				token = tokenizer.NextToken();
				_toTime = ParseTime(token, triggerText);
			}
			else
				throw new ArgumentException(String.Format("Token '{0}' in line '{1}'", token, triggerText));
		}
		
		public string Name
		{
			get{ return _name; }
		}
		
		public bool IsTriggered(ICall call, DateTime triggerDate)
		{
			if(!triggerDate.DayOfWeek.Equals(_day))
				return false;
			
			if(_fromTime != null)
			{
				DateTime combinedFromTime = CombineDate(triggerDate, _fromTime.Value);
				
				if(triggerDate < combinedFromTime)
					return false;
			}
			
			if(_toTime != null)
			{
				DateTime combinedToTime = CombineDate(triggerDate, _toTime.Value);
				
				if(triggerDate > combinedToTime)
					return false;
			}
			
			return true;
		}
		
				
		private DateTime ParseTime(String token, String triggerText)
		{
			String[] timeSplit = token.Split(':');
			
			if(timeSplit.Length != 2)
				throw new ArgumentException(String.Format("Illegal time specification '{0}' in line '{1}'", token, triggerText));
			
			int hour;
			int minute;
			
			if(!int.TryParse(timeSplit[0], out hour) || !int.TryParse(timeSplit[1], out minute) || hour < 0 || hour >= 24 || minute < 0 || minute >= 60)
				throw new ArgumentException(String.Format("Illegal time specification '{0}' in line'{1}'", token, triggerText));
			
			return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hour, minute, 0);
			
		}
		
		private DateTime CombineDate(DateTime date, DateTime time)
		{
			return new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second);
		}
		
		public override string ToString()
		{
			return string.Format("[DayTrigger Day={0}, FromTime={1}, ToTime={2}]", _day, _fromTime, _toTime);
		}

	}
}
