
using UnityEngine;
using System;
using System.Collections.Generic;
#if !UNITY_FLASH
using System.Text.RegularExpressions;
#endif

namespace MegaFiers
{
	public class MegaShapeSXL
	{
		int splineindex = 0;

		public void LoadXML(string sxldata, MegaShape shape, bool clear, int start)
		{
			MegaShapeXMLReader xml = new MegaShapeXMLReader();
			MegaShapeXMLNode node = xml.read(sxldata);

			if ( !clear )
				shape.splines.Clear();

			shape.selcurve = start;
			splineindex = start;
			ParseXML(node, shape);
		}

		public void ParseXML(MegaShapeXMLNode node, MegaShape shape)
		{
			foreach ( MegaShapeXMLNode n in node.children )
			{
				switch ( n.tagName )
				{
					case "Shape": ParseShape(n, shape); break;
				}

				ParseXML(n, shape);
			}
		}

		MegaSpline GetSpline(MegaShape shape)
		{
			MegaSpline spline;

			if ( splineindex < shape.splines.Count )
				spline = shape.splines[splineindex];
			else
			{
				spline = new MegaSpline();
				shape.splines.Add(spline);
			}

			splineindex++;
			return spline;
		}

		public void ParseShape(MegaShapeXMLNode node, MegaShape shape)
		{
			for ( int i = 0; i < node.values.Count; i++ )
			{
				MegaShapeXMLValue val = node.values[i];

				//Debug.Log("Shape val " + val.name);
				switch ( val.name )
				{
					case "name": break;
					case "p": break;
					case "r": break;
					case "s": break;
				}
			}

			foreach ( MegaShapeXMLNode n in node.children )
			{
				//Debug.Log("Shape tagName " + n.tagName);

				switch ( n.tagName )
				{
					case "Spline":
						ParseSpline(n, shape);
						break;
				}
			}
		}

		public void ParseSpline(MegaShapeXMLNode node, MegaShape shape)
		{
			MegaSpline spline = new MegaSpline();

			for ( int i = 0; i < node.values.Count; i++ )
			{
				MegaShapeXMLValue val = node.values[i];

				//Debug.Log("Spline val " + val.name);
				switch ( val.name )
				{
					case "flags": break;
					case "closed": spline.closed = int.Parse(val.value) > 0 ? true : false; break;
				}
			}

			foreach ( MegaShapeXMLNode n in node.children )
			{
				//Debug.Log("Spline tagName " + n.tagName);
				switch ( n.tagName )
				{
					case "K": ParseKnot(n, shape, spline); break;
				}
			}

			//Debug.Log("************** Add Spline");
			shape.splines.Add(spline);
		}

		public void ParseKnot(MegaShapeXMLNode node, MegaShape shape, MegaSpline spline)
		{
			Vector3 p = Vector3.zero;
			Vector3 invec = Vector3.zero;
			Vector3 outvec = Vector3.zero;

			for ( int i = 0; i < node.values.Count; i++ )
			{
				MegaShapeXMLValue val = node.values[i];

				//Debug.Log("Knot val " + val.name);
				switch ( val.name )
				{
					case "p": p = ParseV3Split(val.value, 0); break;
					case "i": invec = ParseV3Split(val.value, 0); break;
					case "o": outvec = ParseV3Split(val.value, 0); break;
					case "l": break;
				}
			}

			spline.AddKnot(p, invec, outvec);
		}

		char[] commaspace = new char[] { ',', ' ' };

		Vector3 ParseV3Split(string str, int i)
		{
			return ParseV3(str.Split(commaspace, StringSplitOptions.RemoveEmptyEntries), i);
		}

		Vector3 ParseV3(string[] str, int i)
		{
			Vector3 p = Vector3.zero;

			p.x = float.Parse(str[i]);
			p.y = float.Parse(str[i + 1]);
			p.z = float.Parse(str[i + 2]);
			return p;
		}

		public void importData(string sxldata, MegaShape shape, float scale, bool clear, int start)
		{
			LoadXML(sxldata, shape, clear, start);
			for ( int i = start; i < splineindex; i++ )
			{
				float area = shape.splines[i].Area();
				if ( area < 0.0f )
					shape.splines[i].reverse = false;
				else
					shape.splines[i].reverse = true;
			}

			//shape.Centre(0.01f, new Vector3(-1.0f, 1.0f, 1.0f));
			//shape.Centre(scale, new Vector3(1.0f, 1.0f, 1.0f), start);
			shape.CoordAdjust(scale, new Vector3(1.0f, 1.0f, 1.0f), start);
			shape.CalcLength();	//10);
		}
	}
}