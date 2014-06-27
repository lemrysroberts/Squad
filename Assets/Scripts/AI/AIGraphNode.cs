using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class AIGraphNode
{
	public Vector2 NodePosition 
	{ 
		get { return m_nodePosition; }
		set { m_nodePosition = value; }
	}
	public List<AIGraphNode> NodeLinks
	{
		get { return m_nodeLinks; }	
		set { m_nodeLinks = value; }
	}
	
	public int ID 
	{ 
		get { return m_ID; }
		set { m_ID = value; }
	}

	public int Annotation
	{ 
		get { return m_annotation; }
		set { m_annotation = value; }
	}

	// So this metric is just a stop-gap means of getting the RouteFinder to essentially ignore a node.
	// TODO: Replace when the new generalised graph setup is in place.
	public int Metric
	{
		get { return m_metric; }
		set { m_metric = value; }
	}
	
	// Miscellaneous reference to identify associated objects when traversing a route.
	public System.Object NodeObject
	{
		get; set;	
	}

	private int m_annotation				= 1;
	private int m_ID 						= -1;
	private int m_metric 					= 0;
	private Vector2 m_nodePosition 			= new Vector2();
	private List<AIGraphNode> m_nodeLinks 	= new List<AIGraphNode>();
}
