using UnityEngine;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;


/// <summary>
/// Simple class for testing the Azure web services
/// </summary>
public class TodoItem
{
	public int id { get; set; }
	public string text { get; set; }
	public bool complete { get; set; }
	
	
	public TodoItem()
	{}
	
	public TodoItem( string text, bool complete )
	{
		this.text = text;
		this.complete = complete;
	}
	
	
	public override string ToString()
	{
		return "[TodoItem] text: " + text + ", complete: " + complete;
	}

}
