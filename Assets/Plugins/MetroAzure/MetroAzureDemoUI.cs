using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Prime31.MetroAzure;
using Prime31;


public class MetroAzureDemoUI : MonoBehaviourGUI
{
	private List<TodoItem> _todoItems;
	
	void OnGUI()
	{
		beginColumn();
		
		if( GUILayout.Button( "Connect Azure Service" ) )
		{
			Azure.connect( "https://testunityapp.azure-mobile.net/", "txiXYxcLYrdEWgpEkIFImNJVgeMkkX73" );
			//Azure.connect( "YOUR_APPLICATION_URL", "YOUR_APPLICATION_KEY" );
		}
		
		
		if( GUILayout.Button( "Insert Incomplete Item" ) )
		{
			var item = new TodoItem( "I'm a todo item", false );
			Azure.insert<TodoItem>( item, () => { Debug.Log( "insert complete" ); } );
		}
		
		
		if( GUILayout.Button( "Insert Completed Item" ) )
		{
			var item = new TodoItem( "I'm a todo item", true );
			Azure.insert<TodoItem>( item, () => { Debug.Log( "insert complete" ); } );
		}
		
		
		if( GUILayout.Button( "Query All Not Completed" ) )
		{
			Azure.where<TodoItem>( i => i.complete == false, itemList =>
			{
				Debug.Log( "query completed with item count: " + itemList.Count );
				_todoItems = itemList;
				
				foreach( var item in itemList )
					Debug.Log( item );
			});
		}
		
		
		if( _todoItems != null && _todoItems.Count > 0 )
		{
			if( GUILayout.Button( "Delete Item" ) )
			{
				var item = _todoItems[0];
				_todoItems.RemoveAt( 0 );
				Azure.delete<TodoItem>( item, null );
			}
			
			
			if( GUILayout.Button( "Update Item" ) )
			{
				var item = _todoItems[0];
				item.text += System.DateTime.Now;
				Azure.update<TodoItem>( item, null );
			}
		}
		
		endColumn();
	}
}
